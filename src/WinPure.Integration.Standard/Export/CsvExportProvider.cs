using System.IO;

namespace WinPure.Integration.Export;

internal class CsvExportProvider : FileExportProviderBase
{
    public CsvExportProvider() : base(ExternalSourceTypes.TextFile)
    {
        DisplayName = "CSV/TXT file";
    }

    public override void Export(DataTable data)
    {
        try
        {
            if (!CheckExportData(data) || !CheckFilePath())
            {
                return;
            }

            string fileLine = "";

            FileHelper.SafeDeleteFile(Options.FilePath);
            using (var fileStream = new FileStream(Options.FilePath, FileMode.CreateNew))
            {
                using (var streamWriter = new StreamWriter(fileStream, FileHelper.GetEncoding(Options.CodePage)))
                {
                    if (Options.FirstRowContainNames)
                    {
                        for (int i = 0; i < data.Columns.Count; i++)
                        {
                            fileLine += (fileLine == "")
                                ? $"{Options.TextQualifier}{data.Columns[i].ColumnName}{Options.TextQualifier}"
                                : $"{Options.FieldDelimiter}{Options.TextQualifier}{data.Columns[i].ColumnName}{Options.TextQualifier}";
                        }
                        streamWriter.WriteLine(fileLine);
                        fileLine = "";
                    }
                    var dateFormat = FormatHelper.GetDateStringFormat(Options);
                    var onePercentOfExport = data.Rows.Count > 100 ? data.Rows.Count / 100 : data.Rows.Count;
                    short currentPercentOfExport = 0;
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        var arr = data.Rows[i].ItemArray;
                        for (int j = 0; j < arr.Length; j++)
                        {
                            var value = string.Empty;
                            if (arr[j] is DateTime)
                            {
                                value = ((DateTime)arr[j]).ToString(dateFormat);
                                if (!value.Contains(Options.DateDelimiter.ToString()))
                                {
                                    value = value.Replace('.', Options.DateDelimiter);
                                }
                            }
                            else if (arr[j] is decimal || arr[j] is double || arr[j] is float)
                            {
                                value = Convert.ToDecimal(arr[j]).ToString()
                                    .Replace(',', Options.DecimalSymbol)
                                    .Replace('.', Options.DecimalSymbol);
                            }
                            else
                            {
                                value = ((arr[j].ToString().Contains(Options.FieldDelimiter.ToString())) ? "\"" + arr[j] + "\"" : arr[j].ToString());
                            }

                            if (!string.IsNullOrEmpty(Options.TextQualifier) && value.Contains(Options.TextQualifier))
                            {
                                value = value.Replace(Options.TextQualifier, $"{Options.TextQualifier}{Options.TextQualifier}");
                            }
                            fileLine += $"{Options.TextQualifier}{value}{Options.TextQualifier}{Options.FieldDelimiter}";
                        }
                        fileLine = fileLine.Substring(0, fileLine.Length - 1);
                        streamWriter.WriteLine(fileLine);
                        fileLine = "";
                        if (i % onePercentOfExport == 0)
                        {
                            currentPercentOfExport++;
                            NotifyProgress(Resources.CAPTION_IO_DATA_EXPORTING, currentPercentOfExport);
                        }
                    }
                }
            }
            NotifyProgress(Resources.CAPTION_IO_EXPORT_COMPLETE, 100);

        }
        catch (Exception ex)
        {
            _logger.Debug($"EXPORT TO {DisplayName} ERROR", ex);
            NotifyException(string.Format(Resources.EXCEPTION_IO_DATA_COULD_NOT_BE_EXPORTED_TO, "CSV file"), ex);
        }
    }

    protected override string GetFileContent(DataTable data)
    {
        throw new NotImplementedException();
    }
}