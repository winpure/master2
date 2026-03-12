using WinPure.Automation.Models;
using WinPure.Automation.Services;

namespace WinPure.CleanAndMatch.Controls;

public partial class UCAutomation : UserControl
{
    public event Action<string, string, MessagesType, Exception> OnException;
    private IAutomationService _service;
    private ILicenseService _licenseService;
    public UCAutomation()
    {
        InitializeComponent();
        Localization();
    }

    #region localization
    private void Localization()
    {
        btnAddConfiguration.Text = Resources.UI_ADD;
        btnRemoveConfiguration.Text = Resources.UI_REMOVE;
        btnEditConfiguration.Text = Resources.UI_EDIT;
        btnAddSchedule.Text = Resources.UI_ADD;
        btnRemoveSchedule.Text = Resources.UI_REMOVE;
        btnEditSchedule.Text = Resources.UI_EDIT;
        btnExportLogs.Text = Resources.UI_EXPORT;
        btnRefresh.Text = Resources.UI_REFRESH;
    }
    #endregion

    public void Initialize()
    {
        _service = WinPureUiDependencyResolver.Resolve<IAutomationService>();
        _service.OnAutomationListUpdated += _service_OnAutomationListUpdated;
        _service.OnException += _service_OnException;
        _licenseService = WinPureUiDependencyResolver.Resolve<ILicenseService>();
        SetControlAvailability();
        _licenseService.LicenseLoaded += LicenseService_LicenseLoaded;
        var configurationService = WinPureUiDependencyResolver.Resolve<IConfigurationService>();
        tsAutomationEnabled.IsOn = configurationService.Configuration.EnableAutomation;
    }

    private void LicenseService_LicenseLoaded()
    {
        SetControlAvailability();
    }

    private void SetControlAvailability()
    {
        if (_licenseService.IsDemo || !ProgramTypeHelper.AutomationPrograms.Contains(Program.CurrentProgramVersion))
        {
            //navFrameAutomation.Enabled = false;
            btnAddConfiguration.Enabled = false;
            btnRemoveConfiguration.Enabled = false;
            btnEditConfiguration.Enabled = false;
            btnAddSchedule.Enabled = false;
            btnRemoveSchedule.Enabled = false;
            btnEditSchedule.Enabled = false;
            tsAutomationEnabled.Enabled = false;
            btnRefresh.Enabled = false;
            btnExportLogs.Enabled = false;
        }
        else
        {
            btnAddConfiguration.Enabled = true;
            btnRemoveConfiguration.Enabled = true;
            btnEditConfiguration.Enabled = true;
            btnAddSchedule.Enabled = true;
            btnRemoveSchedule.Enabled = true;
            btnEditSchedule.Enabled = true;
            tsAutomationEnabled.Enabled = true;
            btnRefresh.Enabled = true;
            btnExportLogs.Enabled = true;

            if (cbAllSchedules.Checked)
            {
                gridScheduling.DataSource = null;
                gridScheduling.DataSource = _service.GetSchedules();
                gridScheduling.Refresh();
            }
        }
    }

    private void _service_OnException(string arg1, string arg2, MessagesType arg3, Exception arg4)
    {
        OnException?.Invoke(arg1, arg2, arg3, arg4);
    }

    private void _service_OnAutomationListUpdated()
    {
        RefreshAutomation();
    }

    internal void ShowSubPanel(AutomationDataType dType)
    {
        switch (dType)
        {
            case AutomationDataType.Configuration:
                navFrameAutomation.SelectedPage = navPageConfiguration;
                RefreshAutomation();
                break;
            case AutomationDataType.Log:
                navFrameAutomation.SelectedPage = navPageLogs;
                RefreshLog();
                break;
        }
    }

    private void RefreshLog()
    {
        gridAutomationLogs.DataSource = null;
        gridAutomationLogs.DataSource = _service.GetAutomationLogs();
        gridAutomationLogs.Refresh();
    }

    private void RefreshAutomation()
    {
        gridAutomationConfiguration.DataSource = null;
        gridAutomationConfiguration.DataSource = _service.GetAutomationHeaders();
        gridAutomationConfiguration.Refresh();
    }

    private void btnAddConfiguration_Click(object sender, EventArgs e)
    {
        var master = WinPureUiDependencyResolver.Resolve<frmAutomationConfig>();
        if (master.ShowDialog() == DialogResult.OK)
        {
            _service.AddAutomation(master.Configuration);
        }
    }

