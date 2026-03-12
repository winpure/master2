
namespace WinPure.CleanAndMatch.Integration.Import
{
    partial class frmImportMySQL
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImportMySQL));
            DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions buttonImageOptions1 = new DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions();
            DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions buttonImageOptions2 = new DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions();
            this.groupBox1 = new DevExpress.XtraEditors.GroupControl();
            this.btnRefreshServers = new DevExpress.XtraEditors.SimpleButton();
            this.cbSsl = new DevExpress.XtraEditors.CheckEdit();
            this.sePort = new DevExpress.XtraEditors.SpinEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.txtServerName = new DevExpress.XtraEditors.TextEdit();
            this.cbeDatebase = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.groupBox2 = new DevExpress.XtraEditors.GroupControl();
            this.rgAuthType = new DevExpress.XtraEditors.RadioGroup();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();
            this.txtLogin = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.gcTable = new DevExpress.XtraEditors.GroupControl();
            this.lstTables = new DevExpress.XtraEditors.ListBoxControl();
            this.gcSshInfo = new DevExpress.XtraEditors.GroupControl();
            this.pnlSsh = new DevExpress.XtraEditors.PanelControl();
            this.cbUseSsh = new DevExpress.XtraEditors.CheckEdit();
            this.txtSshServer = new DevExpress.XtraEditors.TextEdit();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.txtSshLogin = new DevExpress.XtraEditors.TextEdit();
            this.txtSshPassword = new DevExpress.XtraEditors.TextEdit();
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
            ((System.ComponentModel.ISupportInitialize)(this.cbSsl.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sePort.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServerName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbeDatebase.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgAuthType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcTable)).BeginInit();
            this.gcTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lstTables)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcSshInfo)).BeginInit();
            this.gcSshInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSsh)).BeginInit();
            this.pnlSsh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbUseSsh.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSshServer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSshLogin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSshPassword.Properties)).BeginInit();
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
            this.groupBox1.Controls.Add(this.btnRefreshServers);
            this.groupBox1.Controls.Add(this.cbSsl);
            this.groupBox1.Controls.Add(this.sePort);
            this.groupBox1.Controls.Add(this.labelControl5);
            this.groupBox1.Controls.Add(this.txtServerName);
            this.groupBox1.Controls.Add(this.cbeDatebase);
            this.groupBox1.Controls.Add(this.labelControl4);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupBox1.Location = new System.Drawing.Point(8, 9);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(8, 8, 8, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(334, 284);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.Text = "Connection Information";
            // 
            // btnRefreshServers
            // 
            this.btnRefreshServers.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnRefreshServers.ImageOptions.SvgImage")));
            this.btnRefreshServers.Location = new System.Drawing.Point(238, 39);
            this.btnRefreshServers.Name = "btnRefreshServers";
            this.btnRefreshServers.Size = new System.Drawing.Size(85, 26);
            this.btnRefreshServers.TabIndex = 20;
            this.btnRefreshServers.Text = "Connect";
            this.btnRefreshServers.Click += new System.EventHandler(this.btnRefreshServers_Click);
            // 
            // cbSsl
            // 
            this.cbSsl.Location = new System.Drawing.Point(232, 75);
            this.cbSsl.Name = "cbSsl";
            this.cbSsl.Properties.Caption = "SSL";
            this.cbSsl.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbSsl.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbSsl.Size = new System.Drawing.Size(46, 20);
            this.cbSsl.TabIndex = 3;
            // 
            // sePort
            // 
            this.sePort.EditValue = new decimal(new int[] {
            3306,
            0,
            0,
            0});
            this.sePort.Location = new System.Drawing.Point(66, 74);
            this.sePort.Name = "sePort";
            this.sePort.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.sePort.Properties.Appearance.Options.UseFont = true;
            this.sePort.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.sePort.Properties.IsFloatValue = false;
            this.sePort.Properties.Mask.EditMask = "N00";
            this.sePort.Size = new System.Drawing.Size(147, 24);
            this.sePort.TabIndex = 2;
            // 
            // labelControl5
            // 
            this.labelControl5.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl5.Appearance.Options.UseFont = true;
            this.labelControl5.Location = new System.Drawing.Point(35, 77);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(23, 15);
            this.labelControl5.TabIndex = 24;
            this.labelControl5.Text = "Port";
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(23, 39);
            this.txtServerName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtServerName.Properties.Appearance.Options.UseFont = true;
            this.txtServerName.Size = new System.Drawing.Size(209, 26);
            this.txtServerName.TabIndex = 0;
            // 
            // cbeDatebase
            // 
            this.cbeDatebase.Location = new System.Drawing.Point(79, 249);
            this.cbeDatebase.Name = "cbeDatebase";
            this.cbeDatebase.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.cbeDatebase.Properties.Appearance.Options.UseFont = true;
            this.cbeDatebase.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbeDatebase.Size = new System.Drawing.Size(228, 24);
            this.cbeDatebase.TabIndex = 5;
            this.cbeDatebase.EditValueChanged += new System.EventHandler(this.cbeDatebase_EditValueChanged);
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl4.Appearance.Options.UseFont = true;
            this.labelControl4.Location = new System.Drawing.Point(27, 247);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(48, 15);
            this.labelControl4.TabIndex = 20;
            this.labelControl4.Text = "Database";
            // 
            // groupBox2
            // 
            this.groupBox2.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.groupBox2.AppearanceCaption.Options.UseFont = true;
            this.groupBox2.Controls.Add(this.rgAuthType);
            this.groupBox2.Controls.Add(this.txtPassword);
            this.groupBox2.Controls.Add(this.txtLogin);
            this.groupBox2.Controls.Add(this.labelControl3);
            this.groupBox2.Controls.Add(this.labelControl2);
            this.groupBox2.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupBox2.Location = new System.Drawing.Point(23, 105);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(286, 138);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.Text = "Log on to the server";
            // 
            // rgAuthType
            // 
            this.rgAuthType.Location = new System.Drawing.Point(16, 25);
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
            this.txtPassword.Location = new System.Drawing.Point(74, 109);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtPassword.Properties.Appearance.Options.UseFont = true;
            this.txtPassword.Properties.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(159, 24);
            this.txtPassword.TabIndex = 3;
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(74, 83);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtLogin.Properties.Appearance.Options.UseFont = true;
            this.txtLogin.Size = new System.Drawing.Size(159, 24);
            this.txtLogin.TabIndex = 2;
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(16, 107);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(51, 15);
            this.labelControl3.TabIndex = 17;
            this.labelControl3.Text = "Password";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(16, 81);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(29, 15);
            this.labelControl2.TabIndex = 16;
            this.labelControl2.Text = "Login";
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(27, 18);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(134, 15);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Server Name / IP Address";
            // 
            // gcTable
            // 
            this.gcTable.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.gcTable.AppearanceCaption.Options.UseFont = true;
            this.gcTable.Controls.Add(this.lstTables);
            this.gcTable.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.gcTable.Location = new System.Drawing.Point(346, 8);
            this.gcTable.Margin = new System.Windows.Forms.Padding(8, 8, 8, 11);
            this.gcTable.Name = "gcTable";
            this.gcTable.Padding = new System.Windows.Forms.Padding(5);
            this.gcTable.Size = new System.Drawing.Size(303, 336);
            this.gcTable.TabIndex = 2;
            this.gcTable.Text = "Tables";
            // 
            // lstTables
            // 
            this.lstTables.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.lstTables.Appearance.Options.UseFont = true;
            this.lstTables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTables.Location = new System.Drawing.Point(7, 22);
            this.lstTables.Name = "lstTables";
            this.lstTables.Size = new System.Drawing.Size(289, 307);
            this.lstTables.TabIndex = 0;
            this.lstTables.SelectedIndexChanged += new System.EventHandler(this.lstTables_SelectedIndexChanged);
            // 
            // gcSshInfo
            // 
            this.gcSshInfo.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gcSshInfo.AppearanceCaption.Options.UseFont = true;
            this.gcSshInfo.Controls.Add(this.pnlSsh);
            buttonImageOptions1.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("buttonImageOptions1.SvgImage")));
            buttonImageOptions1.SvgImageSize = new System.Drawing.Size(20, 20);
            buttonImageOptions2.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("buttonImageOptions2.SvgImage")));
            buttonImageOptions2.SvgImageSize = new System.Drawing.Size(20, 20);
            this.gcSshInfo.CustomHeaderButtons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraEditors.ButtonsPanelControl.GroupBoxButton("", true, buttonImageOptions1, DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton, "", -1, true, null, true, false, true, "pin", -1),
            new DevExpress.XtraEditors.ButtonsPanelControl.GroupBoxButton("", true, buttonImageOptions2, DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton, "", -1, true, null, true, false, false, "unpin", -1)});
            this.gcSshInfo.CustomHeaderButtonsLocation = DevExpress.Utils.GroupElementLocation.AfterText;
            this.gcSshInfo.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.gcSshInfo.Location = new System.Drawing.Point(8, 297);
            this.gcSshInfo.Margin = new System.Windows.Forms.Padding(8, 8, 8, 11);
            this.gcSshInfo.Name = "gcSshInfo";
            this.gcSshInfo.Size = new System.Drawing.Size(334, 166);
            this.gcSshInfo.TabIndex = 1;
            this.gcSshInfo.Text = "SSH Information";
            this.gcSshInfo.CustomButtonClick += new DevExpress.XtraBars.Docking2010.BaseButtonEventHandler(this.gcSshInfo_CustomButtonClick);
            // 
            // pnlSsh
            // 
            this.pnlSsh.Controls.Add(this.cbUseSsh);
            this.pnlSsh.Controls.Add(this.txtSshServer);
            this.pnlSsh.Controls.Add(this.labelControl7);
            this.pnlSsh.Controls.Add(this.labelControl8);
            this.pnlSsh.Controls.Add(this.labelControl6);
            this.pnlSsh.Controls.Add(this.txtSshLogin);
            this.pnlSsh.Controls.Add(this.txtSshPassword);
            this.pnlSsh.Location = new System.Drawing.Point(5, 28);
            this.pnlSsh.Name = "pnlSsh";
            this.pnlSsh.Size = new System.Drawing.Size(323, 126);
            this.pnlSsh.TabIndex = 26;
            this.pnlSsh.Visible = false;
            // 
            // cbUseSsh
            // 
            this.cbUseSsh.Location = new System.Drawing.Point(20, 6);
            this.cbUseSsh.Name = "cbUseSsh";
            this.cbUseSsh.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.cbUseSsh.Properties.Appearance.Options.UseFont = true;
            this.cbUseSsh.Properties.Caption = "Use SSH";
            this.cbUseSsh.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbUseSsh.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbUseSsh.Size = new System.Drawing.Size(75, 20);
            this.cbUseSsh.TabIndex = 0;
            this.cbUseSsh.CheckedChanged += new System.EventHandler(this.cbUseSsh_CheckedChanged);
            // 
            // txtSshServer
            // 
            this.txtSshServer.Enabled = false;
            this.txtSshServer.Location = new System.Drawing.Point(92, 34);
            this.txtSshServer.Name = "txtSshServer";
            this.txtSshServer.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtSshServer.Properties.Appearance.Options.UseFont = true;
            this.txtSshServer.Size = new System.Drawing.Size(159, 24);
            this.txtSshServer.TabIndex = 1;
            // 
            // labelControl7
            // 
            this.labelControl7.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl7.Appearance.Options.UseFont = true;
            this.labelControl7.Location = new System.Drawing.Point(26, 67);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(29, 13);
            this.labelControl7.TabIndex = 20;
            this.labelControl7.Text = "Login";
            // 
            // labelControl8
            // 
            this.labelControl8.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl8.Appearance.Options.UseFont = true;
            this.labelControl8.Location = new System.Drawing.Point(26, 38);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(31, 13);
            this.labelControl8.TabIndex = 25;
            this.labelControl8.Text = "Server";
            // 
            // labelControl6
            // 
            this.labelControl6.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl6.Appearance.Options.UseFont = true;
            this.labelControl6.Location = new System.Drawing.Point(26, 94);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(49, 13);
            this.labelControl6.TabIndex = 21;
            this.labelControl6.Text = "Password";
            // 
            // txtSshLogin
            // 
            this.txtSshLogin.Enabled = false;
            this.txtSshLogin.Location = new System.Drawing.Point(92, 62);
            this.txtSshLogin.Name = "txtSshLogin";
            this.txtSshLogin.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtSshLogin.Properties.Appearance.Options.UseFont = true;
            this.txtSshLogin.Size = new System.Drawing.Size(159, 24);
            this.txtSshLogin.TabIndex = 2;
            // 
            // txtSshPassword
            // 
            this.txtSshPassword.Enabled = false;
            this.txtSshPassword.Location = new System.Drawing.Point(92, 90);
            this.txtSshPassword.Name = "txtSshPassword";
            this.txtSshPassword.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtSshPassword.Properties.Appearance.Options.UseFont = true;
            this.txtSshPassword.Properties.PasswordChar = '*';
            this.txtSshPassword.Size = new System.Drawing.Size(159, 24);
            this.txtSshPassword.TabIndex = 3;
            // 
            // tcConfiguration
            // 
            this.tcConfiguration.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.tcConfiguration.Appearance.Options.UseBackColor = true;
            this.tcConfiguration.AppearancePage.Header.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tcConfiguration.AppearancePage.Header.Options.UseFont = true;
            this.tcConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcConfiguration.HeaderButtons = DevExpress.XtraTab.TabButtons.None;
            this.tcConfiguration.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Bottom;
            this.tcConfiguration.Location = new System.Drawing.Point(0, 65);
            this.tcConfiguration.Name = "tcConfiguration";
            this.tcConfiguration.SelectedTabPage = this.tpConnection;
            this.tcConfiguration.Size = new System.Drawing.Size(672, 496);
            this.tcConfiguration.TabIndex = 110;
            this.tcConfiguration.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tpConnection,
            this.tpSql});
            this.tcConfiguration.TabPageWidth = 170;
            this.tcConfiguration.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tcConfiguration_SelectedPageChanged);
            // 
            // tpConnection
            // 
            this.tpConnection.Controls.Add(this.groupBox1);
            this.tpConnection.Controls.Add(this.gcSshInfo);
            this.tpConnection.Controls.Add(this.gcTable);
            this.tpConnection.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("tpConnection.ImageOptions.Image")));
            this.tpConnection.Name = "tpConnection";
            this.tpConnection.Size = new System.Drawing.Size(670, 470);
            this.tpConnection.Text = "Connection Information";
            // 
            // tpSql
            // 
            this.tpSql.Controls.Add(this.edtSql);
            this.tpSql.Controls.Add(this.panelControl5);
            this.tpSql.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("tpSql.ImageOptions.Image")));
            this.tpSql.Name = "tpSql";
            this.tpSql.PageEnabled = false;
            this.tpSql.Size = new System.Drawing.Size(670, 470);
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
            this.edtSql.Size = new System.Drawing.Size(670, 435);
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
            this.panelControl5.Size = new System.Drawing.Size(670, 35);
            this.panelControl5.TabIndex = 3;
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
            this.btnSqlPreview.Size = new System.Drawing.Size(63, 26);
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
            this.btnSqlValidate.Location = new System.Drawing.Point(538, 3);
            this.btnSqlValidate.Name = "btnSqlValidate";
            this.btnSqlValidate.Size = new System.Drawing.Size(73, 26);
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
            this.panelControl6.Size = new System.Drawing.Size(672, 41);
            this.panelControl6.TabIndex = 111;
            // 
            // lbSelectTableOrSql
            // 
            this.lbSelectTableOrSql.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSelectTableOrSql.Appearance.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lbSelectTableOrSql.Appearance.Options.UseFont = true;
            this.lbSelectTableOrSql.Appearance.Options.UseForeColor = true;
            this.lbSelectTableOrSql.Location = new System.Drawing.Point(46, 10);
            this.lbSelectTableOrSql.Name = "lbSelectTableOrSql";
            this.lbSelectTableOrSql.Size = new System.Drawing.Size(547, 17);
            this.lbSelectTableOrSql.TabIndex = 0;
            this.lbSelectTableOrSql.Text = "Select the tables you want to import. You may also enter a custom SQL query if pr" +
    "eferred.";
            // 
            // frmImportMySQL
            // 
            this.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 873);
            this.Controls.Add(this.tcConfiguration);
            this.Controls.Add(this.panelControl6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("frmImportMySQL.IconOptions.Icon")));
            this.IconOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("frmImportMySQL.IconOptions.LargeImage")));
            this.IconOptions.ShowIcon = false;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImportMySQL";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import table from MS SQL Server";
            this.Controls.SetChildIndex(this.panelControl6, 0);
            this.Controls.SetChildIndex(this.tcConfiguration, 0);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbSsl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sePort.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServerName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbeDatebase.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgAuthType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcTable)).EndInit();
            this.gcTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lstTables)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcSshInfo)).EndInit();
            this.gcSshInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlSsh)).EndInit();
            this.pnlSsh.ResumeLayout(false);
            this.pnlSsh.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbUseSsh.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSshServer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSshLogin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSshPassword.Properties)).EndInit();
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
        private DevExpress.XtraEditors.ComboBoxEdit cbeDatebase;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.GroupControl groupBox2;
        private DevExpress.XtraEditors.TextEdit txtPassword;
        private DevExpress.XtraEditors.TextEdit txtLogin;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.GroupControl gcTable;
        private DevExpress.XtraEditors.ListBoxControl lstTables;
        private DevExpress.XtraEditors.GroupControl gcSshInfo;
        private DevExpress.XtraEditors.CheckEdit cbUseSsh;
        private DevExpress.XtraEditors.TextEdit txtSshPassword;
        private DevExpress.XtraEditors.TextEdit txtSshLogin;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.SpinEdit sePort;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.TextEdit txtSshServer;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.CheckEdit cbSsl;
        private DevExpress.XtraTab.XtraTabControl tcConfiguration;
        private DevExpress.XtraTab.XtraTabPage tpConnection;
        private DevExpress.XtraTab.XtraTabPage tpSql;
        private DevExpress.XtraRichEdit.RichEditControl edtSql;
        private PanelControl panelControl5;
        private SimpleButton btnSaveSql;
        private SimpleButton btnLoadSql;
        private SimpleButton btnSqlPreview;
        private SimpleButton btnSqlValidate;
        private PanelControl panelControl6;
        private LabelControl lbSelectTableOrSql;
        private PanelControl pnlSsh;
        private RadioGroup rgAuthType;
        private SimpleButton btnRefreshServers;
    }
}