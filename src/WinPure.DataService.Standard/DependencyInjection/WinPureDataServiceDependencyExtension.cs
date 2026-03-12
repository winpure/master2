global using WinPure.Common.Logger;
global using WinPure.Configuration.Service;
global using WinPure.DataService.Senzing;
global using WinPure.DataService.Services;
global using WinPure.Licensing.Services;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using WinPure.Common.Constants;
global using WinPure.Common.Exceptions;
global using WinPure.Common.Helpers;
global using WinPure.DataService.Models;
global using WinPure.Common.Models;


using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using WinPure.DataService.AuditLogs;

[assembly: InternalsVisibleTo("Winpure.AutomationService")]
[assembly: InternalsVisibleTo("Winpure.AutomationService.UnitTest")]
[assembly: InternalsVisibleTo("WinPure.ConsoleTest")]
[assembly: InternalsVisibleTo("WinPure.CleanAndMatch")]
[assembly: InternalsVisibleTo("WinPure.Project")]

namespace WinPure.DataService.DependencyInjection;

internal static partial class WinPureDataServiceDependencyExtension
{
    public static void RegisterDataService(this ServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IDataManagerService>(sp =>
            new DataManagerService(
                sp.GetRequiredService<IWpLogger>(),
                sp.GetRequiredService<IConnectionManager>(),
                sp.GetRequiredService<IAuditLogService>(),
                sp.GetRequiredService<ILicenseService>(),
                sp.GetRequiredService<ProjectSettings>()
            )
        );

        serviceCollection.AddTransient<IImportedDataManagementService>(sp =>
            new ImportedDataManagementService(
                sp.GetRequiredService<IConnectionManager>(),
                sp.GetRequiredService<IWpLogger>(),
                sp.GetRequiredService<IConfigurationService>()
            )
        );

        serviceCollection.AddTransient<ISenzingService>(sp =>
            new SenzingService(
                sp.GetRequiredService<IWpLogger>(),
                sp.GetRequiredService<IConfigurationService>(),
                sp.GetRequiredService<IEntityResolutionMappingSettingService>(),
                sp.GetRequiredService<ILicenseService>()
            )
        );
        
        serviceCollection.AddTransient<IAuditLogGenerator>(sp => new AuditLogGenerator(sp.GetRequiredService<IAuditLogService>()));

        serviceCollection.AddSingleton<IAuditLogService>(sp => new AuditLogService(sp.GetRequiredService<ILogConnectionManager>(), sp.GetRequiredService<IWpLogger>()));
    }
}