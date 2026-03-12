namespace WinPure.Matching.Models.Support;

/// <summary>
/// Define one rule for master record definition
/// </summary>
[Serializable]
public class MasterRecordRule
{
    /// <summary>
    /// Field from match result
    /// </summary>
    public string FieldName { get; set; }
    /// <summary>
    /// System type of the field (i.e. System.String)
    /// </summary>
    public string FieldType { get; set; }
    /// <summary>
    /// If true - then use this rule with logical NOT
    /// </summary>
    public bool Negate { get; set; }
    /// <summary>
    /// Rule value
    /// </summary>
    public string Value { get; set; }
    /// <summary>
    /// Type of rule
    /// </summary>
    public MasterRecordRuleType RuleType { get; set; }
}