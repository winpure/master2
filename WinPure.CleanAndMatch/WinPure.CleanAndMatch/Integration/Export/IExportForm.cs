namespace WinPure.CleanAndMatch.Integration.Export;

internal interface IExportForm
{
    IExportProvider ExportProvider { get; }
    bool ShowExportForm(IExportProvider ieData);
}