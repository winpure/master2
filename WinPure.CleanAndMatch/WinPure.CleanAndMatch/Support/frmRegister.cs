using System.Diagnostics;
using WinPure.Licensing.Enums;

namespace WinPure.CleanAndMatch.Support;

public partial class frmRegister : XtraForm
{
    private readonly bool _isAutomationModuleInstalled;
    private readonly string _automationPath;
    private readonly ILicenseService _licenseService;

    public bool ShouldAppClose { get; set; } = false;
    public frmRegister()
    {
        InitializeComponent();
        Localization();
        labelVersion.Parent = pictureEdit1;
        labelVersion.BackColor = Color.Transparent;

        DialogResult = DialogResult.Cancel;
        _licenseService = WinPureUiDependencyResolver.Resolve<ILicenseService>();
        txtUserCode.Text = _licenseService.GetLocalRegistrationKey();
        var version = Common.Helpers.AssemblyHelper.ApplicationVersion();
        labelVersion.Text = $"{version}";
        lbLicenseState.Text = string.Format(Resources.MESSAGE_LICENSESTATE, _licenseService.GetLicenseState(), _licenseService.LicenseExpiredAtDays);
        //switch (Program.CurrentProgramVersion)
        //{
        //    case ProgramType.CamLte:
        //        pictureEdit1.Image = Resources.CAM_LTE_REGISTRATION;
        //        break;
        //    case ProgramType.CamBiz:
        //        pictureEdit1.Image = Resources.CAM_BIZ_REGISTRATION;
        //        break;
        //    case ProgramType.CamEntAd:
        //        pictureEdit1.Image = Resources.ENTERPRISE_ADDRESS_REGISTER;
        //        break;
        //    case ProgramType.CamEnt:
        //    case ProgramType.CamEntLite:
        //        pictureEdit1.Image = Resources.ENTERPRISE_REGISTER_v8;
        //        break;
        //}

        (_isAutomationModuleInstalled, _automationPath) = SystemHelper.IsAutomationModuleInstalled();
        rgLicenseFileType.SelectedIndex = 0;
        rgLicenseFileType.Visible = _isAutomationModuleInstalled && ProgramTypeHelper.AutomationPrograms.Contains(_licenseService.ProgramType);
    }

