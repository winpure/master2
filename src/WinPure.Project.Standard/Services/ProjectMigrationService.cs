using WinPure.Common.Helpers;
using WinPure.Configuration.Helper;
using WinPure.Matching.Models;
using WinPure.Project.Helpers;

namespace WinPure.Project.DependencyInjection;

internal static partial class WinPureProjectDependencyExtension
{
    private class ProjectMigrationService : IProjectMigrationService
    {
        public void Down(IConnectionManager connectionManager, ProjectSettings projectSettings, MatchSettingsViewModel matchSettings, MatchParameter matchParameter)
        {}

        public void Up(IConnectionManager connectionManager, ProjectSettings projectSettings, MatchSettingsViewModel matchSettings, MatchParameter matchParameter)
        {
            if (projectSettings.Version == ProjectHelper.CurrentProjectVersion)
            {
                return;
            }

            Up_AddProperCaseSettings(connectionManager, projectSettings);
            Up_RenameGroups(connectionManager, projectSettings);
            Up_AddEntityResolutionFields(connectionManager, projectSettings);
            Up_RenameAAdressSplitZipColumn(connectionManager, projectSettings);

            projectSettings.Version = ProjectHelper.CurrentProjectVersion;
        }

        private bool IsUp(string projectVersion, string targetVersion)
        {
            if (string.IsNullOrEmpty(projectVersion))
            {
                return true;
            }

            var projectVersionDetail = new Version(projectVersion);
            var targetVersionDetail = new Version(targetVersion);

            return projectVersionDetail < targetVersionDetail;
        }

        private void Up_AddProperCaseSettings(IConnectionManager connectionManager, ProjectSettings projectSettings)
        {
            if (!IsUp(projectSettings.Version, "1.0.0.0"))
            {
                return;
            }

            foreach (var importedDataInfo in projectSettings.TableList)
            {
                var selectCleanSettingsQuery = SqLiteHelper.GetCleanSettingsQuery(importedDataInfo.TableName);
                var cleanSettings = SqLiteHelper.ExecuteQuery(selectCleanSettingsQuery, connectionManager.Connection, CommandBehavior.SchemaOnly, importedDataInfo.TableName);

                if (!cleanSettings.Columns.Contains("ST_ProperCaseSettings"))
                {
                    SqLiteHelper.AddTableColumn(connectionManager.Connection, NameHelper.GetCleanSettingsTable(importedDataInfo.TableName), "ST_ProperCaseSettings", typeof(string), false);
                }
            }

        }

        private void Up_RenameGroups(IConnectionManager connectionManager, ProjectSettings projectSettings)
        {
            if (!IsUp(projectSettings.Version, "1.0.0.0"))
            {
                return;
            }

            if (SqLiteHelper.CheckTableExists(ProjectService.DbMatchResultTableName, connectionManager.Connection))
            {
                string scrColumns = "";
                string dstColumns = "";

                var tbl = SqLiteHelper.ExecuteQuery($"Select * from [{ProjectService.DbMatchResultTableName}]", connectionManager.Connection, CommandBehavior.SchemaOnly, ProjectService.DbMatchfieldsTableName);
                tbl.TableName = ProjectService.DbMatchResultTableName + "_temp";

                ColumnHelper.RemoveWinPurePrimaryKeyFieldFromTable(tbl);

                foreach (DataColumn column in tbl.Columns)
                {
                    scrColumns += $"[{column.ColumnName}],";
                    
                    if (column.ColumnName.ToLower().StartsWith("group") &&
                        column.ColumnName.ToLower().EndsWith("score"))
                    {
                        var newName = column.ColumnName.Replace("Group", "Rule").Replace("score","Score");
                        column.ColumnName = ColumnHelper.GetDataTableUniqueColumnName(tbl, newName);
                    }

                    if (column.ColumnName.ToLower() == WinPureColumnNamesHelper.WPCOLUMN_TOTALSCORE.ToLower())
                    {
                        column.ColumnName = WinPureColumnNamesHelper.WPCOLUMN_TOTALSCORE;
                    }

                    dstColumns += $"[{column.ColumnName}],";
                }

                scrColumns = scrColumns.Substring(0, scrColumns.Length - 1);
                dstColumns = dstColumns.Substring(0, dstColumns.Length - 1);
                SqLiteHelper.CreateTableSchema(connectionManager.Connection, tbl);
                var sql = $"INSERT INTO [{tbl.TableName}] ({dstColumns}) SELECT {scrColumns} FROM [{ProjectService.DbMatchResultTableName}]";
                SqLiteHelper.ExecuteNonQuery(sql, connectionManager.Connection);
                SqLiteHelper.ExecuteNonQuery(SqLiteHelper.GetDropTableQuery(ProjectService.DbMatchResultTableName), connectionManager.Connection);
                SqLiteHelper.ChangeTableName(connectionManager.Connection, tbl.TableName, ProjectService.DbMatchResultTableName);
            }
        }


