using System.Text;
using WinPure.Configuration.Models.Dictionary;

namespace WinPure.CleanAndMatch.Support;

internal partial class frmEditDictionary : XtraForm
{
    int _currentDictionaryId = -1;
    private readonly IDictionaryService _dictionaryService;
    private readonly IWpLogger _logger;
    private readonly ThemeDetectionService _themeDetectionService;


    public frmEditDictionary(ILicenseService licenseService, IDictionaryService dictionaryService, IWpLogger logger)
    {
        InitializeComponent();
        Localization();
        if (licenseService.IsDemo)
        {
            gridColumn1.OptionsColumn.ReadOnly =
                gridColumn3.OptionsColumn.ReadOnly = gridColumn4.OptionsColumn.ReadOnly = true;
        }

        _logger = logger;
        _dictionaryService = dictionaryService;

        _themeDetectionService = WinPureUiDependencyResolver.Resolve<ThemeDetectionService>();
        _themeDetectionService.SetReferenceControl(this);
    }

    private void Localization()
    {
        btnCancel.Text = Resources.UI_CLOSE;
        gridColumn1.Caption = Resources.UI_NAME;
        labelControl1.Text = Resources.UI_EDITDICTIONARYFORM_FARMATNAME;
        (btnAddNewDictionary.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_EDITDICTIONARYFORM_ADDNEWLIBRARY;
        btnAddNewDictionary.Text = Resources.UI_ADD;
        gridColumn3.Caption = Resources.UI_EDITDICTIONARYFORM_SEARCHVALUE;
        gridColumn4.Caption = Resources.UI_EDITDICTIONARYFORM_REPLACEVALUESHORT;
        labelColumnName.Text = Resources.UI_EDITDICTIONARYFORM_COMPANYTYPE;
        labelControl3.Text = Resources.UI_EDITDICTIONARYFORM_REPLACEVALUE;
        labelControl2.Text = Resources.UI_EDITDICTIONARYFORM_SEARCHVALUE;
        (btnAddNewDictionaryData.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_EDITDICTIONARYFORM_ADDNEWVALUE;
        btnAddNewDictionaryData.Text = Resources.UI_ADD;
        Text = Resources.UI_EDITDICTIONARYFORM_LIBRARYMANAGER;
        mnuLoad.Text = Resources.UI_LOAD;
        mnuSave.Text = Resources.UI_SAVE;
    }

    private void RefreshDictionaryNames(List<DictionaryHeader> headers)
    {
        gridDictionaryNames.DataSource = null;
        gridDictionaryNames.DataSource = headers;
        gridDictionaryNames.Refresh();
    }

    private void RefreshDictionaryData(List<DictionaryData> dData)
    {
        gridDictionaryData.DataSource = null;
        gridDictionaryData.DataSource = dData;
        gridDictionaryData.Refresh();
    }

    private string SafeActionCall(Action act, bool suppressMessage = false)
    {
        try
        {
            act();
            return "";
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null && ex.InnerException is WinPureBaseException)
            {
                ex = ex.InnerException;
            }
            _logger.Error("Dictionary error", ex);
            if (!suppressMessage)
            {
                MessageBox.Show(ex.Message);
            }
            return ex.Message;
        }
    }

    private void frmEditDictionary_Load(object sender, EventArgs e)
    {
        if (_themeDetectionService.IsDarkTheme())
        {
            menuStrip1.BackColor = Color.FromArgb(64, 64, 64);
        }
        else
        {
            menuStrip1.BackColor = Color.FromArgb(244, 244, 244);
        }

        SafeActionCall(() =>
        {
            var dictList = _dictionaryService.GetDictionaryList().Result;
            RefreshDictionaryNames(dictList);
        });
    }

    private void btnDeleteDictionaryValue_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        if (MessageBox.Show(Resources.MESSAGE_EDITDICTIONARYFORM_CONFIRM_DELETE_LIBRARY, Resources.MESSAGE_CONFIRMATION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
        {
            SafeActionCall(() =>
            {
                if (gvDictionaryData.GetRow(gvDictionaryData.FocusedRowHandle) is DictionaryData rw)
                {
                    var dict = _dictionaryService.DeleteDictionaryData(rw.Id, rw.DictionaryId).Result;
                    RefreshDictionaryData(dict);
                }
            });
        }
    }

    private void btnAddNewDictionaryData_Click(object sender, EventArgs e)
    {
        SafeActionCall(() =>
        {
            var data = _dictionaryService.AddNewDictionaryData(txtSearchValue.Text, txtReplaceValue.Text, _currentDictionaryId).Result;
            RefreshDictionaryData(data);
            txtSearchValue.Text = "";
            txtReplaceValue.Text = "";
        });
    }

    private void btnAddNewDictionary_Click(object sender, EventArgs e)
    {
        SafeActionCall(() =>
        {
            var dictList = _dictionaryService.AddNewDictionary(txtDictionaryName.Text).Result;
            txtDictionaryName.Text = "";
            RefreshDictionaryNames(dictList);
        });
    }

    private void gvDictionaryNames_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
    {
        if (gvDictionaryNames.GetRow(e.FocusedRowHandle) is DictionaryHeader rw)
        {
            _currentDictionaryId = rw.Id;
            var dict = _dictionaryService.GetDictionaryData(rw.Name).Result;
            labelColumnName.Text = rw.Name;
            RefreshDictionaryData(dict);
        }
    }

    private void repositoryItemButtonEdit1_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(Resources.MESSAGE_EDITDICTIONARYFORM_CONFIRMATION_DELETE_VALUE, Resources.MESSAGE_CONFIRMATION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
        {
            SafeActionCall(() =>
            {
                if (gvDictionaryNames.GetRow(gvDictionaryNames.FocusedRowHandle) is DictionaryHeader rw)
                {
                    var dict = _dictionaryService.DeleteDictionary(rw.Name).Result;
                    RefreshDictionaryNames(dict);
                }
            });
        }
    }

    private void gvDictionaryNames_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
    {
        var res = SafeActionCall(() =>
        {
            if (e.Row is DictionaryHeader dRow)
            {
                _dictionaryService.UpdateDictionary(dRow.Name, dRow.Id);
                e.Valid = true;
                RefreshDictionaryNames(_dictionaryService.GetDictionaryList().Result);
            }
        }, true);
        if (!string.IsNullOrEmpty(res))
        {
            e.Valid = false;
            e.ErrorText = res;
        }
    }

    private void gvDictionaryData_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
    {
        var res = SafeActionCall(() =>
        {
            if (e.Row is DictionaryData dRow)
            {
                _dictionaryService.UpdateDictionaryData(dRow.SearchValue, dRow.ReplaceValue, dRow.Id, dRow.DictionaryId);
                e.Valid = true;
                RefreshDictionaryData(_dictionaryService.GetDictionaryData(_currentDictionaryId).Result);
            }
        }, true);
        if (!string.IsNullOrEmpty(res))
        {
            e.Valid = false;
            e.ErrorText = res;
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void helpButton_Click(object sender, EventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.KnowledgeBase);
    }

    private async void mnuLoad_Click(object sender, EventArgs e)
    {
        var dlgSelectTextFile = new OpenFileDialog();
        dlgSelectTextFile.Title = Resources.DIALOG_IMPORT_KBLIBRARY_CAPTION;
        dlgSelectTextFile.FileName = "";
        dlgSelectTextFile.CheckFileExists = true;
        dlgSelectTextFile.Filter = Resources.DIALOG_CSVFILE_FORMAT;

        if (dlgSelectTextFile.ShowDialog() == DialogResult.OK)
        {
            try
            {
                string[] lines = File.ReadAllLines(dlgSelectTextFile.FileName, Encoding.UTF8);
                if (lines[0] == "\"SearchValue\";\"ReplaceValue\"")
                {
                    var dict = new List<DictionaryData>();
                    for (int i = 1; i < lines.Length; i++)
                    {
                        var vals = lines[i].Split(';');
                        if (vals.Length != 2)
                        {
                            MessageBox.Show(Resources.EXCEPTION_WORDMANAGERDICTIONARY_FILE_WRONG_FORMAT, Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (vals[0] == "\"\"") continue;

                        dict.Add(new DictionaryData
                        {
                            SearchValue = vals[0].Substring(1, vals[0].Length - 2),
                            ReplaceValue = vals[1].Length > 2 ? vals[1].Substring(1, vals[1].Length - 2) : String.Empty,
                        });
                    }

                    var dictionaryData = await _dictionaryService.AddNewDictionaryData(dict, _currentDictionaryId);
                    RefreshDictionaryData(dictionaryData);
                }
                else
                {
                    MessageBox.Show(Resources.EXCEPTION_WORDMANAGERDICTIONARY_FILE_WRONG_FORMAT, Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.EXCEPTION_WORDMANAGERDICTIONARY_FILE_WRONG_FORMAT, Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }

    private void mnuSave_Click(object sender, EventArgs e)
    {
        var data = gridDictionaryData.DataSource as List<DictionaryData>;
        if (data == null || data.Count == 0)
        {
            MessageBox.Show(Resources.MESSAGE_WORDMANAGERFORM_DICTIONARY_IS_EMPTY, Resources.MESSAGE_MESSAGEDIALOG_WARNING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var dlgSaveFile = new SaveFileDialog();
        dlgSaveFile.Title = Resources.DIALOG_EXPORT_DICTIONARY_CAPTION;
        dlgSaveFile.Filter = Resources.DIALOG_CSVFILE_FORMAT;
        dlgSaveFile.FileName = "dictionary.csv";

        if (dlgSaveFile.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var lines = new List<string>
                {
                    "\"SearchValue\";\"ReplaceValue\""
                };

                foreach (var entry in data)
                {
                    string search = $"\"{entry.SearchValue?.Replace("\"", "\"\"") ?? ""}\"";
                    string replace = $"\"{entry.ReplaceValue?.Replace("\"", "\"\"") ?? ""}\"";
                    lines.Add($"{search};{replace}");
                }

                File.WriteAllLines(dlgSaveFile.FileName, lines, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export error:\n\n{ex.Message}",
                    Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}