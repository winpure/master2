using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http;
using System.Text;
using WinPure.AddressVerification.Models.Dto;
using WinPure.Configuration.DependencyInjection;

namespace WinPure.AddressVerification.DependencyInjection;

internal static partial class WinPureVerificationDependencyExtension
{
    private class UsAddressVerificationOnlineService : IUsAddressVerificationOnlineService
    {
        private readonly IWpLogger _logger;
        private int _rangeSize = 200;
        private int _maxOnlineVerificationThreads = 5;

        public UsAddressVerificationOnlineService()
        {
            var dependency = new WinPureConfigurationDependency();
            var serviceProvider = dependency.ServiceProvider;
            _logger = serviceProvider.GetService(typeof(IWpLogger)) as IWpLogger;
        }

        public UsAddressVerificationOnlineService(IWpLogger logger)
        {
            _logger = logger;
        }

        public async Task<AddressVerificationReport> VerifyAddresses(DataTable data, UsAddressVerificationSettings settings, CancellationToken cToken, Action<string, int> onProgressAction, int rowToProcess = -1)
        {
            if (!settings.Verification && (!settings.CassField || !settings.AmasField) && !settings.ReverseGeocode)
            {
                return null;
            }

            try
            {
                var countryCode = VerificationDictionaries.GetIso3ByCountryName(settings.Country);

                if (settings.CassField && countryCode.ToUpper() != "USA" && countryCode.ToUpper() != "US")
                {
                    throw new WinPureArgumentException(Resources.API_EXCEPTION_CASSONLYWITHUSA);
                }

                //if (settings.CassField && !settings.LocalityColumns.Any())
                //{
                //    throw new WinPureArgumentException(Resources.API_EXCEPTION_LOCALITYFORCASS);
                //}

                var verificationStatistic = new AddressVerificationStatistic { TotalRecords = data.Rows.Count, TableName = data.TableName };
                var wpKeyExists = ColumnHelper.EnsureWinPurePrimaryKeyExists(data);
                var startDate = DateTime.Now;
                var verificationDataQuery = data.AsEnumerable();

                if (data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_CHECK_ADDRESS_VERIFICATION))
                {
                    verificationDataQuery = verificationDataQuery.Where(x => x.Field<bool>(WinPureColumnNamesHelper.WPCOLUMN_CHECK_ADDRESS_VERIFICATION));
                }

                var verificationDataQueryComplete = verificationDataQuery
                    .AsParallel()
                    .Select(x => new MatchAddressDto
                    {
                        Address1 = settings.AddressColumns.Aggregate("", (current, addCol) => current + " " + x.Field<object>(addCol) ?? "").Trim(),
                        Locality = settings.LocalityColumns.Aggregate("", (current, addCol) => current + " " + x.Field<object>(addCol) ?? "").Trim(),
                        PostalCode = settings.PostalCodeColumns.Aggregate("", (current, addCol) => current + " " + x.Field<object>(addCol) ?? "").Trim(),
                        AdministrativeArea = settings.StateColumns.Aggregate("", (current, addCol) => current + " " + x.Field<object>(addCol) ?? "").Trim(),
                        Latitude = string.IsNullOrWhiteSpace(settings.LatitudeColumn) ? (decimal?)null : Convert.ToDecimal(x.Field<object>(settings.LatitudeColumn).ToString(), CultureInfo.InvariantCulture),
                        Longitude = string.IsNullOrWhiteSpace(settings.LongitudeColumn) ? (decimal?)null : Convert.ToDecimal(x.Field<object>(settings.LongitudeColumn).ToString(), CultureInfo.InvariantCulture),
                        WinpureId = x.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY),
                        Country = countryCode
                    });

                if (rowToProcess > 0)
                {
                    verificationDataQueryComplete = verificationDataQueryComplete.Take(rowToProcess);
                }

                var verificationData = verificationDataQueryComplete.ToList();


                //if (GetRequiredCredits(settings, verificationData.Count) > settings.AvailableCredits)
                //{
                //    throw new WinPureLimitException(string.Format(Resources.API_EXCEPTION_NOTENOUGHCREDITS, verificationData.Count));
                //}

                var verificationConcurrentBag = new ConcurrentBag<MatchAddressDto>(verificationData);

                var verificationChunksStatistic = new ConcurrentBag<VerificationStatistic>();

