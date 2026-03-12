using System;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using WinPure.Automation.Enums;
using WinPure.Automation.Models;
using Winpure.AutomationService.DependencyInjection;
using WinPure.Common.Enums;
using WinPure.Common.Logger;
using WinPure.Configuration.Helper;
using WinPure.Configuration.Models.Configuration;
using WinPure.Configuration.Service;
using WinPure.DataService.Models;
using WinPure.DataService.Services;
using WinPure.Integration.Abstractions;
using WinPure.Integration.Export;
using WinPure.Matching.Enums;
using WinPure.Project.Services;
using WinPure.Integration.Models.ImportExportOptions;
using WinPure.Common.Helpers;
using WinPure.Configuration.DependencyInjection;
using WinPure.Project.Helpers;
using System.Collections.Generic;
using WinPure.Common.Models;

namespace Winpure.AutomationService
{
    internal static class AutomationExecutor
    {
        private static ExternalSourceTypes GetExportSourceType(string exportName)
        {
            switch (exportName)
            {
                case "Export to Access": return ExternalSourceTypes.Access;
                case "Export to Excel": return ExternalSourceTypes.Excel;
                case "Export to MySQL": return ExternalSourceTypes.MySqlServer;
                case "Export to SQL Server": return ExternalSourceTypes.SqlServer;
                case "Export to CSV": return ExternalSourceTypes.TextFile;
                case "Export to Oracle": return ExternalSourceTypes.Oracle;
                case "Export to PostgreSQL": return ExternalSourceTypes.Postgres;
                case "Export to Snowflake": return ExternalSourceTypes.Snowflake;
                case "Save to project": return ExternalSourceTypes.DataTable;
                default: return ExternalSourceTypes.NotDefined;
            }
        }

        internal static string RunAutomation(AutomationConfiguration config, CancellationTokenSource cancellationTokenSource)
        {
            if (!config.Steps.Any())
            {
                return "Empty configuration";
            }
            var tmpFile = Path.GetTempFileName();
            var logger = WinPureAutomationDependencyResolver.Resolve<IWpLogger>();
            string result = "";
            logger.Information($"AUTOMATION: {config.Name} has been processed");
            try
            {
                var cancellationToken = cancellationTokenSource.Token;
                bool copyBack = false;
                var step1 = config.Steps.FirstOrDefault(x => x.StepType == AutomationStepType.OpenProject);
                if (step1 == null)
                {
                    throw new ArgumentException("No project specified for configuration");
                }
                
                File.Copy(step1.SourceName, tmpFile, true);
                var connManager = WinPureConfigurationDependency.CreateConnectionManager();
                connManager.Initialize(tmpFile);
                connManager.CheckConnectionState();
                //ACHTUNG HERE
                WinPureAutomationDependencyResolver.Instance.Replace<IConnectionManager>(connManager);
                var projectSettings = new ProjectSettings(ProjectHelper.CurrentProjectVersion);
                WinPureAutomationDependencyResolver.Instance.Replace<ProjectSettings>(projectSettings);

                var dataManagerService = WinPureAutomationDependencyResolver.Resolve<IDataManagerService>();
                var projectService = WinPureAutomationDependencyResolver.Resolve<IProjectService>();
                var configurationService = WinPureAutomationDependencyResolver.Resolve<IConfigurationService>();

                Action<string, string, MessagesType, Exception> exceptionDelegate = (a1, a2, a3, a4) =>
                {
                    result = string.IsNullOrEmpty(result)? a1 : $"{result}, {a1}";
                    logger.Error($"AUTOMATION: Error from DataService. Message='{a1}'", a4?.Message??"", a4?.StackTrace??"");
                };
                dataManagerService.OnException += exceptionDelegate;
                projectService.OnException += exceptionDelegate;
                #region load project

                projectService.LoadProjectData(step1.SourceName);

                RefreshSourceTables(config, dataManagerService, cancellationToken);
                #endregion

                #region apply cleaning

                foreach (var step2 in config.Steps.Where(x => x.StepType == AutomationStepType.ApplyClean).OrderBy(x => x.Order))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var tblInfo = dataManagerService.GetTableInfoByDisplayName(step2.SourceName);
                    dataManagerService.SetSelectedTable(tblInfo.TableName);
                    dataManagerService.LoadCleanMatrix(step2.Param1);
                    var configuration = JsonConvert.DeserializeObject<WinPureConfiguration>(step2.Param2);
                    configurationService.Configuration.LoadConfiguration(configuration);
                    dataManagerService.CleanData(cancellationToken);
                }

                #endregion

                #region export clean result

                foreach (var step3 in config.Steps.Where(x => x.StepType == AutomationStepType.ExportCleanResult).OrderBy(x => x.Order))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var pSett = dataManagerService.TableList.FirstOrDefault(x => x.DisplayName == step3.SourceName);
                    if (pSett == null)
                    {
                        continue;
                    }
                    var table = SqLiteHelper.ExecuteQuery($"Select * from [{pSett.TableName}]", connManager.Connection);

                    var exportType = GetExportSourceType(step3.Param1);
                    if (exportType == ExternalSourceTypes.DataTable)
                    {
                        copyBack = true;
                        
                        table.TableName = pSett.DisplayName + "_CleanResult";
                        dataManagerService.OnProgressShow += OnProgressShow;
                        dataManagerService.SaveResultToData(false, MatchResultViewType.All, table.TableName, table);
                        dataManagerService.OnProgressShow -= OnProgressShow;

                    }
                    else
                    {
                        var exportService = ImportExportDataFactory.GetExportDataInstance(exportType);
                        var param = ImportExportDataFactory.GetExportDataParameters(exportType, step3.Param2);
                        if (exportType == ExternalSourceTypes.Excel && ((ExcelImportExportOptions)param).ExportWithNpoi)
                        {
                            exportService = new ExcelExportUniversalProvider();
                        }
                        exportService.OnException += (message, ex) => { result = string.IsNullOrEmpty(result) ? ex.Message : $"{result}, {ex.Message}"; };
                        exportService.Initialize(param);
                        cancellationToken.ThrowIfCancellationRequested();
                        exportService.Export(table);
                    }
                }

