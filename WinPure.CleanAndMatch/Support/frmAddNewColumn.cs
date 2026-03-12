namespace WinPure.CleanAndMatch.Support;

public partial class frmAddNewColumn : XtraForm
{
    public frmAddNewColumn()
    {
        InitializeComponent();
        Localization();
        cbNewColumnType.SelectedIndex = 0;
        cbNewColumnType.Refresh();
    }

    private void Localization()
    {
        Text = Resources.UI_ADDCOLUMNFORM_ADD_NEW_COLUMN;
        labelControl1.Text = Resources.UI_ADDCOLUMNFORM_ENTER_COLUMN_NAME;
        labelControl2.Text = Resources.UI_ADDCOLUMNFORM_SELECT_COLUMN_TYPE;
        btnCancel.Text = Resources.UI_CANCEL;
        btnOK.Text = Resources.UI_OK;
    }

    public string ColumnName => txtNewName.Text;

    public string ColumnType
    {
        get
        {
            if (cbNewColumnType.Text == "DateTime") return typeof(DateTime).ToString();
            if (cbNewColumnType.Text == "Integer") return typeof(int).ToString();
            if (cbNewColumnType.Text == "Decimal") return typeof(decimal).ToString();
            if (cbNewColumnType.Text == "Boolean") return typeof(bool).ToString();
            return typeof(string).ToString();
        }
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtNewName.Text))
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    private void txtNewName_TextChanged(object sender, EventArgs e)
    {
        btnOK.Enabled = !string.IsNullOrEmpty(txtNewName.Text);
    }
}