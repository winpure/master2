using System.ComponentModel;

namespace WinPure.CleanAndMatch.Integration.Import;

internal partial class frmImportExcel : BaseDatabaseImportForm
{
    private readonly IConfigurationService _configuration;
    private List<string> _tableList;
    private DataTable _previewData;
    private BackgroundWorker _worker;

    public frmImportExcel(IWpLogger logger, IConnectionSettingsService settingsService, IConfigurationService configuration) : base(logger, settingsService, false)
    {
        _configuration = configuration;
        InitializeComponent();
        _tableList = new List<string>();
    }

    internal override void Localization()
    {
        base.Localization();
        groupControl2.Text = Resources.UI_IMPORTEXCELFORM_EXCELFILE;
        labelControl1.Text = Resources.UI_IMPORTEXCELFORM_SHEETS;
        chkFirstRow.Properties.Caption = Resources.UI_FIRSTROWCOLUMNNAMES;
        Text = Resources.UI_IMPORTEXCELFORM_IMPORTFROMEXCEL;
        cbAnalyzeDataType.Text = Resources.UI_CAPTION_ANALYZEDATATYPE;
    }
    
    private void ClearControls()
    {
        lstTables.DataSource = null;
        FillGridWithDataSource(null);
    }

    internal override object GetConfigurationModel() => new ExcelImportExportOptions
    {
        TableName = lstTables.Text,
        FirstRowContainNames = chkFirstRow.Checked,
        FilePath = btnFilePath.Text,
        AnalyzeDataType = cbAnalyzeDataType.Checked
    };

    internal override void ImportData()
    {
        if (btnFilePath.Text == "" || lstTables.SelectedIndex < 0)
        {
            MessageBox.Show(Resources.MASSAGE_IOFORM_YOUSHOULDSELECTFILE);
        }
        else
        {
            ImportDbProvider.Initialize(GetConfigurationModel());
            if (ImportDbProvider.CheckConnect())
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
                MessageBox.Show(Resources.MASSAGE_IOFORM_YOUSHOULDSELECTFILE);
            }
        }
    }

    private void btnFilePath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        var dlgSelectTextFile = new OpenFileDialog
        {
            Title = Resources.DIALOG_IOFORM_IMPORTFROMEXCEL,
            FileName = "",
            CheckFileExists = true,
            Filter = Resources.DIALOG_EXCEL_FORMAT
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
            GetTables();
        }
    }

    private void GetTables()
    {
        HideException();
        ImportDbProvider.Initialize(GetConfigurationModel());
        if (ImportDbProvider.CheckConnect())
        {
            lstTables.DataSource = null;
            lstTables.SelectedIndexChanged -= lstTables_SelectedIndexChanged;
            ShowLoadingPanel();
            _worker = new BackgroundWorker();
            _worker.DoWork += Worker_DoWork;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            _worker.RunWorkerAsync();
        }
        else
        {
            ClearControls();
        }
    }

    private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        lstTables.DataSource = _tableList;

        FillGridWithDataSource(_previewData);
        HideLoadingPanel();
        lstTables.SelectedIndexChanged += lstTables_SelectedIndexChanged;
    }

    private void Worker_DoWork(object sender, DoWorkEventArgs e)
    {
        try
        {
            _tableList = ImportDbProvider.GetDatabaseTables();
            if (_tableList == null || !_tableList.Any()) return;
            ImportDbProvider.Initialize(new ExcelImportExportOptions
            {
                TableName = _tableList.First(),
                FirstRowContainNames = chkFirstRow.Checked,
                FilePath = btnFilePath.Text,
                AnalyzeDataType = cbAnalyzeDataType.Checked
            });
            _previewData = ImportDbProvider.GetPreview();
        }
        catch (Exception ex)
        {
            NotifyError("Could not open Excel file", ex);
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
                if (prev != null)
                {
                    FillGridWithDataSource(prev);
                }
            }
            else
            {
                ClearControls();
            }
        }
    }

    private void helpButton_Click(object sender, EventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.ImportExcel);
    }

    private void btnFilePath_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Space)
        {
            btnFilePath.PerformClick(btnFilePath.Properties.Buttons.First());
        }
    }
}