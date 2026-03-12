using Newtonsoft.Json;
using System.IO;
using WinPure.Integration.Abstractions;

namespace WinPure.Integration.Import;

internal abstract class FileImportProviderBase : ImportProviderBase
{
    internal TextImportExportOptions Options;

    internal FileImportProviderBase(ExternalSourceTypes sourceType) : base(sourceType)
    {
    }

    public override void Initialize(object parameters)
    {
        Options = parameters as TextImportExportOptions;
        if (Options == null)
        {
            NotifyException(Resources.EXCEPTION_IO_PARAMETERS_WRONG_FORMAT, null);
        }
        ImportedInfo.DisplayName = Path.GetFileNameWithoutExtension(Options.FilePath);
        ImportedInfo.FileName = Options.FilePath;
    }

    public override DataTable ImportData()
    {
        try
        {
            Options.Fields = ImportedInfo.Fields;
            var result = GetData();
            ImportedInfo.RowCount = result?.Rows.Count ?? 0;
            ImportedInfo.ImportParameters = JsonConvert.SerializeObject(Options);
            LimitedRecords = ImportedInfo.RowCount == _MAX_ROW_TO_IMPORT;
            return result;
        }
        catch (Exception ex)
        {
            _logger.Debug($"IMPORT FROM {DisplayName} ERROR", ex);
            NotifyException(Resources.EXCEPTION_IO_ERROR_ON_IMPORT, ex);
        }

        return null;
    }

    protected override DataTable GetReimportData(string parameterJson)
    {
        Options = JsonConvert.DeserializeObject<TextImportExportOptions>(parameterJson);
        ImportedInfo.Fields = Options.Fields;
        var data = GetData();
        ImportedInfo.RowCount = data?.Rows.Count ?? 0;
        ImportedInfo.ImportParameters = JsonConvert.SerializeObject(Options);
        LimitedRecords = ImportedInfo.RowCount == _MAX_ROW_TO_IMPORT;
        return data;
    }
}