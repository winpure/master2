namespace WinPure.CleanAndMatch.Integration.Export
{
    partial class frmExportToSQLServer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExportToSQLServer));
            this.groupBox1 = new DevExpress.XtraEditors.GroupControl();
            this.btnRefreshServers = new DevExpress.XtraEditors.SimpleButton();
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
            this.lbError = new WinPure.CleanAndMatch.Controls.GrowLabel();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sePort.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTableName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServerName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbeDatebase.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgAuthType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogin.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AppearanceCaption.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupBox1.AppearanceCaption.Options.UseFont = true;
            this.groupBox1.Controls.Add(this.btnRefreshServers);
            this.groupBox1.Controls.Add(this.sePort);
            this.groupBox1.Controls.Add(this.labelControl9);
            this.groupBox1.Controls.Add(this.txtTableName);
            this.groupBox1.Controls.Add(this.labelControl5);
            this.groupBox1.Controls.Add(this.txtServerName);
            this.groupBox1.Controls.Add(this.cbeDatebase);
            this.groupBox1.Controls.Add(this.labelControl4);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupBox1.Location = new System.Drawing.Point(0, 24);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(8, 8, 8, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(327, 356);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.Text = "Connection Information";
            // 
            // btnRefreshServers
            // 
            this.btnRefreshServers.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnRefreshServers.ImageOptions.SvgImage")));
            this.btnRefreshServers.Location = new System.Drawing.Point(280, 44);
            this.btnRefreshServers.Name = "btnRefreshServers";
            this.btnRefreshServers.Size = new System.Drawing.Size(24, 23);
            this.btnRefreshServers.TabIndex = 83;
            this.btnRefreshServers.Click += new System.EventHandler(this.btnRefreshServers_Click);
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
            this.sePort.Properties.Mask.EditMask = "N00";
            this.sePort.Size = new System.Drawing.Size(147, 24);
            this.sePort.TabIndex = 2;
            // 
            // labelControl9
            // 
            this.labelControl9.Location = new System.Drawing.Point(35, 77);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Size = new System.Drawing.Size(21, 13);
            this.labelControl9.TabIndex = 82;
            this.labelControl9.Text = "Port";
            // 
            // txtTableName
            // 
            this.txtTableName.Location = new System.Drawing.Point(26, 317);
            this.txtTableName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtTableName.Name = "txtTableName";
            this.txtTableName.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtTableName.Properties.Appearance.Options.UseFont = true;
            this.txtTableName.Size = new System.Drawing.Size(280, 24);
            this.txtTableName.TabIndex = 5;
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(26, 300);
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
            this.cbeDatebase.Location = new System.Drawing.Point(26, 273);
            this.cbeDatebase.Name = "cbeDatebase";
            this.cbeDatebase.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.cbeDatebase.Properties.Appearance.Options.UseFont = true;
            this.cbeDatebase.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbeDatebase.Size = new System.Drawing.Size(281, 24);
            this.cbeDatebase.TabIndex = 4;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(26, 256);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(48, 13);
            this.labelControl4.TabIndex = 20;
            this.labelControl4.Text = "Database";
            // 
            // groupBox2
            // 
            this.groupBox2.AppearanceCaption.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupBox2.AppearanceCaption.Options.UseFont = true;
            this.groupBox2.Controls.Add(this.rgAuthType);
            this.groupBox2.Controls.Add(this.txtPassword);
            this.groupBox2.Controls.Add(this.txtLogin);
            this.groupBox2.Controls.Add(this.labelControl3);
            this.groupBox2.Controls.Add(this.labelControl2);
            this.groupBox2.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupBox2.Location = new System.Drawing.Point(26, 105);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(282, 143);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.Text = "Log on to the server";
            // 
            // rgAuthType
            // 
            this.rgAuthType.Location = new System.Drawing.Point(20, 24);
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
            this.txtPassword.Location = new System.Drawing.Point(78, 115);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(159, 22);
            this.txtPassword.TabIndex = 3;
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(78, 86);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(159, 22);
            this.txtLogin.TabIndex = 2;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(20, 119);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(49, 13);
            this.labelControl3.TabIndex = 17;
            this.labelControl3.Text = "Password";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(20, 89);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(29, 13);
            this.labelControl2.TabIndex = 16;
            this.labelControl2.Text = "Login";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(26, 23);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(248, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Server Name / IP Address  (type and press return)";
            // 
            // lbError
            // 
            this.lbError.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lbError.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lbError.Appearance.Options.UseFont = true;
            this.lbError.Appearance.Options.UseForeColor = true;
            this.lbError.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lbError.Location = new System.Drawing.Point(3, 404);
            this.lbError.Margin = new System.Windows.Forms.Padding(6);
            this.lbError.Name = "lbError";
            this.lbError.Padding = new System.Windows.Forms.Padding(5);
            this.lbError.Size = new System.Drawing.Size(73, 23);
            this.lbError.TabIndex = 79;
            this.lbError.Text = "labelControl1";
            this.lbError.Visible = false;
            // 
            // frmExportToSQLServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 464);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("frmExportToSQLServer.IconOptions.Icon")));
            this.IconOptions.ShowIcon = false;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmExportToSQLServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export to MS SQL Server";
            this.Controls.SetChildIndex(this.groupBox1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private Controls.GrowLabel lbError;
        private DevExpress.XtraEditors.SpinEdit sePort;
        private DevExpress.XtraEditors.LabelControl labelControl9;
        private RadioGroup rgAuthType;
        private SimpleButton btnRefreshServers;
    }
}