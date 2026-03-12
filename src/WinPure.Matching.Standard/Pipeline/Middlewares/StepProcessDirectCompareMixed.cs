using WinPure.Matching.Properties;

namespace WinPure.Matching.Pipeline.Middlewares;

internal class StepProcessDirectCompareMixed : IPipelineMiddleware<MatchContext>
{
    public async Task Execute(MatchContext context, Func<MatchContext, Task> next)
    {
        // Report start of duplicate-finding
        context.OnProgress(Resources.API_CAPTION_FINDING_DUPLICATES_MIX, 20 + context.CurrentProgressDelta);

        // Select working set: use pre-clean Items or post-clean Roots
        var items = context.Items ?? context.Roots;
        int n = items.Count;

        // Initialize union-find parent array
        var parent = Enumerable.Range(0, n).ToArray();
        Func<int, int> find = null;
        find = i => parent[i] == i ? i : (parent[i] = find(parent[i]));
        Action<int, int> union = (i, j) =>
        {
            int ri = find(i), rj = find(j);
            if (ri != rj) parent[rj] = ri;
        };

        // Phase 1: union indices sharing the same hash for each rule (transitive clustering)
        var orderedRules = context.Parameter.Groups.OrderBy(g => g.GroupId).ToList();
        int rulesCount = orderedRules.Count;
        for (int idx = 0; idx < rulesCount; idx++)
        {
            var rule = orderedRules[idx];
            // Build buckets by hash
            var buckets = new Dictionary<string, List<int>>();
            for (int i = 0; i < n; i++)
            {
                var gv = items[i].GroupValues.First(v => v.GroupId == rule.GroupId);
                var key = gv.HashCode();
                if (!buckets.TryGetValue(key, out var list))
                    buckets[key] = list = new List<int>();
                list.Add(i);
            }
            // Union within buckets
            foreach (var list in buckets.Values)
            {
                if (list.Count <= 1) continue;
                int root = list[0];
                for (int k = 1; k < list.Count; k++)
                    union(root, list[k]);
            }
            // Report progress after each rule
            int progress = 20 + context.CurrentProgressDelta + (int)(20.0 * (idx + 1) / rulesCount);
            context.OnProgress(Resources.API_CAPTION_FINDING_DUPLICATES_MIX, progress);
        }

        // Phase 2: build clusters by root representative
        var map = new Dictionary<int, List<int>>();
        for (int i = 0; i < n; i++)
        {
            int r = find(i);
            if (!map.TryGetValue(r, out var lst))
                map[r] = lst = new List<int>();
            lst.Add(i);
        }
        context.OnProgress(Resources.API_CAPTION_FINDING_DUPLICATES_MIX, 40 + context.CurrentProgressDelta);

        // Extract clusters >1
        var clusters = map.Values
                          .Where(lst => lst.Count > 1)
                          .Select(lst => lst.Select(i => items[i]).ToList())
                          .ToList();
        context.OnProgress(Resources.API_CAPTION_FINDING_DUPLICATES_MIX, 60 + context.CurrentProgressDelta);

        // Phase 3: attach duplicates to masters
        var masters = new List<Item>();
        foreach (var group in clusters)
        {
            var master = group[0];
            masters.Add(master);
            var rest = group.Skip(1).ToList();
            if (!rest.Any()) continue;

            var dupInfo = MatchPipelineHelper.ProcessDuplicateGroup(master, rest);
            lock (master)
            {
                master.SimilarItems.AddRange(dupInfo.Similar);
            }
        }
        context.OnProgress(Resources.API_CAPTION_FINDING_DUPLICATES_MIX, 70 + context.CurrentProgressDelta);

        // Populate context
        context.Duplicates = clusters;
        context.FirstElements = masters;
        var dupSet = new HashSet<Item>(clusters.SelectMany(g => g));
        context.ItemsWithoutDuplicates = items.Where(it => !dupSet.Contains(it)).ToList();
        context.Items = null;

        // Phase 4: build final Roots (singletons + masters + null-items)
        var roots = new List<Item>();
        roots.AddRange(context.ItemsWithoutDuplicates);
        roots.AddRange(masters);
        if (context.ItemsWithNull?.Any() == true)
            roots.AddRange(context.ItemsWithNull);
        context.Roots = roots;

        // Final progress mark
        context.OnProgress(Resources.API_CAPTION_FINDING_DUPLICATES_MIX, 80 + context.CurrentProgressDelta);

        await next(context);
    }
}