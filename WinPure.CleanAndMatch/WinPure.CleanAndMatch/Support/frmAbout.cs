namespace WinPure.CleanAndMatch.Support;

public partial class frmAbout : XtraForm
{
    private readonly ILicenseService _licenseService;
    public frmAbout()
    {
        InitializeComponent();
        Localization();
        _licenseService = WinPureUiDependencyResolver.Resolve<ILicenseService>();
        var version = Common.Helpers.AssemblyHelper.ApplicationVersion();
        labelVersion.Text = $"Version: {version}";
        lbLicenseState.Text = string.Format(Resources.MESSAGE_LICENSESTATE, _licenseService.GetLicenseState(), _licenseService.LicenseExpiredAtDays);
          
        var licenseService = WinPureUiDependencyResolver.Resolve<ILicenseService>();

        switch (Program.CurrentProgramVersion)
        {
            case ProgramType.CamEntAd:
                labelControl8.Text = Resources.UI_CAM_WPCMENTERPRISE2;
                Text = string.Format(Resources.CAPTION_ABOUTFORM_ABOUT, Resources.UI_CAM_ENTERPRISE_EDITION2);
                break;
            case ProgramType.CamFree:
                labelControl8.Text = Resources.UI_CAM_FREE;
                Text = string.Format(Resources.CAPTION_ABOUTFORM_ABOUT, Resources.UI_CAM_FREE_EDITION);
                break;

            case ProgramType.CamEnt:
                labelControl8.Text = Resources.UI_CAM_WPCMENTERPRISE + (licenseService.IsDemo ? " Demo" : "");
                Text = string.Format(Resources.CAPTION_ABOUTFORM_ABOUT, Resources.UI_CAM_ENTERPRISE_EDITION);
                break;

            case ProgramType.CamEntLite:
                labelControl8.Text = Resources.UI_CAM_WPCMENTERPRISELITE + (licenseService.IsDemo ? " Demo" : "");
                Text = string.Format(Resources.CAPTION_ABOUTFORM_ABOUT, Resources.UI_CAM_ENTERPRISE_LITE_EDITION);
                break;


            case ProgramType.CamLte:
                if (licenseService.IsDemo)
                {
                    labelControl8.Text = Resources.UI_CAMLTE_DEMO_OPTIONS_2;
                    Text = string.Format(Resources.CAPTION_ABOUTFORM_ABOUT, Resources.UI_CAMLTE_DEMO_OPTIONS_3);
                }
                else
                {
                    labelControl8.Text = Resources.UI_CAM_LITE;
                    Text = string.Format(Resources.CAPTION_ABOUTFORM_ABOUT, Resources.UI_CAM_LITE_EDITION);
                }
                break;
            case ProgramType.CamBiz:
                labelControl8.Text = Resources.UI_CAM_BUSINESS;
                Text = string.Format(Resources.CAPTION_ABOUTFORM_ABOUT, Resources.UI_CAM_BUSINESS_EDITION);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Localization()
    {
        labelControl2.Text = Resources.UI_ABOUTFORM_VERSION101;
        labelControl6.Text = Resources.UI_ABOUTFORM_CURRENTVERSION;
        simpleButton3.Text = Resources.UI_CLOSE;
        labelVersion.Text = Resources.UI_ABOUTFORM_VERSION101;
        labelControl7.Text = Resources.UI_ABOUTFORM_COPYRIGHT;
        labelControl8.Text = Resources.UI_CAM_WPCMENTERPRISE;
        btnCheckForUpdate.Text = Resources.UI_CAPTION_CHECKUPDATE;
    }

    private void simpleButton3_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void btnCheckForUpdate_Click(object sender, EventArgs e)
    {
        var logger = WinPureUiDependencyResolver.Resolve<IWpLogger>();
        UpdateHelper.CheckForUpdate(Program.CurrentProgramVersion, logger);
    }
}