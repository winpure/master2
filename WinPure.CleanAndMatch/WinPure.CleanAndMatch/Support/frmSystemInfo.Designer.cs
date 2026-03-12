namespace WinPure.CleanAndMatch.Support
{
    partial class frmSystemInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSystemInfo));
            this.imgCpu = new DevExpress.XtraEditors.SvgImageBox();
            this.imgMemory = new DevExpress.XtraEditors.SvgImageBox();
            this.imgHdd = new DevExpress.XtraEditors.SvgImageBox();
            this.lbCpuCaption = new DevExpress.XtraEditors.LabelControl();
            this.lbHardDriveCaption = new DevExpress.XtraEditors.LabelControl();
            this.lbMemoryCaption = new DevExpress.XtraEditors.LabelControl();
            this.svgImageCollection1 = new DevExpress.Utils.SvgImageCollection(this.components);
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.txtCpu = new DevExpress.XtraEditors.MemoEdit();
            this.txtMemory = new DevExpress.XtraEditors.MemoEdit();
            this.txtHardDrive = new DevExpress.XtraEditors.MemoEdit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCpu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgMemory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgImageCollection1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCpu.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMemory.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHardDrive.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // imgCpu
            // 
            this.imgCpu.Location = new System.Drawing.Point(30, 25);
            this.imgCpu.Name = "imgCpu";
            this.imgCpu.Size = new System.Drawing.Size(34, 38);
            this.imgCpu.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("imgCpu.SvgImage")));
            this.imgCpu.TabIndex = 5;
            // 
            // imgMemory
            // 
            this.imgMemory.Location = new System.Drawing.Point(30, 152);
            this.imgMemory.Name = "imgMemory";
            this.imgMemory.Size = new System.Drawing.Size(34, 38);
            this.imgMemory.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("imgMemory.SvgImage")));
            this.imgMemory.TabIndex = 6;
            // 
            // imgHdd
            // 
            this.imgHdd.Location = new System.Drawing.Point(30, 281);
            this.imgHdd.Name = "imgHdd";
            this.imgHdd.Size = new System.Drawing.Size(34, 38);
            this.imgHdd.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("imgHdd.SvgImage")));
            this.imgHdd.TabIndex = 7;
            // 
            // lbCpuCaption
            // 
            this.lbCpuCaption.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCpuCaption.Appearance.Options.UseFont = true;
            this.lbCpuCaption.Location = new System.Drawing.Point(80, 25);
            this.lbCpuCaption.Name = "lbCpuCaption";
            this.lbCpuCaption.Size = new System.Drawing.Size(278, 21);
            this.lbCpuCaption.TabIndex = 8;
            this.lbCpuCaption.Text = "Insufficient CPU cores to run MatchAI.";
            // 
            // lbHardDriveCaption
            // 
            this.lbHardDriveCaption.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lbHardDriveCaption.Appearance.Options.UseFont = true;
            this.lbHardDriveCaption.Location = new System.Drawing.Point(80, 281);
            this.lbHardDriveCaption.Name = "lbHardDriveCaption";
            this.lbHardDriveCaption.Size = new System.Drawing.Size(281, 21);
            this.lbHardDriveCaption.TabIndex = 15;
            this.lbHardDriveCaption.Text = "Insufficient Hard drive to run MatchAI.";
            // 
            // lbMemoryCaption
            // 
            this.lbMemoryCaption.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lbMemoryCaption.Appearance.Options.UseFont = true;
            this.lbMemoryCaption.Location = new System.Drawing.Point(80, 152);
            this.lbMemoryCaption.Name = "lbMemoryCaption";
            this.lbMemoryCaption.Size = new System.Drawing.Size(280, 21);
            this.lbMemoryCaption.TabIndex = 13;
            this.lbMemoryCaption.Text = "Memory is insufficient to run MatchAI.";
            // 
            // svgImageCollection1
            // 
            this.svgImageCollection1.Add("actions_checkcircled", "image://svgimages/icon builder/actions_checkcircled.svg");
            this.svgImageCollection1.Add("warning", "image://svgimages/status/warning.svg");
            this.svgImageCollection1.Add("security_warningcircled2", "image://svgimages/icon builder/security_warningcircled2.svg");
            // 
            // btnClose
            // 
            this.btnClose.Appearance.BorderColor = System.Drawing.Color.Gray;
            this.btnClose.Appearance.Options.UseBorderColor = true;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(239, 361);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(115, 31);
            this.btnClose.TabIndex = 17;
            this.btnClose.Text = "Close";
            // 
            // txtCpu
            // 
            this.txtCpu.EditValue = "• System has 8 physical cores and 8 logical cores.\r\n• At least 4 physical CPU cor" +
    "es are recommended to run MatchAI.\r\n• The minimum requirement to run MatchAI is " +
    "2 physical CPU cores.";
            this.txtCpu.Location = new System.Drawing.Point(80, 52);
            this.txtCpu.Name = "txtCpu";
            this.txtCpu.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtCpu.Properties.Appearance.Options.UseFont = true;
            this.txtCpu.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.White;
            this.txtCpu.Properties.AppearanceReadOnly.Options.UseBackColor = true;
            this.txtCpu.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.txtCpu.Properties.ReadOnly = true;
            this.txtCpu.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtCpu.Size = new System.Drawing.Size(480, 73);
            this.txtCpu.TabIndex = 18;
            // 
            // txtMemory
            // 
            this.txtMemory.EditValue = "• Total system memory is 31.8 GB with 19.9 GB available.\r\n• At least 8.0 GB of av" +
    "ailable memory is recommended to run MatchAI.\r\n• The minimum requirement to run " +
    "MatchAI is 6.0 GB available memory.";
            this.txtMemory.Location = new System.Drawing.Point(80, 179);
            this.txtMemory.Name = "txtMemory";
            this.txtMemory.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtMemory.Properties.Appearance.Options.UseFont = true;
            this.txtMemory.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.White;
            this.txtMemory.Properties.AppearanceReadOnly.Options.UseBackColor = true;
            this.txtMemory.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.txtMemory.Properties.ReadOnly = true;
            this.txtMemory.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtMemory.Size = new System.Drawing.Size(480, 73);
            this.txtMemory.TabIndex = 18;
            // 
            // txtHardDrive
            // 
            this.txtHardDrive.EditValue = "• System uses fast SSD drive";
            this.txtHardDrive.Location = new System.Drawing.Point(80, 308);
            this.txtHardDrive.Name = "txtHardDrive";
            this.txtHardDrive.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtHardDrive.Properties.Appearance.Options.UseFont = true;
            this.txtHardDrive.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.White;
            this.txtHardDrive.Properties.AppearanceReadOnly.Options.UseBackColor = true;
            this.txtHardDrive.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.txtHardDrive.Properties.ReadOnly = true;
            this.txtHardDrive.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtHardDrive.Size = new System.Drawing.Size(480, 42);
            this.txtHardDrive.TabIndex = 18;
            // 
            // frmSystemInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 404);
            this.ControlBox = false;
            this.Controls.Add(this.txtHardDrive);
            this.Controls.Add(this.txtMemory);
            this.Controls.Add(this.txtCpu);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lbHardDriveCaption);
            this.Controls.Add(this.lbMemoryCaption);
            this.Controls.Add(this.lbCpuCaption);
            this.Controls.Add(this.imgHdd);
            this.Controls.Add(this.imgMemory);
            this.Controls.Add(this.imgCpu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("frmSystemInfo.IconOptions.SvgImage")));
            this.Name = "frmSystemInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "System Requirements and Performance Status";
            ((System.ComponentModel.ISupportInitialize)(this.imgCpu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgMemory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgImageCollection1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCpu.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMemory.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHardDrive.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SvgImageBox imgCpu;
        private DevExpress.XtraEditors.SvgImageBox imgMemory;
        private DevExpress.XtraEditors.SvgImageBox imgHdd;
        private DevExpress.XtraEditors.LabelControl lbCpuCaption;
        private DevExpress.XtraEditors.LabelControl lbHardDriveCaption;
        private DevExpress.XtraEditors.LabelControl lbMemoryCaption;
        private DevExpress.Utils.SvgImageCollection svgImageCollection1;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private MemoEdit txtCpu;
        private MemoEdit txtMemory;
        private MemoEdit txtHardDrive;
    }
}