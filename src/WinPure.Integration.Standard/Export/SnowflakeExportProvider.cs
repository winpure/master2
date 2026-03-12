using Snowflake.Client;
using System.Threading.Tasks;
using WinPure.Integration.Abstractions;

namespace WinPure.Integration.Export;

internal class SnowflakeExportProvider : ExportProviderBase, IDatabaseExportProvider
{
    private SnowflakeImportExportOptions _options;
    private SnowflakeClient _snowflakeClient;
    private const int MaxRowInsertPerRequest = 10;

    public SnowflakeExportProvider() : base(ExternalSourceTypes.Snowflake)
    {
        DisplayName = "Snowflake";
    }

    public override void Initialize(object parameters)
    {
        _options = parameters as SnowflakeImportExportOptions;
        _snowflakeClient = new SnowflakeClient(_options.User, _options.Password, _options.Region);
    }

    public override void Export(DataTable data)
    {
        if (data == null || data.Columns.Count == 0)
        {
            NotifyException(Resources.EXCEPTION_IO_EMPTY_EXPORT_DATA, null);
            return;
        }
        if (string.IsNullOrEmpty(_options.TableName) || string.IsNullOrEmpty(_options.Database) || string.IsNullOrEmpty(_options.Schema))
        {
            NotifyException(Resources.EXCEPTION_IO_TABLE_NAME_SHOULD_BE, null);
            return;
        }

        try
        {
            if (CheckConnect())
            {
                AsyncHelpers.RunSync(() => ExportData(data));
            }
            else
            {
                NotifyException(Resources.EXCEPTION_IO_CANNOT_CONNECT_TO_SERVER, null);
            }
        }
        catch (Exception ex)
        {
            _logger.Debug($"EXPORT TO {DisplayName} ERROR", ex);
            NotifyException(string.Format(Resources.EXCEPTION_IO_DATA_COULD_NOT_BE_EXPORTED_TO, DisplayName), ex.InnerException ?? ex);
        }
    }

    public bool CheckConnect()
    {
        try
        {
            var res = AsyncHelpers.RunSync(() => _snowflakeClient.InitNewSessionAsync());
            return res;
        }
        catch (Exception ex)
        {
            NotifyException(Resources.EXCEPTION_IO_CANNOT_CONNECT_TO_SERVER, ex);
            return false;
        }
    }

    public List<string> GetDatabaseList()
    {
        throw new System.NotImplementedException();
    }

    public List<string> GetDatabaseTables()
    {
        throw new System.NotImplementedException();
    }

    private async Task RemoveTableIfExists()
    {
        var sql = $"drop table if exists {_options.Database}.{_options.Schema}.{_options.TableName}";
        var res = await _snowflakeClient.ExecuteScalarAsync(sql);
    }

    private async Task ExportData(DataTable data)
    {
        await RemoveTableIfExists();
        var columnList = await CreateTable(data);
        var insertRowPerRequest = 0;
        var qry = string.Empty;
        var onePercentOfExport = data.Rows.Count > 100 ? data.Rows.Count / 100 : data.Rows.Count;
        short currentPercentOfExport = 0;
        for (int i = 0; i < data.Rows.Count; i++)
        {
            if (string.IsNullOrEmpty(qry))
            {
                qry = $"INSERT INTO {_options.Database}.{_options.Schema}.{_options.TableName} values ";
            }

            qry += "(";
            for (int j = 0; j < data.Columns.Count; j++)
            {

                var value = data.Rows[i][j] == DBNull.Value 
                    ? "NULL"
                    : data.Columns[j].DataType == typeof(decimal) || data.Columns[j].DataType == typeof(double)
                        ? data.Rows[i][j].ToString().Replace(",", ".")
                        : data.Columns[j].DataType == typeof(DateTime)
                            ? $"'{((DateTime)data.Rows[i][j]).ToString("s")}'"
                            : $"'{data.Rows[i][j].ToString().Replace("'", @"\'")}'";

                qry += $" {value}" + (j == data.Columns.Count-1 ? ")" : ",");
            }

            insertRowPerRequest++;
            if (insertRowPerRequest == MaxRowInsertPerRequest)
            {
                await _snowflakeClient.ExecuteScalarAsync(qry);
                insertRowPerRequest = 0;
                qry = string.Empty;
            }
            else
            {
                qry += ", ";
            }
            if (i % onePercentOfExport == 0)
            {
                currentPercentOfExport++;
                NotifyProgress(Resources.CAPTION_IO_DATA_EXPORTING, currentPercentOfExport);
            }
        }

        if (!string.IsNullOrEmpty(qry))
        {
            Console.WriteLine(qry);
            await _snowflakeClient.ExecuteScalarAsync(qry.Substring(0, qry.Length - 2));
        }
    }

    private async Task<string> CreateTable(DataTable data)
    {
        var columns = "";
        var columnWithTypes = "";
        for (int i = 0; i < data.Columns.Count; i++)
        {
            var column = data.Columns[i];
            columns += $"{column.ColumnName},";
            columnWithTypes += $" \"{column.ColumnName}\" {MapDataType(column.DataType)},";
        }

        columns = columns.Substring(0, columns.Length - 1);
        columnWithTypes = columnWithTypes.Substring(0, columnWithTypes.Length - 1);

        var sql = $"CREATE OR REPLACE TABLE {_options.Database}.{_options.Schema}.{_options.TableName} ({columnWithTypes});";
        var res = await _snowflakeClient.ExecuteScalarAsync(sql);
        return columns;
    }

    private string MapDataType(Type columnType)
    {
        if (columnType == typeof(int) || columnType == typeof(long))
            return "NUMBER(38,0)";

        if (columnType == typeof(decimal))
            return "NUMBER(38,8)";

        if (columnType == typeof(double))
            return "REAL";

        if (columnType == typeof(bool))
            return "BOOLEAN";

        if (columnType == typeof(byte))
            return "BINARY";

        if (columnType == typeof(DateTime))
            return "DATE";

        return "TEXT";
    }
}