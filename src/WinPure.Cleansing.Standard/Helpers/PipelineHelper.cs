using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using WinPure.Cleansing.Models.ContextData;
using WinPure.Cleansing.Pipeline;
using WinPure.Cleansing.Pipeline.CleanExecutors;
using WinPure.Common.Pipeline;

namespace WinPure.Cleansing.Helpers;

internal static class PipelineHelper
{
    internal static string MultiColumnExecutorFieldName = "2D9F7E65-FA31-41CA-A8A7-A312673958D1";

    internal static Dictionary<CleansingContext, IPipeline<CleansingContext, ConcurrentBag<CleansingContextData>>> CreatePipelines(Dictionary<CleansingContext, IPipelineBuilder<CleansingContext, ConcurrentBag<CleansingContextData>>> pipelineBuilders,
        DataTable data,
        WinPureCleanSettings settings,
        IWpLogger logger,
        IServiceProvider serviceProvider,
        CancellationToken token)
    {
        var pipelines = new Dictionary<CleansingContext, IPipeline<CleansingContext, ConcurrentBag<CleansingContextData>>>();

        var splitGenderColumns = settings.GenderSplitSettings.GenderColumns;
        var mergeColumns = settings.ColumnMergeSettings.OrderBy(x => x.Order).Select(x => x.ColumnName).ToList();

        foreach (var builder in pipelineBuilders)
        {
            var context = builder.Key;
            if (context.ColumnName != MultiColumnExecutorFieldName && !data.Columns.Contains(context.ColumnName))
            {
                throw new WinPureArgumentException($"Data doesn't contains column {context.ColumnName}");
            }

            context.ColumnType = context.ColumnName == MultiColumnExecutorFieldName ? typeof(string) : data.Columns[context.ColumnName].DataType;
            context.Logger = logger;
            context.Token = token;
            context.ParallelOptions = new ParallelOptions { CancellationToken = token };
            settings.ColumnMergeSettings = settings.ColumnMergeSettings.OrderBy(x => x.Order).ToList();
            if (settings.ColumnSplitSettings.Any(x =>
                    x.ColumnName == context.ColumnName && x.SplitTelephoneIntoInternationalCodeAndPhoneNumber))
            {
                var dictionaryService = serviceProvider.GetService(typeof(IDictionaryService)) as IDictionaryService;
                context.PhoneCodes = dictionaryService?.GetDictionaryData("Phone Codes").Result.ToDictionary(x => x.SearchValue, x => x.ReplaceValue) ?? new Dictionary<string, string>();
            }

            var contextData = data.AsEnumerable()
                .AsParallel()
                //.Where(x=> x.Field<object>(context.ColumnName) != null && !string.IsNullOrWhiteSpace(x.Field<object>(context.ColumnName).ToString()))
                .Select(x => new CleansingContextData
                {
                    Value = context.ColumnName == MultiColumnExecutorFieldName ? String.Empty : x.Field<object>(context.ColumnName)?.ToString(),
                    WinPureId = x.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY),
                    MergeValues = (builder.Value.ContainsBuilder(typeof(ColumnMergerExecutor))) ? mergeColumns.Select(s => x.Field<object>(s)?.ToString()).ToList() : null,
                    GenderSplitValue = (builder.Value.ContainsBuilder(typeof(GenderSplitExecutor)) && splitGenderColumns.Any())
                        ? splitGenderColumns.Aggregate("", (current, addCol) => current + " " + (x.Field<object>(addCol))).Trim()
                        : string.Empty
                }).ToList();

            context.CleansingData = new ConcurrentBag<CleansingContextData>(contextData);

            pipelines.Add(context, builder.Value.Build());
        }

