using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data.Common;
using WinPure.Common.Exceptions;
using WinPure.Integration.Abstractions;

namespace WinPure.Integration.Import;

internal class MySqlImportProvider : ImportProviderBase, IDatabaseImportProvider
{
    private SqlImportExportOptions _options;

    public MySqlImportProvider() : base(ExternalSourceTypes.MySqlServer)
    {
        DisplayName = "MySql";
    }

    public override void Initialize(object parameters)
    {
        _options = parameters as SqlImportExportOptions;
    }

    protected override DataTable GetPreview(int rowToPreview)
    {
        throw new NotImplementedException();
    }

    public override DataTable GetPreview()
    {
        try
        {
            if (_options.UseSsh)
            {
                using (var tunnel = new SshTunnel(_options.SshServer, _options.SshLogin, _options.SshPassword, _options.ServerAddress, (uint)_options.Port))
                {
                    using (var connection = new MySqlConnection(CreateConnectionString(tunnel.BoundHost, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, tunnel.BoundPort.ToString(), _options.UseSsl)))
                    {
                        return GetPreview(connection);
                    }
                }
            }

            using (var connection = new MySqlConnection(CreateConnectionString(_options.ServerAddress, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, _options.Port.ToString(), _options.UseSsl)))
            {
                return GetPreview(connection);
            }
        }
        catch (Exception ex)
        {
            NotifyException(Resources.EXCEPTION_IO_CANNOT_GET_PREVIEW, ex);
        }
        return null;
    }

    protected override DataTable GetData()
    {
        throw new NotImplementedException();
    }

    public override DataTable ImportData()
    {
        _options.Fields = ImportedInfo.Fields;
        if (_options.UseSsh)
        {
            using (var tunnel = new SshTunnel(_options.SshServer, _options.SshLogin, _options.SshPassword, _options.ServerAddress, (uint)_options.Port))
            {
                using (var connection = new MySqlConnection(CreateConnectionString(tunnel.BoundHost, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, tunnel.BoundPort.ToString(), _options.UseSsl)))
                {
                    return GetData(connection);
                }
            }
        }

        using (var connection = new MySqlConnection(CreateConnectionString(_options.ServerAddress, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, _options.Port.ToString(), _options.UseSsl)))
        {
            return GetData(connection);
        }

    }

    public bool CheckConnect()
    {
        try
        {
            if (_options.UseSsh)
            {
                using (var tunnel = new SshTunnel(_options.SshServer, _options.SshLogin, _options.SshPassword, _options.ServerAddress, (uint)_options.Port))
                {
                    return CheckConnect(tunnel.BoundHost, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, tunnel.BoundPort.ToString(), _options.UseSsl);
                }
            }

            return CheckConnect(_options.ServerAddress, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, _options.Port.ToString(), _options.UseSsl);

        }
        catch (Exception ex)
        {
            NotifyException(Resources.EXCEPTION_IO_CANNOT_CONNECT_TO_SERVER, ex);
            return false;
        }
    }

    public List<string> GetDatabaseList()
    {
        try
        {
            if (_options.UseSsh)
            {
                using (var tunnel = new SshTunnel(_options.SshServer, _options.SshLogin, _options.SshPassword, _options.ServerAddress, (uint)_options.Port))
                {
                    return GetDatabaseList(tunnel.BoundHost, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, tunnel.BoundPort.ToString(), _options.UseSsl);
                }
            }

            return GetDatabaseList(_options.ServerAddress, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, _options.Port.ToString(), _options.UseSsl);
        }
        catch (Exception ex)
        {
            NotifyException(Resources.EXCEPTION_IO_CANNOT_GET_DATABASE_LIST, ex);
            return null;
        }
    }

    public List<string> GetDatabaseTables()
    {
        try
        {
            if (_options.UseSsh)
            {
                using (var tunnel = new SshTunnel(_options.SshServer, _options.SshLogin, _options.SshPassword, _options.ServerAddress, (uint)_options.Port))
                {
                    return GetDatabaseTables(tunnel.BoundHost, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, tunnel.BoundPort.ToString(), _options.UseSsl);
                }
            }

            return GetDatabaseTables(_options.ServerAddress, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, _options.Port.ToString(), _options.UseSsl);
        }
        catch (Exception ex)
        {
            NotifyException(Resources.EXCEPTION_IO_CANNOT_GET_TABLES_LIST, ex);
            return null;
        }
    }

