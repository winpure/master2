using System.ComponentModel;
using DevExpress.Data;
using WinPure.CleanAndMatch.Reports;
using WinPure.Configuration.Helper;
using WinPure.DataService.Senzing.Models;

// ReSharper disable LocalizableElement

namespace WinPure.CleanAndMatch.Controls;

internal partial class UCEntityResolution : XtraUserControl
{
    private IDataManagerService _dataService;
    private IProjectService _projectService;
    private readonly IWpLogger _logger;
    private readonly IConfigurationService _configurationService;
    private string _viewFilter = String.Empty;

    public event Action<string, string, MessagesType, Exception> OnException;

    public VerificationReport Report { get; private set; }

    public UCEntityResolution()
    {
        InitializeComponent();
        Localization();
        if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
        {
            _configurationService = WinPureUiDependencyResolver.Resolve<IConfigurationService>();
            _logger = WinPureUiDependencyResolver.Resolve<IWpLogger>();
            var licenseService = WinPureUiDependencyResolver.Resolve<ILicenseService>();
            barMenuExportWithStyle.Enabled = licenseService.ProgramType != ProgramType.CamFree && !licenseService.IsDemo;
        }
    }

    public void Initialize()
    {
        ucEntityResolutionConfiguration.Initialize(true);
        _dataService = WinPureUiDependencyResolver.Resolve<IDataManagerService>();
        _dataService.OnEntityResolutionStart += _dataService_OnEntityResolutionStart;
        _dataService.OnEntityResolutionReady += _dataService_OnEntityResolutionReady;
        _projectService = WinPureUiDependencyResolver.Resolve<IProjectService>();
        _projectService.OnBeforeProjectLoad += _projectService_OnBeforeProjectLoad;
        _projectService.OnAfterProjectLoad += _projectService_OnAfterProjectLoad;

        var viewType = GetErViewType();

        SetActionButtonVisibility(viewType);

        btnProcessPossibilities.Enabled = viewType == MatchResultViewType.PossibleRelated || viewType == MatchResultViewType.PossibleDuplicates;
    }

    #region localization
    private void Localization()
    {
        labelControl3.Text = Resources.UI_MAINMATCHFORM_MATCHING_TIME;
        lbGroupCount.Text = Resources.UI_MAINMATCHFORM_GROUP_COUNT;
        lbDuplicates.Text = Resources.UI_MAINMATCHFORM_DUPLICATES;
        lbDuplicatesPercent.Text = Resources.UI_MAINMATCHFORM_DUPLICATES_PERCENT;

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
        btnNotDuplicate.Text = Resources.UI_UCMAINMATCHFORM_NOT_DUPLICATE;
        btnMergeDuplicates.Text = Resources.UI_MERGE;
        btnExportMatchResult.Text = Resources.UI_EXPORT;
        tpAllData.Text = Resources.UI_MAINMATCHFORM_ALLDATA;
        tpAcross.Text = Resources.UI_MAINMATCHFORM_ACROSSTABLES;
        tpUniqueInTable.Text = Resources.UI_MAINMATCHFORM_ACROSSTABLESUNIQUE;
        tpOnlyGroup.Text = Resources.UI_MAINMATCHFORM_ONLYGROUPS;
        tpNotMatched.Text = Resources.UI_MAINMATCHFORM_NONMATCHES;
        barBtnCreateNewGroup.Caption = Resources.UI_CAPTION_CREATENEWGROUP;
        barBtnAddToBiggest.Caption = Resources.UI_CAPTION_ADDTOBIGGEST;
        barBtnNotRelated.Caption = Resources.UI_CAPTION_NOTRELATED;
        btnProcessPossibilities.Text = Resources.UI_CAPTION_PROCESSPOSSIBILITIES;

        mnuClearAll.Text = Resources.UI_CAPTION_CLEARALL;
        mnuSelectAll.Text = Resources.UI_CAPTION_SELECTALL;
        mnuSelectAllGroup.Text = Resources.UI_CAPTION_SELECTALLGROUP;
        mnuClearAllGroup.Text = Resources.UI_CAPTION_CLEARALLGROUP;
    }
    #endregion

    public void CloseCurrentData(bool removeControl = true)
    {
        ucEntityResolutionConfiguration.CloseCurrentData(removeControl);
        DisposeConnection();
    }

