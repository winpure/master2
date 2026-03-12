using WinPure.Matching.Models.Support;
using WinPure.Matching.Services;

namespace WinPure.CleanAndMatch.MatchResultProcessing;

//TODO
internal partial class frmUpdateMatchResult : XtraForm
{
    private readonly IRepresentationService _matchRepresentationService;
    private DataTable _updateOptions;

    internal List<UpdateMatchResultSetting> UpdateSettings
    {
        get
        {
            if (_updateOptions == null)
            {
                return null;
            }
            var valTypeArr = EnumExtension.GetDisplayNameDictionary<UpdateOperationType>();
            var updateSettings = new List<UpdateMatchResultSetting>();
            foreach (DataRow row in _updateOptions.Rows)
            {
                var setting = new UpdateMatchResultSetting
                {
                    FieldName = row["FieldName"].ToString(),
                    Operation = valTypeArr[row["ValueType"].ToString()],
                    OnlyEmpty = Convert.ToBoolean(row["UpdateOption"]),
                };

                updateSettings.Add(setting);
            }

            return updateSettings;
        }
    }

    public frmUpdateMatchResult(IRepresentationService matchRepresentationService)
    {
        _matchRepresentationService = matchRepresentationService;
        InitializeComponent();
        Localization();

        var opt = EnumExtension.GetDisplayNameDictionary<UpdateOperationType>();
        repositoryItemComboBox1.Items.AddRange(opt.Keys);
        cbUpdateWith.Properties.Items.AddRange(opt.Keys);
        cbUpdateWith.SelectedIndex = 0;
        cbUpdateWith.Refresh();
        cbUpdateWith.SelectedIndexChanged += CbUpdateWith_SelectedIndexChanged;
    }

    public DialogResult ShowDialog(DataTable matchData, int masterRecordCount)
    {
        _updateOptions = _matchRepresentationService.GetUpdateMatchResultOptionsTable(matchData);
        gridUpdate.DataSource = _updateOptions;
        gridUpdate.Refresh();

        lbMasterRecords.Text = string.Format(Resources.MESSAGE_MASTER_RECORD_COUNT, masterRecordCount);

        return ShowDialog();
    }

    private void Localization()
    {
        btnUpdate.Text = Resources.UI_UPDATE;
        btnCancel.Text = Resources.UI_CANCEL;
        gridColumn1.Caption = Resources.UI_UPDATEMATCHRESULTFORM_COLUMNNAME;
        gridColumn2.Caption = Resources.UI_UPDATEMATCHRESULTFORM_UPDATEWITH;
        gridColumn3.Caption = Resources.UI_UPDATEMATCHRESULTFORM_UPDATEMPTYVALUES;
        Text = Resources.UI_UPDATEMATCHRESULTFORM_UPDATEOVERWRITEDATA;
    }


    private void CbUpdateWith_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        foreach (DataRow row in _updateOptions.Rows)
        {
            row["ValueType"] = cbUpdateWith.Text;
        }
    }

    private void cbMarkAllEmpty_CheckedChanged(object sender, System.EventArgs e)
    {
        foreach (DataRow row in _updateOptions.Rows)
        {
            row["UpdateOption"] = cbMarkAllEmpty.Checked;
        }
    }

    private void pictureBox1_click(object sender, System.EventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.UpdateOverwrite);
    }
}