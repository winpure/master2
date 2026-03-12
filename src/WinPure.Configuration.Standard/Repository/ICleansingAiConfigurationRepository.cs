namespace WinPure.Configuration.Repository;

internal interface ICleansingAiConfigurationRepository : IBaseRepository
{
    Task<List<CleansingAiConfigurationEntity>> GetCleansingAiConfigurationsAsync();
    Task<CleansingAiConfigurationEntity> GetByNameAsync(string name);
}
