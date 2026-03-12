namespace WinPure.CleanAndMatch.Automation
{
    partial class frmAutomationScheduling
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAutomationScheduling));
            this.lbName = new DevExpress.XtraEditors.LabelControl();
            this.txtName = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.cbActiv = new DevExpress.XtraEditors.CheckEdit();
            this.cbType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.dtStartDate = new DevExpress.XtraEditors.DateEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.navFrameScheduling = new DevExpress.XtraBars.Navigation.NavigationFrame();
            this.navPageWeekly = new DevExpress.XtraBars.Navigation.NavigationPage();
            this.cbSunday = new DevExpress.XtraEditors.CheckEdit();
            this.cbSaturday = new DevExpress.XtraEditors.CheckEdit();
            this.cbFriday = new DevExpress.XtraEditors.CheckEdit();
            this.cbThursday = new DevExpress.XtraEditors.CheckEdit();
            this.cbWednesday = new DevExpress.XtraEditors.CheckEdit();
            this.cbTuesday = new DevExpress.XtraEditors.CheckEdit();
            this.cbMonday = new DevExpress.XtraEditors.CheckEdit();
            this.navPageHourly = new DevExpress.XtraBars.Navigation.NavigationPage();
            this.seFrequency = new DevExpress.XtraEditors.SpinEdit();
            this.lnFrequency = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.navPageMonthly = new DevExpress.XtraBars.Navigation.NavigationPage();
            this.cbDayOfMonth = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.dtStartTime = new DevExpress.XtraEditors.TimeEdit();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.cbStopOnFail = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbActiv.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.navFrameScheduling)).BeginInit();
            this.navFrameScheduling.SuspendLayout();
            this.navPageWeekly.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbSunday.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbSaturday.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbFriday.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbThursday.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbWednesday.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTuesday.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbMonday.Properties)).BeginInit();
            this.navPageHourly.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seFrequency.Properties)).BeginInit();
            this.navPageMonthly.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbDayOfMonth.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbStopOnFail.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lbName
            // 
            this.lbName.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lbName.Appearance.Options.UseFont = true;
            this.lbName.Location = new System.Drawing.Point(28, 19);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(35, 15);
            this.lbName.TabIndex = 0;
            this.lbName.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(105, 16);
            this.txtName.Name = "txtName";
            this.txtName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtName.Properties.Appearance.Options.UseFont = true;
            this.txtName.Size = new System.Drawing.Size(234, 24);
            this.txtName.TabIndex = 1;
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(28, 119);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(54, 15);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "Start Date:";
            // 
            // cbActiv
            // 
            this.cbActiv.EditValue = true;
            this.cbActiv.Location = new System.Drawing.Point(105, 88);
            this.cbActiv.Name = "cbActiv";
            this.cbActiv.Properties.Caption = "";
            this.cbActiv.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbActiv.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbActiv.Size = new System.Drawing.Size(32, 20);
            this.cbActiv.TabIndex = 3;
            // 
            // cbType
            // 
            this.cbType.Location = new System.Drawing.Point(105, 53);
            this.cbType.Name = "cbType";
            this.cbType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cbType.Properties.Appearance.Options.UseFont = true;
            this.cbType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbType.Properties.Items.AddRange(new object[] {
            "Once",
            "Hourly",
            "Daily",
            "Weekly",
            "Monthly"});
            this.cbType.Size = new System.Drawing.Size(234, 24);
            this.cbType.TabIndex = 4;
            this.cbType.SelectedIndexChanged += new System.EventHandler(this.cbType_SelectedIndexChanged);
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(28, 56);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(29, 15);
            this.labelControl1.TabIndex = 5;
            this.labelControl1.Text = "Type:";
            // 
            // dtStartDate
            // 
            this.dtStartDate.EditValue = null;
            this.dtStartDate.Location = new System.Drawing.Point(105, 115);
            this.dtStartDate.Name = "dtStartDate";
            this.dtStartDate.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtStartDate.Properties.Appearance.Options.UseFont = true;
            this.dtStartDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtStartDate.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.False;
            this.dtStartDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtStartDate.Properties.CalendarTimeProperties.MaskSettings.Set("mask", "");
            this.dtStartDate.Properties.MaskSettings.Set("mask", "d");
            this.dtStartDate.Properties.MaskSettings.Set("culture", "");
            this.dtStartDate.Properties.MaskSettings.Set("useAdvancingCaret", null);
            this.dtStartDate.Properties.UseMaskAsDisplayFormat = true;
            this.dtStartDate.Size = new System.Drawing.Size(234, 24);
            this.dtStartDate.TabIndex = 6;
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(28, 92);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(36, 15);
            this.labelControl3.TabIndex = 7;
            this.labelControl3.Text = "Active:";
            // 
            // navFrameScheduling
            // 
            this.navFrameScheduling.AllowTransitionAnimation = DevExpress.Utils.DefaultBoolean.False;
            this.navFrameScheduling.Controls.Add(this.navPageWeekly);
            this.navFrameScheduling.Controls.Add(this.navPageHourly);
            this.navFrameScheduling.Controls.Add(this.navPageMonthly);
            this.navFrameScheduling.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.navFrameScheduling.Location = new System.Drawing.Point(0, 183);
            this.navFrameScheduling.Name = "navFrameScheduling";
            this.navFrameScheduling.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] {
            this.navPageHourly,
            this.navPageWeekly,
            this.navPageMonthly});
            this.navFrameScheduling.SelectedPage = this.navPageWeekly;
            this.navFrameScheduling.Size = new System.Drawing.Size(428, 89);
            this.navFrameScheduling.TabIndex = 8;
            this.navFrameScheduling.Text = "navFrameScheduling";
            this.navFrameScheduling.TransitionAnimationProperties.FrameCount = 100;
            // 
            // navPageWeekly
            // 
            this.navPageWeekly.Controls.Add(this.cbSunday);
            this.navPageWeekly.Controls.Add(this.cbSaturday);
            this.navPageWeekly.Controls.Add(this.cbFriday);
            this.navPageWeekly.Controls.Add(this.cbThursday);
            this.navPageWeekly.Controls.Add(this.cbWednesday);
            this.navPageWeekly.Controls.Add(this.cbTuesday);
            this.navPageWeekly.Controls.Add(this.cbMonday);
            this.navPageWeekly.Name = "navPageWeekly";
            this.navPageWeekly.Size = new System.Drawing.Size(428, 89);
            // 
            // cbSunday
            // 
            this.cbSunday.Location = new System.Drawing.Point(233, 51);
            this.cbSunday.Name = "cbSunday";
            this.cbSunday.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cbSunday.Properties.Appearance.Options.UseFont = true;
            this.cbSunday.Properties.Caption = "Sunday";
            this.cbSunday.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbSunday.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbSunday.Size = new System.Drawing.Size(75, 20);
            this.cbSunday.TabIndex = 7;
            // 
            // cbSaturday
            // 
            this.cbSaturday.Location = new System.Drawing.Point(133, 51);
            this.cbSaturday.Name = "cbSaturday";
            this.cbSaturday.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cbSaturday.Properties.Appearance.Options.UseFont = true;
            this.cbSaturday.Properties.Caption = "Saturday";
            this.cbSaturday.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbSaturday.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbSaturday.Size = new System.Drawing.Size(75, 20);
            this.cbSaturday.TabIndex = 6;
            // 
            // cbFriday
            // 
            this.cbFriday.Location = new System.Drawing.Point(27, 51);
            this.cbFriday.Name = "cbFriday";
            this.cbFriday.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cbFriday.Properties.Appearance.Options.UseFont = true;
            this.cbFriday.Properties.Caption = "Friday";
            this.cbFriday.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbFriday.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbFriday.Size = new System.Drawing.Size(75, 20);
            this.cbFriday.TabIndex = 5;
            // 
            // cbThursday
            // 
            this.cbThursday.Location = new System.Drawing.Point(323, 17);
            this.cbThursday.Name = "cbThursday";
            this.cbThursday.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cbThursday.Properties.Appearance.Options.UseFont = true;
            this.cbThursday.Properties.Caption = "Thursday";
            this.cbThursday.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbThursday.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbThursday.Size = new System.Drawing.Size(75, 20);
            this.cbThursday.TabIndex = 4;
            // 
            // cbWednesday
            // 
            this.cbWednesday.Location = new System.Drawing.Point(233, 17);
            this.cbWednesday.Name = "cbWednesday";
            this.cbWednesday.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cbWednesday.Properties.Appearance.Options.UseFont = true;
            this.cbWednesday.Properties.Caption = "Wednesday";
            this.cbWednesday.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbWednesday.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbWednesday.Size = new System.Drawing.Size(84, 20);
            this.cbWednesday.TabIndex = 3;
            // 
            // cbTuesday
            // 
            this.cbTuesday.Location = new System.Drawing.Point(133, 17);
            this.cbTuesday.Name = "cbTuesday";
            this.cbTuesday.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cbTuesday.Properties.Appearance.Options.UseFont = true;
            this.cbTuesday.Properties.Caption = "Tuesday";
            this.cbTuesday.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbTuesday.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbTuesday.Size = new System.Drawing.Size(75, 20);
            this.cbTuesday.TabIndex = 2;
            // 
            // cbMonday
            // 
            this.cbMonday.Location = new System.Drawing.Point(27, 17);
            this.cbMonday.Name = "cbMonday";
            this.cbMonday.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cbMonday.Properties.Appearance.Options.UseFont = true;
            this.cbMonday.Properties.Caption = "Monday";
            this.cbMonday.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbMonday.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbMonday.Size = new System.Drawing.Size(75, 20);
            this.cbMonday.TabIndex = 1;
            // 
            // navPageHourly
            // 
            this.navPageHourly.Caption = "navPageHourly";
            this.navPageHourly.Controls.Add(this.seFrequency);
            this.navPageHourly.Controls.Add(this.lnFrequency);
            this.navPageHourly.Controls.Add(this.labelControl4);
            this.navPageHourly.Name = "navPageHourly";
            this.navPageHourly.Size = new System.Drawing.Size(428, 89);
            // 
            // seFrequency
            // 
            this.seFrequency.EditValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.seFrequency.Location = new System.Drawing.Point(105, 17);
            this.seFrequency.Name = "seFrequency";
            this.seFrequency.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.seFrequency.Properties.Appearance.Options.UseFont = true;
            this.seFrequency.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.seFrequency.Properties.IsFloatValue = false;
            this.seFrequency.Properties.MaskSettings.Set("mask", "N00");
            this.seFrequency.Properties.MaxValue = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.seFrequency.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.seFrequency.Size = new System.Drawing.Size(107, 24);
            this.seFrequency.TabIndex = 2;
            // 
            // lnFrequency
            // 
            this.lnFrequency.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lnFrequency.Appearance.Options.UseFont = true;
            this.lnFrequency.Location = new System.Drawing.Point(233, 20);
            this.lnFrequency.Name = "lnFrequency";
            this.lnFrequency.Size = new System.Drawing.Size(71, 15);
            this.lnFrequency.TabIndex = 1;
            this.lnFrequency.Text = "labelControl5";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(28, 20);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(63, 13);
            this.labelControl4.TabIndex = 0;
            this.labelControl4.Text = "Repeat each";
            // 
            // navPageMonthly
            // 
            this.navPageMonthly.Controls.Add(this.cbDayOfMonth);
            this.navPageMonthly.Controls.Add(this.labelControl7);
            this.navPageMonthly.Name = "navPageMonthly";
            this.navPageMonthly.Size = new System.Drawing.Size(428, 89);
            // 
            // cbDayOfMonth
            // 
            this.cbDayOfMonth.Location = new System.Drawing.Point(105, 16);
            this.cbDayOfMonth.Name = "cbDayOfMonth";
            this.cbDayOfMonth.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cbDayOfMonth.Properties.Appearance.Options.UseFont = true;
            this.cbDayOfMonth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbDayOfMonth.Size = new System.Drawing.Size(100, 24);
            this.cbDayOfMonth.TabIndex = 12;
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(28, 19);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(70, 13);
            this.labelControl7.TabIndex = 11;
            this.labelControl7.Text = "Day of month";
            // 
            // labelControl5
            // 
            this.labelControl5.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelControl5.Appearance.Options.UseFont = true;
            this.labelControl5.Location = new System.Drawing.Point(28, 153);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(57, 15);
            this.labelControl5.TabIndex = 8;
            this.labelControl5.Text = "Start Time:";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnCancel);
            this.panelControl1.Controls.Add(this.btnOK);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 272);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(428, 53);
            this.panelControl1.TabIndex = 9;
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnCancel.ImageOptions.SvgImage")));
            this.btnCancel.Location = new System.Drawing.Point(229, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 31);
            this.btnCancel.TabIndex = 63;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOK.Appearance.Options.UseFont = true;
            this.btnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnOK.ImageOptions.SvgImage")));
            this.btnOK.Location = new System.Drawing.Point(105, 11);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(110, 31);
            this.btnOK.TabIndex = 62;
            this.btnOK.Text = "OK";
            // 
            // dtStartTime
            // 
            this.dtStartTime.EditValue = null;
            this.dtStartTime.Location = new System.Drawing.Point(105, 150);
            this.dtStartTime.Name = "dtStartTime";
            this.dtStartTime.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtStartTime.Properties.Appearance.Options.UseFont = true;
            this.dtStartTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtStartTime.Properties.DisplayFormat.FormatString = "HH:mm";
            this.dtStartTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtStartTime.Properties.EditFormat.FormatString = "HH:mm";
            this.dtStartTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtStartTime.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.dtStartTime.Properties.MaskSettings.Set("mask", "HH:mm");
            this.dtStartTime.Size = new System.Drawing.Size(234, 24);
            this.dtStartTime.TabIndex = 10;
            // 
            // labelControl6
            // 
            this.labelControl6.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelControl6.Appearance.Options.UseFont = true;
            this.labelControl6.Location = new System.Drawing.Point(152, 92);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(63, 15);
            this.labelControl6.TabIndex = 12;
            this.labelControl6.Text = "Stop on fail:";
            // 
            // cbStopOnFail
            // 
            this.cbStopOnFail.EditValue = true;
            this.cbStopOnFail.Location = new System.Drawing.Point(248, 88);
            this.cbStopOnFail.Name = "cbStopOnFail";
            this.cbStopOnFail.Properties.Caption = "";
            this.cbStopOnFail.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.cbStopOnFail.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
            this.cbStopOnFail.Size = new System.Drawing.Size(39, 20);
            this.cbStopOnFail.TabIndex = 11;
            // 
            // frmAutomationScheduling
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(428, 325);
            this.Controls.Add(this.labelControl6);
            this.Controls.Add(this.cbStopOnFail);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.navFrameScheduling);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.dtStartDate);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.cbType);
            this.Controls.Add(this.cbActiv);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.dtStartTime);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IconOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("frmAutomationScheduling.IconOptions.SvgImage")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAutomationScheduling";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Scheduling";
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbActiv.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.navFrameScheduling)).EndInit();
            this.navFrameScheduling.ResumeLayout(false);
            this.navPageWeekly.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cbSunday.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbSaturday.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbFriday.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbThursday.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbWednesday.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTuesday.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbMonday.Properties)).EndInit();
            this.navPageHourly.ResumeLayout(false);
            this.navPageHourly.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seFrequency.Properties)).EndInit();
            this.navPageMonthly.ResumeLayout(false);
            this.navPageMonthly.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbDayOfMonth.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbStopOnFail.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lbName;
        private DevExpress.XtraEditors.TextEdit txtName;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.CheckEdit cbActiv;
        private DevExpress.XtraEditors.ComboBoxEdit cbType;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.DateEdit dtStartDate;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraBars.Navigation.NavigationFrame navFrameScheduling;
        private DevExpress.XtraBars.Navigation.NavigationPage navPageHourly;
        private DevExpress.XtraBars.Navigation.NavigationPage navPageWeekly;
        private DevExpress.XtraBars.Navigation.NavigationPage navPageMonthly;
        private DevExpress.XtraEditors.SpinEdit seFrequency;
        private DevExpress.XtraEditors.LabelControl lnFrequency;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.CheckEdit cbSunday;
        private DevExpress.XtraEditors.CheckEdit cbSaturday;
        private DevExpress.XtraEditors.CheckEdit cbFriday;
        private DevExpress.XtraEditors.CheckEdit cbThursday;
        private DevExpress.XtraEditors.CheckEdit cbWednesday;
        private DevExpress.XtraEditors.CheckEdit cbTuesday;
        private DevExpress.XtraEditors.CheckEdit cbMonday;
        private DevExpress.XtraEditors.ComboBoxEdit cbDayOfMonth;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.TimeEdit dtStartTime;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.CheckEdit cbStopOnFail;
    }
}