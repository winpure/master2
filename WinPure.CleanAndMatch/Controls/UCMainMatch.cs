using DevExpress.Data.Filtering;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Preview;
using System.ComponentModel;
using System.Text.RegularExpressions;
using WinPure.CleanAndMatch.Reports;
using WinPure.Configuration.Helper;
using WinPure.Matching.Algorithm;
using WinPure.Matching.Helpers;
using WinPure.Matching.Models.Reports;

namespace WinPure.CleanAndMatch.Controls;

internal partial class UCMainMatch : XtraUserControl
{
    private IDataManagerService _service;
    private IProjectService _projectService;
    private readonly List<string> _matchTypeFields = new List<string> { "IsFuzzy", "IsDirect" };
    private readonly IWpLogger _logger;
    private readonly IConfigurationService _configurationService;

    public DocumentViewer ReportViewer => matchReportViewer;
    public MatchReport Report { get; private set; }

    public event Action<string, string, MessagesType, Exception> OnException;
    public event Action OnNavigateToFullReport;
    private CriteriaOperator _activeFilter;

    public UCMainMatch()
    {
        InitializeComponent();
        Localization();
        Report = null;
        groupMatchReport.Text = Resources.UI_UCMAINMATCHFORM_MATCHINGRECORDSOPTIONS_2 + " - " + tcMatchResultViewMode.SelectedTabPage?.Text;
       // knowledgebase.Visible = Program.CurrentProgramVersion != ProgramType.CamLte;
        wpSelection.OnSelectionChanged += WpSelection_OnSelectionChanged;

        if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
        {
            _logger = WinPureUiDependencyResolver.Resolve<IWpLogger>();
            _configurationService = WinPureUiDependencyResolver.Resolve<IConfigurationService>();
            var licenseService = WinPureUiDependencyResolver.Resolve<ILicenseService>();
            barMenuExportWithStyle.Enabled = licenseService.ProgramType != ProgramType.CamFree;
            SetBtnRuleFlowCaption();
        }
    }

