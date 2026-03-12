namespace WinPure.CleanAndMatch.StartupForm
{
    public partial class frmStartupOptionsNew : XtraForm
    {
        public StartupOption StartOption { get; private set; }

        public frmStartupOptionsNew()
        {
            InitializeComponent();
            Localization();
            var configuration = WinPureUiDependencyResolver.Resolve<IConfigurationService>().Configuration;
            cbShowOnLaunch.Checked = configuration.ShowStartScreen;
        }

        private void Localization()
        {
            btnNewProject.Text = Resources.UI_MAINFORM_NEWPROJECT;
            btnOpenProject.Text = Resources.UI_UCNEWPROJECTFORM_OPENPROJECT;
            btnImportData.Text = Resources.UI_MAINFORM_IMPORTDATA;
            btnDemoProject.Text = Resources.UI_CAPTION_OPENSAMPLEPROJECT;
            btnGetStarted.Text = Resources.UI_STARTUP_GETSTARTED;
            btnBookDemo.Text = Resources.UI_STARTUP_BOOKDEMO;
            btnOnlineHelp.Text = Resources.UI_STARTUP_ONLINEHELP;
            cbShowOnLaunch.Text = Resources.UI_STARTUPCAMFORM_SHOWONLAUNCH;
        }

        private void CloseForm(StartupAction act)
        {
            StartOption = new StartupOption()
            {
                ActionType = act,
                ShowOnStartup = cbShowOnLaunch.Checked
            };
            Close();
        }
        private void btnNewProject_Click(object sender, EventArgs e)
        {
            CloseForm(StartupAction.NewProject);
        }

        private void btnNewProject_MouseEnter(object sender, EventArgs e)
        {
            this.btnNewProject.ImageOptions.SvgImage = Resources._2026_new_project_filled_64;
        }

        private void btnNewProject_MouseLeave(object sender, EventArgs e)
        {
            this.btnNewProject.ImageOptions.SvgImage = Resources._2026_new_project_64;
        }

        private void btnImportData_Click(object sender, EventArgs e)
        {
            CloseForm(StartupAction.ImportData);
        }

        private void btnImportData_MouseEnter(object sender, EventArgs e)
        {
            this.btnImportData.ImageOptions.SvgImage = Resources._2026_import_filled_data_64;
        }

        private void btnImportData_MouseLeave(object sender, EventArgs e)
        {
            this.btnImportData.ImageOptions.SvgImage = Resources._2026_import_data_64;
        }

        private void btnOpenProject_Click(object sender, EventArgs e)
        {
            CloseForm(StartupAction.OpenProject);
        }

        private void btnOpenProject_MouseEnter(object sender, EventArgs e)
        {
            this.btnOpenProject.ImageOptions.SvgImage = Resources._2026_open_project_filled_64;
        }

        private void btnOpenProject_MouseLeave(object sender, EventArgs e)
        {
            this.btnOpenProject.ImageOptions.SvgImage = Resources._2026_open_project_64;
        }

        private void btnDemoProject_Click(object sender, EventArgs e)
        {
            CloseForm(StartupAction.DemoProject);
        }

        private void btnDemoProject_MouseEnter(object sender, EventArgs e)
        {
            this.btnDemoProject.ImageOptions.SvgImage = Resources._2026_open_demo_project_filled_64;
        }

        private void btnDemoProject_MouseLeave(object sender, EventArgs e)
        {
            this.btnDemoProject.ImageOptions.SvgImage = Resources._2026_open_demo_project_64;
        }

        private void CloseSimpleButton_Click(object sender, EventArgs e)
        {
            CloseForm(StartupAction.None);
        }

        private void GetStartedSimpleButton_MouseEnter(object sender, EventArgs e)
        {
            this.btnGetStarted.BackgroundImage = Resources._2026_GetStarted_filled_50;
        }

        private void GetStartedSimpleButton_MouseLeave(object sender, EventArgs e)
        {
            this.btnGetStarted.BackgroundImage = Resources._2026_GetStarted_50;
        }

        private void BookADemoSimpleButton_MouseEnter(object sender, EventArgs e)
        {
            this.btnBookDemo.BackgroundImage = Resources._2026_BookADemo_filled_50;
            this.btnBookDemo.ForeColor = Color.White;
        }
        private void BookADemoSimpleButton_MouseLeave(object sender, EventArgs e)
        {
            this.btnBookDemo.BackgroundImage = Resources._2026_BookADemo_50;
            this.btnBookDemo.ForeColor = Color.FromArgb(14, 50, 135);

        }

        private void OnlineHelpManualSimpleButton_MouseEnter(object sender, EventArgs e)
        {
            this.btnOnlineHelp.BackgroundImage = Resources._2026_OnlineHelpManual_filled_50;
            this.btnOnlineHelp.ForeColor = Color.White;
        }

        private void OnlineHelpManualSimpleButton_MouseLeave(object sender, EventArgs e)
        {
            this.btnOnlineHelp.BackgroundImage = Resources._2026_OnlineHelpManual_50;
            this.btnOnlineHelp.ForeColor = Color.FromArgb(14, 50, 135);
        }

        private void btnGetStarted_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://meetings.hubspot.com/kathryn-stevenson1?CAMv11");
        }

        private void btnBookDemo_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://meetings.hubspot.com/kathryn-stevenson1?CAMv11");
        }

        private void btnOnlineHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://winpure.com/HelpManuals/CAMv11/CAM.html");
        }
    }
}