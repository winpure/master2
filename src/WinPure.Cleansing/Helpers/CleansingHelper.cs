using LibPostalNet;
using WinPure.Configuration.DependencyInjection;

namespace WinPure.Cleansing.Helpers;

internal static class CleansingHelper
{
    internal static void LibpostalTeardown()
    {
        var configuration = WinPureConfigurationDependency.Resolve<IConfigurationService>().Configuration;
        if (configuration.LibpostalInitialized)
        {
            libpostal.LibpostalTeardown();
            libpostal.LibpostalTeardownParser();
            libpostal.LibpostalTeardownLanguageClassifier();

            configuration.LibpostalInitialized = false;
            GC.Collect();
        }
    }
}