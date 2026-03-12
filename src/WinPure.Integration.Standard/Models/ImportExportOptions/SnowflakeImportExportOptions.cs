namespace WinPure.Integration.Models.ImportExportOptions;

public class SnowflakeImportExportOptions : BaseImportExportOptions
{
    public string Organization { get; set; }
    public string Account { get; set; }
    public string Schema { get; set; }
    public string Database { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public string TableName { get; set; }

    public string Region => $"{Organization}-{Account}";
}