
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WinPure.AddressVerification.Models;
using WinPure.AddressVerification.Services;
using WinPure.Cleansing.Helpers;
using WinPure.Cleansing.Models;
using WinPure.Cleansing.Services;
using WinPure.Common.Enums;
using WinPure.Configuration.DependencyInjection;
using WinPure.Configuration.Helper;
using WinPure.Configuration.Models.Configuration;
using WinPure.DataService.AuditLogs;
using WinPure.DataService.Enums;
using WinPure.DataService.Properties;
using WinPure.DataService.Senzing.Models;
using WinPure.Integration.Abstractions;
using WinPure.Integration.Export;
using WinPure.Integration.Import;
using WinPure.Integration.Models.ImportExportOptions;
using WinPure.Integration.Services;
using WinPure.Matching.Enums;
using WinPure.Matching.Models;
using WinPure.Matching.Models.Reports;
using WinPure.Matching.Models.Support;
using WinPure.Matching.Services;
using FileHelper = WinPure.Common.Helpers.FileHelper;
using NameHelper = WinPure.Common.Helpers.NameHelper;

namespace WinPure.DataService.DependencyInjection;

internal static partial class WinPureDataServiceDependencyExtension
{
    private class DataManagerService : IDataManagerService
    {
        private readonly IWpLogger _logger;
        private readonly IConnectionManager _connectionManager;
        private readonly IAuditLogService _auditLogService;
        private readonly ILicenseService _licenseService;
        private readonly ProjectSettings _settings;
        private int AllowedTableCount => GlobalConstants.AllowedTableCount(_licenseService.ProgramType, _licenseService.IsDemo);

        public event Action<string, Task, bool, CancellationTokenSource> OnProgressShow;
        public event Action<string, int> OnProgressUpdate;
        public event Action<string, string, MessagesType, Exception> OnException;
        public event Action<string> OnAddNewData;
        public event Action<string> OnTableDataUpdateBegin;
        public event Action<string> OnTableDataUpdateComplete;
        public event Action<string> OnCurrentTableChanged;
        public event Action<ImportedDataInfo> OnTableDelete;
        public event Action<bool, bool, MatchResultOperation> OnMatchResultReady;

        public event Action<object, int, bool> OnStatisticUpdated;
        public event Action<string, FiltrateField> OnFiltrateData;
        public event Action<string, string> OnChangeTableDisplayName;
        public event Action<ImportedDataInfo> OnRefreshData;
        public event Action<string, bool> OnAddressVerificationReady;
        public event Action OnEntityResolutionStart;
        public event Action OnEntityResolutionReady;

        public MatchSettingsViewModel MatchSettings { get; set; }
        public MatchParameter LastMatchingParameters { get; set; }
        public MasterRecordSettings LastMasterRecordSettings { get; set; }
        public MasterRecordSettings LastErMasterRecordSettings { get; set; }

        public bool IsImportAllowed => AllowedTableCount == 0 || _settings.TableList.Count < AllowedTableCount;
        public bool IsAnyTable => _settings.TableList.Any();
        public DataTable MatchResult { get; set; }
        public DataTable SelectedColumns { get; set; }
        public string CurrentTable
        {
            get => _settings.CurrentTable;
            set => _settings.CurrentTable = value;
        }

        public ImportedDataInfo GetCurrentTableInfo => GetTableInfo(CurrentTable);

        public string ProjectName
        {
            get => _settings.ProjectName;
            set => _settings.ProjectName = value;
        }

        public ReportData ReportData { get; set; }
        public EntityResolutionReport EntityResolutionReportData { get; set; }
        public Dictionary<string, AddressVerificationReport> VerificationResults { get; set; }

        public DataManagerService(IWpLogger logger, IConnectionManager connectionManager, IAuditLogService auditLogService, ILicenseService licenseService, ProjectSettings settings)
        {
            _logger = logger;
            _connectionManager = connectionManager;
            _auditLogService = auditLogService;
            _licenseService = licenseService;
            _settings = settings;
            VerificationResults = new Dictionary<string, AddressVerificationReport>();
            SelectedColumns = new DataTable();
            MatchSettings = new MatchSettingsViewModel();
        }

        public List<ImportedDataInfo> TableList => _settings.TableList;

        public ImportedDataInfo GetTableInfo(string tableName)
        {
            return _settings.TableList.FirstOrDefault(x => x.TableName == tableName);
        }

        public ImportedDataInfo CurrentTableInfo
        {
            get { return _settings.TableList.Any() ? _settings.TableList.First(x => x.TableName == CurrentTable) : null; }
        }

        public ImportedDataInfo GetTableInfoByDisplayName(string displayName)
        {
            return _settings.TableList.FirstOrDefault(x => x.DisplayName.NormalizeColumnName() == displayName.NormalizeColumnName());
        }

        public void SetSelectedTable(string tableName)
        {
            var tbpProp = GetTableInfo(tableName);
            if (tbpProp == null)
            {
                return;
            }
            CurrentTable = tableName;
            OnCurrentTableChanged?.Invoke(tableName);
        }

        public void DeleteTable(string tableName)
        {
            var tableInfo = _settings.TableList.FirstOrDefault(x => x.TableName == tableName);
            if (tableInfo == null)
            {
                OnException?.Invoke(Resources.EXCEPTION_NO_TABLE, "", MessagesType.Error, null);
                return;
            }
            _connectionManager.CheckConnectionState();

            if (SqLiteHelper.CheckTableExists(tableName, _connectionManager.Connection))
            {
                SqLiteHelper.ExecuteNonQuery(SqLiteHelper.GetDropTableQuery(tableName), _connectionManager.Connection);

                SqLiteHelper.ExecuteNonQuery(SqLiteHelper.GetDropTableQuery(NameHelper.GetCleanSettingsTable(tableName)), _connectionManager.Connection);

                SqLiteHelper.ExecuteNonQuery(SqLiteHelper.GetDropTableQuery(NameHelper.GetStatisticTable(tableName)), _connectionManager.Connection);

                SqLiteHelper.ExecuteNonQuery(SqLiteHelper.GetDropTableQuery(NameHelper.GetWordManagerTable(tableName)), _connectionManager.Connection);

                SqLiteHelper.ExecuteNonQuery(SqLiteHelper.GetDropTableQuery(NameHelper.GetColumnValuesTable(tableName)), _connectionManager.Connection);
            }

            if (tableInfo.IsSelected)
            {
                SelectedColumns.Columns.Remove(_settings.TableList.First(x => x.TableName == tableName).DisplayName);
                if (SelectedColumns.Columns.Count == 1)
                {
                    SelectedColumns.Clear();
                    SelectedColumns.Columns.Clear();
                }
            }
            _settings.TableList.Remove(tableInfo);
            if (!_settings.TableList.Any())
            {
                _settings.CurrentTable = "";
            }

            _auditLogService.AddSingleAuditLogIfEnabled(tableInfo.DisplayName, "Delete");
            OnTableDelete?.Invoke(tableInfo);
        }

        #region Import / export

