using WinPure.Matching.Models.Support;
using WinPure.Matching.Services;

namespace WinPure.CleanAndMatch.MatchResultProcessing;

//TODO 
internal partial class frmMatchResultMerge : XtraForm
{
    private readonly IRepresentationService _representationService;
    private DataTable _updateOptions;

    public List<MergeMatchResultSetting> MergeSettings
    {
        get
        {
            if (_updateOptions == null)
            {
                return null;
            }
            var updateSettings = new List<MergeMatchResultSetting>();
            foreach (DataRow row in _updateOptions.Rows)
            {
                var setting = new MergeMatchResultSetting
                {
                    FieldName = row["FieldName"].ToString(),
                    OnlyEmpty = Convert.ToBoolean(row["OnlyEmpty"]),
                    UpdateField = Convert.ToBoolean(row["UpdateOption"]),
                    KeepAllValues = Convert.ToBoolean(row["SaveAllValues"]),
                };
                updateSettings.Add(setting);
            }

            return updateSettings;
        }
    }

    public frmMatchResultMerge(IRepresentationService representationService)
    {
        _representationService = representationService;
        InitializeComponent();
        Localization();
    }

    public DialogResult ShowDialog(DataTable matchData, int masterRecordCount)
    {
        _updateOptions = _representationService.GetMergeMatchResultOptionsTable(matchData);
        gridUpdate.DataSource = _updateOptions;
        gridUpdate.Refresh();

        lbMasterRecords.Text = string.Format(Resources.MESSAGE_MASTER_RECORD_COUNT, masterRecordCount);

        return ShowDialog();
    }

    private void Localization()
    {
        btnUpdate.Text = Resources.UI_MERGE;
        btnCancel.Text = Resources.UI_CANCEL;
        gridColumn1.Caption = Resources.UI_UPDATEMATCHRESULTFORM_COLUMNNAME;
        gridColumn3.Caption = Resources.UI_MATCHRESULTMERGE_UPDATE_FIELD;
        gridColumn2.Caption = Resources.UI_MATCHRESULTMERGE_ONLYEMPTY;
        gridColumn4.Caption = Resources.UI_MATCHRESULTMERGE_KEEPALLVALUES;
        Text = Resources.UI_MATCHRESULTMERGE_MERGE_OPTION;
    }

    private void cbMarkAllEmpty_CheckedChanged(object sender, System.EventArgs e)
    {
        var cbCtrl = sender as CheckEdit;
        if (cbCtrl == null)
        {
            return;
        }
        var fld = cbCtrl.Tag.ToString();
        foreach (DataRow row in _updateOptions.Rows)
        {
            row[fld] = cbCtrl.Checked;
        }
    }

    private void pictureBox1_Click(object sender, System.EventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.MergeRecords);
    }
}