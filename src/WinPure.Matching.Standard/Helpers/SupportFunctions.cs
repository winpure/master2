using System.Data;
using System.Threading.Tasks.Dataflow;
using WinPure.Common.Exceptions;
using WinPure.Common.Helpers;
using WinPure.Matching.Algorithm;
using WinPure.Matching.Models;
using WinPure.Matching.Models.Support;
using WinPure.Matching.Properties;

namespace WinPure.Matching.Helpers;

internal static class SupportFunctions
{
    public static string MatchGroupDummyCondition => Resources.CAPTION_DUMMYCONDITION;

    internal static void SetGetValueInItems(List<Item> items, MatchParameter parameters, ParallelOptions parallelOption, IDictionaryService dictionaryService)
    {
        var groups = parameters.Groups.OrderBy(x => x.GroupId).ToList();
        var fuzzyAlgorithm = FuzzyFactory.GetFuzzyAlgorithm(parameters.FuzzyAlgorithm);
        //foreach (var item in items)
        //Parallel.ForEach(items, parallelOption, item =>
        var dataFlowTaskOption = new ExecutionDataflowBlockOptions { EnsureOrdered = false, CancellationToken = parallelOption.CancellationToken };
        var action = new ActionBlock<Item>(item =>
        {
            for (var g = 0; g < groups.Count; g++)
            {
                var group = groups[g];
                var grCond = new GroupValue(group);
                var condOrd = group.Conditions.OrderBy(x => x.MatchingType).ToList();
                for (var c = 0; c < condOrd.Count; c++)
                {
                    var condition = condOrd[c];
                    var fld = condition.Fields.Where(x => x.TableName == item.TableName)
                        .OrderBy(x => item.DataRow.Table.Columns.Contains(x.ColumnName) ? 0 : 1)
                        .FirstOrDefault();

                    if (fld == null)
                    {
                        continue;
                    }

                    var columnName = fld.ColumnName;
                    var value = item.DataRow[columnName] == DBNull.Value ? null : item.DataRow[columnName];
                    switch (condition.MatchingType)
                    {
                        case MatchType.Fuzzy:
                            grCond.Values.Add(fuzzyAlgorithm.CreateStringCondition(value?.ToString(), condition, dictionaryService));
                            break;
                        case MatchType.DirectCompare:
                            grCond.Values.Add(new DefaultCondition(value, condition, dictionaryService));
                            break;
                        default:
                            throw new WinPureArgumentException("Match type not found");
                    }
                }
                grCond.CalculateHashCode();
                item.AddGroupValue(grCond);
            }
        }, dataFlowTaskOption);

        for (var i = 0; i < items.Count; i++)
        {
            action.Post(items[i]);
        }
        action.Complete();

        Task.WaitAll(action.Completion);
    }

    internal static object ConvertDataToDataType(object data, Type dtType)
    {
        if (dtType == typeof(string))
        {
            return data;
        }

        if (dtType == typeof(int) || dtType == typeof(long) || dtType == typeof(byte) || dtType == typeof(short))
        {
            long lng;
            return (long.TryParse(data.ToString(), out lng)) ? data : DBNull.Value;
        }

        if (dtType == typeof(double) || dtType == typeof(float) || dtType == typeof(decimal))
        {
            double dbl;
            return (double.TryParse(data.ToString(), out dbl)) ? data : DBNull.Value;
        }

        if (dtType == typeof(DateTime))
        {
            DateTime dbl;
            return (DateTime.TryParse(data.ToString(), out dbl)) ? data : DBNull.Value;
        }

        if (dtType == typeof(bool))
        {
            bool dbl;
            return (bool.TryParse(data.ToString(), out dbl)) ? data : DBNull.Value;
        }

        return DBNull.Value;
    }

