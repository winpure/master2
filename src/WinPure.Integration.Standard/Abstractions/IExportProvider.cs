namespace WinPure.Integration.Abstractions;

internal interface IExportProvider : IImportExportProvider
{
    string ExportParameters { get; }
    void Export(DataTable data);
}