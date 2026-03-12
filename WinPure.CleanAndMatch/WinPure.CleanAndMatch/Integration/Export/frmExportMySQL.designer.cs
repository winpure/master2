using WinPure.CleanAndMatch.Controls;

namespace WinPure.CleanAndMatch.Integration.Export
{
    partial class frmExportMySQL
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExportMySQL));
            DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions buttonImageOptions1 = new DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions();
            DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions buttonImageOptions2 = new DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions();
            this.groupBox1 = new DevExpress.XtraEditors.GroupControl();
            this.btnRefreshServers = new DevExpress.XtraEditors.SimpleButton();
            this.cbSsl = new DevExpress.XtraEditors.CheckEdit();
            this.sePort = new DevExpress.XtraEditors.SpinEdit();
            this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
            this.txtTableName = new DevExpress.XtraEditors.TextEdit();
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
            this.gcSshInfo = new DevExpress.XtraEditors.GroupControl();
            this.pnlSsh = new DevExpress.XtraEditors.PanelControl();
            this.cbUseSsh = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.txtSshServer = new DevExpress.XtraEditors.TextEdit();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.txtSshLogin = new DevExpress.XtraEditors.TextEdit();
            this.txtSshPassword = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbSsl.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sePort.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTableName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServerName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbeDatebase.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgAuthType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcSshInfo)).BeginInit();
            this.gcSshInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSsh)).BeginInit();
            this.pnlSsh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbUseSsh.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSshServer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSshLogin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSshPassword.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.groupBox1.AppearanceCaption.Options.UseFont = true;
            this.groupBox1.Controls.Add(this.btnRefreshServers);
            this.groupBox1.Controls.Add(this.cbSsl);
            this.groupBox1.Controls.Add(this.sePort);
            this.groupBox1.Controls.Add(this.labelControl9);
            this.groupBox1.Controls.Add(this.txtTableName);
            this.groupBox1.Controls.Add(this.labelControl5);
            this.groupBox1.Controls.Add(this.txtServerName);
            this.groupBox1.Controls.Add(this.cbeDatebase);
            this.groupBox1.Controls.Add(this.labelControl4);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupBox1.Location = new System.Drawing.Point(14, 29);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(327, 374);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.Text = "Connection Information";
            // 
            // btnRefreshServers
            // 
            this.btnRefreshServers.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnRefreshServers.ImageOptions.SvgImage")));
            this.btnRefreshServers.Location = new System.Drawing.Point(280, 43);
            this.btnRefreshServers.Name = "btnRefreshServers";
            this.btnRefreshServers.Size = new System.Drawing.Size(25, 23);
            this.btnRefreshServers.TabIndex = 81;
            this.btnRefreshServers.Click += new System.EventHandler(this.btnRefreshServers_Click);
            // 
            // cbSsl
            // 
            this.cbSsl.Location = new System.Drawing.Point(229, 73);
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
            this.sePort.Location = new System.Drawing.Point(62, 72);
            this.sePort.Name = "sePort";
            this.sePort.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.sePort.Properties.Appearance.Options.UseFont = true;
            this.sePort.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.sePort.Properties.IsFloatValue = false;
            this.sePort.Properties.MaskSettings.Set("mask", "N00");
            this.sePort.Size = new System.Drawing.Size(147, 24);
            this.sePort.TabIndex = 2;
            // 
            // labelControl9
            // 
            this.labelControl9.Location = new System.Drawing.Point(26, 75);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Size = new System.Drawing.Size(21, 13);
            this.labelControl9.TabIndex = 80;
            this.labelControl9.Text = "Port";
            // 
            // txtTableName
            // 
            this.txtTableName.Location = new System.Drawing.Point(26, 337);
            this.txtTableName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtTableName.Name = "txtTableName";
            this.txtTableName.Size = new System.Drawing.Size(282, 22);
            this.txtTableName.TabIndex = 6;
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(26, 320);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(58, 13);
            this.labelControl5.TabIndex = 24;
            this.labelControl5.Text = "Table name";
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(26, 41);
            this.txtServerName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtServerName.Properties.Appearance.Options.UseFont = true;
            this.txtServerName.Size = new System.Drawing.Size(248, 26);
            this.txtServerName.TabIndex = 0;
            // 
            // cbeDatebase
            // 
            this.cbeDatebase.Location = new System.Drawing.Point(26, 284);
            this.cbeDatebase.Name = "cbeDatebase";
            this.cbeDatebase.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbeDatebase.Size = new System.Drawing.Size(281, 22);
            this.cbeDatebase.TabIndex = 5;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(26, 264);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(48, 13);
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
            this.groupBox2.Location = new System.Drawing.Point(26, 105);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(282, 153);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.Text = "Log on to the server";
            // 
            // rgAuthType
            // 
            this.rgAuthType.Location = new System.Drawing.Point(22, 24);
            this.rgAuthType.Name = "rgAuthType";
            this.rgAuthType.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.rgAuthType.Properties.Appearance.Options.UseBackColor = true;
            this.rgAuthType.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.rgAuthType.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Use Windows Authentication", true, null, "rgWinAuth"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Use SQL server Authentication", true, null, "rgServerAuth")});
            this.rgAuthType.Size = new System.Drawing.Size(217, 56);
            this.rgAuthType.TabIndex = 18;
            this.rgAuthType.SelectedIndexChanged += new System.EventHandler(this.rgAuthType_SelectedIndexChanged);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(80, 112);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(159, 22);
            this.txtPassword.TabIndex = 3;
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(80, 83);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(159, 22);
            this.txtLogin.TabIndex = 2;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(22, 111);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(49, 13);
            this.labelControl3.TabIndex = 17;
            this.labelControl3.Text = "Password";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(22, 81);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(29, 13);
            this.labelControl2.TabIndex = 16;
            this.labelControl2.Text = "Login";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(17, 19);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(248, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Server Name / IP Address  (type and press return)";
            // 
            // gcSshInfo
            // 
            this.gcSshInfo.AppearanceCaption.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
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
            this.gcSshInfo.Location = new System.Drawing.Point(14, 406);
            this.gcSshInfo.Margin = new System.Windows.Forms.Padding(9);
            this.gcSshInfo.Name = "gcSshInfo";
            this.gcSshInfo.Size = new System.Drawing.Size(327, 150);
            this.gcSshInfo.TabIndex = 1;
            this.gcSshInfo.Text = "SSH Information";
            this.gcSshInfo.CustomButtonClick += new DevExpress.XtraBars.Docking2010.BaseButtonEventHandler(this.gcSshInfo_CustomButtonClick);
            // 
            // pnlSsh
            // 
            this.pnlSsh.Controls.Add(this.cbUseSsh);
            this.pnlSsh.Controls.Add(this.labelControl7);
            this.pnlSsh.Controls.Add(this.txtSshServer);
            this.pnlSsh.Controls.Add(this.labelControl6);
            this.pnlSsh.Controls.Add(this.labelControl8);
            this.pnlSsh.Controls.Add(this.txtSshLogin);
            this.pnlSsh.Controls.Add(this.txtSshPassword);
            this.pnlSsh.Location = new System.Drawing.Point(4, 26);
            this.pnlSsh.Name = "pnlSsh";
            this.pnlSsh.Size = new System.Drawing.Size(318, 117);
            this.pnlSsh.TabIndex = 26;
            this.pnlSsh.Visible = false;
            // 
            // cbUseSsh
            // 
            this.cbUseSsh.Location = new System.Drawing.Point(7, 6);
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
            // labelControl7
            // 
            this.labelControl7.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl7.Appearance.Options.UseFont = true;
            this.labelControl7.Location = new System.Drawing.Point(8, 67);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(29, 15);
            this.labelControl7.TabIndex = 20;
            this.labelControl7.Text = "Login";
            // 
            // txtSshServer
            // 
            this.txtSshServer.Enabled = false;
            this.txtSshServer.Location = new System.Drawing.Point(70, 36);
            this.txtSshServer.Name = "txtSshServer";
            this.txtSshServer.Size = new System.Drawing.Size(231, 22);
            this.txtSshServer.TabIndex = 1;
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(8, 88);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(49, 13);
            this.labelControl6.TabIndex = 21;
            this.labelControl6.Text = "Password";
            // 
            // labelControl8
            // 
            this.labelControl8.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl8.Appearance.Options.UseFont = true;
            this.labelControl8.Location = new System.Drawing.Point(8, 41);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(34, 15);
            this.labelControl8.TabIndex = 25;
            this.labelControl8.Text = "Server";
            // 
            // txtSshLogin
            // 
            this.txtSshLogin.Enabled = false;
            this.txtSshLogin.Location = new System.Drawing.Point(70, 62);
            this.txtSshLogin.Name = "txtSshLogin";
            this.txtSshLogin.Size = new System.Drawing.Size(231, 22);
            this.txtSshLogin.TabIndex = 2;
            // 
            // txtSshPassword
            // 
            this.txtSshPassword.Enabled = false;
            this.txtSshPassword.Location = new System.Drawing.Point(70, 88);
            this.txtSshPassword.Name = "txtSshPassword";
            this.txtSshPassword.Properties.PasswordChar = '*';
            this.txtSshPassword.Size = new System.Drawing.Size(231, 22);
            this.txtSshPassword.TabIndex = 3;
            // 
            // frmExportMySQL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 647);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gcSshInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("frmExportMySQL.IconOptions.Icon")));
            this.IconOptions.ShowIcon = false;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmExportMySQL";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export to MS SQL Server";
            this.Controls.SetChildIndex(this.gcSshInfo, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbSsl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sePort.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTableName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServerName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbeDatebase.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgAuthType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcSshInfo)).EndInit();
            this.gcSshInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlSsh)).EndInit();
            this.pnlSsh.ResumeLayout(false);
            this.pnlSsh.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbUseSsh.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSshServer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSshLogin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSshPassword.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupBox1;
        private DevExpress.XtraEditors.TextEdit txtTableName;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.TextEdit txtServerName;
        private DevExpress.XtraEditors.ComboBoxEdit cbeDatebase;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.GroupControl groupBox2;
        private DevExpress.XtraEditors.TextEdit txtPassword;
        private DevExpress.XtraEditors.TextEdit txtLogin;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.GroupControl gcSshInfo;
        private DevExpress.XtraEditors.TextEdit txtSshServer;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.TextEdit txtSshPassword;
        private DevExpress.XtraEditors.TextEdit txtSshLogin;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.SpinEdit sePort;
        private DevExpress.XtraEditors.LabelControl labelControl9;
        private DevExpress.XtraEditors.CheckEdit cbSsl;
        private DevExpress.XtraEditors.CheckEdit cbUseSsh;
        private PanelControl pnlSsh;
        private RadioGroup rgAuthType;
        private SimpleButton btnRefreshServers;
    }
}