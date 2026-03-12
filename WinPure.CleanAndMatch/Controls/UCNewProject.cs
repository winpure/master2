using DevExpress.LookAndFeel;
using System.ComponentModel;

namespace WinPure.CleanAndMatch.Controls;

public partial class UCNewProject : DevExpress.XtraEditors.XtraUserControl
{
    public event Action OnOpenProject;
    public event Action<bool> OnSaveProject;
    public event Action OnCreateNewProject;
    public event Action OnOpenSettings;

    private readonly ThemeDetectionService _themeDetectionService;
    public UCNewProject()
    {
        InitializeComponent();
        Localization();

        if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
        {
            SetLabelText();
            var licenseService = WinPureUiDependencyResolver.Resolve<ILicenseService>();
            licenseService.LicenseLoaded += LicenseService_LicenseLoaded;
            var configurationService = WinPureUiDependencyResolver.Resolve<IConfigurationService>();

            _themeDetectionService = (ThemeDetectionService)WinPureUiDependencyResolver.Instance
                                .ServiceProvider
                                .GetService(typeof(ThemeDetectionService));

            if (_themeDetectionService != null)
            {
                _themeDetectionService.SetReferenceControl(this);
                UserLookAndFeel.Default.StyleChanged += OnThemeChanged;
            }
        }
    }

    private void Localization()
    {
        labelControl4.Text = Resources.UI_UCNEWPROJECTFORM_WINPURECLEANMATCHENTERPRISE;

        labelControl1.Text = Resources.UI_UCNEWPROJECTFORM_CREATENEWPROJECTOROPENPREVIOUSLY;

        NewProjectTileItem.Text = Resources.UI_ENTERPROJECTNAMEFORM_CREATEPROJECT;
        OpenProjectTileItem.Text = Resources.UI_UCNEWPROJECTFORM_OPENPROJECT;
        SaveProjectTileItem.Text = Resources.DIALOG_PROJECT_SAVE_CAPTION;
        SaveAsProjectTileItem.Text = Resources.DIALOG_PROJECT_SAVEAS_CAPTION;
        SettingsTileBarItem.Text = Resources.UI_SETTINGS2;
    }

