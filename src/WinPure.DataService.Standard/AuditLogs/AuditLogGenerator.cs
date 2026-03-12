using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WinPure.Cleansing.Models;
using WinPure.DataService.AuditLogs;
using WinPure.DataService.Enums;
using WinPure.Matching.Enums;
using WinPure.Matching.Models.Support;

internal class AuditLogGenerator : IAuditLogGenerator
{
    private readonly IAuditLogService _service;

    public AuditLogGenerator(IAuditLogService service)
    {
        _service = service;
    }

    public List<AuditLog> GetAuditLogs(
    string sourceName,
    DataTable original,
    DataTable updated,
    WinPureCleanSettings settings)
    {
        var logs = new List<AuditLog>();
        var currentLogId = _service.GetNextLogId();
        var userName = SystemInfoHelper.GetCurrentUserQualified();
        var primaryKey = WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY;
        var module = AuditLogModule.Cleaning.GetAttributeOfType<DisplayNameAttribute>().DisplayName;
        // Ensure primary keys are set for Find()
        EnsurePrimaryKey(original, primaryKey);

        var cleanedColumns = GetCleanedColumns(settings);
        var columnToReasonMap = BuildReasonMapForCleansing(settings);

        // p1: Build reasons cache once for all updated columns (already distinct inside)
        var reasonsCache = updated.Columns
            .Cast<DataColumn>()
            .ToDictionary(
                c => c.ColumnName,
                c => GetReasonsForCleanedColumn(c.ColumnName, cleanedColumns, settings, columnToReasonMap)
            );

        // p2: Precompute updated columns array and ordinal map to original
        var updatedCols = updated.Columns.Cast<DataColumn>().ToArray();
        var colMap = updatedCols
            .Select(uc => original.Columns.Contains(uc.ColumnName) ? original.Columns[uc.ColumnName].Ordinal : -1)
            .ToArray();

        // New columns detection
        var addedColumns = updated.Columns
            .Cast<DataColumn>()
            .Select(c => c.ColumnName)
            .Except(original.Columns.Cast<DataColumn>().Select(c => c.ColumnName))
            .ToList();
        var addedSet = new HashSet<string>(addedColumns, StringComparer.Ordinal);

        // add new columns (Cleaning)
        foreach (var addedColumn in addedColumns)
        {
            var reasons = reasonsCache.TryGetValue(addedColumn, out var r) ? r : new List<string>();
            if (reasons.Any())
            {
                logs.Add(new AuditLog
                {
                    Id = currentLogId++,
                    SourceName = sourceName,
                    RecordId = null,
                    AffectedField = addedColumn,
                    OriginalValue = null,
                    NewValue = null,
                    Module = module,
                    Reason = "Column added by: " + string.Join(";", reasons),
                    Timestamp = DateTime.Now,
                    UserName = userName,
                });
            }
        }

        // p3: Thread-local buffers + one merge; no lock inside hot loop
        var chunks = new ConcurrentBag<List<AuditLog>>();

        Parallel.ForEach(
            updated.AsEnumerable(),
            () => new List<AuditLog>(64), // thread-local list
            (row, _, local) =>
            {
                var key = row[primaryKey]?.ToString();
                if (string.IsNullOrEmpty(key))
                    return local;

                var origRow = original.Rows.Find(key);
                if (origRow == null)
                    return local;

                // Iterate columns by index for fast access
                for (int i = 0; i < updatedCols.Length; i++)
                {
                    var uc = updatedCols[i];
                    if (uc.ColumnName == primaryKey) continue;

                    // p2: ordinal access; -1 means column absent in original
                    var oldValObj = colMap[i] >= 0 ? origRow[colMap[i]] : null;
                    var newValObj = row[i];

                    // Normalize to strings only when adding to log; for equality use fast string compare
                    var oldValue = NormalizeFast(oldValObj);
                    var newValue = NormalizeFast(newValObj);

                    // Skip if equal (both null/empty or same)
                    if (AreEqual(oldValue, newValue))
                        continue;

                    var id = Interlocked.Increment(ref currentLogId);
                    var reasons = reasonsCache[uc.ColumnName]; // p1: cached; already distinct
                    // For existing columns, drop Split[...] reasons — they describe column origin, not current operation
                    if (!addedSet.Contains(uc.ColumnName))
                    {
                        reasons = reasons.Where(r => !r.StartsWith("Split[", StringComparison.Ordinal)).ToList();
                    }

                    local.Add(new AuditLog
                    {
                        Id = id,
                        SourceName = sourceName,
                        RecordId = key,
                        AffectedField = uc.ColumnName,
                        OriginalValue = oldValue,
                        NewValue = newValue,
                        Module = module,
                        Reason = string.Join(";", reasons), // p5: no Distinct here
                        Timestamp = DateTime.Now,
                        UserName = userName,
                    });
                }

                return local;
            },
            local => chunks.Add(local)
        );

        // Single merge
        var total = logs.Count + chunks.Sum(c => c.Count);
        var result = new List<AuditLog>(total);
        result.AddRange(logs);
        foreach (var chunk in chunks) result.AddRange(chunk);

        return result;
    }

