global using WinPure.Common.Logger;
global using WinPure.Configuration.Infrastructure;
global using WinPure.Configuration.Service;
global using WinPure.DataService.Models;
global using WinPure.DataService.Services;
global using WinPure.Project.Services;
global using System;
global using System.Data;


using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using WinPure.DataService.AuditLogs;

[assembly: InternalsVisibleTo("WinPure.CleanAndMatch")]
[assembly: InternalsVisibleTo("WinPure.Automation")]
[assembly: InternalsVisibleTo("Winpure.AutomationService")]

namespace WinPure.Project.DependencyInjection;

internal static partial class WinPureProjectDependencyExtension
{
    public static void RegisterProject(this ServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IRecentProjectService>(sp =>
            new RecentProjectService(
                sp.GetRequiredService<WinPureConfigurationContext>()
            )
        );

        serviceCollection.AddSingleton<IProjectMigrationService>(_ => new ProjectMigrationService());

        serviceCollection.AddSingleton<IProjectService>(sp =>
            new ProjectService(
                sp.GetRequiredService<IWpLogger>(),
                sp.GetRequiredService<IRecentProjectService>(),
                sp.GetRequiredService<IDataManagerService>(),
                sp.GetRequiredService<IConnectionManager>(),
                sp.GetRequiredService<ILogConnectionManager>(),
                sp.GetRequiredService<IAuditLogService>(),
                sp.GetRequiredService<ProjectSettings>(),
                sp.GetRequiredService<IProjectMigrationService>()
            )
        );

    }
}