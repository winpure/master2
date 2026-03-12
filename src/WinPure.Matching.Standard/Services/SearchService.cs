using System.Data;
using System.Threading;
using WinPure.Common.Exceptions;
using WinPure.Common.Helpers;
using WinPure.Configuration.DependencyInjection;
using WinPure.Configuration.Helper;
using WinPure.Matching.Algorithm;
using WinPure.Matching.Models;
using WinPure.Matching.Properties;

namespace WinPure.Matching.DependencyInjection;

internal static partial class WinPureMatchingDependencyExtension
{
    private class SearchService : ISearchService
    {
        private readonly IWpLogger _logger;
        private readonly IDictionaryService _dictionaryService;

        public SearchService()
        {
            var dependency = new WinPureConfigurationDependency();
            var serviceProvider = dependency.ServiceProvider;
            _logger = serviceProvider.GetService(typeof(IWpLogger)) as IWpLogger;
            _dictionaryService = serviceProvider.GetService(typeof(IDictionaryService)) as IDictionaryService;
        }

        public SearchService(IWpLogger logger, IDictionaryService dictionaryService)
        {
            _logger = logger;
            _dictionaryService = dictionaryService;
        }

        public DataTable SearchData(TableParameter table, SearchParameter parameter, CancellationToken cToken,
            Action<string, int> raiseOnProgressUpdate, int recordsToProcess = -1)
        {
            raiseOnProgressUpdate(Resources.API_CAPTION_CHECK_INPUT_PARAMETERS, 0);

            VerifySearchParameter(table, parameter);

            ColumnHelper.EnsureWinPurePrimaryKeyExists(table.TableData);
            cToken.ThrowIfCancellationRequested();

            var dictionaries = new Dictionary<string, Dictionary<string, string>>();

            foreach (var condition in parameter.Groups.SelectMany(x => x.Conditions))
            {
                if (!condition.DictionaryType.IsWpDictionary()
                    || dictionaries.ContainsKey(condition.DictionaryType))
                {
                    continue;
                }

                var dict = _dictionaryService.GetDictionary(condition.DictionaryType).Result;
                dictionaries.Add(condition.DictionaryType, dict);
            }

            var fuzzyComparison = FuzzyFactory.GetFuzzyAlgorithm(parameter.FuzzyAlgorithm);

            raiseOnProgressUpdate(Resources.API_CAPTION_CREATING_ITEMS_FOR_MATCHING, 10);

            raiseOnProgressUpdate(Resources.API_CAPTION_MATCHING_DATA, 30);

            var tasksList = new List<Task<List<SearchResult>>>();

            foreach (var group in parameter.Groups)
            {
                tasksList.Add(Task.Factory.StartNew(() =>
                {
                    var qry = table.TableData.AsEnumerable().AsEnumerable();

                    if (recordsToProcess >= 0)
                    {
                        qry = qry.Take(recordsToProcess);
                    }

                    var searchResults = qry.AsParallel().Select(x => new SearchResult
                    {
                        GroupId = group.GroupId,
                        GroupLevel = group.GroupLevel,
                        Key = x.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY),
                        Row = x,
                        ConditionScores = new List<double>(),
                        ConditionPass = new List<bool>()
                    });

                    foreach (var condition in group.Conditions)
                    {
                        var useDirect = condition.SearchValue == null || condition.SearchValue.ToString().Length <= 3;
                        searchResults = searchResults.Select(x =>
                        {
                            var objectValue = x.Row.Field<object>(condition.SearchField.ColumnName);

                            if (objectValue == null || objectValue == DBNull.Value)
                            {
                                x.ConditionScores.Add(0);
                                x.ConditionPass.Add(false);
                                return x;
                            }

                            var stringValue = objectValue.ToString();
                            if (condition.DictionaryType.IsWpDictionary())
                            {
                                stringValue = stringValue.ApplyDictionary(dictionaries[condition.DictionaryType]);
                            }

                            if (condition.MatchingType == MatchType.DirectCompare || useDirect ||
                                stringValue.Length <= 3)
                            {
                                if (string.Compare(condition.SearchValue?.ToString(), stringValue,
                                        StringComparison.CurrentCultureIgnoreCase) == 0)
                                {
                                    x.ConditionScores.Add(1);
                                    x.ConditionPass.Add(true);
                                }
                                else
                                {
                                    x.ConditionScores.Add(0);
                                    x.ConditionPass.Add(false);
                                }
                            }
                            else
                            {
                                var score = fuzzyComparison.CompareString(condition.SearchValue.ToString().ToLower(),
                                    stringValue.ToLower());
                                x.ConditionScores.Add(score);
                                x.ConditionPass.Add(score >= condition.Level);
                            }

                            return x;
                        });
                    }

                    return searchResults
                        .Where(x => x.ConditionPass.All(c => c))
                        .Select(x =>
                        {
                            x.Score = Math.Round(x.ConditionScores.Sum() / x.ConditionScores.Count, 2,
                                MidpointRounding.AwayFromZero);
                            return x;
                        }).Where(x => x.Score >= x.GroupLevel).ToList();
                }));
            }

