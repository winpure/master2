using System.Data;
using WinPure.Common.Exceptions;
using WinPure.Common.Helpers;
using WinPure.Common.Models;
using WinPure.Matching.Models;
using WinPure.Matching.Models.Reports;
using WinPure.Matching.Properties;

namespace WinPure.Matching.DependencyInjection;

internal static partial class WinPureMatchingDependencyExtension
{
    private class RepresentationService : IRepresentationService
    {
        /// <summary>
        /// Convert DataTable with match result to the specific view, defined with second parameters
        /// </summary>
        /// <param name="matchResult">DataTable with matching result. It should be same table that was returned by MatchData function.</param>
        /// <param name="viewType">Specify the view type of match result</param>
        /// <returns>New DataTable according to selected view type. Match result table should not be overwriting.</returns>
        public DataTable GetMatchResult(DataTable matchResult, MatchResultViewType viewType)
        {
            if (matchResult == null)
            {
                return null;
            }

            if (matchResult.Rows.Count == 0)
            {
                var dv = matchResult.DefaultView;
                dv.Sort = WinPureColumnNamesHelper.WPCOLUMN_GROUPID;
                return dv.ToTable();
            }

            switch (viewType)
            {
                case MatchResultViewType.All:
                    var dv = matchResult.DefaultView;
                    dv.Sort = WinPureColumnNamesHelper.WPCOLUMN_GROUPID;
                    return dv.ToTable();
                case MatchResultViewType.OnlyGroup:
                    var duplicateIds =
                        (from myRow in matchResult.AsEnumerable()
                            select new { GroupId = myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID) })
                        .GroupBy(x => x.GroupId).Select(x => new { gId = x.Key, cnt = x.Count() }).Where(x => x.cnt > 1)
                        .Select(x => x.gId).ToList();
                    if (duplicateIds.Any())
                    {
                        var res = (from myRow in matchResult.AsEnumerable()
                            join di in duplicateIds on myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID)
                                equals di
                            orderby myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID)
                            select myRow);
                        return res.CopyToDataTable();
                    }

