namespace WinPure.CleanAndMatch.Support
{
    partial class frmErDefaultMapping
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmErDefaultMapping));
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem1 = new DevExpress.Utils.ToolTipTitleItem();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.gridMappingData = new DevExpress.XtraGrid.GridControl();
            this.gvMappingData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoBtnDeleteDictionaryValue = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.pnlAddValue = new DevExpress.XtraEditors.PanelControl();
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.cbExactMatch = new DevExpress.XtraEditors.CheckEdit();
            this.lbLabel = new DevExpress.XtraEditors.LabelControl();
            this.txtLabel = new DevExpress.XtraEditors.TextEdit();
            this.lcExactMatch = new DevExpress.XtraEditors.LabelControl();
            this.lbErType = new DevExpress.XtraEditors.LabelControl();
            this.lbDataColumnName = new DevExpress.XtraEditors.LabelControl();
            this.btnAddNewErMapping = new DevExpress.XtraEditors.SimpleButton();
            this.txtDataColumnName = new DevExpress.XtraEditors.TextEdit();
            this.txtErType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMappingData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvMappingData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnDeleteDictionaryValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlAddValue)).BeginInit();
            this.pnlAddValue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbExactMatch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLabel.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDataColumnName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtErType.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnCancel);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 491);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(873, 41);
            this.panelControl1.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnCancel.ImageOptions.SvgImage")));
            this.btnCancel.Location = new System.Drawing.Point(767, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 31);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Close";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gridMappingData
            // 
            this.gridMappingData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridMappingData.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridMappingData.Location = new System.Drawing.Point(0, 68);
            this.gridMappingData.MainView = this.gvMappingData;
            this.gridMappingData.Name = "gridMappingData";
            this.gridMappingData.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoBtnDeleteDictionaryValue,
            this.repositoryItemCheckEdit1});
            this.gridMappingData.Size = new System.Drawing.Size(873, 423);
            this.gridMappingData.TabIndex = 2;
            this.gridMappingData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvMappingData});
            // 
            // gvMappingData
            // 
            this.gvMappingData.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.gvMappingData.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvMappingData.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn5});
            this.gvMappingData.GridControl = this.gridMappingData;
            this.gvMappingData.Name = "gvMappingData";
            this.gvMappingData.OptionsFilter.DefaultFilterEditorView = DevExpress.XtraEditors.FilterEditorViewMode.VisualAndText;
            this.gvMappingData.OptionsMenu.EnableColumnMenu = false;
            this.gvMappingData.OptionsMenu.EnableFooterMenu = false;
            this.gvMappingData.OptionsMenu.EnableGroupPanelMenu = false;
            this.gvMappingData.OptionsMenu.ShowGroupSortSummaryItems = false;
            this.gvMappingData.OptionsView.ColumnAutoWidth = false;
            this.gvMappingData.OptionsView.ShowFooter = true;
            this.gvMappingData.OptionsView.ShowGroupPanel = false;
            this.gvMappingData.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn3, DevExpress.Data.ColumnSortOrder.Ascending)});
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gridColumn3.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn3.AppearanceHeader.Options.UseFont = true;
            this.gridColumn3.Caption = "Data column name";
            this.gridColumn3.FieldName = "DataColumnName";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn3.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, "SearchValue", "{0}")});
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 0;
            this.gridColumn3.Width = 159;
            // 
            // gridColumn4
            // 
            this.gridColumn4.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gridColumn4.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn4.AppearanceHeader.Options.UseFont = true;
            this.gridColumn4.Caption = "Entity type";
            this.gridColumn4.FieldName = "EntityType";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 1;
            this.gridColumn4.Width = 158;
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gridColumn1.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn1.AppearanceHeader.Options.UseFont = true;
            this.gridColumn1.Caption = "Label";
            this.gridColumn1.FieldName = "UsageGroup";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 2;
            this.gridColumn1.Width = 127;
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gridColumn2.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn2.AppearanceHeader.Options.UseFont = true;
            this.gridColumn2.Caption = "Exact match";
            this.gridColumn2.ColumnEdit = this.repositoryItemCheckEdit1;
            this.gridColumn2.FieldName = "ExactMatch";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 3;
            this.gridColumn2.Width = 89;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // gridColumn6
            // 
            this.gridColumn6.AppearanceCell.BackColor = System.Drawing.Color.WhiteSmoke;
            this.gridColumn6.AppearanceCell.Options.UseBackColor = true;
            this.gridColumn6.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gridColumn6.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn6.AppearanceHeader.Options.UseFont = true;
            this.gridColumn6.Caption = "Conflicts";
            this.gridColumn6.FieldName = "ConflictEntityTypes";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.AllowEdit = false;
            this.gridColumn6.ToolTip = "This column is not editable and cannot be changed or removed";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 5;
            this.gridColumn6.Width = 145;
            // 
            // gridColumn7
            // 
            this.gridColumn7.AppearanceCell.BackColor = System.Drawing.Color.WhiteSmoke;
            this.gridColumn7.AppearanceCell.Options.UseBackColor = true;
            this.gridColumn7.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gridColumn7.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn7.AppearanceHeader.Options.UseFont = true;
            this.gridColumn7.Caption = "Prerequisite";
            this.gridColumn7.FieldName = "PrerequisiteEntityTypes";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.OptionsColumn.AllowEdit = false;
            this.gridColumn7.ToolTip = "This column is not editable and cannot be changed or removed";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 4;
            this.gridColumn7.Width = 129;
            // 
            // gridColumn5
            // 
            this.gridColumn5.AppearanceHeader.Font = new System.Drawing.Font("Open Sans Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.gridColumn5.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn5.AppearanceHeader.Options.UseFont = true;
            this.gridColumn5.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumn5.ColumnEdit = this.repoBtnDeleteDictionaryValue;
            this.gridColumn5.MaxWidth = 24;
            this.gridColumn5.MinWidth = 24;
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn5.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 6;
            this.gridColumn5.Width = 24;
            // 
            // repoBtnDeleteDictionaryValue
            // 
            this.repoBtnDeleteDictionaryValue.AutoHeight = false;
            this.repoBtnDeleteDictionaryValue.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Close)});
            this.repoBtnDeleteDictionaryValue.Name = "repoBtnDeleteDictionaryValue";
            this.repoBtnDeleteDictionaryValue.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.repoBtnDeleteDictionaryValue.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repoBtnDeleteDictionaryValue_ButtonClick);
            // 
            // pnlAddValue
            // 
            this.pnlAddValue.Controls.Add(this.pictureEdit1);
            this.pnlAddValue.Controls.Add(this.cbExactMatch);
            this.pnlAddValue.Controls.Add(this.lbLabel);
            this.pnlAddValue.Controls.Add(this.txtLabel);
            this.pnlAddValue.Controls.Add(this.lcExactMatch);
            this.pnlAddValue.Controls.Add(this.lbErType);
            this.pnlAddValue.Controls.Add(this.lbDataColumnName);
            this.pnlAddValue.Controls.Add(this.btnAddNewErMapping);
            this.pnlAddValue.Controls.Add(this.txtDataColumnName);
            this.pnlAddValue.Controls.Add(this.txtErType);
            this.pnlAddValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlAddValue.Location = new System.Drawing.Point(0, 0);
            this.pnlAddValue.Name = "pnlAddValue";
            this.pnlAddValue.Size = new System.Drawing.Size(873, 68);
            this.pnlAddValue.TabIndex = 3;
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.EditValue = ((object)(resources.GetObject("pictureEdit1.EditValue")));
            this.pictureEdit1.Location = new System.Drawing.Point(846, 3);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.pictureEdit1.Properties.Appearance.Options.UseBackColor = true;
            this.pictureEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEdit1.Properties.ShowMenu = false;
            this.pictureEdit1.Size = new System.Drawing.Size(22, 20);
            this.pictureEdit1.TabIndex = 75;
            this.pictureEdit1.Tag = "ERAutoMapConfig";
            this.pictureEdit1.Click += new System.EventHandler(this.helpButton_Click);
            // 
            // cbExactMatch
            // 
            this.cbExactMatch.Location = new System.Drawing.Point(543, 36);
            this.cbExactMatch.Name = "cbExactMatch";
            this.cbExactMatch.Properties.Caption = "";
            this.cbExactMatch.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbExactMatch.Size = new System.Drawing.Size(39, 20);
            this.cbExactMatch.TabIndex = 74;
            // 
            // lbLabel
            // 
            this.lbLabel.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLabel.Appearance.Options.UseFont = true;
            this.lbLabel.Location = new System.Drawing.Point(363, 13);
            this.lbLabel.Name = "lbLabel";
            this.lbLabel.Size = new System.Drawing.Size(27, 13);
            this.lbLabel.TabIndex = 73;
            this.lbLabel.Text = "Label";
            // 
            // txtLabel
            // 
            this.txtLabel.Location = new System.Drawing.Point(357, 31);
            this.txtLabel.Name = "txtLabel";
            this.txtLabel.Properties.AutoHeight = false;
            this.txtLabel.Size = new System.Drawing.Size(156, 31);
            this.txtLabel.TabIndex = 72;
            // 
            // lcExactMatch
            // 
            this.lcExactMatch.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lcExactMatch.Appearance.Options.UseFont = true;
            this.lcExactMatch.Location = new System.Drawing.Point(531, 13);
            this.lcExactMatch.Name = "lcExactMatch";
            this.lcExactMatch.Size = new System.Drawing.Size(60, 13);
            this.lcExactMatch.TabIndex = 71;
            this.lcExactMatch.Text = "Exact match";
            // 
            // lbErType
            // 
            this.lbErType.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbErType.Appearance.Options.UseFont = true;
            this.lbErType.Location = new System.Drawing.Point(192, 13);
            this.lbErType.Name = "lbErType";
            this.lbErType.Size = new System.Drawing.Size(38, 13);
            this.lbErType.TabIndex = 7;
            this.lbErType.Text = "ER type";
            // 
            // lbDataColumnName
            // 
            this.lbDataColumnName.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDataColumnName.Appearance.Options.UseFont = true;
            this.lbDataColumnName.Location = new System.Drawing.Point(20, 13);
            this.lbDataColumnName.Name = "lbDataColumnName";
            this.lbDataColumnName.Size = new System.Drawing.Size(96, 13);
            this.lbDataColumnName.TabIndex = 5;
            this.lbDataColumnName.Text = "Data column name";
            // 
            // btnAddNewErMapping
            // 
            this.btnAddNewErMapping.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddNewErMapping.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnAddNewErMapping.ImageOptions.SvgImage")));
            this.btnAddNewErMapping.Location = new System.Drawing.Point(607, 31);
            this.btnAddNewErMapping.Name = "btnAddNewErMapping";
            this.btnAddNewErMapping.Size = new System.Drawing.Size(86, 31);
            toolTipTitleItem1.Text = "Add new value onto Entity resolution default mapping configuration";
            superToolTip1.Items.Add(toolTipTitleItem1);
            this.btnAddNewErMapping.SuperTip = superToolTip1;
            this.btnAddNewErMapping.TabIndex = 5;
            this.btnAddNewErMapping.Text = "Add";
            this.btnAddNewErMapping.Click += new System.EventHandler(this.btnAddNewErMapping_Click);
            // 
            // txtDataColumnName
            // 
            this.txtDataColumnName.Location = new System.Drawing.Point(14, 31);
            this.txtDataColumnName.Name = "txtDataColumnName";
            this.txtDataColumnName.Properties.AutoHeight = false;
            this.txtDataColumnName.Size = new System.Drawing.Size(156, 31);
            this.txtDataColumnName.TabIndex = 3;
            // 
            // txtErType
            // 
            this.txtErType.Location = new System.Drawing.Point(186, 31);
            this.txtErType.Name = "txtErType";
            this.txtErType.Properties.AutoHeight = false;
            this.txtErType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtErType.Properties.DropDownRows = 15;
            this.txtErType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.txtErType.Size = new System.Drawing.Size(156, 31);
            this.txtErType.TabIndex = 4;
            // 
            // toolTip1
            // 
            this.toolTip1.Tag = "wordmanager";
            // 
            // frmErDefaultMapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(873, 532);
            this.Controls.Add(this.gridMappingData);
            this.Controls.Add(this.pnlAddValue);
            this.Controls.Add(this.panelControl1);
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("frmErDefaultMapping.IconOptions.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmErDefaultMapping";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MatchAI Auto Mapping Configuration";
            this.Load += new System.EventHandler(this.frmErDefaultMapping_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridMappingData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvMappingData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnDeleteDictionaryValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlAddValue)).EndInit();
            this.pnlAddValue.ResumeLayout(false);
            this.pnlAddValue.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbExactMatch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLabel.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDataColumnName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtErType.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraGrid.GridControl gridMappingData;
        private DevExpress.XtraGrid.Views.Grid.GridView gvMappingData;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoBtnDeleteDictionaryValue;
        private DevExpress.XtraEditors.PanelControl pnlAddValue;
        private DevExpress.XtraEditors.LabelControl lbErType;
        private DevExpress.XtraEditors.LabelControl lbDataColumnName;
        private DevExpress.XtraEditors.SimpleButton btnAddNewErMapping;
        private DevExpress.XtraEditors.TextEdit txtDataColumnName;
        private DevExpress.XtraEditors.LabelControl lbLabel;
        private DevExpress.XtraEditors.TextEdit txtLabel;
        private DevExpress.XtraEditors.LabelControl lcExactMatch;
        private DevExpress.XtraEditors.CheckEdit cbExactMatch;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraEditors.ComboBoxEdit txtErType;
        private System.Windows.Forms.ToolTip toolTip1;
        private PictureEdit pictureEdit1;
    }
}