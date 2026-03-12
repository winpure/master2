using WinPure.Matching.Algorithm;

namespace WinPure.Matching.Models.InternalModel;

/// <summary>
/// Matching condition for strings
/// </summary>
internal class JaroWinklerStringCondition : StringCondition
{
    /// <summary>
    /// String condition for matching. 
    /// </summary>
    /// <param name="value">String value for matching</param>
    /// <param name="comparison">Fuzzy algorithm for compare</param>
    /// <param name="condition">Match condition</param>
    public JaroWinklerStringCondition(string value, IFuzzyComparison comparison, MatchConditionBase condition, IDictionaryService dictionaryService) : base(value, condition, dictionaryService)
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
        var (result, conditionValue, valueToCompare) = CompareBase(obj);

        return result.HasValue ? result.Value :  _comparison.CompareString(conditionValue, valueToCompare);
    }
}