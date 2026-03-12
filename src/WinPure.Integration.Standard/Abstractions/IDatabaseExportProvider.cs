namespace WinPure.Integration.Abstractions;

internal interface IDatabaseExportProvider : IExportProvider
{
    bool CheckConnect();
    List<string> GetDatabaseList();
    List<string> GetDatabaseTables();
}