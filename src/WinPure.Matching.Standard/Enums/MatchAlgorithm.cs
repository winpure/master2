namespace WinPure.Matching.Enums;

/// <summary>
/// Define uses match algorithm
/// </summary>
[Serializable]
[System.Reflection.Obfuscation]
public enum MatchAlgorithm
{
    /// <summary>
    /// Original Jaro distance 
    /// </summary>
    Jaro = 0,
    /// <summary>
    /// WinPure fuzzy algorithm. Pay more attention to the begin of the string. Recommended.
    /// </summary>
    WinPureFuzzy = 1,
    /// <summary>
    /// Jaro-Winkler distance. Pay more attention to the begin of the string.
    /// </summary>
    JaroWinkler = 2,
    /// <summary>
    /// Used to determine if the strings are similar in size. This approach should be used alongside other approaches eg. Jaro, Jaro-winkler, Levenshtein...
    /// </summary>
    ChapmanLengthDeviation = 3,
    /// <summary>
    /// performs local sequence alignment; that is, for determining similar regions between two strings of nucleic acid sequences or protein sequences.
    /// Instead of looking at the entire sequence, the Smith–Waterman algorithm compares segments of all possible lengths and optimizes the similarity measure.
    /// </summary>
    SmithWatermanGotoh = 4
}