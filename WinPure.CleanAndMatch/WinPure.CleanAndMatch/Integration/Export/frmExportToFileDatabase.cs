namespace WinPure.CleanAndMatch.Integration.Export;

internal partial class frmExportToFileDatabase : BaseDatabaseExportForm
{
    public frmExportToFileDatabase(IWpLogger logger, IConnectionSettingsService settingsService) : base(logger, settingsService)
    {
        InitializeComponent();
    }

    internal override void Localization()
    {
        base.Localization();
        groupBox1.Text = Resources.UI_CONNECTIONINFORMATION;
        groupBox2.Text = Resources.UI_CONNECTFILE;
        labelControl1.Text = Resources.UI_EXPORTACCESSFORM_TABLENAMETOEXPORT;
        labelControl3.Text = Resources.UI_PASSWORD;
        rgAuthType.Properties.Items[1].Description = Resources.UI_WITHPASSWORD;
        rgAuthType.Properties.Items[0].Description = Resources.UI_WITHOUTPASSWORD;
        groupControl2.Text = Resources.UI_IMPORTACCESSFORM_ACCESSDATABASE;
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
            txtTableName.Text = options.TableName;
            RefreshData();
        }
    }

    internal override object GetConfigurationModel() => new SqlImportExportOptions
    {
        UserName = "",
        Password = txtPassword.Text,
        IntegrateSecurity = rgAuthType.SelectedIndex == 0,
        TableName = txtTableName.Text,
        DatabaseFile = btnFilePath.Text
    };

    internal override void ExportData()
    {
        if (!ExportDbProvider.CheckConnect())
        {
            MessageBox.Show(Resources.DIALOG_IOFORM_SET_CONNECTION_TO_DATABASE_FOR_EXPORT);
        }
        else if (string.IsNullOrEmpty(txtTableName.Text))
        {
            MessageBox.Show(Resources.DIALOG_IOFORM_SET_TABLE_NAME_FOR_EXPORT);
        }
        else
        {
            ExportDbProvider.Initialize(GetConfigurationModel());
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    private void btnFilePath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        var dlgSelectTextFile = new OpenFileDialog
        {
            Title = string.Format(Resources.UI_IMPORTFROMFILE, ExportDbProvider.DisplayName),
            FileName = "",
            CheckFileExists = true,
            Filter = ExportDbProvider.SourceType == ExternalSourceTypes.Access
                ? Resources.DIALOG_ACCESS_FORMAT
                : "SqLite database|*.db"
        };

        if (dlgSelectTextFile.ShowDialog() == DialogResult.OK)
        {
            btnFilePath.Text = dlgSelectTextFile.FileName;
            RefreshData();
        }
    }

    private void RefreshData()
    {
        HideException();
        ExportDbProvider.Initialize(GetConfigurationModel());
        ExportDbProvider.CheckConnect();
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