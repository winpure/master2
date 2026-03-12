namespace WinPure.CleanAndMatch.Integration.Export;

internal partial class frmExportFile : BaseExportForm
{
    public frmExportFile(IWpLogger logger, IConnectionSettingsService settingsService) : base(logger, settingsService)
    {
        InitializeComponent();
    }

    internal override void Localization()
    {
        base.Localization();
        groupControl2.Text = Resources.UI_FILE;
        Text = Resources.UI_EXPORTTEXTFORM_EXPORTTODELIMITEDTEXT;
    }

    private void SetServiceParameter(object sender, EventArgs e)
    {
        ExportProvider.Initialize(GetConfigurationModel());
    }

    internal override object GetConfigurationModel() => new TextImportExportOptions()
    {
        FilePath = btnFilePath.Text
    };

    internal override void ExportData()
    {
        if (btnFilePath.Text == "")
        {
            NotifyError(Resources.EXCEPTION_SHOULDSETFILEPATHFOREXPORT, null);
        }
        else
        {
            SetServiceParameter(null, null);
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    private void btnFilePath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        var dlgSaveCsvFile = new SaveFileDialog
        {
            Title = string.Format(Resources.UI_EXPORTTOFILE, ExportProvider.DisplayName),
            FileName = "",
            AddExtension = true,
            Filter = ExportProvider.SourceType == ExternalSourceTypes.Xml
                ? "XML Files|*.xml"
                : "JSON Files|*.json"
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