    public List<AuditLog> GetAuditLogs(
        AuditLogModule module,
        DataTable original,
        DataTable updated,
        DeleteFromMatchResultSetting settings)
    {
        if (original == null)
            return new List<AuditLog>();

        var currentLogId = _service.GetNextLogId();
        var now = DateTime.Now;
        var reasonDisplay = settings.DeleteSetting.GetAttributeOfType<DisplayNameAttribute>().DisplayName;
        var reason = $"Process match result [{reasonDisplay}]";
        var moduleName = module.GetAttributeOfType<DisplayNameAttribute>().DisplayName;
        var userName = SystemInfoHelper.GetCurrentUserQualified();

        if (module == AuditLogModule.Match)
        {
            var pk = WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY;
            var deletedRows = original.AsEnumerable()
                .GroupJoin(
                    updated.AsEnumerable(),
                    o => o.Field<long>(pk),
                    u => u.Field<long>(pk),
                    (o, grp) => new { o, grp }
                )
                .SelectMany(x => x.grp.DefaultIfEmpty(), (x, u) => new { x.o, u })
                .Where(x => x.u == null)
                .Select(x => x.o)
                .ToList();


            // LINQ conversion of the selected foreach block
            return deletedRows
                .Select(row => new AuditLog
                {
                    Id = currentLogId++,
                    SourceName = row.Field<string>(WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME),
                    RecordId = row.Field<long>(pk).ToString(),
                    AffectedField = null,
                    OriginalValue = SerializeRowToJson(row),
                    NewValue = null,
                    Module = moduleName,
                    Reason = reason,
                    Timestamp = now,
                    UserName = userName,
                })
                .ToList();
        }

        if (module == AuditLogModule.MatchAI)
        {
            return original.AsEnumerable().AsParallel()
                .Select(row => new AuditLog
                {
                    Id = currentLogId++,
                    SourceName = row.Field<string>(WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME),
                    RecordId = row.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_ORIGINAL_KEY).ToString(),
                    AffectedField = null,
                    OriginalValue = SerializeRowToJson(row),
                    NewValue = null,
                    Module = moduleName,
                    Reason = reason,
                    Timestamp = now,
                    UserName = userName,
                }).ToList();
        }

