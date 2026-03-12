using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WinPure.Cleansing.Models.ContextData;
using WinPure.Common.Pipeline;

namespace WinPure.Cleansing.Pipeline.CleanExecutors;

internal class TextCheckExecutor : IPipelineMiddleware<CleansingContext>
{
    private readonly ColumnCheckSettings _settings;
    private readonly Regex _emailValidateRegex;

    public TextCheckExecutor(ColumnCheckSettings settings)
    {
        _settings = settings;
        _emailValidateRegex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.Compiled);
    }

    public async Task Execute(CleansingContext context, Func<CleansingContext, Task> next)
    {
        if (context.Token.IsCancellationRequested)
        {
            return;
        }

        Parallel.ForEach(context.CleansingData.Where(x => !string.IsNullOrWhiteSpace(x.Value)), context.ParallelOptions, x =>
        {
            if (_settings.CheckEmail)
            {
                try
                {
                    x.TextCheckerResult = new TextCheckerResult { IsValidEmail = _emailValidateRegex.IsMatch(x.Value) };
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
            }
        });

        await next(context);
    }
}