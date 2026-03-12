using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using WinPure.AddressVerification.Models;
using WinPure.AddressVerification.Services;
using WinPure.API.DependencyInjection;
using WinPure.Cleansing.Models;
using WinPure.Cleansing.Services;
using WinPure.Common.Constants;
using WinPure.Common.Exceptions;
using WinPure.Common.Logger;
using WinPure.Common.Models;
using WinPure.Configuration.Helper;
using WinPure.Configuration.Service;
using WinPure.Licensing.Enums;
using WinPure.Licensing.Services;
using WinPure.Matching.Algorithm;
using WinPure.Matching.Enums;
using WinPure.Matching.Models;
using WinPure.Matching.Models.Support;
using WinPure.Matching.Services;

namespace WinPure.APICore
{
    public class WinPureApi
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILicenseService _licenseService;
        private readonly IWpLogger _logger;

        /// <summary>
        /// Indicate of the progress of the current operation. 
        /// Description of action
        /// Total progress in %
        /// Current step (for matching only)
        /// Total steps (for matching only)
        /// </summary>
        public event Action<string, int> OnProgress;


        public WinPureApi()
        {
            _serviceProvider = WinPureApiDependencyResolver.Instance.ServiceProvider;
            _logger = WinPureApiDependencyResolver.Resolve<IWpLogger>();
            _licenseService = WinPureApiDependencyResolver.Resolve<ILicenseService>();
            _licenseService.InitiateApi();            
            var _configurationService = WinPureApiDependencyResolver.Resolve<IConfigurationService>();
            _configurationService.Initiate(Common.Enums.ProgramType.Api);
            DatabaseInitiator.InitiateDatabase(_serviceProvider);
        }

        #region License

        /// <summary>
        /// Get WinPure registration code
        /// </summary>
        /// <returns></returns>
        public string GetRegistrationCode()
        {
            return _licenseService.GetLocalRegistrationKey();
        }

        /// <summary>
        /// Register WinPure API with license file. License file can be received from WinPure Ltd.
        /// </summary>
        /// <param name="licenseFile">*.license file with valid license</param>
        /// <returns></returns>
        public LicenseState Register(string licenseFile)
        {
            return _licenseService.Register(licenseFile);
        }

        /// <summary>
        /// Get current license state (Demo, Expired, Valid etc.)
        /// </summary>
        /// <returns></returns>
        public LicenseState CheckLicenseState()
        {
            return _licenseService.GetLicenseState();
        }


        /// <summary>
        /// Get full license information
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetFullLicenseInfo()
        {
            return _licenseService.GetFullLicenseInfo();
        }
        #endregion

        #region ADDRESS VERIFICATION
        /// <summary>
        /// In-memory OR online USA address verification. 
        /// </summary>
        /// <param name="data">Table with source data</param>
        /// <param name="settings">Verification settings.</param>
        /// <param name="cancellationToken">cancellation token to cancel the  verification</param>
        /// <returns>Address verification report</returns>
        public async Task<AddressVerificationReport> VerifyUsAddressesAsync(DataTable data, UsAddressVerificationSettings settings, CancellationToken cancellationToken)
        {
            IUsAddressVerificationService addressVerificationService;
            if (settings.IsOnlineVerification)
            {
                addressVerificationService = _serviceProvider.GetService(typeof(IUsAddressVerificationOnlineService)) as IUsAddressVerificationOnlineService;
            }
            else
            {
                addressVerificationService = _serviceProvider.GetService(typeof(IUsAddressVerificationService)) as IUsAddressVerificationService;
            }

            return await addressVerificationService.VerifyAddresses(data, settings, cancellationToken, RaiseOnProgressUpdate, RecordCountForAddressVerification());
        }

        /// <summary>
        /// In-memory USA address verification. 
        /// </summary>
        /// <param name="data">Table with source data</param>
        /// <param name="settings">Verification settings</param>
        /// <param name="cancellationToken">cancellation token to cancel the verification</param>
        /// <returns>Address verification report</returns>
        public AddressVerificationReport VerifyUsAddresses(DataTable data, UsAddressVerificationSettings settings, CancellationToken cancellationToken)
        {
            return VerifyUsAddressesAsync(data, settings, cancellationToken).Result;
        }

        private int RecordCountForAddressVerification()
        {
            return _licenseService.IsDemo ? GlobalConstants.AddressVerificationRecordsForDemoVersion : -1;
        }

        #endregion

        #region CLEANSING

        /// <summary>
        /// Perform clean operation under the given table with data
        /// </summary>
        /// <param name="settings">Clean settings</param>
        /// <param name="data">DataTable with data to clean</param>
        /// <param name="cancellationToken">Cancellation token to cancel the cleansing</param>
        public void CleanTable(DataTable data, WinPureCleanSettings settings, CancellationToken cancellationToken)
        {
            var cleansingService = _serviceProvider.GetService(typeof(ICleansingService)) as ICleansingService;
            cleansingService?.CleanTable(data, settings, cancellationToken);
        }

        /// <summary>
        /// Calculate statistic for the given table. 
        /// </summary>
        /// <param name="data">DataTable with data, for which statistic should be calculated</param>
        /// <param name="dataFields">List of field from Data table with their patterns if exists. Can be empty</param>
        /// <param name="cancellationToken">Cancellation token to cancel the statistic calculation</param>
        /// <returns>Table with statistic</returns>
        public DataTable CalculateStatistic(DataTable data, List<DataField> dataFields, CancellationToken cancellationToken)
        {
            var cleansingService = _serviceProvider.GetService(typeof(ICleansingService)) as ICleansingService;
            return cleansingService?.CalculateStatistic(data, dataFields, cancellationToken);
        }

