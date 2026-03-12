using System.ComponentModel;
using DevExpress.Utils.Extensions;
using WinPure.Matching.Models.Support;

namespace WinPure.CleanAndMatch.MatchResultProcessing;

public partial class frmSetMasterRecords : XtraForm
{
    private BindingList<MasterRecordRuleEditable> _rules;
    private List<MatchField> _fields;
    public frmSetMasterRecords()
    {
        InitializeComponent();
        Localization();
        bgRules.Expanded = false;
        _rules = new BindingList<MasterRecordRuleEditable>();
        gridRules.DataSource = _rules;
        gridRules.Refresh();
    }

    private void Localization()
    {
        btnCancel.Text = Resources.UI_CANCEL;
        btnOK.Text = Resources.UI_EXECUTE;
        rbFromTable.Text = Resources.UI_SETMASTERRECORDSFORM_SETMOSTPOPULATED;
        rbMostRelevant.Text = Resources.UI_SETMASTERRECORDSFORM_SELECTMOSTRELEVANT;
        labelControl1.Text = Resources.UI_SETMASTERRECORDSFORM_ASMASTER;
        Text = Resources.UI_SETMASTERRECORDSFORM_SETMASTERRECORDS;
        rbClearAll.Text = Resources.UI_SETMASTERRECORDSFORM_CLEARALL;

        cbAnyRule.Text = Resources.UI_SETMASTERRECORDSFORM_MEETANY;
        cbAllRules.Text = Resources.UI_SETMASTERRECORDSFORM_MEETALL;
        cbMrInAnyCase.Text = Resources.UI_SETMASTERRECORDSFORM_MRANYCASE;
        cbOnlyThisTable.Text = Resources.UI_SETMASTERRECORDSFORM_ONLY;
        bgRules.Caption = Resources.UI_SETMASTERRECORDSFORM_ADD_RULES;
        bgOption.Caption = Resources.UI_SETMASTERRECORDSFORM_ADDIT_OPTIONS;
    }

