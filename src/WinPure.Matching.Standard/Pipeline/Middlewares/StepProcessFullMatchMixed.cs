namespace WinPure.Matching.Pipeline.Middlewares;

internal class StepProcessFullMatchMixed : StepProcessFullMatch
{
    internal override void ProcessHeapItem(ItemSimilarity currItem, Item root, int currentGroupId)
    {
        currItem.RateSimilarity = SupportFunctions.RateSimilarityMixed(currItem.Item, root, out var groupValue, out var rateSimilarities);
        currItem.MatchGroupValue = groupValue;
        currItem.SimilarItem = root;
        currItem.RateSimilarities = rateSimilarities;
    }

    internal override void ProcessHeapFullCheck(Cluster cluster, int index, int detailLevel, int currentGroupId)
    {
        var addItemSimilarity = MatchPipelineHelper.ProcessItem(cluster.HeapFullCheck[index], cluster.Roots, detailLevel);
        MatchPipelineHelper.ProcessSimilarItemsInGroups(addItemSimilarity, cluster.Roots);
    }

    internal override void ProcessBrokenLinks(Cluster cluster, int index, int detailLevel, int currentGroupId)
    {
        var addItemSimilarity = MatchPipelineHelper.SearchBrokenLink(cluster.HeapBrokenLinks[index], cluster.Roots, detailLevel);
        if (addItemSimilarity.Any())
        {
            cluster.BrokenLinks.AddRange(addItemSimilarity);
        }
    }

    internal override int ProgressDelta(MatchContext context) => 1;
}