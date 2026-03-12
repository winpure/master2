namespace WinPure.CleanAndMatch.Integration.Import;

internal partial class frmImportText : BaseImportForm, IImportForm
{
    private readonly IConfigurationService _configuration;

    public frmImportText(IWpLogger logger, IConnectionSettingsService settingsService, IConfigurationService configuration) : base(logger, settingsService)
    {
        InitializeComponent();
        cmbCodePage.Properties.DataSource = FileHelper.GetTextEncoding();
        cmbCodePage.EditValue = 1252;
        cmbCodePage.Refresh();
        cmbDateOrder.SelectedIndex = 0;
        cmbDateOrder.Refresh();
        cmbTextQ.SelectedIndex = 0;
        cmbTextQ.Refresh();

        cmbTextQ.SelectedIndexChanged += TryToPreview;
        cmbDateOrder.SelectedIndexChanged += TryToPreview;
        cmbCodePage.EditValueChanged += TryToPreview;
        _configuration = configuration;
    }

    internal override void Localization()
    {
        base.Localization();
        groupControl1.Text = Resources.UI_IMPORTTEXTFORM_FIELDDELIMITER;
        radioOther.Properties.Caption = Resources.UI_IMPORTTEXTFORM_OTHER;
        radioSpace.Properties.Caption = Resources.UI_IMPORTTEXTFORM_SPACE;
        radioComma.Properties.Caption = Resources.UI_IMPORTTEXTFORM_COMMA;
        radioSemicolon.Properties.Caption = Resources.UI_IMPORTTEXTFORM_SEMICOLON;
        radioTab.Properties.Caption = Resources.UI_IMPORTTEXTFORM_TAB;
        chkFirstRow.Properties.Caption = Resources.UI_FIRSTROWCOLUMNNAMES;
        lblCodePage.Text = Resources.UI_IMPORTTEXTFORM_CODEPAGE;
        lblTextQ.Text = Resources.UI_IMPORTTEXTFORM_TEXTQUALIFIER;
        lblDecimalSymbol.Text = Resources.UI_IMPORTTEXTFORM_DECIMALSYMBOL;
        Label2.Text = Resources.UI_IMPORTTEXTFORM_DATEORDER;
        lblDateDelim.Text = Resources.UI_IMPORTTEXTFORM_DATEDELIMITER;
        lblTimeDelim.Text = Resources.UI_IMPORTTEXTFORM_TIMEDELIMITER;
        groupControl2.Text = Resources.UI_FILE;
        Text = Resources.UI_IMPORTTEXTFORM_IMPORTFROMTEXTFILE;
    }

    internal override void ImportData()
    {
        ImportProvider.Initialize(GetOptions());
        if (ImportProvider.GetPreview() != null)
        {
            var frmSelectFields = WinPureUiDependencyResolver.Resolve<frmSelectFields>();
            if (frmSelectFields.ShowDialog(ImportProvider.SelectFields()) == DialogResult.OK)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
        else
        {
            NotifyError(Resources.EXCEPTION_YOUSHOULDSELECETCORRECTPARAMETER, null);
        }
    }

    internal override object GetConfigurationModel() => GetOptions();
    private TextImportExportOptions GetOptions()
    {
        try
        {
            var _options = new TextImportExportOptions
            {
                DecimalSymbol = Convert.ToChar(txtDecimalSymbol.Text),
                DateDelimiter = Convert.ToChar(txtDateDelim.Text),
                DateOrder = cmbDateOrder.Text,
                FieldDelimiter = Convert.ToChar(txtFieldDelim.Text),
                TextQualifier = cmbTextQ.Text,
                FirstRowContainNames = chkFirstRow.Checked,
                TimeDelimiter = Convert.ToChar(txtTimeDelim.Text),
                CodePage = (int)cmbCodePage.EditValue,
                FilePath = btnFilePath.Text
            };
            return _options;
        }
        catch (Exception ex)
        {
            NotifyError("Wrong parameters for import file.", ex);
            return null;
        }
    }

    private void radioTab_CheckedChange(object sender, EventArgs e)
    {
        txtFieldDelim.ReadOnly = !radioOther.Checked;
        if (radioComma.Checked) txtFieldDelim.Text = ",";
        if (radioSemicolon.Checked) txtFieldDelim.Text = ";";
        if (radioSpace.Checked) txtFieldDelim.Text = @" ";
        if (radioTab.Checked) txtFieldDelim.Text = '\t'.ToString();
    }

    private void TryToPreview(object sender, EventArgs e)
    {
        HideException();
        if (string.IsNullOrEmpty(txtFieldDelim.Text) || string.IsNullOrEmpty(btnFilePath.Text))
        {
            return;
        }
        ImportProvider.Initialize(GetOptions());
        var prev = ImportProvider.GetPreview();
        if (prev != null)
        {
            FillGridWithDataSource(prev);
        }
    }

    private void btnFilePath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        var dlgSelectTextFile = new OpenFileDialog
        {
            Title = "Import from a File into Table",
            FileName = "",
            CheckFileExists = true,
            Filter = "Text Files (Delimited) (*.txt, *.csv)|*.txt;*.csv"
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
            TryToPreview(null, null);
        }
    }

    private void btnFilePath_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Space)
        {
            btnFilePath.PerformClick(btnFilePath.Properties.Buttons.First());
        }
    }
}