using System.Data;
using WinPure.Common.Exceptions;
using WinPure.Common.Helpers;
using WinPure.Configuration.DependencyInjection;
using WinPure.Matching.Models;
using WinPure.Matching.Models.Support;
using WinPure.Matching.Properties;

namespace WinPure.Matching.DependencyInjection;

internal static partial class WinPureMatchingDependencyExtension
{
    private class DataNormalizationService : IDataNormalizationService
    {
        private readonly IWpLogger _logger;
        private readonly IDictionaryService _dictionaryService;
        private readonly IRepresentationService _representationService;

        public DataNormalizationService()
        {
            var dependency = new WinPureConfigurationDependency();
            var serviceProvider = dependency.ServiceProvider;
            _logger = serviceProvider.GetService(typeof(IWpLogger)) as IWpLogger;
            _dictionaryService = serviceProvider.GetService(typeof(IDictionaryService)) as IDictionaryService;
            _representationService =
                serviceProvider.GetService(typeof(IRepresentationService)) as IRepresentationService;
        }

        public DataNormalizationService(IWpLogger logger, IDictionaryService dictionaryService, IRepresentationService representationService)
        {
            _logger = logger;
            _dictionaryService = dictionaryService;
            _representationService = representationService;
        }

        public bool DefineMasterRecord(DataTable matchResult, MatchParameter lastMatchingParameters, MasterRecordSettings settings)
        {
            if (matchResult != null)
            {
                var addedFields = new List<string>();
                try
                {
                    //remove all previous master records if exists. 
                    //matchResult.AsEnumerable()
                    //    .Where(x => x.Field<bool>(WinPureColumnNamesHelper.WPCOLUMN_ISMASTER))
                    //    .AsParallel()
                    //    .ForAll(x => x.SetField(WinPureColumnNamesHelper.WPCOLUMN_ISMASTER, false));

                    foreach (var row in matchResult.AsEnumerable()
                                 .Where(x => x.Field<bool>(WinPureColumnNamesHelper.WPCOLUMN_ISMASTER)))
                    {
                        row.SetField(WinPureColumnNamesHelper.WPCOLUMN_ISMASTER, false);
                    }

                    if (settings.RecordType == MasterRecordType.ClearAll)
                    {
                        return true;
                    }

                    var masterRecordsData = matchResult.AsEnumerable(); //crazy but we need that double AsEnumerable

                    if (settings.Rules.Any())
                    {
                        if (settings.IsAllRules)
                        {
                            DefineMasterRecordWithRulesAndCondition(settings, masterRecordsData, matchResult,
                                lastMatchingParameters);
                        }
                        else
                        {
                            DefineMasterRecordWithRulesOrCondition(settings, matchResult, masterRecordsData,
                                settings.Rules.ToList(), lastMatchingParameters, addedFields);
                        }
                    }

                    if (!settings.ApplyOptionsIfRuleGiveNothing /*|| settings.RecordType == MasterRecordType.MostPopulatedByTable && settings.OnlyThisTable*/)
                    {
                        return true;
                    }

                    var groupWithoutMasterRecord = GetRowWithoutMasterRecord(masterRecordsData);

                    if (!groupWithoutMasterRecord.Any())
                    {
                        return true;
                    }

                    switch (settings.RecordType)
                    {
                        case MasterRecordType.MostPopulatedByTable:
                            var indexesOfSystemColumn = new List<int>();
                            for (int i = 0; i < matchResult.Columns.Count; i++)
                            {
                                if (ColumnHelper.IsSystemField(matchResult.Columns[i].ColumnName))
                                {
                                    indexesOfSystemColumn.Add(i);
                                }
                            }

                            DefineMasterRecords(groupWithoutMasterRecord, settings.PreferredTable, settings.OnlyThisTable, indexesOfSystemColumn);
                            break;
                        case MasterRecordType.MostRelevant:
                            DefineMasterRecords(groupWithoutMasterRecord, lastMatchingParameters);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Debug("DEFINE MASTER RECORD ERROR", ex);
                    throw;
                }
                finally
                {
                    foreach (var fld in addedFields)
                    {
                        matchResult.Columns.Remove(fld);
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Delete rows from the match result accrding to mergeSettings.
        /// </summary>
        /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
        /// <param name="deleteSettings">Delete setting</param>
        /// <returns>New data table with match result. Existing match result table should be overwriting</returns>
        public DataTable DeleteMergeMatchResult(DataTable matchResult, DeleteFromMatchResultSetting deleteSettings)
        {
            var resultData = matchResult;
            switch (deleteSettings.DeleteSetting)
            {
                case DeleteMatchResultSetting.AllMatching:
                    var singles = (from myRow in matchResult.AsEnumerable()
                            select new { GroupId = myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID) })
                        .GroupBy(
                            x => x.GroupId)
                        .Select(x => new { gId = x.Key, cnt = x.Count() })
                        .Where(x => x.cnt == 1)
                        .Select(x => x.gId)
                        .ToList();

                    if (singles.Any())
                    {
                        var res = (from myRow in matchResult.AsEnumerable()
                            join di in singles on myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID) equals di
                            orderby myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID)
                            select myRow);
                        resultData = res.CopyToDataTable();
                    }
                    else
                    {
                        matchResult.Rows.Clear();
                    }

                    break;

                case DeleteMatchResultSetting.NonMaster:
                    var nonMasterRecord =
                        matchResult.AsEnumerable()
                            .Where(x => x.Field<bool>(WinPureColumnNamesHelper.WPCOLUMN_ISMASTER));

                    if (nonMasterRecord.Any())
                    {
                        resultData = nonMasterRecord.CopyToDataTable();
                    }
                    else
                    {
                        matchResult.Rows.Clear();
                    }

                    break;

                case DeleteMatchResultSetting.AllSelected:
                    var selectedRecords =
                        matchResult.AsEnumerable()
                            .Where(x => !x.Field<bool>(WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED));

                    if (selectedRecords.Any())
                    {
                        resultData = selectedRecords.CopyToDataTable();
                    }
                    else
                    {
                        matchResult.Rows.Clear();
                    }

                    break;

                case DeleteMatchResultSetting.NonMatching:
                    var duplicateIds = (from myRow in matchResult.AsEnumerable()
                            select new { GroupId = myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID) })
                        .GroupBy(x => x.GroupId)
                        .Select(x => new { gId = x.Key, cnt = x.Count() })
                        .Where(x => x.cnt > 1)
                        .Select(x => x.gId)
                        .ToList();

                    if (duplicateIds.Any())
                    {
                        var res = (from myRow in matchResult.AsEnumerable()
                            join di in duplicateIds on myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID)
                                equals di
                            orderby myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID)
                            select myRow);
                        resultData = res.CopyToDataTable();
                    }
                    else
                    {
                        matchResult.Rows.Clear();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return resultData;

        }

        /// <summary>
        /// Merging of match result.
        /// </summary>
        /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
        /// <param name="mergeSettings">Merge settings for column from match result</param>
        /// <param name="valueSeparator">character o string to split values from merged rows</param>
        /// <param name="reportProgress">action called to report about task progress</param>
        /// <returns></returns>
        public DataTable MergeMatchResult(DataTable matchResult, List<MergeMatchResultSetting> mergeSettings,
            string valueSeparator, Action<string, int> reportProgress)
        {
            reportProgress(Resources.CAPTION_DATA_PREPARING, 5);
            foreach (var setting in mergeSettings)
            {
                if (setting.KeepAllValues)
                {
                    var colName = setting.FieldName + WinPureColumnNamesHelper.WPCOLUMN_ALLVALUES_SUFFIX;
                    if (!matchResult.Columns.Contains(colName))
                    {
                        var col = new DataColumn(colName, Type.GetType("System.String"));
                        matchResult.Columns.Add(col);
                        col.SetOrdinal(matchResult.Columns[setting.FieldName].Ordinal + 1);
                    }
                }
            }

            var dtp = _representationService.GetMatchResult(matchResult, MatchResultViewType.OnlyGroup).AsEnumerable();
            // we should process only groups with more then 1 records.
            var groupIds = dtp
                .Where(x => x.Field<bool>(WinPureColumnNamesHelper.WPCOLUMN_ISMASTER))
                .Select(x => x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID))
                .ToList();
            var dataToProcess = dtp.Join(groupIds, d => d.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID), g => g,
                (d, g) => d).AsEnumerable();

