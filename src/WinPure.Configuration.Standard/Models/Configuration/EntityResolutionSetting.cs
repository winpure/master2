using WinPure.Configuration.Enums;

namespace WinPure.Configuration.Models.Configuration;

public class EntityResolutionSetting
{
    public EntityResolutionDatabase Database { get; set; }
    public string ConnectionString { get; set; }
    public string DataFolder { get; set; }
    public int MaxDegreeOfParallelism { get; set; }
    public bool EnableDebugLogs { get; set; }
}