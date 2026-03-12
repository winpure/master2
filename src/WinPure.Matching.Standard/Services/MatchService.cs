using System.Data;
using System.Threading;
using WinPure.Common.Exceptions;
using WinPure.Common.Helpers;
using WinPure.Configuration.DependencyInjection;
using WinPure.Matching.Models;
using WinPure.Matching.Properties;

namespace WinPure.Matching.DependencyInjection;

internal static partial class WinPureMatchingDependencyExtension
{
    private class MatchService : IMatchService
    {
        private readonly IWpLogger _logger;
        private readonly IDictionaryService _dictionaryService;

        public MatchService()
        {
            var dependency = new WinPureConfigurationDependency();
            var serviceProvider = dependency.ServiceProvider;
            _logger = serviceProvider.GetService(typeof(IWpLogger)) as IWpLogger;
            _dictionaryService = serviceProvider.GetService(typeof(IDictionaryService)) as IDictionaryService;
        }

        public MatchService(IWpLogger logger, IDictionaryService dictionaryService)
        {
            _logger = logger;
            _dictionaryService = dictionaryService;
        }

        public DataTable MatchData(List<TableParameter> tables,
            MatchParameter parameter,
            List<FieldMapping> fieldMap,
            CancellationToken cToken,
            Action<string, int> raiseOnProgressUpdate,
            int recordsToProcess = -1)
        {
            raiseOnProgressUpdate(Resources.API_CAPTION_CHECK_INPUT_PARAMETERS, 1);

            VerifyMatchParameter(tables, parameter);

            foreach (var tbl in tables)
            {
                ColumnHelper.EnsureWinPurePrimaryKeyExists(tbl.TableData);
            }

            cToken.ThrowIfCancellationRequested();

            var res = GetMatchResultInternal(tables, parameter, raiseOnProgressUpdate, cToken, recordsToProcess);

            cToken.ThrowIfCancellationRequested();

            var startDT = DateTime.Now;
            raiseOnProgressUpdate(Resources.API_CAPTION_PREPARE_RESULT_TABLE, 80);

            var dt = ConvertMatchResultToDataTable(parameter, res, fieldMap);

            _logger.Information($"RESULT TABLE PREPARED, TIME: {(DateTime.Now - startDT):dd\\:hh\\:mm\\:ss}");
            raiseOnProgressUpdate(Resources.API_CAPTION_RETURN_RESULT_TABLE, 90);
            return dt;
        }

        private void VerifyMatchParameter(List<TableParameter> tables, MatchParameter parameter)
        {
            if (parameter == null || !parameter.Groups.Any())
            {
                throw new WinPureAPIWrongParametersException();
            }

            foreach (var condition in parameter.Groups.SelectMany(x => x.Conditions))
            {
                foreach (var fld in condition.Fields)
                {
                    var tbl = tables.FirstOrDefault(t => t.TableName == fld.TableName);

                    if (tbl == null)
                    {
                        throw new WinPureAPINoTableException(fld.TableName);
                    }

                    if (!tbl.TableData.Columns.Contains(fld.ColumnName))
                    {
                        throw new WinPureAPINoFieldException(fld.TableName, fld.ColumnName);
                    }

                    var colType = SupportFunctions.GetColumnType(tbl.TableData.Columns[fld.ColumnName]);

                    if (colType != MatchDataType.String && condition.MatchingType == MatchType.Fuzzy)
                    {
                        throw new WinPureAPIWrongConditionException(fld.TableName, fld.ColumnName, colType.ToString(),
                            condition.MatchingType.ToString());
                    }

                }
            }

            if (!parameter.CheckInternal)
            {
                if (string.IsNullOrEmpty(parameter.MainTable))
                {
                    parameter.MainTable =
                        parameter.Groups.SelectMany(x => x.Conditions)
                            .SelectMany(x => x.Fields)
                            .Select(x => x.TableName)
                            .FirstOrDefault();

                }

                if (!parameter.Groups.SelectMany(x => x.Conditions)
                        .SelectMany(x => x.Fields)
                        .Select(x => x.TableName).Contains(parameter.MainTable))
                {
                    throw new WinPureAPINoTableException(parameter.MainTable);
                }
            }
        }

