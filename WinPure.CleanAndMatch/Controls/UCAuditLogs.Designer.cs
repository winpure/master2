namespace WinPure.CleanAndMatch.Controls
{
    partial class UCAuditLogs
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCAuditLogs));
            this.tsAuditLogsEnabled = new DevExpress.XtraEditors.ToggleSwitch();
            this.deLogTo = new DevExpress.XtraEditors.DateEdit();
            this.cbLogModule = new DevExpress.XtraEditors.CheckedComboBoxEdit();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.cbLogSource = new DevExpress.XtraEditors.CheckedComboBoxEdit();
            this.lbSourceName = new DevExpress.XtraEditors.LabelControl();
            this.deLogFrom = new DevExpress.XtraEditors.DateEdit();
            this.lbDateTo = new DevExpress.XtraEditors.LabelControl();
            this.lbModulName = new DevExpress.XtraEditors.LabelControl();
            this.lbDateFrom = new DevExpress.XtraEditors.LabelControl();
            this.lbDbSize = new DevExpress.XtraEditors.LabelControl();
            this.btnDeleteLog = new DevExpress.XtraEditors.SimpleButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnExportLogs = new DevExpress.XtraEditors.SimpleButton();
            this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.gvAuditLogs = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRecordId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSourceName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAffectedField = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOriginalValue = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNewValue = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colModule = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colReason = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTimestamp = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUserName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridAuditLogs = new DevExpress.XtraGrid.GridControl();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            ((System.ComponentModel.ISupportInitialize)(this.tsAuditLogsEnabled.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deLogTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deLogTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbLogModule.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbLogSource.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deLogFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deLogFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvAuditLogs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridAuditLogs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsAuditLogsEnabled
            // 
            this.tsAuditLogsEnabled.Location = new System.Drawing.Point(30, 29);
            this.tsAuditLogsEnabled.Name = "tsAuditLogsEnabled";
            this.tsAuditLogsEnabled.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.tsAuditLogsEnabled.Properties.Appearance.Options.UseFont = true;
            this.tsAuditLogsEnabled.Properties.AutoHeight = false;
            this.tsAuditLogsEnabled.Properties.LookAndFeel.SkinName = "The Bezier";
            this.tsAuditLogsEnabled.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.tsAuditLogsEnabled.Properties.OffText = "Audit Log Disabled";
            this.tsAuditLogsEnabled.Properties.OnText = "Audit Log Enabled";
            this.tsAuditLogsEnabled.Size = new System.Drawing.Size(193, 24);
            this.tsAuditLogsEnabled.TabIndex = 86;
            this.tsAuditLogsEnabled.Toggled += new System.EventHandler(this.tsAuditLogsEnabled_Toggled);
            // 
            // deLogTo
            // 
            this.deLogTo.EditValue = null;
            this.deLogTo.Location = new System.Drawing.Point(455, 44);
            this.deLogTo.Name = "deLogTo";
            this.deLogTo.Properties.AutoHeight = false;
            this.deLogTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.deLogTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.deLogTo.Size = new System.Drawing.Size(83, 28);
            this.deLogTo.TabIndex = 79;
            // 
            // cbLogModule
            // 
            this.cbLogModule.Location = new System.Drawing.Point(190, 45);
            this.cbLogModule.Name = "cbLogModule";
            this.cbLogModule.Properties.AutoHeight = false;
            this.cbLogModule.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbLogModule.Size = new System.Drawing.Size(155, 28);
            this.cbLogModule.TabIndex = 82;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.Appearance.Options.UseFont = true;
            this.btnRefresh.Appearance.Options.UseTextOptions = true;
            this.btnRefresh.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnRefresh.AppearanceHovered.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.AppearanceHovered.Options.UseFont = true;
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.ImageOptions.SvgImage = global::WinPure.CleanAndMatch.Properties.Resources._2026_Refresh_16;
            this.btnRefresh.Location = new System.Drawing.Point(556, 44);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(84, 29);
            this.btnRefresh.TabIndex = 73;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // cbLogSource
            // 
            this.cbLogSource.Location = new System.Drawing.Point(25, 45);
            this.cbLogSource.Name = "cbLogSource";
            this.cbLogSource.Properties.AutoHeight = false;
            this.cbLogSource.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbLogSource.Properties.PopupWidthMode = DevExpress.XtraEditors.PopupWidthMode.ContentWidth;
            this.cbLogSource.Size = new System.Drawing.Size(155, 28);
            this.cbLogSource.TabIndex = 77;
            // 
            // lbSourceName
            // 
            this.lbSourceName.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSourceName.Appearance.Options.UseFont = true;
            this.lbSourceName.Location = new System.Drawing.Point(29, 24);
            this.lbSourceName.Name = "lbSourceName";
            this.lbSourceName.Size = new System.Drawing.Size(41, 17);
            this.lbSourceName.TabIndex = 76;
            this.lbSourceName.Text = "Source";
            // 
            // deLogFrom
            // 
            this.deLogFrom.EditValue = null;
            this.deLogFrom.Location = new System.Drawing.Point(357, 44);
            this.deLogFrom.Name = "deLogFrom";
            this.deLogFrom.Properties.AutoHeight = false;
            this.deLogFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.deLogFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.deLogFrom.Size = new System.Drawing.Size(83, 28);
            this.deLogFrom.TabIndex = 78;
            // 
            // lbDateTo
            // 
            this.lbDateTo.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.lbDateTo.Appearance.Options.UseFont = true;
            this.lbDateTo.Location = new System.Drawing.Point(455, 24);
            this.lbDateTo.Name = "lbDateTo";
            this.lbDateTo.Size = new System.Drawing.Size(47, 17);
            this.lbDateTo.TabIndex = 75;
            this.lbDateTo.Text = "Date To";
            // 
            // lbModulName
            // 
            this.lbModulName.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.lbModulName.Appearance.Options.UseFont = true;
            this.lbModulName.Location = new System.Drawing.Point(194, 24);
            this.lbModulName.Name = "lbModulName";
            this.lbModulName.Size = new System.Drawing.Size(46, 17);
            this.lbModulName.TabIndex = 81;
            this.lbModulName.Text = "Module";
            // 
            // lbDateFrom
            // 
            this.lbDateFrom.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.lbDateFrom.Appearance.Options.UseFont = true;
            this.lbDateFrom.Location = new System.Drawing.Point(357, 24);
            this.lbDateFrom.Name = "lbDateFrom";
            this.lbDateFrom.Size = new System.Drawing.Size(64, 17);
            this.lbDateFrom.TabIndex = 74;
            this.lbDateFrom.Text = "Date From";
            // 
            // lbDbSize
            // 
            this.lbDbSize.Appearance.BackColor = System.Drawing.SystemColors.Highlight;
            this.lbDbSize.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.lbDbSize.Appearance.ForeColor = System.Drawing.Color.White;
            this.lbDbSize.Appearance.Options.UseBackColor = true;
            this.lbDbSize.Appearance.Options.UseFont = true;
            this.lbDbSize.Appearance.Options.UseForeColor = true;
            this.lbDbSize.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.lbDbSize.Location = new System.Drawing.Point(30, 57);
            this.lbDbSize.Name = "lbDbSize";
            this.lbDbSize.Size = new System.Drawing.Size(81, 19);
            this.lbDbSize.TabIndex = 83;
            this.lbDbSize.Text = "Log File size: ";
            // 
            // btnDeleteLog
            // 
            this.btnDeleteLog.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnDeleteLog.Appearance.Options.UseFont = true;
            this.btnDeleteLog.Appearance.Options.UseTextOptions = true;
            this.btnDeleteLog.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnDeleteLog.AppearanceHovered.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnDeleteLog.AppearanceHovered.Options.UseFont = true;
            this.btnDeleteLog.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDeleteLog.ImageOptions.SvgImage = global::WinPure.CleanAndMatch.Properties.Resources._2026_Delete_Receipt_32;
            this.btnDeleteLog.Location = new System.Drawing.Point(890, 65);
            this.btnDeleteLog.Name = "btnDeleteLog";
            this.btnDeleteLog.Size = new System.Drawing.Size(90, 36);
            this.btnDeleteLog.TabIndex = 80;
            this.btnDeleteLog.Text = "Delete";
            this.btnDeleteLog.Click += new System.EventHandler(this.brnDeleteLog_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(992, 27);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(15, 19);
            this.pictureBox1.TabIndex = 71;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Tag = "Cam";
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // btnExportLogs
            // 
            this.btnExportLogs.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnExportLogs.Appearance.Options.UseFont = true;
            this.btnExportLogs.Appearance.Options.UseTextOptions = true;
            this.btnExportLogs.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnExportLogs.AppearanceHovered.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnExportLogs.AppearanceHovered.Options.UseFont = true;
            this.btnExportLogs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportLogs.ImageOptions.SvgImage = global::WinPure.CleanAndMatch.Properties.Resources._2026_PDF_32;
            this.btnExportLogs.Location = new System.Drawing.Point(890, 23);
            this.btnExportLogs.Name = "btnExportLogs";
            this.btnExportLogs.Size = new System.Drawing.Size(90, 36);
            this.btnExportLogs.TabIndex = 0;
            this.btnExportLogs.Text = "Export";
            this.btnExportLogs.Click += new System.EventHandler(this.btnExportLogs_Click);
            // 
            // repositoryItemMemoEdit1
            // 
            this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
            // 
            // gvAuditLogs
            // 
            this.gvAuditLogs.Appearance.FooterPanel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.gvAuditLogs.Appearance.FooterPanel.Options.UseFont = true;
            this.gvAuditLogs.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvAuditLogs.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvAuditLogs.Appearance.Row.Options.UseTextOptions = true;
            this.gvAuditLogs.Appearance.Row.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gvAuditLogs.AppearancePrint.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gvAuditLogs.AppearancePrint.HeaderPanel.Options.UseFont = true;
            this.gvAuditLogs.AppearancePrint.Row.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.gvAuditLogs.AppearancePrint.Row.Options.UseFont = true;
            this.gvAuditLogs.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colId,
            this.colRecordId,
            this.colSourceName,
            this.colAffectedField,
            this.colOriginalValue,
            this.colNewValue,
            this.colModule,
            this.colReason,
            this.colTimestamp,
            this.colUserName});
            this.gvAuditLogs.GridControl = this.gridAuditLogs;
            this.gvAuditLogs.Name = "gvAuditLogs";
            this.gvAuditLogs.OptionsBehavior.Editable = false;
            this.gvAuditLogs.OptionsView.RowAutoHeight = true;
            this.gvAuditLogs.OptionsView.ShowFooter = true;
            this.gvAuditLogs.OptionsView.ShowGroupPanel = false;
            // 
            // colId
            // 
            this.colId.AppearanceCell.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.colId.AppearanceCell.Options.UseFont = true;
            this.colId.Caption = "ID";
            this.colId.FieldName = "Id";
            this.colId.MaxWidth = 80;
            this.colId.MinWidth = 80;
            this.colId.Name = "colId";
            this.colId.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, "Id", "Count = {0}")});
            this.colId.Width = 80;
            // 
            // colRecordId
            // 
            this.colRecordId.AppearanceCell.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.colRecordId.AppearanceCell.Options.UseFont = true;
            this.colRecordId.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colRecordId.AppearanceHeader.Options.UseFont = true;
            this.colRecordId.Caption = "Record ID";
            this.colRecordId.FieldName = "RecordId";
            this.colRecordId.MinWidth = 70;
            this.colRecordId.Name = "colRecordId";
            this.colRecordId.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, "RecordId", "Count = {0}")});
            this.colRecordId.Visible = true;
            this.colRecordId.VisibleIndex = 0;
            this.colRecordId.Width = 70;
            // 
            // colSourceName
            // 
            this.colSourceName.AppearanceCell.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.colSourceName.AppearanceCell.Options.UseFont = true;
            this.colSourceName.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.colSourceName.AppearanceHeader.Options.UseFont = true;
            this.colSourceName.Caption = "Source Name";
            this.colSourceName.FieldName = "SourceName";
            this.colSourceName.MinWidth = 100;
            this.colSourceName.Name = "colSourceName";
            this.colSourceName.Visible = true;
            this.colSourceName.VisibleIndex = 1;
            this.colSourceName.Width = 240;
            // 
            // colAffectedField
            // 
            this.colAffectedField.AppearanceCell.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.colAffectedField.AppearanceCell.Options.UseFont = true;
            this.colAffectedField.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.colAffectedField.AppearanceHeader.Options.UseFont = true;
            this.colAffectedField.Caption = "Affected Field";
            this.colAffectedField.FieldName = "AffectedField";
            this.colAffectedField.MinWidth = 150;
            this.colAffectedField.Name = "colAffectedField";
            this.colAffectedField.Visible = true;
            this.colAffectedField.VisibleIndex = 2;
            this.colAffectedField.Width = 150;
            // 
            // colOriginalValue
            // 
            this.colOriginalValue.AppearanceCell.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.colOriginalValue.AppearanceCell.Options.UseFont = true;
            this.colOriginalValue.AppearanceCell.Options.UseTextOptions = true;
            this.colOriginalValue.AppearanceCell.TextOptions.Trimming = DevExpress.Utils.Trimming.None;
            this.colOriginalValue.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.colOriginalValue.AppearanceHeader.Options.UseFont = true;
            this.colOriginalValue.Caption = "Original Value";
            this.colOriginalValue.ColumnEdit = this.repositoryItemMemoEdit1;
            this.colOriginalValue.FieldName = "OriginalValue";
            this.colOriginalValue.MinWidth = 300;
            this.colOriginalValue.Name = "colOriginalValue";
            this.colOriginalValue.Visible = true;
            this.colOriginalValue.VisibleIndex = 3;
            this.colOriginalValue.Width = 300;
            // 
            // colNewValue
            // 
            this.colNewValue.AppearanceCell.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.colNewValue.AppearanceCell.Options.UseFont = true;
            this.colNewValue.AppearanceCell.Options.UseTextOptions = true;
            this.colNewValue.AppearanceCell.TextOptions.Trimming = DevExpress.Utils.Trimming.None;
            this.colNewValue.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.colNewValue.AppearanceHeader.Options.UseFont = true;
            this.colNewValue.Caption = "New Value";
            this.colNewValue.ColumnEdit = this.repositoryItemMemoEdit1;
            this.colNewValue.FieldName = "NewValue";
            this.colNewValue.MinWidth = 300;
            this.colNewValue.Name = "colNewValue";
            this.colNewValue.Visible = true;
            this.colNewValue.VisibleIndex = 4;
            this.colNewValue.Width = 300;
            // 
            // colModule
            // 
            this.colModule.AppearanceCell.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.colModule.AppearanceCell.Options.UseFont = true;
            this.colModule.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.colModule.AppearanceHeader.Options.UseFont = true;
            this.colModule.Caption = "Module";
            this.colModule.FieldName = "Module";
            this.colModule.MaxWidth = 100;
            this.colModule.MinWidth = 100;
            this.colModule.Name = "colModule";
            this.colModule.Visible = true;
            this.colModule.VisibleIndex = 5;
            this.colModule.Width = 100;
            // 
            // colReason
            // 
            this.colReason.AppearanceCell.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.colReason.AppearanceCell.Options.UseFont = true;
            this.colReason.AppearanceCell.Options.UseTextOptions = true;
            this.colReason.AppearanceCell.TextOptions.Trimming = DevExpress.Utils.Trimming.None;
            this.colReason.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.colReason.AppearanceHeader.Options.UseFont = true;
            this.colReason.Caption = "Reason";
            this.colReason.ColumnEdit = this.repositoryItemMemoEdit1;
            this.colReason.FieldName = "Reason";
            this.colReason.MinWidth = 150;
            this.colReason.Name = "colReason";
            this.colReason.Visible = true;
            this.colReason.VisibleIndex = 6;
            this.colReason.Width = 150;
            // 
            // colTimestamp
            // 
            this.colTimestamp.AppearanceCell.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.colTimestamp.AppearanceCell.Options.UseFont = true;
            this.colTimestamp.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.colTimestamp.AppearanceHeader.Options.UseFont = true;
            this.colTimestamp.Caption = "Timestamp";
            this.colTimestamp.DisplayFormat.FormatString = "G";
            this.colTimestamp.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colTimestamp.FieldName = "Timestamp";
            this.colTimestamp.MaxWidth = 110;
            this.colTimestamp.MinWidth = 110;
            this.colTimestamp.Name = "colTimestamp";
            this.colTimestamp.Visible = true;
            this.colTimestamp.VisibleIndex = 7;
            this.colTimestamp.Width = 110;
            // 
            // colUserName
            // 
            this.colUserName.AppearanceCell.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.colUserName.AppearanceCell.Options.UseFont = true;
            this.colUserName.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.colUserName.AppearanceHeader.Options.UseFont = true;
            this.colUserName.Caption = "User Name";
            this.colUserName.FieldName = "UserName";
            this.colUserName.MaxWidth = 150;
            this.colUserName.MinWidth = 100;
            this.colUserName.Name = "colUserName";
            this.colUserName.Visible = true;
            this.colUserName.VisibleIndex = 8;
            this.colUserName.Width = 110;
            // 
            // gridAuditLogs
            // 
            this.gridAuditLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridAuditLogs.Location = new System.Drawing.Point(0, 126);
            this.gridAuditLogs.MainView = this.gvAuditLogs;
            this.gridAuditLogs.Name = "gridAuditLogs";
            this.gridAuditLogs.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemMemoEdit1});
            this.gridAuditLogs.Size = new System.Drawing.Size(1504, 366);
            this.gridAuditLogs.TabIndex = 2;
            this.gridAuditLogs.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvAuditLogs});
            // 
            // groupControl2
            // 
            this.groupControl2.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupControl2.AppearanceCaption.Options.UseFont = true;
            this.groupControl2.Controls.Add(this.deLogTo);
            this.groupControl2.Controls.Add(this.cbLogSource);
            this.groupControl2.Controls.Add(this.lbDateTo);
            this.groupControl2.Controls.Add(this.cbLogModule);
            this.groupControl2.Controls.Add(this.deLogFrom);
            this.groupControl2.Controls.Add(this.lbModulName);
            this.groupControl2.Controls.Add(this.btnRefresh);
            this.groupControl2.Controls.Add(this.lbSourceName);
            this.groupControl2.Controls.Add(this.lbDateFrom);
            this.groupControl2.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupControl2.Location = new System.Drawing.Point(221, 14);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(663, 94);
            this.groupControl2.TabIndex = 3;
            this.groupControl2.Text = "Filter";
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.groupControl2);
            this.groupControl1.Controls.Add(this.tsAuditLogsEnabled);
            this.groupControl1.Controls.Add(this.btnExportLogs);
            this.groupControl1.Controls.Add(this.lbDbSize);
            this.groupControl1.Controls.Add(this.pictureBox1);
            this.groupControl1.Controls.Add(this.btnDeleteLog);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupControl1.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(1504, 126);
            this.groupControl1.TabIndex = 3;
            this.groupControl1.Text = "Configuration";
            // 
            // UCAuditLogs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridAuditLogs);
            this.Controls.Add(this.groupControl1);
            this.Name = "UCAuditLogs";
            this.Size = new System.Drawing.Size(1504, 492);
            ((System.ComponentModel.ISupportInitialize)(this.tsAuditLogsEnabled.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deLogTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deLogTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbLogModule.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbLogSource.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deLogFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deLogFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvAuditLogs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridAuditLogs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            this.groupControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private PictureBox pictureBox1;
        private SimpleButton btnExportLogs;
        private SimpleButton btnRefresh;
        private SimpleButton btnDeleteLog;
        private LabelControl lbDbSize;
        private CheckedComboBoxEdit cbLogSource;
        private CheckedComboBoxEdit cbLogModule;
        private DateEdit deLogTo;
        private DateEdit deLogFrom;
        private LabelControl lbModulName;
        private LabelControl lbDateTo;
        private LabelControl lbSourceName;
        private LabelControl lbDateFrom;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit1;
        private GridView gvAuditLogs;
        private GridColumn colId;
        private GridColumn colSourceName;
        private GridColumn colRecordId;
        private GridColumn colAffectedField;
        private GridColumn colOriginalValue;
        private GridColumn colNewValue;
        private GridColumn colModule;
        private GridColumn colReason;
        private GridColumn colTimestamp;
        private GridControl gridAuditLogs;
        private GridColumn colUserName;
        private ToggleSwitch tsAuditLogsEnabled;
        private GroupControl groupControl2;
        private GroupControl groupControl1;
    }
}
