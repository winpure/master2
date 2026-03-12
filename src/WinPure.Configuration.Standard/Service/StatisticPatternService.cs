namespace WinPure.Configuration.DependencyInjection;

internal partial class WinPureConfigurationDependency
{
    private class StatisticPatternService(IStatisticPatternRepository repository) : IStatisticPatternService
    {
        private readonly IStatisticPatternRepository _repository = repository;

        public async Task<List<StatisticPatternSetting>> GetAllPatterns()
        {
            var patterns = await _repository.GetAllPatterns();
            return patterns.Select(x => x.ToSetting()).OrderBy(x => x.Name).ToList();
        }

        public async Task<StatisticPatternSetting> GetPattern(int id)
        {
            var pattern = await _repository.GetPattern(id);
            return pattern.ToSetting();
        }

        public async Task<int> AddOrUpdatePattern(StatisticPatternSetting setting)
        {
            var existingEntity = await _repository.GetPattern(setting.Id);
            if (existingEntity != null)
            {
                var entity = setting.ToEntity();
                existingEntity.Description = entity.Description;
                existingEntity.Name = entity.Name;
                existingEntity.Pattern = entity.Pattern;
                existingEntity.FieldType = entity.FieldType;
            }
            else
            {
                existingEntity = setting.ToEntity();
                _repository.Add(existingEntity);
            }

            await _repository.SaveChangesAsync();
            return existingEntity.Id;
        }

        public async Task Delete(int id)
        {
            var existingEntity = await _repository.GetPattern(id);
            if (existingEntity != null)
            {
                _repository.Delete(existingEntity);
                await _repository.SaveChangesAsync();
            }
        }
    }
}