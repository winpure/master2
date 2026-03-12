using Newtonsoft.Json;
using System.Data.Common;
using WinPure.Integration.Abstractions;

namespace WinPure.Integration.Export;

internal abstract class DatabaseExportProviderBase : ExportProviderBase, IDatabaseExportProvider
{
    protected SqlImportExportOptions Options;
    protected abstract DbConnection DatabaseConnection { get; }

    protected bool TableExists = false;

    protected DatabaseExportProviderBase(ExternalSourceTypes sourceType) : base(sourceType)
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
            ExportParameters = JsonConvert.SerializeObject(Options);
        }
        else
        {
            NotifyException(Resources.EXCEPTION_IO_PARAMETERS_WRONG_FORMAT, null);
        }
    }

    protected virtual string CreateConnectionString()
    {
        return string.Empty;
    }

    protected abstract void CreateConnection(string connectionString);

    protected abstract void ExportData(DataTable data);

    public override void Export(DataTable data)
    {
        if (data == null || data.Columns.Count == 0)
        {
            NotifyException(Resources.EXCEPTION_IO_EMPTY_EXPORT_DATA, null);
            return;
        }
        if (string.IsNullOrEmpty(Options.TableName))
        {
            NotifyException(Resources.EXCEPTION_IO_TABLE_NAME_SHOULD_BE, null);
            return;
        }

        if (GetDatabaseTables().Any(x => x.Split('.').Any(y => string.Compare(y, Options.TableName, StringComparison.InvariantCultureIgnoreCase) == 0)))
        {
            TableExists = true;
        }
        try
        {
            ExportData(data);
        }
        catch (Exception ex)
        {
            _logger.Debug($"EXPORT TO {DisplayName} ERROR", ex);
            NotifyException(string.Format(Resources.EXCEPTION_IO_DATA_COULD_NOT_BE_EXPORTED_TO, DisplayName), ex);
        }
        finally
        {
            Dispose();
        }
    }

    protected abstract List<string> GetDatabases();

    public virtual List<string> GetDatabaseList()
    {
        try
        {
            CheckConnect();
            return GetDatabases();
        }
        catch (Exception ex)
        {
            NotifyException(Resources.EXCEPTION_IO_CANNOT_GET_DATABASE_LIST, ex);
            return new List<string>();
        }
        finally
        {
            CloseConnection(false);
        }
    }

    protected abstract List<string> GetTables();

    public virtual List<string> GetDatabaseTables()
    {
        try
        {
            CheckConnect();
            return GetTables();
        }
        catch (Exception ex)
        {
            NotifyException(Resources.EXCEPTION_IO_CANNOT_GET_TABLES_LIST, ex);
            return new List<string>();
        }
        finally
        {
            CloseConnection(false);
        }
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
            _logger.Debug($"Cannot connect to {DisplayName}", ex);
            NotifyException(Resources.EXCEPTION_IO_CANNOT_CONNECT_TO_SERVER, ex);
            return false;
        }
    }

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