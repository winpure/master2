using System.Data.Common;
using System.Data.SQLite;
using WinPure.Configuration.Helper;

namespace WinPure.Integration.Export;

internal class SqLiteExportProvider : DatabaseExportProviderBase
{
    private SQLiteConnection _connection;

    public SqLiteExportProvider() : base(ExternalSourceTypes.SqLite)
    {
        DisplayName = "SQLite";
    }

    protected override DbConnection DatabaseConnection => _connection;

    protected override void CreateConnection(string connectionString)
    {
        _connection = new SQLiteConnection(connectionString);
    }

    protected override void ExportData(DataTable data)
    {
        SqLiteHelper.SaveDataToDb(_connection, data, Options.TableName, null, false);
    }

    protected override List<string> GetDatabases()
    {
        return new List<string>();
    }

    protected override List<string> GetTables()
    {
        var tables = new List<string>();
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
        return tables;
    }

    protected override string CreateConnectionString()
    {
        return SystemDatabaseConnectionHelper.GetConnectionString(Options.DatabaseFile); ;
    }
}