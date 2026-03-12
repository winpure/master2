using Newtonsoft.Json;

namespace WinPure.Configuration.DependencyInjection;

internal partial class WinPureConfigurationDependency
{
    private class ProperCaseSettingsService : IProperCaseSettingsService
    {
        private readonly IProperCaseSettingsRepository _repository;

        public ProperCaseSettingsService(IProperCaseSettingsRepository repository)
        {
            _repository = repository;
        }

        public ProperCaseConfiguration GetProperCaseSettings()
        {
            var entities = AsyncHelpers.RunSync(() => _repository.GetProperCaseSettings());

            var separators = entities.FirstOrDefault(x => x.Name == ProperCaseNameHelper.Delimiters);
            var exceptions = entities.FirstOrDefault(x => x.Name == ProperCaseNameHelper.Exceptions);
            var prefix = entities.FirstOrDefault(x => x.Name == ProperCaseNameHelper.Prefix);

            var result = new ProperCaseConfiguration();

            result.Delimeters = separators != null ? separators.Value : " ";
            
            result.ExceptionList = exceptions == null || string.IsNullOrEmpty(exceptions.Value)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(exceptions.Value);

            result.PrefixList = prefix == null || string.IsNullOrEmpty(prefix.Value)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(prefix.Value);

            return result;
        }

        public void SaveProperCaseSettings(ProperCaseConfiguration settings)
        {
            var entities = AsyncHelpers.RunSync(() => _repository.GetProperCaseSettings());

            if (entities.Any(x => x.Name == ProperCaseNameHelper.Delimiters))
            {
                entities.First(x => x.Name == ProperCaseNameHelper.Delimiters).Value = settings.Delimeters;
            }
            else
            {
                _repository.Add(new ProperCaseSettingEntity
                {
                    Name = ProperCaseNameHelper.Delimiters,
                    Value = settings.Delimeters
                });
            }

            if (entities.Any(x => x.Name == ProperCaseNameHelper.Exceptions))
            {
                entities.First(x => x.Name == ProperCaseNameHelper.Exceptions).Value = JsonConvert.SerializeObject(settings.ExceptionList);
            }
            else
            {
                _repository.Add(new ProperCaseSettingEntity
                {
                    Name = ProperCaseNameHelper.Exceptions,
                    Value = JsonConvert.SerializeObject(settings.ExceptionList)
                });
            }

            if (entities.Any(x => x.Name == ProperCaseNameHelper.Prefix))
            {
                entities.First(x => x.Name == ProperCaseNameHelper.Prefix).Value = JsonConvert.SerializeObject(settings.PrefixList);
            }
            else
            {
                _repository.Add(new ProperCaseSettingEntity
                {
                    Name = ProperCaseNameHelper.Prefix,
                    Value = JsonConvert.SerializeObject(settings.PrefixList)
                });
            }

            AsyncHelpers.RunSync(() => _repository.SaveChangesAsync());
        }
    }
}