namespace WinPure.Configuration.Service;

public interface IEntityResolutionMappingSettingService
{
    List<EntityResolutionMappingSetting> GetAll();
    List<EntityResolutionMappingEntity> GetAllEntities();
    void Delete(int id);
    void Add(EntityResolutionMappingSetting setting);
}