using System.Collections.Generic;
using System.Data;
using WinPure.Common.Models;
using WinPure.Configuration.Models.Configuration;

namespace WinPure.Cleansing.Services;

internal interface ICleansingConfigurationService
{
    /// <summary>
    /// Export clean settings to json file
    /// </summary>
    /// <param name="settings">WinPure clean settings</param>
    /// <param name="destinationPath">Destination folder</param>
    /// <param name="fileName">File name (without extension).</param>
    void ExportCleanSettings(WinPureCleanSettings settings, string fileName);

    /// <summary>
    /// Import WinPure clean settings from json file
    /// </summary>
    /// <param name="destinationFile">Json file with settings</param>
    /// <returns></returns>
    WinPureCleanSettings ImportCleanSettings(string destinationFile);

    /// <summary>
    /// Export proper case configuration to json file
    /// </summary>
    /// <param name="settings">WinPure proper case configuration</param>
    /// <param name="destinationPath">Destination folder</param>
    /// <param name="fileName">File name (without extension).</param>
    void ExportProperCaseConfiguration(ProperCaseConfiguration settings, string fileName);

    /// <summary>
    /// Import WinPure proper case configuration from json file
    /// </summary>
    /// <param name="destinationFile">Json file with proper case configuration</param>
    /// <returns></returns>
    ProperCaseConfiguration ImportProperCaseConfiguration(string destinationFile);

    void FillCleansingConfigurationTables(DataTable dt);

    DataTable GetDataTableCleanSettings(string tableName);
    DataTable ConvertCleanSettingsToDataTable(string tableName, WinPureCleanSettings cleanSettings);
    WinPureCleanSettings GetWinPureCleanSettings(string tableName);

    void ClearCleanSettings(string tableName);
    void SaveCleanSettings(string tableName, string columnName, string fieldName, object value);

    DataTable GetColumnUniqueValues(string tableName, string columnName, bool refreshData, TextCleanerSetting cleanerSett = null, CaseConverterInternalSetting caseSett = null);
    List<WordManagerSetting> GetWordManagerSettings(string tableName, string columnName);

    void SaveWordManagerSettings(string tableName, List<WordManagerSetting> wmWordManagerSettings, string columnName);
    List<ReportResultData> GetWordManagerColumnValues(string tableName, string columnName);
}