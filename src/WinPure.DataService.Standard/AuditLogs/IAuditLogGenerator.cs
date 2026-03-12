using System.Data;
using WinPure.Cleansing.Models;
using WinPure.DataService.Enums;
using WinPure.Matching.Models.Support;

namespace WinPure.DataService.AuditLogs;

internal interface IAuditLogGenerator
{
    List<AuditLog> GetAuditLogs(
        string sourceName,
        DataTable original,
        DataTable updated,
        WinPureCleanSettings settings);

    List<AuditLog> GetAuditLogs(
        AuditLogModule module,
        DataTable original,
        DataTable updated,
        DeleteFromMatchResultSetting settings);

    public List<AuditLog> GetAuditLogs(
        AuditLogModule module,
        DataTable original,
        DataTable updated,
        List<MergeMatchResultSetting> mergeSettings);
}