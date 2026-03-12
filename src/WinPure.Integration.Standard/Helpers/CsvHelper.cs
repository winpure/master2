using System.Globalization;
using WinPure.Common.Models;

namespace WinPure.Integration.Helpers;

internal static class CsvHelper
{
    internal static DataTable PreimportFromCsv(string text, TextImportExportOptions options, ImportedDataInfo importedDataInfo, int maxRowToImport, bool shouldNormalizeData = true)
    {
        var resultTable = new DataTable();
        var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        if (lines.Length == 1)
        {
            lines = text.Split(new[] { "\n" }, StringSplitOptions.None);
        }

        var cc = lines[0].Split(options.FieldDelimiter).ToList();
        if (shouldNormalizeData)
        {
            cc = NormalizeCsvImportedData(cc.ToArray(), options.FieldDelimiter.ToString()).ToList();
        }

        int columnCount = cc.Count;
        string[] columnHeaders;
        int currentRow = 0;
        if (options.FirstRowContainNames)
        {
            columnHeaders = lines[0].Split(options.FieldDelimiter);

            if (!string.IsNullOrEmpty(options.TextQualifier))
            {
                for (int j = 0; j < columnHeaders.Length; j++)
                {
                    if (columnHeaders[j].Length > 0 && columnHeaders[j][0].ToString() == options.TextQualifier)
                        columnHeaders[j] = columnHeaders[j].Substring(1, columnHeaders[j].Length - 2);
                }
            }
            else
            {
                if (shouldNormalizeData)
                {
                    columnHeaders = NormalizeCsvImportedData(columnHeaders, options.FieldDelimiter.ToString());
                }
            }

            columnCount = columnHeaders.Length;
            currentRow = 1;
        }
        else
        {
            columnHeaders = new string[columnCount];
            for (int i = 0; i < columnCount; i++)
            {
                columnHeaders[i] = "Column" + (i + 1);
            }
        }

        var data = new List<List<string>>();
        for (int i = currentRow; i < lines.Length && (maxRowToImport == 0 || i <= maxRowToImport); i++)
        {
            if (!string.IsNullOrEmpty(lines[i]))
            {
                var nextLine = lines[i].Split(options.FieldDelimiter).ToList();
                if (shouldNormalizeData)
                {
                    nextLine = NormalizeCsvImportedData(nextLine.ToArray(), options.FieldDelimiter.ToString()).ToList();
                }

                if (nextLine.Any(x => !string.IsNullOrEmpty(x)))
                {
                    data.Add(nextLine);
                }
            }
        }

        int testInt;
        double testDbl;
        DateTime testDt;

        for (int i = 0; i < columnCount; i++)
        {
            DataColumn dtColumn = null;
            var columnsValues = data.Select(x => x[i]).Where(x => !string.IsNullOrEmpty(x)).ToList();
            var fld = importedDataInfo.Fields.FirstOrDefault(x => x.DatabaseName == Common.Helpers.ColumnHelper.GetDataTableUniqueColumnName(resultTable, columnHeaders[i]));

            if (!columnsValues.Any() || (fld != null && fld.FieldType == typeof(string).ToString()))
            {
                dtColumn = new DataColumn(Common.Helpers.ColumnHelper.GetDataTableUniqueColumnName(resultTable, columnHeaders[i]), typeof(string));
            }
            //check for int
            else if (columnsValues.All(x => int.TryParse(x, out testInt)))
            {
                dtColumn = new DataColumn(Common.Helpers.ColumnHelper.GetDataTableUniqueColumnName(resultTable, columnHeaders[i]), typeof(int));
            }
            //check for doubl
            else if (columnsValues.Select(x => x.Replace(options.DecimalSymbol.ToString(), CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator))
                     .All(x => double.TryParse(x, out testDbl)))
            {
                dtColumn = new DataColumn(Common.Helpers.ColumnHelper.GetDataTableUniqueColumnName(resultTable, columnHeaders[i]), typeof(double));
            }
            //check for Datetime
            else if (columnsValues.All(x => DateTime.TryParse(x, out testDt)) ||
                     columnsValues.All(x => DateTime.TryParse(x, out testDt)))
            {
                dtColumn = new DataColumn(Common.Helpers.ColumnHelper.GetDataTableUniqueColumnName(resultTable, columnHeaders[i]), typeof(DateTime));
            }
            else
            {
                //another that is string
                dtColumn = new DataColumn(Common.Helpers.ColumnHelper.GetDataTableUniqueColumnName(resultTable, columnHeaders[i]), typeof(string));
            }

            resultTable.Columns.Add(dtColumn);
        }

        foreach (var row in data)
        {
            var r = resultTable.NewRow();
            for (int i = 0; i < columnCount; i++)
            {
                if (resultTable.Columns[i].DataType == typeof(System.Int32))
                {
                    if (int.TryParse(row[i], out testInt)) r[i] = testInt;
                }
                else if (resultTable.Columns[i].DataType == typeof(Double))
                {
                    if (double.TryParse(row[i], out testDbl)) r[i] = testDbl;
                }
                else if (resultTable.Columns[i].DataType == typeof(DateTime))
                {
                    if (DateTime.TryParse(row[i], out testDt))
                    {
                        r[i] = testDt;
                    }
                    else if (DateTime.TryParse(row[i], out testDt))
                    {
                        r[i] = testDt;
                    }
                }
                else
                {
                    r[i] = row[i].Trim('"');
                }
            }

            resultTable.Rows.Add(r);
        }

        return resultTable;
    }

    private static string[] NormalizeCsvImportedData(string[] data, string delimiterChar)
    {
        var max = data.Length;
        var lst = data.ToList();
        for (int i = 0; i < max; i++)
        {
            if (lst[i].Count(x => x == '"') % 2 != 0 && lst[i].StartsWith("\"") &&
                (!lst[i].EndsWith("\"") || lst[i] == "\""))
            {
                while (i + 1 < max)
                {
                    lst[i] += delimiterChar + lst[i + 1];
                    lst.RemoveAt(i + 1);
                    max--;
                    if (lst[i].EndsWith("\""))
                    {
                        break;
                    }

                    if (i + 1 >= max) return data;
                }
            }
        }

        return lst.ToArray();
    }
}