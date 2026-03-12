using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using WinPure.Configuration.Enums;
using WinPure.Configuration.Helper;
using WinPure.Configuration.Models.Configuration;

namespace WinPure.CleanAndMatch.Support;

internal partial class frmSelectFields : XtraForm
{
    private readonly IStatisticPatternService _statisticPatternService;
    private readonly ICleansingAiConfigurationService _aiConfigurationService;
    private List<DataFieldForSelection> _selectedFields;
    private bool _isRename;

    private ImportedDataInfo _dataInfo;

    public frmSelectFields(IStatisticPatternService statisticPatternService, ICleansingAiConfigurationService aiConfigurationService)
    {
        _statisticPatternService = statisticPatternService;
        _aiConfigurationService = aiConfigurationService;
        InitializeComponent();
        Localization();
    }

    public List<DataFieldForSelection> SelectedFields => (_isRename) ?
        _selectedFields.Where(x => !string.IsNullOrEmpty(x.DisplayName) && x.DisplayName != x.DatabaseName).OrderBy(x => x.Id).ToList()
        : _selectedFields.Where(x => x.IsSelected).OrderBy(x => x.Id).ToList();


    public DialogResult ShowDialog(ImportedDataInfo dataInfo)
    {
        edtPattern.DataSource = AsyncHelpers.RunSync(_statisticPatternService.GetAllPatterns);
        var aiTypes = AsyncHelpers.RunSync(_aiConfigurationService.GetAllConfigurations);
        edtAiType.DataSource = aiTypes;

        _dataInfo = dataInfo;
        _isRename = false;
        _selectedFields =
            dataInfo.Fields.Select(
                x =>
                    new DataFieldForSelection
                    {
                        Id = x.Id,
                        DatabaseName = x.DatabaseName,
                        FieldType = x.FieldType,
                        DisplayName = ColumnHelper.IsSystemField(x.DatabaseName) ? x.DatabaseName.Replace(" ", "_") : x.DatabaseName.NormalizeColumnName(),
                        IsSelected = true,
                        NewFieldType = SqLiteHelper.GetSqLiteDataType(x.FieldType),
                        Pattern = null,
                        AiType = null
                    }).ToList();

        SetAiTypes(aiTypes);
        gridFields.DataSource = _selectedFields;
        gridFields.Refresh();
        txtSourceName.Text = dataInfo.DisplayName;
        Text = "Select Fields to Import";
        btnOK.Text = Resources.UI_IMPORT;
        return ShowDialog();
    }

    public DialogResult ShowRenameDialog(ImportedDataInfo dataInfo)
    {
        _dataInfo = dataInfo;
        _isRename = true;
        _selectedFields =
            dataInfo.Fields.Select(
                x =>
                    new DataFieldForSelection
                    {
                        Id = x.Id,
                        DatabaseName = x.DatabaseName,
                        FieldType = x.FieldType,
                        DisplayName = "",
                        IsSelected = false,
                        NewFieldType = SqLiteHelper.GetSqLiteDataType(x.FieldType),
                        Pattern = x.Pattern,
                        AiType = x.AiType
                    }).ToList();
        gridFields.DataSource = _selectedFields;
        gridFields.Refresh();
        txtSourceName.Text = dataInfo.DisplayName;
        Text = "Select Columns to Rename";
        colPattern.Visible = false;
        colAiType.Visible = false;
        repositoryItemComboBox1.ReadOnly = true;
        repositoryItemComboBox1.TextEditStyle = TextEditStyles.DisableTextEditor;
        btnOK.Text = Resources.UI_OK;
        return ShowDialog();
    }

