using System.IO;
using Newtonsoft.Json;

namespace WinPure.Integration.Import;

internal class SenzingImportProvider : FileImportProviderBase
{
    public SenzingImportProvider() : base(ExternalSourceTypes.Senzing)
    {
        DisplayName = "Senzing";
    }

    protected override DataTable GetPreview(int rowToPreview)
    {
        _previewTable = new DataTable();

        var fileData = ReadDataAndConvertToJson();
        _previewTable = JsonConvert.DeserializeObject<DataTable>(fileData);

        PostProcessing(_previewTable);

        return _previewTable;
    }

    protected override DataTable GetData()
    {
        NotifyProgress(Resources.CAPTION_IO_IMPORTING_DATA, 10);
        var dataTable = new DataTable();

        var fileData = ReadDataAndConvertToJson();
        dataTable = JsonConvert.DeserializeObject<DataTable>(fileData);

        PostProcessing(dataTable);
        //_previewTable = dataTable.Copy();

        return dataTable;
    }

    private string ReadDataAndConvertToJson()
    {
       
        var fileData = string.Join(",", File.ReadAllLines(Options.FilePath));

        return $"[{fileData}]";
    }

    private void PostProcessing(DataTable data)
    {
        for (int i = 0; i < data.Columns.Count; i++)
        {
            if (data.Columns[i].ColumnName == "DATA_SOURCE")
            {
                data.Columns[i].ColumnName = "OriginalDataSourceName";
            } else if (data.Columns[i].ColumnName == "RECORD_ID")
            {
                data.Columns[i].ColumnName = "OriginalRecordId";
            }
        }
    }
}