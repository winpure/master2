using System.Data;
using WinPure.Matching.Models;
using WinPure.Matching.Models.Reports;

namespace WinPure.Matching.Services;

internal interface IRepresentationService
{
    /// <summary>
    /// Convert DataTable with match result to the specific view, defined with second parameters
    /// </summary>
    /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
    /// <param name="viewType">Specify the view type of match result</param>
    /// <returns>New datatable according to selected view type. Match result table should not be overwriting.</returns>
    DataTable GetMatchResult(DataTable matchResult, MatchResultViewType viewType);

    /// <summary>
    /// Options table for merge match result opration
    /// </summary>
    /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
    /// <returns>Data table with options populated with default values</returns>
    DataTable GetMergeMatchResultOptionsTable(DataTable matchResult);

    /// <summary>
    /// Options table match/update opration
    /// </summary>
    /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
    /// <returns>Data table with options populated with default values</returns>
    DataTable GetUpdateMatchResultOptionsTable(DataTable matchResult);

    /// <summary>
    /// Build report common data for specific match result view
    /// </summary>
    /// <param name="matchResult">Match result view</param>
    /// <returns></returns>
    ReportCommonData GetMatchReportCommonData(DataTable matchResult);

    /// <summary>
    /// Build the report for given match result.
    /// </summary>
    /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
    /// <returns></returns>
    ReportData GetMatchReportData(DataTable matchResult);

    MatchParameter ConvertMatchSettingsViewToMatchParameters(MatchSettingsViewModel settings, int searchDeep, MatchAlgorithm mAlgorithm);

    void ReindexGroups(MatchSettingsViewModel matchSettings, int removedGroupId);
}