            //.Where(x => groupIds.Contains(x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID)));
            reportProgress(Resources.CAPTION_DATA_UPDATING_WAIT, 10);
            int i = 1;

            foreach (var setting in mergeSettings)
            {
                reportProgress(Resources.CAPTION_DATA_UPDATING_WAIT,
                    Convert.ToInt32(i * 80 / Convert.ToDouble(mergeSettings.Count)));
                i++;

                if (!setting.UpdateField)
                {
                    continue;
                }

                var colName = setting.FieldName;

                var updData = dataToProcess.Where(x =>
                        x.Field<object>(colName) != null &&
                        !string.IsNullOrEmpty(x.Field<object>(colName).ToString()))
                    .Select(x => new
                    {
                        GroupId = x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                        data = x.Field<object>(colName),
                        IsMaster = x.Field<bool>(WinPureColumnNamesHelper.WPCOLUMN_ISMASTER)
                    });


                var maxDataCnt = updData.GroupBy(x => new { x.GroupId, x.data })
                    .Select(x => new
                        { x.Key.GroupId, x.Key.data, MaxCnt = x.Count(), HasMaster = x.Any(s => s.IsMaster) ? 1 : 0 })
                    .ToList();

                var updValues = (from mdc in maxDataCnt
                    group mdc by mdc.GroupId
                    into dptgrp
                    let topCnt = dptgrp.Max(x => x.MaxCnt)
                    select new UpdateDataModel
                    {
                        GroupId = dptgrp.Key,
                        DataValue = dptgrp.Where(y => y.MaxCnt == topCnt).OrderByDescending(x => x.HasMaster).First()
                            .data
                    }).ToList();

                var toUpd = matchResult.AsEnumerable()
                    .Where(x => x.Field<bool>(WinPureColumnNamesHelper.WPCOLUMN_ISMASTER))
                    //&& ((onlyEmpty && (x.Field<object>(colName) == null || string.IsNullOrEmpty(x.Field<string>(colName)))) || !onlyEmpty))
                    .Join(updValues, r => r.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID), d => d.GroupId,
                        (r, d) => new { r, d });