    private static Dictionary<string, string> AddMatchResultFields(DataTable matchResultTable, MatchParameter parameter)
    {
        var fieldMap = new Dictionary<string, string>();
        var indexColumn = new DataColumn(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY, typeof(long));
        matchResultTable.Columns.Add(indexColumn);
        indexColumn.SetOrdinal(0);

        AddColumnToTable(matchResultTable, WinPureColumnNamesHelper.WPCOLUMN_GROUPID, typeof(int), 1, fieldMap);
        AddColumnToTable(matchResultTable, WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME, typeof(string), 2, fieldMap);
        AddColumnToTable(matchResultTable, WinPureColumnNamesHelper.WPCOLUMN_ISMASTER, typeof(bool), 3, fieldMap);
        AddColumnToTable(matchResultTable, WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED, typeof(bool), 4, fieldMap);
        AddColumnToTable(matchResultTable, WinPureColumnNamesHelper.WPCOLUMN_TOTALSCORE, typeof(double), 5, fieldMap);

        var position = 6;
        //add fields for the score of the parameters
        for (var g = 0; g < parameter.Groups.Count; g++)
        {
            var param = parameter.Groups[g];
            AddColumnToTable(matchResultTable, WinPureColumnNamesHelper.GetColumnGroupScoreName(param.GroupId), typeof(double), position, fieldMap);
            position++;

            // add columns to rule score result if groups exists.
            for (var c = 0; c < param.Conditions.Count; c++)
            {
                AddColumnToTable(matchResultTable, WinPureColumnNamesHelper.GetColumnConditionScoreName(param.GroupId, param.Conditions[c].Fields.First().ColumnName), typeof(double), position, fieldMap);
                position++;
            }
        }
        return fieldMap;
    }

    private static Dictionary<string, string> AddSearchResultFields(DataTable searchResultTable, SearchParameter parameter)
    {
        var fieldMap = new Dictionary<string, string>();
        var indexColumn = new DataColumn(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY, typeof(long));
        searchResultTable.Columns.Add(indexColumn);
        indexColumn.SetOrdinal(0);

        AddColumnToTable(searchResultTable, WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME, typeof(string), 1, fieldMap);
        AddColumnToTable(searchResultTable, WinPureColumnNamesHelper.WPCOLUMN_ISMASTER, typeof(bool), 2, fieldMap);
        AddColumnToTable(searchResultTable, WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED, typeof(bool), 3, fieldMap);
        AddColumnToTable(searchResultTable, WinPureColumnNamesHelper.WPCOLUMN_TOTALSCORE, typeof(int), 4, fieldMap);

        var position = 5;
        //add fields for the score of the parameters
        for (var g = 0; g < parameter.Groups.Count; g++)
        {
            var param = parameter.Groups[g];

            AddColumnToTable(searchResultTable, WinPureColumnNamesHelper.GetColumnGroupScoreName(param.GroupId), typeof(int), position, fieldMap);
            position++;

            // add columns to group score result if groups exists.
            for (var c = 0; c < param.Conditions.Count; c++)
            {
                AddColumnToTable(searchResultTable, WinPureColumnNamesHelper.GetColumnConditionScoreName(param.GroupId, param.Conditions[c].SearchField.ColumnName), typeof(int), position, fieldMap);
                position++;
            }
        }
        return fieldMap;
    }

    private static void AddColumnToTable(DataTable data, string columnName, Type columnType, int columnPosition, Dictionary<string, string> fieldName)
    {
        var uniqueColumnName = ColumnHelper.GetDataTableUniqueColumnName(data, columnName);
        fieldName.Add(columnName, uniqueColumnName);
        var column = new DataColumn(uniqueColumnName, columnType);
        data.Columns.Add(column);
        column.SetOrdinal(columnPosition);
    }

    internal static (DataTable, Dictionary<string, string>) GetSearchResultDataTableStructureBase(SearchParameter parameter, DataTable sourceDataTable)
    {
        var res = new DataTable("Search_result");
        foreach (DataColumn col in sourceDataTable.Columns)
        {
            if (col.ColumnName == WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY)
            {
                continue;
            }
            res.Columns.Add(new DataColumn(col.ColumnName, col.DataType));
        }

        var resultFieldMap = AddSearchResultFields(res, parameter);

        return (res, resultFieldMap);
    }