    public void ShowSubPanel(EntityResolutionViewType dType)
    {
        switch (dType)
        {
            case EntityResolutionViewType.Settings: navFrameEntityResolution.SelectedPage = navPageConfigurtion; break;
            case EntityResolutionViewType.Result: navFrameEntityResolution.SelectedPage = navPageResult; break;
        }
    }

    public void RefreshMapping(string tableName)
    {
        ucEntityResolutionConfiguration.RefreshMapping(tableName);
    }

    public void SetFilterRowVisibility(bool isEnabled)
    {
        gvEntityResolutionResult.OptionsView.ShowAutoFilterRow = isEnabled;
        gvEntityResolutionResult.OptionsCustomization.AllowFilter = isEnabled;
    }

    public void UpdateSystemColumnVisibility()
    {
        if (_dataService == null)
        {
            return;
        }

        var sourceData = _dataService.GetErTableSchema();
        var viewType = GetErViewType();
        if (sourceData != null)
        {
            foreach (GridColumn column in gvEntityResolutionResult.Columns)
            {
                if (column.FieldName == WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID ||
                    column.FieldName == WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID ||
                    column.FieldName == WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID_KEY ||
                    column.FieldName == WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID_KEY)
                {
                    continue;
                }

                if (ColumnHelper.IsSystemField(column.FieldName))
                {
                    if (_configurationService.Configuration.ShowSystemFields)
                    {
                        if ((viewType == MatchResultViewType.PossibleDuplicates || viewType == MatchResultViewType.PossibleRelated) &&
                            (column.FieldName == WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED || column.FieldName == WinPureColumnNamesHelper.WPCOLUMN_ISMASTER))
                        { continue; }

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

            //crazy fix. check later may be.
            if (gvEntityResolutionResult.Columns.Any(x => x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED) && 
                _configurationService.Configuration.ShowSystemFields && 
                (viewType == MatchResultViewType.PossibleDuplicates || viewType == MatchResultViewType.PossibleRelated))
            {
                gvEntityResolutionResult.Columns[WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED].VisibleIndex = 3;
            }

            var oldColumnWithSummary = gvEntityResolutionResult.Columns
                .FirstOrDefault(x => x.FieldName != WinPureColumnNamesHelper.WPCOLUMN_GROUPID
                                     && x.FieldName != WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID
                                     && x.FieldName != WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID
                                     && x.FieldName != WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY
                                     && x.Summary.Any());

            oldColumnWithSummary?.Summary.Clear();

            var newColumnWithSummary = gvEntityResolutionResult.Columns
                .Where(x => x.FieldName != WinPureColumnNamesHelper.WPCOLUMN_GROUPID
                            && x.FieldName != WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID
                            && x.FieldName != WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID
                            && x.FieldName != WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY && x.Visible)
                .OrderBy(x => x.VisibleIndex).FirstOrDefault();

            if (newColumnWithSummary != null && !newColumnWithSummary.Summary.Any())
            {
                var item = new GridColumnSummaryItem(SummaryItemType.Count, newColumnWithSummary.FieldName, "Count={0}");
                newColumnWithSummary.Summary.Add(item);
            }

            if (gvEntityResolutionResult.Columns.Any())
            {
                if (gvEntityResolutionResult.Columns.ColumnByFieldName(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY) != null)
                {
                    gvEntityResolutionResult.Columns[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY].Visible = false;
                }

                if (gvEntityResolutionResult.Columns.ColumnByFieldName(WinPureColumnNamesHelper.WPCOLUMN_ORIGINAL_KEY) != null)
                {
                    gvEntityResolutionResult.Columns[WinPureColumnNamesHelper.WPCOLUMN_ORIGINAL_KEY].Visible = false;
                }
            }
        }
    }

    private void _projectService_OnAfterProjectLoad()
    {
        _dataService_OnEntityResolutionReady();
    }

    private void _projectService_OnBeforeProjectLoad()
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(_projectService_OnBeforeProjectLoad));
            return;
        }
        ClearResultTable();
    }

