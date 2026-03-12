namespace WinPure.CleanAndMatch.Integration.Import
{
    partial class frmImportText
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
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions1 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImportText));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.txtFieldDelim = new DevExpress.XtraEditors.TextEdit();
            this.radioOther = new DevExpress.XtraEditors.CheckEdit();
            this.radioSpace = new DevExpress.XtraEditors.CheckEdit();
            this.radioComma = new DevExpress.XtraEditors.CheckEdit();
            this.radioSemicolon = new DevExpress.XtraEditors.CheckEdit();
            this.radioTab = new DevExpress.XtraEditors.CheckEdit();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.chkFirstRow = new DevExpress.XtraEditors.CheckEdit();
            this.lblCodePage = new System.Windows.Forms.Label();
            this.lblTextQ = new System.Windows.Forms.Label();
            this.cmbCodePage = new DevExpress.XtraEditors.LookUpEdit();
            this.cmbTextQ = new DevExpress.XtraEditors.ComboBoxEdit();
            this.txtTimeDelim = new DevExpress.XtraEditors.TextEdit();
            this.txtDateDelim = new DevExpress.XtraEditors.TextEdit();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.txtDecimalSymbol = new DevExpress.XtraEditors.TextEdit();
            this.lblDecimalSymbol = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.lblDateDelim = new System.Windows.Forms.Label();
            this.lblTimeDelim = new System.Windows.Forms.Label();
            this.cmbDateOrder = new DevExpress.XtraEditors.ComboBoxEdit();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.btnFilePath = new DevExpress.XtraEditors.ButtonEdit();
            this.pnlSettings = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtFieldDelim.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioOther.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioSpace.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioComma.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioSemicolon.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioTab.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkFirstRow.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCodePage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbTextQ.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimeDelim.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDateDelim.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDecimalSymbol.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDateOrder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnFilePath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSettings)).BeginInit();
            this.pnlSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.groupControl1.AppearanceCaption.Options.UseFont = true;
            this.groupControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupControl1.Controls.Add(this.txtFieldDelim);
            this.groupControl1.Controls.Add(this.radioOther);
            this.groupControl1.Controls.Add(this.radioSpace);
            this.groupControl1.Controls.Add(this.radioComma);
            this.groupControl1.Controls.Add(this.radioSemicolon);
            this.groupControl1.Controls.Add(this.radioTab);
            this.groupControl1.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupControl1.Location = new System.Drawing.Point(15, 105);
            this.groupControl1.Margin = new System.Windows.Forms.Padding(8, 8, 8, 11);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Padding = new System.Windows.Forms.Padding(5);
            this.groupControl1.Size = new System.Drawing.Size(565, 64);
            this.groupControl1.TabIndex = 1;
            this.groupControl1.Text = "Field Delimiter";
            // 
            // txtFieldDelim
            // 
            this.txtFieldDelim.EditValue = ",";
            this.txtFieldDelim.Location = new System.Drawing.Point(507, 27);
            this.txtFieldDelim.Name = "txtFieldDelim";
            this.txtFieldDelim.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.txtFieldDelim.Properties.Appearance.Options.UseFont = true;
            this.txtFieldDelim.Properties.MaxLength = 1;
            this.txtFieldDelim.Properties.ReadOnly = true;
            this.txtFieldDelim.Size = new System.Drawing.Size(35, 28);
            this.txtFieldDelim.TabIndex = 5;
            this.txtFieldDelim.TextChanged += new System.EventHandler(this.TryToPreview);
            // 
            // radioOther
            // 
            this.radioOther.Location = new System.Drawing.Point(415, 27);
            this.radioOther.Name = "radioOther";
            this.radioOther.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioOther.Properties.Appearance.Options.UseFont = true;
            this.radioOther.Properties.Caption = "Other";
            this.radioOther.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.Radio;
            this.radioOther.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.radioOther.Properties.RadioGroupIndex = 1;
            this.radioOther.Size = new System.Drawing.Size(70, 20);
            this.radioOther.TabIndex = 4;
            this.radioOther.TabStop = false;
            this.radioOther.CheckedChanged += new System.EventHandler(this.radioTab_CheckedChange);
            // 
            // radioSpace
            // 
            this.radioSpace.Location = new System.Drawing.Point(316, 27);
            this.radioSpace.Name = "radioSpace";
            this.radioSpace.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioSpace.Properties.Appearance.Options.UseFont = true;
            this.radioSpace.Properties.Caption = "Space";
            this.radioSpace.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.Radio;
            this.radioSpace.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.radioSpace.Properties.RadioGroupIndex = 1;
            this.radioSpace.Size = new System.Drawing.Size(64, 20);
            this.radioSpace.TabIndex = 3;
            this.radioSpace.TabStop = false;
            this.radioSpace.CheckedChanged += new System.EventHandler(this.radioTab_CheckedChange);
            // 
            // radioComma
            // 
            this.radioComma.EditValue = true;
            this.radioComma.Location = new System.Drawing.Point(217, 27);
            this.radioComma.Name = "radioComma";
            this.radioComma.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioComma.Properties.Appearance.Options.UseFont = true;
            this.radioComma.Properties.Caption = "Comma";
            this.radioComma.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.Radio;
            this.radioComma.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.radioComma.Properties.RadioGroupIndex = 1;
            this.radioComma.Size = new System.Drawing.Size(70, 20);
            this.radioComma.TabIndex = 2;
            this.radioComma.CheckedChanged += new System.EventHandler(this.radioTab_CheckedChange);
            // 
            // radioSemicolon
            // 
            this.radioSemicolon.Location = new System.Drawing.Point(102, 27);
            this.radioSemicolon.Name = "radioSemicolon";
            this.radioSemicolon.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioSemicolon.Properties.Appearance.Options.UseFont = true;
            this.radioSemicolon.Properties.Caption = "Semicolon";
            this.radioSemicolon.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.Radio;
            this.radioSemicolon.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.radioSemicolon.Properties.RadioGroupIndex = 1;
            this.radioSemicolon.Size = new System.Drawing.Size(85, 20);
            this.radioSemicolon.TabIndex = 1;
            this.radioSemicolon.TabStop = false;
            this.radioSemicolon.CheckedChanged += new System.EventHandler(this.radioTab_CheckedChange);
            // 
            // radioTab
            // 
            this.radioTab.Location = new System.Drawing.Point(21, 27);
            this.radioTab.Name = "radioTab";
            this.radioTab.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioTab.Properties.Appearance.Options.UseFont = true;
            this.radioTab.Properties.Caption = "Tab";
            this.radioTab.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.Radio;
            this.radioTab.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.radioTab.Properties.RadioGroupIndex = 1;
            this.radioTab.Size = new System.Drawing.Size(46, 20);
            this.radioTab.TabIndex = 0;
            this.radioTab.TabStop = false;
            this.radioTab.CheckedChanged += new System.EventHandler(this.radioTab_CheckedChange);
            // 
            // panelControl1
            // 
            this.panelControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelControl1.Controls.Add(this.chkFirstRow);
            this.panelControl1.Controls.Add(this.lblCodePage);
            this.panelControl1.Controls.Add(this.lblTextQ);
            this.panelControl1.Controls.Add(this.cmbCodePage);
            this.panelControl1.Controls.Add(this.cmbTextQ);
            this.panelControl1.Location = new System.Drawing.Point(15, 179);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(8, 11, 8, 8);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(5);
            this.panelControl1.Size = new System.Drawing.Size(565, 77);
            this.panelControl1.TabIndex = 2;
            // 
            // chkFirstRow
            // 
            this.chkFirstRow.EditValue = true;
            this.chkFirstRow.Location = new System.Drawing.Point(11, 9);
            this.chkFirstRow.Name = "chkFirstRow";
            this.chkFirstRow.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkFirstRow.Properties.Appearance.Options.UseFont = true;
            this.chkFirstRow.Properties.Caption = "First row contains column names";
            this.chkFirstRow.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.chkFirstRow.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.chkFirstRow.Size = new System.Drawing.Size(211, 20);
            this.chkFirstRow.TabIndex = 0;
            this.chkFirstRow.CheckedChanged += new System.EventHandler(this.TryToPreview);
            // 
            // lblCodePage
            // 
            this.lblCodePage.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblCodePage.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblCodePage.Location = new System.Drawing.Point(165, 40);
            this.lblCodePage.Name = "lblCodePage";
            this.lblCodePage.Size = new System.Drawing.Size(57, 16);
            this.lblCodePage.TabIndex = 123;
            this.lblCodePage.Text = "Code Page";
            // 
            // lblTextQ
            // 
            this.lblTextQ.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblTextQ.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblTextQ.Location = new System.Drawing.Point(18, 41);
            this.lblTextQ.Name = "lblTextQ";
            this.lblTextQ.Size = new System.Drawing.Size(72, 16);
            this.lblTextQ.TabIndex = 122;
            this.lblTextQ.Text = "Text Qualifier";
            // 
            // cmbCodePage
            // 
            this.cmbCodePage.Location = new System.Drawing.Point(228, 34);
            this.cmbCodePage.Name = "cmbCodePage";
            this.cmbCodePage.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbCodePage.Properties.Appearance.Options.UseFont = true;
            this.cmbCodePage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbCodePage.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Text", "Encoding")});
            this.cmbCodePage.Properties.DisplayMember = "Text";
            this.cmbCodePage.Properties.NullText = "";
            this.cmbCodePage.Properties.PopupSizeable = false;
            this.cmbCodePage.Properties.ValueMember = "Id";
            this.cmbCodePage.Size = new System.Drawing.Size(240, 26);
            this.cmbCodePage.TabIndex = 2;
            // 
            // cmbTextQ
            // 
            this.cmbTextQ.Location = new System.Drawing.Point(96, 34);
            this.cmbTextQ.Name = "cmbTextQ";
            this.cmbTextQ.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.cmbTextQ.Properties.Appearance.Options.UseFont = true;
            this.cmbTextQ.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbTextQ.Properties.Items.AddRange(new object[] {
            "",
            "\"",
            "\'"});
            this.cmbTextQ.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbTextQ.Size = new System.Drawing.Size(49, 26);
            this.cmbTextQ.TabIndex = 1;
            // 
            // txtTimeDelim
            // 
            this.txtTimeDelim.EditValue = ":";
            this.txtTimeDelim.Location = new System.Drawing.Point(390, 12);
            this.txtTimeDelim.Name = "txtTimeDelim";
            this.txtTimeDelim.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Black", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTimeDelim.Properties.Appearance.Options.UseFont = true;
            this.txtTimeDelim.Size = new System.Drawing.Size(20, 28);
            this.txtTimeDelim.TabIndex = 124;
            this.txtTimeDelim.Tag = "";
            this.txtTimeDelim.TextChanged += new System.EventHandler(this.TryToPreview);
            // 
            // txtDateDelim
            // 
            this.txtDateDelim.EditValue = "/";
            this.txtDateDelim.Location = new System.Drawing.Point(264, 12);
            this.txtDateDelim.Name = "txtDateDelim";
            this.txtDateDelim.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Black", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDateDelim.Properties.Appearance.Options.UseFont = true;
            this.txtDateDelim.Size = new System.Drawing.Size(20, 28);
            this.txtDateDelim.TabIndex = 124;
            this.txtDateDelim.TextChanged += new System.EventHandler(this.TryToPreview);
            // 
            // panelControl2
            // 
            this.panelControl2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelControl2.Controls.Add(this.txtTimeDelim);
            this.panelControl2.Controls.Add(this.txtDecimalSymbol);
            this.panelControl2.Controls.Add(this.lblDecimalSymbol);
            this.panelControl2.Controls.Add(this.txtDateDelim);
            this.panelControl2.Controls.Add(this.Label2);
            this.panelControl2.Controls.Add(this.lblDateDelim);
            this.panelControl2.Controls.Add(this.lblTimeDelim);
            this.panelControl2.Controls.Add(this.cmbDateOrder);
            this.panelControl2.Location = new System.Drawing.Point(15, 267);
            this.panelControl2.Margin = new System.Windows.Forms.Padding(8);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Padding = new System.Windows.Forms.Padding(5);
            this.panelControl2.Size = new System.Drawing.Size(565, 59);
            this.panelControl2.TabIndex = 3;
            // 
            // txtDecimalSymbol
            // 
            this.txtDecimalSymbol.EditValue = ".";
            this.txtDecimalSymbol.Location = new System.Drawing.Point(512, 12);
            this.txtDecimalSymbol.Name = "txtDecimalSymbol";
            this.txtDecimalSymbol.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Black", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDecimalSymbol.Properties.Appearance.Options.UseFont = true;
            this.txtDecimalSymbol.Size = new System.Drawing.Size(20, 28);
            this.txtDecimalSymbol.TabIndex = 125;
            this.txtDecimalSymbol.Tag = "";
            this.txtDecimalSymbol.TextChanged += new System.EventHandler(this.TryToPreview);
            // 
            // lblDecimalSymbol
            // 
            this.lblDecimalSymbol.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblDecimalSymbol.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblDecimalSymbol.Location = new System.Drawing.Point(416, 17);
            this.lblDecimalSymbol.Name = "lblDecimalSymbol";
            this.lblDecimalSymbol.Size = new System.Drawing.Size(100, 16);
            this.lblDecimalSymbol.TabIndex = 76;
            this.lblDecimalSymbol.Text = "Decimal Symbol";
            // 
            // Label2
            // 
            this.Label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.Label2.Location = new System.Drawing.Point(8, 17);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(59, 19);
            this.Label2.TabIndex = 73;
            this.Label2.Text = "Date Order";
            // 
            // lblDateDelim
            // 
            this.lblDateDelim.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblDateDelim.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblDateDelim.Location = new System.Drawing.Point(175, 17);
            this.lblDateDelim.Name = "lblDateDelim";
            this.lblDateDelim.Size = new System.Drawing.Size(83, 19);
            this.lblDateDelim.TabIndex = 74;
            this.lblDateDelim.Text = "Date Delimiter";
            // 
            // lblTimeDelim
            // 
            this.lblTimeDelim.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblTimeDelim.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblTimeDelim.Location = new System.Drawing.Point(298, 17);
            this.lblTimeDelim.Name = "lblTimeDelim";
            this.lblTimeDelim.Size = new System.Drawing.Size(86, 19);
            this.lblTimeDelim.TabIndex = 75;
            this.lblTimeDelim.Text = "Time Delimiter";
            // 
            // cmbDateOrder
            // 
            this.cmbDateOrder.Location = new System.Drawing.Point(70, 12);
            this.cmbDateOrder.Name = "cmbDateOrder";
            this.cmbDateOrder.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbDateOrder.Properties.Appearance.Options.UseFont = true;
            this.cmbDateOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbDateOrder.Properties.Items.AddRange(new object[] {
            "DMY",
            "MDY",
            "YMD",
            "YDM"});
            this.cmbDateOrder.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbDateOrder.Size = new System.Drawing.Size(86, 26);
            this.cmbDateOrder.TabIndex = 0;
            // 
            // groupControl2
            // 
            this.groupControl2.AppearanceCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.groupControl2.AppearanceCaption.Options.UseFont = true;
            this.groupControl2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupControl2.Controls.Add(this.btnFilePath);
            this.groupControl2.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupControl2.Location = new System.Drawing.Point(15, 8);
            this.groupControl2.Margin = new System.Windows.Forms.Padding(8, 8, 8, 11);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Padding = new System.Windows.Forms.Padding(5);
            this.groupControl2.Size = new System.Drawing.Size(565, 90);
            this.groupControl2.TabIndex = 0;
            this.groupControl2.Text = "File";
            // 
            // btnFilePath
            // 
            this.btnFilePath.Location = new System.Drawing.Point(21, 31);
            this.btnFilePath.Name = "btnFilePath";
            this.btnFilePath.Properties.Appearance.Font = new System.Drawing.Font("Open Sans Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilePath.Properties.Appearance.Options.UseFont = true;
            editorButtonImageOptions1.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("editorButtonImageOptions1.SvgImage")));
            this.btnFilePath.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions1, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.btnFilePath.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnFilePath.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.btnFilePath.Size = new System.Drawing.Size(533, 36);
            this.btnFilePath.TabIndex = 0;
            this.btnFilePath.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnFilePath_ButtonClick);
            this.btnFilePath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnFilePath_KeyDown);
            // 
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.groupControl2);
            this.pnlSettings.Controls.Add(this.groupControl1);
            this.pnlSettings.Controls.Add(this.panelControl1);
            this.pnlSettings.Controls.Add(this.panelControl2);
            this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSettings.Location = new System.Drawing.Point(5, 29);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Padding = new System.Windows.Forms.Padding(5);
            this.pnlSettings.Size = new System.Drawing.Size(599, 330);
            this.pnlSettings.TabIndex = 112;
            // 
            // frmImportText
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(609, 676);
            this.Controls.Add(this.pnlSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("frmImportText.IconOptions.Icon")));
            this.IconOptions.ShowIcon = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImportText";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import from text file";
            this.Controls.SetChildIndex(this.pnlSettings, 0);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtFieldDelim.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioOther.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioSpace.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioComma.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioSemicolon.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioTab.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkFirstRow.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCodePage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbTextQ.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimeDelim.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDateDelim.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtDecimalSymbol.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDateOrder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnFilePath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSettings)).EndInit();
            this.pnlSettings.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.TextEdit txtFieldDelim;
        private DevExpress.XtraEditors.CheckEdit radioOther;
        private DevExpress.XtraEditors.CheckEdit radioSpace;
        private DevExpress.XtraEditors.CheckEdit radioComma;
        private DevExpress.XtraEditors.CheckEdit radioSemicolon;
        private DevExpress.XtraEditors.CheckEdit radioTab;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.CheckEdit chkFirstRow;
        internal System.Windows.Forms.Label lblCodePage;
        internal System.Windows.Forms.Label lblTextQ;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        internal System.Windows.Forms.Label lblDecimalSymbol;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Label lblDateDelim;
        internal System.Windows.Forms.Label lblTimeDelim;
        private DevExpress.XtraEditors.LookUpEdit cmbCodePage;
        private DevExpress.XtraEditors.ComboBoxEdit cmbTextQ;
        private DevExpress.XtraEditors.ComboBoxEdit cmbDateOrder;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.ButtonEdit btnFilePath;
        private PanelControl pnlSettings;
        private TextEdit txtDateDelim;
        private TextEdit txtTimeDelim;
        private TextEdit txtDecimalSymbol;
    }
}