    internal static (DataTable, Dictionary<string, string>) GetMatchResultDataTableStructure(MatchParameter parameter, List<FieldMapping> fieldMap)
    {
        var res = new DataTable("Match_result");
        //add fields for the all source tables
        for (var f = 0; f < fieldMap.Count; f++)
        {
            var fld = fieldMap[f];
            if (fld.FieldName == WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY)
            {
                continue;
            }
            int i = 1;
            string baseName = fld.FieldName;
            while (res.Columns.Contains(fld.FieldName))
            {
                fld.FieldName = baseName + "_" + i;
                i++;
            }
            res.Columns.Add(new DataColumn(fld.FieldName, fld.FieldType));
        }
        var columnFieldMap = AddMatchResultFields(res, parameter);
        return (res, columnFieldMap);
    }

    internal static MatchDataType GetColumnType(DataColumn dc)
    {
        var tp = dc.DataType;

        if (tp == typeof(string))
        {
            return MatchDataType.String;
        }

        if (tp == typeof(Int16)
            || tp == typeof(Int32)
            || tp == typeof(Int64)
            || tp == typeof(int)
            || tp == typeof(long)
            || tp == typeof(short)
            || tp == typeof(byte))
        {
            return MatchDataType.Integer;
        }

        if (tp == typeof(DateTime))
        {
            return MatchDataType.DateTime;
        }

        if (tp == typeof(double)
            || tp == typeof(decimal)
            || tp == typeof(float))
        {
            return MatchDataType.Real;
        }

        if (tp == typeof(bool))
        {
            return MatchDataType.Integer;
        }

        return MatchDataType.String;
    }

    internal static double RateSimilarity(Item currentItem, Item mainItem, int currentGroupId, int levelDetailSearch, out GroupValue matchGroupValue, out List<double> rateSimilarities, out int typeSimilarity)
    {
        double maxRate = -1;
        rateSimilarities = null;
        matchGroupValue = null;
        typeSimilarity = -1;

        var currentRateSimilarities = new List<double>();
        var isNotViolatedMinRate = true;
        double rateWeight = 0;
        var g = currentItem.GroupValues[currentGroupId - 1];
        var g2 = mainItem.GroupValues[currentGroupId - 1];
        double rate;
        for (var i = 0; i < Math.Min(g.Values.Count, g2.Values.Count); i++)
        {
            rate = g.Values[i].Compare(g2.Values[i], levelDetailSearch);

            if (rate < g.Values[i].MinRate)
            {
                isNotViolatedMinRate = false;
                if (levelDetailSearch == 100)
                {
                    rateWeight = 0;
                    break;
                }
            }
            if (rate >= g.Values[i].MinRate * levelDetailSearch / 100.0)
            {
                currentRateSimilarities.Add(rate);
                rateWeight += rate * g.Values[i].Condition.Weight;
            }
        }

        if (currentRateSimilarities.Count != g.Values.Count)
        {
            return maxRate;
        }
        rate = rateWeight / g.Values.Sum(x => x.Condition.Weight);

        maxRate = rate;
        if (rate >= g.MinGroupRate && isNotViolatedMinRate)
        {
            typeSimilarity = 1;
            matchGroupValue = g;
            rateSimilarities = currentRateSimilarities;
            return rate;
        }
        if (levelDetailSearch > 0 && rate >= g.MinGroupRate * levelDetailSearch / 100.0)
        {
            typeSimilarity = 0;
        }

        return maxRate;
    }

    internal static double RateSimilarity(Item currentItem, Item mainItem, int currentGroupId, out GroupValue matchGroupValue, out List<double> rateSimilarities)
    {
        double maxRate = -1;
        rateSimilarities = null;//TODO think and check do we need currentRateSimilarities and can we assign rateSimilarities and matchGroupValue here
        matchGroupValue = null;


        var currentRateSimilarities = new List<double>();
        double rateWeight = 0;
        var g = currentItem.GroupValues[currentGroupId - 1];
        var g2 = mainItem.GroupValues[currentGroupId - 1];
        double rate;
        for (var i = 0; i < Math.Min(g.Values.Count, g2.Values.Count); i++)
        {
            rate = g.Values[i].Compare(g2.Values[i], 0);

            if (rate >= 0)
            {
                currentRateSimilarities.Add(rate);
                rateWeight += rate * g.Values[i].Condition.Weight;
            }
        }

        if (currentRateSimilarities.Count != g.Values.Count)
        {
            return maxRate;
        }
        rate = rateWeight / g.Values.Sum(x => x.Condition.Weight);

        if (rate > maxRate)
        {
            matchGroupValue = g;
            rateSimilarities = currentRateSimilarities;
            maxRate = rate;
        }

        return maxRate;
    }

