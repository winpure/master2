using WinPure.AddressVerification.Models;
using WinPure.AddressVerification.Properties;
using WinPure.Common.Abstractions;
using WinPure.Common.Models;
using WinPure.Configuration.Helper;

namespace WinPure.AddressVerification.DependencyInjection;

internal static partial class WinPureVerificationDependencyExtension
{
    private class AddressVerificationService : WinPureNotification, IAddressVerificationService
    {
        private readonly IConfigurationService _configurationService;
        private readonly IConnectionManager _connectionManager;
        private readonly IWpLogger _logger;
        private readonly IUsAddressVerificationOnlineService _usAddressVerificationOnline;

        public AddressVerificationService(IConfigurationService configurationService,
            IConnectionManager connectionManager,
            IWpLogger logger,
            IUsAddressVerificationOnlineService usAddressVerificationOnline)
        {
            _configurationService = configurationService;
            _connectionManager = connectionManager;
            _logger = logger;
            _usAddressVerificationOnline = usAddressVerificationOnline;
        }

        public AddressVerificationReport VerifyAddresses(AddressVerificationSettings verificationSettings,
            ImportedDataInfo importedDataInfo, DataTable tblOptions, int recordsForVerification,
            CancellationToken cancellationToken)
        {
            var usSettings = verificationSettings as UsAddressVerificationSettings;

            foreach (DataRow rw in tblOptions.Rows)
            {
                if (rw["AF_Address"].ToString() == "1")
                {
                    verificationSettings.AddressColumns.Add(rw["COLUMN_NAME"].ToString());
                }

                if (rw["AF_State"].ToString() == "1")
                {
                    verificationSettings.StateColumns.Add(rw["COLUMN_NAME"].ToString());
                }

                if (rw["AF_City"].ToString() == "1")
                {
                    verificationSettings.LocalityColumns.Add(rw["COLUMN_NAME"].ToString());
                }

                if (rw["AF_Zip"].ToString() == "1")
                {
                    verificationSettings.PostalCodeColumns.Add(rw["COLUMN_NAME"].ToString());
                }

                if (usSettings != null && usSettings.ReverseGeocode)
                {
                    if (rw["RG_Latitude"].ToString() == "1")
                    {
                        usSettings.LatitudeColumn = rw["COLUMN_NAME"].ToString();
                    }

                    if (rw["RG_Longitude"].ToString() == "1")
                    {
                        usSettings.LongitudeColumn = rw["COLUMN_NAME"].ToString();
                    }
                }
            }

            if (!verificationSettings.AddressColumns.Any()
                && usSettings != null
                && !usSettings.ReverseGeocode)
            {
                throw new Exception(Resources.EXCEPTION_ADDRVERIFY_COLUMNS);
            }

            if (usSettings != null
                && usSettings.ReverseGeocode
                && (string.IsNullOrWhiteSpace(usSettings.LatitudeColumn) ||
                    string.IsNullOrWhiteSpace(usSettings.LongitudeColumn)))
            {
                throw new Exception(Resources.EXCEPTION_ADDREVERSEGEOCODE_COLUMNS);
            }

            AddressVerificationReport res = null;

            _connectionManager.CheckConnectionState();

            var table = SqLiteHelper.ExecuteQuery(SqLiteHelper.GetSelectQuery(importedDataInfo.TableName),
                _connectionManager.Connection);

            if (!verificationSettings.SelectedRows.Any() ||
                verificationSettings.SelectedRows.Count == table.Rows.Count)
            {
                table.Columns.Add(
                    new DataColumn(WinPureColumnNamesHelper.WPCOLUMN_CHECK_ADDRESS_VERIFICATION, typeof(bool))
                        { DefaultValue = true });
            }
            else
            {
                table.Columns.Add(
                    new DataColumn(WinPureColumnNamesHelper.WPCOLUMN_CHECK_ADDRESS_VERIFICATION, typeof(bool))
                        { DefaultValue = false });
                foreach (var x in table.AsEnumerable().Join(verificationSettings.SelectedRows,
                             t => t.Field<long>(WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY), s => s,
                             (t, s) => new { t, s }))
                {
                    x.t[WinPureColumnNamesHelper.WPCOLUMN_CHECK_ADDRESS_VERIFICATION] = true;
                }
            }

            if (usSettings != null)
            {
                verificationSettings.PathToDb = _configurationService.Configuration.UsAddressVerificationDataFolder;
                res = VerifyUsAddresses(table, usSettings, cancellationToken, recordsForVerification).Result;
            }

            if (res != null)
            {
                _logger.Information(
                    $"Verification time: {res.VerifyTime.TotalMinutes} min. Used credits {res.UsedCredits}");
                table.Columns.Remove(WinPureColumnNamesHelper.WPCOLUMN_CHECK_ADDRESS_VERIFICATION);
                SqLiteHelper.ExecuteNonQuery(SqLiteHelper.GetDropTableQuery(importedDataInfo.TableName),
                    _connectionManager.Connection);
                SqLiteHelper.SaveDataToDb(_connectionManager.Connection, table, importedDataInfo.TableName,
                    _logger);
                SqLiteHelper.UpdateTableColumnList(_connectionManager.Connection, importedDataInfo, table, true);
            }

            return res;
        }

        public async Task<AddressVerificationReport> VerifyUsAddresses(DataTable data,
            UsAddressVerificationSettings settings, CancellationToken cToken, int recordsForVerification)
        {
            if (settings.IsOnlineVerification)
            {
                return await _usAddressVerificationOnline.VerifyAddresses(data, settings, cToken, NotifyProgress,
                    recordsForVerification);
            }

            throw new WinPureAddressVerificationException("Verification type not supported");
        }
    }
}