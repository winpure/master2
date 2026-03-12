using System.Data.Common;
using IBM.Data.DB2.Core;

namespace WinPure.Integration.Import;

internal class Db2ImportProvider : DatabaseImportProviderBase
{
    private DB2Connection _connection;
    protected override DbConnection DatabaseConnection => _connection;

    public Db2ImportProvider() : base(ExternalSourceTypes.Db2)
    {
        DisplayName = "DB2 Database";
    }

    public override void ExecuteSql(string script)
    {
        if (CheckConnect())
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = script;
                cmd.ExecuteNonQuery();
            }
        }
    }

    protected override DataTable GetPreview(int rowToPreview)
    {
        _previewTable = new DataTable();
        var query = BuildQuery(string.Empty, rowToPreview);
        var tblNameParts = Options.TableName.Split('.');
        ImportedInfo.DisplayName = tblNameParts.Length > 1 ? tblNameParts[1] : Options.TableName;
        using (var cmd = new DB2Command(query, _connection))
        {
            using (var da = new DB2DataAdapter(cmd))
            {
                da.Fill(_previewTable);
                return _previewTable;
            }
        }
    }

    protected override DataTable GetData()
    {
        var tblNameParts = Options.TableName.Split('.');
        ImportedInfo.FileName = tblNameParts.Length > 1 ? tblNameParts[1] : Options.TableName;
        NotifyProgress(Resources.CAPTION_IO_IMPORTING_DATA, 10);
        var dataTable = new DataTable();
        var query = BuildQuery(GetSelectFields(), _MAX_ROW_TO_IMPORT);
        using (var cmd = new DB2Command(query, _connection))
        {
            cmd.CommandTimeout = 0;
            using (var da = new DB2DataAdapter(cmd))
            {
                da.Fill(dataTable);
            }
            _connection.Close();
        }

        return dataTable;
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

    protected override void CreateConnection(string connectionString)
    {
        _connection = new DB2Connection(connectionString);
        _connection.Open();
    }

    protected override List<string> GetDatabases()
    {
        var result = new List<string>();
        try
        {
            var databases = _connection.GetSchema("Databases");
            result = databases.Rows.Cast<DataRow>().Select(row => row["database_name"].ToString()).ToList();
        }
        catch
        {
            //ignore
        }

        return result;
    }

    protected override List<string> GetTables()
    {
        var tables = _connection.GetSchema("Tables");
        var res = tables.Rows.Cast<DataRow>()
            .Select(row => row["TABLE_SCHEMA"].ToString() + "." + row["TABLE_NAME"].ToString()).OrderBy(x => x)
            .ToList();
        return res;
    }

    private string GetSelectFields()
    {
        var sql = Options.Fields.Aggregate("", (current, fld) => current + $"\"{fld.DatabaseName}\",");
        return sql.Substring(0, sql.Length - 1);
    }

    private static string FormatTableName(string tableName)
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
    private string BuildQuery(string fieldList, int numberOfRecords)
    {
        var sql = string.IsNullOrEmpty(fieldList) ? "SELECT *" : $"SELECT {GetSelectFields()}";
        sql += string.IsNullOrWhiteSpace(Options.SqlQuery) ? $" FROM {FormatTableName(Options.TableName)}" : $" FROM ({Options.SqlQuery}) a";
        if (numberOfRecords > 0)
        {
            sql += $" FETCH FIRST {numberOfRecords} ROWS ONLY";
        }

        return sql;
    }
}