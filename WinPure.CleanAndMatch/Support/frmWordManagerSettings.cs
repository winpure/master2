using DevExpress.Utils.Extensions;
using DevExpress.XtraGrid.Views.Base;
using System.ComponentModel;
using System.Text;
using WinPure.Cleansing.Enums;
using WinPure.Cleansing.Models;

namespace WinPure.CleanAndMatch.Support;

//TODO check replacetype <==> replaceoption
internal partial class frmWordManagerSettings : XtraForm
{
    private readonly IConfigurationService _configuration;
    private readonly ThemeDetectionService _themeDetectionService;

    private string _columnName;
    private readonly IDataManagerService _service;
    private BackgroundWorker _dataLoader;
    private List<ReportResultData> _data;
    private BindingList<WordManagerSetting> _wmSettings;

    public frmWordManagerSettings(IConfigurationService configuration)
    {
        _configuration = configuration;
        InitializeComponent();
        Localization();
        _wmSettings = new BindingList<WordManagerSetting> { AllowEdit = true, AllowRemove = true, AllowNew = true };
        _service = WinPureUiDependencyResolver.Resolve<IDataManagerService>();

        _themeDetectionService = WinPureUiDependencyResolver.Resolve<ThemeDetectionService>();
        _themeDetectionService.SetReferenceControl(panelControl3);
    }

    private void Localization()
    {
        gridColumn1.Caption = Resources.UI_WORDMANAGERSETTINGSFORM_WORDS;
        gridColumn3.Caption = Resources.UI_WORDMANAGERSETTINGSFORM_REPLACEMENT;
        gridColumn5.Caption = Resources.UI_OPTION;
        gridColumn2.Caption = Resources.UI_WORDMANAGERSETTINGSFORM_TODELETE;
        gridColumn6.Caption = Resources.UI_WORDMANAGERSETTINGSFORM_WORDS;
        gridColumn7.Caption = Resources.UI_COUNT;

        btnCancel.Text = Resources.UI_CANCEL;
        btnOK.Text = Resources.UI_SAVE;
        mnuLoad.Text = Resources.UI_LOAD;
        mnuSave.Text = Resources.UI_SAVE;
        mnuClear.Text = Resources.UI_CLEAR;
        groupControl1.Text = Resources.UI_DISPLAY;
        groupControl2.Text = Resources.UI_WORDMANAGERLIST;
        Text = Resources.UI_WORDMANAGERSETTINGSFORM_WORDMANAGER;
    }

    public void Show(string columnName)
    {
        _columnName = columnName;
        labelColumnName.Text = string.Format(Resources.CAPTION_WORDMANAGERFORM_COLUMN, columnName);
        InitiateWordManagerValues();
        ShowDialog();
    }

    private void InitiateWordManagerValues()
    {
        gvValues.ShowLoadingPanel();

        _dataLoader = new BackgroundWorker();
        _dataLoader.DoWork += _dataLoader_DoWork;
        _dataLoader.RunWorkerCompleted += _dataLoader_RunWorkerCompleted;
        _dataLoader.RunWorkerAsync();
    }

