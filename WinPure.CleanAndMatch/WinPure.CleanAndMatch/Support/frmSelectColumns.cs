namespace WinPure.CleanAndMatch.Support;

public partial class frmSelectColumns : XtraForm
{
    public frmSelectColumns()
    {
        InitializeComponent();
        Localization();
        _dataFields = new List<DataFieldForSelection>();
    }

    private List<DataFieldForSelection> _dataFields;

    public List<DataField> SelectedColumn
    {
        get
        {
            var res = _dataFields.Where(x => x.IsSelected).Cast<DataField>().ToList();
            return res;
        }
    }

    private void Localization()
    {
        labelControl2.Text = Resources.UI_REMOVECOLUMNFORM_SELECT_COLUMN;
        Text = Resources.UI_REMOVECOLUMNFORM_COLUMN_REMOVING;
        btnCancel.Text = Resources.UI_CANCEL;
        btnOK.Text = Resources.UI_OK;
    }

    public DialogResult Show(IList<DataField> fields, bool forRemove)
    {

        _dataFields = fields.Select(x =>
            new DataFieldForSelection()
            {
                DatabaseName = x.DatabaseName,
                DisplayName = x.DisplayName,
                IsSelected = false,
                FieldType = x.FieldType,
                Id = x.Id
            }).ToList();

        gridRemoveColumn.DataSource = _dataFields;
        gridRemoveColumn.Refresh();
        if (!forRemove) //that is for selection
        {
            labelControl2.Text = Resources.UI_REMOVECOLUMNFORM_SELECT_COLUMN_TO_COPY;
            Text = Resources.UI_REMOVECOLUMNFORM_COLUMN_COPY;
        }
        return ShowDialog();
    }
}