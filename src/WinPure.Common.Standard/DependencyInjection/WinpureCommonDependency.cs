global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using System;
global using System.Linq;
global using WinPure.Common.Logger;
global using WinPure.Common.Services;
global using WinPure.Common.Enums;
using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("WinPure.Automation")]
[assembly: InternalsVisibleTo("WinPure.Cleansing")]
[assembly: InternalsVisibleTo("WinPure.AddressVerification")]
[assembly: InternalsVisibleTo("WinPure.Licensing")]
[assembly: InternalsVisibleTo("WinPure.Configuration")]
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

namespace WinPure.Common.DependencyInjection;

internal partial class WinPureCommonDependency : IDisposable
{
    private ServiceProvider _serviceProvider;
    protected ServiceCollection ServiceCollection;
    protected IConfiguration Configuration;

    public IServiceProvider ServiceProvider => _serviceProvider;

    public WinPureCommonDependency()
    {
        if (!File.Exists("appsettings.json"))
        {
            File.WriteAllLines("appsettings.json", new[] { "{\"Database\": {\"UseRelativePath\": true,\"Path\": \"DB\",\"Name\": \"SystemDb.db\"}}" });
        }

        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        // Simple DI setup
        ServiceCollection = new ServiceCollection();

        RegisterDependencies();

        _serviceProvider = ServiceCollection.BuildServiceProvider();
    }

    public void Replace<TService>(TService instance)
        where TService : class
    {
        var descriptorToRemove = ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(TService));

        if (descriptorToRemove != null)
        {
            ServiceCollection.Remove(descriptorToRemove);
        }

        ServiceCollection.AddSingleton(instance);

        _serviceProvider = ServiceCollection.BuildServiceProvider();
    }

    public virtual void RegisterDependencies()
    {
        ServiceCollection.AddTransient<IWpLogger>(_ => new WpNLogger());
        ServiceCollection.AddTransient<IEncryptionService>(_ => new EncryptionService());
    }

    public void Dispose()
    {
        _serviceProvider?.Dispose();
    }
}