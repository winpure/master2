using Newtonsoft.Json;
using Senzing.Sdk;
using Senzing.Sdk.Core;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WinPure.Common.Enums;
using WinPure.Configuration.Enums;
using WinPure.Configuration.Helper;
using WinPure.Configuration.Models.Configuration;
using WinPure.DataService.Senzing.Helpers;
using WinPure.DataService.Senzing.Models;
using WinPure.DataService.Senzing.Models.G2;
using WinPure.DataService.Senzing.Models.Pipeline;

namespace WinPure.DataService.Senzing;

internal class SenzingPipeline
{
    private readonly string _dbPath;
    private readonly string _senzingReportPath;
    private readonly Action<string, int> _onProgressAction;
    private readonly IWpLogger _logger;

    private SzEnvironment _szEnvironment;
    private SzEngine _szEngine;

    private const int AddDataChunkSize = 3500;
    private const int ProcessResultChuckSize = 3500;
    private const int ProcessResultBlockSize = 500;
    private int AddedRecords;
    private int FetchedRecords;
    private int _LastReportedPercent;
    private DateTime StartDate;
    private SenzingConfiguration _configuration;

    private readonly EntityResolutionSetting _erSettings;
    private int _recordLimit;

    private readonly object _uniqueLock = new object();
    private readonly object _duplicateLock = new object();
    private readonly object _possibleRelatedLock = new object();
    private readonly object _possibleDuplicatedLock = new object();
    private readonly HashSet<long> _resolvedEntities = new HashSet<long>();

    public SenzingPipeline(EntityResolutionSetting erSettings, string dbPath, int recordLimit, Action<string, int> onProgressAction, IWpLogger logger)
    {
        _erSettings = erSettings;
        _recordLimit = recordLimit;
        _dbPath = dbPath;
        _onProgressAction = onProgressAction;
        _logger = logger;
        _senzingReportPath = Path.Combine(_logger.ReportPath, "EntityResolutionDebugReport.log");
        _configuration = new SenzingConfiguration(erSettings);
    }