        private DataTable ConvertMatchResultToDataTable(MatchParameter parameter,
            List<Item> matchResult,
            List<FieldMapping> fieldMap)
        {
            var (dt, columnMap) = SupportFunctions.GetMatchResultDataTableStructure(parameter, fieldMap);

            int groupId = 1;

            long pk = 1;
            foreach (var itm in matchResult)
            {
                var nr = dt.NewRow();
                nr[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY] = pk++;
                nr[columnMap[WinPureColumnNamesHelper.WPCOLUMN_GROUPID]] = groupId;
                nr[columnMap[WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME]] = itm.TableName;
                nr[columnMap[WinPureColumnNamesHelper.WPCOLUMN_ISMASTER]] = false;
                nr[columnMap[WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED]] = false;
                nr[columnMap[WinPureColumnNamesHelper.WPCOLUMN_TOTALSCORE]] = 100; //TODO
                foreach (var param in parameter.Groups)
                {
                    nr[columnMap[WinPureColumnNamesHelper.GetColumnGroupScoreName(param.GroupId)]] =
                        100; //TODO for score of each parameters

                    foreach (var cond in param.Conditions)
                    {
                        nr[
                            columnMap[
                                WinPureColumnNamesHelper.GetColumnConditionScoreName(param.GroupId,
                                    cond.Fields.First().ColumnName)]] = 100;
                    }
                }

                foreach (var field in fieldMap)
                {
                    var fldName = field.FieldMap.FirstOrDefault(x => x.TableName == itm.TableName);
                    if (fldName != null && itm.DataRow.Table.Columns.Contains(fldName.ColumnName))
                    {
                        nr[field.FieldName] = SupportFunctions.ConvertDataToDataType(itm.DataRow[fldName.ColumnName],
                            dt.Columns[field.FieldName].DataType);
                    }
                }

                dt.Rows.Add(nr);
                if (itm.SimilarItems != null)
                {
                    foreach (var similarItem in itm.SimilarItems.Where(x => x != null))
                    {
                        var sr = dt.NewRow();
                        sr[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY] = pk++;
                        sr[columnMap[WinPureColumnNamesHelper.WPCOLUMN_GROUPID]] = groupId;
                        sr[columnMap[WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME]] = similarItem.Item.TableName;
                        sr[columnMap[WinPureColumnNamesHelper.WPCOLUMN_ISMASTER]] = false;
                        sr[columnMap[WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED]] = false;
                        var resGroup = similarItem.MatchGroupValue;
                        sr[columnMap[WinPureColumnNamesHelper.WPCOLUMN_TOTALSCORE]] =
                            Math.Round(similarItem.RateSimilarity * 100, 2, MidpointRounding.AwayFromZero);
                        sr[columnMap[WinPureColumnNamesHelper.GetColumnGroupScoreName(resGroup.GroupId)]] =
                            Math.Round(similarItem.RateSimilarity * 100, 2, MidpointRounding.AwayFromZero);

                        int p = 0;
                        foreach (var cond in resGroup.Values)
                        {
                            var field = ((MatchCondition)cond.Condition).Fields.First();

                            if (cond.ConditionType == MatchType.Fuzzy && similarItem.RateSimilarities.Count >= p + 1)
                            {
                                sr[
                                    columnMap[
                                        WinPureColumnNamesHelper.GetColumnConditionScoreName(resGroup.GroupId,
                                            field.ColumnName)]] = Math.Round(similarItem.RateSimilarities[p] * 100, 2,
                                    MidpointRounding.AwayFromZero);
                            }
                            else
                            {
                                sr[
                                    columnMap[
                                        WinPureColumnNamesHelper.GetColumnConditionScoreName(resGroup.GroupId,
                                            field.ColumnName)]] = 100;
                            }

                            p++;
                        }

                        foreach (var fld in fieldMap)
                        {
                            var fldName = fld.FieldMap.FirstOrDefault(x => x.TableName == similarItem.Item.TableName);
                            if (fldName != null && similarItem.Item.DataRow.Table.Columns.Contains(fldName.ColumnName))
                            {
                                sr[fld.FieldName] = SupportFunctions.ConvertDataToDataType(
                                    similarItem.Item.DataRow[fldName.ColumnName], dt.Columns[fld.FieldName].DataType);
                            }
                        }

                        dt.Rows.Add(sr);
                    }
                }

                groupId++;
            }

            return dt;
        }

        private bool IsDirectMatch(MatchParameter parameters) => parameters.Groups.SelectMany(x => x.Conditions)
            .All(x => x.MatchingType != MatchType.Fuzzy);


        private List<Item> GetMatchResultInternal(List<TableParameter> tables,
            MatchParameter parameters,
            Action<string, int> onProgressAction,
            CancellationToken cToken,
            int recordsToProcess = -1)
        {
            var isDirectCompare = IsDirectMatch(parameters);
            var configuration = WinPureConfigurationDependency.Resolve<IConfigurationService>().Configuration;

            var parallelOption = new ParallelOptions
            {
                CancellationToken = cToken,
            };

            var matchContext = new MatchContext
            {
                OnProgress = onProgressAction,
                Parameter = parameters,
                CToken = cToken,
                DictionaryService = _dictionaryService,
                Logger = _logger,
                ParallelOptions = parallelOption,
                Tables = tables,
                RecordsToProcess = recordsToProcess,
                CurrentGroupId = 1,
                ProgressForGroup = 50 / parameters.Groups.Count
            };

            _logger.Information($"PROCESSOR COUNT =  {Environment.ProcessorCount}, Total memory -");
            _logger.Information(
                $"START NEW INTERNAL SEARCH. Date: {DateTime.Now} | Tables count: {tables.Count()} | Total rows count: {tables.Sum(x => x.TableData.Rows.Count)} | Parameters: {parameters}");
            var startDt = DateTime.Now;

            var pipelineBuilder = isDirectCompare
                ?
                configuration.UseMixedRules 
                    ? MatchPipelineHelper.CreateDirectMixedMatchPipeline()
                    : MatchPipelineHelper.CreateDirectMatchPipeline()
                : configuration.UseMixedRules
                    ? MatchPipelineHelper.CreateMixedFuzzyMatchPipeline(parameters.CheckInternal)
                    : MatchPipelineHelper.CreateFuzzyMatchPipeline();

            var pipeline = pipelineBuilder.Build();
            var result = AsyncHelpers.RunSync(() => pipeline.Execute(matchContext));

            _logger.Information($"Internal match complete, TIME: {(DateTime.Now - startDt):dd\\:hh\\:mm\\:ss}");
            return result
                .OrderByDescending(x => x.SimilarItems.Count)
                .ToList();
        }
    }
}