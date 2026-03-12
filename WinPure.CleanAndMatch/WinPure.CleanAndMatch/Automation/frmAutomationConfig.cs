using Newtonsoft.Json;
using WinPure.Automation.Enums;
using WinPure.Automation.Models;

namespace WinPure.CleanAndMatch.Automation;

public partial class frmAutomationConfig : XtraForm
{
    private AutomationConfiguration _configuration;

    internal AutomationConfiguration Configuration
    {
        get => _configuration;
        set
        {
            _configuration = value;
            if (_configuration == null)
            {
                return;
            }
            txtConfigName.Text = _configuration.Name;
            var step1 = _configuration.Steps.FirstOrDefault(x => x.StepType == AutomationStepType.OpenProject);
            if (step1 == null)
            {
                return;
            }
            txtProjectPath.Text = step1.SourceName;


            _cleanSteps.AddRange(_configuration.Steps.Where(x => x.StepType == AutomationStepType.ApplyClean).OrderBy(x => x.Order).Select(x => new AutomationStepSetting
            {
                TableName = x.SourceName,
                CleanFileName = x.Param1
            }).ToList());
            gridCleanAction.RefreshDataSource();

            _cleanExport.AddRange(_configuration.Steps.Where(x => x.StepType == AutomationStepType.ExportCleanResult).OrderBy(x => x.Order).Select(x => new AutomationStepSetting
            {
                TableName = x.SourceName,
                CleanFileName = x.Param1,
                Parameters = x.Param2
            }).ToList());
            gridCleanExport.RefreshDataSource();

            _matchResultExport.AddRange(_configuration.Steps.Where(x => x.StepType == AutomationStepType.ExportMatchResult).OrderBy(x => x.Order).Select(x => new AutomationStepSetting
            {
                TableName = x.SourceName,
                CleanFileName = x.Param1,
                Parameters = x.Param2
            }).ToList());
            gridMatchResultExport.RefreshDataSource();

            _matchAiResultExport.AddRange(_configuration.Steps.Where(x => x.StepType == AutomationStepType.ExportMatchAiResult).OrderBy(x => x.Order).Select(x => new AutomationStepSetting
            {
                TableName = x.SourceName,
                CleanFileName = x.Param1,
                Parameters = x.Param2
            }).ToList());
            gridMatchAiResultExport.RefreshDataSource();

            var match = _configuration.Steps.FirstOrDefault(x => x.StepType == AutomationStepType.PerformMatch);
            cbRunMatching.Checked = match != null;
            var matchAi = _configuration.Steps.FirstOrDefault(x => x.StepType == AutomationStepType.PerformMatchAi);
            cbRunMatchAi.Checked = matchAi != null;
        }
    }

    private readonly List<AutomationStepSetting> _cleanSteps;
    private readonly List<AutomationStepSetting> _cleanExport;
    private readonly List<AutomationStepSetting> _matchResultExport;
    private readonly List<AutomationStepSetting> _matchAiResultExport;


    private readonly ProjectSettings _projectSettings;
    private readonly IConnectionManager _connectionManager;
    private readonly MatchSettingsViewModel _matchSettings;


    public frmAutomationConfig()
    {
        InitializeComponent();
        Localization();
        configWizard.SelectedPage = welcomeWizardPage1;
        configWizard.Refresh();

        _cleanSteps = new List<AutomationStepSetting>();
        _cleanExport = new List<AutomationStepSetting>();
        _matchResultExport = new List<AutomationStepSetting>();
        _matchAiResultExport = new List<AutomationStepSetting>();
        gridCleanAction.DataSource = _cleanSteps;
        gridCleanAction.Refresh();
        gridCleanExport.DataSource = _cleanExport;
        gridCleanExport.Refresh();
        gridMatchResultExport.DataSource = _matchResultExport;
        gridMatchResultExport.Refresh();
        gridMatchAiResultExport.DataSource = _matchAiResultExport;
        gridMatchAiResultExport.Refresh();

        _projectSettings = new ProjectSettings(Project.Helpers.ProjectHelper.CurrentProjectVersion);
        _connectionManager = WinPureConfigurationDependency.CreateConnectionManager();
        _matchSettings = new MatchSettingsViewModel();

        _configuration = new AutomationConfiguration();

        cbCleanExportType.SelectedIndex = 0;
        cbCleanExportType.Refresh();

        cbExportMatchResult.SelectedIndex = 0;
        cbExportMatchResult.Refresh();

        cbMatchResultType.SelectedIndex = 0;
        cbMatchResultType.Refresh();

        cbMatchAiResultType.SelectedIndex = 0;
        cbMatchAiResultType.Refresh();
    }

