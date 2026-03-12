using DevExpress.XtraEditors.Controls;
using WinPure.Matching.Models.Support;

namespace WinPure.CleanAndMatch.MatchResultProcessing;

public partial class frmMatchResultDelete : XtraForm
{
    public frmMatchResultDelete()
    {
        InitializeComponent();
        Localization();
        var options = EnumExtension.GetDisplayNameDictionary<DeleteMatchResultSetting>();
        foreach (var opt in options)
        {
            rgDelete.Properties.Items.Add(new RadioGroupItem { Description = opt.Key, Tag = opt.Value, Value = opt.Value });
        }
        rgDelete.SelectedIndex = 0;
        rgDelete.Refresh();
    }

    private void Localization()
    {
        simpleButton1.Text = Resources.UI_DELETE;
        btnCancel.Text = Resources.UI_CANCEL;
        Text = Resources.UI_MATCHRESULTDELETEFORM_DELETEOPTIONS;
    }

    internal DeleteFromMatchResultSetting ResultOption => new DeleteFromMatchResultSetting
    {
        DeleteSetting = (DeleteMatchResultSetting)rgDelete.Properties.Items[rgDelete.SelectedIndex].Value
    };

    private void pictureBox1_click(object sender, System.EventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.Delete);
    }
}