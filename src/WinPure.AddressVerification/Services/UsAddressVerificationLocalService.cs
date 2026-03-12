using System.Globalization;

namespace WinPure.AddressVerification.DependencyInjection;

internal static partial class WinPureVerificationDependencyExtension
{
    private class UsAddressVerificationLocalService : IUsAddressVerificationLocalService
    {
        private readonly IWpLogger _logger;
        private static int CHUNK_SIZE = 200;

        public UsAddressVerificationLocalService(IWpLogger logger)
        {
            _logger = logger;
        }

        public AddressVerificationReport VerifyAddresses(DataTable data, UsAddressVerificationSettings settings, CancellationToken cToken, Action<string, int> onProgressAction, int rowToProcess = -1)
        {
            if (!settings.Verification)
            {
                return null;
            }

            var vResult = new AddressVerificationStatistic { TableName = data.TableName };
            var countryCode = VerificationDictionaries.GetIso3ByCountryName(settings.Country);

            var wpIdsForVerification = settings.SelectedRows.Any()
                ? settings.SelectedRows
                : data.AsEnumerable().Select(x => x.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY)).ToList();

            vResult.TotalRecords = data.Rows.Count;

            int processedRecords = 0;

            var verificationRowCount = wpIdsForVerification.Count;
            var onProgress = new Action(() =>
            {
                Interlocked.Increment(ref processedRecords);

                if (processedRecords % 10 == 0)
                {
                    onProgressAction(Resources.CAPTION_ADDRVERIFY_PROGRESS, processedRecords * 90 / verificationRowCount);
                }
            });

            var verificationChunksStatistic = new ConcurrentBag<VerificationStatistic>();
            var verificationResult = new ConcurrentBag<AddressVerificationResult>();
            var startDate = DateTime.Now;

            var degreeOfParallelism = Environment.ProcessorCount;// * 2
            var dataFlowSelectOptions = new ExecutionDataflowBlockOptions { MaxMessagesPerTask = 1, EnsureOrdered = false, MaxDegreeOfParallelism = degreeOfParallelism, CancellationToken = cToken };
            var dataFlowVerifyOptions = new ExecutionDataflowBlockOptions { MaxMessagesPerTask = 1, EnsureOrdered = false, MaxDegreeOfParallelism = degreeOfParallelism, CancellationToken = cToken };
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            var prepareData = new TransformBlock<WinPureDataFlowLocalBlockParameter, WinPureDataFlowLocalBlockParameter>(
                parameter =>
                {
                    parameter.VerificationData = SelectChunkDataForVerification(parameter);
                    return parameter;
                }, dataFlowSelectOptions);

            ActionBlock<WinPureDataFlowLocalBlockParameter> processData;

            if (settings.ReverseGeocode)
            {
                processData = new ActionBlock<WinPureDataFlowLocalBlockParameter>(
                    parameter =>
                    {
                        var statistic = ReverseGeocodeChunk(parameter);
                        if (statistic != null)
                        {
                            parameter.Statistic.Add(statistic);
                        }
                    }, dataFlowVerifyOptions);
            }
            else
            {
                processData = new ActionBlock<WinPureDataFlowLocalBlockParameter>(
                    parameter =>
                    {
                        var statistic = AddressVerificationChunk(parameter);
                        if (statistic != null)
                        {
                            parameter.Statistic.Add(statistic);
                        }
                    }, dataFlowVerifyOptions);
            }

            prepareData.LinkTo(processData, linkOptions);
            while (wpIdsForVerification.Any())
            {
                var idToProcess = wpIdsForVerification.Take(CHUNK_SIZE).ToList();
                var parameter = new WinPureDataFlowLocalBlockParameter
                {
                    Data = data,
                    CountryCode = countryCode,
                    CToken = cToken,
                    ReportProgress = onProgress,
                    Settings = settings,
                    Statistic = verificationChunksStatistic,
                    VerificationResult = verificationResult,
                    WinPureIds = idToProcess,
                };
                prepareData.Post(parameter);
                wpIdsForVerification = wpIdsForVerification.Except(idToProcess).ToList();
            }

            prepareData.Complete();
            Task.WaitAll(prepareData.Completion, processData.Completion);

            vResult.AddressSuccess = verificationChunksStatistic.Sum(x => x.AddressSuccess);
            vResult.GeoCodeSuccess = verificationChunksStatistic.Sum(x => x.GeoCodeSuccess);
            if (vResult.TotalRecords > 0)
            {
                vResult.AddressSuccessPercent = Math.Round(vResult.AddressSuccess * 1.0 / vResult.TotalRecords, 2, MidpointRounding.AwayFromZero);
                vResult.GeoCodeSuccessPercent = Math.Round(vResult.GeoCodeSuccess * 1.0 / vResult.TotalRecords, 2, MidpointRounding.AwayFromZero);
            }