                #endregion

                #region matching

                var step4 = config.Steps.FirstOrDefault(x => x.StepType == AutomationStepType.PerformMatch);
                if (step4 != null)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    dataManagerService.MatchData(10, MatchAlgorithm.WinPureFuzzy, cancellationToken);

                    #region export match result

                    foreach (var step5 in config.Steps.Where(x => x.StepType == AutomationStepType.ExportMatchResult).OrderBy(x => x.Order))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        MatchResultViewType matchResultViewType = MatchResultViewType.All;
                        switch (step5.SourceName)
                        {
                            case "All data":
                                matchResultViewType = MatchResultViewType.All;
                                break;
                            case "Only groups":
                                matchResultViewType = MatchResultViewType.OnlyGroup;
                                break;
                            case "Across tables":
                                matchResultViewType = MatchResultViewType.AcrossTable;
                                break;
                            case "Non-matches":
                                matchResultViewType = MatchResultViewType.NonMatches;
                                break;
                            case "Non matches full":
                                matchResultViewType = MatchResultViewType.TableUnique;
                                break;
                        }
                        var dt = dataManagerService.GetMatchResult(matchResultViewType);
                        var exportType = GetExportSourceType(step5.Param1);

                        if (exportType == ExternalSourceTypes.DataTable)
                        {
                            dataManagerService.OnProgressShow += OnProgressShow;
                            dataManagerService.SaveResultToData(false, matchResultViewType, "MatchResult", dt);
                            dataManagerService.OnProgressShow -= OnProgressShow;
                            copyBack = true;
                        }
                        else
                        {
                            dt.TableName = "MatchResult";
                            var exportService = ImportExportDataFactory.GetExportDataInstance(exportType);
                            var param = ImportExportDataFactory.GetExportDataParameters(exportType, step5.Param2);
                            if (exportType == ExternalSourceTypes.Excel && ((ExcelImportExportOptions)param).ExportWithNpoi)
                            {
                                exportService = new ExcelExportUniversalProvider();
                            }

                            exportService.OnException += (message, ex) => { result = string.IsNullOrEmpty(result) ? ex.Message : $"{result}, {ex.Message}"; };
                            exportService.Initialize(param);
                            exportService.Export(dt);
                        }
                    }

