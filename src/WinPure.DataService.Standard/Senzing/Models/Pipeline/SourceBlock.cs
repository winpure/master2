namespace WinPure.DataService.Senzing.Models.Pipeline;

public class SourceBlock : BaseBlock
{
    public EntityResolutionConfiguration Configuration { get; set; }
    public int RecordToProcess { get; set; }
}