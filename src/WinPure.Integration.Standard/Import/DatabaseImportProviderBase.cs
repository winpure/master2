using Newtonsoft.Json;
using System.Data.Common;
using WinPure.Integration.Abstractions;

namespace WinPure.Integration.Import;

internal abstract class DatabaseImportProviderBase : ImportProviderBase, IDatabaseImportProvider
{
    protected SqlImportExportOptions Options;
    protected abstract DbConnection DatabaseConnection { get; }

    internal DatabaseImportProviderBase(ExternalSourceTypes sourceType) : base(sourceType)
    {
    }

    public override void Initialize(object parameters)
    {
        Options = parameters as SqlImportExportOptions;
        if (Options != null)
        {
            CloseConnection();
            if (!string.IsNullOrEmpty(Options.ServerAddress) || !string.IsNullOrEmpty(Options.DatabaseFile))
            {
                CreateConnection(CreateConnectionString());
            }
        }
        else
        {
            NotifyException(Resources.EXCEPTION_IO_PARAMETERS_WRONG_FORMAT, null);
        }
    }

    public override DataTable ImportData()
    {
        try
        {
            if (CheckConnect())
            {
                Options.Fields = ImportedInfo.Fields;
                var result = GetData();
                ImportedInfo.RowCount = result?.Rows.Count ?? 0;
                ImportedInfo.ImportParameters = JsonConvert.SerializeObject(Options);
                LimitedRecords = ImportedInfo.RowCount == _MAX_ROW_TO_IMPORT;
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.Debug($"IMPORT FROM {DisplayName} ERROR", ex);
            NotifyException(Resources.EXCEPTION_IO_ERROR_ON_IMPORT, ex);
        }
        finally
        {
            Dispose();
        }
        return null;
    }

    public virtual void ExecuteSql(string script)
    {
        throw new NotImplementedException();
    }

    [STAThread]
    public override DataTable GetPreview()
    {
        try
        {
            _previewTable = GetPreview(_MAX_ROW_TO_PREVIEW);
            return _previewTable;
        }
        catch (Exception ex)
        {
            _logger.Debug($"PREVIEW FROM {DisplayName} ERROR", ex);
            NotifyException(Resources.EXCEPTION_IO_ERROR_ON_IMPORT, ex);
        }
        finally
        {
            CloseConnection(false);
        }
        return null;
    }

    public virtual List<string> GetDatabaseList()
    {
        try
        {
            if (CheckConnect())
            {
                return GetDatabases();
            }
        }
        catch (Exception ex)
        {
            NotifyException(Resources.EXCEPTION_IO_CANNOT_GET_DATABASE_LIST, ex);
        }
        finally
        {
            CloseConnection(false);
        }
        return new List<string>();
    }

    public virtual List<string> GetDatabaseTables()
    {
        try
        {
            if (CheckConnect())
            {
                return GetTables();
            }
        }
        catch (Exception ex)
        {
            NotifyException(Resources.EXCEPTION_IO_CANNOT_GET_TABLES_LIST, ex);
        }
        finally
        {
            CloseConnection(false);
        }
        return new List<string>();
    }

    public virtual bool CheckConnect()
    {
        try
        {
            if (DatabaseConnection == null)
            {
                return false;
            }

            if (DatabaseConnection.State != ConnectionState.Open)
            {
                DatabaseConnection.Open();
            }

            return DatabaseConnection.State == ConnectionState.Open;
        }
        catch (Exception ex)
        {
            _logger.Debug($"Can not connect to {DisplayName}", ex);
            NotifyException(Resources.EXCEPTION_IO_CANNOT_CONNECT_TO_SERVER, ex);
        }
        return false;
    }

    public (bool isValid, string error) ValidateSql()
    {
        if (DatabaseConnection == null || Options == null || string.IsNullOrWhiteSpace(Options.SqlQuery))
            return (false, "Database not configured or there is no SQL Query");

        try
        {
            return CheckConnect() 
                ? SqlSyntaxValidator.Validate(DatabaseConnection, Options.SqlQuery)
                : (false, Resources.EXCEPTION_IO_CANNOT_CONNECT_TO_SERVER);
        }
        catch (Exception ex)
        {
            _logger.Debug($"Validation of connection to {DisplayName} failed", ex);
            return (false, ex.Message);
        }
    }

    protected override DataTable GetReimportData(string parametersJson)
    {
        Options = JsonConvert.DeserializeObject<SqlImportExportOptions>(parametersJson);
        ImportedInfo.Fields = Options.Fields;
        CreateConnection(CreateConnectionString());
        if (CheckConnect())
        {
            var data = GetData();
            ImportedInfo.RowCount = data?.Rows.Count ?? 0;
            ImportedInfo.ImportParameters = JsonConvert.SerializeObject(Options);
            LimitedRecords = ImportedInfo.RowCount == _MAX_ROW_TO_IMPORT;
            return data;
        }
        return null;
    }

    protected virtual string CreateConnectionString()
    {
        return string.Empty;
    }

    protected abstract void CreateConnection(string connectionString);

    protected abstract List<string> GetDatabases();

    protected abstract List<string> GetTables();
        
    protected void CloseConnection(bool dispose = true)
    {
        if (DatabaseConnection != null)
        {
            if (DatabaseConnection.State == ConnectionState.Open)
            {
                DatabaseConnection.Close();
            }

            if (dispose)
            {
                DatabaseConnection.Dispose();
            }
        }
    }

    public override void Dispose()
    {
        CloseConnection();
        base.Dispose();
    }
}