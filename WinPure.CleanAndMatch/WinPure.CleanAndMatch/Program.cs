using System.Globalization;
using DevExpress.XtraSplashScreen;
using WinPure.AddressVerification.Models;
using WinPure.CleanAndMatch.Controls;
using WinPure.CleanAndMatch.StartupForm;
using WinPure.Configuration.Helper;
using WinPure.Licensing.Enums;
using WinPure.Matching.Models.Reports;

namespace WinPure.CleanAndMatch;

static class Program
{
    internal static ProgramType CurrentProgramVersion;
    internal static CultureInfo CurrentProgramCultureInfo;
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        try
        {
            DeserializationSettings.RegisterTrustedAssembly(typeof(UCVerification).Assembly);
            DeserializationSettings.RegisterTrustedAssembly(typeof(AuditLogDataReport).Assembly);
            DeserializationSettings.RegisterTrustedAssembly(typeof(AddressVerificationReport).Assembly);
            DeserializationSettings.RegisterTrustedAssembly(typeof(ReportData).Assembly);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SQLitePCL.Batteries_V2.Init();
            CurrentProgramCultureInfo = CultureInfo.CurrentCulture;

            var serviceProvider = WinPureUiDependencyResolver.Instance.ServiceProvider;
            var licenseService = WinPureUiDependencyResolver.Resolve<ILicenseService>();
            licenseService.Initiate();

            var configuration = WinPureUiDependencyResolver.Resolve<IConfigurationService>();
            configuration.Initiate(ProgramType.CamEnt);
            var licenseFile = Path.Combine(configuration.Configuration.License.Path, configuration.Configuration.License.DesktopLicenseName ?? "");

            if (!string.IsNullOrWhiteSpace(configuration.Configuration.License.DesktopLicenseName) && !File.Exists(licenseFile))
            {
                configuration.Configuration.License.DesktopLicenseName = String.Empty;
                configuration.SaveConfiguration();
            }
            licenseService.LoadLicense(configuration.Configuration.License.Path, configuration.Configuration.License.DesktopLicenseName);
            CurrentProgramVersion = licenseService.ProgramType;

            SplashScreenManager.ShowForm(typeof(frmSplashScreenCAM));

            DatabaseInitiator.InitiateDatabase(serviceProvider);
            var languageService = WinPureUiDependencyResolver.Resolve<ILanguageService>();
            languageService.Initiate(CurrentProgramVersion);

            var logger = WinPureUiDependencyResolver.Resolve<IWpLogger>();
            logger.SetReportPath(configuration.Configuration.LogPath);
            languageService.SetProgramLanguage(CurrentProgramCultureInfo);

            DevExpress.UserSkins.BonusSkins.Register();
            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.Skins.SkinManager.EnableMdiFormSkins();

            configuration.DefineProjectDatabases(logger);


            ////open the frmstartupoptions form to let user choose which form to open based on the license state
            //var frmStartupOptions = new frmRegisterOnStartup();
            //Application.Run(frmStartupOptions);
            //return;

            var licenseState = licenseService.GetLicenseState();
            frmMain frmMain;
            switch (licenseState)
            {
                case LicenseState.Valid:
                    frmMain = new frmMain();
                    SplashScreenManager.CloseForm();
                    Application.Run(frmMain);
                    break;
                case LicenseState.Demo:
                    frmMain = new frmMain();
                    var frmRegisterOnStartupDemo = new frmRegisterOnStartup();
                    SplashScreenManager.CloseForm();
                    if (frmRegisterOnStartupDemo.ShowDialog() == DialogResult.OK)
                    {
                        if (frmRegisterOnStartupDemo.ShouldAppClose)
                        {
                            return;
                        }
                        Application.Run(frmMain);
                    }
                    break;
                case LicenseState.LicenseExpire:
                    SplashScreenManager.CloseForm();
                    var registerOnStarupDemo = new frmRegisterOnStartup();
                    registerOnStarupDemo.ShowDialog();
                    if (registerOnStarupDemo.ShouldAppClose)
                    {
                        return;
                    }
                    break;
                case LicenseState.DemoExpire:
                    SplashScreenManager.CloseForm();
                    var regForm1 = new frmRegisterOnStartup();
                    regForm1.ShowDialog();
                    if (regForm1.ShouldAppClose)
                    {
                        return;
                    }
                    break;
                case LicenseState.Invalid:
                    MessageBox.Show(Resources.EXCEPTION_LICENSE_INVALID,
                        Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SplashScreenManager.CloseForm();
                    break;
                default:
                    MessageBox.Show(Resources.EXCEPTION_LICENSE_INVALID,
                        Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SplashScreenManager.CloseForm();
                    break;
            }
        }
        catch (Exception ex)
        {
            var logger = WinPureUiDependencyResolver.Resolve<IWpLogger>();
            logger.Debug("GLOBAL APPLICATION ERROR", ex);
            MessageBox.Show(ex.Message);
        }
    }
}