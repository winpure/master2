using Oracle.ManagedDataAccess.Client;
using System.Data.Common;

namespace WinPure.Integration.Import;

internal class OracleImportProvider : DatabaseImportProviderBase
{
    private OracleConnection _connection;
    protected override DbConnection DatabaseConnection => _connection;

    public OracleImportProvider() : base(ExternalSourceTypes.Oracle)
    {
        DisplayName = "ORACLE Server";
    }

    protected override DataTable GetPreview(int rowToPreview)
    {
        _previewTable = new DataTable();
        var tblNameParts = Options.TableName.Split('.');
        ImportedInfo.DisplayName = tblNameParts.Length > 1 ? tblNameParts[1] : Options.TableName;
        var sql = BuildQuery(string.Empty, rowToPreview);
        using (var command = new OracleCommand(sql, _connection))
        {
            command.CommandType = CommandType.Text;
            using (var adapter = new OracleDataAdapter(command))
            {
                adapter.Fill(_previewTable);
            }
            return _previewTable;
        }
    }

    protected override DataTable GetData()
    {
        var tblNameParts = Options.TableName.Split('.');
        ImportedInfo.FileName = tblNameParts.Length > 1 ? tblNameParts[1] : Options.TableName;
        var dataTable = new DataTable();
        var query = BuildQuery(GetSelectFields(), _MAX_ROW_TO_IMPORT);
        using (var command = new OracleCommand(query, _connection))
        {

            command.CommandType = CommandType.Text;
            using (var adapter = new OracleDataAdapter(command))
            {
                adapter.Fill(dataTable);
            }
            _connection.Close();

        }

        return dataTable;
    }

    protected override string CreateConnectionString()
    {
        if (Options != null && !string.IsNullOrEmpty(Options.ServerAddress))
        {
            var srvParts = Options.ServerAddress.Split(':');
            if (srvParts.Length == 1 || srvParts.Length == 2)
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

            if (srvParts.Length == 3)
            {
                return $"(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={srvParts[0]})(PORT={srvParts[1]}))(CONNECT_DATA=(SERVER = DEDICATED)(SERVICE_NAME={srvParts[2]})));User Id={Options.UserName};Password = {Options.Password}; ";
            }
        }
        return "";
    }

    protected override void CreateConnection(string connectionString)
    {
        _connection = new OracleConnection(connectionString);
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
        return res.OrderBy(x => x).ToList();
    }

    private string BuildQuery(string fieldList, int numberOfRecords)
    {
        var sql = string.IsNullOrEmpty(fieldList) ? "select *" : $"select {GetSelectFields()}";
        sql += string.IsNullOrWhiteSpace(Options.SqlQuery) ? $" from {Options.TableName}" : $" from ({Options.SqlQuery}) a";
        if (numberOfRecords > 0)
        {
            sql += $" where rownum <= {numberOfRecords}";
        }

        return sql;
    }

    private string GetSelectFields()
    {
        var sql = Options.Fields.Aggregate("", (current, fld) => current + $"{fld.DatabaseName},");
        return sql.Substring(0, sql.Length - 1);
    }
}