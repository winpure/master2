using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Data.CData.Excel;
using System.Globalization;
using System.IO;
using WinPure.Integration.Abstractions;

namespace WinPure.Integration.Import;

internal class ExcelImportProvider : ImportProviderBase, IDatabaseImportProvider
{
    private string _filePath;
    private ExcelImportExportOptions _options;
    private IWorkbook _currentWb;
    private ExcelConnection _cdataExcelConnection;
    private bool _isOldExcel;

    public ExcelImportProvider() : base(ExternalSourceTypes.Excel)
    {
        DisplayName = "MS Excel";
    }

    public override void Initialize(object parameters)
    {
        _options = parameters as ExcelImportExportOptions;
        if (_options != null)
        {
            _currentWb?.Close();
            _currentWb = null;
            _cdataExcelConnection?.Close();
            _cdataExcelConnection?.Dispose();
            _cdataExcelConnection = null;

            _filePath = _options.FilePath;
            _previewTable = null;
        }
    }

    protected override DataTable GetPreview(int rowToPreview)
    {
        _previewTable = null;
        InitializeWorkbook();
        ImportedInfo.DisplayName = _options.TableName;

        if (_isOldExcel)
        {
            if (_currentWb == null)
            {
                return null;
            }
            var sheet = _currentWb.GetSheet(_options.TableName);
            int columnCount = -1;

            int max = sheet.LastRowNum;
            if (max == 0)
            {
                return null;
            }

            if (_options.FirstRowContainNames)
            {
                var headers = sheet.GetRow(0);
                columnCount = headers.Count();
            }
            else
            {
                for (int i = 0; i < 500 && i < max; i++)
                {
                    var rw = sheet.GetRow(i);
                    if (rw == null)
                    {
                        continue;
                    }
                    var maxCol = rw.Cells.Max(x => x.ColumnIndex) + 1;
                    columnCount = Math.Max(columnCount, maxCol);
                }
            }

            var textOpt = new TextImportExportOptions
            {
                FirstRowContainNames = _options.FirstRowContainNames,
                FieldDelimiter = '\t',
                TextQualifier = "",
                DateDelimiter = '/',
                TimeDelimiter = ':',
                DateOrder = "DMY",
                DecimalSymbol = '.',
            };


            string text = "";
            for (int i = 0; i <= max && i < rowToPreview; i++)
            {
                var rw = sheet.GetRow(i);
                if (rw == null) continue;
                string nl = "";
                for (int j = 0; j < columnCount; j++)
                {
                    var cell = rw.GetCell(j);
                    if (cell == null)
                    {
                        nl += textOpt.FieldDelimiter;
                    }
                    else
                    {
                        var value = GetCellValue(cell, cell.CellType)
                            .ToString()
                            .Replace(Environment.NewLine, "");
                        nl += (value.Contains(textOpt.FieldDelimiter) ? "\"" + value + "\"" : value) +
                              textOpt.FieldDelimiter;
                    }
                }

                text += nl.Remove(nl.Length - 1) + Environment.NewLine;
            }

            _previewTable = CsvHelper.PreimportFromCsv(text, textOpt, ImportedInfo, _MAX_ROW_TO_IMPORT, false);
        }
        else
        {
            if (_cdataExcelConnection == null)
            {
                return null;
            }
            var cmd = _cdataExcelConnection.CreateCommand();
            cmd.CommandText = $"SELECT TOP {rowToPreview} * FROM [{_options.TableName}]";
            cmd.CommandType = CommandType.Text;
            var adapter = new ExcelDataAdapter(cmd);
            _previewTable = new DataTable("Preview");
            adapter.Fill(_previewTable);
            if (_previewTable.Columns.Count > 0 && _previewTable.Columns[0].ColumnName == "RowId")
            {
                _previewTable.Columns.Remove("RowId");
            }
        }

        return _previewTable;
    }

