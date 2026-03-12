global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using WinPure.Common.Enums;
global using WinPure.Common.Helpers;
global using WinPure.Configuration.Helper;
global using WinPure.Configuration.Infrastructure;
global using WinPure.Configuration.Models.Configuration;
global using WinPure.Configuration.Models.Database;
global using WinPure.Configuration.Repository;
global using WinPure.Configuration.Service;
global using WinPure.Licensing.DependencyInjection;
using System.Runtime.CompilerServices;
using WinPure.Common.Services;
using WinPure.Licensing.Services;

[assembly: InternalsVisibleTo("WinPure.Automation")]
[assembly: InternalsVisibleTo("WinPure.Cleansing")]
[assembly: InternalsVisibleTo("WinPure.AddressVerification")]
[assembly: InternalsVisibleTo("WinPure.Integration")]
[assembly: InternalsVisibleTo("WinPure.Matching")]
[assembly: InternalsVisibleTo("WinPure.Project")]
[assembly: InternalsVisibleTo("WinPure.DataService")]
[assembly: InternalsVisibleTo("WinPure.API")]
[assembly: InternalsVisibleTo("WinPure.API.Core")]
[assembly: InternalsVisibleTo("WinPure.CleanAndMatch")]
[assembly: InternalsVisibleTo("Winpure.AutomationService")]
[assembly: InternalsVisibleTo("Winpure.AutomationService.UnitTest")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace WinPure.Configuration.DependencyInjection;

internal partial class WinPureConfigurationDependency : WinPureLicensingDependency
{
    internal static WinPureConfigurationDependency Resolver;

    public static WinPureConfigurationDependency Instance => Resolver ??= new WinPureConfigurationDependency();

    public static T Resolve<T>()
    {
        Type typeParameterType = typeof(T);
        return (T)Instance.ServiceProvider.GetService(typeParameterType);
    }

    public override void RegisterDependencies()
    {
        base.RegisterDependencies();
        ServiceCollection.AddDbContext<WinPureConfigurationContext>(o =>
        {
            var path = SystemDatabaseConnectionHelper.GetDatabasePath(
                Configuration["SYSTEMDATABASE:USERELATIVEPATH"] == null
                    ? false
                    : Convert.ToBoolean(Configuration["SYSTEMDATABASE:USERELATIVEPATH"]),
                string.IsNullOrEmpty(Configuration["SYSTEMDATABASE:PATH"])
                    ? "DB"
                    : Configuration["SYSTEMDATABASE:PATH"],
                string.IsNullOrEmpty(Configuration["SYSTEMDATABASE:NAME"])
                    ? "SystemDb.db"
                    : Configuration["SYSTEMDATABASE:NAME"]);

            var connectionString = SystemDatabaseConnectionHelper.GetConnectionString(path);
            o.UseSqlite(connectionString);
        }, ServiceLifetime.Transient, ServiceLifetime.Transient);

        ServiceCollection.AddSingleton<IConnectionManager>(provider =>
            ActivatorUtilities.CreateInstance<ConnectionManager>(provider));

        ServiceCollection.AddSingleton<ILogConnectionManager>(provider =>
            ActivatorUtilities.CreateInstance<LogConnectionManager>(provider));

        ServiceCollection.AddTransient<IDictionaryRepository>(sp =>
            new DictionaryRepository(sp.GetRequiredService<WinPureConfigurationContext>()));

        ServiceCollection.AddTransient<IDictionaryService>(sp =>
            new DictionaryService(sp.GetRequiredService<IDictionaryRepository>()));

        ServiceCollection.AddSingleton<ILanguageService>(sp =>
            new LanguageService(sp.GetRequiredService<IValueStore>()));

        ServiceCollection.AddSingleton<IConfigurationService>(_ =>
            new ConfigurationService());

        ServiceCollection.AddTransient<IProperCaseSettingsRepository>(sp =>
            new ProperCaseSettingsRepository(sp.GetRequiredService<WinPureConfigurationContext>()));

        ServiceCollection.AddTransient<IProperCaseSettingsService>(sp =>
            new ProperCaseSettingsService(sp.GetRequiredService<IProperCaseSettingsRepository>()));

        ServiceCollection.AddTransient<IConnectionSettingsRepository>(sp =>
            new ConnectionSettingsRepository(sp.GetRequiredService<WinPureConfigurationContext>()));

        ServiceCollection.AddTransient<IConnectionSettingsService>(sp =>
            new ConnectionSettingsService(
                sp.GetRequiredService<IConnectionSettingsRepository>(),
                sp.GetRequiredService<IEncryptionService>()
            )
        );

        ServiceCollection.AddTransient<IEntityResolutionMappingRepository>(sp =>
            new EntityResolutionMappingRepository(sp.GetRequiredService<WinPureConfigurationContext>()));

        ServiceCollection.AddTransient<IEntityResolutionMappingSettingService>(sp =>
            new EntityResolutionMappingSettingService(sp.GetRequiredService<IEntityResolutionMappingRepository>()));

        ServiceCollection.AddTransient<IStatisticPatternRepository>(sp =>
            new StatisticPatternRepository(sp.GetRequiredService<WinPureConfigurationContext>()));

        ServiceCollection.AddTransient<IStatisticPatternService>(sp =>
            new StatisticPatternService(sp.GetRequiredService<IStatisticPatternRepository>()));

        ServiceCollection.AddTransient<ICleansingAiConfigurationRepository>(sp =>
            new CleansingAiConfigurationRepository(sp.GetRequiredService<WinPureConfigurationContext>()));

        ServiceCollection.AddTransient<ICleansingAiConfigurationService>(sp =>
            new CleansingAiConfigurationService(sp.GetRequiredService<ICleansingAiConfigurationRepository>()));

        ServiceCollection.AddTransient<IFavoritesRepository>(sp =>
            new FavoritesRepository(sp.GetRequiredService<WinPureConfigurationContext>()));

        ServiceCollection.AddTransient<IExternalSourceService>(sp =>
            new ExternalSourceService(sp.GetRequiredService<IFavoritesRepository>()));
    }
}