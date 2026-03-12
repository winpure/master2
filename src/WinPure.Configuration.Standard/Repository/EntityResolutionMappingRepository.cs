namespace WinPure.Configuration.DependencyInjection;

internal partial class WinPureConfigurationDependency
{
    private class EntityResolutionMappingRepository : BaseRepository, IEntityResolutionMappingRepository
    {
        public EntityResolutionMappingRepository(WinPureConfigurationContext context) : base(context)
        {
        }

        public async Task<List<EntityResolutionMappingEntity>> GetAll()
        {
            return await _context.EntityResolutionMapping.ToListAsync();
        }

        public async Task<EntityResolutionMappingEntity> Get(int id)
        {
            return await _context.EntityResolutionMapping.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<EntityResolutionMappingEntity> Get(string dataColumnName)
        {
            return await _context.EntityResolutionMapping.FirstOrDefaultAsync(x => x.DataColumnName == dataColumnName);
        }
    }
}