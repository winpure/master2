using System.Threading.Tasks;
using KellermanSoftware.NameParser;
using WinPure.Cleansing.Models.ContextData;
using WinPure.Common.Pipeline;

namespace WinPure.Cleansing.Pipeline.CleanExecutors;

internal class GenderSplitExecutor : IPipelineMiddleware<CleansingContext>
{
    private NameParserLogic _nameParser;
    private readonly GenderSplitSettings _settings;

    public GenderSplitExecutor(GenderSplitSettings settings)
    {
        _settings = settings;
    }

    public async Task Execute(CleansingContext context, Func<CleansingContext, Task> next)
    {
        if (context.Token.IsCancellationRequested)
        {
            return;
        }

        try
        {
            if (_settings.GenderColumns.Any())
            {
                _nameParser = new NameParserLogic("WinPure 202861", "2;6C18FE1B0724456009DF28BB941BA6E0DDB7");
            }

            Parallel.ForEach(context.CleansingData, context.ParallelOptions, x =>
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(x.GenderSplitValue))
                    {
                        var parts = _nameParser.ParseName(x.GenderSplitValue, NameOrder.AutoDetect);

                        x.SplitGenderResult = new SplitGenderResult
                        {
                            Prefix = parts.Honorific,
                            First = parts.FirstName,
                            Middle = parts.MiddleName,
                            Last = parts.LastName,
                            Suffix = parts.Suffix,
                            Quality = parts.Rank,
                            Gender = parts.IsMale.HasValue
                                ? parts.IsMale.Value ? "M" : "F"
                                : ""
                        };

                    }
                }
                catch (Exception ex)
                {
                    context.Logger.Error(
                        $"Error on {GetType().Name} in gender split. Value={x.Value} Message={ex.Message}",
                        ex);
                }
            });
        }
        catch (Exception ex)
        {
            context.Logger.Error($"Error on {GetType().Name} in name split. Message={ex.Message}", ex);
            context.Exceptions.Add(new PipelineExceptionData
            {
                Executor = GetType().Name,
                OriginalException = ex
            });
        }
        await next(context);
    }
}