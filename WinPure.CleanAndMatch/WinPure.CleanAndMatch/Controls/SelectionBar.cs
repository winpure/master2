namespace WinPure.CleanAndMatch.Controls;

public partial class SelectionBar : XtraUserControl
{
    public SelectionBar()
    {
        InitializeComponent();
    }

    public void Initiate(bool displayFieldList, List<string> fields)
    {
        cbFieldList.Visible = displayFieldList;
        if (displayFieldList && fields != null)
        {
            cbFieldList.Properties.Items.Clear();
            cbFieldList.Properties.Items.AddRange(fields);
            cbFieldList.SelectedIndex = 0;
            cbFieldList.Refresh();
        }
    }

    public event Action<string, SelectionType> OnSelectionChanged;

    public string FieldName
    {
        get => cbFieldList.Text;
        set => cbFieldList.Text = value;
    }

    public void SetAlign(DockStyle dStyle)
    {
        btnInvert.Dock = dStyle;
        btnUncheckAll.Dock = dStyle;
        btnCheckAll.Dock = dStyle;

    }

    private void btnCheckAll_Click(object sender, EventArgs e)
    {
        OnSelectionChanged?.Invoke(FieldName, SelectionType.SelectAll);
    }

    private void btnUncheckAll_Click(object sender, EventArgs e)
    {
        OnSelectionChanged?.Invoke(FieldName, SelectionType.UnselectAll);
    }

    private void btnInvert_Click(object sender, EventArgs e)
    {
        OnSelectionChanged?.Invoke(FieldName, SelectionType.InvertSelection);
    }
}