using WinPure.Matching.Properties;

namespace WinPure.Matching.Pipeline.Middlewares;

internal class StepCleanDataFromDirectDuplicatesMixed : IPipelineMiddleware<MatchContext>
{
    public async Task Execute(MatchContext context, Func<MatchContext, Task> next)
    {
        context.OnProgress(string.Format(Resources.API_CAPTION_FINDING_DUPLICATES, context.CurrentGroupId), 20 + context.CurrentProgressDelta);
        //var startDt = DateTime.Now;

        var duplicates = new List<List<Item>>();
        var firstElements = new List<Item>();

        foreach (var group in context.Parameter.Groups.OrderBy(x => x.GroupId))
        {
            var groupSubQry = context.Items.Where(x => x.GroupValues.Any(g => g.GroupId == group.GroupId /*&& !string.IsNullOrWhiteSpace(g.HashCodeString)*/))
                .GroupBy(x => x.GroupValues.First(g => g.GroupId == group.GroupId).HashCode())
                .Select(x => x.ToList()).ToList();
            var groupDuplicates = groupSubQry.Where(x => x.Count > 1).ToList();
            Parallel.ForEach(groupDuplicates, context.ParallelOptions, groupItemList =>
            {
                for (var j = 0; j < groupItemList.Count; j++)
                {
                    groupItemList[j].GroupId = group.GroupId;
                }
            });
            duplicates.AddRange(groupDuplicates);
            var grpFirst = groupDuplicates.Select(x => x.First()).ToList();
            firstElements.AddRange(grpFirst);
            var el = groupDuplicates.SelectMany(x => x).ToList();
            context.Items = context.Items.Except(el).ToList();
        }
        
        context.ItemsWithoutDuplicates = context.Items.Concat(firstElements).ToList();
        context.Duplicates = duplicates;
        context.FirstElements = firstElements;
        context.Items = null;

        //context.Logger.Information($"DUPLICATES WERE REMOVED, TIME: {DateTime.Now - startDt:dd\\:hh\\:mm\\:ss}");
        GC.Collect();

        await next(context);
    }
}