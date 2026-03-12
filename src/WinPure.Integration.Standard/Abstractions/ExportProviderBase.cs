using WinPure.Integration.Enums;

namespace WinPure.Integration.Abstractions;

internal abstract class ExportProviderBase : ImportExportProviderBase, IExportProvider
{
    internal ExportProviderBase(ExternalSourceTypes sourceType) : base(OperationType.Export, sourceType)
    {
    }

    public string ExportParameters { get; internal set; }
    public abstract void Export(DataTable data);

    protected bool CheckExportData(DataTable data)
    {
        if (data == null || data.Columns.Count == 0)
        {
            NotifyException(Resources.EXCEPTION_IO_EMPTY_EXPORT_DATA, null);
            return false;
        }

        return true;
    }
}