using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using WinPure.Cleansing.Models.ContextData;
using WinPure.Common.Pipeline;

namespace WinPure.Cleansing.Pipeline;

internal class CleansingContext : IPipelineContext<ConcurrentBag<CleansingContextData>>
{
    public CleansingContext()
    {
        Exceptions = new List<PipelineExceptionData>();
    }

    public string ColumnName { get; set; }
    public Type ColumnType { get; set; }

    public ConcurrentBag<CleansingContextData> CleansingData { get; set; }

    public string MergeResultColumnName { get; set; }
    public Dictionary<string, string> PhoneCodes { get; set; }
    public IWpLogger Logger { get; set; }
    public CancellationToken Token { get; set; }
    public ParallelOptions ParallelOptions { get; set; }
    public List<PipelineExceptionData> Exceptions { get; set; }
    public Task<ConcurrentBag<CleansingContextData>> GetResult()
    {
        return Task.FromResult(CleansingData);
    }
}