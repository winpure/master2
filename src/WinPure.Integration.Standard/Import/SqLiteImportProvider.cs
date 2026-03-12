using System.Data.Common;
using System.Data.SQLite;
using WinPure.Configuration.Helper;

namespace WinPure.Integration.Import;

internal class SqLiteImportProvider : DatabaseImportProviderBase
{
    private SQLiteConnection _connection;
    protected override DbConnection DatabaseConnection => _connection;

    public SqLiteImportProvider() : base(ExternalSourceTypes.SqLite)
    {
        DisplayName = "SQLite";
    }

    protected override DataTable GetPreview(int rowToPreview)
    {
        if (CheckConnect())
        {
            ImportedInfo.DisplayName = Options.TableName;
            string selCmd = BuildQuery(string.Empty, _MAX_ROW_TO_PREVIEW);
            return SqLiteHelper.ExecuteQuery(selCmd, _connection, CommandBehavior.Default, "PreviewData");
        }

        return null;
    }

    protected override DataTable GetData()
    {
        NotifyProgress(Resources.CAPTION_IO_PREPARING_TO_IMPORT, 5);

        string selCmd = BuildQuery(GetSelectFields(), _MAX_ROW_TO_IMPORT);

        NotifyProgress(Resources.CAPTION_IO_IMPORTING_DATA, 10);

        var tbl = SqLiteHelper.ExecuteQuery(selCmd, _connection, CommandBehavior.Default, "ImportData");
        return tbl;
    }

    protected override void CreateConnection(string connectionString)
    {
        _connection = new SQLiteConnection(connectionString);
    }

    protected override string CreateConnectionString()
    {
        if (Options != null)
        {
            var res = SystemDatabaseConnectionHelper.GetConnectionString(Options.DatabaseFile);
            return res;
        }
        return "";
    }

    protected override List<string> GetDatabases()
    {
        return new List<string>();
    }

    protected override List<string> GetTables()
    {
        var tables = new List<string>();
        if (CheckConnect())
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "SELECT name from sqlite_master WHERE type='table'";
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        tables.Add(result.GetString(0));
                    }
                }
            }
        }

        return tables;
    }

    private string BuildQuery(string fieldList, int numberOfRecords)
    {
        var sql = string.IsNullOrEmpty(fieldList) ? "SELECT *" : $"SELECT {GetSelectFields()}";
        sql += string.IsNullOrWhiteSpace(Options.SqlQuery) ? $" FROM [{Options.TableName}]" : $" from ({Options.SqlQuery}) a";
        if (numberOfRecords > 0)
        {
            sql += $" LIMIT {numberOfRecords}";
        }

        return sql;
    }

    private string GetSelectFields()
    {
        var sql = Options.Fields.Aggregate("", (current, fld) => current + $"[{fld.DatabaseName}],");
        return sql.Substring(0, sql.Length - 1);
    }
}