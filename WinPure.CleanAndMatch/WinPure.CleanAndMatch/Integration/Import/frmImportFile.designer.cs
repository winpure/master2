namespace WinPure.CleanAndMatch.Integration.Import
{
    partial class frmImportFile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImportFile));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.btnFilePath = new DevExpress.XtraEditors.ButtonEdit();
            this.pnlSettings = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilePath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSettings)).BeginInit();
            this.pnlSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupControl2
            // 
            this.groupControl2.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupControl2.AppearanceCaption.Options.UseFont = true;
            this.groupControl2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupControl2.Controls.Add(this.btnFilePath);
            this.groupControl2.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupControl2.Location = new System.Drawing.Point(7, 7);
            this.groupControl2.Margin = new System.Windows.Forms.Padding(8, 8, 8, 11);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Padding = new System.Windows.Forms.Padding(5);
            this.groupControl2.Size = new System.Drawing.Size(579, 69);
            this.groupControl2.TabIndex = 0;
            this.groupControl2.Text = "File";
            // 
            // btnFilePath
            // 
            this.btnFilePath.Location = new System.Drawing.Point(23, 21);
            this.btnFilePath.Name = "btnFilePath";
            this.btnFilePath.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.groupControl2);
            this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSettings.Location = new System.Drawing.Point(5, 29);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Padding = new System.Windows.Forms.Padding(5);
            this.pnlSettings.Size = new System.Drawing.Size(593, 88);
            this.pnlSettings.TabIndex = 112;
            // 
            // frmImportFile
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(603, 424);
            this.Controls.Add(this.pnlSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("frmImportFile.IconOptions.Icon")));
            this.IconOptions.ShowIcon = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImportFile";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import from text file";
            this.Controls.SetChildIndex(this.pnlSettings, 0);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnFilePath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSettings)).EndInit();
            this.pnlSettings.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.ButtonEdit btnFilePath;
        private PanelControl pnlSettings;
    }
}