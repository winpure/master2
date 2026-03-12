
namespace WinPure.CleanAndMatch.MatchResultProcessing
{
    partial class frmSetMasterRecords
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSetMasterRecords));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
            this.bgRules = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroupControlContainer1 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.cbMrInAnyCase = new DevExpress.XtraEditors.CheckEdit();
            this.gridRules = new DevExpress.XtraGrid.GridControl();
            this.gvRules = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cbFieldName = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.cbAnyRule = new DevExpress.XtraEditors.CheckEdit();
            this.cbAllRules = new DevExpress.XtraEditors.CheckEdit();
            this.navBarGroupControlContainer2 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.cbOnlyThisTable = new DevExpress.XtraEditors.CheckEdit();
            this.cbMasterRecordMainTable = new DevExpress.XtraEditors.ComboBoxEdit();
            this.rbClearAll = new System.Windows.Forms.RadioButton();
            this.rbMostRelevant = new System.Windows.Forms.RadioButton();
            this.rbFromTable = new System.Windows.Forms.RadioButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.bgOption = new DevExpress.XtraNavBar.NavBarGroup();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).BeginInit();
            this.navBarControl1.SuspendLayout();
            this.navBarGroupControlContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbMrInAnyCase.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridRules)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvRules)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbFieldName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbAnyRule.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbAllRules.Properties)).BeginInit();
            this.navBarGroupControlContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbOnlyThisTable.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbMasterRecordMainTable.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 25);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(15, 19);
            this.pictureBox1.TabIndex = 82;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Tag = "";
            this.toolTip1.SetToolTip(this.pictureBox1, "Click to learn more");
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnCancel);
            this.panelControl1.Controls.Add(this.btnOK);
            this.panelControl1.Controls.Add(this.pictureBox1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 380);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(584, 64);
            this.panelControl1.TabIndex = 82;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageOptions.SvgImage = global::WinPure.CleanAndMatch.Properties.Resources._2026_Close;
            this.btnCancel.Location = new System.Drawing.Point(462, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(106, 41);
            this.btnCancel.TabIndex = 84;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Appearance.Options.UseFont = true;
            this.btnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOK.ImageOptions.SvgImage = global::WinPure.CleanAndMatch.Properties.Resources._2026_Execute_24;
            this.btnOK.Location = new System.Drawing.Point(336, 11);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(108, 41);
            this.btnOK.TabIndex = 83;
            this.btnOK.Text = "Execute";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // navBarControl1
            // 
            this.navBarControl1.ActiveGroup = this.bgRules;
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer2);
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer1);
            this.navBarControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navBarControl1.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.bgRules,
            this.bgOption});
            this.navBarControl1.Location = new System.Drawing.Point(0, 0);
            this.navBarControl1.Name = "navBarControl1";
            this.navBarControl1.OptionsNavPane.ExpandedWidth = 584;
            this.navBarControl1.Size = new System.Drawing.Size(584, 444);
            this.navBarControl1.TabIndex = 83;
            this.navBarControl1.Text = "navBarControl1";
            this.navBarControl1.GroupExpanded += new DevExpress.XtraNavBar.NavBarGroupEventHandler(this.navBarControl1_GroupExpanded);
            this.navBarControl1.GroupCollapsed += new DevExpress.XtraNavBar.NavBarGroupEventHandler(this.navBarControl1_GroupCollapsed);
            this.navBarControl1.DoubleClick += new System.EventHandler(this.navBarControl1_DoubleClick);
            // 
            // bgRules
            // 
            this.bgRules.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bgRules.Appearance.Options.UseFont = true;
            this.bgRules.Caption = "Add rules";
            this.bgRules.ControlContainer = this.navBarGroupControlContainer1;
            this.bgRules.Expanded = true;
            this.bgRules.GroupClientHeight = 189;
            this.bgRules.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.bgRules.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("bgRules.ImageOptions.SvgImage")));
            this.bgRules.Name = "bgRules";
            // 
            // navBarGroupControlContainer1
            // 
            this.navBarGroupControlContainer1.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer1.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer1.Controls.Add(this.cbMrInAnyCase);
            this.navBarGroupControlContainer1.Controls.Add(this.gridRules);
            this.navBarGroupControlContainer1.Controls.Add(this.panelControl2);
            this.navBarGroupControlContainer1.Name = "navBarGroupControlContainer1";
            this.navBarGroupControlContainer1.Size = new System.Drawing.Size(576, 189);
            this.navBarGroupControlContainer1.TabIndex = 0;
            // 
            // cbMrInAnyCase
            // 
            this.cbMrInAnyCase.EditValue = true;
            this.cbMrInAnyCase.Location = new System.Drawing.Point(27, 163);
            this.cbMrInAnyCase.Name = "cbMrInAnyCase";
            this.cbMrInAnyCase.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.cbMrInAnyCase.Properties.Appearance.Options.UseFont = true;
            this.cbMrInAnyCase.Properties.Caption = "Define master record in any case";
            this.cbMrInAnyCase.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbMrInAnyCase.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbMrInAnyCase.Size = new System.Drawing.Size(537, 21);
            this.cbMrInAnyCase.TabIndex = 2;
            // 
            // gridRules
            // 
            this.gridRules.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridRules.Location = new System.Drawing.Point(0, 37);
            this.gridRules.MainView = this.gvRules;
            this.gridRules.Name = "gridRules";
            this.gridRules.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemComboBox1,
            this.repositoryItemCheckEdit1,
            this.cbFieldName,
            this.repositoryItemButtonEdit1});
            this.gridRules.Size = new System.Drawing.Size(576, 120);
            this.gridRules.TabIndex = 2;
            this.gridRules.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvRules});
            // 
            // gvRules
            // 
            this.gvRules.Appearance.HeaderPanel.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.gvRules.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvRules.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvRules.Appearance.Row.Options.UseFont = true;
            this.gvRules.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5});
            this.gvRules.GridControl = this.gridRules;
            this.gvRules.Name = "gvRules";
            this.gvRules.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
            this.gvRules.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.True;
            this.gvRules.OptionsCustomization.AllowSort = false;
            this.gvRules.OptionsMenu.ShowGroupSortSummaryItems = false;
            this.gvRules.OptionsView.EnableAppearanceEvenRow = true;
            this.gvRules.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this.gvRules.OptionsView.ShowGroupPanel = false;
            this.gvRules.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(this.gvRules_ValidateRow);
            this.gvRules.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gvRules_KeyDown);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridColumn1.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn1.AppearanceHeader.Options.UseFont = true;
            this.gridColumn1.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumn1.Caption = "Column name";
            this.gridColumn1.ColumnEdit = this.cbFieldName;
            this.gridColumn1.FieldName = "FieldName";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // cbFieldName
            // 
            this.cbFieldName.AutoHeight = false;
            this.cbFieldName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbFieldName.Name = "cbFieldName";
            this.cbFieldName.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gridColumn2.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn2.AppearanceHeader.Options.UseFont = true;
            this.gridColumn2.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumn2.Caption = "Not";
            this.gridColumn2.ColumnEdit = this.repositoryItemCheckEdit1;
            this.gridColumn2.FieldName = "Negate";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.repositoryItemCheckEdit1.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gridColumn3.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn3.AppearanceHeader.Options.UseFont = true;
            this.gridColumn3.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumn3.Caption = "Type";
            this.gridColumn3.ColumnEdit = this.repositoryItemComboBox1;
            this.gridColumn3.FieldName = "RuleTypeName";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // repositoryItemComboBox1
            // 
            this.repositoryItemComboBox1.AutoHeight = false;
            this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox1.DropDownRows = 8;
            this.repositoryItemComboBox1.Items.AddRange(new object[] {
            "Empty",
            "Equal",
            "Contains",
            "Maximum",
            "Minimum",
            "Longest",
            "Shortest",
            "Greater than",
            "Common"});
            this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
            this.repositoryItemComboBox1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // gridColumn4
            // 
            this.gridColumn4.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gridColumn4.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn4.AppearanceHeader.Options.UseFont = true;
            this.gridColumn4.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumn4.Caption = "Value";
            this.gridColumn4.FieldName = "Value";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            // 
            // gridColumn5
            // 
            this.gridColumn5.AppearanceHeader.Font = new System.Drawing.Font("Open Sans SemiBold", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn5.AppearanceHeader.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gridColumn5.AppearanceHeader.Options.UseFont = true;
            this.gridColumn5.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumn5.ColumnEdit = this.repositoryItemButtonEdit1;
            this.gridColumn5.MaxWidth = 24;
            this.gridColumn5.MinWidth = 24;
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.ShowCaption = false;
            this.gridColumn5.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            this.gridColumn5.Width = 24;
            // 
            // repositoryItemButtonEdit1
            // 
            this.repositoryItemButtonEdit1.AutoHeight = false;
            this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Close)});
            this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
            this.repositoryItemButtonEdit1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.repositoryItemButtonEdit1.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEdit1_ButtonClick);
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.cbAnyRule);
            this.panelControl2.Controls.Add(this.cbAllRules);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl2.Location = new System.Drawing.Point(0, 0);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(576, 37);
            this.panelControl2.TabIndex = 3;
            // 
            // cbAnyRule
            // 
            this.cbAnyRule.Location = new System.Drawing.Point(129, 6);
            this.cbAnyRule.Name = "cbAnyRule";
            this.cbAnyRule.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.cbAnyRule.Properties.Appearance.Options.UseFont = true;
            this.cbAnyRule.Properties.Caption = "Meet any";
            this.cbAnyRule.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.Radio;
            this.cbAnyRule.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbAnyRule.Size = new System.Drawing.Size(112, 21);
            this.cbAnyRule.TabIndex = 1;
            this.cbAnyRule.CheckedChanged += new System.EventHandler(this.cbAnyRule_CheckedChanged);
            // 
            // cbAllRules
            // 
            this.cbAllRules.EditValue = true;
            this.cbAllRules.Location = new System.Drawing.Point(27, 6);
            this.cbAllRules.Name = "cbAllRules";
            this.cbAllRules.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbAllRules.Properties.Appearance.Options.UseFont = true;
            this.cbAllRules.Properties.Caption = "Meet all";
            this.cbAllRules.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.Radio;
            this.cbAllRules.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbAllRules.Size = new System.Drawing.Size(75, 21);
            this.cbAllRules.TabIndex = 0;
            this.cbAllRules.CheckedChanged += new System.EventHandler(this.cbAnyRule_CheckedChanged);
            // 
            // navBarGroupControlContainer2
            // 
            this.navBarGroupControlContainer2.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer2.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer2.Controls.Add(this.cbOnlyThisTable);
            this.navBarGroupControlContainer2.Controls.Add(this.cbMasterRecordMainTable);
            this.navBarGroupControlContainer2.Controls.Add(this.rbClearAll);
            this.navBarGroupControlContainer2.Controls.Add(this.rbMostRelevant);
            this.navBarGroupControlContainer2.Controls.Add(this.rbFromTable);
            this.navBarGroupControlContainer2.Controls.Add(this.labelControl1);
            this.navBarGroupControlContainer2.MinimumSize = new System.Drawing.Size(0, 100);
            this.navBarGroupControlContainer2.Name = "navBarGroupControlContainer2";
            this.navBarGroupControlContainer2.Size = new System.Drawing.Size(576, 104);
            this.navBarGroupControlContainer2.TabIndex = 1;
            // 
            // cbOnlyThisTable
            // 
            this.cbOnlyThisTable.Location = new System.Drawing.Point(481, 16);
            this.cbOnlyThisTable.Margin = new System.Windows.Forms.Padding(2);
            this.cbOnlyThisTable.Name = "cbOnlyThisTable";
            this.cbOnlyThisTable.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.cbOnlyThisTable.Properties.Appearance.Options.UseFont = true;
            this.cbOnlyThisTable.Properties.Caption = "Only";
            this.cbOnlyThisTable.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbOnlyThisTable.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbOnlyThisTable.Size = new System.Drawing.Size(87, 20);
            this.cbOnlyThisTable.TabIndex = 89;
            // 
            // cbMasterRecordMainTable
            // 
            this.cbMasterRecordMainTable.Location = new System.Drawing.Point(268, 11);
            this.cbMasterRecordMainTable.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbMasterRecordMainTable.Name = "cbMasterRecordMainTable";
            this.cbMasterRecordMainTable.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbMasterRecordMainTable.Properties.Appearance.Options.UseFont = true;
            this.cbMasterRecordMainTable.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbMasterRecordMainTable.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbMasterRecordMainTable.Size = new System.Drawing.Size(148, 24);
            this.cbMasterRecordMainTable.TabIndex = 88;
            // 
            // rbClearAll
            // 
            this.rbClearAll.AutoSize = true;
            this.rbClearAll.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.rbClearAll.Location = new System.Drawing.Point(16, 61);
            this.rbClearAll.Name = "rbClearAll";
            this.rbClearAll.Size = new System.Drawing.Size(66, 19);
            this.rbClearAll.TabIndex = 84;
            this.rbClearAll.Text = "Clear all";
            this.rbClearAll.UseVisualStyleBackColor = true;
            this.rbClearAll.Click += new System.EventHandler(this.rbClearAll_Click);
            // 
            // rbMostRelevant
            // 
            this.rbMostRelevant.AutoSize = true;
            this.rbMostRelevant.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.rbMostRelevant.Location = new System.Drawing.Point(16, 39);
            this.rbMostRelevant.Name = "rbMostRelevant";
            this.rbMostRelevant.Size = new System.Drawing.Size(143, 19);
            this.rbMostRelevant.TabIndex = 85;
            this.rbMostRelevant.Text = global::WinPure.CleanAndMatch.Properties.Resources.UI_SETMASTERRECORDSFORM_SELECTMOSTRELEVANT;
            this.rbMostRelevant.UseVisualStyleBackColor = true;
            this.rbMostRelevant.Click += new System.EventHandler(this.rbMostRelevant_Click);
            // 
            // rbFromTable
            // 
            this.rbFromTable.AutoSize = true;
            this.rbFromTable.Checked = true;
            this.rbFromTable.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbFromTable.Location = new System.Drawing.Point(16, 16);
            this.rbFromTable.Name = "rbFromTable";
            this.rbFromTable.Size = new System.Drawing.Size(251, 19);
            this.rbFromTable.TabIndex = 86;
            this.rbFromTable.TabStop = true;
            this.rbFromTable.Text = global::WinPure.CleanAndMatch.Properties.Resources.UI_SETMASTERRECORDSFORM_SETMOSTPOPULATED;
            this.rbFromTable.UseVisualStyleBackColor = true;
            this.rbFromTable.Click += new System.EventHandler(this.rbFromTable_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(422, 18);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(50, 15);
            this.labelControl1.TabIndex = 87;
            this.labelControl1.Text = "as master";
            // 
            // bgOption
            // 
            this.bgOption.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.bgOption.Appearance.Options.UseFont = true;
            this.bgOption.Caption = "Additional options";
            this.bgOption.ControlContainer = this.navBarGroupControlContainer2;
            this.bgOption.Expanded = true;
            this.bgOption.GroupClientHeight = 104;
            this.bgOption.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.bgOption.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("bgOption.ImageOptions.SvgImage")));
            this.bgOption.Name = "bgOption";
            // 
            // frmSetMasterRecords
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(584, 444);
            this.ControlBox = false;
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.navBarControl1);
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("frmSetMasterRecords.IconOptions.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(586, 39);
            this.Name = "frmSetMasterRecords";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Set Master Records";
            this.Load += new System.EventHandler(this.frmSetMasterRecords_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).EndInit();
            this.navBarControl1.ResumeLayout(false);
            this.navBarGroupControlContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cbMrInAnyCase.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridRules)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvRules)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbFieldName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cbAnyRule.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbAllRules.Properties)).EndInit();
            this.navBarGroupControlContainer2.ResumeLayout(false);
            this.navBarGroupControlContainer2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbOnlyThisTable.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbMasterRecordMainTable.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private System.Windows.Forms.PictureBox pictureBox1;
        private DevExpress.XtraNavBar.NavBarControl navBarControl1;
        private DevExpress.XtraNavBar.NavBarGroup bgRules;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer1;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer2;
        private DevExpress.XtraEditors.ComboBoxEdit cbMasterRecordMainTable;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.RadioButton rbFromTable;
        private System.Windows.Forms.RadioButton rbMostRelevant;
        private System.Windows.Forms.RadioButton rbClearAll;
        private DevExpress.XtraNavBar.NavBarGroup bgOption;
        private DevExpress.XtraGrid.GridControl gridRules;
        private DevExpress.XtraGrid.Views.Grid.GridView gvRules;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.CheckEdit cbAnyRule;
        private DevExpress.XtraEditors.CheckEdit cbAllRules;
        private DevExpress.XtraEditors.CheckEdit cbMrInAnyCase;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox cbFieldName;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEdit1;
        private DevExpress.XtraEditors.CheckEdit cbOnlyThisTable;
    }
}