using WinPure.Configuration.DependencyInjection;

namespace WinPure.AddressVerification.DependencyInjection;

internal static partial class WinPureVerificationDependencyExtension
{
    private class UsAddressVerificationService : IUsAddressVerificationService
    {
        private readonly IWpLogger _logger;

        public UsAddressVerificationService()
        {
            var dependency = new WinPureConfigurationDependency();
            var serviceProvider = dependency.ServiceProvider;
            _logger = serviceProvider.GetService(typeof(IWpLogger)) as IWpLogger;
        }

        public UsAddressVerificationService(IWpLogger logger)
        {
            _logger = logger;
        }

        public async Task<AddressVerificationReport> VerifyAddresses(DataTable data, UsAddressVerificationSettings settings, CancellationToken cToken, Action<string, int> onProgressAction, int rowToProcess = -1)
        {
            if (!settings.Verification)
            {
                return null;
            }

            try
            {
                var verificationStatistic = new AddressVerificationStatistic { TotalRecords = data.Rows.Count, TableName = data.TableName };
                var countryCode = VerificationDictionaries.GetIso3ByCountryName(settings.Country);
                var wpKeyExists = ColumnHelper.EnsureWinPurePrimaryKeyExists(data);
                var startDate = DateTime.Now;

                var verificationDataQuery = data.AsEnumerable().AsParallel();

                if (data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_CHECK_ADDRESS_VERIFICATION))
                {
                    verificationDataQuery = verificationDataQuery.Where(x => x.Field<bool>(WinPureColumnNamesHelper.WPCOLUMN_CHECK_ADDRESS_VERIFICATION));
                }

                var verificationDataQuery2 = verificationDataQuery
                    .Select(x => new AddressVerificationResult
                    {
                        Address = settings.AddressColumns.Aggregate("", (current, addCol) => current + " " + x.Field<object>(addCol) ?? "").Trim(),
                        Locality = settings.LocalityColumns.Aggregate("", (current, addCol) => current + " " + x.Field<object>(addCol) ?? "").Trim(),
                        PostalCode = settings.PostalCodeColumns.Aggregate("", (current, addCol) => current + " " + x.Field<object>(addCol) ?? "").Trim(),
                        AdministrativeArea = settings.StateColumns.Aggregate("", (current, addCol) => current + " " + x.Field<object>(addCol) ?? "").Trim(),
                        WinpureId = x.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY)
                    });

                if (rowToProcess > 0)
                {
                    verificationDataQuery2 = verificationDataQuery2.Take(rowToProcess);
                }

                var verificationData = verificationDataQuery2.ToList();

                var chunkCount = Environment.ProcessorCount * 2;

                var rangeSize = (int)Math.Ceiling((double)verificationData.Count / chunkCount);

                if (rangeSize < 100)
                {
                    rangeSize = 100;
                }

                int processedRecords = 0;

                var onProgress = new Action(() =>
                {
                    Interlocked.Increment(ref processedRecords);

                    if (processedRecords % 10 == 0)
                    {
                        onProgressAction(Resources.CAPTION_ADDRVERIFY_PROGRESS, processedRecords * 90 / verificationData.Count);
                    }
                });

                var verificationConcurrentBag = new ConcurrentBag<AddressVerificationResult>(verificationData);

                var verificationChunksStatistic = new ConcurrentBag<VerificationStatistic>();

                var taskList = new List<Task>();

                int chunk = 0;

                while (chunk * rangeSize < verificationData.Count)
                {
                    var startIndex = chunk++ * rangeSize;
                    var endIndex = Math.Min(verificationData.Count, startIndex + rangeSize);

                    taskList.Add(Task.Factory.StartNew(() =>
                    {
                        var verificationChunkStatistic = VerifyChunk(verificationConcurrentBag, startIndex, endIndex, settings, countryCode, cToken, onProgress);

                        verificationChunksStatistic.Add(verificationChunkStatistic);

                    }));
                }

                Task.WaitAll(taskList.ToArray());

                CreateResultFields(data, settings);

