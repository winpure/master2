using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Services;

namespace WinPure.CleanAndMatch.Integration.Import;

internal partial class frmImportFromSQLServer : BaseDatabaseImportForm
{
    public frmImportFromSQLServer(IWpLogger logger, IConnectionSettingsService settingsService) : base(logger, settingsService)
    {
        InitializeComponent();
        InitSqlEditor();
        sePort.EditValue = null;
    }

    internal override void Localization()
    {
        base.Localization();
        groupBox1.Text = Resources.UI_CONNECTIONINFORMATION;
        tpConnection.Text = Resources.UI_CONNECTIONINFORMATION;
        labelControl4.Text = Resources.UI_DATABASE;
        groupBox2.Text = Resources.UI_LOGONSERVER;
        labelControl3.Text = Resources.UI_PASSWORD;
        labelControl2.Text = Resources.UI_LOGIN;
        rgAuthType.Properties.Items[0].Description = Resources.UI_IMPORTMSSQLFORM_USEWINDOWS;
        rgAuthType.Properties.Items[1].Description = Resources.UI_IMPORTMSSQLFORM_USESQL;
        labelControl1.Text = Resources.UI_IMPORTMSSQLFORM_SERVERNAME;
        groupBox3.Text = Resources.UI_TABLES;
        btnSaveSql.Text = Resources.UI_SAVE;
        btnLoadSql.Text = Resources.UI_LOAD;
        btnSqlPreview.Text = Resources.UI_RUN;
        btnSqlValidate.Text = Resources.UI_VALIDATE;
        lbSelectTableOrSql.Text = Resources.CAPTION_SELECTTABLEORSQL;
        btnRefreshServers.Text = Resources.CAPTION_CONNECT;
    }

    internal override void ApplyThemeToDerivedControls(bool isDarkTheme)
    {
        if (tcConfiguration == null) return;

        tcConfiguration.BackColor = isDarkTheme
            ? Color.FromArgb(32, 32, 32)
            : Color.FromArgb(244, 244, 244);
    }

    internal override void SetDefaultConfiguration()
    {
        sePort.EditValue = ImportDbProvider.SourceType switch
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

        if (ImportDbProvider.SourceType == ExternalSourceTypes.Oracle)
        {
            rgAuthType.SelectedIndex = 1;
            rgAuthType.Enabled = false;
        }
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

            cbeDatabase.Text = options.DatabaseName;

            HighlightRowByTableName(options.TableName);
        }
    }

    internal override object GetConfigurationModel() => new SqlImportExportOptions
    {
        ServerAddress = txtServerName.Text,
        DatabaseName = cbeDatabase.Text,
        TableName = TableName,
        UserName = txtLogin.Text,
        Password = txtPassword.Text,
        IntegrateSecurity = rgAuthType.SelectedIndex == 0,
        Port = sePort.EditValue != null ? (int)sePort.Value : 0,
        SqlQuery = tcConfiguration.SelectedTabPage == tpSql ? edtSql.Text : string.Empty
    };

    internal override void ImportData()
    {
        HideException();
        ImportDbProvider.Initialize(GetConfigurationModel());
        if (ImportDbProvider.CheckConnect() && !string.IsNullOrEmpty(TableName))
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
            NotifyError(string.Format(Resources.EXCEPTION_SET_CORRECT_CREDENTIALS, ImportDbProvider.DisplayName), null);
        }
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

    private void ClearControls()
    {
        gridTableList.DataSource = null;
        cbeDatabase.Properties.Items.Clear();
        cbeDatabase.SelectedIndex = -1;
        FillGridWithDataSource(null);
    }

    private void btnRefreshServers_Click(object sender, EventArgs e)
    {
        HideException();
        tpSql.Enabled = false;
        if (ImportDbProvider.SourceType == ExternalSourceTypes.Db2)
        {
            var database = cbeDatabase.Text;
            ClearControls();
            cbeDatabase.Text = database;
        }
        else
        {
            ClearControls();
        }

        ImportDbProvider.Initialize(GetConfigurationModel());
        if (ImportDbProvider.CheckConnect())
        {
            if (ImportDbProvider.SourceType == ExternalSourceTypes.Db2)
            {
                gridTableList.DataSource = null;
                gridTableList.DataSource = ImportDbProvider.GetDatabaseTables().Select(x => new TableListData { TableName = x }).ToList();
                gridTableList.RefreshDataSource();
            }
            else
            {
                cbeDatabase.Properties.Items.Clear();
                var items = ImportDbProvider.GetDatabaseList();
                if (items != null && items.Count > 0)
                {
                    cbeDatabase.Properties.Items.AddRange(items);
                    cbeDatabase.SelectedIndex = 0;
                }
            }

            tpSql.Enabled = true;
        }
    }

    private void cbeDatabase_EditValueChanged(object sender, EventArgs e)
    {
        if (ImportDbProvider.SourceType == ExternalSourceTypes.Db2) return;

        if (!string.IsNullOrEmpty(cbeDatabase.Text))
        {
            HideException();
            ImportDbProvider.Initialize(GetConfigurationModel());
            if (ImportDbProvider.CheckConnect())
            {
                gridTableList.DataSource = null;
                gridTableList.DataSource = ImportDbProvider.GetDatabaseTables().Select(x => new TableListData { TableName = x }).ToList();
                gridTableList.RefreshDataSource();
                tpSql.Enabled = true;
            }
            else
            {
                ClearControls();
            }
        }
    }

    private void HighlightRowByTableName(string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
            return;

        for (var i = 0; i < gvTableList.RowCount; i++)
        {
            if (gvTableList.GetRow(i) is TableListData row && row.TableName != null &&
                row.TableName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                gvTableList.FocusedRowHandle = i;
                gvTableList.SelectRow(i);
                gvTableList.MakeRowVisible(i);
                break;
            }
        }
    }
    
    private string TableName =>
        gvTableList.GetRow(gvTableList.FocusedRowHandle) is TableListData data
            ? data.TableName
            : string.Empty;

    private void gvTableList_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(TableName))
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

    private void rgAuthType_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtLogin.Enabled = rgAuthType.SelectedIndex == 1;
        txtPassword.Enabled = rgAuthType.SelectedIndex == 1;
    }
}

class TableListData
{
    public string TableName { get; set; }
}