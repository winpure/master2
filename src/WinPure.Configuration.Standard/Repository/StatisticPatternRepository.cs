namespace WinPure.Configuration.DependencyInjection;

internal partial class WinPureConfigurationDependency
{
    private class StatisticPatternRepository : BaseRepository, IStatisticPatternRepository
    {
        public StatisticPatternRepository(WinPureConfigurationContext context) : base(context)
        {
        }

        public async Task<List<StatisticPatternEntity>> GetAllPatterns() =>
            await _context.StatisticPatterns.ToListAsync();

        public async Task<StatisticPatternEntity> GetPattern(int id) =>
            await _context.StatisticPatterns.FirstOrDefaultAsync(x => x.Id == id);
    }
}