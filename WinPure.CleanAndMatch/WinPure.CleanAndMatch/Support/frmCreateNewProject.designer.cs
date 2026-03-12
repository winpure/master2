namespace WinPure.CleanAndMatch.Support
{
    partial class frmCreateNewProject
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCreateNewProject));
            this.txtNewName = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.lbProjectPath = new DevExpress.XtraEditors.LabelControl();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnSelectFolder = new DevExpress.XtraEditors.SimpleButton();
            this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.txtNewName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtNewName
            // 
            this.txtNewName.Location = new System.Drawing.Point(224, 58);
            this.txtNewName.Name = "txtNewName";
            this.txtNewName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNewName.Properties.Appearance.Options.UseFont = true;
            this.txtNewName.Size = new System.Drawing.Size(302, 28);
            this.txtNewName.TabIndex = 47;
            this.txtNewName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNewProject_KeyDown);
            this.txtNewName.Leave += new System.EventHandler(this.txtNewName_Leave);
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(224, 19);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(279, 20);
            this.labelControl1.TabIndex = 46;
            this.labelControl1.Text = "Please enter the name of the new project";
            // 
            // btnOK
            // 
            this.btnOK.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnOK.Appearance.Options.UseFont = true;
            this.btnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOK.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnOK.ImageOptions.SvgImage")));
            this.btnOK.Location = new System.Drawing.Point(290, 128);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(148, 37);
            this.btnOK.TabIndex = 50;
            this.btnOK.Text = "Create";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lbProjectPath
            // 
            this.lbProjectPath.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProjectPath.Appearance.Options.UseFont = true;
            this.lbProjectPath.Appearance.Options.UseTextOptions = true;
            this.lbProjectPath.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lbProjectPath.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lbProjectPath.Location = new System.Drawing.Point(224, 89);
            this.lbProjectPath.Name = "lbProjectPath";
            this.lbProjectPath.Size = new System.Drawing.Size(302, 33);
            this.lbProjectPath.TabIndex = 52;
            this.lbProjectPath.Text = "Project path";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::WinPure.CleanAndMatch.Properties.Resources._2026_new_project;
            this.pictureBox1.Location = new System.Drawing.Point(25, 24);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(171, 143);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 54;
            this.pictureBox1.TabStop = false;
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnSelectFolder.ImageOptions.SvgImage")));
            this.btnSelectFolder.Location = new System.Drawing.Point(532, 61);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(39, 28);
            this.btnSelectFolder.TabIndex = 55;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // frmCreateNewProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 233);
            this.Controls.Add(this.btnSelectFolder);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lbProjectPath);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtNewName);
            this.Controls.Add(this.labelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("frmCreateNewProject.IconOptions.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCreateNewProject";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create New Project";
            ((System.ComponentModel.ISupportInitialize)(this.txtNewName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraEditors.TextEdit txtNewName;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.LabelControl lbProjectPath;
        private System.Windows.Forms.PictureBox pictureBox1;
        private DevExpress.XtraEditors.SimpleButton btnSelectFolder;
        private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
    }
}