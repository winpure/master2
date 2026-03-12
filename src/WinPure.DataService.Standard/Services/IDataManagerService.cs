using System.Data;
using System.Threading;
using WinPure.AddressVerification.Models;
using WinPure.Cleansing.Models;
using WinPure.Common.Abstractions;
using WinPure.Configuration.Models.Configuration;
using WinPure.DataService.Enums;
using WinPure.DataService.Senzing.Models;
using WinPure.Integration.Abstractions;
using WinPure.Matching.Enums;
using WinPure.Matching.Models;
using WinPure.Matching.Models.Reports;
using WinPure.Matching.Models.Support;

namespace WinPure.DataService.Services;

internal interface IDataManagerService : IWpServiceBase, IDisposable
{
    event Action<string> OnAddNewData;
    event Action<string> OnTableDataUpdateBegin;
    event Action<string> OnTableDataUpdateComplete;
    event Action<string> OnCurrentTableChanged;
    event Action<ImportedDataInfo> OnTableDelete;
    event Action<bool, bool, MatchResultOperation> OnMatchResultReady;
    event Action<object, int, bool> OnStatisticUpdated;
    event Action<string, FiltrateField> OnFiltrateData;
    event Action<string, string> OnChangeTableDisplayName;
    event Action<ImportedDataInfo> OnRefreshData;
    event Action<string, bool> OnAddressVerificationReady;
    event Action OnEntityResolutionStart;
    event Action OnEntityResolutionReady;

    MatchSettingsViewModel MatchSettings { get; set; }
    MatchParameter LastMatchingParameters { get; set; }
    MasterRecordSettings LastMasterRecordSettings { get; set; }
    MasterRecordSettings LastErMasterRecordSettings { get; set; }
    bool IsImportAllowed { get; }
    bool IsAnyTable { get; }
    DataTable MatchResult { get; set; }
    DataTable SelectedColumns { get; set; }
    string CurrentTable { get; set; }
    ImportedDataInfo GetCurrentTableInfo { get; }

    string ProjectName { get; set; }
    ReportData ReportData { get; set; }

    List<ImportedDataInfo> TableList { get; }
    ImportedDataInfo GetTableInfo(string tableName);
    ImportedDataInfo GetTableInfoByDisplayName(string displayName);
    Dictionary<string, AddressVerificationReport> VerificationResults { get; set; }
    EntityResolutionReport EntityResolutionReportData { get; set; }

    void SetSelectedTable(string tableName);
    void DeleteTable(string tableName);


    void SaveResultToData(string resultName, DataTable data);
    void SaveResultToData(bool removeSystemFields, MatchResultViewType viewType, string resultName, DataTable data = null);
    void ImportData(IImportProvider importProvider);
    void ExportData(IExportProvider exportProvider, DataTable exportData = null, string filter = "", bool removeSystemFields = false);
    void ReimportDataAsync(string tableName);
    void ReimportData(string tableName);
        

    void RaiseAddNewData(string tableName);
    void UpdateSelectedForMatchingTables(string tableName);
        

    void AddTableColumn(string tableName, string columnName, string columnType);
    void RenameColumn(string tableName, List<DataField> databaseColumns);
    void CopyColumn(string tableName, List<DataField> databaseColumns);
    void DeleteRecords(string tableName);
    void RemoveColumn(string tableName, List<DataField> databaseColumns);
    void FiltrateMainData(string columnName, FiltrateField filter);
    void ChangeTableDisplayName(string tableName, string newDisplayName);
    List<string> GetTableColumnsByDisplayName(string tableDisplayName);


    void UpdateTableStatistic(string tableName);
    DataTable GetTableStatistic(string tableName);

    void SaveCleanMatrix(string fileName);
    void LoadCleanMatrix(string fileName);
    void SaveProperCaseConfiguration(string fileName, ProperCaseConfiguration configuration);
    ProperCaseConfiguration LoadProperCaseConfiguration(string fileName);