    private void _dataService_OnEntityResolutionStart()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new MethodInvoker(delegate { _dataService_OnEntityResolutionStart(); }));
            return;
        }

        ClearResultTable();
    }

    private void _dataService_OnEntityResolutionReady()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new MethodInvoker(() => _dataService_OnEntityResolutionReady()));
            return;
        }

        UpdateStatistic();
        UpdateResultTable();
        UpdateSystemColumnVisibility();
    }

    private void ClearResultTable()
    {
        DisposeConnection();
        gridEntityResolutionResult.DataSource = null;
        gvEntityResolutionResult.Columns.Clear();
    }

    private void UpdateStatistic()
    {
        //groupReportData.Text = tcMatchResultViewMode.SelectedTabPage.Text;
        groupReportData.Text = Resources.UI_UCMAINMATCHFORM_MATCHINGRECORDSOPTIONS_2 + " - " + tcMatchResultViewMode.SelectedTabPage.Text;
        txtMatchTime.Text = "";
        if (_dataService.EntityResolutionReportData != null)
        {
            var viewType = GetErViewType();
            txtMatchTime.Text = _dataService.EntityResolutionReportData.ExecutionTime.ToString(@"dd\:hh\:mm\:ss");

            switch (viewType)
            {
                case MatchResultViewType.PossibleDuplicates:
                    txtMatchGroupCount.Text = _dataService.EntityResolutionReportData.PossibleDuplicateGroupCount.ToString();
                    txtMatchDuplicates.Text = _dataService.EntityResolutionReportData.PossibleDuplicateRecordsCount.ToString();
                    txtMatchDuplicatePercent.Text = string.Format("{0:P2}",
                        Math.Round(
                            (Convert.ToDouble(_dataService.EntityResolutionReportData.PossibleDuplicateRecordsCount) /
                             Convert.ToDouble(_dataService.EntityResolutionReportData.TotalRecords)), 4)).Replace(",", ".");
                    lbDuplicates.Text = Resources.UI_MAINMATCHFORM_DUPLICATES;
                    lbDuplicatesPercent.Text = Resources.UI_MAINMATCHFORM_DUPLICATES_PERCENT;
                    break;
                case MatchResultViewType.PossibleRelated:
                    txtMatchGroupCount.Text = _dataService.EntityResolutionReportData.RelatedGroupCount.ToString();
                    txtMatchDuplicates.Text = _dataService.EntityResolutionReportData.RelatedRecordsCount.ToString();
                    txtMatchDuplicatePercent.Text = string.Format("{0:P2}",
                        Math.Round(
                            (Convert.ToDouble(_dataService.EntityResolutionReportData.RelatedRecordsCount) /
                             Convert.ToDouble(_dataService.EntityResolutionReportData.TotalRecords)), 4)).Replace(",", ".");
                    lbDuplicates.Text = Resources.UI_MAINMATCHFORM_RELATED;
                    lbDuplicatesPercent.Text = Resources.UI_MAINMATCHFORM_RELATED_PERCENT;
                    break;
                case MatchResultViewType.NonMatches:
                    txtMatchGroupCount.Text = _dataService.EntityResolutionReportData.GroupUnique.Count.ToString();
                    txtMatchDuplicates.Text = (_dataService.EntityResolutionReportData.TotalRecords - _dataService.EntityResolutionReportData.DuplicateRecordsCount).ToString();
                    txtMatchDuplicatePercent.Text = string.Format("{0:P2}",
                        Math.Round(
                            ((Convert.ToDouble(_dataService.EntityResolutionReportData.TotalRecords) - Convert.ToDouble(_dataService.EntityResolutionReportData.DuplicateRecordsCount)) /
                             Convert.ToDouble(_dataService.EntityResolutionReportData.TotalRecords)), 4)).Replace(",", ".");
                    lbDuplicates.Text = Resources.UI_MAINMATCHFORM_NONMATCHES1;
                    lbDuplicatesPercent.Text = Resources.UI_MAINMATCHFORM_NONMATCHES_PERCENT;
                    break;
                case MatchResultViewType.All:
                case MatchResultViewType.OnlyGroup:
                default:
                    txtMatchGroupCount.Text = _dataService.EntityResolutionReportData.GroupWithDuplicates.Count.ToString();
                    txtMatchDuplicates.Text = _dataService.EntityResolutionReportData.DuplicateRecordsCount.ToString();
                    txtMatchDuplicatePercent.Text = string.Format("{0:P2}",
                        Math.Round(
                            (Convert.ToDouble(_dataService.EntityResolutionReportData.DuplicateRecordsCount) /
                             Convert.ToDouble(_dataService.EntityResolutionReportData.TotalRecords)), 4)).Replace(",", ".");
                    lbDuplicates.Text = Resources.UI_MAINMATCHFORM_MATCHES;
                    lbDuplicatesPercent.Text = Resources.UI_MAINMATCHFORM_MATCHES_PERCENT;
                    break;

            }
        }
        else
        {
            txtMatchTime.Text = "";
            txtMatchGroupCount.Text = "";
            txtMatchDuplicates.Text = "";
            txtMatchDuplicatePercent.Text = "";
        }
    }

    private void UpdateResultTable()
    {
        
        var connectionManager = WinPureUiDependencyResolver.Resolve<IConnectionManager>();
        connectionManager.CheckConnectionState();

        if (SqLiteHelper.CheckTableExists(NameHelper.EntityResolutionTable, connectionManager.Connection))
        {
            tpAcross.PageVisible = tpUniqueInTable.PageVisible = _dataService.TableList.Count(x => x.IsResolutionSelected) > 1;

            gridEntityResolutionResult.BeginUpdate();
            try
            {
                bool shouldReTry = true;
                int attempts = 0;

                var viewType = GetErViewType();
                var filer = GetFilterCondition(viewType);

                while (shouldReTry && attempts < 3)
                {
                    try
                    {
                        var ds = _dataService.EntityResolutionReportData != null
                            ? viewType switch
                            {
                                MatchResultViewType.OnlyGroup => NameHelper.EntityResolutionTable.GetData(filer),
                                MatchResultViewType.AcrossTable => NameHelper.EntityResolutionTable.GetData(filer),
                                MatchResultViewType.TableUnique => NameHelper.EntityResolutionTable.GetData(filer),
                                MatchResultViewType.NonMatches => NameHelper.EntityResolutionTable.GetData(filer),
                                MatchResultViewType.PossibleDuplicates => _dataService.GetPossibilities(PossibilityType.Duplicated),
                                MatchResultViewType.PossibleRelated => _dataService.GetPossibilities(PossibilityType.Related),
                                _ => NameHelper.EntityResolutionTable.GetData(String.Empty)
                            }
                            : null;

                        gridEntityResolutionResult.DataSource = ds;

                        shouldReTry = false;
                    }
                    catch
                    {
                        //log
                        attempts++;
                    }
                }

                gridEntityResolutionResult.Refresh();
                gvEntityResolutionResult.PopulateColumns();

                gvEntityResolutionResult.Columns.ForEach(x =>
                {
                    if (x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_GROUPID)
                    {
                        if (viewType == MatchResultViewType.OnlyGroup ||
                            viewType == MatchResultViewType.NonMatches ||
                            viewType == MatchResultViewType.AcrossTable ||
                            viewType == MatchResultViewType.TableUnique ||
                            viewType == MatchResultViewType.All)
                        {
                            x.SortOrder = ColumnSortOrder.Ascending;
                            x.GroupIndex = 0;
                            x.OptionsColumn.AllowEdit = false;
                            x.ToolTip = Resources.SYSTEM_GROUPID_TOOLTIP;
                        }
                    }
                    else if (x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID)
                    {
                        if (viewType == MatchResultViewType.PossibleDuplicates)
                        {
                            x.SortOrder = ColumnSortOrder.Ascending;
                            x.GroupIndex = 0;
                            x.OptionsColumn.AllowEdit = false;
                        }
                        else
                        {
                            x.Visible = false;
                        }
                    }
                    else if (x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID_KEY)
                    {
                        if (viewType == MatchResultViewType.PossibleDuplicates)
                        {
                            x.OptionsColumn.AllowEdit = false;
                        }
                        else
                        {
                            x.Visible = false;
                        }
                    }
                    else if (x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID)
                    {
                        if (viewType == MatchResultViewType.PossibleRelated)
                        {
                            x.SortOrder = ColumnSortOrder.Ascending;
                            x.GroupIndex = 0;
                            x.OptionsColumn.AllowEdit = false;
                        }
                        else
                        {
                            x.Visible = false;
                        }
                    }
                    else if (x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID_KEY)
                    {
                        if (viewType == MatchResultViewType.PossibleRelated)
                        {
                            x.OptionsColumn.AllowEdit = false;
                        }
                        else
                        {
                            x.Visible = false;
                        }
                    }
                    else if (x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_SOURCENAME)
                    {
                        x.OptionsColumn.AllowEdit = false;
                        x.Width = 200;
                        x.ToolTip = Resources.SYSTEM_SOURCENAME_TOOLTIP;
                    }
                    else if (x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_ISMASTER)
                    {
                        if (viewType == MatchResultViewType.PossibleDuplicates ||
                            viewType == MatchResultViewType.PossibleRelated)
                        {
                            x.Visible = false;
                        }
                        else
                        {
                            x.ColumnEdit = repoWpCheckEdit;
                            x.MaxWidth = 70;
                            x.ToolTip = Resources.SYSTEM_ISMASTER_TOOLTIP;
                        }

                    }
                    else if (x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED)
                    {
                        x.ColumnEdit = repoWpCheckEdit;
                        x.MaxWidth = 70;
                        x.ToolTip = Resources.SYSTEM_ISSELLECTED_TOOLTIP;
                        x.VisibleIndex = 3;
                    }
                    else if (x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_MATCH_KEY)
                    {
                        if (viewType == MatchResultViewType.PossibleDuplicates ||
                            viewType == MatchResultViewType.PossibleRelated)
                        {
                            x.Visible = false;
                        }
                    }
                    else if (x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_TOTALSCORE)
                    {
                        x.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                        x.DisplayFormat.FormatType = FormatType.Numeric;
                        x.DisplayFormat.FormatString = "{0:F2}%";
                        x.OptionsColumn.AllowEdit = false;
                        var rl = gvEntityResolutionResult.FormatRules.AddDataBar(x);
                        var barRule = new FormatConditionRuleDataBar
                        {
                            Minimum = 0,
                            MinimumType = FormatConditionValueType.Number,
                            Maximum = 70,
                            MaximumType = FormatConditionValueType.Number
                        };

                        rl.Rule = barRule;

                        x.ToolTip = Resources.SYSTEM_TOTALSCORE_TOOLTIP;
                        x.AppearanceHeader.ForeColor = ForeColor = Color.Blue;
                        barRule.PredefinedName = "Coral Gradient";
                        x.Width = 80;
                        x.OptionsColumn.FixedWidth = true;
                    }
                });

                if (gvEntityResolutionResult.RowCount > 0)
                {
                    var licenseService = WinPureUiDependencyResolver.Resolve<ILicenseService>();
                    btnExportMatchResult.Enabled = true;
                    btnSetMasterRecord.Enabled = btnMergeOverwrite.Enabled = btnDeleteMatchedRecord.Enabled = btnNotDuplicate.Enabled = btnMergeDuplicates.Enabled = true;

                    SetActionButtonVisibility(viewType);
                }
                else
                {
                    btnExportMatchResult.Enabled = btnSetMasterRecord.Enabled = btnMergeOverwrite.Enabled = btnDeleteMatchedRecord.Enabled = btnNotDuplicate.Enabled = btnMergeDuplicates.Enabled = false;
                }

                UpdateSystemColumnVisibility();
                if (!string.IsNullOrEmpty(_viewFilter))
                {
                    try
                    {
                        gvEntityResolutionResult.ActiveFilterString = _viewFilter;
                    }
                    catch
                    {
                        _viewFilter = String.Empty;
                    }
                }

                gvEntityResolutionResult.ExpandAllGroups();

                gvEntityResolutionResult.BestFitColumns();
            }
            finally
            {
                gridEntityResolutionResult.EndUpdate();
                gridEntityResolutionResult.Visible = true;
            }
        }
        else
        {
            tpAcross.PageVisible = tpUniqueInTable.PageVisible = false;
        }
    }

    private void DisposeConnection()
    {
        XpoCollectionHelper.DisposeWithOwner(gridEntityResolutionResult.DataSource);
    }

    private void btnMoveToData_ItemClick(object sender, ItemClickEventArgs e)
    {
        var data = GetErCurrentViewData();
        if (data != null)
        {
            _dataService.SaveResultToData(!_configurationService.Configuration.ShowSystemFields, GetErViewType(), NameHelper.EntityResolutionTable, data);
        }
        else
        {
            OnException?.Invoke("Data not found.", "", MessagesType.Error, null);
        }
    }

    private DataTable GetErCurrentViewData()
    {
        var (viewType, filter) = GetViewTypeAndFilterCondition();

        var data = _dataService.GetErDataTable(viewType, filter);
        return data;
    }

    private void barButtonExport_ItemClick(object sender, ItemClickEventArgs e)
    {
        try
        {
            ExternalSourceTypes sourceType;
            if (!Enum.TryParse(e.Item.Tag.ToString(), out sourceType))
            {
                return;
            }

            var exportService = WinPureUiDependencyResolver.Resolve<IImportExportService>();
            var data = GetErCurrentViewData();
            if (data != null)
            {
                exportService.Export(sourceType, data, "", !_configurationService.Configuration.ShowSystemFields);
            }
            else
            {
                OnException?.Invoke("Data not found.", "", MessagesType.Error, null);
            }
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

    private void btnExportDE_ItemClick(object sender, ItemClickEventArgs e)
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
                    gvEntityResolutionResult.ExportToCsv(dlgSaveCsvFile.FileName);
                    break;
                case "XLSX":
                    gvEntityResolutionResult.ExportToXlsx(dlgSaveCsvFile.FileName);
                    break;

                case "RTF":
                    gvEntityResolutionResult.ExportToRtf(dlgSaveCsvFile.FileName);
                    break;

                case "PDF":
                    gvEntityResolutionResult.ExportToPdf(dlgSaveCsvFile.FileName);
                    break;
                case "HTML":
                    gvEntityResolutionResult.ExportToHtml(dlgSaveCsvFile.FileName);
                    break;
            }
            gridEntityResolutionResult.Refresh();
        }
    }

    private MatchResultViewType GetErViewType()
        => Enum.TryParse(tcMatchResultViewMode.SelectedTabPage.Tag.ToString(), true, out MatchResultViewType viewType)
            ? viewType
            : MatchResultViewType.All;

    private string GetFilterCondition(MatchResultViewType viewType)
    {
        switch (viewType)
        {
            case MatchResultViewType.OnlyGroup:
                return $"[{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] in ({string.Join(",", _dataService.EntityResolutionReportData.GroupWithDuplicates)}) ";
            case MatchResultViewType.AcrossTable:
                var groupsAcross = _dataService.GetErAcrossTableGroups();
                return $"[{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] in ({string.Join(",", groupsAcross)}) ";
            case MatchResultViewType.TableUnique:
                var groupsUnique = _dataService.GetErUniqueTableGroups();
                return $"[{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] in ({string.Join(",", groupsUnique)}) ";
            case MatchResultViewType.NonMatches:
                return $"[{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] in ({string.Join(",", _dataService.EntityResolutionReportData.GroupUnique)}) ";
            case MatchResultViewType.PossibleDuplicates:
                return $"[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID}] > 0";
            case MatchResultViewType.PossibleRelated:
                return $"[{WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID}] > 0";
            default:
                return string.Empty;
        }
    }

    private (MatchResultViewType, string) GetViewTypeAndFilterCondition()
    {
        var viewType = GetErViewType();
        var filter = GetFilterCondition(viewType);

        return (viewType, filter);
    }

    private void tcMatchResultViewMode_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
    {
        splashScreenManager1.ShowWaitForm();
        DisposeConnection();
        UpdateResultTable();
        UpdateStatistic();

        var viewType = GetErViewType();

        SetActionButtonVisibility(viewType);

        btnProcessPossibilities.Enabled = viewType == MatchResultViewType.PossibleRelated || viewType == MatchResultViewType.PossibleDuplicates;
        splashScreenManager1.CloseWaitForm();
    }

    private void btnSetMasterRecord_Click(object sender, EventArgs e)
    {
        var frmSetMasterRecords = WinPureUiDependencyResolver.Resolve<frmSetMasterRecords>();

        //TODO get table list from data or define last settings in case user can change the selection before use master settings
        var tblList =
            _dataService.TableList.Where(x => x.IsResolutionSelected)
                .Select(x => x.DisplayName)
                .ToList();

        var fieldList = (from GridColumn col in gvEntityResolutionResult.Columns
                         where !ColumnHelper.WinPureSystemFields.Contains(col.FieldName)
                         select new MatchField { ColumnName = col.FieldName, ColumnDataType = col.ColumnType.ToString() }).ToList();

        if (frmSetMasterRecords.Show(tblList, fieldList, _dataService.LastErMasterRecordSettings, false) == DialogResult.OK)
        {
            _dataService.DefineErMasterRecord(frmSetMasterRecords.MasterRecordSetting);
        }
    }

    private void btnMergeOverwrite_Click(object sender, EventArgs e)
    {
        var frm = WinPureUiDependencyResolver.Resolve<frmUpdateMatchResult>();
        if (frm.ShowDialog(_dataService.GetErTableSchema(), _dataService.GetErMasterRecordsCount()) == DialogResult.OK)
        {
            _dataService.UpdateErMatchResult(frm.UpdateSettings);
        }
    }

    private void btnNotDuplicate_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(Resources.MESSAGE_REMOVE_NOT_DUPLICATE_FROM_MATCH_RESULT, Resources.MESSAGE_QUESTION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
            DialogResult.Yes)
        {
            _dataService.RemoveErNotDuplicateRecords();
        }
    }

    private void btnDeleteMatchedRecord_Click(object sender, EventArgs e)
    {
        var frm = WinPureUiDependencyResolver.Resolve<frmMatchResultDelete>();
        if (frm.ShowDialog() == DialogResult.OK)
        {
            _dataService.DeleteErMergeMatchResult(frm.ResultOption);
        }
    }

    private void btnMergeDuplicates_Click(object sender, EventArgs e)
    {
        var frm = WinPureUiDependencyResolver.Resolve<frmMatchResultMerge>();
        if (frm.ShowDialog(_dataService.GetErTableSchema(), _dataService.GetErMasterRecordsCount()) == DialogResult.OK)
        {
            _dataService.MergeErMatchResult(frm.MergeSettings);
        }
    }

    private void barBtnCreateNewGroup_ItemClick(object sender, ItemClickEventArgs e)
    {
        var viewType = GetErViewType();

        _dataService.MergePossibilitiesToNewGroup(viewType == MatchResultViewType.PossibleRelated ? PossibilityType.Related : PossibilityType.Duplicated);
    }

    private void barBtnAddToBiggest_ItemClick(object sender, ItemClickEventArgs e)
    {
        var viewType = GetErViewType();
        _dataService.MergePossibilitiesToBiggestGroup(viewType == MatchResultViewType.PossibleRelated ? PossibilityType.Related : PossibilityType.Duplicated);
    }

    private void barBtnNotRelated_ItemClick(object sender, ItemClickEventArgs e)
    {
        var viewType = GetErViewType();
        _dataService.MovePossibilitiesToSeparateGroups(viewType == MatchResultViewType.PossibleRelated ? PossibilityType.Related : PossibilityType.Duplicated);
    }

    private void gvEntityResolutionResult_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
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
        var (viewType, whereCondition) = GetViewTypeAndFilterCondition();

        if (viewType == MatchResultViewType.PossibleRelated || viewType == MatchResultViewType.PossibleDuplicates)
        {
            var ids = new List<string>();
            for (var i = 0; i < gvEntityResolutionResult.RowCount; i++)
            {
                if (gvEntityResolutionResult.GetRow(i) is DataRowView dataRow)
                {
                    ids.Add(dataRow[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY].ToString());
                }
            }

            if (ids.Any())
            {
                whereCondition = $"[{WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY}] IN ({string.Join(",", ids)})";
            }
        }
        else
        {
            var filterSting = DataHelper.GetSqliteFilterCriteriaFromGridView(gvEntityResolutionResult);
            if (!string.IsNullOrEmpty(filterSting))
            {
                whereCondition += " AND " + filterSting;
            }
        }
        _viewFilter = gvEntityResolutionResult.ActiveFilterString;
        _dataService.SetErMatchResultIsSelected(val == 1, whereCondition);
    }


    private void mnuSelectAllGroup_Click(object sender, EventArgs e)
    {
        //gvEntityResolutionResult.ShowLoadingPanel();
        var obj = ((ToolStripMenuItem)sender);
        var val = Convert.ToInt64(obj.Tag);
        var whereCondition = String.Empty;
        var viewType = GetErViewType();

        if (viewType == MatchResultViewType.PossibleRelated || viewType == MatchResultViewType.PossibleDuplicates)
        {
            var ids = new List<string>();
            if (viewType == MatchResultViewType.PossibleRelated)
            {
                if (gvEntityResolutionResult.GetFocusedDataRow()[WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID] is long currentGroupId)
                {
                    for (var i = 0; i < gvEntityResolutionResult.RowCount; i++)
                    {
                        if (gvEntityResolutionResult.GetRow(i) is DataRowView dataRow)
                        {
                            if ((long)dataRow[WinPureColumnNamesHelper.WPCOLUMN_POSSIBLERELATEDID] == currentGroupId)
                                ids.Add(dataRow[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY].ToString());
                        }
                    }

                }
            }
            else if (viewType == MatchResultViewType.PossibleDuplicates)
            {
                if (gvEntityResolutionResult.GetFocusedDataRow()[WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID] is long currentGroupId)
                {
                    for (var i = 0; i < gvEntityResolutionResult.RowCount; i++)
                    {
                        if (gvEntityResolutionResult.GetRow(i) is DataRowView dataRow)
                        {
                            if ((long)dataRow[WinPureColumnNamesHelper.WPCOLUMN_POSSIBLEDUPLICATEID] == currentGroupId)
                                ids.Add(dataRow[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY].ToString());
                        }
                    }
                }
            }
            if (ids.Any())
            {
                whereCondition = $"[{WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY}] IN ({string.Join(",", ids)})";
            }
        }
        else
        {
            object cellValue = gvEntityResolutionResult.GetRowCellValue(gvEntityResolutionResult.FocusedRowHandle, WinPureColumnNamesHelper.WPCOLUMN_GROUPID);
            if (int.TryParse(cellValue.ToString(), out var currentGroupId))
            {
                whereCondition = $"[{WinPureColumnNamesHelper.WPCOLUMN_GROUPID}] = {currentGroupId}";
            }
        }

        _viewFilter = gvEntityResolutionResult.ActiveFilterString;
        _dataService.SetErMatchResultIsSelected(val == 1, whereCondition);
        //gvEntityResolutionResult.HideLoadingPanel();
    }

    private void gvEntityResolutionResult_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
    {
        var viewType = GetErViewType();
        if (viewType == MatchResultViewType.PossibleRelated || viewType == MatchResultViewType.PossibleDuplicates)
        {
            if (e.Column.FieldName == WinPureColumnNamesHelper.WPCOLUMN_ISSELECTED)
            {
                if (gvEntityResolutionResult.GetRow(e.RowHandle) is DataRowView rw)
                {
                    _dataService.SetErMatchResultIsSelected(Convert.ToBoolean(e.Value), $"[{WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY}] = {rw[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY]}", false);
                }
            }
        }
    }

    private void SetActionButtonVisibility(MatchResultViewType viewType)
    {
        btnSetMasterRecord.Enabled = btnDeleteMatchedRecord.Enabled = btnMergeDuplicates.Enabled = btnMergeOverwrite.Enabled = btnNotDuplicate.Enabled
            = viewType == MatchResultViewType.All || viewType == MatchResultViewType.OnlyGroup || viewType == MatchResultViewType.AcrossTable || viewType == MatchResultViewType.TableUnique || viewType == MatchResultViewType.NonMatches;

    }

    private void gvEntityResolutionResult_ColumnFilterChanged(object sender, EventArgs e)
    {
        _viewFilter = gvEntityResolutionResult.ActiveFilterString;
    }

    private void pictureBox6_Click(object sender, EventArgs e)
    {
        var control = sender as Control;
        if (control != null)
        {
            var chapter = (HelpPageChapter)Enum.Parse(typeof(HelpPageChapter), control.Tag.ToString());
            UserManualHelper.OpenHelpPage(chapter);
        }
    }

    private void pictureBox1_Click(object sender, EventArgs e)

    {
        //var control = sender as Control;
        //if (control != null)
        //{
         //   var chapter = (HelpPageChapter)Enum.Parse(typeof(HelpPageChapter), control.Tag.ToString());
         //   UserManualHelper.OpenHelpPage(chapter);
       // }
    }

    private void pictureBox4_Click(object sender, EventArgs e)
    {
        //var control = sender as Control;
        //if (control != null)
        //{
        //    var chapter = (HelpPageChapter)Enum.Parse(typeof(HelpPageChapter), control.Tag.ToString());
        //    UserManualHelper.OpenHelpPage(chapter);
       // }
    }
}