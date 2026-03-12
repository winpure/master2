using WinPure.Common.Helpers;

namespace WinPure.Matching.Enums;

/// <summary>
/// Define which record would be deleted from match result
/// </summary>
public enum DeleteMatchResultSetting
{
    /// <summary>
    /// Delete All Matching records
    /// </summary>
    [DisplayName("Delete All Matching records")]
    AllMatching = 0,
    /// <summary>
    /// Delete non master (leave Master Records)
    /// </summary>
    [DisplayName("Delete non master (leave Master Records)")]
    NonMaster = 1,
    /// <summary>
    /// Delete all Selected records
    /// </summary>
    [DisplayName("Delete all Selected records")]
    AllSelected = 2,
    /// <summary>
    /// Delete Non-matching
    /// </summary>
    [DisplayName("Delete Non-matching")]
    NonMatching = 3
}