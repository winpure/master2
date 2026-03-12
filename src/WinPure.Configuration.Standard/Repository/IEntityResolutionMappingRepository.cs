namespace WinPure.Configuration.Repository;

internal interface IEntityResolutionMappingRepository : IBaseRepository
{
    Task<List<EntityResolutionMappingEntity>> GetAll();
    Task<EntityResolutionMappingEntity> Get(int id);
    Task<EntityResolutionMappingEntity> Get(string dataColumnName);
}