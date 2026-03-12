namespace WinPure.CleanAndMatch.Integration.Import;

internal partial class BaseDatabaseImportForm : BaseImportForm
{
    internal IDatabaseImportProvider ImportDbProvider => ImportProvider as IDatabaseImportProvider;

    public BaseDatabaseImportForm()
    {
        InitializeComponent();
    }

    public BaseDatabaseImportForm(IWpLogger logger, IConnectionSettingsService settingsService) : base(logger, settingsService, true)
    {
        InitializeComponent();
    }
    
    public BaseDatabaseImportForm(IWpLogger logger, IConnectionSettingsService settingsService, bool showMenu) : base(logger, settingsService, showMenu)
    {
        InitializeComponent();
    }
}