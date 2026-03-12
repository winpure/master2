using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WinPure.AddressVerification.Models;
using WinPure.Common.Enums;
using WinPure.Common.Exceptions;
using WinPure.Common.Helpers;
using WinPure.Configuration.Helper;
using WinPure.DataService.AuditLogs;
using WinPure.DataService.Senzing.Models;
using WinPure.Matching.Models;
using WinPure.Matching.Models.Support;
using WinPure.Project.Helpers;
using WinPure.Project.Properties;

namespace WinPure.Project.DependencyInjection;

internal static partial class WinPureProjectDependencyExtension
{
    private class ProjectService : IProjectService
    {
        public static string DbSettingsTableName = "WinPureSettingsData";
        public static string DbMatchfieldsTableName = "WinPureMatchingFiledsData";
        public static string DbMatchResultTableName = "WinPureMatchingResultData";

        private readonly ProjectSettings _settings;
        private readonly IProjectMigrationService _projectMigrationService;
        private readonly IWpLogger _logger;
        private readonly IRecentProjectService _recentProjectService;
        private readonly IDataManagerService _dataManagerService;
        private readonly IConnectionManager _connectionManager;
        private readonly ILogConnectionManager _logConnectionManager;
        private readonly IAuditLogService _auditLogService;
        private readonly string _projectDbPath;
        private readonly string _auditLogDbPath;

        public bool IsNewProject => String.IsNullOrEmpty(_settings.ProjectPath);
        public event Action OnAfterProjectLoad;
        public event Action OnBeforeProjectLoad;
        public event Action<string> OnProjectNameChanged;
        public event Action OnCleanedProject;

        public event Action<string, Task, bool, CancellationTokenSource> OnProgressShow;
        public event Action<string, string, MessagesType, Exception> OnException;


        public ProjectService(IWpLogger logger,
            IRecentProjectService recentProjectService,
            IDataManagerService dataManagerService,
            IConnectionManager connectionManager,
            ILogConnectionManager logConnectionManager,
            IAuditLogService auditLogService,
            ProjectSettings settings,
            IProjectMigrationService projectMigrationService)
        {
            _settings = settings;
            _projectMigrationService = projectMigrationService;
            _logger = logger;
            _recentProjectService = recentProjectService;
            _dataManagerService = dataManagerService;
            _connectionManager = connectionManager;
            _logConnectionManager = logConnectionManager;
            _auditLogService = auditLogService;
            _projectDbPath = connectionManager.DbPath;
            _auditLogDbPath = logConnectionManager.DbPath;
        }

        public void LoadProjectAsync(string fileName)
        {
            var tsk = new Task(() =>
            {
                try
                {
                    OnBeforeProjectLoad?.Invoke();
                    LoadProjectData(fileName);
                    _settings.ProjectPath = fileName;
                    OnProjectNameChanged?.Invoke(_settings.ProjectName);
                    foreach (var tbl in _settings.TableList)
                    {
                        _dataManagerService.RaiseAddNewData(tbl.TableName);
                        _settings.CurrentTable = tbl.TableName;
                        if (tbl.IsSelected && _dataManagerService.SelectedColumns.Columns.Count == 0)
                        {
                            _dataManagerService.UpdateSelectedForMatchingTables(tbl.TableName);
                        }
                    }
                    OnAfterProjectLoad?.Invoke();
                }
                catch (WinPureLoadProjectException wpEx)
                {
                    OnException?.Invoke(wpEx.Message, Resources.EXCEPTION_PROJECT_OPEN_CAPTION, MessagesType.Error, null);
                }
                catch (Exception ex)
                {
                    _logger.Debug("LOAD PROJECT", ex);
                    OnException?.Invoke(Resources.EXCEPTION_PROJECT_FILE_CORRUPTED, Resources.EXCEPTION_PROJECT_OPEN_CAPTION, MessagesType.Error, null);
                }
                finally
                {
                    _connectionManager.Initialize(_projectDbPath);
                }
            });
            OnProgressShow?.Invoke(Resources.CAPTION_OPEN_PROJECT, tsk, false, null);
        }

        public bool CloseCurrentAndCreateNewProject()
        {
            try
            {
                OnBeforeProjectLoad?.Invoke();
                ClearProject();
                _settings.ResetSettings();
                _connectionManager.Initialize(_projectDbPath);
                _connectionManager.CheckConnectionState();
                _logConnectionManager.Initialize(_auditLogDbPath);
                _logConnectionManager.CheckConnectionState();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Debug("CANNOT CREATE NEW PROJECT", ex);
                OnException?.Invoke(Resources.EXCEPTION_NEW_PROJECT_CANNOT_CREATE, "", MessagesType.Error, ex);
            }
            return false;
        }