    public override DataTable ReimportData(string parametersJson)
    {
        try
        {
            _options = JsonConvert.DeserializeObject<SqlImportExportOptions>(parametersJson);
            ImportedInfo.Fields = _options.Fields;
            DataTable data;
            if (_options.UseSsh)
            {
                using (var tunnel = new SshTunnel(_options.SshServer, _options.SshLogin, _options.SshPassword, _options.ServerAddress, (uint)_options.Port))
                {
                    using (var connection = new MySqlConnection(CreateConnectionString(tunnel.BoundHost, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, tunnel.BoundPort.ToString(), _options.UseSsl)))
                    {
                        data = GetData(connection);
                    }
                }
            }

            using (var connection = new MySqlConnection(CreateConnectionString(_options.ServerAddress, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, _options.Port.ToString(), _options.UseSsl)))
            {
                data = GetData(connection);
            }
            ImportedInfo.RowCount = data?.Rows.Count ?? 0;
            ImportedInfo.ImportParameters = JsonConvert.SerializeObject(_options);
            return data;
        }
        catch (Exception ex)
        {
            _logger.Debug($"REIMPORT FROM {DisplayName} ERROR", ex);
            NotifyException(Resources.EXCEPTION_IO_ERROR_ON_IMPORT, ex);
        }
        return null;
    }

    public (bool isValid, string error) ValidateSql()
    {
        if (_options.UseSsh)
        {
            using (var tunnel = new SshTunnel(_options.SshServer, _options.SshLogin, _options.SshPassword, _options.ServerAddress, (uint)_options.Port))
            {
                using (var connection = new MySqlConnection(CreateConnectionString(tunnel.BoundHost, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, tunnel.BoundPort.ToString(), _options.UseSsl)))
                {
                    return ValidateSql(connection);
                }
            }
        }

        using (var connection = new MySqlConnection(CreateConnectionString(_options.ServerAddress, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, _options.Port.ToString(), _options.UseSsl)))
        {
            return ValidateSql(connection);
        }
    }