    DataTable GetCleanedPreviewTable(string tableName);
    DataTable GetDataTableCleanSettings(string tableName);
    void ClearCleanSettings(string tableName);
    void SaveCleanSettings(string columnName, string fieldName, object value);
    DataTable GetColumnUniqueValues(string columnName, bool refreshData, TextCleanerSetting cleanerSett = null, CaseConverterInternalSetting caseSett = null);
    void ExportDataWithUniqueValues(string fileName, string tableName, string columnName, List<string> values);
    List<WordManagerSetting> GetWordManagerSettings(string columnName);
    List<ReportResultData> GetWordManagerColumnValues(string columnName);
    void SaveWordManagerSettings(List<WordManagerSetting> wmWordManagerSettings, string columnName);
    void CleanData(CancellationToken cancellationToken);
    void CleanDataAsync();
    void UndoClean();
    void CheckCleansingAi(string tableName);
    void UpdateCleansingOptionsBasedOnAi(string tableName);

    AddressVerificationReport GetAddressVerificationResult(string tableName);
    DataTable GetDataTableAddressVerificationSetting(string tableName);
    void StartAddressVerification(string tableName, AddressVerificationSettings verificationSettings);


    List<FieldType> GetEntityResolutionFieldTypes();
    DataTable GetDataTableEntityResolutionSetting(string tableName);
    void CleanEntityResolutionSetting(string tableName);
    void SetDefaultEntityResolutionMapping(string tableName);
    void SetErMatchResultIsSelected(bool value, string whereCause, bool raiseUpdate = true);
    List<EntityResolutionConfiguration> VerifyFullEntityResolutionMap(IEnumerable<ImportedDataInfo> tables);
    EntityResolutionConfiguration VerifyEntityResolutionTableMap(ImportedDataInfo table);
    void RunEntityResolution(CancellationTokenSource cts);
    void RunEntityResolutionAsync();
    DataTable GetErDataTable(MatchResultViewType viewType, string whereCondition = "");
    DataTable GetErTableSchema();
    int GetErMasterRecordsCount();
    void DefineErMasterRecord(MasterRecordSettings settings);
    void UpdateErMatchResult(List<UpdateMatchResultSetting> matchResultUpdateSettings);
    void RemoveErNotDuplicateRecords();
    void DeleteErMergeMatchResult(DeleteFromMatchResultSetting deleteSettings);
    void MergeErMatchResult(List<MergeMatchResultSetting> mergeSettings);
    List<long> GetErAcrossTableGroups();
    List<long> GetErUniqueTableGroups();
    DataTable GetPossibilities(PossibilityType possibilityType);
    void MergePossibilitiesToBiggestGroup(PossibilityType possibilityType);
    void MergePossibilitiesToNewGroup(PossibilityType possibilityType);
    void MovePossibilitiesToSeparateGroups(PossibilityType possibilityType);
    void SaveErConfiguration(string fileName, ImportedDataInfo dataInfo = null);
    void LoadErConfiguration(string fileName, ImportedDataInfo dataInfo = null);

    void MatchData(int searchDeep, MatchAlgorithm matchAlgorithm, CancellationToken cancellationToken);
    void MatchDataAsync(int searchDeep, MatchAlgorithm matchAlgorithm = MatchAlgorithm.JaroWinkler);
    DataTable GetMatchResult(MatchResultViewType viewType);
    ReportData GetMatchReport();
    void UpdateMatchReport();
    void DefineMasterRecord(MasterRecordSettings settings);
        
    long SetMasterRecord(long wpKeyId, bool val);
    void SetMatchResultCellValue(long wpKeyId, string columnName, object value);
    void SetMatchResultIsSelected(bool value, List<long> selectedIdList);
    int GetMasterRecordsCount();
    void ReindexGroups(int removedGroupId);
    void UpdateMatchResult(List<UpdateMatchResultSetting> matchResultUpdateSettings);
    void MergeMatchResult(List<MergeMatchResultSetting> mergeSettings);
    void RemoveNotDuplicateRecords();
    void CreateNewDuplicateGroup();
    void DeleteMergeMatchResult(DeleteFromMatchResultSetting deleteSettings);
}