    private void Localization()
    {
        lbTableName.Text = Resources.UI_EXPORTMSSQLFORM_TABLENAME;
        colPattern.Caption = Resources.UI_PATTERNNAME;
        btnOK.Text = Resources.CAPTION_IMPORT;
        btnCancel.Text = Resources.UI_CANCEL;
        gridColumn2.Caption = Resources.CAPTION_ORIGINAL_NAME;
        colFieldType.Caption = Resources.CAPTION_FIELD_TYPE;
        gridColumn4.Caption = Resources.CAPTION_DISPLAY_NAME;
        colPattern.Caption = Resources.CAPTION_PATTERN;
        colAiType.Caption = Resources.CAPTION_AITYPE;
        Text = Resources.CAPTION_SELECTFIELDTOIMPORT;
    }

    private void SetAiTypes(List<CleansingAiFieldType> aiTypes)
    {
        if (aiTypes == null || _selectedFields == null) return;

        // Build dictionary: key = mapped field name (Exact), value = CleansingAiFieldType
        var aiTypesByExactField = aiTypes
            .SelectMany(t => t.MappedFields
                .Where(m => m.MapType == CleanAiMapType.Exact && !string.IsNullOrWhiteSpace(m.Name))
                .Select(m => new { m.Name, Type = t }))
            .GroupBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().Type, StringComparer.InvariantCultureIgnoreCase);
        
