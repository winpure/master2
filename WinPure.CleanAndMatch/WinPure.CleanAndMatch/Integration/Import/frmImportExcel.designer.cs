namespace WinPure.CleanAndMatch.Integration.Import
{
    partial class frmImportExcel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImportExcel));
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions1 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.cbAnalyzeDataType = new DevExpress.XtraEditors.CheckEdit();
            this.lstTables = new DevExpress.XtraEditors.ListBoxControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.chkFirstRow = new DevExpress.XtraEditors.CheckEdit();
            this.btnFilePath = new DevExpress.XtraEditors.ButtonEdit();
            this.pnlSettings = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbAnalyzeDataType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lstTables)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFirstRow.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilePath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSettings)).BeginInit();
            this.pnlSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupControl2
            // 
            this.groupControl2.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupControl2.AppearanceCaption.Options.UseFont = true;
            this.groupControl2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupControl2.Controls.Add(this.pictureEdit1);
            this.groupControl2.Controls.Add(this.cbAnalyzeDataType);
            this.groupControl2.Controls.Add(this.lstTables);
            this.groupControl2.Controls.Add(this.labelControl1);
            this.groupControl2.Controls.Add(this.chkFirstRow);
            this.groupControl2.Controls.Add(this.btnFilePath);
            this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl2.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupControl2.Location = new System.Drawing.Point(7, 7);
            this.groupControl2.Margin = new System.Windows.Forms.Padding(8, 8, 8, 11);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Padding = new System.Windows.Forms.Padding(5);
            this.groupControl2.Size = new System.Drawing.Size(575, 218);
            this.groupControl2.TabIndex = 0;
            this.groupControl2.Text = "Excel file";
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.EditValue = ((object)(resources.GetObject("pictureEdit1.EditValue")));
            this.pictureEdit1.Location = new System.Drawing.Point(467, 73);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.pictureEdit1.Properties.Appearance.Options.UseBackColor = true;
            this.pictureEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEdit1.Properties.ShowMenu = false;
            this.pictureEdit1.Size = new System.Drawing.Size(22, 20);
            this.pictureEdit1.TabIndex = 112;
            this.pictureEdit1.Tag = "wordmanager";
            this.pictureEdit1.Click += new System.EventHandler(this.helpButton_Click);
            // 
            // cbAnalyzeDataType
            // 
            this.cbAnalyzeDataType.Location = new System.Drawing.Point(268, 73);
            this.cbAnalyzeDataType.Name = "cbAnalyzeDataType";
            this.cbAnalyzeDataType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.cbAnalyzeDataType.Properties.Appearance.Options.UseFont = true;
            this.cbAnalyzeDataType.Properties.Caption = "Analyze data type";
            this.cbAnalyzeDataType.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbAnalyzeDataType.Size = new System.Drawing.Size(193, 20);
            this.cbAnalyzeDataType.TabIndex = 2;
            // 
            // lstTables
            // 
            this.lstTables.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstTables.Appearance.Options.UseFont = true;
            this.lstTables.Location = new System.Drawing.Point(21, 132);
            this.lstTables.MultiColumn = true;
            this.lstTables.Name = "lstTables";
            this.lstTables.Size = new System.Drawing.Size(533, 81);
            this.lstTables.TabIndex = 3;
            this.lstTables.SelectedIndexChanged += new System.EventHandler(this.lstTables_SelectedIndexChanged);
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(21, 109);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(40, 17);
            this.labelControl1.TabIndex = 7;
            this.labelControl1.Text = "Sheets";
            // 
            // chkFirstRow
            // 
            this.chkFirstRow.EditValue = true;
            this.chkFirstRow.Location = new System.Drawing.Point(21, 73);
            this.chkFirstRow.Name = "chkFirstRow";
            this.chkFirstRow.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkFirstRow.Properties.Appearance.Options.UseFont = true;
            this.chkFirstRow.Properties.Caption = "First row contains column names";
            this.chkFirstRow.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.chkFirstRow.Size = new System.Drawing.Size(229, 20);
            this.chkFirstRow.TabIndex = 1;
            this.chkFirstRow.CheckStateChanged += new System.EventHandler(this.lstTables_SelectedIndexChanged);
            // 
            // btnFilePath
            // 
            this.btnFilePath.Location = new System.Drawing.Point(21, 31);
            this.btnFilePath.Name = "btnFilePath";
            this.btnFilePath.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilePath.Properties.Appearance.Options.UseFont = true;
            editorButtonImageOptions1.SvgImage = global::WinPure.CleanAndMatch.Properties.Resources._2026_OpenFile_24;
            this.btnFilePath.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions1, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.btnFilePath.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnFilePath.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.btnFilePath.Size = new System.Drawing.Size(533, 28);
            this.btnFilePath.TabIndex = 0;
            this.btnFilePath.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnFilePath_ButtonClick);
            this.btnFilePath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnFilePath_KeyDown);
            // 
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.groupControl2);
            this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSettings.Location = new System.Drawing.Point(0, 24);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Padding = new System.Windows.Forms.Padding(5);
            this.pnlSettings.Size = new System.Drawing.Size(589, 232);
            this.pnlSettings.TabIndex = 112;
            // 
            // frmImportExcel
            // 
            this.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseFont = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(589, 568);
            this.Controls.Add(this.pnlSettings);
            this.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("frmImportExcel.IconOptions.Icon")));
            this.IconOptions.ShowIcon = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImportExcel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import from Excel";
            this.Controls.SetChildIndex(this.pnlSettings, 0);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            this.groupControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbAnalyzeDataType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lstTables)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFirstRow.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilePath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSettings)).EndInit();
            this.pnlSettings.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.ListBoxControl lstTables;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.ButtonEdit btnFilePath;
        public DevExpress.XtraEditors.CheckEdit cbAnalyzeDataType;
        private PanelControl pnlSettings;
        private PictureEdit pictureEdit1;
        public CheckEdit chkFirstRow;
    }
}