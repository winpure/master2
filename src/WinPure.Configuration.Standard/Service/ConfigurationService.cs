using Newtonsoft.Json;
using System.IO;
using WinPure.Common.Exceptions;
using WinPure.Common.Logger;
using WinPure.Configuration.Enums;

namespace WinPure.Configuration.DependencyInjection;

internal partial class WinPureConfigurationDependency
{
    private class ConfigurationService : IConfigurationService
    {
        private static string AppSettingsUiFile = @"C:\ProgramData\WinPure\Clean & Match v11\UiSettings.json";
        private static string AppSettingsBaseFile = @"appsettings.json";
        private ProgramType _programType;
        private readonly string _settingsFileName;

        public event Action OnConfigurationChanged;

        public WinPureConfiguration Configuration { get; private set; }
        public string ConfigurationFilePath => _settingsFileName;

        public ConfigurationService()
        {
            //System.Diagnostics.Debugger.Launch();
            _settingsFileName = AppSettingsBaseFile;
            var currentLocation = AssemblyHelper.GetCurrentLocation();
            if (currentLocation.Contains("Program Files"))
            {
                if (!File.Exists(AppSettingsUiFile))
                {
                    File.Copy(AppSettingsBaseFile, AppSettingsUiFile);
                }

                _settingsFileName = AppSettingsUiFile;
            }

            var configurationFile = FileHelper.ReadFile(_settingsFileName);
            Configuration = JsonConvert.DeserializeObject<WinPureConfiguration>(configurationFile);
        }

        public void Initiate(ProgramType programType)
        {
            _programType = programType;
            CheckDefaultValues();
        }

        public void SaveConfiguration()
        {
            var options = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var configurationFile = JsonConvert.SerializeObject(Configuration, options);
            File.WriteAllText(_settingsFileName, configurationFile);
            OnConfigurationChanged?.Invoke();
        }

        public void DefineProjectDatabases(IWpLogger logger)
        {
            DefineDatabaseFile(Configuration.ProjectDatabase, logger);
            DefineDatabaseFile(Configuration.LogDatabase, logger);
        }

        private void DefineDatabaseFile(DatabaseSetting configuration, IWpLogger logger)
        {
            var projectFilePath = SystemDatabaseConnectionHelper.GetDatabasePath(configuration.UseRelativePath, configuration.Path, configuration.Name);
            int i = 1;
            while (!FileHelper.SafeDeleteFileWithLogging(logger, projectFilePath, "Cannot delete DB file on startup"))
            {
                var fileName = Path.GetFileNameWithoutExtension(configuration.Name);
                var extension = Path.GetExtension(configuration.Name);
                configuration.Name = fileName + i++ + extension;
                projectFilePath = SystemDatabaseConnectionHelper.GetDatabasePath(configuration.UseRelativePath, configuration.Path, configuration.Name);
            }
        }

