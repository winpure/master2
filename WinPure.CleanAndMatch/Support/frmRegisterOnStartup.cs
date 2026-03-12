namespace WinPure.CleanAndMatch.Support
{
    public partial class frmRegisterOnStartup : XtraForm
    {
        public bool ShouldAppClose { get; set; } = false;

        public frmRegisterOnStartup()
        {
            InitializeComponent();
            Localization();

            lbLicenseInfo.BackColor = Color.Transparent;
            DialogResult = DialogResult.Cancel;

            var version = Common.Helpers.AssemblyHelper.ApplicationVersion();
            labelControl1.Text = $"v{version}";

            labelControl1.BackColor = Color.Transparent;
            UpdateLicenseInfo();
        }

        private void Localization()
        {
            btnRequestCustomQuote.Text = Resources.UI_REQUEST_CUSTOM_QUOTE.ToUpper();
            btnRegisterKey.Text = Resources.UI_REGISTERFORM_REGISTER.ToUpper();
            btnContinueWithDemo.Text = Resources.UI_CONTINUE_WITH_DEMO.ToUpper();
            btnScheduleTraining.Text = Resources.UI_SHEDULE_TRAINING.ToUpper();
            Text = Resources.UI_REGISTRATION;
        }

        private void btnScheduleTraining_MouseEnter(object sender, EventArgs e)
        {
            this.btnScheduleTraining.ImageOptions.Image = Resources._2026_Demo_hover;
        }

        private void btnScheduleTraining_MouseLeave(object sender, EventArgs e)
        {
            this.btnScheduleTraining.ImageOptions.Image = Resources._2026_Demo_filled;
        }

        private void btnRequestCustomQuote_MouseEnter(object sender, EventArgs e)
        {
            this.btnRequestCustomQuote.ImageOptions.Image = Resources._2026_Quote_hover;
        }

        private void btnRequestCustomQuote_MouseLeave(object sender, EventArgs e)
        {
            this.btnRequestCustomQuote.ImageOptions.Image = Resources._2026_Quote_filled;
        }

        private void btnRegisterKey_MouseEnter(object sender, EventArgs e)
        {
            this.btnRegisterKey.ImageOptions.Image = Resources._2026_License_hover;
        }

        private void btnRegisterKey_MouseLeave(object sender, EventArgs e)
        {
            this.btnRegisterKey.ImageOptions.Image = Resources._2026_License_filled;
        }


        private void btnScheduleTraining_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://winpure.com/schedule-training/");
        }

        private void btnRequestCustomQuote_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://winpure.com/request-custom-quote/");

        }

        private void btnRegisterKey_Click(object sender, EventArgs e)
        {
            var frm = WinPureUiDependencyResolver.Resolve<frmRegister>();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                ShouldAppClose = frm.ShouldAppClose;
                if (ShouldAppClose)
                {
                    Close();
                    return;
                }
                UpdateLicenseInfo(true);
            }
        }

        private void btnContinueWithDemo_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void UpdateLicenseInfo(bool shouldClose = false)
        {
            var licenseService = WinPureUiDependencyResolver.Resolve<ILicenseService>();
            var licenseExpireAt = licenseService.LicenseExpiredAtDays;
            if (licenseExpireAt <= 0)
            {
                lbLicenseInfo.Text = (licenseService.IsDemo)
                    ? Resources.EXCEPTION_DEMO_LICENSE_EXPIRED
                    : Resources.EXCEPTION_LICENSE_EXPIRED;

                btnContinueWithDemo.Enabled = false;
            }
            else
            {
                if (shouldClose && !licenseService.IsDemo)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }

                lbLicenseInfo.Text = (licenseService.IsDemo)
                    ? string.Format(Resources.MESSAGE_TRIAL_DAY_LEFT, licenseExpireAt, licenseService.ExpirationDays)
                    : Resources.MESSAGE_LICENSE_EXPIRED;
                btnContinueWithDemo.Enabled = licenseService.IsDemo && licenseExpireAt > 0;
            }
            if (btnContinueWithDemo.Enabled)
            {
                btnContinueWithDemo.BackgroundImage = Resources._2026_HoverContinueDemo_50;
                btnContinueWithDemo.ForeColor = Color.White;
            }
            else
            {
                btnContinueWithDemo.BackgroundImage = Resources._2026_DisabledContinueDemo_50;
                btnContinueWithDemo.ForeColor = Color.Gray;
            }
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            Close();
        }



        private void btnContinueWithDemo_MouseEnter(object sender, EventArgs e)
        {
            this.btnContinueWithDemo.BackgroundImage = Resources._2026_OnlineHelpManual_filled_50;

        }

        private void btnContinueWithDemo_MouseLeave(object sender, EventArgs e)
        {
            this.btnContinueWithDemo.BackgroundImage = Resources._2026_HoverContinueDemo_50;
        }
    }
}