global using System;
global using System.Linq;
global using Microsoft.Extensions.DependencyInjection;
global using WinPure.Cleansing.Enums;
global using WinPure.Cleansing.Helpers;
global using WinPure.Cleansing.Models;
global using WinPure.Cleansing.Pipeline;
global using WinPure.Cleansing.Pipeline.CleanExecutors;
global using WinPure.Cleansing.Properties;
global using WinPure.Cleansing.Services;
global using WinPure.Common.Exceptions;
global using WinPure.Common.Helpers;
global using WinPure.Common.Logger;
global using WinPure.Configuration.Service;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("WinPure.DataService")]
[assembly: InternalsVisibleTo("WinPure.API")]
[assembly: InternalsVisibleTo("WinPure.CleanAndMatch")]
[assembly: InternalsVisibleTo("WinPure.Integration")]
[assembly: InternalsVisibleTo("WinPure.Project")]
[assembly: InternalsVisibleTo("Winpure.AutomationService")]

namespace WinPure.Cleansing.DependencyInjection;

internal static partial class WinPureCleansingDependencyExtension
{
    public static void RegisterCleansing(this ServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<ICleansingService>(sp =>
            new CleansingService(
                sp.GetRequiredService<IWpLogger>(),
                sp,
                sp.GetRequiredService<IStatisticPatternService>()
            )
        );
        serviceCollection.AddTransient<ICleansingConfigurationService>(sp =>
            new CleansingConfigurationService(
                sp.GetRequiredService<IWpLogger>(),
                sp.GetRequiredService<IConnectionManager>(),
                sp.GetRequiredService<IConfigurationService>()
            )
        );
    }
}