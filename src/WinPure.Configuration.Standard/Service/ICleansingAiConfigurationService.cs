namespace WinPure.Configuration.Service;

public interface ICleansingAiConfigurationService
{
    Task<List<CleansingAiFieldType>> GetAllConfigurations();
    Task<CleansingAiFieldType> GetConfigurationByType(string aiType);
    Task AddOrUpdate(CleansingAiFieldType configuration);
    Task<bool> Delete(string aiType);
    Task SyncAiTypes(List<CleansingAiFieldType> aiTypes);
}
