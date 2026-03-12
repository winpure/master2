using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using WinPure.Cleansing.Models;

namespace WinPure.CleanAndMatch.Support;

internal partial class frmCleansingRegex : XtraForm
{
    private string _columnName;
    private readonly IDataManagerService _service;
    private List<RegexConfigurationSetting> _data;
    private readonly ThemeDetectionService _themeDetectionService;

    public frmCleansingRegex()
    {
        InitializeComponent();
        Localization();
        _themeDetectionService = WinPureUiDependencyResolver.Resolve<ThemeDetectionService>();
        _service = WinPureUiDependencyResolver.Resolve<IDataManagerService>();
    }

    private void Localization()
    {
        btnAdd.Text = Resources.UI_ADD;
        btnCancel.Text = Resources.UI_CANCEL;
        btnOK.Text = Resources.UI_SAVE;
        mnuLoad.Text = Resources.UI_LOAD;
        mnuSave.Text = Resources.UI_SAVE;
        mnuClear.Text = Resources.UI_CLEAR;
        groupControl2.Text = Resources.UI_CAPTION_REGEXMANAGER;
        lbExpression.Text = Resources.UI_CAPTION_REGEX_EXPRESSION;
        lbReplacemen.Text = Resources.UI_CAPTION_REGEX_REPLACEMENT;
        lbDescription.Text = Resources.CAPTION_DESCRIPTION;
        colValue.Caption = Resources.UI_CAPTION_REGEX_EXPRESSION;
        colReplacement.Caption = Resources.UI_CAPTION_REGEX_REPLACEMENT;
        colDescription.Caption = Resources.CAPTION_DESCRIPTION;
        Text = Resources.UI_CAPTION_REGEXMANAGER;
    }

    public DialogResult Show(string columnName, string configuration)
    {
        _columnName = columnName;
        _data = string.IsNullOrWhiteSpace(configuration) ? new() : JsonConvert.DeserializeObject<List<RegexConfigurationSetting>>(configuration).OrderBy(x => x.Id).ToList();
        InitiateRegexValues();
        return ShowDialog();
    }

    private void InitiateRegexValues()
    {
        gridRegexConfiguration.DataSource = _data;
        gridRegexConfiguration.RefreshDataSource();
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Close();
    }

    private void mnuClear_Click(object sender, EventArgs e)
    {
        _data.Clear();
        InitiateRegexValues();
    }

    private void mnuSave_Click(object sender, EventArgs e)
    {

        if (!_data.Any())
        {
            MessageBox.Show(Resources.MESSAGE_WORDMANAGERFORM_DICTIONARY_IS_EMPTY, Resources.MESSAGE_MESSAGEDIALOG_WARNING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }

        var dlgSaveJsonFile = new SaveFileDialog();
        dlgSaveJsonFile.Title = Resources.DIALOG_EXPORT_DICTIONARY_CAPTION;
        dlgSaveJsonFile.FileName = "";
        dlgSaveJsonFile.AddExtension = true;

        dlgSaveJsonFile.Filter = Resources.DIALOG_JSONFILE_FORMAT;

        if (dlgSaveJsonFile.ShowDialog() == DialogResult.OK)
        {
            var filePath = dlgSaveJsonFile.FileName;

            FileHelper.SafeDeleteFile(filePath);

            var data = JsonConvert.SerializeObject(_data);
            File.WriteAllText(filePath, data, Encoding.UTF8);
        }
    }

    private void mnuLoad_Click(object sender, EventArgs e)
    {
        var dlgSelectTextFile = new OpenFileDialog();
        dlgSelectTextFile.Title = Resources.DIALOG_IMPORT_WORDMANAGER_CAPTION;
        dlgSelectTextFile.FileName = "";
        dlgSelectTextFile.CheckFileExists = true;
        dlgSelectTextFile.Filter = Resources.DIALOG_JSONFILE_FORMAT;


        if (dlgSelectTextFile.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var lines = File.ReadAllText(dlgSelectTextFile.FileName, Encoding.UTF8);
                _data = JsonConvert.DeserializeObject<List<RegexConfigurationSetting>>(lines);
                InitiateRegexValues();
            }
            catch
            {
                MessageBox.Show(Resources.EXCEPTION_WORDMANAGERDICTIONARY_FILE_WRONG_FORMAT, Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }

    private void frmCleansingRegex_Load(object sender, EventArgs e)
    {
        if (_themeDetectionService.IsDarkTheme())
        {
            menuStrip1.BackColor = Color.FromArgb(64, 64, 64);
        }
        else
        {
            menuStrip1.BackColor = Color.FromArgb(244, 244, 244);
        }
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtExpression.Text))
            return;

        try
        {
            var ptrn = new Regex(txtExpression.Text);
        }
        catch (ArgumentException)
        {
            MessageBox.Show("REGEX expression is not valid", Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION, MessageBoxButtons.OK);
        }

        var id = _data.Any() ? _data.Max(x => x.Id) + 1 : 1;

        _data.Add(new RegexConfigurationSetting
        {
            Id = id,
            Expression = txtExpression.Text,
            Replacement = txtReplacement.Text,
            Description = txtDescription.Text
        });

        txtExpression.Text = string.Empty;
        txtReplacement.Text = string.Empty;
        txtDescription.Text = string.Empty;
        InitiateRegexValues();
    }

    private void btnOK_Click_1(object sender, EventArgs e)
    {
        var data = JsonConvert.SerializeObject(_data);
        _service.SaveCleanSettings(_columnName, "RE_Expression", data);
    }

    private void repoBtnDeleteRegexValue_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        if (MessageBox.Show(Resources.MESSAGE_EDITDICTIONARYFORM_CONFIRMATION_DELETE_VALUE, Resources.MESSAGE_CONFIRMATION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
        {

            if (gvRegexConfiguration.GetRow(gvRegexConfiguration.FocusedRowHandle) is RegexConfigurationSetting rw)
            {
                _data.Remove(rw);
                InitiateRegexValues();
            }
        }
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.RegexManager);
    }
}