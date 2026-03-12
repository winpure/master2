namespace WinPure.CleanAndMatch.Controls
{
    partial class SelectionBar
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.cbFieldList = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnInvert = new DevExpress.XtraEditors.SimpleButton();
            this.btnUncheckAll = new DevExpress.XtraEditors.SimpleButton();
            this.btnCheckAll = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbFieldList.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.cbFieldList);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Controls.Add(this.btnInvert);
            this.panelControl1.Controls.Add(this.btnUncheckAll);
            this.panelControl1.Controls.Add(this.btnCheckAll);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(478, 40);
            this.panelControl1.TabIndex = 0;
            // 
            // cbFieldList
            // 
            this.cbFieldList.Cursor = System.Windows.Forms.Cursors.Default;
            this.cbFieldList.Location = new System.Drawing.Point(332, 6);
            this.cbFieldList.Name = "cbFieldList";
            this.cbFieldList.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.cbFieldList.Properties.Appearance.Options.UseFont = true;
            this.cbFieldList.Properties.AutoHeight = false;
            this.cbFieldList.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbFieldList.Properties.DropDownRows = 10;
            this.cbFieldList.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbFieldList.Size = new System.Drawing.Size(132, 25);
            this.cbFieldList.TabIndex = 4;
            // 
            // labelControl1
            // 
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelControl1.Location = new System.Drawing.Point(2, 2);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(3, 36);
            this.labelControl1.TabIndex = 5;
            this.labelControl1.Text = "          ";
            // 
            // btnInvert
            // 
            this.btnInvert.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInvert.Appearance.Options.UseFont = true;
            this.btnInvert.ImageOptions.SvgImage = global::WinPure.CleanAndMatch.Properties.Resources._2026_InvertSelection_20;
            this.btnInvert.Location = new System.Drawing.Point(220, 6);
            this.btnInvert.Name = "btnInvert";
            this.btnInvert.Size = new System.Drawing.Size(111, 25);
            this.btnInvert.TabIndex = 2;
            this.btnInvert.Text = "Invert selection";
            this.btnInvert.Click += new System.EventHandler(this.btnInvert_Click);
            // 
            // btnUncheckAll
            // 
            this.btnUncheckAll.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnUncheckAll.Appearance.Options.UseFont = true;
            this.btnUncheckAll.ImageOptions.SvgImage = global::WinPure.CleanAndMatch.Properties.Resources._2026_Indeterminate_Checkbox;
            this.btnUncheckAll.Location = new System.Drawing.Point(111, 6);
            this.btnUncheckAll.Name = "btnUncheckAll";
            this.btnUncheckAll.Size = new System.Drawing.Size(109, 25);
            this.btnUncheckAll.TabIndex = 1;
            this.btnUncheckAll.Text = "Uncheck all";
            this.btnUncheckAll.Click += new System.EventHandler(this.btnUncheckAll_Click);
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnCheckAll.Appearance.Options.UseFont = true;
            this.btnCheckAll.ImageOptions.SvgImage = global::WinPure.CleanAndMatch.Properties.Resources._2026_Check_All_20;
            this.btnCheckAll.Location = new System.Drawing.Point(5, 6);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(106, 25);
            this.btnCheckAll.TabIndex = 0;
            this.btnCheckAll.Text = "Check all";
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // SelectionBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControl1);
            this.Name = "SelectionBar";
            this.Size = new System.Drawing.Size(478, 40);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cbFieldList.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.ComboBoxEdit cbFieldList;
        private DevExpress.XtraEditors.SimpleButton btnInvert;
        private DevExpress.XtraEditors.SimpleButton btnUncheckAll;
        private DevExpress.XtraEditors.SimpleButton btnCheckAll;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}
