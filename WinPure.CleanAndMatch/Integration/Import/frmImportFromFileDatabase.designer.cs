namespace WinPure.CleanAndMatch.Integration.Import
{
    partial class frmImportFromFileDatabase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImportFromFileDatabase));
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions1 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.groupBox1 = new DevExpress.XtraEditors.GroupControl();
            this.groupBox2 = new DevExpress.XtraEditors.GroupControl();
            this.rgAuthType = new DevExpress.XtraEditors.RadioGroup();
            this.btnCheckConnect = new DevExpress.XtraEditors.SimpleButton();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.btnFilePath = new DevExpress.XtraEditors.ButtonEdit();
            this.groupBox3 = new DevExpress.XtraEditors.GroupControl();
            this.lstTables = new DevExpress.XtraEditors.ListBoxControl();
            this.dGridSample = new DevExpress.XtraGrid.GridControl();
            this.gvSample = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lbError = new WinPure.CleanAndMatch.Controls.GrowLabel();
            this.pnlSettings = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgAuthType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilePath.Properties)).BeginInit();
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
            this.groupBox1.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.AppearanceCaption.Options.UseFont = true;
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.groupControl2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(509, 248);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.Text = "Connection Information";
            // 
            // groupBox2
            // 
            this.groupBox2.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.groupBox2.AppearanceCaption.Options.UseFont = true;
            this.groupBox2.Controls.Add(this.rgAuthType);
            this.groupBox2.Controls.Add(this.btnCheckConnect);
            this.groupBox2.Controls.Add(this.txtPassword);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupBox2.Location = new System.Drawing.Point(2, 103);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(505, 143);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.Text = "Connect to the file";
            // 
            // rgAuthType
            // 
            this.rgAuthType.Location = new System.Drawing.Point(21, 24);
            this.rgAuthType.Name = "rgAuthType";
            this.rgAuthType.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.rgAuthType.Properties.Appearance.Options.UseBackColor = true;
            this.rgAuthType.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.rgAuthType.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Without password", true, null, "rgWinAuth"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "With password", true, null, "rgServerAuth")});
            this.rgAuthType.Size = new System.Drawing.Size(173, 56);
            this.rgAuthType.TabIndex = 83;
            this.rgAuthType.SelectedIndexChanged += new System.EventHandler(this.rgAuthType_SelectedIndexChanged);
            // 
            // btnCheckConnect
            // 
            this.btnCheckConnect.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnCheckConnect.Appearance.Options.UseFont = true;
            this.btnCheckConnect.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnCheckConnect.ImageOptions.SvgImage")));
            this.btnCheckConnect.Location = new System.Drawing.Point(186, 88);
            this.btnCheckConnect.Name = "btnCheckConnect";
            this.btnCheckConnect.Size = new System.Drawing.Size(26, 22);
            this.btnCheckConnect.TabIndex = 3;
            this.btnCheckConnect.Click += new System.EventHandler(this.btnCheckConnect_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(21, 86);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtPassword.Properties.Appearance.Options.UseFont = true;
            this.txtPassword.Properties.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(159, 24);
            this.txtPassword.TabIndex = 2;
            // 
            // groupControl2
            // 
            this.groupControl2.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.groupControl2.AppearanceCaption.Options.UseFont = true;
            this.groupControl2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupControl2.Controls.Add(this.btnFilePath);
            this.groupControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupControl2.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupControl2.Location = new System.Drawing.Point(2, 17);
            this.groupControl2.Margin = new System.Windows.Forms.Padding(8, 8, 8, 11);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Padding = new System.Windows.Forms.Padding(5);
            this.groupControl2.Size = new System.Drawing.Size(505, 86);
            this.groupControl2.TabIndex = 0;
            this.groupControl2.Text = "Access database file";
            // 
            // btnFilePath
            // 
            this.btnFilePath.Location = new System.Drawing.Point(21, 31);
            this.btnFilePath.Name = "btnFilePath";
            this.btnFilePath.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilePath.Properties.Appearance.Options.UseFont = true;
            editorButtonImageOptions1.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("editorButtonImageOptions1.SvgImage")));
            this.btnFilePath.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions1, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.btnFilePath.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnFilePath.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.btnFilePath.Size = new System.Drawing.Size(463, 28);
            this.btnFilePath.TabIndex = 0;
            this.btnFilePath.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnFilePath_ButtonClick);
            this.btnFilePath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnFilePath_KeyDown);
            // 
            // groupBox3
            // 
            this.groupBox3.AppearanceCaption.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupBox3.AppearanceCaption.Options.UseFont = true;
            this.groupBox3.Controls.Add(this.lstTables);
            this.groupBox3.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupBox3.Location = new System.Drawing.Point(524, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(235, 248);
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
            this.lstTables.Size = new System.Drawing.Size(231, 229);
            this.lstTables.TabIndex = 0;
            this.lstTables.SelectedIndexChanged += new System.EventHandler(this.lstTables_SelectedIndexChanged);
            // 
            // dGridSample
            // 
            this.dGridSample.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dGridSample.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.dGridSample.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.dGridSample.Location = new System.Drawing.Point(0, 28);
            this.dGridSample.MainView = this.gvSample;
            this.dGridSample.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.dGridSample.Name = "dGridSample";
            this.dGridSample.Size = new System.Drawing.Size(832, 293);
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
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.groupBox3);
            this.pnlSettings.Controls.Add(this.groupBox1);
            this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSettings.Location = new System.Drawing.Point(0, 24);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Padding = new System.Windows.Forms.Padding(4);
            this.pnlSettings.Size = new System.Drawing.Size(765, 260);
            this.pnlSettings.TabIndex = 112;
            // 
            // frmImportFromFileDatabase
            // 
            this.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 596);
            this.Controls.Add(this.pnlSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("frmImportFromFileDatabase.IconOptions.Icon")));
            this.IconOptions.LargeImage = global::WinPure.CleanAndMatch.Properties.Resources.database_32x32;
            this.IconOptions.ShowIcon = false;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImportFromFileDatabase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import from Access";
            this.Controls.SetChildIndex(this.pnlSettings, 0);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rgAuthType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnFilePath.Properties)).EndInit();
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
        private DevExpress.XtraEditors.GroupControl groupBox2;
        private DevExpress.XtraEditors.TextEdit txtPassword;
        private DevExpress.XtraEditors.GroupControl groupBox3;
        private DevExpress.XtraEditors.ListBoxControl lstTables;
        private DevExpress.XtraGrid.Views.Grid.GridView gvSample;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.ButtonEdit btnFilePath;
        private DevExpress.XtraEditors.SimpleButton btnCheckConnect;
        private GridControl dGridSample;
        private Controls.GrowLabel lbError;
        private PanelControl pnlSettings;
        private RadioGroup rgAuthType;
    }
}