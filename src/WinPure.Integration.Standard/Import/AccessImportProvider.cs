using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics;

namespace WinPure.Integration.Import;

internal class AccessImportProvider : DatabaseImportProviderBase
{
    string _aceVer = "";
    private OleDbConnection _connection;
    protected override DbConnection DatabaseConnection => _connection;

    public AccessImportProvider() : base(ExternalSourceTypes.Access)
    {
        DisplayName = "Access";
    }

    protected override DataTable GetPreview(int rowToPreview)
    {
        ImportedInfo.DisplayName = Options.TableName;
        var myDataSet = new DataSet();
        string selCmd = $"SELECT top {_MAX_ROW_TO_PREVIEW} * FROM [{Options.TableName}]";
        using (var myAccessCommand = new OleDbCommand(selCmd, _connection))
        {
            using (var myDataAdapter = new OleDbDataAdapter(myAccessCommand))
            {
                myDataAdapter.Fill(myDataSet, "PreviewData");
                _previewTable = myDataSet.Tables[0];
                return _previewTable;
            }
        }
    }

    protected override DataTable GetData()
    {
        NotifyProgress(Resources.CAPTION_IO_PREPARING_TO_IMPORT, 5);
        var myDataSet = new DataSet();
        string selCmd = (_MAX_ROW_TO_IMPORT == 0)
            ? $"SELECT {GetSelectFields()} FROM [{Options.TableName}]"
            : $"SELECT TOP {_MAX_ROW_TO_IMPORT} {GetSelectFields()} FROM [{Options.TableName}]";
        using (var myAccessCommand = new OleDbCommand(selCmd, _connection))
        {
            using (var myDataAdapter = new OleDbDataAdapter(myAccessCommand))
            {
                NotifyProgress(Resources.CAPTION_IO_IMPORTING_DATA, 30);
                myDataAdapter.Fill(myDataSet, "ImportData");
            }
        }
        var tbl = myDataSet.Tables[0];
        return tbl;
    }

    protected override string CreateConnectionString()
    {
        var ver = ACEDriverVersion();
        if (string.IsNullOrEmpty(ver))
        {
            NotifyException(Resources.EXCEPTION_ACEOLEDB_NOTREGISTERED, null);
            return null;
        }

        return "Provider=Microsoft.ACE.OLEDB." + ver + ".0;Data Source=" + Options.DatabaseFile + ";OLE DB Services=-1" + (string.IsNullOrEmpty(Options.Password) ? ";Persist Security Info=False;" : ";Jet OLEDB:Database Password=" + Options.Password);
    }

    protected override void CreateConnection(string connectionString)
    {
        if (string.IsNullOrEmpty(Options.DatabaseFile))
        {
            NotifyException(Resources.EXCEPTION_IO_INVALID_DB_NAME, null);
        }

        _connection = new OleDbConnection { ConnectionString = connectionString };
    }

    protected override List<string> GetDatabases()
    {
        return new List<string>();
    }

    protected override List<string> GetTables()
    {
        var tables = new List<string>();

        // We only want user tables, not system tables
        string[] restrictions = new string[4];
        restrictions[3] = "Table";

        var userTables = _connection.GetSchema("Tables", restrictions);

        // Add list of table names to listBox
        for (int i = 0; i < userTables.Rows.Count; i++)
        {
            tables.Add(userTables.Rows[i][2].ToString());
        }

        return tables;
    }

    public override bool CheckConnect()
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
            if (ex.Message.Contains("The 'Microsoft.ACE.OleDb.12.0' provider is not registered"))
            {
                NotifyException(Resources.EXCEPTION_ACEOLEDB_NOTREGISTERED, null);
            }
            else
            {
                NotifyException(Resources.EXCEPTION_IO_CANNOT_CONNECT_TO_FILE, ex);
            }
        }
        return false;
    }

    private string ACEDriverVersion()
    {
        if (_aceVer == "error")
        {
            return "";
        }

        if (!string.IsNullOrEmpty(_aceVer))
        {
            return _aceVer;
        }

        try
        {
            var processInfo = new ProcessStartInfo("powershell.exe",
                "(New-Object system.data.oledb.oledbenumerator).GetElements() | select SOURCES_NAME, SOURCES_DESCRIPTION")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            var process = Process.Start(processInfo);
            if (process != null)
            {
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                var errors = process.StandardError.ReadToEnd();
                process.Close();
                if (string.IsNullOrEmpty(errors))
                {
                    var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Where(x => !string.IsNullOrEmpty(x));
                    var drv = lines.FirstOrDefault(x => x.Contains("ACE.OLEDB"));
                    if (string.IsNullOrEmpty(drv))
                    {
                        _aceVer = "error";
                        return "";
                    }

                    var pos = drv.IndexOf("ACE.OLEDB");
                    _aceVer = drv.Substring(pos + 10, 2);
                    return _aceVer;
                }
            }
        }
        catch
        {
            //
        }
        _aceVer = "12";
        return _aceVer;
    }

    private string GetSelectFields()
    {
        var sql = Options.Fields.Aggregate("", (current, fld) => current + $"[{fld.DatabaseName}],");
        return sql.Substring(0, sql.Length - 1);
    }
}