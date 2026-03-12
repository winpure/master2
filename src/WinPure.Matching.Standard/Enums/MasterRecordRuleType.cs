namespace WinPure.Matching.Enums;

/// <summary>
/// Master record rule type
/// </summary>
public enum MasterRecordRuleType
{
    /// <summary>
    /// If field should be null or empty
    /// </summary>
    IsEmpty,
    /// <summary>
    /// If field should be equal to value
    /// </summary>
    IsEqual,
    /// <summary>
    /// if field should contain value
    /// </summary>
    IsContains,
    /// <summary>
    /// Get the record with maximal value in the field (Date or numeric fields)
    /// </summary>
    IsMaximum,
    /// <summary>
    /// Get the record with minimal value in the field (Date or numeric fields)
    /// </summary>
    IsMinimum,
    /// <summary>
    ///  Get the record with longest value in the field (string field)
    /// </summary>
    IsLongest,
    /// <summary>
    /// Get the record with shortest value in the field (string field)
    /// </summary>
    IsShortest,
    /// <summary>
    /// Get the record where field greater then value (Date or numeric fields)
    /// </summary>
    GreaterThan,
    /// <summary>
    /// Most common value in the group (if multiple values have equal frequency - then first of them)
    /// </summary>
    Common
}