namespace WinPure.CleanAndMatch.Controls
{
    partial class UCMainDataNew
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
            this.navFrameData = new DevExpress.XtraBars.Navigation.NavigationFrame();
            this.navPageProject = new DevExpress.XtraBars.Navigation.NavigationPage();
            this.ucNewProject = new WinPure.CleanAndMatch.Controls.UCNewProject();
            this.navPageImport = new DevExpress.XtraBars.Navigation.NavigationPage();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.importDataSourceNewControl = new WinPure.CleanAndMatch.Controls.DataSourceNewControl();
            this.DataSourceTitle = new DevExpress.XtraEditors.LabelControl();
            this.navPageExport = new DevExpress.XtraBars.Navigation.NavigationPage();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.exportDataSourceNewControl = new WinPure.CleanAndMatch.Controls.DataSourceNewControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitDataContainer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitDataContainer.Panel1)).BeginInit();
            this.splitDataContainer.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitDataContainer.Panel2)).BeginInit();
            this.splitDataContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tcData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.navFrameData)).BeginInit();
            this.navFrameData.SuspendLayout();
            this.navPageProject.SuspendLayout();
            this.navPageImport.SuspendLayout();
            this.navPageExport.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitDataContainer
            // 
            this.splitDataContainer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            // 
            // splitDataContainer.Panel1
            // 
            this.splitDataContainer.Panel1.Controls.Add(this.navFrameData);
            this.splitDataContainer.Size = new System.Drawing.Size(1260, 506);
            this.splitDataContainer.SplitterPosition = 158;
            // 
            // tcData
            // 
            this.tcData.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcData.Appearance.Options.UseFont = true;
            this.tcData.AppearancePage.Header.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.tcData.AppearancePage.Header.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.tcData.AppearancePage.Header.Options.UseFont = true;
            this.tcData.AppearancePage.Header.Options.UseForeColor = true;
            this.tcData.AppearancePage.HeaderActive.BackColor = System.Drawing.SystemColors.Highlight;
            this.tcData.AppearancePage.HeaderActive.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.tcData.AppearancePage.HeaderActive.Options.UseBackColor = true;
            this.tcData.AppearancePage.HeaderActive.Options.UseFont = true;
            this.tcData.AppearancePage.HeaderDisabled.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.tcData.AppearancePage.HeaderDisabled.Options.UseFont = true;
            this.tcData.AppearancePage.HeaderHotTracked.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.tcData.AppearancePage.HeaderHotTracked.Options.UseFont = true;
            this.tcData.AppearancePage.PageClient.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.tcData.AppearancePage.PageClient.Options.UseFont = true;
            this.tcData.Location = new System.Drawing.Point(0, 0);
            this.tcData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tcData.Size = new System.Drawing.Size(1256, 332);
            // 
            // navFrameData
            // 
            this.navFrameData.Controls.Add(this.navPageProject);
            this.navFrameData.Controls.Add(this.navPageImport);
            this.navFrameData.Controls.Add(this.navPageExport);
            this.navFrameData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navFrameData.Location = new System.Drawing.Point(0, 0);
            this.navFrameData.Name = "navFrameData";
            this.navFrameData.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] {
            this.navPageProject,
            this.navPageImport,
            this.navPageExport});
            this.navFrameData.SelectedPage = this.navPageProject;
            this.navFrameData.Size = new System.Drawing.Size(1252, 154);
            this.navFrameData.TabIndex = 1;
            this.navFrameData.Text = "navigationFrame1";
            this.navFrameData.TransitionAnimationProperties.FrameCount = 100;
            this.navFrameData.SelectedPageChanged += new DevExpress.XtraBars.Navigation.SelectedPageChangedEventHandler(this.navFrameData_SelectedPageChanged);
            // 
            // navPageProject
            // 
            this.navPageProject.Caption = "navPageProject";
            this.navPageProject.Controls.Add(this.ucNewProject);
            this.navPageProject.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F);
            this.navPageProject.Name = "navPageProject";
            this.navPageProject.Size = new System.Drawing.Size(1252, 154);
            // 
            // ucNewProject
            // 
            this.ucNewProject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucNewProject.Location = new System.Drawing.Point(0, 0);
            this.ucNewProject.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ucNewProject.Name = "ucNewProject";
            this.ucNewProject.Size = new System.Drawing.Size(1252, 154);
            this.ucNewProject.TabIndex = 1;
            this.ucNewProject.OnOpenProject += new System.Action(this.ucNewProject_OnOpenProject);
            this.ucNewProject.OnSaveProject += new System.Action<bool>(this.ucNewProject_OnSaveProject);
            this.ucNewProject.OnCreateNewProject += new System.Action(this.ucNewProject_OnCreateNewProject);
            this.ucNewProject.OnOpenSettings += new System.Action(this.ucNewProject_OnOpenSettings);
            // 
            // navPageImport
            // 
            this.navPageImport.Controls.Add(this.labelControl2);
            this.navPageImport.Controls.Add(this.importDataSourceNewControl);
            this.navPageImport.Controls.Add(this.DataSourceTitle);
            this.navPageImport.Name = "navPageImport";
            this.navPageImport.Size = new System.Drawing.Size(1252, 154);
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(33, 40);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(357, 17);
            this.labelControl2.TabIndex = 6;
            this.labelControl2.Text = "Click + to add your first connector and begin importing data.";
            // 
            // importDataSourceNewControl
            // 
            this.importDataSourceNewControl.Location = new System.Drawing.Point(1, 53);
            this.importDataSourceNewControl.Name = "importDataSourceNewControl";
            this.importDataSourceNewControl.Size = new System.Drawing.Size(1101, 292);
            this.importDataSourceNewControl.TabIndex = 5;
            // 
            // DataSourceTitle
            // 
            this.DataSourceTitle.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 20.25F, System.Drawing.FontStyle.Bold);
            this.DataSourceTitle.Appearance.Options.UseFont = true;
            this.DataSourceTitle.Location = new System.Drawing.Point(11, 4);
            this.DataSourceTitle.Name = "DataSourceTitle";
            this.DataSourceTitle.Size = new System.Drawing.Size(169, 37);
            this.DataSourceTitle.TabIndex = 3;
            this.DataSourceTitle.Text = "   Import Data";
            // 
            // navPageExport
            // 
            this.navPageExport.Controls.Add(this.labelControl3);
            this.navPageExport.Controls.Add(this.exportDataSourceNewControl);
            this.navPageExport.Controls.Add(this.labelControl1);
            this.navPageExport.Name = "navPageExport";
            this.navPageExport.Size = new System.Drawing.Size(1252, 154);
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(33, 40);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(356, 17);
            this.labelControl3.TabIndex = 51;
            this.labelControl3.Text = "Click + to add your first connector and begin exporting data.";
            // 
            // exportDataSourceNewControl
            // 
            this.exportDataSourceNewControl.Location = new System.Drawing.Point(1, 53);
            this.exportDataSourceNewControl.Name = "exportDataSourceNewControl";
            this.exportDataSourceNewControl.Size = new System.Drawing.Size(1101, 292);
            this.exportDataSourceNewControl.TabIndex = 50;
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 20.25F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(11, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(165, 37);
            this.labelControl1.TabIndex = 4;
            this.labelControl1.Text = "   Export Data";
            // 
            // UCMainDataNew
            // 
            this.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "UCMainDataNew";
            this.Size = new System.Drawing.Size(1260, 506);
            ((System.ComponentModel.ISupportInitialize)(this.splitDataContainer.Panel1)).EndInit();
            this.splitDataContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitDataContainer.Panel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitDataContainer)).EndInit();
            this.splitDataContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tcData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.navFrameData)).EndInit();
            this.navFrameData.ResumeLayout(false);
            this.navPageProject.ResumeLayout(false);
            this.navPageImport.ResumeLayout(false);
            this.navPageImport.PerformLayout();
            this.navPageExport.ResumeLayout(false);
            this.navPageExport.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        public DevExpress.XtraBars.Navigation.NavigationFrame navFrameData;
        internal UCNewProject ucNewProject;
        private LabelControl DataSourceTitle;
        private LabelControl labelControl1;
        public DevExpress.XtraBars.Navigation.NavigationPage navPageProject;
        public DevExpress.XtraBars.Navigation.NavigationPage navPageImport;
        public DevExpress.XtraBars.Navigation.NavigationPage navPageExport;
        private DataSourceNewControl importDataSourceNewControl;
        private DataSourceNewControl exportDataSourceNewControl;
        private LabelControl labelControl2;
        private LabelControl labelControl3;
    }
}
