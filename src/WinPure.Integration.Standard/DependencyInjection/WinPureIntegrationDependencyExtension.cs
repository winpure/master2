global using System;
global using System.Collections.Generic;
global using System.Data;
global using System.Linq;
global using WinPure.Cleansing.Services;
global using WinPure.Common.Enums;
global using WinPure.Common.Helpers;
global using WinPure.Common.Logger;
global using WinPure.Configuration.Service;
global using WinPure.Integration.Helpers;
global using WinPure.Integration.Models.ImportExportOptions;
global using WinPure.Integration.Properties;
global using WinPure.Integration.Services;
global using WinPure.Licensing.Services;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;


[assembly: InternalsVisibleTo("WinPure.AutomationService")]
[assembly: InternalsVisibleTo("WinPure.DataService")]
[assembly: InternalsVisibleTo("WinPure.API")]
[assembly: InternalsVisibleTo("WinPure.APICore")]
[assembly: InternalsVisibleTo("WinPure.CleanAndMatch")]

namespace WinPure.Integration.DependencyInjection;

internal static partial class WinPureIntegrationDependencyExtension
{
    public static void RegisterIntegration(this ServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IIntegrationService>(sp =>
            new IntegrationService(
                sp.GetRequiredService<IConnectionManager>(),
                sp.GetRequiredService<IWpLogger>(),
                sp.GetRequiredService<ICleansingService>(),
                sp.GetRequiredService<ICleansingConfigurationService>()
            )
        );

    }
}