    public EntityResolutionReport ExecutePipeline(List<EntityResolutionConfiguration> sourceData, CancellationToken cToken)
    {
        var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

        var dataFlowCleanUpOption = new ExecutionDataflowBlockOptions { MaxMessagesPerTask = 1, EnsureOrdered = false, MaxDegreeOfParallelism = 1, BoundedCapacity = 32, SingleProducerConstrained = true };
        var dataFlowSingleTaskOption = new ExecutionDataflowBlockOptions { MaxMessagesPerTask = 1, EnsureOrdered = false, MaxDegreeOfParallelism = 1, CancellationToken = cToken, BoundedCapacity = 32 };
        var dataFlowSingleTaskOptionSingleProducer = new ExecutionDataflowBlockOptions { MaxMessagesPerTask = 1, EnsureOrdered = false, MaxDegreeOfParallelism = 1, CancellationToken = cToken, BoundedCapacity = 32, SingleProducerConstrained = true };
        var dataFlowParallelTasksOption = new ExecutionDataflowBlockOptions { MaxMessagesPerTask = -1, EnsureOrdered = false, MaxDegreeOfParallelism = _erSettings.MaxDegreeOfParallelism, CancellationToken = cToken, BoundedCapacity = 32 };
        var dataFlowParallelTasksOptionSingleProducer = new ExecutionDataflowBlockOptions { MaxMessagesPerTask = -1, EnsureOrdered = false, MaxDegreeOfParallelism = _erSettings.MaxDegreeOfParallelism, CancellationToken = cToken, BoundedCapacity = 32, SingleProducerConstrained = true };

        var initiate = new TransformBlock<SenzingExecutionContext, SenzingExecutionContext>(InitiateAndCheckSource, dataFlowSingleTaskOption);
        var prepareSourceBlocks = new TransformManyBlock<SenzingExecutionContext, SourceBlock>(PrepareSourceBlock, dataFlowSingleTaskOptionSingleProducer);
        var prepareDataBlocks = new TransformManyBlock<SourceBlock, DataBlock>(PrepareDataBlock, dataFlowSingleTaskOptionSingleProducer);
        var processData = new TransformBlock<DataBlock, SenzingExecutionContext>(AddDataToSenzing, dataFlowParallelTasksOption);
        var processRedo = new ActionBlock<SenzingExecutionContext>(ProcessRedo, dataFlowParallelTasksOption);

        var prepareResultFromDb = new TransformManyBlock<SenzingExecutionContext, ResultPreparationBlock>(PrepareGetResultFromApiOnDb, dataFlowSingleTaskOptionSingleProducer);
        var getResultFromDb = new TransformBlock<ResultPreparationBlock, ResultBlock>(GetResultFromApi, dataFlowParallelTasksOption);

        var getResultFromApi = new TransformManyBlock<SenzingExecutionContext, ResultBlock>(GetResultFromApi, dataFlowSingleTaskOptionSingleProducer);
        var processResult = new TransformBlock<ResultBlock, SaveBlock>(ProcessResult, dataFlowParallelTasksOptionSingleProducer);
        var saveResultInternally = new ActionBlock<SaveBlock>(SaveResult, dataFlowSingleTaskOption);

        var postProcessing = new ActionBlock<SenzingExecutionContext>(PostProcessing, dataFlowSingleTaskOptionSingleProducer);
        var cleanUp = new ActionBlock<SenzingExecutionContext>(CleanUpSenzing, dataFlowCleanUpOption);

        initiate.LinkTo(prepareSourceBlocks, linkOptions);
        prepareSourceBlocks.LinkTo(prepareDataBlocks, linkOptions);
        prepareDataBlocks.LinkTo(processData, linkOptions);
        processData.LinkTo(processRedo, linkOptions);

        // pipeline for internal DB
        getResultFromApi.LinkTo(processResult, linkOptions);
        processResult.LinkTo(saveResultInternally, linkOptions);

        // pipeline for DB
        prepareResultFromDb.LinkTo(getResultFromDb, linkOptions);
        getResultFromDb.LinkTo(processResult, linkOptions);
        processResult.LinkTo(saveResultInternally, linkOptions);

        _resolvedEntities.Clear();
        AddedRecords = 0;
        FetchedRecords = 0;
        _LastReportedPercent = 0;
        StartDate = DateTime.Now;

        _logger.Information($"Chuck size: {AddDataChunkSize}; MaxDegreeOfParallelismForAddData: {_erSettings.MaxDegreeOfParallelism}; ProcessResultChuckSize: {ProcessResultChuckSize}");
        if (_erSettings.EnableDebugLogs)
        {
            File.AppendAllLines(_senzingReportPath, new[] { "==========START MATCH AI===========" }, Encoding.UTF8);
        }

        using var connection = new SQLiteConnection(SystemDatabaseConnectionHelper.GetConnectionString(_dbPath));
        connection.Open();

        var context = new SenzingExecutionContext
        {
            SourceData = sourceData,
            ResolvedEntityNumbers = new List<long>(),
            Report = new EntityResolutionReport
            {
                DuplicateRecordsCount = 0,
                PossibleDuplicateRecordsCount = 0,
                RelatedRecordsCount = 0
            },
            Errors = new ConcurrentBag<string>(),
            ErrorIds = new ConcurrentBag<EntityRecord>(),
            NotifyProgress = Progress,
            Connection = connection
        };

        try
        {
            var timer = new Stopwatch();
            timer.Start();
            initiate.Post(context);

            initiate.Complete();

            Task.WaitAll(processRedo.Completion);

            context.ResolvedEntityNumbers = _resolvedEntities.ToList();
            context.ResolvedEntityNumbers.Sort();
            _resolvedEntities.Clear();

            timer.Stop();
            _logger.Information($"Add data: time {timer.Elapsed.ToString("g")}");

            timer.Reset();
            timer.Start();
            PrepareResultTable(sourceData, context, connection);

            if (_erSettings.Database == EntityResolutionDatabase.Internal)
            {
                getResultFromApi.Post(context);
                getResultFromApi.Complete();
            }
            else
            {
                prepareResultFromDb.Post(context);
                prepareResultFromDb.Complete();
            }

            Task.WaitAll(saveResultInternally.Completion);
            timer.Stop();
            _logger.Information($"Save result: time {timer.Elapsed.ToString("g")}");
            timer.Reset();
            timer.Start();

            var maxEntityId = context.ResolvedEntityNumbers.Count > 0 ? context.ResolvedEntityNumbers.Max() + 1 : 1;
            context.ResolvedEntityNumbers.Add(maxEntityId);

            if (context.ErrorIds.Any())
            {
                var processError = new TransformBlock<ResultBlock, SaveBlock>(ProcessResult, dataFlowParallelTasksOption);
                var saveErrorInternally = new ActionBlock<SaveBlock>(SaveResult, dataFlowSingleTaskOption);

                processError.LinkTo(saveErrorInternally, linkOptions);

                var resultBlock = new ResultBlock
                {
                    Context = context,
                    Entities = new List<EntityResult>
                    {
                        new EntityResult
                        {
                            Related_Entities = new List<RelatedEntity>(),
                            Resolved_Entity = new ResolvedEntity
                            {
                                Entity_Id = maxEntityId,
                                Records = context.ErrorIds.ToList()
                            }
                        }
                    }
                };
                processError.Post(resultBlock);
                processError.Complete();
                Task.WaitAll(saveErrorInternally.Completion);
            }

            timer.Stop();
            _logger.Information($"Process errors: time {timer.Elapsed.ToString("g")}");
            timer.Reset();
            timer.Start();

            postProcessing.Post(context);
            postProcessing.Complete();

            Task.WaitAll(postProcessing.Completion);

            UpdateData(sourceData, connection, context);

            timer.Stop();
            _logger.Information($"Post processing and update data: time {timer.Elapsed.ToString("g")}");

            context.Report.ExecutionTime = DateTime.Now - StartDate;
            _logger.Information($"ER execution TOTAL time: {context.Report.ExecutionTime.ToString("g")}, Source(s): {context.SourceData.Count} Total records: {context.Report.TotalRecords}");
            foreach (var error in context.Errors)
            {
                if (error.Contains("0037E|Unknown resolved entity value")) continue;
                _logger.Error(error);
            }
            return context.Report;
        }
        finally
        {
            cleanUp.Post(context);
            cleanUp.Complete();
            Task.WaitAll(cleanUp.Completion);
            connection.Close();
        }
    }

