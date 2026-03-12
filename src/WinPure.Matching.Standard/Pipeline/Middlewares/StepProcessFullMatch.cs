using WinPure.Matching.Properties;

namespace WinPure.Matching.Pipeline.Middlewares;

internal class StepProcessFullMatch : IPipelineMiddleware<MatchContext>
{
    double LEVEL1_GENERAL = 0.75;
    int MAX_GROUP_SIZE = 1500;
    int MIN_GROUP_SIZE = 200;

    public async Task Execute(MatchContext context, Func<MatchContext, Task> next)
    {
        context.OnProgress(string.Format(Resources.API_CAPTION_MATCHING_DATA, context.CurrentGroupId), 30 + context.CurrentProgressDelta);
        //var startDt = DateTime.Now;
        var detailLevel = 100 - context.Parameter.SearchDeep;
        double level2General = detailLevel / 100.0;
        //var dataFlowTaskOption = new ExecutionDataflowBlockOptions { EnsureOrdered = false, CancellationToken = context.CToken };
        var resRoots = context.Heaps.Where(x => x.Count <= 1).SelectMany(x => x).ToList();
        var groupToProcess = context.Heaps.Where(x => x.Count > 1).ToList();
        var totalGroup = groupToProcess.Count;
        var currentGroup = 1;
        //Parallel.ForEach(groupToProcess, context.ParallelOptions, h =>
        for (var i = 0; i < groupToProcess.Count; i++)
        {
            var currentGroupToProcess = groupToProcess[i];
            context.CToken.ThrowIfCancellationRequested();
            context.OnProgress(string.Format(Resources.API_CAPTION_MATCHING_DATA_CROUP_OF, currentGroup, totalGroup), 30 + (currentGroup - 1) * 40 / totalGroup / ProgressDelta(context));

            var heap = currentGroupToProcess
                .OrderBy(x => x.GroupValues[context.CurrentGroupId - 1].HashCodeString.Length)
                .Select(x => new ItemSimilarity { Item = x })
                .ToList();

            var clusters = new List<Cluster>();
            double level1Current;

            while (heap.Any())
            {
                var rootSim = heap.First();
                heap.Remove(rootSim);
                var root = rootSim.Item;
                var cluster = new Cluster(root);
                Parallel.ForEach(heap, context.ParallelOptions, currItem =>
                //heap.ForEach(y =>
                {
                    ProcessHeapItem(currItem, root, context.CurrentGroupId);
                });

                if (heap.Count <= MAX_GROUP_SIZE)
                {
                    level1Current = 0;
                }
                else
                {
                    level1Current = LEVEL1_GENERAL;
                    var rateLimit = heap.Select(x => Math.Min(x.RateSimilarity / x.MinGroupRate, x.MinGroupRate))
                        .OrderByDescending(x => x)
                        .Skip(MAX_GROUP_SIZE)
                        .FirstOrDefault();

                    if (rateLimit > 0 && rateLimit > LEVEL1_GENERAL)
                    {
                        level1Current = rateLimit;
                    }
                    else
                    {
                        rateLimit = heap.Select(x => Math.Min(x.RateSimilarity / x.MinGroupRate, x.MinGroupRate))
                            .OrderByDescending(x => x)
                            .Skip(MIN_GROUP_SIZE)
                            .FirstOrDefault();

                        if (rateLimit > 0 && rateLimit < LEVEL1_GENERAL)
                        {
                            level1Current = rateLimit;
                        }
                    }
                }

                var workHeaps = heap.Where(x => x.RateSimilarity >= x.MinGroupRate * level1Current * level2General)
                    .GroupBy(x =>
                        x.RateSimilarity >= x.MinGroupRate * level1Current * level2General &&
                         x.RateSimilarity < x.MinGroupRate * level1Current
                            ? -1
                            : 0)
                    .Select(x => new { x.Key, List = x.ToList() }).ToList();

                cluster.HeapFullCheck = workHeaps.Where(x => x.Key == 0).SelectMany(x => x.List).OrderByDescending(x => x.RateSimilarity).Select(x => x.Item).ToList();
                cluster.HeapBrokenLinks = workHeaps.Where(x => x.Key == -1).SelectMany(x => x.List).Select(x => x.Item).ToList();
                heap.RemoveAll(y => y.RateSimilarity >= y.MinGroupRate * level1Current);
                clusters.Add(cluster);
                context.CToken.ThrowIfCancellationRequested();
                context.OnProgress(string.Format(Resources.API_CAPTION_MATCHING_DATA_CROUP_OF, currentGroup, totalGroup), 30 + (currentGroup - 1) * 40 / totalGroup / ProgressDelta(context));
            }

            context.CToken.ThrowIfCancellationRequested();
            context.OnProgress(string.Format(Resources.API_CAPTION_MATCHING_DATA_CROUP_OF, currentGroup, totalGroup), 30 + (currentGroup - 1) * 40 / totalGroup / ProgressDelta(context));

            Parallel.ForEach(clusters.Where(x => x.HeapFullCheck.Any()).OrderByDescending(x => x.HeapFullCheck.Count), context.ParallelOptions, cluster =>
            {
                for (var ci = 0; ci < cluster.HeapFullCheck.Count; ci++)
                {
                    ProcessHeapFullCheck(cluster, ci, detailLevel, context.CurrentGroupId);
                }
            });

            context.CToken.ThrowIfCancellationRequested();
            var roots = clusters.SelectMany(x => x.Roots).ToList();
            context.OnProgress(string.Format(Resources.API_CAPTION_MATCHING_DATA_CROUP_OF, currentGroup, totalGroup), 30 + (currentGroup - 1) * 40 / totalGroup / ProgressDelta(context));

            Parallel.ForEach(clusters.Where(x => x.HeapBrokenLinks.Any()).OrderByDescending(x => x.HeapBrokenLinks.Count), context.ParallelOptions, cluster =>
            //clusters.Where(x => x.HeapFullCheck.Any()).OrderByDescending(x => x.HeapFullCheck.Count).ToList().ForEach(cluster =>
            {
                for (var ci = 0; ci < cluster.HeapBrokenLinks.Count; ci++)
                {
                    ProcessBrokenLinks(cluster, ci, detailLevel, context.CurrentGroupId);
                }
            });

            context.OnProgress(string.Format(Resources.API_CAPTION_MATCHING_DATA_CROUP_OF, currentGroup, totalGroup), 30 + (currentGroup - 1) * 40 / totalGroup / ProgressDelta(context));
            MatchPipelineHelper.ProcessSimilarItemsInGroups(clusters.SelectMany(x => x.BrokenLinks).ToList(), roots);
            resRoots.AddRange(roots);
            currentGroup++;
        }

        context.Roots = resRoots;
        //context.Logger.Information($"MATCH COMPLETED, TIME: {DateTime.Now - startDt:dd\\:hh\\:mm\\:ss}");
        await next(context);
    }

