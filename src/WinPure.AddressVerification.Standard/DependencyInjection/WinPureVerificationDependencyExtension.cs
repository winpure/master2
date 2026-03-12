global using System;
global using System.Data;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using Microsoft.Extensions.DependencyInjection;
global using WinPure.AddressVerification.Services;
global using WinPure.Common.Exceptions;
global using WinPure.Common.Helpers;
global using WinPure.Common.Logger;
global using WinPure.Configuration.Service;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("WinPure.ConsoleTest")]
[assembly: InternalsVisibleTo("WinPure.API.Core")]
[assembly: InternalsVisibleTo("WinPure.CleanAndMatch")]
[assembly: InternalsVisibleTo("WinPure.DataService")]

namespace WinPure.AddressVerification.DependencyInjection;

internal static partial class WinPureVerificationDependencyExtension
{
    public static void RegisterVerification(this ServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IUsAddressVerificationOnlineService>(sp =>
            new UsAddressVerificationOnlineService(
                sp.GetRequiredService<IWpLogger>()
            )
        );

        serviceCollection.AddTransient<IAddressVerificationService>(sp =>
            new AddressVerificationService(
                sp.GetRequiredService<IConfigurationService>(),
                sp.GetRequiredService<IConnectionManager>(),
                sp.GetRequiredService<IWpLogger>(),
                sp.GetRequiredService<IUsAddressVerificationOnlineService>()
            )
        );

    }
}