    private void Progress(PipelineStepsEnum step, int currentlyProcessed, int totalRecords)
    {
        int percent;
        string message = step.ToString();
        switch (step)
        {
            case PipelineStepsEnum.Initialize:
                percent = 5;
                message = $"Step 1/6 Initializing: {percent}% complete.";
                break;
            case PipelineStepsEnum.Add:
                Interlocked.Add(ref AddedRecords, currentlyProcessed);
                percent = (int)(((double)AddedRecords / totalRecords) * 60) + 5; //adding records 60% of the progress
                message = $"Step 2/6 Loading & Analyzing {AddedRecords} from {totalRecords} records: {percent}% complete.";
                if (_erSettings.EnableDebugLogs && percent > _LastReportedPercent)
                {
                    _LastReportedPercent = percent;
                    File.AppendAllLines(_senzingReportPath, new[] { _szEngine.GetStats() }, Encoding.UTF8);
                }
                break;
            case PipelineStepsEnum.Fetch:
                Interlocked.Add(ref FetchedRecords, currentlyProcessed);
                percent = (int)(((double)FetchedRecords / totalRecords) * 25) + 65; //fetching records 30% of the progress
                message = $"Step 3/6 Preparing results {FetchedRecords} from {totalRecords} records: {percent}% complete";
                if (_erSettings.EnableDebugLogs && percent > _LastReportedPercent)
                {
                    _LastReportedPercent = percent;
                    File.AppendAllLines(_senzingReportPath, new[] { _szEngine.GetStats() }, Encoding.UTF8);
                }
                break;
            case PipelineStepsEnum.ProcessPossibleRelated:
                percent = (int)(((double)currentlyProcessed / totalRecords) * 3) + 90;
                message = $"Step 4/6 Processing possible related records {currentlyProcessed} / {totalRecords}";
                break;
            case PipelineStepsEnum.ProcessPossibleDuplicated:
                percent = (int)(((double)currentlyProcessed / totalRecords) * 3) + 93;
                message = $"Step 5/6 Processing possible duplicated records {currentlyProcessed} / {totalRecords}";
                break;
            case PipelineStepsEnum.SavePossibilities:
                percent = 96;
                message = "Step 6/6 Saving possibilities";
                break;
            case PipelineStepsEnum.UpdatePossibilities:
                percent = 98;
                message = "Step 6/6 Updating possibilities";
                break;
            case PipelineStepsEnum.CleanUp:
                message = "Cleaning up";
                percent = 99;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(step), step, null);
        }

