using System.IO;
using WinPure.Common.Enums;
using WinPure.Configuration.DependencyInjection;
using WinPure.DataService.Senzing.Models.G2;
using WinPure.Integration.Abstractions;
using WinPure.Integration.Models.ImportExportOptions;

namespace WinPure.DataService.Senzing.Helpers;

internal static class ConfigurationHelper
{
    internal static string DbTemplate = "G2C_Template.db";
    internal static string InitPostgres = "InitPostgres.sql";
    internal static string InitMsSql = "InitMsSql.sql";
    internal static string InitSqLite = "InitSqLite.sql";

    public static void InitSqLiteFilesFromTemplate(string basePath, SenzingConfiguration configuration, IWpLogger logger)
    {
        var templatePath = Path.Combine(basePath, DbTemplate);
        if (File.Exists(templatePath))
        {
            try
            {
                File.Copy(templatePath, configuration.DbPath, true);
                File.Copy(templatePath, configuration.DbPathC1, true);
                File.Copy(templatePath, configuration.DbPathC2, true);
                //needPurge = false;
            }
            catch (Exception e)
            {
                logger.Error($"Can not update DB file. Error: {e.Message}");
            }
        }
    }

    public static SqlImportExportOptions GetConnectionString(ExternalSourceTypes source, string connectionName)
    {
        var connectionService = WinPureConfigurationDependency.Resolve<IConnectionSettingsService>();
        var connections = connectionService.Get(source);
        var connection = connections.FirstOrDefault(x => string.Compare(x.Name, connectionName, StringComparison.InvariantCultureIgnoreCase) == 0);

        if (connection == null)
        {
            throw new WinPureArgumentException("MatchAI can not find specified connection");
        }

        return connectionService.Get<SqlImportExportOptions>(connection.Id).Settings as SqlImportExportOptions;
    }

    public static void InitDatabase(ExternalSourceTypes source, string connectionName)
    {
        var connection = GetConnectionString(source, connectionName);

        var importService = ImportExportDataFactory.GetImportDataInstance(source) as IDatabaseImportProvider;
        importService.Initialize(connection);
        var tableList = importService.GetDatabaseTables();
        if (tableList.Any(x => x.ToUpper().Contains("SYS_VARS")))
        {
            return;
        }

        var scriptFile = GetScript(source);
        var script = File.ReadAllText(scriptFile);
        importService.ExecuteSql(script);
    }

    private static string GetScript(ExternalSourceTypes source)
    {
        return source switch
        {
            ExternalSourceTypes.Postgres => Path.Combine("Senzing", "Scripts", InitPostgres),
            ExternalSourceTypes.SqlServer => Path.Combine("MatchAI", "Scripts", InitMsSql),
            ExternalSourceTypes.SqLite => Path.Combine("MatchAI", "Scripts", InitSqLite),
            _ => throw new NotImplementedException(),
        };
    }
}