    protected override DataTable GetData()
    {
        InitializeWorkbook();

        if (_isOldExcel)
        {
            if (_currentWb == null)
            {
                return null;
            }
            var sheet = _currentWb.GetSheet(_options.TableName);
            int columnCount = -1;

            NotifyProgress(Resources.CAPTION_IO_IMPORTING_DATA, 10);

            int max = sheet.LastRowNum;
            if (max == 0)
            {
                return null;
            }
            var prcn = (max / 40);

            if (_options.FirstRowContainNames)
            {
                var headers = sheet.GetRow(0);
                columnCount = headers.Count();
            }
            else
            {
                for (int i = 0; i < 500 && i < max; i++)
                {
                    var rw = sheet.GetRow(i);
                    if (rw == null)
                    {
                        continue;
                    }
                    var maxCol = rw.Cells.Max(x => x.ColumnIndex) + 1;
                    columnCount = Math.Max(columnCount, maxCol);
                }
            }

            var textOpt = new TextImportExportOptions
            {
                FirstRowContainNames = _options.FirstRowContainNames,
                FieldDelimiter = '\t',
                TextQualifier = "",
                DateDelimiter = '/',
                TimeDelimiter = ':',
                DateOrder = "DMY",
                DecimalSymbol = '.',
            };

            string text = "";
            for (int i = 0; i <= max && i < 1000 && (_MAX_ROW_TO_IMPORT == 0 || i < _MAX_ROW_TO_IMPORT); i++)
            {
                var rw = sheet.GetRow(i);
                if (rw == null)
                {
                    continue;
                }
                string nl = "";
                for (int j = 0; j < columnCount; j++)
                {
                    var cell = rw.GetCell(j);
                    if (cell == null)
                    {
                        nl += textOpt.FieldDelimiter;
                    }
                    else
                    {
                        var value = GetCellValue(cell, cell.CellType)
                            .ToString()
                            .Replace(Environment.NewLine, "");
                        nl += (value.Contains(textOpt.FieldDelimiter) ? "\"" + value + "\"" : value) +
                              textOpt.FieldDelimiter;
                    }
                }

                text += nl.Remove(nl.Length - 1) + Environment.NewLine;
            }

            var dat = CsvHelper.PreimportFromCsv(text, textOpt, ImportedInfo, _MAX_ROW_TO_IMPORT, false);

            if (dat == null)
            {
                throw new Exception(Resources.EXCEPTION_IO_DATATABLE_SHOULD_NOT_BE_NULL);
            }

            var skippedColumns = RemoveSkippedField(dat, _options.Fields);

            NotifyProgress(Resources.CAPTION_IO_IMPORTING_DATA, 20);
            DateTime dateTimeValue;
            double doubleValue;
            long longValue;

            var decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            for (int i = 1000; i <= max && (_MAX_ROW_TO_IMPORT == 0 || i <= _MAX_ROW_TO_IMPORT); i++)
            {
                var rowIsEmpty = true;
                var rw = sheet.GetRow(i);
                if (rw == null)
                {
                    continue;
                }
                var offset = 0;
                var newRow = dat.NewRow();
                ImportedInfo.RowCount++;
                for (int j = 0; j < columnCount; j++)
                {
                    if (skippedColumns.Contains(j))
                    {
                        offset++;
                        continue;
                    }

                    var columnIndex = j - offset;
                    var cell = rw.GetCell(j);
                    if (cell == null)
                    {
                        newRow[columnIndex] = null;
                    }
                    else
                    {
                        var value = GetCellValue(cell, cell.CellType).ToString();
                        rowIsEmpty = rowIsEmpty && string.IsNullOrEmpty(value);
                        if (dat.Columns[columnIndex].DataType == typeof(DateTime))
                        {
                            if (DateTime.TryParse(value, out dateTimeValue))
                            {
                                newRow[columnIndex] = dateTimeValue;
                            }
                            else
                            {
                                newRow[columnIndex] = null;
                            }
                        }
                        else if (dat.Columns[columnIndex].DataType == typeof(decimal) ||
                                 dat.Columns[columnIndex].DataType == typeof(float) ||
                                 dat.Columns[columnIndex].DataType == typeof(double))
                        {
                            if (Double.TryParse(value.Replace(textOpt.DecimalSymbol.ToString(), decimalSeparator), out doubleValue))
                            {
                                newRow[columnIndex] = doubleValue;
                            }
                            else
                            {
                                newRow[columnIndex] = null;
                            }
                        }
                        else if (dat.Columns[columnIndex].DataType == typeof(int) ||
                                 dat.Columns[columnIndex].DataType == typeof(long))
                        {
                            if (Int64.TryParse(value, out longValue))
                            {
                                newRow[columnIndex] = longValue;
                            }
                            else
                            {
                                newRow[columnIndex] = null;
                            }
                        }
                        else
                        {
                            newRow[columnIndex] = value;
                        }
                    }

                }

                if (!rowIsEmpty)
                {
                    dat.Rows.Add(newRow);
                }

                if (i % prcn == 0)
                {
                    NotifyProgress(Resources.CAPTION_IO_IMPORTING_DATA, 20 + (i / prcn));
                }
            }

            return dat;
        }
        else
        {
            if (_options.Fields.All(x => x.FieldType == typeof(string).ToString()))
            {
                _cdataExcelConnection = new ExcelConnection(GetConnectionString(_filePath, _options.FirstRowContainNames, false));
                _cdataExcelConnection.Open();
            }

            if (_cdataExcelConnection == null)
            {
                return null;
            }
            var cmd = _cdataExcelConnection.CreateCommand();
            cmd.CommandText = $"SELECT {(_MAX_ROW_TO_IMPORT == 0 ? "" : "TOP " + _MAX_ROW_TO_IMPORT)} * FROM [{_options.TableName}]";
            cmd.CommandType = CommandType.Text;
            var adapter = new ExcelDataAdapter(cmd);
            var dat = new DataTable();

            NotifyProgress(Resources.CAPTION_IO_IMPORTING_DATA, 20);
            adapter.Fill(dat);
            RemoveSkippedField(dat, _options.Fields);
            if (_MAX_ROW_TO_IMPORT != 0 && dat.Rows.Count > _MAX_ROW_TO_IMPORT)
            {
                dat = dat.AsEnumerable().Take(_MAX_ROW_TO_IMPORT).CopyToDataTable();
            }

            return dat;
        }
    }

