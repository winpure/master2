namespace WinPure.CleanAndMatch.Integration.Export;

internal partial class frmExportMySQL : BaseDatabaseExportForm
{
    public frmExportMySQL(IWpLogger logger, IConnectionSettingsService settingsService) : base(logger, settingsService)
    {
        InitializeComponent();
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

        rgAuthType.Properties.Items[0].Description = Resources.UI_IMPORTMSSQLFORM_USEWINDOWS;
        rgAuthType.Properties.Items[1].Description = Resources.UI_IMPORTMSSQLFORM_USESQL;
        labelControl1.Text = Resources.UI_IMPORTMSSQLFORM_SERVERNAME;

        labelControl7.Text = Resources.UI_LOGIN;
        labelControl6.Text = Resources.UI_PASSWORD;
        gcSshInfo.Text = Resources.UI_SSH_INFORMATION;
        cbUseSsh.Text = Resources.UI_SSH_USESSH;
        labelControl8.Text = Resources.UI_SERVER;

        ShowSSHInfo(true);
    }

    private void ClearControls()
    {
        cbeDatebase.Properties.Items.Clear();
        cbeDatebase.SelectedIndex = -1;
    }

    internal override void SetDefaultConfiguration()
    {
        rgAuthType.SelectedIndex = 0;
        rgAuthType_SelectedIndexChanged(null, null);
        ExportDbProvider.OnException += NotifyError;
        if (ExportDbProvider.SourceType == ExternalSourceTypes.Oracle)
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
            txtLogin.Text = options.UserName;
            txtPassword.Text = options.Password;
            rgAuthType.SelectedIndex = options.IntegrateSecurity ? 0 : 1;
            sePort.EditValue = options.Port == 0 ? null : options.Port;

            cbUseSsh.Checked = options.UseSsh;
            txtSshServer.Text = options.SshServer;
            txtSshLogin.Text = options.SshLogin;
            txtSshPassword.Text = options.SshPassword;
            cbSsl.Checked = options.UseSsl;
            cbeDatebase.Text = options.DatabaseName;
            txtTableName.Text= options.TableName;
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
        Port = Convert.ToInt32(sePort.Value),
        UseSsh = cbUseSsh.Checked,
        SshServer = txtSshServer.Text,
        SshLogin = cbUseSsh.Checked ? txtSshLogin.Text : "",
        SshPassword = cbUseSsh.Checked ? txtSshPassword.Text : "",
        UseSsl = cbSsl.Checked
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
            NotifyError("You should set correct credentials to access to MS SQL server and define table name for exporting", null);
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

    private void cbUseSsh_CheckedChanged(object sender, EventArgs e)
    {
        txtSshPassword.Enabled = txtSshLogin.Enabled = txtSshServer.Enabled = cbUseSsh.Checked;
    }

    private void gcSshInfo_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
    {
        ShowSSHInfo(pnlSsh.Visible);
    }

    private void ShowSSHInfo(bool visible)
    {
        if (visible)
        {
            pnlSsh.Visible = false;
            gcSshInfo.Height = 44;
            this.Height = 539;
            gcSshInfo.CustomHeaderButtons[0].Properties.Visible = true;
            gcSshInfo.CustomHeaderButtons[1].Properties.Visible = false;
        }
        else
        {
            gcSshInfo.Height = 150;
            this.Height = 677;
            pnlSsh.Visible = true;
            gcSshInfo.CustomHeaderButtons[0].Properties.Visible = false;
            gcSshInfo.CustomHeaderButtons[1].Properties.Visible = true;
        }
    }

    private void rgAuthType_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtLogin.Enabled = rgAuthType.SelectedIndex == 1;
        txtPassword.Enabled = rgAuthType.SelectedIndex == 1;
    }
}