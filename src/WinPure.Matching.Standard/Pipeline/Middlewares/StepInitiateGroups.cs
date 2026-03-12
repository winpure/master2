using WinPure.Matching.Properties;

namespace WinPure.Matching.Pipeline.Middlewares;

internal class StepInitiateGroups : IPipelineMiddleware<MatchContext>
{
    public async Task Execute(MatchContext context, Func<MatchContext, Task> next)
    {
        //var startDt = DateTime.Now;
        context.OnProgress(Resources.API_CAPTION_INITIATE_ITEMS, 10);
        SupportFunctions.SetGetValueInItems(context.Items, context.Parameter, context.ParallelOptions, context.DictionaryService);

        //context.Logger.Information($"ITEM WAS INITIALIZED, TIME: {DateTime.Now - startDt:dd\\:hh\\:mm\\:ss}");
        await next(context);
    }
}