    internal static double RateSimilarityMixed(Item currentItem, Item mainItem, int levelDetailSearch, out GroupValue matchGroupValue, out List<double> rateSimilarities, out int typeSimilarity)
    {
        double maxRate = -1;
        rateSimilarities = null;
        matchGroupValue = null;
        typeSimilarity = -1;

        for (var j = 0; j < currentItem.GroupValues.Count; j++)
        {
            var currentRateSimilarities = new List<double>();
            var isNotViolatedMinRate = true;
            double rateWeight = 0;
            var g = currentItem.GroupValues[j];
            var g2 = mainItem.GroupValues[j];
            double rate;
            for (var i = 0; i < Math.Min(g.Values.Count, g2.Values.Count); i++)
            {
                rate = g.Values[i].Compare(g2.Values[i], levelDetailSearch);

                if (rate < g.Values[i].MinRate)
                {
                    isNotViolatedMinRate = false;
                    if (levelDetailSearch == 100)
                    {
                        rateWeight = 0;
                        break;
                    }
                }
                if (rate >= g.Values[i].MinRate * levelDetailSearch / 100.0)
                {
                    currentRateSimilarities.Add(rate);
                    rateWeight += rate * g.Values[i].Condition.Weight;
                }
            }

            if (currentRateSimilarities.Count != g.Values.Count)
            {
                continue;
            }
            rate = rateWeight / g.Values.Sum(x => x.Condition.Weight);

            maxRate = rate;
            if (rate >= g.MinGroupRate && isNotViolatedMinRate)
            {
                typeSimilarity = 1;
                matchGroupValue = g;
                rateSimilarities = currentRateSimilarities;
                return rate;
            }
            if (levelDetailSearch > 0 && rate >= g.MinGroupRate * levelDetailSearch / 100.0)
            {
                typeSimilarity = 0;
            }
        }
        return maxRate;
    }

    internal static double RateSimilarityMixed(Item currentItem, Item mainItem, out GroupValue matchGroupValue, out List<double> rateSimilarities)
    {
        double maxRate = -1;
        rateSimilarities = null;
        matchGroupValue = null;

        for (var j = 0; j < currentItem.GroupValues.Count; j++)
        {
            var currentRateSimilarities = new List<double>();
            double rateWeight = 0;
            var g = currentItem.GroupValues[j];
            var g2 = mainItem.GroupValues[j];
            double rate;
            for (var i = 0; i < Math.Min(g.Values.Count, g2.Values.Count); i++)
            {
                rate = g.Values[i].Compare(g2.Values[i], 0);

                if (rate >= 0)
                {
                    currentRateSimilarities.Add(rate);
                    rateWeight += rate * g.Values[i].Condition.Weight;
                }
            }

            if (currentRateSimilarities.Count != g.Values.Count)
            {
                continue;
            }

            rate = rateWeight / g.Values.Sum(x => x.Condition.Weight);

            if (rate > maxRate)
            {
                matchGroupValue = g;
                rateSimilarities = currentRateSimilarities;
                maxRate = rate;
            }

        }
        return maxRate;
    }

    internal static double RateSimilarity(Item currentItem, Item mainItem)
    {
        double maxRate = -1;

        for (var j = 0; j < currentItem.GroupValues.Count; j++)
        {
            double rateWeight = 0;
            var g = currentItem.GroupValues[j];
            var g2 = mainItem.GroupValues[j];
            double rate;
            for (var i = 0; i < Math.Min(g.Values.Count, g2.Values.Count); i++)
            {
                rate = g.Values[i].Compare(g2.Values[i], 0);

                if (rate >= 0)
                {
                    rateWeight += rate * g.Values[i].Condition.Weight;
                }
            }

            rate = rateWeight / g.Values.Sum(x => x.Condition.Weight);

            if (rate > maxRate)
            {
                maxRate = rate;
            }

        }
        return maxRate;
    }

