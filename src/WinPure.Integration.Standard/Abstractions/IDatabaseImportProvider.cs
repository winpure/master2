namespace WinPure.Integration.Abstractions;

internal interface IDatabaseImportProvider : IImportProvider
{
    bool CheckConnect();
    List<string> GetDatabaseList();
    List<string> GetDatabaseTables();
    (bool isValid, string error) ValidateSql();
    void ExecuteSql(string script);
}