using WinPure.Common.Models;
using WinPure.Integration.Abstractions;

namespace WinPure.Integration.Import;

internal class ImportFromDataTableProvider : ImportProviderBase
{
    private ImportFromDataTableOptions _options;
    public ImportFromDataTableProvider() : base(ExternalSourceTypes.DataTable)
    {
        DisplayName = "DataTable";
    }

    public override void Initialize(object parameters)
    {
        _options = parameters as ImportFromDataTableOptions;
    }

    protected override DataTable GetPreview(int rowToPreview)
    {
        return _options.Data;
    }

    protected override DataTable GetData()
    {
        throw new System.NotImplementedException();
    }

    public override DataTable ImportData()
    {
        if (_options == null)
        {
            return null;
        }

        ImportedInfo = new ImportedDataInfo
        {
            TableName = _options.TableName,
            FileName = _options.DisplayName,
            DisplayName = _options.DisplayName,
            RowCount = _options.Data.Rows.Count,
            IsStatisticCalculated = false,
            ImportParameters = "",
            SourceType = ExternalSourceTypes.DataTable,
            Fields = ColumnHelper.GetTableFields(_options.Data)
        };
        return _options.Data;
    }

    protected override DataTable GetReimportData(string parameterJson)
    {
        throw new System.NotImplementedException();
    }
}