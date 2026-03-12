using System.Text.RegularExpressions;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using DevExpress.XtraTab;
using Newtonsoft.Json;
using WinPure.Configuration.Helper;

namespace WinPure.CleanAndMatch.Helpers;

internal static class DataHelper
{
    internal static string GetSqliteFilterCriteriaFromGridView(GridView view)
    {
        // Return null if there is no active filter
        if (view?.ActiveFilter == null || !view.ActiveFilterEnabled || view.ActiveFilter.Expression == "")
            return null;

        var filterString = CriteriaToWhereClauseHelper.GetMsSqlWhere(view.ActiveFilterCriteria)
            .Replace(" N'", " '")
            .Replace("or len(", "or length(")
            .Replace("and len(", "and length(")
            .ProcessContainsConditions()
            .ProcessEndWithConditions()
            .ProcessRemoveNFromInConditions()
            .ReplaceConvertDateTime();

        return filterString;
    }

    internal static object GetData(this string tableName, string whereCondition = "")
    {
        // Resolve main DB connection manager
        var cm = WinPureUiDependencyResolver.Resolve<IConnectionManager>();
        return CreateCollectionSource(tableName, cm, WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY, whereCondition);
    }

    internal static object GetLogData(this string tableName, string whereCondition = "")
    {
        // Resolve log DB connection manager
        var logCm = WinPureUiDependencyResolver.Resolve<ILogConnectionManager>();
        return CreateCollectionSource(tableName, logCm, "Id", whereCondition);
    }

    private static XPServerCollectionSource CreateCollectionSource(
        string tableName,
        IConnectionManager connectionManager,
        string keyField,
        string whereCondition)
    {
        // Ensure the underlying ADO.NET connection is ready
        connectionManager.CheckConnectionState();

        // Get schema only (no rows). We only need columns and PK metadata to build the XPO class.
        var sql = SqLiteHelper.GetSelectQuery(tableName) + " LIMIT 0";
        var table = SqLiteHelper.ExecuteQuery(sql, connectionManager.Connection, CommandBehavior.SchemaOnly, tableName);

        var connectionString = SQLiteConnectionProvider.GetConnectionString(connectionManager.DbPath);

        XpoDbContext db = null;
        XPServerCollectionSource ds = null;

        try
        {
            // Build dictionary (AddClass) FIRST, then create DataLayer/Session.
            db = XpoDbContext.Create(connectionString, dict => AddClass(dict, table, keyField));

            // Resolve ClassInfo by TableName from the built dictionary
            var classInfo = FindClassInfoByTableName(db.Dictionary, table.TableName)
                            ?? throw new InvalidOperationException($"ClassInfo for table '{table.TableName}' was not created.");

            // Create the server collection source over this Session
            if (string.IsNullOrWhiteSpace(whereCondition))
            {
                ds = new XPServerCollectionSource(db.Session, classInfo)
                {
                    AllowEdit = true,
                    AllowRemove = true,
                    DeleteObjectOnRemove = true
                };
            }
            else
            {
                var criteria = CriteriaOperator.Parse(whereCondition);
                ds = new XPServerCollectionSource(db.Session, classInfo, criteria)
                {
                    AllowEdit = true,
                    AllowRemove = true,
                    DeleteObjectOnRemove = true
                };
            }
            ds.DisplayableProperties = BuildDisplayableProperties(classInfo);

            // Register owner so later we can dispose Session/DataLayer together with the data source.
            XpoCollectionHelper.Register(db.Session, db);
            return ds;
        }
        catch
        {
            try { ds?.Dispose(); } catch { /* ignore */ }
            try { db?.Dispose(); } catch { /* ignore */ }
            throw;
        }
    }

    private static XPClassInfo AddClass(XPDictionary dict, DataTable table, string keyField)
    {
        if (table.PrimaryKey.Length > 1)
            throw new NotSupportedException(Resources.EXCEPTION_PRIMARY_KEY_NOT_SUPPORTED);
        var classInfo = dict.CollectClassInfos(true).FirstOrDefault(x => x.TableName == table.TableName);
        if (classInfo != null)
        {
            classInfo.Members.Clear();
        }
        else
        {
            classInfo = dict.CreateClass(dict.GetClassInfo(typeof(BasePersistentClass)), table.TableName);
            classInfo.AddAttribute(new PersistentAttribute(table.TableName));
        }

        foreach (DataColumn col in table.Columns)
        {
            if (col.ColumnName == keyField)
                classInfo.CreateMember(col.ColumnName, col.DataType, new KeyAttribute());
            else
                classInfo.CreateMember(col.ColumnName, col.DataType);
        }
        return classInfo;
    }

