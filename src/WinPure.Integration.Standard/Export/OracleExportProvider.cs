using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Text;
using System.Transactions;

namespace WinPure.Integration.Export;

internal class OracleExportProvider : DatabaseExportProviderBase
{
    private OracleConnection _connection;

    protected override DbConnection DatabaseConnection => _connection;

    public OracleExportProvider() : base(ExternalSourceTypes.Oracle)
    {
        DisplayName = "ORACLE Server";
    }

    protected override void CreateConnection(string connectionString)
    {
        _connection = new OracleConnection(connectionString);
    }

    protected override void ExportData(DataTable data)
    {
        if (CheckConnect())
        {
            Options.TableName =
                Options.UserName + "." +
                Options.TableName; //String.Join(".", _options.TableName.Split('.').Select(x => "[" + x + "]").ToArray());
            data.TableName = Options.TableName;
            var columns = BuildCreateTableScript(data);
            var query = columns["WpOracleCreateTableScript"];
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                       new TransactionOptions
                           {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted}))
            {
                if (TableExists)
                {
                    using (var cmd = new OracleCommand($"DROP TABLE {Options.TableName}", _connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                using (var cmd = new OracleCommand(query, _connection))
                {
                    cmd.ExecuteNonQuery();
                }

                var enu = data.AsEnumerable();
                string cols = "";
                string pars = "";

                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.ArrayBindCount = enu.Count();

                    foreach (DataColumn col in data.Columns)
                    {
                        cols += "\"" + columns[col.ColumnName] + "\",";
                        pars += ":\"" + columns[col.ColumnName] + "\",";
                        var tp = col.DataType;
                        cmd.Parameters.Add(":" + columns[col.ColumnName], GetOracleType(col.DataType),
                            enu.Select(x => x.Field<object>(col.ColumnName)).ToArray(), ParameterDirection.Input);
                    }

                    cols = cols.Substring(0, cols.Length - 1);
                    pars = pars.Substring(0, pars.Length - 1);
                    cmd.CommandText = $"insert into {data.TableName} ( {cols} ) values ( {pars} ) ";

                    int res = cmd.ExecuteNonQuery();
                }

                transaction.Complete();
            }
        }
    }

    protected override List<string> GetDatabases()
    {
        var res = new List<string>();
        string sql = "select ora_database_name from dual";
        using (var command = new OracleCommand(sql, _connection))
        {
            command.CommandType = CommandType.Text;

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    res.Add(reader[0].ToString());
                }
            }
        }
        return res;
    }

    protected override List<string> GetTables()
    {
        var res = new List<string>();
        string sql = "SELECT owner, table_name FROM ALL_TABLES";
        using (var command = new OracleCommand(sql, _connection))
        {
            command.CommandType = CommandType.Text;

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    res.Add(reader[0] + "." + reader[1]);
                }
            }
        }

        return res;
    }

    protected override string CreateConnectionString()
    {
        if (Options != null)
        {
            var res = $"Data Source={Options.ServerAddress}";
            if (!string.IsNullOrEmpty(Options.DatabaseName)) res += "/" + Options.DatabaseName;

            res += ";";

            if (!Options.IntegrateSecurity)
            {
                res += $"Password={Options.Password};User ID={Options.UserName}";
            }
            return res;
        }
        return "";
    }


    /// <summary>
    /// Creates a SQL script that creates a table where the columns matches that of the specified DataTable.
    /// </summary>
    private Dictionary<string, string> BuildCreateTableScript(DataTable table)
    {
        var dict = new Dictionary<string, string>();
        StringBuilder result = new StringBuilder();
        result.AppendFormat("CREATE TABLE {1} ({0}   ", Environment.NewLine, table.TableName);

        bool firstTime = true;
        foreach (DataColumn column in table.Columns)
        {
            var colName = (column.ColumnName.Replace(" ", "").Length > 25) ? column.ColumnName.Replace(" ", "").Substring(0, 25) : column.ColumnName.Replace(" ", "");

            int i = 1;
            while (dict.Values.Contains(colName))
            {
                colName = ((column.ColumnName.Replace(" ", "").Length > 25) ? column.ColumnName.Replace(" ", "").Substring(0, 25) : column.ColumnName.Replace(" ", "")) + i;
                i++;
            }
            dict.Add(column.ColumnName, colName);
            if (firstTime) firstTime = false;
            else
                result.Append("   ,");

            result.AppendFormat("\"{0}\" {1} {2} {3}",
                colName, // 0
                GetOracleTypeAsString(column.DataType), // 1
                column.AllowDBNull ? "" : "NOT NULL", // 2
                Environment.NewLine // 3
            );
        }
        result.AppendFormat("){0}", Environment.NewLine);

        dict.Add("WpOracleCreateTableScript", result.ToString());
        return dict;
    }

    private OracleDbType GetOracleType(Type dataType)
    {
        switch (dataType.Name)
        {
            case "Boolean": return OracleDbType.Byte;
            case "Char": return OracleDbType.Char;
            case "SByte": return OracleDbType.Byte;
            case "Int16": return OracleDbType.Int16;
            case "Int32": return OracleDbType.Int32;
            case "Int64": return OracleDbType.Int64;
            case "Byte": return OracleDbType.Byte;
            case "UInt16": return OracleDbType.Int16;
            case "UInt32": return OracleDbType.Int32;
            case "UInt64": return OracleDbType.Int64;
            case "Single": return OracleDbType.BinaryDouble;
            case "Double": return OracleDbType.BinaryDouble;
            case "Decimal": return OracleDbType.BinaryDouble;
            case "DateTime": return OracleDbType.Date;
            case "Guid": return OracleDbType.NVarchar2;
            case "Object": return OracleDbType.Blob;
            case "String": return OracleDbType.NVarchar2;
            default: return OracleDbType.NVarchar2;
        }
    }

    /// <summary>
    /// Returns the SQL data type equivalent, as a string for use in SQL script generation methods.
    /// </summary>
    private string GetOracleTypeAsString(Type dataType)
    {
        switch (dataType.Name)
        {
            case "Boolean": return "NUMBER(1)";
            case "Char": return "char";
            case "SByte": return "SMALLINT";
            case "Int16": return "SMALLINT";
            case "Int32": return "INTEGER";
            case "Int64": return "NUMBER";
            case "Byte": return "SMALLINT";
            case "UInt16": return "SMALLINT";
            case "UInt32": return "INTEGER";
            case "UInt64": return "NUMBER";
            case "Single": return "BINARY_DOUBLE";
            case "Double": return "BINARY_DOUBLE";
            case "Decimal": return "BINARY_DOUBLE";
            case "DateTime": return "Date";
            case "Guid": return "varchar2(255 CHAR)";
            case "Object": return "BLOB";
            case "String": return "varchar2(255 CHAR)";
            default: return "varchar2(255 CHAR)";
        }
    }
}