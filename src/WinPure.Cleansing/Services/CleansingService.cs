using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WinPure.Cleansing.Models.Statistic;
using WinPure.Common.Models;
using WinPure.Configuration.DependencyInjection;

namespace WinPure.Cleansing.DependencyInjection;

internal static partial class WinPureCleansingDependencyExtension
{
    private class CleansingService : ICleansingService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IStatisticPatternService _patternService;
        private readonly IWpLogger _logger;

        public CleansingService()
        {
            var dependency = new WinPureConfigurationDependency();
            _serviceProvider = dependency.ServiceProvider;
            _logger = _serviceProvider.GetService(typeof(IWpLogger)) as IWpLogger;
            _patternService = _serviceProvider.GetService(typeof(IStatisticPatternService)) as IStatisticPatternService;
        }

        public CleansingService(IWpLogger logger, IServiceProvider serviceProvider, IStatisticPatternService patternService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _patternService = patternService;
        }

        /// <summary>
        /// In-memory data cleansing
        /// </summary>
        /// <param name="settings">WinPure clean settings</param>
        /// <param name="data">DataTable with shource data for cleansing.</param>
        /// <param name="cancellationToken">Cancellation token for interruption of the cleansing process</param>
        public void CleanTable(DataTable data, WinPureCleanSettings settings, CancellationToken cancellationToken)
        {
            if (settings == null)
            {
                throw new WinPureArgumentException(Resources.EXCEPTION_SETTINGSNOTNULL);
            }

            if (data == null)
            {
                throw new WinPureArgumentException(Resources.EXCEPTION_SOURCEDTNOTNULL);
            }

            try
            {
                var wpKeyExists = ColumnHelper.EnsureWinPurePrimaryKeyExists(data);
                var pipelineBuilders = PipelineHelper.PreparePipelineBuilders(settings);
                var pipelines = PipelineHelper.CreatePipelines(pipelineBuilders, data, settings, _logger, _serviceProvider, cancellationToken);
                var parallelOptions = new ParallelOptions {CancellationToken = cancellationToken};

                Parallel.ForEach(pipelines, parallelOptions,
                    x =>
                    {
                        var res = x.Value.Execute(x.Key).Result;
                    });

                ProcessResult(data, pipelines.Keys, _logger);
                ShiftColumnExecutor.Execute(settings.ColumnShiftSettings, data, cancellationToken);
                
                if (!wpKeyExists)
                {
                    ColumnHelper.RemoveWinPurePrimaryKeyFieldFromTable(data);
                }
            }
            catch (OperationCanceledException ex)
            {
                throw ex;
            }
            catch (Exception exception)
            {
                _logger?.Error("Cannot clean the table", exception);
                throw new WinPureCleansingException(Resources.EXCEPTION_CLEANTABLEERROR, exception);
            }
        }

        /// <summary>
        /// In-memory calculation of statistic for given data
        /// </summary>
        /// <param name="data">DataTable with source data</param>
        /// <param name="cancellationToken">Cancellation token for interruption of the cleansing process</param>
        /// <returns>DataTable with WinPure statistic</returns>
        public DataTable CalculateStatistic(DataTable data, List<DataField> dataFields, CancellationToken cancellationToken)
        {
            if (data == null)
            {
                throw new WinPureArgumentException(Resources.EXCEPTION_SOURCEDTNOTNULL);
            }

            var parallelOptions = new ParallelOptions { CancellationToken = cancellationToken };
            var totalCount = data.Rows.Count;
            var taskList = new List<Action>();
            var statResult = new ConcurrentBag<ColumnStatistic>();

            try
            {
                foreach (DataColumn col in data.Columns.AsParallel())
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    if (col.ColumnName == WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY)
                    {
                        continue;
                    }
                    var dataField = dataFields?.FirstOrDefault(x => x.DatabaseName == col.ColumnName || x.DisplayName == col.ColumnName);

                    var lst = data.AsEnumerable()
                        .Select(x => x.Field<object>(col) == null ? null : x.Field<object>(col).ToString())
                        .ToList();
                    taskList.Add(() =>
                    {
                        var res = StatisticHelper.CalculateStatisticForColumn(lst, col.ColumnName, col.DataType, dataField, totalCount, col.Ordinal);
                        statResult.Add(res);
                    });
                }

                if (!cancellationToken.IsCancellationRequested)
                {
                    //logger.Information("Calculate statistic for each column");
                    Parallel.Invoke(parallelOptions, taskList.ToArray());

                    var statisticTable = StatisticHelper.GetStatisticTableResult(statResult, totalCount);
                    var patterns = AsyncHelpers.RunSync(_patternService.GetAllPatterns);

                    for (int i = 0; i < statisticTable.Rows.Count; i++)
                    {
                        var columnName = statisticTable.Rows[i]["FieldName"].ToString();
                        var dataField = dataFields.FirstOrDefault(x => x.DatabaseName == columnName || x.DisplayName == columnName);
                        if (!string.IsNullOrEmpty(dataField?.Pattern))
                        {
                            var pattern = patterns.FirstOrDefault(x => x.Pattern == dataField.Pattern);
                            statisticTable.Rows[i]["PatternName"] = pattern?.Name;
                        }
                    }

                    return statisticTable;
                }
            }
            catch (Exception exception)
            {
                _logger?.Error("Cannot calculate statistic", exception);
                throw new WinPureCleansingException(Resources.EXCEPTION_STATISTICERROR, exception);
            }