            var results = Task.WhenAll(tasksList.ToArray()).Result.SelectMany(x => x).Distinct().ToList();

            raiseOnProgressUpdate(Resources.API_CAPTION_PREPARE_RESULT_TABLE, 80);
            cToken.ThrowIfCancellationRequested();

            var resultDt = ConvertSearchResultToDataTable(parameter, results, table);

            raiseOnProgressUpdate(Resources.API_CAPTION_RETURN_RESULT_TABLE, 90);
            return resultDt;
        }

        private DataTable ConvertSearchResultToDataTable(SearchParameter searchParameter,
            List<SearchResult> searchResult, TableParameter table)
        {
            searchResult = searchResult.OrderBy(x => x.Key).ThenBy(x => x.Score).ToList();
            var (dt, fieldMap) =
                SupportFunctions.GetSearchResultDataTableStructureBase(searchParameter, table.TableData);
            long pk = 1;
            long lastProcessedWpk = -1;
            for (int i = 0; i < searchResult.Count; i++)
            {
                var itm = searchResult[i];
                if (lastProcessedWpk == itm.Key)
                {
                    continue;
                }

                lastProcessedWpk = itm.Key;

                var sr = dt.NewRow();
                sr[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY] = pk++;
                sr[fieldMap[WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME]] = table.TableName;
                sr[fieldMap[WinPureColumnNamesHelper.WPCOLUMN_ISMASTER]] = false;
                sr[fieldMap[WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED]] = false;
                sr[fieldMap[WinPureColumnNamesHelper.WPCOLUMN_TOTALSCORE]] = Convert.ToInt32(itm.Score * 100);
                sr[fieldMap[WinPureColumnNamesHelper.GetColumnGroupScoreName(itm.GroupId)]] =
                    Convert.ToInt32(itm.Score * 100);

                var resGroup = searchParameter.Groups.First(x => x.GroupId == itm.GroupId);
                for (int j = 0; j < resGroup.Conditions.Count; j++)
                {
                    var cond = resGroup.Conditions[j];

                    var field = cond.SearchField;

                    if (cond.MatchingType == MatchType.Fuzzy && itm.ConditionScores.Count > j)
                    {
                        sr[
                            fieldMap[
                                WinPureColumnNamesHelper.GetColumnConditionScoreName(resGroup.GroupId,
                                    field.ColumnName)]] = itm.ConditionScores[j] * 100;
                    }
                    else
                    {
                        sr[
                            fieldMap[
                                WinPureColumnNamesHelper.GetColumnConditionScoreName(resGroup.GroupId,
                                    field.ColumnName)]] = 100;
                    }
                }

                foreach (DataColumn column in table.TableData.Columns)
                {
                    sr[column.ColumnName] = itm.Row[column.ColumnName];
                }

                dt.Rows.Add(sr);
            }

            return dt;
        }

        private void VerifySearchParameter(TableParameter table, SearchParameter parameter)
        {
            if (parameter == null || !parameter.Groups.Any())
            {
                throw new WinPureAPIWrongParametersException();
            }

            foreach (var condition in parameter.Groups.SelectMany(x => x.Conditions))
            {
                if (table.TableName != condition.SearchField.TableName)
                {
                    throw new WinPureAPINoTableException(condition.SearchField.TableName);
                }

                if (!table.TableData.Columns.Contains(condition.SearchField.ColumnName))
                {
                    throw new WinPureAPINoFieldException(condition.SearchField.TableName,
                        condition.SearchField.ColumnName);
                }

                var colType = SupportFunctions.GetColumnType(table.TableData.Columns[condition.SearchField.ColumnName]);

                if (colType != MatchDataType.String && condition.MatchingType == MatchType.Fuzzy)
                {
                    throw new WinPureAPIWrongConditionException(condition.SearchField.TableName,
                        condition.SearchField.ColumnName, colType.ToString(), condition.MatchingType.ToString());
                }
            }
        }
    }
}