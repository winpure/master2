using WinPure.Integration.Enums;

namespace WinPure.CleanAndMatch.Controls;

internal partial class UCMainDataNew : UCDataViewBase
{
    public event Action OnOpenProject;
    public event Action<bool> OnSaveProject;
    public event Action OnCreateNewProject;
    public event Action OnOpenSettings;
    public event Action<string, string, MessagesType, Exception> OnException;

    public UCMainDataNew()
    {
        InitializeComponent();

        importDataSourceNewControl.OnDataSourceClick += ImportExportDataClicked;
        exportDataSourceNewControl.OnDataSourceClick += ImportExportDataClicked;
        importDataSourceNewControl.OnException += (msg, title, type, ex) => OnException?.Invoke(msg, title, type, ex);

        ApplyOperationTypeToDataSourceControls();
    }

    public override void Initialize(bool useDataMenu, bool useRowSelection = false)
    {
        base.Initialize(useDataMenu, useRowSelection);
    }

    public void ShowSubPanel(MainDataType dType)
    {
        switch (dType)
        {
            case MainDataType.Export: navFrameData.SelectedPage = navPageExport; break;
            case MainDataType.Import: navFrameData.SelectedPage = navPageImport; break;
            case MainDataType.Project: navFrameData.SelectedPage = navPageProject; break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dType), dType, null);
        }
    }

    private void ucNewProject_OnCreateNewProject()
    {
        OnCreateNewProject?.Invoke();
    }

    private void ucNewProject_OnSaveProject(bool saveAs)
    {
        OnSaveProject?.Invoke(saveAs);
    }

    private void ucNewProject_OnOpenProject()
    {
        OnOpenProject?.Invoke();
    }

    private void ucNewProject_OnOpenSettings()
    {
        OnOpenSettings?.Invoke();
    }

    private void ImportExportDataClicked(OperationType operationType, ExternalSourceTypes srcType)
    {
        try
        {
            _logger.Information($"{operationType} start for external source {srcType}");
            var srv = WinPureUiDependencyResolver.Resolve<IImportExportService>();
            switch (operationType)
            {
                case OperationType.Import:
                    srv.Import(srcType);
                    break;
                case OperationType.Export:
                    srv.Export(srcType, null, GetDataForExportCriteria());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null);
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

    private void navFrameData_SelectedPageChanged(object sender, DevExpress.XtraBars.Navigation.SelectedPageChangedEventArgs e)
    {
        ApplyOperationTypeToDataSourceControls();
    }

    private void ApplyOperationTypeToDataSourceControls()
    {
        importDataSourceNewControl.DataSourceOperationType = OperationType.Import;
        exportDataSourceNewControl.DataSourceOperationType = OperationType.Export;
    }
}