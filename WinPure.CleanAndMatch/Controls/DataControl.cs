using DevExpress.XtraEditors.Repository;

namespace WinPure.CleanAndMatch.Controls;

public partial class DataControl : XtraUserControl, IDisposable
{
    public event Action OnDataClose;
    public event Action OnRefreshData;
    public event Action OnAddNewColumn;
    public event Action OnRemoveColumn;
    public event Action OnCopyColumn;
    public event Action OnRenameTable;
    public event Action OnRenameColumn;
    public event Action OnDeleteRecords;
    public event Action OnCopyToData;
    private bool _selectWithCheckbox;
    private List<long> _selectedIds;
    private bool _showNullValues = false;
    private IConfigurationService _configurationService;

    public DataControl()
    {
        InitializeComponent();
        btnRefresh.Text = Resources.CAPTION_DATATABLE_REFRESH;
        btnRename.Text = Resources.CAPTION_DATATABLE_RENAME;
        btnRemoveColumn.Text = Resources.CAPTION_DATATABLE_REMOVE_COLUMN;
        btnInsertColumn.Text = Resources.CAPTION_DATATABLE_INSERT_COLUMN;
        //btnFilterPanelOn.Text = Resources.CAPTION_DATATABLE_FILTER_PANEL;
        //btnRemove.Text = Resources.CAPTION_DATATABLE_CLOSE;
        btnCopyColumn.Text = Resources.CAPTION_DATATABLE_COPY_COLUMN;
        btnDeleteRecords.Text = Resources.CAPTION_DATATABLE_DELETE_RECORDS;
        btnMoveToData.Text = Resources.UI_CAPTION_MOVETODATA;
        _configurationService = WinPureUiDependencyResolver.Resolve<IConfigurationService>();
        _showNullValues = _configurationService.Configuration.ShowNullValues;
        _configurationService.OnConfigurationChanged += ConfigurationService_OnConfigurationChanged;
    }

    public void UpdateDataSource(object ds)
    {
        if (ds == null) return;

        if (this.InvokeRequired)
        {
            this.Invoke(new MethodInvoker(delegate { UpdateDataSource(ds); }));
            return;
        }

        gridData.DataSource = null;
        gvData.Columns.Clear();
        SetDataSource(ds);
        CorrectDateTime();
    }

    public void Dispose()
    {
        _configurationService.OnConfigurationChanged -= ConfigurationService_OnConfigurationChanged;
        //base.Dispose();
    }

    private void ConfigurationService_OnConfigurationChanged()
    {
        _showNullValues = _configurationService.Configuration.ShowNullValues;
        gvData.RefreshData();
    }

    private void CheckSelection()
    {
        if (_selectWithCheckbox)
        {
            if (_selectedIds == null || _selectedIds.Count == gvData.RowCount)
            {
                gvData.SelectAll();
            }
            else if (_selectedIds.Any())
            {
                gvData.ClearSelection();
                foreach (var id in _selectedIds)
                {
                    var rw = gvData.LocateByValue(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY, id);
                    gvData.SelectRow(rw);
                }
            }
        }
    }

    private void CorrectDateTime()
    {
        foreach (GridColumn column in gvData.Columns)
        {
            if (column.ColumnType == typeof(DateTime))
            {
                column.ColumnEdit = repositoryItemDateEdit1;
            }
        }
    }

    public void SetAdditionalSettings(bool isVisible, bool showSelection)
    {
        panelMenu.Visible = isVisible;
        gvData.OptionsSelection.MultiSelect = isVisible;
        _selectWithCheckbox = showSelection;
        if (_selectWithCheckbox)
        {
            gvData.OptionsSelection.MultiSelect = true;
            gvData.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
            gvData.Appearance.SelectedRow.BackColor = Color.White;
            gvData.Appearance.SelectedRow.ForeColor = Color.Black;
            gvData.OptionsSelection.EnableAppearanceFocusedCell = false;
            gvData.OptionsSelection.EnableAppearanceFocusedRow = false;
            gvData.OptionsSelection.EnableAppearanceHideSelection = false;
            //gvData.SelectAll();
            gvData.Focus();
        }
    }

