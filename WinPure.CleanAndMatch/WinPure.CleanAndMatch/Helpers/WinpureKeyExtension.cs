namespace WinPure.CleanAndMatch.Helpers;

internal static class WinPureKeyExtension
{
    public static List<long> GetSelectedWpKeysForSelectedRows(this GridView gvData)
    {
        var selectedIds = new List<long>();
        var col = gvData.Columns.FirstOrDefault(x => x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY);

        if (col == null)
        {
            return selectedIds;
        }

        foreach (int r in gvData.GetSelectedRows())
        {
            selectedIds.Add((long)gvData.GetRowCellValue(r, col));
        }

        return selectedIds;
    }

    public static List<long> GetSelectedWpKeysForVisibleRows(this GridView gvData)
    {
        var selectedIds = new List<long>();
        var col = gvData.Columns.FirstOrDefault(x => x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY);
        if (col == null || gvData.DataRowCount <= 0)
        {
            return selectedIds;
        }

        for (int controllerRow = 0; controllerRow < gvData.DataController.VisibleListSourceRowCount; ++controllerRow)
        {
            if (gvData.GetRow(controllerRow) is DataRowView row)
            {
                selectedIds.Add((long)row[col.FieldName]);
            }
        }

        return selectedIds;
    }

    public static List<long> GetSelectedWpKeysForCurrentGroup(this GridView gvData)
    {
        var selectedIds = new List<long>();

        if (gvData.GetFocusedDataRow()[WinPureColumnNamesHelper.WPCOLUMN_GROUPID] is int currentGroupId)
        {
            selectedIds = (gvData.GridControl.DataSource as DataTable).AsEnumerable().Where(x =>
                    x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID) == currentGroupId)
                .Select(x => x.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY)).ToList();
        }

        return selectedIds;
    }
}