                    var tbl = matchResult.Clone();
                    return tbl;
                case MatchResultViewType.NonMatches:
                    var notDuplicateIds =
                        (from myRow in matchResult.AsEnumerable()
                            select new { GroupId = myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID) })
                        .GroupBy(x => x.GroupId).Select(x => new { gId = x.Key, cnt = x.Count() })
                        .Where(x => x.cnt == 1).Select(x => x.gId).ToList();
                    if (notDuplicateIds.Any())
                    {
                        var res = (from myRow in matchResult.AsEnumerable()
                            join di in notDuplicateIds on myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID)
                                equals di
                            orderby myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID)
                            select myRow);
                        return res.CopyToDataTable();
                    }

                    var resultTable = matchResult.Clone();
                    return resultTable;

                case MatchResultViewType.AcrossTable:

                    var acrossds = (from myRow in matchResult.AsEnumerable()
                            select new
                            {
                                GroupId = myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                                TableName = myRow.Field<string>(WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME)
                            })
                        .Distinct()
                        .GroupBy(x => x.GroupId)
                        .Select(x => new { gId = x.Key, cnt = x.Count() })
                        .Where(x => x.cnt > 1)
                        .Select(x => x.gId)
                        .ToList();

                    if (acrossds.Any())
                    {
                        var res = (from myRow in matchResult.AsEnumerable()
                            join di in acrossds on myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID) equals di
                            orderby myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID)
                            select myRow);
                        return res.CopyToDataTable();
                    }

                    var tbl1 = matchResult.Clone();
                    return tbl1; // return empty result

                case MatchResultViewType.TableUnique:
                    var acrossds2 = (from myRow in matchResult.AsEnumerable()
                            select new
                            {
                                GroupId = myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID),
                                TableName = myRow.Field<string>(WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME)
                            })
                        .GroupBy(x => new { x.GroupId })
                        .Select(x => new {x.Key.GroupId, TableName = x.Select(s => s.TableName).Distinct(), rowCount = x.Count()})
                        .Where(x => x.rowCount > 1 && x.TableName.Count() == 1)
                        .Select(x => x.GroupId)
                        .ToList();

                    if (acrossds2.Any())
                    {
                        var res = (from myRow in matchResult.AsEnumerable()
                            join di in acrossds2 on myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID) equals
                                di
                            orderby myRow.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID)
                            select myRow);
                        return res.CopyToDataTable();
                    }

                    var tbl2 = matchResult.Clone();
                    return tbl2; // return empty result
                default:
                    throw new WinPureArgumentException($"View type {viewType} does not exists!");
            }

        }

        /// <summary>
        /// Options table for merge match result opration
        /// </summary>
        /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
        /// <returns>Data table with options populated with default values</returns>
        public DataTable GetMergeMatchResultOptionsTable(DataTable matchResult)
        {
            return GetMatchUpdateOptions(matchResult, UpdateOperationType.MostPopular);
        }

        /// <summary>
        /// Options table match/update operation
        /// </summary>
        /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
        /// <returns>Data table with options populated with default values</returns>
        public DataTable GetUpdateMatchResultOptionsTable(DataTable matchResult)
        {
            return GetMatchUpdateOptions(matchResult, UpdateOperationType.NotUpdate);
        }

        public ReportCommonData GetMatchReportCommonData(DataTable matchResult)
        {
            var items =
                matchResult.Select()
                    .Select(x =>
                        new Item
                        {
                            GroupId = (int)x[WinPureColumnNamesHelper.WPCOLUMN_GROUPID]
                        })
                    .ToList();

            var grp = items.GroupBy(x => x.GroupId).Select(x => new { GrId = x.Key, Cnt = x.Count() })
                .Where(x => x.Cnt > 1).ToList();

            var statMatches = grp.Sum(x => x.Cnt);

            return new ReportCommonData
            {
                TotalRecords = items.Count,
                GroupCount = grp.Count(),
                TotalMatches = statMatches
            };
        }

        public ReportData GetMatchReportData(DataTable matchResult)
        {
            //transform result to item collection
            var items =
                matchResult.Select()
                    .Select(x =>
                        new Item
                        {
                            DataRow = x,
                            TableName = x[WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME].ToString(),
                            GroupId = (int)x[WinPureColumnNamesHelper.WPCOLUMN_GROUPID]
                        })
                    .ToList();

            //create result report data
            var res = new ReportData
            {
                SourceData = items.GroupBy(x => x.TableName)
                    .Select(x => new ReportSourceData { Description = x.Key, RecordsCount = x.Count() }).ToList()
            };

            //prepare data for the calculation result statistic.
            var groups = items.Where(x => x.GroupId.HasValue).ToList();

            // fill data about count of records in the source tables

            // prepare data for statistic how match duplicate records in a source tables.
            var statDupl = groups
                .GroupBy(g => new
                {
                    g.TableName,
                    GroupId = g.GroupId.Value
                })
                .Select(x => new
                {
                    x.Key.TableName,
                    x.Key.GroupId,
                    Cnt = x.Count()
                })
                .Where(x => x.Cnt > 1)
                .GroupBy(g => g.TableName)
                .Select(x => new
                {
                    TableName = x.Key,
                    Cnt = x.Sum(s => s.Cnt - 1)
                });

            // fill information about the duplicate records in source tables
            foreach (var upd in statDupl.Join(res.SourceData, s => s.TableName, r => r.Description,
                         (s, r) => new { stat = s, res = r }))
            {
                upd.res.Duplicates = upd.stat.Cnt;
            }

            var statMatchGroups = groups
                .Select(g => new { g.TableName, GroupId = g.GroupId.Value })
                .Distinct()
                .GroupBy(g => g.GroupId)
                .Select(x => new { grId = x.Key, Cnt = x.Count() }).Where(x => x.Cnt > 1).Select(x => x.grId).ToList();

            var statMatch = groups
                .Join(statMatchGroups, i => i.GroupId.Value, m => m, (i, m) => i)
                .GroupBy(g => new { g.TableName })
                .Select(x => new { x.Key.TableName, Cnt = x.Count() })
                .ToList();

            foreach (var upd in statMatch.Join(res.SourceData, s => s.TableName, r => r.Description,
                         (s, r) => new { stat = s, res = r }))
            {
                upd.res.MatchedRecords = upd.stat.Cnt;
            }

            res.ResultData.Add(new ReportResultData
            {
                Description = Resources.API_REPORT_DISTINCT_RECORD,
                RecordValue = groups.Any() ? groups.Max(x => x.GroupId.Value) : 0
            });

            var grp = groups.GroupBy(x => x.GroupId).Select(x => new { GrId = x.Key, Cnt = x.Count() })
                .Where(x => x.Cnt > 1).ToList();

            // prepare data for statistic how match duplicate records in a source tables.
            var statMatches = grp.Sum(x => x.Cnt);

            res.ResultData.Add(new ReportResultData
            {
                Description = Resources.API_REPORT_MATCHED_RECORDS,
                RecordValue = grp.Any() ? grp.Sum(x => x.Cnt - 1) : 0
            });

            var reportCommonData = new ReportCommonData
            {
                TotalRecords = items.Count,
                GroupCount = grp.Count(),
                TotalMatches = statMatches
            };

            res.CommonData.Add(reportCommonData);
            //fill common report information about total records and total group counts
            res.ViewData.Add(MatchResultViewType.All, reportCommonData);

            return res;
        }

        public MatchParameter ConvertMatchSettingsViewToMatchParameters(MatchSettingsViewModel settings, int searchDeep,
            MatchAlgorithm mAlgorithm)
        {
            var dummyRow =
                settings.MatchParameters.FirstOrDefault(x =>
                    x.FieldName == Resources.CAPTION_DUMMYCONDITION || x.Level == 0);
            while (dummyRow != null)
            {
                settings.MatchParameters.Remove(dummyRow);
                ReindexGroups(settings, dummyRow.GroupId);
                dummyRow = settings.MatchParameters.FirstOrDefault(x =>
                    x.FieldName == Resources.CAPTION_DUMMYCONDITION || x.Level == 0);
            }

            var duplicateColumnInGroup = settings.MatchParameters
                .GroupBy(x => new { x.GroupId, x.FieldName })
                .Select(x => new { Cnt = x.Count() })
                .ToList();

            if (duplicateColumnInGroup.Any(x => x.Cnt > 1))
            {
                throw new WinPureDuplicateColumnInGroupException();
            }

            var param = new MatchParameter
            {
                CheckInternal = !settings.MatchAcrossTables,
                MainTable = settings.AcrossTableMainTable,
                FuzzyAlgorithm = mAlgorithm,
                SearchDeep = searchDeep,
                Groups = settings.MatchParameters.OrderBy(x => x.GroupId).GroupBy(g => g.GroupId).Select(x =>
                    new MatchGroup
                    {
                        GroupId = x.Key,
                        GroupLevel = x.Min(s => s.GroupLevel) / 100.0,
                        Conditions = x.GroupBy(g => new
                        {
                            g.Level,
                            g.Weight,
                            g.Dictionary,
                            g.IsDirect,
                            g.IsFuzzy,
                            g.IncludeNullValue,
                            g.IncludeEmpty,
                            g.FieldName
                        }).Select(s => new MatchCondition
                        {
                            Level = s.Key.Level / 100.0,
                            Weight = s.Key.Weight / 100.0,
                            DictionaryType = s.Key.Dictionary,
                            IncludeNullValues = s.Key.IncludeNullValue,
                            IncludeEmpty = s.Key.IncludeEmpty,
                            MatchingType = (s.Key.IsFuzzy) ? MatchType.Fuzzy : MatchType.DirectCompare,
                            Fields = s.SelectMany(v => v.FieldMap.FieldMap).ToList()
                        }).ToList()
                    }).ToList()
            };

            return param;
        }

        public void ReindexGroups(MatchSettingsViewModel matchSettings, int removedGroupId)
        {
            if (matchSettings.MatchParameters.Any(x => x.GroupId == removedGroupId))
            {
                return;
            }

            foreach (var matchParametersViewModel in matchSettings.MatchParameters.Where(
                         x => x.GroupId > removedGroupId))
            {
                matchParametersViewModel.GroupId--;
            }
        }

        private DataTable GetMatchUpdateOptions(DataTable matchResult, UpdateOperationType operationType)
        {
            if (matchResult == null)
            {
                return null;
            }

            var optTbl = new DataTable("MatchResultMergeOptions");
            optTbl.Columns.Add(new DataColumn("FieldName", typeof(string)));
            optTbl.Columns.Add(new DataColumn("ValueType", typeof(string)));
            optTbl.Columns.Add(new DataColumn("UpdateOption", typeof(bool)));
            optTbl.Columns.Add(new DataColumn("OnlyEmpty", typeof(bool)));
            optTbl.Columns.Add(new DataColumn("SaveAllValues", typeof(bool)));
            var attName = operationType.GetAttributeOfType<DisplayNameAttribute>().DisplayName;

            foreach (DataColumn col in matchResult.Columns)
            {
                if (!ColumnHelper.WinPureSystemFields.Contains(col.ColumnName)
                    && !col.ColumnName.EndsWith(" Score")
                    && !col.ColumnName.EndsWith(WinPureColumnNamesHelper.WPCOLUMN_ALLVALUES_SUFFIX))
                {
                    var r = optTbl.NewRow();
                    r["FieldName"] = col.ColumnName;
                    r["ValueType"] = attName;
                    r["UpdateOption"] = true;
                    r["OnlyEmpty"] = true;
                    r["SaveAllValues"] = false;
                    optTbl.Rows.Add(r);
                }
            }

            return optTbl;
        }
    }
}