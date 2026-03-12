using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace WinPure.Integration.Import;

internal class MsSqlServerImportProvider : DatabaseImportProviderBase
{
    private SqlConnection _connection;
    protected override DbConnection DatabaseConnection => _connection;

    public MsSqlServerImportProvider(ExternalSourceTypes sourcetype) : base(sourcetype)
    {
    }

    public MsSqlServerImportProvider() : base(ExternalSourceTypes.SqlServer)
    {
        DisplayName = "MS SQL Server";
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
        var query = BuildQuery(string.Empty, _MAX_ROW_TO_PREVIEW);
        var tblNameParts = Options.TableName.Split('.');
        ImportedInfo.DisplayName = tblNameParts.Length > 1 ? tblNameParts[1] : Options.TableName;
        using (var cmd = new SqlCommand(query, _connection))
        {
            using (var da = new SqlDataAdapter(cmd))
            {
                // this will query your database and return the result to your datatable
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
        using (var cmd = new SqlCommand(query, _connection))
        {
            cmd.CommandTimeout = 0;
            var da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataTable);
            _connection.Close();
        }

        return dataTable;
    }

    protected override string CreateConnectionString()
    {
        if (Options != null)
        {
            var res = $"Persist Security Info=True;TrustServerCertificate=True;Connection Timeout=15;Data Source={Options.ServerAddress};Integrated Security={Options.IntegrateSecurity};";
            if (!Options.IntegrateSecurity)
            {
                res += $"User ID = {Options.UserName}; Password = {Options.Password};";
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

    protected override void CreateConnection(string connectionString)
    {
        _connection = new SqlConnection(connectionString);
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
    
    private string BuildQuery(string fieldList, int numberOfRecords)
    {
        var sql = "select ";
        if (numberOfRecords > 0)
        {
            sql += $" TOP {numberOfRecords}";
        }

        sql += string.IsNullOrEmpty(fieldList) ? " *" : $" {GetSelectFields()}";
        sql += string.IsNullOrWhiteSpace(Options.SqlQuery) ? $" from [{Options.TableName.Replace(".", "].[")}]" : $" from ({Options.SqlQuery}) a";
        
        return sql;
    }

    private string GetSelectFields()
    {
        var sql = Options.Fields.Aggregate("", (current, fld) => current + $"[{fld.DatabaseName}],");
        return sql.Substring(0, sql.Length - 1);
    }
}