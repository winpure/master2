namespace WinPure.Matching.Algorithm;

internal static partial class FuzzyFactory
{
    private class Jaro : IFuzzyComparison
    {
        private const double WPCOLUMN_DEFAULT_MISMATCH_SCORE = 0.0;
        private const double WPCOLUMN_DEFAULT_MATCH_SCORE = 1.0;

        private readonly Standard.Data.StringMetrics.Jaro _jaro;

        public Jaro()
        {
            _jaro = new Standard.Data.StringMetrics.Jaro();
        }

        /// <summary>
        /// Gets the similarity between two strings by using the Jaro-Winkler algorithm.
        /// A value of 1 means perfect match. A value of zero represents an absolute no match
        /// </summary>
        /// <param name="firstWord"></param>
        /// <param name="secondWord"></param>
        /// <returns>a value between 0-1 of the similarity</returns>
        /// 
        public double CompareString(string firstWord, string secondWord)
        {
            if (string.IsNullOrEmpty(firstWord) || string.IsNullOrEmpty(secondWord))
            {
                return WPCOLUMN_DEFAULT_MISMATCH_SCORE;
            }

            if (string.Equals(firstWord, secondWord, StringComparison.InvariantCultureIgnoreCase))
            {
                return WPCOLUMN_DEFAULT_MATCH_SCORE;
            }

            return _jaro.GetSimilarity(firstWord, secondWord);
        }

        public StringCondition CreateStringCondition(string value, MatchConditionBase condition, IDictionaryService dictionaryService)
        {
            return new JaroStringCondition(value, this, condition, dictionaryService);
        }
    }
}