namespace WinPure.CleanAndMatch.Integration.Import;

internal partial class frmImportFile : BaseImportForm
{
    private readonly IConfigurationService _configuration;
    
    public frmImportFile(IWpLogger logger, IConnectionSettingsService settingsService, IConfigurationService configuration) : base(logger, settingsService)
    {
        _configuration = configuration;
        InitializeComponent();
    }

    internal override void Localization()
    {
        base.Localization();
        groupControl2.Text = Resources.UI_FILE;
        Text = Resources.UI_IMPORTTEXTFORM_IMPORTFROMTEXTFILE;
    }

    internal override object GetConfigurationModel() => new TextImportExportOptions()
    {
        FilePath = btnFilePath.Text
    };

    internal override void ImportData()
    {
        ImportProvider.Initialize(GetConfigurationModel());
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

    private void TryToPreview(object sender, EventArgs e)
    {
        HideException();
        ImportProvider.Initialize(GetConfigurationModel());
        var previewTable = ImportProvider.GetPreview();

        if (previewTable != null)
        {
            FillGridWithDataSource(previewTable);
        }
    }

    private void btnFilePath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        var dlgSelectTextFile = new OpenFileDialog
        {
            Title = string.Format(Resources.UI_IMPORTFROMFILE, ImportProvider.DisplayName),
            FileName = "",
            CheckFileExists = true,
            Filter = ImportProvider.SourceType switch
            {
                ExternalSourceTypes.Json => "JSON Files|*.json",
                ExternalSourceTypes.Xml => "XML Files|*.xml",
                ExternalSourceTypes.Senzing => "JSON Files|*.json",
                ExternalSourceTypes.JSONL => "JSONL Files|*.jsonl",
                _ => "JSON Files|*.json"
            }
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