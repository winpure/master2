using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using WinPure.Integration.Abstractions;


namespace WinPure.Integration.Export;

internal class ExcelExportUniversalProvider : ExportProviderBase
{
    private ExcelImportExportOptions _options;

    public ExcelExportUniversalProvider() : base(ExternalSourceTypes.Excel)
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

            IWorkbook workbook;
            var one_prct = tbl.Rows.Count > 100 ? tbl.Rows.Count / 100 : tbl.Rows.Count;
            short curr_percent = 0;
            using (var excelFile = new FileStream(_options.FilePath, FileMode.Create, FileAccess.ReadWrite))
            {

                if (ext == ".xlsx")
                {
                    workbook = new XSSFWorkbook();
                }
                else if (ext == ".xls")
                {
                    workbook = new HSSFWorkbook();
                }
                else
                {
                    throw new Exception("This format is not supported");
                }

                ISheet sheet1 = workbook.CreateSheet(tbl.TableName);

                //make a header row  
                IRow row1 = sheet1.CreateRow(0);

                for (int j = 0; j < tbl.Columns.Count; j++)
                {

                    ICell cell = row1.CreateCell(j);

                    String columnName = tbl.Columns[j].ToString();
                    cell.SetCellValue(columnName);
                }

                //loops through data  
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    IRow row = sheet1.CreateRow(i + 1);
                    for (int j = 0; j < tbl.Columns.Count; j++)
                    {

                        ICell cell = row.CreateCell(j);
                        String columnName = tbl.Columns[j].ToString();
                        cell.SetCellValue(tbl.Rows[i][columnName].ToString());
                    }
                    if (i % one_prct == 0)
                    {
                        curr_percent++;
                        NotifyProgress(Resources.CAPTION_IO_DATA_EXPORTING, curr_percent);
                    }
                }
                workbook.Write(excelFile);
                excelFile.Close();
            }

            NotifyProgress(Resources.CAPTION_IO_EXPORT_COMPLETE, 100);
        }
        catch (Exception ex)
        {
            _logger.Debug("EXPORT TO EXCEL ERROR", ex);
            NotifyException(string.Format(Resources.EXCEPTION_IO_DATA_COULD_NOT_BE_EXPORTED_TO, "Excel"), ex);
        }
    }
}