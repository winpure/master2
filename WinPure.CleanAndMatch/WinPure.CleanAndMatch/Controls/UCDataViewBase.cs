using System.ComponentModel;
using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;
using WinPure.Configuration.Helper;

namespace WinPure.CleanAndMatch.Controls;

public partial class UCDataViewBase : XtraUserControl
{
    internal IDataManagerService _service;
    internal IWpLogger _logger;
    internal IProjectService _projectService;

    private bool _tableDeleting = false;
    private bool _shouldRaiseAnotherTableEvent = true;
    private bool _useDataMenu;
    private bool _useRowSelection;

    public UCDataViewBase()
    {
        InitializeComponent();
    }

    public virtual void Initialize(bool useDataMenu, bool useRowSelection = false)
    {
        _useRowSelection = useRowSelection;
        _useDataMenu = useDataMenu;
        tcData.ClosePageButtonShowMode = _useDataMenu ? ClosePageButtonShowMode.InAllTabPageHeaders : ClosePageButtonShowMode.Default;
        if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
        {
            _logger = WinPureUiDependencyResolver.Resolve<IWpLogger>();
            _service = WinPureUiDependencyResolver.Resolve<IDataManagerService>();
            _service.OnAddNewData += _service_OnAddNewData;

            _service.OnTableDataUpdateBegin += _service_OnTableDataUpdateBegin;
            _service.OnTableDataUpdateComplete += _service_OnTableDataUpdateComplete;
            _service.OnCurrentTableChanged += _service_CurrentTableChanged;
            _service.OnFiltrateData += _service_FiltrateData;
            _service.OnChangeTableDisplayName += _service_OnChangeTableDisplayName;
            _service.OnRefreshData += _service_OnRefreshData;
            _service.OnTableDelete += _service_OnTableDelete;
                
            _projectService = WinPureUiDependencyResolver.Resolve<IProjectService>();
            _projectService.OnBeforeProjectLoad += _service_OnBeforeProjectLoad;
        }
    }

    public void CloseCurrentData(bool removeControl = true)
    {
        for (int i = tcData.TabPages.Count - 1; i >= 0; i--)
        {
            var tp = tcData.TabPages[i];
            if (tp?.Tag != null)
            {
                DisposeConnection(tp.Controls.Find("wpDataGrid", true).FirstOrDefault() as DataControl);
            }
        }

        if (removeControl)
        {
            ClearTableData();
        }
    }

