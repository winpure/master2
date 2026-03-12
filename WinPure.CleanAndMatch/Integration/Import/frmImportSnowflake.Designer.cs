namespace WinPure.CleanAndMatch.Integration.Import
{
    partial class frmImportSnowflake
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImportSnowflake));
            this.dGridSample = new DevExpress.XtraGrid.GridControl();
            this.gvSample = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lbError = new DevExpress.XtraEditors.LabelControl();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();
            this.txtLogin = new DevExpress.XtraEditors.TextEdit();
            this.lbPassword = new DevExpress.XtraEditors.LabelControl();
            this.lbLogin = new DevExpress.XtraEditors.LabelControl();
            this.lbDatabase = new DevExpress.XtraEditors.LabelControl();
            this.txtDatabase = new DevExpress.XtraEditors.TextEdit();
            this.txtTable = new DevExpress.XtraEditors.TextEdit();
            this.txtSchema = new DevExpress.XtraEditors.TextEdit();
            this.lbSchema = new DevExpress.XtraEditors.LabelControl();
            this.lbTable = new DevExpress.XtraEditors.LabelControl();
            this.btnPreview = new DevExpress.XtraEditors.SimpleButton();
            this.txtAccount = new DevExpress.XtraEditors.TextEdit();
            this.lbAccount = new DevExpress.XtraEditors.LabelControl();
            this.txtOrganization = new DevExpress.XtraEditors.TextEdit();
            this.lbOrganization = new DevExpress.XtraEditors.LabelControl();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pnlSettings = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.dGridSample)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSample)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDatabase.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTable.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSchema.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAccount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrganization.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSettings)).BeginInit();
            this.pnlSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // dGridSample
            // 
            this.dGridSample.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dGridSample.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.dGridSample.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.dGridSample.Location = new System.Drawing.Point(2, 30);
            this.dGridSample.MainView = this.gvSample;
            this.dGridSample.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.dGridSample.Name = "dGridSample";
            this.dGridSample.Size = new System.Drawing.Size(481, 209);
            this.dGridSample.TabIndex = 0;
            this.dGridSample.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvSample,
            this.gridView1});
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
            // gridView1
            // 
            this.gridView1.DetailHeight = 404;
            this.gridView1.GridControl = this.dGridSample;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsEditForm.PopupEditFormWidth = 933;
            // 
            // lbError
            // 
            this.lbError.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbError.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lbError.Appearance.Options.UseFont = true;
            this.lbError.Appearance.Options.UseForeColor = true;
            this.lbError.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbError.Location = new System.Drawing.Point(2, 2);
            this.lbError.Margin = new System.Windows.Forms.Padding(6);
            this.lbError.Name = "lbError";
            this.lbError.Padding = new System.Windows.Forms.Padding(5);
            this.lbError.Size = new System.Drawing.Size(95, 28);
            this.lbError.TabIndex = 78;
            this.lbError.Text = "labelControl1";
            this.lbError.Visible = false;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(108, 101);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.Properties.Appearance.Options.UseFont = true;
            this.txtPassword.Properties.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(184, 24);
            this.txtPassword.TabIndex = 3;
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(108, 71);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLogin.Properties.Appearance.Options.UseFont = true;
            this.txtLogin.Size = new System.Drawing.Size(184, 24);
            this.txtLogin.TabIndex = 2;
            // 
            // lbPassword
            // 
            this.lbPassword.Location = new System.Drawing.Point(27, 105);
            this.lbPassword.Name = "lbPassword";
            this.lbPassword.Size = new System.Drawing.Size(49, 13);
            this.lbPassword.TabIndex = 87;
            this.lbPassword.Text = "Password";
            // 
            // lbLogin
            // 
            this.lbLogin.Location = new System.Drawing.Point(27, 76);
            this.lbLogin.Name = "lbLogin";
            this.lbLogin.Size = new System.Drawing.Size(29, 13);
            this.lbLogin.TabIndex = 86;
            this.lbLogin.Text = "Login";
            // 
            // lbDatabase
            // 
            this.lbDatabase.Location = new System.Drawing.Point(27, 135);
            this.lbDatabase.Name = "lbDatabase";
            this.lbDatabase.Size = new System.Drawing.Size(48, 13);
            this.lbDatabase.TabIndex = 90;
            this.lbDatabase.Text = "Database";
            // 
            // txtDatabase
            // 
            this.txtDatabase.Location = new System.Drawing.Point(108, 131);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDatabase.Properties.Appearance.Options.UseFont = true;
            this.txtDatabase.Size = new System.Drawing.Size(184, 24);
            this.txtDatabase.TabIndex = 4;
            // 
            // txtTable
            // 
            this.txtTable.Location = new System.Drawing.Point(108, 191);
            this.txtTable.Name = "txtTable";
            this.txtTable.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTable.Properties.Appearance.Options.UseFont = true;
            this.txtTable.Size = new System.Drawing.Size(184, 24);
            this.txtTable.TabIndex = 6;
            // 
            // txtSchema
            // 
            this.txtSchema.Location = new System.Drawing.Point(108, 161);
            this.txtSchema.Name = "txtSchema";
            this.txtSchema.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSchema.Properties.Appearance.Options.UseFont = true;
            this.txtSchema.Size = new System.Drawing.Size(184, 24);
            this.txtSchema.TabIndex = 5;
            // 
            // lbSchema
            // 
            this.lbSchema.Location = new System.Drawing.Point(27, 166);
            this.lbSchema.Name = "lbSchema";
            this.lbSchema.Size = new System.Drawing.Size(39, 13);
            this.lbSchema.TabIndex = 94;
            this.lbSchema.Text = "Schema";
            // 
            // lbTable
            // 
            this.lbTable.Location = new System.Drawing.Point(27, 194);
            this.lbTable.Name = "lbTable";
            this.lbTable.Size = new System.Drawing.Size(27, 13);
            this.lbTable.TabIndex = 96;
            this.lbTable.Text = "Table";
            // 
            // btnPreview
            // 
            this.btnPreview.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPreview.Appearance.Options.UseFont = true;
            this.btnPreview.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnPreview.ImageOptions.SvgImage")));
            this.btnPreview.Location = new System.Drawing.Point(301, 166);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(125, 49);
            this.btnPreview.TabIndex = 7;
            this.btnPreview.Text = "Preview";
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // txtAccount
            // 
            this.txtAccount.Location = new System.Drawing.Point(108, 41);
            this.txtAccount.Name = "txtAccount";
            this.txtAccount.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAccount.Properties.Appearance.Options.UseFont = true;
            this.txtAccount.Size = new System.Drawing.Size(184, 24);
            this.txtAccount.TabIndex = 1;
            // 
            // lbAccount
            // 
            this.lbAccount.Location = new System.Drawing.Point(27, 46);
            this.lbAccount.Name = "lbAccount";
            this.lbAccount.Size = new System.Drawing.Size(42, 13);
            this.lbAccount.TabIndex = 104;
            this.lbAccount.Text = "Account";
            // 
            // txtOrganization
            // 
            this.txtOrganization.Location = new System.Drawing.Point(108, 11);
            this.txtOrganization.Name = "txtOrganization";
            this.txtOrganization.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOrganization.Properties.Appearance.Options.UseFont = true;
            this.txtOrganization.Size = new System.Drawing.Size(184, 24);
            this.txtOrganization.TabIndex = 0;
            // 
            // lbOrganization
            // 
            this.lbOrganization.Location = new System.Drawing.Point(27, 15);
            this.lbOrganization.Name = "lbOrganization";
            this.lbOrganization.Size = new System.Drawing.Size(68, 13);
            this.lbOrganization.TabIndex = 102;
            this.lbOrganization.Text = "Organization";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(347, 70);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(15, 19);
            this.pictureBox2.TabIndex = 107;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Tag = "";
            this.toolTip1.SetToolTip(this.pictureBox2, "Click to learn more");
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.pictureBox2);
            this.pnlSettings.Controls.Add(this.lbOrganization);
            this.pnlSettings.Controls.Add(this.lbDatabase);
            this.pnlSettings.Controls.Add(this.txtAccount);
            this.pnlSettings.Controls.Add(this.lbLogin);
            this.pnlSettings.Controls.Add(this.lbAccount);
            this.pnlSettings.Controls.Add(this.lbPassword);
            this.pnlSettings.Controls.Add(this.txtOrganization);
            this.pnlSettings.Controls.Add(this.txtLogin);
            this.pnlSettings.Controls.Add(this.txtPassword);
            this.pnlSettings.Controls.Add(this.btnPreview);
            this.pnlSettings.Controls.Add(this.txtDatabase);
            this.pnlSettings.Controls.Add(this.txtTable);
            this.pnlSettings.Controls.Add(this.lbTable);
            this.pnlSettings.Controls.Add(this.txtSchema);
            this.pnlSettings.Controls.Add(this.lbSchema);
            this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSettings.Location = new System.Drawing.Point(0, 24);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(453, 240);
            this.pnlSettings.TabIndex = 112;
            // 
            // frmImportSnowflake
            // 
            this.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 566);
            this.Controls.Add(this.pnlSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.Image = global::WinPure.CleanAndMatch.Properties.Resources.Snowflake_16;
            this.IconOptions.LargeImage = global::WinPure.CleanAndMatch.Properties.Resources.database_32x32;
            this.IconOptions.ShowIcon = false;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImportSnowflake";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import table from Snowflake";
            this.Controls.SetChildIndex(this.pnlSettings, 0);
            ((System.ComponentModel.ISupportInitialize)(this.dGridSample)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSample)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDatabase.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTable.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSchema.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAccount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrganization.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSettings)).EndInit();
            this.pnlSettings.ResumeLayout(false);
            this.pnlSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraGrid.Views.Grid.GridView gvSample;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.TextEdit txtPassword;
        private DevExpress.XtraEditors.TextEdit txtLogin;
        private DevExpress.XtraEditors.LabelControl lbPassword;
        private DevExpress.XtraEditors.LabelControl lbLogin;
        private DevExpress.XtraEditors.LabelControl lbDatabase;
        private DevExpress.XtraEditors.TextEdit txtDatabase;
        private DevExpress.XtraEditors.TextEdit txtTable;
        private DevExpress.XtraEditors.TextEdit txtSchema;
        private DevExpress.XtraEditors.LabelControl lbSchema;
        private DevExpress.XtraEditors.LabelControl lbTable;
        private DevExpress.XtraEditors.SimpleButton btnPreview;
        private DevExpress.XtraEditors.TextEdit txtAccount;
        private DevExpress.XtraEditors.LabelControl lbAccount;
        private DevExpress.XtraEditors.TextEdit txtOrganization;
        private DevExpress.XtraEditors.LabelControl lbOrganization;
        private System.Windows.Forms.ToolTip toolTip1;
        private GridControl dGridSample;
        private LabelControl lbError;
        private PanelControl pnlSettings;
        private PictureBox pictureBox2;
    }
}