    public void ExecuteSql(string script)
    {
        if (CheckConnect())
        {
            if (_options.UseSsh)
            {
                using (var tunnel = new SshTunnel(_options.SshServer, _options.SshLogin, _options.SshPassword, _options.ServerAddress, (uint)_options.Port))
                {
                    using (var connection = new MySqlConnection(CreateConnectionString(tunnel.BoundHost, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, tunnel.BoundPort.ToString(), _options.UseSsl)))
                    {
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = script;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            else
            {
                using (var connection = new MySqlConnection(CreateConnectionString(_options.ServerAddress, _options.IntegrateSecurity, _options.UserName, _options.Password, _options.DatabaseName, _options.Port.ToString(), _options.UseSsl)))
                {
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = script;
                        cmd.ExecuteNonQuery();
                    }
                }

            }
        }
    }
    
    protected override DataTable GetReimportData(string parameterJson)
    {
        throw new NotImplementedException();
    }

    private bool CheckConnect(string server, bool integrateSecurity, string userName, string userPassword, string database, string port, bool useSsl)
    {
        using (var connection = new MySqlConnection(CreateConnectionString(server, integrateSecurity, userName, userPassword, database, port, useSsl)))
        {
            connection.Open();
            var res = connection.State == ConnectionState.Open;
            connection.Close();
            return res;
        }
    }

    private DataTable GetPreview(MySqlConnection connection)
    {
        _previewTable = new DataTable();
        var query = BuildQuery(string.Empty, _MAX_ROW_TO_PREVIEW);
        var tblNameParts = _options.TableName.Split('.');
        ImportedInfo.DisplayName = tblNameParts.Length > 1 ? tblNameParts[1] : _options.TableName;
        using (var cmd = new MySqlCommand(query, connection))
        {
            var da = new MySqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(_previewTable);

        }
        connection.Close();
        return _previewTable;
    }

    private List<string> GetDatabaseTables(string server, bool integrateSecurity, string userName, string userPassword, string database, string port, bool useSsl)
    {
        using (var connection = new MySqlConnection(CreateConnectionString(server, integrateSecurity, userName, userPassword, database, port, useSsl)))
        {
            connection.Open();
            var tables = connection.GetSchema("Tables");
            connection.Close();
            return tables.Select()
                .Select(x => x["TABLE_SCHEMA"].ToString() + "." + x["TABLE_NAME"].ToString()).OrderBy(x => x)
                .ToList();
        }
    }

    private List<string> GetDatabaseList(string server, bool integrateSecurity, string userName, string userPassword, string database, string port, bool useSsl)
    {
        using (var connection = new MySqlConnection(CreateConnectionString(server, integrateSecurity, userName, userPassword, database, port, useSsl)))
        {
            connection.Open();
            var databases = connection.GetSchema("Databases");
            connection.Close();
            return databases.Select().Select(x => x["database_name"].ToString()).ToList();
        }
    }

    private string CreateConnectionString(string server, bool integrateSecurity, string userName, string userPassword, string database, string port, bool useSsl)
    {
        var res = $"Server={server};";
        if (!string.IsNullOrEmpty(port))
        {
            res += $"Port={port};";
        }

        if (!integrateSecurity)
        {
            res += $"UID={userName}; password={userPassword};";
        }
        else
        {
            res += "UID = auth_windows; IntegratedSecurity = yes;";
        }
        if (!string.IsNullOrEmpty(database))
        {
            res += $"database={database};";
        }

        var sslMode = useSsl ? "Required" : "None";
        res += $"SslMode={sslMode};Connect Timeout=30;Allow User Variables=True";
        return res;
    }

    private DataTable GetData(MySqlConnection connection)
    {
        try
        {
            var tblNameParts = _options.TableName.Split('.');
            ImportedInfo.FileName = tblNameParts.Length > 1 ? tblNameParts[1] : _options.TableName;
            
            NotifyProgress(Resources.CAPTION_IO_IMPORTING_DATA, 10);
            var dataTable = new DataTable();
            var query = BuildQuery(GetSelectFields(), _MAX_ROW_TO_IMPORT);
            using (var cmd = new MySqlCommand(query, connection))
            {
                connection.Open();
                var da = new MySqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataTable);
            }

            ImportedInfo.RowCount = dataTable.Rows.Count;
            ImportedInfo.ImportParameters = JsonConvert.SerializeObject(_options);
            LimitedRecords = ImportedInfo.RowCount == _MAX_ROW_TO_IMPORT;
            return dataTable;
        }
        catch (WinPureImportException ex)
        {
            NotifyException(ex.Message, null);
        }
        catch (OutOfMemoryException m_ex)
        {
            NotifyException(Resources.EXCEPTION_OUTOFMEMORY, m_ex);
        }
        catch (Exception ex)
        {
            _logger.Debug("IMPORT FROM MY SQL ERROR", ex);
            NotifyException(Resources.EXCEPTION_IO_ERROR_ON_IMPORT, ex);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        return null;
    }
    
    private string BuildQuery(string fieldList, int numberOfRecords)
    {
        var sql = string.IsNullOrEmpty(fieldList) ? "select *" : $"select {GetSelectFields()}";
        sql += string.IsNullOrWhiteSpace(_options.SqlQuery) ? $" from {GetSafeTableName()}" : $" from ({_options.SqlQuery}) a";
        if (numberOfRecords > 0)
        {
            sql += $" LIMIT {numberOfRecords}";
        }

        return sql;
    }

    private (bool isValid, string error) ValidateSql(DbConnection connection)
    {
        if (_options == null || string.IsNullOrWhiteSpace(_options.SqlQuery))
            return (false, "Database not configured or there is no SQL Query");

        try
        {
            connection.Open();
            return CheckConnect()
                ? SqlSyntaxValidator.Validate(connection, _options.SqlQuery)
                : (false, Resources.EXCEPTION_IO_CANNOT_CONNECT_TO_SERVER);
        }
        catch (Exception ex)
        {
            _logger.Debug($"Validation of connection to {DisplayName} failed", ex);
            return (false, ex.Message);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }
    }

    private string GetSelectFields()
    {
        var sql = _options.Fields.Aggregate("", (current, fld) => current + $"`{fld.DatabaseName}`,");
        return sql.Substring(0, sql.Length - 1);
    }

    private string GetSafeTableName()
    {
        var tblNameParts = _options.TableName.Split('.');
        return tblNameParts.Length > 1 ? $"`{tblNameParts[0]}`.`{tblNameParts[1]}`" : $"`{_options.TableName}`";
    }
}