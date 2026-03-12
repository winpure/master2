using System.Collections.Concurrent;
using WinPure.Matching.Properties;

namespace WinPure.Matching.Pipeline.Middlewares;

internal class StepAddDuplicatesAndNullsToResult : IPipelineMiddleware<MatchContext>
{
    // These constants can be fine-tuned by testing
    private const int MAX_DUPLICATE_RANGE = 10000;
    private const int COEFFICIENT_OF_PARALLELISM = 4;

    public async Task Execute(MatchContext context, Func<MatchContext, Task> next)
    {
        // Report start of duplicate-addition
        context.OnProgress(Resources.API_CAPTION_ADD_DUPLICATES_TO_RESULT, 70);
        context.Logger.Information($"START ADD DUPLICATES Count = {context.Duplicates.Count}");

        int groupsTotal = context.Duplicates.Count;
        int procs = Environment.ProcessorCount * COEFFICIENT_OF_PARALLELISM;
        int chunkSize = groupsTotal / procs;
        if (chunkSize > MAX_DUPLICATE_RANGE)
            chunkSize = MAX_DUPLICATE_RANGE;

        var bag = new ConcurrentBag<ItemDuplicates>();

        if (chunkSize > 0)
        {
            // Create partitions of [0..groupsTotal) to feed into Parallel.ForEach
            var rangePartitioner = Partitioner.Create(0, groupsTotal, chunkSize);

            Parallel.ForEach(rangePartitioner, context.ParallelOptions, range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var group = context.Duplicates[i];

                    // pick the designated master for this cluster by index
                    var master = context.FirstElements[i];

                    // prepare list of rest duplicates, with known capacity
                    int restCount = group.Count - 1;
                    if (restCount <= 0) continue;
                    var rest = new List<Item>(restCount);
                    for (int j = 0; j < group.Count; j++)
                    {
                        var item = group[j];
                        if (!ReferenceEquals(item, master))
                            rest.Add(item);
                    }

                    // original logic encapsulated in helper
                    var dupInfo = MatchPipelineHelper.ProcessDuplicateGroup(master, rest);
                    bag.Add(dupInfo);
                }
            });
        }
        else
        {
            // fallback to simple foreach
            foreach (var group in context.Duplicates)
            {
                var master = context.FirstElements.Intersect(group).First();
                var rest = group.Where(x => x != master).ToList();
                if (!rest.Any()) continue;

                var dupInfo = MatchPipelineHelper.ProcessDuplicateGroup(master, rest);
                bag.Add(dupInfo);
            }
        }

        // Now group by MainItem and add all Similar ranges at once
        // ToLookup is slightly faster and avoids intermediate allocation of anonymous objects
        var toAdd = bag.ToLookup(d => d.MainItem);
        Parallel.ForEach(toAdd, context.ParallelOptions, entry =>
        {
            // entry.Key == master; entry is IEnumerable<ItemDuplicates>
            var allSims = entry.SelectMany(d => d.Similar);
            entry.Key.SimilarItems.AddRange(allSims);
        });

        // Finally, include any null-valued items in Roots
        if (context.ItemsWithNull.Any())
            context.Roots.AddRange(context.ItemsWithNull);

        await next(context);
    }
}