namespace WinPure.CleanAndMatch.Integration.Import;

internal partial class frmImportFromFileDatabase : BaseDatabaseImportForm
{
    private readonly IConfigurationService _configuration;

    public frmImportFromFileDatabase(IWpLogger logger, IConnectionSettingsService settingsService, IConfigurationService configuration) : base(logger, settingsService)
    {
        InitializeComponent();
        _configuration = configuration;
    }

    internal override void Localization()
    {
        base.Localization();
        groupBox1.Text = Resources.UI_CONNECTIONINFORMATION;
        groupBox2.Text = Resources.UI_CONNECTFILE;
        groupControl2.Text = Resources.UI_IMPORTACCESSFORM_ACCESSDATABASE;
        groupBox3.Text = Resources.UI_TABLES;
        rgAuthType.Properties.Items[1].Description = Resources.UI_WITHPASSWORD;
        rgAuthType.Properties.Items[0].Description = Resources.UI_WITHOUTPASSWORD;
    }

    internal override void SetDefaultConfiguration()
    {
        rgAuthType.SelectedIndex = 0;
        rgAuthType_SelectedIndexChanged(null, null);

        // for testing
    }

    internal override void LoadConfiguration()
    {
        HideException();
        if (_connectionSettings is { Settings: SqlImportExportOptions options })
        {
            txtPassword.Text = options.Password;
            rgAuthType.SelectedIndex = options.IntegrateSecurity ? 0 : 1;
            btnFilePath.Text = options.DatabaseFile;
            RefreshData();
            var tableIndex = lstTables.FindString(options.TableName);
            lstTables.SelectedIndex = tableIndex;
        }
    }

    internal override object GetConfigurationModel() => new SqlImportExportOptions
    {
        TableName = lstTables.Text,
        UserName = "",
        Password = txtPassword.Text,
        IntegrateSecurity = rgAuthType.SelectedIndex == 0,
        DatabaseFile = btnFilePath.Text
    };

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

    private void btnFilePath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        var dlgSelectTextFile = new OpenFileDialog
        {
            Title = string.Format(Resources.UI_IMPORTFROMFILE, ImportDbProvider.DisplayName),
            FileName = "",
            CheckFileExists = true,
            Filter = ImportDbProvider.SourceType == ExternalSourceTypes.Access
                ? Resources.DIALOG_ACCESS_FORMAT
                : "SqLite database|*.db"
        };

        if (_configuration.Configuration.FirstImport)
        {
            _configuration.Configuration.FirstImport = false;
            _configuration.SaveConfiguration();
            dlgSelectTextFile.InitialDirectory = _configuration.Configuration.SampleFolderPath;
        }

        if (dlgSelectTextFile.ShowDialog() == DialogResult.OK)
        {
            btnFilePath.Text = dlgSelectTextFile.FileName;
            RefreshData();
        }
    }

    private void RefreshData()
    {
        HideException();
        if ((rgAuthType.SelectedIndex == 1 && txtPassword.Text != "") || rgAuthType.SelectedIndex == 0)
        {
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

    private void btnFilePath_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Space)
        {
            btnFilePath.PerformClick(btnFilePath.Properties.Buttons.First());
        }
    }

    private void rgAuthType_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtPassword.Enabled = rgAuthType.SelectedIndex == 1;
        RefreshData();
    }
}