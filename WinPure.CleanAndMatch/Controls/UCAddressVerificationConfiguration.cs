using DevExpress.XtraTab;
using WinPure.AddressVerification.Models;

namespace WinPure.CleanAndMatch.Controls;

public partial class UCAddressVerificationConfiguration : UCDataViewBase
{
    private readonly List<string> _shiftFiledList = new List<string> { "AF_Address", "AF_Zip", "AF_City", "AF_State", "RG_Latitude", "RG_Longitude" };
    private ILicenseService _licenseService;
    private IConfigurationService _configurationService;

    public event Action OnNavigateToFullReport;

    public UCAddressVerificationConfiguration()
    {
        InitializeComponent();
        Localization();
        splitDataContainer.SplitterPosition = 320;
    }

    private void Localization()
    {
        colAvFieldName.Caption = Resources.UI_UCMAINCLEANNEWFORM_FIELDNAME;
        colAvAddress.Caption = Resources.UI_CAPTION_ADDRESS;
        cbCass.Text = Resources.UI_ADDRVERIFICATION_CAPTION_CASSFIELDS;
        cbSerp.Text = Resources.UI_ADDRVERIFICATION_CAPTION_SERPFIELDS;
        cbAmas.Text = Resources.UI_ADDRVERIFICATION_CAPTION_AMASFIELDS;
        cbGeoCode.Text = Resources.UI_ADDRVERIFICATION_CAPTION_GEOCODE;
        linkLabel_Report.Text = Resources.UI_FULL_REPORT;
        btnAddressVerificationStart.Text = Resources.UI_ADDRVERIFICATION_CAPTION_STARTVERIFICATION;
        labelControl1.Text = Resources.UI_CAPTION_TOTALRECORDS;
        labelControl2.Text = Resources.UI_CAPTION_ADDRIDENT;
        labelControl3.Text = Resources.UI_CAPTION_GEOCODES;
        btnExportMatchResult.Text = Resources.UI_EXPORT;
        barButtonExportToCsv.Caption = Resources.UI_DATASOURCE_CSV;
        barButtonExportToXls.Caption = Resources.UI_DATASOURCE_EXCEL;
        barButtonExportToAccess.Caption = Resources.UI_DATASOURCE_ACCESS;
        barButtonExportToSqlServer.Caption = Resources.UI_DATASOURCE_SQLSERVER;
        barButtonExportToMySqlServer.Caption = Resources.UI_DATASOURCE_MYSQL;
        barButtonExportToOracle.Caption = Resources.UI_DATASOURCE_ORACLE;
        barButtonExportToXml.Caption = Resources.UI_DATASOURCE_XML;
        barButtonExportToJson.Caption = Resources.UI_DATASOURCE_JSON;
        barButtonExportToAzure.Caption = Resources.UI_DATASOURCE_AZURE;
        barButtonExportToPostgres.Caption = Resources.UI_DATASOURCE_POSTGRESQL;
        barButtonExportToSQLite.Caption = Resources.UI_DATASOURCE_SQLITE;
        grOptions.Text = Resources.UI_ADDRVERIFICATION_CAPTION_OPTIONS;
        groupControl3.Text = Resources.UI_ADDRVERIFICATION_CAPTION_RESULT;
        groupControl2.Text = Resources.UI_ADDRVERIFICATION_CAPTION_OUTPUT;
        groupControl4.Text = Resources.UI_ADDRVERIFICATION_CAPTION_HELP;
        groupControl1.Text = Resources.UI_ADDRVERIFICATION_CAPTION_COUNTRY;
        barMenuFiles.Caption = Resources.UI_DATASOURCE_FILES;
        barMenuDatabase.Caption = Resources.UI_DATASOURCE_DATABASES;
        barMenuCrm.Caption = Resources.UI_DATASOURCE_CRM;
    }

