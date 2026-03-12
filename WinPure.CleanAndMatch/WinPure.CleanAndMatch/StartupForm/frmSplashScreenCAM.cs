using DevExpress.XtraSplashScreen;

namespace WinPure.CleanAndMatch.StartupForm;

public partial class frmSplashScreenCAM : SplashScreen
{
    public frmSplashScreenCAM()
    {
        InitializeComponent();
        lbProgramName.Parent = pictureBox1;
        lbVersion.Parent = pictureBox1;

        Localization();
        var version = Common.Helpers.AssemblyHelper.ApplicationVersion();
        lbVersion.Text = $"v{version}";
        var programDisplayName = Program.CurrentProgramVersion switch
        {
            ProgramType.CamLte => Resources.UI_CAMLTE_OPTIONS,
            ProgramType.CamFree => Resources.UI_CAMFREE_OPTIONS,
            ProgramType.CamBiz => Resources.UI_CAMBIZ_OPTIONS,
            ProgramType.CamEnt => Resources.UI_CAMENT_OPTIONS,
            ProgramType.CamEntAd => Resources.UI_CAMENTAD_OPTIONS,
            ProgramType.CamEntLite => Resources.UI_CAMENTLITE_OPTIONS,
            _ => "Clean & Match"
        };

        var licenseService = WinPureUiDependencyResolver.Resolve<ILicenseService>();
        lbProgramName.Text = licenseService.IsDemo ? Resources.UI_CAMENT_OPTIONS_DEMO : programDisplayName;
    }


    private void Localization()
    {
        Text = Resources.UI_SPLASHSCREENCAMFORM_STARTING;
    }

   
}