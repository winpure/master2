using WinPure.Common.Enums;

namespace WinPure.DataService.Senzing.Models;

public class EntityResolutionConfiguration
{
    public string TableName { get; set; }
    public string TableDisplayName { get; set; }
    public int RowCount { get; set; }
    public bool IsMainTable { get; set; }
    public string Query { get; set; }
    public ExternalSourceTypes Source { get; set; }
    public string AdditionalParameters { get; set; }
    public string UpdateQuery { get; set; }
    public List<MapError> Errors { get; set; }
    public List<EntityResolutionRowConfiguration> Rows { get; set; }
}