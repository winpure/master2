using System.Data.SQLite;
using System.Globalization;
using WinPure.Configuration.Helper;

namespace WinPure.AddressVerification.DependencyInjection;

internal static partial class WinPureVerificationDependencyExtension
{
    private class UsAddressVerificationInDatabaseService : IUsAddressVerificationInDatabaseService
    {
        private readonly IWpLogger _logger;
        private static int CHUNK_SIZE = 200;

        public UsAddressVerificationInDatabaseService(IWpLogger logger)
        {
            _logger = logger;
        }

        public AddressVerificationReport VerifyAddresses(string dbPath, string tableName, UsAddressVerificationSettings settings, CancellationToken cToken, Action<string, int> onProgressAction, int rowToProcess = -1)
        {
            if (!settings.Verification && !settings.ReverseGeocode)
            {
                return null;
            }

            var vResult = new AddressVerificationStatistic { TableName = tableName };
            var countryCode = VerificationDictionaries.GetIso3ByCountryName(settings.Country);
            List<long> wpIdsForVerification;
            using (var connection = new SQLiteConnection(SystemDatabaseConnectionHelper.GetConnectionString(dbPath)))
            {
                connection.Open();
                if (settings.SelectedRows.Any())
                {
                    wpIdsForVerification = settings.SelectedRows;
                }
                else
                {
                    var selectQuery = SqLiteHelper.GetSelectQuery(tableName, WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY);
                    var winpureIds = SqLiteHelper.ExecuteQuery(selectQuery, connection);
                    wpIdsForVerification = winpureIds.AsEnumerable().Select(x => x.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY)).ToList();
                }

                var countQry = SqLiteHelper.GetCountQuery(tableName);
                vResult.TotalRecords = SqLiteHelper.ExecuteScalar<int>(countQry, connection);
                var tableSchema = SqLiteHelper.ExecuteQuery(SqLiteHelper.GetSelectQuery(tableName), connection, CommandBehavior.SchemaOnly);
                CreateColumns(connection, tableSchema, tableName, settings);
                connection.Close();
            }

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
            var startDate = DateTime.Now;

            var degreeOfParallelism = Environment.ProcessorCount;// * 2
            var dataFlowSelectOptions = new ExecutionDataflowBlockOptions { MaxMessagesPerTask = 1, EnsureOrdered = false, MaxDegreeOfParallelism = degreeOfParallelism, CancellationToken = cToken };
            var dataFlowVerifyOptions = new ExecutionDataflowBlockOptions { MaxMessagesPerTask = 1, EnsureOrdered = false, MaxDegreeOfParallelism = degreeOfParallelism, CancellationToken = cToken };
            var dataFlowSaveOptions = new ExecutionDataflowBlockOptions { MaxMessagesPerTask = 1, EnsureOrdered = false, MaxDegreeOfParallelism = 1, CancellationToken = cToken };
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            var prepareData = new TransformBlock<WinPureDataFlowInDatabaseBlockParameter, WinPureDataFlowInDatabaseBlockParameter>(
                parameter =>
                {
                    parameter.VerificationData = SelectChunkDataForVerification(parameter);
                    return parameter;
                }, dataFlowSelectOptions);

            TransformBlock<WinPureDataFlowInDatabaseBlockParameter, WinPureDataFlowInDatabaseBlockParameter> processData;

            if (settings.ReverseGeocode)
            {
                processData = new TransformBlock<WinPureDataFlowInDatabaseBlockParameter, WinPureDataFlowInDatabaseBlockParameter>(
                    parameter =>
                    {
                        var statistic = ReverseGeocodeChunk(parameter);
                        if (statistic != null)
                        {
                            parameter.Statistic.Add(statistic);
                        }
                        return parameter;
                    }, dataFlowVerifyOptions);
            }
            else
            {
                processData = new TransformBlock<WinPureDataFlowInDatabaseBlockParameter, WinPureDataFlowInDatabaseBlockParameter>(
                    parameter =>
                    {
                        var statistic = AddressVerificationChunk(parameter);
                        if (statistic != null)
                        {
                            parameter.Statistic.Add(statistic);
                        }
                        return parameter;
                    }, dataFlowVerifyOptions);
            }

