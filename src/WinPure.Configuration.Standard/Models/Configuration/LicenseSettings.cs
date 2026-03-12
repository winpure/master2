namespace WinPure.Configuration.Models.Configuration
{
    [Serializable]
    internal class LicenseSettings
    {
        public string Path { get; set; }
        public string DesktopLicenseName { get; set; }
        public string AutomationLicenseName { get; set; }
    }
}