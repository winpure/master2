namespace WinPure.Integration.Models.ImportExportOptions;

public class ZohoImportOptions : BaseImportExportOptions
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string CallbackUrl { get; set; }
    public string TableName { get; set; }
}