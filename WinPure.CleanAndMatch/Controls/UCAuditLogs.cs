using WinPure.CleanAndMatch.Reports;
using WinPure.DataService.AuditLogs;

namespace WinPure.CleanAndMatch.Controls;

public partial class UCAuditLogs : UserControl
{
    private IAuditLogService _service;
    private IProjectService _projectService;
    private ILicenseService _licenseService;
    private Dictionary<string, AuditLogModule> modules;

    public event Action<string, string, MessagesType, Exception> OnException;
    public event Action<string, Task, bool, CancellationTokenSource> OnProgressShow;

    public UCAuditLogs()
    {
        InitializeComponent();
        Localization();
    }

    private void Localization()
    {
        btnExportLogs.Text = Resources.UI_EXPORT;
        tsAuditLogsEnabled.Properties.OnText = Resources.UI_AUDITLOG_ENABLED;
        tsAuditLogsEnabled.Properties.OffText = Resources.UI_AUDITLOG_DISABLED;
        btnRefresh.Text = Resources.UI_REFRESH;
        btnDeleteLog.Text = Resources.UI_DELETE;
    }

    public void Initialize()
    {
        _service = WinPureUiDependencyResolver.Resolve<IAuditLogService>();
        var configurationService = WinPureUiDependencyResolver.Resolve<IConfigurationService>();
        tsAuditLogsEnabled.IsOn = configurationService.Configuration.EnableAuditLogs;
        _projectService = WinPureUiDependencyResolver.Resolve<IProjectService>();
        _projectService.OnCleanedProject += CleanProject;
        _licenseService = WinPureUiDependencyResolver.Resolve<ILicenseService>();
        _licenseService.LicenseLoaded += LicenseService_LicenseLoaded;
        modules = EnumExtension.GetDisplayNameDictionary<AuditLogModule>();
        SetControlAvailability();
        foreach (var module in modules)
        {
            cbLogModule.Properties.Items.Add(module.Key, CheckState.Checked);
        }
    }

    public void LoadLogs()
    {
        cbLogSource.Properties.Items.Clear();
        var sources = _service.GetSources();
        sources.ForEach(x => cbLogSource.Properties.Items.Add(x, CheckState.Checked));
        cbLogSource.Height = cbLogSource.CalcMinHeight() + 20;

        var dbSize = _service.GetDatabaseSize();
        lbDbSize.Text = $"{Resources.UI_CAPTION_DBSIZE}: {dbSize}";
        lbDbSize.BackColor = SetDbFileSizeColor(dbSize);
    }

    public void RefreshLogs()
    {
        var selectedSources = cbLogSource.Properties.Items
            .Where(x => x.CheckState == CheckState.Checked)
            .Select(x => x.Value.ToString())
            .ToList();

        var selectedModules = cbLogModule.Properties.Items
            .Where(x => x.CheckState == CheckState.Checked)
            .Select(x => modules[x.Value.ToString()])
            .ToList();

        if (selectedSources.Count > 0)
        {
            var whereString = _service.GetFilterString(selectedSources, selectedModules, (DateTime?)deLogFrom.EditValue, (DateTime?)deLogTo.EditValue, string.Empty);
            var dataSource = NameHelper.AuditLogTable.GetLogData(whereString);
            gridAuditLogs.DataSource = dataSource;
            gridAuditLogs.RefreshDataSource();
            gvAuditLogs.BestFitColumns();
        }
    }

    public void CloseCurrentData()
    {
        DisposeConnection();
    }

    private void SetControlAvailability()
    {
        if (!_licenseService.IsDemo && _licenseService.IsAuditLogEnabled())
        {
            groupControl2.Enabled = true;
            btnDeleteLog.Enabled = true;
            btnExportLogs.Enabled = true;
            tsAuditLogsEnabled.Enabled = true;
            gridAuditLogs.Enabled = true;

        }
        else
        {
            groupControl2.Enabled = false;
            btnDeleteLog.Enabled = false;
            btnExportLogs.Enabled = false;
            tsAuditLogsEnabled.Enabled = false;
            gridAuditLogs.Enabled = false;
        }
    }

