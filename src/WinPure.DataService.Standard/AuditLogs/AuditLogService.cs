using System.Data;
using WinPure.Configuration.DependencyInjection;
using WinPure.Configuration.Helper;
using WinPure.DataService.Enums;

namespace WinPure.DataService.AuditLogs;

internal class AuditLogService : IAuditLogService
{
    private readonly ILogConnectionManager _logConnectionManager;
    private readonly IWpLogger _logger;

    public AuditLogService(ILogConnectionManager logConnectionManager, IWpLogger logger)
    {
        _logConnectionManager = logConnectionManager;
        _logger = logger;
    }

    public void AddAuditLogs(List<AuditLog> logs)
    {
        var table = new DataTable(NameHelper.AuditLogTable);

        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("SourceName", typeof(string));
        table.Columns.Add("RecordId", typeof(string));
        table.Columns.Add("AffectedField", typeof(string));
        table.Columns.Add("OriginalValue", typeof(string));
        table.Columns.Add("NewValue", typeof(string));
        table.Columns.Add("Module", typeof(string));
        table.Columns.Add("Reason", typeof(string));
        table.Columns.Add("Timestamp", typeof(DateTime));
        table.Columns.Add("UserName", typeof(string));

        foreach (var log in logs)
        {
            table.Rows.Add(
                log.Id,
                log.SourceName,
                log.RecordId,
                log.AffectedField,
                log.OriginalValue,
                log.NewValue,
                log.Module,
                log.Reason,
                log.Timestamp,
                log.UserName
            );
        }

        if (SqLiteHelper.CheckTableExists(NameHelper.AuditLogTable, _logConnectionManager.Connection))
        {
            var columnList = "[Id],[SourceName],[RecordId],[AffectedField],[OriginalValue],[NewValue],[Module],[Reason],[Timestamp],[UserName]";

            SqLiteHelper.AppendDataToDb(_logConnectionManager.Connection, table, NameHelper.AuditLogTable, columnList, string.Empty);
        }
        else
        {
            SqLiteHelper.SaveDataToDb(_logConnectionManager.Connection, table, NameHelper.AuditLogTable, _logger, false);
        }
    }

    public string GetFilterString(List<string> sources, List<AuditLogModule> modules, DateTime? dateFrom, DateTime? dateTo, string whereCondition)
    {
        var whereSources = $"[SourceName] IN ('{string.Join("','", sources)}') AND [Module] IN ('{string.Join("','", modules.Select(x => x.GetAttributeOfType<DisplayNameAttribute>().DisplayName))}')";

        if (dateFrom.HasValue)
        {
            whereSources += $" AND [Timestamp] >= '{dateFrom.Value:yyyy-MM-dd}'";
        }

        if (dateTo.HasValue)
        {
            whereSources += $" AND [Timestamp] < '{dateTo.Value.AddDays(1):yyyy-MM-dd}'";
        }

        if (!string.IsNullOrWhiteSpace(whereCondition))
        {
            whereSources += $" AND ({whereCondition})";
        }

        return whereSources;
    }

    public List<AuditLog> GetAuditLogs(List<string> sources, List<AuditLogModule> modules, DateTime? dateFrom, DateTime? dateTo, string whereCondition)
    {
        _logConnectionManager.CheckConnectionState();
        if (_logConnectionManager.Connection.State != ConnectionState.Open || sources == null || !sources.Any())
            return new List<AuditLog>();

        if (SqLiteHelper.CheckTableExists(NameHelper.AuditLogTable, _logConnectionManager.Connection))
        {
            var whereSources = GetFilterString(sources, modules, dateFrom, dateTo, whereCondition);
            var sql = $"select * from {NameHelper.AuditLogTable} WHERE {whereSources}";

            var data = SqLiteHelper.ExecuteQuery(sql, _logConnectionManager.Connection);
            var auditLogs = data.AsEnumerable()
                .AsParallel()
                .Select(row => new AuditLog
                {
                    Id = row.Field<long>("Id"),
                    SourceName = row.Field<string>("SourceName"),
                    RecordId = row.Field<string>("RecordId"),
                    AffectedField = row.Field<string>("AffectedField"),
                    OriginalValue = row.Field<string>("OriginalValue"),
                    NewValue = row.Field<string>("NewValue"),
                    Module = row.Field<string>("Module"),
                    Reason = row.Field<string>("Reason"),
                    Timestamp = row.Field<DateTime>("Timestamp"),
                    UserName = row.Field<string>("UserName"),
                })
                .OrderBy(x => x.SourceName)
                .ThenByDescending(x => x.Id)
                .ToList();

            return auditLogs;
        }

        return [];
    }

