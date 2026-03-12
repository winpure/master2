namespace WinPure.CleanAndMatch.Integration.Export
{
    partial class frmExportExcel
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
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions1 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExportExcel));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.btnFilePath = new DevExpress.XtraEditors.ButtonEdit();
            this.chkFirstRow = new DevExpress.XtraEditors.CheckEdit();
            this.cbNpoiExport = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilePath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFirstRow.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbNpoiExport.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl2
            // 
            this.groupControl2.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.groupControl2.AppearanceCaption.Options.UseFont = true;
            this.groupControl2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupControl2.Controls.Add(this.btnFilePath);
            this.groupControl2.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupControl2.Location = new System.Drawing.Point(17, 17);
            this.groupControl2.Margin = new System.Windows.Forms.Padding(8, 8, 8, 11);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Padding = new System.Windows.Forms.Padding(5);
            this.groupControl2.Size = new System.Drawing.Size(565, 90);
            this.groupControl2.TabIndex = 0;
            this.groupControl2.Text = "File";
            // 
            // btnFilePath
            // 
            this.btnFilePath.Location = new System.Drawing.Point(16, 29);
            this.btnFilePath.Name = "btnFilePath";
            this.btnFilePath.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilePath.Properties.Appearance.Options.UseFont = true;
            editorButtonImageOptions1.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("editorButtonImageOptions1.SvgImage")));
            this.btnFilePath.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions1, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.btnFilePath.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnFilePath.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.btnFilePath.Size = new System.Drawing.Size(533, 36);
            this.btnFilePath.TabIndex = 0;
            this.btnFilePath.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnFilePath_ButtonClick);
            this.btnFilePath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnFilePath_KeyDown);
            // 
            // chkFirstRow
            // 
            this.chkFirstRow.EditValue = true;
            this.chkFirstRow.Location = new System.Drawing.Point(17, 114);
            this.chkFirstRow.Name = "chkFirstRow";
            this.chkFirstRow.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.chkFirstRow.Properties.Appearance.Options.UseFont = true;
            this.chkFirstRow.Properties.Caption = "  First row contains column names";
            this.chkFirstRow.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.chkFirstRow.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.chkFirstRow.Size = new System.Drawing.Size(241, 20);
            this.chkFirstRow.TabIndex = 1;
            // 
            // cbNpoiExport
            // 
            this.cbNpoiExport.Location = new System.Drawing.Point(264, 114);
            this.cbNpoiExport.Name = "cbNpoiExport";
            this.cbNpoiExport.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.cbNpoiExport.Properties.Appearance.Options.UseFont = true;
            this.cbNpoiExport.Properties.Caption = "Use universal export";
            this.cbNpoiExport.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbNpoiExport.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbNpoiExport.Size = new System.Drawing.Size(316, 20);
            this.cbNpoiExport.TabIndex = 2;
            // 
            // frmExportExcel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 216);
            this.Controls.Add(this.cbNpoiExport);
            this.Controls.Add(this.chkFirstRow);
            this.Controls.Add(this.groupControl2);
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("frmExportExcel.IconOptions.Icon")));
            this.IconOptions.ShowIcon = false;
            this.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmExportExcel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export to Excel file";
            this.Controls.SetChildIndex(this.groupControl2, 0);
            this.Controls.SetChildIndex(this.chkFirstRow, 0);
            this.Controls.SetChildIndex(this.cbNpoiExport, 0);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnFilePath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFirstRow.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbNpoiExport.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.ButtonEdit btnFilePath;
        private DevExpress.XtraEditors.CheckEdit chkFirstRow;
        private DevExpress.XtraEditors.CheckEdit cbNpoiExport;
    }
}