    private void Localization()
    {
        cbMatchResultType.Properties.Items.AddRange(new[]
        {
            "All data",
            "Only groups",
            "Across tables",
            "Non-matches",
            "Non matches full"
        });

        cbMatchAiResultType.Properties.Items.AddRange(new[]
        {
            "All data",
            "Only groups",
            "Non-matches",
            "Possible duplicates",
            "Possible related"
        });

        btnAddCleanConfiguration.Text = Resources.UI_ADD;
        btnAddCleanExport.Text = Resources.UI_ADD;
        btnAddMatchResultExport.Text = Resources.UI_ADD;
    }

    private void editProjectPath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        var ov = new OpenFileDialog
        {
            Filter = Resources.DIALOG_PROJECT_FORMAT,
            Multiselect = false,
            CheckFileExists = true,
        };
        if (ov.ShowDialog() == DialogResult.OK)
        {
            txtProjectPath.Text = ov.FileName;
        }
    }

    private void configWizard_CancelClick(object sender, System.ComponentModel.CancelEventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void configWizard_FinishClick(object sender, System.ComponentModel.CancelEventArgs e)
    {
        DialogResult = DialogResult.OK;
        Close();
    }

    private void configWizard_NextClick(object sender, DevExpress.XtraWizard.WizardCommandButtonClickEventArgs e)
    {
        if (configWizard.SelectedPage == wpProject)
        {
            if (txtConfigName.Text == "" || txtProjectPath.Text == "")
            {
                e.Handled = true;
                MessageBox.Show("Please populate all fields");
            }
            else
            {
                try
                {
                    _connectionManager.Initialize(txtProjectPath.Text);
                    _connectionManager.CheckConnectionState();
                    var projectService = WinPureUiDependencyResolver.Resolve<IProjectService>();
                    projectService.PreLoadProject(_connectionManager, _projectSettings, _matchSettings);
                    Configuration.Name = txtConfigName.Text;
                    Configuration.Steps.RemoveAll(x => x.StepType == AutomationStepType.OpenProject);
                    Configuration.Steps.Add(new AutomationStep { Order = 0, StepType = AutomationStepType.OpenProject, SourceName = txtProjectPath.Text, Param1 = string.Empty, Param2 = string.Empty });
                    cbCleanTables.Properties.Items.Clear();
                    cbCleanTables.Properties.Items.AddRange(_projectSettings.TableList.Select(x => x.DisplayName).ToList());
                    cbCleanTables.SelectedIndex = 0;
                    cbCleanTables.Refresh();
                    cbCleanResultTables.Properties.Items.Clear();
                    cbCleanResultTables.Properties.Items.AddRange(_projectSettings.TableList.Select(x => x.DisplayName).ToList());
                    cbCleanResultTables.SelectedIndex = 0;
                    cbCleanResultTables.Refresh();
                }
                catch (Exception)
                {
                    e.Handled = true;
                    MessageBox.Show("Project file has incorrect format");//TODO Translate
                }
            }
        }

        if (configWizard.SelectedPage == wpCleaning)
        {
            var configurationService = WinPureUiDependencyResolver.Resolve<IConfigurationService>();
            string configurationSettings = JsonConvert.SerializeObject(configurationService.Configuration);

            Configuration.Steps.RemoveAll(x => x.StepType == AutomationStepType.ApplyClean);
            for (int i = 0; i < _cleanSteps.Count; i++)
            {
                Configuration.Steps.Add(new AutomationStep
                {
                    Order = i,
                    StepType = AutomationStepType.ApplyClean,
                    SourceName = _cleanSteps[i].TableName,
                    Param1 = _cleanSteps[i].CleanFileName,
                    Param2 = configurationSettings,
                });
            }

            if (!_cleanSteps.Any())
            {
                e.Handled = true;
                configWizard.SelectedPage = wpMatching;
                return;
            }
        }

        if (configWizard.SelectedPage == wpCleanResultExport)
        {
            Configuration.Steps.RemoveAll(x => x.StepType == AutomationStepType.ExportCleanResult);
            for (int i = 0; i < _cleanExport.Count; i++)
            {
                Configuration.Steps.Add(new AutomationStep
                {
                    Order = i,
                    StepType = AutomationStepType.ExportCleanResult,
                    SourceName = _cleanExport[i].TableName,
                    Param1 = _cleanExport[i].CleanFileName,
                    Param2 = _cleanExport[i].Parameters
                });
            }
        }

        if (configWizard.SelectedPage == wpMatching)
        {
            if (cbRunMatching.Checked)
            {
                Configuration.Steps.RemoveAll(x => x.StepType == AutomationStepType.PerformMatch);
                Configuration.Steps.Add(new AutomationStep
                {
                    Order = 0,
                    StepType = AutomationStepType.PerformMatch,
                    SourceName = "True",
                    Param1 = string.Empty,
                    Param2 = string.Empty
                });
            }
            else
            {
                e.Handled = true;
                configWizard.SelectedPage = wpRunMatchAi;
                return;
            }
        }

        if (configWizard.SelectedPage == wpMatchResultExport)
        {
            Configuration.Steps.RemoveAll(x => x.StepType == AutomationStepType.ExportMatchResult);
            for (int i = 0; i < _matchResultExport.Count; i++)
            {
                Configuration.Steps.Add(new AutomationStep
                {
                    Order = i,
                    StepType = AutomationStepType.ExportMatchResult,
                    SourceName = _matchResultExport[i].TableName,
                    Param1 = _matchResultExport[i].CleanFileName,
                    Param2 = _matchResultExport[i].Parameters
                });
            }
        }

        if (configWizard.SelectedPage == wpRunMatchAi)
        {
            if (cbRunMatchAi.Checked)
            {
                Configuration.Steps.RemoveAll(x => x.StepType == AutomationStepType.PerformMatchAi);
                Configuration.Steps.Add(new AutomationStep
                {
                    Order = 0,
                    StepType = AutomationStepType.PerformMatchAi,
                    SourceName = "True",
                    Param1 = string.Empty,
                    Param2 = string.Empty

                });
            }
            else
            {
                e.Handled = true;
                configWizard.SelectedPage = completionWizardPage1;
            }
        }

        if (configWizard.SelectedPage == wpMatchAiResultExport)
        {
            Configuration.Steps.RemoveAll(x => x.StepType == AutomationStepType.ExportMatchAiResult);
            for (int i = 0; i < _matchAiResultExport.Count; i++)
            {
                Configuration.Steps.Add(new AutomationStep
                {
                    Order = i,
                    StepType = AutomationStepType.ExportMatchAiResult,
                    SourceName = _matchAiResultExport[i].TableName,
                    Param1 = _matchAiResultExport[i].CleanFileName,
                    Param2 = _matchAiResultExport[i].Parameters
                });
            }
        }
    }

    private void configWizard_PrevClick(object sender, DevExpress.XtraWizard.WizardCommandButtonClickEventArgs e)
    {
        if (configWizard.SelectedPage == completionWizardPage1 && !_matchAiResultExport.Any() && !cbRunMatchAi.Checked)
        {
            e.Handled = true;
            configWizard.SelectedPage = wpRunMatchAi;
            return;
        }

        if (configWizard.SelectedPage == wpRunMatchAi && !_matchResultExport.Any() && !cbRunMatching.Checked)
        {
            e.Handled = true;
            configWizard.SelectedPage = wpMatching;
            return;
        }

        if (configWizard.SelectedPage == wpMatching && !_cleanExport.Any() && !_cleanSteps.Any())
        {
            e.Handled = true;
            configWizard.SelectedPage = wpCleaning;
            return;
        }
    }

    private void txtCleanConfiguration_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        if (e.Button.Tag.ToString() == "select")
        {
            var dlgSelectTextFile = new OpenFileDialog
            {
                Title = Resources.DIALOG_IMPORT_MATRIX_CAPTION,
                FileName = "",
                CheckFileExists = true,
                Filter = Resources.DIALOG_JSONFILE_FORMAT
            };

            if (dlgSelectTextFile.ShowDialog() == DialogResult.OK)
            {
                txtCleanConfiguration.Text = dlgSelectTextFile.FileName;
            }
        }
        if (e.Button.Tag.ToString() == "clear")
        {
            txtCleanConfiguration.Text = "";
        }
    }

    private void repositoryItemButtonEdit1_Click(object sender, EventArgs e)
    {
        var rw = gvCleanAction.GetRow(gvCleanAction.FocusedRowHandle) as AutomationStepSetting;
        _cleanSteps.Remove(rw);
        gridCleanAction.RefreshDataSource();
    }

    private void btnAddCleanConfiguration_Click(object sender, EventArgs e)
    {
        if (txtCleanConfiguration.Text == "") return;
        _cleanSteps.Add(new AutomationStepSetting
        {
            TableName = cbCleanTables.Text,
            CleanFileName = txtCleanConfiguration.Text
        });
        txtCleanConfiguration.Text = "";
        gridCleanAction.RefreshDataSource();
    }
        
    private void repositoryItemButtonEdit2_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        RemoveExportConfiguration(gvCleanExport);
    }

    private void repositoryItemButtonEdit3_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        RemoveExportConfiguration(gvMatchResultExport);
    }

    private void repositoryItemButtonEdit4_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
    {
        RemoveExportConfiguration(gvMatchAiResultExport);
    }

    private void RemoveExportConfiguration(GridView exportGridView)
    {
        var rw = exportGridView.GetRow(exportGridView.FocusedRowHandle) as AutomationStepSetting;
        var sourceList = exportGridView.GridControl.DataSource as List<AutomationStepSetting>;
        sourceList.Remove(rw);
        exportGridView.GridControl.RefreshDataSource();
    }

    private void btnAddCleanExport_Click(object sender, EventArgs e)
    {
        AddExportStep(gridCleanExport, cbCleanExportType.SelectedIndex, cbCleanResultTables.Text, cbCleanExportType.Text);
    }

    private void btnAddMatchResultExport_Click(object sender, EventArgs e)
    {
        AddExportStep(gridMatchResultExport, cbExportMatchResult.SelectedIndex, cbMatchResultType.Text, cbExportMatchResult.Text);
    }

    private void btnAddMatchAiExport_Click(object sender, EventArgs e)
    {
        AddExportStep(gridMatchAiResultExport, cbMatchAiExportType.SelectedIndex, cbMatchAiResultType.Text, cbMatchAiExportType.Text);
    }

    private void AddExportStep(GridControl exportGrid, int exportType,  string tableName, string fileName)
    {
        ExternalSourceTypes tp;
        switch (exportType)
        {
            case 0: //CSV
                tp = ExternalSourceTypes.TextFile;
                break;
            case 1: //EXCEL
                tp = ExternalSourceTypes.Excel;
                break;
            case 2: //ACCESS
                tp = ExternalSourceTypes.Access;
                break;
            case 3: //SQL
                tp = ExternalSourceTypes.SqlServer;
                break;
            case 4: //MYSQL
                tp = ExternalSourceTypes.MySqlServer;
                break;
            case 5: //ORACLE
                tp = ExternalSourceTypes.Oracle;
                break;
            case 6: //PostgreSQL
                tp = ExternalSourceTypes.Postgres;
                break;
            case 7: //Snowflake
                tp = ExternalSourceTypes.Snowflake;
                break;
            case 8: //Save to project
                tp = ExternalSourceTypes.DataTable;
                break;
            default: throw new ArgumentOutOfRangeException();
        }

        var param = tp == ExternalSourceTypes.DataTable ? string.Empty : ImportExportService.GetExportParameter(tp);
        if (!string.IsNullOrEmpty(param) || tp == ExternalSourceTypes.DataTable)
        {
            if (tp == ExternalSourceTypes.Excel)
            {
                var options = JsonConvert.DeserializeObject<ExcelImportExportOptions>(param);
                options.ExportWithNpoi = true;
                param = JsonConvert.SerializeObject(options);
            }

            var sourceList = exportGrid.DataSource as List<AutomationStepSetting>;

            sourceList.Add(new AutomationStepSetting
            {
                TableName = tableName,
                CleanFileName = fileName,
                Parameters = param
            });
            exportGrid.RefreshDataSource();
        }
    }

    private void frmAutomationConfig_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_connectionManager != null)
        {
            _connectionManager.CloseConnection();
        }
    }
}

class AutomationStepSetting
{
    public string TableName { get; set; }
    public string CleanFileName { get; set; }
    public string Parameters { get; set; }
}