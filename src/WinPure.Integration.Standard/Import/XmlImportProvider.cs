using System.Data.CData.XML;

namespace WinPure.Integration.Import;

internal class XmlImportProvider : FileImportProviderBase
{
    public XmlImportProvider() : base(ExternalSourceTypes.Xml)
    {
        DisplayName = "XML";
    }

    protected override DataTable GetPreview(int rowToPreview)
    {
        _previewTable = new DataTable();

        using (var xmlConnection = new XMLConnection(GetConnectionString()))
        {
            xmlConnection.Open();
            var tableName = GetTables(xmlConnection).First();

            using (var cmd = xmlConnection.CreateCommand())
            {
                cmd.CommandText = $"SELECT TOP {rowToPreview} * FROM [{tableName}]";
                cmd.CommandType = CommandType.Text;
                using (var adapter = new XMLDataAdapter(cmd))
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
        var detectFieldTypes = Options.Fields.Any(x => x.FieldType != typeof(string).ToString());

        using (var xmlConnection = new XMLConnection(GetConnectionString(detectFieldTypes)))
        {
            xmlConnection.Open();
            var xmlTableName = GetTables(xmlConnection).First();

            var query = (_MAX_ROW_TO_IMPORT == 0)
                ? $"select { GetSelectFields() } from [{xmlTableName}]"
                : $"select top { _MAX_ROW_TO_IMPORT } { GetSelectFields() } from [{xmlTableName}]";

            using (var cmd = xmlConnection.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                using (var adapter = new XMLDataAdapter(cmd))
                {
                    adapter.Fill(dataTable);
                }
            }
        }

        return dataTable;
    }

    private XMLConnectionStringBuilder GetConnectionString(bool detectFieldTypes = true)
    {
        var connectionStringBuilder = new XMLConnectionStringBuilder
        {
            RowScanDepth = "0",
            URI = Options.FilePath,
            RTK = "4456524541415355525041413153554231454B473334333000000000000000000000000000000000485058585443455A000045574E41374A3341343946340000"
        };

        if (detectFieldTypes)
        {
            connectionStringBuilder.RowScanDepth = "200";
        }

        return connectionStringBuilder;
    }

    private List<string> GetTables(XMLConnection connection)
    {
        var schema = connection.GetSchema("TABLES", null);
        var tables = schema.Select("TABLE_TYPE='BASE TABLE'");
        var index = schema.Columns.IndexOf("TABLE_NAME");
        var tableNames = new List<string>();
        for (int i = 0; i < tables.Length; i++)
        {
            tableNames.Add((string)tables[i].ItemArray[index]);
        }

        return tableNames;
    }

    private string GetSelectFields()
    {
        var sql = Options.Fields.Aggregate("", (current, fld) => current + $"[{fld.DatabaseName}],");
        return sql.Substring(0, sql.Length - 1);
    }
}