        var aiTypesByContainsField = aiTypes
            .SelectMany(t => t.MappedFields
                .Where(m => m.MapType == CleanAiMapType.Contains && !string.IsNullOrWhiteSpace(m.Name))
                .Select(m => new { m.Name, Type = t }))
            .GroupBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().Type, StringComparer.InvariantCultureIgnoreCase);

        // Optionally pre-select AiType for matching fields
        foreach (var field in _selectedFields)
        {
            if (field == null || string.IsNullOrWhiteSpace(field.DatabaseName)) continue;
            // First, try exact match
            if (aiTypesByExactField.TryGetValue(field.DatabaseName, out var aiType))
            {
                if (aiType == null) continue;
                field.AiType = aiType.AiType;
                if (!string.IsNullOrEmpty(aiType.Options?.Pattern))
                {
                    field.Pattern = aiType.Options.Pattern;
                }
                continue;
            }

            // Fallback: find any contains match
            var containsMatch = aiTypesByContainsField.FirstOrDefault(kv =>
                !string.IsNullOrWhiteSpace(kv.Key) &&
                field.DatabaseName?.IndexOf(kv.Key, StringComparison.InvariantCultureIgnoreCase) >= 0);

            if (!string.IsNullOrEmpty(containsMatch.Key) && containsMatch.Value != null)
            {
                var aiType2 = containsMatch.Value;
                field.AiType = aiType2.AiType;
                if (!string.IsNullOrEmpty(aiType2.Options?.Pattern))
                {
                    field.Pattern = aiType2.Options.Pattern;
                }
            }
        }
    }

    //The earliest this method can be called is when the form loads - the GridView.VisibleColumns property is otherwise not yet available.
    private void ConfigureCheckboxColumn()
    {
        var checkBoxColumn = gvFields.VisibleColumns[0];

        checkBoxColumn.ColumnEdit = repoSelectField;
        checkBoxColumn.Visible = panelControl2.Visible = colFieldType.Visible = true;
        checkBoxColumn.AppearanceHeader.Font = new Font("Open Sans", 9F, FontStyle.Bold);
        checkBoxColumn.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
        checkBoxColumn.AppearanceHeader.Options.UseFont = true;
        checkBoxColumn.AppearanceHeader.Options.UseForeColor = true;
    }

    private void btnOK_Click(object sender, System.EventArgs e)
    {
        var duplicatedField = _selectedFields.Where(x => x.IsSelected)
            .GroupBy(x => x.DisplayName.ToUpper())
            .Select(x => new { nm = x.Key, cnt = x.Count() })
            .Where(x => x.cnt > 1).ToList();

        if (duplicatedField.Any())
        {
            var field = string.Join(", ", duplicatedField.Select(x => x.nm));
            MessageBox.Show(Resources.MESSAGE_DISPLAY_NAME_SHOULD_BE_UNIQUE + $" Fields: [{field}]");
            return;
        }

        var systemFields = _selectedFields.Where(x => x.IsSelected && ColumnHelper.IsSystemField(x.DisplayName))
            .ToList();

        if (systemFields.Any())
        {
            var field = string.Join(", ", systemFields.Select(x => x.DisplayName));
            MessageBox.Show($"Display names should not contain system field name. Fields: [{field}]");
            return;
        }

        if (_selectedFields.Any(x => x.IsSelected && x.DisplayName == ""))
        {
            MessageBox.Show(Resources.MESSAGE_DISPLAY_NAME_SHOULD_NOT_BE_EMPTY);
            return;
        }
        if (_selectedFields.Where(x => x.IsSelected)
            .Any(x => SqLiteHelper.GetSqLiteDataType(x.FieldType) != x.NewFieldType && x.NewFieldType != "text"))
        {
            MessageBox.Show(Resources.MESSAGE_DATATYPE_TO_TEXT_ONLY);
            return;
        }
        if (_isRename)
        {
            foreach (var fld in _selectedFields.Where(x => x.IsSelected).OrderBy(x => x.Id))
            {
                if (fld.DisplayName != fld.DatabaseName &&
                    _selectedFields.Select(x => x.DatabaseName).Contains(fld.DisplayName))
                {
                    MessageBox.Show(Resources.MESSAGE_DISPLAY_NAME_SHOULD_BE_UNIQUE);
                    return;
                }
            }
        }
        else
        {
            _dataInfo.DisplayName = txtSourceName.Text;
            _dataInfo.Fields.Clear();
            int i = 0;
            foreach (var fld in _selectedFields.Where(x => x.IsSelected).OrderBy(x => x.Id))
            {
                _dataInfo.Fields.Add(new DataField
                {
                    DatabaseName = fld.DatabaseName,
                    FieldType =
                        (SqLiteHelper.GetSqLiteDataType(fld.FieldType) == fld.NewFieldType)
                            ? fld.FieldType
                            : "System.String",
                    DisplayName = fld.DisplayName,
                    Pattern = fld.Pattern,
                    AiType = fld.AiType,
                    Id = i++
                });
            }
        }
        DialogResult = DialogResult.OK;
    }

    private void txtSourceName_TextChanged(object sender, System.EventArgs e)
    {
        txtSourceName.Text = txtSourceName.Text.NormalizeColumnName();
    }

    private void gvFields_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
    {
        if (e.Column != gridColumn4) return;
        var rw = gvFields.GetRow(e.RowHandle) as DataFieldForSelection;
        if (rw == null) return;
        if (_isRename)
        {
            if (e.Value == null || e.Value.ToString() == "")
            {
                rw.IsSelected = false;
            }
            else
            {
                rw.DisplayName = rw.DisplayName.NormalizeColumnName();
                rw.IsSelected = true;
            }

        }
        else
        {
            rw.DisplayName = e.Value == null || e.Value.ToString() == ""
                ? rw.DatabaseName
                : rw.DisplayName.NormalizeColumnName();
        }
    }

    private void pictureBox1_Click(object sender, System.EventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.SelectFieldsToImport);
    }

    private void edtPattern_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        if (e.Button.Index == 1) //delete button
        {
            if (gvFields.GetRow(gvFields.FocusedRowHandle) is DataFieldForSelection rw)
            {
                rw.Pattern = null;
                var edit = sender as LookUpEdit;
                edit.EditValue = null;
            }
        }
    }

    private void frmSelectFields_Load(object sender, EventArgs e)
    {
        ConfigureCheckboxColumn();
    }

    private void edtAiType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        if (e.Button.Index == 1) //delete button
        {
            if (gvFields.GetRow(gvFields.FocusedRowHandle) is DataFieldForSelection rw)
            {
                rw.AiType = null;
                var edit = sender as LookUpEdit;
                edit.EditValue = null;
            }
        }
    }
}