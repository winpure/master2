namespace WinPure.Matching.Algorithm;

internal static partial class FuzzyFactory
{
    private class ChapmanLengthDeviation : IFuzzyComparison
    {
        private readonly Standard.Data.StringMetrics.ChapmanLengthDeviation _chapmanLengthDeviation;

        public ChapmanLengthDeviation()
        {
            _chapmanLengthDeviation = new Standard.Data.StringMetrics.ChapmanLengthDeviation();
        }

        public double CompareString(string firstWord, string secondWord)
        {
            return _chapmanLengthDeviation.GetSimilarity(firstWord, secondWord);
        }

        public StringCondition CreateStringCondition(string value, MatchConditionBase condition, IDictionaryService dictionaryService)
        {
            return new ChapmanLengthDeviationStringCondition(value, this, condition, dictionaryService);
        }
    }
}