        return pipelines;
    }

    internal static Dictionary<CleansingContext, IPipelineBuilder<CleansingContext, ConcurrentBag<CleansingContextData>>> PreparePipelineBuilders(WinPureCleanSettings settings)
    {
        var cleansingPipelineBuilders = new Dictionary<CleansingContext, IPipelineBuilder<CleansingContext, ConcurrentBag<CleansingContextData>>>();

        foreach (var columnForClean in settings.TextCleanerSettings.GroupBy(x => x.ColumnName)
                     .Select(x => new { ColumnName = x.Key, Settings = x.Select(s => s) }))
        {
            var executor = new TextCleanerExecutor(columnForClean.Settings.ToList());
            IPipelineBuilder<CleansingContext, ConcurrentBag<CleansingContextData>> pipelineBuilder = new CleansingPipelineBuilder();
            pipelineBuilder = pipelineBuilder.Use(executor);

            cleansingPipelineBuilders.Add(new CleansingContext
            {
                ColumnName = columnForClean.ColumnName
            }, pipelineBuilder);
        }

        foreach (var caseConverting in settings.CaseConverterSettings)
        {
            var pipelineContext = cleansingPipelineBuilders.Keys.FirstOrDefault(x => x.ColumnName == caseConverting.ColumnName);
            var executor = new CaseConverterExecutor(caseConverting);

            if (pipelineContext == null)
            {
                IPipelineBuilder<CleansingContext, ConcurrentBag<CleansingContextData>> pipelineBuilder = new CleansingPipelineBuilder();
                pipelineBuilder = pipelineBuilder.Use(executor);
                cleansingPipelineBuilders.Add(new CleansingContext
                {
                    ColumnName = caseConverting.ColumnName
                }, pipelineBuilder);
            }
            else
            {
                cleansingPipelineBuilders[pipelineContext] = cleansingPipelineBuilders[pipelineContext].Use(executor);
            }
        }

        foreach (var columnCheck in settings.ColumnCheckSettings)
        {
            var pipelineContext = cleansingPipelineBuilders.Keys.FirstOrDefault(x => x.ColumnName == columnCheck.ColumnName);
            var executor = new TextCheckExecutor(columnCheck);

            if (pipelineContext == null)
            {
                IPipelineBuilder<CleansingContext, ConcurrentBag<CleansingContextData>> pipelineBuilder = new CleansingPipelineBuilder();
                pipelineBuilder = pipelineBuilder.Use(executor);
                cleansingPipelineBuilders.Add(new CleansingContext
                {
                    ColumnName = columnCheck.ColumnName
                }, pipelineBuilder);
            }
            else
            {
                cleansingPipelineBuilders[pipelineContext] = cleansingPipelineBuilders[pipelineContext].Use(executor);
            }
        }

        foreach (var columnForSplit in settings.ColumnSplitSettings.GroupBy(x => x.ColumnName)
                     .Select(x => new { ColumnName = x.Key, Settings = x.Select(s => s) }))
        {
            var pipelineContext = cleansingPipelineBuilders.Keys.FirstOrDefault(x => x.ColumnName == columnForSplit.ColumnName);
            var executor = new ColumnSplitterExecutor(columnForSplit.Settings.ToList());

            if (pipelineContext == null)
            {
                IPipelineBuilder<CleansingContext, ConcurrentBag<CleansingContextData>> pipelineBuilder = new CleansingPipelineBuilder();
                pipelineBuilder = pipelineBuilder.Use(executor);
                cleansingPipelineBuilders.Add(new CleansingContext
                {
                    ColumnName = columnForSplit.ColumnName
                }, pipelineBuilder);
            }
            else
            {
                cleansingPipelineBuilders[pipelineContext] = cleansingPipelineBuilders[pipelineContext].Use(executor);
            }
        }

        if (settings.GenderSplitSettings.GenderColumns.Any())
        {
            var executor = new GenderSplitExecutor(settings.GenderSplitSettings);
            IPipelineBuilder<CleansingContext, ConcurrentBag<CleansingContextData>> pipelineBuilder = new CleansingPipelineBuilder();
            pipelineBuilder = pipelineBuilder.Use(executor);

            cleansingPipelineBuilders.Add(new CleansingContext
            {
                ColumnName = String.Join(" ", settings.GenderSplitSettings.GenderColumns)
            }, pipelineBuilder);
        }

        foreach (var columnForWordManager in settings.WordManagerSettings.GroupBy(x => x.ColumnName)
                     .Select(x => new { ColumnName = x.Key, Settings = x.Select(s => s) }))
        {
            var pipelineContext = cleansingPipelineBuilders.Keys.FirstOrDefault(x => x.ColumnName == columnForWordManager.ColumnName);
            var executor = new WordManagerExecutor(columnForWordManager.Settings.ToList());

            if (pipelineContext == null)
            {
                IPipelineBuilder<CleansingContext, ConcurrentBag<CleansingContextData>> pipelineBuilder = new CleansingPipelineBuilder();
                pipelineBuilder = pipelineBuilder.Use(executor);
                cleansingPipelineBuilders.Add(new CleansingContext
                {
                    ColumnName = columnForWordManager.ColumnName
                }, pipelineBuilder);
            }
            else
            {
                cleansingPipelineBuilders[pipelineContext] = cleansingPipelineBuilders[pipelineContext].Use(executor);
            }
        }

        if (settings.ColumnMergeSettings.Any())
        {
            var pipelineContext = cleansingPipelineBuilders.Keys.FirstOrDefault(x => x.ColumnName == MultiColumnExecutorFieldName);
            var executor = new ColumnMergerExecutor(settings.ColumnMergeSettings);

            if (pipelineContext == null)
            {
                IPipelineBuilder<CleansingContext, ConcurrentBag<CleansingContextData>> pipelineBuilder = new CleansingPipelineBuilder();
                pipelineBuilder = pipelineBuilder.Use(executor);
                cleansingPipelineBuilders.Add(new CleansingContext
                {
                    ColumnName = MultiColumnExecutorFieldName
                }, pipelineBuilder);
            }
            else
            {
                cleansingPipelineBuilders[pipelineContext] = cleansingPipelineBuilders[pipelineContext].Use(executor);
            }
        }

        return cleansingPipelineBuilders;
    }
}