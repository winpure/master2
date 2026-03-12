using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Text;

namespace WinPure.Integration.Export;

//TODO
internal class MySqlExportProvider : DatabaseExportProviderBase
{
    private SqlImportExportOptions _options;

    public MySqlExportProvider() : base(ExternalSourceTypes.MySqlServer)
    {
        DisplayName = "MySql";
    }

    private void Export(MySqlConnection connection, DataTable data)
    {
        try
        {
            connection.Open();
            if (TableExists)
            {
                var queryDelete = $"DROP TABLE {_options.TableName}";
                using (var cmd = new MySqlCommand(queryDelete, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            var query = BuildCreateTableScript(data);
            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.ExecuteNonQuery();
            }

            data.AsEnumerable().Select((x, i) => new { row = x, i })
                .GroupBy(x => x.i / 1000).Select(x => x.Select(y => y.row).ToList())
                .Select(x => BulkInsert(x, _options.TableName)).ToList()
                .ForEach(x =>
                {
                    using (var cmd = new MySqlCommand(x, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                });

        }
        finally
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }

    protected override DbConnection DatabaseConnection => null;


    protected override void CreateConnection(string connectionString)
    {

    }

    protected override void ExportData(DataTable data)
    {
        if (CheckConnect())
        {
            _options.TableName =
                string.Join(".", _options.TableName.Split('.').Select(x => "`" + x + "`").ToArray());
            data.TableName = _options.TableName;

            if (_options.UseSsh)
            {
                using (var tunnel = new SshTunnel(_options.SshServer, _options.SshLogin, _options.SshPassword,
                           _options.ServerAddress, (uint)_options.Port))
                {
                    using (var connection = new MySqlConnection(CreateConnectionString(tunnel.BoundHost,
                               _options.IntegrateSecurity, _options.UserName, _options.Password,
                               _options.DatabaseName, tunnel.BoundPort.ToString(), _options.UseSsl)))
                    {
                        Export(connection, data);
                    }
                }
            }
            else
            {
                using (var connection = new MySqlConnection(CreateConnectionString(_options.ServerAddress,
                           _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName,
                           _options.Port.ToString(), _options.UseSsl)))
                {
                    Export(connection, data);
                }
            }
        }
    }

    private bool CheckConnect(string server, bool integrateSecurity, string userName, string userPassword, string database, string port, bool useSsl)
    {
        using (var connection = new MySqlConnection(CreateConnectionString(server, integrateSecurity, userName, userPassword, database, port, useSsl)))
        {
            connection.Open();
            var res = connection.State == ConnectionState.Open;
            connection.Close();
            return res;
        }
    }

    public override bool CheckConnect()
    {
        try
        {
            if (_options.UseSsh)
            {
                using (var tunnel = new SshTunnel(_options.SshServer, _options.SshLogin, _options.SshPassword, _options.ServerAddress, (uint)_options.Port))
                {
                    return CheckConnect(tunnel.BoundHost, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, tunnel.BoundPort.ToString(), _options.UseSsl);
                }
            }

            return CheckConnect(_options.ServerAddress, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, _options.Port.ToString(), _options.UseSsl);

        }
        catch (Exception ex)
        {
            NotifyException(Resources.EXCEPTION_IO_CANNOT_CONNECT_TO_SERVER, ex);
            return false;
        }
    }

    private List<string> GetDatabaseList(string server, bool integrateSecurity, string userName, string userPassword, string database, string port, bool useSsl)
    {
        using (var connection = new MySqlConnection(CreateConnectionString(server, integrateSecurity, userName, userPassword, database, port, useSsl)))
        {
            connection.Open();
            var databases = connection.GetSchema("Databases");
            connection.Close();
            return databases.Select().Select(x => x["database_name"].ToString()).ToList();
        }
    }

    protected override List<string> GetDatabases()
    {
        throw new NotImplementedException();
    }

    public override List<string> GetDatabaseList()
    {
        try
        {
            if (_options.UseSsh)
            {
                using (var tunnel = new SshTunnel(_options.SshServer, _options.SshLogin, _options.SshPassword, _options.ServerAddress, (uint)_options.Port))
                {
                    return GetDatabaseList(tunnel.BoundHost, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, tunnel.BoundPort.ToString(), _options.UseSsl);
                }
            }

            return GetDatabaseList(_options.ServerAddress, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, _options.Port.ToString(), _options.UseSsl);
        }
        catch (Exception ex)
        {
            NotifyException(Resources.EXCEPTION_IO_CANNOT_GET_DATABASE_LIST, ex);
            return null;
        }
    }

    protected override List<string> GetTables()
    {
        throw new NotImplementedException();
    }

    private List<string> GetDatabaseTables(string server, bool integrateSecurity, string userName, string userPassword, string database, string port, bool useSsl)
    {
        using (var connection = new MySqlConnection(CreateConnectionString(server, integrateSecurity, userName, userPassword, database, port, useSsl)))
        {
            connection.Open();
            var tables = connection.GetSchema("Tables");
            connection.Close();
            return tables.Select()
                .Select(x => x["TABLE_SCHEMA"].ToString() + "." + x["TABLE_NAME"].ToString()).OrderBy(x => x)
                .ToList();
        }
    }

    public override List<string> GetDatabaseTables()
    {
        try
        {
            if (_options.UseSsh)
            {
                using (var tunnel = new SshTunnel(_options.SshServer, _options.SshLogin, _options.SshPassword, _options.ServerAddress, (uint)_options.Port))
                {
                    return GetDatabaseTables(tunnel.BoundHost, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, tunnel.BoundPort.ToString(), _options.UseSsl);
                }
            }

            return GetDatabaseTables(_options.ServerAddress, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, _options.Port.ToString(), _options.UseSsl);
        }
        catch (Exception ex)
        {
            NotifyException(Resources.EXCEPTION_IO_CANNOT_GET_TABLES_LIST, ex);
            return null;
        }
    }

    string CreateConnectionString(string server, bool integrateSecurity, string userName, string userPassword, string database, string port, bool useSsl)
    {
        var res = $"Server={server};";
        if (!string.IsNullOrEmpty(port))
        {
            res += $"Port={port};";
        }

        if (!integrateSecurity)
        {
            res += $"UID={userName}; password={userPassword};";
        }
        else
        {
            res += "UID = auth_windows; IntegratedSecurity = yes;";
        }
        if (!string.IsNullOrEmpty(database))
        {
            res += $"database={database};";
        }

        var sslMode = useSsl ? "Required" : "None";
        res += $"SslMode={sslMode};Connect Timeout=30;Allow User Variables=True";
        return res;
    }


    /// <summary>
    /// Creates a SQL script that creates a table where the columns matches that of the specified DataTable.
    /// </summary>
    private static string BuildCreateTableScript(DataTable table)
    {
        StringBuilder result = new StringBuilder();
        result.AppendFormat("CREATE TABLE {1} ({0}   ", Environment.NewLine, table.TableName);

        bool firstTime = true;
        foreach (DataColumn column in table.Columns.OfType<DataColumn>())
        {
            if (firstTime) firstTime = false;
            else
                result.Append("   ,");

            result.AppendFormat("`{0}` {1} {2} {3}",
                column.ColumnName, // 0
                GetSQLTypeAsString(column.DataType), // 1
                column.AllowDBNull ? "NULL" : "NOT NULL", // 2
                Environment.NewLine // 3
            );
        }
        result.AppendFormat(") {0}", Environment.NewLine);

        // Build an ALTER TABLE script that adds keys to a table that already exists.
        //if (table.PrimaryKey.Length > 0)
        //    result.Append(BuildKeysScript(table));

        return result.ToString();
    }

    /// <summary>
    /// Returns the SQL data type equivalent, as a string for use in SQL script generation methods.
    /// </summary>
    private static string GetSQLTypeAsString(Type dataType)
    {
        switch (dataType.Name)
        {
            case "Boolean": return "bit";
            case "Char": return "char";
            case "SByte": return "tinyint";
            case "Int16": return "smallint";
            case "Int32": return "int";
            case "Int64": return "bigint";
            case "Byte": return "tinyint UNSIGNED";
            case "UInt16": return "smallint UNSIGNED";
            case "UInt32": return "int UNSIGNED";
            case "UInt64": return "bigint UNSIGNED";
            case "Single": return "float";
            case "Double": return "double";
            case "Decimal": return "decimal";
            case "DateTime": return "datetime";
            case "Guid": return "uniqueidentifier";
            case "Object": return "variant";
            case "String": return "TEXT";
            default: return "TEXT";
        }
    }

    string BulkInsert(List<DataRow> rows, string tableName)
    {
        var queryBuilder = new StringBuilder();

        queryBuilder.AppendFormat("INSERT INTO {0} (", tableName);

        // more than 1 column required and 1 or more rows
        if (rows.Count > 0 && rows[0].Table.Columns.Count > 0)
        {
            var columns = rows[0].Table.Columns;
            // build all columns
            queryBuilder.AppendFormat("`{0}`", columns[0].ColumnName);

            if (columns.Count > 1)
            {
                for (int i = 1; i < columns.Count; i++)
                {
                    queryBuilder.AppendFormat(", `{0}` ", columns[i].ColumnName);
                }
            }

            queryBuilder.AppendFormat(") VALUES (");

            // build all values for the first row
            // escape String & Datetime values!
            DateTime dt;
            if (rows[0][columns[0].ColumnName] == DBNull.Value)
            {
                queryBuilder.AppendFormat("null");
            }
            else if (columns[0].DataType == typeof(String))
            {
                queryBuilder.AppendFormat("'{0}'", MySqlHelper.EscapeString(rows[0][columns[0].ColumnName].ToString()));
            }
            else if (columns[0].DataType == typeof(DateTime))
            {
                dt = (DateTime)rows[0][columns[0].ColumnName];
                queryBuilder.AppendFormat("'{0:yyyy-MM-dd HH:mm:ss}'", dt);
            }
            else if (columns[0].DataType == typeof(Int32))
            {
                queryBuilder.AppendFormat("{0}", rows[0].Field<Int32?>(columns[0].ColumnName) ?? 0);
            }
            else if (columns[0].DataType == typeof(double) || columns[0].DataType == typeof(decimal) || columns[0].DataType == typeof(float))
            {
                queryBuilder.AppendFormat("{0}", rows[0][columns[0].ColumnName].ToString().Replace(",", "."));
            }
            else
            {
                queryBuilder.AppendFormat("{0}", rows[0][columns[0].ColumnName]);
            }

            for (int i = 1; i < columns.Count; i++)
            {
                // escape String & Datetime values!
                if (rows[0][columns[i].ColumnName] == DBNull.Value)
                {
                    queryBuilder.AppendFormat(", null");
                }
                else if (columns[i].DataType == typeof(String))
                {
                    queryBuilder.AppendFormat(", '{0}'", MySqlHelper.EscapeString(rows[0][columns[i].ColumnName].ToString()));
                }
                else if (columns[i].DataType == typeof(DateTime))
                {
                    dt = (DateTime)rows[0][columns[i].ColumnName];
                    queryBuilder.AppendFormat(", '{0:yyyy-MM-dd HH:mm:ss}'", dt);

                }
                else if (columns[i].DataType == typeof(Int32))
                {
                    queryBuilder.AppendFormat(", {0}", rows[0].Field<Int32?>(columns[i].ColumnName) ?? 0);
                }
                else if (columns[i].DataType == typeof(double) || columns[i].DataType == typeof(decimal) || columns[i].DataType == typeof(float))
                {
                    queryBuilder.AppendFormat(", {0}", rows[0][columns[i].ColumnName].ToString().Replace(",", "."));
                }
                else
                {
                    queryBuilder.AppendFormat(", {0}", rows[0][columns[i].ColumnName]);
                }
            }

            queryBuilder.Append(")");
            queryBuilder.AppendLine();

            // build all values all remaining rows
            if (rows.Count > 1)
            {
                // iterate over the rows
                for (int row = 1; row < rows.Count; row++)
                {
                    // open value block
                    queryBuilder.Append(", (");

                    // escape String & Datetime values!
                    if (rows[row][columns[0].ColumnName] == DBNull.Value)
                    {
                        queryBuilder.AppendFormat("null");
                    }
                    else if (columns[0].DataType == typeof(String))
                    {
                        queryBuilder.AppendFormat("'{0}'", MySqlHelper.EscapeString(rows[row][columns[0].ColumnName].ToString()));
                    }
                    else if (columns[0].DataType == typeof(DateTime))
                    {
                        dt = (DateTime)rows[row][columns[0].ColumnName];
                        queryBuilder.AppendFormat("'{0:yyyy-MM-dd HH:mm:ss}'", dt);
                    }
                    else if (columns[0].DataType == typeof(Int32))
                    {
                        queryBuilder.AppendFormat("{0}", rows[row].Field<Int32?>(columns[0].ColumnName) ?? 0);
                    }
                    else if (columns[0].DataType == typeof(double) || columns[0].DataType == typeof(decimal) || columns[0].DataType == typeof(float))
                    {
                        queryBuilder.AppendFormat("{0}", rows[row][columns[0].ColumnName].ToString().Replace(",", "."));
                    }
                    else
                    {
                        queryBuilder.AppendFormat("{0}", rows[row][columns[0].ColumnName]);
                    }

                    for (int col = 1; col < columns.Count; col++)
                    {
                        // escape String & Datetime values!
                        if (rows[row][columns[col].ColumnName] == DBNull.Value)
                        {
                            queryBuilder.AppendFormat(", null");
                        }
                        else if (columns[col].DataType == typeof(String))
                        {
                            queryBuilder.AppendFormat(", '{0}'", MySqlHelper.EscapeString(rows[row][columns[col].ColumnName].ToString()));
                        }
                        else if (columns[col].DataType == typeof(DateTime))
                        {
                            dt = (DateTime)rows[row][columns[col].ColumnName];
                            queryBuilder.AppendFormat(", '{0:yyyy-MM-dd HH:mm:ss}'", dt);
                        }
                        else if (columns[col].DataType == typeof(Int32))
                        {
                            queryBuilder.AppendFormat(", {0}", rows[row].Field<Int32?>(columns[col].ColumnName) ?? 0);
                        }
                        else if (columns[col].DataType == typeof(double) || columns[col].DataType == typeof(decimal) || columns[col].DataType == typeof(float))
                        {
                            queryBuilder.AppendFormat(", {0}", rows[row][columns[col].ColumnName].ToString().Replace(",", "."));
                        }
                        else
                        {
                            queryBuilder.AppendFormat(", {0}", rows[row][columns[col].ColumnName]);
                        }
                    } // end for (int i = 1; i < columns.Count; i++)

                    // close value block
                    queryBuilder.Append(")");
                    queryBuilder.AppendLine();

                } // end for (int r = 1; r < rows.Count; r++)

                // sql delimiter =)
                queryBuilder.Append(";");

            } // end if (rows.Count > 1)

            return queryBuilder.ToString();
        }
        return "";
    }
}