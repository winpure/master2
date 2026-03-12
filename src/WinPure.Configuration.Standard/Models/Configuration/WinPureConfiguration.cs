using Newtonsoft.Json;

namespace WinPure.Configuration.Models.Configuration
{
    [Serializable]
    internal class WinPureConfiguration
    {
        public DatabaseSetting SystemDatabase { get; set; }
        public DatabaseSetting ProjectDatabase { get; set; }
        public DatabaseSetting LogDatabase { get; set; }
        public LicenseSettings License { get; set; }
        public string SkinTheme { get; set; } = "WXI Compact";
        public bool ShowStartScreen { get; set; } = true;
        public string SampleFolderPath { get; set; }
        public string SampleProjectName { get; set; } = "Sample Project.wppj";
        public string LogPath { get; set; }
        public string NewProjectSamplePath { get; set; }
        public bool FirstImport { get; set; } = true;
        public bool FirstWM { get; set; } = true;
        public bool FirstNewProject { get; set; } = true;
        public bool ShowNullValues { get; set; } = false;
        public bool CheckForUpdates { get; set; } = false;
        public bool ShowStartupForm { get; set; } = true;

        public bool EnableAutomation { get; set; } = false;
        public bool EnableAuditLogs { get; set; } = false;
        public string AuditLogReportPath { get; set; }

        public string CleanMergeSeparator { get; set; } = ",";
        public string CleanSplitSeparator { get; set; } = " ";
        public string WordManagerSamplePath { get; set; }
        public int PreviewRowCount { get; set; } = 200;
        public bool AllowUndo { get; set; } = true;
        public bool UseCleansingAi { get; set; } = false;

        public string AddressVerificationCountry { get; set; } = "United States";
        public string AddressVerificationLicense { get; set; } //online
        public int AddressVerificationCredits { get; set; }
        public string AddressReportPath { get; set; }
        public string UsAddressVerificationDataFolder { get; set; }
        public EntityResolutionSetting ErSettings { get; set; }

        public bool AutoSetMasterRecord { get; set; } = true;
        public bool AutoSetAiMasterRecord { get; set; } = true;

        public int AutoSetMasterRecordType { get; set; } = 0;

        /// <summary>
        /// This property only for internal usage
        /// Show if Libpostal for address split was already initialized or not
        /// </summary>
        [JsonIgnore]
        public bool LibpostalInitialized { get; set; } = false;


        public bool cbCass { get; set; }
        public bool cbAmas { get; set; }
        public bool cbSerp { get; set; }
        public bool cbGeoCode { get; set; }
        public bool cbVerification { get; set; }
        public bool cbReverseGeocode { get; set; }
        public bool ShowAutoFilterRow { get; set; }
        public bool ShowSystemFields { get; set; } = true;
        public bool UseOnlineAddressVerification { get; set; }

        public string MatchReportPath { get; set; }
        public string FuzzyAlgorithm { get; set; } = "WinPureFuzzy";
        public decimal DefaultFuzzyLevel { get; set; } = 95m;
        public bool IncludeEmptyValues { get; set; } = false;
        public bool IncludeNullValues { get; set; } = false;
        public bool UseMixedRules { get; set; } = false;
        public bool GenerateMatchReport { get; set; } = true;

        public void LoadConfiguration(WinPureConfiguration configuration)
        {
            SystemDatabase = configuration.SystemDatabase;
            ProjectDatabase = configuration.ProjectDatabase;
            License = configuration.License;
            SkinTheme = configuration.SkinTheme;
            ShowStartScreen = configuration.ShowStartScreen;
            SampleFolderPath = configuration.SampleFolderPath;
            SampleProjectName = configuration.SampleProjectName;
            NewProjectSamplePath = configuration.NewProjectSamplePath;
            LogPath = configuration.LogPath;
            FirstImport = configuration.FirstImport;
            FirstWM = configuration.FirstWM;
            FirstNewProject = configuration.FirstNewProject;
            ShowStartupForm = configuration.ShowStartupForm;
            UseCleansingAi = configuration.UseCleansingAi;
            AllowUndo = configuration.AllowUndo;

            EnableAutomation = configuration.EnableAutomation;
            EnableAuditLogs = configuration.EnableAuditLogs;
            AuditLogReportPath = configuration.AuditLogReportPath;

            CleanMergeSeparator = configuration.CleanMergeSeparator;
            CleanSplitSeparator = configuration.CleanSplitSeparator;
            WordManagerSamplePath = configuration.WordManagerSamplePath;
            PreviewRowCount = configuration.PreviewRowCount;

            UsAddressVerificationDataFolder = configuration.UsAddressVerificationDataFolder;
            AddressVerificationCountry = configuration.AddressVerificationCountry;
            AddressVerificationLicense = configuration.AddressVerificationLicense;
            AddressVerificationCredits = configuration.AddressVerificationCredits;
            AddressReportPath = configuration.AddressReportPath;

            cbCass = configuration.cbCass;
            cbAmas = configuration.cbAmas;
            cbSerp = configuration.cbSerp;
            cbGeoCode = configuration.cbGeoCode;
            cbVerification = configuration.cbVerification;
            ShowAutoFilterRow = configuration.ShowAutoFilterRow;
            ShowSystemFields = configuration.ShowSystemFields;
            UseOnlineAddressVerification = configuration.UseOnlineAddressVerification;

            MatchReportPath = configuration.MatchReportPath;
            FuzzyAlgorithm = configuration.FuzzyAlgorithm;
            DefaultFuzzyLevel = configuration.DefaultFuzzyLevel;
            IncludeEmptyValues = configuration.IncludeEmptyValues;
            IncludeNullValues = configuration.IncludeNullValues;
            GenerateMatchReport = configuration.GenerateMatchReport;

            AutoSetMasterRecord = configuration.AutoSetMasterRecord;
            AutoSetAiMasterRecord = configuration.AutoSetAiMasterRecord;
            AutoSetMasterRecordType = configuration.AutoSetMasterRecordType;
        }
    }
}