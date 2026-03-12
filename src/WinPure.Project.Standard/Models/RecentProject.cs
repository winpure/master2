namespace WinPure.Project.Models;

public class RecentProject
{
    public string Name { get; set; }
    public string Id { get; set; }
    public string Path { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime EditDate { get; set; }
    public int NumberOfDataset { get; set; }
    public int NumberOfRecords { get; set; }
    public bool IsCleansing { get; set; }
    public bool IsMatch { get; set; }
    public bool IsMatchAi { get; set; }
    public bool IsAddressVerification { get; set; }
    public bool IsAutomation { get; set; }
    public bool IsAuditLog { get; set; }
}