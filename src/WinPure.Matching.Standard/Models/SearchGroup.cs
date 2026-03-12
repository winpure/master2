namespace WinPure.Matching.Models;

/// <summary>
/// Search group
/// </summary>
[Serializable]
public class SearchGroup : MatchGroupBase
{
    public SearchGroup()
    {
        Conditions = new List<SearchCondition>();
    }

    /// <summary>
    /// List of group conditions
    /// </summary>
    public List<SearchCondition> Conditions { get; set; }
}