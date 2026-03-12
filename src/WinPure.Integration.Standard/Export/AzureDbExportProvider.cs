namespace WinPure.Integration.Export;

internal class AzureDbExportProvider : MsSqlServerExportProvider
{
    public AzureDbExportProvider() : base(ExternalSourceTypes.AzureDb)
    {
        DisplayName = "Azure DB";
    }

}