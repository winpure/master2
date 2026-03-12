namespace WinPure.Configuration.Repository;

internal interface IProperCaseSettingsRepository : IBaseRepository
{
    Task<List<ProperCaseSettingEntity>> GetProperCaseSettings();
}