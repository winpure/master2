namespace WinPure.CleanAndMatch.Integration.Import;

internal partial class frmImportFromSalesforce : BaseDatabaseImportForm
{
    public frmImportFromSalesforce(IWpLogger logger, IConnectionSettingsService settingsService) : base(logger, settingsService)
    {
        InitializeComponent();
    }

    internal override void Localization()
    {
        base.Localization();
        groupBox1.Text = Resources.UI_CONNECTIONINFORMATION;
        groupBox3.Text = Resources.UI_TABLES;
        labelControl1.Text = Resources.UI_LOGIN;
        labelControl2.Text = Resources.UI_PASSWORD;
    }

    internal override void LoadConfiguration()
    {
        if (_connectionSettings is { Settings: SalesforceImportOptions options })
        {
            lstTables.Text = options.TableName;
            txtLogin.Text = options.UserName;
            txtPassword.Text = options.Password;
            txtToken.Text = options.Token;
            cbSandBox.Checked = options.UseSandbox;
        }
    }

    internal override object GetConfigurationModel() => new SalesforceImportOptions
        {
            TableName = lstTables.Text,
            UserName = txtLogin.Text,
            Password = txtPassword.Text,
            Token = txtToken.Text,
            UseSandbox = cbSandBox.Checked
        };
    
    internal override void GetConnectionSettings(int id)
    {
        _connectionSettings = _settingsService.Get<SalesforceImportOptions>(id);
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

    private void ClearControls()
    {
        lstTables.DataSource = null;
        FillGridWithDataSource(null);
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