namespace WinPure.Configuration.DependencyInjection;

internal partial class WinPureConfigurationDependency
{
    private class EntityResolutionMappingSettingService : IEntityResolutionMappingSettingService
    {
        private readonly IEntityResolutionMappingRepository _repository;

        public EntityResolutionMappingSettingService(IEntityResolutionMappingRepository repository)
        {
            _repository = repository;
        }

        public List<EntityResolutionMappingSetting> GetAll()
        {
            var entities = AsyncHelpers.RunSync(_repository.GetAll);
            return entities.Select(x => x.ToSettings()).ToList();
        }

        public List<EntityResolutionMappingEntity> GetAllEntities()
        {
            var entities = AsyncHelpers.RunSync(_repository.GetAll);
            return entities;
        }

        public void Delete(int id)
        {
            var entity = AsyncHelpers.RunSync(() => _repository.Get(id));
            if (entity != null)
            {
                _repository.Delete(entity);
                AsyncHelpers.RunSync(() => _repository.SaveChangesAsync());
            }
        }

        public void Add(EntityResolutionMappingSetting setting)
        {
            var result = GetAll();
            var newEntity = setting.ToEntity();
            if (result.Any(x =>
                    string.Compare(x.DataColumnName, newEntity.DataColumnName, StringComparison.OrdinalIgnoreCase) ==
                    0))
            {
                return;
            }

            _repository.Add(newEntity);
            AsyncHelpers.RunSync(() => _repository.SaveChangesAsync());
        }
    }
}