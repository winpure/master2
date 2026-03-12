namespace WinPure.Configuration.Repository;

internal interface IConnectionSettingsRepository : IBaseRepository
{
    Task<List<ConnectionSettings>> GetConnectionSettings();
    Task<List<ConnectionSettings>> GetConnectionSettings(ExternalSourceTypes sourceType);
    Task<ConnectionSettingEntity> GetConnectionSetting(int id);
}