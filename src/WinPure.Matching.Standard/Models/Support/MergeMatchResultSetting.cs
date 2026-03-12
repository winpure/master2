namespace WinPure.Matching.Models.Support;

/// <summary>
/// Define settings for merging match result
/// </summary>
public class MergeMatchResultSetting : MatchResultSettingBase
{
    /// <summary>
    /// n merging should we override only empty fields in master record with values from another records or we should override value in master record
    /// </summary>
    public bool OnlyEmpty { get; set; }

    public bool UpdateField { get; set; }
    /// <summary>
    /// Should we merge all values from other records in the group to one string
    /// </summary>
    public bool KeepAllValues { get; set; }
}