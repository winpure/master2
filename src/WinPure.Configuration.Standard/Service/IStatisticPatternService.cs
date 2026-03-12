namespace WinPure.Configuration.Service;

public interface IStatisticPatternService
{
    Task<List<StatisticPatternSetting>> GetAllPatterns();
    Task<StatisticPatternSetting> GetPattern(int id);
    Task<int> AddOrUpdatePattern(StatisticPatternSetting setting);
    Task Delete(int id);
}