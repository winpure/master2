using WinPure.Matching.Properties;

namespace WinPure.Matching.Pipeline.Middlewares;

internal class StepCleanDataFromDirectDuplicates : IPipelineMiddleware<MatchContext>
{
    public async Task Execute(MatchContext context, Func<MatchContext, Task> next)
    {
        context.OnProgress(string.Format(Resources.API_CAPTION_FINDING_DUPLICATES, context.CurrentGroupId), 20 + context.CurrentProgressDelta);
        //var startDt = DateTime.Now;

        var groupSubQry = context.Items
            .GroupBy(x => x.GroupValues.First(g => g.GroupId == context.CurrentGroupId).HashCode())
            .Select(x => x.ToList()).ToList();
        var duplicates = groupSubQry.Where(x => x.Count > 1).ToList();
        Parallel.ForEach(duplicates, context.ParallelOptions, groupItemList =>
        {
            for (var j = 0; j < groupItemList.Count; j++)
            {
                groupItemList[j].GroupId = context.CurrentGroupId;
            }
        });
        var firstElements = duplicates.Select(x => x.First()).ToList();
        var duplicateItems = duplicates.SelectMany(x => x).ToList();
        context.Items = context.Items.Except(duplicateItems).ToList();

        context.ItemsWithoutDuplicates = context.Items.Concat(firstElements).ToList();
        context.Duplicates = duplicates;
        context.FirstElements = firstElements;
        context.Items = null;

        //context.Logger.Information($"DUPLICATES WERE REMOVED, TIME: {DateTime.Now - startDt:dd\\:hh\\:mm\\:ss}");
        GC.Collect();

        await next(context);
    }
}