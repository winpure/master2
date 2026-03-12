using DevExpress.LookAndFeel;
using WinPure.Configuration.Enums;
using WinPure.Configuration.Models.Configuration;


namespace WinPure.CleanAndMatch.Support;

internal partial class frmConfiguration : XtraForm
{
    private readonly IConfigurationService _configurationService;
    private readonly ILanguageService _languageService;
    private readonly IWpLogger _logger;
    private readonly IConnectionSettingsService _connectionSettingsService;

    public frmConfiguration(IConfigurationService configurationService,
        ILanguageService languageService,
        IWpLogger logger,
        IConnectionSettingsService connectionSettingsService)
    {
        _logger = logger;
        _connectionSettingsService = connectionSettingsService;
        _languageService = languageService;
        _configurationService = configurationService;

        InitializeComponent();
        Localization();

        if (Program.CurrentProgramVersion != ProgramType.CamEntAd)
        {
            //tpAddressVerification.PageVisible = false;
            lbAddressDatafile.Enabled = false;
            txtAddressDatafile.Enabled = false;
        }

        var languageDictionary = _languageService.GetLanguageList().Where(x => x.Value == LanguageEnum.en || x.Value == LanguageEnum.bp || x.Value == LanguageEnum.de || x.Value == LanguageEnum.sp);
        foreach (var lang in languageDictionary)
        {
            cbLanguage.Properties.Items.Add(lang.Key);
        }
        var currentLanguage = _languageService.ProgramLanguage;
        var currentLanguageName = languageDictionary.First(x => x.Value == currentLanguage).Key;
        cbLanguage.EditValue = currentLanguageName;

        foreach (var edDb in Enum.GetValues(typeof(EntityResolutionDatabase)).Cast<EntityResolutionDatabase>())
        {
            cbErDatabase.Properties.Items.Add(edDb.ToString());
        }

        LoadConfiguration();
        RefreshConnectionSettings();
    }

    private void Localization()
    {
        btnCancel.Text = Resources.UI_CANCEL;
        btnSaveConfiguration.Text = Resources.UI_SAVE;
        Text = Resources.UI_SETTINGS2;
        cbMatchReport.Text = Resources.UI_CAPTION_GENERATEMATCHREPORT;
        cbAllowUndo.Text = Resources.UI_ALLOWUNDO;
        cbStartupWindow.Text = Resources.UI_MAINFORM_STARTUPWINDOW;
        lbDataFile.Text = Resources.UI_CAPTION_DATAFILE;
        lbLanguage.Text = Resources.UI_MAINFORM_LANGUAGE;
        lbPreviewLimit.Text = Resources.UI_CAPTION_PREVIEWLIMIT;
        lbMergeSeparator.Text = Resources.UI_MAINFORM_MERGESEPARATOR;
        lbSplitSeparator.Text = Resources.UI_MAINFORM_SPLITSEPARATOR;
        lbMatchingLevel.Text = Resources.UI_CAPTION_FUZZYTHRESHOLD;
        lbAddressDatafile.Text = Resources.UI_CAPTION_ADDRESSDATAFILES;
        cbShowNull.Text = Resources.UI_CAPTION_SHOWNULL;
        cbCheckForUpdate.Text = Resources.UI_CAPTION_AUTOCHECKUPDATE;
        tpSavedSettings.Text = Resources.UI_CAPTION_SAVEDSETTINGS;
        lbEntityAiCaption.Text = Resources.UI_CAPTION_ENTITYAILOCATION;
        LbErDatabase.Text = Resources.UI_DATABASE;
        lbErConnection.Text = Resources.UI_CONNECTION;
        lbErThreads.Text = Resources.UI_THREADS;
        cbErExtendedLog.Text = Resources.UI_CAPTION_EXTENDEDLOGS;

        cbAutoSelectMasterRecords.Properties.Caption = Resources.UI_CAPTION_AUTOSELECTMASTERRECORD;
        cbAutoSelectAiMasterRecords.Properties.Caption = Resources.UI_CAPTION_AUTOSELECTMASTERRECORD;
        
        cbIncludeEmpty.Text = Resources.UI_CAPTION_IGNOREEMPTY;
        cbIncludeNull.Text = Resources.UI_UCMAINMATCHFORM_IGNORENULLVALUES;

        cbMasterRecordsType.Properties.Items.Add(Resources.UI_MASTERRECORD_MOSTRELEVANT);
        cbMasterRecordsType.Properties.Items.Add(Resources.UI_MASTERRECORD_MOSTPOPULATED);
        cbMasterRecordsType.SelectedIndex = 0;
        cbMasterRecordsType.Refresh();
    }

