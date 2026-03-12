using Newtonsoft.Json;
using Snowflake.Client;
using Snowflake.Client.Json;
using Snowflake.Client.Model;
using System.Globalization;
using System.Threading.Tasks;
using WinPure.Common.Exceptions;
using WinPure.Integration.Abstractions;

namespace WinPure.Integration.Import;

internal class SnowflakeImportProvider : ImportProviderBase, IDatabaseImportProvider
{
    private SnowflakeImportExportOptions _options;
    private SnowflakeClient _snowflakeClient;
    private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

    public SnowflakeImportProvider() : base(ExternalSourceTypes.Snowflake)
    {
        DisplayName = "Snowflake";
    }

    public override void Initialize(object parameters)
    {
        _options = parameters as SnowflakeImportExportOptions;
        _snowflakeClient = new SnowflakeClient(_options.User, _options.Password, _options.Region);
    }

    protected override DataTable GetPreview(int rowToPreview)
    {
        _previewTable = new DataTable();
        var query = GetQuery(rowToPreview);

        _previewTable = FetchData(query);
        _previewTable.TableName = _options.TableName;
        return _previewTable;
    }

    protected override DataTable GetData()
    {
        try
        {
            ImportedInfo.DisplayName = _options.TableName;
            ImportedInfo.FileName = _options.TableName;

            NotifyProgress(Resources.CAPTION_IO_IMPORTING_DATA, 10);

            var query = GetQuery(_MAX_ROW_TO_IMPORT);

            var dataTable = FetchData(query);
            dataTable.TableName = _options.TableName;

            ImportedInfo.RowCount = dataTable.Rows.Count;
            ImportedInfo.ImportParameters = JsonConvert.SerializeObject(_options);
            LimitedRecords = ImportedInfo.RowCount == _MAX_ROW_TO_IMPORT;
            RemoveSkippedField(dataTable, _options.Fields);
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
            _logger.Debug("IMPORT FROM SNOWFLAKE ERROR", ex);
            NotifyException(Resources.EXCEPTION_IO_ERROR_ON_IMPORT, ex.InnerException ?? ex);
        }

        return null;
    }

