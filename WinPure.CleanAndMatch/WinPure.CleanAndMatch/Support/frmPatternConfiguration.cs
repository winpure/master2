using WinPure.Configuration.Models.Configuration;


namespace WinPure.CleanAndMatch.Support;

internal partial class frmPatternConfiguration : XtraForm
{
    private readonly IStatisticPatternService _service;
    private readonly IWpLogger _logger;

    public frmPatternConfiguration(IStatisticPatternService statisticPatternService, IWpLogger logger)
    {
        _service = statisticPatternService;
        _logger = logger;
        InitializeComponent();
        Localization();
    }

    private void Localization()
    {
        btnAdd.Text = Resources.UI_ADD;
        btnCancel.Text = Resources.UI_CANCEL;
        lbName.Text = Resources.UI_PATTERNNAME;
        lbFieldType.Text = Resources.CAPTION_FIELD_TYPE;
        lbPattern.Text = Resources.CAPTION_REGULAR_EXPRESSION;
        lbDescription.Text = Resources.CAPTION_DESCRIPTION;
        Text = Resources.CAPTION_PATTERNMANAGER;
        colName.Caption = Resources.UI_PATTERNNAME;
        colPattern.Caption = Resources.CAPTION_REGULAR_EXPRESSION;
        colDescription.Caption = Resources.CAPTION_DESCRIPTION;
        colFieldType.Caption = Resources.CAPTION_FIELD_TYPE;
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

    private void helpButton_Click(object sender, EventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.PatternManager);
    }

    private void frmPatternConfiguration_Load(object sender, EventArgs e)
    {
        SafeActionCall(RefreshData);
    }

    private void gvPatterns_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
    {
        var res = SafeActionCall(() =>
        {
            if (e.Row is StatisticPatternSetting dRow)
            {
                AsyncHelpers.RunSync(() => _service.AddOrUpdatePattern(dRow));
                RefreshData();
            }
        }, true);
        if (!string.IsNullOrEmpty(res))
        {
            e.Valid = false;
            e.ErrorText = res;
        }
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        var patternList = gridPatterns.DataSource as List<StatisticPatternSetting>;
        if (patternList.Any(x => x.Pattern == txtPattern.Text))
        {
            MessageBox.Show("The same Regex expression cannot be added twice.", Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }

        SafeActionCall(() =>
        {
            AsyncHelpers.RunSync(() => _service.AddOrUpdatePattern(new StatisticPatternSetting
            {
                Pattern = txtPattern.Text,
                Description = txtDescription.Text,
                FieldType = txtFieldType.Text,
                Name = txtName.Text
            }));

            txtPattern.Text = String.Empty;
            txtDescription.Text = String.Empty;
            txtFieldType.Text = String.Empty;
            txtName.Text = String.Empty;
            RefreshData();
        }, true);

    }

    private void repoBtnDeletePatternValue_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        if (MessageBox.Show(Resources.MESSAGE_EDITDICTIONARYFORM_CONFIRM_DELETE_LIBRARY, Resources.MESSAGE_CONFIRMATION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
        {
            SafeActionCall(() =>
            {
                if (gvPatterns.GetRow(gvPatterns.FocusedRowHandle) is StatisticPatternSetting rw)
                {
                    AsyncHelpers.RunSync(() => _service.Delete(rw.Id));
                    RefreshData();
                }
            });
        }
    }

    private void RefreshData()
    {
        var patternList = AsyncHelpers.RunSync(_service.GetAllPatterns);
        gridPatterns.DataSource = patternList;
        gridPatterns.RefreshDataSource();
    }
}