                    #endregion
                }

                #endregion

                #region MatchAI
                var step6 = config.Steps.FirstOrDefault(x => x.StepType == AutomationStepType.PerformMatchAi);
                if (step6 != null)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    dataManagerService.RunEntityResolution(cancellationTokenSource);

                    #region export match result

                    foreach (var step7 in config.Steps.Where(x => x.StepType == AutomationStepType.ExportMatchAiResult).OrderBy(x => x.Order))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        MatchResultViewType matchResultViewType = MatchResultViewType.All;
                        var whereString = string.Empty;

                        switch (step7.SourceName)
                        {
                            case "All data":
                                matchResultViewType = MatchResultViewType.All;
                                break;
                            case "Only groups":
                                matchResultViewType = MatchResultViewType.OnlyGroup;
                                whereString = $"[{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] in ({string.Join(",", dataManagerService.EntityResolutionReportData.GroupWithDuplicates)}) ";
                                break;
                            case "Non-matches":
                                matchResultViewType = MatchResultViewType.NonMatches;
                                whereString = $"[{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] in ({string.Join(",", dataManagerService.EntityResolutionReportData.GroupUnique)}) ";
                                break;
                            case "Possible duplicates":
                                matchResultViewType = MatchResultViewType.PossibleDuplicates;
                                break;
                            case "Possible related":
                                matchResultViewType = MatchResultViewType.PossibleRelated;
                                break;
                        }
                        var dt = dataManagerService.GetErDataTable(matchResultViewType, whereString);
                        var exportType = GetExportSourceType(step7.Param1);

                        if (exportType == ExternalSourceTypes.DataTable)
                        {
                            dataManagerService.OnProgressShow += OnProgressShow;
                            dataManagerService.SaveResultToData(false, matchResultViewType, "MatchAiResult", dt);
                            dataManagerService.OnProgressShow -= OnProgressShow;
                            copyBack = true;
                        }
                        else
                        {
                            dt.TableName = "MatchAIResult";
                            var exportService = ImportExportDataFactory.GetExportDataInstance(exportType);
                            var param = ImportExportDataFactory.GetExportDataParameters(exportType, step7.Param2);
                            if (exportType == ExternalSourceTypes.Excel && ((ExcelImportExportOptions)param).ExportWithNpoi)
                            {
                                exportService = new ExcelExportUniversalProvider();
                            }

                            exportService.OnException += (message, ex) => { result = string.IsNullOrEmpty(result) ? ex.Message : $"{result}, {ex.Message}"; };
                            exportService.Initialize(param);
                            exportService.Export(dt);
                        }
                    }

                    #endregion
                }
                #endregion

                if (copyBack)
                {
                    projectService.OnProgressShow += OnProgressShow;
                    projectService.SaveProjectAsync(projectSettings.ProjectName, true, step1.SourceName);
                    projectService.OnProgressShow -= OnProgressShow;
                }
                connManager.CloseConnection();
            }
            catch (OperationCanceledException)
            {
                logger.Information("Automatic run was cancelled");
                return "Operation was cancelled";
            }
            catch (Exception ex)
            {
                logger.Debug("Cannot process automation configuration", ex);
                result = string.IsNullOrEmpty(result) ? ex.Message : $"{result}, {ex.Message}";
            }
            finally
            {

                if (File.Exists(tmpFile))
                {
                    try
                    {
                        File.Delete(tmpFile);
                    }
                    catch (Exception e)
                    {
                        logger.Error("Automation cannot delete temp file", e.Message);
                    }
                }
            }
            return result;
        }

        private static void OnProgressShow(string caption, System.Threading.Tasks.Task saveTask, bool arg3, CancellationTokenSource cancellationToken)
        {
            saveTask.Start();
            saveTask.Wait();
        }

        private static void RefreshSourceTables(AutomationConfiguration config, IDataManagerService dataManagerService, CancellationToken cancellationToken)
        {
            var processedTableInfo = new List<ImportedDataInfo>();
            foreach (var step in config.Steps.Where(x => x.StepType == AutomationStepType.ApplyClean).OrderBy(x => x.Order))
            {
                var tblInfo = dataManagerService.GetTableInfoByDisplayName(step.SourceName);
                processedTableInfo.Add(tblInfo);
            }
            foreach (var step in config.Steps.Where(x => x.StepType == AutomationStepType.ExportCleanResult).OrderBy(x => x.Order))
            {
                var tblInfo = dataManagerService.GetTableInfoByDisplayName(step.SourceName);
                processedTableInfo.Add(tblInfo);
            }

            var isMatching = config.Steps.FirstOrDefault(x => x.StepType == AutomationStepType.PerformMatch);
            if (isMatching != null && dataManagerService.MatchSettings != null)
            {
                dataManagerService.MatchSettings.MatchParameters.Select(x => x.FieldMap).SelectMany(x => x.FieldMap).Select(x => x.TableName).Distinct().ToList().ForEach(tableName =>
                {
                    var tblInfo = dataManagerService.GetTableInfo(tableName);
                    if (tblInfo != null)
                    {
                        processedTableInfo.Add(tblInfo);
                    }
                });
            }

            var isMatchAi = config.Steps.FirstOrDefault(x => x.StepType == AutomationStepType.PerformMatchAi);
            if (isMatchAi != null)
            {
                processedTableInfo.AddRange(dataManagerService.TableList.Where(x => x.IsResolutionSelected).ToList());
            }

            // processedTableInfo we need only unique tables
            processedTableInfo = processedTableInfo.GroupBy(x => x.TableName).Select(g => g.First()).ToList();

            // Refresh required tables
            foreach (var tbl in processedTableInfo)
            {
                cancellationToken.ThrowIfCancellationRequested();
                dataManagerService.SetSelectedTable(tbl.TableName);

                dataManagerService.ReimportData(tbl.TableName);
            }
        }
    }
}