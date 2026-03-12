using DevExpress.LookAndFeel;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraNavBar;
using System.ComponentModel;
using System.Diagnostics;
using WinPure.CleanAndMatch.StartupForm;
using WinPure.Project.Models;


namespace WinPure.CleanAndMatch;

public partial class frmMain : RibbonForm
{
    private readonly IDataManagerService _service;
    private readonly IProjectService _projectService;
    private LoadingMaskForm _frmProgress;
    private readonly IWpLogger _logger;
    private readonly IConfigurationService _configurationService;
    private readonly ILicenseService _licenseService;
    private IRecentProjectService _recentProjectService;
    private readonly ThemeDetectionService _themeDetectionService;

    private DevExpress.XtraBars.Navigation.AccordionControlElement _selectedAccordionElement = null;
    private int _latestProjectsHotRowHandle = GridControl.InvalidRowHandle;

    private TileViewItemElement _latestProjectsHotElement;

    private const string IsCleansingTileElement = "IsCleansingTileElement";
    private const string IsMatchTileElement = "IsMatchTileElement";
    private const string IsMatchAiTileElement = "IsMatchAiTileElement";
    private const string IsAddressVerificationTileElement = "IsAddressVerificationTileElement";
    private const string IsAutomationTileElement = "IsAutomationTileElement";
    private const string IsAuditLogTileElement = "IsAuditLogTileElement";

    public frmMain()
    {
        InitializeComponent();
        Localization();
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        _configurationService = WinPureUiDependencyResolver.Resolve<IConfigurationService>();
        UserLookAndFeel.Default.SetSkinStyle(SkinStyle.WXICompact, "Office Colorful");

        _themeDetectionService = WinPureUiDependencyResolver.Resolve<ThemeDetectionService>();
        _themeDetectionService.SetReferenceControl(mnuMainAccordion);

        barToggleSwitchMatchTableAutoFiler.Checked = _configurationService.Configuration.ShowAutoFilterRow;
        barToggleSwitchMatchResultSystemFields.Checked = _configurationService.Configuration.ShowSystemFields;
        barToggleSwitchMatchAiTableAutoFiler.Checked = _configurationService.Configuration.ShowAutoFilterRow;
        barToggleSwitchMatchAiResultSystemFields.Checked = _configurationService.Configuration.ShowSystemFields;
        ucMainMatch.SetFilterRowVisibility(_configurationService.Configuration.ShowAutoFilterRow);
        ucMainMatch.UpdateSystemColumnVisibility();
        ucEntityResolution.SetFilterRowVisibility(_configurationService.Configuration.ShowAutoFilterRow);
        ucEntityResolution.UpdateSystemColumnVisibility();
        barToggleSwitchMatchTableAutoFiler.CheckedChanged += barToggleSwitchMatchTableAutoFiler_CheckedChanged;
        barToggleSwitchMatchResultSystemFields.CheckedChanged += barToggleSwitchMatchResultSystemFields_CheckedChanged;
        barToggleSwitchMatchAiTableAutoFiler.CheckedChanged += barToggleSwitchMatchAiTableAutoFiler_CheckedChanged;
        barToggleSwitchMatchAiResultSystemFields.CheckedChanged += barToggleSwitchMatchAiResultSystemFields_CheckedChanged;


        // Listen to internal Match frame page changes
        ucMainMatch.navFrameMatching.SelectedPageChanged += ucMainMatch_SelectedPageChanged;
        ucEntityResolution.navFrameEntityResolution.SelectedPageChanged += ucEntityResolution_SelectedPageChanged;

        navFraimMain.SelectedPage = navPageData;
        menuLastProject.OptionsNavPane.NavPaneState = NavPaneState.Expanded;
        //mainRibbon.Minimized = true;

        _licenseService = WinPureUiDependencyResolver.Resolve<ILicenseService>();
        SetControlTextAndAvailability();
        _licenseService.LicenseLoaded += _licenseService_LicenseLoaded;

        _logger = WinPureUiDependencyResolver.Resolve<IWpLogger>();

        _service = WinPureUiDependencyResolver.Resolve<IDataManagerService>();

        _service.OnException += InformationDisplay;
        _service.OnProgressShow += ProgressFormShow;
        _service.OnProgressUpdate += _service_OnProgressUpdate;
        _service.OnAddNewData += _service_OnAddNewData;

        _service.OnTableDelete += _service_OnTableDelete;
        _service.OnMatchResultReady += _service_OnMatchResultReady;
        _service.OnAddressVerificationReady += _service_AddressVerificationReady;
        _service.OnStatisticUpdated += _service_OnStatisticUpdated;
        _service.OnFiltrateData += _service_FiltrateData;

        _service.OnEntityResolutionReady += _service_OnEntityResolutionReady;

        _projectService = WinPureUiDependencyResolver.Resolve<IProjectService>();
        _projectService.OnProjectNameChanged += _service_OnProjectNameChanged;
        _projectService.OnBeforeProjectLoad += _service_BeforeProjectLoad;
        _projectService.OnException += InformationDisplay;
        _projectService.OnProgressShow += ProgressFormShow;

        // Subscribe to theme change event
        UserLookAndFeel.Default.StyleChanged += OnThemeChanged;

        ucMainData.OnOpenProject += UsMainData_OnOpenProject;
        ucMainData.OnSaveProject += SaveProject;
        ucMainData.OnCreateNewProject += CreateNewProject;
        ucMainData.OnOpenSettings += UcMainData_OnOpenSettings;
        ucMainData.OnException += InformationDisplay;

        ucMainData.Initialize(true);
        ucMainCleanNew.Initialize();

        ucVerification.Initialize();
        ucVerification.OnNavigateToFullReport += UcVerification_OnNavigateToFullReport;
        ucVerification.OnException += InformationDisplay;

        ucEntityResolution.Initialize();

        ucAutomation.Initialize();
        ucAuditLogs.Initialize();

        if (ProgramTypeHelper.AutomationPrograms.Contains(Program.CurrentProgramVersion))
        {
            ucAutomation.OnException += InformationDisplay;
        }
        else
        {
            acitAutomationConfiguration.Enabled = acitAutomationLog.Enabled = false;
        }

        if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
        {
            _recentProjectService = WinPureUiDependencyResolver.Resolve<IRecentProjectService>();
            _recentProjectService.OnRecentListChanged += _recentProjectService_OnRecentListChanged;
            UpdateRecentProject();
        }

        btnEditNormalization.Enabled = !ProgramTypeHelper.CamTypes.Contains(Program.CurrentProgramVersion);

        ucMainMatch.Initialize();
        ucMainMatch.OnException += InformationDisplay;
        ucMainMatch.OnNavigateToFullReport += UcMainMatch_OnNavigateToFullReport;

        ucAuditLogs.OnException += InformationDisplay;
        ucAuditLogs.OnProgressShow += ProgressFormShow;

        var licState = _licenseService.IsDemo ? "DEMO" : "LICENSE";
        _logger.Information($"WinPure {Program.CurrentProgramVersion}, Version:{Common.Helpers.AssemblyHelper.ApplicationVersion()} {licState} Started {DateTime.Now.ToShortDateString()}.");// System: {si.GetInfo()}");

        // Add resize event handler for responsive layout
        this.Resize += FrmMain_Resize;
        this.Load += (s, e) => AdjustLayoutForFormSize();

        mainRibbon.Minimized = true;
    }

    private void FrmMain_Resize(object sender, EventArgs e)
    {
        if (WindowState != FormWindowState.Minimized)
        {
            AdjustLayoutForFormSize();
        }
    }

    private void AdjustLayoutForFormSize()
    {
        // Ensure we don't adjust during loading or when form is minimized
        if (this.Width <= 0 || this.WindowState == FormWindowState.Minimized)
        {
            return;
        }

        // Determine threshold for collapsing left accordion
        // When form width is less than 1400px, minimize the left accordion to save space
        const int COLLAPSE_THRESHOLD = 1400;
        const int EXPAND_THRESHOLD = 1600;

        if (this.Width < COLLAPSE_THRESHOLD)
        {
            if (mnuMainAccordion.OptionsMinimizing.State != DevExpress.XtraBars.Navigation.AccordionControlState.Minimized)
            {
                mnuMainAccordion.OptionsMinimizing.State = DevExpress.XtraBars.Navigation.AccordionControlState.Minimized;
            }
        }
        else if (this.Width >= EXPAND_THRESHOLD)
        {
            if (mnuMainAccordion.OptionsMinimizing.State == DevExpress.XtraBars.Navigation.AccordionControlState.Minimized)
            {
                mnuMainAccordion.OptionsMinimizing.State = DevExpress.XtraBars.Navigation.AccordionControlState.Normal;
            }
        }
        // Force refresh to apply changes
        menuLastProject.Refresh();
    }

