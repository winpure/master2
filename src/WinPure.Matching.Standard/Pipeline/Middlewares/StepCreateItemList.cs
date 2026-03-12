using WinPure.Common.Helpers;
using WinPure.Matching.Properties;

namespace WinPure.Matching.Pipeline.Middlewares;

internal class StepCreateItemList : IPipelineMiddleware<MatchContext>
{
    public async Task Execute(MatchContext context, Func<MatchContext, Task> next)
    {
        context.OnProgress(Resources.API_CAPTION_CREATING_ITEMS_FOR_MATCHING, 5);
        var items = new List<Item>();
        //var startDt = DateTime.Now;
        foreach (var table in context.Tables)
        {
            if (!context.Parameter.Groups.Any(x => x.Conditions.Any(y => y.Fields.Any(z => z.TableName == table.TableName))))
            {
                continue;
            }

            var tableItems = context.RecordsToProcess <= 0 || items.Count + table.TableData.Rows.Count < context.RecordsToProcess
                ? table.TableData.Select()
                    .Select(x => new Item { DataRow = x, TableName = table.TableName, WinPurePK = (long)x[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY] })
                    .ToList()
                : table.TableData.Select().Take(context.RecordsToProcess - items.Count)
                    .Select(x => new Item { DataRow = x, TableName = table.TableName, WinPurePK = (long)x[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY] })
                    .ToList();

            items.AddRange(tableItems);

            if (context.RecordsToProcess > 0 && items.Count >= context.RecordsToProcess)
            {
                break;
            }
        }

        var fldToCheck = context.Parameter.Groups.SelectMany(x => x.Conditions)
            .Where(x => !x.IncludeEmpty)
            .SelectMany(x => x.Fields)
            .Distinct()
            .ToList();

        var itemsWithNull = new List<Item>();

        for (var i = 0; i < fldToCheck.Count; i++)
        {
            var fld = fldToCheck[i];
            itemsWithNull.AddRange(items.Where(x => x.TableName == fld.TableName && (x.DataRow[fld.ColumnName] == DBNull.Value || string.IsNullOrEmpty(x.DataRow[fld.ColumnName].ToString()))));
            items.RemoveAll(x => x.TableName == fld.TableName && (x.DataRow[fld.ColumnName] == DBNull.Value || string.IsNullOrEmpty(x.DataRow[fld.ColumnName].ToString())));
        }

        context.Items = items;
        context.ItemsWithNull = itemsWithNull;
        //context.Logger.Information($"ITEM WAS CREATED, TIME: {DateTime.Now - startDt:dd\\:hh\\:mm\\:ss}");
        await next(context);
    }
}