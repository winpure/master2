using System.Text;

namespace WinPure.Matching.Algorithm;

internal static partial class FuzzyFactory
{
    /// <summary>
    /// Jaro-Winkler is an algorithm to compare strings and evaluate the level of similarity on a range between 0 (not similar) and 1 (perfect match)
    /// The degree to which two words are similar is also known as "Distance". Jaro-Winkler tries to calculate the "Distance" between two words,
    /// which reflects the number of steps required to perform in one word in order to make it identical to the other one (steps such as:"add a character", "transpose a character", "delete a character", etc
    /// The more steps required, the closer the "Distance" is to zero. The less steps required, the closer the "Distance" is to one.
    /// For more information http://en.wikipedia.org/wiki/Jaro%E2%80%93Winkler_distance
    /// For details on the implementation http://isolvable.blogspot.co.uk/2011/05/jaro-winkler-fast-fuzzy-linkage.html
    /// We need to use the Jaro-Winkler algorithm to find possible matches between the database and Security names entered by the users in trade imports
    /// in order to suggest possible matches for the secuity names obtained from the broker.
    /// SQL has a FREETEXT function, as well as a SOUNDEX for similar purposes but they do not offer the degree of flexibility we need.
    /// FREETEXT searches for similar words, synonyms, alterations of a verb in different tenses also match, but it always has to be real words (something like McDNLDS would't match any similar word as McDnlds is not a real noun or verb)
    /// AS for SOUNDEX, it matches words that phonetically sound similar, but it doesn't help us 
    /// in more complex scenarios like "ytpe" and "type" which sound totally different however they are very similar words.
    /// </summary>
    private class WinPureFuzzy : IWinPureFuzzy
    {
        private const double WPCOLUMN_DEFAULT_MISMATCH_SCORE = 0.0;
        public double WinklerPrefix => 0.1275;

        public double CompareString(string firstWord, string secondWord)
        {
            var commonPrefixCharactersCount = GetCommonPrefix(firstWord, secondWord);
            return CompareString(firstWord, secondWord, commonPrefixCharactersCount);
        }

        public StringCondition CreateStringCondition(string value, MatchConditionBase condition, IDictionaryService dictionaryService)
        {
            return new WinPureFuzzyStringCondition(value, this, condition, dictionaryService);
        }

        /// <summary>
        /// Gets the similarity between two strings by using the Jaro-Winkler algorithm.
        /// A value of 1 means perfect match. A value of zero represents an absolute no match
        /// </summary>
        /// <param name="firstWord"></param>
        /// <param name="secondWord"></param>
        /// <param name="commonPrefixCharactersCount"></param>
        /// <returns>a value between 0-1 of the similarity</returns>
        /// 
        public double CompareString(string firstWord, string secondWord, int commonPrefixCharactersCount)
        {
            // Get half the length of the string rounded up - (this is the distance used for acceptable transpositions)
            int halfLength = Math.Min(firstWord.Length, secondWord.Length) / 2 + 1;

            // Get common characters
            var common1 = GetCommonCharacters(firstWord, secondWord, halfLength);
            var common2 = GetCommonCharacters(secondWord, firstWord, halfLength);

            int commonMatches = common1.Length < common2.Length ? common1.Length : common2.Length;

            // Check for zero in common
            if (commonMatches == 0)
            {
                //return (SqlDouble)defaultMismatchScore;
                return WPCOLUMN_DEFAULT_MISMATCH_SCORE;
            }

            // Get the number of transpositions
            int transpositions = 0;
            for (int i = 0; i < commonMatches; i++)
            {
                if (common1[i] != common2[i])
                {
                    transpositions++;
                }
            }

            // Calculate Jaro metric
            transpositions /= 2;
            double jaroMetric = (commonMatches / (double)firstWord.Length + commonMatches / (double)secondWord.Length + (commonMatches - transpositions) / (double)commonMatches) / 3.0;

            return jaroMetric + commonPrefixCharactersCount * (WinklerPrefix * (1.0 - jaroMetric));
        }

        private static int GetCommonPrefix(string compOne, string compTwo)
        {
            int cp = 0;
            for (int i = 0; i < 4; i++)
            {
                if (compOne[i] == compTwo[i]) cp++;
            }
            return cp;
        }

        /// <summary>
        /// Returns a string buffer of characters from string1 within string2 if they are of a given
        /// distance separation from the position in string1.
        /// </summary>
        /// <param name="firstWord">string one</param>
        /// <param name="secondWord">string two</param>
        /// <param name="separationDistance">separation distance</param>
        /// <returns>A string buffer of characters from string1 within string2 if they are of a given
        /// distance separation from the position in string1</returns>
        private static StringBuilder GetCommonCharacters(string firstWord, string secondWord, int separationDistance)
        {
            if (firstWord == null || secondWord == null)
            {
                return null;
            }
            var returnCommons = new StringBuilder(50);
            var copy = new StringBuilder(secondWord);
            var firstWordLength = firstWord.Length;
            var secondWordLength = secondWord.Length;

            for (var i = 0; i < firstWordLength; i++)
            {
                var character = firstWord[i];
                var found = false;

                for (var j = Math.Max(0, i - separationDistance); !found && j < Math.Min(i + separationDistance + 1, secondWordLength); j++)
                {
                    if (copy[j] != character)
                    {
                        continue;
                    }
                    found = true;
                    returnCommons.Append(character);
                    copy[j] = '#';
                }
            }
            return returnCommons;
        }

    }
}

internal interface IWinPureFuzzy : IFuzzyComparison
{
    double WinklerPrefix { get; }
    double CompareString(string firstWord, string secondWord, int commonPrefixCharactersCount);
}