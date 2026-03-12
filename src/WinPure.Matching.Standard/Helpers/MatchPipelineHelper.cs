using WinPure.Matching.Models;
using WinPure.Matching.Pipeline.Middlewares;

namespace WinPure.Matching.Helpers;

internal static class MatchPipelineHelper
{
    internal static MatchPipelineBuilder CreateFuzzyMatchPipeline()
    {
        var builder = new MatchPipelineBuilder();
        builder.Use(new StepCreateItemList());
        builder.Use(new StepInitiateGroups());
        builder.Use(new StepInitiateItems());
        builder.Use(new ProcessFuzzyCompare());
        builder.Use(new StepAddDuplicatesAndNullsToResult());

        return builder;
    }

    internal static MatchPipelineBuilder CreateMixedFuzzyMatchPipeline(bool checkInternal)
    {
        var builder = new MatchPipelineBuilder();
        builder.Use(new StepCreateItemList());
        builder.Use(new StepInitiateGroups());
        builder.Use(new StepInitiateItems());
        builder.Use(new StepCleanDataFromDirectDuplicatesMixed());
        builder.Use(new StepPrepareHeapsForMatchingMixed());
        if (checkInternal)
        {
            builder.Use(new StepProcessFullMatchMixed());
        }
        else
        {
            builder.Use(new StepProcessMatchAcrossTablesMixed());
        }
        builder.Use(new StepAddDuplicatesAndNullsToResult());

        return builder;
    }

    internal static MatchPipelineBuilder CreateDirectMatchPipeline()
    {
        var builder = new MatchPipelineBuilder();
        builder.Use(new StepCreateItemList());
        builder.Use(new StepInitiateGroups());
        builder.Use(new StepProcessDirectCompare());

        return builder;
    }
    internal static MatchPipelineBuilder CreateDirectMixedMatchPipeline()
    {
        var builder = new MatchPipelineBuilder();
        builder.Use(new StepCreateItemList());
        builder.Use(new StepInitiateGroups());
        builder.Use(new StepProcessDirectCompareMixed());

        return builder;
    }

    internal static MatchPipelineBuilder CreateMatchAcrossTablesPipeline()
    {
        var builder = new MatchPipelineBuilder();
        builder.Use(new StepCleanDataFromDirectDuplicates());
        builder.Use(new StepPrepareHeapsForMatching());
        builder.Use(new StepProcessMatchAcrossTables());

        return builder;
    }

    internal static MatchPipelineBuilder CreateMatchAllDataPipeline()
    {
        var builder = new MatchPipelineBuilder();
        builder.Use(new StepCleanDataFromDirectDuplicates());
        builder.Use(new StepPrepareHeapsForMatching());
        builder.Use(new StepProcessFullMatch());

        return builder;
    }

    internal static List<ItemSimilarity> SearchBrokenLink(Item currentItem, List<Item> roots, int currentGroupId, int detailCoefficient)
    {
        var i = 0;
        var additionalItemSimilarity = new List<ItemSimilarity>();
        while (i < roots.Count)
        {
            var verifiedItem = roots[i];
            GroupValue matchGroupValue;
            List<double> rateSimilarities;
            var rateSimilarity = SupportFunctions.RateSimilarity(currentItem, verifiedItem, currentGroupId, roots[i].SimilarItems.Count == 0 ? 100 : detailCoefficient, out matchGroupValue, out rateSimilarities, out var typeSimilarity);
            if (typeSimilarity == 0)
            {
                var j = 0;
                while (typeSimilarity <= 0)
                {
                    if (j == roots[i].SimilarItems.Count) break;
                    verifiedItem = roots[i].SimilarItems[j].Item;
                    rateSimilarity = SupportFunctions.RateSimilarity(currentItem, verifiedItem, currentGroupId, 100, out matchGroupValue, out rateSimilarities, out typeSimilarity);
                    j++;
                }
            }
            if (typeSimilarity > 0)
            {
                additionalItemSimilarity.Add(new ItemSimilarity
                {
                    Item = verifiedItem,
                    SimilarItem = currentItem,
                    MatchGroupValue = matchGroupValue,
                    RateSimilarities = rateSimilarities,
                    RateSimilarity = rateSimilarity,
                });
            }
            i++;
        }

        return additionalItemSimilarity;
    }

