namespace WinPure.Configuration.Models.Configuration;

public class ConnectionSettings
{
    public int Id { get; set; }
    public string Name { get; set; }
    public object Settings { get; set; }
    public ExternalSourceTypes SourceType { get; set; }
}