namespace WinPure.Configuration.DependencyInjection;

internal partial class WinPureConfigurationDependency
{
    internal class CleansingAiConfigurationService : ICleansingAiConfigurationService
    {
        private readonly ICleansingAiConfigurationRepository _repository;

        public CleansingAiConfigurationService(ICleansingAiConfigurationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CleansingAiFieldType>> GetAllConfigurations()
        {
            var entities = await _repository.GetCleansingAiConfigurationsAsync();
            return entities.Select(x => x.ToModel()).ToList();
        }

        public async Task<CleansingAiFieldType> GetConfigurationByType(string aiType)
        {
            if (string.IsNullOrWhiteSpace(aiType)) return null;
            var entity = await _repository.GetByNameAsync(aiType);
            return entity.ToModel();
        }

        public async Task AddOrUpdate(CleansingAiFieldType configuration)
        {
            if (configuration == null || string.IsNullOrWhiteSpace(configuration.AiType)) return;

            var existing = await _repository.GetByNameAsync(configuration.AiType);
            if (existing != null)
            {
                existing.Options = configuration.Options ?? new CleanAiFieldOptions();
                existing.MappedFields = configuration.MappedFields ?? new List<CleanAiMappedField>();
            }
            else
            {
                existing = new CleansingAiConfigurationEntity
                {
                    AiType = configuration.AiType,
                    Options = configuration.Options ?? new CleanAiFieldOptions(),
                    MappedFields = configuration.MappedFields ?? new List<CleanAiMappedField>()
                };
                _repository.Add(existing);
            }

            await _repository.SaveChangesAsync();
        }

        public async Task<bool> Delete(string aiType)
        {
            if (string.IsNullOrWhiteSpace(aiType)) return false;
            var existing = await _repository.GetByNameAsync(aiType);
            if (existing == null) return false;
            _repository.Delete(existing);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task SyncAiTypes(List<CleansingAiFieldType> aiTypes)
        {
            // Load current from repository
            var current = await _repository.GetCleansingAiConfigurationsAsync();
            var incomingLookup = aiTypes?.Where(x => !string.IsNullOrWhiteSpace(x.AiType))
                                         .GroupBy(x => x.AiType.Trim(), StringComparer.InvariantCultureIgnoreCase)
                                         .ToDictionary(g => g.Key, g => g.First());

            // Remove missing
            foreach (var existing in current)
            {
                if (incomingLookup == null || !incomingLookup.ContainsKey(existing.AiType))
                {
                    _repository.Delete(existing);
                }
            }

            // Add or update
            if (incomingLookup != null)
            {
                foreach (var kv in incomingLookup)
                {
                    var incoming = kv.Value;
                    var match = current.FirstOrDefault(x => string.Equals(x.AiType, kv.Key, StringComparison.InvariantCultureIgnoreCase));
                    if (match == null)
                    {
                        _repository.Add(new CleansingAiConfigurationEntity
                        {
                            AiType = incoming.AiType,
                            Options = incoming.Options ?? new CleanAiFieldOptions(),
                            MappedFields = incoming.MappedFields ?? new List<CleanAiMappedField>()
                        });
                    }
                    else
                    {
                        match.Options = incoming.Options ?? new CleanAiFieldOptions();
                        match.MappedFields = incoming.MappedFields ?? new List<CleanAiMappedField>();
                    }
                }
            }

            await _repository.SaveChangesAsync();
        }
    }
}