    internal static List<ItemSimilarity> SearchBrokenLink(Item currentItem, List<Item> roots, int detailCoefficient)
    {
        var i = 0;
        var additionalItemSimilarity = new List<ItemSimilarity>();
        while (i < roots.Count)
        {
            var verifiedItem = roots[i];
            GroupValue matchGroupValue;
            List<double> rateSimilarities;
            var rateSimilarity = SupportFunctions.RateSimilarityMixed(currentItem, verifiedItem, roots[i].SimilarItems.Count == 0 ? 100 : detailCoefficient, out matchGroupValue, out rateSimilarities, out var typeSimilarity);
            if (typeSimilarity == 0)
            {
                var j = 0;
                while (typeSimilarity <= 0)
                {
                    if (j == roots[i].SimilarItems.Count) break;
                    verifiedItem = roots[i].SimilarItems[j].Item;
                    rateSimilarity = SupportFunctions.RateSimilarityMixed(currentItem, verifiedItem, 100, out matchGroupValue, out rateSimilarities, out typeSimilarity);
                    j++;
                }
            }
            if (typeSimilarity > 0)
            {
                additionalItemSimilarity.Add(new ItemSimilarity
                {
                    Item = verifiedItem,
                    SimilarItem = currentItem,
                    MatchGroupValue = matchGroupValue,
                    RateSimilarities = rateSimilarities,
                    RateSimilarity = rateSimilarity,
                });
            }
            i++;
        }

        return additionalItemSimilarity;
    }

    internal static List<ItemSimilarity> ProcessItem(Item currentItem, List<Item> roots, int detailCoefficient)
    {
        var i = 0;
        var addItemSimilarity = new List<ItemSimilarity>();
        while (i < roots.Count)
        {
            var rootI = roots[i];
            if (rootI == null)
            {
                i--;
                continue;
            }
            if (currentItem.MainItem == null)//TODO Null reference
            {
                int typeSimilarity;
                var verifiedItem = roots[i];
                GroupValue matchGroupValue;
                List<double> rateSimilarities;
                var rateSimilarity = SupportFunctions.RateSimilarityMixed(currentItem, verifiedItem, roots[i].SimilarItems.Count == 0 ? 100 : detailCoefficient, out matchGroupValue, out rateSimilarities, out typeSimilarity);
                if (typeSimilarity == 0)
                {
                    var j = 0;
                    while (typeSimilarity <= 0)
                    {
                        if (j == roots[i].SimilarItems.Count) break;
                        verifiedItem = roots[i].SimilarItems[j].Item;
                        rateSimilarity = SupportFunctions.RateSimilarityMixed(currentItem, verifiedItem, 100, out matchGroupValue, out rateSimilarities, out typeSimilarity);
                        j++;
                    }
                }
                if (typeSimilarity > 0)
                {
                    if (currentItem.MainItem == null)
                    {
                        IncludeItemInSimilarItems(currentItem, verifiedItem, matchGroupValue, rateSimilarity, rateSimilarities);
                    }
                    else
                    {
                        if (verifiedItem.MainItem != null)
                            addItemSimilarity.Add(new ItemSimilarity
                            {
                                Item = verifiedItem,
                                SimilarItem = currentItem,
                                MatchGroupValue = matchGroupValue,
                                RateSimilarities = rateSimilarities,
                                RateSimilarity = rateSimilarity,
                            });
                    }
                }
            }
            i++;
        }

        if (currentItem.MainItem == null)
        {
            roots.Add(currentItem);
        }

        return addItemSimilarity;
    }

    internal static List<ItemSimilarity> ProcessItem(Item currentItem, List<Item> roots, int currentGroupId, int detailCoefficient)
    {
        var i = 0;
        var addItemSimilarity = new List<ItemSimilarity>();
        while (i < roots.Count)
        {
            var root = roots[i];
            if (root == null)
            {
                i--;
                continue;
            }
            if (currentItem.MainItem == null)//TODO Null reference
            {
                int typeSimilarity;
                var verifiedItem = roots[i];
                GroupValue matchGroupValue;
                List<double> rateSimilarities;
                var rateSimilarity = SupportFunctions.RateSimilarity(currentItem, verifiedItem, currentGroupId, roots[i].SimilarItems.Count == 0 ? 100 : detailCoefficient, out matchGroupValue, out rateSimilarities, out typeSimilarity);
                if (typeSimilarity == 0)
                {
                    var j = 0;
                    while (typeSimilarity <= 0)
                    {
                        if (j == roots[i].SimilarItems.Count) break;
                        verifiedItem = roots[i].SimilarItems[j].Item;
                        rateSimilarity = SupportFunctions.RateSimilarity(currentItem, verifiedItem, currentGroupId, 100, out matchGroupValue, out rateSimilarities, out typeSimilarity);
                        j++;
                    }
                }
                if (typeSimilarity > 0)
                {
                    if (currentItem.MainItem == null)
                    {
                        IncludeItemInSimilarItems(currentItem, verifiedItem, matchGroupValue, rateSimilarity, rateSimilarities);
                    }
                    else
                    {
                        if (verifiedItem.MainItem != null)
                            addItemSimilarity.Add(new ItemSimilarity
                            {
                                Item = verifiedItem,
                                SimilarItem = currentItem,
                                MatchGroupValue = matchGroupValue,
                                RateSimilarities = rateSimilarities,
                                RateSimilarity = rateSimilarity,
                            });
                    }
                }
            }
            i++;
        }

        if (currentItem.MainItem == null)
        {
            roots.Add(currentItem);
        }

        return addItemSimilarity;
    }

