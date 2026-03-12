using WinPure.Matching.Properties;

namespace WinPure.Matching.Pipeline.Middlewares;

internal class ProcessFuzzyCompare : IPipelineMiddleware<MatchContext>
{
    public async Task Execute(MatchContext context, Func<MatchContext, Task> next)
    {
        context.OnProgress(string.Format(Resources.API_CAPTION_MATCHING_DATA, context.CurrentGroupId), 20);
        var builder = context.Parameter.CheckInternal
            ? MatchPipelineHelper.CreateMatchAllDataPipeline()
            : MatchPipelineHelper.CreateMatchAcrossTablesPipeline();

        var pipeline = builder.Build();
        
        var result = new List<Item>();
        var duplicates = new List<List<Item>>();
        var firstElements = new List<Item>();

        foreach (var group in context.Parameter.Groups.OrderBy(x => x.GroupId))
        {
            var lastMatchedItemList = context.Items;
            context.CurrentGroupId = group.GroupId;
            var groupResult = await pipeline.Execute(context);
            if (group.GroupId < context.Parameter.Groups.Max(m => m.GroupId))
            {
                var matchedItems = groupResult.Where(x => x.SimilarItems.Any()).ToList();
                duplicates.AddRange(context.Duplicates);
                firstElements.AddRange(context.FirstElements);

                context.FirstElements.Clear();
                context.Duplicates.Clear();
                context.Roots.Clear();
                context.Heaps = null;

                context.Items = lastMatchedItemList
                    .Except(duplicates.SelectMany(x => x))
                    .Except(matchedItems)
                    .Except(matchedItems.SelectMany(x => x.SimilarItems).Select(x => x.Item)).ToList();

                result.AddRange(groupResult.Except(context.Items));
                if (!context.Items.Any())
                {
                    break;
                }
            }
        }

        context.Duplicates.AddRange(duplicates);
        context.FirstElements.AddRange(firstElements);
        context.Roots.AddRange(result);

        await next(context);
    }
}