using System.Reflection;
using WinPure.Configuration.Enums;
using WinPure.Configuration.Models.Configuration;

namespace WinPure.CleanAndMatch.Support;

internal partial class frmCleansingAiConfiguration : XtraForm
{
    private readonly ICleansingAiConfigurationService _service;
    private readonly IStatisticPatternService _statisticPatternService;
    private readonly IWpLogger _logger;
    private List<CleansingAiFieldType> _aiFields = new();

    public frmCleansingAiConfiguration(
        ICleansingAiConfigurationService service,
        IStatisticPatternService statisticPatternService,
        IWpLogger logger)
    {
        _service = service;
        _statisticPatternService = statisticPatternService;
        _logger = logger;
        InitializeComponent();
        Localization();
    }

    private void Localization()
    {
        btnCancel.Text = Resources.UI_CANCEL;
        btnOK.Text = Resources.UI_SAVE;
        btnAddAiType.Text = Resources.UI_ADD;
        btnAddMappedField.Text = Resources.UI_ADD;
        lbPattern.Text = Resources.UI_PATTERNNAME;
        colAiType.Caption = Resources.UI_COLUMN_TYPE;
        colPattern.Caption = Resources.CAPTION_PATTERN;
        colName.Caption = Resources.UI_COLUMN_NAME;
        colPatternDescription.Caption = Resources.CAPTION_DESCRIPTION;
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
            _logger.Error("CleansingAI manage form error", ex);
            if (!suppressMessage)
            {
                MessageBox.Show(ex.Message);
            }
            return ex.Message;
        }
    }

    private void frmCleansingAiConfiguration_Load(object sender, EventArgs e)
    {
        edtPattern.Properties.DataSource = AsyncHelpers.RunSync(_statisticPatternService.GetAllPatterns);
        // Populate mapping type combo with enum values
        repoFieldMapType.Items.Clear();
        cbFieldMapType.Properties.Items.Clear();
        foreach (var name in Enum.GetNames(typeof(CleanAiMapType)))
        {
            repoFieldMapType.Items.Add(name);
            cbFieldMapType.Properties.Items.Add(name);
        }

        cbFieldMapType.SelectedIndex = 0;
        cbFieldMapType.Refresh();
        LoadData();
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
        AsyncHelpers.RunSync(() => _service.SyncAiTypes(_aiFields));
    }

    private void LoadData()
    {
        _aiFields = AsyncHelpers.RunSync(_service.GetAllConfigurations);
        RefreshData();
        gvAiType.FocusedRowChanged += gvAiType_FocusedRowChanged;
    }

    private void RefreshData()
    {
        gridAiType.DataSource = _aiFields;
        gridAiType.RefreshDataSource();
        if (gvAiType.GetRow(gvAiType.FocusedRowHandle) is CleansingAiFieldType rw)
        {
            gridMappedField.DataSource = rw.MappedFields;
            gridMappedField.RefreshDataSource();
            gridCleansingOptions.DataSource = ToDataTable(rw.Options);
            gridCleansingOptions.RefreshDataSource();
            edtPattern.EditValue = rw.Options.Pattern;
        }
        else
        {
            gridMappedField.DataSource = null;
            gridMappedField.RefreshDataSource();
            gridCleansingOptions.DataSource = null;
            gridCleansingOptions.RefreshDataSource();
            edtPattern.EditValue = null;
        }
    }

    private void repoRemoveAiType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        if (MessageBox.Show(Resources.MESSAGE_EDITDICTIONARYFORM_CONFIRM_DELETE_LIBRARY, Resources.MESSAGE_CONFIRMATION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
        {
            SafeActionCall(() =>
            {
                if (gvAiType.GetRow(gvAiType.FocusedRowHandle) is CleansingAiFieldType rw)
                {
                    _aiFields.Remove(rw);
                    RefreshData();
                }
            });
        }
    }

    private void repoRemoveMapField_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        if (MessageBox.Show(Resources.MESSAGE_EDITDICTIONARYFORM_CONFIRM_DELETE_LIBRARY, Resources.MESSAGE_CONFIRMATION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
        {
            SafeActionCall(() =>
            {
                if (gvMappedField.GetRow(gvMappedField.FocusedRowHandle) is CleanAiMappedField rw && gvAiType.GetRow(gvAiType.FocusedRowHandle) is CleansingAiFieldType parentRow)
                {
                    parentRow.MappedFields.Remove(rw);
                    RefreshData();
                }
            });
        }
    }

    private void gvAiType_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
    {
        RefreshData();
    }

    private void cbFieldMapType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Enum.TryParse<CleanAiMapType>(cbFieldMapType.SelectedItem.ToString(), out var selectedMapType))
        {
            switch (selectedMapType)
            {
                case CleanAiMapType.Contains:
                    sePrecision.Enabled = false;
                    sePrecision.Value = 0;
                    break;
                case CleanAiMapType.Exact:
                    sePrecision.Enabled = false;
                    sePrecision.Value = 100;
                    break;
                case CleanAiMapType.Fuzzy:
                    sePrecision.Enabled = true;
                    sePrecision.Value = 95;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown CleansingAi Map type");
            }
        }
    }

    private void btnAddAiType_Click(object sender, EventArgs e)
    {
        _aiFields.Add(new CleansingAiFieldType
        {
            AiType = txtAiName.Text
        });
        RefreshData();
    }

    private void btnAddMappedField_Click(object sender, EventArgs e)
    {
        if (gvAiType.GetRow(gvAiType.FocusedRowHandle) is CleansingAiFieldType rw
            && Enum.TryParse<CleanAiMapType>(cbFieldMapType.SelectedItem.ToString(), out var selectedMapType))
        {
            rw.MappedFields.Add(new CleanAiMappedField
            {
                Name = txtMappedFieldName.Text,
                MapType = selectedMapType,
                Precision = sePrecision.Value
            });
            gridMappedField.DataSource = rw.MappedFields;
            gridMappedField.RefreshDataSource();
        }
    }

    private void gvCleansingOptions_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
    {
        if (gvAiType.GetRow(gvAiType.FocusedRowHandle) is CleansingAiFieldType rw)
        {
            var table = gridCleansingOptions.DataSource as DataTable;
            var options = FromDataTable(table);
            options.Pattern = rw.Options.Pattern;
            rw.Options = options;
        }
    }

    private void edtPattern_EditValueChanged(object sender, EventArgs e)
    {
        if (gvAiType.GetRow(gvAiType.FocusedRowHandle) is CleansingAiFieldType rw)
        {
            rw.Options.Pattern = edtPattern.EditValue?.ToString() ?? string.Empty;
        }
    }

    private void edtPattern_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        if (e.Button.Tag.ToString() == "Delete") //delete button
        {
            if (gvAiType.GetRow(gvAiType.FocusedRowHandle) is CleansingAiFieldType rw)
            {
                rw.Options.Pattern = string.Empty;
                edtPattern.EditValue = null;
            }
        }
    }

    /// <summary>
    /// Convert options into a DataTable with columns: Option (string) and Value (bool)
    /// Only boolean properties are exported.
    /// </summary>
    DataTable ToDataTable(CleanAiFieldOptions options)
    {
        var table = new DataTable();
        table.Columns.Add("Option", typeof(string));
        table.Columns.Add("Value", typeof(bool));

        if (options == null) return table;

        foreach (var prop in typeof(CleanAiFieldOptions).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.PropertyType != typeof(bool)) continue; // skip non-bool (e.g., Pattern)
            var displayAttr = prop.GetCustomAttribute<Common.Helpers.DisplayNameAttribute>();
            var name = displayAttr?.DisplayName ?? prop.Name;
            var value = (bool)prop.GetValue(options);
            var row = table.NewRow();
            row["Option"] = name;
            row["Value"] = value;
            table.Rows.Add(row);
        }
        return table;
    }

    /// <summary>
    /// Create a CleanAiFieldOptions instance from a DataTable produced by ToDataTable.
    /// Matches rows by DisplayNameAttribute or property name (case-insensitive).
    /// </summary>
    CleanAiFieldOptions FromDataTable(DataTable table)
    {
        var result = new CleanAiFieldOptions();
        if (table == null) return result;

        // Build lookup: display name -> PropertyInfo
        var props = typeof(CleanAiFieldOptions).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(bool))
            .Select(p => new
            {
                Prop = p,
                Display = p.GetCustomAttribute<Common.Helpers.DisplayNameAttribute>()?.DisplayName ?? p.Name
            })
            .ToList();

        foreach (DataRow row in table.Rows)
        {
            var nameObj = row["Option"];
            var valueObj = row["Value"];
            if (nameObj == null || valueObj == null) continue;
            var name = nameObj.ToString();
            if (string.IsNullOrWhiteSpace(name)) continue;

            var match = props.FirstOrDefault(x => string.Equals(x.Display, name, StringComparison.InvariantCultureIgnoreCase)
                                                   || string.Equals(x.Prop.Name, name, StringComparison.InvariantCultureIgnoreCase));
            if (match == null) continue;
            bool boolVal;
            try
            {
                boolVal = valueObj is bool b ? b : Convert.ToBoolean(valueObj);
            }
            catch
            {
                continue; // skip invalid value
            }
            match.Prop.SetValue(result, boolVal);
        }

        return result;
    }
}