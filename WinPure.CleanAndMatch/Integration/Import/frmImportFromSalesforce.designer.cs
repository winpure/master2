namespace WinPure.CleanAndMatch.Integration.Import
{
    partial class frmImportFromSalesforce
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImportFromSalesforce));
            this.groupBox1 = new DevExpress.XtraEditors.GroupControl();
            this.btnRefreshServers = new DevExpress.XtraEditors.SimpleButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.cbSandBox = new DevExpress.XtraEditors.CheckEdit();
            this.txtToken = new DevExpress.XtraEditors.TextEdit();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();
            this.txtLogin = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.groupBox3 = new DevExpress.XtraEditors.GroupControl();
            this.lstTables = new DevExpress.XtraEditors.ListBoxControl();
            this.dGridSample = new DevExpress.XtraGrid.GridControl();
            this.gvSample = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lbError = new WinPure.CleanAndMatch.Controls.GrowLabel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.pnlSettings = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbSandBox.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtToken.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox3)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lstTables)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dGridSample)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSample)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSettings)).BeginInit();
            this.pnlSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.groupBox1.AppearanceCaption.Options.UseFont = true;
            this.groupBox1.Controls.Add(this.btnRefreshServers);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.labelControl4);
            this.groupBox1.Controls.Add(this.cbSandBox);
            this.groupBox1.Controls.Add(this.txtToken);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.txtLogin);
            this.groupBox1.Controls.Add(this.labelControl3);
            this.groupBox1.Controls.Add(this.labelControl2);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(464, 234);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.Text = "Connection Information";
            // 
            // btnRefreshServers
            // 
            this.btnRefreshServers.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefreshServers.Appearance.Options.UseFont = true;
            this.btnRefreshServers.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnRefreshServers.ImageOptions.SvgImage")));
            this.btnRefreshServers.Location = new System.Drawing.Point(129, 168);
            this.btnRefreshServers.Name = "btnRefreshServers";
            this.btnRefreshServers.Size = new System.Drawing.Size(152, 35);
            this.btnRefreshServers.TabIndex = 20;
            this.btnRefreshServers.Text = "Connect";
            this.btnRefreshServers.Click += new System.EventHandler(this.btnCheckConnect_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(316, 38);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(113, 95);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(22, 141);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(67, 13);
            this.labelControl4.TabIndex = 18;
            this.labelControl4.Text = "Use Sandbox";
            // 
            // cbSandBox
            // 
            this.cbSandBox.Location = new System.Drawing.Point(96, 143);
            this.cbSandBox.Name = "cbSandBox";
            this.cbSandBox.Properties.Caption = "";
            this.cbSandBox.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbSandBox.Size = new System.Drawing.Size(29, 20);
            this.cbSandBox.TabIndex = 3;
            // 
            // txtToken
            // 
            this.txtToken.Location = new System.Drawing.Point(96, 109);
            this.txtToken.Name = "txtToken";
            this.txtToken.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtToken.Properties.Appearance.Options.UseFont = true;
            this.txtToken.Size = new System.Drawing.Size(208, 24);
            this.txtToken.TabIndex = 2;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(96, 71);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtPassword.Properties.Appearance.Options.UseFont = true;
            this.txtPassword.Size = new System.Drawing.Size(208, 24);
            this.txtPassword.TabIndex = 1;
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(96, 33);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtLogin.Properties.Appearance.Options.UseFont = true;
            this.txtLogin.Size = new System.Drawing.Size(208, 24);
            this.txtLogin.TabIndex = 0;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(13, 107);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(74, 13);
            this.labelControl3.TabIndex = 2;
            this.labelControl3.Text = "Security Token";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(42, 70);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(49, 13);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "Password";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(61, 32);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(29, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Login";
            // 
            // groupBox3
            // 
            this.groupBox3.AppearanceCaption.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupBox3.AppearanceCaption.Options.UseFont = true;
            this.groupBox3.Controls.Add(this.lstTables);
            this.groupBox3.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupBox3.Location = new System.Drawing.Point(487, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(272, 244);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.Text = "Tables";
            // 
            // lstTables
            // 
            this.lstTables.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.lstTables.Appearance.Options.UseFont = true;
            this.lstTables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTables.Location = new System.Drawing.Point(2, 17);
            this.lstTables.Name = "lstTables";
            this.lstTables.Size = new System.Drawing.Size(268, 225);
            this.lstTables.TabIndex = 0;
            this.lstTables.SelectedIndexChanged += new System.EventHandler(this.lstTables_SelectedIndexChanged);
            // 
            // dGridSample
            // 
            this.dGridSample.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dGridSample.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.dGridSample.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.dGridSample.Location = new System.Drawing.Point(0, 29);
            this.dGridSample.MainView = this.gvSample;
            this.dGridSample.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.dGridSample.Name = "dGridSample";
            this.dGridSample.Size = new System.Drawing.Size(782, 292);
            this.dGridSample.TabIndex = 0;
            this.dGridSample.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvSample});
            // 
            // gvSample
            // 
            this.gvSample.Appearance.HeaderPanel.Font = new System.Drawing.Font("Open Sans Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gvSample.Appearance.HeaderPanel.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gvSample.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvSample.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvSample.Appearance.Row.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.gvSample.Appearance.Row.Options.UseFont = true;
            this.gvSample.DetailHeight = 404;
            this.gvSample.GridControl = this.dGridSample;
            this.gvSample.Name = "gvSample";
            this.gvSample.OptionsBehavior.FocusLeaveOnTab = true;
            this.gvSample.OptionsBehavior.ReadOnly = true;
            this.gvSample.OptionsEditForm.PopupEditFormWidth = 933;
            this.gvSample.OptionsView.ColumnAutoWidth = false;
            this.gvSample.OptionsView.ShowGroupPanel = false;
            // 
            // lbError
            // 
            this.lbError.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbError.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lbError.Appearance.Options.UseFont = true;
            this.lbError.Appearance.Options.UseForeColor = true;
            this.lbError.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbError.Location = new System.Drawing.Point(0, 0);
            this.lbError.Margin = new System.Windows.Forms.Padding(6);
            this.lbError.Name = "lbError";
            this.lbError.Padding = new System.Windows.Forms.Padding(5);
            this.lbError.Size = new System.Drawing.Size(95, 28);
            this.lbError.TabIndex = 78;
            this.lbError.Text = "labelControl1";
            this.lbError.Visible = false;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.groupBox3);
            this.pnlSettings.Controls.Add(this.groupBox1);
            this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSettings.Location = new System.Drawing.Point(0, 24);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Padding = new System.Windows.Forms.Padding(4);
            this.pnlSettings.Size = new System.Drawing.Size(765, 246);
            this.pnlSettings.TabIndex = 112;
            // 
            // frmImportFromSalesforce
            // 
            this.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 582);
            this.Controls.Add(this.pnlSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("frmImportFromSalesforce.IconOptions.Icon")));
            this.IconOptions.Image = global::WinPure.CleanAndMatch.Properties.Resources._2019_salesforce;
            this.IconOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("frmImportFromSalesforce.IconOptions.LargeImage")));
            this.IconOptions.ShowIcon = false;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImportFromSalesforce";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import from Salesforce";
            this.Controls.SetChildIndex(this.pnlSettings, 0);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbSandBox.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtToken.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox3)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lstTables)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dGridSample)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSample)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSettings)).EndInit();
            this.pnlSettings.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupBox1;
        private DevExpress.XtraEditors.GroupControl groupBox3;
        private DevExpress.XtraEditors.ListBoxControl lstTables;
        private DevExpress.XtraGrid.Views.Grid.GridView gvSample;
        private DevExpress.XtraEditors.TextEdit txtToken;
        private DevExpress.XtraEditors.TextEdit txtPassword;
        private DevExpress.XtraEditors.TextEdit txtLogin;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.ImageList imageList1;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.CheckEdit cbSandBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private GridControl dGridSample;
        private Controls.GrowLabel lbError;
        private PanelControl pnlSettings;
        private SimpleButton btnRefreshServers;
    }
}