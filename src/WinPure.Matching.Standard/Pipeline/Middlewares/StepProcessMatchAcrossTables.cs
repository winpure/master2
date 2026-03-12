using WinPure.Matching.Properties;

namespace WinPure.Matching.Pipeline.Middlewares;

internal class StepProcessMatchAcrossTables : IPipelineMiddleware<MatchContext>
{
    public async Task Execute(MatchContext context, Func<MatchContext, Task> next)
    {
        context.OnProgress(string.Format(Resources.API_CAPTION_MATCHING_DATA, context.CurrentGroupId), 30 + context.CurrentProgressDelta);
        //var startDt = DateTime.Now;
        var lockObject = new object();
        var lockDetail = new object();
        var resRoots = new List<Item>();
        for (var i = 0; i < context.Heaps.Count; i++)
        {
            context.CToken.ThrowIfCancellationRequested();
            context.OnProgress(string.Format(Resources.API_CAPTION_MATCHING_DATA_CROUP_OF, i+1, context.Heaps.Count), 30 + i * 40 / context.Heaps.Count / ProgressDelta(context));
            var currentHeap = context.Heaps[i];
            var roots = currentHeap.Where(x => x.TableName == context.Parameter.MainTable).OrderBy(x => x.GroupValues[context.CurrentGroupId - 1].HashCodeString).ToList();
            var heap = currentHeap.Where(x => x.TableName != context.Parameter.MainTable).OrderBy(x => x.GroupValues[context.CurrentGroupId - 1].HashCodeString).ToList();

            Parallel.ForEach(heap, context.ParallelOptions, currentItem =>
            //heap.ForEach(currentItem =>
                ProcessHeap(currentItem, roots, context.CurrentGroupId, lockObject, lockDetail)
            );

            //var dataFlowTaskOption = new ExecutionDataflowBlockOptions { EnsureOrdered = false, CancellationToken = context.CToken };
            //var action = new ActionBlock<Item>(currentItem =>
            //{
            //    ProcessHeap(currentItem, roots, context.CurrentGroupId, lockObject, lockDetail);
            //}, dataFlowTaskOption);

            //for (var j = 0; j < heap.Count; j++)
            //{
            //    action.Post(heap[j]);
            //}
            //action.Complete();
            //await action.Completion;
            resRoots.AddRange(roots);
            resRoots.AddRange(heap.Where(x => x.MainItem == null).ToList());
        }
        context.Roots = resRoots;
        //context.Logger.Information($"MATCH COMPLETED, TIME: {DateTime.Now - startDt:dd\\:hh\\:mm\\:ss}");
        await next(context);
    }

    internal virtual void ProcessHeap(Item currentItem, List<Item> roots, int currentGroupId, object lockObject, object lockDetail)
    {
        try
        {
            MatchPipelineHelper.ProcessItem(currentItem, roots, currentGroupId, lockDetail);
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
            MatchPipelineHelper.ProcessItem(currentItem, roots, currentGroupId, lockDetail);
        }
    }
    internal virtual int ProgressDelta(MatchContext context) =>
        context.Parameter.Groups.Count + context.CurrentProgressDelta;
}