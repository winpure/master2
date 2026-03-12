using System.Data;
using System.Data.SQLite;
using WinPure.Common.Exceptions;
using WinPure.Common.Logger;
using WinPure.Common.Models;
using WinPure.Configuration.Properties;

namespace WinPure.Configuration.Helper;

internal static class SqLiteHelper
{
    /// <summary>
    /// Gets the column type for database.
    /// </summary>
    /// <param name="dc">The data column.</param>
    /// <returns>System.String.</returns>
    public static string GetSqLiteDataType(DataColumn dc)
    {
        return GetSqLiteDataType(dc.DataType);
    }

    public static string GetSqLiteDataType(string type)
    {
        return GetSqLiteDataType(Type.GetType(type)).ToLower().Trim();
    }

    public static string GetSqLiteDataType(Type tp)
    {
        if (tp == typeof(string))
        {
            return " text ";
        }

        if (tp == typeof(Int16)
            || tp == typeof(Int32)
            || tp == typeof(Int64)
            || tp == typeof(int)
            || tp == typeof(long)
            || tp == typeof(short)
            || tp == typeof(byte))
        {
            return " INTEGER ";
        }

        if (tp == typeof(DateTime))
        {
            return " datetime ";
        }

        if (tp == typeof(double)
            || tp == typeof(decimal)
            || tp == typeof(float))
        {
            return " REAL ";
        }

        if (tp == typeof(bool))
        {
            return " INTEGER ";
        }

        return " text ";
    }

    public static void ExecuteNonQuery(string qry, SQLiteConnection conn)
    {
        using (var cmd = new SQLiteCommand(qry, conn))
        {
            cmd.CommandTimeout = 1000;
            cmd.ExecuteNonQuery();
        }
    }

    public static T ExecuteScalar<T>(string query, SQLiteConnection connection)
    {
        using (var cmd = new SQLiteCommand(query, connection))
        {
            var result = cmd.ExecuteScalar();

            return result == null || result == DBNull.Value ? default(T) : (T)Convert.ChangeType(result, typeof(T));
        }
    }

    public static bool CheckTableExists(string tableName, SQLiteConnection connection)
    {
        string sql = $"SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = '{tableName}'";
        using (var cmd = new SQLiteCommand(sql, connection))
        {
            var res = cmd.ExecuteScalar();

            return Convert.ToInt32(res) > 0;
        }
    }

    public static DataTable ExecuteQuery(string query, SQLiteConnection connection, CommandBehavior behavior = CommandBehavior.Default, string tableName = "", DataTable table = null)
    {
        var res = table ?? new DataTable(tableName);
        using (var cmd = new SQLiteCommand(query, connection))
        {
            using (var reader = cmd.ExecuteReader(behavior))
            {
                res.Load(reader);
            }
        }

        return res;
    }

    public static string GetUpdateQuery(string tableName, string updateFields, string whereCondition)
    {
        var res = $"UPDATE [{tableName}] SET {updateFields}";

        if (!string.IsNullOrEmpty(whereCondition))
        {
            res += $" WHERE {whereCondition}";
        }

        return res;
    }

    public static string GetCleanSettingsQuery(string tableName)
    {
        return
            $"select * ,  CASE WHEN EXISTS(SELECT * FROM [{NameHelper.GetWordManagerTable(tableName)}] w WHERE w.COLUMN_NAME = s.COLUMN_NAME) THEN 'WM' ELSE '' END AS WordManager" +
            $" from [{NameHelper.GetCleanSettingsTable(tableName)}] s";
    }

    public static string GetCountQuery(string tableName)
    {
        return $"SELECT COUNT(*) FROM [{tableName}]";
    }

