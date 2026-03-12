namespace WinPure.CleanAndMatch.MatchResultProcessing
{
    partial class frmMatchResultMerge
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMatchResultMerge));
            this.gridUpdate = new DevExpress.XtraGrid.GridControl();
            this.gvUpdate = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.cbMarkSaveAll = new DevExpress.XtraEditors.CheckEdit();
            this.cbMarkOnlyEmpty = new DevExpress.XtraEditors.CheckEdit();
            this.cbMarkUpdateField = new DevExpress.XtraEditors.CheckEdit();
            this.lbMasterRecords = new DevExpress.XtraEditors.LabelControl();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnUpdate = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.gridUpdate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvUpdate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbMarkSaveAll.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbMarkOnlyEmpty.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbMarkUpdateField.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridUpdate
            // 
            this.gridUpdate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridUpdate.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gridUpdate.Location = new System.Drawing.Point(0, 37);
            this.gridUpdate.MainView = this.gvUpdate;
            this.gridUpdate.Name = "gridUpdate";
            this.gridUpdate.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemComboBox1,
            this.repositoryItemCheckEdit1});
            this.gridUpdate.Size = new System.Drawing.Size(517, 297);
            this.gridUpdate.TabIndex = 4;
            this.gridUpdate.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvUpdate});
            // 
            // gvUpdate
            // 
            this.gvUpdate.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.gvUpdate.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvUpdate.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn3,
            this.gridColumn2,
            this.gridColumn4});
            this.gvUpdate.GridControl = this.gridUpdate;
            this.gvUpdate.Name = "gvUpdate";
            this.gvUpdate.OptionsView.ColumnAutoWidth = false;
            this.gvUpdate.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn1.AppearanceHeader.Options.UseFont = true;
            this.gridColumn1.Caption = "Column Name";
            this.gridColumn1.FieldName = "FieldName";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.ReadOnly = true;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 196;
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceHeader.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn3.AppearanceHeader.Options.UseFont = true;
            this.gridColumn3.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn3.Caption = "Update field";
            this.gridColumn3.ColumnEdit = this.repositoryItemCheckEdit1;
            this.gridColumn3.FieldName = "UpdateOption";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.ToolTip = "Select the columns you wish to use for merging.";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 1;
            this.gridColumn3.Width = 108;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.repositoryItemCheckEdit1.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn2.AppearanceHeader.Options.UseFont = true;
            this.gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn2.Caption = "Only empty";
            this.gridColumn2.ColumnEdit = this.repositoryItemCheckEdit1;
            this.gridColumn2.FieldName = "OnlyEmpty";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.ToolTip = "Only update if column is empty";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 2;
            this.gridColumn2.Width = 83;
            // 
            // gridColumn4
            // 
            this.gridColumn4.AppearanceHeader.Font = new System.Drawing.Font("Open Sans Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn4.AppearanceHeader.Options.UseFont = true;
            this.gridColumn4.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn4.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn4.Caption = "Keep all values";
            this.gridColumn4.ColumnEdit = this.repositoryItemCheckEdit1;
            this.gridColumn4.FieldName = "SaveAllValues";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.ToolTip = "Add additional columns to store all values";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            this.gridColumn4.Width = 93;
            // 
            // repositoryItemComboBox1
            // 
            this.repositoryItemComboBox1.AutoHeight = false;
            this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
            this.repositoryItemComboBox1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.cbMarkSaveAll);
            this.panelControl2.Controls.Add(this.cbMarkOnlyEmpty);
            this.panelControl2.Controls.Add(this.cbMarkUpdateField);
            this.panelControl2.Controls.Add(this.lbMasterRecords);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl2.Location = new System.Drawing.Point(0, 0);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(517, 37);
            this.panelControl2.TabIndex = 5;
            // 
            // cbMarkSaveAll
            // 
            this.cbMarkSaveAll.AutoSizeInLayoutControl = true;
            this.cbMarkSaveAll.Location = new System.Drawing.Point(442, 11);
            this.cbMarkSaveAll.Name = "cbMarkSaveAll";
            this.cbMarkSaveAll.Properties.Caption = "";
            this.cbMarkSaveAll.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbMarkSaveAll.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbMarkSaveAll.Size = new System.Drawing.Size(22, 20);
            this.cbMarkSaveAll.TabIndex = 3;
            this.cbMarkSaveAll.Tag = "SaveAllValues";
            this.cbMarkSaveAll.CheckedChanged += new System.EventHandler(this.cbMarkAllEmpty_CheckedChanged);
            // 
            // cbMarkOnlyEmpty
            // 
            this.cbMarkOnlyEmpty.AutoSizeInLayoutControl = true;
            this.cbMarkOnlyEmpty.EditValue = true;
            this.cbMarkOnlyEmpty.Location = new System.Drawing.Point(352, 12);
            this.cbMarkOnlyEmpty.Name = "cbMarkOnlyEmpty";
            this.cbMarkOnlyEmpty.Properties.Caption = "";
            this.cbMarkOnlyEmpty.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbMarkOnlyEmpty.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbMarkOnlyEmpty.Size = new System.Drawing.Size(22, 20);
            this.cbMarkOnlyEmpty.TabIndex = 2;
            this.cbMarkOnlyEmpty.Tag = "OnlyEmpty";
            this.cbMarkOnlyEmpty.CheckedChanged += new System.EventHandler(this.cbMarkAllEmpty_CheckedChanged);
            // 
            // cbMarkUpdateField
            // 
            this.cbMarkUpdateField.AutoSizeInLayoutControl = true;
            this.cbMarkUpdateField.EditValue = true;
            this.cbMarkUpdateField.Location = new System.Drawing.Point(259, 11);
            this.cbMarkUpdateField.Name = "cbMarkUpdateField";
            this.cbMarkUpdateField.Properties.Caption = "";
            this.cbMarkUpdateField.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbMarkUpdateField.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbMarkUpdateField.Size = new System.Drawing.Size(22, 20);
            this.cbMarkUpdateField.TabIndex = 1;
            this.cbMarkUpdateField.Tag = "UpdateOption";
            this.cbMarkUpdateField.CheckedChanged += new System.EventHandler(this.cbMarkAllEmpty_CheckedChanged);
            // 
            // lbMasterRecords
            // 
            this.lbMasterRecords.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMasterRecords.Appearance.Options.UseFont = true;
            this.lbMasterRecords.Location = new System.Drawing.Point(12, 12);
            this.lbMasterRecords.Name = "lbMasterRecords";
            this.lbMasterRecords.Size = new System.Drawing.Size(74, 16);
            this.lbMasterRecords.TabIndex = 0;
            this.lbMasterRecords.Text = "labelControl1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(5, 38);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(15, 19);
            this.pictureBox1.TabIndex = 72;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Tag = "";
            this.toolTip1.SetToolTip(this.pictureBox1, "Click to learn more");
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.pictureBox1);
            this.panelControl1.Controls.Add(this.btnUpdate);
            this.panelControl1.Controls.Add(this.btnCancel);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 334);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(517, 69);
            this.panelControl1.TabIndex = 3;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdate.Appearance.Options.UseFont = true;
            this.btnUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUpdate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnUpdate.ImageOptions.SvgImage = global::WinPure.CleanAndMatch.Properties.Resources._2026_Merge_Vertical_24;
            this.btnUpdate.Location = new System.Drawing.Point(282, 16);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(100, 41);
            this.btnUpdate.TabIndex = 4;
            this.btnUpdate.Text = "&Merge";
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageOptions.SvgImage = global::WinPure.CleanAndMatch.Properties.Resources._2026_Close;
            this.btnCancel.Location = new System.Drawing.Point(388, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 41);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            // 
            // frmMatchResultMerge
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 403);
            this.ControlBox = false;
            this.Controls.Add(this.gridUpdate);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmMatchResultMerge";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Merge options";
            ((System.ComponentModel.ISupportInitialize)(this.gridUpdate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvUpdate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.panelControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbMarkSaveAll.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbMarkOnlyEmpty.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbMarkUpdateField.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridUpdate;
        private DevExpress.XtraGrid.Views.Grid.GridView gvUpdate;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.CheckEdit cbMarkUpdateField;
        private DevExpress.XtraEditors.LabelControl lbMasterRecords;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnUpdate;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraEditors.CheckEdit cbMarkSaveAll;
        private DevExpress.XtraEditors.CheckEdit cbMarkOnlyEmpty;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}