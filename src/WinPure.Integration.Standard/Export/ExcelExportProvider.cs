using Newtonsoft.Json;
using System.IO;
using WinPure.Integration.Abstractions;
using Excel = Microsoft.Office.Interop.Excel;

namespace WinPure.Integration.Export;

internal class ExcelExportProvider : ExportProviderBase
{
    private ExcelImportExportOptions _options;
    private Excel.Application _excel;
    private Excel.Workbook _workbook;

    public ExcelExportProvider() : base(ExternalSourceTypes.Excel)
    {
        DisplayName = "MS Excel";
    }

    public override void Initialize(object parameters)
    {
        _options = parameters as ExcelImportExportOptions;
        if (_options == null)
        {
            NotifyException(Resources.EXCEPTION_IO_PARAMETERS_WRONG_FORMAT, null);
        }
        else
        {
            ExportParameters = JsonConvert.SerializeObject(_options);
        }
    }

    public override void Export(DataTable tbl)
    {
        try
        {
            if (!CheckExportData(tbl) || string.IsNullOrWhiteSpace(_options.FilePath))
            {
                return;
            }

            NotifyProgress(Resources.CAPTION_IO_DATA_EXPORTING, 10);
            var ext = Path.GetExtension(_options.FilePath);
            if (ext == ".xls" && tbl.Rows.Count > 65536)
            {
                NotifyException(Resources.EXCEPTION_IO_TOO_MUCH_DATA_FOR_OLD_EXCEL, null);
                return;
            }
            FileHelper.SafeDeleteFile(_options.FilePath);

            var tmpFile = Path.GetTempFileName();
            var opt = new TextImportExportOptions
            {
                TextQualifier = "",
                FirstRowContainNames = _options.FirstRowContainNames,
                FieldDelimiter = '\t',
                DateDelimiter = '/',
                DateOrder = "Default",
                DecimalSymbol = '.',
                TimeDelimiter = ':',
                CodePage = 1252,
                FilePath = tmpFile
            };

            if (tbl.Columns[0].ColumnName.ToLower() == "id")
            {
                tbl.Columns[0].SetOrdinal(1);
            }

            var exportCsv = new CsvExportProvider();
            exportCsv.OnException += NotifyException;
            exportCsv.OnProgressUpdate += ExportCsv_OnProgeressUpdate;
            exportCsv.Initialize(opt);
            exportCsv.Export(tbl);
            NotifyProgress(Resources.CAPTION_IO_CONVERT_FILE_TO_EXCEL, 90);

            _excel = new Excel.Application();
            _workbook = _excel.Workbooks.Open(tmpFile);

            switch (ext)
            {
                case ".xls":
                    _workbook.SaveAs(_options.FilePath, Excel.XlFileFormat.xlExcel8);
                    break;
                case ".xlsx":
                    _workbook.SaveAs(_options.FilePath, Excel.XlFileFormat.xlOpenXMLWorkbook);
                    break;
                case ".xlsb":
                    _workbook.SaveAs(_options.FilePath, Excel.XlFileFormat.xlExcel12);
                    break;
            }

            FileHelper.SafeDeleteFile(tmpFile);

            NotifyProgress(Resources.CAPTION_IO_EXPORT_COMPLETE, 100);

        }
        catch (Exception ex)
        {
            _logger.Debug("EXPORT TO EXCEL ERROR", ex);
            NotifyException(string.Format(Resources.EXCEPTION_IO_DATA_COULD_NOT_BE_EXPORTED_TO, "Excel"), ex);
        }
        finally
        {
            _workbook?.Close();
            _excel?.Quit();
        }
    }

    private void ExportCsv_OnProgeressUpdate(string message, int percent)
    {
        if (percent != 100)
        {
            NotifyProgress(message, 10 + Convert.ToInt32(percent * 0.8));
        }
    }
}