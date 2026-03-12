namespace WinPure.Matching.Enums;

/// <summary>
/// Possible types of matching
/// </summary>
[Serializable]
public enum MatchType
{
    /// <summary>
    /// Direct compare of two strings or another types
    /// </summary>
    DirectCompare = 0,
    /// <summary>
    /// Compare two strings using fuzzy algorithm
    /// </summary>
    Fuzzy = 1
}