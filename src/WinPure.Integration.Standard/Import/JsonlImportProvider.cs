namespace WinPure.Integration.Import;

internal class JsonlImportProvider : FileImportProviderBase
{
    public JsonlImportProvider() : base(ExternalSourceTypes.JSONL)
    {
        DisplayName = "JSONL";
    }

    protected override DataTable GetPreview(int rowToPreview)
    {
        _previewTable = new DataTable();

        var fileData = ReadDataAndConvertToJson();
        _previewTable = fileData.Table;

        PostProcessing(_previewTable);

        return _previewTable;
    }

    protected override DataTable GetData()
    {
        NotifyProgress(Resources.CAPTION_IO_IMPORTING_DATA, 10);
        var dataTable = new DataTable();

        var fileData = ReadDataAndConvertToJson();
        dataTable = fileData.Table;
        ImportedInfo.AdditionalInfo = fileData.DocumentSchema;

        PostProcessing(dataTable);
        //_previewTable = dataTable.Copy();

        return dataTable;
    }

    private JsonlConversionResult ReadDataAndConvertToJson()
    {
        return JsonlConverter.ConvertJsonlToDataTable(Options.FilePath);
    }

    private void PostProcessing(DataTable data)
    {
        for (int i = 0; i < data.Columns.Count; i++)
        {
            if (data.Columns[i].ColumnName == "DATA_SOURCE")
            {
                data.Columns[i].ColumnName = "OriginalDataSourceName";
            }
            else if (data.Columns[i].ColumnName == "RECORD_ID")
            {
                data.Columns[i].ColumnName = "OriginalRecordId";
            }
        }
    }
}