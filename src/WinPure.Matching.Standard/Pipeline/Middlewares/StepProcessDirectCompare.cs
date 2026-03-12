using WinPure.Matching.Properties;

namespace WinPure.Matching.Pipeline.Middlewares;

internal class StepProcessDirectCompare : IPipelineMiddleware<MatchContext>
{
    public async Task Execute(MatchContext context, Func<MatchContext, Task> next)
    {
        // Report start of duplicate-finding
        context.OnProgress(Resources.API_CAPTION_FINDING_DUPLICATES_MIX, 20 + context.CurrentProgressDelta);

        // Work off a local copy so we can remove matched items as we go
        var remaining = new List<Item>(context.Items);

        // List of clusters tagged with the rule GroupId they matched on
        var clustersWithRule = new List<(int GroupId, List<Item> Items)>();

        // 1) Direct clustering per rule without transitive chaining
        var orderedRules = context.Parameter.Groups.OrderBy(g => g.GroupId).ToList();
        foreach (var rule in orderedRules)
        {
            int progress = 20 + context.CurrentProgressDelta + (int)(20.0 * (rule.GroupId / orderedRules.Count));
            context.OnProgress(string.Format(Resources.API_CAPTION_FINDING_DUPLICATES, rule.GroupId), progress);
            // Build buckets by hash for this rule
            var buckets = new Dictionary<string, List<Item>>();
            foreach (var item in remaining)
            {
                var gv = item.GroupValues.First(v => v.GroupId == rule.GroupId);
                var key = gv.HashCode();
                if (!buckets.TryGetValue(key, out var list))
                    buckets[key] = list = new List<Item>();
                list.Add(item);
            }

            // Extract clusters: only direct duplicates (count > 1)
            var hitClusters = buckets.Values.Where(b => b.Count > 1).ToList();
            if (hitClusters.Any())
            {
                foreach (var group in hitClusters)
                {
                    // Tag each item with this GroupId for correct scoring
                    group.ForEach(it => it.GroupId = rule.GroupId);

                    clustersWithRule.Add((rule.GroupId, group));
                }

                // Remove all items that were clustered in this rule
                var toRemove = new HashSet<Item>(hitClusters.SelectMany(c => c));
                remaining = remaining.Where(it => !toRemove.Contains(it)).ToList();
            }
        }

        context.OnProgress(Resources.API_CAPTION_FINDING_DUPLICATES_MIX, 40 + context.CurrentProgressDelta);
        // 2) Attach each cluster's duplicates to its master
        var masters = new List<Item>();
        foreach (var (groupId, group) in clustersWithRule)
        {
            var master = group[0];
            masters.Add(master);
            var rest = group.Skip(1).ToList();
            if (!rest.Any()) continue;

            // Build similarity info for this group and attach
            var dupInfo = MatchPipelineHelper.ProcessDuplicateGroup(master, rest);
            lock (master)
            {
                master.SimilarItems.AddRange(dupInfo.Similar);
            }
        }
        context.OnProgress(Resources.API_CAPTION_FINDING_DUPLICATES_MIX, 70 + context.CurrentProgressDelta);
        // 3) Populate context for downstream
        context.Duplicates = clustersWithRule.Select(cr => cr.Items).ToList();
        context.FirstElements = masters;
        context.ItemsWithoutDuplicates = remaining;
        context.Items = null;
        context.OnProgress(Resources.API_CAPTION_FINDING_DUPLICATES_MIX, 80 + context.CurrentProgressDelta);
        // 4) Build final Roots: masters ordered by GroupId, then singletons, then null-items
        var roots = new List<Item>();
        roots.AddRange(masters.OrderBy(m => m.GroupId));
        roots.AddRange(remaining);
        if (context.ItemsWithNull?.Any() == true)
            roots.AddRange(context.ItemsWithNull);

        context.Roots = roots;

        await next(context);
    }
}