    private void btnRemoveConfiguration_Click(object sender, EventArgs e)
    {
        if (!gvAutomationConfiguration.GetSelectedRows().Any()) return;
        var rwId = gvAutomationConfiguration.GetSelectedRows().First();
        if (gvAutomationConfiguration.GetRow(rwId) is AutomationHeader automationHeader)
        {
            if (MessageBox.Show("Are you sure to delete that configuration?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _service.DeleteAutomation(automationHeader.Id);
                UpdateSchedules();
            }
        }
    }

    private void btnEditConfiguration_Click(object sender, EventArgs e)
    {
        if (!gvAutomationConfiguration.GetSelectedRows().Any())
        {
            return;
        }
        var rwId = gvAutomationConfiguration.GetSelectedRows().First();
        if (gvAutomationConfiguration.GetRow(rwId) is AutomationHeader automationHeader)
        {
            var conf = _service.GetAutomation(automationHeader.Id);
            if (conf == null)
            {
                return;
            }
            var master = WinPureUiDependencyResolver.Resolve<frmAutomationConfig>();
            master.Configuration = conf;
            if (master.ShowDialog() == DialogResult.OK)
            {
                _service.UpdateAutomation(master.Configuration);
            }

        }
    }

    private void gvAutomationConfiguration_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
    {
        if (e.Column.FieldName == "IsActive")
        {
            if (gvAutomationConfiguration.GetRow(e.RowHandle) is AutomationHeader automationHeader)
            {
                automationHeader.IsActive = Convert.ToBoolean(e.Value);
                _service.SetAutomationActiveState(automationHeader.Id, automationHeader.IsActive);
            }
        }
    }

    private void gvAutomationConfiguration_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
    {
        if (cbAllSchedules.Checked)
        {
            return;
        }

        if (gvAutomationConfiguration.GetRow(e.FocusedRowHandle) is AutomationHeader automationHeader)
        {
            gridScheduling.DataSource = null;
            gridScheduling.DataSource = _service.GetSchedules(automationHeader.Id);
            gridScheduling.Refresh();
        }
    }

    private void UpdateSchedules()
    {
        gridScheduling.DataSource = null;
        if (cbAllSchedules.Checked)
        {
            gridScheduling.DataSource = _service.GetSchedules();
        }
        else
        {
            if (!gvAutomationConfiguration.GetSelectedRows().Any()) return;
            if (gvAutomationConfiguration.GetRow(gvAutomationConfiguration.GetSelectedRows()[0]) is AutomationHeader automationHeader)
            {
                gridScheduling.DataSource = _service.GetSchedules(automationHeader.Id);
            }
        }
        gridScheduling.Refresh();
    }

    private void cbAllSchedules_CheckedChanged(object sender, EventArgs e)
    {
        UpdateSchedules();
    }

    private void btnAddSchedule_Click(object sender, EventArgs e)
    {
        if (!gvAutomationConfiguration.GetSelectedRows().Any()) return;
        if (gvAutomationConfiguration.GetRow(gvAutomationConfiguration.GetSelectedRows()[0]) is AutomationHeader automationHeader)
        {
            var frm = WinPureUiDependencyResolver.Resolve<frmAutomationScheduling>();
            if (frm.ShowScheduling(automationHeader.Id) == DialogResult.OK)
            {
                _service.AddAutomationSchedule(frm.Schedule);
                UpdateSchedules();
            }
        }
    }

    private void btnRemoveSchedule_Click(object sender, EventArgs e)
    {
        if (!gvScheduling.GetSelectedRows().Any())
        {
            return;
        }
        if (gvScheduling.GetRow(gvScheduling.GetSelectedRows()[0]) is AutomationSchedule rw)
        {
            var id = Convert.ToInt32(rw.Id);
            _service.DeleteSchedule(id);
            UpdateSchedules();
        }
    }

    private void btnEditSchedule_Click(object sender, EventArgs e)
    {
        if (!gvScheduling.GetSelectedRows().Any())
        {
            return;
        }
        if (gvScheduling.GetRow(gvScheduling.GetSelectedRows()[0]) is AutomationSchedule rw)
        {
            var frm = WinPureUiDependencyResolver.Resolve<frmAutomationScheduling>();
            frm.Schedule = rw;
            if (frm.ShowScheduling(rw.ConfigurationId) == DialogResult.OK)
            {
                _service.UpdateAutomationSchedule(frm.Schedule);
                UpdateSchedules();
            }
        }
    }

    private void gvScheduling_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
    {
        if (e.Column.FieldName == "IsActive")
        {
            if (gvScheduling.GetRow(e.RowHandle) is AutomationSchedule rw)
            {
                rw.IsActive = Convert.ToBoolean(e.Value);
                _service.UpdateAutomationSchedule(rw);
            }
        }
        else if (e.Column.FieldName == "StopOnFail")
        {
            if (gvScheduling.GetRow(e.RowHandle) is AutomationSchedule rw)
            {
                rw.StopOnFail = Convert.ToBoolean(e.Value);
                _service.UpdateAutomationSchedule(rw);
            }
        }
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.Automation);
    }

    private void tsAutomationEnabled_Toggled(object sender, EventArgs e)
    {
        var configurationService = WinPureUiDependencyResolver.Resolve<IConfigurationService>();
        var configuration = configurationService.Configuration;

        if (tsAutomationEnabled.IsOn != configuration.EnableAutomation)
        {
            if (!SystemHelper.IsAdministrator())
            {
                MessageBox.Show(Resources.MESSAGE_OPTIONONLYFORADMIN, Resources.MESSAGE_MESSAGEDIALOG_WARNING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                tsAutomationEnabled.IsOn = configurationService.Configuration.EnableAutomation;
                return;
            }

            SystemHelper.SetAutomationWindowsService(tsAutomationEnabled.IsOn); //TODO message if the service is not installed, but that should not happen as we always install it with the main installer
            configuration.EnableAutomation = tsAutomationEnabled.IsOn;
            configurationService.SaveConfiguration();
        }
    }

    private void btnRefresh_Click(object sender, EventArgs e)
    {
        RefreshLog();
    }

    private void btnExportLogs_Click(object sender, EventArgs e)
    {
        var dlgSaveCsvFile = new SaveFileDialog
        {
            FileName = "AutomationLogs",
            AddExtension = true,
            Filter = "Microsoft Excel files (*.xlsx, *.xls)|*.xlsx;*.xls",
            Title = "Export data to Microsoft Excel file"
        };

        if (dlgSaveCsvFile.ShowDialog() == DialogResult.OK)
        {
            gvAutomationLogs.ExportToXlsx(dlgSaveCsvFile.FileName);
        }
    }
}