        #endregion

        #region MATCHING

        /// <summary>
        /// Match data
        /// </summary>
        /// <param name="tables">Input tables with data</param>
        /// <param name="parameter">Matching parameters</param>
        /// <param name="fieldMap">Fields from original table(s), which should be included to the result. If there are multiple tables in source list, 
        /// then that parameter also define correspondence between fields in different tables</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Table with result of matching</returns>
        /// <exception cref="WinPureAPIWrongParametersException"></exception>
        /// <exception cref="WinPureAPINoTableException"></exception>
        /// <exception cref="WinPureAPINoFieldException"></exception>
        /// <exception cref="WinPureAPIWrongConditionException"></exception>
        public DataTable MatchData(List<TableParameter> tables, MatchParameter parameter, List<FieldMapping> fieldMap, CancellationToken cancellationToken)
        {
            var matchService = _serviceProvider.GetService(typeof(IMatchService)) as IMatchService;
            return matchService?.MatchData(tables, parameter, fieldMap, cancellationToken, RaiseOnProgressUpdate, RecordCountForMatching());
        }

        /// <summary>
        /// Search duplicates of value(s) in the given data
        /// </summary>
        /// <param name="table">Table with data where we search</param>
        /// <param name="parameter">Search parameter which define what and in which field should be searched</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Table with result of searching</returns>
        public DataTable SearchData(TableParameter table, SearchParameter parameter, CancellationToken cancellationToken)
        {
            var matchService = _serviceProvider.GetService(typeof(ISearchService)) as ISearchService;
            return matchService?.SearchData(table, parameter, cancellationToken, RaiseOnProgressUpdate, RecordCountForMatching());
        }

        private int RecordCountForMatching()
        {
            return _licenseService.IsDemo ? GlobalConstants.MatchRecordsForDemoVersion : -1;
        }
        #endregion

        #region Match support functions

        /// <summary>
        /// Define master record according to selected parameters
        /// </summary>
        /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
        /// <param name="lastMatchingParameters">matching parameters</param>
        /// <param name="settings">Master record settings</param>
        /// <returns>True if master record definition was successed</returns>
        public bool DefineMasterRecord(DataTable matchResult, MatchParameter lastMatchingParameters, MasterRecordSettings settings)
        {
            var dataNormalizationService = _serviceProvider.GetService(typeof(IDataNormalizationService)) as IDataNormalizationService;
            return dataNormalizationService?.DefineMasterRecord(matchResult, lastMatchingParameters, settings) ?? false;
        }

        /// <summary>
        /// Merging of match result.
        /// </summary>
        /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
        /// <param name="mergeSettings">Merge settings for column from match result</param>
        /// <param name="valueSeparator">character or string to insert between values from merged rows</param>
        /// <returns></returns>
        public DataTable MergeMatchResult(DataTable matchResult, List<MergeMatchResultSetting> mergeSettings, string valueSeparator)
        {
            var dataNormalizationService = _serviceProvider.GetService(typeof(IDataNormalizationService)) as IDataNormalizationService;
            return dataNormalizationService?.MergeMatchResult(matchResult, mergeSettings, valueSeparator, (s, i) => RaiseOnProgressUpdate(s, i));
        }

        /// <summary>
        /// Delete rows from the match result according to options.
        /// </summary>
        /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
        /// <param name="setting">Delete setting</param>
        /// <returns>New data table with match result. Existing match result table should be overwriting</returns>
        public DataTable DeleteMergeMatchResult(DataTable matchResult, DeleteFromMatchResultSetting setting)
        {
            var dataNormalizationService = _serviceProvider.GetService(typeof(IDataNormalizationService)) as IDataNormalizationService;
            return dataNormalizationService?.DeleteMergeMatchResult(matchResult, setting);
        }

        /// <summary>
        /// Update the result according to specified mergeSettings. 
        /// </summary>
        /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
        /// <param name="updateSettings">Update settings for columns from match result</param>
        public void UpdateMatchResult(DataTable matchResult, List<UpdateMatchResultSetting> updateSettings)
        {
            var dataNormalizationService = _serviceProvider.GetService(typeof(IDataNormalizationService)) as IDataNormalizationService;
            dataNormalizationService?.UpdateMatchResult(matchResult, updateSettings, (s, i) => RaiseOnProgressUpdate(s, i));
        }

        /// <summary>
        /// Remove not duplicate rows from match result.
        /// </summary>
        /// <param name="matchResult">Datatable with matching result. It should be same table that was returned by MatchData function.</param>
        public void RemoveNotDuplicateRecords(DataTable matchResult)
        {
            var dataNormalizationService = _serviceProvider.GetService(typeof(IDataNormalizationService)) as IDataNormalizationService;
            dataNormalizationService?.RemoveNotDuplicateRecords(matchResult);
        }

        public double MatchTwoStrings(string source1, string source2, MatchAlgorithm algorithm)
        {
            var fuzzyComparing = FuzzyFactory.GetFuzzyAlgorithm(algorithm);
            return fuzzyComparing.CompareString(source1, source2);
        }

        #endregion

        /// <summary>
        /// Call progress event
        /// </summary>
        /// <param name="description"></param>
        /// <param name="progress"></param>
        /// <param name="currentStep"></param>
        /// <param name="totalSteps"></param>
        private void RaiseOnProgressUpdate(string description, int progress)
        {
            OnProgress?.Invoke(description, progress);
        }
    }
}