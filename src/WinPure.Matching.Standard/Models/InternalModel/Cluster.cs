namespace WinPure.Matching.Models.InternalModel;

/// <summary>
/// Group matched items by specific criteria
/// </summary>
internal struct Cluster
{

    public Cluster(Item root)
    {
        Roots = new List<Item> { root };
        BrokenLinks = new List<ItemSimilarity>();
        HeapFullCheck = new List<Item>();
        HeapBrokenLinks = new List<Item>();
    }
    public List<Item> HeapFullCheck { get; set; }
    public List<Item> HeapBrokenLinks { get; set; }
    public List<Item> Roots { get; set; }
    public List<ItemSimilarity> BrokenLinks { get; set; }
}