        public void SaveResultToData(string resultName, DataTable data)
        {
            if (data == null || data.Rows.Count == 0)
            {
                return;
            }
            var tsk = new Task(() =>
            {
                try
                {
                    ColumnHelper.RemoveWinPurePrimaryKeyFieldFromTable(data);
                    SaveToData(resultName, data);
                }
                catch (Exception e)
                {
                    _logger.Error("Can not save to data", e);
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DATAIMPORTING, tsk, false, null);
        }

        public void SaveResultToData(bool removeSystemFields, MatchResultViewType viewType, string resultName, DataTable data)
        {
            if (data == null || data.Rows.Count == 0)
            {
                return;
            }

            var tsk = new Task(() =>
            {
                try
                {
                    if (removeSystemFields)
                    {
                        ColumnHelper.RemoveSystemFieldsFromTable(data);
                    }
                    else
                    {
                        ColumnHelper.RemoveWinPurePrimaryKeyFieldFromTable(data);
                        ColumnHelper.RenameSystemFieldsFromTable(data);
                    }

                    var baseNameForImportedMatchResult = $"{resultName}_{viewType}";
                    SaveToData(baseNameForImportedMatchResult, data);
                }
                catch (Exception e)
                {
                    _logger.Error("Can not save to data", e);
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DATAIMPORTING, tsk, false, null);
        }

        private void SaveToData(string dataTableName, DataTable data)
        {
            var allowedExportLimit = GlobalConstants.MatchResultExportLimitForProgram(_licenseService.ProgramType, _licenseService.IsDemo);
            if (allowedExportLimit > 0)
            {
                data = data.Rows.Cast<DataRow>().Take(allowedExportLimit).CopyToDataTable();
            }

            var tableName = GetNextTableName();
            var importedMatchResultName = dataTableName;
            int i = 1;
            while (_settings.TableList.Any(x => x.DisplayName == importedMatchResultName))
            {
                importedMatchResultName = dataTableName + $"_{i++}";
            }

            var importProvider = new ImportFromDataTableProvider();
            importProvider.Initialize(new ImportFromDataTableOptions { DisplayName = importedMatchResultName, TableName = tableName, Data = data });

            var integrationService = WinPureConfigurationDependency.Resolve<IIntegrationService>();
            var importedInfo = integrationService.ImportData(importProvider, tableName);
            _settings.TableList.Add(importedInfo);
            _settings.CurrentTable = tableName;
            OnAddNewData?.Invoke(tableName);
        }

        public void ImportData(IImportProvider importProvider)
        {
            var currentCulture = CultureInfo.CurrentCulture;
            var tsk = new Task(() =>
            {
                var integrationService = WinPureConfigurationDependency.Resolve<IIntegrationService>();

                integrationService.OnException += NotifyException;
                integrationService.OnProgressUpdate += NotifyProgress;
                Thread.CurrentThread.CurrentCulture = currentCulture;
                var tblName = GetNextTableName();
                try
                {
                    var importedInfo = integrationService.ImportData(importProvider, tblName);
                    if (importedInfo.RowCount > 0)
                    {
                        importedInfo.DisplayName = GetTableDisplayName(importedInfo.DisplayName);
                        _settings.TableList.Add(importedInfo);
                        _settings.CurrentTable = tblName;

                        OnProgressUpdate?.Invoke(Resources.CAPTION_PREPARING_FOR_DISPLAY, 90);

                        if (importedInfo.SourceType == ExternalSourceTypes.Senzing || importedInfo.SourceType == ExternalSourceTypes.JSONL)
                        {
                            SetDefaultEntityResolutionMapping(importedInfo.TableName);
                            importedInfo.IsResolutionSelected = true;
                        }

                        _auditLogService.AddSingleAuditLogIfEnabled(importedInfo.DisplayName, "Import");

                        OnAddNewData?.Invoke(tblName);

                        if (importedInfo.IsStatisticCalculated)
                        {
                            CheckCleansingAi(importedInfo.TableName);
                            var statisticTable = GetTableStatistic(importedInfo.TableName);
                            OnStatisticUpdated?.Invoke(statisticTable, importedInfo.RowCount, importedInfo.IsStatisticCalculated);
                        }

                        if (importProvider.LimitedRecords)
                        {
                            if (_licenseService.ProgramType == ProgramType.CamFree)
                            {
                                OnException?.Invoke(Resources.MESSAGE_FREEEDITION_LIMITED, "", MessagesType.Information, null);
                            }
                            else if (_licenseService.IsDemo)
                            {
                                if (_licenseService.ProgramType == ProgramType.CamBiz || _licenseService.ProgramType == ProgramType.CamLte)
                                {
                                    OnException?.Invoke(Resources.MESSAGE_DEMO_50_LIMITED, "", MessagesType.Information, null);
                                }
                                else
                                {
                                    OnException?.Invoke(string.Format(Resources.MESSAGE_DEMO_CAM_LIMITED, ProgramTypeHelper.GetProgramName(_licenseService.ProgramType), importedInfo.RowCount), "", MessagesType.Information, null);
                                }
                            }
                        }

                    }

                    OnProgressUpdate?.Invoke(Resources.CAPTION_COMPLETE, 100);
                }
                catch (WinPureImportException ex)
                {
                    if (_settings.TableList.Any(x => x.TableName == tblName))
                    {
                        _settings.TableList.Remove(_settings.TableList.First(x => x.TableName == tblName));
                    }
                    OnException?.Invoke(ex.Message, "", MessagesType.Error, null);
                }
                catch (Exception ex)
                {
                    _logger.Debug("IMPORT DATA", ex);
                    if (_settings.TableList.Any(x => x.TableName == tblName))
                    {
                        _settings.TableList.Remove(_settings.TableList.First(x => x.TableName == tblName));
                    }
                    OnException?.Invoke(Resources.EXCEPTION_IMPORTDATA_FAIL, "", MessagesType.Error, ex);
                }
                finally
                {
                    integrationService.OnException -= NotifyException;
                    integrationService.OnProgressUpdate -= NotifyProgress;
                    GC.Collect(0, GCCollectionMode.Forced);
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DATAIMPORTING, tsk, true, null);
        }

        public void ExportData(IExportProvider exportProvider, DataTable exportData = null, string filter = "", bool removeSystemFields = false)
        {
            if (!_settings.TableList.Any())
            {
                OnException?.Invoke(Resources.EXCEPTION_NO_DATA_FOR_EXPORT, "", MessagesType.Error, null);
                return;
            }

            var tsk = new Task(() =>
            {
                var integrationService = WinPureConfigurationDependency.Resolve<IIntegrationService>();
                integrationService.OnException += NotifyException;
                integrationService.OnProgressUpdate += NotifyProgress;

                try
                {
                    OnProgressUpdate(Resources.CAPTION_PREPARE_DATA_FOR_EXPORT, 0);

                    DataTable exportTable;
                    if (!string.IsNullOrEmpty(filter) && filter.Contains("\""))
                    {
                        exportTable = SqLiteHelper.ExecuteQuery(SqLiteHelper.GetSelectQuery(_settings.CurrentTable), _connectionManager.Connection, CommandBehavior.SchemaOnly);
                        filter = exportTable.Columns.Cast<DataColumn>().Aggregate(filter, (current, col) => current.Replace($"\"{col.ColumnName}\"", $"[{col.ColumnName}]"));
                    }

                    var exportLimit = GlobalConstants.ExportRowLimitForProgram(_licenseService.ProgramType, _licenseService.IsDemo);

                    var qry = SqLiteHelper.GetSelectQuery(_settings.CurrentTable, "", filter?.Replace("\"", "").Replace("N'", "'"), false, exportLimit);

                    exportTable = exportData == null
                        ? SqLiteHelper.ExecuteQuery(qry, _connectionManager.Connection, CommandBehavior.KeyInfo, _settings.CurrentTable)
                        : (_licenseService.IsDemo
                            ? exportData.Rows.Cast<DataRow>().Take(exportLimit).CopyToDataTable()
                            : exportData);


                    integrationService.ExportData(exportProvider, exportTable, removeSystemFields);

                    OnProgressUpdate(Resources.CAPTION_EXPORT_COMPLETE, 100);
                }
                catch (WinPureBaseException ex)
                {
                    _logger.Debug("WP EXPORT DATA", ex);
                    OnException?.Invoke(ex.Message, "", MessagesType.Error, null);
                }
                catch (Exception ex)
                {
                    _logger.Debug("EXPORT DATA", ex);
                    OnException?.Invoke(Resources.EXCEPTION_EXPORT_FAIL, Resources.CAPTION_EXPORT_ERROR, MessagesType.Error, ex);
                }
                finally
                {
                    integrationService.OnException -= NotifyException;
                    integrationService.OnProgressUpdate -= NotifyProgress;
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DATA_EXPORTING, tsk, true, null);
        }

        public void ReimportData(string tableName)
        {
            var importedDataInfo = GetTableInfo(tableName);
            if (importedDataInfo == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(importedDataInfo.ImportParameters) || importedDataInfo.SourceType == ExternalSourceTypes.DataTable || importedDataInfo.SourceType == ExternalSourceTypes.NotDefined)
            {
                OnException?.Invoke("This data source could not be updated.", "", MessagesType.Warning, null);
                return;
            }

            var importProvider = ImportExportDataFactory.GetImportDataInstance(importedDataInfo.SourceType);
            if (importProvider == null)
            {
                return;
            }
            var integrationService = WinPureConfigurationDependency.Resolve<IIntegrationService>();
            integrationService.OnException += NotifyException;
            integrationService.OnProgressUpdate += NotifyProgress;
            try
            {
                var tmpFileNameErConfiguration = Path.GetTempFileName();
                var tmpFileNameCleanConfiguration = Path.GetTempFileName();

                SaveCleanMatrix(tmpFileNameCleanConfiguration);

                if (importedDataInfo.IsResolutionSelected)
                {
                    SaveErConfiguration(tmpFileNameErConfiguration, importedDataInfo);
                }

                integrationService.ReimportData(importProvider, importedDataInfo);

                if (importProvider.LimitedRecords)
                {
                    if (_licenseService.ProgramType == ProgramType.CamFree)
                    {
                        OnException?.Invoke(Resources.MESSAGE_FREEEDITION_LIMITED, "", MessagesType.Warning, null);
                    }
                    else if (_licenseService.IsDemo)
                    {
                        if (_licenseService.ProgramType == ProgramType.CamBiz || _licenseService.ProgramType == ProgramType.CamLte)
                        {
                            OnException?.Invoke(Resources.MESSAGE_DEMO_50_LIMITED, "", MessagesType.Information, null);
                        }
                        else
                        {
                            OnException?.Invoke(string.Format(Resources.MESSAGE_DEMO_CAM_LIMITED, ProgramTypeHelper.GetProgramName(_licenseService.ProgramType), importedDataInfo.RowCount), "", MessagesType.Information, null);
                        }
                    }
                }

                LoadCleanMatrix(tmpFileNameCleanConfiguration);
                FileHelper.SafeDeleteFileWithLogging(_logger, tmpFileNameCleanConfiguration, $"Can not delete temp {tmpFileNameErConfiguration} file after Clean configuration");

                if (importedDataInfo.IsResolutionSelected)
                {
                    LoadErConfiguration(tmpFileNameErConfiguration, importedDataInfo);
                    FileHelper.SafeDeleteFileWithLogging(_logger, tmpFileNameErConfiguration, $"Can not delete temp {tmpFileNameErConfiguration} file after ER configuration");
                }

                if (importedDataInfo.IsStatisticCalculated)
                {
                    CheckCleansingAi(importedDataInfo.TableName);
                    var statisticTable = GetTableStatistic(importedDataInfo.TableName);
                    OnStatisticUpdated?.Invoke(statisticTable, importedDataInfo.RowCount, importedDataInfo.IsStatisticCalculated);
                }

                _auditLogService.AddSingleAuditLogIfEnabled(importedDataInfo.DisplayName, "Re-Import");

                OnRefreshData?.Invoke(importedDataInfo);
                OnProgressUpdate?.Invoke(Resources.CAPTION_COMPLETE, 100);
            }
            catch (WinPureImportException ex)
            {
                OnException?.Invoke(ex.Message, "", MessagesType.Error, null);
            }
            catch (Exception ex)
            {
                _logger.Debug("REIMPORT DATA", ex);
                OnException?.Invoke(Resources.EXCEPTION_IMPORTDATA_FAIL, "", MessagesType.Error, ex);
            }
            finally
            {
                integrationService.OnException -= NotifyException;
                integrationService.OnProgressUpdate -= NotifyProgress;
                GC.Collect(0, GCCollectionMode.Forced);
            }
        }

        public void ReimportDataAsync(string tableName)
        {
            var tsk = new Task(() =>
            {
                ReimportData(tableName);
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DATAIMPORTING, tsk, true, null);
        }

        #endregion

        public void RaiseAddNewData(string tableName)
        {
            OnAddNewData?.Invoke(tableName);
        }

        public void UpdateSelectedForMatchingTables(string tableName)
        {
            SelectedColumns.Columns.Clear();
            SelectedColumns.Rows.Clear();
            if (_settings.TableList.Any(x => x.IsSelected))
            {
                foreach (var tbl in _settings.TableList.Where(x => x.IsSelected))
                {
                    SelectedColumns.Columns.Add(new DataColumn(tbl.DisplayName, typeof(string)));

                    if (SelectedColumns.Columns.Count == 1)
                    {
                        var colNames = GetTableColumns(tbl.TableName);
                        foreach (var nm in colNames)
                        {
                            var rw = SelectedColumns.NewRow();
                            rw[tbl.DisplayName] = nm;
                            SelectedColumns.Rows.Add(rw);
                        }
                    }
                }
                var nCol = new DataColumn(WinPureColumnNamesHelper.WPCOLUMN_INCLUDE_IN_RESULT, typeof(bool)) { DefaultValue = true };
                SelectedColumns.Columns.Add(nCol);
            }
        }

        #region Modify table

        public void AddTableColumn(string tableName, string columnName, string columnType)
        {
            var importedDataInfo = GetTableInfo(tableName);
            if (importedDataInfo == null)
            {
                return;
            }

            var dataManagementService = WinPureConfigurationDependency.Resolve<IImportedDataManagementService>();

            try
            {
                dataManagementService.AddTableColumn(importedDataInfo, columnName, columnType);
                OnRefreshData?.Invoke(importedDataInfo);
            }
            catch (Exception ex)
            {
                _logger.Error("ADD COLUMN TO TABLE FAIL", ex);
                NotifyException(Resources.EXCEPTION_COLUMN_ADD_ERROR, ex);
            }
        }

        public void RenameColumn(string tableName, List<DataField> databaseColumns)
        {
            var importedDataInfo = GetTableInfo(tableName);
            if (importedDataInfo == null)
            {
                return;
            }
            var tsk = new Task(() =>
            {
                var dataManagementService = WinPureConfigurationDependency.Resolve<IImportedDataManagementService>();

                try
                {
                    dataManagementService.RenameColumn(importedDataInfo, databaseColumns);
                    OnRefreshData?.Invoke(importedDataInfo);
                }
                catch (Exception ex)
                {
                    _logger.Error("RENAME COLUMN IN TABLE", ex);
                    NotifyException(Resources.EXCEPTION_COLUMN_RENAME_ERROR, ex);
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_COLUMNS_RENAMING, tsk, false, null);
        }

        public void CopyColumn(string tableName, List<DataField> databaseColumns)
        {
            var importedDataInfo = GetTableInfo(tableName);
            if (importedDataInfo == null)
            {
                return;
            }
            var tsk = new Task(() =>
            {
                var dataManagementService = WinPureConfigurationDependency.Resolve<IImportedDataManagementService>();

                try
                {
                    dataManagementService.CopyColumn(importedDataInfo, databaseColumns);
                    OnRefreshData?.Invoke(importedDataInfo);
                }
                catch (Exception ex)
                {
                    _logger.Error("COPY COLUMN IN TABLE", ex);
                    NotifyException(Resources.EXCEPTION_COLUMN_COPY_ERROR, ex);
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_COLUMNS_COPYING, tsk, false, null);
        }

        public void DeleteRecords(string tableName)
        {
            var importedDataInfo = GetTableInfo(tableName);
            if (importedDataInfo != null)
            {
                if (importedDataInfo.RowCount > 50000)
                {
                    importedDataInfo.IsStatisticCalculated = false;
                }
                else
                {
                    CalculateStatistic(importedDataInfo);
                }
            }
        }

        public void RemoveColumn(string tableName, List<DataField> databaseColumns)
        {
            var importedDataInfo = GetTableInfo(tableName);
            if (importedDataInfo == null)
            {
                return;
            }

            var dataManagementService = WinPureConfigurationDependency.Resolve<IImportedDataManagementService>();

            try
            {
                dataManagementService.RemoveColumn(importedDataInfo, databaseColumns);
                OnRefreshData?.Invoke(importedDataInfo);
            }
            catch (Exception ex)
            {
                _logger.Error("REMOVE COLUMN FROM TABLE", ex);
                NotifyException(Resources.EXCEPTION_COLUMN_REMOVE_ERROR, ex);
            }
        }

        public void FiltrateMainData(string columnName, FiltrateField filter)
        {
            OnFiltrateData?.Invoke(columnName, filter);
        }

        public void ChangeTableDisplayName(string tableName, string newDisplayName)
        {
            var nName = GetTableDisplayName(newDisplayName);
            var tbl = GetTableInfo(tableName);
            if (tbl != null)
            {
                if (tbl.IsSelected)
                {
                    var col = SelectedColumns.Columns[tbl.DisplayName];

                    if (col == null)
                    {
                        col = SelectedColumns.Columns[ColumnHelper.NormalizeColumnName(tbl.DisplayName)];
                    }

                    if (col != null)
                    {
                        col.ColumnName = nName;
                        col.Caption = nName;
                    }
                }

                MatchSettings?.MatchParameters.ForEach(x =>
                {
                    foreach (var matchFiled in x.FieldMap.FieldMap)
                    {
                        if (matchFiled.TableName == tbl.DisplayName || matchFiled.TableName == ColumnHelper.NormalizeColumnName(tbl.DisplayName))
                        {
                            matchFiled.TableName = newDisplayName;
                        }
                    }
                });

                tbl.DisplayName = nName;
                OnChangeTableDisplayName?.Invoke(tableName, nName);
            }
        }

        public List<string> GetTableColumnsByDisplayName(string tableDisplayName)
        {
            var tbl = _settings.TableList.FirstOrDefault(x => x.DisplayName == tableDisplayName || ColumnHelper.NormalizeColumnName(x.DisplayName) == tableDisplayName);
            if (tbl == null) return new List<string>();
            return GetTableColumns(tbl.TableName);
        }

        #endregion

        #region Clean and Statistic
        public void UpdateTableStatistic(string tableName)
        {
            var importedInfo = _settings.TableList.FirstOrDefault(x => x.TableName == tableName);
            if (importedInfo != null)
            {
                var tsk = new Task(() =>
                {
                    try
                    {
                        CalculateStatistic(importedInfo);
                    }
                    catch (Exception ex)
                    {
                        _logger.Debug("UPDATE STATISTIC", ex);
                        OnException?.Invoke(Resources.EXCEPTION_STATISTIC_CANNOT_BE_CALCULATED, "", MessagesType.Error, ex);
                    }
                });
                OnProgressShow?.Invoke(Resources.CAPTION_STATISTIC_PROCESSING, tsk, false, null);
            }
        }

        public DataTable GetTableStatistic(string tableName)
        {
            if (_settings.TableList.All(x => x.TableName != tableName))
            {
                OnException?.Invoke(Resources.EXCEPTION_NO_TABLE, "", MessagesType.Error, null);
                return null;
            }

            try
            {
                return SqLiteHelper.ExecuteQuery(SqLiteHelper.GetSelectQuery(NameHelper.GetStatisticTable(tableName)), _connectionManager.Connection, CommandBehavior.Default, tableName + "Statistic");
            }
            catch (Exception ex)
            {
                _logger.Error($"Cannot load statistic for {tableName}", ex);
                OnException?.Invoke(Resources.EXCEPTION_CANNOT_GET_DATA_FROM_TABLE, "", MessagesType.Error, ex);
            }
            return null;
        }

        public void SaveCleanMatrix(string fileName)
        {
            try
            {
                var cleansingConfigurationService = WinPureConfigurationDependency.Resolve<ICleansingConfigurationService>();

                var winPureCleanSettings = cleansingConfigurationService.GetWinPureCleanSettings(CurrentTableInfo.TableName);
                cleansingConfigurationService.ExportCleanSettings(winPureCleanSettings, fileName);
            }
            catch (Exception ex)
            {
                OnException?.Invoke(Resources.EXCEPTION_CLEAN_MATRIX_NOT_SAVED, "", MessagesType.Error, ex);
            }
        }

        public void LoadCleanMatrix(string fileName)
        {
            try
            {
                var cleansingConfigurationService = WinPureConfigurationDependency.Resolve<ICleansingConfigurationService>();
                var settings = cleansingConfigurationService.ImportCleanSettings(fileName);
                var cleanSettingsDataTable = cleansingConfigurationService.ConvertCleanSettingsToDataTable(CurrentTableInfo.TableName, settings);
                cleansingConfigurationService.ClearCleanSettings(CurrentTableInfo.TableName);
                foreach (DataRow row in cleanSettingsDataTable.Rows)
                {
                    var columnName = row["COLUMN_NAME"].ToString();
                    for (int i = 1; i < cleanSettingsDataTable.Columns.Count; i++)
                    {
                        if (row[i].ToString() == "WordManager")
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(row[i].ToString()) && row[i].ToString() != "0")
                        {
                            cleansingConfigurationService.SaveCleanSettings(CurrentTableInfo.TableName, columnName, cleanSettingsDataTable.Columns[i].ColumnName, row[i]);
                        }
                    }
                }

                var wmColumns = settings.WordManagerSettings.Select(x => x.ColumnName).Distinct().ToList();

                foreach (var wmColumn in wmColumns)
                {
                    var wmSettings = settings.WordManagerSettings.Where(x => x.ColumnName == wmColumn).ToList();
                    if (wmSettings.Any())
                    {
                        SaveWordManagerSettings(wmSettings, wmColumn);
                    }
                }
            }
            catch (Exception ex)
            {
                OnException?.Invoke(Resources.EXCEPTION_MATRIX_FILE_WRONG_FORMAT, "", MessagesType.Error, ex);
            }
        }

        public void SaveProperCaseConfiguration(string fileName, ProperCaseConfiguration configuration)
        {
            try
            {
                var cleansingConfigurationService = WinPureConfigurationDependency.Resolve<ICleansingConfigurationService>();

                cleansingConfigurationService.ExportProperCaseConfiguration(configuration, fileName);
            }
            catch (Exception ex)
            {
                OnException?.Invoke(Resources.EXCEPTION_CLEAN_MATRIX_NOT_SAVED, "", MessagesType.Error, ex);
            }
        }

        public ProperCaseConfiguration LoadProperCaseConfiguration(string fileName)
        {
            try
            {
                var cleansingConfigurationService = WinPureConfigurationDependency.Resolve<ICleansingConfigurationService>();
                var settings = cleansingConfigurationService.ImportProperCaseConfiguration(fileName);
                return settings;
            }
            catch (Exception ex)
            {
                OnException?.Invoke(Resources.EXCEPTION_MATRIX_FILE_WRONG_FORMAT, "", MessagesType.Error, ex);
            }

            return new ProperCaseConfiguration();
        }

        public DataTable GetDataTableCleanSettings(string tableName)
        {
            if (_settings.TableList.All(x => x.TableName != tableName))
            {
                OnException?.Invoke(Resources.EXCEPTION_NO_TABLE, "", MessagesType.Error, null);
                return null;
            }
            try
            {
                var importedDataInfo = GetTableInfo(tableName);
                var cleansingConfigurationService = WinPureConfigurationDependency.Resolve<ICleansingConfigurationService>();
                var settingTable = cleansingConfigurationService.GetDataTableCleanSettings(importedDataInfo.TableName);
                settingTable.Columns.Add("PATTERN", typeof(string), string.Empty);
                settingTable.Columns.Add("AITYPE", typeof(string), string.Empty);

                foreach (DataRow row in settingTable.Rows)
                {
                    var field = importedDataInfo.Fields.FirstOrDefault(x => x.DisplayName == row["COLUMN_NAME"].ToString() || x.DatabaseName == row["COLUMN_NAME"].ToString());
                    row["PATTERN"] = field?.Pattern;
                    row["AITYPE"] = field?.AiType;
                }

                return settingTable;
            }
            catch (Exception ex)
            {
                OnException?.Invoke(Resources.EXCEPTION_CANNOT_GET_DATA_FROM_TABLE, "", MessagesType.Error, ex);
            }
            return null;
        }

        public void ClearCleanSettings(string tableName)
        {
            try
            {
                var cleansingConfigurationService = WinPureConfigurationDependency.Resolve<ICleansingConfigurationService>();
                cleansingConfigurationService.ClearCleanSettings(CurrentTableInfo.TableName);
            }
            catch (Exception ex)
            {
                _logger.Error("Clear clean settings error", ex);
                OnException?.Invoke(Resources.EXCEPTION_MATRIX_FILE_WRONG_FORMAT, "", MessagesType.Error, ex);
            }
        }

        public void SaveCleanSettings(string columnName, string fieldName, object value)
        {
            try
            {
                var cleansingConfigurationService = WinPureConfigurationDependency.Resolve<ICleansingConfigurationService>();
                cleansingConfigurationService.SaveCleanSettings(CurrentTableInfo.TableName, columnName, fieldName, value);
            }
            catch (Exception ex)
            {
                _logger.Error("Save clean settings error", ex);
                OnException?.Invoke(Resources.EXCEPTION_CANNOT_SAVE_CLEAN_SETTINGS, "", MessagesType.Error, ex);
            }
        }

        public DataTable GetColumnUniqueValues(string columnName, bool refreshData, TextCleanerSetting cleanerSett = null, CaseConverterInternalSetting caseSett = null)
        {
            try
            {
                if (CurrentTableInfo == null)
                {
                    return new DataTable();
                }
                var cleansingConfigurationService = WinPureConfigurationDependency.Resolve<ICleansingConfigurationService>();
                return cleansingConfigurationService.GetColumnUniqueValues(CurrentTableInfo.TableName, columnName, refreshData, cleanerSett, caseSett);
            }
            catch (Exception ex)
            {
                _logger.Error("Get column unique values error", ex);
                OnException?.Invoke(Resources.EXCEPTION_CANNOT_CALCULATE_UNIQUE_VALUES, "", MessagesType.Error, ex);
            }

            return null;
        }

        public void ExportDataWithUniqueValues(string fileName, string tableName, string columnName, List<string> values)
        {
            var whereCause = $"[{columnName}] in ('{string.Join("','", values)}')";
            var qry = SqLiteHelper.GetSelectQuery(tableName, whereCondition: whereCause);
            var dataForExport = SqLiteHelper.ExecuteQuery(qry, _connectionManager.Connection);
            var tableInfo = GetTableInfo(tableName);
            var exportService = new ExcelExportProvider();
            exportService.Initialize(new ExcelImportExportOptions
            {
                TableName = tableInfo.DisplayName,
                FilePath = fileName,
                ExportWithNpoi = true,
                FirstRowContainNames = true
            });
            var integrationService = WinPureConfigurationDependency.Resolve<IIntegrationService>();
            integrationService.ExportData(exportService, dataForExport);
        }

        public List<WordManagerSetting> GetWordManagerSettings(string columnName)
        {
            try
            {
                var cleansingConfigurationService = WinPureConfigurationDependency.Resolve<ICleansingConfigurationService>();
                return cleansingConfigurationService.GetWordManagerSettings(CurrentTableInfo.TableName, columnName);
            }
            catch (Exception ex)
            {
                _logger.Error("Get word manager settings error", ex);
                OnException?.Invoke(Resources.EXCEPTION_MATRIX_FILE_WRONG_FORMAT, "", MessagesType.Error, ex);
            }

            return null;
        }

        public List<ReportResultData> GetWordManagerColumnValues(string columnName)
        {
            try
            {
                var cleansingConfigurationService = WinPureConfigurationDependency.Resolve<ICleansingConfigurationService>();
                return cleansingConfigurationService.GetWordManagerColumnValues(CurrentTableInfo.TableName, columnName);
            }
            catch (Exception ex)
            {
                _logger.Error("Get word manager settings error", ex);
                OnException?.Invoke(Resources.EXCEPTION_CANNOT_GET_WORDMANAGER_PARAMETERS, "", MessagesType.Error, ex);
            }

            return null;
        }

        public void SaveWordManagerSettings(List<WordManagerSetting> wmWordManagerSettings, string columnName)
        {
            try
            {
                var cleansingConfigurationService = WinPureConfigurationDependency.Resolve<ICleansingConfigurationService>();
                cleansingConfigurationService.SaveWordManagerSettings(CurrentTableInfo.TableName, wmWordManagerSettings, columnName);
            }
            catch (Exception ex)
            {
                _logger.Error("Get word manager settings error", ex);
                OnException?.Invoke(Resources.EXCEPTION_CANNOT_SAVE_WORDMANAGER_PARAMETERS, "", MessagesType.Error, ex);
            }
        }

        public void CleanData(CancellationToken cancellationToken)
        {
            var startTime = DateTime.Now;
            try
            {
                _connectionManager.CheckConnectionState();
                var dbConnection = _connectionManager.Connection;
                var importedDataInfo = CurrentTableInfo;
                _logger.Information($"Start clean data {importedDataInfo.FileName}. Date ={DateTime.Now.ToShortDateString()} Time = {DateTime.Now.ToLongTimeString()}");
                OnTableDataUpdateBegin?.Invoke(importedDataInfo.TableName);

                var dataForClean = SqLiteHelper.ExecuteQuery($"SELECT * FROM [{importedDataInfo.TableName}]", dbConnection);

                string resultTableName = importedDataInfo.TableName + "_CleanResult";

                var cleansingConfigurationService = WinPureConfigurationDependency.Resolve<ICleansingConfigurationService>();

                var winPureCleanSettings = cleansingConfigurationService.GetWinPureCleanSettings(importedDataInfo.TableName);

                var cleansingService = WinPureConfigurationDependency.Resolve<ICleansingService>();

                cleansingService.CleanTable(dataForClean, winPureCleanSettings, cancellationToken);

                var configurationService = WinPureConfigurationDependency.Resolve<IConfigurationService>();
                if (configurationService.Configuration.EnableAuditLogs)
                {
                    OnProgressUpdate?.Invoke(Resources.CAPTION_PREPARINGAUDITLOGS, 0);
                    var originalData = SqLiteHelper.ExecuteQuery($"SELECT * FROM [{importedDataInfo.TableName}]", dbConnection);
                    var auditLogService = new AuditLogGenerator(_auditLogService);
                    var logs = auditLogService.GetAuditLogs(importedDataInfo.DisplayName, originalData, dataForClean, winPureCleanSettings);
                    _auditLogService.AddAuditLogs(logs);
                }

                OnProgressUpdate?.Invoke(Resources.CAPTION_SAVINGCHANGES, 0);
                SqLiteHelper.SaveDataToDb(dbConnection, dataForClean, resultTableName, _logger);

                _logger.Information($"Table cleaned {importedDataInfo.FileName}. Time spent ={(DateTime.Now - startTime).TotalMinutes} min");
                if (configurationService.Configuration.AllowUndo)
                {
                    var backupTableName = NameHelper.GetDataBackupTable(importedDataInfo.TableName);
                    if (SqLiteHelper.CheckTableExists(backupTableName, dbConnection))
                    {
                        SqLiteHelper.ExecuteNonQuery(SqLiteHelper.GetDropTableQuery(backupTableName), dbConnection);
                    }
                    SqLiteHelper.ChangeTableName(dbConnection, importedDataInfo.TableName, backupTableName);
                    importedDataInfo.IsUndoAvailable = true;
                }
                else
                {
                    SqLiteHelper.ExecuteNonQuery(SqLiteHelper.GetDropTableQuery(importedDataInfo.TableName), dbConnection);
                }

                SqLiteHelper.ChangeTableName(dbConnection, resultTableName, importedDataInfo.TableName);
                importedDataInfo.IsStatisticCalculated = false;
                SqLiteHelper.ExecuteNonQuery(SqLiteHelper.GetDeleteQuery(NameHelper.GetStatisticTable(importedDataInfo.TableName)), dbConnection);
                SqLiteHelper.ExecuteNonQuery(SqLiteHelper.GetDeleteQuery(NameHelper.GetColumnValuesTable(importedDataInfo.TableName)), dbConnection);

                var dt = SqLiteHelper.ExecuteQuery(SqLiteHelper.GetSelectQuery(NameHelper.GetCleanSettingsTable(importedDataInfo.TableName)), dbConnection);
                var table = SqLiteHelper.ExecuteQuery(SqLiteHelper.GetSelectQuery(importedDataInfo.TableName), dbConnection, CommandBehavior.SchemaOnly);

                foreach (DataColumn col in table.Columns)
                {
                    if (col.ColumnName == WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY) continue;

                    if (dt.AsEnumerable().All(row => col.ColumnName != row.Field<string>("COLUMN_NAME")))
                    {
                        SqLiteHelper.ExecuteNonQuery($"INSERT INTO [{NameHelper.GetCleanSettingsTable(table.TableName)}] (COLUMN_NAME) VALUES ('{col.ColumnName}');", dbConnection);
                    }
                }
                SqLiteHelper.UpdateTableColumnList(dbConnection, importedDataInfo, table);
                if (importedDataInfo.RowCount <= 50000)
                {
                    CalculateStatistic(importedDataInfo);
                }
                _logger.Information($"Clean table complete for table {importedDataInfo.FileName}. Time spent ={(DateTime.Now - startTime).TotalMinutes} min");
            }
            catch (OperationCanceledException)
            {
                OnException?.Invoke(Resources.MESSAGE_OPERATION_WAS_CANCELLED, "", MessagesType.Information, null);
            }
            catch (Exception ex)
            {
                _logger.Debug("CLEAN FAIL", ex);
                OnException?.Invoke(Resources.EXCEPTION_CLEAN_FAILED, "", MessagesType.Error, ex);
            }
            finally
            {
                OnTableDataUpdateComplete?.Invoke(_settings.CurrentTable);
            }
        }

        public void CleanDataAsync()
        {
            if (IsAnyTable)
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var tsk = new Task(() =>
                {
                    CleanData(cancellationTokenSource.Token);
                });
                OnProgressShow?.Invoke(Resources.CAPTION_DATA_CLEANING, tsk, false, cancellationTokenSource);
            }
            else
            {
                OnException?.Invoke(Resources.MESSAGE_NO_DATA_FOR_CLEANING, "", MessagesType.Information, null);
            }
        }

        public void UndoClean()
        {
            try
            {
                _connectionManager.CheckConnectionState();
                var dbConnection = _connectionManager.Connection;
                var importedDataInfo = CurrentTableInfo;
                _logger.Information($"Start clean data {importedDataInfo.FileName}. Date ={DateTime.Now.ToShortDateString()} Time = {DateTime.Now.ToLongTimeString()}");
                OnTableDataUpdateBegin?.Invoke(importedDataInfo.TableName);

                var backupTableName = NameHelper.GetDataBackupTable(importedDataInfo.TableName);
                if (SqLiteHelper.CheckTableExists(backupTableName, dbConnection))
                {
                    SqLiteHelper.ExecuteNonQuery(SqLiteHelper.GetDropTableQuery(importedDataInfo.TableName), dbConnection);
                    SqLiteHelper.ChangeTableName(dbConnection, backupTableName, importedDataInfo.TableName);
                }
                importedDataInfo.IsUndoAvailable = false;
            }
            catch (Exception ex)
            {
                OnException?.Invoke(Resources.EXCEPTION_UNDOFAIL, "", MessagesType.Error, ex);
            }
            finally
            {
                OnTableDataUpdateComplete?.Invoke(_settings.CurrentTable);
            }
        }

        public DataTable GetCleanedPreviewTable(string tableName)
        {
            var importedDataInfo = GetTableInfo(tableName);
            if (importedDataInfo == null)
            {
                OnException?.Invoke(Resources.EXCEPTION_NO_TABLE, "", MessagesType.Error, null);
                return null;
            }

            var dataManagementService = WinPureConfigurationDependency.Resolve<IImportedDataManagementService>();
            var previewTable = dataManagementService.GetPreview(importedDataInfo);

            var cleansingConfigurationService = WinPureConfigurationDependency.Resolve<ICleansingConfigurationService>();

            var winPureCleanSettings = cleansingConfigurationService.GetWinPureCleanSettings(importedDataInfo.TableName);

            var cleansingService = WinPureConfigurationDependency.Resolve<ICleansingService>();
            cleansingService.CleanTable(previewTable, winPureCleanSettings, new CancellationToken());

            return previewTable;
        }

        public void CheckCleansingAi(string tableName)
        {
            var configuration = WinPureConfigurationDependency.Resolve<IConfigurationService>();
            if (configuration.Configuration.UseCleansingAi)
            {
                UpdateCleansingOptionsBasedOnAi(tableName);
            }
        }

        public void UpdateCleansingOptionsBasedOnAi(string tableName)
        {
            var tableInfo = GetTableInfo(tableName);
            if (tableInfo == null || !tableInfo.IsStatisticCalculated)
                return;

            var statistic = GetTableStatistic(tableName);
            if (statistic == null || statistic.Rows.Count == 0)
                return;

            var aiConfigurationService = WinPureConfigurationDependency.Resolve<ICleansingAiConfigurationService>();
            //var cleansingSettings = GetDataTableCleanSettings(tableName);
            var aiTypes = AsyncHelpers.RunSync(aiConfigurationService.GetAllConfigurations);


            foreach (DataRow row in statistic.Rows)
            {
                var column = row["FieldName"].ToString();
                var columnInfo = tableInfo.Fields.FirstOrDefault(x => string.Equals(x.DisplayName, column, StringComparison.InvariantCultureIgnoreCase));
                if (columnInfo == null || string.IsNullOrWhiteSpace(columnInfo.AiType))
                    continue;

                var aiType = aiTypes.FirstOrDefault(x => x.AiType == columnInfo.AiType);
                if (aiType == null)
                    continue;

                var options = aiType.Options;
                if (options.AddressParser)
                    SaveCleanSettings(column, "SP_Address", 1);
                // Text cleaner toggles against statistic counters
                if (options.Apostrophes && Convert.ToInt32(row["Apostrophes"]) > 0)
                    SaveCleanSettings(column, "RM_Apostrophes", 1);
                if (options.Comma && Convert.ToInt32(row["Commas"]) > 0)
                    SaveCleanSettings(column, "RM_Commas", 1);
                if (options.Dots && Convert.ToInt32(row["Dots"]) > 0)
                    SaveCleanSettings(column, "RM_Dots", 1);
                if (options.Hyphens && Convert.ToInt32(row["Hyphens"]) > 0)
                    SaveCleanSettings(column, "RM_Hyphens", 1);
                if (options.NonPrintable && Convert.ToInt32(row["NonPrintable"]) > 0)
                    SaveCleanSettings(column, "RM_Nonprintable", 1);
                if (options.Spaces && Convert.ToInt32(row["WithSpaces"]) > 0)
                    SaveCleanSettings(column, "RM_Spaces", 1);
                if (options.MultipleSpaces && Convert.ToInt32(row["MultipleSpaces"]) > 0)
                    SaveCleanSettings(column, "RM_MultiSpaces", 1);
                if (options.LeadingSpaces && Convert.ToInt32(row["LeadingSpaces"]) > 0)
                    SaveCleanSettings(column, "RM_LeadingSpaces", 1);
                if (options.TrailingSpaces && Convert.ToInt32(row["TrailingSpaces"]) > 0)
                    SaveCleanSettings(column, "RM_TrailingSpaces", 1);
                if (options.TabChar && Convert.ToInt32(row["TabChar"]) > 0)
                    SaveCleanSettings(column, "RM_Tab", 1);
                if (options.NewLine && Convert.ToInt32(row["NewLineChar"]) > 0)
                    SaveCleanSettings(column, "RM_NewLine", 1);
                if (options.Letters && Convert.ToInt32(row["Letters"]) > 0)
                    SaveCleanSettings(column, "RM_Letters", 1);
                if (options.Numbers && Convert.ToInt32(row["Numbers"]) > 0)
                    SaveCleanSettings(column, "RM_Numbers", 1);
            }
        }

        private void CalculateStatistic(ImportedDataInfo importedInfo)
        {
            var startTime = DateTime.Now;
            DataTable statistic = null;

            _logger.Information($"Start update statistic for table {importedInfo.FileName}. Date ={DateTime.Now.ToShortDateString()} Time = {DateTime.Now.ToLongTimeString()}");
            try
            {
                var cleansingService = WinPureConfigurationDependency.Resolve<ICleansingService>();
                importedInfo.RowCount = SqLiteHelper.ExecuteScalar<int>(SqLiteHelper.GetCountQuery(importedInfo.TableName), _connectionManager.Connection);

                var data = SqLiteHelper.ExecuteQuery(SqLiteHelper.GetSelectQuery(importedInfo.TableName), _connectionManager.Connection);

                var statisticTableName = NameHelper.GetStatisticTable(importedInfo.TableName);
                statistic = cleansingService.CalculateStatistic(data, importedInfo.Fields, new CancellationToken());
                SqLiteHelper.SaveDataToDb(_connectionManager.Connection, statistic, statisticTableName, _logger, false);
                importedInfo.IsStatisticCalculated = true;
            }
            catch (Exception e)
            {
                OnException?.Invoke(e.Message, "", MessagesType.Error, null);
                return;
            }

            _logger.Information($"End update statistic for table {importedInfo.FileName}. Spent time = {(DateTime.Now - startTime).TotalMinutes} min");

            OnStatisticUpdated?.Invoke(statistic, importedInfo.RowCount, importedInfo.IsStatisticCalculated);
        }

        #endregion

        #region Address verification

        public AddressVerificationReport GetAddressVerificationResult(string tableName)
        {
            return VerificationResults.ContainsKey(tableName) ? VerificationResults[tableName] : null;
        }

        public DataTable GetDataTableAddressVerificationSetting(string tableName)
        {
            if (_settings.TableList.All(x => x.TableName != tableName))
            {
                OnException?.Invoke(Resources.EXCEPTION_NO_TABLE, "", MessagesType.Error, null);
                return null;
            }

            var dbTableName = NameHelper.GetCleanSettingsTable(tableName);
            var selectQry = SqLiteHelper.GetSelectQuery(dbTableName, "COLUMN_NAME, AF_Address, AF_Zip, AF_City, AF_State, RG_Latitude, RG_Longitude");
            while (true)
            {
                try
                {
                    var result = SqLiteHelper.ExecuteQuery(selectQry, _connectionManager.Connection, CommandBehavior.Default, tableName);
                    return result;
                }
                catch (SQLiteException sqlEx)
                {
                    if (sqlEx.ErrorCode == 1)
                    {
                        var columnName = sqlEx.Message.Substring(sqlEx.Message.IndexOf("no such column:") + 15).Trim();
                        columnName = columnName.Substring(0, columnName.Length - 2);
                        SqLiteHelper.AddTableColumn(_connectionManager.Connection, dbTableName, columnName, typeof(int));
                    }
                    else
                    {
                        OnException?.Invoke(Resources.EXCEPTION_CANNOT_GET_DATA_FROM_TABLE, "", MessagesType.Error, sqlEx);

                        return null;
                    }
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(Resources.EXCEPTION_CANNOT_GET_DATA_FROM_TABLE, "", MessagesType.Error, ex);

                    return null;
                }
            }
        }

        public void StartAddressVerification(string tableName, AddressVerificationSettings verificationSettings)
        {
            var tblInfo = GetTableInfo(tableName);
            if (tblInfo == null)
            {
                return;
            }
            var cts = new CancellationTokenSource();

            var tsk = new Task(() =>
            {
                var addressVerificationService = WinPureConfigurationDependency.Resolve<IAddressVerificationService>();
                addressVerificationService.OnProgressUpdate += NotifyProgress;
                try
                {
                    OnTableDataUpdateBegin?.Invoke(tblInfo.TableName);
                    var tblOptions = GetDataTableAddressVerificationSetting(tableName);
                    var recordsForVerification = _licenseService.IsDemo
                        ? GlobalConstants.AddressVerificationRecordsForDemoVersion
                        : -1;

                    var res = addressVerificationService.VerifyAddresses(verificationSettings, tblInfo, tblOptions, recordsForVerification, cts.Token);

                    cts.Token.ThrowIfCancellationRequested();
                    OnProgressUpdate?.Invoke(Resources.CAPTION_RESULT_PREPARING, 90);

                    if (res != null)
                    {
                        if (VerificationResults.ContainsKey(tblInfo.TableName))
                        {
                            VerificationResults[tblInfo.TableName] = res;
                        }
                        else
                        {
                            VerificationResults.Add(tblInfo.TableName, res);
                        }

                        OnAddressVerificationReady?.Invoke(tblInfo.TableName, false);
                    }
                }
                catch (OperationCanceledException)
                {
                    OnException?.Invoke(Resources.CAPTION_OPERATION_CANCELLED, "", MessagesType.Error, null);
                }
                catch (AggregateException ex)
                {
                    if (cts.IsCancellationRequested)
                    {
                        OnException?.Invoke(Resources.CAPTION_OPERATION_CANCELLED, "", MessagesType.Error, null);
                    }
                    else
                    {
                        _logger.Debug("ADDRESS VERIFICATION ERROR", ex);
                        OnException?.Invoke("", "", MessagesType.Error, ex);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Debug("ADDRESS VERIFICATION ERROR", ex);
                    OnException?.Invoke("", "", MessagesType.Error, ex);
                }
                finally
                {
                    addressVerificationService.OnProgressUpdate -= NotifyProgress;
                    OnTableDataUpdateComplete?.Invoke(tblInfo.TableName);

                }
            }, cts.Token);
            OnProgressShow?.Invoke(Resources.CAPTION_ADDRVERIFY_PROGRESS, tsk, true, cts);
        }


        #endregion

        #region Matching
        public void MatchData(int searchDeep, MatchAlgorithm matchAlgorithm, CancellationToken cancellationToken)
        {
            var matchService = WinPureConfigurationDependency.Resolve<IMatchService>();
            var representationService = WinPureConfigurationDependency.Resolve<IRepresentationService>();

            try
            {
                var startMatching = DateTime.Now;
                _logger.Information($"START MATCHING, TIME: {startMatching}");
                NotifyProgress(Resources.CAPTION_PARAMETERS_PREPARING, 0);
                MatchResult = null;
                GC.Collect();

                var matchParameters = representationService.ConvertMatchSettingsViewToMatchParameters(MatchSettings, searchDeep, matchAlgorithm);

                var tblStruct = new Dictionary<string, DataTable>();

                foreach (var tbl in _settings.TableList)
                {
                    var table = SqLiteHelper.ExecuteQuery(SqLiteHelper.GetSelectQuery(tbl.TableName), _connectionManager.Connection, CommandBehavior.SchemaOnly, tbl.TableName);
                    tblStruct.Add(tbl.TableName, table);
                }

                var fieldMap = new List<FieldMapping>();

                foreach (DataRow row in SelectedColumns.Rows)
                {
                    if (row.RowState == DataRowState.Deleted || !Convert.ToBoolean(row[WinPureColumnNamesHelper.WPCOLUMN_INCLUDE_IN_RESULT])) continue;

                    var fieldList = new FieldMapping();
                    foreach (DataColumn column in SelectedColumns.Columns)
                    {
                        if (column.ColumnName != WinPureColumnNamesHelper.WPCOLUMN_INCLUDE_IN_RESULT && row[column.ColumnName].ToString() != "")
                        {
                            var tbl = GetTableInfoByDisplayName(column.ColumnName);
                            fieldList.FieldMap.Add(new MatchField
                            {
                                TableName = tbl.DisplayName,
                                ColumnName = row[column.ColumnName].ToString(),
                                ColumnDataType = tblStruct[tbl.TableName].Columns[row[column.ColumnName].ToString()].DataType.ToString()
                            });
                        }
                    }
                    fieldMap.Add(fieldList);
                }

                OnProgressUpdate?.Invoke(Resources.CAPTION_MATCHING_STEP1OF10, 5);
                var tables = new List<TableParameter>();

                foreach (var tbl in matchParameters.Groups.SelectMany(x => x.Conditions).SelectMany(x => x.Fields).Select(x => x.TableName).Distinct())
                {
                    var cols = fieldMap.SelectMany(x => x.FieldMap)
                        .Where(x => x.TableName == tbl)
                        .Select(x => x.ColumnName)
                        .Aggregate("", (current, colName) => current + ((current == "") ? $"[{colName}]" : $",[{colName}]"));

                    if (string.IsNullOrEmpty(cols))
                    {
                        OnException?.Invoke(Resources.EXCEPTION_NO_FIELD_WERE_SELECTED, "", MessagesType.Warning, null);
                    }

                    var tblParam = new TableParameter { TableName = tbl };
                    var settTable = _settings.TableList.First(x => x.DisplayName == tbl);
                    var dbTable = settTable.TableName;
                    _logger.Information($"Select data from DB, Name={settTable.TableName} Row count = {settTable.RowCount}");
                    var sql = SqLiteHelper.GetSelectQuery(dbTable, $"{cols}, {WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY}");
                    tblParam.TableData = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection);
                    tables.Add(tblParam);
                    cancellationToken.ThrowIfCancellationRequested();
                }
                _logger.Information("Data was loaded to memory successfully ");

                if (LastMasterRecordSettings != null && TableList.Count > 1)
                {
                    LastMasterRecordSettings = null;
                }

                MatchResult = matchService.MatchData(tables, matchParameters, fieldMap, cancellationToken, NotifyProgress);
                LastMatchingParameters = matchParameters;

                if (MatchResult != null)
                {
                    OnProgressUpdate?.Invoke(Resources.CAPTION_REPORT_PREPARING, 92);
                    ReportData = GetMatchReport();
                    ReportData.MatchingTime = DateTime.Now - startMatching;

                    var configurationService = WinPureConfigurationDependency.Resolve<IConfigurationService>();
                    if (configurationService.Configuration.AutoSetMasterRecord)
                    {
                        OnProgressUpdate?.Invoke(Resources.CAPTION_DEFINING_MASTER_RECORD_WAIT, 95);
                        var dataNormalizationService = WinPureConfigurationDependency.Resolve<IDataNormalizationService>();
                        var mrSettings = configurationService.Configuration.AutoSetMasterRecordType == 0
                            ? new MasterRecordSettings { RecordType = MasterRecordType.MostRelevant, ApplyOptionsIfRuleGiveNothing = true }
                            : new MasterRecordSettings { RecordType = MasterRecordType.MostPopulatedByTable, ApplyOptionsIfRuleGiveNothing = true };
                        
                        LastMasterRecordSettings = mrSettings;
                        dataNormalizationService.DefineMasterRecord(MatchResult, LastMatchingParameters, mrSettings);
                    }

                    OnProgressUpdate?.Invoke(Resources.CAPTION_RESULT_PREPARING, 98);
                    OnMatchResultReady?.Invoke(true, true, MatchResultOperation.Matching);
                }

                _auditLogService.AddSingleAuditLogIfEnabled("Match Result", $"Match table(s): {string.Join(",", tables.Select(x => x.TableName))}");
                _logger.Information($"END MATCHING, SPENT TIME: {(DateTime.Now - startMatching).ToString(@"dd\:hh\:mm\:ss")}");
            }
            catch (OperationCanceledException)
            {
                OnException?.Invoke(Resources.MESSAGE_OPERATION_WAS_CANCELLED, "", MessagesType.Information, null);
            }
            catch (Exception ex)
            {
                _logger.Debug("RUN MATCHING ERROR", ex);
                NotifyException(Resources.EXCEPTION_COULDNOT_PREPARE_MATCHING_PARAMETERS, ex);
            }


        }

        public void MatchDataAsync(int searchDeep, MatchAlgorithm matchAlgorithm = MatchAlgorithm.JaroWinkler)
        {
            CleansingHelper.LibpostalTeardown();
            if (TableList.All(x => !x.IsSelected) || SelectedColumns.Columns.Count == 0 || MatchSettings.MatchParameters.Count == 0)
            {
                OnException?.Invoke(Resources.EXCEPTION_SET_MATCHING_PARAMETERS.Replace(@"\r\n", Environment.NewLine), "", MessagesType.Warning, null);
                return;
            }
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var tsk = new Task(() =>
            {
                MatchData(searchDeep, matchAlgorithm, cancellationToken);

            }, cancellationTokenSource.Token);

            OnProgressShow?.Invoke(Resources.CAPTION_MATCHING_DATA, tsk, true, cancellationTokenSource);
        }

        public DataTable GetMatchResult(MatchResultViewType viewType)
        {
            var representationService = WinPureConfigurationDependency.Resolve<IRepresentationService>();
            return representationService.GetMatchResult(MatchResult, viewType);
        }

        public ReportData GetMatchReport()
        {
            if (MatchResult == null)
            {
                return null;
            }

            var configurationService = WinPureConfigurationDependency.Resolve<IConfigurationService>();
            if (configurationService.Configuration.GenerateMatchReport)
            {
                var representationService = WinPureConfigurationDependency.Resolve<IRepresentationService>();
                var repData = representationService.GetMatchReportData(MatchResult);
                repData.MatchSettings = MatchSettings;

                var matchNonMatches = representationService.GetMatchResult(MatchResult, MatchResultViewType.NonMatches);
                var reportData = representationService.GetMatchReportCommonData(matchNonMatches);
                repData.ViewData.Add(MatchResultViewType.NonMatches, reportData);
                repData.ViewData.Add(MatchResultViewType.OnlyGroup, repData.CommonData.First());

                if (repData.SourceData.Count > 1)
                {
                    var acrossTablesMatches = representationService.GetMatchResult(MatchResult, MatchResultViewType.AcrossTable);
                    repData.ViewData.Add(MatchResultViewType.AcrossTable, representationService.GetMatchReportCommonData(acrossTablesMatches));

                    var tablesUniqueMatches = representationService.GetMatchResult(MatchResult, MatchResultViewType.TableUnique);
                    repData.ViewData.Add(MatchResultViewType.TableUnique, representationService.GetMatchReportCommonData(tablesUniqueMatches));
                }
                else
                {
                    repData.ViewData.Add(MatchResultViewType.AcrossTable, reportData);
                    repData.ViewData.Add(MatchResultViewType.TableUnique, reportData);
                }


                return repData;
            }
            return new ReportData { MatchSettings = MatchSettings };
        }

        public void UpdateMatchReport()
        {
            if (MatchResult == null)
            {
                return;
            }

            var representationService = WinPureConfigurationDependency.Resolve<IRepresentationService>();

            ReportData = representationService.GetMatchReportData(MatchResult);
            ReportData.MatchSettings = MatchSettings;
        }

        public void DefineMasterRecord(MasterRecordSettings settings)
        {
            var tsk = new Task(() =>
            {
                try
                {
                    LastMasterRecordSettings = settings;

                    var dataNormalizationService = WinPureConfigurationDependency.Resolve<IDataNormalizationService>();

                    if (dataNormalizationService.DefineMasterRecord(MatchResult, LastMatchingParameters, settings))
                    {
                        OnMatchResultReady?.Invoke(true, true, MatchResultOperation.SetMasterRecord);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Debug("DEFINE MASTER RECORD", ex);
                    OnException?.Invoke(Resources.EXCEPTION_MASTER_RECORD_CANNOT_BE_DEFINED, "", MessagesType.Error, ex);
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DEFINING_MASTER_RECORD_WAIT, tsk, false, null);
        }

        public long SetMasterRecord(long wpKeyId, bool val)
        {
            var rw = (from myRow in MatchResult.AsEnumerable() where myRow.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY) == wpKeyId select myRow).FirstOrDefault();
            if (rw != null)
            {
                rw[WinPureColumnNamesHelper.WPCOLUMN_ISMASTER] = val;
                if (val)
                {
                    var oldMr = (from myRow in MatchResult.AsEnumerable() where myRow.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY) != wpKeyId && myRow.Field<bool>(WinPureColumnNamesHelper.WPCOLUMN_ISMASTER) && myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID) == Convert.ToInt32(rw[WinPureColumnNamesHelper.WPCOLUMN_GROUPID]) select myRow).FirstOrDefault();
                    if (oldMr != null)
                    {
                        oldMr[WinPureColumnNamesHelper.WPCOLUMN_ISMASTER] = false;
                        return Convert.ToInt64(oldMr[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY]);
                    }
                }
            }
            return -1;
        }

        public void SetMatchResultCellValue(long wpKeyId, string columnName, object value)
        {
            var rw = (from myRow in MatchResult.AsEnumerable()
                      where myRow.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY) == wpKeyId
                      select myRow)
                .FirstOrDefault();
            if (rw != null)
            {
                rw[columnName] = value;
            }
        }

        public void SetMatchResultIsSelected(bool value, List<long> selectedIdList)
        {
            foreach (var matchResultWithSelection in MatchResult?.AsEnumerable()
                .Join(selectedIdList, r => r[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY], i => i, (r, i) => new { res = r }))
            {
                matchResultWithSelection.res[WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED] = value;
            }
        }

        public int GetMasterRecordsCount()
        => MatchResult?.AsEnumerable().Count(x => x.Field<bool>(WinPureColumnNamesHelper.WPCOLUMN_ISMASTER)) ?? 0;

        public void ReindexGroups(int removedGroupId)
        {
            var representationService = WinPureConfigurationDependency.Resolve<IRepresentationService>();
            representationService.ReindexGroups(MatchSettings, removedGroupId);
        }

        public void UpdateMatchResult(List<UpdateMatchResultSetting> matchResultUpdateSettings)
        {
            var tsk = new Task(() =>
            {
                try
                {
                    var dataNormalizationService = WinPureConfigurationDependency.Resolve<IDataNormalizationService>();
                    var configurationService = WinPureConfigurationDependency.Resolve<IConfigurationService>();
                    if (configurationService.Configuration.EnableAuditLogs)
                    {
                        var original = MatchResult.Copy();
                        var auditLogGenerator = new AuditLogGenerator(_auditLogService);

                        dataNormalizationService.UpdateMatchResult(MatchResult, matchResultUpdateSettings, NotifyProgress);

                        OnProgressUpdate?.Invoke(Resources.CAPTION_PREPARINGAUDITLOGS, 0);
                        var logs = auditLogGenerator.GetAuditLogs(AuditLogModule.Match, original, MatchResult, matchResultUpdateSettings);
                        _auditLogService.AddAuditLogs(logs);
                    }
                    else
                    {
                        dataNormalizationService.UpdateMatchResult(MatchResult, matchResultUpdateSettings, NotifyProgress);
                    }
                    OnMatchResultReady?.Invoke(true, true, MatchResultOperation.Update);
                }
                catch (Exception ex)
                {
                    _logger.Debug("UPDATE MATCH RESULT", ex);
                    OnException?.Invoke(Resources.EXCEPTION_MATCH_RESULT_NOT_UPDATED, "", MessagesType.Error, ex);
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DATA_UPDATING_WAIT, tsk, true, null);
        }

        public void MergeMatchResult(List<MergeMatchResultSetting> mergeSettings)
        {
            var tsk = new Task(() =>
            {
                try
                {
                    DataTable res = null;
                    var dataNormalizationService = WinPureConfigurationDependency.Resolve<IDataNormalizationService>();
                    var configurationService = WinPureConfigurationDependency.Resolve<IConfigurationService>();

                    if (configurationService.Configuration.EnableAuditLogs)
                    {
                        var auditLogGenerator = new AuditLogGenerator(_auditLogService);
                        var original = MatchResult.Copy();
                        res = dataNormalizationService.MergeMatchResult(MatchResult, mergeSettings, configurationService.Configuration.CleanMergeSeparator, NotifyProgress);

                        OnProgressUpdate?.Invoke(Resources.CAPTION_PREPARINGAUDITLOGS, 0);
                        var logs = auditLogGenerator.GetAuditLogs(AuditLogModule.Match, original, res, mergeSettings);
                        _auditLogService.AddAuditLogs(logs);
                    }
                    else
                    {
                        res = dataNormalizationService.MergeMatchResult(MatchResult, mergeSettings, configurationService.Configuration.CleanMergeSeparator, NotifyProgress);
                    }

                    if (res != null)
                    {
                        MatchResult = res;
                        OnMatchResultReady?.Invoke(true, true, MatchResultOperation.Merge);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Debug("MERGE MATCH RESULT", ex);
                    OnException?.Invoke(Resources.EXCEPTION_MATCH_RESULT_NOT_UPDATED, "", MessagesType.Error, ex);
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DATA_UPDATING_WAIT, tsk, true, null);
        }

        public void RemoveNotDuplicateRecords()
        {
            var tsk = new Task(() =>
            {
                var dataNormalizationService = WinPureConfigurationDependency.Resolve<IDataNormalizationService>();
                dataNormalizationService.RemoveNotDuplicateRecords(MatchResult);
                OnMatchResultReady?.Invoke(true, true, MatchResultOperation.NotDuplicate);
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DATA_PROCESSING_WAIT, tsk, false, null);
        }

        public void CreateNewDuplicateGroup()
        {
            var tsk = new Task(() =>
            {
                var dataNormalizationService = WinPureConfigurationDependency.Resolve<IDataNormalizationService>();
                dataNormalizationService.CreateNewDuplicateGroup(MatchResult);
                OnMatchResultReady?.Invoke(true, true, MatchResultOperation.Duplicate);
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DATA_PROCESSING_WAIT, tsk, false, null);
        }

        public void DeleteMergeMatchResult(DeleteFromMatchResultSetting deleteSettings)
        {
            var tsk = new Task(() =>
            {
                try
                {
                    var dataNormalizationService = WinPureConfigurationDependency.Resolve<IDataNormalizationService>();
                    var mr = dataNormalizationService.DeleteMergeMatchResult(MatchResult, deleteSettings);
                    if (mr != null)
                    {
                        var configurationService = WinPureConfigurationDependency.Resolve<IConfigurationService>();
                        if (configurationService.Configuration.EnableAuditLogs)
                        {
                            var auditLogGenerator = new AuditLogGenerator(_auditLogService);

                            OnProgressUpdate?.Invoke(Resources.CAPTION_PREPARINGAUDITLOGS, 0);
                            var logs = auditLogGenerator.GetAuditLogs(AuditLogModule.Match, MatchResult, mr, deleteSettings);
                            _auditLogService.AddAuditLogs(logs);
                        }

                        MatchResult = mr;
                        OnMatchResultReady?.Invoke(true, true, MatchResultOperation.Delete);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Debug("DELETE MERGE MATCH RESULT", ex);
                    OnException?.Invoke(Resources.EXCEPTION_MATCH_RESULT_NOT_PROCESSED, "", MessagesType.Error, ex);
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DATA_PROCESSING_WAIT, tsk, false, null);
        }

        #endregion

        #region Senzing Entity resolution

        public List<FieldType> GetEntityResolutionFieldTypes()
        {
            var senzingService = WinPureConfigurationDependency.Resolve<ISenzingService>();
            return senzingService.GetFieldTypes();
        }

        public DataTable GetDataTableEntityResolutionSetting(string tableName)
        {
            if (_settings.TableList.All(x => x.TableName != tableName))
            {
                OnException?.Invoke(Resources.EXCEPTION_NO_TABLE, "", MessagesType.Error, null);
                return null;
            }

            var dbTableName = NameHelper.GetCleanSettingsTable(tableName);
            var selectQry = SqLiteHelper.GetSelectQuery(dbTableName, "COLUMN_NAME, AI_Type, AI_Label, AI_Include, AI_Ignore");

            var result = SqLiteHelper.ExecuteQuery(selectQry, _connectionManager.Connection, CommandBehavior.Default, tableName);
            return result;
        }

        public void CleanEntityResolutionSetting(string tableName)
        {
            if (_settings.TableList.All(x => x.TableName != tableName))
            {
                OnException?.Invoke(Resources.EXCEPTION_NO_TABLE, "", MessagesType.Error, null);
            }
            var dbTableName = NameHelper.GetCleanSettingsTable(tableName);
            var updateQry = SqLiteHelper.GetUpdateQuery(dbTableName, "AI_Type = '', AI_Label = '', AI_Include = 1, AI_Ignore = 0", String.Empty);
            SqLiteHelper.ExecuteNonQuery(updateQry, _connectionManager.Connection);
        }

        public void SetDefaultEntityResolutionMapping(string tableName)
        {
            var entityConfigurationTable = GetDataTableEntityResolutionSetting(tableName);

            var rowConfiguration = entityConfigurationTable.AsEnumerable().Select(x =>
                new EntityResolutionRowConfiguration
                {
                    ColumnName = x["COLUMN_NAME"].ToString(),
                    FieldType = x["AI_Type"].ToString(),
                    Label = x["AI_Label"].ToString(),
                    IsIgnore = Convert.ToBoolean(x["AI_Ignore"]),
                    IsInclude = Convert.ToBoolean(x["AI_Include"])
                }).ToList();
            var senzingService = WinPureConfigurationDependency.Resolve<ISenzingService>();
            senzingService.MapColumns(rowConfiguration);

            var dbTableName = NameHelper.GetCleanSettingsTable(tableName);
            foreach (var row in rowConfiguration)
            {
                var updateQry = SqLiteHelper.GetUpdateQuery(dbTableName, $"AI_Type = '{row.FieldType}', AI_Label = '{row.Label}', AI_Include = {Convert.ToInt32(row.IsInclude)}, AI_Ignore = {Convert.ToInt32(row.IsIgnore)}", $"COLUMN_NAME = '{row.ColumnName}'");
                SqLiteHelper.ExecuteNonQuery(updateQry, _connectionManager.Connection);
            }
        }

        public void SetErMatchResultIsSelected(bool value, string whereCause, bool raiseUpdate = true)
        {
            var tsk = new Task(() =>
            {
                try
                {
                    if (raiseUpdate)
                    {
                        OnEntityResolutionStart?.Invoke();
                    }
                    var sql = $"UPDATE [{NameHelper.EntityResolutionTable}] SET [{WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED}] = {(value ? "1" : "0")} WHERE {whereCause}";
                    SqLiteHelper.ExecuteNonQuery(sql, _connectionManager.Connection);
                }
                catch (Exception ex)
                {
                    _logger.Debug("UPDATE ER IsSelected Fail", ex);
                    OnException?.Invoke(Resources.EXCEPTION_MATCH_RESULT_NOT_UPDATED, "", MessagesType.Error, ex);
                }
                finally
                {
                    if (raiseUpdate)
                    {
                        OnEntityResolutionReady?.Invoke();
                    }
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DATA_PROCESSING_WAIT, tsk, false, null);

        }

        public List<EntityResolutionConfiguration> VerifyFullEntityResolutionMap(IEnumerable<ImportedDataInfo> tables)
        {
            var configurations = new List<EntityResolutionConfiguration>();

            foreach (var table in tables)
            {
                var configuration = VerifyEntityResolutionTableMap(table);
                configurations.Add(configuration);
            }

            return configurations;
        }

        public EntityResolutionConfiguration VerifyEntityResolutionTableMap(ImportedDataInfo table)
        {
            var senzingService = WinPureConfigurationDependency.Resolve<ISenzingService>();

            var entityConfigurationTable = GetDataTableEntityResolutionSetting(table.TableName);

            var rowConfiguration = entityConfigurationTable.AsEnumerable().Select(x =>
                new EntityResolutionRowConfiguration
                {
                    ColumnName = x["COLUMN_NAME"].ToString(),
                    FieldType = x["AI_Type"].ToString(),
                    Label = x["AI_Label"].ToString(),
                    IsIgnore = x["AI_Ignore"] == DBNull.Value || Convert.ToBoolean(x["AI_Ignore"]),
                    IsInclude = x["AI_Include"] == DBNull.Value || Convert.ToBoolean(x["AI_Include"])
                }).ToList();

            var errors = senzingService.VerifyRowConfigurations(table.DisplayName, table.SourceType, rowConfiguration);

            var configuration = new EntityResolutionConfiguration
            {
                TableName = table.TableName,
                TableDisplayName = table.DisplayName,
                IsMainTable = table.IsErMainTable,
                Rows = rowConfiguration,
                RowCount = table.RowCount,
                Errors = errors,
                Source = table.SourceType,
                AdditionalParameters = table.AdditionalInfo
            };

            return configuration;
        }

        public void RunEntityResolution(CancellationTokenSource cts)
        {
            var senzingService = WinPureConfigurationDependency.Resolve<ISenzingService>();
            senzingService.OnProgressUpdate += NotifyProgress;
            try
            {
                if (!TableList.Any(x => x.IsResolutionSelected))
                {
                    throw new WinPureEntityResolutionException(Resources.MESSAGE_SELECRERSOURCE);
                }

                var configurations = VerifyFullEntityResolutionMap(TableList.Where(x => x.IsResolutionSelected));
                if (configurations.Any(x => x.Errors.Any()))
                {
                    var errors = configurations.SelectMany(x => x.Errors);
                    throw new WinPureArgumentException(string.Join(Environment.NewLine, errors.Select(x => x.Message)));
                }
                OnEntityResolutionStart?.Invoke();
                EntityResolutionReportData = senzingService.RunAnalyze(_connectionManager.DbPath, configurations, cts.Token);

                var configurationService = WinPureConfigurationDependency.Resolve<IConfigurationService>();
                if (configurationService.Configuration.AutoSetAiMasterRecord)
                {
                    OnProgressUpdate?.Invoke(Resources.CAPTION_DEFINING_MASTER_RECORD_WAIT, 95);
                    var mrSettings = new MasterRecordSettings { RecordType = MasterRecordType.MostPopulatedByTable, ApplyOptionsIfRuleGiveNothing = true };
                    DefineErMasterRecordInternally(mrSettings);
                }

                _auditLogService.AddSingleAuditLogIfEnabled("MatchAI result", $"Match table(s): {string.Join(",", configurations.Select(x => x.TableDisplayName).Distinct())}");
                OnEntityResolutionReady?.Invoke();
            }
            catch (OperationCanceledException)
            {
                OnException?.Invoke(Resources.CAPTION_OPERATION_CANCELLED, "", MessagesType.Error, null);
            }
            catch (AggregateException ex)
            {
                if (cts.IsCancellationRequested)
                {
                    OnException?.Invoke(Resources.CAPTION_OPERATION_CANCELLED, "", MessagesType.Error, null);
                }
                else
                {
                    Exception e = ex;
                    while (e.Message == "One or more errors occurs" && e.InnerException != null)
                    {
                        e = e.InnerException;
                    }
                    _logger.Debug("MatchAI error", e);
                    OnException?.Invoke(e.Message, "Internal error", MessagesType.Error, ex);
                }
            }
            catch (Exception ex)
            {
                OnException?.Invoke("MatchAI error: " + ex.Message, "", MessagesType.Error, ex);
            }
            finally
            {
                senzingService.OnProgressUpdate -= NotifyProgress;
            }
        }

        public void RunEntityResolutionAsync()
        {
            var cts = new CancellationTokenSource();

            var tsk = new Task(() =>
            {
                RunEntityResolution(cts);
            }, cts.Token);
            OnProgressShow?.Invoke("Step 1/5 Analyzing Data", tsk, true, cts);
        }

        public DataTable GetErDataTable(MatchResultViewType viewType, string whereCondition = "")
        {
            _connectionManager.CheckConnectionState();
            if (!SqLiteHelper.CheckTableExists(NameHelper.EntityResolutionTable, _connectionManager.Connection))
            {
                return null;
            }

            if (viewType == MatchResultViewType.PossibleDuplicates)
            {
                return GetPossibilities(PossibilityType.Duplicated);
            }

            if (viewType == MatchResultViewType.PossibleRelated)
            {
                return GetPossibilities(PossibilityType.Related);
            }

            var sql = SqLiteHelper.GetSelectQuery(NameHelper.EntityResolutionTable, String.Empty, whereCondition);
            var matchDataSchema = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection,
                CommandBehavior.SchemaOnly, NameHelper.EntityResolutionTable);
            matchDataSchema.Columns[WinPureColumnNamesHelper.WPCOLUMN_GROUPID].DataType = typeof(int);
            matchDataSchema.Columns[WinPureColumnNamesHelper.WPCOLUMN_ISMASTER].DataType = typeof(bool);
            matchDataSchema.Columns[WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED].DataType = typeof(bool);
            var erData = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection, CommandBehavior.Default, NameHelper.EntityResolutionTable, matchDataSchema);
            erData.TableName = NameHelper.EntityResolutionTable;
            return erData;

        }

        public int GetErMasterRecordsCount()
        {
            var sql = $"SELECT count(*) FROM [{NameHelper.EntityResolutionTable}] WHERE [{WinPureColumnNamesHelper.WPCOLUMN_ISMASTER}] = 1";
            var mrCount = SqLiteHelper.ExecuteScalar<int>(sql, _connectionManager.Connection);
            return mrCount;
        }

        public DataTable GetErTableSchema()
        {
            if (_connectionManager?.Connection == null || _connectionManager.Connection.State != ConnectionState.Open)
            {
                return null;
            }

            if (SqLiteHelper.CheckTableExists(NameHelper.EntityResolutionTable, _connectionManager.Connection))
            {
                var sql = SqLiteHelper.GetSelectQuery(NameHelper.EntityResolutionTable);
                var sourceData = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection,
                    CommandBehavior.SingleRow, NameHelper.EntityResolutionTable);
                return sourceData;
            }
            return null;
        }

        public void DefineErMasterRecord(MasterRecordSettings settings)
        {
            var tsk = new Task(() =>
            {
                try
                {
                    OnEntityResolutionStart?.Invoke();
                    if (settings.RecordType == MasterRecordType.ClearAll)
                    {
                        var updateQry =
                            $"UPDATE [{NameHelper.EntityResolutionTable}] SET [{WinPureColumnNamesHelper.WPCOLUMN_ISMASTER}] = 0";
                        SqLiteHelper.ExecuteNonQuery(updateQry, _connectionManager.Connection);
                    }
                    else
                    {
                        DefineErMasterRecordInternally(settings);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Debug("DEFINE ER MASTER RECORD", ex);
                    OnException?.Invoke(Resources.EXCEPTION_MASTER_RECORD_CANNOT_BE_DEFINED, "", MessagesType.Error, ex);
                }
                finally
                {
                    OnEntityResolutionReady?.Invoke();
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DEFINING_MASTER_RECORD_WAIT, tsk, false, null);
        }

        private void DefineErMasterRecordInternally(MasterRecordSettings settings)
        {
            LastErMasterRecordSettings = settings;

            var whereSql = EntityResolutionReportData.GroupWithDuplicates.Count < EntityResolutionReportData.GroupUnique.Count
                ? $"[{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] IN ({string.Join(",", EntityResolutionReportData.GroupWithDuplicates)}) "
                : $"[{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] NOT IN ({string.Join(",", EntityResolutionReportData.GroupUnique)}) ";

            var matchData = GetErDataTable(MatchResultViewType.OnlyGroup, whereSql);
            matchData.Columns.Remove(WinPureColumnNamesHelper.WPCOLUMN_MATCH_KEY);

            var dataNormalizationService =
                WinPureConfigurationDependency.Resolve<IDataNormalizationService>();

            if (dataNormalizationService.DefineMasterRecord(matchData, null, settings))
            {
                var mrIds = matchData.AsEnumerable()
                    .Where(x => x.Field<bool>(WinPureColumnNamesHelper.WPCOLUMN_ISMASTER))
                    .Select(x => x.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY)).ToList();
                var updateQry = $"UPDATE [{NameHelper.EntityResolutionTable}] SET [{WinPureColumnNamesHelper.WPCOLUMN_ISMASTER}] = 1 WHERE [{WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY}] IN ({string.Join(",", mrIds)})";
                SqLiteHelper.ExecuteNonQuery(updateQry, _connectionManager.Connection);
                if (settings.ApplyOptionsIfRuleGiveNothing)
                {
                    updateQry = $"UPDATE [{NameHelper.EntityResolutionTable}] SET [{WinPureColumnNamesHelper.WPCOLUMN_ISMASTER}] = 1 WHERE [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] IN ({string.Join(",", EntityResolutionReportData.GroupUnique)})";
                    SqLiteHelper.ExecuteNonQuery(updateQry, _connectionManager.Connection);
                }
            }
        }

        public void UpdateErMatchResult(List<UpdateMatchResultSetting> matchResultUpdateSettings)
        {
            var tsk = new Task(() =>
            {
                try
                {
                    OnEntityResolutionStart?.Invoke();
                    NotifyProgress(Matching.Properties.Resources.CAPTION_DATA_PREPARING, 5);
                    var whereSql = $"[{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] IN ({string.Join(",", EntityResolutionReportData.GroupWithDuplicates)}) ";
                    var deleteSql = SqLiteHelper.GetDeleteQuery(NameHelper.EntityResolutionTable, whereSql);

                    var erMatchResult = GetErDataTable(MatchResultViewType.OnlyGroup, whereSql);
                    var dataNormalizationService = WinPureConfigurationDependency.Resolve<IDataNormalizationService>();
                    var configurationService = WinPureConfigurationDependency.Resolve<IConfigurationService>();

                    if (configurationService.Configuration.EnableAuditLogs)
                    {
                        var original = erMatchResult.Copy();
                        var auditLogGenerator = new AuditLogGenerator(_auditLogService);

                        dataNormalizationService.UpdateMatchGroups(erMatchResult, matchResultUpdateSettings, NotifyProgress);

                        OnProgressUpdate?.Invoke(Resources.CAPTION_PREPARINGAUDITLOGS, 0);
                        var logs = auditLogGenerator.GetAuditLogs(AuditLogModule.MatchAI, original, erMatchResult, matchResultUpdateSettings);
                        _auditLogService.AddAuditLogs(logs);
                    }
                    else
                    {
                        dataNormalizationService.UpdateMatchGroups(erMatchResult, matchResultUpdateSettings, NotifyProgress);
                    }

                    OnProgressUpdate?.Invoke(Resources.CAPTION_SAVINGCHANGES, 0);
                    ColumnHelper.RemoveWinPurePrimaryKeyFieldFromTable(erMatchResult);
                    var filedList = string.Empty;
                    foreach (DataColumn column in erMatchResult.Columns)
                    {
                        filedList = string.IsNullOrEmpty(filedList) ? $"[{column.ColumnName}]" : $"{filedList},[{column.ColumnName}]";
                    }

                    NotifyProgress(Matching.Properties.Resources.CAPTION_PREPARING_FOR_DISPLAY, 90);

                    SqLiteHelper.ExecuteNonQuery(deleteSql, _connectionManager.Connection);
                    SqLiteHelper.AppendDataToDb(_connectionManager.Connection, erMatchResult, NameHelper.EntityResolutionTable, filedList, String.Empty);
                }
                catch (Exception ex)
                {
                    _logger.Debug("UPDATE ER MATCH RESULT", ex);
                    OnException?.Invoke(Resources.EXCEPTION_MATCH_RESULT_NOT_UPDATED, "", MessagesType.Error, ex);
                }
                finally
                {
                    OnEntityResolutionReady?.Invoke();
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DATA_UPDATING_WAIT, tsk, true, null);
        }

        public void RemoveErNotDuplicateRecords()
        {
            var tsk = new Task(() =>
            {
                try
                {
                    OnEntityResolutionStart?.Invoke();
                    var sql = $"SELECT * FROM [{NameHelper.EntityResolutionTable}] WHERE [{WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED}] = 1";
                    var nextGroupId = Math.Max(EntityResolutionReportData.GroupWithDuplicates.Any() ? EntityResolutionReportData.GroupWithDuplicates.Max() : 0,
                        EntityResolutionReportData.GroupUnique.Any() ? EntityResolutionReportData.GroupUnique.Max() : 0) + 1;
                    var matchedData = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection);

                    foreach (DataRow row in matchedData.Rows)
                    {
                        var updateQry = $"UPDATE [{NameHelper.EntityResolutionTable}] SET [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] = {nextGroupId++}, [{WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED}] = 0, [{WinPureColumnNamesHelper.WPCOLUMN_MATCH_KEY}] = '' WHERE [{WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY}] = {row[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY]}";
                        SqLiteHelper.ExecuteNonQuery(updateQry, _connectionManager.Connection);
                    }

                    UpdateErGroupList();
                }
                catch (Exception ex)
                {
                    _logger.Debug("UPDATE ER MATCH RESULT", ex);
                    OnException?.Invoke(Resources.EXCEPTION_MATCH_RESULT_NOT_UPDATED, "", MessagesType.Error, ex);
                }
                finally
                {
                    OnEntityResolutionReady?.Invoke();
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DATA_PROCESSING_WAIT, tsk, false, null);
        }

        public void DeleteErMergeMatchResult(DeleteFromMatchResultSetting deleteSettings)
        {
            var tsk = new Task(() =>
            {
                try
                {
                    OnEntityResolutionStart?.Invoke();
                    string whereCause = String.Empty;

                    switch (deleteSettings.DeleteSetting)
                    {
                        case DeleteMatchResultSetting.AllMatching:
                            whereCause = EntityResolutionReportData.GroupWithDuplicates.Count < EntityResolutionReportData.GroupUnique.Count
                                ? $"[{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] IN ({string.Join(",", EntityResolutionReportData.GroupWithDuplicates)}) "
                                : $"[{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] NOT IN ({string.Join(",", EntityResolutionReportData.GroupUnique)}) ";
                            break;
                        case DeleteMatchResultSetting.NonMaster:
                            whereCause = $"[{WinPureColumnNamesHelper.WPCOLUMN_ISMASTER}] = 0";
                            break;
                        case DeleteMatchResultSetting.AllSelected:
                            whereCause = $"[{WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED}] = 1";
                            break;
                        case DeleteMatchResultSetting.NonMatching:
                            whereCause = EntityResolutionReportData.GroupWithDuplicates.Count < EntityResolutionReportData.GroupUnique.Count
                                ? $"[{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] NOT IN ({string.Join(",", EntityResolutionReportData.GroupWithDuplicates)}) "
                                : $"[{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] IN ({string.Join(",", EntityResolutionReportData.GroupUnique)}) ";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    var configurationService = WinPureConfigurationDependency.Resolve<IConfigurationService>();
                    if (configurationService.Configuration.EnableAuditLogs)
                    {
                        var dataToDeleteSql = SqLiteHelper.GetSelectQuery(NameHelper.EntityResolutionTable, string.Empty, whereCause);
                        var removedData = SqLiteHelper.ExecuteQuery(dataToDeleteSql, _connectionManager.Connection);
                        var auditLogGenerator = new AuditLogGenerator(_auditLogService);

                        OnProgressUpdate?.Invoke(Resources.CAPTION_PREPARINGAUDITLOGS, 0);
                        var logs = auditLogGenerator.GetAuditLogs(
                            AuditLogModule.MatchAI,
                            removedData,
                            null,
                            deleteSettings);

                        _auditLogService.AddAuditLogs(logs);
                    }

                    OnProgressUpdate?.Invoke(Resources.CAPTION_SAVINGCHANGES, 0);
                    var deleteSql = SqLiteHelper.GetDeleteQuery(NameHelper.EntityResolutionTable, whereCause);
                    SqLiteHelper.ExecuteNonQuery(deleteSql, _connectionManager.Connection);

                    UpdateErGroupList();
                }
                catch (Exception ex)
                {
                    _logger.Debug("DELETE ER MERGE MATCH RESULT", ex);
                    OnException?.Invoke(Resources.EXCEPTION_MATCH_RESULT_NOT_PROCESSED, "", MessagesType.Error, ex);
                }
                finally
                {
                    OnEntityResolutionReady?.Invoke();
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DATA_PROCESSING_WAIT, tsk, false, null);
        }

        public void MergeErMatchResult(List<MergeMatchResultSetting> mergeSettings)
        {
            var tsk = new Task(() =>
            {
                try
                {
                    OnEntityResolutionStart?.Invoke();
                    DataTable res = null;

                    var dataNormalizationService = WinPureConfigurationDependency.Resolve<IDataNormalizationService>();
                    var configurationService = WinPureConfigurationDependency.Resolve<IConfigurationService>();

                    var erMatchResult = GetErDataTable(MatchResultViewType.OnlyGroup);

                    if (configurationService.Configuration.EnableAuditLogs)
                    {
                        var auditLogGenerator = new AuditLogGenerator(_auditLogService);
                        var original = erMatchResult.Copy();
                        res = dataNormalizationService.MergeMatchResult(erMatchResult, mergeSettings, configurationService.Configuration.CleanMergeSeparator, NotifyProgress);

                        OnProgressUpdate?.Invoke(Resources.CAPTION_PREPARINGAUDITLOGS, 0);
                        var logs = auditLogGenerator.GetAuditLogs(AuditLogModule.MatchAI, original, res, mergeSettings);
                        _auditLogService.AddAuditLogs(logs);
                    }
                    else
                    {
                        res = dataNormalizationService.MergeMatchResult(erMatchResult, mergeSettings, configurationService.Configuration.CleanMergeSeparator, NotifyProgress);
                    }

                    NotifyProgress(Matching.Properties.Resources.CAPTION_PREPARING_FOR_DISPLAY, 90);

                    SqLiteHelper.SaveDataToDb(_connectionManager.Connection, res, NameHelper.EntityResolutionTable, _logger);

                    UpdateErGroupList();
                }
                catch (Exception ex)
                {
                    _logger.Debug("MERGE ER MATCH RESULT", ex);
                    OnException?.Invoke(Resources.EXCEPTION_MATCH_RESULT_NOT_UPDATED, "", MessagesType.Error, ex);
                }
                finally
                {
                    OnEntityResolutionReady?.Invoke();
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DATA_UPDATING_WAIT, tsk, true, null);
        }

        public List<long> GetErUniqueTableGroups()
        {
            if (SqLiteHelper.CheckTableExists(NameHelper.EntityResolutionTable, _connectionManager.Connection))
            {
                var sql = $"SELECT [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] FROM {NameHelper.EntityResolutionTable} WHERE [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] IS NOT NULL GROUP BY [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] HAVING COUNT(*) > 1 AND COUNT(DISTINCT [{WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME}]) = 1";
                var data = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection);
                return data.AsEnumerable().Select(x => x.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID))
                    .ToList();
            }

            return new List<long>();
        }

        public List<long> GetErAcrossTableGroups()
        {
            if (SqLiteHelper.CheckTableExists(NameHelper.EntityResolutionTable, _connectionManager.Connection))
            {
                var sql = $"SELECT [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] FROM {NameHelper.EntityResolutionTable} WHERE [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] IS NOT NULL GROUP BY [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] HAVING COUNT(DISTINCT [{WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME}]) > 1";
                var data = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection);
                return data.AsEnumerable().Select(x => x.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID))
                    .ToList();
            }

            return new List<long>();
        }

        public DataTable GetPossibilities(PossibilityType possibilityType)
        {
            var qry = possibilityType switch
            {
                PossibilityType.Duplicated => SenzingHelper.GetPossibleDuplicatedView(),
                PossibilityType.Related => SenzingHelper.GetPossibleRelatedView(),
                _ => throw new ArgumentOutOfRangeException(nameof(possibilityType), possibilityType, null)
            };

            return SqLiteHelper.ExecuteQuery(qry, _connectionManager.Connection);
        }

        public void MergePossibilitiesToBiggestGroup(PossibilityType possibilityType)
        {
            var tsk = new Task(() =>
            {
                try
                {
                    OnEntityResolutionStart?.Invoke();
                    var possibleView = possibilityType == PossibilityType.Related
                        ? NameHelper.EntityResolutionRelatedView
                        : NameHelper.EntityResolutionDuplicatedView;

                    var relatedColumn = possibilityType == PossibilityType.Related
                        ? WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID
                        : WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID;

                    var sql = $"select * FROM [{(possibleView)}] WHERE [{WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED}] = 1";
                    var dataToUpdate = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection);

                    var groups = dataToUpdate.AsEnumerable().Select(x => x.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID)).ToList();
                    var relatedIds = dataToUpdate.AsEnumerable().Select(x => x.Field<long>(relatedColumn)).Distinct().ToList();

                    sql = $"select [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}], COUNT(*) as CNT FROM [{NameHelper.EntityResolutionTable}] WHERE [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] in ({string.Join(",", groups)}) GROUP BY [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] ";
                    var groupSize = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection);
                    var biggestGroup = groupSize.AsEnumerable().OrderByDescending(x => x.Field<long>("CNT")).FirstOrDefault()[WinPureColumnNamesHelper.WPCOLUMN_GROUPID];

                    sql = $"UPDATE [{NameHelper.EntityResolutionTable}] SET [{WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED}] = 0, [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] = {biggestGroup} WHERE [{WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED}] = 1";
                    SqLiteHelper.ExecuteNonQuery(sql, _connectionManager.Connection);

                    UpdateErGroupList();
                    CleanUpEmptyRelations(possibilityType, relatedIds);
                }
                catch (Exception ex)
                {
                    _logger.Debug("Error on merge ER possibilities to a biggest group", ex);
                    OnException?.Invoke(Resources.EXCEPTION_MATCH_RESULT_NOT_UPDATED, "", MessagesType.Error, ex);
                }
                finally
                {
                    OnEntityResolutionReady?.Invoke();
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DATA_UPDATING_WAIT, tsk, true, null);
        }

        public void MergePossibilitiesToNewGroup(PossibilityType possibilityType)
        {
            var tsk = new Task(() =>
            {
                try
                {
                    OnEntityResolutionStart?.Invoke();
                    var possibleView = possibilityType == PossibilityType.Related
                        ? NameHelper.EntityResolutionRelatedView
                        : NameHelper.EntityResolutionDuplicatedView;

                    var relatedColumn = possibilityType == PossibilityType.Related
                        ? WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID
                        : WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID;

                    var sql = $"select * FROM [{(possibleView)}] WHERE [{WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED}] = 1";
                    var dataToUpdate = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection);

                    var relatedIds = dataToUpdate.AsEnumerable().Select(x => x.Field<long>(relatedColumn)).Distinct().ToList();

                    var nextGroup = Math.Max(EntityResolutionReportData.GroupWithDuplicates.Any() ? EntityResolutionReportData.GroupWithDuplicates.Max() : 0,
                        EntityResolutionReportData.GroupUnique.Any() ? EntityResolutionReportData.GroupUnique.Max() : 0) + 1;

                    sql = $"UPDATE [{NameHelper.EntityResolutionTable}] SET [{WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED}] = 0, [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] = {nextGroup} WHERE [{WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED}] = 1";
                    SqLiteHelper.ExecuteNonQuery(sql, _connectionManager.Connection);
                    UpdateErGroupList();
                    CleanUpEmptyRelations(possibilityType, relatedIds);
                }
                catch (Exception ex)
                {
                    _logger.Debug("Error on merge ER possibilities to a new group", ex);
                    OnException?.Invoke(Resources.EXCEPTION_MATCH_RESULT_NOT_UPDATED, "", MessagesType.Error, ex);
                }
                finally
                {
                    OnEntityResolutionReady?.Invoke();
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DATA_UPDATING_WAIT, tsk, true, null);
        }

        public void MovePossibilitiesToSeparateGroups(PossibilityType possibilityType)
        {
            var tsk = new Task(() =>
            {
                try
                {
                    OnEntityResolutionStart?.Invoke();
                    var possibleView = possibilityType == PossibilityType.Related
                        ? NameHelper.EntityResolutionRelatedView
                        : NameHelper.EntityResolutionDuplicatedView;

                    var relatedColumn = possibilityType == PossibilityType.Related
                        ? WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID
                        : WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID;

                    var nextGroup = Math.Max(EntityResolutionReportData.GroupWithDuplicates.Max(), EntityResolutionReportData.GroupUnique.Max()) + 1;
                    var sql = $"select * FROM [{possibleView}] WHERE [{WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED}] = 1";
                    var dataToUpdate = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection);
                    var relatedIds = dataToUpdate.AsEnumerable().Select(x => x.Field<long>(relatedColumn)).Distinct().ToList();

                    foreach (DataRow row in dataToUpdate.Rows)
                    {
                        sql = $"UPDATE [{NameHelper.EntityResolutionTable}] SET [{WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED}] = 0, [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] = {nextGroup++} WHERE [{WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY}] = {row[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY]}";
                        SqLiteHelper.ExecuteNonQuery(sql, _connectionManager.Connection);
                    }

                    UpdateErGroupList();
                    CleanUpEmptyRelations(possibilityType, relatedIds);
                }
                catch (Exception ex)
                {
                    _logger.Debug("Error on merge ER possibilities to a separate groups", ex);
                    OnException?.Invoke(Resources.EXCEPTION_MATCH_RESULT_NOT_UPDATED, "", MessagesType.Error, ex);
                }
                finally
                {
                    OnEntityResolutionReady?.Invoke();
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_DATA_UPDATING_WAIT, tsk, true, null);
        }

        public void SaveErConfiguration(string fileName, ImportedDataInfo dataInfo = null)
        {
            try
            {
                var importedDataInfo = dataInfo ?? CurrentTableInfo;
                var configuration = VerifyFullEntityResolutionMap(new[] { importedDataInfo }).First();

                var senzingService = WinPureConfigurationDependency.Resolve<ISenzingService>();

                senzingService.ExportErSettings(configuration.Rows, fileName);
            }
            catch (Exception ex)
            {
                OnException?.Invoke(Resources.EXCEPTION_ER_SETTINGS_NOT_SAVED, "", MessagesType.Error, ex);
            }
        }

        public void LoadErConfiguration(string fileName, ImportedDataInfo dataInfo = null)
        {
            try
            {
                var senzingService = WinPureConfigurationDependency.Resolve<ISenzingService>();
                var rowConfigurations = senzingService.ImportErSettings(fileName);
                var importedDataInfo = dataInfo ?? CurrentTableInfo;
                var dbTableName = NameHelper.GetCleanSettingsTable(importedDataInfo.TableName);
                CleanEntityResolutionSetting(importedDataInfo.TableName);
                foreach (var field in importedDataInfo.Fields)
                {
                    var row = rowConfigurations.FirstOrDefault(x => x.ColumnName == field.DisplayName);
                    if (row != null)
                    {
                        var updateQry = SqLiteHelper.GetUpdateQuery(dbTableName, $"AI_Type = '{row.FieldType}', AI_Label = '{row.Label}', AI_Include = {Convert.ToInt32(row.IsInclude)}, AI_Ignore = {Convert.ToInt32(row.IsIgnore)}", $"COLUMN_NAME = '{row.ColumnName}'");
                        SqLiteHelper.ExecuteNonQuery(updateQry, _connectionManager.Connection);
                    }
                }
            }
            catch (Exception ex)
            {
                OnException?.Invoke(Resources.EXCEPTION_ER_FILE_WRONG_FORMAT, "", MessagesType.Error, ex);
            }
        }

        private void UpdateErGroupList()
        {
            var sql = $"select [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] FROM [{NameHelper.EntityResolutionTable}] GROUP BY [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] HAVING COUNT(*) > 1";
            var groupList = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection);
            EntityResolutionReportData.GroupWithDuplicates = groupList.AsEnumerable().Select(x => x.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID)).ToList();

            sql = $"select [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] FROM [{NameHelper.EntityResolutionTable}] GROUP BY [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] HAVING COUNT(*) = 1";
            groupList = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection);
            EntityResolutionReportData.GroupUnique = groupList.AsEnumerable().Select(x => x.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID)).ToList();
        }

        private void CleanUpEmptyRelations(PossibilityType possibilityType, List<long> entities)
        {
            var possibleView = possibilityType == PossibilityType.Related
                ? NameHelper.EntityResolutionRelatedView
                : NameHelper.EntityResolutionDuplicatedView;

            var relatedColumn = possibilityType == PossibilityType.Related
                ? WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID
                : WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID;

            var sql = $"select [{relatedColumn}] as Ids " +
                      $"from (select distinct [{relatedColumn}], [{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] from {possibleView} " +
                      $"where [{relatedColumn}] in ({string.Join(",", entities)})) a " +
                      $"GROUP BY [{relatedColumn}] " +
                      $"HAVING count([{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}]) = 1";

            var data = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection);
            var entityToDelete = data.AsEnumerable().Select(x => x.Field<long>("Ids")).Distinct().ToList();

            sql = $"delete from {NameHelper.EntityResolutionRelatedTable} where [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYID}] in ({string.Join(",", entityToDelete)}) and [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYTYPE}] = {(int)possibilityType}";
            SqLiteHelper.ExecuteNonQuery(sql, _connectionManager.Connection);
        }
        #endregion

        private string GetNextTableName()
        {
            int i = _settings.TableList.Count + 1;
            string tblName = "Table" + i;
            while (_settings.TableList.Any(x => x.TableName == tblName))
            {
                i++;
                tblName = "Table" + i;
            }
            return tblName;
        }

        private string GetTableDisplayName(string name)
        {
            int i = 1;
            string displayName = name;
            while (_settings.TableList.Any(x => x.DisplayName == displayName))
            {
                displayName = $"{name}_{i++}";
            }
            return displayName;
        }

        private List<string> GetTableColumns(string tableName)
        {
            var tblInfo = GetTableInfo(tableName);
            return tblInfo.Fields.Select(x => x.DisplayName).ToList();
        }

        private void NotifyException(string message, Exception exception)
        {
            OnException?.Invoke(message, "", MessagesType.Error, exception);
        }

        private void NotifyProgress(string message, int progress)
        {
            OnProgressUpdate?.Invoke(message, progress);
        }

        public void Dispose()
        {
            _connectionManager.Dispose();
            _auditLogService.Dispose();
            FileHelper.SafeDeleteFileWithLogging(_logger, _connectionManager.DbPath, "Cannot delete DB file on project closing");
        }
    }
}
