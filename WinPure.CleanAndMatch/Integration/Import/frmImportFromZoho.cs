namespace WinPure.CleanAndMatch.Integration.Import;

internal partial class frmImportFromZoho : BaseDatabaseImportForm
{
    public frmImportFromZoho(IWpLogger logger, IConnectionSettingsService settingsService) : base(logger, settingsService)
    {
        InitializeComponent();
    }

    internal override void Localization()
    {
        base.Localization();
        groupBox1.Text = Resources.UI_CONNECTIONINFORMATION;
        groupBox3.Text = Resources.UI_TABLES;
        labelControl1.Text = Resources.UI_CLIENTID;
        labelControl2.Text = Resources.UI_CLIENTSECRET;
        labelControl3.Text = Resources.UI_CALLBACKURL;
    }

    internal override void LoadConfiguration()
    {
        if (_connectionSettings is { Settings: ZohoImportOptions options })
        {
            txtClientId.Text = options.ClientId;
            txtClientSecret.Text = options.ClientSecret;
            txtCallbackUrl.Text = options.CallbackUrl;
            lstTables.Text = options.TableName;
        }
    }

    internal override object GetConfigurationModel() => new ZohoImportOptions
    {
        TableName = lstTables.Text,
        ClientId = txtClientId.Text,
        ClientSecret = txtClientSecret.Text,
        CallbackUrl = txtCallbackUrl.Text,
    };

    internal override void GetConnectionSettings(int id)
    {
        _connectionSettings = _settingsService.Get<ZohoImportOptions>(id);
    }

    internal override void ImportData()
    {
        if (!ImportDbProvider.CheckConnect() && lstTables.SelectedIndex >= 0)
        {
            MessageBox.Show(Resources.DIALOG_IOFORM_SET_CONNECTION_TO_DATABASE_FOR_IMPORT);
        }
        else
        {
            ImportDbProvider.Initialize(GetConfigurationModel());
            var frmSelectFields = WinPureUiDependencyResolver.Resolve<frmSelectFields>();
            if (frmSelectFields.ShowDialog(ImportDbProvider.SelectFields()) == DialogResult.OK)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }

    private void RefreshData()
    {
        HideException();
        ImportDbProvider.Initialize(GetConfigurationModel());
        if (ImportDbProvider.CheckConnect())
        {
            lstTables.DataSource = null;
            lstTables.DataSource = ImportDbProvider.GetDatabaseTables();
        }
        else
        {
            ClearControls();
        }
    }

    private void ClearControls()
    {
        lstTables.DataSource = null;
        FillGridWithDataSource(null);
    }

    private void lstTables_SelectedIndexChanged(object sender, EventArgs e)
    {
        HideException();
        ImportDbProvider.Initialize(GetConfigurationModel());
        FillGridWithDataSource(ImportDbProvider.GetPreview());
    }

    private void btnCheckConnect_Click(object sender, EventArgs e)
    {
        RefreshData();
    }
}