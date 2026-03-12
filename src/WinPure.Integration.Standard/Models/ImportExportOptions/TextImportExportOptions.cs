namespace WinPure.Integration.Models.ImportExportOptions;

[Serializable]
public class TextImportExportOptions : BaseImportExportOptions
{
    public string DateOrder { get; set; }
    public char DateDelimiter { get; set; }
    public char TimeDelimiter { get; set; }
    public char DecimalSymbol { get; set; }
    public bool AddTime { get; set; }
    public char FieldDelimiter { get; set; }
    public string TextQualifier { get; set; }
    public bool FirstRowContainNames { get; set; }
    public int CodePage { get; set; }
    public string FilePath { get; set; } // connection
}