    [NonPersistent]
    public class BasePersistentClass : XPLiteObject
    {
        public BasePersistentClass(Session session) : base(session)
        {
        }

        public BasePersistentClass(Session session, XPClassInfo classInfo) : base(session, classInfo)
        {
        }
    }

    private static XPClassInfo FindClassInfoByTableName(XPDictionary dictionary, string tableName)
    {
        foreach (XPClassInfo ci in dictionary.Classes)
            if (string.Equals(ci.TableName, tableName, StringComparison.Ordinal))
                return ci;
        return null;
    }

    // Build DisplayableProperties from persistent scalar members + "This" + Key
    static string BuildDisplayableProperties(XPClassInfo classInfo)
    {
        var names = new List<string>();

        // "This" must go first to ensure the whole object is fetched (important in server mode)
        names.Add("This");

        // Make sure Key is included (some grids rely on it for editing/focus)
        if (classInfo.KeyProperty != null)
            names.Add(classInfo.KeyProperty.Name);

        // Include only persistent, non-association, non-reference members
        foreach (var m in classInfo.Members)
        {
            if (!m.IsPersistent) continue;
            if (m == classInfo.KeyProperty) continue; // already added
            if (m.IsAssociation) continue;
            if (m.ReferenceType != null) continue;
            names.Add(m.Name);
        }

        return string.Join(";", names);
    }

    internal static void SetTabIconAndTooltip(XtraTabPage page, ExternalSourceTypes sourceType, string importParameters)
    {
        switch (sourceType)
        {
            case ExternalSourceTypes.TextFile:
                var textParameters = JsonConvert.DeserializeObject<TextImportExportOptions>(importParameters);
                page.Tooltip = textParameters.FilePath;
                page.ImageOptions.Image = Resources.txtcsv_16;
                break;
            case ExternalSourceTypes.SqlServer:
                page.ImageOptions.Image = Resources.sql_server_16;
                break;
            case ExternalSourceTypes.MySqlServer:
                page.ImageOptions.Image = Resources.MySQL_16;
                break;
            case ExternalSourceTypes.Excel:
                var excelParameters = JsonConvert.DeserializeObject<ExcelImportExportOptions>(importParameters);
                page.Tooltip = excelParameters.FilePath;
                page.ImageOptions.Image = Resources.excel_16;
                break;
            case ExternalSourceTypes.Access:
                var accessParameters = JsonConvert.DeserializeObject<SqlImportExportOptions>(importParameters);
                page.Tooltip = accessParameters.DatabaseFile;
                page.ImageOptions.Image = Resources.access_16;
                break;
            case ExternalSourceTypes.Oracle:
                page.ImageOptions.Image = Resources.oracle_16;
                break;
            case ExternalSourceTypes.SqLite:
                var sqliteParameters = JsonConvert.DeserializeObject<SqlImportExportOptions>(importParameters);
                page.Tooltip = sqliteParameters.DatabaseFile;
                page.ImageOptions.Image = Resources.sqlite_16;
                break;
            case ExternalSourceTypes.DataTable:
                page.ImageOptions.Image = Resources.datatable_16;
                break;
            case ExternalSourceTypes.Json:
                var jsonParameters = JsonConvert.DeserializeObject<TextImportExportOptions>(importParameters);
                page.Tooltip = jsonParameters.FilePath;
                page.ImageOptions.Image = Resources.json_16;
                break;
            case ExternalSourceTypes.Xml:
                var xmlParameters = JsonConvert.DeserializeObject<TextImportExportOptions>(importParameters);
                page.Tooltip = xmlParameters.FilePath;
                page.ImageOptions.Image = Resources.xml_16;
                break;
            case ExternalSourceTypes.AzureDb:
                page.ImageOptions.Image = Resources.azure_16;
                break;
            case ExternalSourceTypes.Postgres:
                page.ImageOptions.Image = Resources.postgre_16;
                break;
            case ExternalSourceTypes.ZohoCrm:
                page.ImageOptions.Image = Resources.zoho_16;
                break;
            case ExternalSourceTypes.Salesforce:
                page.ImageOptions.Image = Resources.salesforce_16;
                break;
            case ExternalSourceTypes.Snowflake:
                page.ImageOptions.Image = Resources.Snowflake_16;
                break;
            default:
                page.ImageOptions.Image = Resources.txtcsv_16;
                break;
        }
    }