    private void CleanProject()
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(CleanProject));
            return;
        }

        gridAuditLogs.DataSource = null;
        gridAuditLogs.RefreshDataSource();
    }

    private void tsAuditLogsEnabled_Toggled(object sender, EventArgs e)
    {
        var configurationService = WinPureUiDependencyResolver.Resolve<IConfigurationService>();
        var configuration = configurationService.Configuration;

        if (tsAuditLogsEnabled.IsOn != configuration.EnableAuditLogs)
        {
            if (!SystemHelper.IsAdministrator())
            {
                MessageBox.Show(Resources.MESSAGE_LOGOPTION_ONLYFORADMIN, Resources.MESSAGE_MESSAGEDIALOG_WARNING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                tsAuditLogsEnabled.IsOn = configurationService.Configuration.EnableAuditLogs;
                return;
            }

            configuration.EnableAuditLogs = tsAuditLogsEnabled.IsOn;
            configurationService.SaveConfiguration();
        }
    }

    private void btnFilter_Click(object sender, EventArgs e)
    {
        RefreshLogs();
    }

    private void brnDeleteLog_Click(object sender, EventArgs e)
    {
        if (!SystemHelper.IsAdministrator())
        {
            MessageBox.Show(Resources.MESSAGE_ONLYFORADMIN, Resources.MESSAGE_MESSAGEDIALOG_WARNING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }

        var selectedSources = cbLogSource.Properties.Items
            .Where(x => x.CheckState == CheckState.Checked)
            .Select(x => x.Value.ToString())
            .ToList();

        var selectedModules = cbLogModule.Properties.Items
            .Where(x => x.CheckState == CheckState.Checked)
            .Select(x => modules[x.Value.ToString()])
            .ToList();

        if (selectedSources.Count == 0 || selectedModules.Count == 0)
            return;

        if (MessageBox.Show(Resources.MESSAGE_REMOVELOGS, Resources.MESSAGE_QUESTION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        DisposeConnection();
        gridAuditLogs.DataSource = null;
        gridAuditLogs.RefreshDataSource();

        _service.ClearLogs(selectedSources, selectedModules, (DateTime?)deLogFrom.EditValue, (DateTime?)deLogTo.EditValue);
        LoadLogs();
    }

    private void btnExportLogs_Click(object sender, EventArgs e)
    {
        if (!SystemHelper.IsAdministrator())
        {
            MessageBox.Show(Resources.MESSAGE_ONLYFORADMIN, Resources.MESSAGE_MESSAGEDIALOG_WARNING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }

        var selectedSources = cbLogSource.Properties.Items
            .Where(x => x.CheckState == CheckState.Checked)
            .Select(x => x.Value.ToString())
            .ToList();

        var selectedModules = cbLogModule.Properties.Items
            .Where(x => x.CheckState == CheckState.Checked)
            .Select(x => modules[x.Value.ToString()])
            .ToList();

        if (selectedSources.Count == 0 || selectedModules.Count == 0)
        {
            OnException?.Invoke(Resources.UI_NO_LOGS_TO_EXPORT, Resources.MESSAGE_MESSAGEDIALOG_WARNING_CAPTION, MessagesType.Warning, null);
            return;
        }

        var dlgSavePdfFile = new SaveFileDialog
        {
            FileName = "",
            AddExtension = true,
            Filter = "PDF files (*.pdf)|*.pdf",
            Title = "Export logs to PDF file"
        };

        if (dlgSavePdfFile.ShowDialog() != DialogResult.OK)
            return;

        var configurationService = WinPureUiDependencyResolver.Resolve<IConfigurationService>();
        var report = new AuditLogReport();
        var reportPath = configurationService.Configuration.AuditLogReportPath;

        if (!File.Exists(reportPath))
        {
            OnException?.Invoke(string.Format(Resources.EXCEPTION_REPORT_FILE_NOT_EXISTS, reportPath), "", MessagesType.Error, null);
            return;
        }

        var whereCause = DataHelper.GetSqliteFilterCriteriaFromGridView(gvAuditLogs);
        var cancellationTokenSource = new CancellationTokenSource();
        var task = new Task(() =>
        {
            var data = _service.GetAuditLogs(selectedSources, selectedModules, (DateTime?)deLogFrom.EditValue, (DateTime?)deLogTo.EditValue, whereCause);
            var reportData = new AuditLogDataReport
            {
                Common = new CommonData()
                {
                    LogEnabled = configurationService.Configuration.EnableAuditLogs,
                    NumberOfRecords = data.Count,
                    StartDate = ((DateTime?)deLogFrom.EditValue)?.Date ?? data.Min(x => x.Timestamp).Date,
                    EndDate = ((DateTime?)deLogTo.EditValue)?.Date ?? data.Max(x => x.Timestamp).Date,

                },
                Logs = data
            };

            report.LoadLayout(reportPath);
            report.DataSource = reportData;
            report.DataMember = nameof(AuditLogDataReport.Logs);
            report.CreateDocument();
            report.ExportToPdf(dlgSavePdfFile.FileName);
        });

        OnProgressShow?.Invoke(string.Format(Resources.UI_EXPORTTOFILE, "PDF"), task, false, cancellationTokenSource);
    }

    private void DisposeConnection()
    {
        XpoCollectionHelper.DisposeWithOwner(gridAuditLogs.DataSource);
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.AuditLogs);
    }
    private void LicenseService_LicenseLoaded()
    {
        SetControlAvailability();
    }

    private Color SetDbFileSizeColor(string dbSize)
    {
        if (!string.IsNullOrEmpty(dbSize) && dbSize.Contains("Gb"))
        {
            if (Decimal.TryParse(dbSize.Replace("Gb", ""), out var size))
            {
                if (size >= 5)
                    return Color.OrangeRed;
                if (size >= 1)
                    return Color.Orange;
            }
        }
        return SystemColors.Highlight;
    }
}