                SetVerificationResult(data, verificationConcurrentBag, settings);
                if (!wpKeyExists)
                {
                    ColumnHelper.RemoveWinPurePrimaryKeyFieldFromTable(data);
                }

                return CreateVerificationStatistic(verificationStatistic, startDate, verificationChunksStatistic);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception exception)
            {
                _logger.Error("Address verification error~.", exception);
                throw new WinPureAddressVerificationException("US addresses cannot be processed.", exception);
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
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_UNMATCHED)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_UNMATCHED, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSQUALITYINDEX)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSQUALITYINDEX, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSVERIFICATIONCODE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSVERIFICATIONCODE, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_MATCHSCORE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_MATCHSCORE, typeof(string));

            if (settings.GeoTag)
            {
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LATITUDE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LATITUDE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LONGITUDE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LONGITUDE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEOACCURACY)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEOACCURACY, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE, typeof(string));
            }

            if (settings.CassField)
            {
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_AUTOZONEINDICATOR)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_AUTOZONEINDICATOR, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CARRIERROUTE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CARRIERROUTE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CMRAINDICATOR)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CMRAINDICATOR, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CONGRESSIONALDISTRICT)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CONGRESSIONALDISTRICT, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DEFAULTFLAG)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DEFAULTFLAG, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DELIVERYPOINTBARCODE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DELIVERYPOINTBARCODE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVCONFIRMEDINDICATOR)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVCONFIRMEDINDICATOR, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVFOOTNOTES)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVFOOTNOTES, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTCODE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTCODE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTNUMBER)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTNUMBER, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_EWSFLAG)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_EWSFLAG, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FALSEPOSITIVEINDICATOR)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FALSEPOSITIVEINDICATOR, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FIPSCOUNTYCODE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FIPSCOUNTYCODE, typeof(string));
               // if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FOOTNOTES)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FOOTNOTES, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKCODE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKCODE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKINDICATOR)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKINDICATOR, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSSTATUS)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSSTATUS, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_NOSTATINDICATOR)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_NOSTATINDICATOR, typeof(string));
              //  if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBNUMBER)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBNUMBER, typeof(string));
               // if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBTYPE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBTYPE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PRIMARYADDRESSLINE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PRIMARYADDRESSLINE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RECORDTYPE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RECORDTYPE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RETURNCODE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RETURNCODE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RESIDENTIALDELIVERY)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RESIDENTIALDELIVERY, typeof(string));
               // if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SECONDARYADDRESSLINE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SECONDARYADDRESSLINE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SUITELINKFOOTNOTE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SUITELINKFOOTNOTE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_VACANTINDICATOR)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_VACANTINDICATOR, typeof(string));
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

            if (settings.SerpField)
            {
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_SERPSTATUSEX)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_SERPSTATUSEX, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_QUESTIONABLE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_QUESTIONABLE, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_RESULT)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_RESULT, typeof(string));
            }
        }

        private void SetVerificationResult(DataTable data, ConcurrentBag<AddressVerificationResult> verificationConcurrentBag, UsAddressVerificationSettings settings)
        {
            foreach (var x in data.AsEnumerable().Join(verificationConcurrentBag, d => d.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY), c => c.WinpureId, (d, a) => new { data = d, addresses = a }))
            {
                #region Common

                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_ORGANIZATIONNAME] = x.addresses.OrganizationName;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS1] = x.addresses.Address1;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS2] = x.addresses.Address2;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBBUILDINGNAME] = x.addresses.SubBuildingName;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_PREMISENUMBER] = x.addresses.PremiseNumber;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_BUILDINGNAME] = x.addresses.BuildingName;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTBOX] = x.addresses.Postbox;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTTHOROUGHFARENAME] = x.addresses.DependentThoroughfareName;
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
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_UNMATCHED] = x.addresses.Unmatched;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSQUALITYINDEX] = x.addresses.AddressQualityIndex;
                x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSVERIFICATIONCODE] = x.addresses.AddressVerificationCode;
                #endregion

                #region Geotag
                if (settings.GeoTag)
                {
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LATITUDE] = x.addresses.GeoLatitude;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LONGITUDE] = x.addresses.GeoLongitude;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEOACCURACY] = x.addresses.GeoAccuracy;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE] = x.addresses.GeoDistance;
                }
                #endregion

                #region AMAS
                if (settings.AmasField)
                {
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_DPID] = x.addresses.AmasDpid;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORTYPE] = x.addresses.AmasFloorType;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORNUMBER] = x.addresses.AmasFloorNumber;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_LOTNUMBER] = x.addresses.AmasLotNumber;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUM] = x.addresses.AmasPostboxNumber;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERPREFIX] = x.addresses.AmasPostboxNumberPrefix;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERSUFFIX] = x.addresses.AmasPostboxNumberSuffix;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISE] = x.addresses.AmasPrimaryPremise;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISESUFFIX] = x.addresses.AmasPrimaryPremiseSuffix;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISE] = x.addresses.AmasSecondaryPremise;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISESUFFIX] = x.addresses.AmasSecondaryPremiseSuffix;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRESORTZONE] = x.addresses.AmasPresortZone;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRINTPOSTZONE] = x.addresses.AmasPrintPostZone;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_BARCODE] = x.addresses.AmasBarcode;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYADDRESSLINE] = x.addresses.AmasPrimaryAddressLine;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYADDRESSLINE] = x.addresses.AmasSecondaryAddressLine;
                }
                #endregion

                #region SERP
                if (settings.SerpField)
                {
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_SERPSTATUSEX] = x.addresses.SerpStatusEx;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_QUESTIONABLE] = x.addresses.SerpQuestionable;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_RESULT] = x.addresses.SerpResult;
                }
                #endregion

                #region CASS

                if (settings.CassField)
                {
                    // x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_AUTOZONEINDICATOR] = x.addresses.CassAutozoneIndicator;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CARRIERROUTE] = x.addresses.CassCarrierRoute;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CMRAINDICATOR] = x.addresses.CassCmraIndicator;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CONGRESSIONALDISTRICT] = x.addresses.CassCongressionalDistrict;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DEFAULTFLAG] = x.addresses.CassDefaultFlag;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DELIVERYPOINTBARCODE] = x.addresses.CassDeliveryPointBarCode;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVCONFIRMEDINDICATOR] = x.addresses.CassDpvConfirmedIndicator;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVFOOTNOTES] = x.addresses.CassDpvFootnotes;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTCODE] = x.addresses.CassElotCode;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTNUMBER] = x.addresses.CassElotNumber;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_EWSFLAG] = x.addresses.CassEwsFlag;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FALSEPOSITIVEINDICATOR] = x.addresses.CassFalsePositiveIndicator;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FIPSCOUNTYCODE] = x.addresses.CassFipsCountyCode;
                   // x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FOOTNOTES] = x.addresses.CassFootnotes;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKCODE] = x.addresses.CassLacsLinkCode;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKINDICATOR] = x.addresses.CassLacsLinkIndicator;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSSTATUS] = x.addresses.CassLacsStatus;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_NOSTATINDICATOR] = x.addresses.CassNostatIndicator;
                   // x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBNUMBER] = x.addresses.CassPmbNumber;
                    //x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBTYPE] = x.addresses.CassPmbType;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PRIMARYADDRESSLINE] = x.addresses.CassPrimaryAddressLine;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RECORDTYPE] = x.addresses.CassRecordType;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RETURNCODE] = x.addresses.CassReturnCode;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RESIDENTIALDELIVERY] = x.addresses.CassResidentialDelivery;
                   // x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SECONDARYADDRESSLINE] = x.addresses.CassSecondaryAddressLine;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SUITELINKFOOTNOTE] = x.addresses.CassSuiteLinkFootnote;
                    x.data[WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_VACANTINDICATOR] = x.addresses.CassVacantIndicator;

                }
                #endregion
            }
        }

        private AddressVerificationReport CreateVerificationStatistic(AddressVerificationStatistic verificationStatistic, DateTime startDate, ConcurrentBag<VerificationStatistic> verificationChunksStatistic)
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
            return result;
        }

        private VerificationStatistic VerifyChunk(ConcurrentBag<AddressVerificationResult> verificationData, int startIndex, int endIndex, UsAddressVerificationSettings settings, string countryCode, CancellationToken cToken, Action reportProgress)
        {
            var vStat = new VerificationStatistic();
            using (var srv = lqtServer.create())
            {
                srv.init(settings.PathToDb);
                var addressInputRecord = lqtInputRecord.create();
                var addressResults = lqtProcessResult.create();


                // Create the process list
                var processList = lqtProcessList.create();
                var processOptions = lqtProcessOptions.create();

                processList.add("Verify", processOptions);
                processList.add("Geocode", processOptions);

                // Open the Loqate session
                int session = srv.open();

                try
                {
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        cToken.ThrowIfCancellationRequested();
                        var addressVerificationData = verificationData.ElementAt(i);
                        try
                        {
                            addressInputRecord.set("Address1", addressVerificationData.Address);

                            if (!string.IsNullOrWhiteSpace(addressVerificationData.Locality))
                            {
                                addressInputRecord.set("Locality", addressVerificationData.Locality);
                            }

                            if (!string.IsNullOrWhiteSpace(addressVerificationData.PostalCode))
                            {
                                addressInputRecord.set("PostalCode", addressVerificationData.PostalCode);
                            }

                            if (!string.IsNullOrWhiteSpace(addressVerificationData.AdministrativeArea))
                            {
                                addressInputRecord.set("AdministrativeArea", addressVerificationData.AdministrativeArea);
                            }

                            addressInputRecord.set("Country", countryCode);

                            string verificationCode, qualityIndex;
                            // Process the record
                            srv.process(addressInputRecord, processList, addressResults);

                            if (addressResults.getCount() > 0)
                            {
                                addressVerificationData.OrganizationName = addressResults.getField(0, "OrganizationName");
                                addressVerificationData.Address1 = addressResults.getField(0, "Address1");
                                addressVerificationData.Address2 = addressResults.getField(0, "Address2");
                                addressVerificationData.SubBuildingName = addressResults.getField(0, "SubBuilding");
                                addressVerificationData.PremiseNumber = addressResults.getField(0, "Premise");
                                addressVerificationData.BuildingName = addressResults.getField(0, "BuildingName");
                                addressVerificationData.Postbox = addressResults.getField(0, "PostBoxNumber");
                                addressVerificationData.DependentThoroughfareName = addressResults.getField(0, "DependentThoroughfareName");
                                addressVerificationData.ThoroughfareName = addressResults.getField(0, "ThoroughfareName");
                                addressVerificationData.DoubleDependentLocality = addressResults.getField(0, "DoubleDependentLocality");
                                addressVerificationData.DependentLocality = addressResults.getField(0, "DependentLocality");
                                addressVerificationData.Locality = addressResults.getField(0, "Locality");
                                addressVerificationData.SubAdministrativeArea = addressResults.getField(0, "SubAdministrativeArea");
                                addressVerificationData.AdministrativeArea = addressResults.getField(0, "AdministrativeArea");
                                addressVerificationData.SuperAdministrativeArea = addressResults.getField(0, "SuperAdministrativeArea");
                                addressVerificationData.PostalCode = addressResults.getField(0, "PostalCode");
                                addressVerificationData.PostalCodePrimary = addressResults.getField(0, "PostalCodePrimary");
                                addressVerificationData.PostalCodeSecondary = addressResults.getField(0, "PostalCodeSecondary");
                                addressVerificationData.DeliveryAddress = addressResults.getField(0, "DeliveryAddress");
                                addressVerificationData.Country = addressResults.getField(0, "CountryName");
                                addressVerificationData.Unmatched = addressResults.getField(0, "Unmatched");
                                qualityIndex = addressResults.getField(0, "AQI");
                                addressVerificationData.AddressQualityIndex = qualityIndex;
                                verificationCode = addressResults.getField(0, "AVC");
                                addressVerificationData.AddressVerificationCode = verificationCode;

                                #region Geotag

                                if (settings.GeoTag)
                                {
                                    addressVerificationData.GeoLatitude = addressResults.getField(0, "Latitude");
                                    addressVerificationData.GeoLongitude = addressResults.getField(0, "Longitude");
                                    addressVerificationData.GeoAccuracy = addressResults.getField(0, "GeoAccuracy");
                                    addressVerificationData.GeoDistance = addressResults.getField(0, "GeoDistance");
                                    if (addressResults.getField(0, "GeoAccuracy") != "U0")
                                    {
                                        vStat.GeoCodeSuccess++;
                                    }

                                }

                                #endregion

                                #region AMAS

                                if (settings.AmasField)
                                {
                                    addressVerificationData.AmasDpid = addressResults.getField(0, "DPID");
                                    addressVerificationData.AmasFloorType = addressResults.getField(0, "FloorType");
                                    addressVerificationData.AmasFloorNumber = addressResults.getField(0, "FloorNumber");
                                    addressVerificationData.AmasLotNumber = addressResults.getField(0, "LotNumber");
                                    addressVerificationData.AmasPostboxNumber = addressResults.getField(0, "PostBoxNum");
                                    addressVerificationData.AmasPostboxNumberPrefix = addressResults.getField(0, "PostBoxNumberPrefix");
                                    addressVerificationData.AmasPostboxNumberSuffix = addressResults.getField(0, "PostBoxNumberSuffix");
                                    addressVerificationData.AmasPrimaryPremise = addressResults.getField(0, "PrimaryPremise");
                                    addressVerificationData.AmasPrimaryPremiseSuffix = addressResults.getField(0, "PrimaryPremiseSuffix");
                                    addressVerificationData.AmasSecondaryPremise = addressResults.getField(0, "SecondaryPremise");
                                    addressVerificationData.AmasSecondaryPremiseSuffix = addressResults.getField(0, "SecondaryPremiseSuffix");
                                    addressVerificationData.AmasPresortZone = addressResults.getField(0, "PreSortZone");
                                    addressVerificationData.AmasPrintPostZone = addressResults.getField(0, "PrintPostZone");
                                    addressVerificationData.AmasBarcode = addressResults.getField(0, "Barcode");
                                    addressVerificationData.AmasPrimaryAddressLine = addressResults.getField(0, "PrimaryAddressLine");
                                    addressVerificationData.AmasSecondaryAddressLine = addressResults.getField(0, "SecondaryAddressLine");
                                }

                                #endregion

                                #region SERP

                                if (settings.SerpField)
                                {
                                    addressVerificationData.SerpStatusEx = addressResults.getField(0, "SerpStatusEx");
                                    addressVerificationData.SerpQuestionable = addressResults.getField(0, "Questionable");
                                    addressVerificationData.SerpResult = addressResults.getField(0, "Result");
                                }

                                #endregion

                                var avc = verificationCode.Split('-');
                                if (avc.Length == 4)
                                {
                                    addressVerificationData.MatchScore = avc[3];

                                    vStat.VerificationStatus[avc[0][0].ToString()]++;
                                    vStat.PostProcessing[avc[0][1].ToString()]++;
                                    vStat.PreProcessing[avc[0][2].ToString()]++;

                                    vStat.ParsingStatus[avc[1][0].ToString()]++;
                                    vStat.LexiconIdent[avc[1][1].ToString()]++;
                                    vStat.ContextIdent[avc[1][2].ToString()]++;

                                    vStat.Postcode[avc[2]]++;
                                }


                                if (!string.IsNullOrEmpty(qualityIndex))
                                {
                                    vStat.AddressSuccess++;
                                }

                                if (settings.CassField)
                                {
                                    var cassAddressInputRecord = lqtInputRecord.create();
                                    var cassAddressResults = lqtProcessResult.create();


                                    // Create the process list
                                    var processCassList = lqtProcessList.create();
                                    var processCassOptions = lqtProcessOptions.create();

                                    processCassList.add("Verify", processCassOptions);
                                    processCassList.add("CASS2", processCassOptions);

                                    cassAddressInputRecord.set("Address1", addressVerificationData.Address1);
                                    cassAddressInputRecord.set("Country", "US");
                                    cassAddressInputRecord.set("CertifiedCountryList", "USA");
                                    cassAddressInputRecord.set("CASSavcEnable", "Yes");
                                    cassAddressInputRecord.set("Locality", addressVerificationData.Locality);
                                    cassAddressInputRecord.set("PostalCode", addressVerificationData.PostalCode);
                                    cassAddressInputRecord.set("AdministrativeArea", addressVerificationData.AdministrativeArea);

                                    srv.process(cassAddressInputRecord, processCassList, cassAddressResults);
                                    if (cassAddressResults.getCount() > 0)
                                    {
                                        // addressVerificationData.CassAutozoneIndicator = cassAddressResults.getField(1, "AutoZoneIndicator");
                                        addressVerificationData.CassCarrierRoute = cassAddressResults.getField(1, "CarrierRoute");
                                        addressVerificationData.CassCmraIndicator = cassAddressResults.getField(1, "CMRAIndicator");
                                        addressVerificationData.CassCongressionalDistrict = cassAddressResults.getField(1, "CongressionalDistrict");
                                        addressVerificationData.CassDefaultFlag = cassAddressResults.getField(1, "DefaultFlag");
                                        addressVerificationData.CassDeliveryPointBarCode = cassAddressResults.getField(1, "DeliveryPointBarCode");
                                        addressVerificationData.CassDpvConfirmedIndicator = cassAddressResults.getField(1, "DPVConfirmedIndicator");
                                        addressVerificationData.CassDpvFootnotes = cassAddressResults.getField(1, "DPVFootnotes");
                                        addressVerificationData.CassElotCode = cassAddressResults.getField(1, "eLOTCode");
                                        addressVerificationData.CassElotNumber = cassAddressResults.getField(1, "eLOTNumber");
                                        addressVerificationData.CassEwsFlag = cassAddressResults.getField(1, "EWSFlag");
                                        addressVerificationData.CassFalsePositiveIndicator = cassAddressResults.getField(1, "FalsePositiveIndicator");
                                        addressVerificationData.CassFipsCountyCode = cassAddressResults.getField(1, "FIPSCountyCode");
                                       // addressVerificationData.CassFootnotes = cassAddressResults.getField(1, "Footnotes");
                                        addressVerificationData.CassLacsLinkCode = cassAddressResults.getField(1, "LACSLinkCode");
                                        addressVerificationData.CassLacsLinkIndicator = cassAddressResults.getField(1, "LACSLinkIndicator");
                                        addressVerificationData.CassLacsStatus = cassAddressResults.getField(1, "LACSStatus");
                                        addressVerificationData.CassNostatIndicator = cassAddressResults.getField(1, "NoStatIndicator");
                                       // addressVerificationData.CassPmbNumber = cassAddressResults.getField(1, "PMBNumber");
                                        //addressVerificationData.CassPmbType = cassAddressResults.getField(1, "PMBType");
                                        addressVerificationData.CassPrimaryAddressLine = cassAddressResults.getField(1, "PrimaryAddressLine");
                                        addressVerificationData.CassRecordType = cassAddressResults.getField(1, "RecordType");
                                        addressVerificationData.CassReturnCode = cassAddressResults.getField(1, "ReturnCode");
                                        addressVerificationData.CassResidentialDelivery = cassAddressResults.getField(1, "ResidentialDelivery");
                                        addressVerificationData.CassSecondaryAddressLine = cassAddressResults.getField(1, "SecondaryAddressLine");
                                        addressVerificationData.CassSuiteLinkFootnote = cassAddressResults.getField(1, "SUITELinkFootnote");
                                        addressVerificationData.CassVacantIndicator = cassAddressResults.getField(1, "VacantIndicator");
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        reportProgress();
                    }
                }
                finally
                {
                    // Close the session
                    srv.close(session);

                    // Destroy the process list
                    lqtProcessList.destroy(processList);
                    lqtProcessOptions.destroy(processOptions);

                    // Tidy up
                    srv.shutdown();
                    lqtInputRecord.destroy(addressInputRecord);
                    lqtProcessResult.destroy(addressResults);
                    lqtServer.destroy(srv);
                }
            }

            return vStat;
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