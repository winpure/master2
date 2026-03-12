namespace WinPure.Configuration.Models.Database;

public class ConnectionSettingEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Settings { get; set; }
    public ExternalSourceTypes SourceType { get; set; }
}