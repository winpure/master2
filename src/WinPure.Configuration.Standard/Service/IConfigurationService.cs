using WinPure.Common.Logger;

namespace WinPure.Configuration.Service
{
    internal interface IConfigurationService
    {
        event Action OnConfigurationChanged;
        WinPureConfiguration Configuration { get; }
        string ConfigurationFilePath { get; }
        void Initiate(ProgramType programType);
        void DefineProjectDatabases(IWpLogger logger);
        string ProgramInstallationFolder { get; }
        void SaveConfiguration();
    }
}