    #region Master record rules functions

    internal static Func<DataRow, bool> GetIsEmptyWhereCondition(MasterRecordRule rule)
    {
        if (rule.FieldType == typeof(string).ToString())
        {
            if (rule.Negate)
            {
                return row => !(row.Field<object>(rule.FieldName) == null || string.IsNullOrEmpty(row.Field<string>(rule.FieldName)));
            }

            return row => row.Field<object>(rule.FieldName) == null || string.IsNullOrEmpty(row.Field<string>(rule.FieldName));
        }
        if (rule.Negate)
        {
            return row => row.Field<object>(rule.FieldName) != null;
        }
        return row => row.Field<object>(rule.FieldName) == null;
    }

    internal static Func<DataRow, bool> GetContainsWhereCondition(MasterRecordRule rule)
    {
        if (rule.FieldType == typeof(string).ToString())
        {
            if (rule.Negate)
            {
                return row => !(row.Field<object>(rule.FieldName) != null && row.Field<string>(rule.FieldName).ToLower().Contains(rule.Value.ToLower()));
            }

            return row => row.Field<object>(rule.FieldName) != null && row.Field<string>(rule.FieldName).ToLower().Contains(rule.Value.ToLower());
        }

        return null;
    }

    internal static Func<DataRow, bool> GetIsEqualWhereCondition(MasterRecordRule rule)
    {
        if (rule.FieldType == typeof(DateTime).ToString())
        {
            if (rule.Negate)
            {
                return row => row.Field<object>(rule.FieldName) == null || (row.Field<object>(rule.FieldName) != null && row.Field<DateTime>(rule.FieldName) != Convert.ToDateTime(rule.Value));
            }
            return row => row.Field<object>(rule.FieldName) != null && row.Field<DateTime>(rule.FieldName) == Convert.ToDateTime(rule.Value);
        }

        if (rule.FieldType == typeof(int).ToString())
        {
            if (rule.Negate)
            {
                return row => row.Field<object>(rule.FieldName) == null || (row.Field<object>(rule.FieldName) != null && row.Field<int>(rule.FieldName) != Convert.ToInt32(rule.Value));
            }
            return row => row.Field<object>(rule.FieldName) != null && row.Field<int>(rule.FieldName) == Convert.ToInt32(rule.Value);
        }

        if (rule.FieldType == typeof(long).ToString())
        {
            if (rule.Negate)
            {
                return row => row.Field<object>(rule.FieldName) == null || (row.Field<object>(rule.FieldName) != null && row.Field<long>(rule.FieldName) != Convert.ToInt64(rule.Value));
            }
            return row => row.Field<object>(rule.FieldName) != null && row.Field<long>(rule.FieldName) == Convert.ToInt64(rule.Value);
        }

        if (rule.FieldType == typeof(double).ToString())
        {
            if (rule.Negate)
            {
                return row => row.Field<object>(rule.FieldName) == null || (row.Field<object>(rule.FieldName) != null && row.Field<double>(rule.FieldName) != Convert.ToDouble(rule.Value));
            }
            return row => row.Field<object>(rule.FieldName) != null && row.Field<double>(rule.FieldName) == Convert.ToDouble(rule.Value);
        }


        if (rule.Negate)
        {
            return row => row.Field<object>(rule.FieldName) == null || (row.Field<object>(rule.FieldName) != null && !string.Equals(row.Field<string>(rule.FieldName), rule.Value, StringComparison.CurrentCultureIgnoreCase));
        }

        return row => row.Field<object>(rule.FieldName) != null && string.Equals(row.Field<string>(rule.FieldName), rule.Value, StringComparison.CurrentCultureIgnoreCase);

    }