    #region localization
    private void Localization()
    {
        (gridMatchTables.EmbeddedNavigator.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_TEST;
        gvMatchTables.ViewCaption = Resources.UI_UCMAINMATCHFORM_SELECTTABLES;
        gridColumn1.Caption = Resources.UI_TABLE;
        gridColumn1.ToolTip = Resources.UI_EXPORTMSSQLFORM_TABLENAME;
        //gridMatchTitle.Text = Resources.UI_UCMAINMATCHFORM_SELECTCOLUMNSTOMATCH;
        (btnMatchSettingSetColumnAuto.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_UCMAINMATCHFORM_CLICKAUTOMATICALLYMAP;
        btnMatchSettingSetColumnAuto.Text = Resources.UI_UCMAINMATCHFORM_APPLYCOLUMNMAPPINGS;
        gvMatchParameters.ViewCaption = Resources.UI_UCMAINMATCHFORM_CREATEMATCHDEFINITION;
        colColumnName.Caption = Resources.UI_UPDATEMATCHRESULTFORM_COLUMNNAME;
        colFuzzyMatch.Caption = Resources.UI_UCMAINMATCHFORM_FUZZYMATCH;
        colExactMatch.Caption = Resources.UI_UCMAINMATCHFORM_EXACTMATCH;
        colLevel.Caption = Resources.UI_UCMAINMATCHFORM_FUZZYLEVEL;
        bandedGridColumn1.Caption = Resources.UI_UCMAINMATCHFORM_IGNORENULLVALUES;
        colDictionary.Caption = Resources.UI_UCMAINMATCHFORM_KNOWLEGEBASELIBRARY;
        colGroupId.Caption = Resources.UI_UCMAINMATCHFORM_GROUPID;
        colGroupLevel.Caption = Resources.UI_UCMAINMATCHFORM_GROUPLEVEL;
        colWeightInGroup.Caption = Resources.UI_UCMAINMATCHFORM_WEIGHTGROUP;
        colIgnoreEmpty.Caption = Resources.UI_CAPTION_IGNOREEMPTY;
        (btnMatchSettingStartMatching.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_UCMAINMATCHFORM_CLICKSTARTMATCHINGPROCESS;
        btnMatchSettingStartMatching.Text = Resources.UI_UCMAINMATCHFORM_STARTMATCHING;
        cbMatchSettingAcrossTables.Properties.Caption = Resources.UI_UCMAINMATCHFORM_MATCHACROOSTABLES;
        (cbMatchSettingAcrossTables.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_UCMAINMATCHFORM_SWITCHONOFFMATCINGACROOS;
        lbSearchDeep.Text = Resources.UI_UCMAINMATCHFORM_SEARCHDEEP10;
        (tbMatchSettingSearchDeep.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_UCMAINMATCHFORM_USESEARCHDEEPOPTION;
        (btnMatchSettingRemoveCondition.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_UCMAINMATCHFORM_REMOVESELECTEDFROMMATCH;
        btnMatchSettingRemoveCondition.Text = Resources.UI_UCMAINMATCHFORM_REMOVECONDITION;
        (btnMatchSettingAddCondition.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_UCMAINMATCHFORM_ADDSELECTEDCOLUMNONTOMATCH;
        btnMatchSettingAddCondition.Text = Resources.UI_UCMAINMATCHFORM_ADDCONDITION;
        btnMatchSettingRemoveGroup.Text = Resources.UI_UCMAINMATCHFORM_REMOVEGROUP;
        btnMatchSettingAddGroup.Text = Resources.UI_UCMAINMATCHFORM_ADDGROUP;
        groupControl1.Text = Resources.UI_UCMAINMATCHFORM_MATCHINGRECORDSOPTIONS;
        groupMatchReport.Text = Resources.UI_UCMAINMATCHFORM_MATCHINGRECORDSOPTIONS_2;
        btnExportMatchResult.Text = Resources.UI_EXPORT;

        barButtonExportToCsv.Caption = Resources.UI_DATASOURCE_CSV;
        barButtonExportToXls.Caption = Resources.UI_DATASOURCE_EXCEL;
        barButtonExportToAccess.Caption = Resources.UI_DATASOURCE_ACCESS;
        barButtonExportToSqlServer.Caption = Resources.UI_DATASOURCE_SQLSERVER;
        barButtonExportToMySqlServer.Caption = Resources.UI_DATASOURCE_MYSQL;
        barButtonExportToOracle.Caption = Resources.UI_DATASOURCE_ORACLE;
        barButtonJson.Caption = Resources.UI_DATASOURCE_JSON;
        barButtonXml.Caption = Resources.UI_DATASOURCE_XML;
        barButtonAzure.Caption = Resources.UI_DATASOURCE_AZURE;
        barButtonPostgres.Caption = Resources.UI_DATASOURCE_POSTGRESQL;
        barButtonSqlite.Caption = Resources.UI_DATASOURCE_SQLITE;

        btnSetMasterRecord.Text = Resources.UI_SETMASTERRECORDSFORM_SETMASTERRECORDS;
        btnDeleteMatchedRecord.Text = Resources.UI_UCMAINMATCHFORM_DELETE;
        btnMergeOverwrite.Text = Resources.UI_UCMAINMATCHFORM_UPDATEOVERWRITE;
        lbMainTable.Text = Resources.UI_UCMAINMATCHFORM_MAIN_TABLE;
        btnNotDuplicate.Text = Resources.UI_UCMAINMATCHFORM_NOT_DUPLICATE;
        btnDuplicate.Text = Resources.UI_UCMAINMATCHFORM_DUPLICATE;
        txtGroupCount.Text = Resources.UI_MAINMATCHFORM_GROUP_COUNT;
        txtDuplicates.Text = Resources.UI_MAINMATCHFORM_DUPLICATES;
        labelControl3.Text = Resources.UI_MAINMATCHFORM_MATCHING_TIME;
        labelControl4.Text = Resources.UI_MAINMATCHFORM_DUPLICATES_PERCENT;
        tpAllData.Text = Resources.UI_MAINMATCHFORM_ALLDATA;
        tpAcross.Text = Resources.UI_MAINMATCHFORM_ACROSSTABLES;
        tpUniqueInTable.Text = Resources.UI_MAINMATCHFORM_ACROSSTABLESUNIQUE;
        tpOnlyGroup.Text = Resources.UI_MAINMATCHFORM_ONLYGROUPS;
        tpNotMatched.Text = Resources.UI_MAINMATCHFORM_NONMATCHES;
        btnMergeDuplicates.Text = Resources.UI_MERGE;
       // linkLabel_Report.Text = Resources.UI_FULL_REPORT;
        wpSelection.Initiate(false, null);
        wpSelection.FieldName = WinPureColumnNamesHelper.WPCOLUMN_INCLUDE_IN_RESULT;
        
        lbAlgorithm.Text = Resources.CAPTION_ALGORITHM;

        barMenuDatabase.Caption = Resources.UI_DATASOURCE_DATABASES;
        barMenuFiles.Caption = Resources.UI_DATASOURCE_FILES;
        btnMoveToData.Caption = Resources.UI_CAPTION_MOVETODATA;
        barMenuExportWithStyle.Caption = Resources.UI_CAPTION_EXPORTWITHSTYLE;

        mnuClearAll.Text = Resources.UI_CAPTION_CLEARALL;
        mnuSelectAll.Text = Resources.UI_CAPTION_SELECTALL;
        mnuSelectAllGroup.Text = Resources.UI_CAPTION_SELECTALLGROUP;
        mnuClearAllGroup.Text = Resources.UI_CAPTION_CLEARALLGROUP;
    }
    #endregion

    #region public methods
    public void Initialize()
    {
        _service = WinPureUiDependencyResolver.Resolve<IDataManagerService>();

        _service.OnAddNewData += _service_OnAddNewData;
        _service.OnMatchResultReady += _service_OnMatchResultReady;

        _service.OnTableDelete += _service_OnTableDelete;
        _service.OnChangeTableDisplayName += _service_OnChangeTableDisplayName;
        _service.OnRefreshData += _service_OnRefreshData;

        _projectService = WinPureUiDependencyResolver.Resolve<IProjectService>();
        _projectService.OnBeforeProjectLoad += _service_BeforeProjectLoad;
        _projectService.OnAfterProjectLoad += _service_AfterProjectLoad;

        UpdateDictionaries();
        SetUpGrid(gridMatchColumns);
        SetUpGrid(gridMatchParameters);

        var algorithms = Enum.GetValues(typeof(MatchAlgorithm)).Cast<MatchAlgorithm>();
        foreach (var algorithm in algorithms)
        {
            if (algorithm == MatchAlgorithm.ChapmanLengthDeviation)
            {
                continue;
            }
            cbAlgorithm.Properties.Items.Add(algorithm.ToString());
            cbAlgorithm.SelectedItem = _configurationService.Configuration.FuzzyAlgorithm;
            cbAlgorithm.Refresh();
        }
    }

    public void SetFilterRowVisibility(bool isVisible)
    {
        gvMatchResult.OptionsView.ShowAutoFilterRow = isVisible;
    }

    public void UpdateSystemColumnVisibility()
    {
        var sourceData = gridMatchResult.DataSource as DataTable;
        foreach (GridColumn column in gvMatchResult.Columns)
        {
            if (ColumnHelper.IsSystemField(column.FieldName))
            {
                if (_configurationService.Configuration.ShowSystemFields)
                {
                    column.Visible = true;
                    column.VisibleIndex = sourceData.Columns[column.FieldName].Ordinal;
                }
                else
                {
                    column.Visible = false;
                }
            }
            else
            {
                column.VisibleIndex = sourceData.Columns[column.FieldName].Ordinal;
            }
        }

        var oldColumnWithSummary = gvMatchResult.Columns.FirstOrDefault(x => x.FieldName != WinPureColumnNamesHelper.WPCOLUMN_GROUPID
                                                                             && x.FieldName != WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY
                                                                             && x.Summary.Any());

        oldColumnWithSummary?.Summary.Clear();

        var newColumnWithSummary = gvMatchResult.Columns.Where(x => x.FieldName != WinPureColumnNamesHelper.WPCOLUMN_GROUPID
                                                                    && x.FieldName != WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY
                                                                    && x.Visible).OrderBy(x => x.VisibleIndex).FirstOrDefault();

        if (newColumnWithSummary != null && !newColumnWithSummary.Summary.Any())
        {
            GridColumnSummaryItem item = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, newColumnWithSummary.FieldName, "Count={0}");
            newColumnWithSummary.Summary.Add(item);
        }

        if (gvMatchResult.Columns.Any() && gvMatchResult.Columns.ColumnByFieldName(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY) != null)
        {
            gvMatchResult.Columns[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY].Visible = false;
        }
    }
    

    public void ShowSubPanel(MatchDataMenuItem dType)
    {
        switch (dType)
        {
            case MatchDataMenuItem.Settings: navFrameMatching.SelectedPage = navPageSetting; break;
            case MatchDataMenuItem.MatchResult: navFrameMatching.SelectedPage = navPagematchResult; break;
            case MatchDataMenuItem.Report: navFrameMatching.SelectedPage = navPageMatchReport; break;
        }
    }

    public void StartMatching()
    {
        btnMatchSettingStartMatching_Click(null, null);
    }

    public void UpdateDictionaries()
    {
        repoCBMatchFieldDisctionary.Items.Clear();
        repoCBMatchFieldDisctionary.Items.Add(DictionaryHelper.NO_DICTIONARY);
        var dictionaryService = WinPureUiDependencyResolver.Resolve<IDictionaryService>();
        repoCBMatchFieldDisctionary.Items.AddRange(dictionaryService.GetDictionaryList().Result.Select(x => x.Name).ToList());
    }
    #endregion

    #region Service events
    private void _service_OnRefreshData(ImportedDataInfo tableInfo)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { _service_OnRefreshData(tableInfo); }));
            return;
        }

        if (!tableInfo.IsSelected || !_service.SelectedColumns.Columns.Contains(tableInfo.DisplayName))
        {
            return;
        }

        var oldCoList = _service.SelectedColumns.AsEnumerable()
            .Select(x => x.Field<string>(tableInfo.DisplayName))
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct().ToList();
        var newColList = tableInfo.Fields.Select(x => x.DisplayName).ToList();

        if (newColList.All(x => oldCoList.Contains(x)) &&
            oldCoList.All(x => newColList.Contains(x)))
        {
            return;
        }

        var addedColumns = newColList.Where(x => !oldCoList.Contains(x)).ToList();
        var removedColumns = oldCoList.Where(x => !newColList.Contains(x)).ToList();
        var matchedColumns = _service.MatchSettings.MatchParameters
            .Select(x => x.FieldMap)
            .SelectMany(x => x.FieldMap)
            .Where(x => x.TableName == tableInfo.DisplayName)
            .Select(x => x.ColumnName);

        if (matchedColumns.Any(x => removedColumns.Contains(x)))
        {
            tableInfo.IsSelected = false;
            _service.UpdateSelectedForMatchingTables(tableInfo.TableName);
            UpdateMatchTables();
            UpdateMatchSelectedFields();
            return;
        }

        if (removedColumns.Any())
        {
            var rowsToRemove = _service.SelectedColumns.AsEnumerable()
                .Where(x => removedColumns.Contains(x.Field<string>(tableInfo.DisplayName))).ToList();

            foreach (DataRow row in rowsToRemove)
            {
                _service.SelectedColumns.Rows.Remove(row);
            }
        }

        if (addedColumns.Any())
        {
            foreach (var column in addedColumns)
            {
                var newRow = _service.SelectedColumns.NewRow();
                newRow[tableInfo.DisplayName] = column;
                _service.SelectedColumns.Rows.Add(newRow);
            }
        }

        gridMatchColumns.DataSource = null;
        gvMatchColumns.Columns.Clear();
        gridMatchColumns.DataSource = _service.SelectedColumns;
        gridMatchColumns.Refresh();
        gvMatchColumns.BestFitColumns();
    }

    private void _service_OnChangeTableDisplayName(string arg1, string arg2)
    {
        UpdateMatchTables();
        UpdateMatchSelectedFields(false);
    }

    private void _service_AfterProjectLoad()
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(_service_AfterProjectLoad));
            return;
        }

        tpAcross.PageVisible = tpUniqueInTable.PageVisible = _service.LastMatchingParameters.Groups
            .SelectMany(x => x.Conditions)
            .SelectMany(x => x.Fields)
            .Select(x => x.TableName)
            .Distinct()
            .Count() > 1;

        if (_service.MatchSettings.MatchAcrossTables && tcMatchResultViewMode.SelectedTabPage != tpAcross)
        {
            tcMatchResultViewMode.SelectedTabPage = tpAcross;
        }
        else if (!_service.MatchSettings.MatchAcrossTables && tcMatchResultViewMode.SelectedTabPage == tpAcross)
        {
            tcMatchResultViewMode.SelectedTabPage = tpOnlyGroup;
        }
        else
        {
            UpdateMatchResultTable();
        }

        UpdateMatchReport();

        cbMatchAcrossMainTable.SelectedIndexChanged -= cbMatchAcrossMainTable_SelectedIndexChanged;
        UpdateMatchTables();
        UpdateMatchSelectedFields(false);

