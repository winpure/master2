namespace WinPure.CleanAndMatch.Integration.Export;

internal partial class frmExportText : BaseExportForm
{
    public frmExportText(IWpLogger logger, IConnectionSettingsService settingsService) : base(logger, settingsService)
    {
        InitializeComponent();
        cmbCodePage.Properties.DataSource = FileHelper.GetTextEncoding();
        cmbCodePage.EditValue = 1252;
        cmbCodePage.Refresh();
        cmbDateOrder.SelectedIndex = 0;
        cmbDateOrder.Refresh();
        cmbTextQ.SelectedIndex = 0;
        cmbTextQ.Refresh();

        cmbTextQ.SelectedIndexChanged += SetServiceParameter;
        cmbDateOrder.SelectedIndexChanged += SetServiceParameter;
        cmbCodePage.EditValueChanged += SetServiceParameter;
    }

    internal override void Localization()
    {
        base.Localization();
        cbAddTime.Properties.Caption = Resources.UI_EXPORTTEXTFORM_ADDTIMETODATE;
        lblDecimalSymbol.Text = Resources.UI_IMPORTTEXTFORM_DECIMALSYMBOL;
        Label2.Text = Resources.UI_IMPORTTEXTFORM_DATEORDER;
        lblDateDelim.Text = Resources.UI_IMPORTTEXTFORM_DATEDELIMITER;
        lblTimeDelim.Text = Resources.UI_IMPORTTEXTFORM_TIMEDELIMITER;
        chkFirstRow.Properties.Caption = Resources.UI_FIRSTROWCOLUMNNAMES;
        lblCodePage.Text = Resources.UI_IMPORTTEXTFORM_CODEPAGE;
        lblTextQ.Text = Resources.UI_IMPORTTEXTFORM_TEXTQUALIFIER;
        groupControl1.Text = Resources.UI_IMPORTTEXTFORM_FIELDDELIMITER;
        radioOther.Properties.Caption = Resources.UI_IMPORTTEXTFORM_OTHER;
        radioSpace.Properties.Caption = Resources.UI_IMPORTTEXTFORM_SPACE;
        radioComma.Properties.Caption = Resources.UI_IMPORTTEXTFORM_COMMA;
        radioSemicolon.Properties.Caption = Resources.UI_IMPORTTEXTFORM_SEMICOLON;
        groupControl2.Text = Resources.UI_FILE;
        Text = Resources.UI_EXPORTTEXTFORM_EXPORTTODELIMITEDTEXT;
    }

    private void SetServiceParameter(object sender, EventArgs e)
    {
        ExportProvider.Initialize(GetOptions());
    }

    internal override object GetConfigurationModel() => GetOptions();

    internal override void ExportData()
    {
        if (btnFilePath.Text == "")
        {
            NotifyError("You should set file path for exporting", null);
        }
        else
        {
            SetServiceParameter(null, null);
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    private TextImportExportOptions GetOptions()
    {
        try
        {
            var options = new TextImportExportOptions
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
            return options;
        }
        catch (Exception ex)
        {
            NotifyError(Resources.EXCEPTION_SHOULDSETFILEPATHFOREXPORT, ex);
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

    private void btnFilePath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        var dlgSaveCsvFile = new SaveFileDialog
        {
            Title = "Export data to CSV file",
            FileName = "",
            AddExtension = true,
            Filter = "Text Files (Delimited) (*.csv)|*.csv"
        };


        if (dlgSaveCsvFile.ShowDialog() == DialogResult.OK)
        {
            btnFilePath.Text = dlgSaveCsvFile.FileName;
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