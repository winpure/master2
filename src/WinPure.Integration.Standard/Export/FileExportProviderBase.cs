using Newtonsoft.Json;
using WinPure.Integration.Abstractions;

namespace WinPure.Integration.Export;

internal abstract class FileExportProviderBase : ExportProviderBase
{
    protected TextImportExportOptions Options;

    internal FileExportProviderBase(ExternalSourceTypes sourceType) : base(sourceType)
    {
    }

    public override void Initialize(object parameters)
    {
        Options = parameters as TextImportExportOptions;
        if (Options == null)
        {
            NotifyException(Resources.EXCEPTION_IO_PARAMETERS_WRONG_FORMAT, null);
        }
        else
        {
            ExportParameters = JsonConvert.SerializeObject(Options);
        }
    }

    public override void Export(DataTable data)
    {
        try
        {
            if (!CheckExportData(data) || !CheckFilePath())
            {
                return;
            }

            var fileContent = GetFileContent(data);
            FileHelper.CreateOrOverrideFile(Options.FilePath, fileContent);
        }
        catch (Exception ex)
        {
            _logger.Debug($"EXPORT TO {DisplayName} ERROR", ex);
            NotifyException(string.Format(Resources.EXCEPTION_IO_DATA_COULD_NOT_BE_EXPORTED_TO, $"{DisplayName} file"), ex);
        }
    }

    protected bool CheckFilePath()
    {
        if (string.IsNullOrWhiteSpace(Options.FilePath))
        {
            NotifyException(Resources.EXCEPTION_IO_INVALID_EXPORT_FILE_NAME, null);
            return false;
        }

        return true;
    }

    protected abstract string GetFileContent(DataTable data);
}