            return null;
        }

        private void ProcessResult(DataTable data, IEnumerable<CleansingContext> results, IWpLogger logger)
        {
            var cleansingExceptions = results.SelectMany(x => x.Exceptions).ToList();
            if (cleansingExceptions.Any())
            {
                var exceptionsOverview = cleansingExceptions.Select(x => new
                {
                    x.Executor,
                    x.OriginalValue,
                    ExceptionType = x.OriginalException?.GetType(),
                    Message = x.OriginalException?.Message,
                    InnerException = x.OriginalException?.InnerException?.Message
                });

                var exceptionsOverviewJson = JsonConvert.SerializeObject(exceptionsOverview);
                throw new WinPureCleansingException(string.Format(Resources.EXCEPTION_CLEANSINGCUMMULATIVEERROR, exceptionsOverviewJson), null);
            }

            try
            {
                CreateResultColumns(data, results);

                foreach (var cleansingContext in results)
                {
                    foreach (var s in data.AsEnumerable().Join(cleansingContext.CleansingData, d => d.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY), r => r.WinPureId, (d, r) => new { sourceData = d, result = r }))
                    {
                        if (cleansingContext.ColumnName != PipelineHelper.MultiColumnExecutorFieldName)
                        {
                            if (data.Columns[cleansingContext.ColumnName].DataType == typeof(string))
                            {
                                s.sourceData.SetField(cleansingContext.ColumnName, s.result.Value);
                            }
                            else if (string.IsNullOrEmpty(s.result.Value))
                            {
                                s.sourceData.SetField(cleansingContext.ColumnName, DBNull.Value);
                            }
                        }

                        if (!string.IsNullOrEmpty(s.result.MergeResult))
                        {
                            s.sourceData.SetField(cleansingContext.MergeResultColumnName, s.result.MergeResult);
                        }
                        
                        if (s.result.TextCheckerResult != null)
                        {
                            s.sourceData.SetField(cleansingContext.ColumnName + WinPureColumnNamesHelper.WPCOLUMN_VALIDATE_EMAIL, s.result.TextCheckerResult.IsValidEmail);
                        }

                        if (s.result.SplitResult != null)
                        {
                            if (s.result.SplitResult.SplitByRegex != null)
                            {
                                for (int i = 0; i < s.result.SplitResult.SplitByRegex.Count; i++)
                                {
                                    s.sourceData.SetField(
                                        $"{cleansingContext.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_REGEX}_{i + 1}",
                                        s.result.SplitResult.SplitByRegex[i]);
                                }
                            }

                            if (s.result.SplitResult.SplitIntoWords != null)
                            {
                                for (int i = 0; i < s.result.SplitResult.SplitIntoWords.Count; i++)
                                {
                                    s.sourceData.SetField($"{cleansingContext.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_INTO_WORDS}_{i + 1}",
                                        s.result.SplitResult.SplitIntoWords[i]);
                                }
                            }

                            if (s.result.SplitResult.Year.HasValue)
                            {
                                s.sourceData.SetField(cleansingContext.ColumnName + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DATE_DAY, s.result.SplitResult.Day);
                                s.sourceData.SetField(cleansingContext.ColumnName + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DATE_MONTH, s.result.SplitResult.Month);
                                s.sourceData.SetField(cleansingContext.ColumnName + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DATE_YEAR, s.result.SplitResult.Year);
                                s.sourceData.SetField(cleansingContext.ColumnName + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DATE_HOUR, s.result.SplitResult.Hour);
                                s.sourceData.SetField(cleansingContext.ColumnName + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DATE_MINUTE, s.result.SplitResult.Minute);
                                s.sourceData.SetField(cleansingContext.ColumnName + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DATE_SECOND, s.result.SplitResult.Second);
                            }

                            if (!string.IsNullOrEmpty(s.result.SplitResult.EmailAccount))
                            {
                                s.sourceData.SetField(cleansingContext.ColumnName + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ACCOUNT, s.result.SplitResult.EmailAccount);
                                s.sourceData.SetField(cleansingContext.ColumnName + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DOMAIN, s.result.SplitResult.EmailDomain);
                                s.sourceData.SetField(cleansingContext.ColumnName + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_EMAIL_COUNTRY, s.result.SplitResult.EmailCountry);
                                s.sourceData.SetField(cleansingContext.ColumnName + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_SUB_DOMAIN, s.result.SplitResult.EmailSubDomain);
                            }

                            if (!string.IsNullOrEmpty(s.result.SplitResult.SplitEmail) ||
                                !string.IsNullOrEmpty(s.result.SplitResult.SplitEmailName))
                            {
                                s.sourceData.SetField(cleansingContext.ColumnName + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_EMAIL, s.result.SplitResult.SplitEmail);
                                s.sourceData.SetField(cleansingContext.ColumnName + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_EMAIL_NAME, s.result.SplitResult.SplitEmailName);
                            }

                            if (!string.IsNullOrEmpty(s.result.SplitResult.PhoneCountry) ||
                                !string.IsNullOrEmpty(s.result.SplitResult.PhoneNumber))
                            {
                                s.sourceData.SetField(cleansingContext.ColumnName + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_PHONE_COUNTRY, s.result.SplitResult.PhoneCountry);
                                s.sourceData.SetField(cleansingContext.ColumnName + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_PHONE_NUMBER, s.result.SplitResult.PhoneNumber);
                            }
                        }

                        if (s.result.SplitGenderResult != null)
                        {
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_GENDER_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_PREFIX, s.result.SplitGenderResult.Prefix);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_GENDER_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_FIRST, s.result.SplitGenderResult.First);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_GENDER_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_MIDDLE, s.result.SplitGenderResult.Middle);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_GENDER_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_LAST, s.result.SplitGenderResult.Last);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_GENDER_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_SUFFIX, s.result.SplitGenderResult.Suffix);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_GENDER_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_QUALITY, s.result.SplitGenderResult.Quality);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_GENDER_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_GENDER, s.result.SplitGenderResult.Gender);
                        }

                        if (s.result.SplitAddressResult != null)
                        {
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_HOUSENUMBER, s.result.SplitAddressResult.House_Number);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_STREETNAME, s.result.SplitAddressResult.Road);
                            //s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_NEAR, s.result.SplitAddressResult.Near);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_HOUSE, s.result.SplitAddressResult.House);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_LEVEL, s.result.SplitAddressResult.Level);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_CATEGORY, s.result.SplitAddressResult.Category);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_POBOX, s.result.SplitAddressResult.PO_Box);
                            //s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_STAIRCASE, s.result.SplitAddressResult.Staircase);
                            //s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_WORLDREGION, s.result.SplitAddressResult.World_Region);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_UNIT, s.result.SplitAddressResult.Unit);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_STATEDISTRICT, s.result.SplitAddressResult.State_District);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_CITY, s.result.SplitAddressResult.City);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_STATE, s.result.SplitAddressResult.State);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_POSTCODE, s.result.SplitAddressResult.PostCode);
                            //s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_COUNTRYREGION, s.result.SplitAddressResult.Country_Region);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_COUNTRY, s.result.SplitAddressResult.Country);
                            //s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_ENTRANCE, s.result.SplitAddressResult.Entrance);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_SUBURB, s.result.SplitAddressResult.Suburb);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_CITYDISTRICT, s.result.SplitAddressResult.City_District);
                            s.sourceData.SetField(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX + WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_ISLAND, s.result.SplitAddressResult.Island);
                        }

                    }
                }
                //);
            }
            catch (Exception e)
            {
                logger.Error("Error on processing cleansing result.", e);
                throw new WinPureCleansingException(Resources.EXCEPTION_CLEAN_RESULT_NOT_PROCESSED, e);
            }
        }

        private void CreateResultColumns(DataTable data, IEnumerable<CleansingContext> results)
        {
            foreach (var cleansingContext in results.Where(x => x.CleansingData.Any(r => r.TextCheckerResult != null)))
            {
                ColumnHelper.AddColumn(data, cleansingContext.ColumnName, WinPureColumnNamesHelper.WPCOLUMN_VALIDATE_EMAIL, true, typeof(bool));
            }

            foreach (var cleansingContext in results.Where(x => x.CleansingData.Any(r => !string.IsNullOrEmpty(r.MergeResult))))
            {
                cleansingContext.MergeResultColumnName = ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_MERGE_RESULT, "", false, typeof(string));
            }

            foreach (var cleansingContext in results.Where(x => x.CleansingData.Any(r => r.SplitAddressResult != null)))
            {
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_HOUSENUMBER, true, typeof(string));
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_STREETNAME, true, typeof(string));
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_CITY, true, typeof(string));
                
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_POSTCODE, true, typeof(string));
                
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_HOUSE, true, typeof(string));
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_POBOX, true, typeof(string));
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_CITYDISTRICT, true, typeof(string));
                //ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_COUNTRYREGION, true, typeof(string));
                
                //ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_NEAR, true, typeof(string));
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_LEVEL, true, typeof(string));
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_CATEGORY, true, typeof(string));
                //ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_STAIRCASE, true, typeof(string));
                //ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_WORLDREGION, true, typeof(string));
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_UNIT, true, typeof(string)); 
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_STATEDISTRICT, true, typeof(string));
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_STATE, true, typeof(string));
                //ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_ENTRANCE, true, typeof(string));
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_SUBURB, true, typeof(string));
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_ISLAND, true, typeof(string));
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_COUNTRY, true, typeof(string));
            }

            foreach (var cleansingContext in results.Where(x => x.CleansingData.Any(r => r.SplitGenderResult != null)))
            {
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_GENDER_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_PREFIX, true, typeof(string));
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_GENDER_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_FIRST, true, typeof(string));
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_GENDER_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_MIDDLE, true, typeof(string));
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_GENDER_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_LAST, true, typeof(string)); 
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_GENDER_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_SUFFIX, true, typeof(string));
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_GENDER_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_QUALITY, true, typeof(string));
                ColumnHelper.AddColumn(data, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_GENDER_PREFIX, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_GENDER, true, typeof(string)); 
            }

            foreach (var cleansingContext in results.Where(x => x.CleansingData.Any(r => r.SplitResult != null)))
            {
                var splitResult = cleansingContext.CleansingData.Where(x => x.SplitResult != null).ToList();
                if (splitResult.Any(x => !string.IsNullOrEmpty(x.SplitResult.EmailAccount)))
                {
                    ColumnHelper.AddColumn(data, cleansingContext.ColumnName, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ACCOUNT, true, typeof(string));
                    ColumnHelper.AddColumn(data, cleansingContext.ColumnName, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DOMAIN, true, typeof(string));
                    ColumnHelper.AddColumn(data, cleansingContext.ColumnName, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_EMAIL_COUNTRY, true, typeof(string));
                    ColumnHelper.AddColumn(data, cleansingContext.ColumnName, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_SUB_DOMAIN, true, typeof(string));
                }

                if (splitResult.Any(x => !string.IsNullOrEmpty(x.SplitResult.SplitEmail) || !string.IsNullOrEmpty(x.SplitResult.SplitEmailName)))
                {
                    ColumnHelper.AddColumn(data, cleansingContext.ColumnName, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_EMAIL, true, typeof(string));
                    ColumnHelper.AddColumn(data, cleansingContext.ColumnName, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_EMAIL_NAME, true, typeof(string));
                }

                if (splitResult.Any(x => !string.IsNullOrEmpty(x.SplitResult.PhoneCountry) || !string.IsNullOrEmpty(x.SplitResult.PhoneNumber)))
                {
                    ColumnHelper.AddColumn(data, cleansingContext.ColumnName, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_PHONE_COUNTRY, true, typeof(string));
                    ColumnHelper.AddColumn(data, cleansingContext.ColumnName, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_PHONE_NUMBER, true, typeof(string));
                }

                if (splitResult.Any(x => x.SplitResult.Year.HasValue || x.SplitResult.Hour.HasValue))
                {
                    ColumnHelper.AddColumn(data, cleansingContext.ColumnName, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DATE_DAY, true, typeof(int));
                    ColumnHelper.AddColumn(data, cleansingContext.ColumnName, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DATE_MONTH, true, typeof(int));
                    ColumnHelper.AddColumn(data, cleansingContext.ColumnName, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DATE_YEAR, true, typeof(int));
                    ColumnHelper.AddColumn(data, cleansingContext.ColumnName, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DATE_HOUR, true, typeof(int));
                    ColumnHelper.AddColumn(data, cleansingContext.ColumnName, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DATE_MINUTE, true, typeof(int));
                    ColumnHelper.AddColumn(data, cleansingContext.ColumnName, WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DATE_SECOND, true, typeof(int));
                }

                var maxResultCount = splitResult.Max(x => x.SplitResult.SplitByRegex?.Count ?? 0);
                for (int i = 0; i < maxResultCount; i++)
                {
                    ColumnHelper.AddColumn(data, cleansingContext.ColumnName, $"{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_REGEX}_{i + 1}", true, typeof(string));
                }

                maxResultCount = splitResult.Max(x => x.SplitResult.SplitIntoWords?.Count ?? 0);
                for (int i = 0; i < maxResultCount; i++)
                {
                    ColumnHelper.AddColumn(data, cleansingContext.ColumnName, $"{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_INTO_WORDS}_{i + 1}", true, typeof(string));
                }
            }
        }
    }
}