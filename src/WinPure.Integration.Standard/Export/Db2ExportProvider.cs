using System.Data.Common;
using System.Text;
using IBM.Data.DB2.Core;

namespace WinPure.Integration.Export;

internal class Db2ExportProvider : DatabaseExportProviderBase
{
    private DB2Connection _connection;

    public Db2ExportProvider() : base(ExternalSourceTypes.Db2)
    {
        DisplayName = "DB2 database";
    }

    protected override DbConnection DatabaseConnection => _connection;

    protected override void CreateConnection(string connectionString)
    {
        _connection = new DB2Connection(connectionString);
        _connection.Open();
    }

    protected override void ExportData(DataTable data)
    {
        if (CheckConnect())
        {
            if (TableExists)
            {
                using (var cmd = new DB2Command($"DROP TABLE {FormatTableName(Options.TableName)}", _connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            // Create the table
            Options.TableName = string.Join(".", Options.TableName.Split('.').Select(x => $"\"{x}\"").ToArray());
            data.TableName = Options.TableName;
            var query = BuildCreateTableScript(data);


            using (var cmd = new DB2Command(query, _connection))
            {
                cmd.ExecuteNonQuery();
            }

            // Insert data using bulk copy
            using (var bulkCopy = new DB2BulkCopy(_connection, DB2BulkCopyOptions.Default))
            {
                bulkCopy.DestinationTableName = Options.TableName;
                bulkCopy.WriteToServer(data);
            }
        }
    }

    protected override List<string> GetDatabases()
    {
        List<string> databaseList = new List<string>();

        try
        {

            string query = @"SELECT db_name FROM sysibmadm.dbcfg";

            using (DB2Command command = new DB2Command(query, _connection))
            using (DB2DataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    databaseList.Add(reader["db_name"].ToString());
                }
            }
        }
        catch
        {
            //ignore
        }
        return databaseList;
    }

    protected override List<string> GetTables()
    {
        var tables = _connection.GetSchema("Tables");
        var res = tables.Rows.Cast<DataRow>()
            .Select(row => row["TABLE_SCHEMA"].ToString() + "." + row["TABLE_NAME"].ToString()).OrderBy(x => x)
            .ToList();
        return res;

    }

    protected override string CreateConnectionString()
    {
        if (Options != null)
        {
            if (string.IsNullOrEmpty(Options.DatabaseName))
                throw new ArgumentException("Database name can not be empty");

            var builder = new DB2ConnectionStringBuilder
            {
                Server = $"{Options.ServerAddress}:{Options.Port}",
                ClientApplicationName = "WinPure",
                Database = Options.DatabaseName
            };

            if (Options.IntegrateSecurity)
            {
                builder.Authentication = "Windows";
            }
            else
            {
                builder.UserID = Options.UserName;
                builder.Password = Options.Password;
            }

            return builder.ToString();
        }
        return string.Empty;
    }

    private string BuildCreateTableScript(DataTable table)
    {
        var result = new StringBuilder();
        result.AppendFormat("CREATE TABLE {0} ({1}", table.TableName, Environment.NewLine);

        bool firstColumn = true;
        foreach (DataColumn column in table.Columns)
        {
            if (!firstColumn) result.Append(",");
            firstColumn = false;

            result.AppendFormat("\"{0}\" {1} {2}",
                column.ColumnName, // Column name
                GetDb2TypeAsString(column.DataType), // DB2 type
                column.AllowDBNull ? "NULL" : "NOT NULL" // Nullability
            );
        }
        result.Append(");");

        return result.ToString();
    }

    private string GetDb2TypeAsString(Type dataType)
    {
        return dataType.Name switch
        {
            "Boolean" => "SMALLINT",
            "Byte" => "SMALLINT",
            "Int16" => "SMALLINT",
            "Int32" => "INTEGER",
            "Int64" => "BIGINT",
            "Single" => "REAL",
            "Double" => "DOUBLE",
            "Decimal" => "DECIMAL(18, 2)",
            "DateTime" => "TIMESTAMP",
            "String" => "VARCHAR(255)",
            _ => "VARCHAR(255)",
        };
    }

    public static string FormatTableName(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));
        }

        // Check if the table name contains a schema (indicated by a dot)
        if (tableName.Contains('.'))
        {
            var parts = tableName.Split('.');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid format for table name with schema. Expected 'schema.table'.", nameof(tableName));
            }

            string schema = parts[0].Trim();
            string table = parts[1].Trim();

            // Return the formatted schema.table with quotes
            return $"{schema}.\"{table}\"";
        }
        // Return the table name with quotes
        return $"\"{tableName.Trim()}\"";
    }
}