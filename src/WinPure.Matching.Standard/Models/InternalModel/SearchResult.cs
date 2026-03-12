
using System.Data;

namespace WinPure.Matching.Models.InternalModel;

public class SearchResult
{
    public long Key { get; set; }
    public int GroupId { get; set; }
    public double GroupLevel { get; set; }
    public List<double> ConditionScores { get; set; }
    public List<bool> ConditionPass { get; set; }
    public double Score { get; set; }
    public DataRow Row { get; set; }
}