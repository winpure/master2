namespace WinPure.CleanAndMatch.Support
{
    partial class frmCleansingRegex
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCleansingRegex));
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClear = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.lbDescription = new DevExpress.XtraEditors.LabelControl();
            this.txtDescription = new DevExpress.XtraEditors.TextEdit();
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            this.lbReplacemen = new DevExpress.XtraEditors.LabelControl();
            this.lbExpression = new DevExpress.XtraEditors.LabelControl();
            this.txtReplacement = new DevExpress.XtraEditors.TextEdit();
            this.txtExpression = new DevExpress.XtraEditors.TextEdit();
            this.gridRegexConfiguration = new DevExpress.XtraGrid.GridControl();
            this.gvRegexConfiguration = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colValue = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colReplacement = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoBtnDeleteRegexValue = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReplacement.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExpression.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridRegexConfiguration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvRegexConfiguration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnDeleteRegexValue)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.btnCancel);
            this.panelControl2.Controls.Add(this.btnOK);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl2.Location = new System.Drawing.Point(0, 558);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(892, 55);
            this.panelControl2.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(801, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(79, 27);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnOK.Appearance.Options.UseFont = true;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(716, 16);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(79, 27);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "Save";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click_1);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLoad,
            this.mnuSave,
            this.mnuClear});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(892, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuLoad
            // 
            this.mnuLoad.Name = "mnuLoad";
            this.mnuLoad.Size = new System.Drawing.Size(45, 20);
            this.mnuLoad.Text = "Load";
            this.mnuLoad.Click += new System.EventHandler(this.mnuLoad_Click);
            // 
            // mnuSave
            // 
            this.mnuSave.Name = "mnuSave";
            this.mnuSave.Size = new System.Drawing.Size(43, 20);
            this.mnuSave.Text = "Save";
            this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            // 
            // mnuClear
            // 
            this.mnuClear.Name = "mnuClear";
            this.mnuClear.Size = new System.Drawing.Size(46, 20);
            this.mnuClear.Text = "Clear";
            this.mnuClear.Click += new System.EventHandler(this.mnuClear_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(865, 25);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(15, 19);
            this.pictureBox1.TabIndex = 72;
            this.pictureBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBox1, "Click to Learn more");
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // groupControl2
            // 
            this.groupControl2.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupControl2.AppearanceCaption.Options.UseFont = true;
            this.groupControl2.AppearanceCaption.Options.UseTextOptions = true;
            this.groupControl2.AppearanceCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.groupControl2.Controls.Add(this.lbDescription);
            this.groupControl2.Controls.Add(this.txtDescription);
            this.groupControl2.Controls.Add(this.btnAdd);
            this.groupControl2.Controls.Add(this.lbReplacemen);
            this.groupControl2.Controls.Add(this.lbExpression);
            this.groupControl2.Controls.Add(this.txtReplacement);
            this.groupControl2.Controls.Add(this.txtExpression);
            this.groupControl2.Controls.Add(this.pictureBox1);
            this.groupControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupControl2.Location = new System.Drawing.Point(0, 24);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(892, 135);
            this.groupControl2.TabIndex = 7;
            this.groupControl2.Text = "REGEX editor";
            // 
            // lbDescription
            // 
            this.lbDescription.Location = new System.Drawing.Point(24, 104);
            this.lbDescription.Name = "lbDescription";
            this.lbDescription.Size = new System.Drawing.Size(59, 13);
            this.lbDescription.TabIndex = 79;
            this.lbDescription.Text = "Description";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(202, 100);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(406, 22);
            this.txtDescription.TabIndex = 78;
            // 
            // btnAdd
            // 
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnAdd.ImageOptions.SvgImage")));
            this.btnAdd.Location = new System.Drawing.Point(629, 88);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(94, 31);
            this.btnAdd.TabIndex = 77;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lbReplacemen
            // 
            this.lbReplacemen.Location = new System.Drawing.Point(24, 69);
            this.lbReplacemen.Name = "lbReplacemen";
            this.lbReplacemen.Size = new System.Drawing.Size(132, 13);
            this.lbReplacemen.TabIndex = 76;
            this.lbReplacemen.Text = "REGEX Replacement value";
            // 
            // lbExpression
            // 
            this.lbExpression.Location = new System.Drawing.Point(24, 34);
            this.lbExpression.Name = "lbExpression";
            this.lbExpression.Size = new System.Drawing.Size(91, 13);
            this.lbExpression.TabIndex = 75;
            this.lbExpression.Text = "REGEX Expression";
            // 
            // txtReplacement
            // 
            this.txtReplacement.Location = new System.Drawing.Point(202, 65);
            this.txtReplacement.Name = "txtReplacement";
            this.txtReplacement.Size = new System.Drawing.Size(406, 22);
            this.txtReplacement.TabIndex = 74;
            // 
            // txtExpression
            // 
            this.txtExpression.Location = new System.Drawing.Point(202, 30);
            this.txtExpression.Name = "txtExpression";
            this.txtExpression.Size = new System.Drawing.Size(406, 22);
            this.txtExpression.TabIndex = 73;
            // 
            // gridRegexConfiguration
            // 
            this.gridRegexConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridRegexConfiguration.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridRegexConfiguration.Location = new System.Drawing.Point(0, 159);
            this.gridRegexConfiguration.MainView = this.gvRegexConfiguration;
            this.gridRegexConfiguration.Name = "gridRegexConfiguration";
            this.gridRegexConfiguration.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoBtnDeleteRegexValue});
            this.gridRegexConfiguration.Size = new System.Drawing.Size(892, 399);
            this.gridRegexConfiguration.TabIndex = 8;
            this.gridRegexConfiguration.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvRegexConfiguration});
            // 
            // gvRegexConfiguration
            // 
            this.gvRegexConfiguration.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.gvRegexConfiguration.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvRegexConfiguration.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colId,
            this.colValue,
            this.colReplacement,
            this.colDescription,
            this.gridColumn5});
            this.gvRegexConfiguration.GridControl = this.gridRegexConfiguration;
            this.gvRegexConfiguration.Name = "gvRegexConfiguration";
            this.gvRegexConfiguration.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gvRegexConfiguration.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.gvRegexConfiguration.OptionsFilter.DefaultFilterEditorView = DevExpress.XtraEditors.FilterEditorViewMode.VisualAndText;
            this.gvRegexConfiguration.OptionsMenu.EnableColumnMenu = false;
            this.gvRegexConfiguration.OptionsMenu.EnableFooterMenu = false;
            this.gvRegexConfiguration.OptionsMenu.EnableGroupPanelMenu = false;
            this.gvRegexConfiguration.OptionsMenu.ShowAutoFilterRowItem = false;
            this.gvRegexConfiguration.OptionsMenu.ShowGroupSortSummaryItems = false;
            this.gvRegexConfiguration.OptionsView.ColumnAutoWidth = false;
            this.gvRegexConfiguration.OptionsView.ShowFooter = true;
            this.gvRegexConfiguration.OptionsView.ShowGroupPanel = false;
            this.gvRegexConfiguration.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.colId, DevExpress.Data.ColumnSortOrder.Ascending)});
            // 
            // colId
            // 
            this.colId.Caption = "#";
            this.colId.FieldName = "Id";
            this.colId.MaxWidth = 60;
            this.colId.MinWidth = 40;
            this.colId.Name = "colId";
            this.colId.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.colId.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
            this.colId.Visible = true;
            this.colId.VisibleIndex = 0;
            this.colId.Width = 40;
            // 
            // colValue
            // 
            this.colValue.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.colValue.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.colValue.AppearanceHeader.Options.UseFont = true;
            this.colValue.AppearanceHeader.Options.UseForeColor = true;
            this.colValue.Caption = "REGEX Expression";
            this.colValue.FieldName = "Expression";
            this.colValue.Name = "colValue";
            this.colValue.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colValue.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, "SearchValue", "{0}")});
            this.colValue.Visible = true;
            this.colValue.VisibleIndex = 1;
            this.colValue.Width = 284;
            // 
            // colReplacement
            // 
            this.colReplacement.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 8F);
            this.colReplacement.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.colReplacement.AppearanceHeader.Options.UseFont = true;
            this.colReplacement.AppearanceHeader.Options.UseForeColor = true;
            this.colReplacement.Caption = "REGEX Replacement value";
            this.colReplacement.FieldName = "Replacement";
            this.colReplacement.Name = "colReplacement";
            this.colReplacement.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colReplacement.Visible = true;
            this.colReplacement.VisibleIndex = 2;
            this.colReplacement.Width = 273;
            // 
            // colDescription
            // 
            this.colDescription.Caption = "Description";
            this.colDescription.FieldName = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 3;
            this.colDescription.Width = 239;
            // 
            // gridColumn5
            // 
            this.gridColumn5.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn5.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumn5.ColumnEdit = this.repoBtnDeleteRegexValue;
            this.gridColumn5.MaxWidth = 24;
            this.gridColumn5.MinWidth = 24;
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn5.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            this.gridColumn5.Width = 24;
            // 
            // repoBtnDeleteRegexValue
            // 
            this.repoBtnDeleteRegexValue.AutoHeight = false;
            this.repoBtnDeleteRegexValue.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Close)});
            this.repoBtnDeleteRegexValue.Name = "repoBtnDeleteRegexValue";
            this.repoBtnDeleteRegexValue.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.repoBtnDeleteRegexValue.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repoBtnDeleteRegexValue_ButtonClick);
            // 
            // frmCleansingRegex
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(892, 613);
            this.Controls.Add(this.gridRegexConfiguration);
            this.Controls.Add(this.groupControl2);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.menuStrip1);
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("frmCleansingRegex.IconOptions.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCleansingRegex";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "REGEX Editor";
            this.Load += new System.EventHandler(this.frmCleansingRegex_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            this.groupControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReplacement.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExpression.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridRegexConfiguration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvRegexConfiguration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnDeleteRegexValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuLoad;
        private System.Windows.Forms.ToolStripMenuItem mnuSave;
        private System.Windows.Forms.ToolStripMenuItem mnuClear;
        private System.Windows.Forms.ToolTip toolTip1;
        private GroupControl groupControl2;
        private PictureBox pictureBox1;
        private LabelControl lbReplacemen;
        private LabelControl lbExpression;
        private TextEdit txtReplacement;
        private TextEdit txtExpression;
        private SimpleButton btnAdd;
        private GridControl gridRegexConfiguration;
        private GridView gvRegexConfiguration;
        private GridColumn colValue;
        private GridColumn colReplacement;
        private GridColumn gridColumn5;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoBtnDeleteRegexValue;
        private LabelControl lbDescription;
        private TextEdit txtDescription;
        private GridColumn colDescription;
        private GridColumn colId;
    }
}