using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using WinPure.Common.Models;

namespace WinPure.Common.Helpers
{
    internal static class ColumnHelper
    {
        private static readonly Regex FieldNameEscapeCharactersPattern = new Regex(@"([%/\\#&()\""'€$^:.\[\]*~!?`|={}\-])", RegexOptions.Compiled);

        public static List<string> WinPureSystemFields = new List<string> { WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY, 
            WinPureColumnNamesHelper.WPCOLUMN_ISMASTER, WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED, WinPureColumnNamesHelper.WPCOLUMN_GROUPID,
            WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME, WinPureColumnNamesHelper.WPCOLUMN_TOTALSCORE, 
            WinPureColumnNamesHelper.WPCOLUMN_ORIGINAL_KEY, WinPureColumnNamesHelper.WPCOLUMN_MATCH_KEY,
            WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID_KEY, WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID_KEY,
            WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID, WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID };

        public static string NormalizeColumnName(this string columnName)
        {
            return FieldNameEscapeCharactersPattern.Replace(columnName, "").Trim();
        }

        public static string AddColumn(DataTable data, string columnName, string columnSuffix, bool overrideExisting, Type columnDataType)
        {
            var columnFullName = columnName + columnSuffix;
            int? columnPosition = null;
            if (data.Columns.Contains(columnFullName))
            {
                if (overrideExisting)
                {
                    columnPosition = data.Columns[columnFullName].Ordinal;
                    data.Columns.Remove(columnFullName);
                }
                else
                {
                    columnFullName = GetDataTableUniqueColumnName(data, columnFullName);
                }
            }

            data.Columns.Add(new DataColumn(columnFullName, columnDataType));
            if (columnPosition.HasValue)
            {
                data.Columns[columnFullName].SetOrdinal(columnPosition.Value);
            }

            return columnFullName;
        }

        public static string GetDataTableUniqueColumnName(DataTable table, string baseName)
        {
            int i = 1;
            string resName = baseName.NormalizeColumnName();
            while (table.Columns.Contains(resName))
            {
                resName = baseName + i++;
            }

            return resName;
        }

        public static bool IsSystemField(string fieldName)
        {
            //TODO remove group later
            return WinPureSystemFields.Contains(fieldName) || ((fieldName.StartsWith("Group") || fieldName.StartsWith("Rule ")) && fieldName.EndsWith(" Score"));
        }

        public static void RemoveSystemFieldsFromTable(DataTable dt)
        {
            var fldToRemove = new List<string>();

            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName == WinPureColumnNamesHelper.WPCOLUMN_GROUPID) continue;

                if (IsSystemField(col.ColumnName))
                {
                    fldToRemove.Add(col.ColumnName);
                }
            }

            foreach (var fldName in fldToRemove.Distinct())
            {
                dt.Columns.Remove(fldName);
            }
        }

        public static void RenameSystemFieldsFromTable(DataTable dt)
        {
            foreach (DataColumn col in dt.Columns)
            {
                if (IsSystemField(col.ColumnName))
                {
                    var newColumnName = col.ColumnName.Replace(" ", "_");
                    newColumnName = GetDataTableUniqueColumnName(dt, newColumnName);
                    col.ColumnName = newColumnName;
                }
            }
        }

        public static bool RemoveWinPurePrimaryKeyFieldFromTable(DataTable dt)
        {
            if (dt.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY))
            {
                if (dt.PrimaryKey.Any(x => x.ColumnName == WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY))
                {
                    dt.PrimaryKey = null;
                }

                dt.Constraints.Clear();

                dt.Columns.Remove(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY);
                return true;
            }

            return false;
        }

        public static List<DataField> GetTableFields(DataTable table)
        {
            if (table == null)
            {
                return null;
            }
            var fields = new List<DataField>();
            int i = 0;
            foreach (DataColumn column in table.Columns)
            {
                fields.Add(new DataField
                {
                    Id = i++,
                    DatabaseName = column.ColumnName,
                    DisplayName = column.ColumnName.NormalizeColumnName(),
                    FieldType = column.DataType.ToString()
                });
            }
            return fields;
        }

        /// <summary>
        /// Table with data
        /// </summary>
        /// <param name="data"></param>
        /// <returns>TRUE if original data contains WP primary key field and FALSE if field wan not in the data</returns>
        public static bool EnsureWinPurePrimaryKeyExists(DataTable data)
        {
            if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY))
            {
                data.PrimaryKey = null;

                var column = new DataColumn(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY, Type.GetType("System.Int64"));
                data.Columns.Add(column);
                long index = 0;
                foreach (DataRow row in data.Rows)
                {
                    row.SetField(column, ++index);
                }
                data.PrimaryKey = new[] { column };
                return false;
            }

            return true;
        }
    }
}