namespace WinPure.CleanAndMatch.Support
{
    partial class LoadingMaskForm
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
            this.MarqueeBar = new DevExpress.XtraEditors.MarqueeProgressBarControl();
            this.timerForClose = new System.Windows.Forms.Timer(this.components);
            this.progressBar = new DevExpress.XtraEditors.ProgressBarControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.MarqueeBar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.progressBar.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // MarqueeBar
            // 
            this.MarqueeBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MarqueeBar.EditValue = "Suche läuft ...";
            this.MarqueeBar.Location = new System.Drawing.Point(0, 0);
            this.MarqueeBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MarqueeBar.Name = "MarqueeBar";
            this.MarqueeBar.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MarqueeBar.Properties.AppearanceDisabled.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MarqueeBar.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.MarqueeBar.Properties.AppearanceReadOnly.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.MarqueeBar.Properties.MarqueeWidth = 250;
            this.MarqueeBar.Properties.ProgressAnimationMode = DevExpress.Utils.Drawing.ProgressAnimationMode.PingPong;
            this.MarqueeBar.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
            this.MarqueeBar.Properties.ShowTitle = true;
            this.MarqueeBar.Properties.Stopped = true;
            this.MarqueeBar.Size = new System.Drawing.Size(501, 51);
            this.MarqueeBar.TabIndex = 132;
            this.MarqueeBar.UseWaitCursor = true;
            // 
            // timerForClose
            // 
            this.timerForClose.Interval = 500;
            this.timerForClose.Tick += new System.EventHandler(this.timerForClose_Tick);
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar.EditValue = "0";
            this.progressBar.Location = new System.Drawing.Point(0, 0);
            this.progressBar.Margin = new System.Windows.Forms.Padding(0);
            this.progressBar.Name = "progressBar";
            this.progressBar.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressBar.Properties.EndColor = System.Drawing.Color.RoyalBlue;
            this.progressBar.Properties.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.progressBar.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.progressBar.Properties.PercentView = false;
            this.progressBar.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
            this.progressBar.Properties.ShowTitle = true;
            this.progressBar.Properties.StartColor = System.Drawing.Color.SteelBlue;
            this.progressBar.Properties.Step = 1;
            this.progressBar.ShowProgressInTaskBar = true;
            this.progressBar.ShowToolTips = false;
            this.progressBar.Size = new System.Drawing.Size(501, 51);
            this.progressBar.TabIndex = 134;
            this.progressBar.UseWaitCursor = true;
            this.progressBar.Visible = false;
            this.progressBar.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.progressBar_CustomDisplayText);
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.BackColor = System.Drawing.Color.Orange;
            this.btnCancel.Appearance.BackColor2 = System.Drawing.Color.Bisque;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnCancel.Appearance.Options.UseBackColor = true;
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.AppearanceDisabled.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.AppearanceDisabled.Options.UseFont = true;
            this.btnCancel.AppearanceHovered.BackColor = System.Drawing.Color.Orange;
            this.btnCancel.AppearanceHovered.BackColor2 = System.Drawing.Color.Coral;
            this.btnCancel.AppearanceHovered.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.AppearanceHovered.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnCancel.AppearanceHovered.Options.UseBackColor = true;
            this.btnCancel.AppearanceHovered.Options.UseFont = true;
            this.btnCancel.AppearancePressed.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.AppearancePressed.Options.UseFont = true;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.Location = new System.Drawing.Point(501, 0);
            this.btnCancel.LookAndFeel.SkinName = "WXI";
            this.btnCancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
            this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCancel.Margin = new System.Windows.Forms.Padding(0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(131, 51);
            this.btnCancel.TabIndex = 135;
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.UseWaitCursor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // LoadingMaskForm
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 51);
            this.ControlBox = false;
            this.Controls.Add(this.MarqueeBar);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.None;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "LoadingMaskForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LoadingMaskForm";
            this.UseWaitCursor = true;
            this.Shown += new System.EventHandler(this.LoadingMaskForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.MarqueeBar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.progressBar.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal DevExpress.XtraEditors.MarqueeProgressBarControl MarqueeBar;
        private System.Windows.Forms.Timer timerForClose;
        private DevExpress.XtraEditors.ProgressBarControl progressBar;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
    }
}