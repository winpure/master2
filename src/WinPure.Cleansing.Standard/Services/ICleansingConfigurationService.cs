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

    void FillCleansingConfigurationTables(DataTable dt);

    DataTable GetDataTableCleanSettings(ImportedDataInfo importedDataInfo);
    DataTable ConvertCleanSettingsToDataTable(ImportedDataInfo importedDataInfo, WinPureCleanSettings cleanSettings);
    WinPureCleanSettings GetWinPureCleanSettings(ImportedDataInfo importedDataInfo);

    void ClearCleanSettings(ImportedDataInfo importedDataInfo);
    void SaveCleanSettings(ImportedDataInfo importedDataInfo, string columnName, string fieldName, object value);

    DataTable GetColumnUniqueValues(ImportedDataInfo importedDataInfo, string columnName, bool refreshData, TextCleanerSetting cleanerSett = null, CaseConverterSetting caseSett = null);
    List<WordManagerSetting> GetWordManagerSettings(ImportedDataInfo importedDataInfo, string columnName);

    void SaveWordManagerSettings(ImportedDataInfo importedDataInfo, List<WordManagerSetting> wmWordManagerSettings, string columnName);
    List<ReportResultData> GetWordManagerColumnValues(ImportedDataInfo importedDataInfo, string columnName);
}