    public void SetRowHeight()
    {
        var memoEdit = new RepositoryItemMemoEdit();
        gridData.RepositoryItems.Add(memoEdit);

        foreach (GridColumn column in gvData.Columns)
        {
            column.ColumnEdit = memoEdit;
        }
        gridData.RefreshDataSource();
    }

    public List<long> GetSelectedRows()
    {
        _selectedIds = gvData.GetSelectedWpKeysForSelectedRows();

        return _selectedIds;
    }

    public void SetDataSource(object ds)
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new MethodInvoker(delegate { SetDataSource(ds); }));
            return;
        }

        if (ds == null)
        {
            gridData.DataSource = null;
            return;
        }

        if (this.InvokeRequired)
        {
            this.Invoke(new MethodInvoker(delegate { SetDataSource(ds); }));
            return;
        }

        gridData.BeginUpdate();
        gridData.DataSource = ds;

        gridData.Refresh();
        gvData.PopulateColumns();

        //var src = ds as XPServerCollectionSource;
        //src?.Reload();
        var col = gvData.Columns.FirstOrDefault(x => x.FieldName == WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY);
        if (col != null) col.Visible = false;

        var visibleColumn = gvData.Columns.FirstOrDefault(x => x.Visible);
        if (visibleColumn != null)
        {
            visibleColumn.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Count;
            visibleColumn.SummaryItem.DisplayFormat = "{0} rows";
        }

        gvData.BestFitMaxRowCount = 100;
        gvData.BestFitColumns();

        CorrectDateTime();
        CheckSelection();
        gridData.EndUpdate();
    }

    public string GetDataCriteria()
    {
        return DataHelper.GetSqliteFilterCriteriaFromGridView(gvData);
    }

    public void SetFilter(string columnName, FiltrateField filter)
    {
        switch (filter)
        {
            case FiltrateField.Filled:
                gvData.ActiveFilterString = $"Not IsNullOrEmpty([{columnName}])";
                break;
            case FiltrateField.Empty:
                gvData.ActiveFilterString = $"IsNullOrEmpty([{columnName}])";
                break;
            default:
                return;
        }
        gvData.ActiveFilterEnabled = true;
    }

    public object ControlDataSource()
    {
        return gridData.DataSource;
    }

   
    private void btnRefresh_Click(object sender, EventArgs e)
    {
        OnRefreshData?.Invoke();
    }

    private void btnRename_Click(object sender, EventArgs e)
    {
        OnRenameTable?.Invoke();
    }

    private void btnRemoveColumn_Click(object sender, EventArgs e)
    {
        OnRemoveColumn?.Invoke();
    }

    private void btnInsertColumn_Click(object sender, EventArgs e)
    {
        OnAddNewColumn?.Invoke();
    }

    private void btnCopyColumn_Click(object sender, EventArgs e)
    {
        OnCopyColumn?.Invoke();
    }

    private void btnRenameColumn_Click(object sender, EventArgs e)
    {
        OnRenameColumn?.Invoke();
    }

    private void btnRemoveData_Click(object sender, EventArgs e)
    {
        OnDataClose?.Invoke();
    }

    private void btnHideUnhideColumns_Click(object sender, EventArgs e)
    {
        if (gvData.CustomizationForm == null)
            gvData.ShowCustomization();
        else
            gvData.DestroyCustomization();
    }

    private void btnFilterPanelOn_Click(object sender, EventArgs e)
    {
        gvData.OptionsView.ShowAutoFilterRow = !gvData.OptionsView.ShowAutoFilterRow;
    }

    private void btnDeleteRecords_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show(Resources.CAPTION_DATATABLE_DELETE_RECORDS, Resources.MESSAGE_QUESTION_CAPTION,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            var ds = gridData.DataSource as XPServerCollectionSource;
            if (ds != null)
            {
                ds.Session.BeginTransaction();
                try
                {
                    gvData.DeleteSelectedRows();
                    ds.Session.CommitTransaction();
                }
                catch (Exception)
                {
                    ds.Session.RollbackTransaction();
                }
            }
            else
            {
                gvData.DeleteSelectedRows();
            }

            OnDeleteRecords?.Invoke();
        }
    }

    private void gvData_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
    {
        if (_showNullValues && e.Value == null)
        {
            e.DisplayText = "{null}";
        }
    }

    private void btnMoveToData_Click(object sender, EventArgs e)
    {
        OnCopyToData?.Invoke();
    }
}