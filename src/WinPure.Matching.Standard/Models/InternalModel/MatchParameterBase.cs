namespace WinPure.Matching.Models.InternalModel;

/// <summary>
/// Base class for Match and Search parameters
/// </summary>
[Serializable]
public abstract class MatchParameterBase
{
    /// <summary>
    /// Selected fuzzy algorithm
    /// </summary>
    public MatchAlgorithm FuzzyAlgorithm { get; set; } = MatchAlgorithm.WinPureFuzzy;
}