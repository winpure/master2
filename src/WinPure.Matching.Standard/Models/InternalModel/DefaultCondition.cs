namespace WinPure.Matching.Models.InternalModel;

/// <summary>
///  Default condition for all data types, which should be compare directly. For example DateTime, any numeric types, strings, when string comparing for equal
/// </summary>
internal class DefaultCondition : BaseCondition
{
    private readonly object _value;

    /// <summary>
    /// default constructor.
    /// </summary>
    /// <param name="value">Value to compare</param>
    /// <param name="condition">Matching condition</param>
    /// <param name="dictionaryService"></param>
    public DefaultCondition(object value, MatchConditionBase condition, IDictionaryService dictionaryService) : base(condition, dictionaryService)
    {
        _value = value is string
            ? ProcessDictionary(value.ToString().ToLower(), condition.DictionaryType)
            : value;

    }

    /// <summary>
    /// Matching value
    /// </summary>
    public override object Value => _value;

    /// <summary>
    /// Matching value with handling of Ignore NULL value option
    /// </summary>
    public override object ValueWithNullHandling => Condition.IncludeNullValues
        ? _value ?? ""
        : _value ?? Guid.NewGuid().ToString("N");

    /// <summary>
    /// Compare two values. 
    /// </summary>
    /// <param name="obj">Second value to compare</param>
    /// <param name="coefficientDetail">not used</param>
    /// <returns>Coefficient of equality: 1 if values are equal, 0 in otherwise</returns>
    public override double Compare(IConditionValue obj, int coefficientDetail)
    {
        if (!Condition.IncludeNullValues)
        {
            if (obj.Value == null || Value == null) return 0;
        }

        var valueToCompare = obj.ValueWithNullHandling.ToString();
        var conditionValue = ValueWithNullHandling.ToString();

        return string.Compare(conditionValue, valueToCompare, StringComparison.InvariantCultureIgnoreCase) == 0 ? 1 : 0;
    }
}