    public override DataTable ImportData()
    {
        try
        {
            if (CheckConnect())
            {
                _options.Fields = ImportedInfo.Fields;
                var result = GetData();
                ImportedInfo.RowCount = result?.Rows.Count ?? 0;
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
        try
        {
            _options = JsonConvert.DeserializeObject<SnowflakeImportExportOptions>(parameterJson);
            ImportedInfo.Fields = _options.Fields;
            DataTable data;
            _snowflakeClient = new SnowflakeClient(_options.User, _options.Password, _options.Region);

            data = GetData();
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

    public bool CheckConnect()
    {
        try
        {
            var res = AsyncHelpers.RunSync(() => _snowflakeClient.InitNewSessionAsync());
            return res;
        }
        catch (Exception ex)
        {
            NotifyException(Resources.EXCEPTION_IO_CANNOT_CONNECT_TO_SERVER, ex.InnerException ?? ex);
            return false;
        }
    }

    public List<string> GetDatabaseList()
    {
        return new List<string>();
    }

    public List<string> GetDatabaseTables()
    {
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

    private string GetQuery(int rowToSelect)
    {
        var qry = $"SELECT * FROM {_options.Database}.{_options.Schema}.{_options.TableName}";
        qry += (rowToSelect > 0) ? $" LIMIT {rowToSelect}" : string.Empty;
        return qry;
    }

    private DataTable FetchData(string query)
    {
        var queryRawResponse = AsyncHelpers.RunSync(() => _snowflakeClient.QueryRawResponseAsync(query));

        var data = AsyncHelpers.RunSync(() => GetAllData(queryRawResponse));

        var dt = ConvertToDataTable(queryRawResponse.Columns, data);

        return dt;
    }

    private async Task<List<List<string>>> GetAllData(SnowflakeQueryRawResponse rawResponse)
    {
        if (rawResponse.Chunks == null)
        {
            return rawResponse.Rows;
        }

        var chunksDownloadInfo = new ChunksDownloadInfo
        {
            ChunkHeaders = rawResponse.ChunkHeaders,
            Chunks = rawResponse.Chunks,
            Qrmk = rawResponse.Qrmk
        };
        var parsed = await ChunksDownloader.DownloadAndParseChunksAsync(chunksDownloadInfo);

        return parsed;
    }

    private DataTable ConvertToDataTable(List<ColumnDescription> columns, List<List<string>> rows)
    {
        var result = new DataTable();
        for (int i = 0; i < columns.Count; i++)
        {
            var sfColumn = columns[i];
            result.Columns.Add(CreateColumn(sfColumn));
        }

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            var dataRow = result.NewRow();
            
            for (int j = 0; j < columns.Count; j++)
            {
                var columnType = columns[j].Type.ToUpper();
                if (row[j] == null)
                {
                    dataRow[j] = DBNull.Value;
                } else if (columnType == "DATE" || columnType == "TIME" || columnType == "TIMESTAMP_NTZ")
                {
                    dataRow[j] = ConvertToDateTime(row[j], columnType);
                } else if (columnType == "TIMESTAMP_LTZ" || columnType == "TIMESTAMP_TZ")
                {
                    dataRow[j] = ConvertToDateTimeOffset(row[j], columnType);
                }
                else if ((columnType == "FIXED" || columnType == "REAL") && (columns[j].Scale ?? 0) > 0)
                {
                    dataRow[j] = Convert.ToDecimal(row[j].ToString()
                        .Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                        .Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                }
                else
                {
                    dataRow[j] = row[j];
                }
            }

            result.Rows.Add(dataRow);
        }

        return result;
    }

    private static DataColumn CreateColumn(ColumnDescription columnDescription)
    {
        var dataColumn = new DataColumn(columnDescription.Name, ConvertSFDataType(columnDescription.Type, columnDescription.Scale));
        dataColumn.AllowDBNull = columnDescription.Nullable;
        return dataColumn;
    }

    private static Type ConvertSFDataType(string sfDataType, long? scale)
    {
        switch (sfDataType.ToUpper())
        {
            case "NONE":
            case "TEXT":
            case "OBJECT":
            case "BINARY":
            case "ARRAY":
            case "VARIANT":
                return typeof(string);

            case "FIXED":
            case "REAL":
                return (scale.HasValue && scale.Value > 0)
                    ? typeof(decimal)
                    : typeof(int);

            case "DATE":
            case "TIME":
            case "TIMESTAMP_NTZ":
                return typeof(DateTime);

            case "TIMESTAMP_LTZ":
            case "TIMESTAMP_TZ":
                return typeof(DateTimeOffset);

            default: return typeof(string);
        }
    }
    
    private DateTime ConvertToDateTime(string value, string snowflakeType)
    {
        switch (snowflakeType)
        {
            case "DATE":
                var srcValLong = long.Parse(value);
                return UnixEpoch.AddDays(srcValLong);

            case "TIME": // to timespan? https://github.com/snowflakedb/snowflake-connector-net/issues/327
                         // https://github.com/snowflakedb/snowflake-connector-net/commit/1fa03d92cdf6f7ae5720fdef8ecf25371f0f4c95
            case "TIMESTAMP_NTZ":
                var secAndNsecTuple = ExtractTimestamp(value);
                var tickDiff = secAndNsecTuple.Item1 * 10000000L + secAndNsecTuple.Item2 / 100L;
                return UnixEpoch.AddTicks(tickDiff);

            default:
                throw new SnowflakeException($"Conversion from {snowflakeType} to DateTime is not supported.");
        }
    }

    private DateTimeOffset ConvertToDateTimeOffset(string value, string snowflakeType)
    {
        switch (snowflakeType)
        {
            case "TIMESTAMP_TZ":
                var spaceIndex = value.IndexOf(' ');
                var offset = int.Parse(value.Substring(spaceIndex + 1, value.Length - spaceIndex - 1));
                var offSetTimespan = new TimeSpan((offset - 1440) / 60, 0, 0);

                var secAndNsecTzPair = ExtractTimestamp(value.Substring(0, spaceIndex));
                var ticksTz = (secAndNsecTzPair.Item1 * 1000 * 1000 * 1000 + secAndNsecTzPair.Item2) / 100;
                return new DateTimeOffset(UnixEpoch.Ticks + ticksTz, TimeSpan.Zero).ToOffset(offSetTimespan);

            case "TIMESTAMP_LTZ":
                var secAndNsecLtzPair = ExtractTimestamp(value);
                var ticksLtz = (secAndNsecLtzPair.Item1 * 1000 * 1000 * 1000 + secAndNsecLtzPair.Item2) / 100;
                return new DateTimeOffset(UnixEpoch.Ticks + ticksLtz, TimeSpan.Zero).ToLocalTime();

            default:
                throw new SnowflakeException($"Conversion from {snowflakeType} to DateTimeOffset is not supported.");
        }
    }

    private Tuple<long, long> ExtractTimestamp(string srcVal)
    {
        var dotIndex = srcVal.IndexOf('.');

        if (dotIndex == -1)
            return Tuple.Create(long.Parse(srcVal), 0L);

        var intPart = long.Parse(srcVal.Substring(0, dotIndex));
        var decimalPartLength = srcVal.Length - dotIndex - 1;
        var decimalPartStr = srcVal.Substring(dotIndex + 1, decimalPartLength);
        var decimalPart = long.Parse(decimalPartStr);

        // If the decimal part contained less than nine characters, we must convert the value to nanoseconds by
        // multiplying by 10^[precision difference].
        if (decimalPartLength < 9)
        {
            decimalPart *= (int)Math.Pow(10, 9 - decimalPartLength);
        }

        return Tuple.Create(intPart, decimalPart);
    }
}