    internal static bool ProjectSaveAndContinue(string projectName)
    {
        var res = MessageBox.Show(string.Format(Resources.MESSAGE_MAINFORM_WANTSAVETHEPROJECT, projectName), Resources.MESSAGE_QUESTION_CAPTION, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        switch (res)
        {
            case DialogResult.Cancel:
                return false;
            case DialogResult.Yes:
                ProjectHelper.SaveProject(false);
                break;
            case DialogResult.No:
                break;
        }
        return true;
    }

    private void Localization()
    {
        this.cbShowPreview.Caption = Resources.UI_UCMAINCLEANNEWFORM_DATAPREVIEW;
        (cbShowPreview.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_MAINFORM_HIDEDISPLAYDATAPREVIEW;
        this.cbShowStatistic.Caption = Resources.UI_MAINFORM_STATISTICS;
        (cbShowStatistic.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_MAINFORM_HIDEDISPLAYSTATISTICSFOREACHTABLE;
        this.qbtnOpenProject.Caption = Resources.UI_UCNEWPROJECTFORM_OPENPROJECT;
        this.qbtnSaveProject.Caption = Resources.DIALOG_PROJECT_SAVE_CAPTION;
        this.qbtnSaveAsProject.Caption = Resources.DIALOG_PROJECT_SAVEAS_CAPTION;
        this.btnRunClean.Caption = Resources.UI_MAINFORM_RUN_CLEAN;
        (btnRunClean.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_MAINFORM_PERFORM_DATA_CLEAN;
        this.qbtnImportData.Caption = Resources.UI_MAINFORM_IMPORTDATA;
        this.qbtnExportData.Caption = Resources.UI_MAINFORM_EXPORTDATA;
        this.btnCleanSettings.Caption = Resources.UI_CLEAR;
        (btnCleanSettings.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_MAINFORM_CLEARALLSETTINGS;
        this.btnLoadMatrix.Caption = Resources.UI_MAINFORM_LOADMATRIX;
        (btnLoadMatrix.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_MAINFORM_LOADPREVIOUSLYSAVEDMATRIX;
        this.btnSaveMatrix.Caption = Resources.UI_MAINFORM_SAVEMATRIX;
        (btnSaveMatrix.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_MAINFORM_SAVEMATRIXSETTINGS;
        this.btnSaveStatistic.Caption = Resources.UI_MAINFORM_EXPORTSTATISTICS;
        (btnSaveStatistic.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_MAINFORM_EXPORTSTATISTICS;
        this.btnStartMatch.Caption = Resources.UI_MAINFORM_RUNMATCH;
        this.btnSaveCleanedData.Caption = Resources.UI_MAINFORM_EXPORTCLEANEDDATA;
        (btnSaveCleanedData.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_MAINFORM_EXPORTCLEANSEDTOASEPARATE;

        this.barButtonJson.Caption = Resources.UI_DATASOURCE_JSON;

        this.qbtnAbout.Caption = Resources.UI_MAINFORM_ABOUT;
        this.qbtnHelp.Caption = Resources.UI_HELP;
        this.barButtonNewProject.Caption = Resources.UI_MAINFORM_NEWPROJECT;
        this.barButtonLoadProject.Caption = Resources.UI_UCNEWPROJECTFORM_OPENPROJECT;
        this.barButtonSaveProjectAs.Caption = Resources.UI_MAINFORM_SAVEPROJECTAS;
        this.barButtonSaveProject.Caption = Resources.DIALOG_PROJECT_SAVE_CAPTION;
        this.barButtonHelp.Caption = Resources.UI_HELP;
        this.barButtonAbout.Caption = Resources.UI_MAINFORM_ABOUT;
        this.barButtonVideoTutorial.Caption = Resources.UI_MAINFORM_VIDEOTUTORIALS;
        this.barButtonOnlineDemo.Caption = Resources.UI_MAINFORM_SCHEDULEONLINEDEMO;
        this.barButtonAbout_Right.Caption = Resources.UI_MAINFORM_ABOUT;
        this.barButtonRegistration.Caption = Resources.UI_REGISTRATION;
        this.btnEditNormalization.Caption = Resources.UI_EDITDICTIONARYFORM_LIBRARYMANAGER;
        this.barMatchingVideo.Caption = Resources.UI_MAINFORM_MATCHINGVIDEOTUTORIAL;
        this.mnuModulesBtnDataExport.Caption = Resources.UI_EXPORT;
        this.barButtonClean.Caption = Resources.UI_CLEAN;
        this.btnRunStatistics.Caption = Resources.UI_UCMAINCLEANNEWFORM_REFRESHSTATISTICS;
        (btnRunStatistics.SuperTip.Items[0] as ToolTipTitleItem).Text = Resources.UI_MAINFORM_THISWILLREFRESHALLSTATISTICS;
        this.qbtnExpandRibbon.Caption = Resources.UI_MAINFORM_MAXIMISERIBBON;
        this.barButtonMinimizeRibbon.Caption = Resources.UI_MAINFORM_MINIMISERIBBON;
        this.qbtnProject.Caption = Resources.UI_MAINFORM_PROJECT;
        this.qbtnCleanSettings.Caption = Resources.UI_MAINFORM_CLEANSETTINGS;
        this.qbtnMatchConfiguration.Caption = Resources.UI_MAINFORM_MATCHCONFIGURATION;
        this.qbtnMatchResult.Caption = Resources.UI_MAINFORM_MATCHRESULT;
        this.qbtnMatchReport.Caption = Resources.UI_MAINFORM_MATCHREPORT;


        this.barReportDesigner.Caption = Resources.UI_MAINFORM_EDITREPORT;
        this.rpProject.Text = Resources.UI_MAINFORM_PROJECT2;
        this.ribbonPageGroup8.Text = Resources.UI_MAINFORM_PROJECTMANAGEMENT;
        this.rpClean.Text = Resources.UI_CLEAN;
        this.ribbonPageGroup4.Text = Resources.UI_MAINFORM_FILESETTINGS;
        this.ribbonPageGroup2.Text = Resources.UI_MAINFORM_OPTIONSVIEW;

        this.rpMatch.Text = Resources.UI_MATCH;

        this.ribbonPageGroup10.Text = Resources.UI_MAINFORM_KNOWLEDGEBASE;
        this.rpgMatchSwitches.Text = Resources.UI_HELP;
        this.rpReport.Text = Resources.UI_MAINFORM_REPORTOPTIONS;

        this.rpHelp.Text = Resources.UI_HELP;
        this.ribbonPageGroup9.Text = Resources.UI_HELP;
        this.accMatch.Text = Resources.UI_MATCH;
        this.acitMatchSettings.Text = Resources.UI_CONFIGURATION;
        this.acitMatchResult.Text = Resources.UI_RESULT;
        this.acitMatchReport.Text = Resources.UI_REPORT;
        this.accData.Text = Resources.UI_DATA;
        this.acitProject.Text = Resources.UI_MAINFORM_PROJECT;
        this.acitImport.Text = Resources.UI_IMPORT;
        this.acitExport.Text = Resources.UI_EXPORT;
        this.accClean.Text = Resources.UI_CLEAN;
        this.accAuditLogs.Text = Resources.UI_AUDITLOG;

        this.barButtonItem1.Caption = Resources.UI_MAINFORM_EXPORTSTATISTIC;
        this.ribbonPageGroup7.Text = Resources.UI_OPERATION;
        this.ribbonPageGroup11.Text = Resources.UI_NAVIGATION;
        this.barSubItem6.Caption = Resources.UI_MODULES;
        this.barSubItem7.Caption = Resources.UI_MODULES;
        this.barSubItem8.Caption = Resources.UI_MODULES;
        this.ribbonPageGroup12.Text = Resources.UI_NAVIGATION;
        this.Text = Resources.UI_CAM_ENTERPRISE_EDITION;
        this.barButtonReportOpen.Caption = Resources.CAPTION_OPEN_REPORT;

        this.acitEntityResolutionConfiguration.Text = Resources.UI_CONFIGURATION;
        this.acitEntityResolutionResult.Text = Resources.UI_RESULT;

        rbCleanSettings.Text = Resources.UI_SETTINGS;
        ribbonPageGroup14.Text = Resources.UI_SETTINGS;
        accVerification.Text = Resources.UI_MAINFORM_VERIFICATION;
        acitAddressVerification.Text = Resources.UI_CONFIGURATION;
        acitVerificationReport.Text = Resources.UI_REPORT;
        btnOpenErrorFolder.Caption = Resources.UI_OPENERRORLOG;
        btnOpenConfiguration.Caption = Resources.UI_SETTINGS2;
        btnProperCaseSettings.Caption = Resources.UI_MAINFORM_PROPERCASESETTINGS;
        btnCheckUpdate.Caption = Resources.UI_CAPTION_CHECKUPDATE;

        pgErSetting.Text = Resources.UI_SETTINGS;
        barBtnLoadErConfiguration.Caption = Resources.UI_LOAD_MAPPING;
        barBtnSaveErConfiguration.Caption = Resources.UI_SAVE_MAPPING;
        barBtnMappingConfiguration.Caption = Resources.UI_CAPTION_MAPPINGCONFIGURATION;
        btnStatisticPatterns.Caption = Resources.CAPTION_PATTERNMANAGER;

        barToggleSwitchMatchTableAutoFiler.Caption = Resources.UI_UCMATCH_AUTOFILTERROW;
        barToggleSwitchMatchResultSystemFields.Caption = Resources.UI_UCMATCH_SYSTEMFIELDS;
        barToggleSwitchMatchAiTableAutoFiler.Caption = Resources.UI_UCMATCH_AUTOFILTERROW;
        barToggleSwitchMatchAiResultSystemFields.Caption = Resources.UI_UCMATCH_SYSTEMFIELDS;

        btnSaveStatistic.Caption = Resources.UI_MAINFORM_EXPORTSTATISTICS;
    }

    private void SetControlTextAndAvailability()
    {
        switch (Program.CurrentProgramVersion)
        {
            case ProgramType.CamFree:
                Text = Resources.UI_CAM_FREE_EDITION;
                barButtonRegistration.Visibility = BarItemVisibility.Never;
                break;
            case ProgramType.CamLte:
                Text = Resources.UI_CAM_LITE_EDITION;
                break;
            case ProgramType.CamBiz:
                Text = Resources.UI_CAM_BUSINESS_EDITION;
                break;
            case ProgramType.CamEntLite:
                Text = Resources.UI_CAM_ENTERPRISE_LITE_EDITION + (_licenseService.IsDemo ? " Demo" : "");
                break;
            case ProgramType.CamEnt:
                Text = Resources.UI_CAM_ENTERPRISE_EDITION + (_licenseService.IsDemo ? " Demo" : "");
                break;
            case ProgramType.CamEntAd:
                Text = Resources.UI_CAM_ENTERPRISE_EDITION2 + (_licenseService.IsDemo ? " Demo" : "");
                break;
            default:
                throw new WinPureArgumentException($"Program version {Program.CurrentProgramVersion} not defined");
        }
        ribbonGroupReportDesigner.Enabled = barReportDesigner.Enabled = !_licenseService.IsDemo && ProgramTypeHelper.EnterpriseTypes.Contains(Program.CurrentProgramVersion);
    }

    private void _licenseService_LicenseLoaded()
    {
        SetControlTextAndAvailability();
    }

    private void _service_AddressVerificationReady(string tableName, bool activateReport)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { _service_AddressVerificationReady(tableName, activateReport); }));
            return;
        }

        if (!activateReport) return;

        _frmProgress?.CloseFormAndStopTimer();

        acitVerificationReport_Click(null, null);
    }

    private void _service_FiltrateData(string columnName, FiltrateField filter)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { _service_FiltrateData(columnName, filter); }));
            return;
        }

        navFraimMain.SelectedPage = navPageData;
        mainRibbon.SelectedPage = rpProject;

        rpClean.Visible = false;
        rpMatch.Visible = false;
        rpReport.Visible = false;
        rpEr.Visible = false;

        menuLastProject.OptionsNavPane.NavPaneState = NavPaneState.Expanded;

        ucMainData.ShowSubPanel(MainDataType.Project);

        // Update accordion highlight
        var element = FindAccordionElementByTag(mnuMainAccordion.Elements, "Data.Project");
        if (element != null)
        {
            HighlightAccordionElement(element);
        }
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        _logger.Fatal("APPLICATION GLOBAL ERROR", e.ExceptionObject);
    }

    private void frmMain_Load(object sender, EventArgs e)
    {
        InitializeLatestProjectsTileHover();

        if (_configurationService.Configuration.CheckForUpdates)
        {
            var newVersion = UpdateHelper.RequireUpdate(Program.CurrentProgramVersion, _logger);
            if (!string.IsNullOrEmpty(newVersion))
            {
                if (MessageBox.Show(string.Format(Resources.MESSAGE_DOWNLOADNEWVERSION, newVersion),
                        Resources.MESSAGE_QUESTION_CAPTION,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    UpdateHelper.OpenDownloadPage(Program.CurrentProgramVersion);
                }
            }
        }
        //CollapseAllAccordionGroupsExceptFirst();
        ShowStartupScreen();
    }

    private void _service_BeforeProjectLoad()
    {
        SetCleanButtonsEnabled(false);
    }

    private void _service_OnStatisticUpdated(object ds, int rowCount, bool statisticReady)
    {
        btnSaveStatistic.Enabled = statisticReady;
    }

    private void _service_OnTableDelete(ImportedDataInfo obj)
    {
        if (!_service.IsAnyTable)
        {
            SetCleanButtonsEnabled(false);
        }
    }

    private void _service_OnMatchResultReady(bool matchSuccess, bool activateResult, MatchResultOperation operation)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { _service_OnMatchResultReady(matchSuccess, activateResult, operation); }));
            return;
        }

        if (!matchSuccess || !activateResult) return;

        _frmProgress?.CloseFormAndStopTimer();

        acitMatchResult_Click(null, null);
    }

    private void _service_OnProjectNameChanged(string obj)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { _service_OnProjectNameChanged(obj); }));
            return;
        }
        Text = string.Format(Resources.CAPTION_PROJECT, obj);
    }

    private void _service_OnEntityResolutionReady()
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(_service_OnEntityResolutionReady));
            return;
        }

        _frmProgress?.CloseFormAndStopTimer();

        acitEntityResolutionResult_Click(null, null);
    }

    private void UcMainMatch_OnNavigateToFullReport()
    {
        acitMatchReport_Click(null, null);
    }

    private void InformationDisplay(string message, string caption, MessagesType mType, Exception ex)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { InformationDisplay(message, caption, mType, ex); }));
            return;
        }

        var icon = MessageBoxIcon.Error;

        if (string.IsNullOrEmpty(caption))
        {
            switch (mType)
            {
                case MessagesType.Information:
                    caption = Resources.MESSAGE_MESSAGEDIALOG_INFORMATION_CAPTION;
                    icon = MessageBoxIcon.Information;
                    break;
                case MessagesType.Warning:
                    caption = Resources.MESSAGE_MESSAGEDIALOG_WARNING_CAPTION;
                    icon = MessageBoxIcon.Warning;
                    break;
                case MessagesType.Error:
                    caption = Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION;
                    icon = MessageBoxIcon.Error;
                    break;
                default:
                    caption = Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION;
                    icon = MessageBoxIcon.Error;
                    break;
            }
        }

        if (ex == null)
        {
            _logger.Warning(message);
        }
        else
        {
            _logger.Error(message);
        }

        XtraMessageBox.Show($"{message}  {ex?.Message}", caption, MessageBoxButtons.OK, icon);
    }

    private void _service_OnAddNewData(string tableName)
    {
        SetCleanButtonsEnabled(true);
    }

    private void _service_OnProgressUpdate(string text, int value)
    {
        _frmProgress?.UpdateText(text, value);
    }

    private void ProgressFormShow(string caption, Task task, bool showPb, CancellationTokenSource cts)
    {
        if (_frmProgress == null)
        {
            _frmProgress = new LoadingMaskForm
            {
                Owner = this,
                StartPosition = FormStartPosition.Manual
            };
            _frmProgress.Location = new Point(Location.X + 10, Location.Y + Size.Height - _frmProgress.Height - 10);
            _frmProgress.Width = Width - 20;
            _frmProgress.Closed += FrmProgress_Closed;
            _frmProgress.ShowLoadingMask(caption, task, showPb, cts);
        }
    }

    private void ShowStartupScreen(bool showAnyway = false)
    {
        if (_configurationService.Configuration.ShowStartScreen || showAnyway)
        {
            var startForm = WinPureUiDependencyResolver.Resolve<frmStartupOptionsNew>();

            startForm.ShowDialog();
            if (startForm.StartOption != null)
            {
                if (_configurationService.Configuration.ShowStartScreen != startForm.StartOption.ShowOnStartup)
                {
                    _configurationService.Configuration.ShowStartScreen = startForm.StartOption.ShowOnStartup;
                    _configurationService.SaveConfiguration();
                }

                switch (startForm.StartOption.ActionType)
                {
                    case StartupAction.OpenProject:
                        UsMainData_OnOpenProject();
                        break;
                    case StartupAction.ImportData:
                        SelectAccordionElement(acitImport);
                        break;
                    case StartupAction.NewProject:
                        barButtonNewProject_ItemClick(null, null);
                        break;
                    case StartupAction.DemoProject:
                        var projName = Path.Combine(_configurationService.Configuration.SampleFolderPath, _configurationService.Configuration.SampleProjectName);
                        _projectService.LoadProjectAsync(projName);
                        break;
                    case StartupAction.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    private void FrmProgress_Closed(object sender, EventArgs e)
    {
        _frmProgress = null;
    }

    private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
    {
        try
        {
            var path = Application.StartupPath;
            var dir = new DirectoryInfo(path);

            foreach (var file in dir.EnumerateFiles("*.tmp"))
            {
                file.Delete();
            }
        }
        catch
        {
            // ignore
        }

        // Unsubscribe from theme change event
        UserLookAndFeel.Default.StyleChanged -= OnThemeChanged;
        // Unsubscribe to avoid dangling references
        ucMainMatch.navFrameMatching.SelectedPageChanged -= ucMainMatch_SelectedPageChanged;
        ucEntityResolution.navFrameEntityResolution.SelectedPageChanged -= ucEntityResolution_SelectedPageChanged;

        if (_service.IsAnyTable)
        {
            if (!ProjectSaveAndContinue(_service.ProjectName))
            {
                e.Cancel = true;
                return;
            }

        }
        else
        {
            if (XtraMessageBox.Show(Resources.MESSAGE_MAINFORM_QUIT_CONFIRMATION, Resources.MESSAGE_QUESTION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
        }
        ucMainData.CloseCurrentData(false);
        ucMainCleanNew.ClearTableData();
        ucVerification.CloseCurrentData(false);
        ucEntityResolution.CloseCurrentData(false);
        ucAuditLogs.CloseCurrentData();
        _service.Dispose();
    }

    private void CreateNewProject()
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(CreateNewProject));
            return;
        }

        if (_service.IsAnyTable)
        {
            if (!ProjectSaveAndContinue(_service.ProjectName)) return;
        }

        var frmNewProject = WinPureUiDependencyResolver.Resolve<frmCreateNewProject>();
        var notAllowd = @"\/:*?""<>|[]";
        if (frmNewProject.Show(Resources.UI_ENTERPROJECTNAMEFORM_CREATEPROJECT, Resources.UI_ENTERPROJECTNAMEFORM_NAMENEWPROJECT, notAllowd.ToCharArray()) == DialogResult.OK)
        {
            if (_projectService.CloseCurrentAndCreateNewProject())
            {
                _projectService.SaveProjectAsync(frmNewProject.ProjectName, false, frmNewProject.ProjectPath);
            }
        }
    }

    private void UsMainData_OnOpenProject()
    {
        if (_service.IsAnyTable)
        {
            if (!ProjectSaveAndContinue(_service.ProjectName)) return;
        }
        var ov = new OpenFileDialog
        {
            Filter = Resources.DIALOG_PROJECT_FORMAT,
            Multiselect = false,
            CheckFileExists = true,
        };
        if (ov.ShowDialog() == DialogResult.OK)
        {
            _projectService.LoadProjectAsync(ov.FileName);
        }
    }

    private void UcMainData_OnOpenSettings()
    {
        var frmConfiguration = WinPureUiDependencyResolver.Resolve<frmConfiguration>();
        frmConfiguration.ShowDialog();
    }

    private void CleanDisplayOptions_CheckedChanged(object sender, ItemClickEventArgs e)
    {
        var clSett = new DisplaySettings
        {
            ShowCleanPreview = cbShowPreview.Checked,
            ShowCleanStatistic = cbShowStatistic.Checked,
        };

        ucMainCleanNew.SetDisplaySettings(clSett);
    }

    private void qbtnOpenProject_ItemClick(object sender, ItemClickEventArgs e)
    {
        UsMainData_OnOpenProject();
    }

    private void SaveProject(bool saveAs)
    {
        ProjectHelper.SaveProject(saveAs);
    }

    private void qbtnSaveProject_ItemClick(object sender, ItemClickEventArgs e)
    {
        ProjectHelper.SaveProject(false);
    }

    private void qbtnSaveAsProject_ItemClick(object sender, ItemClickEventArgs e)
    {
        ProjectHelper.SaveProject(true);
    }

    private void qbtnImportData_ItemClick(object sender, ItemClickEventArgs e)
    {
        acitImport_Click(null, null);
    }

    private void qbtnExportData_ItemClick(object sender, ItemClickEventArgs e)
    {
        acitExport_Click(null, null);
    }

    private void btnRunClean_ItemClick(object sender, ItemClickEventArgs e)
    {
        _service.CleanDataAsync();
    }

    private void btnCleanSettings_ItemClick(object sender, ItemClickEventArgs e)
    {
        var tbl = _service.CurrentTable;
        _service.ClearCleanSettings(tbl);
        ucMainCleanNew.SetCleanSettings(_service.GetDataTableCleanSettings(tbl));
    }

    private void btnSaveStatistic_ItemClick(object sender, ItemClickEventArgs e)
    {
        ucMainCleanNew.ExportStatistic();
    }

    private void btnExportUniqueValues_ItemClick(object sender, ItemClickEventArgs e)
    {
        ucMainCleanNew.ExportUniqueValues();
    }

    private void btnExportFullRecords_ItemClick(object sender, ItemClickEventArgs e)
    {
        ucMainCleanNew.ExportUniqueValues(true);
    }

    private void SetCleanButtonsEnabled(bool state)
    {
        btnCleanSettings.Enabled = state;
        btnRunClean.Enabled = state;
        btnRunStatistics.Enabled = state;
        btnSaveMatrix.Enabled = state;
        btnLoadMatrix.Enabled = state;
        btnSaveStatistic.Enabled = state;
        btnSaveUnqueValue.Enabled = state;
        btnSaveCleanedData.Enabled = state;// && !WinPureUiDependencyResolver.Resolve<ILicenseValidator>().IsDemo;
    }

    private void btnSaveMatrix_ItemClick(object sender, ItemClickEventArgs e)
    {
        var dlgSaveCsvFile = new SaveFileDialog { Title = Resources.DIALOG_EXPORT_MATRIX_CAPTION };
        var tblInfo = _service.GetTableInfo(_service.CurrentTable);
        if (tblInfo == null)
        {
            InformationDisplay(Resources.EXCEPTION_NO_MATRIX_FOR_EXPORT, "", MessagesType.Information, null);
            return;
        }
        dlgSaveCsvFile.FileName = Path.GetFileNameWithoutExtension(tblInfo.FileName) + "_Matrix.json";
        dlgSaveCsvFile.AddExtension = true;

        dlgSaveCsvFile.Filter = Resources.DIALOG_JSONFILE_FORMAT;

        if (dlgSaveCsvFile.ShowDialog() == DialogResult.OK)
        {
            _service.SaveCleanMatrix(dlgSaveCsvFile.FileName);
        }
    }

    private void btnLoadMatrix_ItemClick(object sender, ItemClickEventArgs e)
    {
        var dlgSelectTextFile = new OpenFileDialog
        {
            Title = Resources.DIALOG_IMPORT_MATRIX_CAPTION,
            FileName = "",
            CheckFileExists = true,
            Filter = Resources.DIALOG_JSONFILE_FORMAT
        };

        if (dlgSelectTextFile.ShowDialog() != DialogResult.OK) return;
        try
        {
            _service.LoadCleanMatrix(dlgSelectTextFile.FileName);
            var dt = _service.GetDataTableCleanSettings(_service.CurrentTable);
            ucMainCleanNew.SetCleanSettings(dt);
        }
        catch (Exception ex)
        {
            InformationDisplay(Resources.EXCEPTION_MATRIX_CANNOT_BE_LOADED, "", MessagesType.Error, ex);
        }
    }

    private void btnStartMatch_ItemClick(object sender, ItemClickEventArgs e)
    {
        ucMainMatch.StartMatching();
    }

    private void qbtnAbout_ItemClick(object sender, ItemClickEventArgs e)
    {
        var frmDct = WinPureUiDependencyResolver.Resolve<frmAbout>();
        frmDct.ShowDialog();
    }

    private void OpenManual(object sender, ItemClickEventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.Cam);
    }

    private void barButtonNewProject_ItemClick(object sender, ItemClickEventArgs e)
    {
        CreateNewProject();
    }

    private void btnEditNormalization_ItemClick(object sender, ItemClickEventArgs e)
    {
        var frmDct = WinPureUiDependencyResolver.Resolve<frmEditDictionary>();
        frmDct.ShowDialog();
        ucMainMatch.UpdateDictionaries();
    }

    private void barButtonVideoTutorial_ItemClick(object sender, ItemClickEventArgs e)
    {
        UserManualHelper.OpenGetStartedHelp(Program.CurrentProgramVersion);
    }

    private void barButtonOnlineDemo_ItemClick(object sender, ItemClickEventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.Schedule);
    }

    private void btnRunStatistics_ItemClick(object sender, ItemClickEventArgs e)
    {
        _service.UpdateTableStatistic(_service.CurrentTable);
    }

    private void qbtnExpandRibbon_ItemClick(object sender, ItemClickEventArgs e)
    {
        mainRibbon.Minimized = false;
    }

    private void barButtonMinimizeRibbon_ItemClick(object sender, ItemClickEventArgs e)
    {
        mainRibbon.Minimized = true;
    }

    private void qbtnProject_ItemClick(object sender, ItemClickEventArgs e)
    {
        acitProject_Click(null, null);
    }

    private void qbtnCleanSettings_ItemClick(object sender, ItemClickEventArgs e)
    {
        navFraimMain.SelectedPage = navPageClean;
        mainRibbon.SelectedPage = rpClean;
        accClean_Click(null, null);
    }

    private void qbtnMatchConfiguration_ItemClick(object sender, ItemClickEventArgs e)
    {
        navFraimMain.SelectedPage = navPageMatch;
        mainRibbon.SelectedPage = rpMatch;
        acitMatchSettings_Click(null, null);
    }

    private void qbtnMatchResult_ItemClick(object sender, ItemClickEventArgs e)
    {
        navFraimMain.SelectedPage = navPageMatch;
        mainRibbon.SelectedPage = rpMatch;
        acitMatchResult_Click(null, null);
    }

    private void qbtnMatchReport_ItemClick(object sender, ItemClickEventArgs e)
    {
        navFraimMain.SelectedPage = navPageMatch;
        mainRibbon.SelectedPage = rpMatch;
        acitMatchReport_Click(null, null);
    }

    private void barReportDesigner_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (mnuMainAccordion.ActiveGroup == accMatch)
        {
            ucMainMatch.Report?.ShowDesigner();
        }
        else if (mnuMainAccordion.ActiveGroup == accVerification)
        {
            ucVerification.Report?.ShowDesigner();
        }
    }

    private void barButtonRegistration_ItemClick(object sender, ItemClickEventArgs e)
    {
        var frm = WinPureUiDependencyResolver.Resolve<frmRegister>();
        if (frm.ShowDialog() == DialogResult.OK)
        {
            if (frm.ShouldAppClose)
            {
                Close();
                return;
            }
            SetCleanButtonsEnabled(_service.IsAnyTable);
            ribbonGroupReportDesigner.Enabled = barReportDesigner.Enabled = (_licenseService.IsDemo && (Program.CurrentProgramVersion == ProgramType.CamEnt || Program.CurrentProgramVersion == ProgramType.CamEntAd));
        }
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

            var srv = WinPureUiDependencyResolver.Resolve<IImportExportService>();
            srv.Export(sourceType);
        }
        catch (WinPureBaseException ex)
        {
            _logger.Debug("IMPORT/EXPORT ERROR", ex);
            InformationDisplay(ex.Message, "", MessagesType.Error, null);
        }
        catch (Exception ex)
        {
            _logger.Debug("IMPORT/EXPORT ERROR", ex);
            InformationDisplay(Resources.EXCEPTION_DATA_CANNOT_BE_EXPORTED, "", MessagesType.Error, ex);
        }
    }

    private void barMatchingVideo_ItemClick(object sender, ItemClickEventArgs e)
    {
        Process.Start("https://www.winpure.com/support/matching-video-tutorial.html");
    }

    private void barButtonReportOpen_ItemClick(object sender, ItemClickEventArgs e)
    {
        {
            //reportDesigner1.OpenReport(new XtraReport());

            using var dialog = new OpenFileDialog
            {
                Title = Resources.CAPTION_OPEN_REPORT,
                Filter = "DevExpress Report Layout (*.repx)|*.repx|All files (*.*)|*.*",
                Multiselect = false,
                CheckFileExists = true,
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                var report = new DevExpress.XtraReports.UI.XtraReport();
                report.LoadLayout(dialog.FileName);
                report.CreateDocument();

                documentViewerRibbonController1.DocumentViewer = null;
                documentViewerRibbonController1.DocumentViewer = ucMainMatch.ReportViewer;

                ucMainMatch.ReportViewer.DocumentSource = report;

                rpReport.Visible = true;
                mainRibbon.SelectedPage = rpReport;
                navFraimMain.SelectedPage = navPageMatch;
            }
            catch (Exception ex)
            {
                InformationDisplay($"Failed to open report layout: {dialog.FileName}", Resources.CAPTION_OPEN_REPORT, MessagesType.Error, ex);
            }
        }
    }

    private void btnOpenErrorFolder_ItemClick(object sender, ItemClickEventArgs e)
    {
        Process.Start("explorer.exe", Path.GetDirectoryName(_configurationService.Configuration.LogPath));
    }

    private void btnOpenConfiguration_ItemClick(object sender, ItemClickEventArgs e)
    {
        var frmConfiguration = WinPureUiDependencyResolver.Resolve<frmConfiguration>();
        frmConfiguration.ShowDialog();
    }

    private void barFeedback_ItemClick(object sender, ItemClickEventArgs e)
    {
        Process.Start("https://winpure.com/feedback-about-cam/");
    }

    private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
    {
        var frmConfiguration = WinPureUiDependencyResolver.Resolve<frmConfiguration>();
        frmConfiguration.ShowDialog();
    }

    private void btnProperCaseSettings_ItemClick(object sender, ItemClickEventArgs e)
    {
        var frmProperCaseSettings = WinPureUiDependencyResolver.Resolve<frmProperCaseConfiguration>();
        frmProperCaseSettings.ShowDialog();
    }

    private void btnCheckUpdate_ItemClick(object sender, ItemClickEventArgs e)
    {
        UpdateHelper.CheckForUpdate(Program.CurrentProgramVersion, _logger);
    }

    private void barBtnSaveErConfiguration_ItemClick(object sender, ItemClickEventArgs e)
    {
        var dlgSaveCsvFile = new SaveFileDialog { Title = Resources.DIALOG_EXPORT_ERCONFIGURATION_CAPTION };
        var tblInfo = _service.GetTableInfo(_service.CurrentTable);
        if (tblInfo == null)
        {
            InformationDisplay(Resources.EXCEPTION_NO_ERCONFIGURATION_FOR_EXPORT, "", MessagesType.Information, null);
            return;
        }
        dlgSaveCsvFile.FileName = Path.GetFileNameWithoutExtension(tblInfo.FileName) + "_ErConfiguration.json";
        dlgSaveCsvFile.AddExtension = true;

        dlgSaveCsvFile.Filter = Resources.DIALOG_JSONFILE_FORMAT;

        if (dlgSaveCsvFile.ShowDialog() == DialogResult.OK)
        {
            _service.SaveErConfiguration(dlgSaveCsvFile.FileName);
        }
    }

    private void barBtnLoadErConfiguration_ItemClick(object sender, ItemClickEventArgs e)
    {
        var dlgSelectTextFile = new OpenFileDialog
        {
            Title = Resources.DIALOG_IMPORT_ERCONFIGURATION_CAPTION,
            FileName = "",
            CheckFileExists = true,
            Filter = Resources.DIALOG_JSONFILE_FORMAT
        };

        if (dlgSelectTextFile.ShowDialog() != DialogResult.OK) return;
        try
        {
            _service.LoadErConfiguration(dlgSelectTextFile.FileName);
            ucEntityResolution.RefreshMapping(_service.CurrentTable);
        }
        catch (Exception ex)
        {
            InformationDisplay(Resources.EXCEPTION_ERCONFIGURATION_CANNOT_BE_LOADED, "", MessagesType.Error, ex);
        }
    }

    private void barBtnMappingConfiguration_ItemClick(object sender, ItemClickEventArgs e)
    {
        var frmMapping = WinPureUiDependencyResolver.Resolve<frmErDefaultMapping>();
        frmMapping.ShowDialog();
    }

    private void btnStatisticPatterns_ItemClick(object sender, ItemClickEventArgs e)
    {
        var frmPatternConfiguration = WinPureUiDependencyResolver.Resolve<frmPatternConfiguration>();
        frmPatternConfiguration.ShowDialog();
        ucMainCleanNew.RefreshSupportEditor();
    }

    private void barButtonItem2_ItemClick_1(object sender, ItemClickEventArgs e)
    {
        UserLookAndFeel.Default.SetSkinStyle(SkinStyle.WXICompact);
    }

    private void btnCleansingAi_ItemClick(object sender, ItemClickEventArgs e)
    {
        var frmCleansingAiConfiguration = WinPureUiDependencyResolver.Resolve<frmCleansingAiConfiguration>();
        frmCleansingAiConfiguration.ShowDialog();
    }

    private void acitProject_Click(object sender, EventArgs e)
    {
        ucMainData.ShowSubPanel(MainDataType.Project);
    }

    private void acitImport_Click(object sender, EventArgs e)
    {
        ucMainData.ShowSubPanel(MainDataType.Import);
    }

    private void acitExport_Click(object sender, EventArgs e)
    {
        ucMainData.ShowSubPanel(MainDataType.Export);
    }

    private void acitMatchSettings_Click(object sender, EventArgs e)
    {
        ucMainMatch.ShowSubPanel(MatchDataMenuItem.Settings);
        mainRibbon.SelectedPage = rpMatch;
        documentViewerRibbonController1.DocumentViewer = null;
    }

    private void acitMatchResult_Click(object sender, EventArgs e)
    {
        ucMainMatch.ShowSubPanel(MatchDataMenuItem.MatchResult);
        mainRibbon.SelectedPage = rpMatch;
        documentViewerRibbonController1.DocumentViewer = null;
    }

    private void acitMatchReport_Click(object sender, EventArgs e)
    {
        ucMainMatch.ShowSubPanel(MatchDataMenuItem.Report);
        documentViewerRibbonController1.DocumentViewer = ucMainMatch.ReportViewer;
        rpReport.Visible = true;
        mainRibbon.SelectedPage = rpReport;
    }

    private void acitEntityResolutionConfiguration_Click(object sender, EventArgs e)
    {
        ucEntityResolution.ShowSubPanel(EntityResolutionViewType.Settings);
    }

    private void acitEntityResolutionResult_Click(object sender, EventArgs e)
    {
        ucEntityResolution.ShowSubPanel(EntityResolutionViewType.Result);
    }

    private void acitAddressVerification_Click(object sender, EventArgs e)
    {
        ucVerification.ShowSubPanel(AddressVerificationViewType.Settings);
        documentViewerRibbonController1.DocumentViewer = null;
        rpReport.Visible = false;
    }

    private void acitVerificationReport_Click(object sender, EventArgs e)
    {
        ucVerification.ShowSubPanel(AddressVerificationViewType.Report);
        documentViewerRibbonController1.DocumentViewer = ucVerification.ReportViewer;
    }

    private void acitAutomationConfiguration_Click(object sender, EventArgs e)
    {
        ucAutomation.ShowSubPanel(AutomationDataType.Configuration);
    }

    private void acitAutomationLog_Click(object sender, EventArgs e)
    {
        ucAutomation.ShowSubPanel(AutomationDataType.Log);
    }

    private void accClean_Click(object sender, EventArgs e)
    {
        navFraimMain.SelectedPage = navPageClean;
        rpMatch.Visible = false;
        rpReport.Visible = false;
        rpClean.Visible = true;
        rpEr.Visible = false;
        mainRibbon.SelectedPage = rpClean;
    }

    private void accAuditLogs_Click(object sender, EventArgs e)
    {
        navFraimMain.SelectedPage = navPageAuditLogs;
        mainRibbon.SelectedPage = rpProject;
        rpClean.Visible = false;
        rpMatch.Visible = false;
        rpReport.Visible = false;
        rpEr.Visible = false;
        if (_licenseService.IsDemo)
        {
            if (XtraMessageBox.Show(Resources.MESSAGE_AUDITLOG_NOT_AVAILABLE_ON_DEMO, Resources.CAPTION_DEMO_LIMITATION, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Process.Start("https://winpure.com/request-custom-quote/");
            }
        }
    }

    private void accordionControl1_ElementClick(object sender, DevExpress.XtraBars.Navigation.ElementClickEventArgs e)
    {
        SelectAccordionElement(e?.Element);

    }

    private void SelectAccordionElement(DevExpress.XtraBars.Navigation.AccordionControlElement element)
    {
        if (element == null || element.Tag == null)
        {
            return;
        }

        if (element.Style != DevExpress.XtraBars.Navigation.ElementStyle.Item)
        {
            return;
        }


        // ensure parent group is visible (helps if starting collapsed)
        if (accData != null && accData.Style == DevExpress.XtraBars.Navigation.ElementStyle.Group)
        {
            accData.Expanded = true;
            mnuMainAccordion.ActiveGroup = accData;
        }

        var tag = element.Tag as string ?? element.Tag.ToString();
        if (string.IsNullOrWhiteSpace(tag))
        {
            return;
        }

        HandleAccordionSelectionTag(tag);
        HighlightAccordionElement(element);

    }

    private void HandleAccordionSelectionTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            return;
        }

        // Normalize tag for consistent comparisons
        var parts = tag.Split('.');
        var module = parts.Length > 0 ? parts[0] : null;
        var item = parts.Length > 1 ? parts[1] : null;
        mainRibbon.Minimized = false;

        switch (module)
        {
            case "Data":
                navFraimMain.SelectedPage = navPageData;
                rpClean.Visible = false;
                rpMatch.Visible = false;
                rpReport.Visible = false;
                rpEr.Visible = false;

                if (item == "Project")
                {
                    ucMainData.ShowSubPanel(MainDataType.Project);
                    rpProject.Visible = false;
                    mainRibbon.Minimized = true; // Minimize ribbon for Project as it has no actions
                }
                else if (item == "Import")
                {
                    ucMainData.ShowSubPanel(MainDataType.Import);
                    rpProject.Visible = true;
                    mainRibbon.SelectedPage = rpProject;

                }
                else if (item == "Export")
                {
                    ucMainData.ShowSubPanel(MainDataType.Export);
                    rpProject.Visible = true;
                    mainRibbon.SelectedPage = rpProject;
                }
                break;

            case "Clean":
                navFraimMain.SelectedPage = navPageClean;
                rpMatch.Visible = false;
                rpReport.Visible = false;
                rpClean.Visible = true;
                rpEr.Visible = false;
                rpProject.Visible = true; // Show rpProject for Clean module
                mainRibbon.SelectedPage = rpClean;
                break;

            case "Match":
                navFraimMain.SelectedPage = navPageMatch;
                rpClean.Visible = false;
                rpMatch.Visible = false;
                rpReport.Visible = false;
                rpEr.Visible = false;
                rpgMatchSwitches.Visible = false;
                rpProject.Visible = true; // Show rpProject for Match module

                if (item == "Config")
                {
                    ucMainMatch.ShowSubPanel(MatchDataMenuItem.Settings);
                    documentViewerRibbonController1.DocumentViewer = null;
                    mainRibbon.Minimized = true;
                }
                else if (item == "Result")
                {
                    rpgMatchSwitches.Visible = true;
                    rpMatch.Visible = true;
                    ucMainMatch.ShowSubPanel(MatchDataMenuItem.MatchResult);
                    documentViewerRibbonController1.DocumentViewer = null;
                    mainRibbon.SelectedPage = rpMatch;

                }
                else if (item == "Report")
                {
                    ucMainMatch.ShowSubPanel(MatchDataMenuItem.Report);
                    documentViewerRibbonController1.DocumentViewer = ucMainMatch.ReportViewer;
                    rpReport.Visible = true;
                    mainRibbon.SelectedPage = rpReport;
                }
                break;

            case "Verify":
                navFraimMain.SelectedPage = navPageVerification;
                rpClean.Visible = false;
                rpMatch.Visible = false;
                rpReport.Visible = false;
                rpEr.Visible = false;
                rpProject.Visible = true;
                mainRibbon.Minimized = true;

                if (item == "Settings")
                {
                    ucVerification.ShowSubPanel(AddressVerificationViewType.Settings);
                    documentViewerRibbonController1.DocumentViewer = null;
                    rpReport.Visible = false;
                }
                else if (item == "Report")
                {
                    ucVerification.ShowSubPanel(AddressVerificationViewType.Report);
                    documentViewerRibbonController1.DocumentViewer = ucVerification.ReportViewer;
                }
                break;

            case "MatchAI":
                navFraimMain.SelectedPage = navPageEntityResolution;
                rpClean.Visible = false;
                rpMatch.Visible = false;
                rpReport.Visible = false;
                rpgMatchAiSwitches.Visible = false;
                rpEr.Visible = true;
                rpProject.Visible = true;
                mainRibbon.SelectedPage = rpEr;

                if (item == "Settings")
                {
                    ucEntityResolution.ShowSubPanel(EntityResolutionViewType.Settings);
                }
                else if (item == "Result")
                {
                    rpgMatchAiSwitches.Visible = true;
                    ucEntityResolution.ShowSubPanel(EntityResolutionViewType.Result);
                }
                break;

            case "Automation":
                navFraimMain.SelectedPage = navPageAutomation;
                rpClean.Visible = false;
                rpMatch.Visible = false;
                rpReport.Visible = false;
                rpEr.Visible = false;
                rpProject.Visible = true; // Show rpProject for Automation module
                mainRibbon.Minimized = true;

                if (!ProgramTypeHelper.AutomationPrograms.Contains(Program.CurrentProgramVersion))
                {
                    if (XtraMessageBox.Show(Resources.MESSAGE_AUTOMATION_NOT_AVAILABLE_ON_EDITION, Resources.CAPTION_EDITION_LIMITATION, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Process.Start("https://winpure.com/request-custom-quote/");
                    }
                }
                else if (_licenseService.IsDemo)
                {
                    if (XtraMessageBox.Show(Resources.MESSAGE_AUTOMATION_NOT_AVAILABLE_ON_DEMO,
                            Resources.CAPTION_DEMO_LIMITATION, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Process.Start("https://winpure.com/request-custom-quote/");
                    }
                }

                if (item == "Config")
                {
                    ucAutomation.ShowSubPanel(AutomationDataType.Configuration);
                }
                else if (item == "Log")
                {
                    ucAutomation.ShowSubPanel(AutomationDataType.Log);
                }
                break;

            case "Audit":
                navFraimMain.SelectedPage = navPageAuditLogs;
                rpClean.Visible = false;
                rpMatch.Visible = false;
                rpReport.Visible = false;
                rpEr.Visible = false;
                rpProject.Visible = true; // Show rpProject for Audit module
                mainRibbon.Minimized = true;

                if (_licenseService.IsDemo)
                {
                    if (XtraMessageBox.Show(Resources.MESSAGE_AUDITLOG_NOT_AVAILABLE_ON_DEMO, Resources.CAPTION_DEMO_LIMITATION, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Process.Start("https://winpure.com/request-custom-quote/");
                    }
                }
                ucAuditLogs.LoadLogs();
                break;
        }
    }

    private Color GetThemeAwareHighlightColor()
    {
        if (_themeDetectionService.IsDarkTheme())
        {
            var baseColor = mnuMainAccordion.BackColor;
            return ControlPaint.Light(baseColor, 0.15f);
        }
        else
        {
            return Color.FromArgb(237, 238, 239);
        }
    }

    private void OnThemeChanged(object sender, EventArgs e)
    {
        // Refresh the highlight color for the currently selected element
        if (_selectedAccordionElement != null)
        {
            var highlightColor = GetThemeAwareHighlightColor();
            _selectedAccordionElement.Appearance.Normal.BackColor = highlightColor;
        }

        mnuMainAccordion.Refresh();
    }

    private DevExpress.XtraBars.Navigation.AccordionControlElement FindAccordionElementByTag(DevExpress.XtraBars.Navigation.AccordionControlElementCollection elements, string tag)
    {
        foreach (DevExpress.XtraBars.Navigation.AccordionControlElement element in elements)
        {
            // Check if this element matches
            if (element.Tag != null &&
                (element.Tag as string ?? element.Tag.ToString()) == tag &&
                element.Style == DevExpress.XtraBars.Navigation.ElementStyle.Item)
            {
                return element;
            }

            // Recursively search child elements
            if (element.Elements.Count > 0)
            {
                var found = FindAccordionElementByTag(element.Elements, tag);
                if (found != null)
                    return found;
            }
        }

        return null;
    }

    private void ucMainMatch_SelectedPageChanged(object sender, DevExpress.XtraBars.Navigation.SelectedPageChangedEventArgs e)
    {
        // Only react when the Match module is active
        if (navFraimMain.SelectedPage != navPageMatch)
        {
            return;
        }
        rpgMatchSwitches.Visible = false;
        string targetTag = null;
        if (ucMainMatch.navFrameMatching.SelectedPage == ucMainMatch.navPageMatchReport)
        {
            targetTag = "Match.Report";
            documentViewerRibbonController1.DocumentViewer = ucMainMatch.ReportViewer;
            rpReport.Visible = true;
            mainRibbon.SelectedPage = rpReport;
        }
        else if (ucMainMatch.navFrameMatching.SelectedPage == ucMainMatch.navPagematchResult)
        {
            targetTag = "Match.Result";
            documentViewerRibbonController1.DocumentViewer = null;
            rpReport.Visible = false;
            rpgMatchSwitches.Visible = true;
            mainRibbon.SelectedPage = rpMatch;
        }
        else if (ucMainMatch.navFrameMatching.SelectedPage == ucMainMatch.navPageSetting)
        {
            targetTag = "Match.Config";
            documentViewerRibbonController1.DocumentViewer = null;
            rpReport.Visible = false;
            mainRibbon.SelectedPage = rpMatch;
        }

        if (string.IsNullOrEmpty(targetTag))
        {
            return;
        }

        var element = FindAccordionElementByTag(mnuMainAccordion.Elements, targetTag);
        if (element != null)
        {
            HighlightAccordionElement(element);
        }
    }

    private void ucEntityResolution_SelectedPageChanged(object sender, DevExpress.XtraBars.Navigation.SelectedPageChangedEventArgs e)
    {
        // Only react when the MatchAI module is active
        if (navFraimMain.SelectedPage != navPageEntityResolution)
        {
            return;
        }

        string targetTag = null;
        rpgMatchAiSwitches.Visible = false;
        if (ucEntityResolution.navFrameEntityResolution.SelectedPage == ucEntityResolution.navPageResult)
        {
            rpgMatchAiSwitches.Visible = true;
            targetTag = "MatchAI.Result";
        }
        else if (ucEntityResolution.navFrameEntityResolution.SelectedPage == ucEntityResolution.navPageConfigurtion)
        {
            targetTag = "MatchAI.Settings";
        }

        var element = FindAccordionElementByTag(mnuMainAccordion.Elements, targetTag);
        if (element != null)
        {
            HighlightAccordionElement(element);
        }
    }


    private void HighlightAccordionElement(DevExpress.XtraBars.Navigation.AccordionControlElement element)
    {
        if (element == null) return;

        var regularFont = new Font("Segoe UI", 11.25F, FontStyle.Regular);
        var boldFont = new Font("Segoe UI", 11.25F, FontStyle.Bold);

        var tag = element.Tag as string ?? element.Tag?.ToString() ?? string.Empty;
        var module = tag.Split('.').FirstOrDefault() ?? string.Empty;

        var isItem = element.Style == DevExpress.XtraBars.Navigation.ElementStyle.Item;
        var shouldApplyFont = isItem && module != "Clean" && module != "Audit";

        // De-highlight previous selection
        if (_selectedAccordionElement != null && _selectedAccordionElement != element)
        {
            _selectedAccordionElement.Appearance.Normal.BackColor = Color.Transparent;

            var prevTag = _selectedAccordionElement.Tag as string ?? _selectedAccordionElement.Tag?.ToString() ?? string.Empty;
            var prevModule = prevTag.Split('.').FirstOrDefault() ?? string.Empty;

            var prevIsItem = _selectedAccordionElement.Style == DevExpress.XtraBars.Navigation.ElementStyle.Item;
            var prevShouldApplyFont = prevIsItem && prevModule != "Clean" && prevModule != "Audit";

            if (prevShouldApplyFont)
            {
                _selectedAccordionElement.Appearance.Normal.Font = regularFont;
            }
        }

        // Highlight new selection
        element.Appearance.Normal.BackColor = GetThemeAwareHighlightColor();

        if (shouldApplyFont)
        {
            element.Appearance.Normal.Font = boldFont;
        }

        _selectedAccordionElement = element;
    }

    private void CollapseAllAccordionGroupsExceptFirst()
    {
        if (mnuMainAccordion == null)
            return;

        mnuMainAccordion.BeginUpdate();
        try
        {
            foreach (DevExpress.XtraBars.Navigation.AccordionControlElement el in mnuMainAccordion.Elements)
            {
                if (el == null)
                    continue;

                if (el.Style == DevExpress.XtraBars.Navigation.ElementStyle.Group)
                {
                    el.Expanded = false;
                }
            }

            // Expand only the first top-level group
            var firstGroup = mnuMainAccordion.Elements
                .OfType<DevExpress.XtraBars.Navigation.AccordionControlElement>()
                .FirstOrDefault(x => x.Style == DevExpress.XtraBars.Navigation.ElementStyle.Group);

            if (firstGroup != null)
            {
                firstGroup.Expanded = true;

                mnuMainAccordion.ActiveGroup = firstGroup;
            }
        }
        finally
        {
            mnuMainAccordion.EndUpdate();
        }
    }

    private void _recentProjectService_OnRecentListChanged()
    {
        UpdateRecentProject();
    }

    private void UpdateRecentProject()
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(UpdateRecentProject));
            return;
        }
        gridLatestProjects.BeginUpdate();
        gridLatestProjects.DataSource = null;
        var recentProjectResult = _recentProjectService.GetRecentProjectsAsync().Result;
        gridLatestProjects.DataSource = recentProjectResult;
        gridLatestProjects.Refresh();
        gridLatestProjects.EndUpdate();

        if (!recentProjectResult.Any())
        {
            menuLastProject.OptionsNavPane.NavPaneState = NavPaneState.Collapsed;
        }
    }

    private void gridLatestProjects_DoubleClick(object sender, EventArgs e)
    {
        if (gvLatestProject.GetSelectedRows().Length <= 0)
        {
            return;
        }
        var project = gvLatestProject.GetRow(gvLatestProject.GetSelectedRows()[0]) as RecentProject;

        if (project == null)
        {
            return;
        }

        if (navFraimMain.SelectedPage != navPageData) menuLastProject.OptionsNavPane.NavPaneState = NavPaneState.Collapsed;

        if (_recentProjectService.CheckProjectIfExist(project))
        {
            var service = WinPureUiDependencyResolver.Resolve<IDataManagerService>();
            var projectService = WinPureUiDependencyResolver.Resolve<IProjectService>();

            if (service.IsAnyTable)
            {
                if (!frmMain.ProjectSaveAndContinue(service.ProjectName)) return;

            }
            projectService.LoadProjectAsync(project.Path);

        }
        else
        {
            if (MessageBox.Show("Project does not exist. Remove it from list?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _recentProjectService.RemoveProjectAsync(project);
            }
        }

    }

    private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
    {
        if (e.SelectedControl != gridLatestProjects)
        {
            return;
        }

        ToolTipControlInfo info = null;

        // Get the view at the current mouse position.
        var view = gvLatestProject;

        // Get the information about the visual element at the current mouse position.
        var hi = view.CalcHitInfo(e.ControlMousePosition);

        // Get row object based on mouse location
        var rowObject = (RecentProject)view.GetRow(hi.RowHandle); // get object
        if (rowObject == null)
        {
            return;
        }
        // do some calculation to get necessary values to display in the tooltip


        // Assign these values to your tooltip
        info = new ToolTipControlInfo();
        info.Object = rowObject;

        // Create your super tooltip
        var superToolTip = new SuperToolTip();
        superToolTip.FixedTooltipWidth = false;
        superToolTip.MaxWidth = 500;
        var titleItem = new ToolTipTitleItem();
        titleItem.Text = rowObject.Path;

        superToolTip.Items.Add(titleItem);

        // Assign super tooltip
        info.SuperTip = superToolTip;

        // Assign the tooltip information if applicable; otherwise, preserve the default tooltip.
        if (info != null)
        {
            e.Info = info;
        }
    }

    private void navFraimMain_SelectedPageChanged(object sender, DevExpress.XtraBars.Navigation.SelectedPageChangedEventArgs e)
    {
        if (e.Page == navPageData)
        {
            menuLastProject.OptionsNavPane.NavPaneState = NavPaneState.Expanded;
        }
        else
        {
            menuLastProject.OptionsNavPane.NavPaneState = NavPaneState.Collapsed;
        }

        // Re-adjust layout when page changes
        AdjustLayoutForFormSize();
    }

    private static void ApplySkin(SkinSvgPalette skinName)
    {
        if (string.IsNullOrWhiteSpace(skinName)) return;
        UserLookAndFeel.Default.SetSkinStyle(skinName);
    }

    private void bbLightDefault_ItemClick(object sender, ItemClickEventArgs e)
    {
        ApplySkin(SkinSvgPalette.WXICompact.Default);
    }

    private void bbLightClearness_ItemClick(object sender, ItemClickEventArgs e)
    {
        ApplySkin(SkinSvgPalette.WXICompact.Clearness);
    }

    private void bbLightOfficeWhite_ItemClick(object sender, ItemClickEventArgs e)
    {
        ApplySkin(SkinSvgPalette.WXICompact.OfficeWhite);
    }

    private void bbLightOfficeColourful_ItemClick(object sender, ItemClickEventArgs e)
    {
        ApplySkin(SkinSvgPalette.WXICompact.OfficeColorful);
    }

    private void barDarknessSubButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        ApplySkin(SkinSvgPalette.WXICompact.Darkness);
    }

    private void barSharpnessButtonItem_ItemClick(object sender, ItemClickEventArgs e)
    {
        ApplySkin(SkinSvgPalette.WXICompact.Sharpness);
    }

    private void barToggleSwitchMatchTableAutoFiler_CheckedChanged(object sender, ItemClickEventArgs e)
    {
        barToggleSwitchMatchAiTableAutoFiler.CheckedChanged -= barToggleSwitchMatchAiTableAutoFiler_CheckedChanged;
        barToggleSwitchMatchAiTableAutoFiler.Checked = barToggleSwitchMatchTableAutoFiler.Checked;
        barToggleSwitchMatchAiTableAutoFiler.CheckedChanged += barToggleSwitchMatchAiTableAutoFiler_CheckedChanged;
        ucMainMatch.SetFilterRowVisibility(barToggleSwitchMatchTableAutoFiler.Checked);
        ucEntityResolution.SetFilterRowVisibility(barToggleSwitchMatchTableAutoFiler.Checked);

        _configurationService.Configuration.ShowAutoFilterRow = barToggleSwitchMatchTableAutoFiler.Checked;
        _configurationService.SaveConfiguration();
    }

    private void barToggleSwitchMatchResultSystemFields_CheckedChanged(object sender, ItemClickEventArgs e)
    {
        _configurationService.Configuration.ShowSystemFields = barToggleSwitchMatchResultSystemFields.Checked;
        _configurationService.SaveConfiguration();

        barToggleSwitchMatchAiResultSystemFields.CheckedChanged -= barToggleSwitchMatchAiResultSystemFields_CheckedChanged;
        barToggleSwitchMatchAiResultSystemFields.Checked = barToggleSwitchMatchResultSystemFields.Checked;
        barToggleSwitchMatchAiResultSystemFields.CheckedChanged += barToggleSwitchMatchAiResultSystemFields_CheckedChanged;
        ucMainMatch.UpdateSystemColumnVisibility();
        ucEntityResolution.UpdateSystemColumnVisibility();
    }

    private void barToggleSwitchMatchAiTableAutoFiler_CheckedChanged(object sender, ItemClickEventArgs e)
    {
        barToggleSwitchMatchTableAutoFiler.CheckedChanged -= barToggleSwitchMatchAiTableAutoFiler_CheckedChanged;
        barToggleSwitchMatchTableAutoFiler.Checked = barToggleSwitchMatchAiTableAutoFiler.Checked;
        barToggleSwitchMatchTableAutoFiler.CheckedChanged += barToggleSwitchMatchAiTableAutoFiler_CheckedChanged;
        ucMainMatch.SetFilterRowVisibility(barToggleSwitchMatchAiTableAutoFiler.Checked);
        ucEntityResolution.SetFilterRowVisibility(barToggleSwitchMatchAiTableAutoFiler.Checked);

        _configurationService.Configuration.ShowAutoFilterRow = barToggleSwitchMatchAiTableAutoFiler.Checked;
        _configurationService.SaveConfiguration();
    }

    private void barToggleSwitchMatchAiResultSystemFields_CheckedChanged(object sender, ItemClickEventArgs e)
    {
        _configurationService.Configuration.ShowSystemFields = barToggleSwitchMatchAiResultSystemFields.Checked;
        _configurationService.SaveConfiguration();

        barToggleSwitchMatchResultSystemFields.CheckedChanged -= barToggleSwitchMatchResultSystemFields_CheckedChanged;
        barToggleSwitchMatchResultSystemFields.Checked = barToggleSwitchMatchAiResultSystemFields.Checked;
        barToggleSwitchMatchResultSystemFields.CheckedChanged += barToggleSwitchMatchResultSystemFields_CheckedChanged;
        ucMainMatch.UpdateSystemColumnVisibility();
        ucEntityResolution.UpdateSystemColumnVisibility();
    }

    private void InitializeLatestProjectsTileHover()
    {
        gvLatestProject.OptionsTiles.AllowItemHover = true;
    }

    private void UcVerification_OnNavigateToFullReport()
    {
        acitVerificationReport_Click(null, null);
    }

    private void gvLatestProject_ItemCustomize(object sender, TileViewItemCustomizeEventArgs e)
    {

        if (gvLatestProject.GetRow(e.RowHandle) is not RecentProject project)
        {
            return;
        }

        static TileViewItemElement FindElement(TileViewItem tileItem, string elementName)
        {
            return tileItem.Elements
                .OfType<TileViewItemElement>()
                .FirstOrDefault(x => string.Equals(x.Name, elementName, StringComparison.Ordinal));
        }

        void SetSvg(string elementName, bool enabled, int trueIndex, int falseIndex)
        {
            var element = FindElement(e.Item, elementName);
            if (element == null)
            {
                return;
            }

            element.ImageOptions.SvgImage = enabled
                ? svgImageCollection2[trueIndex]
                : svgImageCollection2[falseIndex];

            element.ImageOptions.SvgImageSize = new Size(20, 20);
        }

        SetSvg(IsCleansingTileElement, project.IsCleansing, trueIndex: 0, falseIndex: 1);
        SetSvg(IsMatchTileElement, project.IsMatch, trueIndex: 2, falseIndex: 3);
        SetSvg(IsMatchAiTileElement, project.IsMatchAi, trueIndex: 4, falseIndex: 5);
        SetSvg(IsAddressVerificationTileElement, project.IsAddressVerification, trueIndex: 6, falseIndex: 7);
        SetSvg(IsAutomationTileElement, project.IsAutomation, trueIndex: 8, falseIndex: 9);
        SetSvg(IsAuditLogTileElement, project.IsAuditLog, trueIndex: 10, falseIndex: 11);
    }

    private void gvLatestProject_MouseDown(object sender, MouseEventArgs e)
    {
        var hitInfo = gvLatestProject.CalcHitInfo(e.Location);

        if (!hitInfo.InItem || hitInfo.ItemInfo == null)
            return;

        var currentItem = gvLatestProject.GetRow(hitInfo.RowHandle) as RecentProject;
        if (currentItem == null)
            return;

        // Identify the clicked tile element by checking which element bounds contain the click point
        var clickedElement = hitInfo.ItemInfo.Elements
            .OfType<TileItemElementViewInfo>()
            .FirstOrDefault(el => el.EntireElementBounds.Contains(e.Location));

        if (clickedElement == null)
            return;

        var elementName = (clickedElement.Element as TileViewItemElement)?.Name;
        if (string.IsNullOrEmpty(elementName))
            return;

        if (elementName == "DeleteProjectTileElement")
        {
            if (MessageBox.Show(string.Format(Resources.UI_MESSAGE_CONFIRM_REMOVE_PROJECT, currentItem.Name), Resources.MESSAGE_MESSAGEDIALOG_WARNING_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _recentProjectService.RemoveProjectAsync(currentItem);
            }
        }
        else if (elementName == "EditProjectTileElement")
        {
            var newNameForm = WinPureUiDependencyResolver.Resolve<frmNewName>();
            newNameForm.Show(Resources.UI_CAPTION_SET_PROJECT_NAME, Resources.UI_CAPTION_SET_PROJECT_NAME, currentItem.Name, new char[0]);
            if (newNameForm.DialogResult == DialogResult.OK)
            {
                currentItem.Name = newNameForm.NewName;
                _recentProjectService.RenameProjectAsync(currentItem);
            }
        }
    }

    private void btnstartup_ItemClick(object sender, ItemClickEventArgs e)
    {
        ShowStartupScreen(true);
    }
}