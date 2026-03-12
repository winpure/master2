namespace WinPure.Matching.Models;

/// <summary>
/// That class describe one matching condition i.e. field (or list of corresponding fields from multiple tables), 
/// type of comparing for values in that field, level, when two values considered equal, used dictionary for that field
/// </summary>
[Serializable]
public class MatchCondition : MatchConditionBase
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public MatchCondition()
    {
        Fields = new List<MatchField>();
    }

    /// <summary>
    /// List of field for matching.
    /// </summary>
    public List<MatchField> Fields { get; set; }
}