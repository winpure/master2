namespace WinPure.CleanAndMatch.Controls
{
    partial class UCVerification
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.navFrameVerification = new DevExpress.XtraBars.Navigation.NavigationFrame();
            this.navPageConfigurtion = new DevExpress.XtraBars.Navigation.NavigationPage();
            this.ucAddressVerificationConfiguration = new WinPure.CleanAndMatch.Controls.UCAddressVerificationConfiguration();
            this.navPageReport = new DevExpress.XtraBars.Navigation.NavigationPage();
            this.addressReportViewer = new DevExpress.XtraPrinting.Preview.DocumentViewer();
            ((System.ComponentModel.ISupportInitialize)(this.navFrameVerification)).BeginInit();
            this.navFrameVerification.SuspendLayout();
            this.navPageConfigurtion.SuspendLayout();
            this.navPageReport.SuspendLayout();
            this.SuspendLayout();
            // 
            // navFrameVerification
            // 
            this.navFrameVerification.Controls.Add(this.navPageConfigurtion);
            this.navFrameVerification.Controls.Add(this.navPageReport);
            this.navFrameVerification.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navFrameVerification.Location = new System.Drawing.Point(0, 0);
            this.navFrameVerification.Name = "navFrameVerification";
            this.navFrameVerification.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] {
            this.navPageConfigurtion,
            this.navPageReport});
            this.navFrameVerification.SelectedPage = this.navPageReport;
            this.navFrameVerification.Size = new System.Drawing.Size(1227, 515);
            this.navFrameVerification.TabIndex = 0;
            this.navFrameVerification.Text = "navigationFrame1";
            this.navFrameVerification.TransitionAnimationProperties.FrameCount = 100;
            // 
            // navPageConfigurtion
            // 
            this.navPageConfigurtion.Caption = "navPageConfigurtion";
            this.navPageConfigurtion.Controls.Add(this.ucAddressVerificationConfiguration);
            this.navPageConfigurtion.Name = "navPageConfigurtion";
            this.navPageConfigurtion.Size = new System.Drawing.Size(1227, 515);
            // 
            // ucAddressVerificationConfiguration
            // 
            this.ucAddressVerificationConfiguration.AutoSize = true;
            this.ucAddressVerificationConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucAddressVerificationConfiguration.Location = new System.Drawing.Point(0, 0);
            this.ucAddressVerificationConfiguration.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ucAddressVerificationConfiguration.Name = "ucAddressVerificationConfiguration";
            this.ucAddressVerificationConfiguration.Size = new System.Drawing.Size(1227, 515);
            this.ucAddressVerificationConfiguration.TabIndex = 0;
            // 
            // navPageReport
            // 
            this.navPageReport.Caption = "navPageReport";
            this.navPageReport.Controls.Add(this.addressReportViewer);
            this.navPageReport.Name = "navPageReport";
            this.navPageReport.Size = new System.Drawing.Size(1227, 515);
            // 
            // addressReportViewer
            // 
            this.addressReportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addressReportViewer.IsMetric = true;
            this.addressReportViewer.Location = new System.Drawing.Point(0, 0);
            this.addressReportViewer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.addressReportViewer.Name = "addressReportViewer";
            this.addressReportViewer.Size = new System.Drawing.Size(1227, 515);
            this.addressReportViewer.TabIndex = 3;
            this.addressReportViewer.Visible = false;
            // 
            // UCVerification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.navFrameVerification);
            this.Name = "UCVerification";
            this.Size = new System.Drawing.Size(1227, 515);
            ((System.ComponentModel.ISupportInitialize)(this.navFrameVerification)).EndInit();
            this.navFrameVerification.ResumeLayout(false);
            this.navPageConfigurtion.ResumeLayout(false);
            this.navPageConfigurtion.PerformLayout();
            this.navPageReport.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Navigation.NavigationFrame navFrameVerification;
        private DevExpress.XtraBars.Navigation.NavigationPage navPageConfigurtion;
        private UCAddressVerificationConfiguration ucAddressVerificationConfiguration;
        private DevExpress.XtraBars.Navigation.NavigationPage navPageReport;
        private DevExpress.XtraPrinting.Preview.DocumentViewer addressReportViewer;
    }
}