    public static string GetSelectQuery(string tableName, string selectFields = "", string whereCondition = "", bool orderBy = false, int rowCount = 0)
    {
        var sql = string.IsNullOrEmpty(selectFields)
            ? $"SELECT * FROM [{tableName}] "
            : $"SELECT {selectFields} FROM [{tableName}] ";

        if (!string.IsNullOrEmpty(whereCondition))
        {
            sql += $" WHERE {whereCondition}";
        }

        if (orderBy)
        {
            sql += " order by WinPurePrimK";
        }

        if (rowCount > 0)
        {
            sql += $" LIMIT {rowCount}";
        }

        return sql;
    }

    public static string GetDeleteQuery(string tableName, string whereCondition = "")
    {
        var sql = $"DELETE FROM [{tableName}] ";

        if (!string.IsNullOrEmpty(whereCondition))
        {
            sql += $" WHERE {whereCondition}";
        }

        return sql;
    }

    public static string GetDropTableQuery(string tableName)
    {
        var sql = $"DROP TABLE [{tableName}] ";

        return sql;
    }

    public static void ChangeTableName(SQLiteConnection conn, string oldName, string newName)
    {
        var sql = $"ALTER TABLE [{oldName}] RENAME TO [{newName}]";
        ExecuteNonQuery(sql, conn);
    }

    public static void AddTableColumn(SQLiteConnection conn, string tableName, DataColumn newColumn, bool isPrimaryKey = false)
    {
        AddTableColumn(conn, tableName, newColumn.ColumnName, newColumn.DataType, isPrimaryKey);
    }

    public static void AddTableColumn(SQLiteConnection conn, string tableName, string fieldName, Type fieldType, bool isPrimaryKey = false)
    {
        if (isPrimaryKey)
        {
            var dt = ExecuteQuery(GetSelectQuery(tableName), conn, CommandBehavior.SchemaOnly);
            ChangeTableName(conn, tableName, tableName + "_old");
            var fld = CreateTableSchema(conn, dt);
            var sql = $"insert into [{tableName}]({fld}) SELECT {fld} from [{tableName}_old]";
            ExecuteNonQuery(sql, conn);
            ExecuteNonQuery(GetDropTableQuery($"{tableName}_old"), conn);
        }
        else
        {
            var sql = $"ALTER TABLE [{tableName}] ADD COLUMN [{fieldName}] {GetSqLiteDataType(fieldType)}";
            ExecuteNonQuery(sql, conn);
        }

    }

    public static void RenameColumn(SQLiteConnection conn, string tableName, string oldColumnName, string newColumnName)
    {
        var sql = $"ALTER TABLE {tableName} RENAME COLUMN {oldColumnName} TO {newColumnName};";
        ExecuteNonQuery(sql, conn);
    }

    public static bool ClearTable(SQLiteConnection conn, string tableName)
    {
        if (CheckTableExists(tableName, conn))
        {
            ExecuteNonQuery(GetDeleteQuery(tableName), conn);
            return true;
        }
        return false;
    }

    public static void RemoveTableIfExists(SQLiteConnection conn, string tableName)
    {
        CheckConnectionState(conn);
        if (CheckTableExists(tableName, conn))
        {
            ExecuteNonQuery(GetDropTableQuery(tableName), conn);
        }
    }

    /// <summary>
    /// Creates the table schema in the database.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="data">The datatable with required columns.</param>
    /// <param name="addPrimaryKey"></param>
    /// <param name="logger"></param>
    /// <returns>return comma delimited fields list</returns>
    public static string CreateTableSchema(SQLiteConnection connection, DataTable data, bool addPrimaryKey = true, IWpLogger logger = null)
    {
        string columnStructure = "";
        string columnList = "";
        try
        {
            CheckConnectionState(connection);

            if (CheckTableExists(data.TableName, connection))
            {
                ExecuteNonQuery(GetDropTableQuery(data.TableName), connection);
            }

            foreach (DataColumn dc in data.Columns)
            {
                if (dc.ColumnName != WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY)
                {
                    var value = dc.ColumnName.NormalizeColumnName();
                    value = value.Trim();
                    var isColumnNull = dc.AllowDBNull == false ? "not null " : "null ";
                    var columnType = GetSqLiteDataType(dc);

                    columnStructure += $"[{value}] {columnType} {isColumnNull}, ";
                    columnList += columnList == "" ? $"[{value}]" : $",[{value}]";
                }
            }
            if (addPrimaryKey)
            {
                columnStructure += $"[{WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY}] INTEGER PRIMARY KEY AUTOINCREMENT";
            }
            else
            {
                columnStructure = columnStructure.Substring(0, columnStructure.Length - 2);
            }

            ExecuteNonQuery($" create table [{data.TableName}] ({columnStructure})", connection);
            return columnList;
        }

        catch (Exception ex)
        {
            if (logger != null)
            {
                logger.Debug("Could not create table in local database", ex);
            }
            throw new WinPureImportException(Resources.EXCEPTION_COULD_NOT_CREATE_TABLE);
        }
    }

