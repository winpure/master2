namespace WinPure.Common.Models;

public class AuditLog
{
    public long Id { get; set; } //Autoincrement
    public string SourceName { get; set; }
    public string RecordId { get; set; }
    public string AffectedField { get; set; }
    public string OriginalValue { get; set; }
    public string NewValue { get; set; }
    public string Module { get; set; }
    public string Reason { get; set; }
    public DateTime Timestamp { get; set; }
    public string UserName { get; set; }
}