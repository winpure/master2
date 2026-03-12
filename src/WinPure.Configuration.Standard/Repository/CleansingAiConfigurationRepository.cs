namespace WinPure.Configuration.DependencyInjection;

internal partial class WinPureConfigurationDependency
{
    private class CleansingAiConfigurationRepository : BaseRepository, ICleansingAiConfigurationRepository
    {
        public CleansingAiConfigurationRepository(WinPureConfigurationContext context) : base(context)
        {
        }

        public async Task<List<CleansingAiConfigurationEntity>> GetCleansingAiConfigurationsAsync()
        {
            return await _context.CleansingAiConfigurations.ToListAsync();
        }

        public async Task<CleansingAiConfigurationEntity> GetByNameAsync(string name)
        {
            return await _context.CleansingAiConfigurations.FirstOrDefaultAsync(x => x.AiType == name);
        }
    }
}