    private void LoadConfiguration()
    {
        var configuration = _configurationService.Configuration;

        cbStartupWindow.Checked = configuration.ShowStartScreen;
        txtDatabaseFile.Text = configuration.ProjectDatabase.Path;
        cbShowNull.Checked = configuration.ShowNullValues;
        cbCheckForUpdate.Checked = configuration.CheckForUpdates;

        sePreviewLimit.Value = configuration.PreviewRowCount;
        txtMergeSeparator.Text = configuration.CleanMergeSeparator;
        txtSplitSeparator.Text = configuration.CleanSplitSeparator;
        cbAllowUndo.Checked = configuration.AllowUndo;
        cbShowOnStartup.Checked = configuration.ShowStartupForm;
        cbUseCleansingAi.Checked = configuration.UseCleansingAi;

        cbMatchReport.Checked = configuration.GenerateMatchReport;
        seMatchLevel.Value = configuration.DefaultFuzzyLevel;
        cbUseMixedRules.Checked = configuration.UseMixedRules;
        cbIncludeNull.Checked = configuration.IncludeNullValues;
        cbIncludeEmpty.Checked = configuration.IncludeEmptyValues;


        txtAddressDatafile.Text = configuration.UsAddressVerificationDataFolder;

        txtEntityResolutionPath.Text = configuration.ErSettings.DataFolder;
        cbErDatabase.SelectedIndex = (int)configuration.ErSettings.Database;
        cbErConnection.Text = configuration.ErSettings.ConnectionString;
        seErThreads.Value = configuration.ErSettings.MaxDegreeOfParallelism;
        cbErExtendedLog.Checked = configuration.ErSettings.EnableDebugLogs;

        cbAutoSelectMasterRecords.Checked = configuration.AutoSetMasterRecord;
        cbAutoSelectAiMasterRecords.Checked = configuration.AutoSetAiMasterRecord;
        cbMasterRecordsType.SelectedIndex = configuration.AutoSetMasterRecordType;
        cbMasterRecordsType.Refresh();
    }