    internal virtual void ProcessHeapItem(ItemSimilarity currItem, Item root, int currentGroupId)
    {
        currItem.RateSimilarity = SupportFunctions.RateSimilarity(currItem.Item, root, currentGroupId, out var groupValue, out var rateSimilarities);
        currItem.MatchGroupValue = groupValue;
        currItem.SimilarItem = root;
        currItem.RateSimilarities = rateSimilarities;
    }

    internal virtual void ProcessHeapFullCheck(Cluster cluster, int index, int detailLevel, int currentGroupId)
    {
        var addItemSimilarity = MatchPipelineHelper.ProcessItem(cluster.HeapFullCheck[index], cluster.Roots, currentGroupId, detailLevel);
        MatchPipelineHelper.ProcessSimilarItemsInGroups(addItemSimilarity, cluster.Roots);
    }

    internal virtual void ProcessBrokenLinks(Cluster cluster, int index, int detailLevel, int currentGroupId)
    {
        var addItemSimilarity = MatchPipelineHelper.SearchBrokenLink(cluster.HeapBrokenLinks[index], cluster.Roots, currentGroupId, detailLevel);
        if (addItemSimilarity.Any())
        {
            cluster.BrokenLinks.AddRange(addItemSimilarity);
        }
    }

    internal virtual int ProgressDelta(MatchContext context) =>
        context.Parameter.Groups.Count + context.CurrentProgressDelta;

}