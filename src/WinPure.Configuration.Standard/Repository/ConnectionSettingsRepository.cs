namespace WinPure.Configuration.DependencyInjection;

internal partial class WinPureConfigurationDependency
{
    private class ConnectionSettingsRepository : BaseRepository, IConnectionSettingsRepository
    {
        public ConnectionSettingsRepository(WinPureConfigurationContext context) : base(context)
        {
        }

        public async Task<List<ConnectionSettings>> GetConnectionSettings()
        {
            return await _context.ConnectionSettings
                .Select(x => new ConnectionSettings
                {
                    Id = x.Id,
                    Name = x.Name,
                    SourceType = x.SourceType,
                }).ToListAsync();
        }

        public async Task<List<ConnectionSettings>> GetConnectionSettings(ExternalSourceTypes sourceType)
        {
            return await _context.ConnectionSettings.Where(x => x.SourceType == sourceType)
                .Select(x => new ConnectionSettings
                {
                    Id = x.Id,
                    Name = x.Name,
                    SourceType = x.SourceType,
                }).ToListAsync();
        }

        public async Task<ConnectionSettingEntity> GetConnectionSetting(int id)
        {
            return await _context.ConnectionSettings.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}