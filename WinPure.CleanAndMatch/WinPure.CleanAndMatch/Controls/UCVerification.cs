using DevExpress.XtraPrinting.Preview;
using WinPure.CleanAndMatch.Reports;

namespace WinPure.CleanAndMatch.Controls;

internal partial class UCVerification : UserControl
{
    private IDataManagerService _service;
    public event Action OnNavigateToFullReport;
    public event Action<string, string, MessagesType, Exception> OnException;
    public DocumentViewer ReportViewer => addressReportViewer;
    public VerificationReport Report { get; private set; }

    public UCVerification()
    {
        InitializeComponent();
    }

    public void Initialize()
    {
        ucAddressVerificationConfiguration.Initialize(true, true);
        ucAddressVerificationConfiguration.OnNavigateToFullReport += UcAddressVerificationConfigurationOnNavigateToFullReport;
        _service = WinPureUiDependencyResolver.Resolve<IDataManagerService>();
        _service.OnAddressVerificationReady += _service_AddressVerificationReady;
        _service.OnCurrentTableChanged += _service_OnCurrentTableChanged;
    }

    private void _service_OnCurrentTableChanged(string tableName)
    {
        UpdateAddressVerificationReport(tableName);
    }

    private void _service_AddressVerificationReady(string tableName, bool navigateToReport)
    {
        UpdateAddressVerificationReport(tableName);
    }

    private void UpdateAddressVerificationReport(string tableName)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { UpdateAddressVerificationReport(tableName); }));
            return;
        }

        var configurationService = WinPureUiDependencyResolver.Resolve<IConfigurationService>();

        Report = new VerificationReport();
        var reportPath = configurationService.Configuration.AddressReportPath;
        if (!File.Exists(reportPath))
        {
            OnException?.Invoke(string.Format(Resources.EXCEPTION_REPORT_FILE_NOT_EXISTS, reportPath), "", MessagesType.Error, null);
            return;
        }
        Report.LoadLayout(reportPath);
        var ds = _service.GetAddressVerificationResult(tableName);
        if (ds != null)
        {
            Report.DataSource = ds;
            Report.DisplayName = "Address verification report";
            Report.CreateDocument();
            //report.ShowDesigner();

            addressReportViewer.DocumentSource = Report;
            addressReportViewer.SetThumbnailsVisibility(true);
            addressReportViewer.Refresh();
            addressReportViewer.Show();
        }
        else
        {
            addressReportViewer.Hide();
        }
    }

    private void UcAddressVerificationConfigurationOnNavigateToFullReport()
    {
        OnNavigateToFullReport?.Invoke();
    }

    public void CloseCurrentData(bool removeControl = true)
    {
        ucAddressVerificationConfiguration.CloseCurrentData(removeControl);
    }

    public void ShowSubPanel(AddressVerificationViewType dType)
    {
        switch (dType)
        {
            case AddressVerificationViewType.Settings: navFrameVerification.SelectedPage = navPageConfigurtion; break;
            case AddressVerificationViewType.Report: navFrameVerification.SelectedPage = navPageReport; break;
        }
    }
}