    private void CbErDatabase_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cbErDatabase.SelectedIndex == (int)EntityResolutionDatabase.Internal)
        {
            cbErConnection.Enabled = false;
            cbErConnection.Properties.Items.Clear();
            cbErConnection.Text = "";
            seErThreads.Value = 6;
            seErThreads.Enabled = false;
        }
        else
        {
            var source = (EntityResolutionDatabase)cbErDatabase.SelectedIndex switch
            {
                EntityResolutionDatabase.Postgres => ExternalSourceTypes.Postgres,
                EntityResolutionDatabase.SqlServer => ExternalSourceTypes.SqlServer,
                EntityResolutionDatabase.Internal => ExternalSourceTypes.SqLite,
                _ => throw new NotImplementedException(),
            };

            var connectionService = WinPureConfigurationDependency.Resolve<IConnectionSettingsService>();
            var connections = connectionService.Get(source);
            cbErConnection.Properties.Items.Clear();
            cbErConnection.Properties.Items.AddRange(connections.Select(x => x.Name).ToList());
            cbErConnection.SelectedIndex = 0;
            cbErConnection.RefreshEditValue();
            cbErConnection.Enabled = true;
            seErThreads.Enabled = true;
            seErThreads.Value = Environment.ProcessorCount * 3;
        }
    }

    private void RefreshConnectionSettings()
    {
        gridSavedSettings.DataSource = null;
        gridSavedSettings.DataSource = _connectionSettingsService.Get();
        gridSavedSettings.Refresh();
    }

    private bool SaveConfiguration()
    {
        var configuration = _configurationService.Configuration;

        if (string.IsNullOrEmpty(txtSplitSeparator.Text))
        {
            InformationDisplay(Resources.MESSAGE_SPLITSEPARATORNOTEMPTY, Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION, MessagesType.Error, null);
            return false;
        }

        configuration.ShowStartScreen = cbStartupWindow.Checked;
        configuration.ShowNullValues = cbShowNull.Checked;
        configuration.CheckForUpdates = cbCheckForUpdate.Checked;
        configuration.ShowStartupForm = cbShowOnStartup.Checked;

        configuration.PreviewRowCount = Convert.ToInt32(sePreviewLimit.Value);
        configuration.CleanMergeSeparator = txtMergeSeparator.Text;
        configuration.CleanSplitSeparator = txtSplitSeparator.Text;
        configuration.AllowUndo = cbAllowUndo.Checked;
        configuration.UseCleansingAi = cbUseCleansingAi.Checked;

        configuration.GenerateMatchReport = cbMatchReport.Checked;
        configuration.DefaultFuzzyLevel = seMatchLevel.Value;
        configuration.IncludeNullValues = cbIncludeNull.Checked;
        configuration.IncludeEmptyValues = cbIncludeEmpty.Checked;
        configuration.UseMixedRules = cbUseMixedRules.Checked;

        if (string.Compare(txtDatabaseFile.Text, configuration.ProjectDatabase.Path, StringComparison.InvariantCultureIgnoreCase) != 0)
        {
            configuration.ProjectDatabase.Path = txtDatabaseFile.Text;
            InformationDisplay(Resources.MESSAGE_APPLYDATABASEPATH, Resources.MESSAGE_DATABASEPATHCHANGE, MessagesType.Information, null);
        }

        configuration.UsAddressVerificationDataFolder = txtAddressDatafile.Text;
        if (configuration.ErSettings.DataFolder != txtEntityResolutionPath.Text)
        {
            if (!SystemHelper.IsAdministrator())
            {
                MessageBox.Show("You can change MatchAI Datafiles Location only under Administration account", Resources.MESSAGE_MESSAGEDIALOG_WARNING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                void _changePathVariableFunction(EnvironmentVariableTarget targetNode)
                {
                    var path = Environment.GetEnvironmentVariable("Path", targetNode).Split(';').ToList();
                    var existingPath = path.FirstOrDefault(x => x.Contains(configuration.ErSettings.DataFolder));
                    if (existingPath != null)
                    {
                        var existingPathIndex = path.IndexOf(existingPath);
                        if (existingPathIndex >= 0)
                        {
                            path[existingPathIndex] = existingPath.Replace(configuration.ErSettings.DataFolder, txtEntityResolutionPath.Text);
                        }
                    }
                    else
                    {
                        path.Add($"{txtEntityResolutionPath.Text}\\api\\lib");
                    }

                    var pathVariable = string.Join(";", path);
                    Environment.SetEnvironmentVariable("Path", pathVariable, targetNode);
                }

                _changePathVariableFunction(EnvironmentVariableTarget.User);
                _changePathVariableFunction(EnvironmentVariableTarget.Machine);

                configuration.ErSettings.DataFolder = txtEntityResolutionPath.Text;
            }
        }
        configuration.ErSettings.MaxDegreeOfParallelism = Convert.ToInt32(seErThreads.Value);
        configuration.ErSettings.Database = (EntityResolutionDatabase)cbErDatabase.SelectedIndex;
        configuration.ErSettings.ConnectionString = configuration.ErSettings.Database == EntityResolutionDatabase.Internal ? "" : cbErConnection.Text;
        configuration.ErSettings.EnableDebugLogs = cbErExtendedLog.Checked;
        
        configuration.AutoSetMasterRecord = cbAutoSelectMasterRecords.Checked;
        configuration.AutoSetAiMasterRecord = cbAutoSelectAiMasterRecords.Checked;
        configuration.AutoSetMasterRecordType = cbMasterRecordsType.SelectedIndex;

        SetLanguage();
        _configurationService.SaveConfiguration();
        return true;
    }

    private void SetLanguage()
    {
        try
        {
            var currentLanguage = _languageService.ProgramLanguage;
            var selectLanguage = cbLanguage.EditValue.ToString();
            var langDict = _languageService.GetLanguageList();
            var newLang = langDict[selectLanguage];
            if (currentLanguage == newLang)
            {
                return;
            }

            if (MessageBox.Show(Resources.MESSAGE_DO_YOU_WANT_CHANGE_LANGUAGE, Resources.MESSAGE_QUESTION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _languageService.SetProgramLanguage(Program.CurrentProgramCultureInfo, newLang);
                InformationDisplay(Resources.MESSAGE_YOU_SHOULD_RESTART_APP, "", MessagesType.Information, null);
            }
            else
            {
                cbLanguage.EditValue = langDict.First(x => x.Value == currentLanguage).Key;
            }
        }
        catch (Exception ex)
        {
            _logger.Debug("CHANGE LANGUAGE FAIL", ex);
            InformationDisplay(Resources.EXCEPTION_LANGUAGE_SET_FAIL, "", MessagesType.Error, ex);
        }
    }

    private void btnSaveConfiguration_Click(object sender, EventArgs e)
    {
        if (SaveConfiguration())
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    private void InformationDisplay(string message, string caption, MessagesType mType, Exception ex)
    {
        if (string.IsNullOrEmpty(caption))
        {
            switch (mType)
            {
                case MessagesType.Information:
                    caption = Resources.MESSAGE_MESSAGEDIALOG_INFORMATION_CAPTION;
                    break;
                case MessagesType.Warning:
                    caption = Resources.MESSAGE_MESSAGEDIALOG_WARNING_CAPTION;
                    break;
                case MessagesType.Error:
                    caption = Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION;
                    break;
                default:
                    caption = Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION;
                    break;
            }
        }

        if (ex == null)
        {
            _logger.Warning(message);
        }
        else
        {
            _logger.Error(message);
        }

        XtraMessageBox.Show($"{message}  {ex?.Message}", caption);
    }

    private void txtDatabaseFile_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        var folderDialog = new FolderBrowserDialog
        {
            SelectedPath = _configurationService.Configuration.ProjectDatabase.Path,
            ShowNewFolderButton = true
        };

        if (folderDialog.ShowDialog() == DialogResult.OK)
        {
            txtDatabaseFile.Text = folderDialog.SelectedPath;
        }
    }

    private void txtAddressDatafile_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        var folderDialog = new FolderBrowserDialog
        {
            SelectedPath = _configurationService.Configuration.UsAddressVerificationDataFolder,
            ShowNewFolderButton = false
        };

        if (folderDialog.ShowDialog() == DialogResult.OK)
        {
            txtAddressDatafile.Text = folderDialog.SelectedPath;
        }
    }

    private void helpButton_Click(object sender, EventArgs e)
    {
        if (sender is Control control)
        {
            var chapter = (HelpPageChapter)Enum.Parse(typeof(HelpPageChapter), control.Tag.ToString());
            UserManualHelper.OpenHelpPage(chapter);
        }
    }

    private void labelControl1_Click(object sender, EventArgs e)
    {
        UserLookAndFeel.Default.SetSkinStyle(SkinStyle.WXICompact);
    }


    private void btnDeleteSavedConfigurationValue_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        if (MessageBox.Show(Resources.MESSAGE_EDITDICTIONARYFORM_CONFIRM_DELETE_LIBRARY, Resources.MESSAGE_CONFIRMATION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
        {
            if (gvSavedSettings.GetRow(gvSavedSettings.FocusedRowHandle) is ConnectionSettings rw)
            {
                _connectionSettingsService.Delete(rw.Id);
                RefreshConnectionSettings();
            }
        }
    }

    private void txtEntityResolutionPath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        var folderDialog = new FolderBrowserDialog
        {
            SelectedPath = _configurationService.Configuration.ErSettings.DataFolder,
            ShowNewFolderButton = false
        };

        if (folderDialog.ShowDialog() == DialogResult.OK)
        {
            txtEntityResolutionPath.Text = folderDialog.SelectedPath;
        }
    }

    private void cbIncludeEmpty_CheckedChanged(object sender, EventArgs e)
    {
        if (!cbIncludeEmpty.Checked)
        {             
            cbIncludeNull.Checked = false;
        }
    }

    private void cbIncludeNull_CheckedChanged(object sender, EventArgs e)
    {
        if (cbIncludeNull.Checked)
        {
            cbIncludeEmpty.Checked = true;
        }
    }
}