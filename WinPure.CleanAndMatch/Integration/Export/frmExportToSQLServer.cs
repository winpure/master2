namespace WinPure.CleanAndMatch.Integration.Export;

internal partial class frmExportToSQLServer : BaseDatabaseExportForm
{
    public frmExportToSQLServer(IWpLogger logger, IConnectionSettingsService settingsService) : base(logger, settingsService)
    {
        InitializeComponent();

        sePort.EditValue = null;
    }

    internal override void Localization()
    {
        base.Localization();
        groupBox1.Text = Resources.UI_CONNECTIONINFORMATION;
        labelControl5.Text = Resources.UI_EXPORTMSSQLFORM_TABLENAME;
        labelControl4.Text = Resources.UI_DATABASE;
        groupBox2.Text = Resources.UI_LOGONSERVER;
        labelControl3.Text = Resources.UI_PASSWORD;
        labelControl2.Text = Resources.UI_LOGIN;
        rgAuthType.Properties.Items[1].Description = Resources.UI_IMPORTMSSQLFORM_USESQL;
        rgAuthType.Properties.Items[0].Description = Resources.UI_IMPORTMSSQLFORM_USEWINDOWS;
        labelControl1.Text = Resources.UI_IMPORTMSSQLFORM_SERVERNAME;
    }

    private void ClearControls()
    {
        cbeDatebase.Properties.Items.Clear();
        cbeDatebase.SelectedIndex = -1;
    }

    internal override void SetDefaultConfiguration()
    {
        sePort.EditValue = ExportDbProvider.SourceType switch
        {
            ExternalSourceTypes.Db2 => 50000,
            ExternalSourceTypes.Postgres => 5432,
            ExternalSourceTypes.AzureDb => 1433,
            ExternalSourceTypes.SqlServer => 1433,
            ExternalSourceTypes.Oracle => 1521,
            _ => 1433
        };

        rgAuthType.SelectedIndex = 0;
        rgAuthType_SelectedIndexChanged(null, null);

        if (ExportDbProvider.SourceType == ExternalSourceTypes.Oracle || ExportDbProvider.SourceType == ExternalSourceTypes.AzureDb)
        {
            rgAuthType.SelectedIndex = 1;
            rgAuthType.Enabled = false;
        }

        // for testing
        //txtServerName.Text = @"";
        //txtLogin.Text = "";
        //txtPassword.Text = "";

        //txtTableName.Text = "AAA_TEST";
    }

    internal override void LoadConfiguration()
    {
        HideException();
        if (_connectionSettings is { Settings: SqlImportExportOptions options })
        {
            txtServerName.Text = options.ServerAddress;
            sePort.EditValue = options.Port == 0 ? null : options.Port;
            txtLogin.Text = options.UserName;
            txtPassword.Text = options.Password;
            rgAuthType.SelectedIndex = options.IntegrateSecurity ? 0 : 1;
            rgAuthType_SelectedIndexChanged(null, null);    

            cbeDatebase.Text = options.DatabaseName;
            txtTableName.Text = options.TableName;
        }
    }

    internal override object GetConfigurationModel() => new SqlImportExportOptions
    {
        ServerAddress = txtServerName.Text,
        DatabaseName = cbeDatebase.Text,
        TableName = txtTableName.Text,
        UserName = txtLogin.Text,
        Password = txtPassword.Text,
        IntegrateSecurity = rgAuthType.SelectedIndex == 0,
        Port = sePort.EditValue != null ? (int)sePort.Value : 0
    };

    internal override void ExportData()
    {
        HideException();
        ExportDbProvider.Initialize(GetConfigurationModel());
        if (ExportDbProvider.CheckConnect() && txtTableName.Text != "")
        {
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            NotifyError(string.Format(Resources.EXCEPTION_SET_CORRECT_CREDENTIALS, ExportDbProvider.DisplayName), null);
        }
    }

    private void btnRefreshServers_Click(object sender, EventArgs e)
    {
        HideException();
        ExportDbProvider.Initialize(GetConfigurationModel());
        if (ExportDbProvider.CheckConnect())
        {
            cbeDatebase.Properties.Items.Clear();
            var items = ExportDbProvider.GetDatabaseList();
            if (items != null && items.Count > 0)
            {
                cbeDatebase.Properties.Items.AddRange(items);
                cbeDatebase.SelectedIndex = 0;
            }
        }
        else
        {
            ClearControls();
        }
    }

    private void rgAuthType_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtLogin.Enabled = rgAuthType.SelectedIndex == 1;
        txtPassword.Enabled = rgAuthType.SelectedIndex == 1;
    }
}