            var saveResult = new ActionBlock<WinPureDataFlowInDatabaseBlockParameter>(parameter =>
            {
                SaveResultToDatabase(parameter);
            }, dataFlowSaveOptions);

            prepareData.LinkTo(processData, linkOptions);
            processData.LinkTo(saveResult, linkOptions);

            while (wpIdsForVerification.Any())
            {
                var idToProcess = wpIdsForVerification.Take(CHUNK_SIZE).ToList();
                var parameter = new WinPureDataFlowInDatabaseBlockParameter
                {
                    CountryCode = countryCode,
                    CToken = cToken,
                    DbPath = dbPath,
                    ReportProgress = onProgress,
                    Settings = settings,
                    Statistic = verificationChunksStatistic,
                    TableName = tableName,
                    WinPureIds = idToProcess
                };
                prepareData.Post(parameter);
                wpIdsForVerification = wpIdsForVerification.Except(idToProcess).ToList();
            }

            prepareData.Complete();
            Task.WaitAll(prepareData.Completion, processData.Completion, saveResult.Completion);

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

            result.CommonData = vResult;
            result.VerifyTime = (DateTime.Now - startDate);
            return result;
        }

        private void CreateColumns(SQLiteConnection connection, DataTable tableSchema, string tableName, UsAddressVerificationSettings settings)
        {
            if (settings.ReverseGeocode)
            {
                if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS, typeof(string));
                if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE, typeof(string));
            }

            if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS1)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS1, typeof(string));
            if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS2)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS2, typeof(string));
            if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADMINISTRATIVEAREA)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADMINISTRATIVEAREA, typeof(string));
            if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DELIVERYADDRESS)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_DELIVERYADDRESS, typeof(string));
            if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_COUNTRY)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_COUNTRY, typeof(string));
            if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_LOCALITY)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_LOCALITY, typeof(string));
            if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODE, typeof(string));
            if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODEPRIMARY)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODEPRIMARY, typeof(string));
            if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_PREMISENUMBER)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_PREMISENUMBER, typeof(string));
            if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBADMINISTRATIVEAREA)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBADMINISTRATIVEAREA, typeof(string));
            if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_THOROUGHFARENAME)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_THOROUGHFARENAME, typeof(string));

            if (!settings.ReverseGeocode)
            {
                if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ORGANIZATIONNAME)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_ORGANIZATIONNAME, typeof(string));
                if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBBUILDINGNAME)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBBUILDINGNAME, typeof(string));
                if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_BUILDINGNAME)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_BUILDINGNAME, typeof(string));
                if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTBOX)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTBOX, typeof(string));
                if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTTHOROUGHFARENAME)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTTHOROUGHFARENAME, typeof(string));
                if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DOUBLEDEPENDENTLOCALITY)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_DOUBLEDEPENDENTLOCALITY, typeof(string));
                if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTLOCALITY)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTLOCALITY, typeof(string));
                if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUPERADMINISTRATIVEAREA)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUPERADMINISTRATIVEAREA, typeof(string));
                if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODESECONDARY)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODESECONDARY, typeof(string));
                if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_UNMATCHED)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_UNMATCHED, typeof(string));
                if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSQUALITYINDEX)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSQUALITYINDEX, typeof(string));
                if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSVERIFICATIONCODE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSVERIFICATIONCODE, typeof(string));
                if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_MATCHSCORE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_MATCHSCORE, typeof(string));


                if (settings.GeoTag)
                {
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LATITUDE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LATITUDE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LONGITUDE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LONGITUDE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEOACCURACY)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEOACCURACY, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE, typeof(string));
                }

                if (settings.CassField)
                {
                   // if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_AUTOZONEINDICATOR)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_AUTOZONEINDICATOR, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CARRIERROUTE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CARRIERROUTE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CMRAINDICATOR)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CMRAINDICATOR, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CONGRESSIONALDISTRICT)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CONGRESSIONALDISTRICT, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DEFAULTFLAG)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DEFAULTFLAG, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DELIVERYPOINTBARCODE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DELIVERYPOINTBARCODE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVCONFIRMEDINDICATOR)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVCONFIRMEDINDICATOR, typeof(string));
                     if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVFOOTNOTES)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVFOOTNOTES, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTCODE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTCODE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTNUMBER)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTNUMBER, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_EWSFLAG)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_EWSFLAG, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FALSEPOSITIVEINDICATOR)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FALSEPOSITIVEINDICATOR, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FIPSCOUNTYCODE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FIPSCOUNTYCODE, typeof(string));
                   //  if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FOOTNOTES)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FOOTNOTES, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKCODE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKCODE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKINDICATOR)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKINDICATOR, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSSTATUS)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSSTATUS, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_NOSTATINDICATOR)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_NOSTATINDICATOR, typeof(string));
                   // if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBNUMBER)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBNUMBER, typeof(string));
                    //if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBTYPE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBTYPE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PRIMARYADDRESSLINE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PRIMARYADDRESSLINE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RECORDTYPE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RECORDTYPE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RETURNCODE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RETURNCODE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RESIDENTIALDELIVERY)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RESIDENTIALDELIVERY, typeof(string));
                    //if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SECONDARYADDRESSLINE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SECONDARYADDRESSLINE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SUITELINKFOOTNOTE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SUITELINKFOOTNOTE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_VACANTINDICATOR)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_VACANTINDICATOR, typeof(string));
                }

                if (settings.AmasField)
                {
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_DPID)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_DPID, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORTYPE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORTYPE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORNUMBER)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORNUMBER, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_LOTNUMBER)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_LOTNUMBER, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUM)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUM, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERPREFIX)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERPREFIX, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERSUFFIX)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERSUFFIX, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISESUFFIX)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISESUFFIX, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISESUFFIX)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISESUFFIX, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRESORTZONE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRESORTZONE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRINTPOSTZONE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRINTPOSTZONE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_BARCODE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_BARCODE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYADDRESSLINE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYADDRESSLINE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYADDRESSLINE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYADDRESSLINE, typeof(string));
                }

                if (settings.SerpField)
                {
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_SERPSTATUSEX)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_SERPSTATUSEX, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_QUESTIONABLE)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_QUESTIONABLE, typeof(string));
                    if (!tableSchema.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_RESULT)) SqLiteHelper.AddTableColumn(connection, tableName, WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_RESULT, typeof(string));
                }
            }
        }

        private List<AddressVerificationResult> SelectChunkDataForVerification(WinPureDataFlowInDatabaseBlockParameter parameter)
        {
            try
            {
                //var dtStart = DateTime.Now;
                using (var connection = new SQLiteConnection(SystemDatabaseConnectionHelper.GetConnectionString(parameter.DbPath)))
                {
                    connection.Open();
                    var fieldList = $"[{WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY}]";

                    if (parameter.Settings.AddressColumns.Any())
                    {
                        var addressFields = "[" + parameter.Settings.AddressColumns.Aggregate("", (current, addCol) => current + "],[" + addCol).Trim() + "]";
                        fieldList = $"{fieldList},{addressFields}";
                    }
                    if (parameter.Settings.LocalityColumns.Any())
                    {
                        var localityFields = "[" + parameter.Settings.LocalityColumns.Aggregate("", (current, addCol) => current + "],[" + addCol).Trim() + "]";
                        fieldList = $"{fieldList},{localityFields}";
                    }
                    if (parameter.Settings.PostalCodeColumns.Any())
                    {
                        var postalCodeFields = "[" + parameter.Settings.PostalCodeColumns.Aggregate("", (current, addCol) => current + "],[" + addCol).Trim() + "]";
                        fieldList = $"{fieldList},{postalCodeFields}";
                    }
                    if (parameter.Settings.StateColumns.Any())
                    {
                        var administrativeAreaFields = "[" + parameter.Settings.StateColumns.Aggregate("", (current, addCol) => current + "],[" + addCol).Trim() + "]";
                        fieldList = $"{fieldList},{administrativeAreaFields}";
                    }
                    if (!string.IsNullOrWhiteSpace(parameter.Settings.LatitudeColumn))
                    {
                        fieldList = $"{fieldList},{parameter.Settings.LatitudeColumn}";
                    }
                    if (!string.IsNullOrWhiteSpace(parameter.Settings.LongitudeColumn))
                    {
                        fieldList = $"{fieldList},{parameter.Settings.LongitudeColumn}";
                    }

                    fieldList = fieldList.Replace("[],", ""); //crazy ? 

                    var listOfIds = parameter.WinPureIds.Aggregate("", (current, addId) => current + "," + addId);

                    listOfIds = listOfIds.Substring(1, listOfIds.Length - 1);

                    var selectQuery = SqLiteHelper.GetSelectQuery(parameter.TableName, fieldList, $"{WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY} IN ({listOfIds})");

                    var data = SqLiteHelper.ExecuteQuery(selectQuery, connection);

                    var verificationDataQuery = data.AsEnumerable()
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

                    connection.Close();
                    //Console.WriteLine($"SELECT Chunk time {(DateTime.Now - dtStart).ToString("g")}");
                    return verificationDataQuery;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"SELECT ERROR {e.Message}");
            }
            return new List<AddressVerificationResult>();
        }

        private VerificationStatistic AddressVerificationChunk(WinPureDataFlowInDatabaseBlockParameter parameter)
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
                                            //addressVerificationData.CassPmbNumber = cassAddressResults.getField(1, "PMBNumber");
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
                            finally
                            {
                                lqtInputRecord.destroy(addressInputRecord);
                                lqtProcessResult.destroy(addressResults);
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

        private static VerificationStatistic ReverseGeocodeChunk(WinPureDataFlowInDatabaseBlockParameter parameter)
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

        private void SaveResultToDatabase(WinPureDataFlowInDatabaseBlockParameter parameter)
        {
            try
            {
                //var dtStart = DateTime.Now;
                using (var connection = new SQLiteConnection(SystemDatabaseConnectionHelper.GetConnectionString(parameter.DbPath)))
                {
                    connection.Open();
                    string updateFields = $"[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS1}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS1}";
                    updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS2}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS2}";
                    updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADMINISTRATIVEAREA}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADMINISTRATIVEAREA}";
                    updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_DELIVERYADDRESS}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_DELIVERYADDRESS}";
                    updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_COUNTRY}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_COUNTRY}";
                    updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_LOCALITY}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_LOCALITY}";
                    updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODE}";
                    updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODEPRIMARY}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODEPRIMARY}";
                    updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_PREMISENUMBER}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_PREMISENUMBER}";
                    updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBADMINISTRATIVEAREA}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBADMINISTRATIVEAREA}";
                    updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_THOROUGHFARENAME}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_THOROUGHFARENAME}";

                    if (parameter.Settings.ReverseGeocode)
                    {
                        updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS}";
                        updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE}";
                    }
                    else
                    {
                        updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_ORGANIZATIONNAME}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_ORGANIZATIONNAME}";
                        updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBBUILDINGNAME}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBBUILDINGNAME}";
                        updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_BUILDINGNAME}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_BUILDINGNAME}";
                        updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTBOX}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTBOX}";
                        updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTTHOROUGHFARENAME}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTTHOROUGHFARENAME}";
                        updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_DOUBLEDEPENDENTLOCALITY}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_DOUBLEDEPENDENTLOCALITY}";
                        updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTLOCALITY}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTLOCALITY}";
                        updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUPERADMINISTRATIVEAREA}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUPERADMINISTRATIVEAREA}";
                        updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODESECONDARY}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODESECONDARY}";
                        updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_UNMATCHED}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_UNMATCHED}";
                        updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSQUALITYINDEX}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSQUALITYINDEX}";
                        updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSVERIFICATIONCODE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSVERIFICATIONCODE}";
                        updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_MATCHSCORE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_MATCHSCORE}";


                        if (parameter.Settings.GeoTag)
                        {
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LATITUDE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LATITUDE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LONGITUDE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LONGITUDE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEOACCURACY}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEOACCURACY}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE}";
                        }

                        if (parameter.Settings.CassField)
                        {
                            // updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_AUTOZONEINDICATOR}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_AUTOZONEINDICATOR}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CARRIERROUTE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CARRIERROUTE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CMRAINDICATOR}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CMRAINDICATOR}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CONGRESSIONALDISTRICT}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CONGRESSIONALDISTRICT}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DEFAULTFLAG}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DEFAULTFLAG}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DELIVERYPOINTBARCODE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DELIVERYPOINTBARCODE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVCONFIRMEDINDICATOR}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVCONFIRMEDINDICATOR}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVFOOTNOTES}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVFOOTNOTES}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTCODE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTCODE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTNUMBER}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTNUMBER}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_EWSFLAG}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_EWSFLAG}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FALSEPOSITIVEINDICATOR}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FALSEPOSITIVEINDICATOR}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FIPSCOUNTYCODE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FIPSCOUNTYCODE}";
                            // updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FOOTNOTES}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FOOTNOTES}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKCODE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKCODE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKINDICATOR}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKINDICATOR}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSSTATUS}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSSTATUS}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_NOSTATINDICATOR}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_NOSTATINDICATOR}";
                           // updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBNUMBER}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBNUMBER}";
                            //updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBTYPE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBTYPE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PRIMARYADDRESSLINE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PRIMARYADDRESSLINE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RECORDTYPE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RECORDTYPE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RETURNCODE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RETURNCODE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RESIDENTIALDELIVERY}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RESIDENTIALDELIVERY}";
                           // updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SECONDARYADDRESSLINE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SECONDARYADDRESSLINE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SUITELINKFOOTNOTE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SUITELINKFOOTNOTE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_VACANTINDICATOR}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_VACANTINDICATOR}";
                        }

                        if (parameter.Settings.AmasField)
                        {
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_DPID}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_DPID}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORTYPE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORTYPE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORNUMBER}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORNUMBER}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_LOTNUMBER}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_LOTNUMBER}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUM}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUM}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERPREFIX}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERPREFIX}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERSUFFIX}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERSUFFIX}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISESUFFIX}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISESUFFIX}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISESUFFIX}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISESUFFIX}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRESORTZONE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRESORTZONE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRINTPOSTZONE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRINTPOSTZONE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_BARCODE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_BARCODE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYADDRESSLINE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYADDRESSLINE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYADDRESSLINE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYADDRESSLINE}";
                        }

                        if (parameter.Settings.SerpField)
                        {
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_SERPSTATUSEX}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_SERPSTATUSEX}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_QUESTIONABLE}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_QUESTIONABLE}";
                            updateFields += $",[{WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_RESULT}] = @p{WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_RESULT}";
                        }
                    }

                    var updateQuery = $"UPDATE [{parameter.TableName}] SET {updateFields}  WHERE {WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY}=@pWpId";

                    using (var command = new SQLiteCommand(updateQuery, connection))
                    {
                        //TODO Null value param
                        for (int i = 0; i < parameter.VerificationData.Count; i++)
                        {
                            try
                            {
                                command.Parameters.AddWithValue("@pWpId", parameter.VerificationData[i].WinpureId);
                                command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS1, parameter.VerificationData[i].Address1);
                                command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS2, parameter.VerificationData[i].Address2);
                                command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_PREMISENUMBER, parameter.VerificationData[i].PremiseNumber);
                                command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_THOROUGHFARENAME, parameter.VerificationData[i].ThoroughfareName);
                                command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_LOCALITY, parameter.VerificationData[i].Locality);
                                command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADMINISTRATIVEAREA, parameter.VerificationData[i].AdministrativeArea);
                                command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODE, parameter.VerificationData[i].PostalCode);
                                command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODEPRIMARY, parameter.VerificationData[i].PostalCodePrimary);
                                command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_DELIVERYADDRESS, parameter.VerificationData[i].DeliveryAddress);
                                command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_COUNTRY, parameter.VerificationData[i].Country);
                                command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBADMINISTRATIVEAREA, parameter.VerificationData[i].SubAdministrativeArea);

                                if (parameter.Settings.ReverseGeocode)
                                {
                                    command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESS, parameter.VerificationData[i].Address);
                                    command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE, parameter.VerificationData[i].GeoDistance);
                                }
                                else
                                {
                                    command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_ORGANIZATIONNAME, parameter.VerificationData[i].OrganizationName);
                                    command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUBBUILDINGNAME, parameter.VerificationData[i].SubBuildingName);
                                    command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_BUILDINGNAME, parameter.VerificationData[i].BuildingName);
                                    command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTBOX, parameter.VerificationData[i].Postbox);
                                    command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTTHOROUGHFARENAME, parameter.VerificationData[i].DependentThoroughfareName);
                                    command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_DOUBLEDEPENDENTLOCALITY, parameter.VerificationData[i].DoubleDependentLocality);
                                    command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_DEPENDENTLOCALITY, parameter.VerificationData[i].DependentLocality);
                                    command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_SUPERADMINISTRATIVEAREA, parameter.VerificationData[i].SuperAdministrativeArea);
                                    command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_POSTALCODESECONDARY, parameter.VerificationData[i].PostalCodeSecondary);
                                    command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_UNMATCHED, parameter.VerificationData[i].Unmatched);
                                    command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSQUALITYINDEX, parameter.VerificationData[i].AddressQualityIndex);
                                    command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_ADDRESSVERIFICATIONCODE, parameter.VerificationData[i].AddressVerificationCode);
                                    command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_MATCHSCORE, parameter.VerificationData[i].MatchScore);

                                    if (parameter.Settings.GeoTag)
                                    {
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LATITUDE, parameter.VerificationData[i].GeoLatitude);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_LONGITUDE, parameter.VerificationData[i].GeoLongitude);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEOACCURACY, parameter.VerificationData[i].GeoAccuracy);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_GEO_GEODISTANCE, parameter.VerificationData[i].GeoDistance);
                                    }

                                    if (parameter.Settings.CassField)
                                    {
                                        // command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_AUTOZONEINDICATOR, parameter.VerificationData[i].CassAutozoneIndicator);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CARRIERROUTE, parameter.VerificationData[i].CassCarrierRoute);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CMRAINDICATOR, parameter.VerificationData[i].CassCmraIndicator);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_CONGRESSIONALDISTRICT, parameter.VerificationData[i].CassCongressionalDistrict);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DEFAULTFLAG, parameter.VerificationData[i].CassDefaultFlag);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DELIVERYPOINTBARCODE, parameter.VerificationData[i].CassDeliveryPointBarCode);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVCONFIRMEDINDICATOR, parameter.VerificationData[i].CassDpvConfirmedIndicator);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_DPVFOOTNOTES, parameter.VerificationData[i].CassDpvFootnotes);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTCODE, parameter.VerificationData[i].CassElotCode);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_ELOTNUMBER, parameter.VerificationData[i].CassElotNumber);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_EWSFLAG, parameter.VerificationData[i].CassEwsFlag);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FALSEPOSITIVEINDICATOR, parameter.VerificationData[i].CassFalsePositiveIndicator);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FIPSCOUNTYCODE, parameter.VerificationData[i].CassFipsCountyCode);
                                       // command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_FOOTNOTES, parameter.VerificationData[i].CassFootnotes);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKCODE, parameter.VerificationData[i].CassLacsLinkCode);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSLINKINDICATOR, parameter.VerificationData[i].CassLacsLinkIndicator);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_LACSSTATUS, parameter.VerificationData[i].CassLacsStatus);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_NOSTATINDICATOR, parameter.VerificationData[i].CassNostatIndicator);
                                       // command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBNUMBER, parameter.VerificationData[i].CassPmbNumber);
                                       // command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PMBTYPE, parameter.VerificationData[i].CassPmbType);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_PRIMARYADDRESSLINE, parameter.VerificationData[i].CassPrimaryAddressLine);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RECORDTYPE, parameter.VerificationData[i].CassRecordType);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RETURNCODE, parameter.VerificationData[i].CassReturnCode);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_RESIDENTIALDELIVERY, parameter.VerificationData[i].CassResidentialDelivery);
                                       // command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SECONDARYADDRESSLINE, parameter.VerificationData[i].CassSecondaryAddressLine);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_SUITELINKFOOTNOTE, parameter.VerificationData[i].CassSuiteLinkFootnote);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_CASS_VACANTINDICATOR, parameter.VerificationData[i].CassVacantIndicator);
                                    }

                                    if (parameter.Settings.AmasField)
                                    {
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_DPID, parameter.VerificationData[i].AmasDpid);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORTYPE, parameter.VerificationData[i].AmasFloorType);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_FLOORNUMBER, parameter.VerificationData[i].AmasFloorNumber);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_LOTNUMBER, parameter.VerificationData[i].AmasLotNumber);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUM, parameter.VerificationData[i].AmasPostboxNumberSuffix);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERPREFIX, parameter.VerificationData[i].AmasPostboxNumberPrefix);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_POSTBOXNUMBERSUFFIX, parameter.VerificationData[i].AmasPostboxNumberSuffix);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISE, parameter.VerificationData[i].AmasPrimaryPremise);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYPREMISESUFFIX, parameter.VerificationData[i].AmasPrimaryPremiseSuffix);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISE, parameter.VerificationData[i].AmasSecondaryPremise);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYPREMISESUFFIX, parameter.VerificationData[i].AmasSecondaryPremiseSuffix);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRESORTZONE, parameter.VerificationData[i].AmasPresortZone);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRINTPOSTZONE, parameter.VerificationData[i].AmasPrintPostZone);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_BARCODE, parameter.VerificationData[i].AmasBarcode);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_PRIMARYADDRESSLINE, parameter.VerificationData[i].AmasPrimaryAddressLine);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_AMAS_SECONDARYADDRESSLINE, parameter.VerificationData[i].AmasSecondaryAddressLine);
                                    }

                                    if (parameter.Settings.SerpField)
                                    {
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_SERPSTATUSEX, parameter.VerificationData[i].SerpStatusEx);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_QUESTIONABLE, parameter.VerificationData[i].SerpQuestionable);
                                        command.Parameters.AddWithValue("@p" + WinPureColumnNamesHelper.WPCOLUMN_USADDR_SERP_RESULT, parameter.VerificationData[i].SerpResult);
                                    }
                                }
                                command.ExecuteNonQuery();
                            }
                            catch (Exception e)
                            {
                                _logger.Debug($"ROW SAVE ERROR {e.Message}", e);
                            }
                            finally
                            {
                                command.Parameters.Clear();
                            }
                        }
                    }
                    connection.Close();
                    //Console.WriteLine($"SAVE Chunk time {(DateTime.Now - dtStart).ToString("g")}");
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

    class WinPureDataFlowInDatabaseBlockParameter
    {
        public string DbPath { get; set; }
        public string CountryCode { get; set; }
        public string TableName { get; set; }
        public List<AddressVerificationResult> VerificationData { get; set; }
        public Action ReportProgress { get; set; }
        public UsAddressVerificationSettings Settings { get; set; }
        public CancellationToken CToken { get; set; }
        public List<long> WinPureIds { get; set; }
        public ConcurrentBag<VerificationStatistic> Statistic { get; set; }
    }
}