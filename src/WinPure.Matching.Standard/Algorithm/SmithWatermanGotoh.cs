namespace WinPure.Matching.Algorithm;

internal static partial class FuzzyFactory
{
    private class SmithWatermanGotoh : IFuzzyComparison
    {
        private readonly Standard.Data.StringMetrics.SmithWatermanGotoh _smithWatermanGotoh;

        public SmithWatermanGotoh()
        {
            _smithWatermanGotoh = new Standard.Data.StringMetrics.SmithWatermanGotoh();
        }

        public double CompareString(string firstWord, string secondWord)
        {
            return _smithWatermanGotoh.GetSimilarity(firstWord, secondWord);
        }

        public StringCondition CreateStringCondition(string value, MatchConditionBase condition, IDictionaryService dictionaryService)
        {
            return new SmithWatermanGotohStringCondition(value, this, condition, dictionaryService);
        }
    }
}