    public override void Initialize(bool useDataMenu, bool useRowSelection = false)
    {
        base.Initialize(useDataMenu, useRowSelection);
        tcData.ClosePageButtonShowMode = ClosePageButtonShowMode.Default;
        _configurationService = WinPureUiDependencyResolver.Resolve<IConfigurationService>();
        var configuration = _configurationService.Configuration;
        cbUseOnlineVerification.Checked = false; //Settings.Default.UseOnlineAddressVerification;

        if (Program.CurrentProgramVersion == ProgramType.CamLte ||
            Program.CurrentProgramVersion == ProgramType.CamFree ||
            Program.CurrentProgramVersion == ProgramType.CamBiz ||
            //Program.CurrentProgramVersion == ProgramType.CamEntLite ||
            Program.CurrentProgramVersion == ProgramType.CamEnt)
        {
            panelTestFile.Visible = true;
            cbCass.Enabled = false;
            cbAmas.Enabled = false;
            cbSerp.Enabled = false;
            cbGeoCode.Enabled = false;
            cbReverseGeoCodes.Enabled = false;
        }

        _licenseService = WinPureUiDependencyResolver.Resolve<ILicenseService>();

        if (Program.CurrentProgramVersion == ProgramType.CamEntAd ||
            Program.CurrentProgramVersion == ProgramType.CamBiz)
        {
            pnlUsAddressOptions.Visible = true;
            pnlUkAddressOptions.Visible = false;

            SetControlAvailability();
            _licenseService.LicenseLoaded += _licenseService_LicenseLoaded;

            tcData.ClosePageButtonShowMode = ClosePageButtonShowMode.Default;
            cbCass.Checked = configuration.cbCass;
            cbAmas.Checked = configuration.cbAmas;
            cbSerp.Checked = configuration.cbSerp;
            cbGeoCode.Checked = configuration.cbGeoCode;
            cbVerification.Checked = configuration.cbVerification;
            cbReverseGeoCodes.Checked = configuration.cbReverseGeocode;
            txtOnlineLicenseKey.Text = configuration.AddressVerificationLicense;

            CheckAddressVerificationOptions();

            SetVerificationCredits();

            txtOnlineLicenseKey.TextChanged += txtOnlineLicenseKey_TextChanged;
            cbCass.CheckedChanged += VerificationOption_CheckedChanged;
            cbVerification.CheckedChanged += VerificationOption_CheckedChanged;
            cbAmas.CheckedChanged += VerificationOption_CheckedChanged;
            cbSerp.CheckedChanged += VerificationOption_CheckedChanged;
            cbGeoCode.CheckedChanged += VerificationOption_CheckedChanged;
            cbReverseGeoCodes.CheckedChanged += VerificationOption_CheckedChanged;

            cbCountry.EditValue = configuration.AddressVerificationCountry;
            cbCountry.Refresh();
            cbCountry.SelectedIndexChanged += CbCountry_SelectedIndexChanged;
        }
        else
        {
            pnlUsAddressOptions.Visible = true;
            pnlUkAddressOptions.Visible = false;
            cbReverseGeoCodes.Enabled = false;
            cbVerification.Checked = configuration.cbVerification;
            cbVerification.CheckedChanged += VerificationOption_CheckedChanged;
            cbCountry.EditValue = configuration.AddressVerificationCountry;
            cbCountry.Refresh();
            cbCountry.SelectedIndexChanged += CbCountry_SelectedIndexChanged;
        }

        _service.OnCurrentTableChanged += _service_OnCurrentTableChanged;
        _service.OnAddressVerificationReady += _service_AddressVerificationReady;
        _service.OnTableDelete += _service_OnTableDelete;
        _service.OnTableDataUpdateComplete += _service_OnTableDataUpdateComplete;
        _service.OnRefreshData += _service_OnRefreshData;
    }

    private void SetControlAvailability()
    {
        if (_licenseService.IsDemo)
        {
            panelTestFile.Visible = true;
            cbCass.Enabled = false;
            cbAmas.Enabled = false;
            cbSerp.Enabled = false;
            cbGeoCode.Enabled = false;
            cbReverseGeoCodes.Enabled = false;
        }
        else
        {
            panelTestFile.Visible = false;
            cbCass.Enabled = true;
            cbAmas.Enabled = true;
            cbSerp.Enabled = true;
            cbGeoCode.Enabled = true;
        }
    }

    private void SetVerificationCredits()
    {
        txtAvCredits.Text = _configurationService.Configuration.AddressVerificationCredits.ToString();
    }

