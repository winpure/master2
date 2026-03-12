using System.Threading;
using WinPure.Matching.Models;

namespace WinPure.Matching.Pipeline;

internal class MatchContext : IPipelineContext<List<Item>>
{
    public IDictionaryService DictionaryService { get; set; }
    public List<TableParameter> Tables { get; set; }
    public MatchParameter Parameter { get; set; }
    public int CurrentGroupId { get; set; }
    public CancellationToken CToken { get; set; }
    public ParallelOptions ParallelOptions { get; set; }
    public Action<string, int> OnProgress { get; set; }
    public int ProgressForGroup { get; set; }
    public int CurrentProgressDelta => ProgressForGroup * (CurrentGroupId - 1);
    public int RecordsToProcess { get; set; }
    internal IWpLogger Logger { get; set; }

    public List<Item> Items { get; set; }
    public List<Item> ItemsWithNull { get; set; }
    public List<List<Item>> Duplicates { get; set; }
    public List<Item> FirstElements { get; set; }
    public List<Item> ItemsWithoutDuplicates { get; set; }
    public List<Item> Roots { get; set; }
    public List<List<Item>> Heaps { get; set; }
    public Task<List<Item>> GetResult()
    {
        return Task.FromResult(Roots?.OrderByDescending(x => x.SimilarItems.Count).ToList() ?? new List<Item>());
    }
}