namespace WinPure.CleanAndMatch.Integration.Import;

internal partial class BaseImportForm : BaseIntegrationForm, IImportForm
{
    internal IImportProvider ImportProvider;

    public BaseImportForm()
    {
        InitializeComponent();
    }

    public BaseImportForm(IWpLogger logger, IConnectionSettingsService settingsService) : base(logger, settingsService, false)
    {
        InitializeComponent();
    }

    public BaseImportForm(IWpLogger logger, IConnectionSettingsService settingsService, bool showConnectionMenu) : base(logger, settingsService, showConnectionMenu)
    {
        InitializeComponent();
    }

    public bool ShowImportForm(IImportProvider importProvider)
    {
        ImportProvider = importProvider;
        if (ImportProvider == null)
        {
            MessageBox.Show(Resources.EXCEPTION_IEFORM_WRONG_IMPORT_SERVICE);
            return false;
        }

        ImportProvider.OnException += NotifyError;
        
        Localization();

        LoadConnectionSettings();

        SetDefaultConfiguration();
        
        return ShowDialog() == DialogResult.OK;
    }

    internal override void Localization()
    {
        base.Localization();
        Text = string.Format(Resources.UI_IMPORTFILEDATABASE_IMPORTFROMFILEDATABASE, ImportProvider.DisplayName);
        btnCancel.Text = Resources.UI_CANCEL;
        btnImport.Text = Resources.UI_OK;
    }

    internal override ExternalSourceTypes IntegrationSource => ImportProvider.SourceType;

    internal virtual void SetDefaultConfiguration() { }

    internal virtual void NotifyError(string message, Exception exception)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { NotifyError(message, exception); }));
            return;
        }

        _logger.Debug($"Cannot import data from {ImportProvider.SourceType}", exception);
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

    internal virtual void FillGridWithDataSource(object dataSource)
    {
        dGridSample.BeginUpdate();
        dGridSample.DataSource = null;
        gvSample.Columns.Clear();
        if (dataSource != null)
        {
            dGridSample.DataSource = dataSource;
            dGridSample.Refresh();
            gvSample.BestFitColumns();
        }

        dGridSample.EndUpdate();
    }

    internal virtual void ImportData() {}

    internal void ShowLoadingPanel() => gvSample.ShowLoadingPanel();

    internal void HideLoadingPanel() => gvSample.HideLoadingPanel();

    private void btnImport_Click(object sender, EventArgs e)
    {
        ImportData();
    }

    private void BaseImportForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        ImportProvider.OnException -= NotifyError;
    }
}