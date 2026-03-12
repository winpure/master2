using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTab;
using Newtonsoft.Json;
using System.Diagnostics;
using WinPure.Cleansing.Models;

namespace WinPure.CleanAndMatch.Controls;

public partial class UCMainCleanNew : XtraUserControl
{
    private IWpLogger _logger;
    private ThemeDetectionService _themeDetectionService;
    private readonly List<string> _standartizeFiledList = new List<string> { "ST_UpperCase", "ST_LowerCase", "ST_ProperCase" };
    private readonly List<string> _shiftFiledList = new List<string> { "SH_LEFT", "SH_RIGHT" };
    private readonly List<string> _splitFiledList = new List<string> { "SP_EmailAddress", "SP_Email", "SP_Telephones", "SP_DateTime", "SP_Words", "SP_Address", "SP_Country", "SP_City", "SP_Region", "SP_Postcode", "SP_Gender" };
    private int _rowCount = 1;
    private string lastUniqueColumnName = String.Empty;
    private bool _shouldRaiseAnotherTableEvent = true;
    private IDataManagerService _service;
    private IProjectService _projectService;

    public UCMainCleanNew()
    {
        InitializeComponent();
        Localization();
        columnWordManager.OptionsColumn.FixedWidth = true;
        tcCleanSettings.SelectedTabPage = tpRemove;
        var cleanToolTipController = ToolTipController.DefaultController;
        cleanToolTipController.HyperlinkClick += CleanToolTipController_HyperlinkClick;
    }

