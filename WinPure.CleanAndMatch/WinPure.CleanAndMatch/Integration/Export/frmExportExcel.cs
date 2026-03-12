using WinPure.Integration.Export;

namespace WinPure.CleanAndMatch.Integration.Export;

internal partial class frmExportExcel : BaseExportForm
{
    public frmExportExcel(IWpLogger logger, IConnectionSettingsService settingsService) : base(logger, settingsService)
    {
        InitializeComponent();
    }

    internal override void Localization()
    {
        base.Localization();
        groupControl2.Text = Resources.UI_FILE;
        Text = Resources.UI_EXPORTEXCELFORM_EXPORTTOEXCEL;
        chkFirstRow.Properties.Caption = Resources.UI_FIRSTROWCOLUMNNAMES;
    }
    internal override object GetConfigurationModel() => new ExcelImportExportOptions
    {
        FirstRowContainNames = chkFirstRow.Checked,
        ExportWithNpoi = cbNpoiExport.Checked,
        FilePath = btnFilePath.Text
    };
    
    internal override void ExportData()
    {
        if (btnFilePath.Text == "")
        {
            NotifyError("You need to enter a destination filename", null);
        }
        else
        {
            ExportProvider = cbNpoiExport.Checked ? new ExcelExportUniversalProvider() : new ExcelExportProvider();
            SetServiceParameter(null, null);
            DialogResult = DialogResult.OK;
            Close();
        }

    }

    private void SetServiceParameter(object sender, EventArgs e)
    {
        ExportProvider.Initialize(GetConfigurationModel());
    }

    private void btnFilePath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        var dlgSaveCsvFile = new SaveFileDialog
        {
            Title = "Export data to Microsoft Excel file",
            FileName = "",
            AddExtension = true,
            Filter = "Microsoft Excel files (*.xlsx, *.xls)|*.xlsx;*.xls"
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