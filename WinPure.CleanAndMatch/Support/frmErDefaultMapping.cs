using WinPure.Configuration.Models.Configuration;
using WinPure.Configuration.Models.Database;
using WinPure.DataService.Senzing;

namespace WinPure.CleanAndMatch.Support;

internal partial class frmErDefaultMapping : XtraForm
{
    private readonly IEntityResolutionMappingSettingService _erMappingService;
    private readonly ISenzingService _senzingService;
    private readonly IWpLogger _logger;

    public frmErDefaultMapping(IEntityResolutionMappingSettingService erMappingService,
        ISenzingService senzingService,
        IWpLogger logger)
    {
        _erMappingService = erMappingService;
        _senzingService = senzingService;
        _logger = logger;
        InitializeComponent();
        Localization();
    }

    private void Localization()
    {
        btnCancel.Text = Resources.UI_CLOSE;
        btnAddNewErMapping.Text = Resources.UI_ADD;
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

    private void btnCancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void repoBtnDeleteDictionaryValue_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        if (MessageBox.Show(Resources.MESSAGE_EDITDICTIONARYFORM_CONFIRM_DELETE_LIBRARY, Resources.MESSAGE_CONFIRMATION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
        {
            SafeActionCall(() =>
            {
                if (gvMappingData.GetRow(gvMappingData.FocusedRowHandle) is EntityResolutionMappingEntity rw)
                {
                    if (rw.ConflictEntityTypes.Any() || rw.PrerequisiteEntityTypes.Any())
                    {
                        MessageBox.Show("You can not remove this system mapping", Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    _erMappingService.Delete(rw.Id);
                    RefreshMappingData();
                }
            });
        }
    }

    private void btnAddNewErMapping_Click(object sender, EventArgs e)
    {
        var newSettings = new EntityResolutionMappingSetting
        {
            DataColumnName = txtDataColumnName.Text.ToUpper(),
            EntityType = txtErType.Text.ToUpper(),
            ExactMatch = cbExactMatch.Checked,
            UsageGroup = txtLabel.Text,
            ConflictEntityTypes = new List<string>(),
            PrerequisiteEntityTypes = new List<string>()
        };
        _erMappingService.Add(newSettings);
        RefreshMappingData();
    }

    private void frmErDefaultMapping_Load(object sender, EventArgs e)
    {
        SafeActionCall(() =>
        {
            RefreshMappingData();
            var erTypes = _senzingService.GetFieldTypes();
            txtErType.Properties.Items.AddRange(erTypes.Where(x => !string.IsNullOrWhiteSpace(x.SystemName)).Select(x => x.SystemName).ToList());
        });
    }

    private void RefreshMappingData()
    {
        var erData = _erMappingService.GetAllEntities();
        gridMappingData.DataSource = null;
        gridMappingData.DataSource = erData;
        gridMappingData.Refresh();
    }

    private void helpButton_Click(object sender, EventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.ERAutoMapConfig);
    }
}