    public MasterRecordSettings MasterRecordSetting
    {
        get =>
            new MasterRecordSettings
            {
                RecordType = rbMostRelevant.Checked
                    ? MasterRecordType.MostRelevant
                    : (rbClearAll.Checked)
                        ? MasterRecordType.ClearAll
                        : MasterRecordType.MostPopulatedByTable,
                PreferredTable = cbMasterRecordMainTable.Text == "<Any>" ? "" : cbMasterRecordMainTable.Text,
                IsAllRules = cbAllRules.Checked,
                OnlyThisTable = cbOnlyThisTable.Checked,
                ApplyOptionsIfRuleGiveNothing = cbMrInAnyCase.Checked,
                Rules = _rules.Cast<MasterRecordRule>().ToList()
            };
        private set
        {
            rbFromTable.Checked = false;
            rbMostRelevant.Checked = false;
            rbClearAll.Checked = false;
            switch (value.RecordType)
            {
                case MasterRecordType.MostPopulatedByTable:
                    rbFromTable.Checked = true;
                    break;
                case MasterRecordType.MostRelevant:
                    rbMostRelevant.Checked = true;
                    break;
                case MasterRecordType.ClearAll:
                    rbFromTable.Checked = true;
                    value.RecordType = MasterRecordType.MostPopulatedByTable;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            navBarGroupControlContainer1.Enabled = !rbClearAll.Checked;
            cbMasterRecordMainTable.Text = string.IsNullOrEmpty(value.PreferredTable) ? "<Any>" : value.PreferredTable;
            cbAllRules.Checked = value.IsAllRules;
            cbOnlyThisTable.Checked = value.OnlyThisTable;
            cbMrInAnyCase.Checked = value.ApplyOptionsIfRuleGiveNothing;
            _rules = new BindingList<MasterRecordRuleEditable>(value.Rules.Select(x => new MasterRecordRuleEditable
            {
                Value = x.Value,
                FieldName = x.FieldName,
                FieldType = x.FieldType,
                Negate = x.Negate,
                RuleType = x.RuleType,
                RuleTypeName = x.RuleType switch
                {
                    MasterRecordRuleType.IsEqual => "Equal",
                    MasterRecordRuleType.IsContains => "Contains",
                    MasterRecordRuleType.IsEmpty => "Empty",
                    MasterRecordRuleType.IsMaximum => "Maximum",
                    MasterRecordRuleType.IsMinimum => "Minimum",
                    MasterRecordRuleType.IsLongest => "Longest",
                    MasterRecordRuleType.IsShortest => "Shortest",
                    MasterRecordRuleType.GreaterThan => "Greater than",
                    MasterRecordRuleType.Common => "Common",
                    _ => throw new ArgumentException()
                }
            }).ToList());
            gridRules.DataSource = _rules;
            gridRules.Refresh();
        }
    }

    public DialogResult Show(List<string> tables, List<MatchField> fields, MasterRecordSettings settings, bool showMostRelevant)
    {
        _rules.Clear();
        gridRules.RefreshDataSource();
        _fields = fields;
        cbMasterRecordMainTable.Properties.Items.Clear();
        cbMasterRecordMainTable.Properties.Items.Add("<Any>");
        cbMasterRecordMainTable.Properties.Items.AddRange(tables);
        cbMasterRecordMainTable.SelectedIndex = 0;
        cbMasterRecordMainTable.Refresh();

        cbFieldName.Items.Clear();
        cbFieldName.Items.AddRange(_fields.Select(x => x.ColumnName).ToList());

        if (settings != null)
        {
            MasterRecordSetting = settings;
        }

        if (_rules.Any())
        {
            bgRules.Expanded = true;
        }

        rbMostRelevant.Visible = showMostRelevant;
        ShowDialog();
        return DialogResult;
    }

    private void rbMostRelevant_Click(object sender, EventArgs e)
    {
        navBarGroupControlContainer1.Enabled = true;
        rbFromTable.Checked = false;
        cbMasterRecordMainTable.Enabled = false;
        rbMostRelevant.Checked = true;
        rbClearAll.Checked = false;
        cbOnlyThisTable.Enabled = false;
    }

    private void rbFromTable_Click(object sender, EventArgs e)
    {
        navBarGroupControlContainer1.Enabled = true;
        rbFromTable.Checked = true;
        cbMasterRecordMainTable.Enabled = true;
        rbMostRelevant.Checked = false;
        rbClearAll.Checked = false;
        cbOnlyThisTable.Enabled = true;
    }

    private void rbClearAll_Click(object sender, EventArgs e)
    {
        navBarGroupControlContainer1.Enabled = false;
        rbFromTable.Checked = false;
        cbMasterRecordMainTable.Enabled = false;
        rbMostRelevant.Checked = false;
        rbClearAll.Checked = true;
        cbOnlyThisTable.Enabled = false;
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.SetMasterRecords);
    }

    private void navBarControl1_GroupCollapsed(object sender, DevExpress.XtraNavBar.NavBarGroupEventArgs e)
    {
        ResizeForm();
    }

    private void ResizeForm()
    {
        var sz = panelControl1.Height + 121;
        if (bgRules.Expanded)
        {
            sz += navBarGroupControlContainer1.Height;
        }
        if (bgOption.Expanded)
        {
            navBarGroupControlContainer2.Height = 110;
            sz += navBarGroupControlContainer2.Height;
        }
        Height = sz;
        Invalidate();
    }

    private void navBarControl1_GroupExpanded(object sender, DevExpress.XtraNavBar.NavBarGroupEventArgs e)
    {
        ResizeForm();
    }

    private void cbAnyRule_CheckedChanged(object sender, EventArgs e)
    {
        if (((Control)sender).Name == "cbAllRules")
        {
            cbAnyRule.Checked = !cbAllRules.Checked;
        }
        else
        {
            cbAllRules.Checked = !cbAnyRule.Checked;
        }
    }

