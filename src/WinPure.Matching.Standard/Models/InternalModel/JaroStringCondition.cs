using WinPure.Matching.Algorithm;

namespace WinPure.Matching.Models.InternalModel;

/// <summary>
/// Matching condition for strings
/// </summary>
internal class JaroStringCondition : StringCondition
{
    private IWinPureFuzzy _winPureFuzzy => (IWinPureFuzzy)_comparison;

    /// <summary>
    /// String condition for matching. 
    /// </summary>
    /// <param name="value">String value for matching</param>
    /// <param name="comparison">Fuzzy algorithm for compare</param>
    /// <param name="condition">Match condition</param>
    public JaroStringCondition(string value, IFuzzyComparison comparison, MatchConditionBase condition, IDictionaryService dictionaryService) : base(value, condition, dictionaryService)
    {
        _comparison = comparison;
    }

    /// <summary>
    /// Match two values and return rate. 
    ///  </summary>
    /// <param name="obj">Second value to compare</param>
    /// <param name="coefficientDetail">Coefficient , how strong equality should be</param>
    /// <returns>Coefficient of equality</returns>
    public override double Compare(IConditionValue obj, int coefficientDetail)
    {
        if (!Condition.IncludeNullValues)
        {
            if (obj.Value == null || Value == null) return 0;
        }

        var valueToCompare = obj.Value?.ToString() ?? "";
        var conditionValue = Value?.ToString() ?? "";


        if (string.Compare(conditionValue, valueToCompare, StringComparison.InvariantCultureIgnoreCase) == 0)
        {
            return 1;
        }

        if (string.IsNullOrEmpty(conditionValue) || string.IsNullOrEmpty(valueToCompare) 
                                                 || conditionValue.Length < 4 || valueToCompare.Length < 4)
        {
            return -1;
        }

        if (coefficientDetail <= 0)
        {
            return _comparison.CompareString(conditionValue, obj.Value.ToString());
        }

        var charMapKey1 = ((JaroStringCondition)obj).CharMapKey;
        var charMapValue1 = ((JaroStringCondition)obj).CharMapValue;
        var commonChar = CompareDict1(CharMapKey, CharMapValue, charMapKey1, charMapValue1);

        var jEst = 1.0 / 3 + 1.0 / 3 * commonChar / valueToCompare.Length + 1.0 / 3 * commonChar / conditionValue.Length;

        int? commonPref = valueToCompare[0] != conditionValue[0]
            ? 0
            : valueToCompare[1] != conditionValue[1]
                ? 1
                : valueToCompare[2] != conditionValue[2]
                    ? 2
                    : valueToCompare[3] != conditionValue[3] ? 3 : 4;


        var rate = jEst + commonPref.Value * _winPureFuzzy.WinklerPrefix * (1 - jEst);

        if (rate < Condition.Level * coefficientDetail / 100.0)
        {
            return 0;
        }

        return _comparison.CompareString(conditionValue, obj.Value.ToString());
    }

    private static int CompareDict1(char[] k1, ushort[] v1, char[] k2, ushort[] v2)
    {
        int i = 0;
        int j = 0;
        int commonChar = 0;
        while (true)
        {
            if (i >= k1.Length || j >= k2.Length)
            {
                return commonChar;
            }
            if (k1[i] == k2[j])
            {
                commonChar += Math.Min(v1[i], v2[j]);
                i++;
                j++;
            }
            else if (k1[i] > k2[j])
            {
                j++;
            }
            else
            {
                i++;
            }
        }
    }
}