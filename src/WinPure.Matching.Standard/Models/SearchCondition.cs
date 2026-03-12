namespace WinPure.Matching.Models;

/// <summary>
/// Search condition
/// </summary>
[Serializable]
public class SearchCondition : MatchConditionBase
{
    /// <summary>
    /// Value we want to search in given field
    /// </summary>
    public object SearchValue;

    /// <summary>
    /// List of field for matching.
    /// </summary>
    public MatchField SearchField { get; set; }
}