    // --- Your string filter helpers (unchanged) ---

    private static string TransformContainsToWhereClause(string devExpressCondition)
    {
        var match = Regex.Match(devExpressCondition, @"CharIndEX\(N'(.*?)', ""(.*?)""\)");
        if (match.Success)
        {
            string searchValue = match.Groups[1].Value;
            string columnName = match.Groups[2].Value;
            return $" [{columnName}] LIKE '%{searchValue}%' ";
        }
        return null;
    }

    private static string ProcessContainsConditions(this string filterString)
    {
        var containsConditions = FindContainsConditionsInText(filterString);
        foreach (var condition in containsConditions)
        {
            var sqlCondition = TransformContainsToWhereClause(condition);
            filterString = filterString.Replace(condition, sqlCondition);
        }
        return filterString;
    }

    private static string ProcessEndWithConditions(this string filterString)
    {
        var containsConditions = FindEndWithConditionsInText(filterString);
        foreach (var condition in containsConditions)
        {
            var sqlCondition = TransformEndWithToWhereClause(condition);
            filterString = filterString.Replace(condition, sqlCondition);
        }
        return filterString;
    }

    private static List<string> FindContainsConditionsInText(string text)
    {
        var pattern = "isnuLL\\(CharIndEX\\(N'.*?', \\\".*?\\\"\\), 0\\) > 0";
        var matches = Regex.Matches(text, pattern);
        var results = new List<string>();
        foreach (Match match in matches)
            results.Add(match.Value);
        return results;
    }

    private static string TransformEndWithToWhereClause(string sqlCondition)
    {
        var match = Regex.Match(sqlCondition, @"RigHt\(""(.*?)"", DATALENGTH\(Cast\(N'(.*?)' as ntext\)\) / 2\) = \(N'(.*?)'\)");
        if (match.Success)
        {
            string columnName = match.Groups[1].Value;
            string comparisonValue = match.Groups[3].Value;
            return $" [{columnName}] LIKE '%{comparisonValue}' ";
        }
        return null;
    }

    private static List<string> FindEndWithConditionsInText(string text)
    {
        var pattern = @"RigHt\("".*?"", DATALENGTH\(Cast\(N'.*?' as ntext\)\) / 2\) = \(N'.*?'\)";
        var matches = Regex.Matches(text, pattern);
        var results = new List<string>();
        foreach (Match match in matches)
            results.Add(match.Value);
        return results;
    }

    private static string ReplaceConvertDateTime(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Regex: convert(datetime, 'DATE_STRING', style)
        var pattern = @"convert\s*\(\s*datetime\s*,\s*'([^']+)'\s*,\s*\d+\s*\)";
        return Regex.Replace(input, pattern, m => $"'{m.Groups[1].Value}'", RegexOptions.IgnoreCase);
    }
    
    private static string ProcessRemoveNFromInConditions(this string filterString)
    {
        var inConditions = FindInConditionsWithN(filterString);
        foreach (var condition in inConditions)
        {
            var sqlCondition = RemoveNFromInCondition(condition);
            filterString = filterString.Replace(condition, sqlCondition);
        }
        return filterString;
    }

    private static List<string> FindInConditionsWithN(string text)
    {
        var pattern = "\\\".*?\\\" in \\(" + "N'.*?'(?:, N'.*?')*\\)";
        var matches = Regex.Matches(text, pattern);
        var results = new List<string>();
        foreach (Match match in matches)
            results.Add(match.Value);
        return results;
    }

    private static string RemoveNFromInCondition(string condition)
    {
        // Remove the "N" prefix from each string value inside the IN clause
        return Regex.Replace(condition, "N'(.*?)'", "'$1'");
    }
}
