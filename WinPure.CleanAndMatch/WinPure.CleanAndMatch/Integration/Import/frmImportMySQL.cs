using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Services;

namespace WinPure.CleanAndMatch.Integration.Import;

internal partial class frmImportMySQL : BaseDatabaseImportForm
{
    public frmImportMySQL(IWpLogger logger, IConnectionSettingsService settingsService) : base(logger, settingsService)
    {
        InitializeComponent();
        InitSqlEditor();
        //this.tcConfiguration.ShowTabHeader = DefaultBoolean.False;
        ShowSSHInfo(true);
    }

    internal override void Localization()
    {
        base.Localization();
        groupBox1.Text = Resources.UI_CONNECTIONINFORMATION;
        tpConnection.Text = Resources.UI_CONNECTIONINFORMATION;
        labelControl4.Text = Resources.UI_DATABASE;
        groupBox2.Text = Resources.UI_LOGONSERVER;
        labelControl3.Text = Resources.UI_PASSWORD;
        rgAuthType.Properties.Items[1].Description = Resources.UI_IMPORTMSSQLFORM_USESQL;
        labelControl2.Text = Resources.UI_LOGIN;
        rgAuthType.Properties.Items[0].Description = Resources.UI_IMPORTMSSQLFORM_USEWINDOWS;
        labelControl1.Text = Resources.UI_IMPORTMSSQLFORM_SERVERNAME;
        gcTable.Text = Resources.UI_TABLES;
        labelControl7.Text = Resources.UI_LOGIN;
        labelControl6.Text = Resources.UI_PASSWORD;
        gcSshInfo.Text = Resources.UI_SSH_INFORMATION;
        cbUseSsh.Text = Resources.UI_SSH_USESSH;
        labelControl8.Text = Resources.UI_SERVER;
        btnSaveSql.Text = Resources.UI_SAVE;
        btnLoadSql.Text = Resources.UI_LOAD;
        btnSqlPreview.Text = Resources.UI_RUN;
        btnSqlValidate.Text = Resources.UI_VALIDATE;
        lbSelectTableOrSql.Text = Resources.CAPTION_SELECTTABLEORSQL;
        btnRefreshServers.Text = Resources.CAPTION_CONNECT;
    }

    private void InitSqlEditor()
    {
        var sqlDocument = edtSql.Document;
        foreach (Section section in sqlDocument.Sections)
        {
            var ln = section.LineNumbering;
            ln.Start = 1;
            ln.CountBy = 1;
            ln.RestartType = LineNumberingRestart.Continuous;
        }

        edtSql.ReplaceService<ISyntaxHighlightService>(new SqlSyntaxHighlightService(sqlDocument));
        sqlDocument.DefaultCharacterProperties.FontSize = 11;
        edtSql.BackColor = Color.FromArgb(30, 30, 30);
    }

    internal override void SetDefaultConfiguration()
    {
        rgAuthType.SelectedIndex = 0;
        rgAuthType_SelectedIndexChanged(null, null);

        // for testing
        //txtServerName.Text = @"";
        //txtLogin.Text = "";
        //txtPassword.Text = "";
        //txtSshLogin.Text = "";
        //txtSshServer.Text = "";
        //txtSshPassword.Text = "";
    }

