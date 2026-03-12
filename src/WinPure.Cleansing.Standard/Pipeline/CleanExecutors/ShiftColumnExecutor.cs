using System.Threading;

namespace WinPure.Cleansing.Pipeline.CleanExecutors;

internal static class ShiftColumnExecutor
{
    public static void Execute(List<ColumnShiftSetting> operations, DataTable data, CancellationToken cancellationToken)
    {
        if (!operations.Any())
        {
            return;
        }

        var toLeft = operations.Where(x => x.ShiftLeft).OrderBy(x => x.SourceIndex).ToList();
        for (int r = 0; r < data.Rows.Count; r++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var rw = data.Rows[r];
            for (int i = 0; i < toLeft.Count - 1; i++)
            {
                if (!string.IsNullOrEmpty(rw[toLeft[i].ColumnName].ToString()))
                {
                    continue;
                }

                for (int j = i + 1; j < toLeft.Count; j++)
                {
                    if (!string.IsNullOrEmpty(rw[toLeft[j].ColumnName].ToString()))
                    {
                        rw[toLeft[i].ColumnName] = rw[toLeft[j].ColumnName];
                        if (data.Columns[toLeft[j].ColumnName].DataType == typeof(string))
                        {
                            rw[toLeft[j].ColumnName] = null;
                        }
                        else
                        {
                            rw[toLeft[j].ColumnName] = DBNull.Value;
                        }
                        break;
                    }
                }
            }
        }

        var toRight = operations.Where(x => !x.ShiftLeft).OrderByDescending(x => x.SourceIndex).ToList();
        for (int r = 0; r < data.Rows.Count; r++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            var rw = data.Rows[r];
            for (int i = 0; i < toRight.Count - 1; i++)
            {
                if (!string.IsNullOrEmpty(rw[toRight[i].ColumnName].ToString())) continue;

                for (int j = i + 1; j < toRight.Count; j++)
                {
                    if (!string.IsNullOrEmpty(rw[toRight[j].ColumnName].ToString()))
                    {
                        rw[toRight[i].ColumnName] = rw[toRight[j].ColumnName];
                        if (data.Columns[toRight[j].ColumnName].DataType == typeof(string))
                        {
                            rw[toRight[j].ColumnName] = null;
                        }
                        else
                        {
                            rw[toRight[j].ColumnName] = DBNull.Value;
                        }
                        break;
                    }
                }
            }
        }
    }
}