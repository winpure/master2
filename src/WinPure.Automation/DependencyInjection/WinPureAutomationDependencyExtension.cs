global using System.Runtime.CompilerServices;
global using WinPure.Automation.Repository;
global using WinPure.Automation.Services;
global using System;
global using System.Collections.Generic;
global using WinPure.Automation.Models;
global using WinPure.Common.Logger;

using Microsoft.Extensions.DependencyInjection;
using WinPure.Configuration.Infrastructure;

[assembly: InternalsVisibleTo("WinPure.CleanAndMatch")]
[assembly: InternalsVisibleTo("WinPure.DataService.Standard")]
[assembly: InternalsVisibleTo("Winpure.AutomationService")]
[assembly: InternalsVisibleTo("Winpure.AutomationService.UnitTest")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace WinPure.Automation.DependencyInjection;

internal static partial class WinPureAutomationDependencyExtension
{
    public static void RegisterAutomation(this ServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IAutomationService>(sp =>
            new AutomationService(
                sp.GetRequiredService<IAutomationRepository>(),
                sp.GetRequiredService<IWpLogger>()
            )
        );
        serviceCollection.AddTransient<IAutomationRepository>(sp =>
            new AutomationRepository(
                sp.GetRequiredService<WinPureConfigurationContext>()
            )
        );

    }
}