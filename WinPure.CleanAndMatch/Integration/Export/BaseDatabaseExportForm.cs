namespace WinPure.CleanAndMatch.Integration.Export
{
    internal partial class BaseDatabaseExportForm : BaseExportForm
    {
        internal IDatabaseExportProvider ExportDbProvider => ExportProvider as IDatabaseExportProvider;

        public BaseDatabaseExportForm()
        {
            InitializeComponent();
        }

        public BaseDatabaseExportForm(IWpLogger logger, IConnectionSettingsService settingsService) : base(logger, settingsService, true)
        {
            InitializeComponent();
        }

        public BaseDatabaseExportForm(IWpLogger logger, IConnectionSettingsService settingsService, bool showMenu) : base(logger, settingsService, showMenu)
        {
            InitializeComponent();
        }
    }
}
