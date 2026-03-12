namespace WinPure.Cleansing.Models.ContextData;

public class PipelineExceptionData
{
    public string Executor { get; set; }
    public Exception OriginalException { get; set; }
    public string OriginalValue { get; set; }
}