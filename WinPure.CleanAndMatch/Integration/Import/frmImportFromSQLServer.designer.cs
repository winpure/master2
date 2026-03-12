namespace WinPure.CleanAndMatch.Integration.Import
{
    partial class frmImportFromSQLServer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImportFromSQLServer));
            this.groupBox1 = new DevExpress.XtraEditors.GroupControl();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            this.btnRefreshServers = new DevExpress.XtraEditors.SimpleButton();
            this.sePort = new DevExpress.XtraEditors.SpinEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.txtServerName = new DevExpress.XtraEditors.TextEdit();
            this.cbeDatabase = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.groupBox2 = new DevExpress.XtraEditors.GroupControl();
            this.rgAuthType = new DevExpress.XtraEditors.RadioGroup();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();
            this.txtLogin = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.groupBox3 = new DevExpress.XtraEditors.GroupControl();
            this.gridTableList = new DevExpress.XtraGrid.GridControl();
            this.gvTableList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colTableName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.dGridSample = new DevExpress.XtraGrid.GridControl();
            this.gvSample = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lbError = new DevExpress.XtraEditors.LabelControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.tcConfiguration = new DevExpress.XtraTab.XtraTabControl();
            this.tpConnection = new DevExpress.XtraTab.XtraTabPage();
            this.tpSql = new DevExpress.XtraTab.XtraTabPage();
            this.edtSql = new DevExpress.XtraRichEdit.RichEditControl();
            this.panelControl5 = new DevExpress.XtraEditors.PanelControl();
            this.btnSaveSql = new DevExpress.XtraEditors.SimpleButton();
            this.btnLoadSql = new DevExpress.XtraEditors.SimpleButton();
            this.btnSqlPreview = new DevExpress.XtraEditors.SimpleButton();
            this.btnSqlValidate = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl6 = new DevExpress.XtraEditors.PanelControl();
            this.lbSelectTableOrSql = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sePort.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServerName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbeDatabase.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgAuthType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox3)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridTableList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvTableList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dGridSample)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSample)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tcConfiguration)).BeginInit();
            this.tcConfiguration.SuspendLayout();
            this.tpConnection.SuspendLayout();
            this.tpSql.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl5)).BeginInit();
            this.panelControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl6)).BeginInit();
            this.panelControl6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.groupBox1.AppearanceCaption.Options.UseFont = true;
            this.groupBox1.Controls.Add(this.xtraTabControl1);
            this.groupBox1.Controls.Add(this.btnRefreshServers);
            this.groupBox1.Controls.Add(this.sePort);
            this.groupBox1.Controls.Add(this.labelControl5);
            this.groupBox1.Controls.Add(this.txtServerName);
            this.groupBox1.Controls.Add(this.cbeDatabase);
            this.groupBox1.Controls.Add(this.labelControl4);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(26);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(328, 335);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.Text = "Connection Information";
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.xtraTabControl1.BorderStylePage = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.xtraTabControl1.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Bottom;
            this.xtraTabControl1.Location = new System.Drawing.Point(459, 164);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.PaintStyleName = "Skin";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
            this.xtraTabControl1.Size = new System.Drawing.Size(151, 104);
            this.xtraTabControl1.TabIndex = 112;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage1,
            this.xtraTabPage2});
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(149, 80);
            this.xtraTabPage1.Text = "xtraTabPage1";
            // 
            // xtraTabPage2
            // 
            this.xtraTabPage2.Name = "xtraTabPage2";
            this.xtraTabPage2.Size = new System.Drawing.Size(149, 80);
            this.xtraTabPage2.Text = "xtraTabPage2";
            // 
            // btnRefreshServers
            // 
            this.btnRefreshServers.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnRefreshServers.Appearance.Options.UseFont = true;
            this.btnRefreshServers.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnRefreshServers.ImageOptions.SvgImage")));
            this.btnRefreshServers.Location = new System.Drawing.Point(234, 43);
            this.btnRefreshServers.Name = "btnRefreshServers";
            this.btnRefreshServers.Size = new System.Drawing.Size(82, 26);
            this.btnRefreshServers.TabIndex = 27;
            this.btnRefreshServers.Text = "Connect";
            this.btnRefreshServers.Click += new System.EventHandler(this.btnRefreshServers_Click);
            // 
            // sePort
            // 
            this.sePort.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.sePort.Location = new System.Drawing.Point(59, 78);
            this.sePort.Name = "sePort";
            this.sePort.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.sePort.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.sePort.Properties.IsFloatValue = false;
            this.sePort.Properties.Mask.EditMask = "N00";
            this.sePort.Properties.NullText = "<no port>";
            this.sePort.Size = new System.Drawing.Size(147, 22);
            this.sePort.TabIndex = 2;
            // 
            // labelControl5
            // 
            this.labelControl5.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl5.Appearance.Options.UseFont = true;
            this.labelControl5.Location = new System.Drawing.Point(31, 82);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(22, 15);
            this.labelControl5.TabIndex = 26;
            this.labelControl5.Text = "Port";
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(20, 43);
            this.txtServerName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtServerName.Properties.Appearance.Options.UseFont = true;
            this.txtServerName.Size = new System.Drawing.Size(209, 24);
            this.txtServerName.TabIndex = 0;
            // 
            // cbeDatabase
            // 
            this.cbeDatabase.Location = new System.Drawing.Point(19, 300);
            this.cbeDatabase.Name = "cbeDatabase";
            this.cbeDatabase.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbeDatabase.Size = new System.Drawing.Size(240, 22);
            this.cbeDatabase.TabIndex = 4;
            this.cbeDatabase.EditValueChanged += new System.EventHandler(this.cbeDatabase_EditValueChanged);
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl4.Appearance.Options.UseFont = true;
            this.labelControl4.Location = new System.Drawing.Point(19, 281);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(48, 15);
            this.labelControl4.TabIndex = 20;
            this.labelControl4.Text = "Database";
            // 
            // groupBox2
            // 
            this.groupBox2.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.AppearanceCaption.Options.UseFont = true;
            this.groupBox2.Controls.Add(this.rgAuthType);
            this.groupBox2.Controls.Add(this.txtPassword);
            this.groupBox2.Controls.Add(this.txtLogin);
            this.groupBox2.Controls.Add(this.labelControl3);
            this.groupBox2.Controls.Add(this.labelControl2);
            this.groupBox2.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupBox2.Location = new System.Drawing.Point(20, 109);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(282, 159);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.Text = "Log on to the server";
            // 
            // rgAuthType
            // 
            this.rgAuthType.Location = new System.Drawing.Point(15, 25);
            this.rgAuthType.Name = "rgAuthType";
            this.rgAuthType.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.rgAuthType.Properties.Appearance.Options.UseBackColor = true;
            this.rgAuthType.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.rgAuthType.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Use Windows Authentication", true, null, "rgWinAuth"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Use SQL server Authentication", true, null, "rgServerAuth")});
            this.rgAuthType.Size = new System.Drawing.Size(217, 56);
            this.rgAuthType.TabIndex = 19;
            this.rgAuthType.SelectedIndexChanged += new System.EventHandler(this.rgAuthType_SelectedIndexChanged);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(78, 119);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(159, 22);
            this.txtPassword.TabIndex = 3;
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(78, 88);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(159, 22);
            this.txtLogin.TabIndex = 2;
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(15, 122);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(50, 15);
            this.labelControl3.TabIndex = 17;
            this.labelControl3.Text = "Password";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(15, 88);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(30, 15);
            this.labelControl2.TabIndex = 16;
            this.labelControl2.Text = "Login";
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(20, 23);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(133, 15);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Server Name / IP Address";
            // 
            // groupBox3
            // 
            this.groupBox3.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.groupBox3.AppearanceCaption.Options.UseFont = true;
            this.groupBox3.Controls.Add(this.gridTableList);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupBox3.Location = new System.Drawing.Point(6, 6);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(26);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(5);
            this.groupBox3.Size = new System.Drawing.Size(336, 335);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.Text = "Tables and Views";
            // 
            // gridTableList
            // 
            this.gridTableList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridTableList.Location = new System.Drawing.Point(7, 22);
            this.gridTableList.MainView = this.gvTableList;
            this.gridTableList.Name = "gridTableList";
            this.gridTableList.Size = new System.Drawing.Size(322, 306);
            this.gridTableList.TabIndex = 0;
            this.gridTableList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvTableList});
            // 
            // gvTableList
            // 
            this.gvTableList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colTableName});
            this.gvTableList.DetailHeight = 303;
            this.gvTableList.GridControl = this.gridTableList;
            this.gvTableList.Name = "gvTableList";
            this.gvTableList.OptionsBehavior.Editable = false;
            this.gvTableList.OptionsBehavior.FocusLeaveOnTab = true;
            this.gvTableList.OptionsBehavior.ReadOnly = true;
            this.gvTableList.OptionsEditForm.PopupEditFormWidth = 686;
            this.gvTableList.OptionsView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.Hidden;
            this.gvTableList.OptionsView.ShowAutoFilterRow = true;
            this.gvTableList.OptionsView.ShowGroupPanel = false;
            this.gvTableList.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvTableList_FocusedRowChanged);
            // 
            // colTableName
            // 
            this.colTableName.Caption = "Table Name";
            this.colTableName.FieldName = "TableName";
            this.colTableName.MinWidth = 17;
            this.colTableName.Name = "colTableName";
            this.colTableName.Visible = true;
            this.colTableName.VisibleIndex = 0;
            this.colTableName.Width = 64;
            // 
            // dGridSample
            // 
            this.dGridSample.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dGridSample.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.dGridSample.Location = new System.Drawing.Point(2, 31);
            this.dGridSample.MainView = this.gvSample;
            this.dGridSample.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.dGridSample.Name = "dGridSample";
            this.dGridSample.Size = new System.Drawing.Size(754, 300);
            this.dGridSample.TabIndex = 0;
            this.dGridSample.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvSample});
            // 
            // gvSample
            // 
            this.gvSample.Appearance.HeaderPanel.Font = new System.Drawing.Font("Open Sans Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gvSample.Appearance.HeaderPanel.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
            this.gvSample.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvSample.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvSample.Appearance.Row.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.gvSample.Appearance.Row.Options.UseFont = true;
            this.gvSample.DetailHeight = 404;
            this.gvSample.GridControl = this.dGridSample;
            this.gvSample.Name = "gvSample";
            this.gvSample.OptionsBehavior.FocusLeaveOnTab = true;
            this.gvSample.OptionsBehavior.ReadOnly = true;
            this.gvSample.OptionsEditForm.PopupEditFormWidth = 933;
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
            this.lbError.Margin = new System.Windows.Forms.Padding(6);
            this.lbError.Name = "lbError";
            this.lbError.Padding = new System.Windows.Forms.Padding(5);
            this.lbError.Size = new System.Drawing.Size(95, 29);
            this.lbError.TabIndex = 78;
            this.lbError.Text = "labelControl1";
            this.lbError.Visible = false;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.groupBox1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(4);
            this.panelControl1.Size = new System.Drawing.Size(340, 347);
            this.panelControl1.TabIndex = 0;
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.groupBox3);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl2.Location = new System.Drawing.Point(340, 0);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Padding = new System.Windows.Forms.Padding(4);
            this.panelControl2.Size = new System.Drawing.Size(348, 347);
            this.panelControl2.TabIndex = 1;
            // 
            // tcConfiguration
            // 
            this.tcConfiguration.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.tcConfiguration.Appearance.Options.UseFont = true;
            this.tcConfiguration.AppearancePage.Header.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcConfiguration.AppearancePage.Header.Options.UseFont = true;
            this.tcConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcConfiguration.HeaderButtons = DevExpress.XtraTab.TabButtons.None;
            this.tcConfiguration.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Bottom;
            this.tcConfiguration.Location = new System.Drawing.Point(0, 65);
            this.tcConfiguration.MinimumSize = new System.Drawing.Size(0, 373);
            this.tcConfiguration.Name = "tcConfiguration";
            this.tcConfiguration.SelectedTabPage = this.tpConnection;
            this.tcConfiguration.Size = new System.Drawing.Size(690, 373);
            this.tcConfiguration.TabIndex = 109;
            this.tcConfiguration.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tpConnection,
            this.tpSql});
            this.tcConfiguration.TabPageWidth = 170;
            this.tcConfiguration.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tcConfiguration_SelectedPageChanged);
            // 
            // tpConnection
            // 
            this.tpConnection.Controls.Add(this.panelControl2);
            this.tpConnection.Controls.Add(this.panelControl1);
            this.tpConnection.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("tpConnection.ImageOptions.Image")));
            this.tpConnection.Name = "tpConnection";
            this.tpConnection.Size = new System.Drawing.Size(688, 347);
            this.tpConnection.Text = "Connection Information";
            // 
            // tpSql
            // 
            this.tpSql.Controls.Add(this.edtSql);
            this.tpSql.Controls.Add(this.panelControl5);
            this.tpSql.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("tpSql.ImageOptions.Image")));
            this.tpSql.Name = "tpSql";
            this.tpSql.PageEnabled = false;
            this.tpSql.Size = new System.Drawing.Size(688, 347);
            this.tpSql.Text = "SQL";
            // 
            // edtSql
            // 
            this.edtSql.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Draft;
            this.edtSql.Dock = System.Windows.Forms.DockStyle.Fill;
            this.edtSql.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
            this.edtSql.Location = new System.Drawing.Point(0, 35);
            this.edtSql.Name = "edtSql";
            this.edtSql.Options.DocumentCapabilities.Bookmarks = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
            this.edtSql.Options.DocumentSaveOptions.CurrentFormat = DevExpress.XtraRichEdit.DocumentFormat.PlainText;
            this.edtSql.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
            this.edtSql.Options.Search.RegExResultMaxGuaranteedLength = 500;
            this.edtSql.Options.VerticalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
            this.edtSql.Size = new System.Drawing.Size(688, 312);
            this.edtSql.TabIndex = 2;
            this.edtSql.Views.DraftView.AllowDisplayLineNumbers = true;
            this.edtSql.Views.DraftView.Padding = new DevExpress.Portable.PortablePadding(50, 0, 0, 0);
            // 
            // panelControl5
            // 
            this.panelControl5.Controls.Add(this.btnSaveSql);
            this.panelControl5.Controls.Add(this.btnLoadSql);
            this.panelControl5.Controls.Add(this.btnSqlPreview);
            this.panelControl5.Controls.Add(this.btnSqlValidate);
            this.panelControl5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl5.Location = new System.Drawing.Point(0, 0);
            this.panelControl5.Name = "panelControl5";
            this.panelControl5.Size = new System.Drawing.Size(688, 35);
            this.panelControl5.TabIndex = 0;
            // 
            // btnSaveSql
            // 
            this.btnSaveSql.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnSaveSql.Appearance.Options.UseFont = true;
            this.btnSaveSql.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveSql.ImageOptions.Image")));
            this.btnSaveSql.Location = new System.Drawing.Point(86, 4);
            this.btnSaveSql.Name = "btnSaveSql";
            this.btnSaveSql.Size = new System.Drawing.Size(63, 26);
            this.btnSaveSql.TabIndex = 3;
            this.btnSaveSql.Text = "Save";
            this.btnSaveSql.ToolTip = "Save SQL Script";
            this.btnSaveSql.Click += new System.EventHandler(this.btnSaveSql_Click);
            // 
            // btnLoadSql
            // 
            this.btnLoadSql.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnLoadSql.Appearance.Options.UseFont = true;
            this.btnLoadSql.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnLoadSql.ImageOptions.Image")));
            this.btnLoadSql.Location = new System.Drawing.Point(17, 4);
            this.btnLoadSql.Name = "btnLoadSql";
            this.btnLoadSql.Size = new System.Drawing.Size(63, 26);
            this.btnLoadSql.TabIndex = 2;
            this.btnLoadSql.Text = "Load";
            this.btnLoadSql.ToolTip = "Load SQL Script";
            this.btnLoadSql.Click += new System.EventHandler(this.btnLoadSql_Click);
            // 
            // btnSqlPreview
            // 
            this.btnSqlPreview.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnSqlPreview.Appearance.Options.UseFont = true;
            this.btnSqlPreview.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSqlPreview.ImageOptions.Image")));
            this.btnSqlPreview.Location = new System.Drawing.Point(154, 4);
            this.btnSqlPreview.Name = "btnSqlPreview";
            this.btnSqlPreview.Size = new System.Drawing.Size(79, 26);
            this.btnSqlPreview.TabIndex = 1;
            this.btnSqlPreview.Text = "Run SQL";
            this.btnSqlPreview.ToolTip = "Execute SQL Script";
            this.btnSqlPreview.Click += new System.EventHandler(this.btnSqlPreview_Click);
            // 
            // btnSqlValidate
            // 
            this.btnSqlValidate.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnSqlValidate.Appearance.Options.UseFont = true;
            this.btnSqlValidate.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSqlValidate.ImageOptions.Image")));
            this.btnSqlValidate.Location = new System.Drawing.Point(510, 3);
            this.btnSqlValidate.Name = "btnSqlValidate";
            this.btnSqlValidate.Size = new System.Drawing.Size(103, 26);
            this.btnSqlValidate.TabIndex = 0;
            this.btnSqlValidate.Text = "Validate";
            this.btnSqlValidate.Visible = false;
            this.btnSqlValidate.Click += new System.EventHandler(this.btnSqlValidate_Click);
            // 
            // panelControl6
            // 
            this.panelControl6.Controls.Add(this.lbSelectTableOrSql);
            this.panelControl6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl6.Location = new System.Drawing.Point(0, 24);
            this.panelControl6.Name = "panelControl6";
            this.panelControl6.Size = new System.Drawing.Size(690, 41);
            this.panelControl6.TabIndex = 110;
            // 
            // lbSelectTableOrSql
            // 
            this.lbSelectTableOrSql.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSelectTableOrSql.Appearance.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lbSelectTableOrSql.Appearance.Options.UseFont = true;
            this.lbSelectTableOrSql.Appearance.Options.UseForeColor = true;
            this.lbSelectTableOrSql.Location = new System.Drawing.Point(55, 13);
            this.lbSelectTableOrSql.Name = "lbSelectTableOrSql";
            this.lbSelectTableOrSql.Size = new System.Drawing.Size(547, 17);
            this.lbSelectTableOrSql.TabIndex = 0;
            this.lbSelectTableOrSql.Text = "Select the tables you want to import. You may also enter a custom SQL query if pr" +
    "eferred.";
            // 
            // frmImportFromSQLServer
            // 
            this.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 744);
            this.Controls.Add(this.tcConfiguration);
            this.Controls.Add(this.panelControl6);
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("frmImportFromSQLServer.IconOptions.Icon")));
            this.IconOptions.LargeImage = global::WinPure.CleanAndMatch.Properties.Resources.database_32x32;
            this.IconOptions.ShowIcon = false;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(692, 774);
            this.Name = "frmImportFromSQLServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import table or view from MS SQL Server";
            this.Controls.SetChildIndex(this.panelControl6, 0);
            this.Controls.SetChildIndex(this.tcConfiguration, 0);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sePort.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServerName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbeDatabase.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgAuthType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox3)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridTableList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvTableList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dGridSample)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSample)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tcConfiguration)).EndInit();
            this.tcConfiguration.ResumeLayout(false);
            this.tpConnection.ResumeLayout(false);
            this.tpSql.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl5)).EndInit();
            this.panelControl5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl6)).EndInit();
            this.panelControl6.ResumeLayout(false);
            this.panelControl6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupBox1;
        private DevExpress.XtraEditors.TextEdit txtServerName;
        private DevExpress.XtraEditors.ComboBoxEdit cbeDatabase;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.GroupControl groupBox2;
        private DevExpress.XtraEditors.TextEdit txtPassword;
        private DevExpress.XtraEditors.TextEdit txtLogin;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.GroupControl groupBox3;
        private DevExpress.XtraGrid.Views.Grid.GridView gvSample;
        private DevExpress.XtraEditors.SpinEdit sePort;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private GridControl gridTableList;
        private GridView gvTableList;
        private GridColumn colTableName;
        private PanelControl panelControl1;
        private PanelControl panelControl2;
        private DevExpress.XtraTab.XtraTabPage tpSql;
        private DevExpress.XtraTab.XtraTabPage tpConnection;
        private PanelControl panelControl5;
        private SimpleButton btnSqlPreview;
        private SimpleButton btnSqlValidate;
        private DevExpress.XtraRichEdit.RichEditControl edtSql;
        private SimpleButton btnSaveSql;
        private SimpleButton btnLoadSql;
        private PanelControl panelControl6;
        private LabelControl lbSelectTableOrSql;
        private GridControl dGridSample;
        private LabelControl lbError;
        private RadioGroup rgAuthType;
        private SimpleButton btnRefreshServers;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
        public DevExpress.XtraTab.XtraTabControl tcConfiguration;
    }
}