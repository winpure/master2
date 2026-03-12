using WinPure.Configuration.Models.Configuration;

namespace WinPure.CleanAndMatch.Support;

public partial class frmProperCaseConfiguration : XtraForm
{
    private readonly IProperCaseSettingsService _properCaseSettingsService;

    public frmProperCaseConfiguration(IProperCaseSettingsService properCaseSettingsService)
    {
        _properCaseSettingsService = properCaseSettingsService;
        InitializeComponent();
        Localization();
        LoadConfiguration(_properCaseSettingsService.GetProperCaseSettings());
    }

    private void Localization()
    {
        btnExportConfiguration.Text = Resources.UI_EXPORT;
        btnImportConfiguration.Text = Resources.UI_IMPORT;
        btnCancel.Text = Resources.UI_CANCEL;
        btnSaveConfiguration.Text = Resources.UI_SAVE;
        Text = Resources.UI_MAINFORM_PROPERCASESETTINGS;
        lbDelimeters.Text = Resources.UI_PROPERCASE_DELIMITERS;
        lbExceptions.Text = Resources.UI_PROPERCASE_EXCEPTIONS;
        lbPrefix.Text = Resources.UI_PROPERCASE_PREFIX;
        mnuLoad.Text = Resources.UI_LOAD;
        mnuSave.Text = Resources.UI_SAVE;
    }

    private void txtAddException_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        lbExceptionList.Items.Add(txtAddException.Text);
        txtAddException.Text = "";
    }

    private void txtAddPrefix_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        lbPrefixList.Items.Add(txtAddPrefix.Text);
        txtAddPrefix.Text = "";
    }

    private void btnSaveConfiguration_Click(object sender, EventArgs e)
    {
        _properCaseSettingsService.SaveProperCaseSettings(GetConfiguration());
    }

    private ProperCaseConfiguration GetConfiguration()
    {
        var settings = new ProperCaseConfiguration();
        settings.Delimeters = txtDelimeters.Text;
        settings.ExceptionList = new List<string>();
        foreach (var item in lbExceptionList.Items)
        {
            settings.ExceptionList.Add(item.ToString());
        }
        settings.PrefixList = new List<string>();
        foreach (var item in lbPrefixList.Items)
        {
            settings.PrefixList.Add(item.ToString());
        }
        return settings;
    }

    private void LoadConfiguration(ProperCaseConfiguration configuration)
    {
        txtDelimeters.Text = configuration.Delimeters;
        lbExceptionList.Items.Clear();
        lbExceptionList.Items.AddRange(configuration.ExceptionList.ToArray());
        lbPrefixList.Items.Clear();
        lbPrefixList.Items.AddRange(configuration.PrefixList.ToArray());
    }

    private void listBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyValue == (int)Keys.Delete)
        {
            var listBox = (ListBoxControl)sender;
            listBox.Items.Remove(listBox.SelectedItem);
        }
    }

    private void txtAddException_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            txtAddException_ButtonClick(sender, null);
        }
    }

    private void txtAddPrefix_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            txtAddPrefix_ButtonClick(sender, null);
        }
    }

    private void helpButton_Click(object sender, EventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.ProperCaseSettings);
    }

    private void btnExportConfiguration_Click(object sender, EventArgs e)
    {
        var dlgSaveCsvFile = new SaveFileDialog
        {
            Title = Resources.DIALOG_EXPORT_MATRIX_CAPTION //TODO
        };
        dlgSaveCsvFile.FileName = "ProperCaseConfiguration.json";
        dlgSaveCsvFile.AddExtension = true;

        dlgSaveCsvFile.Filter = Resources.DIALOG_JSONFILE_FORMAT;

        if (dlgSaveCsvFile.ShowDialog() == DialogResult.OK)
        {
            var dataManagerService = WinPureUiDependencyResolver.Resolve<IDataManagerService>();
            dataManagerService.SaveProperCaseConfiguration(dlgSaveCsvFile.FileName, GetConfiguration());
        }
    }

    private void btnImportConfiguration_Click(object sender, EventArgs e)
    {
        var dlgSelectTextFile = new OpenFileDialog
        {
            Title = Resources.DIALOG_IMPORT_MATRIX_CAPTION,//TODO
            FileName = "",
            CheckFileExists = true,
            Filter = Resources.DIALOG_JSONFILE_FORMAT
        };

        if (dlgSelectTextFile.ShowDialog() != DialogResult.OK)
        {
            return;
        }
        try
        {
            var dataManagerService = WinPureUiDependencyResolver.Resolve<IDataManagerService>();
            var configuration = dataManagerService.LoadProperCaseConfiguration(dlgSelectTextFile.FileName);
            LoadConfiguration(configuration);
        }
        catch
        {
            MessageBox.Show(Resources.EXCEPTION_MATRIX_CANNOT_BE_LOADED, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}