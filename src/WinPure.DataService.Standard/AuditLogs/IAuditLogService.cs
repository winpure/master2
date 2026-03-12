using WinPure.DataService.Enums;

namespace WinPure.DataService.AuditLogs;

public interface IAuditLogService: IDisposable
{
    void AddAuditLogs(List<AuditLog> logs);
    string GetFilterString(List<string> sources, List<AuditLogModule> modules, DateTime? dateFrom, DateTime? dateTo, string whereCondition);
    List<AuditLog> GetAuditLogs(List<string> sources, List<AuditLogModule> modules, DateTime? dateFrom, DateTime? dateTo, string whereCondition);
    List<string> GetSources();
    long GetNextLogId();
    void AddSingleAuditLogIfEnabled(string tableName, string message);
    bool LogExists();
    void ClearLogs(List<string> sources, List<AuditLogModule> modules, DateTime? dateFrom, DateTime? dateTo);
    string GetDatabaseSize();
}