namespace WinPure.Integration.Models.ImportExportOptions;

[Serializable]
public class ExcelImportExportOptions : BaseImportExportOptions
{
    public string TableName { get; set; }
    public bool FirstRowContainNames { get; set; }
    public string FilePath { get; set; } //connection
    public bool AnalyzeDataType { get; set; }
    public bool ExportWithNpoi { get; set; }
}