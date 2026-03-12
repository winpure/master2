namespace WinPure.Integration.Models.ImportExportOptions;

[Serializable]
public class SqlImportExportOptions : BaseImportExportOptions
{
    public string ServerAddress { get; set; }
    public int Port { get; set; }
    public string DatabaseName { get; set; }
    public string TableName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool IntegrateSecurity { get; set; }
    public string DatabaseFile { get; set; } //connection
    public bool UseSsh { get; set; }
    public string SshServer { get; set; }
    public string SshLogin { get; set; }
    public string SshPassword { get; set; }
    public bool UseSsl { get; set; }
    public string SqlQuery { get; set; }
}