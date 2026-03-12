namespace WinPure.Configuration.Repository;

public interface IStatisticPatternRepository
{
    Task<List<StatisticPatternEntity>> GetAllPatterns();
    Task<StatisticPatternEntity> GetPattern(int id);
    void Add(object entity);
    void Delete(object entity);
    Task SaveChangesAsync();
}