    internal static void ProcessItem(Item currentItem, List<Item> roots, int currentGroupId, object lockDetail)
    {
        var i = 0;
        while (i < roots.Count)
        {
            var rootI = roots[i];
            if (rootI == null)
            {
                i--;
                continue;
            }
            if (currentItem.MainItem == null)//TODO Null reference
            {
                int typeSimilarity;
                var verifiedItem = roots[i];
                GroupValue matchGroupValue;
                List<double> rateSimilarities;
                var rateSimilarity = SupportFunctions.RateSimilarity(currentItem, verifiedItem, currentGroupId, 100, out matchGroupValue, out rateSimilarities, out typeSimilarity);
                if (typeSimilarity == 0)
                {
                    var j = 0;
                    while (typeSimilarity <= 0)
                    {
                        if (j == roots[i].SimilarItems.Count) break;
                        verifiedItem = roots[i].SimilarItems[j].Item;
                        rateSimilarity = SupportFunctions.RateSimilarity(currentItem, verifiedItem, currentGroupId, 100, out matchGroupValue, out rateSimilarities, out typeSimilarity);
                        j++;
                    }
                }
                if (typeSimilarity > 0)
                {
                    if (currentItem.MainItem == null)
                    {
                        lock (lockDetail)
                        {
                            IncludeItemInSimilarItems(currentItem, verifiedItem, matchGroupValue, rateSimilarity, rateSimilarities);
                        }
                        break;
                    }
                }
            }
            i++;
        }
    }

    internal static void ProcessItemMixed(Item currentItem, List<Item> roots, object lockDetail)
    {
        var i = 0;
        while (i < roots.Count)
        {
            var rootI = roots[i];
            if (rootI == null)
            {
                i--;
                continue;
            }
            if (currentItem.MainItem == null)//TODO Null reference
            {
                int typeSimilarity;
                var verifiedItem = roots[i];
                GroupValue matchGroupValue;
                List<double> rateSimilarities;
                var rateSimilarity = SupportFunctions.RateSimilarityMixed(currentItem, verifiedItem, 100, out matchGroupValue, out rateSimilarities, out typeSimilarity);
                if (typeSimilarity == 0)
                {
                    var j = 0;
                    while (typeSimilarity <= 0)
                    {
                        if (j == roots[i].SimilarItems.Count) break;
                        verifiedItem = roots[i].SimilarItems[j].Item;
                        rateSimilarity = SupportFunctions.RateSimilarityMixed(currentItem, verifiedItem, 100, out matchGroupValue, out rateSimilarities, out typeSimilarity);
                        j++;
                    }
                }
                if (typeSimilarity > 0)
                {
                    if (currentItem.MainItem == null)
                    {
                        if (lockDetail != null)
                            lock (lockDetail)
                            {
                                IncludeItemInSimilarItems(currentItem, verifiedItem, matchGroupValue, rateSimilarity, rateSimilarities);
                            }
                        else
                            IncludeItemInSimilarItems(currentItem, verifiedItem, matchGroupValue, rateSimilarity, rateSimilarities);
                        break;
                    }
                }
            }
            i++;
        }
    }

    internal static void ProcessSimilarItemsInGroups(List<ItemSimilarity> allAddItemSimilarity, List<Item> roots)
    {
        for (var i = 0; i < allAddItemSimilarity.Count; i++)
        {
            var oldMain = MergeGroups(allAddItemSimilarity[i]);
            if (oldMain != null)
            {
                roots.Remove(oldMain);
            }
        }
    }

