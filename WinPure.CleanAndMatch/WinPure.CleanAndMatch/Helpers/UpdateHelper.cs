using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace WinPure.CleanAndMatch.Helpers;

internal static class UpdateHelper
{
    internal static void CheckForUpdate(ProgramType programType, IWpLogger logger)
    {
        var newVersion = UpdateHelper.RequireUpdate(programType, logger);
        if (!string.IsNullOrEmpty(newVersion))
        {
            if (MessageBox.Show(string.Format(Resources.MESSAGE_DOWNLOADNEWVERSION, newVersion),
                    Resources.MESSAGE_QUESTION_CAPTION,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information) == DialogResult.Yes)
            {
                UpdateHelper.OpenDownloadPage(Program.CurrentProgramVersion);
            }
        }
        else
        {
            MessageBox.Show(Resources.MESSAGE_VERSIONUPTODATE, Resources.UI_CAPTION_CHECKUPDATE, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    internal static string RequireUpdate(ProgramType programType, IWpLogger logger)
    {
        try
        {
            var latestVersion = GetLatestVersion(programType);
            var latest = new Version(latestVersion);
            var currentVersion = Common.Helpers.AssemblyHelper.ApplicationVersion();
            logger.Information($"Check a varsion for {programType}: current: {currentVersion},  Latest: {latest}");
            return latest > currentVersion ? latest.ToString() : String.Empty;
        }
        catch (Exception e)
        {
            logger.Error("Cannot get the latest version", e);
            return String.Empty;
        }
    }

    internal static void OpenDownloadPage(ProgramType programType)
    {
        var url = string.Empty;

        var licenseService = WinPureUiDependencyResolver.Resolve<ILicenseService>();
        if (licenseService.IsDemo)
        {
            url = "https://winpure.com/latest-release-cam-enterprise-demo/";
        }
        else
        {
            url = programType switch
            {
                ProgramType.CamEntAd => "https://winpure.com/latest-release-cam-enterprise-av/",
                ProgramType.CamEnt => "https://winpure.com/latest-release-cam-enterprise/",
                ProgramType.CamEntLite => "https://winpure.com/latest-release-cam-enterprise/",
                ProgramType.CamFree => "https://winpure.com/latest-release-cam-community/",
                ProgramType.CamLte => "https://winpure.com/latest-release-cam-small-business/",
                ProgramType.CamBiz => "https://winpure.com/latest-release-cam-pro-business/",
                _ => throw new NotSupportedException()
            };
        }

        if (!string.IsNullOrEmpty(url))
        {
            System.Diagnostics.Process.Start(url);
        }
    }

    private static string GetLatestVersion(ProgramType programType)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        var handler = new WebRequestHandler();
        handler.ServerCertificateValidationCallback = delegate { return true; };
        var httpClient = new HttpClient(handler);
        httpClient.DefaultRequestHeaders.ConnectionClose = true;
        var response = httpClient.GetAsync("https://www.winpure.com/Full/v11/Version.json").Result;
        response.EnsureSuccessStatusCode();

        var resultText = response.Content.ReadAsStringAsync().Result;
        var versionList = JsonConvert.DeserializeObject<VersionList>(resultText);

        return programType switch
        {
            ProgramType.CamEntAd => versionList.CamEntAd,
            ProgramType.CamEnt => versionList.CamEnt,
            ProgramType.CamEntLite => versionList.CamEntLite,
            ProgramType.CamFree => versionList.CamFree,
            ProgramType.CamLte => versionList.CamLte,
            ProgramType.CamBiz => versionList.CamBiz,
            _ => throw new ArgumentException("Program type not expected")
        };
    }
}

class VersionList
{
    public string CamEnt { get; set; }
    public string CamEntLite { get; set; }
    public string CamEntAd { get; set; }
    public string CamFree { get; set; }
    public string CamBiz { get; set; }
    public string CamLte { get; set; }
}