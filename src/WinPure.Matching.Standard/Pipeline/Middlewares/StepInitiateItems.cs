using WinPure.Matching.Properties;

namespace WinPure.Matching.Pipeline.Middlewares;

internal class StepInitiateItems : IPipelineMiddleware<MatchContext>
{
    public async Task Execute(MatchContext context, Func<MatchContext, Task> next)
    {
        var startDt = DateTime.Now;
        context.OnProgress(Resources.API_CAPTION_CALCULATE_CHARACTESS_MAP, 15);
        var charMap = context.Items.SelectMany(x => x.GroupValues.SelectMany(y => y.Values))
            .Where(x => x.ConditionType == MatchType.Fuzzy)
            .AsParallel()
            .Select(x => new
            {
                x,
                charCount = x.Value == null
                    ? new Dictionary<char, int>()
                    : x.Value.ToString().GroupBy(y => y).Select(y => new { count = y.Count(), item = y.Key }).OrderBy(y => y.item).ToDictionary(z => z.item, y => y.count)
            }).ToList();

        Parallel.ForEach(charMap, context.ParallelOptions, x =>
        {
            (x.x as StringCondition).CharMapKey = x.charCount.Keys.ToArray();
            (x.x as StringCondition).CharMapValue = x.charCount.Values.Select(Convert.ToUInt16).ToArray();

        });
        context.Logger.Information($"ITEM CHARACTERS MAP WAS CREATED, TIME: {DateTime.Now - startDt:dd\\:hh\\:mm\\:ss}");
        await next(context);
    }
}