    private void UpdateStatisticInfo(string tableName)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { UpdateStatisticInfo(tableName); }));
            return;
        }

        SetVerificationCredits();
        var res = _service.GetAddressVerificationResult(tableName);
        var verificationStatistic = res?.CommonData;
        if (verificationStatistic != null)
        {
            txtTotalRecords.Text = verificationStatistic.TotalRecords.ToString();
            txtAddrCount.Text = verificationStatistic.AddressSuccess.ToString();
            txtAddrPrct.Text = verificationStatistic.AddressSuccessPercent.ToString("P2");
            txtGeoCount.Text = verificationStatistic.GeoCodeSuccess.ToString();
            txtGeoPrcn.Text = verificationStatistic.GeoCodeSuccessPercent.ToString("P2");
            var totalTime = res.VerifyTime.ToString("g");
            txtVerificationTime.Text = totalTime.Length <= 8 ? "0" : totalTime.Substring(0, totalTime.Replace(",",".").IndexOf("."));
        }
        else
        {
            ClearControls();
        }
    }

    private void ClearControls()
    {
        txtTotalRecords.Text = "";
        txtVerificationTime.Text = "";
        txtAddrCount.Text = "";
        txtAddrPrct.Text = "";
        txtGeoCount.Text = "";
        txtGeoPrcn.Text = "";
    }

    private void SetVerificationSettings(DataTable ds)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(delegate { SetVerificationSettings(ds); }));
            return;
        }

        gridVerificationSettings.DataSource = null;
        gridVerificationSettings.DataSource = ds;
        gridVerificationSettings.Refresh();
    }

    private void CheckAddressVerificationOptions()
    {
        if (cbReverseGeoCodes.Checked)
        {
            cbAmas.Enabled = cbGeoCode.Enabled = cbSerp.Enabled = cbVerification.Enabled = false;
            colAvAddress.Visible = colAvLocality.Visible = colAvPostalCode.Visible = colAvState.Visible = cbCass.Enabled = false;
            colRgLatitude.Visible = colRgLongitude.Visible = true;
        }
        else
        {
            cbAmas.Enabled = cbGeoCode.Enabled = cbSerp.Enabled = cbVerification.Enabled = cbCass.Enabled = true;
            colRgLatitude.Visible = colRgLongitude.Visible = false;
            colAvAddress.Visible = colAvLocality.Visible = colAvPostalCode.Visible = colAvState.Visible = true;

            bool isEnabled = true;
            if (cbUseOnlineVerification.Checked)
            {
                isEnabled = cbVerification.Checked;
            }

            cbAmas.Enabled = cbGeoCode.Enabled = cbSerp.Enabled = isEnabled && !_licenseService.IsDemo;
        }
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

    private void CbCountry_SelectedIndexChanged(object sender, EventArgs e)
    {
        _configurationService.Configuration.AddressVerificationCountry = cbCountry.EditValue?.ToString();
        _configurationService.SaveConfiguration();
    }
        
    private void _licenseService_LicenseLoaded()
    {
        SetControlAvailability();
    }
        
    private void _service_OnRefreshData(ImportedDataInfo tableInfo)
    {
        SetVerificationSettings(_service.GetDataTableAddressVerificationSetting(tableInfo.TableName));
    }

    private void _service_OnCurrentTableChanged(string tableName)
    {
        SetVerificationSettings(_service.GetDataTableAddressVerificationSetting(tableName));
        UpdateStatisticInfo(tableName);
    }

    private void _service_OnTableDataUpdateComplete(string tableName)
    {
        SetVerificationSettings(_service.GetDataTableAddressVerificationSetting(tableName));
    }

    private void _service_OnTableDelete(ImportedDataInfo obj)
    {
        if (!_service.IsAnyTable)
        {
            SetVerificationSettings(null);
        }
    }

    private void _service_AddressVerificationReady(string tableName, bool activateReport)
    {
        var res = _service.GetAddressVerificationResult(tableName);
        _configurationService.Configuration.AddressVerificationCredits = _configurationService.Configuration.AddressVerificationCredits - res.UsedCredits;
        _configurationService.SaveConfiguration();

        UpdateStatisticInfo(tableName);
    }

    private void OpenHelp_Click(object sender, EventArgs e)
    {
        if (sender is Control control)
        {
            var chapter = (HelpPageChapter)Enum.Parse(typeof(HelpPageChapter), control.Tag.ToString());
            UserManualHelper.OpenHelpPage(chapter);
        }
    }

    private void hyperlinkLabelControl1_Click(object sender, EventArgs e)
    {
        UserManualHelper.OpenHelpPage(HelpPageChapter.AVDemo);
    }

    private void cbUseOnlineVerification_CheckedChanged(object sender, EventArgs e)
    {
        txtOnlineLicenseKey.Enabled = cbUseOnlineVerification.Checked;
        _configurationService.Configuration.UseOnlineAddressVerification = cbUseOnlineVerification.Checked;
        _configurationService.SaveConfiguration();
        CheckAddressVerificationOptions();
    }

    private void txtOnlineLicenseKey_TextChanged(object sender, EventArgs e)
    {
        _configurationService.Configuration.AddressVerificationLicense = txtOnlineLicenseKey.Text;
        _configurationService.SaveConfiguration();
    }

    private void VerificationOption_CheckedChanged(object sender, EventArgs e)
    {
        if (sender is CheckEdit ctrl)
        {
            switch (ctrl.Name)
            {
                case "cbAmas": _configurationService.Configuration.cbAmas = ctrl.Checked; break;
                case "cbCass": _configurationService.Configuration.cbCass = ctrl.Checked; break;
                case "cbGeoCode": _configurationService.Configuration.cbGeoCode = ctrl.Checked; break;
                case "cbSerp": _configurationService.Configuration.cbSerp = ctrl.Checked; break;
                case "cbVerification":
                    _configurationService.Configuration.cbVerification = ctrl.Checked;
                    CheckAddressVerificationOptions();
                    break;
                case "cbReverseGeoCodes":
                    _configurationService.Configuration.cbReverseGeocode = ctrl.Checked;
                    CheckAddressVerificationOptions();
                    break;
            }
            _configurationService.SaveConfiguration();
        }
    }

    private void linkLabel_Report_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        OnNavigateToFullReport?.Invoke();
    }

    private void gvVerificationSettings_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
    {
        if (e.Column.FieldName != "")
        {
            var rw = gvVerificationSettings.GetRow(e.RowHandle) as DataRowView;
            if (rw == null)
            {
                return;
            }
            ProcessLinkedSettings(_shiftFiledList, e.Column.FieldName, rw.Row);

            _service.SaveCleanSettings(rw.Row[0].ToString(), e.Column.FieldName, e.Value);

            gvVerificationSettings.RefreshData();
        }
    }

    private void btnAddressVerificationStart_Click(object sender, EventArgs e)
    {
        if (tcData.TabPages.Count > 0)
        {
            var verificationSettings = new UsAddressVerificationSettings
            {
                CassField = cbCass.Checked,
                AmasField = cbAmas.Checked,
                SerpField = cbSerp.Checked,
                Verification = cbVerification.Checked,
                Country = cbCountry.Text,
                GeoTag = cbGeoCode.Checked,
                IsOnlineVerification = cbUseOnlineVerification.Checked,
                LicenseKey = txtOnlineLicenseKey.Text,
                AvailableCredits = _configurationService.Configuration.AddressVerificationCredits,
                ReverseGeocode = cbReverseGeoCodes.Checked
            };

            var tp = tcData.SelectedTabPage;
            if (tp?.Tag != null)
            {
                if (tp.Controls.Find("wpDataGrid", true).FirstOrDefault() is DataControl dataControl)
                {
                    verificationSettings.SelectedRows = dataControl.GetSelectedRows();
                }
            }

            _service.StartAddressVerification(tcData.SelectedTabPage?.Tag?.ToString(), verificationSettings);
        }
    }

    private void barButtonExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
        var srv = WinPureUiDependencyResolver.Resolve<IImportExportService>();
        if (Enum.TryParse(e.Item.Tag.ToString(), out ExternalSourceTypes sourceType))
        {
            srv.Export(sourceType);
        }
    }
}