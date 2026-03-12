using LumenWorks.Framework.IO.Csv;
using System.Globalization;
using System.IO;

namespace WinPure.Integration.Import;

internal class CsvImportProvider : FileImportProviderBase
{
    public CsvImportProvider() : base(ExternalSourceTypes.TextFile)
    {
        DisplayName = "CSV/TXT file";
    }

    protected override DataTable GetPreview(int rowToPreview)
    {
        string text = "";
        using (var csv = new CsvReader(new StreamReader(Options.FilePath, FileHelper.GetEncoding(Options.CodePage)), Options.FirstRowContainNames, Options.FieldDelimiter, Options.TextQualifier.Length > 0 ? Options.TextQualifier[0] : '\0', '\0', '\0', ValueTrimmingOptions.All))
        {
            if (Options.FirstRowContainNames)
            {
                var hdr = csv.GetFieldHeaders();
                for (int i = 0; i < csv.FieldCount; i++)
                {
                    var fld = hdr[i].Trim('"');
                    text += (fld.Contains(Options.FieldDelimiter)
                        ? "\"" + fld + "\""
                        : fld) + Options.FieldDelimiter;
                }
                text = text.Remove(text.Length - 1) + Environment.NewLine;
            }
            int r = 0;

            while (csv.ReadNextRecord() && r < rowToPreview)
            {
                string nl = "";
                for (int i = 0; i < csv.FieldCount; i++)
                {
                    var fld = csv[i].Trim('"');
                    nl += (fld.Contains(Options.FieldDelimiter) ? "\"" + fld + "\"" : fld) + Options.FieldDelimiter;
                }
                text += nl.Remove(nl.Length - 1) + Environment.NewLine;
                r++;
            }

            var previewTable = CsvHelper.PreimportFromCsv(text, Options, ImportedInfo, _MAX_ROW_TO_IMPORT);
            return previewTable;
        }
    }

    protected override DataTable GetData()
    {
        NotifyProgress(Resources.CAPTION_IO_READING_FILE, 5);
        int max;
        using (var csvFirst = new CsvReader(new StreamReader(Options.FilePath, FileHelper.GetEncoding(Options.CodePage)), Options.FirstRowContainNames, Options.FieldDelimiter))
        {
            max = csvFirst.Count();
        }

        if (max == 0)
        {
            return new DataTable();
        }

        var fileLines = "";
        var progressPercent = (max / 40);

        using (var csv = new CsvReader(new StreamReader(Options.FilePath, FileHelper.GetEncoding(Options.CodePage)), Options.FirstRowContainNames, Options.FieldDelimiter, Options.TextQualifier.Length > 0 ? Options.TextQualifier[0] : '\0', '\0', '\0', ValueTrimmingOptions.All))
        {
            if (Options.FirstRowContainNames)
            {
                for (int i = 0; i < csv.FieldCount; i++)
                {
                    var fld = csv.GetFieldHeaders()[i].Trim('"');
                    fileLines += (fld.Contains(Options.FieldDelimiter)
                        ? "\"" + fld + "\""
                        : fld) + Options.FieldDelimiter;
                }
                fileLines = fileLines.Remove(fileLines.Length - 1) + Environment.NewLine;
            }

            int currentRow = 0;
            NotifyProgress(Resources.CAPTION_IO_CREATE_TABLE_STRUCTURE, 10);

            DataTable importedDataTable = null;
            DateTime dateTimeValue;
            double doubleValue;
            long longValue;

            var skippedColumns = new List<int>();
            var decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            while (csv.ReadNextRecord() && (_MAX_ROW_TO_IMPORT == 0 || currentRow < _MAX_ROW_TO_IMPORT))
            {
                if (currentRow < 1000)
                {
                    string fileTextLine = "";
                    for (int i = 0; i < csv.FieldCount; i++)
                    {
                        var fld = csv[i].Trim('"');
                        fileTextLine += (fld.Contains(Options.FieldDelimiter) ? "\"" + fld + "\"" : fld) + Options.FieldDelimiter;
                    }
                    fileLines += fileTextLine.Remove(fileTextLine.Length - 1) + Environment.NewLine;
                    currentRow++;
                }
                else
                {
                    if (currentRow == 1000)
                    {
                        importedDataTable = CsvHelper.PreimportFromCsv(fileLines, Options, ImportedInfo, _MAX_ROW_TO_IMPORT);

                        if (importedDataTable == null)
                        {
                            throw new Exception(Resources.EXCEPTION_IO_DATATABLE_SHOULD_NOT_BE_NULL);
                        }

                        skippedColumns = RemoveSkippedField(importedDataTable, Options.Fields);

                        NotifyProgress(Resources.CAPTION_IO_IMPORTING_DATA, 20);
                    }
                    int offset = 0;
                    var newRow = importedDataTable.NewRow();
                    for (int i = 0; i < csv.FieldCount; i++)
                    {
                        if (skippedColumns.Contains(i))
                        {
                            offset++;
                            continue;
                        }
                        var columnIndex = i - offset;
                        if (importedDataTable.Columns[columnIndex].DataType == typeof(DateTime))
                        {
                            if (DateTime.TryParse(csv[i], out dateTimeValue))
                            {
                                newRow[columnIndex] = dateTimeValue;
                            }
                            else
                            {
                                newRow[columnIndex] = DBNull.Value;
                            }
                        }
                        else if (importedDataTable.Columns[columnIndex].DataType == typeof(decimal) ||
                                 importedDataTable.Columns[columnIndex].DataType == typeof(float) ||
                                 importedDataTable.Columns[columnIndex].DataType == typeof(double))
                        {
                            if (Double.TryParse(csv[i].Replace(Options.DecimalSymbol.ToString(), decimalSeparator), out doubleValue))
                            {
                                newRow[columnIndex] = doubleValue;
                            }
                            else
                            {
                                newRow[columnIndex] = DBNull.Value;
                            }
                        }
                        else if (importedDataTable.Columns[columnIndex].DataType == typeof(int) ||
                                 importedDataTable.Columns[columnIndex].DataType == typeof(long))
                        {
                            if (long.TryParse(csv[i], out longValue))
                            {
                                newRow[columnIndex] = longValue;
                            }
                            else
                            {
                                newRow[columnIndex] = DBNull.Value;
                            }
                        }
                        else
                        {
                            newRow[columnIndex] = csv[i].Trim('"');
                        }

                    }

                    importedDataTable.Rows.Add(newRow);
                    currentRow++;
                    if (currentRow % progressPercent == 0)
                    {
                        NotifyProgress(Resources.CAPTION_IO_IMPORTING_DATA, 20 + (currentRow / progressPercent));
                    }
                }
            }

            if (currentRow <= 1000) // so if file was short but really wide
            {
                importedDataTable = CsvHelper.PreimportFromCsv(fileLines, Options, ImportedInfo, _MAX_ROW_TO_IMPORT);
                RemoveSkippedField(importedDataTable, Options.Fields);
            }

            return importedDataTable;
        }
    }
}