        private void Up_AddEntityResolutionFields(IConnectionManager connectionManager, ProjectSettings projectSettings)
        {
            if (!IsUp(projectSettings.Version, "1.0.0.1"))
            {
                return;
            }

            foreach (var importedDataInfo in projectSettings.TableList)
            {
                var selectCleanSettingsQuery = SqLiteHelper.GetCleanSettingsQuery(importedDataInfo.TableName);
                var cleanSettings = SqLiteHelper.ExecuteQuery(selectCleanSettingsQuery, connectionManager.Connection, CommandBehavior.SchemaOnly, importedDataInfo.TableName);

                if (!cleanSettings.Columns.Contains("AI_Type"))
                {
                    SqLiteHelper.AddTableColumn(connectionManager.Connection, NameHelper.GetCleanSettingsTable(importedDataInfo.TableName), "AI_Type", typeof(string), false);
                }

                if (!cleanSettings.Columns.Contains("AI_Label"))
                {
                    SqLiteHelper.AddTableColumn(connectionManager.Connection, NameHelper.GetCleanSettingsTable(importedDataInfo.TableName), "AI_Label", typeof(string), false);
                }

                if (!cleanSettings.Columns.Contains("AI_Include"))
                {
                    SqLiteHelper.AddTableColumn(connectionManager.Connection, NameHelper.GetCleanSettingsTable(importedDataInfo.TableName), "AI_Include", typeof(Int32), false);
                }

                if (!cleanSettings.Columns.Contains("AI_Ignore"))
                {
                    SqLiteHelper.AddTableColumn(connectionManager.Connection, NameHelper.GetCleanSettingsTable(importedDataInfo.TableName), "AI_Ignore", typeof(Int32), false);
                }

                var sql = SqLiteHelper.GetUpdateQuery(NameHelper.GetCleanSettingsTable(importedDataInfo.TableName), "AI_Type = '', AI_Label = '', AI_Include = 1, AI_Ignore = 0", "");
                SqLiteHelper.ExecuteNonQuery(sql, connectionManager.Connection);
            }
        }

        private void Up_RenameAAdressSplitZipColumn(IConnectionManager connectionManager, ProjectSettings projectSettings)
        {
            if (!IsUp(projectSettings.Version, "1.0.0.2"))
            {
                return;
            }

            foreach (var importedDataInfo in projectSettings.TableList)
            {
                var selectCleanSettingsQuery = SqLiteHelper.GetCleanSettingsQuery(importedDataInfo.TableName);
                var cleanSettings = SqLiteHelper.ExecuteQuery(selectCleanSettingsQuery, connectionManager.Connection, CommandBehavior.SchemaOnly, importedDataInfo.TableName);

                if (cleanSettings.Columns.Contains("SP_Zip"))
                {
                    SqLiteHelper.RenameColumn(connectionManager.Connection, NameHelper.GetCleanSettingsTable(importedDataInfo.TableName), "SP_Zip", "SP_Country");
                }
                if (!cleanSettings.Columns.Contains("SP_City"))
                {
                    SqLiteHelper.AddTableColumn(connectionManager.Connection, NameHelper.GetCleanSettingsTable(importedDataInfo.TableName), "SP_City", typeof(Int32), false);
                }
                if (!cleanSettings.Columns.Contains("SP_Region"))
                {
                    SqLiteHelper.AddTableColumn(connectionManager.Connection, NameHelper.GetCleanSettingsTable(importedDataInfo.TableName), "SP_Region", typeof(Int32), false);
                }
                if (!cleanSettings.Columns.Contains("SP_Postcode"))
                {
                    SqLiteHelper.AddTableColumn(connectionManager.Connection, NameHelper.GetCleanSettingsTable(importedDataInfo.TableName), "SP_Postcode", typeof(Int32), false);
                }
                var sql = SqLiteHelper.GetUpdateQuery(NameHelper.GetCleanSettingsTable(importedDataInfo.TableName), "SP_City = 0, SP_Region = 0, SP_Postcode = 0", "");
                SqLiteHelper.ExecuteNonQuery(sql, connectionManager.Connection);
            }
        }
    }
}