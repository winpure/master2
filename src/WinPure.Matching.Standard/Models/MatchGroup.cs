namespace WinPure.Matching.Models;

/// <summary>
/// Group of conditions for matching data. 
/// Two rows are considered equal when each of condition matched with rate greater then condition level and common group rate
/// (calculated from rate of conditions in that group considering conditions weight) greater then group level.
/// </summary>
[Serializable]
public class MatchGroup : MatchGroupBase
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public MatchGroup()
    {
        Conditions = new List<MatchCondition>();
    }

    /// <summary>
    /// List of group conditions
    /// </summary>
    public List<MatchCondition> Conditions { get; set; }
}