        //Notify progress
        _onProgressAction(message, percent);
    }

    private Dictionary<long, string> ConvertDataTableToRecords(DataTable data)
    {
        if (data == null || data.Rows.Count == 0)
            return new Dictionary<long, string>(0);

        if (!data.Columns.Contains(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY))
            throw new ArgumentException($"Column '{WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY}' not found in DataTable.", nameof(data));

        // Cache columns and their type codes once
        var cols = data.Columns.Cast<DataColumn>().ToArray();
        var typeCodes = new TypeCode[cols.Length];

        for (int i = 0; i < cols.Length; i++)
            typeCodes[i] = Type.GetTypeCode(cols[i].DataType);

        var result = new Dictionary<long, string>(data.Rows.Count);
        var inv = CultureInfo.InvariantCulture;
        var sb = new StringBuilder(4096);

        foreach (DataRow row in data.Rows)
        {
            var key = Convert.ToInt64(row[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY]);

            sb.Clear();
            using (var sw = new StringWriter(sb, inv))
            using (var jw = new JsonTextWriter(sw) { Formatting = Formatting.None, CloseOutput = false })
            {
                jw.WriteStartObject();

                for (int c = 0; c < cols.Length; c++)
                {
                    var col = cols[c];
                    jw.WritePropertyName(col.ColumnName);

                    var val = row[c];
                    if (val == null || val == DBNull.Value)
                    {
                        jw.WriteNull();
                        continue;
                    }

                    switch (typeCodes[c])
                    {
                        case TypeCode.String: jw.WriteValue((string)val); break;
                        case TypeCode.Boolean: jw.WriteValue((bool)val); break;
                        case TypeCode.DateTime: jw.WriteValue(((DateTime)val).ToString("yyyy-MM-ddTHH:mm:ss", inv)); break;
                        case TypeCode.Int16: jw.WriteValue((short)val); break;
                        case TypeCode.Int32: jw.WriteValue((int)val); break;
                        case TypeCode.Int64: jw.WriteValue((long)val); break;
                        case TypeCode.UInt16: jw.WriteValue((ushort)val); break;
                        case TypeCode.UInt32: jw.WriteValue((uint)val); break;
                        case TypeCode.UInt64: jw.WriteValue((ulong)val); break;
                        case TypeCode.Single: jw.WriteValue((float)val); break;
                        case TypeCode.Double: jw.WriteValue((double)val); break;
                        case TypeCode.Decimal: jw.WriteValue((decimal)val); break;
                        default:
                            jw.WriteValue(val); // fallback для Guid, byte[] и т.п.
                            break;
                    }
                }

                jw.WriteEndObject();
                jw.Flush();
            }

            result.Add(key, sb.ToString());
        }

        return result;
    }

    private SenzingExecutionContext InitiateAndCheckSource(SenzingExecutionContext context)
    {
        var timer = new Stopwatch();
        timer.Start();

        var needPurge = true;

        switch (_erSettings.Database)
        {
            case EntityResolutionDatabase.Internal:
                ConfigurationHelper.InitSqLiteFilesFromTemplate(_erSettings.DataFolder, _configuration, _logger);
                break;
            case EntityResolutionDatabase.Postgres:
                ConfigurationHelper.InitDatabase(ExternalSourceTypes.Postgres, _erSettings.ConnectionString);
                break;
            case EntityResolutionDatabase.SqlServer:
                ConfigurationHelper.InitDatabase(ExternalSourceTypes.SqlServer, _erSettings.ConnectionString);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        context.Report.TotalRecords = context.SourceData.Sum(x => x.RowCount);

        string engineConfig = JsonConvert.SerializeObject(_configuration);

        _szEnvironment = SzCoreEnvironment.NewBuilder().Settings(engineConfig).Build();
        var szConfigManager = _szEnvironment.GetConfigManager();

        var szConfigId = szConfigManager.GetDefaultConfigID();
        var szConfig = (szConfigId == 0) ? szConfigManager.CreateConfig() : szConfigManager.CreateConfig(szConfigId);

        var dataSources = szConfig.GetDataSourceRegistry();

        var jsonObj = JsonNode.Parse(dataSources)?.AsObject();

        var jsonArr = jsonObj?["DATA_SOURCES"]?.AsArray();

        foreach (var data in context.SourceData)
        {
            if (!jsonArr.Any(x => x?["DSRC_CODE"]?.GetValue<string>().ToUpper() == data.TableName.ToUpper()))
            {
                szConfig.RegisterDataSource(data.TableName);
            }
        }

        string configDefinition = szConfig.Export();

        if (szConfigId == 0)
            szConfigManager.SetDefaultConfig(configDefinition);
        else
        {
            var newConfigId = szConfigManager.RegisterConfig(configDefinition);
            szConfigManager.ReplaceDefaultConfigID(szConfigId, newConfigId);
        }

        if (needPurge)
        {
            var diagnostic = _szEnvironment.GetDiagnostic();
            diagnostic.PurgeRepository();
        }

        _szEngine = _szEnvironment.GetEngine();

        context.NotifyProgress(PipelineStepsEnum.Initialize, 0, context.Report.TotalRecords);
        timer.Stop();
        _logger.Information($"Init time {timer.Elapsed.ToString()}");
        return context;
    }

    private IEnumerable<SourceBlock> PrepareSourceBlock(SenzingExecutionContext context)
    {
        int processedRecords = 0;
        foreach (var data in context.SourceData)
        {
            var recordToProcess = (processedRecords + data.RowCount) <= _recordLimit
                ? data.RowCount
                : _recordLimit - processedRecords;

            yield return new SourceBlock
            {
                Context = context,
                Configuration = data,
                RecordToProcess = recordToProcess
            };

            processedRecords += recordToProcess;
            if (processedRecords >= _recordLimit) break;
        }
    }

    private IEnumerable<DataBlock> PrepareDataBlock(SourceBlock source)
    {
        int currentIndex = 1;
        while (currentIndex <= source.RecordToProcess)
        {
            var rangeEnd = currentIndex + AddDataChunkSize - 1 > source.Configuration.RowCount
                ? source.Configuration.RowCount
                : currentIndex + AddDataChunkSize - 1;

            if (rangeEnd > source.RecordToProcess)
            {
                rangeEnd = source.RecordToProcess;
            }

            yield return new DataBlock
            {
                Context = source.Context,
                SourceType = source.Configuration.Source,
                AdditionalParameters = source.Configuration.AdditionalParameters,
                SourceName = source.Configuration.TableName,
                Query = $"{source.Configuration.Query} WHERE {WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY} BETWEEN {currentIndex} and {rangeEnd}"
            };

            currentIndex = rangeEnd + 1;
        }
    }

    private SenzingExecutionContext AddDataToSenzing(DataBlock dataBlock)
    {
        //var timer = new Stopwatch();
        //timer.Start();

        var data = SqLiteHelper.ExecuteQuery(dataBlock.Query, dataBlock.Context.Connection);

        //connection.Close();
        var rows = dataBlock.SourceType != ExternalSourceTypes.JSONL
            ? ConvertDataTableToRecords(data)
            : JsonlConverter.ConvertDataTableToJsonl(new JsonlConversionResult
            {
                Table = data,
                DocumentSchema = dataBlock.AdditionalParameters
            });

        string addResult = string.Empty;
        var affectedEntities = new HashSet<long>();

        foreach (var row in rows)
        {
            if (dataBlock.Context.LicenseLimitReached)
                break;
            addResult = string.Empty;

            try
            {
                addResult = _szEngine.AddRecord(dataBlock.SourceName.ToUpper(), row.Key.ToString(), row.Value, SzFlag.SzWithInfo);
                // TODO: Test it and record warning on such case
                if (string.IsNullOrEmpty(addResult))
                {
                    int numberOfIterate = 0;
                    while (string.IsNullOrEmpty(addResult) && numberOfIterate < 3)
                    {
                        addResult = string.Empty;
                        addResult = _szEngine.AddRecord(dataBlock.SourceName.ToUpper(), row.Key.ToString(), row.Value, SzFlag.SzWithInfo);
                        numberOfIterate++;
                    }
                }
                var info = JsonConvert.DeserializeObject<SenzingAddRecordInfo>(addResult);
                if (info?.Affected_Entities != null)
                {
                    foreach (var e in info.Affected_Entities)
                        affectedEntities.Add(e.Entity_Id);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("9000E|LIMIT: Maximum number of records ingested"))
                {
                    dataBlock.Context.Errors.Add(e.Message);
                    dataBlock.Context.LicenseLimitReached = true;
                }
                else
                {
                    dataBlock.Context.Errors.Add($"DS: {dataBlock.SourceName} Key:{row.Key} Data:{row} Error:{e.Message}  SB: {addResult}");
                    dataBlock.Context.ErrorIds.Add(new EntityRecord
                    {
                        Data_Source = dataBlock.SourceName,
                        Record_Id = row.Key.ToString(),
                        Match_Key = "Error"
                    });
                }
            }
        }

        lock (_uniqueLock)
        {
            _resolvedEntities.UnionWith(affectedEntities);
        }

        dataBlock.Context.NotifyProgress?.Invoke(PipelineStepsEnum.Add, rows.Count, dataBlock.Context.Report.TotalRecords);

        //WriteAdditionalLog("Add data time", timer);

        return dataBlock.Context;
    }

    private void ProcessRedo(SenzingExecutionContext context)
    {
        try
        {
            //var timer = new Stopwatch();
            //timer.Start();
            int iterate = 0;
            var redoRecord = _szEngine.GetRedoRecord();
            string redoResult = string.Empty;

            while (!string.IsNullOrEmpty(redoRecord))
            {
                iterate++;
                redoResult = _szEngine.ProcessRedoRecord(redoRecord, SzFlag.SzWithInfo);

                var result = JsonConvert.DeserializeObject<SenzingAddRecordInfo>(redoResult);
                if (result?.Affected_Entities != null)
                {
                    lock (_uniqueLock)
                    {
                        // Add directly into HashSet to avoid duplicates
                        foreach (var e in result.Affected_Entities)
                            _resolvedEntities.Add(e.Entity_Id);
                    }
                }

                redoRecord = _szEngine.GetRedoRecord();
            }

            //if (iterate > 0)
            //{
            //    WriteAdditionalLog("REDO data time", timer);
            //}
            //else
            //{
            //    timer.Stop();
            //}

        }
        catch (Exception e)
        {
            context.Errors.Add($"REDO Error: {e.Message}");
        }
    }

    private IEnumerable<ResultPreparationBlock> PrepareGetResultFromApiOnDb(SenzingExecutionContext context)
    {
        // Build a sorted snapshot from the HashSet only once here
        var entityIds = context.ResolvedEntityNumbers.ToList();

        while (entityIds.Count > 0)
        {
            List<long> entitiesToProcess;
            if (ProcessResultBlockSize >= entityIds.Count)
            {
                entitiesToProcess = entityIds.ToList();
                entityIds.Clear();
            }
            else
            {
                entitiesToProcess = entityIds.GetRange(0, ProcessResultBlockSize);
                entityIds.RemoveRange(0, ProcessResultBlockSize);
            }

            yield return new ResultPreparationBlock
            {
                Context = context,
                EntityIds = entitiesToProcess
            };
        }
    }

    private ResultBlock GetResultFromApi(ResultPreparationBlock resultBlock)
    {
        //var timer = new Stopwatch();
        //timer.Start();
        var resolvedEntities = new List<EntityResult>();

        foreach (var entityId in resultBlock.EntityIds)
        {
            try
            {
                var entity =
                    _szEngine.GetEntity(entityId, SzFlag.SzEntityIncludeRecordData | SzFlag.SzEntityIncludePossiblyRelatedRelations | SzFlag.SzEntityIncludePossiblySameRelations
                                                  | SzFlag.SzEntityIncludeRelatedMatchingInfo | SzFlag.SzEntityIncludeNameOnlyRelations | SzFlag.SzEntityIncludeDisclosedRelations | SzFlag.SzEntityIncludeRecordMatchingInfo); //TODO: check flags
                var result = JsonConvert.DeserializeObject<EntityResult>(entity);
                resolvedEntities.Add(result);
            }
            catch (Exception e)
            {
                resultBlock.Context.Errors.Add($"Result entity {entityId} error: {e.Message}");
            }
        }

        //WriteAdditionalLog("Get result data time", timer);

        return new ResultBlock
        {
            Context = resultBlock.Context,
            Entities = resolvedEntities
        };
    }

    private IEnumerable<ResultBlock> GetResultFromApi(SenzingExecutionContext context)
    {
        //var timer = new Stopwatch();
        //timer.Start();

        AddedRecords = 0;

        var entityIds = context.ResolvedEntityNumbers.ToList();

        int currentChunk = 0;
        var resolvedEntities = new List<EntityResult>();

        // Iterate without LINQ
        for (int i = 0; i < entityIds.Count; i++)
        {
            var entityId = entityIds[i];
            EntityResult result = null;
            try
            {
                var entity = _szEngine.GetEntity(entityId, SzFlag.SzEntityIncludeRecordData | SzFlag.SzEntityIncludePossiblyRelatedRelations | SzFlag.SzEntityIncludePossiblySameRelations
                                                           | SzFlag.SzEntityIncludeRelatedMatchingInfo | SzFlag.SzEntityIncludeNameOnlyRelations | SzFlag.SzEntityIncludeDisclosedRelations | SzFlag.SzEntityIncludeRecordMatchingInfo);
                result = JsonConvert.DeserializeObject<EntityResult>(entity);
                resolvedEntities.Add(result);
            }
            catch (Exception e)
            {
                context.Errors.Add($"Result entity {entityId} error: {e.Message}");
            }

            currentChunk += result?.Resolved_Entity.Records.Count ?? 0;

            if (currentChunk > ProcessResultChuckSize)
            {
                yield return new ResultBlock
                {
                    Context = context,
                    Entities = new List<EntityResult>(resolvedEntities)
                };
                currentChunk = 0;
                resolvedEntities.Clear();
            }
        }

        //WriteAdditionalLog("Get result data time", timer);

        if (resolvedEntities.Count > 0)
        {
            yield return new ResultBlock
            {
                Context = context,
                Entities = new List<EntityResult>(resolvedEntities)
            };
        }
    }

    private SaveBlock ProcessResult(ResultBlock resultBlock)
    {
        //var timer = new Stopwatch();
        //timer.Start();

        // Compute unique and duplicate groups without LINQ
        var uniqueGroups = new List<long>();
        var groupsWithDuplicate = new List<long>();
        int duplicateRecordsToAdd = 0;

        var entities = resultBlock.Entities;
        for (int i = 0; i < entities.Count; i++)
        {
            var resolved = entities[i].Resolved_Entity;
            int recordsCount = resolved.Records.Count;
            if (recordsCount == 1)
            {
                uniqueGroups.Add(resolved.Entity_Id);
            }
            else if (recordsCount > 1)
            {
                groupsWithDuplicate.Add(resolved.Entity_Id);
                duplicateRecordsToAdd += recordsCount;
            }
        }

        if (uniqueGroups.Count > 0)
        {
            lock (_uniqueLock)
            {
                resultBlock.Context.Report.GroupUnique.AddRange(uniqueGroups);
            }
        }

        if (groupsWithDuplicate.Count > 0)
        {
            lock (_duplicateLock)
            {
                resultBlock.Context.Report.GroupWithDuplicates.AddRange(groupsWithDuplicate);
                resultBlock.Context.Report.DuplicateRecordsCount += duplicateRecordsToAdd;
            }
        }

        // Build table rows without LINQ filters
        var table = resultBlock.Context.ResultTable.Clone();

        for (int i = 0; i < entities.Count; i++)
        {
            var resolvedEntity = entities[i];

            var relatedEntities = resolvedEntity.Related_Entities.Where(x => x.Match_Level == (int)PossibilityType.Related).ToList();
            var duplicatedEntities = resolvedEntity.Related_Entities.Where(x => x.Match_Level == (int)PossibilityType.Duplicated).ToList();

            if (relatedEntities.Any() && relatedEntities.All(x => x.Entity_Id > resolvedEntity.Resolved_Entity.Entity_Id))
            {
                relatedEntities.Add(new RelatedEntity
                {
                    Entity_Id = resolvedEntity.Resolved_Entity.Entity_Id,
                    Match_Level_Code = "POSSIBLY_RELATED",
                    Match_Key = string.Empty
                });
                lock (_possibleRelatedLock)
                {
                    resultBlock.Context.PossibleRelated.Add(relatedEntities.OrderBy(x => x.Entity_Id).ToList());
                }
            }

            if (duplicatedEntities.Any() && duplicatedEntities.All(x => x.Entity_Id > resolvedEntity.Resolved_Entity.Entity_Id))
            {
                duplicatedEntities.Add(new RelatedEntity
                {
                    Entity_Id = resolvedEntity.Resolved_Entity.Entity_Id,
                    Match_Level_Code = "POSSIBLY_SAME",
                    Match_Key = string.Empty
                });

                lock (_possibleDuplicatedLock)
                {
                    resultBlock.Context.PossibleDuplicates.Add(duplicatedEntities.OrderBy(x => x.Entity_Id).ToList());
                }
            }

            // Build rows for each record
            var records = resolvedEntity.Resolved_Entity.Records;
            for (int j = 0; j < records.Count; j++)
            {
                var rec = records[j];
                var newRow = table.NewRow();
                newRow[WinPureColumnNamesHelper.WPCOLUMN_GROUPID] = resolvedEntity.Resolved_Entity.Entity_Id;
                newRow[WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME] = rec.Data_Source;
                newRow[WinPureColumnNamesHelper.WPCOLUMN_ORIGINAL_KEY] = rec.Record_Id;
                newRow[WinPureColumnNamesHelper.WPCOLUMN_MATCH_KEY] = rec.Match_Key;

                table.Rows.Add(newRow);
            }
        }

        //WriteAdditionalLog("Process result time", timer);

        return new SaveBlock
        {
            Data = table,
            Context = resultBlock.Context
        };
    }

    private void SaveResult(SaveBlock saveBlock)
    {
        //var timer = new Stopwatch();
        //timer.Start();

        try
        {
            saveBlock.Context.InsertParameters = SqLiteHelper.AppendDataToDb(saveBlock.Context.Connection, saveBlock.Data, NameHelper.EntityResolutionTable, saveBlock.Context.ColumnList, saveBlock.Context.InsertParameters);
            saveBlock.Context.NotifyProgress?.Invoke(PipelineStepsEnum.Fetch, saveBlock.Data.Rows.Count, saveBlock.Context.Report.TotalRecords);
        }
        catch (Exception e)
        {
            saveBlock.Context.Errors.Add($"Save result error: {e.Message}");
        }

        //WriteAdditionalLog("Save data time", timer);
    }

    private void PostProcessing(SenzingExecutionContext context)
    {
        //_logger.Information("Start processing possibilities");
        //var timer = new Stopwatch();
        //timer.Start();

        var possibleTable = new DataTable(NameHelper.EntityResolutionRelatedTable);
        possibleTable.Columns.Add(new DataColumn(WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYID, typeof(int)));
        possibleTable.Columns.Add(new DataColumn(WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYENTITYID, typeof(long)));
        possibleTable.Columns.Add(new DataColumn(WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYTYPE, typeof(int)));
        possibleTable.Columns.Add(new DataColumn(WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYKEY, typeof(string)));


        var sql = string.Empty;
        var totalCount = context.PossibleRelated.Count;

        for (int i = 0; i < context.PossibleRelated.Count; i++)
        {
            var relatedRow = context.PossibleRelated[i];
            context.Report.RelatedGroupCount++;

            for (int j = 0; j < relatedRow.Count; j++)
            {
                var entity = relatedRow[j];
                var newRow = possibleTable.NewRow();
                newRow[WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYID] = context.Report.RelatedGroupCount;
                newRow[WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYENTITYID] = entity.Entity_Id;
                newRow[WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYTYPE] = entity.Match_Level;
                newRow[WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYKEY] = entity.Match_Key;
                possibleTable.Rows.Add(newRow);
            }

            if (context.Report.RelatedGroupCount % 100 == 0)
            {
                context.NotifyProgress?.Invoke(PipelineStepsEnum.ProcessPossibleRelated, context.Report.RelatedGroupCount, totalCount);
            }
        }

        totalCount = context.PossibleDuplicates.Count;

        for (int i = 0; i < context.PossibleDuplicates.Count; i++)
        {
            var duplicatedRow = context.PossibleDuplicates[i];
            context.Report.PossibleDuplicateGroupCount++;

            for (int j = 0; j < duplicatedRow.Count; j++)
            {
                var entity = duplicatedRow[j];
                var newRow = possibleTable.NewRow();
                newRow[WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYID] = context.Report.PossibleDuplicateGroupCount;
                newRow[WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYENTITYID] = entity.Entity_Id;
                newRow[WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYTYPE] = entity.Match_Level;
                newRow[WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYKEY] = entity.Match_Key;
                possibleTable.Rows.Add(newRow);
            }

            if (context.Report.PossibleDuplicateGroupCount % 100 == 0)
            {
                context.NotifyProgress?.Invoke(PipelineStepsEnum.ProcessPossibleDuplicated, context.Report.PossibleDuplicateGroupCount, totalCount);
            }
        }

        var dropTableSql = SqLiteHelper.GetDropTableQuery(NameHelper.EntityResolutionRelatedTable);
        if (SqLiteHelper.CheckTableExists(NameHelper.EntityResolutionRelatedTable, context.Connection))
        {
            SqLiteHelper.ExecuteNonQuery(dropTableSql, context.Connection);
        }

        context.NotifyProgress?.Invoke(PipelineStepsEnum.SavePossibilities, 0, totalCount);

        SqLiteHelper.SaveDataToDb(context.Connection, possibleTable, NameHelper.EntityResolutionRelatedTable, _logger, true);

        context.NotifyProgress?.Invoke(PipelineStepsEnum.UpdatePossibilities, 0, totalCount);
        //delete records where entities are duplicated between different groups
        sql = $@"DELETE FROM [{NameHelper.EntityResolutionRelatedTable}]
                        WHERE [{WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY}] IN
                        (SELECT p.[{WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY}]
                        FROM [{NameHelper.EntityResolutionRelatedTable}] p
                        INNER JOIN 
                        ( SELECT [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYENTITYID}], [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYTYPE}], MIN([{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYID}]) AS mg FROM [{NameHelper.EntityResolutionRelatedTable}] GROUP BY [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYENTITYID}], [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYTYPE}] HAVING COUNT(*) > 1) sq 
                        ON p.[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYENTITYID}] = sq.[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYENTITYID}] AND p.[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYTYPE}] = sq.[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYTYPE}] AND p.[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYID}] > sq.mg)
                        ";
        SqLiteHelper.ExecuteNonQuery(sql, context.Connection);

        //delete records where there is only one records left after previous cleaning
        sql = $@"DELETE FROM [{NameHelper.EntityResolutionRelatedTable}]
                        WHERE [{WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY}] IN
                        (SELECT p.[{WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY}]
                        FROM [{NameHelper.EntityResolutionRelatedTable}] p
                        INNER JOIN 
                        (SELECT [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYID}], [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYTYPE}] FROM [{NameHelper.EntityResolutionRelatedTable}] GROUP BY [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYID}], [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYTYPE}] HAVING COUNT(*) = 1) sq 
                        ON p.[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYID}] = sq.[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYID}] AND p.[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYTYPE}] = sq.[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYTYPE}])";
        SqLiteHelper.ExecuteNonQuery(sql, context.Connection);


        SqLiteHelper.ExecuteNonQuery(SenzingHelper.GetPossibleRelatedCreateQuery(), context.Connection);
        SqLiteHelper.ExecuteNonQuery(SenzingHelper.GetPossibleDuplicatedCreateQuery(), context.Connection);

        sql = $"SELECT count(*) FROM (SELECT DISTINCT [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYID}] FROM [{NameHelper.EntityResolutionRelatedTable}] WHERE [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYTYPE}] = {(int)PossibilityType.Related})";
        context.Report.RelatedGroupCount = SqLiteHelper.ExecuteScalar<int>(sql, context.Connection);

        sql = $"SELECT count(*) FROM (SELECT DISTINCT [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYID}] FROM [{NameHelper.EntityResolutionRelatedTable}] WHERE [{WinPureColumnNamesHelper.WPCOLUMN_POSSIBILITYTYPE}] = {(int)PossibilityType.Duplicated})";
        context.Report.PossibleDuplicateGroupCount = SqLiteHelper.ExecuteScalar<int>(sql, context.Connection);

        sql = $"SELECT count(*) FROM [{NameHelper.EntityResolutionRelatedView}]";
        context.Report.RelatedRecordsCount = SqLiteHelper.ExecuteScalar<int>(sql, context.Connection);

        sql = $"SELECT count(*) FROM [{NameHelper.EntityResolutionDuplicatedView}]";
        context.Report.PossibleDuplicateRecordsCount = SqLiteHelper.ExecuteScalar<int>(sql, context.Connection);

        //WriteAdditionalLog("Post processing", timer);
    }

    private void CleanUpSenzing(SenzingExecutionContext context)
    {
        context.NotifyProgress(PipelineStepsEnum.CleanUp, 0, context.Report.TotalRecords);

        _szEnvironment.Destroy();
        var files = Directory.GetFiles(_erSettings.DataFolder);
        foreach (var file in files.Where(x => Path.GetFileNameWithoutExtension(x).StartsWith("G2C") && !x.EndsWith(ConfigurationHelper.DbTemplate)))
        {
            FileHelper.SafeDeleteFile(file);
        }
    }

    private void PrepareResultTable(List<EntityResolutionConfiguration> sourceData, SenzingExecutionContext context, SQLiteConnection connection)
    {
        var resultTable = new DataTable(NameHelper.EntityResolutionTable);
        resultTable.Columns.Add(new DataColumn(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY, typeof(long)));
        resultTable.Columns.Add(new DataColumn(WinPureColumnNamesHelper.WPCOLUMN_GROUPID, typeof(int)));
        resultTable.Columns.Add(new DataColumn(WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME, typeof(string)));
        resultTable.Columns.Add(new DataColumn(WinPureColumnNamesHelper.WPCOLUMN_ORIGINAL_KEY, typeof(long)));
        resultTable.Columns.Add(new DataColumn(WinPureColumnNamesHelper.WPCOLUMN_ISMASTER, typeof(bool)) { DefaultValue = false });
        resultTable.Columns.Add(new DataColumn(WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED, typeof(bool)) { DefaultValue = false });
        resultTable.Columns.Add(new DataColumn(WinPureColumnNamesHelper.WPCOLUMN_MATCH_KEY, typeof(string)));

        foreach (var source in sourceData)
        {
            var columnsToUpdate = string.Empty;
            var columnsFromUpdate = string.Empty;
            foreach (var rowConfiguration in source.Rows.Where(x => !x.IsIgnore))
            {
                if (rowConfiguration.ColumnName == WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY)
                {
                    continue;
                }

                var columnName = SenzingHelper.GetSenzingFieldName(rowConfiguration);

                columnsToUpdate = string.IsNullOrEmpty(columnsToUpdate)
                    ? $"\"{columnName}\""
                    : columnsToUpdate + $", \"{columnName}\"";

                columnsFromUpdate = string.IsNullOrEmpty(columnsFromUpdate)
                    ? $"\"{rowConfiguration.ColumnName}\""
                    : columnsFromUpdate + $", \"{rowConfiguration.ColumnName}\"";

                if (!resultTable.Columns.Contains(columnName))
                {
                    resultTable.Columns.Add(new DataColumn(columnName, typeof(string)));
                }
            }

            source.UpdateQuery = $"UPDATE {NameHelper.EntityResolutionTable} SET ({columnsToUpdate}) = (SELECT {columnsFromUpdate} FROM {source.TableName} WHERE {source.TableName}.{WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY}= {NameHelper.EntityResolutionTable}.{WinPureColumnNamesHelper.WPCOLUMN_ORIGINAL_KEY}) WHERE {NameHelper.EntityResolutionTable}.\"{WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME}\" = '{source.TableName.ToUpper()}' ";
        }

        SqLiteHelper.ExecuteNonQuery($"DROP VIEW IF EXISTS [{NameHelper.EntityResolutionRelatedView}]", connection);
        SqLiteHelper.ExecuteNonQuery($"DROP VIEW IF EXISTS [{NameHelper.EntityResolutionDuplicatedView}]", connection);

        context.ColumnList = SqLiteHelper.CreateTableSchema(connection, resultTable);

        context.ResultTable = resultTable;
    }

    private void UpdateData(List<EntityResolutionConfiguration> sourceData, SQLiteConnection connection, SenzingExecutionContext context)
    {
        for (int i = 0; i < sourceData.Count; i++)
        {
            var source = sourceData[i];
            SqLiteHelper.ExecuteNonQuery(source.UpdateQuery, connection);
            var updateSourceName = $"UPDATE {NameHelper.EntityResolutionTable} SET \"{WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME}\" = '{source.TableDisplayName}' WHERE \"{WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME}\" = '{source.TableName.ToUpper()}'";
            SqLiteHelper.ExecuteNonQuery(updateSourceName, connection);
        }
        //SqLiteHelper.RemoveColumn(connection, NameHelper.EntityResolutionTable, new List<DataField> { new DataField { DatabaseName = WinPureColumnNamesHelper.WPCOLUMN_ORIGINAL_KEY } });
    }

    private void WriteAdditionalLog(string caption, Stopwatch timer)
    {
        timer.Stop();
        if (_erSettings.EnableDebugLogs)
        {
            _logger.Information($"{caption}: {timer.Elapsed.ToString()}");
        }
    }
}