    private void OnThemeChanged(object sender, EventArgs e)
    {
        if (_themeDetectionService == null)
        {
            return;
        }

        if (_themeDetectionService.IsDarkTheme())
        {
            NewProjectTileItem.AppearanceItem.Normal.BackColor = Color.FromArgb(64, 64, 64);
            OpenProjectTileItem.AppearanceItem.Normal.BackColor = Color.FromArgb(64, 64, 64);
            SaveProjectTileItem.AppearanceItem.Normal.BackColor = Color.FromArgb(64, 64, 64);
            SaveAsProjectTileItem.AppearanceItem.Normal.BackColor = Color.FromArgb(64, 64, 64);
            SettingsTileBarItem.AppearanceItem.Normal.BackColor = Color.FromArgb(64, 64, 64);

            NewProjectTileItem.AppearanceItem.Normal.ForeColor = Color.White;
            OpenProjectTileItem.AppearanceItem.Normal.ForeColor = Color.White;
            SaveProjectTileItem.AppearanceItem.Normal.ForeColor = Color.White;
            SaveAsProjectTileItem.AppearanceItem.Normal.ForeColor = Color.White;
            SettingsTileBarItem.AppearanceItem.Normal.ForeColor = Color.White;

            NewProjectTileItem.AppearanceItem.Normal.BorderColor = Color.FromArgb(64, 64, 64);
            OpenProjectTileItem.AppearanceItem.Normal.BorderColor = Color.FromArgb(64, 64, 64);
            SaveProjectTileItem.AppearanceItem.Normal.BorderColor = Color.FromArgb(64, 64, 64);
            SaveAsProjectTileItem.AppearanceItem.Normal.BorderColor = Color.FromArgb(64, 64, 64);
            SettingsTileBarItem.AppearanceItem.Normal.BorderColor = Color.FromArgb(64, 64, 64);

            LogoPictureBox.Image = Resources._2026_logo_transprent_dark_theme;
        }
        else
        {
            NewProjectTileItem.AppearanceItem.Normal.BackColor = Color.WhiteSmoke;
            OpenProjectTileItem.AppearanceItem.Normal.BackColor = Color.WhiteSmoke;
            SaveProjectTileItem.AppearanceItem.Normal.BackColor = Color.WhiteSmoke;
            SaveAsProjectTileItem.AppearanceItem.Normal.BackColor = Color.WhiteSmoke;
            SettingsTileBarItem.AppearanceItem.Normal.BackColor = Color.WhiteSmoke;

            NewProjectTileItem.AppearanceItem.Normal.ForeColor = Color.FromArgb(64, 64, 64);
            OpenProjectTileItem.AppearanceItem.Normal.ForeColor = Color.FromArgb(64, 64, 64);
            SaveProjectTileItem.AppearanceItem.Normal.ForeColor = Color.FromArgb(64, 64, 64);
            SaveAsProjectTileItem.AppearanceItem.Normal.ForeColor = Color.FromArgb(64, 64, 64);
            SettingsTileBarItem.AppearanceItem.Normal.ForeColor = Color.FromArgb(64, 64, 64);

            NewProjectTileItem.AppearanceItem.Normal.BorderColor = Color.Gainsboro;
            OpenProjectTileItem.AppearanceItem.Normal.BorderColor = Color.Gainsboro;
            SaveProjectTileItem.AppearanceItem.Normal.BorderColor = Color.Gainsboro;
            SaveAsProjectTileItem.AppearanceItem.Normal.BorderColor = Color.Gainsboro;
            SettingsTileBarItem.AppearanceItem.Normal.BorderColor = Color.Gainsboro;

            LogoPictureBox.Image = Resources._2026_logo_transprent_light_theme;
        }


    }

    private void SetLabelText()
    {
        var licenseService = WinPureUiDependencyResolver.Resolve<ILicenseService>();
        switch (Program.CurrentProgramVersion)
        {
            case ProgramType.CamEnt:
                labelControl4.Text = Resources.UI_CAM_WPCMENTERPRISE + (licenseService.IsDemo ? " Demo" : "");
                break;
            case ProgramType.CamEntAd:
                labelControl4.Text = Resources.UI_CAM_WPCMENTERPRISE2 + (licenseService.IsDemo ? " Demo" : "");
                break;
            case ProgramType.CamLte:
                labelControl4.Text = Resources.UI_CAM_LITE;
                break;
            case ProgramType.CamFree:
                labelControl4.Text = Resources.UI_CAM_FREE;
                break;
            case ProgramType.CamBiz:
                labelControl4.Text = Resources.UI_CAM_BUSINESS;
                break;

            case ProgramType.CamEntLite:
                labelControl4.Text = Resources.UI_CAM_WPCMENTERPRISELITE + (licenseService.IsDemo ? " Demo" : "");
                break;
        }
    }

    private void LicenseService_LicenseLoaded()
    {
        SetLabelText();
    }

    private void NewProjectTileItem_ItemClick(object sender, TileItemEventArgs e)
    {
        OnCreateNewProject?.Invoke();
    }

    private void OpenProjectTileItem_ItemClick(object sender, TileItemEventArgs e)
    {
        OnOpenProject?.Invoke();
    }

    private void SaveProjectTileItem_ItemClick(object sender, TileItemEventArgs e)
    {
        OnSaveProject?.Invoke(false);
    }

    private void SaveAsProjectTileItem_ItemClick(object sender, TileItemEventArgs e)
    {
        OnSaveProject?.Invoke(true);
    }

    private void SettingsTileBarItem_ItemClick(object sender, TileItemEventArgs e)
    {
        OnOpenSettings?.Invoke();
    }
}