        public void SaveProjectAsync(string projectName, bool saveAs, string projectPath = "")
        {
            if (String.IsNullOrEmpty(projectPath))
            {
                projectPath = _settings.ProjectPath;
            }

            SetProjectName(projectName);

            if (CanSaveProject() && !String.IsNullOrEmpty(projectPath))
            {
                var tsk = new Task(async () =>
                {
                    try
                    {
                        _settings.ProjectPath = projectPath;
                        _connectionManager.CheckConnectionState();

                        PrepareSettingsTable();

                        if (saveAs)
                        {
                            _settings.GenerateNewId();
                        }

                        SqLiteHelper.ExecuteNonQuery($"INSERT INTO {DbSettingsTableName}(SettingsName, WPSettings) VALUES ('ProjectSettings','{_settings.ToString().Replace("'", "''")}')", _connectionManager.Connection);

                        SqLiteHelper.ExecuteNonQuery($"INSERT INTO {DbSettingsTableName}(SettingsName, WPSettings) VALUES ('MatchSettings','{_dataManagerService.MatchSettings}')", _connectionManager.Connection);

                        SqLiteHelper.ExecuteNonQuery(
                            _dataManagerService.LastMatchingParameters != null
                                ? $"INSERT INTO {DbSettingsTableName}(SettingsName, WPSettings) VALUES ('LastMatchParameters','{_dataManagerService.LastMatchingParameters}')"
                                : $"INSERT INTO {DbSettingsTableName}(SettingsName, WPSettings) VALUES ('LastMatchParameters','')",
                            _connectionManager.Connection);

                        SqLiteHelper.ExecuteNonQuery(
                            _dataManagerService.LastMasterRecordSettings != null
                                ? $"INSERT INTO {DbSettingsTableName}(SettingsName, WPSettings) VALUES ('LastMasterRecordSettings','{_dataManagerService.LastMasterRecordSettings}')"
                                : $"INSERT INTO {DbSettingsTableName}(SettingsName, WPSettings) VALUES ('LastMasterRecordSettings','')",
                            _connectionManager.Connection);

                        SqLiteHelper.ExecuteNonQuery(
                            _dataManagerService.LastErMasterRecordSettings != null
                                ? $"INSERT INTO {DbSettingsTableName}(SettingsName, WPSettings) VALUES ('LastErMasterRecordSettings','{_dataManagerService.LastErMasterRecordSettings}')"
                                : $"INSERT INTO {DbSettingsTableName}(SettingsName, WPSettings) VALUES ('LastErMasterRecordSettings','')",
                            _connectionManager.Connection);

                        var verificationResultJson = JsonConvert.SerializeObject(_dataManagerService.VerificationResults);
                        SqLiteHelper.ExecuteNonQuery($"INSERT INTO {DbSettingsTableName}(SettingsName, WPSettings) VALUES ('VerificationResults','{verificationResultJson}')", _connectionManager.Connection);

                        var entityResolutionResultJson = JsonConvert.SerializeObject(_dataManagerService.EntityResolutionReportData);
                        SqLiteHelper.ExecuteNonQuery($"INSERT INTO {DbSettingsTableName}(SettingsName, WPSettings) VALUES ('EntityResolutionResults','{entityResolutionResultJson}')", _connectionManager.Connection);

                        if (_dataManagerService.SelectedColumns?.Columns.Count > 0)
                        {
                            SqLiteHelper.SaveDataToDb(_connectionManager.Connection, _dataManagerService.SelectedColumns, DbMatchfieldsTableName, _logger, false);
                        }

                        if (_dataManagerService.MatchResult != null)
                        {
                            SqLiteHelper.SaveDataToDb(_connectionManager.Connection, _dataManagerService.MatchResult, DbMatchResultTableName, _logger);
                        }

                        FileHelper.SafeDeleteFile(projectPath);

                        File.Copy(_projectDbPath, projectPath);

                        await _recentProjectService.AddOrUpdateProjectAsync(_settings);

                        if (_auditLogService.LogExists())
                        {
                            var auditLogProjectPath = CreateAuditLogProjectFilePath(projectPath);
                            FileHelper.SafeDeleteFile(auditLogProjectPath);
                            File.Copy(_auditLogDbPath, auditLogProjectPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Debug("SAVE PROJECT", ex);
                        OnException?.Invoke(Resources.EXCEPTION_PROJECT_SAVE_ERROR, "", MessagesType.Error, ex);
                    }
                });
                OnProgressShow?.Invoke(Resources.CAPTION_SAVE_PROJECT, tsk, false, null);
            }
        }

        public void SetProjectName(string projectName)
        {
            _settings.ProjectName = projectName;
            OnProjectNameChanged?.Invoke(_settings.ProjectName);
        }

        public void PreLoadProject(IConnectionManager connManager, ProjectSettings settings, MatchSettingsViewModel matchSettings)
        {
            if (SqLiteHelper.CheckTableExists(DbSettingsTableName, connManager.Connection))
            {
                var settingsTable = SqLiteHelper.ExecuteQuery($"Select * from [{DbSettingsTableName}] ", connManager.Connection, CommandBehavior.Default, DbSettingsTableName);

                bool isProjectSettingsProcessed = false;
                foreach (DataRow settingRow in settingsTable.Rows)
                {
                    var settingsJson = settingRow["WPSettings"].ToString();
                    if (string.IsNullOrWhiteSpace(settingsJson))
                    {
                        continue;
                    }

                    switch (settingRow["SettingsName"].ToString())
                    {
                        case "ProjectSettings":
                            var projectSettings = JsonConvert.DeserializeObject<ProjectSettings>(settingsJson);
                            settings.InitiateSettings(projectSettings);
                            isProjectSettingsProcessed = true;
                            break;
                        case "MatchSettings":
                            var sett = JsonConvert.DeserializeObject<MatchSettingsViewModel>(settingsJson);
                            matchSettings.InitiateSettings(sett);
                            break;
                    }
                }

                if (!isProjectSettingsProcessed)
                {
                    throw new WinPureLoadProjectException(Resources.EXCEPTION_PROJECT_WRONG_FILE_FORMAT);
                }
            }
            else
            {
                throw new WinPureLoadProjectException(Resources.EXCEPTION_PROJECT_WRONG_FILE_FORMAT);
            }
        }

        public void LoadProjectData(string projectPath)
        {
            ClearProject();

            File.Copy(projectPath, _projectDbPath);

            _connectionManager.Initialize(_projectDbPath);
            _connectionManager.CheckConnectionState();
            if (SqLiteHelper.CheckTableExists(DbSettingsTableName, _connectionManager.Connection))
            {
                var settingsTable = SqLiteHelper.ExecuteQuery($"Select * from [{DbSettingsTableName}] ", _connectionManager.Connection, CommandBehavior.Default, DbSettingsTableName);
                bool isProjectSettingsProcessed = false;
                foreach (DataRow settingRow in settingsTable.Rows)
                {
                    var settingsJson = settingRow["WPSettings"].ToString();
                    if (string.IsNullOrWhiteSpace(settingsJson))
                    {
                        continue;
                    }

                    switch (settingRow["SettingsName"].ToString())
                    {
                        case "ProjectSettings":
                            var sett = JsonConvert.DeserializeObject<ProjectSettings>(settingsJson);
                            var recentProject = _recentProjectService.GetRecentProjectByPath(projectPath).Result;
                            if (recentProject != null && !string.Equals(sett.ProjectName, recentProject.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                sett.ProjectName = recentProject.Name;
                            }
                            _settings.InitiateSettings(sett);
                            isProjectSettingsProcessed = true;
                            break;
                        case "MatchSettings":
                            _dataManagerService.MatchSettings = JsonConvert.DeserializeObject<MatchSettingsViewModel>(settingsJson);
                            break;
                        case "LastMatchParameters":
                            _dataManagerService.LastMatchingParameters = JsonConvert.DeserializeObject<MatchParameter>(settingsJson);
                            break;
                        case "LastMasterRecordSettings":
                            _dataManagerService.LastMasterRecordSettings = (string.IsNullOrWhiteSpace(settingsJson))
                                ? null
                                : JsonConvert.DeserializeObject<MasterRecordSettings>(settingsJson);
                            break;
                        case "LastErMasterRecordSettings":
                            _dataManagerService.LastErMasterRecordSettings = (string.IsNullOrWhiteSpace(settingsJson))
                                ? null
                                : JsonConvert.DeserializeObject<MasterRecordSettings>(settingsJson);
                            break;
                        case "VerificationResults":
                            _dataManagerService.VerificationResults = JsonConvert.DeserializeObject<Dictionary<string, AddressVerificationReport>>(settingsJson);
                            break;
                        case "EntityResolutionResults":
                            _dataManagerService.EntityResolutionReportData = JsonConvert.DeserializeObject<EntityResolutionReport>(settingsJson);
                            break;
                    }
                }

                if (!isProjectSettingsProcessed)
                {
                    throw new WinPureLoadProjectException(Resources.EXCEPTION_PROJECT_WRONG_FILE_FORMAT);
                }

                _projectMigrationService.Up(_connectionManager, _settings, _dataManagerService.MatchSettings, _dataManagerService.LastMatchingParameters);

                if (SqLiteHelper.CheckTableExists(DbMatchfieldsTableName, _connectionManager.Connection))
                {
                    var sql = $"Select * from [{DbMatchfieldsTableName}]";
                    var tbl = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection, CommandBehavior.SchemaOnly, DbMatchfieldsTableName);
                    ColumnHelper.RemoveWinPurePrimaryKeyFieldFromTable(tbl);

                    tbl.Columns[WinPureColumnNamesHelper.WPCOLUMN_INCLUDE_IN_RESULT].DataType = typeof(bool);
                    tbl.Columns[WinPureColumnNamesHelper.WPCOLUMN_INCLUDE_IN_RESULT].DefaultValue = true;

                    _dataManagerService.SelectedColumns = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection, CommandBehavior.Default, DbMatchfieldsTableName, tbl);
                }

                if (SqLiteHelper.CheckTableExists(DbMatchResultTableName, _connectionManager.Connection))
                {
                    var sql = $"Select * from [{DbMatchResultTableName}]";
                    var tbl = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection, CommandBehavior.SchemaOnly, DbMatchfieldsTableName);
                    tbl.PrimaryKey = new DataColumn[0];

                    tbl.Columns[WinPureColumnNamesHelper.WPCOLUMN_GROUPID].DataType = typeof(int);
                    tbl.Columns[WinPureColumnNamesHelper.WPCOLUMN_ISMASTER].DataType = typeof(bool);
                    tbl.Columns[WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED].DataType = typeof(bool);

                    _dataManagerService.MatchResult = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection, CommandBehavior.Default, DbMatchfieldsTableName, tbl);

                    foreach (DataRow rw in _dataManagerService.MatchResult.Rows)
                    {
                        if (rw[WinPureColumnNamesHelper.WPCOLUMN_ISMASTER] == null) rw[WinPureColumnNamesHelper.WPCOLUMN_ISMASTER] = false;
                        if (rw[WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED] == null) rw[WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED] = false;
                    }
                    _dataManagerService.UpdateMatchReport();
                }

                var auditLogProjectPath = CreateAuditLogProjectFilePath(projectPath);
                if (File.Exists(auditLogProjectPath))
                {
                    File.Copy(auditLogProjectPath, _auditLogDbPath, true);
                }
                _logConnectionManager.Initialize(_auditLogDbPath);
                _logConnectionManager.CheckConnectionState();
            }
            else
            {
                throw new WinPureLoadProjectException(Resources.EXCEPTION_PROJECT_WRONG_FILE_FORMAT);
            }
        }