    public static string SaveDataToDb(SQLiteConnection conn, DataTable table, string tableName, IWpLogger logger = null, bool addPrimaryKey = true)
    {
        //table.AcceptChanges();
        table.TableName = tableName;
        var filedList = CreateTableSchema(conn, table, addPrimaryKey, logger);

        if (!string.IsNullOrEmpty(filedList))
        {
            using (var transaction = conn.BeginTransaction())
            {
                using (var cmd = new SQLiteCommand("", conn))
                {
                    cmd.Transaction = transaction;

                    string val = "";
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        if (table.Columns[i].ColumnName != WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY)
                        {
                            val += ((string.IsNullOrWhiteSpace(val)) ? "@p" : ",@p") + i;
                        }
                    }
                    cmd.CommandText = $"INSERT INTO [{table.TableName}] ({filedList}) VALUES ({val});";

                    for (var i = 0; i < table.Rows.Count; i++)
                    {
                        if (table.Rows[i].RowState == DataRowState.Deleted) continue;

                        for (int j = 0; j < table.Columns.Count; j++)
                        {
                            if (table.Columns[j].ColumnName != WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY)
                            {
                                cmd.Parameters.AddWithValue("@p" + j, table.Rows[i][j]);
                            }
                        }
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    transaction.Commit();
                }
            }

        }
        return filedList;
    }

    public static string AppendDataToDb(SQLiteConnection conn, DataTable table, string tableName, string filedList, string insertParameters)
    {
        //table.AcceptChanges();
        table.TableName = tableName;
        string val = insertParameters;
        if (!string.IsNullOrEmpty(filedList))
        {
            using (var transaction = conn.BeginTransaction())
            {
                using (var cmd = new SQLiteCommand("", conn))
                {
                    cmd.Transaction = transaction;

                    if (string.IsNullOrWhiteSpace(val))
                    {
                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            if (table.Columns[i].ColumnName != WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY)
                            {
                                val += ((string.IsNullOrWhiteSpace(val)) ? "@p" : ",@p") + i;
                            }
                        }
                    }

                    cmd.CommandText = $"INSERT INTO [{table.TableName}] ({filedList}) VALUES ({val});";

                    for (var i = 0; i < table.Rows.Count; i++)
                    {
                        for (int j = 0; j < table.Columns.Count; j++)
                        {
                            if (table.Columns[j].ColumnName != WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY)
                            {
                                cmd.Parameters.AddWithValue("@p" + j, table.Rows[i][j]);
                            }
                        }
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    transaction.Commit();
                }
            }

        }
        return val;
    }

