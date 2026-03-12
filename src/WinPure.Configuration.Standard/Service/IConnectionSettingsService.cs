namespace WinPure.Configuration.Service;

internal interface IConnectionSettingsService
{
    List<ConnectionSettings> Get();
    List<ConnectionSettings> Get(ExternalSourceTypes sourceType);
    ConnectionSettings Get<T>(int id);
    int Save(ConnectionSettings settings);
    void Delete(int id);
}