    public override DataTable ImportData()
    {
        try
        {
            _options.Fields = ImportedInfo.Fields;
            ImportedInfo.FileName = _options.TableName;
            var result = GetData();
            ImportedInfo.RowCount = result?.Rows.Count ?? 0;
            ImportedInfo.ImportParameters = JsonConvert.SerializeObject(_options);
            LimitedRecords = ImportedInfo.RowCount == _MAX_ROW_TO_IMPORT;
            NotifyProgress(Resources.CAPTION_IO_SAVING_DATA_TO_LOCAL_DB, 60);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Debug($"IMPORT FROM {DisplayName} ERROR", ex);
            NotifyException(Resources.EXCEPTION_IO_ERROR_ON_IMPORT, ex);
        }

        return null;
    }

    protected override DataTable GetReimportData(string parameterJson)
    {
        _options = JsonConvert.DeserializeObject<ExcelImportExportOptions>(parameterJson);
        ImportedInfo.Fields = _options.Fields;
        _filePath = _options.FilePath;
        var data = GetData();
        ImportedInfo.RowCount = data?.Rows.Count ?? 0;
        ImportedInfo.ImportParameters = JsonConvert.SerializeObject(_options);
        return data;
    }

    private object GetCellValue(ICell cell, CellType cType)
    {
        switch (cType)
        {
            case CellType.Unknown:
                return cell.StringCellValue;
            case CellType.Numeric:
                if (DateUtil.IsCellDateFormatted(cell))
                {
                    return cell.DateCellValue;
                }
                return cell.NumericCellValue;
            case CellType.String:
                return cell.StringCellValue;
            case CellType.Formula:
                return GetCellValue(cell, cell.CachedFormulaResultType);
            case CellType.Blank:
                return "";
            case CellType.Boolean:
                return cell.BooleanCellValue;
            case CellType.Error:
                return cell.ErrorCellValue;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [STAThread]
    private void InitializeWorkbook()
    {
        if (!File.Exists(_filePath) || _currentWb != null || _cdataExcelConnection != null)
        {
            return;
        }
        var fileExt = Path.GetExtension(_filePath);
        if (fileExt == ".xls")
        {
            _isOldExcel = true;
            using (FileStream file = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            {
                _currentWb = new HSSFWorkbook(file, true);
            }
        }
        if (fileExt == ".xlsx")
        {
            _isOldExcel = false;
            _cdataExcelConnection = new ExcelConnection(GetConnectionString(_filePath, _options.FirstRowContainNames, _options.AnalyzeDataType));
            _cdataExcelConnection.Open();
        }
    }

    private ExcelConnectionStringBuilder GetConnectionString(string filePath, bool firstRowContainNames, bool detectColumnType = true)
    {
        var connectionStringBuilder = new ExcelConnectionStringBuilder
        {
            ExcelFile = filePath,
            Header = firstRowContainNames ? "true" : "false",
            RTK = "5258524541415355525041413153554231454B473334333000000000000000000000000000000000485058585443455A00004542485841575942473437480000",
            TypeDetectionScheme = "None",
            AutoCache = false, 
            CacheMetadata = false,
        };

        if (detectColumnType)
        {
            connectionStringBuilder.RowScanDepth = "200";
            connectionStringBuilder.TypeDetectionScheme = "RowScan";
        }

        return connectionStringBuilder;
    }

    public bool CheckConnect()
    {
        return File.Exists(_filePath);
    }

    public List<string> GetDatabaseList()
    {
        return new List<string>();
    }

    [STAThread]
    public List<string> GetDatabaseTables()
    {
        var tables = new List<string>();

        try
        {
            InitializeWorkbook();
            if (_isOldExcel)
            {
                if (_currentWb != null)
                {
                    for (int i = 0; i < _currentWb.NumberOfSheets; i++)
                    {
                        tables.Add(_currentWb.GetSheetAt(i).SheetName);
                    }
                }
            }
            else
            {
                if (_cdataExcelConnection != null)
                {
                    DataTable schema = _cdataExcelConnection.GetSchema("TABLES", null);
                    DataRow[] rTables = schema.Select("TABLE_TYPE='BASE TABLE'");
                    var index = schema.Columns.IndexOf("TABLE_NAME");
                    for (int i = 0; i < rTables.Length; i++)
                    {
                        tables.Add((string)rTables[i].ItemArray[index]);
                    }
                }
            }
            return tables;
        }
        catch (OutOfMemoryException m_ex)
        {
            NotifyException(Resources.EXCEPTION_OUTOFMEMORY, m_ex);
        }
        catch (Exception ex)
        {
            tables = new List<string>();
            NotifyException(Resources.EXCEPTION_IO_CANNOT_OPEN_EXCEL_SHEET, ex);
        }
        return tables;
    }

    public (bool isValid, string error) ValidateSql()
    {
        return (false, "SQL is not possible!");
    }

    public void ExecuteSql(string script)
    {
        throw new NotImplementedException();
    }
}