            var result = new AddressVerificationReport();
            foreach (var chunkStatistic in verificationChunksStatistic)
            {
                ProcessStatistic(result, chunkStatistic.VerificationStatus, VerificationDictionaries.VerificationStatus);
                ProcessStatistic(result, chunkStatistic.PostProcessing, VerificationDictionaries.PostProcessed);
                ProcessStatistic(result, chunkStatistic.PreProcessing, VerificationDictionaries.PreProcessed);
                ProcessStatistic(result, chunkStatistic.ParsingStatus, VerificationDictionaries.ParsingStatus);
                ProcessStatistic(result, chunkStatistic.LexiconIdent, VerificationDictionaries.Lexicon);
                ProcessStatistic(result, chunkStatistic.ContextIdent, VerificationDictionaries.Context);
                ProcessStatistic(result, chunkStatistic.Postcode, VerificationDictionaries.PostcodeStatus);
            }

            CreateColumns(data, settings);
            UpdateResultToTable(data, verificationResult, settings);
            result.Correction = result.Correction.OrderBy(x => x.Code).ToList();
            result.CommonData = vResult;
            result.VerifyTime = (DateTime.Now - startDate);
            return result;
        }

        private void CreateColumns(DataTable data, UsAddressVerificationSettings settings)
        {
            if (settings.ReverseGeocode)
            {
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE, typeof(string));
            }

            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS1)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS1, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS2)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS2, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADMINISTRATIVEAREA)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADMINISTRATIVEAREA, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DELIVERYADDRESS)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DELIVERYADDRESS, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_COUNTRY)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_COUNTRY, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_LOCALITY)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_LOCALITY, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODE, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODEPRIMARY)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODEPRIMARY, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_PREMISENUMBER)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_PREMISENUMBER, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBADMINISTRATIVEAREA)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBADMINISTRATIVEAREA, typeof(string));
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_THOROUGHFARENAME)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_THOROUGHFARENAME, typeof(string));

            if (!settings.ReverseGeocode)
            {
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ORGANIZATIONNAME)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ORGANIZATIONNAME, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBBUILDINGNAME)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBBUILDINGNAME, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_BUILDINGNAME)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_BUILDINGNAME, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTBOX)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTBOX, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTTHOROUGHFARENAME)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTTHOROUGHFARENAME, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DOUBLEDEPENDENTLOCALITY)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DOUBLEDEPENDENTLOCALITY, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTLOCALITY)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTLOCALITY, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUPERADMINISTRATIVEAREA)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUPERADMINISTRATIVEAREA, typeof(string));
                if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODESECONDARY)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODESECONDARY, typeof(string));
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
                    // if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_AUTOZONEINDICATOR)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_AUTOZONEINDICATOR, typeof(string));
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
                    //if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBNUMBER)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBNUMBER, typeof(string));
                   // if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBTYPE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBTYPE, typeof(string));
                    if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PRIMARYADDRESSLINE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PRIMARYADDRESSLINE, typeof(string));
                    if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RECORDTYPE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RECORDTYPE, typeof(string));
                    if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RETURNCODE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RETURNCODE, typeof(string));
                    if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RESIDENTIALDELIVERY)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RESIDENTIALDELIVERY, typeof(string));
                    //if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SECONDARYADDRESSLINE)) data.Columns.Add(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SECONDARYADDRESSLINE, typeof(string));
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
        }

        private List<AddressVerificationResult> SelectChunkDataForVerification(WinPureDataFlowLocalBlockParameter parameter)
        {
            try
            {
                var verificationDataQuery = parameter.Data.AsEnumerable()
                    .Join(parameter.WinPureIds, f => f.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY), i => i, (dr, i) => dr)
                    .Select(x => new AddressVerificationResult
                    {
                        Address = parameter.Settings.AddressColumns.Aggregate("", (current, addCol) => current + " " + x.Field<object>(addCol) ?? "").Trim(),
                        Locality = parameter.Settings.LocalityColumns.Aggregate("", (current, addCol) => current + " " + x.Field<object>(addCol) ?? "").Trim(),
                        PostalCode = parameter.Settings.PostalCodeColumns.Aggregate("", (current, addCol) => current + " " + x.Field<object>(addCol) ?? "").Trim(),
                        AdministrativeArea = parameter.Settings.StateColumns.Aggregate("", (current, addCol) => current + " " + x.Field<object>(addCol) ?? "").Trim(),
                        Latitude = string.IsNullOrWhiteSpace(parameter.Settings.LatitudeColumn) ? 0 : Convert.ToDecimal(x.Field<string>(parameter.Settings.LatitudeColumn), CultureInfo.InvariantCulture),
                        Longitude = string.IsNullOrWhiteSpace(parameter.Settings.LongitudeColumn) ? 0 : Convert.ToDecimal(x.Field<string>(parameter.Settings.LongitudeColumn), CultureInfo.InvariantCulture),
                        WinpureId = x.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY)
                    }).ToList();

                return verificationDataQuery;
            }
            catch (Exception e)
            {
                Console.WriteLine($"SELECT ERROR {e.Message}");
            }
            return new List<AddressVerificationResult>();
        }

        private VerificationStatistic AddressVerificationChunk(WinPureDataFlowLocalBlockParameter parameter)
        {
            try
            {
                //var dtStart = DateTime.Now;
                var vStat = new VerificationStatistic();
                using (var srv = lqtServer.create())
                {
                    srv.init(parameter.Settings.PathToDb);

                    // Create the process list
                    var processList = lqtProcessList.create();
                    var processOptions = lqtProcessOptions.create();

                    processList.add("Verify", processOptions);
                    processList.add("Geocode", processOptions);

                    // Open the Loqate session
                    int session = srv.open();

                    try
                    {
                        for (int i = 0; i < parameter.VerificationData.Count; i++)
                        {
                            parameter.CToken.ThrowIfCancellationRequested();
                            var addressInputRecord = lqtInputRecord.create();
                            var addressResults = lqtProcessResult.create();

                            var addressVerificationData = parameter.VerificationData.ElementAt(i);
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
                                    addressInputRecord.set("AdministrativeArea",
                                        addressVerificationData.AdministrativeArea);
                                }

                                addressInputRecord.set("Country", parameter.CountryCode);


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

                                    if (parameter.Settings.GeoTag)
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

                                    if (parameter.Settings.AmasField)
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

                                    #region SERP NEW

                                    if (parameter.Settings.SerpField && addressVerificationData.AddressQualityIndex != "E")
                                    {
                                        var serpAddressInputRecord = lqtInputRecord.create();
                                        var serpAddressResults = lqtProcessResult.create();

                                        // Create the process list
                                        var processSerpList = lqtProcessList.create();
                                        var processSerpOptions = lqtProcessOptions.create();

                                        processSerpList.add("Verify", processSerpOptions);
                                        processSerpList.add("SERP", processSerpOptions);

                                        serpAddressInputRecord.set("Address1", addressVerificationData.Address1);
                                        serpAddressInputRecord.set("Country", parameter.CountryCode);
                                        serpAddressInputRecord.set("CertifiedCountryList", parameter.CountryCode);

                                        serpAddressInputRecord.set("Locality", addressVerificationData.Locality);
                                        serpAddressInputRecord.set("PostalCode", addressVerificationData.PostalCode);
                                        serpAddressInputRecord.set("AdministrativeArea", addressVerificationData.AdministrativeArea);

                                        srv.process(serpAddressInputRecord, processSerpList, serpAddressResults);
                                        if (serpAddressResults.getCount() > 0)
                                        {
                                            addressVerificationData.SerpStatusEx = serpAddressResults.getField(1, "SerpStatusEx");
                                            addressVerificationData.SerpQuestionable = serpAddressResults.getField(1, "Questionable");
                                            addressVerificationData.SerpResult = serpAddressResults.getField(1, "Result");
                                        }
                                    }

                                    #endregion

                                    if (parameter.Settings.CassField)
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
                                        cassAddressInputRecord.set("AdministrativeArea",
                                            addressVerificationData.AdministrativeArea);

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
                                           // addressVerificationData.CassPmbType = cassAddressResults.getField(1, "PMBType");
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
                            finally
                            {
                                lqtInputRecord.destroy(addressInputRecord);
                                lqtProcessResult.destroy(addressResults);
                            }
                            parameter.VerificationResult.Add(addressVerificationData);
                            parameter.ReportProgress();
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
                        lqtServer.destroy(srv);
                    }
                }
                return vStat;
            }
            catch (Exception e)
            {
                Console.WriteLine($"VERIFY ERROR {e.Message}");
            }

            return null;
        }

        private static VerificationStatistic ReverseGeocodeChunk(WinPureDataFlowLocalBlockParameter parameter)
        {
            try
            {
                //var dtStart = DateTime.Now;
                var vStat = new VerificationStatistic();
                using (var srv = lqtServer.create())
                {
                    srv.init(parameter.Settings.PathToDb);
                    var addressInputRecord = lqtInputRecord.create();
                    var addressResults = lqtProcessResult.create();


                    // Create the process list
                    var processList = lqtProcessList.create();
                    var processOptions = lqtProcessOptions.create();

                    processList.add("ReverseGeocode", processOptions);

                    // Open the Loqate session
                    int session = srv.open();

                    try
                    {
                        for (int i = 0; i < parameter.VerificationData.Count; i++)
                        {
                            parameter.CToken.ThrowIfCancellationRequested();
                            var addressVerificationData = parameter.VerificationData.ElementAt(i);
                            try
                            {
                                addressInputRecord.set("Latitude", addressVerificationData.Latitude.ToString());
                                addressInputRecord.set("Longitude", addressVerificationData.Longitude.ToString());
                                addressInputRecord.set("Country", parameter.CountryCode);


                                string verificationCode, qualityIndex;
                                // Process the record
                                srv.process(addressInputRecord, processList, addressResults);

                                if (addressResults.getCount() > 0)
                                {
                                    addressVerificationData.Address = addressResults.getField(0, "Address");
                                    addressVerificationData.Address1 = addressResults.getField(0, "Address1");
                                    addressVerificationData.Address2 = addressResults.getField(0, "Address2");
                                    addressVerificationData.GeoDistance = addressResults.getField(0, "GeoDistance");

                                    addressVerificationData.PremiseNumber = addressResults.getField(0, "Premise");
                                    addressVerificationData.ThoroughfareName = addressResults.getField(0, "ThoroughfareName");
                                    addressVerificationData.Locality = addressResults.getField(0, "Locality");
                                    addressVerificationData.SubAdministrativeArea = addressResults.getField(0, "SubAdministrativeArea");
                                    addressVerificationData.AdministrativeArea = addressResults.getField(0, "AdministrativeArea");
                                    addressVerificationData.PostalCode = addressResults.getField(0, "PostalCode");
                                    addressVerificationData.PostalCodePrimary = addressResults.getField(0, "PostalCodePrimary");
                                    addressVerificationData.DeliveryAddress = addressResults.getField(0, "DeliveryAddress");
                                    addressVerificationData.Country = addressResults.getField(0, "CountryName");
                                    qualityIndex = addressResults.getField(0, "AQI");
                                    addressVerificationData.AddressQualityIndex = qualityIndex;
                                    verificationCode = addressResults.getField(0, "AVC");
                                    addressVerificationData.AddressVerificationCode = verificationCode;

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
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }

                            parameter.ReportProgress();
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
                //Console.WriteLine($"VERIFY Chunk time {(DateTime.Now - dtStart).ToString("g")}");
                return vStat;
            }
            catch (Exception e)
            {
                Console.WriteLine($"VERIFY ERROR {e.Message}");
            }

            return null;
        }

        private void UpdateResultToTable(DataTable data, ConcurrentBag<AddressVerificationResult> result, UsAddressVerificationSettings settings)
        {
            try
            {
                foreach (var process in data.AsEnumerable().Join(result, d => d.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY), r => r.WinpureId, (d,r) => new {data = d, result = r}))
                {
                    try
                    {
                        process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS1, process.result.Address1);
                        process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS2, process.result.Address2);
                        process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_PREMISENUMBER, process.result.PremiseNumber);
                        process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_THOROUGHFARENAME, process.result.ThoroughfareName);
                        process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_LOCALITY, process.result.Locality);
                        process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADMINISTRATIVEAREA, process.result.AdministrativeArea);
                        process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODE, process.result.PostalCode);
                        process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODEPRIMARY, process.result.PostalCodePrimary);
                        process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DELIVERYADDRESS, process.result.DeliveryAddress);
                        process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_COUNTRY, process.result.Country);
                        process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBADMINISTRATIVEAREA, process.result.SubAdministrativeArea);

                        if (settings.ReverseGeocode)
                        {
                            process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS, process.result.Address);
                            process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE, process.result.GeoDistance);
                        }
                        else
                        {
                            process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ORGANIZATIONNAME, process.result.OrganizationName);
                            process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBBUILDINGNAME, process.result.SubBuildingName);
                            process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_BUILDINGNAME, process.result.BuildingName);
                            process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTBOX, process.result.Postbox);
                            process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTTHOROUGHFARENAME, process.result.DependentThoroughfareName);
                            process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DOUBLEDEPENDENTLOCALITY, process.result.DoubleDependentLocality);
                            process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTLOCALITY, process.result.DependentLocality);
                            process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUPERADMINISTRATIVEAREA, process.result.SuperAdministrativeArea);
                            process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODESECONDARY, process.result.PostalCodeSecondary);
                            process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_UNMATCHED, process.result.Unmatched);
                            process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSQUALITYINDEX, process.result.AddressQualityIndex);
                            process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSVERIFICATIONCODE, process.result.AddressVerificationCode);
                            process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_MATCHSCORE, process.result.MatchScore);

                            if (settings.GeoTag)
                            {
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LATITUDE, process.result.GeoLatitude);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LONGITUDE, process.result.GeoLongitude);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEOACCURACY, process.result.GeoAccuracy);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE, process.result.GeoDistance);
                            }

                            if (settings.CassField)
                            {
                                // process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_AUTOZONEINDICATOR, process.result.CassAutozoneIndicator);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CARRIERROUTE, process.result.CassCarrierRoute);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CMRAINDICATOR, process.result.CassCmraIndicator);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CONGRESSIONALDISTRICT, process.result.CassCongressionalDistrict);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DEFAULTFLAG, process.result.CassDefaultFlag);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DELIVERYPOINTBARCODE, process.result.CassDeliveryPointBarCode);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVCONFIRMEDINDICATOR, process.result.CassDpvConfirmedIndicator);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVFOOTNOTES, process.result.CassDpvFootnotes);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTCODE, process.result.CassElotCode);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTNUMBER, process.result.CassElotNumber);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_EWSFLAG, process.result.CassEwsFlag);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FALSEPOSITIVEINDICATOR, process.result.CassFalsePositiveIndicator);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FIPSCOUNTYCODE, process.result.CassFipsCountyCode);
                                // process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FOOTNOTES, process.result.CassFootnotes);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKCODE, process.result.CassLacsLinkCode);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKINDICATOR, process.result.CassLacsLinkIndicator);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSSTATUS, process.result.CassLacsStatus);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_NOSTATINDICATOR, process.result.CassNostatIndicator);
                                //process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBNUMBER, process.result.CassPmbNumber);
                                //process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBTYPE, process.result.CassPmbType);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PRIMARYADDRESSLINE, process.result.CassPrimaryAddressLine);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RECORDTYPE, process.result.CassRecordType);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RETURNCODE, process.result.CassReturnCode);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RESIDENTIALDELIVERY, process.result.CassResidentialDelivery);
                               // process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SECONDARYADDRESSLINE, process.result.CassSecondaryAddressLine);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SUITELINKFOOTNOTE, process.result.CassSuiteLinkFootnote);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_VACANTINDICATOR, process.result.CassVacantIndicator);
                            }

                            if (settings.AmasField)
                            {
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_DPID, process.result.AmasDpid);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORTYPE, process.result.AmasFloorType);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORNUMBER, process.result.AmasFloorNumber);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_LOTNUMBER, process.result.AmasLotNumber);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUM, process.result.AmasPostboxNumberSuffix);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERPREFIX, process.result.AmasPostboxNumberPrefix);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERSUFFIX, process.result.AmasPostboxNumberSuffix);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISE, process.result.AmasPrimaryPremise);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISESUFFIX, process.result.AmasPrimaryPremiseSuffix);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISE, process.result.AmasSecondaryPremise);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISESUFFIX, process.result.AmasSecondaryPremiseSuffix);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRESORTZONE, process.result.AmasPresortZone);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRINTPOSTZONE, process.result.AmasPrintPostZone);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_BARCODE, process.result.AmasBarcode);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYADDRESSLINE, process.result.AmasPrimaryAddressLine);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYADDRESSLINE, process.result.AmasSecondaryAddressLine);
                            }

                            if (settings.SerpField)
                            {
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_SERPSTATUSEX, process.result.SerpStatusEx);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_QUESTIONABLE, process.result.SerpQuestionable);
                                process.data.SetField(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_RESULT, process.result.SerpResult);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Debug($"ROW SAVE ERROR {e.Message}", e);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error($"SAVE ERROR {e.Message}", e);
            }
        }

        private void ProcessStatistic(AddressVerificationReport report, Dictionary<string, int> stat, Dictionary<string, string> description)
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

    class WinPureDataFlowLocalBlockParameter
    {
        public DataTable Data { get; set; }
        public string CountryCode { get; set; }
        public List<AddressVerificationResult> VerificationData { get; set; }
        public Action ReportProgress { get; set; }
        public UsAddressVerificationSettings Settings { get; set; }
        public CancellationToken CToken { get; set; }
        public List<long> WinPureIds { get; set; }
        public ConcurrentBag<VerificationStatistic> Statistic { get; set; }
        public ConcurrentBag<AddressVerificationResult> VerificationResult { get; set; }
    }
}