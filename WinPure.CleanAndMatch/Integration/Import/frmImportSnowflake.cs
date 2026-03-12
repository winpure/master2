namespace WinPure.CleanAndMatch.Integration.Import;

internal partial class frmImportSnowflake : BaseDatabaseImportForm
{
    public frmImportSnowflake(IWpLogger logger, IConnectionSettingsService settingsService) : base(logger, settingsService)
    {
        InitializeComponent();
    }

    internal override void SetDefaultConfiguration()
    {
        // for testing
        //txtOrganization.Text = "";
        //txtAccount.Text = "";
        //txtLogin.Text = "";
        //txtPassword.Text = "";
        //txtDatabase.Text = "";
        //txtSchema.Text = "";
        //txtTable.Text = "";
    }

    internal override void Localization()
    {
        base.Localization();
        lbDatabase.Text = Resources.UI_DATABASE;
        lbPassword.Text = Resources.UI_PASSWORD;
        lbLogin.Text = Resources.UI_LOGIN;

        btnPreview.Text = Resources.UI_PREVIEW;
        lbOrganization.Text = Resources.UI_ORGANIZATION;
        lbAccount.Text = Resources.UI_ACCOUNT;
        lbSchema.Text = Resources.UI_SCHEMA;
        lbTable.Text = Resources.UI_TABLE;
    }

    internal override void LoadConfiguration()
    {
        if (_connectionSettings is {Settings: SnowflakeImportExportOptions options})
        {
            txtTable.Text = options.TableName;
            txtPassword.Text = options.Password;
            txtLogin.Text = options.User;
            txtDatabase.Text = options.Database;
            txtSchema.Text = options.Schema;
            txtAccount.Text = options.Account;
            txtOrganization.Text = options.Organization;
        }
    }

    internal override object GetConfigurationModel() => new SnowflakeImportExportOptions
    {
        TableName = txtTable.Text,
        Password = txtPassword.Text,
        User = txtLogin.Text,
        Database = txtDatabase.Text,
        Schema = txtSchema.Text,
        Account = txtAccount.Text,
        Organization = txtOrganization.Text
    };

    internal override void GetConnectionSettings(int id)
    {
        _connectionSettings = _settingsService.Get<SnowflakeImportExportOptions>(id);
    }

    internal override void ImportData()
    {
        HideException();
        ImportDbProvider.Initialize(GetConfigurationModel());
        if (ImportDbProvider.CheckConnect())
        {
            var frmSelectFields = WinPureUiDependencyResolver.Resolve<frmSelectFields>();
            if (frmSelectFields.ShowDialog(ImportDbProvider.SelectFields()) == DialogResult.OK)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
        else
        {
            NotifyError($"You should set correct credentials to access to {ImportDbProvider.SourceType}", null);
        }
    }

    private void btnPreview_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtTable.Text))
        {
            HideException();
            ImportDbProvider.Initialize(GetConfigurationModel());
            if (ImportDbProvider.CheckConnect())
            {
                var prev = ImportDbProvider.GetPreview();
                FillGridWithDataSource(prev);
            }
            else
            {
                FillGridWithDataSource(null);
            }
        }
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.ImportSnowflake);
    }
}