    private void Localization()
    {
        labelVersion.Text = Resources.UI_ABOUTFORM_VERSION101;
        txtUserCode.Properties.Buttons.First().ToolTip = Resources.UI_REGISTERFORM_COPYCODE;
        labelControl2.Text = Resources.UI_LICENCEKEY;
        labelControl1.Text = Resources.UI_REGISTERFORM_GENERATIONKEY;
        btnCancel.Text = Resources.UI_CANCEL;
        btnRegister.Text = Resources.UI_REGISTERFORM_REGISTER;
        Text = Resources.UI_REGISTRATION;
        btnContinueDemo.Text = Resources.UI_CAPTION_CONTINUEDEMO;
        rgLicenseFileType.Properties.Items[0].Description = Resources.UI_LICESEFORDESKTOP;
        rgLicenseFileType.Properties.Items[1].Description = Resources.UI_LICESEFORAUTOMATION;
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void btnRegister_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtLicenseFile.Text))
        {
            var configurationService = WinPureUiDependencyResolver.Resolve<IConfigurationService>();
            var licenseExtension = Path.GetExtension(txtLicenseFile.Text);
            var licenseFileName = Path.GetFileNameWithoutExtension(txtLicenseFile.Text);

            if (rgLicenseFileType.SelectedIndex == 0) //register desktop solution
            {
                var licenseState = _licenseService.Register(txtLicenseFile.Text);
                if (licenseState == LicenseState.Valid)
                {
                    var newLicenseFileName = $"{licenseFileName}_{_licenseService.ProgramType}{licenseExtension}";
                    var destinationFile = Path.Combine(configurationService.Configuration.License.Path, newLicenseFileName);
                    configurationService.Configuration.License.DesktopLicenseName = newLicenseFileName;
                    File.Copy(txtLicenseFile.Text, destinationFile, true);
                    if (_licenseService.LoadLicense(configurationService.Configuration.License.Path,
                            configurationService.Configuration.License.DesktopLicenseName) == LicenseState.Valid)
                    {
                        configurationService.SaveConfiguration();
                    }

                    Program.CurrentProgramVersion = _licenseService.ProgramType;
                        
                    MessageBox.Show(Resources.MESSAGE_REGISTERFORM_THANKSFORREGISTERING + Environment.NewLine + Resources.UI_MESSAGE_RESTARTREQUIRED,
                        Resources.MESSAGE_MESSAGEDIALOG_INFORMATION_CAPTION,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    Process.Start(Application.ExecutablePath);

                    // Close the current form
                    ShouldAppClose = true;
                    //Application.Exit();
                }
                else
                {
                    MessageBox.Show(Resources.MESSAGE_REGISTERFORM_CODENOTVALID, Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            } 
            else if (rgLicenseFileType.SelectedIndex == 1) // register automation module
            {
                if (!SystemHelper.IsAdministrator())
                {
                    MessageBox.Show(Resources.MESSAGE_REGISTERRUNPROGRAMASADMIN);
                    return;
                }

                var newLicenseFileName = $"{licenseFileName}_automation{licenseExtension}";
                var destinationFile = Path.Combine(configurationService.Configuration.License.Path, newLicenseFileName);
                File.Copy(txtLicenseFile.Text, destinationFile, true);
                configurationService.Configuration.License.AutomationLicenseName = newLicenseFileName;
                configurationService.SaveConfiguration();

                System.Diagnostics.ProcessStartInfo myProcessInfo = new System.Diagnostics.ProcessStartInfo(); //Initializes a new ProcessStartInfo of name myProcessInfo
                myProcessInfo.FileName = _automationPath; //Sets the FileName property of myProcessInfo to %SystemRoot%\System32\cmd.exe where %SystemRoot% is a system variable which is expanded using Environment.ExpandEnvironmentVariables
                myProcessInfo.Arguments = "/STOP"; //Sets the arguments to cd..
                //myProcessInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; //Sets the WindowStyle of myProcessInfo which indicates the window state to use when the process is started to Hidden
                var proc = System.Diagnostics.Process.Start(myProcessInfo);
                proc.WaitForExit();
                myProcessInfo.Arguments = "/START"; //Sets the arguments to cd..
                // myProcessInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; //Sets the WindowStyle of myProcessInfo which indicates the window state to use when the process is started to Hidden
                System.Diagnostics.Process.Start(myProcessInfo);
            }

            DialogResult = DialogResult.OK;
            Close();
            MessageBox.Show(Resources.MESSAGE_REGISTERFORM_THANKSFORREGISTERING, Resources.MESSAGE_MESSAGEDIALOG_SUCCESS_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                
        }
    }

    private void txtUserCode_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        Clipboard.SetText(txtUserCode.Text);
    }

    private void txtLicenseFile_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        var fileDialog = new OpenFileDialog
        {
            Title = Resources.DIALOG_TITLE_SELECTLICENSE,
            FileName = "",
            CheckFileExists = true,
            Filter = Resources.DIALOG_LICENSE_FILE
        };

        if (fileDialog.ShowDialog() == DialogResult.OK)
        {
            txtLicenseFile.Text = fileDialog.FileName;
        }
    }

    private void btnContinueDemo_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtLicenseFile.Text))
        {
            var configurationService = WinPureUiDependencyResolver.Resolve<IConfigurationService>();
            var licenseExtension = Path.GetExtension(txtLicenseFile.Text);
            var licenseFileName = Path.GetFileNameWithoutExtension(txtLicenseFile.Text);

            if (rgLicenseFileType.SelectedIndex == 0) //register desktop solution
            {
                var licenseState = _licenseService.Register(txtLicenseFile.Text);
                if (licenseState == LicenseState.Demo)
                {
                    var newLicenseFileName = $"{licenseFileName}_{_licenseService.ProgramType}{licenseExtension}";
                    var destinationFile = Path.Combine(configurationService.Configuration.License.Path, newLicenseFileName);
                    configurationService.Configuration.License.DesktopLicenseName = newLicenseFileName;
                    File.Copy(txtLicenseFile.Text, destinationFile, true);
                    if (_licenseService.LoadLicense(configurationService.Configuration.License.Path,
                            configurationService.Configuration.License.DesktopLicenseName) == LicenseState.Demo)
                    {
                        configurationService.SaveConfiguration();
                    }
                }
                else
                {
                    MessageBox.Show(Resources.MESSAGE_REGISTERFORM_CODENOTVALID, Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            DialogResult = DialogResult.OK;
            Close();
            MessageBox.Show(Resources.MESSAGE_REGISTERFORM_DEMOCONTINUED, Resources.MESSAGE_MESSAGEDIALOG_SUCCESS_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
    }
}