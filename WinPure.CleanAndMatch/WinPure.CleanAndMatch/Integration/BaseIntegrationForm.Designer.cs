using System.ComponentModel;

namespace WinPure.CleanAndMatch.Integration
{
    internal partial class BaseIntegrationForm
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
            // Unsubscribe to avoid dangling references
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                DevExpress.LookAndFeel.UserLookAndFeel.Default.StyleChanged -= OnThemeChanged;
            }

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
            this.ctxMenu = new System.Windows.Forms.MenuStrip();
            this.mnuLoadConfiguration = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSaveConfiguration = new System.Windows.Forms.ToolStripMenuItem();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.ctxMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // ctxMenu
            // 
            this.ctxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLoadConfiguration,
            this.mnuSaveConfiguration});
            this.ctxMenu.Location = new System.Drawing.Point(0, 0);
            this.ctxMenu.Name = "ctxMenu";
            this.ctxMenu.Size = new System.Drawing.Size(439, 24);
            this.ctxMenu.TabIndex = 108;
            this.ctxMenu.Text = "menuStrip1";
            // 
            // mnuLoadConfiguration
            // 
            this.mnuLoadConfiguration.Image = global::WinPure.CleanAndMatch.Properties.Resources.Document_In_Folder;
            this.mnuLoadConfiguration.Name = "mnuLoadConfiguration";
            this.mnuLoadConfiguration.Size = new System.Drawing.Size(126, 20);
            this.mnuLoadConfiguration.Text = "Load Connection";
            // 
            // mnuSaveConfiguration
            // 
            this.mnuSaveConfiguration.Image = global::WinPure.CleanAndMatch.Properties.Resources.Save;
            this.mnuSaveConfiguration.Name = "mnuSaveConfiguration";
            this.mnuSaveConfiguration.Size = new System.Drawing.Size(124, 20);
            this.mnuSaveConfiguration.Text = "Save Connection";
            this.mnuSaveConfiguration.Click += new System.EventHandler(this.SaveConnectionMenuItem_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 361);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(439, 10);
            this.panelControl1.TabIndex = 109;
            this.panelControl1.Visible = false;
            // 
            // BaseIntegrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 371);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.ctxMenu);
            this.IconOptions.ShowIcon = false;
            this.Name = "BaseIntegrationForm";
            this.Load += new System.EventHandler(this.BaseIntegrationForm_Load);
            this.ctxMenu.ResumeLayout(false);
            this.ctxMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.MenuStrip ctxMenu;
        internal System.Windows.Forms.ToolStripMenuItem mnuLoadConfiguration;
        internal System.Windows.Forms.ToolStripMenuItem mnuSaveConfiguration;
        private PanelControl panelControl1;
    }
}