        private bool CanSaveProject()
        {
            if (String.IsNullOrEmpty(_settings.ProjectName))
            {
                OnException?.Invoke(Resources.EXCEPTION_SET_PROJECT_NAME_BEFORE_SAVING_PROJECT, "", MessagesType.Error, null);
                return false;
            }
            return true;
        }

        private void PrepareSettingsTable()
        {
            if (!SqLiteHelper.ClearTable(_connectionManager.Connection, DbSettingsTableName))
            {
                SqLiteHelper.ExecuteNonQuery($"create table [{DbSettingsTableName}] (SettingsName TEXT, WPSettings TEXT)", _connectionManager.Connection);
            }

            SqLiteHelper.ClearTable(_connectionManager.Connection, DbMatchfieldsTableName);

        }

        private void ClearProject()
        {
            OnCleanedProject?.Invoke();

            _connectionManager.CloseConnection();
            _logConnectionManager.CloseConnection();
            _settings.InitiateSettings(new ProjectSettings(ProjectHelper.CurrentProjectVersion));

            _dataManagerService.LastMatchingParameters = new MatchParameter();
            _dataManagerService.LastMasterRecordSettings = null;
            _dataManagerService.LastErMasterRecordSettings = null;
            _dataManagerService.MatchSettings = new MatchSettingsViewModel();
            _dataManagerService.MatchResult = null;
            _dataManagerService.ReportData = null;
            _dataManagerService.EntityResolutionReportData = null;
            _dataManagerService.SelectedColumns = new DataTable();

            File.Delete(_projectDbPath);
            FileHelper.SafeDeleteFile(_auditLogDbPath);
        }

        private string CreateAuditLogProjectFilePath(string projectFile)
        {
            var fileName = Path.GetFileNameWithoutExtension(projectFile);
            var directory = Path.GetDirectoryName(projectFile);
            var fileExtension = Path.GetFileName(projectFile).Replace(fileName, "");
            var auditLogFileName = $"{fileName}_Log{fileExtension}";
            return Path.Combine(directory, auditLogFileName);
        }
    }
}