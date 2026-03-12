namespace WinPure.Configuration.DependencyInjection;

internal partial class WinPureConfigurationDependency
{
    private class ProperCaseSettingsRepository : BaseRepository, IProperCaseSettingsRepository
    {
        public ProperCaseSettingsRepository(WinPureConfigurationContext context) : base(context)
        {
        }

        public async Task<List<ProperCaseSettingEntity>> GetProperCaseSettings()
        {
            return await _context.ProperCaseSettings.ToListAsync();
        }
    }
}