    private void Localization()
    {
        bandedGridColumn1.Caption = Resources.UI_UCMAINCLEANNEWFORM_FIELDNAME;
        gridBandConvert.Caption = Resources.UI_CONVERT;
        bandedGridColumn10.Caption = Resources.UI_UCMAINCLEANNEWFORM_OSTONOUGHTS;
        bandedGridColumn11.Caption = Resources.UI_UCMAINCLEANNEWFORM_LSTO1S;
        bandedGridColumn12.Caption = Resources.UI_UCMAINCLEANNEWFORM_NOUGTSTOOS;
        bandedGridColumn13.Caption = Resources.UI_UCMAINCLEANNEWFORM_1STOLS;
        gridBandMerge.Caption = Resources.UI_MERGE;
        bandedGridColumn24.Caption = Resources.UI_COLUMNS;
        gridBandSplit.Caption = Resources.UI_SPLIT;
        bandedGridColumn19.Caption = Resources.UI_UCMAINCLEANNEWFORM_NAMEANDEMAIL;
        bandedGridColumn20.Caption = Resources.UI_EMAIL;
        bandedGridColumn21.Caption = Resources.UI_TELEPHONE;
        bandedGridColumn22.Caption = Resources.UI_UCMAINCLEANNEWFORM_DATETIME;
        bandedGridColumn23.Caption = Resources.UI_WORDMANAGERSETTINGSFORM_WORDS;
        gridBandRemove.Caption = Resources.UI_REMOVE;
        gridBandAddressSplit.Caption = Resources.UI_CAPTION_ADDRESSPARSER;
        bandedGridColumn2.Caption = Resources.UI_UCMAINCLEANNEWFORM_TRAILINGSPACES;
        bandedGridColumn2.ToolTip = Resources.UI_UCMAINCLEANNEWFORM_REMOVETRAILINGSPACES;
        bandedGridColumn3.Caption = Resources.UI_IMPORTTEXTFORM_COMMA;
        bandedGridColumn3.ToolTip = Resources.UI_UCMAINCLEANNEWFORM_REMOVECOMMAS;
        bandedGridColumn4.Caption = Resources.UI_DOTS;
        bandedGridColumn4.ToolTip = Resources.UI_UCMAINCLEANNEWFORM_REMOVEDOTS;
        bandedGridColumn5.Caption = Resources.UI_HYPHENS;
        bandedGridColumn5.ToolTip = Resources.UI_UCMAINCLEANNEWFORM_REMOVEHYPHENS;
        bandedGridColumn6.Caption = Resources.UI_APOSTROPHES;
        bandedGridColumn6.ToolTip = Resources.UI_UCMAINCLEANNEWFORM_REMOVEAPOSTROPHES;
        bandedGridColumn26.Caption = Resources.UI_UCMAINCLEANNEWFORM_LEADINGSPACES;
        bandedGridColumn26.ToolTip = Resources.UI_UCMAINCLEANNEWFORM_REMOVELEADINGSPACES;
        bandedGridColumn7.Caption = Resources.UI_LETTERS;
        bandedGridColumn7.ToolTip = Resources.UI_UCMAINCLEANNEWFORM_REMOVELETTERS;
        bandedGridColumn8.Caption = Resources.UI_NUMBERS;
        bandedGridColumn8.ToolTip = Resources.UI_UCMAINCLEANNEWFORM_REMOVENAMBERS;
        bandedGridColumn27.Caption = Resources.UI_UCMAINCLEANNEWFORM_NONPRINTABLE;
        bandedGridColumn27.ToolTip = Resources.UI_UCMAINCLEANNEWFORM_REMOVENONPRINTABLECHARACTERS;
        bandedGridColumn28.Caption = Resources.UI_SPACES;
        bandedGridColumn28.ToolTip = Resources.UI_UCMAINCLEANNEWFORM_REMOVEALLSPACES;
        bandedGridColumn9.Caption = Resources.UI_UCMAINCLEANNEWFORM_OTHERCHARACTERS;
        bandedGridColumn9.ToolTip = Resources.UI_UCMAINCLEANNEWFORM_REMOVEOTHERCHARACTERS;
        gridBandStandart.Caption = Resources.UI_STANDARDISE;
        bandedGridColumn14.Caption = Resources.UI_UPPERCASE;
        bandedGridColumn15.Caption = Resources.UI_LOWERCASE;
        bandedGridColumn16.Caption = Resources.UI_PROPERCASE;
        bandedGridColumn32.Caption = Resources.UI_EMPTY_WITH_VALUE;

        bandedGridColumn17.Caption = Resources.UI_UCMAINCLEANNEWFORM_SHIFTLEFT;
        bandedGridColumn25.Caption = Resources.UI_UCMAINCLEANNEWFORM_SHIFTRIGHT;
        bandedGridColumn29.Caption = Resources.UI_UCMAINCLEANNEWFORM_MULTIPLESPACES;
        bandedGridColumn30.Caption = Resources.UI_UCMAINCLEANNEWFORM_NEWLINE;
        bandedGridColumn31.Caption = Resources.UI_UCMAINCLEANNEWFORM_TABCHAR;

        gridBandWordManager.Caption = Resources.UI_WORD;
        columnWordManager.Caption = Resources.UI_MANAGER;
        groupControl1.Text = Resources.UI_UCMAINCLEANNEWFORM_DATAPREVIEW;
        gridColumn1.Caption = Resources.UI_UPDATEMATCHRESULTFORM_COLUMNNAME;
        gridColumn2.Caption = Resources.UI_TYPE;
        gridColumn3.Caption = Resources.UI_FILLED;
        gridColumn4.Caption = Resources.UI_EMPTY;
        gridColumn5.Caption = Resources.UI_DISTINCT;
        gridColumn6.Caption = Resources.UI_UCMAINCLEANNEWFORM_LEADINGSPACES;
        gridColumn7.Caption = Resources.UI_UCMAINCLEANNEWFORM_TRAILINGSPACES;
        gridColumn8.Caption = Resources.UI_UCMAINCLEANNEWFORM_MULTIPLESPACES;
        gridColumn9.Caption = Resources.UI_UCMAINCLEANNEWFORM_WITHSPACES;
        gridColumn10.Caption = Resources.UI_UCMAINCLEANNEWFORM_ALPHAONLY;
        gridColumn13.Caption = Resources.UI_NUMBERS;
        gridColumn14.Caption = Resources.UI_PUNCTUATION;
        gridColumn15.Caption = Resources.UI_UCMAINCLEANNEWFORM_UPPERONLY;
        gridColumn16.Caption = Resources.UI_UCMAINCLEANNEWFORM_LOWERONLY;
        gridColumn17.Caption = Resources.UI_UCMAINCLEANNEWFORM_PROPERCASE;
        gridColumn30.Caption = Resources.UI_UCMAINCLEANNEWFORM_MIXEDCASE;
        gridColumn18.Caption = Resources.UI_UCMAINCLEANNEWFORM_TABCHAR;
        gridColumn19.Caption = Resources.UI_UCMAINCLEANNEWFORM_NEWLINECHAR;
        gridColumn20.Caption = Resources.UI_UCMAINCLEANNEWFORM_MOSTCOMMON;
        gridColumn21.Caption = Resources.UI_UCMAINCLEANNEWFORM_MOSTCOMMONCOUNT;
        gridColumn22.Caption = Resources.UI_UCMAINCLEANNEWFORM_MINNUMBER;
        gridColumn23.Caption = Resources.UI_UCMAINCLEANNEWFORM_MAXNUMBER;
        gridColumn24.Caption = Resources.UI_UCMAINCLEANNEWFORM_MAXWORDS;
        gridColumn25.Caption = Resources.UI_UCMAINCLEANNEWFORM_AVERAGEWORDS;
        gridColumn26.Caption = Resources.UI_UCMAINCLEANNEWFORM_MAXLENGTH;
        gridColumn27.Caption = Resources.UI_UCMAINCLEANNEWFORM_AVERAGELENGTH;
        gridColumn32.Caption = Resources.UI_PATTERNNAME;

        gridColumn31.Caption = Resources.UI_UCMAINCLEANNEWFORM_COMMAS;
        gridColumn35.Caption = Resources.UI_DOTS;
        gridColumn34.Caption = Resources.UI_UCMAINCLEANNEWFORM_HYPENS;
        gridColumn33.Caption = Resources.UI_APOSTROPHES;
        gridColumn11.Caption = Resources.CAPTION_MATCH_PATTERN;
        gridColumn12.Caption = Resources.CAPTION_NONMATCH_PATTERN;

        btnRefreshStatistic.Text = Resources.UI_UCMAINCLEANNEWFORM_REFRESHSTATISTICS;

        btnMatchSettingRefreshStats.Text = Resources.UI_UCMAINCLEANNEWFORM_REFRESHSTATISTICS;
        btnMatchSettingRefreshStats.ToolTip = Resources.UI_MAINFORM_THISWILLREFRESHALLSTATISTICS;
        (btnMatchSettingRefreshStats.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_MAINFORM_THISWILLREFRESHALLSTATISTICS;
        colAddress.Caption = Resources.UI_CAPTION_ADDRESS;
        colCountry.Caption = Resources.UI_ADDRVERIFICATION_CAPTION_COUNTRY;
        colCity.Caption = Resources.UI_CAPTION_CITY;
        colPostcode.Caption = Resources.UI_CAPTION_ZIP;
        colRegion.Caption = Resources.UI_CAPTION_STATE;
        tpConvert.Text = Resources.UI_CONVERT;
        tpMerge.Text = Resources.UI_MERGE;
        tpRemove.Text = Resources.UI_REMOVE;
        tpSplit.Text = Resources.UI_SPLIT;
        tpStandart.Text = Resources.UI_STANDARDISE;
        tpWordManager.Text = Resources.UI_WORDMANAGERSETTINGSFORM_WORDMANAGER;
        tpAddressParser.Text = Resources.UI_CAPTION_ADDRESSPARSER;
        tpAll.Text = Resources.UI_ALL;
        lbl_double_click.Text = Resources.UI_DOUBLECLICK_CLEAN;
        btnMatchSettingStartCleaning.ToolTip = Resources.UI_MAINFORM_PERFORM_DATA_CLEAN;
        btnMatchSettingStartCleaning.Text = Resources.UI_MAINFORM_RUN_CLEAN;
        (btnMatchSettingStartCleaning.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_MAINFORM_PERFORM_DATA_CLEAN;
        btnUndoCleansing.Text = Resources.UI_UNDO;
        colPattern.Caption = Resources.UI_PATTERNNAME;
        gridBandPattern.Caption = Resources.CAPTION_PATTERNMANAGER;



    }

    public void Initialize()
    {
        _logger = WinPureUiDependencyResolver.Resolve<IWpLogger>();
        _service = WinPureUiDependencyResolver.Resolve<IDataManagerService>();
        _projectService = WinPureUiDependencyResolver.Resolve<IProjectService>();


        _themeDetectionService = WinPureUiDependencyResolver.Resolve<ThemeDetectionService>();
        _themeDetectionService.SetReferenceControl(gridCleanSettings);

        _service.OnAddNewData += _service_OnAddNewData;
        _service.OnTableDataUpdateComplete += _service_OnTableDataUpdateComplete;
        _service.OnTableDelete += _service_OnTableDelete;
        _service.OnStatisticUpdated += UpdateStatistic;
        _service.OnCurrentTableChanged += _service_OnCurrentTableChanged;
        _service.OnRefreshData += _service_OnRefreshData;
        _service.OnChangeTableDisplayName += _service_OnChangeTableDisplayName;

        _projectService.OnBeforeProjectLoad += ClearTableData;

        var columnList = gvCleanSettings.Columns
            .Where(x => x.ColumnEdit == repoCheckEdit)
            .OrderBy(x => x.VisibleIndex)
            .Select(x => x.Caption)
            .ToList();
        wpSelection.Initiate(true, columnList);
        wpSelection.OnSelectionChanged += WpSelection_OnSelectionChanged;
        if (Program.CurrentProgramVersion == ProgramType.CamLte || Program.CurrentProgramVersion == ProgramType.CamFree)
        {
            tcCleanSettings.TabPages.Remove(tpAddressParser);
            gridBandAddressSplit.Visible = false;
        }

        // Subscribe to theme change event
        UserLookAndFeel.Default.StyleChanged += OnThemeChanged;
        RefreshSupportEditor();
    }

    public void RefreshSupportEditor()
    {
        var patternService = WinPureUiDependencyResolver.Resolve<IStatisticPatternService>();
        edtPatterns.DataSource = patternService.GetAllPatterns().Result;

        var aiConfigurationService = WinPureUiDependencyResolver.Resolve<ICleansingAiConfigurationService>();
        edtAiType.DataSource = aiConfigurationService.GetAllConfigurations().Result;
    }

    private void WpSelection_OnSelectionChanged(string columnName, SelectionType action)
    {
        var col = gvCleanSettings.Columns.FirstOrDefault(x => x.Caption == columnName);
        if (col != null)
        {
            for (int i = 0; i < gvCleanSettings.RowCount; i++)
            {
                long newVal;
                switch (action)
                {
                    case SelectionType.SelectAll:
                        newVal = 1;
                        break;
                    case SelectionType.UnselectAll:
                        newVal = 0;
                        break;
                    case SelectionType.InvertSelection:
                        newVal = (long)gvCleanSettings.GetRowCellValue(i, col) == 0 ? 1 : 0;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(action), action, null);
                }
                gvCleanSettings.SetRowCellValue(i, col, newVal);
                var ea = new DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs(i, col, newVal);
                gvCleanSettings_CellValueChanging(gvCleanSettings, ea);

            }
        }
    }

    public void SetDisplaySettings(DisplaySettings settings)
    {
        if (!settings.ShowCleanPreview && !settings.ShowCleanStatistic)
        {
            splitCleanMain.PanelVisibility = SplitPanelVisibility.Panel1;
        }
        else
        {
            splitCleanMain.PanelVisibility = SplitPanelVisibility.Both;
            if (settings.ShowCleanPreview)
            {
                splitCleanDeteil.PanelVisibility = settings.ShowCleanStatistic
                    ? SplitPanelVisibility.Both
                    : SplitPanelVisibility.Panel1;
            }
            else
            {
                splitCleanDeteil.PanelVisibility = SplitPanelVisibility.Panel2;
            }
        }
    }

    public void ExportStatistic()
    {
      //  if (WinPureUiDependencyResolver.Resolve<ILicenseService>().IsDemo)
       // {
       //     MessageBox.Show(Resources.EXCEPTION_EXPORT_DISABLED_ON_DEMO);
       //     return;
       // }

        if (navFrameCleanStat.SelectedPage == npCleanStatData)
        {
            var sv = new SaveFileDialog
            {
                Filter = Resources.DIALOG_EXCELNEW_FORMAT,
                Title = Resources.DIALOG_EXPORT_EXCEL_CAPTION,
                AddExtension = true
            };
            var importedDataInfo = _service.GetTableInfo(_service.CurrentTable);
            if (importedDataInfo == null)
            {
                MessageBox.Show(Resources.EXCEPTION_NO_STATISTIC_FOR_EXPORT, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            sv.FileName = Path.GetFileNameWithoutExtension(importedDataInfo.FileName) + "_Statistic.xlsx";
            sv.ValidateNames = true;
            sv.OverwritePrompt = true;
            if (sv.ShowDialog() == DialogResult.OK)
            {
                gridCleanStatistic.ExportToXlsx(sv.FileName);
            }

        }
    }

    public void ExportUniqueValues(bool fullRecords = false)
    {
        if (WinPureUiDependencyResolver.Resolve<ILicenseService>().IsDemo)
        {
            MessageBox.Show(Resources.EXCEPTION_EXPORT_DISABLED_ON_DEMO);
            return;
        }

        if (gridUniqueValues.DataSource == null || gvUniqueValues.RowCount == 0)
        {
            MessageBox.Show("No data for export.", Resources.MESSAGE_MESSAGEDIALOG_WARNING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (navFrameCleanStat.SelectedPage == npCleanStatData)
        {
            var sv = new SaveFileDialog
            {
                Filter = Resources.DIALOG_EXCELNEW_FORMAT,
                Title = Resources.DIALOG_EXPORT_EXCEL_CAPTION,
                AddExtension = true
            };
            var importedDataInfo = _service.GetTableInfo(_service.CurrentTable);
            if (importedDataInfo == null)
            {
                MessageBox.Show(Resources.EXCEPTION_NO_STATISTIC_FOR_EXPORT, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            sv.FileName = Path.GetFileNameWithoutExtension(importedDataInfo.FileName) + "_UniqueValues.xlsx";
            sv.ValidateNames = true;
            sv.OverwritePrompt = true;
            if (sv.ShowDialog() != DialogResult.OK) return;

            if (fullRecords)
            {
                var dataList = (gridUniqueValues.DataSource as DataTable).AsEnumerable()
                    .Select(x => x.Field<string>("COLUMN_VALUE").Replace("'", "''")).ToList();
                _service.ExportDataWithUniqueValues(sv.FileName, _service.CurrentTable, lastUniqueColumnName, dataList);
            }
            else
            {
                gridUniqueValues.ExportToXlsx(sv.FileName);
            }

        }
    }

    public void SetCleanSettings(object ds)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { SetCleanSettings(ds); }));
            return;
        }

        gridCleanSettings.DataSource = null;
        gridCleanSettings.DataSource = ds;
        gridCleanSettings.Refresh();
        if (tcCleanSettings.SelectedTabPage != tpAll)
        {
            gvCleanSettings.BestFitColumns(false);
        }
    }

    private void tcCleanSettings_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
    {
        if (tcCleanSettings.SelectedTabPage == tpAll)
        {
            gridBandConvert.Visible = true;
            gridBandRemove.Visible = true;
            gridBandStandart.Visible = true;
            gridBandSplit.Visible = true;
            gridBandMerge.Visible = true;
            gridBandShift.Visible = true;
            gridBandPattern.Visible = true;
            gridBandWordManager.Visible = true;
            gridBandAiType.Visible = true;
            gridBandAddressSplit.Visible = !(Program.CurrentProgramVersion == ProgramType.CamLte || Program.CurrentProgramVersion == ProgramType.CamFree);
            gvCleanSettings.OptionsView.ColumnAutoWidth = true;
            gridBandConvert.OptionsBand.ShowCaption = true;
            gridBandRemove.OptionsBand.ShowCaption = true;
            gridBandStandart.OptionsBand.ShowCaption = true;
            gridBandSplit.OptionsBand.ShowCaption = true;
            gridBandMerge.OptionsBand.ShowCaption = true;
            gridBandWordManager.OptionsBand.ShowCaption = true;
            gridBandShift.OptionsBand.ShowCaption = true;
            gridBandPattern.OptionsBand.ShowCaption = true;
            gridBandAiType.OptionsBand.ShowCaption = true;
            gridBandAddressSplit.OptionsBand.ShowCaption = !(Program.CurrentProgramVersion == ProgramType.CamLte || Program.CurrentProgramVersion == ProgramType.CamFree); ;
            gvCleanSettings.OptionsView.ColumnAutoWidth = false;
            gvCleanSettings.BestFitColumns(false);

        }
        else
        {
            gridBandConvert.OptionsBand.ShowCaption = false;
            gridBandRemove.OptionsBand.ShowCaption = false;
            gridBandStandart.OptionsBand.ShowCaption = false;
            gridBandSplit.OptionsBand.ShowCaption = false;
            gridBandMerge.OptionsBand.ShowCaption = false;
            gridBandWordManager.OptionsBand.ShowCaption = false;
            gridBandShift.OptionsBand.ShowCaption = false;
            gridBandAddressSplit.OptionsBand.ShowCaption = false;
            gridBandPattern.OptionsBand.ShowCaption = false;
            gridBandAiType.OptionsBand.ShowCaption = false;

            gridBandConvert.Visible = tcCleanSettings.SelectedTabPage == tpConvert;
            gridBandRemove.Visible = tcCleanSettings.SelectedTabPage == tpRemove;
            gridBandStandart.Visible = tcCleanSettings.SelectedTabPage == tpStandart;
            gridBandSplit.Visible = tcCleanSettings.SelectedTabPage == tpSplit;
            gridBandMerge.Visible = tcCleanSettings.SelectedTabPage == tpMerge;
            gridBandWordManager.Visible = tcCleanSettings.SelectedTabPage == tpWordManager;
            gridBandShift.Visible = tcCleanSettings.SelectedTabPage == tpShift;
            gridBandAddressSplit.Visible = tcCleanSettings.SelectedTabPage == tpAddressParser;
            gridBandPattern.Visible = tcCleanSettings.SelectedTabPage == tpPatternManager;
            gridBandAiType.Visible = tcCleanSettings.SelectedTabPage == tpAiType;
            gvCleanSettings.OptionsView.ColumnAutoWidth = false;
            gvCleanSettings.BestFitColumns(false);
            gridBandConvert.OptionsBand.ShowCaption = true;
            gridBandRemove.OptionsBand.ShowCaption = true;
            gridBandStandart.OptionsBand.ShowCaption = true;
            gridBandSplit.OptionsBand.ShowCaption = true;
            gridBandMerge.OptionsBand.ShowCaption = true;
            gridBandWordManager.OptionsBand.ShowCaption = true;
            gridBandShift.OptionsBand.ShowCaption = true;
            gridBandAddressSplit.OptionsBand.ShowCaption = true;
            gridBandPattern.OptionsBand.ShowCaption = true;
            gridBandAiType.OptionsBand.ShowCaption = true;
        }
    }

    private void _service_OnCurrentTableChanged(string tableName)
    {
        var tbpProp = _service.GetTableInfo(tableName);
        SetSelectedPage(tableName);
        SetCleanSettings(_service.GetDataTableCleanSettings(tableName));

        var dt = _service.GetTableStatistic(tableName);
        RefreshUndoButton(tbpProp);
        UpdateStatistic(dt, tbpProp.RowCount, tbpProp.IsStatisticCalculated);
    }

    private void _service_OnTableDelete(ImportedDataInfo importedDataInfo)
    {
        RemovePreviewTable(importedDataInfo.TableName);
        if (!_service.IsAnyTable)
        {
            SetCleanSettings(null);
            UpdateStatistic(null, 1, true);
        }
    }

    private void _service_OnTableDataUpdateComplete(string tableName)
    {
        var tbpProp = _service.GetTableInfo(tableName);

        if (tbpProp == null)
        {
            return;
        }
        UpdatePreview(tableName);
        SetCleanSettings(_service.GetDataTableCleanSettings(tableName));

        var dt = _service.GetTableStatistic(tableName);
        RefreshUndoButton(tbpProp);
        UpdateStatistic(dt, tbpProp.RowCount, tbpProp.IsStatisticCalculated);
    }

    private void RefreshUndoButton(ImportedDataInfo dataInfo)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { RefreshUndoButton(dataInfo); }));
            return;
        }
        var configurationService = WinPureConfigurationDependency.Resolve<IConfigurationService>();
        btnUndoCleansing.Enabled = configurationService.Configuration.AllowUndo && dataInfo.IsUndoAvailable;
    }

    private void _service_OnAddNewData(string tableName)
    {
        string displayName;
        var importedDataInfo = _service.GetTableInfo(tableName);
        if (importedDataInfo != null)
        {
            displayName = "   " + importedDataInfo.DisplayName + "   ";
        }
        else
        {
            displayName = tableName;
        }
        var pw = _service.GetCleanedPreviewTable(tableName);
        AddNewPreviewData(tableName, displayName, importedDataInfo?.SourceType ?? ExternalSourceTypes.NotDefined, pw);
    }

    private void _service_OnChangeTableDisplayName(string tableName, string newDisplayName)
    {
        var pg = tcDataPreview.TabPages.FirstOrDefault(x => x.Tag != null && x.Tag.ToString() == tableName);
        if (pg != null)
        {
            pg.Text = newDisplayName;
        }
    }

    private void _service_OnRefreshData(ImportedDataInfo tableInfo)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { _service_OnRefreshData(tableInfo); }));
            return;
        }

        XtraTabPage tp = null;
        foreach (XtraTabPage page in tcDataPreview.TabPages)
        {
            if (page.Tag.ToString() == tableInfo.TableName)
            {
                tp = page;
            }
        }

        if (tp == null)
        {
            return;
        }

        var pw = _service.GetCleanedPreviewTable(tableInfo.TableName);
        tcDataPreview.BeginUpdate();
        var winPureDataGrid = tp.Controls.Find("wpDataGrid", true).FirstOrDefault() as DataControl;
        winPureDataGrid.UpdateDataSource(pw);
        tcDataPreview.EndUpdate();
        SetCleanSettings(_service.GetDataTableCleanSettings(tableInfo.TableName));
        var dt = _service.GetTableStatistic(tableInfo.TableName);
        UpdateStatistic(dt, tableInfo.RowCount, tableInfo.IsStatisticCalculated);
    }

    private void SetSelectedPage(string tableName)
    {
        if (tcDataPreview.SelectedTabPage == null || tcDataPreview.SelectedTabPage.Tag.ToString() == tableName)
        {
            return;
        }

        _shouldRaiseAnotherTableEvent = false;
        foreach (XtraTabPage page in tcDataPreview.TabPages)
        {
            if (page.Tag.ToString() == tableName)
            {
                tcDataPreview.SelectedTabPage = page;
                _shouldRaiseAnotherTableEvent = true;
                return;
            }
        }
        _shouldRaiseAnotherTableEvent = true;
    }

    internal void ClearTableData()
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(ClearTableData));
            return;
        }

        try
        {
            foreach (XtraTabPage page in tcDataPreview.TabPages)
            {
                var dataControl = page.Controls.Find("wpDataGrid", true).FirstOrDefault() as DataControl;
                if (dataControl != null)
                {
                    dataControl.Dispose();
                }
            }

            tcDataPreview.TabPages.Clear();
            tcDataPreview.Visible = false;
            navFrameCleanStat.SelectedPage = npCleanStatRefresh;

            gridCleanSettings.DataSource = null;
            gridCleanSettings.Refresh();

            gridCleanStatistic.DataSource = null;
            gridCleanStatistic.Refresh();
        }
        catch (Exception ex)
        {
            _logger.Error("Error on clean of data", ex);
        }
    }

    private void UpdateStatistic(object ds, int rowCount, bool statisticReady)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { UpdateStatistic(ds, rowCount, statisticReady); }));
            return;
        }

        navFrameCleanStat.SelectedPage = (statisticReady) ? npCleanStatData : npCleanStatRefresh;

        _rowCount = rowCount;
        gridCleanStatistic.DataSource = null;

        gridCleanStatistic.DataSource = ds;
        gridCleanStatistic.Refresh();

        SetCleanSettings(_service.GetDataTableCleanSettings((_service.CurrentTable)));
    }

    private void UpdatePreview(string tableName)
    {
        var dataSource = _service.GetCleanedPreviewTable(tableName);
        if (tcDataPreview.SelectedTabPage != null)
        {
            var winPureDataControl = tcDataPreview.SelectedTabPage.Controls[0] as DataControl;
            winPureDataControl?.UpdateDataSource(dataSource);
        }
    }

    private void btnRefreshStatistic_Click(object sender, EventArgs e)
    {
        _service.UpdateTableStatistic(_service.CurrentTable);
    }

    private void AddNewPreviewData(string tableName, string displayName, ExternalSourceTypes sourceType, object dataSource)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { AddNewPreviewData(tableName, displayName, sourceType, dataSource); }));
            return;
        }

        tcDataPreview.BeginUpdate();

        var tp = new XtraTabPage();
        var winPureDataControl = new DataControl { Name = "wpDataGrid" };
        winPureDataControl.SetAdditionalSettings(false, false);
        tp.Controls.Add(winPureDataControl);
        winPureDataControl.Dock = DockStyle.Fill;
        tp.Visible = true;
        tp.ShowCloseButton = DefaultBoolean.True;
        tp.Tag = tableName;
        tp.Text = displayName;
        var tableInfo = _service.GetTableInfo(tableName);
        DataHelper.SetTabIconAndTooltip(tp, sourceType, tableInfo.ImportParameters);
        winPureDataControl.SetDataSource(dataSource);
        tcDataPreview.TabPages.Add(tp);
        tcDataPreview.SelectedTabPage = tp;

        tcDataPreview.EndUpdate();
        tcDataPreview.Visible = true;
    }

    private void tcData_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
    {
        if (_shouldRaiseAnotherTableEvent && tcDataPreview.TabPages.Any() && tcDataPreview.SelectedTabPage != null)
        {
            _service.SetSelectedTable(tcDataPreview.SelectedTabPage.Tag.ToString());
        }
    }

    private void RemovePreviewTable(string tableName)
    {
        _shouldRaiseAnotherTableEvent = false;

        XtraTabPage tp = null;
        foreach (XtraTabPage page in tcDataPreview.TabPages)
        {
            if (page.Tag.ToString() == tableName)
            {
                tp = page;
            }
        }

        if (tp != null)
        {
            var dataControl = tp.Controls.Find("wpDataGrid", true).FirstOrDefault() as DataControl;
            if (dataControl != null)
            {
                dataControl.Dispose();
            }

            tcDataPreview.TabPages.Remove(tp);
        }

        _shouldRaiseAnotherTableEvent = true;
    }

    private void ProcessLinkedSettings(List<string> columnList, string column, DataRow rw)
    {
        if (columnList.Contains(column))
        {
            foreach (var colName in columnList.Where(x => x != column))
            {
                if (rw[colName].ToString() == "1")
                {
                    rw[colName] = "0";
                    _service.SaveCleanSettings(rw[0].ToString(), colName, "0");
                }
            }
        }

    }

    private void gvCleanSettings_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
    {
        if (e.Column.FieldName != "" && e.Column.FieldName != "PATTERN" && e.Column.FieldName != "AITYPE")
        {
            var rw = gvCleanSettings.GetRow(e.RowHandle) as DataRowView;
            if (rw == null)
            {
                return;
            }

            ProcessLinkedSettings(_standartizeFiledList, e.Column.FieldName, rw.Row);
            ProcessLinkedSettings(_splitFiledList, e.Column.FieldName, rw.Row);
            ProcessLinkedSettings(_shiftFiledList, e.Column.FieldName, rw.Row);

            var newValue = e.Value;

            if (e.Column.FieldName == "ST_ProperCaseSettings")
            {
                var checkBoxEdit = e.Column.ColumnEdit as RepositoryItemCheckedComboBoxEdit;
                var properCaseSettings = new ProperCaseSettings
                {
                    UseDelimiter = checkBoxEdit.Items[0].CheckState == CheckState.Checked,
                    UsePrefix = checkBoxEdit.Items[1].CheckState == CheckState.Checked,
                    UseExceptions = checkBoxEdit.Items[2].CheckState == CheckState.Checked
                };
                newValue = JsonConvert.SerializeObject(properCaseSettings);
                rw.Row["ST_ProperCaseSettings"] = newValue;
                rw["ST_ProperCaseSettings"] = newValue;
            }

            if (e.Column.FieldName == "RE_Expression")
            {
                newValue = e.OldValue;
                rw.Row["RE_Expression"] = newValue;
                rw["RE_Expression"] = newValue;
            }

            _service.SaveCleanSettings(rw.Row[0].ToString(), e.Column.FieldName, newValue);

            UpdatePreview(_service.CurrentTable);

            gvCleanSettings.RefreshData();
        }
    }

    private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        var frmWordManagerSettings = WinPureUiDependencyResolver.Resolve<frmWordManagerSettings>();
        var focusedRow = (gvCleanSettings.GetRow(gvCleanSettings.FocusedRowHandle) as DataRowView)?.Row;

        frmWordManagerSettings.Show(focusedRow["COLUMN_NAME"].ToString());
        if (frmWordManagerSettings.DialogResult == DialogResult.OK)
        {
            SetCleanSettings(_service.GetDataTableCleanSettings(_service.CurrentTable));
            UpdatePreview(_service.CurrentTable);
        }
    }

    private void gvCleanSettings_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
    {
        UpdateValueCountData(lastUniqueColumnName, false);
        if (navFrameCleanStat.SelectedPage != npCleanStatData)
        {
            return;
        }

        if (!(gvCleanSettings.GetRow(e.FocusedRowHandle) is DataRowView selectedRow))
        {
            return;
        }

        var colName = selectedRow["COLUMN_NAME"].ToString();

        for (int i = 0; i < gvCleanStatistic.RowCount; i++)
        {
            if (!(gvCleanStatistic.GetRow(i) is DataRowView rw))
            {
                return;
            }

            if (rw["FieldName"].ToString() == colName)
            {
                gvCleanStatistic.FocusedRowHandle = i;
                return;
            }
        }
    }

    private void gvCleanStatistic_RowCellClick(object sender, RowCellClickEventArgs e)
    {
        if (gvCleanStatistic.GetRow(e.RowHandle) is DataRowView selectedRow)
        {
            var colName = selectedRow["FieldName"].ToString();
            for (int i = 0; i < gvCleanStatistic.RowCount; i++)
            {
                if (!(gvCleanSettings.GetRow(i) is DataRowView rw))
                {
                    return;
                }
                if (rw["COLUMN_NAME"].ToString() == colName)
                {
                    gvCleanSettings.FocusedRowHandle = i;

                    switch (e.Column.FieldName)
                    {
                        case "TrailingSpaces": gvCleanSettings.FocusedColumn = bandedGridColumn2; break;
                        case "Commas": gvCleanSettings.FocusedColumn = bandedGridColumn3; break;
                        case "Dots": gvCleanSettings.FocusedColumn = bandedGridColumn4; break;
                        case "Hyphens": gvCleanSettings.FocusedColumn = bandedGridColumn5; break;
                        case "Apostrophes": gvCleanSettings.FocusedColumn = bandedGridColumn6; break;
                        case "LeadingSpaces": gvCleanSettings.FocusedColumn = bandedGridColumn26; break;
                        case "Letters": gvCleanSettings.FocusedColumn = bandedGridColumn7; break;
                        case "Numbers": gvCleanSettings.FocusedColumn = bandedGridColumn8; break;
                        case "NonPrintable": gvCleanSettings.FocusedColumn = bandedGridColumn27; break;
                        case "WithSpaces": gvCleanSettings.FocusedColumn = bandedGridColumn28; break;
                        case "MultipleSpaces": gvCleanSettings.FocusedColumn = bandedGridColumn29; break;
                        case "NewLineChar": gvCleanSettings.FocusedColumn = bandedGridColumn30; break;
                        case "TabChar": gvCleanSettings.FocusedColumn = bandedGridColumn31; break;
                            //case "Punctuation": gvCleanSettings.FocusedColumn = bandedGridColumn2; break;
                    }
                    return;
                }
            }
        }
    }

    private void UpdateValueCountData(string columnName, bool refreshData, TextCleanerSetting cleanerSett = null, CaseConverterInternalSetting caseSett = null)
    {
        var data = _service.GetColumnUniqueValues(columnName, refreshData, cleanerSett, caseSett);
        lastUniqueColumnName = columnName;
        gridUniqueValues.DataSource = null;
        gridUniqueValues.DataSource = data;
        gridUniqueValues.Refresh();
    }

    private void gvCleanSettings_DoubleClick(object sender, EventArgs e)
    {
        if (gvCleanSettings.GetSelectedRows().Length == 0)
        {
            return;
        }
        lbl_double_click.Visible = false;

        if (gvCleanSettings.GetRow(gvCleanSettings.GetSelectedRows()[0]) is DataRowView selectedRow)
        {
            var colName = selectedRow["COLUMN_NAME"].ToString();
            UpdateValueCountData(colName, true);
        }
    }

    private void gvCleanStatistic_DoubleClick(object sender, EventArgs e)
    {
        if (gvCleanStatistic.GetSelectedRows().Length == 0)
        {
            return;
        }

        if (!(gvCleanStatistic.GetRow(gvCleanStatistic.GetSelectedRows()[0]) is DataRowView selectedRow))
        {
            return;
        }

        var fieldName = selectedRow["FieldName"].ToString();
        lbl_double_click.Visible = false;

        TextCleanerSetting cleanerSetting = null;
        CaseConverterInternalSetting caseSetting = null;

        GridView view = (GridView)sender;
        Point pt = view.GridControl.PointToClient(Control.MousePosition);
        var info = view.CalcHitInfo(pt);

        if (info.InRowCell)
        {
            if (info.Column != null)
            {
                switch (info.Column.FieldName)
                {

                    case "TrailingSpaces":
                        cleanerSetting = new TextCleanerSetting { RemoveTrailingSpace = true };
                        break;
                    case "MultipleSpaces":
                        cleanerSetting = new TextCleanerSetting { RemoveMultipleSpaces = true };
                        break;
                    case "LeadingSpaces":
                        cleanerSetting = new TextCleanerSetting { RemoveLeadingSpace = true };
                        break;
                    case "Commas":
                        cleanerSetting = new TextCleanerSetting { RemoveCommas = true };
                        break;
                    case "Dots":
                        cleanerSetting = new TextCleanerSetting { RemoveDots = true };
                        break;
                    case "Hyphens":
                        cleanerSetting = new TextCleanerSetting { RemoveHyphens = true };
                        break;
                    case "Apostrophes":
                        cleanerSetting = new TextCleanerSetting { RemoveApostrophes = true };
                        break;
                    case "NewLineChar":
                        cleanerSetting = new TextCleanerSetting { RemoveNewLine = true };
                        break;
                    case "TabChar":
                        cleanerSetting = new TextCleanerSetting { RemoveTabs = true };
                        break;
                    case "NonPrintable":
                        cleanerSetting = new TextCleanerSetting { RemoveNonPrintableCharacters = true };
                        break;
                    case "Numbers":
                        cleanerSetting = new TextCleanerSetting { RemoveAllDigits = true };
                        break;
                    case "WithSpaces":
                        cleanerSetting = new TextCleanerSetting { RemoveAllSpaces = true };
                        break;
                    case "Punctuation":
                        cleanerSetting = new TextCleanerSetting { RemovePunctuation = true };
                        break;
                    case "Letters":
                        cleanerSetting = new TextCleanerSetting { RemoveAllLetters = true };
                        break;
                    case "MatchPattern":
                        var tableInfo = _service.GetCurrentTableInfo;
                        var dataField = tableInfo.Fields.FirstOrDefault(x => x.DatabaseName == fieldName || x.DisplayName == fieldName);
                        cleanerSetting = new TextCleanerSetting { RegexExpression = new List<RegexConfigurationSetting>() { new RegexConfigurationSetting { Expression = dataField?.Pattern, Replacement = "" } } };
                        break;
                    case "UnmatchPattern":
                        var tableInfo2 = _service.GetCurrentTableInfo;
                        var dataField2 = tableInfo2.Fields.FirstOrDefault(x => x.DatabaseName == fieldName || x.DisplayName == fieldName);
                        cleanerSetting = new TextCleanerSetting { RegexExpression = new List<RegexConfigurationSetting>() { new RegexConfigurationSetting { Expression = dataField2?.Pattern, Replacement = "NOT" } } };
                        break;
                    case "UpperOnly":
                        caseSetting = new CaseConverterInternalSetting { ToUpperCase = true };
                        break;
                    case "LowerOnly":
                        caseSetting = new CaseConverterInternalSetting { ToLowerCase = true };
                        break;
                    case "ProperCase":
                        caseSetting = new CaseConverterInternalSetting { ToProperCase = true };
                        break;
                    case "MixedCase":
                        caseSetting = new CaseConverterInternalSetting { ToMixedCase = true };
                        break;
                }
            }
        }

        UpdateValueCountData(fieldName, true, cleanerSetting, caseSetting);
    }

    private void btnRunClean_ItemClick(object sender, EventArgs e)
    {
        _service.CleanDataAsync();
    }

    private void gvCleanStatistic_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
    {
        if (_columnProgressBar.Contains(e.Column.FieldName))
        {
            var rp = new RepositoryItemProgressBar
            {
                ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid,
                ShowTitle = true,
                PercentView = false
            };

            rp.LookAndFeel.UseDefaultLookAndFeel = false;
            rp.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            //rp.StartColor = DXColor.FromArgb(0, 170, 255);
            //rp.EndColor = DXColor.FromArgb(0, 120, 200);
            rp.EndColor = DXColor.FromArgb(227, 242, 250);
            rp.StartColor = DXColor.FromArgb(140, 191, 226);
            rp.Appearance.ForeColor = Color.Black;
            rp.Appearance.ForeColor2 = Color.Black;


            ConfigureFullBarForNonZero(rp, e.CellValue);

            e.RepositoryItem = rp;
            return;
        }

        if (_columnRed.Contains(e.Column.FieldName))
        {
            var rp = new RepositoryItemProgressBar
            {
                ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid,
                ShowTitle = true,
                PercentView = false
            };

            rp.LookAndFeel.UseDefaultLookAndFeel = false;
            rp.LookAndFeel.Style = LookAndFeelStyle.Flat;
            //rp.StartColor = DXColor.FromArgb(247, 124, 124);
            //rp.EndColor = DXColor.FromArgb(116, 0, 0);
            //rp.EndColor = DXColor.FromArgb(243, 207, 206);
            rp.Appearance.ForeColor = Color.Black;
           rp.Appearance.ForeColor2 = Color.Black;

            rp.StartColor = DXColor.FromArgb(227,154, 150);
            rp.EndColor = DXColor.FromArgb(246, 226, 225);

            ConfigureFullBarForNonZero(rp, e.CellValue);

            e.RepositoryItem = rp;
            return;
        }

        if (_columnAmber.Contains(e.Column.FieldName))
        {
            var rp = new RepositoryItemProgressBar
            {
                ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid,
                ShowTitle = true,
                PercentView = false
            };

            rp.LookAndFeel.UseDefaultLookAndFeel = false;
            rp.LookAndFeel.Style = LookAndFeelStyle.Flat;
            //rp.StartColor = DXColor.FromArgb(255, 255, 153);
            //rp.EndColor = DXColor.FromArgb(218, 126, 8);
           //p.Appearance.ForeColor = Color.Black;
            rp.StartColor = DXColor.FromArgb(236, 183, 92);
            rp.EndColor = DXColor.FromArgb(252, 236, 185);
            rp.Appearance.ForeColor = Color.Black;
            rp.Appearance.ForeColor2 = Color.Black;

            ConfigureFullBarForNonZero(rp, e.CellValue);

            e.RepositoryItem = rp;
            e.Column.AppearanceCell.ForeColor = DXSkinColors.ForeColors.WindowText;
            return;
        }
    }

    private static void ConfigureFullBarForNonZero(RepositoryItemProgressBar rp, object cellValue)
    {
        var value = 0;

        if (cellValue != null && cellValue != DBNull.Value)
        {
            _ = int.TryParse(cellValue.ToString(), out value);
        }

        rp.Minimum = 0;

        // If value is > 0, make it render as "full bar" by setting Maximum == Value.
        // If value is 0, keep Maximum as 1 so it renders empty.
        rp.Maximum = value > 0 ? value : 1;
    }

    private readonly List<string> _columnRed = new List<string> { "UnmatchPattern", "TrailingSpaces", "Commas", "Dots", "Hypens", "Apostrophes", "LeadingSpaces", "Letters", "Numbers", "NonPrintable", "WithSpaces", "MultipleSpaces", "NewLineChar", "TabChar" };
    private readonly List<string> _columnAmber = new List<string> { "Punctuation", "UpperOnly", "LowerOnly", "ProperCase" };
    private readonly List<string> _columnProgressBar = new List<string> { "Filled", "Empty", "Distinct", "MatchPattern" };

    private void btnRunStatistics_ItemClick(object sender, EventArgs e)
    {
        _service.UpdateTableStatistic(_service.CurrentTable);
    }

    private void tcCleanSettings_CustomHeaderButtonClick(object sender, DevExpress.XtraTab.ViewInfo.CustomHeaderButtonEventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.Clean);
    }

    private void groupControl1_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.DataPreview);
    }

    private void CleanToolTipController_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
    {
        try
        {
            Process.Start(e.Link);
        }
        catch (Exception ex)
        {
            _logger.Error("Web link open fail", ex);
        }
    }

    private void gvCleanStatistic_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
    {
        if (e.HitInfo?.Column != null && (e.HitInfo.Column.FieldName == "Filled" || e.HitInfo.Column.FieldName == "Empty"))
        {
            var view = sender as GridView;
            filterOnDataTableToolStripMenuItem.Tag = e.HitInfo.Column.FieldName;
            contextMenuStrip1.Show(view.GridControl, e.Point);
        }
    }

    private void filterOnDataTableToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (gvCleanStatistic.GetSelectedRows().Length == 0)
        {
            return;
        }

        if (!(gvCleanStatistic.GetRow(gvCleanStatistic.GetSelectedRows()[0]) is DataRowView selectedRow))
        {
            return;
        }
        var menuItem = sender as ToolStripMenuItem;
        if (menuItem?.Tag == null)
        {
            return;
        }

        _service.FiltrateMainData(selectedRow["FieldName"].ToString(),
            menuItem.Tag.ToString() == "Filled" ? FiltrateField.Filled : FiltrateField.Empty);
    }

    private void gvCleanSettings_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
    {
        if (e.FocusedColumn != null && e.FocusedColumn.ColumnEdit == repoCheckEdit)
        {
            wpSelection.FieldName = e.FocusedColumn.Caption;
        }
    }

    private void btnExportStatistic_Click(object sender, EventArgs e)
    {
        ExportStatistic();
    }

    private void btnUndoCleansing_Click(object sender, EventArgs e)
    {
        _service.UndoClean();
    }

    private void gvCleanSettings_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
    {
        if (e.Column.FieldName == "ST_ProperCaseSettings")
        {
            var gv = sender as GridView;
            var checkBoxEdit = e.RepositoryItem as RepositoryItemCheckedComboBoxEdit;
            var rw = (gv.GetRow(e.RowHandle) as DataRowView)?.Row;
            var rowValue = rw["ST_ProperCaseSettings"].ToString();
            if (checkBoxEdit != null && !string.IsNullOrEmpty(rowValue) && rowValue.StartsWith("{") && rowValue.EndsWith("}"))
            {
                var properCaseSettings = string.IsNullOrEmpty(rw["ST_ProperCaseSettings"].ToString())
                    ? new ProperCaseSettings()
                    : JsonConvert.DeserializeObject<ProperCaseSettings>(rw["ST_ProperCaseSettings"].ToString());
                rowValue = "";
                if (properCaseSettings.UseDelimiter)
                {
                    checkBoxEdit.Items[0].CheckState = CheckState.Checked;
                    rowValue = checkBoxEdit.Items[0].Value.ToString();
                }
                if (properCaseSettings.UsePrefix)
                {
                    checkBoxEdit.Items[1].CheckState = CheckState.Checked;
                    rowValue = string.IsNullOrEmpty(rowValue) ? checkBoxEdit.Items[1].Value.ToString() : rowValue + ", " + checkBoxEdit.Items[1].Value.ToString();
                }
                if (properCaseSettings.UseExceptions)
                {
                    checkBoxEdit.Items[2].CheckState = CheckState.Checked;
                    rowValue = string.IsNullOrEmpty(rowValue) ? checkBoxEdit.Items[2].Value.ToString() : rowValue + ", " + checkBoxEdit.Items[2].Value.ToString();
                }

                rw["ST_ProperCaseSettings"] = rowValue;
            }
        }
    }

    private void repositoryItemCheckedComboBoxEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        if (e.Button.Tag != null && e.Button.Tag.ToString() == "1")
        {
            var frmProperCaseSettings = WinPureUiDependencyResolver.Resolve<frmProperCaseConfiguration>();
            if (frmProperCaseSettings.ShowDialog() == DialogResult.OK)
            {
                UpdatePreview(_service.CurrentTable);
            }
        }
    }

    private void btnExportUniqueValues_Click(object sender, EventArgs e)
    {
        ExportUniqueValues();
    }

    private void edtPatterns_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        if (e.Button.Index == 1) //delete button
        {
            if (gvCleanSettings.GetRow(gvCleanSettings.FocusedRowHandle) is DataRowView rw)
            {
                var edit = sender as LookUpEdit;
                edit.EditValue = String.Empty;
            }
        }
        else if (e.Button.Index == 2) //edit patterns
        {
            var frmPatternConfiguration = WinPureUiDependencyResolver.Resolve<frmPatternConfiguration>();
            frmPatternConfiguration.ShowDialog();
            RefreshSupportEditor();
        }
    }

    private void edtPatterns_EditValueChanged(object sender, EventArgs e)
    {
        if (gvCleanSettings.GetRow(gvCleanSettings.FocusedRowHandle) is DataRowView rw)
        {
            var columnName = rw["COLUMN_NAME"].ToString();
            var edit = sender as LookUpEdit;
            var pattern = edit.EditValue.ToString();
            var importedInfo = _service.GetCurrentTableInfo;
            var field = importedInfo.Fields.First(x => x.DatabaseName == columnName || x.DisplayName == columnName);
            field.Pattern = pattern;
        }
    }

    private void edtAiType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        if (e.Button.Index == 1) //delete button
        {
            if (gvCleanSettings.GetRow(gvCleanSettings.FocusedRowHandle) is DataRowView rw)
            {
                var edit = sender as LookUpEdit;
                edit.EditValue = String.Empty;
            }
        }
        else if (e.Button.Index == 2) //edit ai type
        {
            var frmAiConfiguration = WinPureUiDependencyResolver.Resolve<frmCleansingAiConfiguration>();
            frmAiConfiguration.ShowDialog();
            RefreshSupportEditor();
        }
    }

    private void edtAiType_EditValueChanged(object sender, EventArgs e)
    {
        if (gvCleanSettings.GetRow(gvCleanSettings.FocusedRowHandle) is DataRowView rw)
        {
            var columnName = rw["COLUMN_NAME"].ToString();
            var edit = sender as LookUpEdit;
            var aiType = edit.EditValue.ToString();
            var importedInfo = _service.GetCurrentTableInfo;
            var field = importedInfo.Fields.First(x => x.DatabaseName == columnName || x.DisplayName == columnName);
            field.AiType = aiType;
        }
    }

    private void barBtnUniqueValues_ItemClick(object sender, ItemClickEventArgs e)
    {
        ExportUniqueValues();
    }

    private void barBtnFullRecords_ItemClick(object sender, ItemClickEventArgs e)
    {
        ExportUniqueValues(true);
    }

    private void pictureEdit1_Click(object sender, EventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.DataProfilingStatistics);
    }

    private void OnThemeChanged(object sender, EventArgs e)
    {
        var wordManagerEditButton = repositoryItemButtonEdit1.Buttons[0];
        var editAiButton = edtAiType.Buttons[2];
        var editPatternsButton = edtPatterns.Buttons[2];

        wordManagerEditButton.Appearance.ForeColor = Color.Black;
        wordManagerEditButton.Appearance.Options.UseForeColor = true;

        editAiButton.Appearance.ForeColor = Color.Black;
        editAiButton.Appearance.Options.UseForeColor = true;
        editPatternsButton.Appearance.ForeColor = Color.Black;
        editPatternsButton.Appearance.Options.UseForeColor = true;

        if (IsDarkTheme())
        {

            wordManagerEditButton.AppearanceHovered.ForeColor = Color.White;
            wordManagerEditButton.AppearanceHovered.Options.UseForeColor = true;
            editAiButton.AppearanceHovered.ForeColor = Color.White;
            editAiButton.AppearanceHovered.Options.UseForeColor = true;
            editPatternsButton.AppearanceHovered.ForeColor = Color.White;
            editPatternsButton.AppearanceHovered.Options.UseForeColor = true;

            // Set all appearance states for colMergerOrder's repository control 
            repositoryItemSpinEdit1.Appearance.ForeColor = Color.White;
            repositoryItemSpinEdit1.Appearance.Options.UseForeColor = true;

            repositoryItemSpinEdit1.AppearanceFocused.ForeColor = Color.White;
            repositoryItemSpinEdit1.AppearanceFocused.Options.UseForeColor = true;

            repositoryItemSpinEdit1.AppearanceDisabled.ForeColor = Color.Gray;
            repositoryItemSpinEdit1.AppearanceDisabled.Options.UseForeColor = true;

            repositoryItemSpinEdit1.AppearanceReadOnly.ForeColor = Color.White;
            repositoryItemSpinEdit1.AppearanceReadOnly.Options.UseForeColor = true;
        }
        else
        {
            wordManagerEditButton.AppearanceHovered.ForeColor = Color.FromArgb(0, 0, 0);
            wordManagerEditButton.AppearanceHovered.Options.UseForeColor = true;
            editAiButton.AppearanceHovered.ForeColor = Color.FromArgb(0, 0, 0);
            editAiButton.AppearanceHovered.Options.UseForeColor = true;
            editPatternsButton.AppearanceHovered.ForeColor = Color.FromArgb(0, 0, 0);
            editPatternsButton.AppearanceHovered.Options.UseForeColor = true;

            // Set all appearance states for colMergerOrder's repository control 
            repositoryItemSpinEdit1.Appearance.ForeColor = Color.Black;
            repositoryItemSpinEdit1.Appearance.Options.UseForeColor = true;

            repositoryItemSpinEdit1.AppearanceFocused.ForeColor = Color.Black;
            repositoryItemSpinEdit1.AppearanceFocused.Options.UseForeColor = true;

            repositoryItemSpinEdit1.AppearanceDisabled.ForeColor = Color.Gray;
            repositoryItemSpinEdit1.AppearanceDisabled.Options.UseForeColor = true;

            repositoryItemSpinEdit1.AppearanceReadOnly.ForeColor = Color.Black;
            repositoryItemSpinEdit1.AppearanceReadOnly.Options.UseForeColor = true;
        }

    }

    private bool IsDarkTheme()
    {
        return _themeDetectionService.IsDarkTheme();
    }

    private void btnApplyAi_Click(object sender, EventArgs e)
    {
        _service.UpdateCleansingOptionsBasedOnAi(_service.CurrentTable);
        SetCleanSettings(_service.GetDataTableCleanSettings(_service.CurrentTable));
        UpdatePreview(_service.CurrentTable);
    }

    private void regexLookupEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        if (e.Button.Index == 0) //show button
        {
            if (gvCleanSettings.GetRow(gvCleanSettings.FocusedRowHandle) is DataRowView rw)
            {
                var configuration = rw["RE_Expression"].ToString();
                var source = string.IsNullOrWhiteSpace(configuration) ? new List<RegexConfigurationSetting>() : JsonConvert.DeserializeObject<List<RegexConfigurationSetting>>(configuration);
                var edit = sender as LookUpEdit;
                var hasItems = source?.Count > 0;
                edit.Properties.DataSource = hasItems ? source.OrderBy(x => x.Id) : new List<RegexConfigurationSetting> { new RegexConfigurationSetting() };
                if (!hasItems)
                {
                    edit.Properties.PopupFormMinSize = new Size(edit.Properties.PopupFormMinSize.Width, edit.Height * edit.Properties.DropDownRows);
                }
                edit.ShowPopup();
            }
        }
        else if (e.Button.Index == 1) //remove REGEX patterns
        {
            if (gvCleanSettings.GetRow(gvCleanSettings.FocusedRowHandle) is DataRowView rw)
            {
                _service.SaveCleanSettings(rw[0].ToString(), "RE_Expression", string.Empty);
                SetCleanSettings(_service.GetDataTableCleanSettings(_service.CurrentTable));
                UpdatePreview(_service.CurrentTable);
            }
        } else if (e.Button.Index == 2) //edit REGEX patterns
        {
            if (gvCleanSettings.GetRow(gvCleanSettings.FocusedRowHandle) is DataRowView rw)
            {
                var frmRegexConfiguration = WinPureUiDependencyResolver.Resolve<frmCleansingRegex>();
                if (frmRegexConfiguration.Show(rw["COLUMN_NAME"].ToString(), rw["RE_Expression"].ToString()) == DialogResult.OK)
                {
                    SetCleanSettings(_service.GetDataTableCleanSettings(_service.CurrentTable));
                    UpdatePreview(_service.CurrentTable);
                }
            }
        }
    }

    private void gvCleanSettings_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
    {
        if (e.Column.FieldName == "RE_Expression")
        {
            var value = e.Value?.ToString();
            if (!string.IsNullOrWhiteSpace(value))
            {
                e.DisplayText = "REGEX";
            }
        }
    }

    private void RegexLookupEdit_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
    {
        var value = e.Value?.ToString();
        if (!string.IsNullOrWhiteSpace(value))
        {
            e.DisplayText = "REGEX";
        }
    }

    private void RegexLookupEdit_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
    {
        e.Cancel = true;
    }
}