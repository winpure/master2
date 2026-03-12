using WinPure.Configuration.Enums;

namespace WinPure.Configuration.DependencyInjection;

internal partial class WinPureConfigurationDependency
{
    private class LogConnectionManager : ConnectionManager, ILogConnectionManager
    {
        public LogConnectionManager()
        {
            DbKind = DatabaseKind.Log;
        }

        [ActivatorUtilitiesConstructor]
        public LogConnectionManager(IServiceProvider serviceProvider)
        {
            DbKind = DatabaseKind.Log;
            Configure(serviceProvider);
        }
    }
}