    internal override void ApplyThemeToDerivedControls(bool isDarkTheme)
    {
        if (tcConfiguration == null) return;

        tcConfiguration.BackColor = isDarkTheme
            ? Color.FromArgb(32, 32, 32)
            : Color.FromArgb(244, 244, 244);
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

            var tableIndex = lstTables.FindString(options.TableName);
            lstTables.SelectedIndex = tableIndex;
        }
    }

    internal override object GetConfigurationModel() => new SqlImportExportOptions
    {
        ServerAddress = txtServerName.Text,
        DatabaseName = cbeDatebase.Text,
        TableName = lstTables.Text,
        UserName = txtLogin.Text,
        Password = txtPassword.Text,
        IntegrateSecurity = rgAuthType.SelectedIndex == 0,
        Port = Convert.ToInt32(sePort.Value),
        UseSsh = cbUseSsh.Checked,
        SshServer = txtSshServer.Text,
        SshLogin = cbUseSsh.Checked ? txtSshLogin.Text : "",
        SshPassword = cbUseSsh.Checked ? txtSshPassword.Text : "",
        UseSsl = cbSsl.Checked,
        SqlQuery = tcConfiguration.SelectedTabPage == tpSql ? edtSql.Text : string.Empty
    };

    internal override void ImportData()
    {
        HideException();
        ImportDbProvider.Initialize(GetConfigurationModel());
        if (ImportDbProvider.CheckConnect() && lstTables.SelectedIndex >= 0)
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
            NotifyError("You should set correct credentials to access to MS SQL server and select table for importing", null);
        }
    }

    private void ClearControls()
    {
        lstTables.DataSource = null;
        cbeDatebase.Properties.Items.Clear();
        cbeDatebase.SelectedIndex = -1;
        FillGridWithDataSource(null);
    }

    private void btnRefreshServers_Click(object sender, EventArgs e)
    {
        HideException();
        ClearControls();
        tpSql.Enabled = false;
        ImportDbProvider.Initialize(GetConfigurationModel());
        if (ImportDbProvider.CheckConnect())
        {
            cbeDatebase.Properties.Items.Clear();
            var items = ImportDbProvider.GetDatabaseList();
            if (items != null && items.Count > 0)
            {
                cbeDatebase.Properties.Items.AddRange(items);
                cbeDatebase.SelectedIndex = 0;
            }
            tpSql.Enabled = true;
        }
    }

    private void lstTables_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(lstTables.Text))
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
                ClearControls();
            }
        }
    }

    private void cbeDatebase_EditValueChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(cbeDatebase.Text))
        {
            HideException();
            ImportDbProvider.Initialize(GetConfigurationModel());
            if (ImportDbProvider.CheckConnect())
            {
                lstTables.DataSource = null;
                lstTables.DataSource = ImportDbProvider.GetDatabaseTables();
                tpSql.Enabled = true;
            }
            else
            {
                ClearControls();
            }
        }
    }
    
    private void btnLoadSql_Click(object sender, EventArgs e)
    {
        using (var dlg = new OpenFileDialog())
        {
            dlg.Filter = "SQL files (*.sql)|*.sql|All files (*.*)|*.*";
            dlg.Title = "Open SQL Script";
            dlg.CheckFileExists = true;
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    HideException();
                    // Load as plain text into RichEditControl
                    edtSql.LoadDocument(dlg.FileName, DevExpress.XtraRichEdit.DocumentFormat.PlainText);
                    // Re-apply syntax highlighting service (in case document recreated)
                    InitSqlEditor();
                }
                catch (Exception ex)
                {
                    NotifyError($"Cannot load file '{Path.GetFileName(dlg.FileName)}'", ex);
                }
            }
        }
    }

    private void btnSaveSql_Click(object sender, EventArgs e)
    {
        using (var dlg = new SaveFileDialog())
        {
            dlg.Filter = "SQL files (*.sql)|*.sql|All files (*.*)|*.*";
            dlg.Title = "Save SQL Script";
            dlg.AddExtension = true;
            dlg.DefaultExt = "sql";
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    HideException();
                    // Save as plain text from RichEditControl
                    edtSql.SaveDocument(dlg.FileName, DevExpress.XtraRichEdit.DocumentFormat.PlainText);
                    MessageBox.Show("Saved");
                }
                catch (Exception ex)
                {
                    NotifyError($"Cannot save file '{Path.GetFileName(dlg.FileName)}'", ex);
                }
            }
        }
    }

    private void cbUseSsh_CheckedChanged(object sender, EventArgs e)
    {
        txtSshServer.Enabled = txtSshLogin.Enabled = txtSshPassword.Enabled = cbUseSsh.Checked;
    }

    private void btnSqlValidate_Click(object sender, EventArgs e)
    {
        HideException();
        ImportDbProvider.Initialize(GetConfigurationModel());
        if (ImportDbProvider.CheckConnect())
        {
            var (isValid, errorMessage) = ImportDbProvider.ValidateSql();
            if (isValid)
            {
                MessageBox.Show("Valid");
            }
            else
            {
                NotifyError("SQL is not valid", new Exception(errorMessage));
            }
        }
        else
        {
            ClearControls();
        }
    }

    private void btnSqlPreview_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(edtSql.Text))
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
                ClearControls();
            }
        }
    }

    private void tcConfiguration_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
    {
        FillGridWithDataSource(null);
        ctxMenu.Visible = tcConfiguration.SelectedTabPage == tpConnection;
        lbSelectTableOrSql.Text = tcConfiguration.SelectedTabPage == tpConnection ? Resources.CAPTION_SELECTTABLEORSQL : Resources.UI_CAPTION_USESQLEDITOR;
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
            gcSshInfo.Height = 51;
            this.Height = 785;
            gcSshInfo.CustomHeaderButtons[0].Properties.Visible = true;
            gcSshInfo.CustomHeaderButtons[1].Properties.Visible = false;
        }
        else
        {
            gcSshInfo.Height = 166;
            this.Height = 903;
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