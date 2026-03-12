using System.Collections.Concurrent;
using System.Globalization;
using System.Text.RegularExpressions;
using WinPure.Cleansing.Models.Statistic;

namespace WinPure.Cleansing.Helpers;

internal static class StatisticHelper
{
    internal static ColumnStatistic CalculateStatisticForColumn(List<string> values, string columnName, Type columnType, DataField dataField, int totalCount, int order)
    {
        var columnStatistic = new ColumnStatistic
        {
            ColumnName = columnName,
            Order = order,
            ColumnType = columnType.ToString().Replace("System.", "").Replace("Int64", "Integer")
                .Replace("Int32", "Integer").Replace("Long", "Integer"),
            CountNull = values.Count(string.IsNullOrEmpty),
            CountNotNull = values.Count(x => !string.IsNullOrEmpty(x)),
            Distinct = values.Distinct().Count()
        };

        var cmn = values.GroupBy(x => x)
            .Select(x => new { val = x.Key, Cnt = x.Count() })
            .OrderByDescending(x => x.Cnt)
            .FirstOrDefault();

        var regexPattern = string.IsNullOrEmpty(dataField?.Pattern)
            ? null
            : new Regex(dataField.Pattern, RegexOptions.Compiled);

        if (cmn != null)
        {
            columnStatistic.MostCommon = cmn.val;
            columnStatistic.MostCommonCount = cmn.Cnt;
        }

        var notNullRows = values.Where(x => !string.IsNullOrEmpty(x)).ToList();

        if (columnType == typeof(string))
        {
            columnStatistic.LeadingSpaces = notNullRows.Count(x => x.StartsWith(" "));

            columnStatistic.TrailingSpaces = notNullRows.Count(x => x.EndsWith(" "));

            columnStatistic.MultipleSpaces = notNullRows.Count(x => Regex.IsMatch(x, @"[ ]{2,}"));

            columnStatistic.WithSpaces = notNullRows.Count(x => x.Contains(" "));

            columnStatistic.Letters = notNullRows.Count(x => Regex.IsMatch(x, @"[a-zA-Z]+"));

            columnStatistic.Punctuation = notNullRows.Count(x => Regex.IsMatch(x, @"\p{P}"));

            columnStatistic.TabChar = notNullRows.Count(x => x.Contains("\t"));

            columnStatistic.Dots = notNullRows.Count(x => x.Contains("."));

            columnStatistic.Commas = notNullRows.Count(x => x.Contains(","));

            columnStatistic.Apostrophes = notNullRows.Count(x => x.Contains("'"));

            columnStatistic.Hyphens = notNullRows.Count(x => x.Contains("-"));

            columnStatistic.NotPrintable = notNullRows.Count(x => x.Any(char.IsControl));

            columnStatistic.NewLineChar = notNullRows.Count(x => x.Contains(Environment.NewLine) || x.Contains("\r\n") || x.Contains("\r"));

            var textInfo = new CultureInfo("en-US", false).TextInfo;
            columnStatistic.ProperCase = notNullRows.Count(x => x.Equals(textInfo.ToTitleCase(textInfo.ToLower(x)), StringComparison.CurrentCulture));

            columnStatistic.UpperOnly = notNullRows.Count(x => x.Equals(x.ToUpper(), StringComparison.InvariantCulture));

            columnStatistic.LowerOnly = notNullRows.Count(x => x.Equals(x.ToLower(), StringComparison.InvariantCulture));

            columnStatistic.MixedCase = notNullRows.Count(x => x.ToLower() != x && x.ToUpper() != x);

            if (regexPattern != null)
            {
                columnStatistic.MatchPattern = notNullRows.Count(x => regexPattern.IsMatch(x));
                columnStatistic.UnmatchPattern = totalCount - columnStatistic.MatchPattern;
            }
            else
            {
                columnStatistic.MatchPattern = 0;
                columnStatistic.UnmatchPattern = 0;
            }

            columnStatistic.MaxNumber = 0;
            columnStatistic.MinNumber = 0;
        }

        if (columnType != typeof(DateTime))
        {
            columnStatistic.Numbers = notNullRows.Count(x => Regex.IsMatch(x, @"[0-9]"));
        }

        foreach (var val in notNullRows)
        {

            if (double.TryParse(val, out var d))
            {
                if (d > columnStatistic.MaxNumber)
                {
                    columnStatistic.MaxNumber = d;
                }

                if (d < columnStatistic.MinNumber)
                {
                    columnStatistic.MinNumber = d;
                }
            }

            if (columnType == typeof(string))
            {
                var wrdCount = Regex.Matches(val, @"[\S]+").Count;
                columnStatistic.TotalWords += wrdCount;
                if (wrdCount > columnStatistic.MaxWords)
                {
                    columnStatistic.MaxWords = wrdCount;
                }

                if (val.Length > columnStatistic.MaxLength)
                {
                    columnStatistic.MaxLength = val.Length;
                }
                columnStatistic.TotalLength += val.Length;
            }
        }


        if (columnType == typeof(string))
        {
            columnStatistic.AvgWords = (double)columnStatistic.TotalWords / (totalCount == 0 ? 1 : totalCount);
            columnStatistic.AvgLength = (double)columnStatistic.TotalLength / (totalCount == 0 ? 1 : totalCount);
        }

        return columnStatistic;
    }

