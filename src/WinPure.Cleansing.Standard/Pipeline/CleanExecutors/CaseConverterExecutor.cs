using System.Globalization;
using System.Threading.Tasks;
using WinPure.Cleansing.Models.ContextData;
using WinPure.Common.Pipeline;

namespace WinPure.Cleansing.Pipeline.CleanExecutors;

internal class CaseConverterExecutor : IPipelineMiddleware<CleansingContext>
{
    private readonly CaseConverterSetting _setting;

    public CaseConverterExecutor(CaseConverterSetting setting)
    {
        _setting = setting;
    }

    public async Task Execute(CleansingContext context, Func<CleansingContext, Task> next)
    {
        if (context.Token.IsCancellationRequested)
        {
            return;
        }

        Parallel.ForEach(context.CleansingData.Where(x => !string.IsNullOrWhiteSpace(x.Value)), context.ParallelOptions, x =>
        {
            try
            {
                var val = x.Value;

                if (_setting.ToLowerCase)
                {
                    val = val.ToLower();
                }

                if (_setting.ToUpperCase)
                {
                    val = val.ToUpper();
                }

                if (_setting.ToProperCase)
                {
                    var cultureInfo = new CultureInfo("en-US", false).TextInfo;
                    val = val.ToLower();
                    val = cultureInfo.ToTitleCase(val);
                }

                x.Value = val;
            }
            catch (Exception ex)
            {
                context.Logger.Error($"Error on {GetType().Name}. Value={x.Value} Message={ex.Message}", ex);
                context.Exceptions.Add(new PipelineExceptionData
                {
                    Executor = GetType().Name,
                    OriginalException = ex,
                    OriginalValue = x.Value
                });
            }
        });

        await next(context);
    }
}