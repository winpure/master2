namespace WinPure.CleanAndMatch.Support;

public partial class frmCreateNewProject : XtraForm
{
    private char[] _deniedSymbol;

    public string ProjectName => Path.GetFileNameWithoutExtension(txtNewName.Text.Trim());
    public string ProjectPath => Path.Combine(lbProjectPath.Text, FileName);
    public string FileName => txtNewName.Text.Trim();

    public frmCreateNewProject()
    {
        InitializeComponent();
        Localization();
    }

    public DialogResult Show(string caption, string text, char[] deniedSymbol)
    {
        _deniedSymbol = deniedSymbol;
        txtNewName.Text = GetProjectFileNameWithExtension(Resources.CAPTION_DEFAULT_PROJECT_NAME);

        lbProjectPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        lbProjectPath.ToolTip = "";
        if (!string.IsNullOrEmpty(text))
        {
            labelControl1.Text = text;
            labelControl1.Focus();
        }
        Text = caption;
        return ShowDialog();
    }

    private void Localization()
    {
        btnOK.Text = Resources.UI_CREATE;
    }

    private bool ValidateProjectName()
    {
        if (_deniedSymbol != null && _deniedSymbol.Any() && txtNewName.Text.IndexOfAny(_deniedSymbol) != -1)
        {
            MessageBox.Show(Resources.MESSAGE_CHARS_NOT_ALLOWED_IN_PROJECT + " " + string.Join(" ", _deniedSymbol));
            return false;
        }
        return true;
    }

    private void txtNewProject_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            btnOK_Click(null, null);
        }
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
        if (ValidateProjectName())
        {
            if (!string.IsNullOrWhiteSpace(ProjectPath) && !string.IsNullOrWhiteSpace(ProjectName))
            {
                if (File.Exists(ProjectPath))
                {
                    if (MessageBox.Show(Resources.MESSAGE_OVERRIDEFILE, Resources.MESSAGE_QUESTION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
                else
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            else
            {
                MessageBox.Show(Resources.MESSAGE_OVERRIDEFILE);
            }
        }
    }

    private void btnSelectFolder_Click(object sender, EventArgs e)
    {
        var sv = new SaveFileDialog
        {
            Filter = Resources.DIALOG_PROJECT_FORMAT,
            FileName = txtNewName.Text,
            ValidateNames = true,
            Title = Resources.DIALOG_PROJECT_SAVE_CAPTION,
            InitialDirectory = lbProjectPath.Text
        };

        var configurationService = WinPureUiDependencyResolver.Resolve<IConfigurationService>();

        if (configurationService.Configuration.FirstNewProject)
        {
            configurationService.Configuration.FirstNewProject = false;
            configurationService.SaveConfiguration();
            sv.InitialDirectory = lbProjectPath.Text; //PathHelperService.GetNewProjectSamplePath();
        }

        if (sv.ShowDialog() == DialogResult.OK)
        {
            lbProjectPath.Text = Path.GetDirectoryName(sv.FileName);
            lbProjectPath.ToolTip = Path.GetDirectoryName(sv.FileName);
            txtNewName.Text = Path.GetFileName(sv.FileName);
        }
    }

    private void txtNewName_Leave(object sender, EventArgs e)
    {
        if (Path.GetExtension(txtNewName.Text) != "wppj")
        {
            txtNewName.Text = GetProjectFileNameWithExtension(Path.GetFileNameWithoutExtension(txtNewName.Text));
        }
    }

    private string GetProjectFileNameWithExtension(string projectName)
    {
        return $"{projectName}.wppj";
    }
}