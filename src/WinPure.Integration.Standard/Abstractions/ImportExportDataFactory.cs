using Newtonsoft.Json;
using WinPure.Common.Exceptions;
using WinPure.Integration.Export;
using WinPure.Integration.Import;

namespace WinPure.Integration.Abstractions;

public static class ImportExportDataFactory
{
    internal static IImportProvider GetImportDataInstance(ExternalSourceTypes sourceType)
    {
        switch (sourceType)
        {
            case ExternalSourceTypes.TextFile:
                return new CsvImportProvider();
            case ExternalSourceTypes.SqlServer:
                return new MsSqlServerImportProvider();
            case ExternalSourceTypes.Db2:
                return new Db2ImportProvider();
            case ExternalSourceTypes.MySqlServer:
                return new MySqlImportProvider();
            case ExternalSourceTypes.Excel:
                return new ExcelImportProvider();
            case ExternalSourceTypes.Access:
                return new AccessImportProvider();
            case ExternalSourceTypes.Oracle:
                return new OracleImportProvider();

            case ExternalSourceTypes.Json:
                return new JsonImportProvider();
            case ExternalSourceTypes.Xml:
                return new XmlImportProvider();                
            case ExternalSourceTypes.Senzing:
                return new SenzingImportProvider();
            case ExternalSourceTypes.JSONL:
                return new JsonlImportProvider();

            case ExternalSourceTypes.SqLite:
                return new SqLiteImportProvider();
            case ExternalSourceTypes.Salesforce:
                return new SalesforceImportProvider();

            case ExternalSourceTypes.Postgres:
                return new PostgresImportProvider();
            case ExternalSourceTypes.AzureDb:
                return new AzureDbImportProvides();
            case ExternalSourceTypes.ZohoCrm:
                return new ZohoImportProvider();                
            case ExternalSourceTypes.Snowflake:
                return new SnowflakeImportProvider();

            default:
                throw new WinPureArgumentException($"Type {nameof(sourceType)} with value {sourceType} is out of range");
        }
    }

    internal static IExportProvider GetExportDataInstance(ExternalSourceTypes sourceType)
    {
        switch (sourceType)
        {
            case ExternalSourceTypes.TextFile:
                return new CsvExportProvider();
            case ExternalSourceTypes.SqlServer:
                return new MsSqlServerExportProvider();
            case ExternalSourceTypes.MySqlServer:
                return new MySqlExportProvider();
            case ExternalSourceTypes.Excel:
                return new ExcelExportProvider();
            case ExternalSourceTypes.Access:
                return new AccessExportProvider();
            case ExternalSourceTypes.Oracle:
                return new OracleExportProvider();
            case ExternalSourceTypes.Db2:
                return new Db2ExportProvider();

            case ExternalSourceTypes.Json:
                return new JsonExportProvider();
            case ExternalSourceTypes.Xml:
                return new XmlExportProvider();
            case ExternalSourceTypes.SqLite:
                return new SqLiteExportProvider();
            case ExternalSourceTypes.Postgres:
                return new PostgresExportProvider();
            case ExternalSourceTypes.AzureDb:
                return new AzureDbExportProvider();
            case ExternalSourceTypes.Snowflake:
                return new SnowflakeExportProvider();
            case ExternalSourceTypes.DataTable:
            default:
                throw new WinPureArgumentException($"Type {nameof(sourceType)} with value {sourceType} is out of range");
        }
    }

    public static object GetExportDataParameters(ExternalSourceTypes sourceType, string paramString)
    {
        switch (sourceType)
        {
            case ExternalSourceTypes.TextFile:
                return JsonConvert.DeserializeObject<TextImportExportOptions>(paramString);
            case ExternalSourceTypes.Excel:
                return JsonConvert.DeserializeObject<ExcelImportExportOptions>(paramString);
            case ExternalSourceTypes.SqlServer:
            case ExternalSourceTypes.MySqlServer:
            case ExternalSourceTypes.Access:
            case ExternalSourceTypes.Oracle:
            case ExternalSourceTypes.AzureDb:
            case ExternalSourceTypes.Postgres:
            case ExternalSourceTypes.Db2:
                return JsonConvert.DeserializeObject<SqlImportExportOptions>(paramString);
            case ExternalSourceTypes.Snowflake:
                return JsonConvert.DeserializeObject<SnowflakeImportExportOptions>(paramString);


            case ExternalSourceTypes.Odbc:
            case ExternalSourceTypes.Json:
            case ExternalSourceTypes.Xml:
            case ExternalSourceTypes.MongoDb:

            case ExternalSourceTypes.MsDynamics:
            case ExternalSourceTypes.Hadoop:
            case ExternalSourceTypes.ZohoCrm:
            case ExternalSourceTypes.SqLite:

            case ExternalSourceTypes.DataTable:
            case ExternalSourceTypes.NotDefined:
            default:
                throw new ArgumentOutOfRangeException(nameof(sourceType), sourceType, null);
        }
    }
}