    internal static Func<DataRow, bool> GetGreaterThenWhereCondition(MasterRecordRule rule)
    {
        if (rule.FieldType == typeof(DateTime).ToString())
        {
            if (rule.Negate)
            {
                return row => row.Field<object>(rule.FieldName) != null && row.Field<DateTime>(rule.FieldName) < Convert.ToDateTime(rule.Value);
            }
            return row => row.Field<object>(rule.FieldName) != null && row.Field<DateTime>(rule.FieldName) >= Convert.ToDateTime(rule.Value);
        }

        if (rule.FieldType == typeof(int).ToString())
        {
            if (rule.Negate)
            {
                return row => (row.Field<object>(rule.FieldName) != null && row.Field<int>(rule.FieldName) < Convert.ToInt32(rule.Value));
            }
            return row => row.Field<object>(rule.FieldName) != null && row.Field<int>(rule.FieldName) > Convert.ToInt32(rule.Value);
        }

        if (rule.FieldType == typeof(long).ToString())
        {
            if (rule.Negate)
            {
                return row => (row.Field<object>(rule.FieldName) != null && row.Field<long>(rule.FieldName) < Convert.ToInt64(rule.Value));
            }
            return row => row.Field<object>(rule.FieldName) != null && row.Field<long>(rule.FieldName) > Convert.ToInt64(rule.Value);
        }

        if (rule.FieldType == typeof(double).ToString())
        {
            if (rule.Negate)
            {
                return row => (row.Field<object>(rule.FieldName) != null && row.Field<double>(rule.FieldName) < Convert.ToDouble(rule.Value));
            }
            return row => row.Field<object>(rule.FieldName) != null && row.Field<double>(rule.FieldName) > Convert.ToDouble(rule.Value);
        }

        return null;
    }

    internal static List<RuleDeterminedValueModel> GetMaxMin(DataTable data, bool isMin, string fieldName, string fieldType)
    {
        if (isMin)
        {
            if (fieldType == typeof(int).ToString())
                return GetMinData<int>(data, fieldName);

            if (fieldType == typeof(long).ToString())
                return GetMinData<long>(data, fieldName);

            if (fieldType == typeof(DateTime).ToString())
                return GetMinData<DateTime>(data, fieldName);

            if (fieldType == typeof(double).ToString())
                return GetMinData<double>(data, fieldName);

            if (fieldType == typeof(decimal).ToString())
                return GetMinData<decimal>(data, fieldName);
        }
        else
        {
            if (fieldType == typeof(int).ToString())
                return GetMaxData<int>(data, fieldName);

            if (fieldType == typeof(long).ToString())
                return GetMaxData<long>(data, fieldName);

            if (fieldType == typeof(DateTime).ToString())
                return GetMaxData<DateTime>(data, fieldName);

            if (fieldType == typeof(double).ToString())
                return GetMaxData<double>(data, fieldName);

            if (fieldType == typeof(decimal).ToString())
                return GetMaxData<decimal>(data, fieldName);
        }
        throw new ArgumentException("This type is denied for maximum/minimum rule");
    }

