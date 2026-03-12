using WinPure.Common.Helpers;

namespace WinPure.Matching.Enums;

/// <summary>
/// Action for the match result field on updating
/// </summary>
public enum UpdateOperationType
{
    /// <summary>
    /// Do not update given field
    /// </summary>
    [DisplayName("Not updated")]
    NotUpdate = 0,
    /// <summary>
    /// Update field with most popular value 
    /// </summary>
    [DisplayName("Most popular value")]
    MostPopular = 1,
    /// <summary>
    /// update field with value from master record
    /// </summary>
    [DisplayName("From Master record")]
    FromMaster = 2,
}