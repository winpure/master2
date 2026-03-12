namespace WinPure.Matching.Models.InternalModel;

internal class ItemSimilarity
{
    public ItemSimilarity()
    {
        RateSimilarities = new List<double>();
    }

    public Item Item { get; set; }
    public Item SimilarItem { get; set; }
    public GroupValue MatchGroupValue { get; set; }
    public List<double> RateSimilarities { get; set; }
    public double RateSimilarity { get; set; }
    public double MinGroupRate => MatchGroupValue?.MinGroupRate ?? 1;
}