    private static DataTable GetStatisticTableSchema()
    {
        var retTable = new DataTable();
        retTable.Columns.Add(new DataColumn("FieldName"));
        retTable.Columns.Add(new DataColumn("FieldType"));

        retTable.Columns.Add(new DataColumn("Filled", typeof(int)));
        retTable.Columns.Add(new DataColumn("Empty", typeof(int)));
        retTable.Columns.Add(new DataColumn("Distinct", typeof(int)));
        retTable.Columns.Add(new DataColumn("TrailingSpaces", typeof(int)));
        retTable.Columns.Add(new DataColumn("Commas", typeof(int)));
        retTable.Columns.Add(new DataColumn("Dots", typeof(int)));
        retTable.Columns.Add(new DataColumn("Hyphens", typeof(int)));
        retTable.Columns.Add(new DataColumn("Apostrophes", typeof(int)));
        retTable.Columns.Add(new DataColumn("LeadingSpaces", typeof(int)));
        retTable.Columns.Add(new DataColumn("Letters", typeof(int)));
        retTable.Columns.Add(new DataColumn("Numbers", typeof(int)));
        retTable.Columns.Add(new DataColumn("NonPrintable", typeof(int)));
        retTable.Columns.Add(new DataColumn("WithSpaces", typeof(int)));
        retTable.Columns.Add(new DataColumn("MultipleSpaces", typeof(int)));
        retTable.Columns.Add(new DataColumn("TabChar", typeof(int)));
        retTable.Columns.Add(new DataColumn("NewLineChar", typeof(int)));

        retTable.Columns.Add(new DataColumn("Punctuation", typeof(int)));
        retTable.Columns.Add(new DataColumn("UpperOnly", typeof(int)));
        retTable.Columns.Add(new DataColumn("LowerOnly", typeof(int)));
        retTable.Columns.Add(new DataColumn("ProperCase", typeof(int)));
        retTable.Columns.Add(new DataColumn("MixedCase", typeof(int)));

        retTable.Columns.Add(new DataColumn("MatchPattern", typeof(int)));
        retTable.Columns.Add(new DataColumn("UnmatchPattern", typeof(int)));
            
        retTable.Columns.Add(new DataColumn("MostCommon", typeof(string)));
        retTable.Columns.Add(new DataColumn("MostCommonCount", typeof(string)));
        retTable.Columns.Add(new DataColumn("MinNumber", typeof(string)));
        retTable.Columns.Add(new DataColumn("MaxNumber", typeof(string)));
        retTable.Columns.Add(new DataColumn("MaxWords", typeof(long)));
        retTable.Columns.Add(new DataColumn("AvgWords", typeof(string)));
        retTable.Columns.Add(new DataColumn("MaxLength", typeof(long)));
        retTable.Columns.Add(new DataColumn("AvgLength", typeof(string)));

        //retTable.Columns.Add(new DataColumn("_totalCount", typeof(Int64)));
        //retTable.Columns.Add(new DataColumn("_totalWords", typeof(Int64)));
        //retTable.Columns.Add(new DataColumn("_totalLength", typeof(Int64)));

        return retTable;
    }

    internal static DataTable GetStatisticTableResult(ConcurrentBag<ColumnStatistic> statistics, int totalCount)
    {
        var culture = CultureInfo.GetCultureInfo("EN-Us");

        var retTable = GetStatisticTableSchema();

        if (!statistics.Any())
        {
            //logger.Information("NO statistic result!!!");
            return retTable;
        }

        //logger.Information("Start statistic calculation processing.");
        //logger.Information("StatResult.Count="+statResult.Count);
        foreach (var stat in statistics.OrderBy(x => x.Order))
        {
            var newRow = retTable.NewRow();

            newRow["FieldName"] = stat.ColumnName;
            newRow["FieldType"] = stat.ColumnType;

            newRow["Filled"] = stat.CountNotNull;
            newRow["Empty"] = stat.CountNull;
            newRow["LeadingSpaces"] = stat.LeadingSpaces;
            newRow["TrailingSpaces"] = stat.TrailingSpaces;
            newRow["MultipleSpaces"] = stat.MultipleSpaces;
            newRow["WithSpaces"] = stat.WithSpaces;
            newRow["Letters"] = stat.Letters;
            newRow["Numbers"] = stat.Numbers;
            newRow["Punctuation"] = stat.Punctuation;
            newRow["UpperOnly"] = stat.UpperOnly;
            newRow["LowerOnly"] = stat.LowerOnly;
            newRow["ProperCase"] = stat.ProperCase;
            newRow["MixedCase"] = stat.MixedCase;
            newRow["TabChar"] = stat.TabChar;
            newRow["NewLineChar"] = stat.NewLineChar;

            newRow["Commas"] = stat.Commas;
            newRow["Dots"] = stat.Dots;
            newRow["Apostrophes"] = stat.Apostrophes;
            newRow["Hyphens"] = stat.Hyphens;
            newRow["NonPrintable"] = stat.NotPrintable;

            newRow["MaxWords"] = stat.MaxWords;
            newRow["MaxLength"] = stat.MaxLength;

            newRow["MatchPattern"] = stat.MatchPattern;
            newRow["UnmatchPattern"] = stat.UnmatchPattern;

            newRow["MinNumber"] = stat.MinNumber.ToString();
            newRow["MaxNumber"] = stat.MaxNumber.ToString();
            newRow["Distinct"] = stat.Distinct;

            var avgWords = (double)stat.TotalWords / totalCount;
            var avgLength = (double)stat.TotalLength / totalCount;

            newRow["AvgWords"] = string.Format(culture, "{0:0.0}", avgWords);
            newRow["AvgLength"] = string.Format(culture, "{0:0.0}", avgLength);

            newRow["MostCommon"] = stat.MostCommon;
            newRow["MostCommonCount"] = stat.MostCommonCount;


            retTable.Rows.Add(newRow);
        }
        //logger.Information("Return statistic datatable");
        return retTable;
    }
}