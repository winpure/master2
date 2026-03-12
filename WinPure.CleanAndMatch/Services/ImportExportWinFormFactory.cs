namespace WinPure.CleanAndMatch.Services;

internal static class ImportExportWinFormFactory
{
    public static Integration.Import.IImportForm GetImportForm(ExternalSourceTypes srcType)
    {
        switch (srcType)
        {
            case ExternalSourceTypes.TextFile:
                return WinPureUiDependencyResolver.Resolve<frmImportText>();

            case ExternalSourceTypes.SqlServer:
            case ExternalSourceTypes.Oracle:
            case ExternalSourceTypes.AzureDb:
            case ExternalSourceTypes.Postgres:
            case ExternalSourceTypes.Db2:
                return WinPureUiDependencyResolver.Resolve<frmImportFromSQLServer>();

            case ExternalSourceTypes.MySqlServer:
                return WinPureUiDependencyResolver.Resolve<frmImportMySQL>();

            case ExternalSourceTypes.Excel:
                return WinPureUiDependencyResolver.Resolve<frmImportExcel>();

            case ExternalSourceTypes.Access:
            case ExternalSourceTypes.SqLite:
                return WinPureUiDependencyResolver.Resolve<frmImportFromFileDatabase>();

            case ExternalSourceTypes.Json:
            case ExternalSourceTypes.Xml:
            case ExternalSourceTypes.Senzing:
            case ExternalSourceTypes.JSONL:
                return WinPureUiDependencyResolver.Resolve<frmImportFile>();

            case ExternalSourceTypes.Salesforce:
                return WinPureUiDependencyResolver.Resolve<frmImportFromSalesforce>();

            case ExternalSourceTypes.ZohoCrm:
                return WinPureUiDependencyResolver.Resolve<frmImportFromZoho>();

            case ExternalSourceTypes.Snowflake :
                return WinPureUiDependencyResolver.Resolve<frmImportSnowflake>();

            case ExternalSourceTypes.Odbc:
            case ExternalSourceTypes.MongoDb:
            case ExternalSourceTypes.MsDynamics:
            case ExternalSourceTypes.Hadoop:
            case ExternalSourceTypes.SugarCrm:
            case ExternalSourceTypes.DataTable:
            case ExternalSourceTypes.NotDefined:
            default:
                throw new ArgumentOutOfRangeException(nameof(srcType), srcType, null);
        }
    }

    public static Integration.Export.IExportForm GetExportForm(ExternalSourceTypes srcType)
    {
        switch (srcType)
        {
            case ExternalSourceTypes.TextFile:
                return WinPureUiDependencyResolver.Resolve<frmExportText>();

            case ExternalSourceTypes.SqlServer:
            case ExternalSourceTypes.Oracle:
            case ExternalSourceTypes.AzureDb:
            case ExternalSourceTypes.Postgres:
            case ExternalSourceTypes.Db2:
                return WinPureUiDependencyResolver.Resolve<frmExportToSQLServer>();

            case ExternalSourceTypes.MySqlServer:
                return WinPureUiDependencyResolver.Resolve<frmExportMySQL>();

            case ExternalSourceTypes.Excel:
                return WinPureUiDependencyResolver.Resolve<frmExportExcel>();

            case ExternalSourceTypes.Access:
            case ExternalSourceTypes.SqLite:
                return WinPureUiDependencyResolver.Resolve<frmExportToFileDatabase>();

            case ExternalSourceTypes.Json:
            case ExternalSourceTypes.Xml:
                return WinPureUiDependencyResolver.Resolve<frmExportFile>();

            case ExternalSourceTypes.Snowflake:
                return WinPureUiDependencyResolver.Resolve<frmExportToSnowflake>();

            case ExternalSourceTypes.Odbc:
            case ExternalSourceTypes.MongoDb:

            case ExternalSourceTypes.MsDynamics:
            case ExternalSourceTypes.Hadoop:
            case ExternalSourceTypes.ZohoCrm:

            case ExternalSourceTypes.DataTable:
            case ExternalSourceTypes.Senzing:
            case ExternalSourceTypes.JSONL:
            case ExternalSourceTypes.NotDefined:
            default:
                throw new ArgumentOutOfRangeException(nameof(srcType), srcType, null);
        }
    }
}