    private static List<RuleDeterminedValueModel> GetMaxData<T>(DataTable data, string fieldName)
    {
        return data.AsEnumerable()
            .Where(x => x.Field<object>(fieldName) != null)
            .GroupBy(x => x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID))
            .Select(x => new RuleDeterminedValueModel
            {
                GroupId = x.Key,
                Val = x.Max(s => s.Field<T>(fieldName))
            }).ToList();
    }

    private static List<RuleDeterminedValueModel> GetMinData<T>(DataTable data, string fieldName)
    {
        return data.AsEnumerable().Where(x => x.Field<object>(fieldName) != null)
            .GroupBy(x => x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID))
            .Select(x => new RuleDeterminedValueModel
            {
                GroupId = x.Key,
                Val = x.Min(s => s.Field<T>(fieldName))
            }).ToList();
    }

    internal static IEnumerable<DataRow> ApplyMaxMinRule(IEnumerable<DataRow> data, string fieldName, string fieldType, bool isNegate, List<RuleDeterminedValueModel> filter)
    {
        if (fieldType == typeof(int).ToString())
            return ApplyMaxMinRule<int>(data, fieldName, isNegate, filter);

        if (fieldType == typeof(long).ToString())
            return ApplyMaxMinRule<long>(data, fieldName, isNegate, filter);

        if (fieldType == typeof(DateTime).ToString())
            return ApplyMaxMinRule<DateTime>(data, fieldName, isNegate, filter);

        if (fieldType == typeof(double).ToString())
            return ApplyMaxMinRule<double>(data, fieldName, isNegate, filter);

        if (fieldType == typeof(decimal).ToString())
            return ApplyMaxMinRule<decimal>(data, fieldName, isNegate, filter);

        throw new ArgumentException("This type is denied for maximum/minimum rule");

    }

    private static IEnumerable<DataRow> ApplyMaxMinRule<T>(IEnumerable<DataRow> data, string fieldName, bool isNegate, List<RuleDeterminedValueModel> filter)
    {
        if (isNegate)
        {
            return data.Join(filter,
                    d => d.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                    f => f.GroupId, (d, f) => new { d, f })
                .Where(x =>
                    x.d.Field<object>(fieldName) == null ||
                    x.d.Field<T>(fieldName) != x.f.Val)
                .Select(s => s.d);
        }

        return data.Where(x => x.Field<object>(fieldName) != null)
            .Join(filter,
                d => new
                {
                    Gr = d.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                    val = d.Field<T>(fieldName)
                },
                f => new { Gr = f.GroupId, val = (T)f.Val }, (d, f) => new { d, f })
            .Select(s => s.d);
    }

    internal static string ApplyMaxMinRule(DataTable data, string fieldName, string fieldType, bool isNegate, List<RuleDeterminedValueModel> filter)
    {
        if (fieldType == typeof(int).ToString())
            return ApplyMaxMinRule<int>(data, fieldName, isNegate, filter);

        if (fieldType == typeof(long).ToString())
            return ApplyMaxMinRule<long>(data, fieldName, isNegate, filter);

        if (fieldType == typeof(DateTime).ToString())
            return ApplyMaxMinRule<DateTime>(data, fieldName, isNegate, filter);

        if (fieldType == typeof(double).ToString())
            return ApplyMaxMinRule<double>(data, fieldName, isNegate, filter);

        if (fieldType == typeof(decimal).ToString())
            return ApplyMaxMinRule<decimal>(data, fieldName, isNegate, filter);

        throw new ArgumentException("This type is denied for maximum/minimum rule");
    }

    private static string ApplyMaxMinRule<T>(DataTable data, string fieldName, bool isNegate, List<RuleDeterminedValueModel> filter)
    {
        var colName = ColumnHelper.GetDataTableUniqueColumnName(data, "WinPureRuleColumnMinMax" + fieldName);

        var col = new DataColumn(colName, typeof(bool)) { DefaultValue = false };
        data.Columns.Add(col);
        if (isNegate)
        {
            data.AsEnumerable()
                .GroupJoin(filter,
                    d => d.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                    f => f.GroupId, (d, f) => new { d, f })
                .SelectMany(sm => sm.f.DefaultIfEmpty(), (d, f) => new { d.d, f })
                .Where(x => x.f == null || x.d.Field<object>(fieldName) == null || x.d.Field<T>(fieldName) != x.f.Val)
                .Select(x => x.d)
                .ToList()
                .ForEach(x => x[colName] = true);
        }
        else
        {
            data.AsEnumerable()
                .Where(x => x.Field<object>(fieldName) != null)
                .Join(filter,
                    d => new
                    {
                        Gr = d.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                        val = d.Field<T>(fieldName)
                    },
                    f => new { Gr = f.GroupId, val = (T)f.Val }, (d, f) => new { d, f })
                .Select(s => s.d)
                .ToList()
                .ForEach(x => x[colName] = true);
        }
        return colName;
    }

    internal static IEnumerable<DataRow> ApplyLongShortRule(IEnumerable<DataRow> data, string fieldName, bool isNegate, List<RuleDeterminedValueModel> filter)
    {
        if (isNegate)
        {
            return data.Join(filter,
                    d => d.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                    f => f.GroupId, (d, f) => new { d, f })
                .Where(
                    x =>
                        x.d.Field<object>(fieldName) == null ||
                        x.d.Field<string>(fieldName).Length != x.f.Val)
                .Select(s => s.d);
        }

        return data.Where(x => x.Field<object>(fieldName) != null)
            .Join(filter,
                d => new
                {
                    Gr = d.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                    val = d.Field<string>(fieldName).Length
                },
                f => new { Gr = f.GroupId, val = (int)f.Val }, (d, f) => new { d, f })
            .Select(s => s.d);
    }

    internal static string ApplyLongShortRule(DataTable data, string fieldName, bool isNegate, List<RuleDeterminedValueModel> filter)
    {
        var colName = ColumnHelper.GetDataTableUniqueColumnName(data, "WinPureRuleColumnShortLong" + fieldName);

        var col = new DataColumn(colName, typeof(bool)) { DefaultValue = false };
        data.Columns.Add(col);
        if (isNegate)
        {
            data.AsEnumerable()
                .GroupJoin(filter,
                    d => d.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                    f => f.GroupId, (d, f) => new { d, f })
                .SelectMany(sm => sm.f.DefaultIfEmpty(), (d, f) => new { d.d, f })
                .Where(
                    x =>
                        x.f == null || x.d.Field<object>(fieldName) == null ||
                        x.d.Field<string>(fieldName).Length != (int)x.f.Val)
                .Select(x => x.d)
                .ToList()
                .ForEach(x => x[colName] = true);
        }
        else
        {
            data.AsEnumerable()
                .Where(x => x.Field<object>(fieldName) != null)
                .Join(filter,
                    d => new
                    {
                        Gr = d.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                        val = d.Field<string>(fieldName).Length
                    },
                    f => new { Gr = f.GroupId, val = (int)f.Val }, (d, f) => new { d, f })
                .Select(s => s.d)
                .ToList()
                .ForEach(x => x[colName] = true);
        }
        return colName;
    }

    internal static IEnumerable<DataRow> ApplyCommonRule(IEnumerable<DataRow> data, string fieldName, bool isNegate, List<RuleDeterminedValueModel> filter)
    {
        if (isNegate)
        {
            return data.Join(filter,
                    d => d.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                    f => f.GroupId, (d, f) => new { d, f })
                .Where(
                    x =>
                        x.d.Field<object>(fieldName) == null ||
                        x.d.Field<object>(fieldName) != x.f.Val)
                .Select(s => s.d);
        }

        return data.Where(x => x.Field<object>(fieldName) != null)
            .Join(filter,
                d => new
                {
                    Gr = d.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                    val = d.Field<object>(fieldName)
                },
                f => new
                {
                    Gr = f.GroupId,
                    val = (object)f.Val
                }, (d, f) => new { d, f })
            .Select(s => s.d).ToList();
    }

    internal static string ApplyCommonRule(DataTable data, string fieldName, bool isNegate, List<RuleDeterminedValueModel> filter)
    {
        var colName = ColumnHelper.GetDataTableUniqueColumnName(data, "WinPureRuleColumnShortLong" + fieldName);

        var col = new DataColumn(colName, typeof(bool)) { DefaultValue = false };
        data.Columns.Add(col);
        if (isNegate)
        {
            data.AsEnumerable()
                .GroupJoin(filter,
                    d => d.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                    f => f.GroupId, (d, f) => new { d, f })
                .SelectMany(sm => sm.f.DefaultIfEmpty(), (d, f) => new { d.d, f })
                .Where(
                    x =>
                        x.f == null || x.d.Field<object>(fieldName) == null ||
                        x.d.Field<object>(fieldName) != x.f.Val)
                .Select(x => x.d)
                .ToList()
                .ForEach(x => x[colName] = true);
        }
        else
        {
            data.AsEnumerable()
                .Where(x => x.Field<object>(fieldName) != null)
                .Join(filter,
                    d => new
                    {
                        Gr = d.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                        val = d.Field<object>(fieldName)
                    },
                    f => new { Gr = f.GroupId, val = (object)f.Val }, (d, f) => new { d, f })
                .Select(s => s.d)
                .ToList()
                .ForEach(x => x[colName] = true);
        }
        return colName;
    }
    #endregion
}