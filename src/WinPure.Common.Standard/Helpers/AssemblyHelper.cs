using System.Reflection;

namespace WinPure.Common.Helpers
{
    internal static class AssemblyHelper
    {
        public static string GetCurrentLocation()
        {
            return Assembly.GetEntryAssembly().Location;
        }

        public static Version ApplicationVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version;
        }
    }
}