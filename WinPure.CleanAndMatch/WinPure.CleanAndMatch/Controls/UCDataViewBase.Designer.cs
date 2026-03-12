namespace WinPure.CleanAndMatch.Controls
{
    partial class UCDataViewBase
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitDataContainer = new DevExpress.XtraEditors.SplitContainerControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.tcData = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            ((System.ComponentModel.ISupportInitialize)(this.splitDataContainer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitDataContainer.Panel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitDataContainer.Panel2)).BeginInit();
            this.splitDataContainer.Panel2.SuspendLayout();
            this.splitDataContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tcData)).BeginInit();
            this.SuspendLayout();
            // 
            // splitDataContainer
            // 
            this.splitDataContainer.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            this.splitDataContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitDataContainer.Horizontal = false;
            this.splitDataContainer.Location = new System.Drawing.Point(0, 0);
            this.splitDataContainer.Name = "splitDataContainer";
            // 
            // splitDataContainer.Panel1
            // 
            this.splitDataContainer.Panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.splitDataContainer.Panel1.Text = "Panel1";
            // 
            // splitDataContainer.Panel2
            // 
            this.splitDataContainer.Panel2.Controls.Add(this.panelControl1);
            this.splitDataContainer.Panel2.Text = "Panel2";
            this.splitDataContainer.Size = new System.Drawing.Size(1143, 662);
            this.splitDataContainer.SplitterPosition = 122;
            this.splitDataContainer.TabIndex = 1;
            this.splitDataContainer.Text = "splitContainerControl1";
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.tcData);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1139, 524);
            this.panelControl1.TabIndex = 48;
            // 
            // tcData
            // 
            this.tcData.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcData.Appearance.Options.UseFont = true;
            this.tcData.AppearancePage.HeaderActive.BackColor = System.Drawing.SystemColors.Highlight;
            this.tcData.AppearancePage.HeaderActive.Options.UseBackColor = true;
            this.tcData.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.tcData.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.InAllTabPageHeaders;
            this.tcData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcData.Location = new System.Drawing.Point(2, 2);
            this.tcData.Name = "tcData";
            this.tcData.Size = new System.Drawing.Size(1135, 520);
            this.tcData.TabIndex = 47;
            this.tcData.Visible = false;
            this.tcData.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tcData_SelectedPageChanged);
            this.tcData.CloseButtonClick += new System.EventHandler(this.tcData_CloseButtonClick);
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(1135, 524);
            this.xtraTabPage1.Text = "xtraTabPage1";
            // 
            // UCDataViewBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitDataContainer);
            this.Name = "UCDataViewBase";
            this.Size = new System.Drawing.Size(1143, 662);
            ((System.ComponentModel.ISupportInitialize)(this.splitDataContainer.Panel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitDataContainer.Panel2)).EndInit();
            this.splitDataContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitDataContainer)).EndInit();
            this.splitDataContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tcData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public DevExpress.XtraEditors.SplitContainerControl splitDataContainer;
        public DevExpress.XtraTab.XtraTabControl tcData;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private PanelControl panelControl1;
    }
}