    internal static ItemDuplicates ProcessDuplicateGroup(Item firstDuplicateElement, List<Item> restDuplicateElements)
    {
        var mainItem = firstDuplicateElement.MainItem ?? firstDuplicateElement;

        var rateSimilarities = firstDuplicateElement.MainItem?.SimilarItems?.FirstOrDefault(x => x.Item == firstDuplicateElement)?.RateSimilarities ?? new List<double>();

        var groupSimilarity = firstDuplicateElement.MainItem?.SimilarItems?.FirstOrDefault(x => x.Item == firstDuplicateElement)?.RateSimilarity ?? 1;

        var newElements = restDuplicateElements.Select(y => new ItemSimilarity
        {
            Item = y,
            SimilarItem = mainItem,
            MatchGroupValue = firstDuplicateElement.GroupId.HasValue
                ? firstDuplicateElement.GroupValues.First(x => x.GroupId == firstDuplicateElement.GroupId.Value)
                : firstDuplicateElement.GroupValues.First(),
            RateSimilarities = rateSimilarities,
            RateSimilarity = groupSimilarity
        }).ToList();

        for (var y = 0; y < restDuplicateElements.Count; y++)
        {
            var rEl = restDuplicateElements[y];
            rEl.MainItem = mainItem;
            rEl.SimilarItems.Clear();
        }

        return new ItemDuplicates { MainItem = mainItem, Similar = newElements };
    }

    internal static Item MergeGroups(ItemSimilarity link)
    {
        if ((link.Item.MainItem ?? link.Item) == (link.SimilarItem.MainItem ?? link.SimilarItem))
        {
            return null;
        }

        if (link.Item.MainItem != null && link.SimilarItem.MainItem == null)
        {
            var trans = link.Item;
            link.Item = link.SimilarItem;
            link.SimilarItem = trans;
        }
        Item oldMain;
        if (link.Item.MainItem == null)
        {
            oldMain = link.Item;
            oldMain.MainItem = link.SimilarItem.MainItem ?? link.SimilarItem;
            oldMain.MainItem.SimilarItems.Add(link);
            for (var y = 0; y < oldMain.SimilarItems.Count; y++)
            {
                oldMain.SimilarItems[y].Item.MainItem = oldMain.MainItem;
            }

            if (oldMain.MainItem.SimilarItems.Count > oldMain.SimilarItems.Count)
            {
                oldMain.MainItem.SimilarItems.AddRange(oldMain.SimilarItems);
                oldMain.SimilarItems.Clear();
            }
            else
            {
                oldMain.SimilarItems.AddRange(oldMain.MainItem.SimilarItems);
                oldMain.MainItem.SimilarItems = oldMain.SimilarItems;
                oldMain.SimilarItems = new List<ItemSimilarity>();
            }
        }
        else //TODO double reference
        {
            oldMain = link.Item.MainItem;
            var searchItem = link.Item;
            ItemSimilarity lastSimilarItems = null;
            while (true)
            {
                var corrSimilarItems = oldMain.SimilarItems.First(y => y.Item == searchItem && y != lastSimilarItems);
                corrSimilarItems.Item = corrSimilarItems.SimilarItem;
                corrSimilarItems.SimilarItem = searchItem;
                if (corrSimilarItems.Item == oldMain)
                {
                    break;
                }
                searchItem = corrSimilarItems.Item;
                lastSimilarItems = corrSimilarItems;
            }
            oldMain.MainItem = link.SimilarItem.MainItem;
            link.Item.MainItem = oldMain.MainItem;
            oldMain.MainItem.SimilarItems.Add(link);

            for (var y = 0; y < oldMain.SimilarItems.Count; y++)
            {
                oldMain.SimilarItems[y].Item.MainItem = oldMain.MainItem;
            }

            if (oldMain.MainItem.SimilarItems.Count > oldMain.SimilarItems.Count)
            {
                oldMain.MainItem.SimilarItems.AddRange(oldMain.SimilarItems);
                oldMain.SimilarItems.Clear();
            }
            else
            {
                oldMain.SimilarItems.AddRange(oldMain.MainItem.SimilarItems);
                oldMain.MainItem.SimilarItems = oldMain.SimilarItems;
                oldMain.SimilarItems = new List<ItemSimilarity>();
            }
        }
        return oldMain;
    }

    internal static void IncludeItemInSimilarItems(Item item, Item similarItem, GroupValue matchGroupValue, double rateSimilarity, List<double> rateSimilarities)
    {
        item.MainItem = similarItem.MainItem ?? similarItem;
        item.MainItem.SimilarItems.Add(new ItemSimilarity
        {
            Item = item,
            SimilarItem = similarItem,
            MatchGroupValue = matchGroupValue,
            RateSimilarities = rateSimilarities,
            RateSimilarity = rateSimilarity,
        });
        if (!item.SimilarItems.Any())
        {
            return;
        }

        for (var x = 0; x < item.SimilarItems.Count; x++)
        {
            item.SimilarItems[x].Item.MainItem = item.MainItem;
        }

        item.MainItem.SimilarItems.AddRange(item.SimilarItems);
        item.SimilarItems.Clear();
    }
}