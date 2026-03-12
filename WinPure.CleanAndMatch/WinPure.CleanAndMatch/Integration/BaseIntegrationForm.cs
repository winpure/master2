using DevExpress.LookAndFeel;
using System.ComponentModel;
using WinPure.Configuration.Models.Configuration;

namespace WinPure.CleanAndMatch.Integration;

internal partial class BaseIntegrationForm : XtraForm
{
    internal ConnectionSettings _connectionSettings = null;
    internal readonly IWpLogger _logger;
    internal readonly IConnectionSettingsService _settingsService;
    private readonly bool _showConnectionMenu;
    private readonly ThemeDetectionService _themeDetectionService;

    public BaseIntegrationForm()
    {
        InitializeComponent();
    }

    public BaseIntegrationForm(IWpLogger logger, IConnectionSettingsService settingsService, bool showConnectionMenu)
    {
        InitializeComponent();
        _logger = logger;
        _settingsService = settingsService;
        _showConnectionMenu = showConnectionMenu;
        ctxMenu.Visible = showConnectionMenu;

        if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
        {
            _themeDetectionService = WinPureUiDependencyResolver.Resolve<ThemeDetectionService>();
            _themeDetectionService.SetReferenceControl(panelControl1);

            UserLookAndFeel.Default.StyleChanged += OnThemeChanged;
        }
    }


    internal virtual void Localization()
    {
        mnuLoadConfiguration.Text = Resources.UI_LOAD_CONNECTION;
        mnuSaveConfiguration.Text = Resources.UI_SAVE_CONNECTION;
    }

    internal virtual void LoadConfiguration() { }

    internal virtual object GetConfigurationModel() => null;

    internal virtual ExternalSourceTypes IntegrationSource => ExternalSourceTypes.NotDefined;

    internal virtual void GetConnectionSettings(int id)
    {
        _connectionSettings = _settingsService.Get<SqlImportExportOptions>(id);
    }

    internal virtual void HideException() { }

    internal void LoadConnectionSettings()
    {
        if (!_showConnectionMenu) return;

        var configurations = _settingsService.Get(IntegrationSource);
        mnuLoadConfiguration.DropDownItems.Clear();
        foreach (var configuration in configurations)
        {
            mnuLoadConfiguration.DropDownItems.Add(CreateConfigurationItem(configuration));
        }
    }

    private void LoadConnectionMenuItem_Click(object sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem menuItem)
        {
            var id = Convert.ToInt32(menuItem.Tag);
            GetConnectionSettings(id);
            LoadConfiguration();
        }
    }

    private void SaveConnectionMenuItem_Click(object sender, EventArgs e)
    {
        _connectionSettings ??= new ConnectionSettings();

        var frmEnterName = WinPureUiDependencyResolver.Resolve<frmNewName>();
        if (frmEnterName.Show(Resources.UI_CAPTION_CONFIGURATIONNAME, Resources.UI_CAPTION_ENTERCONFIGURATIONNAME, _connectionSettings.Name, NameHelper.ProhibitedChars.ToCharArray()) == DialogResult.OK)
        {
            _connectionSettings.Name = frmEnterName.NewName;
            _connectionSettings.SourceType = IntegrationSource;
            _connectionSettings.Settings = GetConfigurationModel();
            var newId = _settingsService.Save(_connectionSettings);
            if (newId != _connectionSettings.Id)
            {
                _connectionSettings.Id = newId;
                mnuLoadConfiguration.DropDownItems.Add(CreateConfigurationItem(_connectionSettings));
            }
        }
    }

    private ToolStripMenuItem CreateConfigurationItem(ConnectionSettings configuration)
    {
        var item = new ToolStripMenuItem(configuration.Name) { Tag = configuration.Id };
        item.Click += LoadConnectionMenuItem_Click;
        return item;
    }

    private void BaseIntegrationForm_Load(object sender, EventArgs e)
    {
        ApplyTheme();
    }

    private void OnThemeChanged(object sender, EventArgs e)
    {
        ApplyTheme();
    }

    private void ApplyTheme()
    {
        if (!_showConnectionMenu) return;

        if (_themeDetectionService == null) return;

        var isDark = _themeDetectionService.IsDarkTheme();

        if (isDark)
        {
            ctxMenu.BackColor = Color.FromArgb(32, 32, 32);
            mnuLoadConfiguration.ForeColor = Color.White;
            mnuSaveConfiguration.ForeColor = Color.White;
        }
        else
        {
            ctxMenu.BackColor = Color.FromArgb(244, 244, 244);
            mnuLoadConfiguration.ForeColor = Color.Black;
            mnuSaveConfiguration.ForeColor = Color.Black;
        }

        ApplyThemeToDerivedControls(isDark);
    }

    internal virtual void ApplyThemeToDerivedControls(bool isDarkTheme) { }

}