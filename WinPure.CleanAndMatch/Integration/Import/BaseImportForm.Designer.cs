namespace WinPure.CleanAndMatch.Integration.Import
{
    partial class BaseImportForm
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
            this.pnlBottom = new DevExpress.XtraEditors.PanelControl();
            this.pnlPreview = new DevExpress.XtraEditors.PanelControl();
            this.dGridSample = new DevExpress.XtraGrid.GridControl();
            this.gvSample = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lbError = new DevExpress.XtraEditors.LabelControl();
            this.pnlButtons = new DevExpress.XtraEditors.PanelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnImport = new DevExpress.XtraEditors.SimpleButton();
            this.previewSplitter = new DevExpress.XtraEditors.SplitterControl();
            ((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).BeginInit();
            this.pnlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlPreview)).BeginInit();
            this.pnlPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGridSample)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSample)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.pnlPreview);
            this.pnlBottom.Controls.Add(this.pnlButtons);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 265);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(650, 290);
            this.pnlBottom.TabIndex = 110;
            // 
            // pnlPreview
            // 
            this.pnlPreview.Controls.Add(this.dGridSample);
            this.pnlPreview.Controls.Add(this.lbError);
            this.pnlPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPreview.Location = new System.Drawing.Point(2, 2);
            this.pnlPreview.Margin = new System.Windows.Forms.Padding(4);
            this.pnlPreview.Name = "pnlPreview";
            this.pnlPreview.Size = new System.Drawing.Size(646, 247);
            this.pnlPreview.TabIndex = 3;
            // 
            // dGridSample
            // 
            this.dGridSample.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dGridSample.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dGridSample.Location = new System.Drawing.Point(2, 28);
            this.dGridSample.MainView = this.gvSample;
            this.dGridSample.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dGridSample.Name = "dGridSample";
            this.dGridSample.Size = new System.Drawing.Size(642, 217);
            this.dGridSample.TabIndex = 0;
            this.dGridSample.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvSample});
            // 
            // gvSample
            // 
            this.gvSample.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gvSample.Appearance.HeaderPanel.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gvSample.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvSample.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvSample.GridControl = this.dGridSample;
            this.gvSample.Name = "gvSample";
            this.gvSample.OptionsBehavior.FocusLeaveOnTab = true;
            this.gvSample.OptionsBehavior.ReadOnly = true;
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
            this.lbError.Location = new System.Drawing.Point(2, 2);
            this.lbError.Margin = new System.Windows.Forms.Padding(5);
            this.lbError.Name = "lbError";
            this.lbError.Padding = new System.Windows.Forms.Padding(4);
            this.lbError.Size = new System.Drawing.Size(93, 26);
            this.lbError.TabIndex = 78;
            this.lbError.Text = "labelControl1";
            this.lbError.Visible = false;
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Controls.Add(this.btnImport);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(2, 249);
            this.pnlButtons.Margin = new System.Windows.Forms.Padding(4);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(4, 4, 4, 8);
            this.pnlButtons.Size = new System.Drawing.Size(646, 39);
            this.pnlButtons.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(554, 6);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(79, 27);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImport.Appearance.Options.UseFont = true;
            this.btnImport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImport.Location = new System.Drawing.Point(469, 6);
            this.btnImport.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(79, 27);
            this.btnImport.TabIndex = 0;
            this.btnImport.Text = "Import";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // previewSplitter
            // 
            this.previewSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.previewSplitter.Location = new System.Drawing.Point(0, 253);
            this.previewSplitter.Name = "previewSplitter";
            this.previewSplitter.Size = new System.Drawing.Size(650, 12);
            this.previewSplitter.TabIndex = 111;
            this.previewSplitter.TabStop = false;
            this.previewSplitter.Visible = false;
            // 
            // BaseImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 565);
            this.Controls.Add(this.previewSplitter);
            this.Controls.Add(this.pnlBottom);
            this.IconOptions.ShowIcon = false;
            this.Name = "BaseImportForm";
            this.Text = "BaseImportForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BaseImportForm_FormClosing);
            this.Controls.SetChildIndex(this.pnlBottom, 0);
            this.Controls.SetChildIndex(this.previewSplitter, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlPreview)).EndInit();
            this.pnlPreview.ResumeLayout(false);
            this.pnlPreview.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGridSample)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSample)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PanelControl pnlBottom;
        private PanelControl pnlPreview;
        private GridControl dGridSample;
        private GridView gvSample;
        private LabelControl lbError;
        private PanelControl pnlButtons;
        private SimpleButton btnCancel;
        private SimpleButton btnImport;
        private SplitterControl previewSplitter;
    }
}