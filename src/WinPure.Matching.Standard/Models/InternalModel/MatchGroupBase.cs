namespace WinPure.Matching.Models.InternalModel;

/// <summary>
/// Base class for matching and searching groups
/// </summary>
[Serializable]
public abstract class MatchGroupBase
{
    /// <summary>
    /// number of the group. There could not be two groups with similar IDs
    /// </summary>
    public int GroupId { get; set; } = 1;

    /// <summary>
    /// Minimal rate when two rows are considered equal.
    /// </summary>
    public double GroupLevel { get; set; } = 0.9;
}