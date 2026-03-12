namespace WinPure.Matching.Pipeline.Middlewares;

internal class StepPrepareHeapsForMatchingMixed : IPipelineMiddleware<MatchContext>
{
    public async Task Execute(MatchContext context, Func<MatchContext, Task> next)
    {
        //var startDt = DateTime.Now;
        var heaps = new List<List<Item>>();
        foreach (var matchGroup in context.Parameter.Groups)
        {
            var groupHeap = context.ItemsWithoutDuplicates.GroupBy(g =>
            {
                var group = g.GroupValues.First(x => x.GroupId == matchGroup.GroupId);
                var hCode = string.Join("$", group.Values.Where(x => x.ConditionType != MatchType.Fuzzy).Select(x => x.ValueWithNullHandling));

                return HashHelper.GetHash(hCode);
            }).Select(x => x.ToList()).ToList();

            var itemsForHeap = groupHeap.Where(x => x.Count > 1).ToList();
            heaps.AddRange(itemsForHeap);
            context.ItemsWithoutDuplicates = context.ItemsWithoutDuplicates.Except(itemsForHeap.SelectMany(x => x)).ToList();
        }

        heaps.AddRange(new List<List<Item>> { context.ItemsWithoutDuplicates });

        context.Heaps = heaps;
        //context.Logger.Information($"HEAPS prepared, TIME: {DateTime.Now - startDt:dd\\:hh\\:mm\\:ss}");
        await next(context);
    }
}