using WinPure.Matching.Algorithm;

namespace WinPure.Matching.Models.InternalModel;

/// <summary>
/// Matching condition for strings
/// </summary>
abstract class StringCondition : BaseCondition
{
    private readonly string _value;
    internal IFuzzyComparison _comparison;

    /// <summary>
    /// String condition for matching. 
    /// </summary>
    /// <param name="value">String value for matching</param>
    /// <param name="condition">Match condition</param>
    /// <param name="dictionaryService"></param>
    protected StringCondition(string value, MatchConditionBase condition, IDictionaryService dictionaryService) : base(condition, dictionaryService)
    {
        _value = value == null ? value : ProcessDictionary(value, condition.DictionaryType);
    }

    /// <summary>
    /// Array of characters, which present in the value
    /// </summary>
    public char[] CharMapKey { get; set; }

    /// <summary>
    /// Array of characters count, which present in the value. 
    /// </summary>
    public ushort[] CharMapValue { get; set; }

    /// <summary>
    /// Matching value with considering of ignore NULL option
    /// </summary>
    public override object ValueWithNullHandling => Condition.IncludeNullValues
        ? _value ?? ""
        : _value ?? Guid.NewGuid().ToString("N");

    /// <summary>
    /// Matching value
    /// </summary>
    public override object Value => _value;
}