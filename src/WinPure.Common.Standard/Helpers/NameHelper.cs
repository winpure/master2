namespace WinPure.Common.Helpers
{
    public static class NameHelper
    {
        internal static readonly string ProhibitedChars = @"\/:*?""<>|[]%#()'€$~`^,";
        internal static readonly string EntityResolutionTable = "EntityResolution";
        internal static readonly string EntityResolutionRelatedTable = "ER_Relationship";
        internal static readonly string EntityResolutionRelatedView = "ER_Related";
        internal static readonly string EntityResolutionDuplicatedView = "ER_Duplicated";
        internal static readonly string MatchResultTable = "MatchResult";
        internal static readonly string AuditLogTable = "AuditLogs";

        public static string GetCleanSettingsTable(string tableName)
        {
            return tableName + "_CleanSettings";
        }

        public static string GetColumnValuesTable(string tableName)
        {
            return tableName + "_ColumnValues";
        }

        public static string GetStatisticTable(string tableName)
        {
            return tableName + "_CleanStatistic";
        }

        public static string GetWordManagerTable(string tableName)
        {
            return tableName + "_CleanSettingsWM";
        }

        public static string GetDataBackupTable(string tableName)
        {
            return tableName + "_DataBackup";
        }
    }
}