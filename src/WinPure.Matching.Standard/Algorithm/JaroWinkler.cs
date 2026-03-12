namespace WinPure.Matching.Algorithm;

internal static partial class FuzzyFactory
{
    private class JaroWinkler : IFuzzyComparison
    {
        private readonly Standard.Data.StringMetrics.JaroWinkler _jaroWinkler;

        public JaroWinkler()
        {
            _jaroWinkler = new Standard.Data.StringMetrics.JaroWinkler();
        }

        public double CompareString(string firstWord, string secondWord)
        {
            return _jaroWinkler.GetSimilarity(firstWord, secondWord);
        }

        public StringCondition CreateStringCondition(string value, MatchConditionBase condition, IDictionaryService dictionaryService)
        {
            return new JaroWinklerStringCondition(value, this, condition, dictionaryService);
        }
    }
}