namespace WinPure.DataService.Models;

public class AuditLogDataReport
{
    public CommonData Common { get; set; }
    public List<AuditLog> Logs { get; set; }
}

public class CommonData
{
    public bool LogEnabled { get; set; }
    public int NumberOfRecords { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}