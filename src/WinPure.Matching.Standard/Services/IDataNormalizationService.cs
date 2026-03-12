using System.Data;
using WinPure.Matching.Models;
using WinPure.Matching.Models.Support;

namespace WinPure.Matching.Services;

internal interface IDataNormalizationService
{
    bool DefineMasterRecord(DataTable matchResult, MatchParameter lastMatchingParameters, MasterRecordSettings settings);

    /// <summary>
    /// Delete rows from the match result accrding to mergeSettings.
    /// </summary>
    /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
    /// <param name="deleteSettings">Delete setting</param>
    /// <returns>New data table with match result. Existing match result table should be overwriting</returns>
    DataTable DeleteMergeMatchResult(DataTable matchResult, DeleteFromMatchResultSetting deleteSettings);

    /// <summary>
    /// Merging of match result.
    /// </summary>
    /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
    /// <param name="mergeSettings">Merge settings for column from match result</param>
    /// <param name="valueSeparator">character o string to split values from merged rows</param>
    /// <param name="reportProgress">action called to report about task progress</param>
    /// <returns></returns>
    DataTable MergeMatchResult(DataTable matchResult, List<MergeMatchResultSetting> mergeSettings, string valueSeparator, Action<string, int> reportProgress);

    /// <summary>
    /// Update the result according to specified mergeSettings. 
    /// </summary>
    /// <param name="matchResult">Datatable with full matching result. It should be same table that was returned by MatchData function.</param>
    /// <param name="updateSettings">Update settings for columns from match result</param>
    /// <param name="reportProgress">action called to report about task progress</param>
    void UpdateMatchResult(DataTable matchResult, List<UpdateMatchResultSetting> updateSettings, Action<string, int> reportProgress);

    /// <summary>
    /// Update the match result groups according to specified mergeSettings. 
    /// </summary>
    /// <param name="matchGroups">Datatable with matching result groups only.</param>
    /// <param name="updateSettings">Update settings for columns from match result</param>
    /// <param name="reportProgress">action called to report about task progress</param>
    void UpdateMatchGroups(DataTable matchGroups, List<UpdateMatchResultSetting> updateSettings, Action<string, int> reportProgress);

    /// <summary>
    /// Remove not duplicate rows from match result.
    /// </summary>
    /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
    void RemoveNotDuplicateRecords(DataTable matchResult);

    /// <summary>
    /// Create a new group of duplicate from selected records.
    /// </summary>
    /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
    void CreateNewDuplicateGroup(DataTable matchResult);
}