    public static void RemoveColumn(SQLiteConnection conn, string tableName, List<DataField> columnNames)
    {
        if (!columnNames.Any()) return;
        CheckConnectionState(conn);

        if (CheckTableExists(tableName, conn))
        {
            var tbl = ExecuteQuery($"select * from [{tableName}]", conn, CommandBehavior.SchemaOnly);
            tbl.TableName = tableName + "_temp";

            var deletedColumns = columnNames.Select(x => x.DatabaseName).ToList();
            int i = 0;

            ColumnHelper.RemoveWinPurePrimaryKeyFieldFromTable(tbl);

            while (i < tbl.Columns.Count)
            {
                if (deletedColumns.Contains(tbl.Columns[i].ColumnName))
                {
                    tbl.Columns.Remove(tbl.Columns[i]);
                }
                else
                {
                    i++;
                }
            }
            var columns = CreateTableSchema(conn, tbl);
            var sql = $"INSERT INTO [{tbl.TableName}] ({columns}) SELECT {columns} FROM [{tableName}]";
            ExecuteNonQuery(sql, conn);
            ExecuteNonQuery(GetDropTableQuery(tableName), conn);
            ChangeTableName(conn, tbl.TableName, tableName);

        }
    }

    private static void CheckConnectionState(SQLiteConnection connection)
    {
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }
    }

    public static void UpdateTableColumnList(SQLiteConnection conn, ImportedDataInfo importedDataInfo, DataTable table, bool syncSupportTables = false)
    {
        importedDataInfo.Fields.RemoveAll(x => !table.Columns.Contains(x.DatabaseName));

        var colNextId = importedDataInfo.Fields.Any() ? importedDataInfo.Fields.Max(x => x.Id) + 1 : 1;
        foreach (DataColumn col in table.Columns)
        {
            if (col.ColumnName == WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY) continue;
            if (importedDataInfo.Fields.All(x => x.DatabaseName != col.ColumnName))
            {
                importedDataInfo.Fields.Add(new DataField
                {
                    DisplayName = col.ColumnName,
                    DatabaseName = col.ColumnName,
                    FieldType = col.DataType.ToString(),
                    Id = colNextId++
                });
            }
        }

        if (syncSupportTables)
        {
            var dt = ExecuteQuery(SqLiteHelper.GetSelectQuery(NameHelper.GetCleanSettingsTable(importedDataInfo.TableName)), conn);
            foreach (DataColumn col in table.Columns)
            {
                if (col.ColumnName == WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY) continue;

                if (dt.AsEnumerable().All(row => col.ColumnName != row.Field<string>("COLUMN_NAME")))
                {
                    ExecuteNonQuery($"INSERT INTO [{NameHelper.GetCleanSettingsTable(importedDataInfo.TableName)}] (COLUMN_NAME) VALUES ('{col.ColumnName}');", conn);
                }
            }

            if (importedDataInfo.IsStatisticCalculated)
            {
                dt = ExecuteQuery(GetSelectQuery(NameHelper.GetStatisticTable(importedDataInfo.TableName)), conn);
                foreach (DataColumn col in table.Columns)
                {
                    if (col.ColumnName == WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY) continue;

                    if (dt.AsEnumerable().All(row => col.ColumnName != row.Field<string>("FieldName")))
                    {
                        ExecuteNonQuery($"INSERT INTO [{NameHelper.GetStatisticTable(table.TableName)}] (FieldName) VALUES ('{col.ColumnName}');", conn);
                    }
                }
            }
        }
    }

    public static DataTable ConvertGuidColumnsToString(DataTable original)
    {
        // Return the original table if there are no Guid columns
        if (!original.Columns.Cast<DataColumn>().Any(c => c.DataType == typeof(Guid)))
            return original;

        var newTable = new DataTable(original.TableName);

        // Clone the schema, replacing Guid columns with string
        foreach (DataColumn col in original.Columns)
        {
            var newColType = col.DataType == typeof(Guid) ? typeof(string) : col.DataType;
            newTable.Columns.Add(col.ColumnName, newColType);
        }

        // Copy rows, converting Guid values to string
        foreach (DataRow row in original.Rows)
        {
            var newRow = newTable.NewRow();
            foreach (DataColumn col in original.Columns)
            {
                var value = row[col];
                if (col.DataType == typeof(Guid) && value is Guid guid)
                    newRow[col.ColumnName] = guid.ToString();
                else
                    newRow[col.ColumnName] = value;
            }
            newTable.Rows.Add(newRow);
        }

        return newTable;
    }
}