using WinPure.Common.Exceptions;

namespace WinPure.Matching.Algorithm;

internal static partial class FuzzyFactory
{
    public static IFuzzyComparison GetFuzzyAlgorithm(MatchAlgorithm algorithm)
    {
        switch (algorithm)
        {
            case MatchAlgorithm.Jaro: return new Jaro();
            case MatchAlgorithm.WinPureFuzzy: return new WinPureFuzzy();
            case MatchAlgorithm.JaroWinkler: return new JaroWinkler();
            case MatchAlgorithm.ChapmanLengthDeviation: return new ChapmanLengthDeviation();
            case MatchAlgorithm.SmithWatermanGotoh: return new SmithWatermanGotoh();
            default:
                throw new WinPureArgumentException($"Fuzzy algorithm {nameof(algorithm)} with value {algorithm} not supported");
        }
    }
}