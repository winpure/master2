namespace WinPure.CleanAndMatch.Support;

public partial class frmNewName : XtraForm
{
    private char[] _deniedSymbol;

    public frmNewName()
    {
        InitializeComponent();
        Localization();
    }

    private void Localization()
    {
        btnCancel.Text = Resources.UI_CANCEL;
        btnOK.Text = Resources.UI_OK;
    }

    public string NewName => txtNewName.Text.Trim();

    private bool ValidateName()
    {
        if (_deniedSymbol != null && _deniedSymbol.Any() && txtNewName.Text.IndexOfAny(_deniedSymbol) != -1)
        {
            MessageBox.Show(Resources.MESSAGE_CHARS_NOT_ALLOWED_IN_PROJECT + " " + string.Join(" ", _deniedSymbol));
            return false;
        }
        return true;
    }

    public DialogResult Show(string caption, string text, string oldValue, char[] deniedSymbol)
    {
        _deniedSymbol = deniedSymbol;
        txtNewName.Text = oldValue;
        if (!string.IsNullOrEmpty(text))
        {
            labelControl1.Text = text;
            labelControl1.Focus();
        }
        Text = caption;
        return ShowDialog();
    }

    private void txtNewProject_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            btnOK_Click(null, null);
        }
    }

    private void btnOK_Click(object sender, System.EventArgs e)
    {
        if (ValidateName())
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}