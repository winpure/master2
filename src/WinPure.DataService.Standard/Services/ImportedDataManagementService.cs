using System.Data;
using WinPure.Configuration.Helper;
using WinPure.DataService.Properties;

namespace WinPure.DataService.DependencyInjection;

internal static partial class WinPureDataServiceDependencyExtension
{
    private class ImportedDataManagementService : IImportedDataManagementService
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IWpLogger _logger;
        private readonly IConfigurationService _configurationService;

        public ImportedDataManagementService(IConnectionManager connectionManager, IWpLogger logger, IConfigurationService configurationService)
        {
            _connectionManager = connectionManager;
            _logger = logger;
            _configurationService = configurationService;
        }

        public DataTable GetPreview(ImportedDataInfo importedDataInfo)
        {
            var sql = SqLiteHelper.GetSelectQuery(importedDataInfo.TableName, "", "", true, _configurationService.Configuration.PreviewRowCount);
            var table = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection, CommandBehavior.Default, importedDataInfo.TableName);
            return table;
        }

        public void AddTableColumn(ImportedDataInfo importedDataInfo, string columnName, string columnType)
        {
            var dbColumnName = columnName.NormalizeColumnName();

            if (importedDataInfo.Fields.Any(x => x.DatabaseName == dbColumnName))
            {
                throw new WinPureIncorrectInputParameters(Resources.EXCEPTION_COLUMN_EXISTS_IN_TABLE);
            }

            var newCol = new DataColumn(dbColumnName, Type.GetType(columnType));
            SqLiteHelper.AddTableColumn(_connectionManager.Connection, importedDataInfo.TableName, newCol);
            var df = new DataField
            {
                DisplayName = columnName,
                DatabaseName = dbColumnName,
                FieldType = newCol.DataType.ToString(),
                Id = importedDataInfo.Fields.Any() ? importedDataInfo.Fields.Max(x => x.Id) + 1 : 1
            };

            importedDataInfo.Fields.Add(df);

            SqLiteHelper.ExecuteNonQuery($"INSERT INTO [{NameHelper.GetCleanSettingsTable(importedDataInfo.TableName)}] (COLUMN_NAME) VALUES ('{dbColumnName}');", _connectionManager.Connection);

            if (importedDataInfo.IsStatisticCalculated)
            {
                SqLiteHelper.ExecuteNonQuery($"INSERT INTO [{NameHelper.GetStatisticTable(importedDataInfo.TableName)}] (FieldName,FieldType,Empty) VALUES ('{dbColumnName}', '{columnName}', 100);", _connectionManager.Connection);
            }
        }

        public void RenameColumn(ImportedDataInfo importedDataInfo, List<DataField> databaseColumns)
        {

            if (!databaseColumns.Any())
            {
                return;
            }

            var tbl = SqLiteHelper.ExecuteQuery(SqLiteHelper.GetSelectQuery(importedDataInfo.TableName), _connectionManager.Connection, CommandBehavior.SchemaOnly);
            tbl.TableName = importedDataInfo.TableName + "_temp";
            string scrColumns = "";
            string dstColumns = "";

            ColumnHelper.RemoveWinPurePrimaryKeyFieldFromTable(tbl);

            foreach (DataColumn col in tbl.Columns)
            {
                scrColumns += $"[{col.ColumnName}],";

                var nCol = databaseColumns.FirstOrDefault(x => x.DatabaseName == col.ColumnName);
                if (nCol != null)
                {
                    var newColName = ColumnHelper.GetDataTableUniqueColumnName(tbl, nCol.DisplayName);

                    var sqlUpdateStatistic = SqLiteHelper.GetUpdateQuery(NameHelper.GetStatisticTable(importedDataInfo.TableName), $"[FieldName] = '{newColName}'", $"[FieldName]='{col.ColumnName}'");
                    SqLiteHelper.ExecuteNonQuery(sqlUpdateStatistic, _connectionManager.Connection);
                    var sqlUpdateSettings = SqLiteHelper.GetUpdateQuery(NameHelper.GetCleanSettingsTable(importedDataInfo.TableName), $"[COLUMN_NAME] = '{newColName}'", $"[COLUMN_NAME]='{col.ColumnName}'");
                    SqLiteHelper.ExecuteNonQuery(sqlUpdateSettings, _connectionManager.Connection);
                    var iField = importedDataInfo.Fields.FirstOrDefault(x => x.Id == nCol.Id);
                    if (iField != null)
                    {
                        iField.DatabaseName = newColName;
                        iField.DisplayName = newColName;
                    }
                    col.ColumnName = newColName;
                }
                dstColumns += $"[{col.ColumnName}],";
            }
            scrColumns = scrColumns.Substring(0, scrColumns.Length - 1);
            dstColumns = dstColumns.Substring(0, dstColumns.Length - 1);
            SqLiteHelper.CreateTableSchema(_connectionManager.Connection, tbl);
            var sql = $"INSERT INTO [{tbl.TableName}] ({dstColumns}) SELECT {scrColumns} FROM [{importedDataInfo.TableName}]";
            SqLiteHelper.ExecuteNonQuery(sql, _connectionManager.Connection);
            SqLiteHelper.ExecuteNonQuery(SqLiteHelper.GetDropTableQuery(importedDataInfo.TableName), _connectionManager.Connection);
            SqLiteHelper.ChangeTableName(_connectionManager.Connection, tbl.TableName, importedDataInfo.TableName);
        }

        public void CopyColumn(ImportedDataInfo importedDataInfo, List<DataField> databaseColumns)
        {
            if (!databaseColumns.Any())
            {
                return;
            }

            var table = SqLiteHelper.ExecuteQuery(SqLiteHelper.GetSelectQuery(importedDataInfo.TableName), _connectionManager.Connection, CommandBehavior.SchemaOnly, importedDataInfo.TableName);
            var tableStatistic = SqLiteHelper.ExecuteQuery(SqLiteHelper.GetSelectQuery(NameHelper.GetStatisticTable(importedDataInfo.TableName)), _connectionManager.Connection, CommandBehavior.SchemaOnly, importedDataInfo.TableName);
            var statisticColumns = tableStatistic.Columns.Cast<DataColumn>().Where(x => x.ColumnName != WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY).Aggregate("", (current, col) => current + $"[{col.ColumnName}],");
            statisticColumns = statisticColumns.Substring(0, statisticColumns.Length - 1);

            var tableSettings = SqLiteHelper.ExecuteQuery(SqLiteHelper.GetSelectQuery(NameHelper.GetCleanSettingsTable(importedDataInfo.TableName)), _connectionManager.Connection, CommandBehavior.SchemaOnly, importedDataInfo.TableName);
            var settingColumns = tableSettings.Columns.Cast<DataColumn>().Where(x => x.ColumnName != WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY).Aggregate("", (current, col) => current + $"[{col.ColumnName}],");
            settingColumns = settingColumns.Substring(0, settingColumns.Length - 1);

            var sqlCopy = $"update [{importedDataInfo.TableName}] SET ";
            foreach (var fld in databaseColumns)
            {
                var colNewName = ColumnHelper.GetDataTableUniqueColumnName(table, fld.DatabaseName + "_copy");
                var newCol = new DataColumn(colNewName, table.Columns[fld.DatabaseName].DataType);
                SqLiteHelper.AddTableColumn(_connectionManager.Connection, importedDataInfo.TableName, newCol);
                sqlCopy += $"[{colNewName}] = [{fld.DatabaseName}],";
                var sqlUpdateStatistic = $"INSERT INTO [{NameHelper.GetStatisticTable(importedDataInfo.TableName)}] SELECT {statisticColumns.Replace("[FieldName],", $"'{colNewName}',")} FROM [{NameHelper.GetStatisticTable(importedDataInfo.TableName)}] WHERE [FieldName]='{fld.DatabaseName}'";
                SqLiteHelper.ExecuteNonQuery(sqlUpdateStatistic, _connectionManager.Connection);
                var sqlUpdateSettings = $"INSERT INTO [{NameHelper.GetCleanSettingsTable(importedDataInfo.TableName)}] SELECT {settingColumns.Replace("[COLUMN_NAME],", $"'{colNewName}',")} FROM [{NameHelper.GetCleanSettingsTable(importedDataInfo.TableName)}] WHERE [COLUMN_NAME]='{fld.DatabaseName}'";
                SqLiteHelper.ExecuteNonQuery(sqlUpdateSettings, _connectionManager.Connection);
                importedDataInfo.Fields.Add(new DataField()
                {
                    DatabaseName = colNewName,
                    DisplayName = colNewName,
                    FieldType = table.Columns[fld.DatabaseName].DataType.ToString(),
                    Id = importedDataInfo.Fields.Any() ? importedDataInfo.Fields.Max(x => x.Id) + 1 : 1
                });
            }
            sqlCopy = sqlCopy.Substring(0, sqlCopy.Length - 1);
            SqLiteHelper.ExecuteNonQuery(sqlCopy, _connectionManager.Connection);
        }

        public void RemoveColumn(ImportedDataInfo importedDataInfo, List<DataField> databaseColumns)
        {
            if (!databaseColumns.Any())
            {
                return;
            }

            SqLiteHelper.RemoveColumn(_connectionManager.Connection, importedDataInfo.TableName, databaseColumns);
            var ids = databaseColumns.Select(x => x.Id).ToList();
            foreach (var id in ids)
            {
                importedDataInfo.Fields.Remove(importedDataInfo.Fields.First(x => id == x.Id));
            }


            var removedFields = "'" + string.Join("','", databaseColumns.Select(x => x.DatabaseName)) + "'";

            SqLiteHelper.ExecuteNonQuery(SqLiteHelper.GetDeleteQuery(NameHelper.GetCleanSettingsTable(importedDataInfo.TableName), $"[COLUMN_NAME] IN ({removedFields})"), _connectionManager.Connection);

            if (importedDataInfo.IsStatisticCalculated)
            {
                SqLiteHelper.ExecuteNonQuery(SqLiteHelper.GetDeleteQuery(NameHelper.GetStatisticTable(importedDataInfo.TableName), $"[FieldName] IN ({removedFields})"), _connectionManager.Connection);
            }
        }
    }
}