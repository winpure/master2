using WinPure.Common.Enums;

namespace WinPure.DataService.Senzing.Models.Pipeline;

public class DataBlock : BaseBlock
{
    public string SourceName { get; set; }
    public string Query { get; set; }
    public ExternalSourceTypes SourceType { get; set; }
    public string AdditionalParameters { get; set; }
}