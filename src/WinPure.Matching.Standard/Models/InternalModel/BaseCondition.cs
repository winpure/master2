using System.Collections;
using WinPure.Configuration.Helper;

namespace WinPure.Matching.Models.InternalModel;

/// <summary>
/// Base class for all conditions. Condition of that type could not be created
/// </summary>
abstract class BaseCondition : IConditionValue
{
    private readonly IDictionaryService _dictionaryService;

    /// <summary>
    /// Default constructor. 
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="dictionaryService"></param>
    protected BaseCondition(MatchConditionBase condition, IDictionaryService dictionaryService)
    {
        _dictionaryService = dictionaryService;
        Condition = condition;
    }

    /// <summary>
    /// minimum rate, when the two values are considered equivalent
    /// </summary>
    public double MinRate => Condition.Level;

    /// <summary>
    /// Matching condition
    /// </summary>
    public MatchConditionBase Condition { get; }

    /// <summary>
    /// Matching value with handling NULL values depends from IgnoreNullOption
    /// </summary>
    public abstract object ValueWithNullHandling { get; }

    /// <summary>
    /// Matching value
    /// </summary>
    public abstract object Value { get; }

    /// <summary>
    /// Type of matching condition
    /// </summary>
    public MatchType ConditionType => Condition.MatchingType;

    /// <summary>
    /// Match two values and return rate. 
    /// </summary>
    /// <param name="obj">Second value to compare</param>
    /// <param name="coefficientDetail">Coefficient , how strong equality should be</param>
    /// <returns>Coefficient of equality</returns>
    public abstract double Compare(IConditionValue obj, int coefficientDetail);

    /// <summary>
    /// Determines whether the specified objects are equal.
    /// </summary>
    /// <returns>
    /// true if the specified objects are equal; otherwise, false.
    /// </returns>
    /// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param><exception cref="T:System.ArgumentException"><paramref name="x"/> and <paramref name="y"/> are of different types and neither one can handle comparisons with the other.</exception>
    bool IEqualityComparer.Equals(object x, object y)
    {
        var b1 = (BaseCondition)x;
        var b2 = (BaseCondition)y;
        return b1.Value.Equals(b2.Value);
    }

    /// <summary>
    /// Returns a hash code for the specified object.
    /// </summary>
    /// <returns>
    /// A hash code for the specified object.
    /// </returns>
    /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
    public virtual int GetHashCode(object obj)
    {
        return Value.GetHashCode();
    }

    internal string ProcessDictionary(string val, string dictionaryName)
    {
        var res = val.ToLower();

        if (!dictionaryName.IsWpDictionary())
        {
            return res;
        }

        var dictionary = _dictionaryService.GetDictionary(dictionaryName).Result;

        return res.ApplyDictionary(dictionary);
    }

    internal virtual (double?, string, string) CompareBase(IConditionValue obj)
    {
        var result = (double?)null;

        var valueToCompare = obj.Value?.ToString() ?? "";
        var conditionValue = Value?.ToString() ?? "";

        if (!Condition.IncludeNullValues)
        {
            if (obj.Value == null || Value == null)
            {
                result = 0;
                return (result, conditionValue, valueToCompare);
            }
        }

        if (string.Compare(conditionValue, valueToCompare, StringComparison.InvariantCultureIgnoreCase) == 0)
        {
            result = 1;
        } 
        else if (string.IsNullOrEmpty(conditionValue) || string.IsNullOrEmpty(valueToCompare) || conditionValue.Length < 4 || valueToCompare.Length < 4)
        {
            result = - 1;
        }

        return (result, conditionValue, valueToCompare);
    }
}