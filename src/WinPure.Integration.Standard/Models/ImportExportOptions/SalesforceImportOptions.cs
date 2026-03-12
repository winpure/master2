namespace WinPure.Integration.Models.ImportExportOptions;

[Serializable]
public class SalesforceImportOptions : BaseImportExportOptions
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Token { get; set; }
    public string TableName { get; set; }
    public bool UseSandbox { get; set; }
}