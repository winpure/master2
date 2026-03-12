namespace WinPure.CleanAndMatch.Reports;

public partial class AuditLogReport : XtraReport
{
    public AuditLogReport()
    {
        InitializeComponent();
    }

    private void AuditLogReport_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (DataSource is AuditLogDataReport rd)
        {
            lbLogEnabled.Text = rd.Common.LogEnabled.ToString();
            lbTotalRecords.Text = rd.Common.NumberOfRecords.ToString("N0");
            lbDateRange.Text = $"{rd.Common.StartDate:dd.MM.yyyy} - {rd.Common.EndDate:dd.MM.yyyy}";
        }
    }
}