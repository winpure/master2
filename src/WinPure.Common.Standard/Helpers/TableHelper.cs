using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace WinPure.Common.Helpers;
internal static class TableHelper
{
    public static List<T> ConvertDataTableToList<T>(DataTable table) where T : new()
    {
        var result = new List<T>();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var columnNames = new HashSet<string>(
            table.Columns
                .Cast<DataColumn>()
                .Select(c => c.ColumnName),
            StringComparer.OrdinalIgnoreCase
        );

        foreach (DataRow row in table.Rows)
        {
            var item = new T();

            foreach (var prop in properties)
            {
                if (!columnNames.Contains(prop.Name))
                    continue;

                try
                {
                    object value = row[prop.Name];
                    if (value == DBNull.Value) continue;

                    Type targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    object safeValue = Convert.ChangeType(value, targetType);
                    prop.SetValue(item, safeValue);
                }
                catch
                {
                    // ignore conversion errors
                }
            }

            result.Add(item);
        }

        return result;
    }

    public static DataTable ConvertListToDataTable<T>(List<T> items)
    {
        var table = new DataTable();
        if (items == null || items.Count == 0)
            return table;

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // create columns
        foreach (var prop in properties)
        {
            Type columnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            table.Columns.Add(prop.Name, columnType);
        }

        // fill rows
        foreach (var item in items)
        {
            var values = new object[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                values[i] = properties[i].GetValue(item) ?? DBNull.Value;
            }

            table.Rows.Add(values);
        }

        return table;
    }
}