        if (_service.MatchSettings.MatchParameters.Any())
        {
            UpdateMatchParameters();
        }
        cbMatchSettingAcrossTables.Checked = _service.MatchSettings.MatchAcrossTables;
        cbMatchAcrossMainTable.SelectedIndex = cbMatchAcrossMainTable.Properties.Items.IndexOf(_service.MatchSettings.AcrossTableMainTable);
        cbMatchAcrossMainTable.Refresh();
        cbMatchAcrossMainTable.SelectedIndexChanged += cbMatchAcrossMainTable_SelectedIndexChanged;
    }

    private void _service_OnTableDelete(ImportedDataInfo tblDataInfo)
    {
        UpdateMatchTables();
        if (tblDataInfo.IsSelected)
        {
            UpdateMatchSelectedFields();
            ClearReport();
            ClearMatchResultTable();
        }
    }

    private void _service_OnAddNewData(string obj)
    {
        UpdateMatchTables();
    }

    private void _service_OnMatchResultReady(bool matchSuccess, bool activateResult, MatchResultOperation operation)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { _service_OnMatchResultReady(matchSuccess, activateResult, operation); }));
            return;
        }

        if (!matchSuccess)
        {
            return;
        }

        if (operation == MatchResultOperation.Merge && tcMatchResultViewMode.SelectedTabPage != tpAllData)
        {
            tcMatchResultViewMode.SelectedTabPage = tpAllData;
        }
        else if (operation == MatchResultOperation.Matching && _service.MatchSettings.MatchAcrossTables && tcMatchResultViewMode.SelectedTabPage != tpAcross)
        {
            tcMatchResultViewMode.SelectedTabPage = tpAcross;
        }
        else if (operation == MatchResultOperation.Matching && !_service.MatchSettings.MatchAcrossTables && tcMatchResultViewMode.SelectedTabPage == tpAcross)
        {
            tcMatchResultViewMode.SelectedTabPage = tpOnlyGroup;
        }
        else
        {
            UpdateMatchResultTable();
        }

        if (operation == MatchResultOperation.Matching)
        {
            tpAcross.PageVisible = tpUniqueInTable.PageVisible = _service.LastMatchingParameters.Groups
                .SelectMany(x => x.Conditions)
                .SelectMany(x => x.Fields)
                .Select(x => x.TableName)
                .Distinct()
                .Count() > 1;
        }
        UpdateMatchParameters();
    }

    private void _service_BeforeProjectLoad()
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(_service_BeforeProjectLoad));
            return;
        }
        ClearReport();
        ClearMatchResultTable();
        ClearMatchParameters();
    }
    #endregion

    #region Inner helpers methods
    private void UpdateMatchTables()
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(UpdateMatchTables));
            return;
        }
        gridMatchTables.DataSource = null;
        gridMatchTables.DataSource = _service.TableList;
        gridMatchTables.Refresh();
        gvMatchColumns.BestFitColumns();
    }

    private void ClearMatchParameters()
    {
        gridMatchColumns.DataSource = null;
        gvMatchColumns.Columns.Clear();
        gridMatchColumns.Refresh();

        gridMatchParameters.DataSource = null;
        gridMatchParameters.Refresh();

        gridMatchTables.DataSource = null;
        gridMatchTables.Refresh();
    }

    private void ClearMatchResultTable()
    {
        gridMatchResult.DataSource = null;
        gvMatchResult.Columns.Clear();
        gridMatchResult.Refresh();
        btnSetMasterRecord.Enabled = btnExportMatchResult.Enabled = btnMergeOverwrite.Enabled = btnDeleteMatchedRecord.Enabled = btnNotDuplicate.Enabled = btnDuplicate.Enabled = btnMergeDuplicates.Enabled = false;
    }

    private void UpdateMatchSelectedFields(bool calculateMatchingFields = true)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { UpdateMatchSelectedFields(calculateMatchingFields); }));
            return;
        }
        cbMatchAcrossMainTable.Properties.Items.Clear();
        cbMatchAcrossMainTable.Properties.Items.AddRange(_service.TableList.Where(x => x.IsSelected).Select(x => x.DisplayName).ToList());
        cbMatchAcrossMainTable.SelectedIndex = 0;
        cbMatchAcrossMainTable.Refresh();
        gridMatchColumns.DataSource = null;
        gvMatchColumns.Columns.Clear();
        gridMatchColumns.DataSource = _service.SelectedColumns;
        gridMatchColumns.Refresh();
        gvMatchColumns.BestFitColumns();

        btnMatchSettingAddCondition.Enabled = btnMatchSettingAddGroup.Enabled = _service.SelectedColumns?.Columns.Count > 0;

        cbMatchSettingAcrossTables.Enabled = gvMatchColumns.Columns.Count > 2; //we should enable this option only if more then one table selected. 
        cbMatchSettingAcrossTables.Checked = cbMatchSettingAcrossTables.Enabled && cbMatchSettingAcrossTables.Checked;
        panelControl5.Visible = cbMatchSettingAcrossTables.Enabled;


        if (_service.SelectedColumns?.Columns.Count == 0)
        {
            btnMatchSettingRemoveGroup.Enabled = false;
            btnMatchSettingRemoveCondition.Enabled = false;
            btnMatchSettingStartMatching.Enabled = false;
        }

        foreach (GridColumn col in gvMatchColumns.Columns)
        {
            if (col.VisibleIndex == 0)
            {
                col.OptionsColumn.AllowEdit = false;
            }
            else if (col.VisibleIndex == gvMatchColumns.Columns.Count - 1)
            {
                col.ColumnEdit = repoCheckEdit;
            }
            else
            {
                var edt = new RepositoryItemComboBox
                {
                    DropDownRows = 15,
                    TextEditStyle = TextEditStyles.DisableTextEditor
                };
                var columns = _service.GetTableColumnsByDisplayName(col.FieldName);
                edt.Items.Add(Resources.SYSTEM_NOT_MAPPED_FIELD);
                edt.Items.AddRange(columns);

                col.ColumnEdit = edt;
                col.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
            }
        }

        if (!calculateMatchingFields)
        {
            return;
        }

        btnMatchSettingSetColumnAuto_Click(null, null);
        _service.MatchSettings.MatchParameters.Clear();
        UpdateMatchParameters();
    }

    private void UpdateMatchParameters()
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(UpdateMatchParameters));
            return;
        }
        gridMatchParameters.DataSource = null;
        gridMatchParameters.DataSource = _service.MatchSettings.MatchParameters;
        gridMatchParameters.Refresh();
        btnMatchSettingStartMatching.Enabled = btnMatchFlow.Enabled = btnMatchSettingRemoveCondition.Enabled = btnMatchSettingRemoveGroup.Enabled = _service.MatchSettings.MatchParameters.Any() && _service.SelectedColumns.Columns.Count > 0;
    }

    private void ProcessLinkedSettings(string column, MatchParametersViewModel rw)
    {
        rw.IsDirect = column == "IsDirect";
        rw.IsFuzzy = column == "IsFuzzy";
    }

    private bool CheckRowData(int rowNumber, DataRowView rw)
    {
        if (rw == null)
        {
            return false;
        }
        int j = 0;
        bool rowHasValue = false;

        while (j < gvMatchColumns.Columns.Count - 1)
        {
            if (rw[j].ToString() == Resources.SYSTEM_NOT_MAPPED_FIELD)
            {
                rw[j] = "";
            }

            if (!string.IsNullOrEmpty(rw[j].ToString())) rowHasValue = true;

            j++;
        }

        if (!rowHasValue)
        {
            gvMatchColumns.DeleteRow(rowNumber);
            return true;
        }
        return false;
    }

    private void CheckColumnMatching(string columnName)
    {
        var tableInfo = _service.TableList.FirstOrDefault(x => x.DisplayName == columnName);
        if (tableInfo == null)
        {
            return;
        }

        var availableColumns = tableInfo.Fields.Select(x => x.DisplayName).ToList();
        var displayedColumns = _service.SelectedColumns.AsEnumerable().Select(x => x.Field<string>(columnName))
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList();

        var notDisplayedColumns = availableColumns.Except(displayedColumns).ToList();
        foreach (var column in notDisplayedColumns)
        {
            var newRow = _service.SelectedColumns.NewRow();
            newRow[columnName] = column;
            _service.SelectedColumns.Rows.Add(newRow);
        }
        gridMatchColumns.RefreshDataSource();
        gvMatchColumns.BestFitColumns();
    }

    private void CheckColumnMatching(int colIndex, int rowIndex, string colValue)
    {
        if (string.IsNullOrEmpty(colValue))
        {
            return;
        }
        int i = 0;

        while (i < gvMatchColumns.RowCount)
        {
            if (i != rowIndex)
            {
                var rw = gvMatchColumns.GetRow(i) as DataRowView;
                if (rw == null)
                {
                    i = gvMatchColumns.RowCount;
                    continue;
                }
                if (rw[colIndex].ToString() == colValue)
                {
                    rw[colIndex] = "";
                    if (CheckRowData(i, rw))
                    {
                        i--;
                    }
                }
            }
            i++;
        }
        _service.MatchSettings.MatchParameters.Clear();
        UpdateMatchParameters();
    }

    private void ClearReport()
    {
        matchReportViewer.DocumentSource = null;
        matchReportViewer.Hide();
        Report?.Dispose();
        Report = null;
    }

    private void UpdateMatchReport()
    {
        try
        {
            groupMatchReport.Text = Resources.UI_UCMAINMATCHFORM_MATCHINGRECORDSOPTIONS_2 + " - " + tcMatchResultViewMode.SelectedTabPage?.Text;

            var currentView = GetMatchResultViewType();

            if (currentView == MatchResultViewType.NonMatches)
            {
                txtDuplicates.Text = Resources.UI_UCMAINMATCHFORM_NOT_DUPLICATE;
            }
            else
            {
                txtDuplicates.Text = Resources.UI_MAINMATCHFORM_DUPLICATES;
            }

            if (_service.ReportData != null && _service.ReportData.CommonData.Any())
            {
                txtMatchTime.Text = _service.ReportData.MatchingTime.ToString(@"dd\:hh\:mm\:ss");

                if (_service.ReportData.ViewData.TryGetValue(currentView, out var currentViewReport))
                {
                    var duplicates = currentView == MatchResultViewType.NonMatches || currentView == MatchResultViewType.TableUnique
                        ? currentViewReport.TotalRecords
                        : currentViewReport.TotalMatches;
                    txtMatchDuplicates.Text = duplicates.ToString();

                    txtMatchDuplicatePercent.Text = string.Format("{0:P2}",
                        Math.Round(
                            (Convert.ToDouble(duplicates) /
                             Convert.ToDouble(_service.ReportData.CommonData.First().TotalRecords)),
                            4));


                    txtMatchGroupCount.Text = currentViewReport.GroupCount.ToString();
                }

                else
                {
                    txtMatchDuplicates.Text = "";
                    txtMatchDuplicatePercent.Text = "";
                    txtMatchGroupCount.Text = "";
                }
            }
            else
            {
                txtMatchTime.Text = "";
                txtMatchDuplicates.Text = "";
                txtMatchGroupCount.Text = "";
                txtMatchDuplicatePercent.Text = "";
            }
        }
        catch (Exception ex)
        {
            _logger.Error("UPDATE REPORT ERROR", ex);
        }

        Report = new MatchReport();
        var reportPath = _configurationService.Configuration.MatchReportPath;
        if (!File.Exists(reportPath))
        {
            OnException?.Invoke(string.Format(Resources.EXCEPTION_REPORT_FILE_NOT_EXISTS, reportPath), "", MessagesType.Error, null);
            return;
        }
        Report.LoadLayout(reportPath);
        Report.DataSource = _service.ReportData ?? new ReportData();
        Report.DisplayName = Resources.SYSTEM_REPORT_DISPLAYNAME;
        Report.CreateDocument();
        //report.ShowDesigner();

        matchReportViewer.DocumentSource = Report;
        matchReportViewer.SetThumbnailsVisibility(true);
        matchReportViewer.Refresh();
        matchReportViewer.Show();
    }

    private void CorrectDateTime()
    {
        foreach (GridColumn column in gvMatchResult.Columns)
        {
            if (column.ColumnType == typeof(DateTime))
            {
                column.ColumnEdit = repositoryItemDateEdit1;
            }
        }
    }

    private MatchResultViewType GetMatchResultViewType()
        => Enum.TryParse(tcMatchResultViewMode.SelectedTabPage.Tag.ToString(), true, out MatchResultViewType viewType)
            ? viewType
            : MatchResultViewType.All;

    private void UpdateMatchResultTable()
    {
        splashScreenManager1.ShowWaitForm();
        string filter = gvMatchResult.ActiveFilterString;

        gridMatchResult.BeginUpdate();
        gridMatchResult.DataSource = null;
        gridMatchResult.Refresh();
        gvMatchResult.Columns.Clear();
        DataTable matchResult = null;
        var wrk = new BackgroundWorker();
        wrk.DoWork += (sender, e) =>
        {
            var currentView = GetMatchResultViewType();
            matchResult = _service.GetMatchResult(currentView);
        };

        wrk.RunWorkerCompleted += (sender, e) =>
        {
            gridMatchResult.DataSource = matchResult;

            gridMatchResult.Refresh();

            CorrectDateTime();

            if (gridMatchResult.DataSource != null)
            {
                UpdateSystemColumnVisibility();

                if (gvMatchResult.RowCount > 0)
                {
                    btnExportMatchResult.Enabled = btnSetMasterRecord.Enabled = btnMergeOverwrite.Enabled = btnDeleteMatchedRecord.Enabled = btnDuplicate.Enabled = btnNotDuplicate.Enabled = btnMergeDuplicates.Enabled = true;

                    gvMatchResult.FormatRules.Clear();

                    gvMatchResult.Columns.ForEach(x =>
                    {
                        if (_service.MatchSettings.MatchParameters.SelectMany(s => s.FieldMap.FieldMap.Select(f => f.ColumnName)).Contains(x.FieldName))
                        {
                            x.AppearanceHeader.ForeColor = Color.Red;

                            x.AppearanceCell.BackColor = Color.FromArgb(140, 198, 154);
                            x.AppearanceCell.BackColor2 = Color.FromArgb(232, 245, 236);
                            x.AppearanceCell.ForeColor = Color.Black;
                            x.AppearanceCell.Options.UseForeColor = true;
                            x.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;

                        }
                        else if (x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_GROUPID)
                        {
                            if (tcMatchResultViewMode.SelectedTabPage == tpOnlyGroup ||
                                tcMatchResultViewMode.SelectedTabPage == tpAcross ||
                                tcMatchResultViewMode.SelectedTabPage == tpUniqueInTable)
                            {
                                x.GroupIndex = 0;
                            }
                            x.OptionsColumn.AllowEdit = false;
                            x.ToolTip = Resources.SYSTEM_GROUPID_TOOLTIP;
                        }
                        else if (x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME)
                        {
                            x.OptionsColumn.AllowEdit = false;
                            x.Width = 200;
                            x.ToolTip = Resources.SYSTEM_SOURCENAME_TOOLTIP;
                        }
                        else if (x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_ISMASTER)

                        {
                            x.ColumnEdit = repoWpCheckEdit;
                            x.MinWidth = 80;
                            x.Width = 90;
                            x.MaxWidth = 120;
                            x.ToolTip = Resources.SYSTEM_ISMASTER_TOOLTIP;

                        }
                        else if (x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED)

                        {
                            x.ColumnEdit = repoWpCheckEdit;
                            x.MinWidth = 80;
                            x.Width = 90;
                            x.MaxWidth = 120;
                            x.ToolTip = Resources.SYSTEM_ISSELLECTED_TOOLTIP;
                        }
                        else if (x.FieldName.EndsWith("Score"))
                        {
                            x.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                            x.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                            x.DisplayFormat.FormatType = FormatType.Numeric;
                            x.DisplayFormat.FormatString = "{0:F2}%";
                            x.OptionsColumn.AllowEdit = false;
                            var rl = gvMatchResult.FormatRules.AddDataBar(x);
                            var barRule = new FormatConditionRuleDataBar
                            {
                                Minimum = 1,
                                MinimumType = FormatConditionValueType.Number,
                                Maximum = 100,
                                MaximumType = FormatConditionValueType.Number
                            };

                            rl.Rule = barRule;

                            if (x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_TOTALSCORE)
                            {
                                x.ToolTip = Resources.SYSTEM_TOTALSCORE_TOOLTIP;
                                //barRule.PredefinedName = "Coral Gradient";

                                barRule.Appearance.BackColor = Color.FromArgb(227, 154, 150);
                                barRule.Appearance.BackColor2 = Color.FromArgb(246, 226, 225);

                                x.MaxWidth = 150;
                                x.Width = 100;
                                x.OptionsColumn.FixedWidth = true;

                            }
                            else if (x.FieldName.StartsWith("Rule"))
                            {
                                barRule.PredefinedName = GetColorThemeByColumnName(x.FieldName);
                                x.MaxWidth = 200;
                                x.Width = 100;
                                x.OptionsColumn.FixedWidth = true;
                            }
                        }
                    });

                    gvMatchResult.ExpandAllGroups();
                }
            }

            gvMatchResult.ActiveFilterCriteria = _activeFilter;
            _activeFilter = null;

            
            gvMatchResult.BestFitColumns();
            if (!string.IsNullOrEmpty(filter))
            {
                gvMatchResult.ActiveFilterString = filter;
            }
            gridMatchResult.EndUpdate();
            splashScreenManager1.CloseWaitForm();
            UpdateMatchReport();
        };

        wrk.RunWorkerAsync();
    }

    private string GetColorThemeByColumnName(string columnName)
    {
        var numbers = Regex.Split(columnName, @"\D+").Where(x => !string.IsNullOrEmpty(x)).ToArray();
        int groupId;
        if (!numbers.Any() || !int.TryParse(numbers[0], out groupId))
        {
            return "Yellow Gradient";
        }

        if (groupId > 8)
        {
            groupId = groupId % 8;
        }

        switch (groupId)
        {
            case 1: return "Light Blue Gradient";
            case 2: return "Yellow Gradient";
            case 3: return "Blue Gradient";
            case 4: return "Orange Gradient";
            case 5: return "Mint Gradient";
            case 6: return "Raspberry Gradient";
            case 7: return "Violet Gradient";
            case 8: case 0: return "Green Gradient";
            default: return "Yellow Gradient";
        }
    }
    #endregion

    #region Controls events
    private void WpSelection_OnSelectionChanged(string columnName, SelectionType selectionType)
    {
        if (gvMatchColumns.Columns.Any())
        {
            var col = gvMatchColumns.Columns.FirstOrDefault(x => x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_INCLUDE_IN_RESULT);
            if (col != null)
            {
                for (int i = 0; i < gvMatchColumns.RowCount - 1; i++)
                {
                    switch (selectionType)
                    {
                        case SelectionType.SelectAll:
                            gvMatchColumns.SetRowCellValue(i, col, 1);
                            break;
                        case SelectionType.UnselectAll:
                            gvMatchColumns.SetRowCellValue(i, col, 0);
                            break;
                        case SelectionType.InvertSelection:
                            gvMatchColumns.SetRowCellValue(i, col, !(bool)gvMatchColumns.GetRowCellValue(i, col));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(selectionType), selectionType, null);
                    }
                }
            }
        }
    }

    private void gvMatchTables_CellValueChanging(object sender, CellValueChangedEventArgs e)
    {
        var row = gvMatchTables.GetRow(e.RowHandle) as ImportedDataInfo;
        if (row != null)
        {
            row.IsSelected = (bool)e.Value;
            _service.UpdateSelectedForMatchingTables(row.TableName);
        }
        UpdateMatchSelectedFields();
    }

    private void gvMatchColumns_CellValueChanged(object sender, CellValueChangedEventArgs e)
    {
        if (e.Column.ColumnType == typeof(string))
        {
            if (e.Value.ToString() == Resources.SYSTEM_NOT_MAPPED_FIELD)
            {
                CheckColumnMatching(e.Column.FieldName);
            }
            else
            {
                CheckColumnMatching(e.Column.VisibleIndex, e.RowHandle, e.Value.ToString());
            }
        }
    }

    private void gvMatchColumns_DoubleClick(object sender, EventArgs e)
    {
        if (gvMatchColumns.GetSelectedRows().Length > 0 &&
            gvMatchColumns.GetRow(gvMatchColumns.GetSelectedRows()[0]) is DataRowView)
        {
            btnMatchSettingAddParameter_Click(null, null);
        }
    }

    private void btnMatchSettingAddParameter_Click(object sender, EventArgs e)
    {
        if (gvMatchColumns.GetSelectedRows().Length > 0)
        {
            var focusedRow = gvMatchParameters.FocusedRowHandle;
            var rw = gvMatchColumns.GetRow(gvMatchColumns.GetSelectedRows()[0]) as DataRowView;
            if (rw == null) return;

            var groupId = _service.MatchSettings.MatchParameters.Any() && gvMatchParameters.GetSelectedRows().Any()
                ? (gvMatchParameters.GetRow(gvMatchParameters.GetSelectedRows()[0]) as MatchParametersViewModel).GroupId
                : 1;

            AddMatchConditionToGroup(groupId, rw);
            gvMatchParameters.FocusedRowHandle = focusedRow;
        }
        else
        {
            OnException?.Invoke(Resources.EXCEPTION_SELECT_FIELD_FOR_MATCHING, "", MessagesType.Warning, null);
        }
    }

    private void AddMatchConditionToGroup(int groupId, DataRowView matchColumnRow)
    {
        var fm = new FieldMapping();
        foreach (GridColumn col in gvMatchColumns.Columns.Where(x => x.FieldName != WinPureColumnNamesHelper.WPCOLUMN_INCLUDE_IN_RESULT))
        {
            if (!string.IsNullOrEmpty(matchColumnRow[col.FieldName].ToString()))
            {
                var tbl = _service.GetTableInfoByDisplayName(col.FieldName);
                fm.FieldMap.Add(new MatchField
                {
                    TableName = tbl.DisplayName,
                    ColumnName = matchColumnRow[col.FieldName].ToString(),
                    ColumnDataType = col.ColumnType.ToString()
                });
            }
        }

        _service.MatchSettings.MatchParameters.Add(new MatchParametersViewModel(fm)
        {
            GroupId = groupId,
            Level = (double)_configurationService.Configuration.DefaultFuzzyLevel,
            IncludeEmpty = _configurationService.Configuration.IncludeEmptyValues,
            IncludeNullValue = _configurationService.Configuration.IncludeNullValues,
        });

        var defaultRow = _service.MatchSettings.MatchParameters.FirstOrDefault(x => x.GroupId == groupId && (x.FieldName == SupportFunctions.MatchGroupDummyCondition || x.Level == 0));

        if (defaultRow != null)
        {
            _service.MatchSettings.MatchParameters.Remove(defaultRow);
        }

        UpdateMatchParameters();
    }

    private void btnMatchSettingAddGroup_Click(object sender, EventArgs e)
    {
        var fm = new FieldMapping
        {
            FieldName = SupportFunctions.MatchGroupDummyCondition
        };
        var groupId = _service.MatchSettings.MatchParameters.Any()
            ? _service.MatchSettings.MatchParameters.Max(x => x.GroupId) + 1
            : 1;
        _service.MatchSettings.MatchParameters.Add(new MatchParametersViewModel(fm)
        {
            IsFuzzy = false,
            Level = 0,
            GroupLevel = 0,
            Weight = 0,
            GroupId = groupId
        });
        UpdateMatchParameters();
        gvMatchParameters.MoveLast();
    }

    private void btnMatchSettingRemoveParameter_Click(object sender, EventArgs e)
    {
        if (!gvMatchParameters.GetSelectedRows().Any())
        {
            return;
        }
        int focusedRow = gvMatchParameters.FocusedRowHandle;
        var selectedRowIndex = gvMatchParameters.GetSelectedRows()[0];
        var dat = gvMatchParameters.GetRow(selectedRowIndex) as MatchParametersViewModel;

        if (dat == null)
        {
            return;
        }

        gvMatchParameters.BeginUpdate();
        if (_service.MatchSettings.MatchParameters.Count(x => x.GroupId == dat.GroupId) == 1 && dat.FieldName != SupportFunctions.MatchGroupDummyCondition)
        {
            var fm = new FieldMapping
            {
                FieldName = SupportFunctions.MatchGroupDummyCondition
            };

            _service.MatchSettings.MatchParameters.Add(new MatchParametersViewModel(fm)
            {
                IsFuzzy = false,
                Level = 0,
                GroupLevel = 0,
                Weight = 0,
                GroupId = dat.GroupId
            });
        }

        _service.MatchSettings.MatchParameters.Remove(dat);
        _service.ReindexGroups(dat.GroupId);
        UpdateMatchParameters();
        if (focusedRow >= _service.MatchSettings.MatchParameters.Count)
        {
            gvMatchParameters.FocusedRowHandle = _service.MatchSettings.MatchParameters.Count - 1;
        }
        else
        {
            var currentFocusedRow = gvMatchParameters.GetRow(focusedRow) as MatchParametersViewModel;
            gvMatchParameters.FocusedRowHandle = currentFocusedRow == null || currentFocusedRow.GroupId > dat.GroupId
                ? focusedRow - 1
                : focusedRow;
        }

        gvMatchParameters.EndUpdate();
    }

    private void btnMatchSettingRemoveGroup_Click(object sender, EventArgs e)
    {
        if (gvMatchParameters.GetSelectedRows().Any())
        {
            var dat = gvMatchParameters.GetRow(gvMatchParameters.GetSelectedRows()[0]) as MatchParametersViewModel;
            if (MessageBox.Show(string.Format(Resources.MESSAGE_REMOVE_CONDITIONGROUP, dat.GroupId), Resources.MESSAGE_MESSAGEDIALOG_WARNING_CAPTION, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _service.MatchSettings.MatchParameters.RemoveAll(x => x.GroupId == dat.GroupId);
                _service.ReindexGroups(dat.GroupId);
                UpdateMatchParameters();
            }
        }
    }

    private void tbMatchSettingSearchDeep_EditValueChanged(object sender, EventArgs e)
    {
        lbSearchDeep.Text = string.Format(Resources.SYSTEM_SEARCH_DEEP, tbMatchSettingSearchDeep.EditValue);
    }

    private void btnMatchSettingSetColumnAuto_Click(object sender, EventArgs e)
    {
        var processedColumns = new List<string>();
        int i = 0;
        var jw = FuzzyFactory.GetFuzzyAlgorithm(MatchAlgorithm.WinPureFuzzy);
        while (i < gvMatchColumns.RowCount)
        {
            string colName = "";
            var rw = gvMatchColumns.GetRow(i) as DataRowView;
            if (rw == null) break;
            if (rw[0].ToString() == "")
            {
                gvMatchColumns.DeleteRow(i);
            }
            else
            {
                int j = 0;
                while (j < gvMatchColumns.Columns.Count - 1)
                {
                    var col = gvMatchColumns.Columns[j];
                    if (j == 0)
                    {
                        colName = rw[j].ToString();
                        if (string.IsNullOrEmpty(colName)) break;
                    }
                    else
                    {
                        rw[j] = "";
                        var colEdt = col.ColumnEdit as RepositoryItemComboBox;
                        var directNameId = colEdt.Items.IndexOf(colName);
                        if (directNameId >= 0)
                        {
                            rw[j] = colEdt.Items[directNameId].ToString();
                        }
                        else
                        {
                            foreach (var itm in colEdt.Items)
                            {
                                if (!processedColumns.Contains(itm.ToString()) &&
                                    (((itm.ToString().Length < 4 || colName.Length < 4) && itm.ToString() == colName) ||
                                     (itm.ToString().Length > 3 && colName.Length > 3 &&
                                      jw.CompareString(itm.ToString(), colName) > 0.9)))
                                {
                                    rw[j] = itm.ToString();
                                    processedColumns.Add(itm.ToString());
                                    break;
                                }
                            }
                        }
                    }
                    j++;
                }
                i++;
            }
        }

        i = 1;
        while (i < gvMatchColumns.Columns.Count - 1)
        {
            var col = gvMatchColumns.Columns[i];
            var columnEdit = col.ColumnEdit as RepositoryItemComboBox;
            foreach (var itm in columnEdit.Items)
            {
                if (itm.ToString() == Resources.SYSTEM_NOT_MAPPED_FIELD) continue;
                var j = 0;
                bool isAdded = false;
                while (!isAdded && j < gvMatchColumns.RowCount)
                {
                    var rw = gvMatchColumns.GetRow(j) as DataRowView;
                    if (rw == null) break;
                    if (rw[i].ToString() == itm.ToString()) isAdded = true;
                    j++;
                }

                if (isAdded)
                {
                    continue;
                }

                var dt = gridMatchColumns.DataSource as DataTable;
                var r = dt.NewRow();
                r[i] = itm.ToString();

                j = i + 1;
                while (j < gvMatchColumns.Columns.Count - 1)
                {
                    var itemComboBox = gvMatchColumns.Columns[j].ColumnEdit as RepositoryItemComboBox;
                    foreach (var nItem in itemComboBox.Items)
                    {
                        if (((nItem.ToString().Length < 4 || itm.ToString().Length < 4) &&
                             nItem.ToString() == itm.ToString()) ||
                            (nItem.ToString().Length > 3 && itm.ToString().Length > 3 &&
                             jw.CompareString(nItem.ToString(), itm.ToString()) > 0.9))
                        {
                            r[j] = nItem.ToString();
                            break;
                        }
                    }
                    j++;
                }

                dt.Rows.Add(r);
                gridMatchColumns.RefreshDataSource();
            }
            i++;
        }
    }

    private void btnMatchSettingStartMatching_Click(object sender, EventArgs e)
    {
        ClearReport();
        Enum.TryParse(cbAlgorithm.Text, out MatchAlgorithm algorithm);
        _service.MatchDataAsync(tbMatchSettingSearchDeep.Value, algorithm);
        UpdateMatchParameters();
    }

    private void gvMatchColumns_ValidateRow(object sender, ValidateRowEventArgs e)
    {
        CheckRowData(e.RowHandle, e.Row as DataRowView);
        e.Valid = true;
    }

    private void gvMatchParameters_CellValueChanged(object sender, CellValueChangedEventArgs e)
    {
        if (e.Column.Name != "colLevel") return;
        var rw = gvMatchParameters.GetRow(e.RowHandle) as MatchParametersViewModel;
        if (rw != null)
        {
            rw.GroupLevel = Convert.ToInt32(e.Value);
        }
    }

    private void gvMatchParameters_CellValueChanging(object sender, CellValueChangedEventArgs e)
    {
        if (e.Column.FieldName != "" && _matchTypeFields.Contains(e.Column.FieldName))
        {
            gridMatchParameters.BeginUpdate();
            var ind = gvMatchParameters.FocusedRowHandle;
            var rw = gvMatchParameters.GetRow(e.RowHandle) as MatchParametersViewModel;

            ProcessLinkedSettings(e.Column.FieldName, rw);
            rw.Level = e.Column.FieldName == "IsFuzzy" ? (double)_configurationService.Configuration.DefaultFuzzyLevel : 100.0;
            rw.GroupLevel = e.Column.FieldName == "IsFuzzy" ? 90 : 100;
            UpdateMatchParameters();
            gvMatchParameters.FocusedRowHandle = ind;
            gridMatchParameters.EndUpdate();
        }

        if (e.Column.FieldName == "IncludeNullValue")
        {
            if (Boolean.TryParse(e.Value.ToString(), out bool newValue))
            {
                if (newValue)
                {
                    gridMatchParameters.BeginUpdate();
                    var ind = gvMatchParameters.FocusedRowHandle;
                    var rw = gvMatchParameters.GetRow(e.RowHandle) as MatchParametersViewModel;

                    rw.IncludeEmpty = true;
                    rw.IncludeNullValue = newValue;
                    UpdateMatchParameters();
                    gvMatchParameters.FocusedRowHandle = ind;
                    gridMatchParameters.EndUpdate();
                }
            }
        }

        if (e.Column.FieldName == "IncludeEmpty")
        {
            if (Boolean.TryParse(e.Value.ToString(), out bool newValue))
            {
                if (!newValue)
                {
                    gridMatchParameters.BeginUpdate();
                    var ind = gvMatchParameters.FocusedRowHandle;
                    var rw = gvMatchParameters.GetRow(e.RowHandle) as MatchParametersViewModel;

                    rw.IncludeEmpty = newValue;
                    rw.IncludeNullValue = false;
                    UpdateMatchParameters();
                    gvMatchParameters.FocusedRowHandle = ind;
                    gridMatchParameters.EndUpdate();
                }
            }
        }

    }

    private void btnSetMasterRecord_Click(object sender, EventArgs e)
    {
        var frmSetMasterRecords = WinPureUiDependencyResolver.Resolve<frmSetMasterRecords>();
        var tblList =
            _service.MatchSettings.MatchParameters.SelectMany(x => x.FieldMap.FieldMap)
                .Select(x => x.TableName)
                .Distinct()
                .ToList();
        var fieldList = (from DataColumn col in _service.GetMatchResult(MatchResultViewType.All).Columns
            where !ColumnHelper.WinPureSystemFields.Contains(col.ColumnName)
            select new MatchField { ColumnName = col.ColumnName, ColumnDataType = col.DataType.ToString() }).ToList();

        if (frmSetMasterRecords.Show(tblList, fieldList, _service.LastMasterRecordSettings, true) == DialogResult.OK)
        {
            _service.DefineMasterRecord(frmSetMasterRecords.MasterRecordSetting);
        }
    }

    private void barButtonExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
        try
        {
            ExternalSourceTypes sourceType;
            if (!Enum.TryParse(e.Item.Tag.ToString(), out sourceType))
            {
                return;
            }

            DataView dw;
            if (gvMatchResult.ActiveFilter == null || !gvMatchResult.ActiveFilterEnabled
                                                   || gvMatchResult.ActiveFilter.Expression == "")
            {
                dw = gvMatchResult.DataSource as DataView;
            }
            else
            {
                var table = ((DataView)gvMatchResult.DataSource).Table;
                dw = new DataView(table)
                {
                    RowFilter = CriteriaToWhereClauseHelper.GetDataSetWhere(gvMatchResult.ActiveFilterCriteria)
                };
            }
            var srv = WinPureUiDependencyResolver.Resolve<IImportExportService>();
            var tbl = dw?.ToTable();
            if (tbl != null)
            {
                tbl.TableName = "MatchResult";
            }
            srv.Export(sourceType, tbl, "", !_configurationService.Configuration.ShowSystemFields);
        }
        catch (WinPureBaseException ex)
        {
            _logger.Debug("IMPORT/EXPORT ERROR", ex);
            OnException?.Invoke(ex.Message, "", MessagesType.Error, null);
        }
        catch (Exception ex)
        {
            _logger.Debug("IMPORT/EXPORT ERROR", ex);
            OnException?.Invoke(Resources.EXCEPTION_DATA_CANNOT_BE_EXPORTED, "", MessagesType.Error, ex);
        }
    }

    private void gvMatchResult_CellValueChanging(object sender, CellValueChangedEventArgs e)
    {
        var rw = gvMatchResult.GetRow(e.RowHandle) as DataRowView;
        if (rw == null) return;

        if (e.Column.FieldName == WinPureColumnNamesHelper.WPCOLUMN_ISMASTER)
        {
            rw[WinPureColumnNamesHelper.WPCOLUMN_ISMASTER] = e.Value;
            var rowNumber = _service.SetMasterRecord(Convert.ToInt64(rw[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY]), Convert.ToBoolean(e.Value));
            if (rowNumber < 0) return;

            var gridRow = gvMatchResult.LocateByValue(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY, rowNumber);
            if (gridRow == GridControl.InvalidRowHandle) return;

            var oldRow = gvMatchResult.GetRow(gridRow) as DataRowView;
            if (oldRow != null) oldRow[WinPureColumnNamesHelper.WPCOLUMN_ISMASTER] = false;
        }
        else if (!ColumnHelper.WinPureSystemFields.Contains(e.Column.FieldName) && !e.Column.FieldName.EndsWith(" Score")
                 || e.Column.FieldName == WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED)
        {
            _service.SetMatchResultCellValue(Convert.ToInt64(rw[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY]), e.Column.FieldName, e.Value);
        }
    }

    private void btnMergeOverwrite_Click(object sender, EventArgs e)
    {
        var frm = WinPureUiDependencyResolver.Resolve<frmUpdateMatchResult>();
        if (frm.ShowDialog(_service.MatchResult, _service.GetMasterRecordsCount()) == DialogResult.OK)
        {
            _service.UpdateMatchResult(frm.UpdateSettings);
        }
    }

    private void btnDeleteMatchedRecord_Click(object sender, EventArgs e)
    {
        var frm = WinPureUiDependencyResolver.Resolve<frmMatchResultDelete>();
        if (frm.ShowDialog() == DialogResult.OK)
        {
            _service.DeleteMergeMatchResult(frm.ResultOption);
        }
    }

    private void cbMatchSettingAcrossTables_CheckedChanged(object sender, EventArgs e)
    {
        lbMainTable.Visible = cbMatchAcrossMainTable.Visible = cbMatchSettingAcrossTables.Checked;
        _service.MatchSettings.MatchAcrossTables = cbMatchSettingAcrossTables.Checked;
    }

    private void btnNotDuplicate_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(Resources.MESSAGE_REMOVE_NOT_DUPLICATE_FROM_MATCH_RESULT, Resources.MESSAGE_QUESTION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
            DialogResult.Yes)
        {
            _service.RemoveNotDuplicateRecords();
        }
    }

    private void cbMatchAcrossMainTable_SelectedIndexChanged(object sender, EventArgs e)
    {
        _service.MatchSettings.AcrossTableMainTable = cbMatchAcrossMainTable.Text;
    }

    private void linkLabel_Report_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        OnNavigateToFullReport?.Invoke();
    }

    private void tcMatchResultViewMode_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
    {
        _activeFilter = gvMatchResult.ActiveFilterCriteria;
        UpdateMatchResultTable();
    }

    private void btnMergeDuplicates_Click(object sender, EventArgs e)
    {
        var frm = WinPureUiDependencyResolver.Resolve<frmMatchResultMerge>();
        if (frm.ShowDialog(_service.MatchResult, _service.GetMasterRecordsCount()) == DialogResult.OK)
        {
            _service.MergeMatchResult(frm.MergeSettings);
        }
    }
    #endregion

    private void OpenHelp_Click(object sender, EventArgs e)
    {
        var control = sender as Control;
        if (control != null)
        {
            var chapter = (HelpPageChapter)Enum.Parse(typeof(HelpPageChapter), control.Tag.ToString());
            UserManualHelper.OpenHelpPage(chapter);
        }
    }

    private void btnExportDV_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e?.Item?.Tag == null) return;

        if (WinPureUiDependencyResolver.Resolve<ILicenseService>().IsDemo)
        {
            OnException(Resources.EXCEPTION_EXPORT_DISABLED_ON_DEMO, "", MessagesType.Warning, null);
            return;
        }

        var dlgSaveCsvFile = new SaveFileDialog
        {
            FileName = "",
            AddExtension = true,
        };

        switch (e.Item.Tag.ToString())
        {
            case "CSV":
                dlgSaveCsvFile.Filter = "Text Files (Delimited) (*.csv)|*.csv";
                dlgSaveCsvFile.Title = "Export data to CSV file";
                break;
            case "XLSX":
                dlgSaveCsvFile.Filter = "Microsoft Excel files (*.xlsx, *.xls)|*.xlsx;*.xls";
                dlgSaveCsvFile.Title = "Export data to Microsoft Excel file";
                break;

            case "RTF":
                dlgSaveCsvFile.Filter = "Microsoft Word files (*.rtf)|*.rtf";
                dlgSaveCsvFile.Title = "Export data to Microsoft Word file";
                break;

            case "PDF":
                dlgSaveCsvFile.Filter = "PDF files (*.pdf)|*.pdf";
                dlgSaveCsvFile.Title = "Export data to PDF file";
                break;
            case "HTML":
                dlgSaveCsvFile.Filter = "Web-page (*.html|*.html";
                dlgSaveCsvFile.Title = "Export data to Web page file";
                break;
        }

        if (dlgSaveCsvFile.ShowDialog() == DialogResult.OK)
        {
            switch (e.Item.Tag.ToString())
            {
                case "CSV":
                    gvMatchResult.ExportToCsv(dlgSaveCsvFile.FileName);
                    break;
                case "XLSX":
                    gvMatchResult.ExportToXlsx(dlgSaveCsvFile.FileName);
                    break;

                case "RTF":
                    gvMatchResult.ExportToRtf(dlgSaveCsvFile.FileName);
                    break;

                case "PDF":
                    gvMatchResult.ExportToPdf(dlgSaveCsvFile.FileName);
                    break;
                case "HTML":
                    gvMatchResult.ExportToHtml(dlgSaveCsvFile.FileName);
                    break;
            }
            gridMatchResult.Refresh();
        }

    }

    private void gvMatchResult_PrintInitialize(object sender, PrintInitializeEventArgs e)
    {
        var pb = e.PrintingSystem as PrintingSystemBase;
        pb.PageSettings.Landscape = true;
    }

    private void btnMoveToData_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
        var dw = (gvMatchResult.ActiveFilter == null
                  || !gvMatchResult.ActiveFilterEnabled
                  || gvMatchResult.ActiveFilter.Expression == "")
            ? gvMatchResult.DataSource as DataView
            : new DataView(((DataView)gvMatchResult.DataSource).Table)
            {
                RowFilter = CriteriaToWhereClauseHelper.GetDataSetWhere(gvMatchResult.ActiveFilterCriteria)
            };

        _service.SaveResultToData(!_configurationService.Configuration.ShowSystemFields, GetMatchResultViewType(), NameHelper.MatchResultTable, dw?.ToTable());
    }

    private void gvMatchResult_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
    {
        if (e.HitInfo?.Column != null && e.HitInfo.Column.FieldName == WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED)
        {
            var view = sender as GridView;
            contextMenuStrip1.Show(view.GridControl, e.Point);
        }
    }

    private void mnuSelectAll_Click(object sender, EventArgs e)
    {
        var obj = ((ToolStripMenuItem)sender);
        var val = Convert.ToInt64(obj.Tag);
        var selectedRows = gvMatchResult.GetSelectedWpKeysForVisibleRows();
        _service.SetMatchResultIsSelected(val == 1, selectedRows);
        UpdateMatchResultTable();
    }

    private void mnuSelectAllGroup_Click(object sender, EventArgs e)
    {
        var obj = ((ToolStripMenuItem)sender);
        var val = Convert.ToInt64(obj.Tag);
        var selectedRows = gvMatchResult.GetSelectedWpKeysForCurrentGroup();
        _service.SetMatchResultIsSelected(val == 1, selectedRows);
        UpdateMatchResultTable();
    }

    private GridHitInfo _downHitInfo;
    private bool _moveGroup;

    public void SetUpGrid(GridControl grid)
    {
        grid.AllowDrop = true;
        grid.DragOver += grid_DragOver;
        grid.DragDrop += grid_DragDrop;
        var view = grid.MainView as GridView;
        if (view == null)
        {
            return;
        }

        view.MouseMove += view_MouseMove;
        view.MouseDown += view_MouseDown;
    }

    private void view_MouseDown(object sender, MouseEventArgs e)
    {
        var view = sender as GridView;
        _downHitInfo = null;
        GridHitInfo hitInfo = view.CalcHitInfo(new Point(e.X, e.Y));
        if (ModifierKeys != Keys.None)
        {
            return;
        }

        if (e.Button == MouseButtons.Left && hitInfo.RowHandle >= -1000)
        {
            _moveGroup = hitInfo.RowHandle < 0;
            _downHitInfo = hitInfo;
        }
    }

    private void view_MouseMove(object sender, MouseEventArgs e)
    {
        var view = sender as GridView;
        if (view == null)
        {
            return;
        }

        if (e.Button == MouseButtons.Left && _downHitInfo != null)
        {
            var dragSize = SystemInformation.DragSize;
            var dragRect = new Rectangle(new Point(_downHitInfo.HitPoint.X - dragSize.Width / 2,
                _downHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);

            if (!dragRect.Contains(new Point(e.X, e.Y)))
            {
                view.GridControl.DoDragDrop(view, DragDropEffects.Move);
                _downHitInfo = null;
                _moveGroup = false;
                DXMouseEventArgs.GetMouseArgs(e).Handled = true;
            }
        }

    }

    private void grid_DragOver(object sender, DragEventArgs e)
    {
        if (sender is GridControl)
        {
            var currentView = (sender as GridControl).FocusedView as GridView;

            if (e.Data.GetDataPresent(typeof(GridView)) && currentView.Name != "gvMatchColumns")
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
    }

    private void grid_DragDrop(object sender, DragEventArgs e)
    {
        var currentView = (sender as GridControl).FocusedView as GridView;

        var sourceView = e.Data.GetData(typeof(GridView)) as GridView;
        if (currentView != null && sourceView != null && currentView.Name == "gvMatchParameters")
        {
            int groupId = 1;

            int focusedRowIndex = 0;
            if (currentView.RowCount != 0)
            {
                var hitPoint = currentView.GridControl.PointToClient(new Point(e.X, e.Y));
                var hitInfo = currentView.CalcHitInfo(hitPoint);

                if (hitInfo.RowHandle < -1000)
                {
                    groupId = _service.MatchSettings.MatchParameters.Max(x => x.GroupId);
                    focusedRowIndex = -1;
                }
                else
                {
                    if (hitInfo.RowHandle < 0)
                    {
                        groupId = -hitInfo.RowHandle;
                        focusedRowIndex = currentView.GetVisibleRowHandle(hitInfo.RowHandle);
                    }
                    else
                    {
                        int targetRowIndex = hitInfo.RowHandle; // currentView.GetDataSourceRowIndex(targetRowHandle);
                        var srcRow = currentView.GetRow(targetRowIndex) as MatchParametersViewModel;
                        groupId = srcRow.GroupId;
                        focusedRowIndex = targetRowIndex;
                    }
                }
            }

            if (currentView == sourceView && _service.MatchSettings.MatchParameters.Any())//move conditions between groups or groups
            {
                var rw = sourceView.GetRow(sourceView.GetSelectedRows()[0]) as MatchParametersViewModel;
                if (rw == null || rw.GroupId == groupId || rw.FieldName == SupportFunctions.MatchGroupDummyCondition)
                {
                    return;
                }

                if (_moveGroup)
                {
                    var groupFrom = rw.GroupId;
                    var rowsGroupFrom = _service.MatchSettings.MatchParameters.Where(x => x.GroupId == groupFrom).ToList();

                    if (groupFrom < groupId)
                    {
                        _service.MatchSettings.MatchParameters.Where(x => x.GroupId > groupFrom && x.GroupId <= groupId).ForEach(g => g.GroupId--);
                    }
                    else if (groupFrom > groupId)
                    {
                        _service.MatchSettings.MatchParameters.Where(x => x.GroupId >= groupId && x.GroupId < groupFrom).ForEach(g => g.GroupId++);
                    }
                    rowsGroupFrom.ForEach(x => x.GroupId = groupId);
                }
                else
                {
                    if (_service.MatchSettings.MatchParameters.Count(x => x.GroupId == rw.GroupId) == 1)
                    {
                        _service.MatchSettings.MatchParameters.Add(new MatchParametersViewModel(new FieldMapping
                        {
                            FieldName = SupportFunctions.MatchGroupDummyCondition
                        })
                        {
                            IsFuzzy = false,
                            Level = 0,
                            GroupLevel = 0,
                            Weight = 0,
                            GroupId = rw.GroupId
                        });
                    }

                    rw.GroupId = groupId;

                    _service.MatchSettings.MatchParameters
                        .RemoveAll(x =>
                            x.GroupId == groupId && x.FieldName == SupportFunctions.MatchGroupDummyCondition);
                }

                UpdateMatchParameters();
            }
            else
            {
                var rw = sourceView.GetRow(sourceView.GetSelectedRows()[0]) as DataRowView;
                if (rw == null) return;

                AddMatchConditionToGroup(groupId, rw);
                if (focusedRowIndex >= 0)
                {
                    currentView.FocusedRowHandle = focusedRowIndex;
                }
                else
                {
                    currentView.MoveLastVisible();
                }
            }
        }
    }

    private void cbAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
    {
        _configurationService.Configuration.FuzzyAlgorithm = cbAlgorithm.Text;
        _configurationService.SaveConfiguration();
    }

    private void pictureBox8_Click(object sender, EventArgs e)
    {
        var control = sender as Control;
        if (control != null)
        {
            var chapter = (HelpPageChapter)Enum.Parse(typeof(HelpPageChapter), control.Tag.ToString());
            UserManualHelper.OpenHelpPage(chapter);
        }
    }

    private void gvMatchResult_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
    {
        if (tcMatchResultViewMode.SelectedTabPage == tpAllData && e.RowHandle < gvMatchResult.RowCount)
        {
            var currentData = gvMatchResult.GetRow(e.RowHandle) as DataRowView;
            var nextData = gvMatchResult.GetRow(e.RowHandle + 1) as DataRowView;
            if (currentData != null && nextData != null &&
                (int)currentData.Row[WinPureColumnNamesHelper.WPCOLUMN_GROUPID] != (int)nextData.Row[WinPureColumnNamesHelper.WPCOLUMN_GROUPID])
            {
                var r = e.Bounds;
                e.Cache.Paint.DrawLine(e.Graphics, Pens.Black, new Point(r.Left - 1, r.Bottom), new Point(r.Right, r.Bottom));

                //e.Graphics.DrawLine(Pens.Black, r.Left - 1, r.Bottom, r.Right, r.Bottom); //Bottom
                //e.Appearance.DrawString(e.Cache, e.DisplayText, r);
                //e.Handled = true;
            }
        }
    }

    private void knowledgebase_Click(object sender, EventArgs e)
    {
        var frmDct = WinPureUiDependencyResolver.Resolve<frmEditDictionary>();
        frmDct.ShowDialog();
    }

    private void btnRuleProcessing_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e?.Item?.Tag == null) return;

        _configurationService.Configuration.UseMixedRules = e.Item.Tag.ToString() == "mixed";
        _configurationService.SaveConfiguration();

        SetBtnRuleFlowCaption();
    }

    private void SetBtnRuleFlowCaption()
    {
        btnMatchFlow.Text = "Rule Flow\r\n" + (_configurationService.Configuration.UseMixedRules ? "Mixed" : "Direct");
    }

    private void btnDuplicate_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(Resources.MESSAGE_CRETATE_NEW_DUPLICATE_GROUP, Resources.MESSAGE_QUESTION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
            DialogResult.Yes)
        {
            _service.CreateNewDuplicateGroup();
        }
    }

    private void groupControl1_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.MatchingRecordsOptions);
    }

    private void panelControl1_Paint(object sender, PaintEventArgs e)
    {

    }
}