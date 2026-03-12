global using DevExpress.Utils;
global using DevExpress.Xpo;
global using DevExpress.XtraBars;
global using DevExpress.XtraBars.Ribbon;
global using DevExpress.XtraEditors;
global using DevExpress.XtraGrid;
global using DevExpress.XtraGrid.Columns;
global using DevExpress.XtraGrid.Views.Grid;
global using DevExpress.XtraReports.UI;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using System;
global using System.Collections.Generic;
global using System.Data;
global using System.Drawing;
global using System.IO;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Windows.Forms;
global using WinPure.AddressVerification.DependencyInjection;
global using WinPure.Automation.DependencyInjection;
global using WinPure.CleanAndMatch.Automation;
global using WinPure.CleanAndMatch.DependencyInjection;
global using WinPure.CleanAndMatch.Enums;
global using WinPure.CleanAndMatch.Helpers;
global using WinPure.CleanAndMatch.Integration.Export;
global using WinPure.CleanAndMatch.Integration.Import;
global using WinPure.CleanAndMatch.MatchResultProcessing;
global using WinPure.CleanAndMatch.Models;
global using WinPure.CleanAndMatch.Properties;
global using WinPure.CleanAndMatch.Services;
global using WinPure.CleanAndMatch.Support;
global using WinPure.Cleansing.DependencyInjection;
global using WinPure.Common.Constants;
global using WinPure.Common.Enums;
global using WinPure.Common.Exceptions;
global using WinPure.Common.Helpers;
global using WinPure.Common.Logger;
global using WinPure.Common.Models;
global using WinPure.Configuration.DependencyInjection;
global using WinPure.Configuration.Service;
global using WinPure.DataService.DependencyInjection;
global using WinPure.DataService.Enums;
global using WinPure.DataService.Models;
global using WinPure.DataService.Services;
global using WinPure.Integration.Abstractions;
global using WinPure.Integration.DependencyInjection;
global using WinPure.Integration.Models.ImportExportOptions;
global using WinPure.Licensing.Services;
global using WinPure.Matching.DependencyInjection;
global using WinPure.Matching.Enums;
global using WinPure.Matching.Models;
global using WinPure.Project.DependencyInjection;
global using WinPure.Project.Services;
using WinPure.CleanAndMatch.StartupForm;

namespace WinPure.CleanAndMatch.DependencyInjection;

internal class WinPureUiDependencyResolver : WinPureConfigurationDependency
{
    public new static WinPureConfigurationDependency Instance => Resolver ??= new WinPureUiDependencyResolver();

    public override void RegisterDependencies()
    {
        base.RegisterDependencies();
        ServiceCollection.RegisterVerification();
        ServiceCollection.RegisterCleansing();
        ServiceCollection.RegisterMatching();
        ServiceCollection.RegisterIntegration();
        ServiceCollection.RegisterDataService();
        ServiceCollection.RegisterAutomation();
        ServiceCollection.RegisterProject();

        ServiceCollection.AddTransient<IImportExportService, ImportExportService>();
        ServiceCollection.AddSingleton<ThemeDetectionService>();
        ServiceCollection.AddSingleton(x => new ProjectSettings(Project.Helpers.ProjectHelper.CurrentProjectVersion));
        var descriptor =
            new ServiceDescriptor(
                typeof(IValueStore),
                typeof(RegistryStore),
                ServiceLifetime.Transient);

        ServiceCollection.Replace(descriptor);
        RegisterForms(ServiceCollection);
    }

    private void RegisterForms(ServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<frmAbout>();
        serviceCollection.AddTransient<frmAddNewColumn>();
        serviceCollection.AddTransient<frmCreateNewProject>();
        serviceCollection.AddTransient<frmEditDictionary>();
        serviceCollection.AddTransient<frmPatternConfiguration>();
        serviceCollection.AddTransient<frmNewName>();
        serviceCollection.AddTransient<frmRegister>();
        serviceCollection.AddTransient<frmRegisterOnStartup>();
        serviceCollection.AddTransient<frmSelectColumns>();
        serviceCollection.AddTransient<frmSelectFields>();
        serviceCollection.AddTransient<frmWordManagerSettings>();
        serviceCollection.AddTransient<LoadingMaskForm>();
        serviceCollection.AddTransient<frmExportExcel>();
        serviceCollection.AddTransient<frmExportFile>();
        serviceCollection.AddTransient<frmExportMySQL>();
        serviceCollection.AddTransient<frmExportText>();
        serviceCollection.AddTransient<frmExportToFileDatabase>();
        serviceCollection.AddTransient<frmExportToSQLServer>();
        serviceCollection.AddTransient<frmExportToSnowflake>();
        serviceCollection.AddTransient<frmImportExcel>();
        serviceCollection.AddTransient<frmImportFile>();
        serviceCollection.AddTransient<frmImportFromFileDatabase>();
        serviceCollection.AddTransient<frmImportFromSQLServer>();
        serviceCollection.AddTransient<frmImportFromSalesforce>();
        serviceCollection.AddTransient<frmImportFromZoho>();
        serviceCollection.AddTransient<frmImportMySQL>();
        serviceCollection.AddTransient<frmImportSnowflake>();
        serviceCollection.AddTransient<frmImportText>();
        serviceCollection.AddTransient<frmAutomationScheduling>();
        serviceCollection.AddTransient<frmAutomationConfig>();
        serviceCollection.AddTransient<frmConfiguration>();
        serviceCollection.AddTransient<frmProperCaseConfiguration>();

        serviceCollection.AddTransient<frmSetMasterRecords>();
        serviceCollection.AddTransient<frmUpdateMatchResult>();
        serviceCollection.AddTransient<frmMatchResultDelete>();
        serviceCollection.AddTransient<frmMatchResultMerge>();
        serviceCollection.AddTransient<frmSystemInfo>();
        serviceCollection.AddTransient<frmLoadingForm>();
        serviceCollection.AddTransient<frmErDefaultMapping>();
        serviceCollection.AddTransient<frmCleansingAiConfiguration>();
        serviceCollection.AddTransient<frmStartupOptionsNew>();
        serviceCollection.AddTransient<frmCleansingRegex>();
    }
}