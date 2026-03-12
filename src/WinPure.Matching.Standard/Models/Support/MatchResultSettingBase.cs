namespace WinPure.Matching.Models.Support;

/// <summary>
/// Base class for match result normalization settings
/// </summary>
public abstract class MatchResultSettingBase
{
    /// <summary>
    /// Specify field name from match result
    /// </summary>
    public string FieldName { get; set; }
}