    private void frmSetMasterRecords_Load(object sender, EventArgs e)
    {
        ResizeForm();
    }

    private void gvRules_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
        {
            if (gvRules.GetSelectedRows().Length == 0) return;

            var rrd = gvRules.GetRow(gvRules.GetSelectedRows()[0]) as MasterRecordRuleEditable;

            _rules.Remove(rrd);

            gridRules.RefreshDataSource();
        }
    }

    private void gvRules_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
    {
        var rw = (e.Row as MasterRecordRuleEditable);
        if (rw == null) return;

        switch (rw.RuleTypeName)
        {
            case "Equal": rw.RuleType = MasterRecordRuleType.IsEqual; break;
            case "Contains": rw.RuleType = MasterRecordRuleType.IsContains; break;
            case "Empty": rw.RuleType = MasterRecordRuleType.IsEmpty; break;
            case "Maximum": rw.RuleType = MasterRecordRuleType.IsMaximum; break;
            case "Minimum": rw.RuleType = MasterRecordRuleType.IsMinimum; break;
            case "Longest": rw.RuleType = MasterRecordRuleType.IsLongest; break;
            case "Shortest": rw.RuleType = MasterRecordRuleType.IsShortest; break;
            case "Greater than": rw.RuleType = MasterRecordRuleType.GreaterThan; break;
            case "Common": rw.RuleType = MasterRecordRuleType.Common; break;
        }

        if (string.IsNullOrEmpty(rw.Value) && (rw.RuleType == MasterRecordRuleType.IsEqual || rw.RuleType == MasterRecordRuleType.IsContains || rw.RuleType == MasterRecordRuleType.GreaterThan)) e.Valid = false;
        if (string.IsNullOrEmpty(rw.FieldName)) e.Valid = false;
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
        _rules.Join(_fields, r => r.FieldName, f => f.ColumnName, (r, f) => new { r, f }).ForEach(x => x.r.FieldType = x.f.ColumnDataType);

        if (_rules.Any(x => x.FieldType != typeof(string).ToString() && x.RuleType == MasterRecordRuleType.IsContains))
        {
            MessageBox.Show(Resources.MESSAGE_RULE_ONLY_TO_STRING_FIELD);
            return;
        }

        if (_rules.Any(x =>
                (x.RuleType == MasterRecordRuleType.IsMaximum || x.RuleType == MasterRecordRuleType.IsMinimum) &&
                (x.FieldType == typeof(string).ToString() || x.FieldType == typeof(bool).ToString())))
        {
            MessageBox.Show(Resources.MESSAGE_RULE_MAX_MIN_ONLY_FOR_NUMETIC_AND_DATETIME_FIELDS);
            return;
        }

        if (_rules.Any(x =>
                (x.RuleType == MasterRecordRuleType.IsLongest || x.RuleType == MasterRecordRuleType.IsShortest) &&
                (x.FieldType != typeof(string).ToString())))
        {
            MessageBox.Show(Resources.MESSAGE_RULE_LONGEST_SHORTEST_ONLY_FOR_NUMETIC_AND_DATETIME_FIELDS);
            return;
        }

        if (_rules.Any(x =>
                (x.RuleType == MasterRecordRuleType.GreaterThan) &&
                (x.FieldType == typeof(string).ToString() || x.FieldType == typeof(bool).ToString())))
        {
            MessageBox.Show(Resources.MESSAGE_GREATERTHEN_ONLY_FOR_NUMETIC_AND_DATETIME_FIELDS);
            return;
        }
        DialogResult = DialogResult.OK;
        Close();
    }

    private void navBarControl1_DoubleClick(object sender, EventArgs e)
    {
        if (!navBarControl1.ActiveGroup.Expanded)
        {
            navBarControl1.ActiveGroup.Expanded = true;
        }
    }

    private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        if (gvRules.GetRow(gvRules.FocusedRowHandle) is MasterRecordRuleEditable rw)
        {
            _rules.Remove(rw);
            gridRules.RefreshDataSource();
        }
    }
}