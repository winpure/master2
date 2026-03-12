using System.Data.CData.JSON;

namespace WinPure.Integration.Import;

internal class JsonImportProvider : FileImportProviderBase
{
    public JsonImportProvider() : base(ExternalSourceTypes.Json)
    {
        DisplayName = "JSON";
    }

    protected override DataTable GetPreview(int rowToPreview)
    {
        _previewTable = new DataTable();

        using (var jsonConnection = new JSONConnection(GetConnectionString()))
        {
            jsonConnection.Open();

            using (var cmd = jsonConnection.CreateCommand())
            {
                cmd.CommandText = $"SELECT TOP {rowToPreview} * FROM [json]";
                cmd.CommandType = CommandType.Text;
                using (var adapter = new JSONDataAdapter(cmd))
                {
                    adapter.Fill(_previewTable);
                    return _previewTable;
                }
            }
        }
    }

    protected override DataTable GetData()
    {
        NotifyProgress(Resources.CAPTION_IO_IMPORTING_DATA, 10);
        var dataTable = new DataTable();
        var query = (_MAX_ROW_TO_IMPORT == 0)
            ? $"select { GetSelectFields() } from [json]"
            : $"select top { _MAX_ROW_TO_IMPORT } { GetSelectFields() } from [json]";

        using (var jsonConnection = new JSONConnection(GetConnectionString()))
        {
            jsonConnection.Open();

            using (var cmd = jsonConnection.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                using (var adapter = new JSONDataAdapter(cmd))
                {
                    adapter.Fill(dataTable);
                }
            }
        }
        return dataTable;
    }

    private JSONConnectionStringBuilder GetConnectionString()
    {
        var connectionStringBuilder = new JSONConnectionStringBuilder
        {
            RowScanDepth = "200",
            URI = Options.FilePath,
            RTK = "444A524541415355525041413153554231454B473334333000000000000000000000000000000000485058585443455A000037585A34314B5253423650370000"

        };
        return connectionStringBuilder;
    }

    private string GetSelectFields()
    {
        var sql = Options.Fields.Aggregate("", (current, fld) => current + $"[{fld.DatabaseName}],");
        return sql.Substring(0, sql.Length - 1);
    }
}