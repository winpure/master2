using System.Threading.Tasks;
using WinPure.Cleansing.Models.ContextData;
using WinPure.Common.Pipeline;

namespace WinPure.Cleansing.Pipeline.CleanExecutors;

internal class ColumnMergerExecutor : IPipelineMiddleware<CleansingContext>
{
    private readonly List<ColumnMergeSetting> _settings;

    public ColumnMergerExecutor(List<ColumnMergeSetting> settings)
    {
        _settings = settings;
    }

    public async Task Execute(CleansingContext context, Func<CleansingContext, Task> next)
    {
        if (context.Token.IsCancellationRequested)
        {
            return;
        }

        var charBetweenValues = _settings.First().CharToInsertBetweenColumn;

        Parallel.ForEach(context.CleansingData, context.ParallelOptions, x =>
        {
            try
            {
                for (int t = 0; t < x.MergeValues.Count; t++)
                {
                    x.MergeResult += string.IsNullOrEmpty(x.MergeResult) ? x.MergeValues[t] : charBetweenValues + x.MergeValues[t];
                }
            }
            catch (Exception ex)
            {
                context.Logger.Error($"Error on {GetType().Name}. Value={x.Value} Message={ex.Message}", ex);
                context.Exceptions.Add(new PipelineExceptionData
                {
                    Executor = GetType().Name,
                    OriginalException = ex
                });
            }
        });

        await next(context);
    }
}