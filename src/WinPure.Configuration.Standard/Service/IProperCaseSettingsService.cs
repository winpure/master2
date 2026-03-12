namespace WinPure.Configuration.Service
{
    public interface IProperCaseSettingsService
    {
        ProperCaseConfiguration GetProperCaseSettings();
        void SaveProperCaseSettings(ProperCaseConfiguration settings);
    }
}