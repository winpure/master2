namespace WinPure.Configuration.Models.Database
{
    public class RecentProjectEntity
    {
        public string Id { get; set; }
        public string ProjectName { get; set; }
        public string ProjectPath { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public int NumberOfDataset { get; set; }
        public int NumberOfRecords { get; set; }
        public bool IsCleansing { get; set; }
        public bool IsMatch { get; set; }
        public bool IsMatchAi { get; set; }
        public bool IsAddressVerification { get; set; }
        public bool IsAutomation { get; set; }
        public bool IsAuditLog { get; set; }
    }
}