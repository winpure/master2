namespace WinPure.CleanAndMatch.Support
{
    partial class frmEditDictionary
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditDictionary));
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem1 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.SuperToolTip superToolTip2 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem2 = new DevExpress.Utils.ToolTipTitleItem();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.scDictionary = new DevExpress.XtraEditors.SplitContainerControl();
            this.gridDictionaryNames = new DevExpress.XtraGrid.GridControl();
            this.gvDictionaryNames = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.pnlAddDictionary = new DevExpress.XtraEditors.PanelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnAddNewDictionary = new DevExpress.XtraEditors.SimpleButton();
            this.txtDictionaryName = new DevExpress.XtraEditors.TextEdit();
            this.gridDictionaryData = new DevExpress.XtraGrid.GridControl();
            this.gvDictionaryData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoBtnDeleteDictionaryValue = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.pnlAddValue = new DevExpress.XtraEditors.PanelControl();
            this.helpButton = new System.Windows.Forms.PictureBox();
            this.labelColumnName = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtReplaceValue = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.btnAddNewDictionaryData = new DevExpress.XtraEditors.SimpleButton();
            this.txtSearchValue = new DevExpress.XtraEditors.TextEdit();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLoad = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scDictionary)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scDictionary.Panel1)).BeginInit();
            this.scDictionary.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scDictionary.Panel2)).BeginInit();
            this.scDictionary.Panel2.SuspendLayout();
            this.scDictionary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDictionaryNames)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDictionaryNames)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlAddDictionary)).BeginInit();
            this.pnlAddDictionary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDictionaryName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDictionaryData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDictionaryData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnDeleteDictionaryValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlAddValue)).BeginInit();
            this.pnlAddValue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.helpButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReplaceValue.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchValue.Properties)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnCancel);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 600);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(837, 40);
            this.panelControl1.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnCancel.ImageOptions.SvgImage")));
            this.btnCancel.Location = new System.Drawing.Point(720, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 31);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Close";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // scDictionary
            // 
            this.scDictionary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scDictionary.Location = new System.Drawing.Point(0, 24);
            this.scDictionary.Name = "scDictionary";
            // 
            // scDictionary.Panel1
            // 
            this.scDictionary.Panel1.Controls.Add(this.gridDictionaryNames);
            this.scDictionary.Panel1.Controls.Add(this.pnlAddDictionary);
            this.scDictionary.Panel1.Text = "Panel1";
            // 
            // scDictionary.Panel2
            // 
            this.scDictionary.Panel2.Controls.Add(this.gridDictionaryData);
            this.scDictionary.Panel2.Controls.Add(this.pnlAddValue);
            this.scDictionary.Panel2.Text = "Panel2";
            this.scDictionary.Size = new System.Drawing.Size(837, 576);
            this.scDictionary.SplitterPosition = 370;
            this.scDictionary.TabIndex = 1;
            this.scDictionary.Text = "scDictionary";
            // 
            // gridDictionaryNames
            // 
            this.gridDictionaryNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDictionaryNames.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gridDictionaryNames.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridDictionaryNames.Location = new System.Drawing.Point(0, 100);
            this.gridDictionaryNames.MainView = this.gvDictionaryNames;
            this.gridDictionaryNames.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gridDictionaryNames.Name = "gridDictionaryNames";
            this.gridDictionaryNames.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemButtonEdit1});
            this.gridDictionaryNames.Size = new System.Drawing.Size(370, 476);
            this.gridDictionaryNames.TabIndex = 2;
            this.gridDictionaryNames.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvDictionaryNames});
            // 
            // gvDictionaryNames
            // 
            this.gvDictionaryNames.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.gvDictionaryNames.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvDictionaryNames.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2});
            this.gvDictionaryNames.GridControl = this.gridDictionaryNames;
            this.gvDictionaryNames.Name = "gvDictionaryNames";
            this.gvDictionaryNames.OptionsFilter.DefaultFilterEditorView = DevExpress.XtraEditors.FilterEditorViewMode.VisualAndText;
            this.gvDictionaryNames.OptionsView.ShowGroupPanel = false;
            this.gvDictionaryNames.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn1, DevExpress.Data.ColumnSortOrder.Ascending)});
            this.gvDictionaryNames.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvDictionaryNames_FocusedRowChanged);
            this.gvDictionaryNames.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(this.gvDictionaryNames_ValidateRow);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridColumn1.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn1.AppearanceHeader.Options.UseFont = true;
            this.gridColumn1.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumn1.Caption = global::WinPure.CleanAndMatch.Properties.Resources.UI_PATTERNNAME;
            this.gridColumn1.FieldName = "Name";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 315;
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn2.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumn2.ColumnEdit = this.repositoryItemButtonEdit1;
            this.gridColumn2.MaxWidth = 24;
            this.gridColumn2.MinWidth = 24;
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 24;
            // 
            // repositoryItemButtonEdit1
            // 
            this.repositoryItemButtonEdit1.AutoHeight = false;
            this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Close)});
            this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
            this.repositoryItemButtonEdit1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.repositoryItemButtonEdit1.Click += new System.EventHandler(this.repositoryItemButtonEdit1_Click);
            // 
            // pnlAddDictionary
            // 
            this.pnlAddDictionary.Controls.Add(this.labelControl1);
            this.pnlAddDictionary.Controls.Add(this.btnAddNewDictionary);
            this.pnlAddDictionary.Controls.Add(this.txtDictionaryName);
            this.pnlAddDictionary.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlAddDictionary.Location = new System.Drawing.Point(0, 0);
            this.pnlAddDictionary.Name = "pnlAddDictionary";
            this.pnlAddDictionary.Size = new System.Drawing.Size(370, 100);
            this.pnlAddDictionary.TabIndex = 1;
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(23, 7);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(127, 21);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "Dictionary name";
            // 
            // btnAddNewDictionary
            // 
            this.btnAddNewDictionary.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddNewDictionary.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnAddNewDictionary.ImageOptions.SvgImage")));
            this.btnAddNewDictionary.Location = new System.Drawing.Point(185, 57);
            this.btnAddNewDictionary.Name = "btnAddNewDictionary";
            this.btnAddNewDictionary.Size = new System.Drawing.Size(94, 31);
            toolTipTitleItem1.Text = global::WinPure.CleanAndMatch.Properties.Resources.UI_EDITDICTIONARYFORM_ADDNEWLIBRARY;
            superToolTip1.Items.Add(toolTipTitleItem1);
            this.btnAddNewDictionary.SuperTip = superToolTip1;
            this.btnAddNewDictionary.TabIndex = 1;
            this.btnAddNewDictionary.Text = "Add";
            this.btnAddNewDictionary.Click += new System.EventHandler(this.btnAddNewDictionary_Click);
            // 
            // txtDictionaryName
            // 
            this.txtDictionaryName.Location = new System.Drawing.Point(23, 57);
            this.txtDictionaryName.Name = "txtDictionaryName";
            this.txtDictionaryName.Properties.AutoHeight = false;
            this.txtDictionaryName.Size = new System.Drawing.Size(156, 31);
            this.txtDictionaryName.TabIndex = 0;
            // 
            // gridDictionaryData
            // 
            this.gridDictionaryData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDictionaryData.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridDictionaryData.Location = new System.Drawing.Point(0, 100);
            this.gridDictionaryData.MainView = this.gvDictionaryData;
            this.gridDictionaryData.Name = "gridDictionaryData";
            this.gridDictionaryData.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoBtnDeleteDictionaryValue});
            this.gridDictionaryData.Size = new System.Drawing.Size(455, 476);
            this.gridDictionaryData.TabIndex = 0;
            this.gridDictionaryData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvDictionaryData});
            // 
            // gvDictionaryData
            // 
            this.gvDictionaryData.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.gvDictionaryData.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvDictionaryData.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5});
            this.gvDictionaryData.GridControl = this.gridDictionaryData;
            this.gvDictionaryData.Name = "gvDictionaryData";
            this.gvDictionaryData.OptionsFilter.DefaultFilterEditorView = DevExpress.XtraEditors.FilterEditorViewMode.VisualAndText;
            this.gvDictionaryData.OptionsMenu.EnableColumnMenu = false;
            this.gvDictionaryData.OptionsMenu.EnableFooterMenu = false;
            this.gvDictionaryData.OptionsMenu.EnableGroupPanelMenu = false;
            this.gvDictionaryData.OptionsMenu.ShowAutoFilterRowItem = false;
            this.gvDictionaryData.OptionsMenu.ShowGroupSortSummaryItems = false;
            this.gvDictionaryData.OptionsView.ColumnAutoWidth = false;
            this.gvDictionaryData.OptionsView.ShowFooter = true;
            this.gvDictionaryData.OptionsView.ShowGroupPanel = false;
            this.gvDictionaryData.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn3, DevExpress.Data.ColumnSortOrder.Ascending)});
            this.gvDictionaryData.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(this.gvDictionaryData_ValidateRow);
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gridColumn3.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn3.AppearanceHeader.Options.UseFont = true;
            this.gridColumn3.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumn3.Caption = global::WinPure.CleanAndMatch.Properties.Resources.UI_EDITDICTIONARYFORM_SEARCHVALUE;
            this.gridColumn3.FieldName = "SearchValue";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn3.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, "SearchValue", "{0}")});
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 0;
            this.gridColumn3.Width = 134;
            // 
            // gridColumn4
            // 
            this.gridColumn4.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 8F);
            this.gridColumn4.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn4.AppearanceHeader.Options.UseFont = true;
            this.gridColumn4.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumn4.Caption = "Substitute value (short term)";
            this.gridColumn4.FieldName = "ReplaceValue";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 1;
            this.gridColumn4.Width = 158;
            // 
            // gridColumn5
            // 
            this.gridColumn5.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn5.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumn5.ColumnEdit = this.repoBtnDeleteDictionaryValue;
            this.gridColumn5.MaxWidth = 24;
            this.gridColumn5.MinWidth = 24;
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn5.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 2;
            this.gridColumn5.Width = 24;
            // 
            // repoBtnDeleteDictionaryValue
            // 
            this.repoBtnDeleteDictionaryValue.AutoHeight = false;
            this.repoBtnDeleteDictionaryValue.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Close)});
            this.repoBtnDeleteDictionaryValue.Name = "repoBtnDeleteDictionaryValue";
            this.repoBtnDeleteDictionaryValue.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.repoBtnDeleteDictionaryValue.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnDeleteDictionaryValue_ButtonClick);
            // 
            // pnlAddValue
            // 
            this.pnlAddValue.Controls.Add(this.helpButton);
            this.pnlAddValue.Controls.Add(this.labelColumnName);
            this.pnlAddValue.Controls.Add(this.labelControl3);
            this.pnlAddValue.Controls.Add(this.txtReplaceValue);
            this.pnlAddValue.Controls.Add(this.labelControl2);
            this.pnlAddValue.Controls.Add(this.btnAddNewDictionaryData);
            this.pnlAddValue.Controls.Add(this.txtSearchValue);
            this.pnlAddValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlAddValue.Location = new System.Drawing.Point(0, 0);
            this.pnlAddValue.Name = "pnlAddValue";
            this.pnlAddValue.Size = new System.Drawing.Size(455, 100);
            this.pnlAddValue.TabIndex = 1;
            // 
            // helpButton
            // 
            this.helpButton.Image = ((System.Drawing.Image)(resources.GetObject("helpButton.Image")));
            this.helpButton.Location = new System.Drawing.Point(432, 4);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(15, 19);
            this.helpButton.TabIndex = 70;
            this.helpButton.TabStop = false;
            this.helpButton.Tag = "wordmanager";
            this.toolTip1.SetToolTip(this.helpButton, "Click to Learn more");
            this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
            // 
            // labelColumnName
            // 
            this.labelColumnName.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.labelColumnName.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelColumnName.Appearance.Options.UseBackColor = true;
            this.labelColumnName.Appearance.Options.UseFont = true;
            this.labelColumnName.Location = new System.Drawing.Point(14, 7);
            this.labelColumnName.Name = "labelColumnName";
            this.labelColumnName.Size = new System.Drawing.Size(114, 21);
            this.labelColumnName.TabIndex = 8;
            this.labelColumnName.Text = "Company Type";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(188, 39);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(83, 13);
            this.labelControl3.TabIndex = 7;
            this.labelControl3.Text = "Substitute value";
            // 
            // txtReplaceValue
            // 
            this.txtReplaceValue.Location = new System.Drawing.Point(186, 55);
            this.txtReplaceValue.Name = "txtReplaceValue";
            this.txtReplaceValue.Properties.AutoHeight = false;
            this.txtReplaceValue.Size = new System.Drawing.Size(156, 31);
            this.txtReplaceValue.TabIndex = 4;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(16, 39);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(64, 13);
            this.labelControl2.TabIndex = 5;
            this.labelControl2.Text = "Search value";
            // 
            // btnAddNewDictionaryData
            // 
            this.btnAddNewDictionaryData.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddNewDictionaryData.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnAddNewDictionaryData.ImageOptions.SvgImage")));
            this.btnAddNewDictionaryData.Location = new System.Drawing.Point(348, 55);
            this.btnAddNewDictionaryData.Name = "btnAddNewDictionaryData";
            this.btnAddNewDictionaryData.Size = new System.Drawing.Size(94, 31);
            toolTipTitleItem2.Text = global::WinPure.CleanAndMatch.Properties.Resources.UI_EDITDICTIONARYFORM_ADDNEWVALUE;
            superToolTip2.Items.Add(toolTipTitleItem2);
            this.btnAddNewDictionaryData.SuperTip = superToolTip2;
            this.btnAddNewDictionaryData.TabIndex = 5;
            this.btnAddNewDictionaryData.Text = "Add";
            this.btnAddNewDictionaryData.Click += new System.EventHandler(this.btnAddNewDictionaryData_Click);
            // 
            // txtSearchValue
            // 
            this.txtSearchValue.Location = new System.Drawing.Point(14, 55);
            this.txtSearchValue.Name = "txtSearchValue";
            this.txtSearchValue.Properties.AutoHeight = false;
            this.txtSearchValue.Size = new System.Drawing.Size(156, 31);
            this.txtSearchValue.TabIndex = 3;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSave,
            this.mnuLoad});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(837, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuSave
            // 
            this.mnuSave.Name = "mnuSave";
            this.mnuSave.Size = new System.Drawing.Size(43, 20);
            this.mnuSave.Text = "Save";
            this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            // 
            // mnuLoad
            // 
            this.mnuLoad.Name = "mnuLoad";
            this.mnuLoad.Size = new System.Drawing.Size(45, 20);
            this.mnuLoad.Tag = "Load";
            this.mnuLoad.Text = global::WinPure.CleanAndMatch.Properties.Resources.UI_LOAD;
            this.mnuLoad.Click += new System.EventHandler(this.mnuLoad_Click);
            // 
            // frmEditDictionary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(837, 640);
            this.Controls.Add(this.scDictionary);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.menuStrip1);
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("frmEditDictionary.IconOptions.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEditDictionary";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Knowledge Base Library Manager";
            this.Load += new System.EventHandler(this.frmEditDictionary_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scDictionary.Panel1)).EndInit();
            this.scDictionary.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scDictionary.Panel2)).EndInit();
            this.scDictionary.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scDictionary)).EndInit();
            this.scDictionary.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridDictionaryNames)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDictionaryNames)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlAddDictionary)).EndInit();
            this.pnlAddDictionary.ResumeLayout(false);
            this.pnlAddDictionary.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDictionaryName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDictionaryData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDictionaryData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnDeleteDictionaryValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlAddValue)).EndInit();
            this.pnlAddValue.ResumeLayout(false);
            this.pnlAddValue.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.helpButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReplaceValue.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchValue.Properties)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SplitContainerControl scDictionary;
        private DevExpress.XtraGrid.GridControl gridDictionaryData;
        private DevExpress.XtraGrid.Views.Grid.GridView gvDictionaryData;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoBtnDeleteDictionaryValue;
        private DevExpress.XtraEditors.PanelControl pnlAddDictionary;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnAddNewDictionary;
        private DevExpress.XtraEditors.TextEdit txtDictionaryName;
        private DevExpress.XtraEditors.PanelControl pnlAddValue;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtReplaceValue;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SimpleButton btnAddNewDictionaryData;
        private DevExpress.XtraEditors.TextEdit txtSearchValue;
        private DevExpress.XtraGrid.GridControl gridDictionaryNames;
        private DevExpress.XtraGrid.Views.Grid.GridView gvDictionaryNames;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEdit1;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.LabelControl labelColumnName;
        private System.Windows.Forms.PictureBox helpButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuLoad;
        private ToolStripMenuItem mnuSave;
    }
}