        return new List<AuditLog>();
    }

    public List<AuditLog> GetAuditLogs(
    AuditLogModule module,
    DataTable original,
    DataTable updated,
    List<UpdateMatchResultSetting> matchResultUpdateSettings)
    {
        if (original == null || updated == null || matchResultUpdateSettings == null || matchResultUpdateSettings.Count == 0)
            return new List<AuditLog>();

        var currentLogId = _service.GetNextLogId();
        var now = DateTime.Now;
        var moduleName = module.GetAttributeOfType<DisplayNameAttribute>().DisplayName;
        var userName = SystemInfoHelper.GetCurrentUserQualified();

        // Build dictionary: columnName -> reason
        var reasonByColumn = matchResultUpdateSettings
            .Where(s => !string.IsNullOrEmpty(s.FieldName) && s.Operation != UpdateOperationType.NotUpdate)
            .ToDictionary(
                s => s.FieldName,
                s => $"{moduleName} result update [{s.Operation.GetAttributeOfType<DisplayNameAttribute>().DisplayName}]",
                StringComparer.Ordinal
            );

        if (reasonByColumn.Count == 0)
            return new List<AuditLog>();

        // Collect primary key (must exist in tables)
        var pk = WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY;
        EnsurePrimaryKey(original, pk);
        EnsurePrimaryKey(updated, pk);

        var logsBag = new ConcurrentBag<List<AuditLog>>();

        Parallel.ForEach(
            Enumerable.Range(0, updated.Rows.Count),
            () => new List<AuditLog>(16),
            (rowIndex, _, local) =>
            {
                var uRow = updated.Rows[rowIndex];
                var oRow = original.Rows[rowIndex];

                var recordId = uRow[pk]?.ToString();
                if (string.IsNullOrEmpty(recordId))
                    return local;

                foreach (var kv in reasonByColumn)
                {
                    var colName = kv.Key;
                    if (!updated.Columns.Contains(colName) || !original.Columns.Contains(colName))
                        continue;

                    var oldValue = NormalizeFast(oRow[colName]);
                    var newValue = NormalizeFast(uRow[colName]);

                    if (AreEqual(oldValue, newValue))
                        continue;

                    var id = Interlocked.Increment(ref currentLogId);

                    local.Add(new AuditLog
                    {
                        Id = id,
                        SourceName = uRow.Table.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME)
                            ? NormalizeFast(uRow[WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME])
                            : null,
                        RecordId = recordId,
                        AffectedField = colName,
                        OriginalValue = oldValue,
                        NewValue = newValue,
                        Module = moduleName,
                        Reason = kv.Value,
                        Timestamp = now,
                        UserName = userName,
                    });
                }

                return local;
            },
            local => logsBag.Add(local)
        );

        var result = new List<AuditLog>(logsBag.Sum(l => l.Count));
        foreach (var chunk in logsBag)
            result.AddRange(chunk);

        return result;
    }

    public List<AuditLog> GetAuditLogs(
    AuditLogModule module,
    DataTable original,
    DataTable updated,
    List<MergeMatchResultSetting> mergeSettings)
    {
        if (original == null || updated == null || mergeSettings == null || mergeSettings.Count == 0)
            return new List<AuditLog>();

        var result = new List<AuditLog>();
        var currentLogId = _service.GetNextLogId();
        var now = DateTime.Now;
        var userName = SystemInfoHelper.GetCurrentUserQualified();

        var pk = WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY;
        var groupCol = WinPureColumnNamesHelper.WPCOLUMN_GROUPID;
        var srcCol = WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME;

        var moduleName = module.GetAttributeOfType<DisplayNameAttribute>().DisplayName;
        string reason = $"Process {moduleName} result [Merge]";
        string reasonDelete = $"Process {moduleName} result [Merge.Delete non master]";
        string reasonNewColumn = $"Process {moduleName} result [Merge. Add column to keep all values]";

        EnsurePrimaryKey(original, pk);
        EnsurePrimaryKey(updated, pk);

        // ---------- PART 1: deletions ----------
        var updatedPkSet = new HashSet<long>();
        foreach (DataRow row in updated.Rows)
            updatedPkSet.Add(row.Field<long>(pk));

        int srcOrdOriginal = original.Columns[srcCol].Ordinal;
        var deletedGroupIds = new HashSet<int>();

        foreach (DataRow row in original.Rows)
        {
            var id64 = row.Field<long>(pk);
            if (!updatedPkSet.Contains(id64))
            {
                deletedGroupIds.Add(row.Field<int>(groupCol));

                result.Add(new AuditLog
                {
                    Id = currentLogId++,
                    SourceName = (string)row[srcOrdOriginal],
                    RecordId = id64.ToString(),
                    AffectedField = null,
                    OriginalValue = SerializeRowToJson(row),
                    NewValue = null,
                    Module = moduleName,
                    Reason = reasonDelete,
                    Timestamp = now,
                    UserName = userName,
                });
            }
        }

        if (deletedGroupIds.Count == 0)
            return result;

        // added KeepAllValues columns
        var keepAllNewCols = mergeSettings
            .Where(s => s.KeepAllValues && !string.IsNullOrEmpty(s.FieldName))
            .Select(s => s.FieldName + WinPureColumnNamesHelper.WPCOLUMN_ALLVALUES_SUFFIX)
            .Distinct(StringComparer.Ordinal)
            .Where(col => updated.Columns.Contains(col) && !original.Columns.Contains(col))
            .ToArray();

        if (keepAllNewCols.Length > 0)
        {
            // Use any existing SourceName (guaranteed present)
            var sourceNameForSchema = updated.Rows.Count > 0 ? updated.Rows[0].Field<string>(srcCol) : null;

            foreach (var newCol in keepAllNewCols)
            {
                result.Add(new AuditLog
                {
                    Id = currentLogId++,
                    SourceName = sourceNameForSchema,
                    RecordId = null,
                    AffectedField = newCol,
                    OriginalValue = null,
                    NewValue = null,
                    Module = moduleName,
                    Reason = reasonNewColumn,
                    Timestamp = now,
                    UserName = userName,
                });
            }
        }

        // changed values
        // 2.1 Regular fields with UpdateField = true (must exist in both tables)
        var trackCols = mergeSettings
            .Where(s => s.UpdateField && !string.IsNullOrEmpty(s.FieldName))
            .Select(s => s.FieldName)
            .Distinct(StringComparer.Ordinal)
            .Where(c => updated.Columns.Contains(c) && original.Columns.Contains(c))
            .ToArray();

        // 2.2 KeepAllValues fields
        var keepAllCols = mergeSettings
            .Where(s => s.KeepAllValues && !string.IsNullOrEmpty(s.FieldName))
            .Select(s => s.FieldName + WinPureColumnNamesHelper.WPCOLUMN_ALLVALUES_SUFFIX)
            .Distinct(StringComparer.Ordinal)
            .Where(c => updated.Columns.Contains(c) && !original.Columns.Contains(c))
            .ToArray();

        if (trackCols.Length == 0 && keepAllCols.Length == 0)
            return result;

        var bag = new ConcurrentBag<List<AuditLog>>();

        // Parallel over updated rows that belong to groups affected by deletions
        Parallel.ForEach(
            updated.AsEnumerable().Where(r => deletedGroupIds.Contains(r.Field<int>(groupCol))),
            () => new List<AuditLog>(32),
            (updatedRow, _, local) =>
            {
                long key = updatedRow.Field<long>(pk);
                var originalRow = original.Rows.Find(key);
                if (originalRow == null)
                    return local; // new master row after merge – skip by spec

                string srcName = updatedRow.Field<string>(srcCol);
                string recordIdStr = key.ToString();

                // 2.1 Regular tracked columns
                foreach (var colName in trackCols)
                {
                    var oldObj = originalRow[colName];
                    var newObj = updatedRow[colName];

                    if (Equals(oldObj, newObj))
                        continue;

                    var oldVal = NormalizeFast(oldObj);
                    var newVal = NormalizeFast(newObj);
                    if (AreEqual(oldVal, newVal))
                        continue;

                    local.Add(new AuditLog
                    {
                        Id = Interlocked.Increment(ref currentLogId),
                        SourceName = srcName,
                        RecordId = recordIdStr,
                        AffectedField = colName,
                        OriginalValue = oldVal,
                        NewValue = newVal,
                        Module = moduleName,
                        Reason = reason,
                        Timestamp = now,
                        UserName = userName,
                    });
                }

                // 2.2 KeepAllValues columns (<Field>_ALLVALUES)
                foreach (var colName in keepAllCols)
                {
                    object oldObj = original.Columns.Contains(colName) ? originalRow[colName] : null;
                    object newObj = updatedRow[colName];

                    if (Equals(oldObj, newObj))
                        continue;

                    var oldVal = NormalizeFast(oldObj);
                    var newVal = NormalizeFast(newObj);
                    if (AreEqual(oldVal, newVal))
                        continue;

                    local.Add(new AuditLog
                    {
                        Id = Interlocked.Increment(ref currentLogId),
                        SourceName = srcName,
                        RecordId = recordIdStr,
                        AffectedField = colName,
                        OriginalValue = oldVal, // likely null if column is new
                        NewValue = newVal,
                        Module = moduleName,
                        Reason = reason,
                        Timestamp = now,
                        UserName = userName,
                    });
                }

                return local;
            },
            local => bag.Add(local)
        );

        foreach (var chunk in bag)
            result.AddRange(chunk);

        return result;
    }

    // --- Helpers ---

    private static void EnsurePrimaryKey(DataTable table, string primaryKey)
    {
        if (table == null || string.IsNullOrEmpty(primaryKey)) return;

        if (table.PrimaryKey == null || table.PrimaryKey.Length == 0 ||
            table.PrimaryKey[0].ColumnName != primaryKey)
        {
            if (!table.Columns.Contains(primaryKey))
                return; // nothing we can do; Find() will fail, but caller guards against null

            table.PrimaryKey = new[] { table.Columns[primaryKey] };
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string NormalizeFast(object value)
    {
        if (value == null || value == DBNull.Value)
            return null;

        // Fast path: already a string
        if (value is string s)
        {
            return s;
        }

        // Fallback: convert to string once
        return Convert.ToString(value);
    }

    private static bool AreEqual(string a, string b)
        => string.Equals(a ?? string.Empty, b ?? string.Empty, StringComparison.Ordinal);

    private static HashSet<string> GetCleanedColumns(WinPureCleanSettings settings)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);

        foreach (var item in settings.TextCleanerSettings)
            result.Add(item.ColumnName);

        foreach (var item in settings.CaseConverterSettings)
            result.Add(item.ColumnName);

        foreach (var item in settings.ColumnCheckSettings)
            result.Add(item.ColumnName);

        foreach (var item in settings.ColumnSplitSettings)
            result.Add(item.ColumnName);

        foreach (var item in settings.ColumnShiftSettings)
            result.Add(item.ColumnName);

        foreach (var item in settings.WordManagerSettings)
            result.Add(item.ColumnName);

        return result;
    }

    private static List<string> GetReasonsForCleanedColumn(
        string columnName,
        HashSet<string> cleanedColumns,
        WinPureCleanSettings settings,
        Dictionary<string, string> columnToReasonMap)
    {
        var result = new List<string>();

        // Map-derived reasons (prefix-based)
        foreach (var kvp in columnToReasonMap)
        {
            if (columnName.StartsWith(kvp.Key, StringComparison.Ordinal))
                result.Add(kvp.Value);
        }

        if (cleanedColumns.Contains(columnName))
        {
            foreach (var tc in settings.TextCleanerSettings)
            {
                if (tc.ColumnName == columnName)
                {
                    var removeOptions = new List<string>();

                    if (tc.RemoveNonPrintableCharacters) removeOptions.Add("NonPrintableCharacters");
                    if (tc.RemoveAllDigits) removeOptions.Add("AllDigits");
                    if (!string.IsNullOrWhiteSpace(tc.RemoveCharacters)) removeOptions.Add("Characters");
                    if (tc.RemoveAllLetters) removeOptions.Add("AllLetters");
                    if (tc.RemoveAllSpaces) removeOptions.Add("AllSpaces");
                    if (tc.RemoveDots) removeOptions.Add("Dots");
                    if (tc.RemoveCommas) removeOptions.Add("Commas");
                    if (tc.RemoveHyphens) removeOptions.Add("Hyphens");
                    if (tc.RemoveApostrophes) removeOptions.Add("Apostrophes");
                    if (tc.RemoveLeadingSpace) removeOptions.Add("LeadingSpace");
                    if (tc.RemoveTrailingSpace) removeOptions.Add("TrailingSpace");
                    if (tc.RemoveMultipleSpaces) removeOptions.Add("MultipleSpaces");
                    if (tc.RemoveTabs) removeOptions.Add("Tabs");
                    if (tc.RemoveNewLine) removeOptions.Add("NewLine");
                    if (tc.RemovePunctuation) removeOptions.Add("Punctuation");
                    if (tc.RegexExpression != null && tc.RegexExpression.Count > 0) removeOptions.Add("Regex");

                    if (removeOptions.Count > 0)
                        result.Add($"Remove[{string.Join(",", removeOptions)}]");

                    removeOptions.Clear();
                    if (tc.ConvertLsToOnes) removeOptions.Add("LsToOnes");
                    if (tc.ConvertNaughtsToOs) removeOptions.Add("NaughtsToOs");
                    if (tc.ConvertOnesToLs) removeOptions.Add("OnesToLs");
                    if (tc.ConvertOsToNaughts) removeOptions.Add("OsToNaughts");
                    if (!string.IsNullOrWhiteSpace(tc.ConvertEmptyToDefaultValue)) removeOptions.Add("EmptyToDefaultValue");

                    if (removeOptions.Count > 0)
                        result.Add($"Convert[{string.Join(",", removeOptions)}]");
                }
            }

            foreach (var cc in settings.CaseConverterSettings)
            {
                if (cc.ColumnName == columnName)
                {
                    var convertOptions = new List<string>();
                    if (cc.ToUpperCase) convertOptions.Add("ToUpperCase");
                    if (cc.ToLowerCase) convertOptions.Add("ToLowerCase");
                    if (cc.ToProperCase) convertOptions.Add("ToProperCase");

                    if (convertOptions.Count > 0)
                        result.Add($"Standardize[{string.Join(",", convertOptions)}]");
                }
            }

            if (settings.ColumnShiftSettings.Any(x => x.ColumnName == columnName))
                result.Add($"ShiftColumns[{string.Join(";", settings.ColumnShiftSettings.Select(x => x.ColumnName))}]");

            if (settings.WordManagerSettings.Any(x => x.ColumnName == columnName))
                result.Add("WordManager");

        }

        // p5: ensure uniqueness here (once), so no Distinct in the hot path
        return result.Distinct().ToList();
    }

    private static Dictionary<string, string> BuildReasonMapForCleansing(WinPureCleanSettings settings)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var split in settings.ColumnSplitSettings)
        {
            if (!string.IsNullOrEmpty(split.ColumnName))
            {
                if (split.SplitEmailAddressIntoAccountDomainAndZone)
                {
                    map[$"{split.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ACCOUNT}"] = "Split[SplitEmailAddressIntoAccountDomainAndZone]";
                    map[$"{split.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DOMAIN}"] = "Split[SplitEmailAddressIntoAccountDomainAndZone]";
                    map[$"{split.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_EMAIL_COUNTRY}"] = "Split[SplitEmailAddressIntoAccountDomainAndZone]";
                    map[$"{split.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_SUB_DOMAIN}"] = "Split[SplitEmailAddressIntoAccountDomainAndZone]";
                }
                if (split.SplitNameAndEmailAddress)
                {
                    map[$"{split.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_EMAIL}"] = "Split[SplitNameAndEmailAddress]";
                    map[$"{split.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_EMAIL_NAME}"] = "Split[SplitNameAndEmailAddress]";
                }
                if (split.SplitTelephoneIntoInternationalCodeAndPhoneNumber)
                {
                    map[$"{split.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_PHONE_COUNTRY}"] = "Split[Phone]";
                    map[$"{split.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_PHONE_NUMBER}"] = "Split[Phone]";
                }
                if (split.SplitDatetime)
                {
                    map[$"{split.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DATE_DAY}"] = "Split[Date]";
                    map[$"{split.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DATE_MONTH}"] = "Split[Date]";
                    map[$"{split.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DATE_YEAR}"] = "Split[Date]";
                    map[$"{split.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DATE_HOUR}"] = "Split[Date]";
                    map[$"{split.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DATE_MINUTE}"] = "Split[Date]";
                    map[$"{split.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_DATE_SECOND}"] = "Split[Date]";
                }
                if (!string.IsNullOrEmpty(split.RegexCopy))
                {
                    for (int i = 1; i <= 5; i++)
                        map[$"{split.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_REGEX}_{i}"] = "Split[SplitByRegex]";
                }
                if (split.SplitIntoWords != null)
                {
                    for (int i = 1; i <= 5; i++)
                        map[$"{split.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_INTO_WORDS}_{i}"] = "Split[SplitIntoWords]";
                }
            }
        }

        foreach (var check in settings.ColumnCheckSettings)
        {
            if (!string.IsNullOrEmpty(check.ColumnName) && check.CheckEmail)
            {
                map[$"{check.ColumnName}{WinPureColumnNamesHelper.WPCOLUMN_VALIDATE_EMAIL}"] = $"CheckEmail[{check.ColumnName}]";
            }
        }

        map[WinPureColumnNamesHelper.WPCOLUMN_MERGE_RESULT] = "Merge";

        void Add(string suffix, string reason) =>
            map[$"{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_GENDER_PREFIX}{suffix}"] = reason;

        Add(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_PREFIX, "Split[Person name]");
        Add(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_FIRST, "Split[Person name]");
        Add(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_MIDDLE, "Split[Person name]");
        Add(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_LAST, "Split[Person name]");
        Add(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_SUFFIX, "Split[Person name]");
        Add(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_QUALITY, "Split[Person name]");
        Add(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_NAME_GENDER, "Split[Person name]");

        void AddAddress(string suffix)
            => map[$"{WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_PREFIX}{suffix}"] = "Split[Address]";

        AddAddress(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_HOUSENUMBER);
        AddAddress(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_STREETNAME);
        AddAddress(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_CITY);
        AddAddress(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_POSTCODE);
        AddAddress(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_HOUSE);
        AddAddress(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_POBOX);
        AddAddress(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_CITYDISTRICT);
        AddAddress(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_LEVEL);
        AddAddress(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_CATEGORY);
        AddAddress(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_UNIT);
        AddAddress(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_STATEDISTRICT);
        AddAddress(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_STATE);
        AddAddress(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_SUBURB);
        AddAddress(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_ISLAND);
        AddAddress(WinPureColumnNamesHelper.WPCOLUMN_SPLIT_ADDRESS_COUNTRY);

        return map;
    }

    // Serialize only current row values into JSON (no DataTable included)
    private static string SerializeRowToJson(DataRow row)
    {
        // Build a plain dictionary: columnName -> value (null for DBNull)
        var dict = new Dictionary<string, object>(StringComparer.Ordinal);
        foreach (DataColumn col in row.Table.Columns)
        {
            var val = row[col];
            dict[col.ColumnName] = (val == DBNull.Value) ? null : val;
        }

        return JsonConvert.SerializeObject(dict);
    }
}