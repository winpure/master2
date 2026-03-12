using WinPure.Automation.Models;
using WinPure.Configuration.Enums;

namespace WinPure.CleanAndMatch.Automation;

public partial class frmAutomationScheduling : XtraForm
{
    private AutomationSchedule _schedule;
    private int _configurationId;
    private bool _isNew = true;
    internal AutomationSchedule Schedule
    {
        get
        {
            UpdateSchedule();
            return _schedule;
        }
        set
        {
            _schedule = value;
            _isNew = false;
            _configurationId = _schedule.ConfigurationId;
            txtName.Text = _schedule.Name;
            dtStartDate.DateTime = _schedule.StartDate.Date;
            dtStartTime.Time = _schedule.WeeklyTime;
            cbActiv.Checked = _schedule.IsActive;
            cbStopOnFail.Checked = _schedule.StopOnFail;
            switch (_schedule.ScheduleType)
            {
                case AutomationScheduleType.Once:
                    cbType.SelectedIndex = 0;
                    break;
                case AutomationScheduleType.Hourly:
                    seFrequency.Value = _schedule.Frequency ?? 0;
                    cbType.SelectedIndex = 1;
                    break;
                case AutomationScheduleType.Daily:
                    seFrequency.Value = _schedule.Frequency ?? 0;
                    cbType.SelectedIndex = 2;
                    break;
                case AutomationScheduleType.Weekly:
                    cbType.SelectedIndex = 3;
                    var days = _schedule.DayOfWeek.Split(' ');
                    cbMonday.Checked = days.Contains("Mo");
                    cbTuesday.Checked = days.Contains("Tu");
                    cbWednesday.Checked = days.Contains("We");
                    cbThursday.Checked = days.Contains("Th");
                    cbFriday.Checked = days.Contains("Fr");
                    cbSaturday.Checked = days.Contains("Sa");
                    cbSunday.Checked = days.Contains("Su");
                    break;
                case AutomationScheduleType.Monthly:
                    cbType.SelectedIndex = 4;
                    cbDayOfMonth.SelectedIndex = _schedule.DayOfMonth.Value - 1;
                    cbDayOfMonth.Refresh();
                    break;
            }
            cbType.Refresh();
        }
    }
    public frmAutomationScheduling()
    {
        InitializeComponent();
        _schedule = new AutomationSchedule();
        cbType.SelectedIndex = 0;
        cbType.Refresh();
        navFrameScheduling.Visible = false;
        for (int i = 1; i <= 31; i++)
        {
            cbDayOfMonth.Properties.Items.Add(i.ToString());
        }
        cbDayOfMonth.SelectedIndex = 0;
        cbDayOfMonth.Refresh();
        dtStartDate.DateTime = DateTime.Now;
        dtStartTime.Time = DateTime.Now;
    }

    public DialogResult ShowScheduling(int configurationId)
    {
        if (_isNew) _configurationId = configurationId;
        return ShowDialog();
    }

    private void UpdateSchedule()
    {
        _schedule.ConfigurationId = _configurationId;
        _schedule.Name = txtName.Text;
        _schedule.ScheduleType = (AutomationScheduleType)Enum.Parse(typeof(AutomationScheduleType), cbType.Text);
        _schedule.StartDate = dtStartDate.DateTime.Date;
        _schedule.IsActive = cbActiv.Checked;
        _schedule.StopOnFail = cbStopOnFail.Checked;

        _schedule.DayOfMonth = 0;
        _schedule.DayOfWeek = "";
        _schedule.Frequency = 0;
        _schedule.WeeklyTime = dtStartTime.Time;

        switch (cbType.Text)
        {
            case "Once":
                break;
            case "Hourly":
            case "Daily":
                _schedule.Frequency = Convert.ToInt32(seFrequency.Value);
                break;
            case "Weekly":
                var wd = "";
                if (cbMonday.Checked) wd += "Mo ";
                if (cbTuesday.Checked) wd += "Tu ";
                if (cbWednesday.Checked) wd += "We ";
                if (cbThursday.Checked) wd += "Th ";
                if (cbFriday.Checked) wd += "Fr ";
                if (cbSaturday.Checked) wd += "Sa ";
                if (cbSunday.Checked) wd += "Su ";
                _schedule.DayOfWeek = wd.Trim();
                break;
            case "Monthly":
                _schedule.DayOfMonth = Convert.ToInt16(cbDayOfMonth.Text);
                break;
        }
    }

    private void cbType_SelectedIndexChanged(object sender, EventArgs e)
    {
        navFrameScheduling.Visible = true;
        switch (cbType.Text)
        {
            case "Once":
                navFrameScheduling.Visible = false;
                break;
            case "Hourly":
                navFrameScheduling.SelectedPage = navPageHourly;
                lnFrequency.Text = "Hours";
                break;
            case "Daily":
                navFrameScheduling.SelectedPage = navPageHourly;
                lnFrequency.Text = "Days";
                break;
            case "Weekly":
                navFrameScheduling.SelectedPage = navPageWeekly;
                break;
            case "Monthly":
                navFrameScheduling.SelectedPage = navPageMonthly;
                break;
        }
    }
}