using DevExpress.LookAndFeel;
using DevExpress.XtraTab;
using System.ComponentModel;
using WinPure.DataService.Senzing.Models;
using DriveType = WinPure.Common.Models.DriveType;


namespace WinPure.CleanAndMatch.Controls;

public partial class UCEntityResolutionConfiguration : UCDataViewBase
{
    private readonly List<string> _shiftFiledList = new List<string> { "AI_Type", "AI_Ignore", "AI_Include" };
    private System.Threading.Timer _timer;
    private readonly ILicenseService _licenseService;
    private EntityResolutionConfiguration _erConfiguration;
    private readonly ThemeDetectionService _themeDetectionService;

    // rectangle where the help icon is painted on header
    private Rectangle _headerHelpRect = Rectangle.Empty;

    public UCEntityResolutionConfiguration()
    {
        InitializeComponent();
        if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
        {
            _licenseService = WinPureUiDependencyResolver.Resolve<ILicenseService>();
            _themeDetectionService = WinPureUiDependencyResolver.Resolve<ThemeDetectionService>();
            _themeDetectionService.SetReferenceControl(this);
        }



        Localization();
    }

    public override void Initialize(bool useDataMenu, bool useRowSelection = false)
    {
        base.Initialize(useDataMenu, useRowSelection);

        tlEntityFieldType.DataSource = _service.GetEntityResolutionFieldTypes();
        tcData.ClosePageButtonShowMode = ClosePageButtonShowMode.Default;
        _service.OnAddNewData += _service_OnAddNewData;
        _service.OnCurrentTableChanged += _service_OnCurrentTableChanged;
        _service.OnTableDelete += _service_OnTableDelete;
        _service.OnTableDataUpdateComplete += _service_OnTableDataUpdateComplete;
        _service.OnRefreshData += _service_OnRefreshData;
        _service.OnChangeTableDisplayName += _service_OnChangeTableDisplayName;

        // subscribe to custom draw for column header to draw help icon on the top-left of grid header area
        gvEntityConfiguration.CustomDrawColumnHeader += gvEntityConfiguration_CustomDrawColumnHeader;
        gvEntityConfiguration.MouseDown += gvEntityConfiguration_MouseDown;

        _timer = new System.Threading.Timer((e) =>
        {
            CheckSystem();
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

        // Subscribe to theme change event
        UserLookAndFeel.Default.StyleChanged += OnThemeChanged;
    }

    public void RefreshMapping(string tableName)
    {
        SetEntityResolutionSettings(tableName);
    }

    private void Localization()
    {
        btnAnalyze.Text = $"{Resources.UI_CAPTION_REVIEWMAPPING} / {Resources.UI_CAPTION_ANALYZE}";
        gvMatchTablesTitle.Text = Resources.UI_CAPTION_SELECTTABLESFORANALYZE;
        gvEntityConfigurationTitle.Text = Resources.UI_CAPTION_DEFINEFIELDSTYPE;
        colTableName.Caption = Resources.UI_TABLE;
        colFieldName.Caption = Resources.UI_UCMAINCLEANNEWFORM_FIELDNAME;
        colType.Caption = Resources.UI_TYPE;
        colLabel.Caption = Resources.UI_CAPTION_LABEL;
        colIncluded.Caption = Resources.UI_CAPTION_INCLUDED;
        colSuppressed.Caption = Resources.UI_CAPTION_SEPPRESSED;
        btnClean.Text = Resources.UI_SETMASTERRECORDSFORM_CLEARALL;
        lbMapError.Text = Resources.UI_ER_FIXMAPPING;

        if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
        {
            labelAiRegistrationMessage.Text = _licenseService.IsDemo
                ? string.Format(Resources.CAPTION_AI_DEMOMODE, GlobalConstants.ErRecordsForDemoVersion.ToString("N0"))
                : _licenseService.GetErRecordLimit() < 1000000
                    ? string.Format(Resources.CAPTION_AI_DEMOMODE, _licenseService.GetErRecordLimit().ToString("N0"))
                    : string.Format(Resources.CAPTION_AI_REGISTEREDVERSION, _licenseService.GetErRecordLimit());

            panelDemoMode.Visible = _licenseService.GetErRecordLimit() == GlobalConstants.ErRecordsForDemoVersion;

            lblErLimit.Text = _licenseService.IsDemo
                ? string.Format(Resources.CAPTION_AI_DEMOMODE2, GlobalConstants.ErRecordsForDemoVersion.ToString("N0"))
                : _licenseService.GetErRecordLimit() < 1000000
                    ? string.Format(Resources.CAPTION_AI_DEMOMODE2, _licenseService.GetErRecordLimit().ToString("N0"))
                    : string.Format(Resources.CAPTION_AI_REGISTEREDVERSION2, _licenseService.GetErRecordLimit());
        }
    }

    private void SetEntityResolutionSettings(string tableName)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { SetEntityResolutionSettings(tableName); }));
            return;
        }

        var settings = string.IsNullOrEmpty(tableName)
            ? null
            : _service.GetDataTableEntityResolutionSetting(tableName);

        _erConfiguration = string.IsNullOrEmpty(tableName)
            ? null
            : _service.VerifyEntityResolutionTableMap(_service.GetTableInfo(tableName));

        //pnlMapError.Visible = _erConfiguration != null && _erConfiguration.Errors.Any();

        gridEntityConfiguration.DataSource = null;
        gridEntityConfiguration.DataSource = settings;
        gridEntityConfiguration.Refresh();
    }

    private void UpdateMatchTables()
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(UpdateMatchTables));
            return;
        }

        btnAnalyze.Enabled = btnMap.Enabled = btnClean.Enabled = _service.IsAnyTable;

        gridMatchTables.DataSource = null;
        gridMatchTables.DataSource = _service.TableList;
        gridMatchTables.Refresh();
    }

    private void CheckSystem()
    {
        var systemInfo = SystemInfoHelper.GetSystemInformation();
        UpdateSystemIcon(systemInfo);
    }

    private void UpdateSystemIcon(HostSystemInformation systemInformation)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { UpdateSystemIcon(systemInformation); }));
            return;
        }
        var configuration = WinPureUiDependencyResolver.Resolve<IConfigurationService>().Configuration;
        var drive = Path.GetPathRoot(configuration.ErSettings.DataFolder);
        var driveInfo = systemInformation.Drives.FirstOrDefault(x => x.Letter == drive);


        if (systemInformation != null)
        {
            if (systemInformation.CoresCount < 2 ||
                systemInformation.FreeMemorySize < 6)
            {
                imageSystemCheck.SvgImage = svgImageCollection1[2];
                lbMainTable.Text = "System status impaired.";
            }
            else if (systemInformation.CoresCount < 4 ||
                     systemInformation.FreeMemorySize < 8 ||
                     driveInfo == null ||
                     driveInfo.DriveType != DriveType.SSD ||
                     (driveInfo.ConnectionType != DriveConnectionType.RAID &&
                      driveInfo.ConnectionType != DriveConnectionType.SATA &&
                      driveInfo.ConnectionType != DriveConnectionType.NVMe))
            {
                imageSystemCheck.SvgImage = svgImageCollection1[1];
                lbMainTable.Text = "System status impaired.";
            }
            else
            {
                imageSystemCheck.SvgImage = svgImageCollection1[0];
                lbMainTable.Text = "System status good.";
            }
        }
    }

    private void ProcessLinkedSettings(List<string> columnList, string column, DataRow rw)
    {
        if (columnList.Contains(column))
        {
            foreach (var colName in columnList.Where(x => x != column))
            {
                if (colName == "AI_Type" && !string.IsNullOrEmpty(rw[colName].ToString()))
                {
                    rw[colName] = "";
                    _service.SaveCleanSettings(rw[0].ToString(), colName, "");
                }
                else if (rw[colName].ToString() == "1")
                {
                    rw[colName] = "0";
                    _service.SaveCleanSettings(rw[0].ToString(), colName, "0");
                }
            }
        }
    }

    private void _service_OnTableDelete(ImportedDataInfo obj)
    {
        if (!_service.IsAnyTable)
        {
            SetEntityResolutionSettings(null);
        }
        UpdateMatchTables();
        UpdateRecordToAnalyze();
    }

    private void _service_OnRefreshData(ImportedDataInfo tableInfo)
    {
        SetEntityResolutionSettings(tableInfo.TableName);
        UpdateMatchTables();
        UpdateRecordToAnalyze();
    }

    private void _service_OnCurrentTableChanged(string tableName)
    {
        SetEntityResolutionSettings(tableName);
        var table = _service.TableList.FirstOrDefault(x => x.TableName == tableName);
        var rowId = gvMatchTables.FindRow(table);
        gvMatchTables.FocusedRowChanged -= gvMatchTables_FocusedRowChanged;
        gvMatchTables.FocusedRowHandle = rowId;
        gvMatchTables.FocusedRowChanged += gvMatchTables_FocusedRowChanged;

        btnMap.Enabled = btnClean.Enabled = table.SourceType != ExternalSourceTypes.JSONL;
        gvEntityConfiguration.OptionsBehavior.ReadOnly = table.SourceType == ExternalSourceTypes.JSONL;
    }

    private void _service_OnTableDataUpdateComplete(string tableName)
    {
        SetEntityResolutionSettings(tableName);
    }

    private void _service_OnChangeTableDisplayName(string arg1, string arg2)
    {
        UpdateMatchTables();
    }

    private void _service_OnAddNewData(string obj)
    {
        UpdateMatchTables();
        UpdateRecordToAnalyze();
    }

    private void gvEntityConfiguration_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
    {
        if (e.Column.FieldName != "")
        {
            var rw = gvEntityConfiguration.GetRow(e.RowHandle) as DataRowView;
            if (rw == null)
            {
                return;
            }

            var newValue = e.Value;
            if (e.Column.FieldName == "AI_Type" && (e.Value == null || string.IsNullOrWhiteSpace(e.Value.ToString())))
            {
                newValue = "";
            }

            ProcessLinkedSettings(_shiftFiledList, e.Column.FieldName, rw.Row);

            _service.SaveCleanSettings(rw.Row[0].ToString(), e.Column.FieldName, newValue);

            if (e.Column.FieldName != "AI_Label")
            {
                var position = gvEntityConfiguration.FocusedRowHandle;
                //gvEntityConfiguration.RefreshData();
                SetEntityResolutionSettings(_service.CurrentTable);

                gvEntityConfiguration.FocusedRowHandle = position;
            }
        }
    }


    private void gvMatchTables_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
    {
        var row = gvMatchTables.GetRow(e.RowHandle) as ImportedDataInfo;
        if (row != null)
        {
            if (e.Column.Name == "colIsSelected")
            {
                row.IsResolutionSelected = (bool)e.Value;
            }
            else if (e.Column.Name == "colIsMainTable")
            {
                row.IsErMainTable = (bool)e.Value;
            }
            _service.UpdateSelectedForMatchingTables(row.TableName);
        }

        UpdateRecordToAnalyze();
    }

    private void UpdateRecordToAnalyze()
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(UpdateRecordToAnalyze));
            return;
        }

        if (_service.TableList.Any(x => x.IsResolutionSelected))
        {
            var recordToAnalyze = _service.TableList.Where(x => x.IsResolutionSelected).Sum(x => x.RowCount);
            txtRecordToAnalyze.Text = recordToAnalyze.ToString("N0");
            txtRecordToAnalyze.BackColor = recordToAnalyze > _licenseService.GetErRecordLimit()
                ? Color.OrangeRed
                : DefaultBackColor;
            txtRecordToAnalyze.ForeColor = recordToAnalyze > _licenseService.GetErRecordLimit()
                ? Color.White
                : DefaultForeColor;
        }
    }

    private void ShowSystemInfo(object sender, EventArgs e)
    {
        var frm = WinPureUiDependencyResolver.Resolve<frmSystemInfo>();
        frm.ShowDialog();
    }

    private void btnAnalyze_Click(object sender, EventArgs e)
    {
        SetEntityResolutionSettings(_service.CurrentTable);
        var configurations = _service.VerifyFullEntityResolutionMap(_service.TableList.Where(x => x.TableName == _service.CurrentTable)).Where(x => x.Errors.Any());
        if (configurations.Any())
        {
            var errors = configurations.SelectMany(x => x.Errors);
            var errorMessage = string.Join(Environment.NewLine, errors.Select(x => x.Message));
            MessageBox.Show(errorMessage, Resources.MESSAGE_MESSAGEDIALOG_ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        else
        {
            if (MessageBox.Show(string.Format(Resources.UI_MESSAGE_PROCESSER.Replace(@"\r\n", Environment.NewLine), txtRecordToAnalyze.Text),
                    Resources.MESSAGE_MESSAGEDIALOG_SUCCESS_CAPTION, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var erRecordLimit = _licenseService.IsDemo
                    ? GlobalConstants.ErRecordsForDemoVersion
                    : _licenseService.GetErRecordLimit();
                var recordToAnalyze = _service.TableList.Where(x => x.IsResolutionSelected).Sum(x => x.RowCount);
                if (recordToAnalyze > erRecordLimit)
                {
                    if (MessageBox.Show(String.Format(Resources.CAPTION_AI_PROCESSANYWAY, erRecordLimit),
                            Resources.MESSAGE_QUESTION_CAPTION,
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    {
                        return;
                    }
                }
                _service.RunEntityResolutionAsync();
                SetEntityResolutionSettings(_service.CurrentTable);
            }
        }
        //pnlMapError.Visible = _erConfiguration != null && _erConfiguration.Errors.Any();
    }

    private void btnClean_Click(object sender, EventArgs e)
    {
        _service.CleanEntityResolutionSetting(_service.CurrentTable);
        SetEntityResolutionSettings(_service.CurrentTable);
    }

    private void btnMap_Click(object sender, EventArgs e)
    {
        _service.SetDefaultEntityResolutionMapping(_service.CurrentTable);
        SetEntityResolutionSettings(_service.CurrentTable);
    }

    private void gvMatchTables_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
    {
        var table = gvMatchTables.FocusedRowObject as ImportedDataInfo;
        if (table != null)
        {
            _service.SetSelectedTable(table.TableName);
        }
    }

    private void gvEntityConfiguration_RowStyle(object sender, RowStyleEventArgs e)
    {
        if (_erConfiguration == null || !_erConfiguration.Errors.Any())
        {
            return;
        }

        if (sender is GridView view && e.RowHandle >= 0)
        {
            var column = view.GetRowCellValue(e.RowHandle, "COLUMN_NAME").ToString();
            var mapType = view.GetRowCellValue(e.RowHandle, "AI_Type").ToString();
            if (_erConfiguration.Errors.Any(x =>
                    string.Equals(column, x.ColumnName, StringComparison.InvariantCultureIgnoreCase)
                    || string.Equals(mapType, x.TypeName, StringComparison.InvariantCultureIgnoreCase)))
            {
                e.Appearance.BackColor = Color.LightSalmon;

            }
        }
    }

    private void gvEntityConfiguration_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
    {
        try
        {
            // Draw the default header so theme is preserved
            e.Painter.DrawObject(e.Info);

            // only draw on the left-most header area (we'll draw on the entire header row left area)
            // compute an area relative to grid header: use e.Bounds of the column header; if this is the first visible column, draw left to it
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view == null) return;

            // find leftmost visible column header bounds by asking for the first visible column's header bounds via view.CalcColumnHeaderBounds? Not exposed.
            // As a simple approach draw icon inside the current e.Bounds when it is the first visible column (VisibleIndex == 0)
            if (e.Column != null && e.Column.VisibleIndex == 0)
            {
                const int iconSize = 16;
                var iconLeft = e.Bounds.Left - 22; // position slightly left of the first column header text
                var iconTop = e.Bounds.Top + Math.Max(0, (e.Bounds.Height - iconSize) / 2);
                var iconRect = new Rectangle(iconLeft, iconTop, iconSize, iconSize);

                // store rect in control coordinates for click detection
                _headerHelpRect = iconRect;

                // draw a simple circular help icon (blue circle with white '?') to avoid Svg rendering complexity
                using (var brush = new SolidBrush(Color.FromArgb(0, 120, 215))) // blue
                {
                    e.Graphics.FillEllipse(brush, iconRect);
                }
                using (var pen = new Pen(Color.FromArgb(0, 80, 143)))
                {
                    e.Graphics.DrawEllipse(pen, iconRect);
                }
                // draw question mark
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                using (var f = new Font("Segoe UI", 9, FontStyle.Bold))
                using (var textBrush = new SolidBrush(Color.White))
                {
                    e.Graphics.DrawString("?", f, textBrush, iconRect, sf);
                }
            }

            e.Handled = true;
        }
        catch
        {
            // ignore drawing errors
            _headerHelpRect = Rectangle.Empty;
        }
    }

    private void gvEntityConfiguration_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && !_headerHelpRect.IsEmpty)
        {
            var pt = new Point(e.X, e.Y);
            if (_headerHelpRect.Contains(pt))
            {
                try
                {
                    if (!string.IsNullOrEmpty(pictureEdit6?.Tag?.ToString()))
                    {
                        var chapter = (HelpPageChapter)Enum.Parse(typeof(HelpPageChapter), pictureEdit6.Tag.ToString());
                        UserManualHelper.OpenHelpPage(chapter);
                    }
                }
                catch
                {
                    // ignore
                }
            }
        }
    }

    private void OpenHelp_Click(object sender, EventArgs e)
    {
        if (sender is Control control)
        {
            var chapter = (HelpPageChapter)Enum.Parse(typeof(HelpPageChapter), control.Tag.ToString());
            UserManualHelper.OpenHelpPage(chapter);
        }
    }

    private void groupControl_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
    {
        if (sender is Control control)
        {
            var chapter = (HelpPageChapter)Enum.Parse(typeof(HelpPageChapter), control.Tag.ToString());
            UserManualHelper.OpenHelpPage(chapter);
        }
    }

    private void OnThemeChanged(object sender, EventArgs e)
    {
        if(_themeDetectionService.IsDarkTheme())
        {
            this.colType.AppearanceCell.BackColor = DXSkinColors.ForeColors.DisabledText;
            this.colLabel.AppearanceCell.BackColor = DXSkinColors.ForeColors.DisabledText;
        }
        else
        {
            this.colLabel.AppearanceCell.BackColor = Color.AliceBlue;
            this.colType.AppearanceCell.BackColor = Color.AliceBlue;
        }

        this.colType.AppearanceCell.Options.UseBackColor = true;
        this.colLabel.AppearanceCell.Options.UseBackColor = true;

    }

    private void splitDataContainer_Paint(object sender, PaintEventArgs e)
    {

    }
}