                int chunk = 0;
                int totalChunksCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(verificationData.Count) / _rangeSize));

                int processedChunks = 0;

                var onProgress = new Action(() =>
                {
                    Interlocked.Increment(ref processedChunks);

                    onProgressAction(Resources.CAPTION_ADDRVERIFY_PROGRESS, processedChunks * _rangeSize * 90 / verificationData.Count);
                });

                var bufferBlock = new BufferBlock<Tuple<ConcurrentBag<MatchAddressDto>, int, UsAddressVerificationSettings, string, CancellationToken, Action, int>>(new DataflowBlockOptions { EnsureOrdered = false });

                var dataFlowActionOptions = new ExecutionDataflowBlockOptions { MaxMessagesPerTask = 1, EnsureOrdered = false, MaxDegreeOfParallelism = _maxOnlineVerificationThreads };
                var verificationActionBlock = new ActionBlock<Tuple<ConcurrentBag<MatchAddressDto>, int, UsAddressVerificationSettings, string, CancellationToken, Action, int>>(
                    async tup =>
                    {
                        var verificationChunkStatistic = await VerifyChunk(tup.Item1, tup.Item2, tup.Item3, tup.Item4, tup.Item5, tup.Item6);

                        verificationChunksStatistic.Add(verificationChunkStatistic);

                    }, dataFlowActionOptions);

                bufferBlock.LinkTo(verificationActionBlock, new DataflowLinkOptions { PropagateCompletion = true });

                while (chunk * _rangeSize < verificationData.Count)
                {
                    var currentChunk = chunk++;

                    await bufferBlock.SendAsync(new Tuple<ConcurrentBag<MatchAddressDto>, int, UsAddressVerificationSettings, string, CancellationToken, Action, int>(verificationConcurrentBag, currentChunk, settings, countryCode, cToken, onProgress, totalChunksCount), cToken);
                }
                bufferBlock.Complete();
                Task.WaitAll(bufferBlock.Completion, verificationActionBlock.Completion);

                CreateResultFields(data, settings);

                SetVerificationResult(data, verificationConcurrentBag, settings);
                if (!wpKeyExists)
                {
                    ColumnHelper.RemoveWinPurePrimaryKeyFieldFromTable(data);
                }
                return CreateVerificationStatistic(verificationStatistic, startDate, verificationChunksStatistic, settings, verificationConcurrentBag.Count);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception exception)
            {
                _logger.Error("Verification online error.", exception);
                throw new WinPureAddressVerificationException("US addresses cannot be processed online.", exception);
            }
        }

        private void CreateResultFields(DataTable data, UsAddressVerificationSettings settings)
        {
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ORGANIZATIONNAME)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ORGANIZATIONNAME, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS1)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS1, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS2)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS2, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBBUILDINGNAME)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBBUILDINGNAME, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_PREMISENUMBER)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_PREMISENUMBER, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_BUILDINGNAME)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_BUILDINGNAME, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTBOX)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTBOX, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTTHOROUGHFARENAME)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTTHOROUGHFARENAME, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_THOROUGHFARENAME)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_THOROUGHFARENAME, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DOUBLEDEPENDENTLOCALITY)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DOUBLEDEPENDENTLOCALITY, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTLOCALITY)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTLOCALITY, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_LOCALITY)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_LOCALITY, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBADMINISTRATIVEAREA)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBADMINISTRATIVEAREA, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADMINISTRATIVEAREA)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADMINISTRATIVEAREA, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUPERADMINISTRATIVEAREA)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUPERADMINISTRATIVEAREA, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODE, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODEPRIMARY)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODEPRIMARY, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODESECONDARY)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODESECONDARY, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DELIVERYADDRESS)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DELIVERYADDRESS, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_COUNTRY)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_COUNTRY, typeof(string));
            //if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_UNMATCHED)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_UNMATCHED, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSQUALITYINDEX)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSQUALITYINDEX, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSVERIFICATIONCODE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSVERIFICATIONCODE, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_MATCHSCORE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_MATCHSCORE, typeof(string));

            if (settings.GeoTag || settings.ReverseGeocode)
            {
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LATITUDE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LATITUDE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LONGITUDE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LONGITUDE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEOACCURACY)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEOACCURACY, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE, typeof(string));
            }

            if (settings.AmasField)
            {
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_DPID)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_DPID, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORTYPE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORTYPE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORNUMBER)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORNUMBER, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_LOTNUMBER)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_LOTNUMBER, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUM)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUM, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERPREFIX)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERPREFIX, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERSUFFIX)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERSUFFIX, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISESUFFIX)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISESUFFIX, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISESUFFIX)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISESUFFIX, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRESORTZONE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRESORTZONE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRINTPOSTZONE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRINTPOSTZONE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_BARCODE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_BARCODE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYADDRESSLINE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYADDRESSLINE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYADDRESSLINE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYADDRESSLINE, typeof(string));
            }

            //if (settings.SerpField)
            //{
            //    if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_SERPSTATUSEX)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_SERPSTATUSEX, typeof(string));
            //    if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_QUESTIONABLE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_QUESTIONABLE, typeof(string));
            //    if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_RESULT)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_RESULT, typeof(string));
            //}


            if (settings.CassField)
            {
               //  if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_AUTOZONEINDICATOR)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_AUTOZONEINDICATOR, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CARRIERROUTE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CARRIERROUTE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CMRAINDICATOR)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CMRAINDICATOR, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CONGRESSIONALDISTRICT)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CONGRESSIONALDISTRICT, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DEFAULTFLAG)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DEFAULTFLAG, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DELIVERYPOINTBARCODE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DELIVERYPOINTBARCODE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVCONFIRMEDINDICATOR)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVCONFIRMEDINDICATOR, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVFOOTNOTES)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVFOOTNOTES, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTCODE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTCODE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTNUMBER)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTNUMBER, typeof(string));
                //if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_EWSFLAG)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_EWSFLAG, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FALSEPOSITIVEINDICATOR)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FALSEPOSITIVEINDICATOR, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FIPSCOUNTYCODE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FIPSCOUNTYCODE, typeof(string));
               // if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FOOTNOTES)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FOOTNOTES, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKCODE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKCODE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKINDICATOR)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKINDICATOR, typeof(string));
                //if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSSTATUS)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSSTATUS, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_NOSTATINDICATOR)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_NOSTATINDICATOR, typeof(string));
               // if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBNUMBER)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBNUMBER, typeof(string));
                //if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBTYPE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBTYPE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PRIMARYADDRESSLINE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PRIMARYADDRESSLINE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RECORDTYPE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RECORDTYPE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RETURNCODE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RETURNCODE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RESIDENTIALDELIVERY)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RESIDENTIALDELIVERY, typeof(string));
               // if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SECONDARYADDRESSLINE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SECONDARYADDRESSLINE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SUITELINKFOOTNOTE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SUITELINKFOOTNOTE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_VACANTINDICATOR)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_VACANTINDICATOR, typeof(string));
            }
        }

        private void SetVerificationResult(DataTable data, ConcurrentBag<MatchAddressDto> verificationConcurrentBag, UsAddressVerificationSettings settings)
        {
            foreach (var x in data.AsEnumerable().Join(verificationConcurrentBag, d => d.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY), c => c.WinpureId, (d, a) => new { data = d, addresses = a }))
            {
                #region Common
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_ORGANIZATIONNAME] = x.addresses.Organization;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS1] = x.addresses.Address1;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS2] = x.addresses.Address2;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBBUILDINGNAME] = x.addresses.SubBuilding;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_PREMISENUMBER] = x.addresses.PremiseNumber;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_BUILDINGNAME] = x.addresses.Building;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTBOX] = x.addresses.PostBox;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTTHOROUGHFARENAME] = x.addresses.DependentThoroughfare;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_THOROUGHFARENAME] = x.addresses.ThoroughfareName;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_DOUBLEDEPENDENTLOCALITY] = x.addresses.DoubleDependentLocality;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTLOCALITY] = x.addresses.DependentLocality;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_LOCALITY] = x.addresses.Locality;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBADMINISTRATIVEAREA] = x.addresses.SubAdministrativeArea;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADMINISTRATIVEAREA] = x.addresses.AdministrativeArea;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUPERADMINISTRATIVEAREA] = x.addresses.SuperAdministrativeArea;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODE] = x.addresses.PostalCode;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODEPRIMARY] = x.addresses.PostalCodePrimary;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODESECONDARY] = x.addresses.PostalCodeSecondary;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_DELIVERYADDRESS] = x.addresses.DeliveryAddress;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_COUNTRY] = x.addresses.Country;
                //x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_UNMATCHED] = x.addresses.Unmatched;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSQUALITYINDEX] = x.addresses.AQI;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSVERIFICATIONCODE] = x.addresses.AVC;
                #endregion

                #region Geotag
                if (settings.GeoTag || settings.ReverseGeocode)
                {
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LATITUDE] = x.addresses.Latitude;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LONGITUDE] = x.addresses.Longitude;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEOACCURACY] = x.addresses.GeoAccuracy;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE] = x.addresses.GeoDistance;
                }
                #endregion

                //#region SERP
                //if (settings.SerpField)
                //{
                //    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_SERPSTATUSEX] = x.addresses.SerpStatusEx;
                //    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_QUESTIONABLE] = x.addresses.SerpQuestionable;
                //    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_RESULT] = x.addresses.SerpResult;
                //}
                //#endregion

                #region AMAS
                if (settings.AmasField)
                {
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_DPID] = x.addresses.DPID;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORTYPE] = x.addresses.FloorType;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORNUMBER] = x.addresses.FloorNumber;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_LOTNUMBER] = x.addresses.LotNumber;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUM] = x.addresses.PostBoxNumber;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERPREFIX] = x.addresses.PostBoxNumberPrefix;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERSUFFIX] = x.addresses.PostBoxNumberSuffix;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISE] = x.addresses.PrimaryPremise;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISESUFFIX] = x.addresses.PrimaryPremiseSuffix;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISE] = x.addresses.SecondaryPremise;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISESUFFIX] = x.addresses.SecondaryPremiseSuffix;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRESORTZONE] = x.addresses.PreSortZone;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRINTPOSTZONE] = x.addresses.PrintPostZone;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_BARCODE] = x.addresses.Barcode;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYADDRESSLINE] = x.addresses.PrimaryAddressLine;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYADDRESSLINE] = x.addresses.SecondaryAddressLine;
                }
                #endregion

                #region CASS
                if (settings.CassField)
                {
                   //  x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_AUTOZONEINDICATOR] = x.addresses.AutoZoneIndicator;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CARRIERROUTE] = x.addresses.CarrierRoute;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CMRAINDICATOR] = x.addresses.CMRAIndicator;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CONGRESSIONALDISTRICT] = x.addresses.CongressionalDistrict;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DEFAULTFLAG] = x.addresses.DefaultFlag;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DELIVERYPOINTBARCODE] = x.addresses.DeliveryPointBarCode;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVCONFIRMEDINDICATOR] = x.addresses.DPVConfirmedIndicator;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVFOOTNOTES] = x.addresses.DPVFootnotes;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTCODE] = x.addresses.eLOTCode;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTNUMBER] = x.addresses.eLOTNumber;
                    //x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_EWSFLAG] = x.addresses.EwsFlag;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FALSEPOSITIVEINDICATOR] = x.addresses.FalsePositiveIndicator;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FIPSCOUNTYCODE] = x.addresses.FIPSCountyCode;
                    //x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FOOTNOTES] = x.addresses.Footnotes;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKCODE] = x.addresses.LACSLinkCode;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKINDICATOR] = x.addresses.LACSLinkIndicator;
                    //x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSSTATUS] = x.addresses.LacsStatus;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_NOSTATINDICATOR] = x.addresses.NoStatIndicator;
                   // x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBNUMBER] = x.addresses.PMBNumber;
                    //x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBTYPE] = x.addresses.PMBType;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PRIMARYADDRESSLINE] = x.addresses.PrimaryAddressLine;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RECORDTYPE] = x.addresses.RecordType;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RETURNCODE] = x.addresses.ReturnCode;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RESIDENTIALDELIVERY] = x.addresses.ResidentialDelivery;
                   // x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SECONDARYADDRESSLINE] = x.addresses.SecondaryAddressLine;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SUITELINKFOOTNOTE] = x.addresses.SUITELinkFootnote;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_VACANTINDICATOR] = x.addresses.VacantIndicator;
                }
                #endregion
            }
        }

        private int GetRequiredCredits(UsAddressVerificationSettings settings, int addressRecordsCount)
        {
            return (settings.Verification && settings.CassField) ? 2 * addressRecordsCount : addressRecordsCount;
        }

        private AddressVerificationReport CreateVerificationStatistic(AddressVerificationStatistic verificationStatistic, DateTime startDate, ConcurrentBag<VerificationStatistic> verificationChunksStatistic, UsAddressVerificationSettings settings, int addressRecordsCount)
        {
            verificationStatistic.AddressSuccess = verificationChunksStatistic.Sum(x => x.AddressSuccess);
            verificationStatistic.GeoCodeSuccess = verificationChunksStatistic.Sum(x => x.GeoCodeSuccess);
            if (verificationStatistic.TotalRecords > 0)
            {
                verificationStatistic.AddressSuccessPercent = Math.Round(verificationStatistic.AddressSuccess * 1.0 / verificationStatistic.TotalRecords, 2, MidpointRounding.AwayFromZero);
                verificationStatistic.GeoCodeSuccessPercent = Math.Round(verificationStatistic.GeoCodeSuccess * 1.0 / verificationStatistic.TotalRecords, 2, MidpointRounding.AwayFromZero);
            }

            var result = new AddressVerificationReport();
            foreach (var chunkStatistic in verificationChunksStatistic)
            {
                ProcessChunkStatistic(result, chunkStatistic.VerificationStatus, VerificationDictionaries.VerificationStatus);
                ProcessChunkStatistic(result, chunkStatistic.PostProcessing, VerificationDictionaries.PostProcessed);
                ProcessChunkStatistic(result, chunkStatistic.PreProcessing, VerificationDictionaries.PreProcessed);
                ProcessChunkStatistic(result, chunkStatistic.ParsingStatus, VerificationDictionaries.ParsingStatus);
                ProcessChunkStatistic(result, chunkStatistic.LexiconIdent, VerificationDictionaries.Lexicon);
                ProcessChunkStatistic(result, chunkStatistic.ContextIdent, VerificationDictionaries.Context);
                ProcessChunkStatistic(result, chunkStatistic.Postcode, VerificationDictionaries.PostcodeStatus);
            }

            result.CommonData = verificationStatistic;
            result.VerifyTime = (DateTime.Now - startDate);
            result.UsedCredits = GetRequiredCredits(settings, addressRecordsCount);
            return result;
        }

        private async Task<List<VerificationResultDto>> VerifyAddresses(AddressRequestDto addressRequest, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(addressRequest, Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }
                );
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(ApiHelper.LoqateDataCleansingApi, requestContent, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var responseText = await response.Content.ReadAsStringAsync();
            if (responseText.Contains("Cause") && responseText.Contains("Resolution"))
            {
                var error = JsonConvert.DeserializeObject<VerificationErrorDto>(responseText);
                throw new WinPureAddressVerificationException($"Error: {error.Description}; Resolution: {error.Resolution}", null);
            }
            var result = JsonConvert.DeserializeObject<List<VerificationResultDto>>(responseText);
            return result;
        }

        private async Task<VerificationStatistic> VerifyChunk(ConcurrentBag<MatchAddressDto> verificationData, int chunkNumber, UsAddressVerificationSettings settings, string countryCode, CancellationToken cToken, Action reportProgress)
        {
            var vStat = new VerificationStatistic();
            cToken.ThrowIfCancellationRequested();


            // Process the record
            var addressesForVerification = verificationData.Skip(chunkNumber * _rangeSize).Take(_rangeSize).Select(x => (AddressVerificationData)x).ToList();

            if (settings.ReverseGeocode)
            {
                await VerifyReverseGeocodeChunk(verificationData, addressesForVerification, settings, countryCode, vStat, cToken);
            }
            else
            {
                if (settings.Verification || settings.CassField || settings.AmasField)
                {
                    await VerifyAddressChunk(verificationData, addressesForVerification, settings, countryCode, vStat, cToken);
                }

                if (settings.CassField)
                {
                    await CertifyAddressChunk(verificationData, addressesForVerification, settings, countryCode, vStat, cToken);
                }
            }

            reportProgress();

            return vStat;
        }

        private async Task VerifyAddressChunk(ConcurrentBag<MatchAddressDto> verificationData, List<AddressVerificationData> addressesForVerification, UsAddressVerificationSettings settings, string countryCode, VerificationStatistic vStat, CancellationToken cToken)
        {
            var verificationRequest = new AddressRequestDto
            {
                Key = settings.LicenseKey,
                Geocode = settings.GeoTag,
                Addresses = addressesForVerification
            };

            if (settings.GeoTag)
            {
                verificationRequest.Geocode = true;
                verificationRequest.Options = new OptionsDto { Process = "Verify" };
            }

            var verifiedResult = await VerifyAddresses(verificationRequest, cToken);

            foreach (var processResult in verificationData.Join(verifiedResult, d => d.WinpureId, r => r.Input.WinpureId, (d, r) => new { data = d, result = r.Matches.FirstOrDefault() }))
            {
                if (processResult.result == null)
                {
                    continue;
                }

                SetVerificationFields(processResult.data, processResult.result);

                #region Geotag

                if (settings.GeoTag)
                {
                    processResult.data.Latitude = processResult.result.Latitude;
                    processResult.data.Longitude = processResult.result.Longitude;
                    processResult.data.GeoAccuracy = processResult.result.GeoAccuracy;
                    processResult.data.GeoDistance = processResult.result.GeoDistance;
                    if (processResult.data.GeoAccuracy != "U0")
                    {
                        vStat.GeoCodeSuccess++;
                    }
                }

                #endregion
                ProcessRowStatistic(vStat, processResult.data);
            }
        }

        private async Task VerifyReverseGeocodeChunk(ConcurrentBag<MatchAddressDto> verificationData, List<AddressVerificationData> addressesForVerification, UsAddressVerificationSettings settings, string countryCode, VerificationStatistic vStat, CancellationToken cToken)
        {
            var verificationRequest = new AddressRequestDto
            {
                Key = settings.LicenseKey,
                Geocode = false,
                Options = new OptionsDto
                {
                    Process = "ReverseGeocode",
                    ServerOptions = new ServerOptionsDto
                    {
                        MaxResults = 1
                    }
                },
                Addresses = addressesForVerification
            };

            var verifiedResult = await VerifyAddresses(verificationRequest, cToken);

            foreach (var processResult in verificationData.Join(verifiedResult, d => d.WinpureId, r => r.Input.WinpureId, (d, r) => new { data = d, result = r.Matches.FirstOrDefault() }))
            {
                if (processResult.result == null)
                {
                    continue;
                }

                SetVerificationFields(processResult.data, processResult.result);

                #region Geotag

                processResult.data.Latitude = processResult.result.Latitude;
                processResult.data.Longitude = processResult.result.Longitude;
                processResult.data.GeoAccuracy = processResult.result.GeoAccuracy;
                processResult.data.GeoDistance = processResult.result.GeoDistance;
                if (processResult.data.GeoAccuracy != "U0")
                {
                    vStat.GeoCodeSuccess++;
                }

                #endregion

                ProcessRowStatistic(vStat, processResult.data);
            }
        }

        private async Task CertifyAddressChunk(ConcurrentBag<MatchAddressDto> verificationData, List<AddressVerificationData> addressesForVerification, UsAddressVerificationSettings settings, string countryCode, VerificationStatistic vStat, CancellationToken cToken)
        {
            if (addressesForVerification.Any())
            {
                var certifyRequest = new AddressRequestDto
                {
                    Key = settings.LicenseKey,
                    Options = new OptionsDto { Certify = true },
                    Addresses = addressesForVerification
                };

                var verifiedResult = await VerifyAddresses(certifyRequest, cToken);

                foreach (var processResult in verificationData.Join(verifiedResult, d => d.WinpureId, r => r.Input.WinpureId, (d, r) => new { data = d, result = r.Matches.FirstOrDefault() }))
                {
                    if (processResult.result == null)
                    {
                        continue;
                    }

                    SetCertifiedFields(processResult.data, processResult.result);
                }
            }
        }

        private void SetVerificationFields(MatchAddressDto data, MatchAddressDto result)
        {
            data.Organization = result.Organization;
            data.Address1 = result.Address1;
            data.Address2 = result.Address2;
            data.SubBuilding = result.SubBuilding;
            data.PremiseNumber = result.Premise;
            data.Building = result.Building;
            data.PostBox = result.PostBoxNumber;
            data.DependentThoroughfare = result.DependentThoroughfare;
            data.ThoroughfareName = result.ThoroughfareName;
            data.DoubleDependentLocality = result.DoubleDependentLocality;
            data.DependentLocality = result.DependentLocality;
            data.Locality = result.Locality;
            data.SubAdministrativeArea = result.SubAdministrativeArea;
            data.AdministrativeArea = result.AdministrativeArea;
            data.SuperAdministrativeArea = result.SuperAdministrativeArea;
            data.PostalCode = result.PostalCode;
            data.PostalCodePrimary = result.PostalCodePrimary;
            data.PostalCodeSecondary = result.PostalCodeSecondary;
            data.DeliveryAddress = result.DeliveryAddress;
            data.Country = result.CountryName;
            //data.Unmatched =  result.Unmatched;
            data.AQI = result.AQI;
            data.AVC = result.AVC;
        }

        private void SetCertifiedFields(MatchAddressDto data, MatchAddressDto result)
        {
           // data.AutoZoneIndicator = result.AutoZoneIndicator;
            data.CarrierRoute = result.CarrierRoute;
            data.CMRAIndicator = result.CMRAIndicator;
            data.CongressionalDistrict = result.CongressionalDistrict;
            data.DefaultFlag = result.DefaultFlag;
            data.DeliveryPointBarCode = result.DeliveryPointBarCode;
            data.DPVConfirmedIndicator = result.DPVConfirmedIndicator;
            data.DPVFootnotes = result.DPVFootnotes;
            data.eLOTCode = result.eLOTCode;
            data.eLOTNumber = result.eLOTNumber;
            //data.EwsFlag = result.EWSFlag;
            data.FalsePositiveIndicator = result.FalsePositiveIndicator;
            data.FIPSCountyCode = result.FIPSCountyCode;
           //data.Footnotes = result.Footnotes;
            data.LACSLinkCode = result.LACSLinkCode;
            data.LACSLinkIndicator = result.LACSLinkIndicator;
            //data.LacsStatus = result.LACSStatus;
            data.NoStatIndicator = result.NoStatIndicator;
            //data.PMBNumber = result.PMBNumber;
            //data.PMBType = result.PMBType;
            data.PrimaryAddressLine = result.PrimaryAddressLine;
            data.RecordType = result.RecordType;
            data.ReturnCode = result.ReturnCode;
            data.ResidentialDelivery = result.ResidentialDelivery;
            data.SecondaryAddressLine = result.SecondaryAddressLine;
            data.SUITELinkFootnote = result.SUITELinkFootnote;
            data.VacantIndicator = result.VacantIndicator;
            data.PostBoxType = result.PostBoxType;

            data.PostBoxNum = result.PostBoxNum;
            data.PrimaryPremise = result.PrimaryPremise;
            data.Barcode = result.Barcode;
            data.DPID = result.DPID;
            data.ErrorCode = result.ErrorCode;
            data.FloorNumber = result.FloorNumber;
            data.FloorType = result.FloorType;
            data.LotNumber = result.LotNumber;
            data.PreSortZone = result.PreSortZone;
            data.SecondaryPremise = result.SecondaryPremise;
            data.SecondaryPremiseSuffix = result.SecondaryPremiseSuffix;
            data.PostBoxNumberPrefix = result.PostBoxNumberPrefix;
            data.PostBoxNumberSuffix = result.PostBoxNumberSuffix;
            data.PrimaryPremiseSuffix = result.PrimaryPremiseSuffix;
            data.PrintPostZone = result.PrintPostZone;
        }

        private void ProcessRowStatistic(VerificationStatistic vStat, MatchAddressDto data)
        {
            var avc = data.AVC.Split('-');
            if (avc.Length == 4)
            {
                data.MatchScore = avc[3];

                vStat.VerificationStatus[avc[0][0].ToString()]++;
                vStat.PostProcessing[avc[0][1].ToString()]++;
                vStat.PreProcessing[avc[0][2].ToString()]++;

                vStat.ParsingStatus[avc[1][0].ToString()]++;
                vStat.LexiconIdent[avc[1][1].ToString()]++;
                vStat.ContextIdent[avc[1][2].ToString()]++;

                vStat.Postcode[avc[2]]++;
            }


            if (!string.IsNullOrEmpty(data.AQI))
            {
                vStat.AddressSuccess++;
            }
        }

        private void ProcessChunkStatistic(AddressVerificationReport report, Dictionary<string, int> stat, Dictionary<string, string> description)
        {
            foreach (var statValue in stat.Where(x => x.Value != 0))
            {
                var correctionCode = report.Correction.FirstOrDefault(x => x.Code == statValue.Key);
                if (correctionCode == null)
                {
                    report.Correction.Add(new AddressCorrectionCode
                    {
                        Count = statValue.Value,
                        Code = statValue.Key,
                        Description = description[statValue.Key]
                    });
                }
                else
                {
                    correctionCode.Count += statValue.Value;
                }
            }
        }
    }
}