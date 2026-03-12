global using WinPure.Licensing.Services;
global using WinPure.Common.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using WinPure.Common.Services;



[assembly: InternalsVisibleTo("WinPure.Cleansing")]
[assembly: InternalsVisibleTo("WinPure.AddressVerification")]
[assembly: InternalsVisibleTo("WinPure.Integration")]
[assembly: InternalsVisibleTo("WinPure.Matching")]
[assembly: InternalsVisibleTo("WinPure.Configuration")]
[assembly: InternalsVisibleTo("WinPure.Project")]
[assembly: InternalsVisibleTo("WinPure.DataService")]
[assembly: InternalsVisibleTo("WinPure.API")]
[assembly: InternalsVisibleTo("WinPure.API.Core")]
[assembly: InternalsVisibleTo("WinPure.CleanAndMatch")]
[assembly: InternalsVisibleTo("Winpure.AutomationService")]

namespace WinPure.Licensing.DependencyInjection;

internal partial class WinPureLicensingDependency : WinPureCommonDependency
{
    public override void RegisterDependencies()
    {
        base.RegisterDependencies();
        ServiceCollection.AddTransient<IValueStore>(_ => new FileStore());

        ServiceCollection.AddTransient<ILicenseService>(provider =>
        {
            var encryptionService = provider.GetRequiredService<IEncryptionService>();
            var valueStore = provider.GetRequiredService<IValueStore>();
            return new LicenseService(encryptionService, valueStore);
        });
    }
}