        private void CheckDefaultValues()
        {
            var wasChanged = false;

            if (Configuration.License == null)
            {
                Configuration.License = new LicenseSettings
                {
                    Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WinPure", GetWinPureSubfolder())
                };
                wasChanged = true;
            }

            if (string.IsNullOrEmpty(Configuration.License.Path))
            {
                Configuration.License.Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WinPure", GetWinPureSubfolder());
                wasChanged = true;
            }

            if (string.IsNullOrWhiteSpace(Configuration.License.DesktopLicenseName))
            {
                Configuration.License.DesktopLicenseName = String.Empty;
                wasChanged = true;
            }

            if (Configuration.ProjectDatabase == null)
            {
                Configuration.ProjectDatabase = new DatabaseSetting
                {
                    Name = "WinPureCMDB.db",
                    UseRelativePath = false,
                    Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WinPure", GetWinPureSubfolder(), "DB")
                };
                wasChanged = true;
            }

            if (Configuration.LogDatabase == null)
            {
                Configuration.LogDatabase = new DatabaseSetting
                {
                    Name = "WinPureCMDB_Log.db",
                    UseRelativePath = false,
                    Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WinPure", GetWinPureSubfolder(), "DB")
                };
                wasChanged = true;
            }

            if (Configuration.SystemDatabase == null)
            {
                Configuration.ProjectDatabase = new DatabaseSetting
                {
                    Name = "SystemDb.db",
                    UseRelativePath = false,
                    Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WinPure", GetWinPureSubfolder(), "SystemDb")
                };
                wasChanged = true;
            }

            if (string.IsNullOrWhiteSpace(Configuration.SampleFolderPath))
            {
                Configuration.SampleFolderPath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WinPure", GetWinPureSubfolder(), "Sample Files");
                wasChanged = true;
            }

            if (string.IsNullOrWhiteSpace(Configuration.WordManagerSamplePath))
            {
                Configuration.WordManagerSamplePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WinPure", GetWinPureSubfolder(), "Word Manager Files");
                wasChanged = true;
            }

            if (string.IsNullOrWhiteSpace(Configuration.NewProjectSamplePath))
            {
                Configuration.NewProjectSamplePath = Path.Combine(GetUserDocumentFolder(), "WinPure", GetWinPureSubfolder(), "Projects");
                wasChanged = true;
            }

            if (string.IsNullOrWhiteSpace(Configuration.LogPath))
            {
                Configuration.LogPath = Path.Combine(GetUserDocumentFolder(), "WinPure", GetWinPureSubfolder(), "ErrorLog", "WpErrorLog.log");
                wasChanged = true;
            }

            if (string.IsNullOrWhiteSpace(Configuration.MatchReportPath))
            {
                Configuration.MatchReportPath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WinPure", GetWinPureSubfolder(), "Reports", "MatchReport.repx");
                wasChanged = true;
            }

            if (string.IsNullOrWhiteSpace(Configuration.AddressReportPath))
            {
                Configuration.AddressReportPath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WinPure", GetWinPureSubfolder(), "Reports", "VerificationReport.repx");
                wasChanged = true;
            }
            
            if (string.IsNullOrWhiteSpace(Configuration.AuditLogReportPath))
            {
                Configuration.AuditLogReportPath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WinPure", GetWinPureSubfolder(), "Reports", "AuditLogReport.repx");
                wasChanged = true;
            }

            string rootDrive = Path.GetPathRoot(Environment.SystemDirectory);

            if (string.IsNullOrWhiteSpace(Configuration.UsAddressVerificationDataFolder))
            {
                Configuration.UsAddressVerificationDataFolder = Path.Combine(rootDrive, "WinPure", "AddressData");
                wasChanged = true;
            }

            if (Configuration.ErSettings == null)
            {
                Configuration.ErSettings = new EntityResolutionSetting
                {
                    Database = EntityResolutionDatabase.Internal,
                    DataFolder = Path.Combine(rootDrive, "WinPure", "ER"),
                    MaxDegreeOfParallelism = 6
                };
                wasChanged = true;
            }
            else if (string.IsNullOrWhiteSpace(Configuration.ErSettings.DataFolder))
            {
                Configuration.ErSettings.DataFolder = Path.Combine(rootDrive, "WinPure", "ER");
                wasChanged = true;
            }

            if (wasChanged)
            {
                SaveConfiguration();
            }
        }

        public string ProgramInstallationFolder => Path.GetDirectoryName(AssemblyHelper.GetCurrentLocation());

        private string GetWinPureSubfolder()
        {
            switch (_programType)
            {
                case ProgramType.Api:
                    return "API";
                case ProgramType.CamEnt:
                case ProgramType.CamEntLite:
                case ProgramType.CamEntAd:
                case ProgramType.CamLte:
                case ProgramType.CamBiz:
                case ProgramType.CamFree:
                    return "Clean & Match V11";
                default:
                    throw new WinPureArgumentException($"Program type {_programType} not defined");
            }
        }

        private static string GetUserDocumentFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        }
    }
}