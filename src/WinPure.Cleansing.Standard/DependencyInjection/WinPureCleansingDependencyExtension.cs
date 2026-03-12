global using System;
global using System.Collections.Generic;
global using System.Data;
global using System.Linq;
global using Microsoft.Extensions.DependencyInjection;
global using WinPure.Cleansing.Models;
global using WinPure.Cleansing.Properties;
global using WinPure.Cleansing.Services;
global using WinPure.Common.Exceptions;
global using WinPure.Common.Helpers;
global using WinPure.Common.Logger;
global using WinPure.Common.Models;
global using WinPure.Configuration.Service;

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("WinPure.ConsoleTest")]
[assembly: InternalsVisibleTo("WinPure.API.Core")]
[assembly: InternalsVisibleTo("WinPure.CleanAndMatch")]
[assembly: InternalsVisibleTo("WinPure.DataService")]
[assembly: InternalsVisibleTo("WinPure.Integration")]

namespace WinPure.Cleansing.DependencyInjection;

internal static partial class WinPureCleansingDependencyExtension
{
    public static void RegisterCleansing(this ServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<ICleansingService>(sp =>
            new CleansingService(
                sp.GetRequiredService<IWpLogger>(),
                sp
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