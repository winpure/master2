using Newtonsoft.Json;
using WinPure.Common.Services;

namespace WinPure.Configuration.DependencyInjection;

internal partial class WinPureConfigurationDependency
{
    private class ConnectionSettingsService : IConnectionSettingsService
    {
        private readonly IConnectionSettingsRepository _repository;
        private readonly IEncryptionService _encryptionService;

        public ConnectionSettingsService(IConnectionSettingsRepository repository, IEncryptionService encryptionService)
        {
            _repository = repository;
            _encryptionService = encryptionService;
        }

        public List<ConnectionSettings> Get() => AsyncHelpers.RunSync(() => _repository.GetConnectionSettings());

        public List<ConnectionSettings> Get(ExternalSourceTypes sourceType) =>
            AsyncHelpers.RunSync(() => _repository.GetConnectionSettings(sourceType));

        public ConnectionSettings Get<T>(int id)
        {
            var entity = AsyncHelpers.RunSync(() => _repository.GetConnectionSetting(id));
            return new ConnectionSettings
            {
                Id = entity.Id,
                Name = entity.Name,
                SourceType = entity.SourceType,
                Settings = JsonConvert.DeserializeObject<T>(_encryptionService.Decrypt(entity.Settings))
            };
        }

        public int Save(ConnectionSettings settings)
        {
            var entity = AsyncHelpers.RunSync(() => _repository.GetConnectionSetting(settings.Id));
            if (entity == null)
            {
                entity = new ConnectionSettingEntity
                {
                    SourceType = settings.SourceType,
                    Name = settings.Name,
                    Settings = _encryptionService.Encrypt(JsonConvert.SerializeObject(settings.Settings))
                };
                _repository.Add(entity);
            }
            else
            {
                entity.Name = settings.Name;
                entity.Settings = _encryptionService.Encrypt(JsonConvert.SerializeObject(settings.Settings));
            }

            AsyncHelpers.RunSync(() => _repository.SaveChangesAsync());
            return entity.Id;
        }

        public void Delete(int id)
        {
            var entity = AsyncHelpers.RunSync(() => _repository.GetConnectionSetting(id));
            if (entity != null)
            {
                _repository.Delete(entity);
                AsyncHelpers.RunSync(() => _repository.SaveChangesAsync());
            }
        }
    }
}