using System.Data.Common;
using System.Text;
using System.Transactions;
using Microsoft.Data.SqlClient;

namespace WinPure.Integration.Export;

internal class MsSqlServerExportProvider : DatabaseExportProviderBase
{
    private SqlConnection _connection;

    public MsSqlServerExportProvider(ExternalSourceTypes sourceType) : base(sourceType)
    {
    }

    public MsSqlServerExportProvider() : base(ExternalSourceTypes.SqlServer)
    {
        DisplayName = "MS SQL Server";
    }

    protected override void CreateConnection(string connectionString)
    {
        _connection = new SqlConnection(connectionString);
    }

    protected override void ExportData(DataTable data)
    {
        if (CheckConnect())
        {
            if (TableExists)
            {
                using (var cmd = new SqlCommand($"DROP TABLE {Options.TableName}", _connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            using (var bulkCopy = new SqlBulkCopy(_connection))
            {
                Options.TableName = String.Join(".",
                    Options.TableName.Split('.').Select(x => "[" + x + "]").ToArray());
                data.TableName = Options.TableName;
                var query = BuildCreateTableScript(data);
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                {
                    using (var cmd = new SqlCommand(query, _connection))
                    {
                        cmd.ExecuteNonQuery();
                        transaction.Complete();
                    }

                    bulkCopy.DestinationTableName = Options.TableName;
                    bulkCopy.BulkCopyTimeout = 0;
                    bulkCopy.WriteToServer(data);
                }
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
        var res =
            tables.Select()
                .Select(x => /*x["TABLE_SCHEMA"].ToString() + "." + */x["TABLE_NAME"].ToString())
                .ToList();
        return res;
    }

    protected override DbConnection DatabaseConnection => _connection;

    protected override string CreateConnectionString()
    {
        if (Options != null)
        {
            var res = $"Persist Security Info=True;TrustServerCertificate=True;Connection Timeout=0;Data Source={Options.ServerAddress};Integrated Security={Options.IntegrateSecurity};";
            if (!Options.IntegrateSecurity)
            {
                res += $"User ID = {Options.UserName}; Password = {Options.Password};";
            }
            else
            {
                res += $"UID = auth_windows;";
            }
            if (!string.IsNullOrEmpty(Options.DatabaseName))
            {
                res += $"Initial Catalog = {Options.DatabaseName}";
            }
            //;Initial Catalog = {DatabaseName}; User ID = {UserName}; Password = {Password}";
            return res;
        }
        return "";
    }


    /// <summary>
    /// Creates a SQL script that creates a table where the columns matches that of the specified DataTable.
    /// </summary>
    private string BuildCreateTableScript(DataTable table)
    {
        StringBuilder result = new StringBuilder();
        result.AppendFormat("CREATE TABLE {1} ({0}   ", Environment.NewLine, table.TableName);

        bool firstTime = true;
        foreach (DataColumn column in table.Columns)
        {
            if (firstTime) firstTime = false;
            else
                result.Append("   ,");

            result.AppendFormat("[{0}] {1} {2} {3}",
                column.ColumnName, // 0
                GetSQLTypeAsString(column.DataType), // 1
                column.AllowDBNull ? "NULL" : "NOT NULL", // 2
                Environment.NewLine // 3
            );
        }
        result.AppendFormat(") ON [PRIMARY]{0}", Environment.NewLine);

        // Build an ALTER TABLE script that adds keys to a table that already exists.
        //if (table.PrimaryKey.Length > 0)
        //    result.Append(BuildKeysScript(table));

        return result.ToString();
    }

    /// <summary>
    /// Returns the SQL data type equivalent, as a string for use in SQL script generation methods.
    /// </summary>
    private string GetSQLTypeAsString(Type dataType)
    {
        switch (dataType.Name)
        {
            case "Boolean": return "[bit]";
            case "Char": return "[char]";
            case "SByte": return "[tinyint]";
            case "Int16": return "[smallint]";
            case "Int32": return "[int]";
            case "Int64": return "[bigint]";
            case "Byte": return "[tinyint] UNSIGNED";
            case "UInt16": return "[smallint] UNSIGNED";
            case "UInt32": return "[int] UNSIGNED";
            case "UInt64": return "[bigint] UNSIGNED";
            case "Single": return "[float]";
            case "Double": return "[float]";
            case "Decimal": return "[decimal]";
            case "DateTime": return "[datetime]";
            case "Guid": return "[uniqueidentifier]";
            case "Object": return "[variant]";
            case "String": return "[nvarchar](MAX)";
            default: return "[nvarchar](MAX)";
        }
    }
}