                var toUpd2 = toUpd.ToList();
                toUpd2.ForEach(x =>
                {
                    if (!setting.OnlyEmpty ||
                        (setting.OnlyEmpty && (x.r.Field<object>(colName) == null ||
                                               string.IsNullOrEmpty(x.r.Field<object>(colName).ToString()))))
                    {
                        x.r.SetField(colName, x.d.DataValue);
                    }

                    if (setting.KeepAllValues)
                    {
                        var colNameAll = colName + WinPureColumnNamesHelper.WPCOLUMN_ALLVALUES_SUFFIX;
                        var allVal = string.Join(valueSeparator,
                            maxDataCnt.Where(d => d.GroupId == x.d.GroupId).Select(s => s.data.ToString()));
                        x.r.SetField(colNameAll, allVal);
                    }
                });
            }

            var rowsToDelete = matchResult.AsEnumerable().Where(x =>
                groupIds.Contains(x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID)) &&
                !x.Field<bool>(WinPureColumnNamesHelper.WPCOLUMN_ISMASTER));
            reportProgress(Resources.CAPTION_PREPARING_FOR_DISPLAY, 90);
            return matchResult.AsEnumerable().Except(rowsToDelete).CopyToDataTable();
        }

        /// <summary>
        /// Update the result according to specified mergeSettings. 
        /// </summary>
        /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
        /// <param name="updateSettings">Update settings for columns from match result</param>
        /// <param name="reportProgress">action called to report about task progress</param>
        public void UpdateMatchResult(DataTable matchResult, List<UpdateMatchResultSetting> updateSettings,
            Action<string, int> reportProgress)
        {
            reportProgress(Resources.CAPTION_DATA_PREPARING, 5);
            var dataToProcess = _representationService.GetMatchResult(matchResult, MatchResultViewType.OnlyGroup);
            // we should process only groups with more then 1 records.
            reportProgress(Resources.CAPTION_DATA_UPDATING_WAIT, 10);
            int i = 1;
            foreach (var setting in updateSettings)
            {
                reportProgress(Resources.CAPTION_DATA_UPDATING_WAIT,
                    Convert.ToInt32(i * 80 / Convert.ToDouble(updateSettings.Count)));
                i++;
                if (setting.Operation == UpdateOperationType.NotUpdate)
                {
                    continue;
                }

                List<UpdateDataModel> updValues;

                var updData = dataToProcess.AsEnumerable()
                    .Where(
                        x =>
                            x.Field<object>(setting.FieldName) != null &&
                            !string.IsNullOrEmpty(x.Field<object>(setting.FieldName).ToString()))
                    .Select(x => new
                    {
                        GroupId = x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                        data = x.Field<object>(setting.FieldName),
                        IsMaster = x.Field<bool>(WinPureColumnNamesHelper.WPCOLUMN_ISMASTER)
                    });

                switch (setting.Operation)
                {
                    case UpdateOperationType.MostPopular:
                        var maxDataCnt = updData.GroupBy(x => new { x.GroupId, x.data })
                            .Select(x => new { x.Key.GroupId, x.Key.data, MaxCnt = x.Count() }).ToList();

                        updValues = (from mdc in maxDataCnt
                            group mdc by mdc.GroupId
                            into dptgrp
                            let topCnt = dptgrp.Max(x => x.MaxCnt)
                            select new UpdateDataModel
                            {
                                GroupId = dptgrp.Key,
                                DataValue = dptgrp.First(y => y.MaxCnt == topCnt).data
                            }).ToList();

                        break;
                    case UpdateOperationType.FromMaster:
                        updValues = updData.Where(x => x.IsMaster && !string.IsNullOrEmpty(x.data.ToString()))
                            .Select(x => new UpdateDataModel { GroupId = x.GroupId, DataValue = x.data })
                            .ToList();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var toUpd = matchResult.AsEnumerable()
                    .Join(updValues, r => r.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID), d => d.GroupId,
                        (r, d) => new { r, d });

                if (setting.OnlyEmpty)
                {
                    toUpd = toUpd.Where(x =>
                        x.r.Field<object>(setting.FieldName) == null ||
                        string.IsNullOrEmpty(x.r.Field<object>(setting.FieldName).ToString()));
                }

                var toUpd2 = toUpd.ToList();
                toUpd2.ForEach(x => { x.r.SetField(setting.FieldName, x.d.DataValue); });
            }

            reportProgress(Resources.CAPTION_PREPARING_FOR_DISPLAY, 90);
        }

        public void UpdateMatchGroups(DataTable matchGroups, List<UpdateMatchResultSetting> updateSettings,
            Action<string, int> reportProgress)
        {
            reportProgress(Resources.CAPTION_DATA_UPDATING_WAIT, 10);
            int i = 1;
            foreach (var setting in updateSettings)
            {
                reportProgress(Resources.CAPTION_DATA_UPDATING_WAIT,
                    Convert.ToInt32(i * 80 / Convert.ToDouble(updateSettings.Count)));
                i++;
                if (setting.Operation == UpdateOperationType.NotUpdate)
                {
                    continue;
                }

                List<UpdateDataModel> updValues;

                var updData = matchGroups.AsEnumerable()
                    .Where(
                        x =>
                            x.Field<object>(setting.FieldName) != null &&
                            !string.IsNullOrEmpty(x.Field<object>(setting.FieldName).ToString()))
                    .Select(x => new
                    {
                        GroupId = x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                        data = x.Field<object>(setting.FieldName),
                        IsMaster = x.Field<bool>(WinPureColumnNamesHelper.WPCOLUMN_ISMASTER)
                    });

                switch (setting.Operation)
                {
                    case UpdateOperationType.MostPopular:
                        var maxDataCnt = updData.GroupBy(x => new { x.GroupId, x.data })
                            .Select(x => new { x.Key.GroupId, x.Key.data, MaxCnt = x.Count() }).ToList();

                        updValues = (from mdc in maxDataCnt
                            group mdc by mdc.GroupId
                            into dptgrp
                            let topCnt = dptgrp.Max(x => x.MaxCnt)
                            select new UpdateDataModel
                            {
                                GroupId = dptgrp.Key,
                                DataValue = dptgrp.First(y => y.MaxCnt == topCnt).data
                            }).ToList();

                        break;
                    case UpdateOperationType.FromMaster:
                        updValues = updData.Where(x => x.IsMaster && !string.IsNullOrEmpty(x.data.ToString()))
                            .Select(x => new UpdateDataModel { GroupId = x.GroupId, DataValue = x.data })
                            .ToList();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var toUpd = matchGroups.AsEnumerable()
                    .Join(updValues, r => r.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID), d => d.GroupId,
                        (r, d) => new { r, d });

                if (setting.OnlyEmpty)
                {
                    toUpd = toUpd.Where(x =>
                        x.r.Field<object>(setting.FieldName) == null ||
                        string.IsNullOrEmpty(x.r.Field<object>(setting.FieldName).ToString()));
                }

                var toUpd2 = toUpd.ToList();
                toUpd2.ForEach(x => { x.r.SetField(setting.FieldName, x.d.DataValue); });
            }
        }

        /// <summary>
        /// Remove not duplicate rows from match result.
        /// </summary>
        /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
        public void RemoveNotDuplicateRecords(DataTable matchResult)
        {
            if (matchResult.Rows.Count == 0)
            {
                return;
            }

            var nextGroupId = (from myRow in matchResult.AsEnumerable()
                    select new { GroupId = myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID) })
                .Max(x => x.GroupId) + 1;

            var notDuplicate = matchResult.AsEnumerable()
                .Where(x => x.Field<bool>(WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED));

            foreach (DataRow dataRow in notDuplicate)
            {
                dataRow[WinPureColumnNamesHelper.WPCOLUMN_GROUPID] = nextGroupId++;
                dataRow[WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED] = false;
            }
        }


        /// <summary>
        /// Remove not duplicate rows from match result.
        /// </summary>
        /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
        public void CreateNewDuplicateGroup(DataTable matchResult)
        {
            if (matchResult.Rows.Count == 0)
            {
                return;
            }

            var nextGroupId = (from myRow in matchResult.AsEnumerable()
                    select new { GroupId = myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID) })
                .Max(x => x.GroupId) + 1;

            var duplicate = matchResult.AsEnumerable()
                .Where(x => x.Field<bool>(WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED));

            foreach (DataRow dataRow in duplicate)
            {
                dataRow[WinPureColumnNamesHelper.WPCOLUMN_GROUPID] = nextGroupId;
                dataRow[WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED] = false;
                dataRow[WinPureColumnNamesHelper.WPCOLUMN_ISMASTER] = false;
            }
        }

        private void DefineMasterRecords(IEnumerable<DataRow> data, string preferredTable, bool onlyPreferred, List<int> indexesOfSystemFields)
        {
            var items = data.AsParallel()
                .Where(x => !onlyPreferred || (onlyPreferred &&
                                               x[WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME].ToString() ==
                                               preferredTable))
                .Select(x =>
                {
                    var itemList = x.ItemArray.ToList();

                    return new MasterRecordItem
                    {
                        DataRow = x,
                        IsPreferredTable =
                            (byte)((x[WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME].ToString() == preferredTable &&
                                    !string.IsNullOrEmpty(preferredTable))
                                ? 0
                                : 1),
                        GroupId = (int)x[WinPureColumnNamesHelper.WPCOLUMN_GROUPID],
                        PrimK = (long)x[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY],
                        IsMaster = false,
                        FilledColumnsCount = itemList
                            .Where(i => i != null && !indexesOfSystemFields.Contains(itemList.IndexOf(i)))
                            .Count(i => i.ToString() != "")
                    };
                }).ToList();

            var res = items.GroupBy(x => x.GroupId,
                    (x, y) => new
                    {
                        Key = x,
                        Value = y.OrderBy(v => v.IsPreferredTable).ThenByDescending(v => v.FilledColumnsCount).First()
                    })
                .ToList();

            for (var i = 0; i < res.Count; i++)
            {
                res[i].Value.IsMaster = true;
            }

            for (var i = 0; i < items.Count; i++)
            {
                var el = items[i];
                el.DataRow[WinPureColumnNamesHelper.WPCOLUMN_ISMASTER] = el.IsMaster;
            }
        }

        private void DefineMasterRecords(IEnumerable<DataRow> data, MatchParameter parameters)
        {
            if (parameters == null)
            {
                throw new WinPureArgumentException(
                    "Most relevant master record cannot be defined without match parameters.");
            }

            var parallelOption = new ParallelOptions();
            var param = new MatchParameter(parameters);
            var cols = data.First().Table.Columns;
            for (var g = 0; g < param.Groups.Count; g++)
            {
                var group = param.Groups[g];
                for (var c = 0; c < group.Conditions.Count; c++)
                {
                    var cond = group.Conditions[c];
                    var fldToRemove = cond.Fields.Where(x => !cols.Contains(x.ColumnName)).ToList();
                    var resField = cond.Fields.Except(fldToRemove).ToList();
                    for (var x = 0; x < resField.Count; x++)
                    {
                        resField[x].TableName = "MatchResult";
                    }

                    cond.Fields = resField;
                }
            }

            var items = data.AsParallel().Select(x => new Item
            {
                DataRow = x,
                TableName = "MatchResult",
                GroupId = (int)x[WinPureColumnNamesHelper.WPCOLUMN_GROUPID]
            }).ToList();
            SupportFunctions.SetGetValueInItems(items, param, parallelOption, _dictionaryService);


            var firstElements = new List<FirstElementData>();

            foreach (var grp in parameters.Groups.OrderBy(x => x.GroupId))
            {
                var groupSubQry = items.GroupBy(x => x.GroupValues.First(g => g.GroupId == grp.GroupId).HashCode())
                    .Select(x => x.ToList()).ToList();
                var groupDuplicates = groupSubQry.Where(x => x.Count > 1).ToList();
                var grpFirst = groupDuplicates.Select(x => new FirstElementData
                    { item = x.First(), CountOfDuplicates = x.Count }).ToList();

                firstElements.AddRange(grpFirst);
                var el = groupDuplicates.SelectMany(x => x).ToList();
                items = items.Except(el).ToList();
            }

            for (var i = 0; i < firstElements.Count; i++)
            {
                var el = firstElements[i];
                el.item.CountDuplicates = el.CountOfDuplicates;
            }

            items = items.Concat(firstElements.Select(x => x.item)).ToList();

            var groups = items
                .GroupBy(x => x.GroupId).Select(x => x.Take(1000).ToList())
                .ToList();

            Parallel.ForEach(groups, parallelOption, g =>
                //groups.ForEach(g =>
            {
                var masterItem = MostRelevantItemInGroup(g);
                masterItem.IsMaster = true;
            });
            items.ForEach(x => x.DataRow[WinPureColumnNamesHelper.WPCOLUMN_ISMASTER] = x.IsMaster);
        }

        private static Item MostRelevantItemInGroup(List<Item> items)
        {
            if (items == null || !items.Any())
            {
                return null;
            }

            if (items.Count == 1)
            {
                return items.First();
            }

            var rates = items.Select(x => (double)x.CountDuplicates).ToList();

            if (items.Count > 2)
                for (var i = 0; i < items.Count; i++)
                {
                    for (var j = i + 1; j < items.Count; j++)
                    {
                        var rate = SupportFunctions.RateSimilarity(items[i], items[j]);
                        rates[i] += rate;
                        rates[j] += rate;
                    }
                }

            var maxIndex = rates.Select((value, index) => new { value, index })
                .OrderByDescending(vi => vi.value)
                .Select(x => x.index).First();
            return items[maxIndex];
        }

        private List<DataRow> GetRowWithoutMasterRecord(IEnumerable<DataRow> matchResult)
        {
            var groupsWithMasterRecord =
                matchResult
                    .Where(x => x.Field<bool>(WinPureColumnNamesHelper.WPCOLUMN_ISMASTER))
                    .Select(x => new { grId = x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID) });

            var groupWithoutMasterRecord = matchResult
                .GroupJoin(groupsWithMasterRecord, r => r.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                    g => g.grId, (r, i) => new { r, i })
                .SelectMany(sm => sm.i.DefaultIfEmpty(), (r, i) => new { r.r, i })
                .Where(x => x.i == null)
                .Select(x => x.r)
                .ToList();

            return groupWithoutMasterRecord;
        }

        private void DefineMasterRecordWithRulesOrCondition(MasterRecordSettings settings, DataTable matchResult,
            IEnumerable<DataRow> masterRecordsData, List<MasterRecordRule> rules, MatchParameter lastMatchingParameters,
            List<string> addedFields)
        {
            while (rules.Any())
            {
                var rl = rules.First();
                rules.Remove(rl);
                Func<DataRow, bool> filter = null;

                switch (rl.RuleType)
                {
                    case MasterRecordRuleType.IsEmpty:
                        filter = filter.OrElse(SupportFunctions.GetIsEmptyWhereCondition(rl));
                        break;
                    case MasterRecordRuleType.IsEqual:
                        filter = filter.OrElse(SupportFunctions.GetIsEqualWhereCondition(rl));
                        break;
                    case MasterRecordRuleType.IsContains:
                        filter = filter.OrElse(SupportFunctions.GetContainsWhereCondition(rl));
                        break;
                    case MasterRecordRuleType.GreaterThan:
                        filter = filter.OrElse(SupportFunctions.GetGreaterThenWhereCondition(rl));
                        break;
                    case MasterRecordRuleType.IsMaximum:
                        var qMax = SupportFunctions.GetMaxMin(matchResult, false, rl.FieldName, rl.FieldType);
                        var columnName1 = SupportFunctions.ApplyMaxMinRule(matchResult, rl.FieldName, rl.FieldType,
                            rl.Negate, qMax);
                        filter = filter.OrElse(row => row.Field<bool>(columnName1));
                        addedFields.Add(columnName1);
                        break;
                    case MasterRecordRuleType.IsMinimum:
                        var qMin = SupportFunctions.GetMaxMin(matchResult, true, rl.FieldName, rl.FieldType);
                        var columnName2 = SupportFunctions.ApplyMaxMinRule(matchResult, rl.FieldName, rl.FieldType,
                            rl.Negate, qMin);
                        filter = filter.OrElse(row => row.Field<bool>(columnName2));
                        addedFields.Add(columnName2);
                        break;
                    case MasterRecordRuleType.IsLongest:
                        var qLong =
                            matchResult.AsEnumerable()
                                .Where(x => x.Field<object>(rl.FieldName) != null &&
                                            !string.IsNullOrWhiteSpace(x.Field<string>(rl.FieldName)))
                                .GroupBy(x => x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID))
                                .Select(x => new RuleDeterminedValueModel
                                {
                                    GroupId = x.Key,
                                    Val = x.Max(s => s.Field<string>(rl.FieldName).Length)
                                }).ToList();
                        var columnName3 =
                            SupportFunctions.ApplyLongShortRule(matchResult, rl.FieldName, rl.Negate, qLong);

                        filter = filter.OrElse(row => row.Field<bool>(columnName3));
                        addedFields.Add(columnName3);
                        break;
                    case MasterRecordRuleType.IsShortest:
                        var qShort =
                            matchResult.AsEnumerable()
                                .Where(x => x.Field<object>(rl.FieldName) != null &&
                                            !string.IsNullOrWhiteSpace(x.Field<string>(rl.FieldName)))
                                .GroupBy(x => x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID))
                                .Select(x => new RuleDeterminedValueModel
                                {
                                    GroupId = x.Key,
                                    Val = x.Min(s => s.Field<string>(rl.FieldName).Length)
                                }).ToList();
                        var columnName4 =
                            SupportFunctions.ApplyLongShortRule(matchResult, rl.FieldName, rl.Negate, qShort);

                        filter = filter.OrElse(row => row.Field<bool>(columnName4));
                        addedFields.Add(columnName4);
                        break;
                    case MasterRecordRuleType.Common:
                        var qCommon =
                            matchResult.AsEnumerable()
                                .Where(x => x.Field<object>(rl.FieldName) != null &&
                                            !string.IsNullOrWhiteSpace(x.Field<string>(rl.FieldName)))
                                .GroupBy(x => new
                                {
                                    groupID = x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                                    commonValue = x.Field<object>(rl.FieldName)
                                })
                                .Select(x => new
                                {
                                    GroupId = x.Key.groupID,
                                    CommonValue = x.Key.commonValue,
                                    Count = x.Count()
                                })
                                .GroupBy(x => x.GroupId)
                                .Select(x => new RuleDeterminedValueModel
                                {
                                    GroupId = x.Key,
                                    Val = x.OrderByDescending(s => s.Count).First().CommonValue
                                })
                                .ToList();
                        var columnName =
                            SupportFunctions.ApplyCommonRule(matchResult, rl.FieldName, rl.Negate, qCommon);

                        filter = filter.OrElse(row => row.Field<bool>(columnName));
                        addedFields.Add(columnName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


                var filteredData = masterRecordsData.Where(filter).ToList();

                if (filteredData.Any())
                {
                    var (uniqueCandidate, toBeDefinedWithNextRules) = GetUniqueAndDuplicateGroups(filteredData);

                    filteredData
                        .Join(uniqueCandidate, f => f.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID), c => c,
                            (f, c) => f)
                        .ToList()
                        .ForEach(x => x.SetField(WinPureColumnNamesHelper.WPCOLUMN_ISMASTER, true));

                    if (toBeDefinedWithNextRules.Any())
                    {
                        var newMasterRecordsData = filteredData
                            .Join(toBeDefinedWithNextRules,
                                f => f.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                                c => c, (f, c) => f)
                            .ToList();

                        if (rules.Any())
                        {
                            DefineMasterRecordWithRulesOrCondition(settings, matchResult, newMasterRecordsData,
                                rules.ToList(), lastMatchingParameters, addedFields);
                        }

                        var recordsWithoutMaster = GetRowWithoutMasterRecord(newMasterRecordsData);

                        if (recordsWithoutMaster.Any())
                        {
                            switch (settings.RecordType)
                            {
                                case MasterRecordType.MostPopulatedByTable:
                                    var indexesOfSystemColumn = new List<int>();
                                    for (int i = 0; i < matchResult.Columns.Count; i++)
                                    {
                                        if (ColumnHelper.IsSystemField(matchResult.Columns[i].ColumnName))
                                        {
                                            indexesOfSystemColumn.Add(i);
                                        }
                                    }

                                    DefineMasterRecords(recordsWithoutMaster, settings.PreferredTable,
                                        settings.OnlyThisTable, indexesOfSystemColumn);
                                    break;
                                case MasterRecordType.MostRelevant:
                                    DefineMasterRecords(recordsWithoutMaster, lastMatchingParameters);
                                    break;
                            }
                        }
                    }
                }

                if (rules.Any())
                {
                    var groupsWithoutMasterRecord = GetRowWithoutMasterRecord(matchResult.AsEnumerable());

                    if (!groupsWithoutMasterRecord.Any())
                    {
                        return;
                    }

                    masterRecordsData = groupsWithoutMasterRecord;
                }
            }
        }

        private void DefineMasterRecordWithRulesAndCondition(MasterRecordSettings settings,
            IEnumerable<DataRow> masterRecordsData, DataTable matchResult, MatchParameter lastMatchingParameters)
        {
            Func<DataRow, bool> filter = null;

            foreach (var rl in settings.Rules)
            {
                switch (rl.RuleType)
                {
                    case MasterRecordRuleType.IsEmpty:
                        filter = filter.AndAlso(SupportFunctions.GetIsEmptyWhereCondition(rl));
                        break;
                    case MasterRecordRuleType.IsEqual:
                        filter = filter.AndAlso(SupportFunctions.GetIsEqualWhereCondition(rl));
                        break;
                    case MasterRecordRuleType.IsContains:
                        filter = filter.AndAlso(SupportFunctions.GetContainsWhereCondition(rl));
                        break;
                    case MasterRecordRuleType.GreaterThan:
                        filter = filter.AndAlso(SupportFunctions.GetGreaterThenWhereCondition(rl));
                        break;
                    case MasterRecordRuleType.IsMaximum:
                        var qMax = SupportFunctions.GetMaxMin(matchResult, false, rl.FieldName, rl.FieldType);
                        masterRecordsData = SupportFunctions.ApplyMaxMinRule(masterRecordsData, rl.FieldName,
                            rl.FieldType, rl.Negate, qMax);
                        break;
                    case MasterRecordRuleType.IsMinimum:
                        var qMin = SupportFunctions.GetMaxMin(matchResult, true, rl.FieldName, rl.FieldType);
                        masterRecordsData = SupportFunctions.ApplyMaxMinRule(masterRecordsData, rl.FieldName,
                            rl.FieldType, rl.Negate, qMin);
                        break;
                    case MasterRecordRuleType.IsLongest:
                        var qLong =
                            matchResult.AsEnumerable()
                                .Where(x => x.Field<object>(rl.FieldName) != null &&
                                            !string.IsNullOrWhiteSpace(x.Field<string>(rl.FieldName)))
                                .GroupBy(x => x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID))
                                .Select(x => new RuleDeterminedValueModel
                                {
                                    GroupId = x.Key,
                                    Val = x.Max(s => s.Field<string>(rl.FieldName).Length)
                                }).ToList();
                        masterRecordsData =
                            SupportFunctions.ApplyLongShortRule(masterRecordsData, rl.FieldName, rl.Negate, qLong);
                        break;
                    case MasterRecordRuleType.IsShortest:
                        var qShort =
                            matchResult.AsEnumerable()
                                .Where(x => x.Field<object>(rl.FieldName) != null &&
                                            !string.IsNullOrWhiteSpace(x.Field<string>(rl.FieldName)))
                                .GroupBy(x => x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID))
                                .Select(x => new RuleDeterminedValueModel
                                {
                                    GroupId = x.Key,
                                    Val = x.Min(s => s.Field<string>(rl.FieldName).Length)
                                }).ToList();
                        masterRecordsData =
                            SupportFunctions.ApplyLongShortRule(masterRecordsData, rl.FieldName, rl.Negate, qShort);
                        break;
                    case MasterRecordRuleType.Common:
                        var qCommon =
                            matchResult.AsEnumerable()
                                .Where(x => x.Field<object>(rl.FieldName) != null &&
                                            !string.IsNullOrWhiteSpace(x.Field<string>(rl.FieldName)))
                                .GroupBy(x => new
                                {
                                    groupID = x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                                    commonValue = x.Field<object>(rl.FieldName).ToString()
                                })
                                .Select(x => new
                                {
                                    GroupId = x.Key.groupID,
                                    CommonValue = x.Key.commonValue,
                                    Count = x.Count()
                                })
                                .GroupBy(x => x.GroupId)
                                .Select(x => new RuleDeterminedValueModel
                                {
                                    GroupId = x.Key,
                                    Val = x.OrderByDescending(s => s.Count).First().CommonValue
                                })
                                .ToList();
                        masterRecordsData =
                            SupportFunctions.ApplyCommonRule(masterRecordsData, rl.FieldName, rl.Negate, qCommon);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (filter != null)
            {
                masterRecordsData = masterRecordsData.Where(filter).ToList();
            }

            if (masterRecordsData.Any())
            {
                switch (settings.RecordType)
                {
                    case MasterRecordType.MostPopulatedByTable:
                        var indexesOfSystemColumn = new List<int>();
                        for (int i = 0; i < matchResult.Columns.Count; i++)
                        {
                            if (ColumnHelper.IsSystemField(matchResult.Columns[i].ColumnName))
                            {
                                indexesOfSystemColumn.Add(i);
                            }
                        }

                        DefineMasterRecords(masterRecordsData, settings.PreferredTable, settings.OnlyThisTable,
                            indexesOfSystemColumn);
                        break;
                    case MasterRecordType.MostRelevant:
                        DefineMasterRecords(masterRecordsData, lastMatchingParameters);
                        break;
                }
            }
        }

        private (List<int>, List<int>) GetUniqueAndDuplicateGroups(IEnumerable<DataRow> masterData)
        {
            var recordCountInGroups = masterData.GroupBy(x => x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID))
                .Select(x => new { groupId = x.Key, candidates = x.Count() }).ToList();

            var uniqueCandidate = recordCountInGroups
                .Where(x => x.candidates == 1)
                .Select(x => x.groupId).ToList();

            var groupsWithMultipleRecords = recordCountInGroups
                .Where(x => x.candidates > 1)
                .Select(x => x.groupId).ToList();

            return (uniqueCandidate, groupsWithMultipleRecords);
        }
    }
}