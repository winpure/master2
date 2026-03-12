namespace WinPure.Matching.Enums;

/// <summary>
/// How master record should be defined
/// </summary>
public enum MasterRecordType
{
    /// <summary>
    /// Get the record which has data in most amount of fields as master record
    /// </summary>
    MostPopulatedByTable = 1,
    /// <summary>
    /// Get record which matched with all other records in the group with higher average rate as master record
    /// </summary>
    MostRelevant = 2,
    /// <summary>
    /// Remove all master record marks from data set
    /// </summary>
    ClearAll = 3
}