    private void _dataLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        gridWordManager.DataSource = _wmSettings;
        UpdateValuesGrid();
        gvValues.HideLoadingPanel();
        _dataLoader = null;
    }

    private void _dataLoader_DoWork(object sender, DoWorkEventArgs e)
    {
        _data = _service.GetWordManagerColumnValues(_columnName);
        _service.GetWordManagerSettings(_columnName).ForEach(x =>
            _wmSettings.Add(new WordManagerSetting
            {
                ColumnName = _columnName,
                SearchValue = x.SearchValue,
                ReplaceValue = x.ReplaceValue,
                ReplaceType = x.ReplaceType,
                ToDelete = x.ToDelete
            }));
    }

    private void UpdateValuesGrid()
    {
        gridValues.DataSource = null;
        if (_data != null && _data.Any())
        {
            if (cbValues.Checked)
            {
                gridValues.DataSource = _data;
            }
            else
            {
                var words =
                    _data.Select(x => new
                        {
                            words = x.Description.Split(' '),
                            x.RecordValue
                        })
                        .SelectMany(x => x.words, (c,w) =>  new {Cnt = c.RecordValue, word = w})
                        .Where(x => !string.IsNullOrEmpty(x.word) && !NameHelper.ProhibitedChars.Contains(x.word))
                        .GroupBy(x => x.word)
                        .Select(x => new ReportResultData
                        {
                            Description = x.Key,
                            RecordValue = x.Sum(s => s.Cnt)
                        }).ToList();
                gridValues.DataSource = words;
            }
        }
        gridValues.Refresh();
    }

    private void UpdateWordManagerGrid()
    {
        gridWordManager.RefreshDataSource();
    }

    private void UpdateSettings()
    {
        _wmSettings.ForEach(
            x =>
            {
                x.ColumnName = _columnName;
                if (x.ReplaceValue == null)
                {
                    x.ReplaceValue = "";
                }
            }
        );
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
        gvWordManager.PostEditor();
        gvWordManager.UpdateCurrentRow();
        UpdateSettings();
        _service.SaveWordManagerSettings(_wmSettings.ToList(), _columnName);
    }

    private void mnuClear_Click(object sender, EventArgs e)
    {
        var wmValuesToRemove = GetFilteredData<WordManagerSetting>(gvWordManager);
        foreach (var wmValue in wmValuesToRemove)
        {
            _wmSettings.Remove(wmValue);
        }
        UpdateWordManagerGrid();
    }

    private void mnuSave_Click(object sender, EventArgs e)
    {
        gvWordManager.PostEditor();
        gvWordManager.UpdateCurrentRow();
        UpdateSettings();
        if (!_wmSettings.Any())
        {
            MessageBox.Show(Resources.MESSAGE_WORDMANAGERFORM_DICTIONARY_IS_EMPTY, Resources.MESSAGE_MESSAGEDIALOG_WARNING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }
        
        var dlgSaveCsvFile = new SaveFileDialog();
        dlgSaveCsvFile.Title = Resources.DIALOG_EXPORT_DICTIONARY_CAPTION;
        dlgSaveCsvFile.FileName = "";
        dlgSaveCsvFile.AddExtension = true;

        dlgSaveCsvFile.Filter = Resources.DIALOG_CSVFILE_FORMAT;

        if (dlgSaveCsvFile.ShowDialog() == DialogResult.OK)
        {
            var filePath = dlgSaveCsvFile.FileName;

            FileHelper.SafeDeleteFile(filePath);

            var data = new List<string> { "SearchValue,ToReplace,ToDelete,ReplaceType" };
            foreach (var sett in _wmSettings)
            {
                var ln = $"{sett.SearchValue.Replace(",", "")},{sett.ReplaceValue.Replace(",", "")},{sett.ToDelete},{(int)sett.ReplaceType}";
                data.Add(ln);
            }
            File.WriteAllLines(filePath, data, Encoding.UTF8);
        }
    }

    private void mnuLoad_Click(object sender, EventArgs e)
    {
        var dlgSelectTextFile = new OpenFileDialog();
        dlgSelectTextFile.Title = Resources.DIALOG_IMPORT_WORDMANAGER_CAPTION;
        dlgSelectTextFile.FileName = "";
        dlgSelectTextFile.CheckFileExists = true;
        dlgSelectTextFile.Filter = Resources.DIALOG_CSVFILE_FORMAT;


        if (_configuration.Configuration.FirstWM)
        {
            _configuration.Configuration.FirstWM = false;
            _configuration.SaveConfiguration();
            dlgSelectTextFile.InitialDirectory = _configuration.Configuration.WordManagerSamplePath;
        }

        if (dlgSelectTextFile.ShowDialog() == DialogResult.OK)
        {
            try
            {
                string[] lines = File.ReadAllLines(dlgSelectTextFile.FileName, Encoding.UTF8);
                if (lines[0] == "SearchValue,ToReplace,ToDelete,ReplaceType")
                {
                    var dict = new BindingList<WordManagerSetting>();
                    for (int i = 1; i < lines.Length; i++)
                    {
                        var vals = lines[i].Split(',');
                        if (vals.Length != 4)
                        {
                            MessageBox.Show(Resources.EXCEPTION_WORDMANAGERDICTIONARY_FILE_WRONG_FORMAT,
                                Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        dict.Add(new WordManagerSetting
                        {
                            ColumnName = _columnName,
                            SearchValue = vals[0],
                            ReplaceValue = vals[1],
                            ToDelete = Convert.ToBoolean(vals[2]),
                            ReplaceType = (WordManagerReplaceType)Convert.ToInt32(vals[3])
                        });
                    }

                    _wmSettings.Clear();
                    _wmSettings = dict;
                    gridWordManager.DataSource = _wmSettings;
                    UpdateWordManagerGrid();
                }
                else
                {
                    MessageBox.Show(Resources.EXCEPTION_WORDMANAGERDICTIONARY_FILE_WRONG_FORMAT,
                        Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                MessageBox.Show(Resources.EXCEPTION_WORDMANAGERDICTIONARY_FILE_WRONG_FORMAT, Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }

    private void cbValues_CheckedChanged(object sender, EventArgs e)
    {
        UpdateValuesGrid();
    }

    private void gvValues_DoubleClick(object sender, EventArgs e)
    {
        if (gvValues.GetSelectedRows().Length == 0) return;

        if (gvValues.GetRow(gvValues.GetSelectedRows()[0]) is ReportResultData rrd && _wmSettings.All(x => x.SearchValue != rrd.Description))
        {
            _wmSettings.Add(new WordManagerSetting
            {
                ColumnName = _columnName,
                SearchValue = rrd.Description,
                ReplaceValue = "",
                ReplaceType = WordManagerReplaceType.WholeWord,
                ToDelete = false
            });
            UpdateWordManagerGrid();
        }
    }

    private void btnAddAllValues_Click(object sender, EventArgs e)
    {
        var values = GetFilteredData<ReportResultData>(gvValues);

        if (values == null) return;
        values.Where(x => !_wmSettings.Select(s => s.SearchValue).Contains(x.Description))
            .ForEach(
                x =>
                    _wmSettings.Add(new WordManagerSetting
                    {
                        ColumnName = _columnName,
                        SearchValue = x.Description,
                        ReplaceValue = "",
                        ReplaceType = WordManagerReplaceType.WholeWord,
                        ToDelete = false
                    }));

        UpdateWordManagerGrid();
    }

    public List<T> GetFilteredData<T>(ColumnView view)
    {
        List<T> resp = new List<T>();
        int currentRowHandle = view.GetVisibleRowHandle(0);
        while (currentRowHandle >= 0)
        {
            var value = (T) view.GetRow(currentRowHandle);
            if (value != null)
            {
                resp.Add(value);
            }
            currentRowHandle = view.GetNextVisibleRow(currentRowHandle);
        }
        return resp;
    }

    private void btnRemoveValue_Click(object sender, EventArgs e)
    {
        if (gvWordManager.GetSelectedRows().Length == 0) return;

        var rrd = gvWordManager.GetRow(gvWordManager.GetSelectedRows()[0]) as WordManagerSetting;

        _wmSettings.Remove(rrd);

        UpdateWordManagerGrid();
    }

    private void gvWordManager_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
    {
        var rw = (e.Row as WordManagerSetting);
        if (rw == null)
        {
            return;
        }
        if (string.IsNullOrEmpty(rw.SearchValue))
        {
            e.Valid = false;
        }
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.WordManager);
    }

    private void gvWordManager_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
        {
            btnRemoveValue_Click(null, null);
        }
    }

    private void frmWordManagerSettings_Load(object sender, EventArgs e)
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
}