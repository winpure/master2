using System.Data.Common;
using System.Data.OleDb;
using System.Text;

namespace WinPure.Integration.Export;

internal class AccessExportProvider : DatabaseExportProviderBase
{
    private OleDbConnection _connection;

    public AccessExportProvider() : base(ExternalSourceTypes.Access)
    {
        DisplayName = "Access";
    }

    protected override DbConnection DatabaseConnection => _connection;

    protected override void CreateConnection(string connectionString)
    {
        _connection = CreateConnection(Options.DatabaseFile, Options.Password);
    }

    protected override void ExportData(DataTable data)
    {
        if (CheckConnect())
        {
            var sqlCreateTbl = new StringBuilder($"CREATE TABLE [{Options.TableName}] (");

            if (TableExists)
            {
                var sqlDeleteTbl = $"DROP TABLE [{Options.TableName}]";
                var cmd_delete = new OleDbCommand(sqlDeleteTbl, _connection);
                cmd_delete.ExecuteNonQuery();
            }

            bool firstTime = true;
            var colNames = "";
            var insertParam = "";

            foreach (DataColumn column in data.Columns)
            {
                if (firstTime) firstTime = false;
                else
                {
                    sqlCreateTbl.Append("   ,");
                    colNames += ",";
                    insertParam += ",";
                }

                var colType = GetSQLTypeAsString(column.DataType);

                if (colType == "TEXT(255)")
                {
                    var qry = (from myRow in data.AsEnumerable()
                            where !string.IsNullOrEmpty(myRow.Field<string>(column))
                            select myRow.Field<string>(column)
                        );
                    if (qry.Any())
                    {
                        if (qry.Max(x => x.Length) > 255)
                        {
                            colType = "MEMO";
                        }
                    }
                }

                sqlCreateTbl.AppendFormat("[{0}] {1} {2} {3}",
                    column.ColumnName, // 0
                    colType, // 1
                    column.AllowDBNull ? "" : "NOT NULL", // 2
                    Environment.NewLine // 3
                );
                colNames += "[" + column.ColumnName + "]";
                insertParam += "?";
            }

            sqlCreateTbl.Append(")");
            var cmd3 = new OleDbCommand($"INSERT INTO [{Options.TableName}] ({colNames}) VALUES ({insertParam})",
                _connection);
            var cmd2 = new OleDbCommand(sqlCreateTbl.ToString(), _connection);
            cmd2.ExecuteNonQuery();
            var i = 1;
            var step = Math.Max(data.Rows.Count / 100, 1);
            foreach (DataRow rw in data.Rows)
            {
                cmd3.Parameters.Clear();
                foreach (DataColumn col in data.Columns)
                {
                    cmd3.Parameters.AddWithValue("@" + col.ColumnName.Replace(" ", "").ToLower(),
                        string.IsNullOrEmpty(rw[col.ColumnName].ToString()) ? DBNull.Value : rw[col.ColumnName]);
                }

                cmd3.ExecuteNonQuery();
                if (i % step == 0)
                {
                    NotifyProgress(Resources.CAPTION_IO_DATA_EXPORTING, i / step);
                }

                i++;
            }
        }
    }

    protected override List<string> GetDatabases()
    {
        return new List<string>();
    }

    /// <summary>
    /// Returns the SQL data type equivalent, as a string for use in SQL script generation methods.
    /// </summary>
    private string GetSQLTypeAsString(Type dataType)
    {
        switch (dataType.Name)
        {
            case "Boolean": return "Bit";
            case "Char": return "TEXT(1)";
            case "SByte": return "Integer";
            case "Int16": return "Integer";
            case "Int32": return "Integer";
            case "Int64": return "Long";
            case "Byte": return "Byte";
            case "UInt16": return "Integer";
            case "UInt32": return "Integer";
            case "UInt64": return "Long";
            case "Single": return "float";
            case "Double": return "Double";
            case "Decimal": return "Double";
            case "DateTime": return "DateTime";
            case "Guid": return "TEXT(255)";
            case "Object": return "MEMO";
            case "String": return "TEXT(255)";
            default: return "TEXT(255)";
        }
    }

    public override bool CheckConnect()
    {
        try
        {
            if (_connection == null)
            {
                return false;
            }

            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            return _connection.State == ConnectionState.Open;
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("The 'Microsoft.ACE.OleDb.12.0' provider is not registered"))
            {
                NotifyException(Resources.EXCEPTION_ACEOLEDB_NOTREGISTERED, null);
            }
            else
            {
                NotifyException(Resources.EXCEPTION_IO_CANNOT_CONNECT_TO_FILE, ex);
            }
        }
        return false;
    }

    protected override List<string> GetTables()
    {
        var tables = new List<string>();

        // We only want user tables, not system tables
        string[] restrictions = new string[4];
        restrictions[3] = "Table";

        var userTables = _connection.GetSchema("Tables", restrictions);

        // Add list of table names to listBox
        for (int i = 0; i < userTables.Rows.Count; i++)
        {
            tables.Add(userTables.Rows[i][2].ToString());
        }

        return tables;
    }

    private OleDbConnection CreateConnection(string databaseName, string password)
    {
        if (string.IsNullOrEmpty(databaseName))
        {
            NotifyException(Resources.EXCEPTION_IO_INVALID_DB_NAME, null);
            return null;
        }

        string msAccess2007ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + databaseName + ";OLE DB Services=-1" +
                                              (string.IsNullOrEmpty(password) ? ";Persist Security Info=False;" : ";Jet OLEDB:Database Password=" + password);

        var connection = new OleDbConnection { ConnectionString = msAccess2007ConnectionString };
        return connection;
    }
}