global using System;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.Data;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Threading.Tasks.Dataflow;
global using com.loqate;
global using WinPure.AddressVerification.Helpers;
global using WinPure.AddressVerification.Models;
global using WinPure.AddressVerification.Properties;
global using WinPure.AddressVerification.Services;
global using WinPure.Common.Exceptions;
global using WinPure.Common.Helpers;
global using WinPure.Common.Logger;
global using WinPure.Configuration.Service;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("WinPure.API")]
[assembly: InternalsVisibleTo("WinPure.CleanAndMatch")]
[assembly: InternalsVisibleTo("WinPure.DataService")]

namespace WinPure.AddressVerification.DependencyInjection;

internal static partial class WinPureVerificationDependencyExtension
{
    public static void RegisterVerification(this ServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IUsAddressVerificationOnlineService>(sp =>
            new UsAddressVerificationOnlineService(sp.GetRequiredService<IWpLogger>())
        );

        serviceCollection.AddTransient<IUsAddressVerificationService>(sp =>
            new UsAddressVerificationService(sp.GetRequiredService<IWpLogger>())
        );

        serviceCollection.AddTransient<IUsAddressVerificationLocalService>(sp =>
            new UsAddressVerificationLocalService(sp.GetRequiredService<IWpLogger>())
        );

        serviceCollection.AddTransient<IUsAddressVerificationInDatabaseService>(sp => 
            new UsAddressVerificationInDatabaseService(sp.GetRequiredService<IWpLogger>()
            )
        );
        serviceCollection.AddTransient<IAddressVerificationService>(sp =>
            new AddressVerificationService(
                sp.GetRequiredService<IConfigurationService>(),
                sp.GetRequiredService<IConnectionManager>(),
                sp.GetRequiredService<IWpLogger>(),
                sp.GetRequiredService<IUsAddressVerificationLocalService>(),
                sp.GetRequiredService<IUsAddressVerificationInDatabaseService>(),
                sp.GetRequiredService<IUsAddressVerificationOnlineService>(),
                sp.GetRequiredService<IUsAddressVerificationService>()
            )
        );

    }
}