    public List<string> GetSources()
    {
        _logConnectionManager.CheckConnectionState();
        if (_logConnectionManager.Connection.State != ConnectionState.Open)
            return new List<string>();
        if (SqLiteHelper.CheckTableExists(NameHelper.AuditLogTable, _logConnectionManager.Connection))
        {
            var sql = $"select distinct SourceName from {NameHelper.AuditLogTable}";
            var data = SqLiteHelper.ExecuteQuery(sql, _logConnectionManager.Connection);
            return data.AsEnumerable().Select(row => row.Field<string>("SourceName")).ToList();
        }
        return [];
    }

    public long GetNextLogId()
    {
        _logConnectionManager.CheckConnectionState();
        if (SqLiteHelper.CheckTableExists(NameHelper.AuditLogTable, _logConnectionManager.Connection))
        {
            var sql = $"select max(Id) from {NameHelper.AuditLogTable}";
            var maxId = SqLiteHelper.ExecuteScalar<long>(sql, _logConnectionManager.Connection);
            return maxId + 1;
        }
        return 1;
    }

    public void AddSingleAuditLogIfEnabled(string tableName, string message)
    {
        var configurationService = WinPureConfigurationDependency.Resolve<IConfigurationService>();
        if (!configurationService.Configuration.EnableAuditLogs)
            return;

        var nextId = GetNextLogId();
        var log = new AuditLog
        {
            Id = nextId,
            SourceName = tableName,
            RecordId = string.Empty,
            AffectedField = string.Empty,
            OriginalValue = string.Empty,
            NewValue = string.Empty,
            Module = string.Empty,
            Reason = message,
            Timestamp = DateTime.Now,
            UserName = SystemInfoHelper.GetCurrentUserQualified()
        };

        AddAuditLogs([log]);
    }

    public bool LogExists()
    {
        _logConnectionManager.CheckConnectionState();
        if (_logConnectionManager.Connection.State == ConnectionState.Open
            && SqLiteHelper.CheckTableExists(NameHelper.AuditLogTable, _logConnectionManager.Connection))
        {
            var sql = $"select * from {NameHelper.AuditLogTable} LIMIT 1";
            var data = SqLiteHelper.ExecuteQuery(sql, _logConnectionManager.Connection);
            return data.Rows.Count > 0;
        }
        return false;
    }

    public void ClearLogs(List<string> sources, List<AuditLogModule> modules, DateTime? dateFrom, DateTime? dateTo)
    {
        _logConnectionManager.CheckConnectionState();
        if (_logConnectionManager.Connection.State != ConnectionState.Open || sources == null || !sources.Any())
            return;

        if (SqLiteHelper.CheckTableExists(NameHelper.AuditLogTable, _logConnectionManager.Connection))
        {
            var whereString = GetFilterString(sources, modules, dateFrom, dateTo, String.Empty);
            var sql = $"DELETE from {NameHelper.AuditLogTable} WHERE {whereString}";

            SqLiteHelper.ExecuteNonQuery(sql, _logConnectionManager.Connection);
            SqLiteHelper.ExecuteNonQuery("VACUUM;", _logConnectionManager.Connection);
        }
    }

    public string GetDatabaseSize() => _logConnectionManager.GetDbSize();

    public void Dispose()
    {
        _logConnectionManager?.Dispose();
        FileHelper.SafeDeleteFileWithLogging(_logger, _logConnectionManager?.DbPath, "Cannot delete LOG DB file on project closing");
    }
}