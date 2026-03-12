namespace WinPure.DataService.Senzing.Models;

public class EntityResolutionRowConfiguration
{
    public string ColumnName { get; set; }
    public string FieldType { get; set; }
    public string Label { get; set; }
    public bool IsIgnore { get; set; }
    public bool IsInclude { get; set; }
}