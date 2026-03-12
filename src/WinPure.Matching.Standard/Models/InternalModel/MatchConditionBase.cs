namespace WinPure.Matching.Models.InternalModel;

/// <summary>
/// Base condition for match and search conditions
/// </summary>
[Serializable]
public abstract class MatchConditionBase
{
    /// <summary>
    /// Minimal rate, when two values define as equal.
    /// </summary>
    public double Level { get; set; } = 0.95;

    /// <summary>
    /// Weight of that condition in the total group result, if group contains more then one condition
    /// </summary>
    public double Weight { get; set; } = 1;

    /// <summary>
    /// Dictionary, which linked to that field. 
    /// </summary>
    public string DictionaryType { get; set; }

    /// <summary>
    /// How the values should be matched.
    /// </summary>
    public MatchType MatchingType { get; set; }

    /// <summary>
    /// If true, NULL value in this column would be calculates same as empty value.
    /// </summary>
    public bool IncludeNullValues { get; set; } = false;

    /// <summary>
    /// If true, all rows which contains empty value in that column will be added to matching and all empty values would be the same
    /// </summary>
    public bool IncludeEmpty { get; set; } = false;
}