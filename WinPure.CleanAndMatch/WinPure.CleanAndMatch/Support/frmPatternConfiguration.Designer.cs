namespace WinPure.CleanAndMatch.Support
{
    partial class frmPatternConfiguration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPatternConfiguration));
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem1 = new DevExpress.Utils.ToolTipTitleItem();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.gridPatterns = new DevExpress.XtraGrid.GridControl();
            this.gvPatterns = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPattern = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colFieldType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoBtnDeletePatternValue = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.colId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.pnlAddValue = new DevExpress.XtraEditors.PanelControl();
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.lbFieldType = new DevExpress.XtraEditors.LabelControl();
            this.txtFieldType = new DevExpress.XtraEditors.TextEdit();
            this.lbDescription = new DevExpress.XtraEditors.LabelControl();
            this.txtDescription = new DevExpress.XtraEditors.TextEdit();
            this.lbPattern = new DevExpress.XtraEditors.LabelControl();
            this.txtPattern = new DevExpress.XtraEditors.TextEdit();
            this.lbName = new DevExpress.XtraEditors.LabelControl();
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            this.txtName = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridPatterns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvPatterns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnDeletePatternValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlAddValue)).BeginInit();
            this.pnlAddValue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFieldType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPattern.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnCancel);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 511);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(704, 40);
            this.panelControl1.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(607, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(79, 27);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Close";
            // 
            // gridPatterns
            // 
            this.gridPatterns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPatterns.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridPatterns.Location = new System.Drawing.Point(0, 121);
            this.gridPatterns.MainView = this.gvPatterns;
            this.gridPatterns.Name = "gridPatterns";
            this.gridPatterns.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoBtnDeletePatternValue});
            this.gridPatterns.Size = new System.Drawing.Size(704, 390);
            this.gridPatterns.TabIndex = 2;
            this.gridPatterns.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvPatterns});
            // 
            // gvPatterns
            // 
            this.gvPatterns.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.gvPatterns.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvPatterns.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colName,
            this.colPattern,
            this.colDescription,
            this.colFieldType,
            this.gridColumn5,
            this.colId});
            this.gvPatterns.GridControl = this.gridPatterns;
            this.gvPatterns.Name = "gvPatterns";
            this.gvPatterns.OptionsFilter.DefaultFilterEditorView = DevExpress.XtraEditors.FilterEditorViewMode.VisualAndText;
            this.gvPatterns.OptionsMenu.EnableColumnMenu = false;
            this.gvPatterns.OptionsMenu.EnableFooterMenu = false;
            this.gvPatterns.OptionsMenu.EnableGroupPanelMenu = false;
            this.gvPatterns.OptionsMenu.ShowAutoFilterRowItem = false;
            this.gvPatterns.OptionsMenu.ShowGroupSortSummaryItems = false;
            this.gvPatterns.OptionsView.ColumnAutoWidth = false;
            this.gvPatterns.OptionsView.ShowGroupPanel = false;
            this.gvPatterns.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.colName, DevExpress.Data.ColumnSortOrder.Ascending)});
            this.gvPatterns.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(this.gvPatterns_ValidateRow);
            // 
            // colName
            // 
            this.colName.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.colName.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.colName.AppearanceHeader.Options.UseFont = true;
            this.colName.AppearanceHeader.Options.UseForeColor = true;
            this.colName.Caption = "Pattern Name";
            this.colName.FieldName = "Name";
            this.colName.Name = "colName";
            this.colName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.colName.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, "SearchValue", "{0}")});
            this.colName.Visible = true;
            this.colName.VisibleIndex = 0;
            this.colName.Width = 143;
            // 
            // colPattern
            // 
            this.colPattern.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.colPattern.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.colPattern.AppearanceHeader.Options.UseFont = true;
            this.colPattern.AppearanceHeader.Options.UseForeColor = true;
            this.colPattern.Caption = "Regular Expression";
            this.colPattern.FieldName = "Pattern";
            this.colPattern.Name = "colPattern";
            this.colPattern.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.colPattern.Visible = true;
            this.colPattern.VisibleIndex = 1;
            this.colPattern.Width = 177;
            // 
            // colDescription
            // 
            this.colDescription.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.colDescription.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.colDescription.AppearanceHeader.Options.UseFont = true;
            this.colDescription.AppearanceHeader.Options.UseForeColor = true;
            this.colDescription.Caption = "Description";
            this.colDescription.FieldName = "Description";
            this.colDescription.MinWidth = 17;
            this.colDescription.Name = "colDescription";
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 2;
            this.colDescription.Width = 191;
            // 
            // colFieldType
            // 
            this.colFieldType.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.colFieldType.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.colFieldType.AppearanceHeader.Options.UseFont = true;
            this.colFieldType.AppearanceHeader.Options.UseForeColor = true;
            this.colFieldType.Caption = "Field Type";
            this.colFieldType.FieldName = "FieldType";
            this.colFieldType.MinWidth = 17;
            this.colFieldType.Name = "colFieldType";
            this.colFieldType.Visible = true;
            this.colFieldType.VisibleIndex = 3;
            this.colFieldType.Width = 123;
            // 
            // gridColumn5
            // 
            this.gridColumn5.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn5.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumn5.ColumnEdit = this.repoBtnDeletePatternValue;
            this.gridColumn5.MaxWidth = 24;
            this.gridColumn5.MinWidth = 24;
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn5.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            this.gridColumn5.Width = 24;
            // 
            // repoBtnDeletePatternValue
            // 
            this.repoBtnDeletePatternValue.AutoHeight = false;
            this.repoBtnDeletePatternValue.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Close)});
            this.repoBtnDeletePatternValue.Name = "repoBtnDeletePatternValue";
            this.repoBtnDeletePatternValue.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.repoBtnDeletePatternValue.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repoBtnDeletePatternValue_ButtonClick);
            // 
            // colId
            // 
            this.colId.Caption = "gridColumn1";
            this.colId.FieldName = "Id";
            this.colId.MinWidth = 17;
            this.colId.Name = "colId";
            this.colId.OptionsColumn.AllowEdit = false;
            this.colId.Width = 64;
            // 
            // pnlAddValue
            // 
            this.pnlAddValue.Controls.Add(this.pictureEdit1);
            this.pnlAddValue.Controls.Add(this.lbFieldType);
            this.pnlAddValue.Controls.Add(this.txtFieldType);
            this.pnlAddValue.Controls.Add(this.lbDescription);
            this.pnlAddValue.Controls.Add(this.txtDescription);
            this.pnlAddValue.Controls.Add(this.lbPattern);
            this.pnlAddValue.Controls.Add(this.txtPattern);
            this.pnlAddValue.Controls.Add(this.lbName);
            this.pnlAddValue.Controls.Add(this.btnAdd);
            this.pnlAddValue.Controls.Add(this.txtName);
            this.pnlAddValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlAddValue.Location = new System.Drawing.Point(0, 0);
            this.pnlAddValue.Name = "pnlAddValue";
            this.pnlAddValue.Size = new System.Drawing.Size(704, 121);
            this.pnlAddValue.TabIndex = 3;
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.EditValue = ((object)(resources.GetObject("pictureEdit1.EditValue")));
            this.pictureEdit1.Location = new System.Drawing.Point(664, 5);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.pictureEdit1.Properties.Appearance.Options.UseBackColor = true;
            this.pictureEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEdit1.Properties.ShowMenu = false;
            this.pictureEdit1.Size = new System.Drawing.Size(22, 20);
            this.pictureEdit1.TabIndex = 76;
            this.pictureEdit1.Tag = "patternmanager";
            this.pictureEdit1.Click += new System.EventHandler(this.helpButton_Click);
            // 
            // lbFieldType
            // 
            this.lbFieldType.Location = new System.Drawing.Point(23, 60);
            this.lbFieldType.Name = "lbFieldType";
            this.lbFieldType.Size = new System.Drawing.Size(50, 13);
            this.lbFieldType.TabIndex = 74;
            this.lbFieldType.Text = "Field type";
            // 
            // txtFieldType
            // 
            this.txtFieldType.Location = new System.Drawing.Point(23, 77);
            this.txtFieldType.Name = "txtFieldType";
            this.txtFieldType.Properties.AutoHeight = false;
            this.txtFieldType.Size = new System.Drawing.Size(156, 33);
            this.txtFieldType.TabIndex = 2;
            // 
            // lbDescription
            // 
            this.lbDescription.Location = new System.Drawing.Point(191, 60);
            this.lbDescription.Name = "lbDescription";
            this.lbDescription.Size = new System.Drawing.Size(59, 13);
            this.lbDescription.TabIndex = 72;
            this.lbDescription.Text = "Description";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(192, 77);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Properties.AutoHeight = false;
            this.txtDescription.Size = new System.Drawing.Size(261, 33);
            this.txtDescription.TabIndex = 3;
            // 
            // lbPattern
            // 
            this.lbPattern.Location = new System.Drawing.Point(191, 7);
            this.lbPattern.Name = "lbPattern";
            this.lbPattern.Size = new System.Drawing.Size(98, 13);
            this.lbPattern.TabIndex = 7;
            this.lbPattern.Text = "Regular Expression";
            // 
            // txtPattern
            // 
            this.txtPattern.Location = new System.Drawing.Point(190, 24);
            this.txtPattern.Name = "txtPattern";
            this.txtPattern.Properties.AutoHeight = false;
            this.txtPattern.Size = new System.Drawing.Size(263, 33);
            this.txtPattern.TabIndex = 1;
            // 
            // lbName
            // 
            this.lbName.Location = new System.Drawing.Point(23, 7);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(29, 13);
            this.lbName.TabIndex = 5;
            this.lbName.Text = "Name";
            // 
            // btnAdd
            // 
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnAdd.ImageOptions.SvgImage")));
            this.btnAdd.Location = new System.Drawing.Point(459, 76);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(94, 33);
            toolTipTitleItem1.Text = global::WinPure.CleanAndMatch.Properties.Resources.UI_EDITDICTIONARYFORM_ADDNEWVALUE;
            superToolTip1.Items.Add(toolTipTitleItem1);
            this.btnAdd.SuperTip = superToolTip1;
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(23, 24);
            this.txtName.Name = "txtName";
            this.txtName.Properties.AutoHeight = false;
            this.txtName.Size = new System.Drawing.Size(156, 33);
            this.txtName.TabIndex = 0;
            // 
            // frmPatternConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 551);
            this.Controls.Add(this.gridPatterns);
            this.Controls.Add(this.pnlAddValue);
            this.Controls.Add(this.panelControl1);
            this.IconOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("frmPatternConfiguration.IconOptions.LargeImage")));
            this.Name = "frmPatternConfiguration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pattern Manager";
            this.Load += new System.EventHandler(this.frmPatternConfiguration_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridPatterns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvPatterns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnDeletePatternValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlAddValue)).EndInit();
            this.pnlAddValue.ResumeLayout(false);
            this.pnlAddValue.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFieldType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPattern.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraGrid.GridControl gridPatterns;
        private DevExpress.XtraGrid.Views.Grid.GridView gvPatterns;
        private DevExpress.XtraGrid.Columns.GridColumn colName;
        private DevExpress.XtraGrid.Columns.GridColumn colPattern;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoBtnDeletePatternValue;
        private DevExpress.XtraEditors.PanelControl pnlAddValue;
        private DevExpress.XtraEditors.LabelControl lbDescription;
        private DevExpress.XtraEditors.TextEdit txtDescription;
        private DevExpress.XtraEditors.LabelControl lbPattern;
        private DevExpress.XtraEditors.TextEdit txtPattern;
        private DevExpress.XtraEditors.LabelControl lbName;
        private DevExpress.XtraEditors.SimpleButton btnAdd;
        private DevExpress.XtraEditors.TextEdit txtName;
        private DevExpress.XtraEditors.LabelControl lbFieldType;
        private DevExpress.XtraEditors.TextEdit txtFieldType;
        private DevExpress.XtraGrid.Columns.GridColumn colDescription;
        private DevExpress.XtraGrid.Columns.GridColumn colFieldType;
        private DevExpress.XtraGrid.Columns.GridColumn colId;
        private PictureEdit pictureEdit1;
    }
}