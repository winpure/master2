using Npgsql;
using NpgsqlTypes;
using System.Data.Common;
using System.Text;
using System.Transactions;

namespace WinPure.Integration.Export;

internal class PostgresExportProvider : DatabaseExportProviderBase
{
    private NpgsqlConnection _connection;

    public PostgresExportProvider() : base(ExternalSourceTypes.Postgres)
    {
        DisplayName = "Postgres";
    }

    protected override DbConnection DatabaseConnection => _connection;

    protected override void CreateConnection(string connectionString)
    {
        _connection = new NpgsqlConnection(connectionString);
    }

    protected override void ExportData(DataTable data)
    {
        if (CheckConnect())
        {
            Options.TableName =
                String.Join(".", Options.TableName.Split('.').Select(x => "\"" + x + "\"").ToArray());
            data.TableName = Options.TableName;
            var query = BuildCreateTableScript(data);
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                       new TransactionOptions
                           { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
            {
                if (TableExists)
                {
                    using (var cmd = new NpgsqlCommand($"DROP TABLE {Options.TableName}", _connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    string cols = "";
                    string pars = "";

                    foreach (DataColumn col in data.Columns)
                    {
                        cols += "\"" + col.ColumnName.Replace(" ", "_") + "\",";
                        pars += ":" + col.ColumnName.Replace(" ", "_") + ",";
                        cmd.Parameters.Add(":" + col.ColumnName.Replace(" ", "_"), GetParameterType(col.DataType));
                    }

                    cols = cols.Substring(0, cols.Length - 1);
                    pars = pars.Substring(0, pars.Length - 1);
                    cmd.CommandText = $"insert into {data.TableName} ( {cols} ) values ( {pars} ) ";
                    var step = Math.Max(data.Rows.Count / 100, 1);

                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        for (int j = 0; j < data.Columns.Count; j++)
                        {
                            cmd.Parameters[data.Columns[j].ColumnName.Replace(" ", "_")].Value = data.Rows[i][j];
                        }

                        int res = cmd.ExecuteNonQuery();

                        if (i % step == 0)
                        {
                            NotifyProgress(Resources.CAPTION_IO_DATA_EXPORTING, i / step);
                        }

                    }

                }

                transaction.Complete();
            }
        }
    }

    protected override List<string> GetDatabases()
    {
        var databases = _connection.GetSchema("Databases");
        var res = databases.Select().Select(x => x["database_name"].ToString()).ToList();
        return res;
    }

    protected override List<string> GetTables()
    {
        var tables = _connection.GetSchema("Tables");
        var views = _connection.GetSchema("Views");
        var res = tables.Select()
            .Select(x => x["TABLE_SCHEMA"].ToString() + "." + x["TABLE_NAME"].ToString()).OrderBy(x => x)
            .ToList();
        res.AddRange(views.Select()
            .Select(x => x["TABLE_SCHEMA"].ToString() + "." + x["TABLE_NAME"].ToString()).OrderBy(x => x)
            .ToList());
        return res;
    }

    protected override string CreateConnectionString()
    {
        if (Options == null)
        {
            return null;
        }

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = Options.ServerAddress,
            Port = Options.Port,
            CommandTimeout = 0,
            SslMode = SslMode.Prefer
        };

        if (Options.IntegrateSecurity)
        {
            connectionStringBuilder.PersistSecurityInfo = true;
        }
        else
        {
            connectionStringBuilder.PersistSecurityInfo = false;
            connectionStringBuilder.Username = Options.UserName;
            connectionStringBuilder.Password = Options.Password;
        }

        if (!string.IsNullOrWhiteSpace(Options.DatabaseName))
        {
            connectionStringBuilder.Database = Options.DatabaseName;
        }

        return connectionStringBuilder.ToString();
    }

    /// <summary>
    /// Creates a SQL script that creates a table where the columns matches that of the specified DataTable.
    /// </summary>
    private string BuildCreateTableScript(DataTable table)
    {
        var result = new StringBuilder();
        result.AppendFormat("CREATE TABLE {1} ({0}   ", Environment.NewLine, table.TableName);

        bool firstTime = true;
        foreach (DataColumn column in table.Columns)
        {
            if (firstTime) firstTime = false;
            else
                result.Append("   ,");

            result.AppendFormat("\"{0}\" {1} {2} {3}",
                column.ColumnName.Replace(" ", "_"), // 0
                GetSQLTypeAsString(column.DataType), // 1
                column.AllowDBNull ? "NULL" : "NOT NULL", // 2
                Environment.NewLine // 3
            );
        }
        result.Append(")");

        return result.ToString();
    }

    private NpgsqlDbType GetParameterType(Type dataType)
    {
        switch (dataType.Name)
        {
            case "Boolean": return NpgsqlDbType.Boolean;
            case "Char": return NpgsqlDbType.Char;
            case "SByte": return NpgsqlDbType.Smallint;
            case "Int16":
            case "Int32": return NpgsqlDbType.Integer;
            case "Int64": return NpgsqlDbType.Bigint;
            case "Byte": return NpgsqlDbType.Smallint;
            case "UInt16":
            case "UInt32": return NpgsqlDbType.Integer;
            case "UInt64": return NpgsqlDbType.Bigint;
            case "Single":
            case "Double": return NpgsqlDbType.Double;
            case "Decimal": return NpgsqlDbType.Money;
            case "DateTime": return NpgsqlDbType.Timestamp;
            case "Guid": return NpgsqlDbType.Uuid;
            case "Object": return NpgsqlDbType.Bytea;
            case "String": return NpgsqlDbType.Text;
            default: return NpgsqlDbType.Text;
        }
    }

    /// <summary>
    /// Returns the SQL data type equivalent, as a string for use in SQL script generation methods.
    /// </summary>
    private string GetSQLTypeAsString(Type dataType)
    {
        switch (dataType.Name)
        {
            case "Boolean": return "bool";
            case "Char": return "char";
            case "SByte": return "smallint";
            case "Int16":
            case "Int32": return "int4";
            case "Int64": return "bigint";
            case "Byte": return "smallint";
            case "UInt16":
            case "UInt32": return "int4";
            case "UInt64": return "bigint";
            case "Single":
            case "Double": return "double precision";
            case "Decimal": return "money";
            case "DateTime": return "timestamp";
            case "Guid": return "uuid";
            case "Object": return "bytea";
            case "String": return "text";
            default: return "text";
        }
    }

}