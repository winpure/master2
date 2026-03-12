using Newtonsoft.Json;
using System.Data.CData.ZohoCRM;
using WinPure.Integration.Abstractions;

namespace WinPure.Integration.Import;

internal class ZohoImportProvider : ImportProviderBase, IDatabaseImportProvider
{
    private ZohoCRMConnection _connection;
    private ZohoImportOptions _options;

    public ZohoImportProvider() : base(ExternalSourceTypes.ZohoCrm)
    {
        DisplayName = "ZohoCRM";
    }

    public override void Initialize(object parameters)
    {
        _options = parameters as ZohoImportOptions;
        if (_options == null)
        {
            return;
        }
        CloseConnection();
        _connection = new ZohoCRMConnection(CreateConnectionString());
    }

    protected override DataTable GetPreview(int rowToPreview)
    {
        _previewTable = null;
        try
        {
            if (!CheckConnect())
            {
                return _previewTable;
            }

            ImportedInfo.DisplayName = _options.TableName;
            var cmd = _connection.CreateCommand();
            cmd.CommandText = $"SELECT TOP {_MAX_ROW_TO_PREVIEW} * FROM [{_options.TableName}]";
            cmd.CommandType = CommandType.Text;
            var adapter = new ZohoCRMDataAdapter(cmd);
            _previewTable = new DataTable("Preview");
            adapter.Fill(_previewTable);
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

    protected override DataTable GetData()
    {
        using (var cmd = _connection.CreateCommand())
        {
            cmd.CommandText = $"SELECT {(_MAX_ROW_TO_IMPORT == 0 ? "" : "TOP " + _MAX_ROW_TO_IMPORT)} * FROM [{_options.TableName}]";
            cmd.CommandType = CommandType.Text;
            using (var adapter = new ZohoCRMDataAdapter(cmd))
            {
                var dat = new DataTable();
                NotifyProgress(Resources.CAPTION_IO_IMPORTING_DATA, 20);
                adapter.Fill(dat);
                var skippedColumns = RemoveSkippedField(dat, _options.Fields);
                return dat;
            }
        }
    }

    public override DataTable ImportData()
    {
        try
        {
            if (CheckConnect())
            {
                _options.Fields = ImportedInfo.Fields;
                var result = GetData();
                ImportedInfo.RowCount = result.Rows.Count;
                ImportedInfo.ImportParameters = JsonConvert.SerializeObject(_options);
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

    protected override DataTable GetReimportData(string parameterJson)
    {
        _options = JsonConvert.DeserializeObject<ZohoImportOptions>(parameterJson);
        ImportedInfo.Fields = _options.Fields;

        if (_connection == null)
        {
            _connection = new ZohoCRMConnection(CreateConnectionString());
        }

        if (CheckConnect())
        {
            var result = GetData();
            ImportedInfo.RowCount = result.Rows.Count;
            ImportedInfo.ImportParameters = JsonConvert.SerializeObject(_options);
            LimitedRecords = ImportedInfo.RowCount == _MAX_ROW_TO_IMPORT;
        }
        return null;
    }

    public bool CheckConnect()
    {
        try
        {
            if (_connection == null)
            {
                return false;
            }

            if (_connection?.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            return _connection.State == ConnectionState.Open;
        }
        catch (Exception ex)
        {
            _logger.Debug($"Can not connect to {DisplayName}", ex);
            NotifyException(Resources.EXCEPTION_IO_CANNOT_CONNECT_TO_SERVER, ex);
        }
        return false;
    }

    public List<string> GetDatabaseList()
    {
        return new List<string>();
    }

    public List<string> GetDatabaseTables()
    {
        try
        {
            if (CheckConnect())
            {
                DataTable schema = _connection.GetSchema("TABLES", null);
                DataRow[] tables = schema.Select("TABLE_TYPE='BASE TABLE'");
                int index = schema.Columns.IndexOf("TABLE_NAME");
                var tableNames = new List<string>();
                for (int i = 0; i < tables.Length; i++)
                {
                    tableNames.Add((string)tables[i].ItemArray[index]);
                }
                return tableNames;
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

    public (bool isValid, string error) ValidateSql()
    {
        return (false, "SQL is not possible!");
    }

    public void ExecuteSql(string script)
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        CloseConnection();
        base.Dispose();
    }

    private void CloseConnection(bool dispose = true)
    {
        if (_connection != null)
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }

            if (dispose)
            {
                _connection.Dispose();
            }
        }
    }

    private ZohoCRMConnectionStringBuilder CreateConnectionString()
    {
        return new ZohoCRMConnectionStringBuilder()
        {
            CallbackURL = _options.CallbackUrl,
            OAuthClientId = _options.ClientId,
            OAuthClientSecret = _options.ClientSecret,
            InitiateOAuth = "GETANDREFRESH",
            OAuthAccessToken = "",
            RTK = "435A524541415355525041413153554231454B473334333000000000000000000000000000000000485058585443455A00003133365853565A59543642420000"

        };
    }
}