    internal void AddNewData(string tableName, string displayName, ExternalSourceTypes sourceType, object dataSource)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { AddNewData(tableName, displayName, sourceType, dataSource); }));
            return;
        }
        tcData.BeginUpdate();
        var tp = new XtraTabPage();
        var winPureDataGrid = new DataControl { Name = "wpDataGrid" };
        tp.Controls.Add(winPureDataGrid);
        winPureDataGrid.Dock = DockStyle.Fill;
        tp.Visible = true;
        tp.ShowCloseButton = DefaultBoolean.True;
        tp.Tag = tableName;

        tp.Text = displayName;
        var tableInfo = _service.GetTableInfo(tableName);

        DataHelper.SetTabIconAndTooltip(tp, sourceType, tableInfo.ImportParameters);

        winPureDataGrid.SetDataSource(dataSource);

        if (sourceType == ExternalSourceTypes.JSONL)
        {
            winPureDataGrid.SetRowHeight();
        }

        tcData.TabPages.Add(tp);
        tcData.SelectedTabPage = tp;

        if (_useDataMenu)
        {
            winPureDataGrid.OnDataClose += OnCloseData;
            winPureDataGrid.OnRenameTable += OnRenameTable;
            winPureDataGrid.OnRefreshData += OnRefreshData;
            winPureDataGrid.OnAddNewColumn += OnAddNewColumn;
            winPureDataGrid.OnRemoveColumn += OnRemoveColumn;
            winPureDataGrid.OnCopyColumn += OnCopyColumn;
            winPureDataGrid.OnRenameColumn += OnRenameColumn;
            winPureDataGrid.OnDeleteRecords += OnDeleteRecords;
            winPureDataGrid.OnCopyToData += WinPureDataGrid_OnCopyToData;
        }

        if (_useRowSelection)
        {
            winPureDataGrid.SetAdditionalSettings(_useDataMenu, _useRowSelection);
        }

        tcData.EndUpdate();
        tcData.Visible = true;
    }

    internal string GetDataForExportCriteria()
    {
        var tp = tcData.SelectedTabPage;
        if (tp?.Tag != null)
        {
            if (tp.Controls.Find("wpDataGrid", true).FirstOrDefault() is DataControl dataControl)
            {
                return dataControl.GetDataCriteria();
            }
        }
        return null;
    }

    private void _service_OnTableDelete(ImportedDataInfo table)
    {
        if (!_tableDeleting)
        {
            var page = tcData.TabPages.FirstOrDefault(x => x.Tag != null && x.Tag.ToString() == table.TableName);
            if (page != null)
            {
                tcData.TabPages.Remove(page);
                DisposeConnection(page.Controls.Find("wpDataGrid", true).FirstOrDefault() as DataControl);


                if (tcData.TabPages.Count == 0)
                {
                    tcData.Visible = false;
                }
            }
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
        if (tcData.SelectedTabPage == null)
        {
            return;
        }
        if (tcData.SelectedTabPage?.Tag?.ToString() == tableInfo.TableName)
        {
            tp = tcData.SelectedTabPage;
        }
        else
        {
            foreach (XtraTabPage page in tcData.TabPages)
            {
                if (page?.Tag?.ToString() == tableInfo.TableName)
                {
                    tp = page;
                    break;
                }
            }
        }

        if (tp == null)
        {
            return;
        }

        var ds = tableInfo.TableName.GetData();

        tcData.BeginUpdate();

        var dat = tp.Controls.Find("wpDataGrid", true).FirstOrDefault() as DataControl;
        if (dat == null)
        {
            return;
        }
        DisposeConnection(dat);
        dat.UpdateDataSource(ds);

        tcData.EndUpdate();
        tcData.Visible = true;
    }

    private void _service_OnChangeTableDisplayName(string tableName, string newDisplayName)
    {
        var pg = tcData.TabPages.FirstOrDefault(x => x.Tag != null && x.Tag.ToString() == tableName);
        if (pg != null)
        {
            pg.Text = newDisplayName;
        }
    }

    private void _service_FiltrateData(string columnName, FiltrateField filter)
    {
        var tp = tcData.SelectedTabPage;
        if (tp?.Tag != null)
        {
            var ctrl = tp.Controls.Find("wpDataGrid", true).FirstOrDefault() as DataControl;
            ctrl?.SetFilter(columnName, filter);
        }
    }

    private void _service_OnTableDataUpdateComplete(string tableName)
    {
        var tbpProp = _service.GetTableInfo(tableName);

        if (tbpProp == null)
        {
            return;
        }
        var ds = tbpProp.TableName.GetData();

        for (int i = tcData.TabPages.Count - 1; i >= 0; i--)
        {
            var tp = tcData.TabPages[i];
            if (tp.Tag.ToString() == tableName)
            {
                var dataControl = tp.Controls.Find("wpDataGrid", true).FirstOrDefault() as DataControl;
                dataControl?.UpdateDataSource(ds);
            }
        }
    }

    private void _service_OnTableDataUpdateBegin(string tableName)
    {
        for (int i = tcData.TabPages.Count - 1; i >= 0; i--)
        {
            var tp = tcData.TabPages[i];
            if (tp.Tag.ToString() == tableName)
            {
                DisposeConnection(tp.Controls.Find("wpDataGrid", true).FirstOrDefault() as DataControl);
                return;
            }
        }
    }

    private void _service_OnBeforeProjectLoad()
    {
        CloseCurrentData();
    }

    private void _service_OnAddNewData(string tableName)
    {
        string displayName;
        var tableInfo = _service.GetTableInfo(tableName);
        if (tableInfo != null)
        {
            displayName = "   " + tableInfo.DisplayName + "   ";
        }
        else
        {
            displayName = tableName;
        }
        var ds = tableInfo.TableName.GetData();
        AddNewData(tableName, displayName, tableInfo.SourceType, ds);
    }

    private void _service_CurrentTableChanged(string tableName)
    {
        if (tcData.SelectedTabPage == null || tcData.SelectedTabPage.Tag.ToString() == tableName)
        {
            return;
        }

        _shouldRaiseAnotherTableEvent = false;
        foreach (XtraTabPage page in tcData.TabPages)
        {
            if (page.Tag.ToString() == tableName && !IsDisposing(page))
            {
                tcData.SelectedTabPage = page;
                _shouldRaiseAnotherTableEvent = true;
                return;
            }
        }
        _shouldRaiseAnotherTableEvent = true;
    }

    private void DisposeConnection(DataControl dtCtrl)
    {
        var dataSource = dtCtrl.ControlDataSource();
        dtCtrl.SetDataSource(null);
        XpoCollectionHelper.DisposeWithOwner(dataSource);
        dtCtrl.Dispose();
    }

    private bool IsDisposing(XtraTabPage page)
    {
        if (page?.Tag != null)
        {
            var dtCtrl = page.Controls.Find("wpDataGrid", true).FirstOrDefault() as DataControl;
            if (dtCtrl?.ControlDataSource() is XPServerCollectionSource ds)
            {
                var oldSession = ds.Session;

                return !oldSession.IsConnected;
            }
        }

        return false;
    }

    private void CloseDataTable(XtraTabPage page)
    {
        if (page?.Tag != null)
        {
            _tableDeleting = true;
            _service.DeleteTable(page.Tag.ToString());
            tcData.TabPages.Remove(page);
            DisposeConnection(page.Controls.Find("wpDataGrid", true).FirstOrDefault() as DataControl);


            if (tcData.TabPages.Count == 0)
            {
                tcData.Visible = false;
            }
            _tableDeleting = false;
        }
    }

    private void ClearTableData()
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(ClearTableData));
            return;
        }
        tcData.TabPages.Clear();
        tcData.Visible = false;
    }

    private void OnRefreshData()
    {
        var ctp = tcData.SelectedTabPage;
        if (ctp?.Tag != null)
        {
            _service.ReimportDataAsync(ctp.Tag.ToString());
        }
    }

    private void OnCloseData()
    {
        CloseDataTable(tcData.SelectedTabPage);
    }

    private void OnAddNewColumn()
    {
        var ctp = tcData.SelectedTabPage;
        if (ctp?.Tag != null)
        {
            var frmAdd = WinPureUiDependencyResolver.Resolve<frmAddNewColumn>();
            if (frmAdd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(frmAdd.ColumnName))
            {
                _service.AddTableColumn(ctp.Tag.ToString(), frmAdd.ColumnName, frmAdd.ColumnType);
            }
        }
    }

    private void OnRenameColumn()
    {
        var ctp = tcData.SelectedTabPage;
        if (ctp?.Tag != null)
        {
            var tbl = _service.GetTableInfo(ctp.Tag.ToString());
            var frmSelect = WinPureUiDependencyResolver.Resolve<frmSelectFields>();

            if (frmSelect.ShowRenameDialog(tbl) == DialogResult.OK)
            {
                _service.RenameColumn(ctp.Tag.ToString(), frmSelect.SelectedFields.Cast<DataField>().Where(x => x.DisplayName != x.DatabaseName).ToList());
            }
        }
    }

    private void OnCopyColumn()
    {
        var ctp = tcData.SelectedTabPage;
        if (ctp?.Tag == null)
        {
            return;
        }
        var tbl = _service.GetTableInfo(ctp.Tag.ToString());
        var frmRemove = WinPureUiDependencyResolver.Resolve<frmSelectColumns>();

        if (frmRemove.Show(tbl.Fields, false) == DialogResult.OK)
        {
            _service.CopyColumn(ctp.Tag.ToString(), frmRemove.SelectedColumn);
        }
    }

    private void OnRemoveColumn()
    {
        var ctp = tcData.SelectedTabPage;
        if (ctp?.Tag == null)
        {
            return;
        }
        var tbl = _service.GetTableInfo(ctp.Tag.ToString());
        var frmRemove = WinPureUiDependencyResolver.Resolve<frmSelectColumns>();

        if (frmRemove.Show(tbl.Fields, true) == DialogResult.OK)
        {
            _service.RemoveColumn(ctp.Tag.ToString(), frmRemove.SelectedColumn);
        }
    }

    private void OnRenameTable()
    {
        var ctp = tcData.SelectedTabPage;
        if (ctp?.Tag == null)
        {
            return;
        }
        var frmEnterName = WinPureUiDependencyResolver.Resolve<frmNewName>();
        var notAllowed = @"\/:*?""<>|[]%#()'€$~`^,";
        if (frmEnterName.Show("Table name", "Please enter new table name", ctp.Text.Trim(), notAllowed.ToCharArray()) == DialogResult.OK)
        {
            _service.ChangeTableDisplayName(ctp.Tag.ToString(), frmEnterName.NewName);
        }
    }

    private void WinPureDataGrid_OnCopyToData()
    {
        var connectionManager = WinPureUiDependencyResolver.Resolve<IConnectionManager>();
        var filter = GetDataForExportCriteria();
        var tableInfo = _service.GetTableInfo(_service.CurrentTable);

        var qry = SqLiteHelper.GetSelectQuery(tableInfo.TableName, String.Empty, filter);
        var data = SqLiteHelper.ExecuteQuery(qry, connectionManager.Connection);
        var dataName = $"{tableInfo.DisplayName}_Copy";
        _service.SaveResultToData(dataName, data);
    }

    private void OnDeleteRecords()
    {
        var ctp = tcData.SelectedTabPage;
        if (ctp?.Tag != null)
        {
            _service.DeleteRecords(ctp.Tag.ToString());
        }
    }

    private void tcData_CloseButtonClick(object sender, System.EventArgs e)
    {
        var arg = e as ClosePageButtonEventArgs;
        var tp = arg?.Page as XtraTabPage;
        CloseDataTable(tp);
    }

    private void tcData_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
    {
        if (_shouldRaiseAnotherTableEvent && tcData.TabPages.Any() && tcData.SelectedTabPage != null)
        {
            _service.SetSelectedTable(tcData.SelectedTabPage.Tag.ToString());
        }
    }
}