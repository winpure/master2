using WinPure.Cleansing.DependencyInjection;
using WinPure.Configuration.DependencyInjection;
using WinPure.Matching.DependencyInjection;
using WinPure.Automation.DependencyInjection;
using WinPure.DataService.DependencyInjection;
using WinPure.Integration.DependencyInjection;
using WinPure.Project.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Winpure.AutomationService.UnitTest")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Winpure.AutomationService.DependencyInjection
{
    internal class WinPureAutomationDependencyResolver : WinPureConfigurationDependency
    {
        public new static WinPureConfigurationDependency Instance => Resolver ?? (Resolver = new WinPureAutomationDependencyResolver());
        public override void RegisterDependencies()
        {
            base.RegisterDependencies();
            ServiceCollection.RegisterCleansing();
            ServiceCollection.RegisterMatching();
            ServiceCollection.RegisterAutomation();
            ServiceCollection.RegisterIntegration();
            ServiceCollection.RegisterProject();
            ServiceCollection.RegisterDataService();
        }
    }
}
