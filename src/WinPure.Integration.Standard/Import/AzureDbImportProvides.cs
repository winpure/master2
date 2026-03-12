namespace WinPure.Integration.Import;

internal class AzureDbImportProvides : MsSqlServerImportProvider
{
    public AzureDbImportProvides() : base(ExternalSourceTypes.AzureDb)
    {
        DisplayName = "Azure DB";
    }
}