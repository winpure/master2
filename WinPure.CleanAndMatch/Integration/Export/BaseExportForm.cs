namespace WinPure.CleanAndMatch.Integration.Export;

internal partial class BaseExportForm : BaseIntegrationForm, IExportForm
{
    public IExportProvider ExportProvider { get; set; }


    public BaseExportForm()
    {
        InitializeComponent();
    }

    public BaseExportForm(IWpLogger logger, IConnectionSettingsService settingsService) : base(logger, settingsService, false)
    {
        InitializeComponent();
    }
    public BaseExportForm(IWpLogger logger, IConnectionSettingsService settingsService, bool showConnectionMenu) : base(logger, settingsService, showConnectionMenu)
    {
        InitializeComponent();
    }
    
    public bool ShowExportForm(IExportProvider databaseExportProvider)
    {
        ExportProvider = databaseExportProvider;
        if (ExportProvider == null)
        {
            MessageBox.Show(Resources.EXCEPTION_IEFORM_WRONG_IMPORT_SERVICE);
            return false;
        }

        Localization();

        ExportProvider.OnException += NotifyError;
        
        SetDefaultConfiguration();
        
        LoadConnectionSettings();

        return ShowDialog() == DialogResult.OK;
    }

    internal override void Localization()
    {
        base.Localization();
        Text = string.Format(Resources.UI_EXPORTMSSQLFORM_EXPORTTOMSSQL, ExportProvider.DisplayName);
        btnCancel.Text = Resources.UI_CANCEL;
        btnExport.Text = Resources.UI_EXPORT;
    }

    internal override ExternalSourceTypes IntegrationSource => ExportProvider.SourceType;

    internal virtual void SetDefaultConfiguration() { }

    internal virtual void NotifyError(string message, Exception exception)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { NotifyError(message, exception); }));
            return;
        }

        _logger.Debug($"Cannot export data to {ExportProvider.SourceType}", exception);
        lbError.Visible = true;
        lbError.Text = message;
        if (exception != null)
        {
            lbError.ToolTip = exception.Message;
            lbError.ShowToolTips = true;
            lbError.Text += ((lbError.Text.EndsWith(".") || lbError.Text.EndsWith("!") || lbError.Text.EndsWith("?")) ? "" : ".") + " " + Resources.MESSAGE_HOVEROVER;
        }
    }

    internal override void HideException()
    {
        lbError.Visible = false;
        lbError.ShowToolTips = false;
    }

    internal virtual void ExportData() { }

    private void BaseExportForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        ExportProvider.OnException -= NotifyError;
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
        ExportData();
    }
}