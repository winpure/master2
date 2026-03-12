namespace WinPure.Matching.Pipeline.Middlewares;

internal class StepPrepareHeapsForMatching : IPipelineMiddleware<MatchContext>
{
    public async Task Execute(MatchContext context, Func<MatchContext, Task> next)
    {
        //var startDt = DateTime.Now;
        var heaps = new List<List<Item>>();
        heaps = context.ItemsWithoutDuplicates.AsParallel().GroupBy(g =>
        {
            var hCode = string.Join("$", g.GroupValues[context.CurrentGroupId - 1].Values.Where(x => x.ConditionType != MatchType.Fuzzy).Select(x => x.ValueWithNullHandling));
            return HashHelper.GetHash(hCode);
        }).Select(x => x.ToList()).ToList();

        context.Heaps = heaps;
        //context.Logger.Information($"HEAPS prepared, TIME: {DateTime.Now - startDt:dd\\:hh\\:mm\\:ss}");
        await next(context);
    }
}