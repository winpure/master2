namespace WinPure.CleanAndMatch.Integration.Export;

internal partial class frmExportToSnowflake : BaseDatabaseExportForm
{
    public frmExportToSnowflake(IWpLogger logger, IConnectionSettingsService settingsService) : base(logger, settingsService)
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

        lbOrganization.Text = Resources.UI_ORGANIZATION;
        lbAccount.Text = Resources.UI_ACCOUNT;
        lbSchema.Text = Resources.UI_SCHEMA;
        lbTable.Text = Resources.UI_TABLE;
    }

    internal override void LoadConfiguration()
    {
        if (_connectionSettings is { Settings: SnowflakeImportExportOptions options })
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

    internal override void ExportData()
    {
        HideException();
        ExportDbProvider.Initialize(GetConfigurationModel());
        if (ExportDbProvider.CheckConnect())
        {
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            NotifyError(string.Format(Resources.EXCEPTION_SET_CORRECT_CREDENTIALS, ExportDbProvider.DisplayName), null);
        }
    }

    private void btnTestConnection_Click(object sender, EventArgs e)
    {
        HideException();
        ExportDbProvider.Initialize(GetConfigurationModel());
        if (ExportDbProvider.CheckConnect())
        {
            MessageBox.Show("OK");
        }
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.ExportSnowflake);
    }
}