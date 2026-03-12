namespace WinPure.Matching.Pipeline.Middlewares;

internal class StepProcessMatchAcrossTablesMixed : StepProcessMatchAcrossTables
{
    internal override void ProcessHeap(Item currentItem, List<Item> roots, int currentGroupId, object lockObject, object lockDetail)
    {
        try
        {
            MatchPipelineHelper.ProcessItemMixed(currentItem, roots, lockDetail);
        }
        catch
        {
            lock (lockObject)
            {
                if (roots.Contains(currentItem))
                {
                    roots.Remove(currentItem);
                }
            }
            MatchPipelineHelper.ProcessItemMixed(currentItem, roots, lockDetail);
        }
    }

    internal override int ProgressDelta(MatchContext context) => 1;
}