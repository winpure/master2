using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using WinPure.Cleansing.Enums;
using WinPure.Configuration.Helper;

namespace WinPure.Cleansing.DependencyInjection;

internal static partial class WinPureCleansingDependencyExtension
{
    private class CleansingConfigurationService : ICleansingConfigurationService
    {
        public static List<string> TextCleanSettingsFields => new List<string> { "RM_Others", "RE_Expression", "RE_Replace", "RE_Copy", "CV_EmptyToDefault" };

        private readonly IWpLogger _logger;
        private readonly IConnectionManager _connectionManager;
        private readonly IConfigurationService _configurationService;

        public CleansingConfigurationService(IWpLogger logger, IConnectionManager connectionManager, IConfigurationService configurationService)
        {
            _logger = logger;
            _connectionManager = connectionManager;
            _configurationService = configurationService;
        }

        /// <summary>
        /// Export clean settings to json file
        /// </summary>
        /// <param name="settings">WinPure clean settings</param>
        /// <param name="destinationPath">Destination folder</param>
        /// <param name="fileName">File name (without extension).</param>
        public void ExportCleanSettings(WinPureCleanSettings settings, string fileName)
        {
            if (settings == null)
            {
                throw new WinPureArgumentException(Resources.EXCEPTION_SETTINGSNOTNULL);
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new WinPureArgumentException(Resources.EXCEPTION_PATHANDFILE);
            }

            try
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                var fileContent = JsonConvert.SerializeObject(settings);
                FileHelper.CreateOrOverrideFile(fileName, fileContent);
            }
            catch (Exception exception)
            {
                _logger?.Error("Cannot export settings to file", exception);
                throw new WinPureCannotProcessFile(Resources.EXCEPTION_EXPORT_SETTING_ERROR, exception);
            }
        }

        /// <summary>
        /// Import WinPure clean settings from json file
        /// </summary>
        /// <param name="destinationFile">Json file with settings</param>
        /// <returns></returns>
        public WinPureCleanSettings ImportCleanSettings(string destinationFile)
        {
            try
            {
                var content = FileHelper.ReadFile(destinationFile);
                var settings = JsonConvert.DeserializeObject<WinPureCleanSettings>(content);
                return settings;
            }
            catch (Exception exception)
            {
                _logger?.Error("Cannot import settings to file", exception);
                throw new WinPureCannotProcessFile(Resources.EXCEPTION_IMPORT_SETTING_ERROR, exception);
            }
        }

        public void FillCleansingConfigurationTables(DataTable dt)
        {
            CreateCleanSettingsTable(NameHelper.GetCleanSettingsTable(dt.TableName));
            CreateWordManagerTable(NameHelper.GetWordManagerTable(dt.TableName));
            CreateStatisticResultTable(NameHelper.GetStatisticTable(dt.TableName));
            CreateColumnValuesTable(NameHelper.GetColumnValuesTable(dt.TableName));

            foreach (DataColumn dc in dt.Columns)
            {
                SqLiteHelper.ExecuteNonQuery($"INSERT INTO [{NameHelper.GetCleanSettingsTable(dt.TableName)}] (COLUMN_NAME) VALUES ('{dc.ColumnName}');", _connectionManager.Connection);
            }
        }

        public DataTable GetDataTableCleanSettings(ImportedDataInfo importedDataInfo)
        {
            return SqLiteHelper.ExecuteQuery(SqLiteHelper.GetCleanSettingsQuery(importedDataInfo.TableName), _connectionManager.Connection, CommandBehavior.Default, importedDataInfo.TableName);
        }

        public DataTable ConvertCleanSettingsToDataTable(ImportedDataInfo importedDataInfo, WinPureCleanSettings cleanSettings)
        {
            var cleanSettingsTable = SqLiteHelper.ExecuteQuery(SqLiteHelper.GetCleanSettingsQuery(importedDataInfo.TableName), _connectionManager.Connection, CommandBehavior.SchemaOnly, importedDataInfo.TableName);

            DataRow GetColumnRow(string columnName)
            {
                var row = cleanSettingsTable.Rows.OfType<DataRow>().FirstOrDefault(x => x["COLUMN_NAME"].ToString() == columnName);
                if (row == null)
                {
                    row = cleanSettingsTable.NewRow();
                    row["COLUMN_NAME"] = columnName;
                    cleanSettingsTable.Rows.Add(row);
                    row = cleanSettingsTable.Rows.OfType<DataRow>().FirstOrDefault(x => x["COLUMN_NAME"].ToString() == columnName);
                }

                return row;
            }

            foreach (var converterSetting in cleanSettings.CaseConverterSettings)
            {
                var row = GetColumnRow(converterSetting.ColumnName);
                row["ST_LowerCase"] = Convert.ToInt32(converterSetting.ToLowerCase);
                row["ST_UpperCase"] = Convert.ToInt32(converterSetting.ToUpperCase);
                row["ST_ProperCase"] = Convert.ToInt32(converterSetting.ToProperCase);
            }

            foreach (var textCleanerSetting in cleanSettings.TextCleanerSettings)
            {
                var row = GetColumnRow(textCleanerSetting.ColumnName);
                row["RM_TrailingSpaces"] = Convert.ToInt32(textCleanerSetting.RemoveTrailingSpace);
                row["RM_Commas"] = Convert.ToInt32(textCleanerSetting.RemoveCommas);
                row["RM_Dots"] = Convert.ToInt32(textCleanerSetting.RemoveDots);
                row["RM_Hyphens"] = Convert.ToInt32(textCleanerSetting.RemoveHyphens);
                row["RM_Apostrophes"] = Convert.ToInt32(textCleanerSetting.RemoveApostrophes);
                row["RM_LeadingSpaces"] = Convert.ToInt32(textCleanerSetting.RemoveLeadingSpace);
                row["RM_Numbers"] = Convert.ToInt32(textCleanerSetting.RemoveAllDigits);
                row["RM_Letters"] = Convert.ToInt32(textCleanerSetting.RemoveAllLetters);
                row["CV_0sToNothing"] = Convert.ToInt32(textCleanerSetting.ConvertOsToNaughts);
                row["CV_LsTo1s"] = Convert.ToInt32(textCleanerSetting.ConvertLsToOnes);
                row["CV_NoughtsTo0s"] = Convert.ToInt32(textCleanerSetting.ConvertNaughtsToOs);
                row["CV_1sToLs"] = Convert.ToInt32(textCleanerSetting.ConvertOnesToLs);
                row["RM_Spaces"] = Convert.ToInt32(textCleanerSetting.RemoveAllSpaces);
                row["RM_Nonprintable"] = Convert.ToInt32(textCleanerSetting.RemoveNonPrintableCharacters);
                row["RM_MultiSpaces"] = Convert.ToInt32(textCleanerSetting.RemoveMultipleSpaces);
                row["RM_Tab"] = Convert.ToInt32(textCleanerSetting.RemoveTabs);
                row["RM_NewLine"] = Convert.ToInt32(textCleanerSetting.RemoveNewLine);
                row["CV_EmptyToDefault"] = textCleanerSetting.ConvertEmptyToDefaultValue;
                row["RM_Others"] = textCleanerSetting.RemoveCharacters;
                row["RE_Expression"] = textCleanerSetting.RegexExpression;
                row["RE_Replace"] = textCleanerSetting.RegexReplace;
            }

            foreach (var checkSetting in cleanSettings.ColumnCheckSettings)
            {
                var row = GetColumnRow(checkSetting.ColumnName);
                row["CHK_Email"] = Convert.ToInt32(checkSetting.CheckEmail);
            }

            foreach (var mergeSetting in cleanSettings.ColumnMergeSettings)
            {
                var row = GetColumnRow(mergeSetting.ColumnName);
                row["Merger_order"] = mergeSetting.Order;
            }

            foreach (var shiftSetting in cleanSettings.ColumnShiftSettings)
            {
                var row = GetColumnRow(shiftSetting.ColumnName);
                if (shiftSetting.ShiftLeft)
                {
                    row["SH_LEFT"] = 1;
                }
                else
                {
                    row["SH_RIGHT"] = 1;
                }
            }

            foreach (var splitSetting in cleanSettings.ColumnSplitSettings)
            {
                var row = GetColumnRow(splitSetting.ColumnName);
                row["SP_EmailAddress"] = Convert.ToInt32(splitSetting.SplitNameAndEmailAddress);
                row["SP_Email"] = Convert.ToInt32(splitSetting.SplitEmailAddressIntoAccountDomainAndZone);
                row["SP_DateTime"] = Convert.ToInt32(splitSetting.SplitDatetime);
                row["RE_Copy"] = splitSetting.RegexCopy;
                row["SP_Telephones"] = Convert.ToInt32(splitSetting.SplitTelephoneIntoInternationalCodeAndPhoneNumber);
                row["SP_Words"] = Convert.ToInt32(splitSetting.SplitIntoWords);
            }

            return cleanSettingsTable;
        }

        //TODO include Word manager to this settings
        public WinPureCleanSettings GetWinPureCleanSettings(ImportedDataInfo importedDataInfo)
        {
            var cleanSettingsDataTable = GetDataTableCleanSettings(importedDataInfo);
            var cleanSettings = new WinPureCleanSettings();
            int shiftIndex = 0;

            foreach (DataRow rw in cleanSettingsDataTable.Rows)
            {
                if (rw["ST_UpperCase"].ToString() == "1" || rw["ST_LowerCase"].ToString() == "1" || rw["ST_ProperCase"].ToString() == "1")
                {
                    var ccSett = new CaseConverterSetting
                    {
                        ColumnName = rw["COLUMN_NAME"].ToString(),
                        ToLowerCase = rw["ST_LowerCase"].ToString() == "1",
                        ToUpperCase = rw["ST_UpperCase"].ToString() == "1",
                        ToProperCase = rw["ST_ProperCase"].ToString() == "1"
                    };
                    cleanSettings.CaseConverterSettings.Add(ccSett);
                }

                if (rw["RM_TrailingSpaces"].ToString() == "1" || rw["RM_Commas"].ToString() == "1" || rw["RM_Dots"].ToString() == "1"
                    || rw["RM_Hyphens"].ToString() == "1" || rw["RM_Apostrophes"].ToString() == "1" || rw["RM_LeadingSpaces"].ToString() == "1"
                    || rw["RM_Letters"].ToString() == "1" || rw["RM_Numbers"].ToString() == "1" || rw["RM_Others"].ToString() != ""
                    || rw["RM_Nonprintable"].ToString() == "1" || rw["RM_Spaces"].ToString() == "1" || rw["CV_0sToNothing"].ToString() == "1"
                    || rw["CV_LsTo1s"].ToString() == "1" || rw["CV_NoughtsTo0s"].ToString() == "1" || rw["CV_1sToLs"].ToString() == "1"
                    || rw["CV_EmptyToDefault"].ToString() != ""
                    || rw["RM_MultiSpaces"].ToString() == "1" || rw["RM_Tab"].ToString() == "1" || rw["RM_NewLine"].ToString() == "1"
                    || rw["RE_Expression"].ToString().Trim() != "")
                {
                    var cleanSett = new TextCleanerSetting
                    {
                        ColumnName = rw["COLUMN_NAME"].ToString(),
                        RemoveTrailingSpace = rw["RM_TrailingSpaces"].ToString() == "1",
                        RemoveCommas = rw["RM_Commas"].ToString() == "1",
                        RemoveDots = rw["RM_Dots"].ToString() == "1",
                        RemoveHyphens = rw["RM_Hyphens"].ToString() == "1",
                        RemoveApostrophes = rw["RM_Apostrophes"].ToString() == "1",
                        RemoveLeadingSpace = rw["RM_LeadingSpaces"].ToString() == "1",
                        RemoveAllDigits = rw["RM_Numbers"].ToString() == "1",
                        RemoveAllLetters = rw["RM_Letters"].ToString() == "1",
                        ConvertOsToNaughts = rw["CV_0sToNothing"].ToString() == "1",
                        ConvertLsToOnes = rw["CV_LsTo1s"].ToString() == "1",
                        ConvertNaughtsToOs = rw["CV_NoughtsTo0s"].ToString() == "1",
                        ConvertOnesToLs = rw["CV_1sToLs"].ToString() == "1",
                        ConvertEmptyToDefaultValue = rw["CV_EmptyToDefault"].ToString(),
                        RemoveCharacters = rw["RM_Others"].ToString(),
                        RemoveAllSpaces = rw["RM_Spaces"].ToString() == "1",
                        RemoveNonPrintableCharacters = rw["RM_Nonprintable"].ToString() == "1",
                        RemoveMultipleSpaces = rw["RM_MultiSpaces"].ToString() == "1",
                        RemoveTabs = rw["RM_Tab"].ToString() == "1",
                        RemoveNewLine = rw["RM_NewLine"].ToString() == "1",
                        RegexExpression = rw["RE_Expression"].ToString().Trim(),
                        RegexReplace = rw["RE_Replace"].ToString().Trim()
                    };

                    cleanSettings.TextCleanerSettings.Add(cleanSett);
                }

                if (rw["CHK_Email"].ToString() == "1")
                {
                    cleanSettings.ColumnCheckSettings.Add(new ColumnCheckSettings
                    {
                        ColumnName = rw["COLUMN_NAME"].ToString(),
                        CheckEmail = true
                    });
                }

                if (rw["Merger"].ToString() == "1")
                {
                    cleanSettings.ColumnMergeSettings.Add(new ColumnMergeSetting
                    {
                        ColumnName = rw["COLUMN_NAME"].ToString(),
                        CharToInsertBetweenColumn = _configurationService.Configuration.CleanMergeSeparator,
                        Order = Convert.ToInt32(rw["Merger_order"])
                    });
                }

                if (rw["SH_LEFT"].ToString() == "1" || rw["SH_RIGHT"].ToString() == "1")
                {
                    var shSett = new ColumnShiftSetting
                    {
                        ColumnName = rw["COLUMN_NAME"].ToString(),
                        ShiftLeft = rw["SH_LEFT"].ToString() == "1",
                        SourceIndex = shiftIndex++
                    };
                    cleanSettings.ColumnShiftSettings.Add(shSett);
                }

                if (rw["SP_EmailAddress"].ToString() == "1" || rw["SP_Email"].ToString() == "1" || rw["SP_Telephones"].ToString() == "1" || rw["SP_DateTime"].ToString() == "1" || rw["SP_Words"].ToString() == "1" || rw["RE_Copy"].ToString().Trim() != "")
                {
                    var csSett = new ColumnSplitSetting
                    {
                        ColumnName = rw["COLUMN_NAME"].ToString(),
                        RegexCopy = rw["RE_Copy"].ToString().Trim(),
                        SplitNameAndEmailAddress = rw["SP_EmailAddress"].ToString() == "1",
                        SplitEmailAddressIntoAccountDomainAndZone = rw["SP_Email"].ToString() == "1",
                        SplitDatetime = rw["SP_DateTime"].ToString() == "1",
                        SplitTelephoneIntoInternationalCodeAndPhoneNumber = rw["SP_Telephones"].ToString() == "1"
                    };

                    if (rw["SP_Words"].ToString() == "1")
                    {
                        csSett.SplitIntoWords = new SplitIntoWords
                        {
                            SplitSeparator = (_configurationService.Configuration.CleanSplitSeparator == "\\n")
                                ? "\n"
                                : (_configurationService.Configuration.CleanSplitSeparator == "\\t\\n")
                                    ? "\t\n"
                                    : (_configurationService.Configuration.CleanSplitSeparator == "\\t")
                                        ? "\t"
                                        : _configurationService.Configuration.CleanSplitSeparator,
                        };
                    }

                    cleanSettings.ColumnSplitSettings.Add(csSett);
                }
            }

            var tableWM = SqLiteHelper.ExecuteQuery(SqLiteHelper.GetSelectQuery(NameHelper.GetWordManagerTable(importedDataInfo.TableName)), _connectionManager.Connection);

            foreach (DataRow r in tableWM.Rows)
            {
                var wmSett = new WordManagerSetting();
                wmSett.ColumnName = r["COLUMN_NAME"].ToString();
                wmSett.SearchValue = r["COLUMN_VALUE"].ToString();
                wmSett.ReplaceValue = r["ToReplace"].ToString();
                wmSett.ToDelete = r["ToDelete"].ToString() == "1";
                wmSett.ReplaceType = (WordManagerReplaceType)Convert.ToInt32(r["ReplaceOption"].ToString());
                cleanSettings.WordManagerSettings.Add(wmSett);
            }

            if (cleanSettings.ColumnMergeSettings.Count == 1) cleanSettings.ColumnMergeSettings.Clear(); // one column we should not merge

            return cleanSettings;
        }

        public void ClearCleanSettings(ImportedDataInfo importedDataInfo)
        {
            _connectionManager.CheckConnectionState();

            var tableName = importedDataInfo.TableName;
            var sql = "UPDATE [" + NameHelper.GetCleanSettingsTable(tableName) + "] SET RM_TrailingSpaces = 0, RM_Commas = 0, RM_Dots = 0,RM_Hyphens = 0,RM_Apostrophes = 0,RM_LeadingSpaces = 0,RM_Letters = 0,RM_Nonprintable = 0,RM_Spaces = 0,RM_Numbers = 0,RM_MultiSpaces=0,RM_NewLine=0,RM_Tab=0, CV_EmptyToDefault = '', RM_Others = '',CV_0sToNothing = 0,CV_LsTo1s = 0,CV_NoughtsTo0s = 0,CV_1sToLs = 0,ST_UpperCase = 0,ST_LowerCase = 0,ST_ProperCase = 0,SP_EmailAddress = 0,SP_Email = 0,SP_Telephones = 0,SP_DateTime = 0,SP_Words = 0,Merger = 0,SH_LEFT = 0,SH_RIGHT = 0,RE_Expression='',RE_Replace='',RE_Copy='',Merger_order = 0,CHK_Email = 0";
            SqLiteHelper.ExecuteNonQuery(sql, _connectionManager.Connection);

            sql = SqLiteHelper.GetDeleteQuery(NameHelper.GetWordManagerTable(tableName), $"[TABLE_NAME]='{tableName}'");
            SqLiteHelper.ExecuteNonQuery(sql, _connectionManager.Connection);

        }

        public void SaveCleanSettings(ImportedDataInfo importedDataInfo, string columnName, string fieldName, object value)
        {
            _connectionManager.CheckConnectionState();

            var sql = TextCleanSettingsFields.Contains(fieldName)
                ? $"UPDATE [{NameHelper.GetCleanSettingsTable(importedDataInfo.TableName)}] SET {fieldName}='{value.ToString().Replace("'", "''")}' WHERE COLUMN_NAME='{columnName}'"
                : $"UPDATE [{NameHelper.GetCleanSettingsTable(importedDataInfo.TableName)}] SET {fieldName}={value} WHERE COLUMN_NAME='{columnName}'";

            SqLiteHelper.ExecuteNonQuery(sql, _connectionManager.Connection);
        }

        public DataTable GetColumnUniqueValues(ImportedDataInfo importedDataInfo, string columnName, bool refreshData, TextCleanerSetting cleanerSett = null, CaseConverterSetting caseSett = null)
        {
            _connectionManager.CheckConnectionState();

            var sql = SqLiteHelper.GetSelectQuery(NameHelper.GetColumnValuesTable(importedDataInfo.TableName), "[COLUMN_VALUE], [VALUE_COUNT]", $"[COLUMN_NAME]='{columnName}'");
            var res = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection);
            if (res.Rows.Count == 0 && refreshData)
            {
                UpdateUniqueValuesForColumn(importedDataInfo.TableName, columnName);
                res = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection);
            }

            if (cleanerSett != null)
            {
                var qry = res.AsEnumerable().Where(x => x.Field<string>("COLUMN_VALUE") != "" && x.Field<object>("COLUMN_VALUE") != null).AsQueryable();
                if (cleanerSett.RemoveNewLine)
                {
                    qry = qry.Where(x => x.Field<string>("COLUMN_VALUE").Contains(Environment.NewLine));
                }
                if (cleanerSett.RemoveTrailingSpace)
                {
                    qry = qry.Where(x => x.Field<string>("COLUMN_VALUE").EndsWith(" "));
                }
                if (cleanerSett.RemoveMultipleSpaces)
                {
                    qry = qry.Where(x => Regex.IsMatch(x.Field<string>("COLUMN_VALUE"), @"[ ]{2,}"));
                }
                if (cleanerSett.RemoveLeadingSpace)
                {
                    qry = qry.Where(x => x.Field<string>("COLUMN_VALUE").StartsWith(" "));
                }
                if (cleanerSett.RemoveAllSpaces)
                {
                    qry = qry.Where(x => x.Field<string>("COLUMN_VALUE").Contains(" "));
                }
                if (cleanerSett.RemoveCommas)
                {
                    qry = qry.Where(x => x.Field<string>("COLUMN_VALUE").Contains(","));
                }
                if (cleanerSett.RemoveDots)
                {
                    qry = qry.Where(x => x.Field<string>("COLUMN_VALUE").Contains("."));
                }
                if (cleanerSett.RemoveHyphens)
                {
                    qry = qry.Where(x => x.Field<string>("COLUMN_VALUE").Contains("-"));
                }
                if (cleanerSett.RemoveApostrophes)
                {
                    qry = qry.Where(x => x.Field<string>("COLUMN_VALUE").Contains("'"));
                }
                if (cleanerSett.RemoveTabs)
                {
                    qry = qry.Where(x => x.Field<string>("COLUMN_VALUE").Contains("\t"));
                }
                if (cleanerSett.RemovePunctuation)
                {
                    qry = qry.Where(x => Regex.IsMatch(x.Field<string>("COLUMN_VALUE"), @"\p{P}"));
                }
                if (cleanerSett.RemoveNonPrintableCharacters)
                {
                    qry = qry.Where(x => x.Field<string>("COLUMN_VALUE").Any(char.IsControl));
                }
                if (cleanerSett.RemoveAllLetters)
                {
                    qry = qry.Where(x => Regex.IsMatch(x.Field<string>("COLUMN_VALUE"), @"[a-zA-Z]+"));
                }
                if (cleanerSett.RemoveAllDigits)
                {
                    qry = qry.Where(x => Regex.IsMatch(x.Field<string>("COLUMN_VALUE"), @"[0-9]"));
                }
                res = (qry.Any()) ? qry.CopyToDataTable() : res.Clone();
            }
            if (caseSett != null)
            {
                var qry = res.AsEnumerable().Where(x => x.Field<string>("COLUMN_VALUE") != "" && x.Field<object>("COLUMN_VALUE") != null).AsQueryable();
                if (caseSett.ToUpperCase)
                {
                    qry = qry.Where(x => x.Field<string>("COLUMN_VALUE").ToUpper() == x.Field<string>("COLUMN_VALUE"));
                }
                if (caseSett.ToLowerCase)
                {
                    qry = qry.Where(x => x.Field<string>("COLUMN_VALUE").ToLower() == x.Field<string>("COLUMN_VALUE"));
                }
                if (caseSett.ToProperCase)
                {
                    var textInfo = new CultureInfo("en-US", false).TextInfo;
                    qry = qry.Where(x => x.Field<string>("COLUMN_VALUE").Equals(textInfo.ToTitleCase(textInfo.ToLower(x.Field<string>("COLUMN_VALUE")))));
                }
                res = (qry.Any()) ? qry.CopyToDataTable() : res.Clone();
            }
            return res;

        }

        public List<WordManagerSetting> GetWordManagerSettings(ImportedDataInfo importedDataInfo, string columnName)
        {
            _connectionManager.CheckConnectionState();
            var sql = SqLiteHelper.GetSelectQuery(NameHelper.GetWordManagerTable(importedDataInfo.TableName), "", $"[TABLE_NAME]='{importedDataInfo.TableName}' AND [COLUMN_NAME]='{columnName}'");
            var dat = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection);
            return dat.AsEnumerable().Select(x => new WordManagerSetting
            {
                ColumnName = columnName,
                ToDelete = x.Field<long>("ToDelete") == 1,
                ReplaceType = x.Field<long>("ReplaceOption") == 0 ? WordManagerReplaceType.WholeWord : (x.Field<long>("ReplaceOption") == 1 ? WordManagerReplaceType.AnyPart : WordManagerReplaceType.WholeValue),
                ReplaceValue = x.Field<string>("ToReplace"),
                SearchValue = x.Field<string>("COLUMN_VALUE")
            }).ToList();
        }

        public void SaveWordManagerSettings(ImportedDataInfo importedDataInfo, List<WordManagerSetting> wmWordManagerSettings, string columnName)
        {
            _connectionManager.CheckConnectionState();

            if (wmWordManagerSettings.Any())
            {
                string sql;
                foreach (string col in wmWordManagerSettings.Select(x => x.ColumnName).Distinct())
                {
                    sql = SqLiteHelper.GetDeleteQuery(NameHelper.GetWordManagerTable(importedDataInfo.TableName), $"[TABLE_NAME]='{importedDataInfo.TableName}' AND [COLUMN_NAME]='{col}'");
                    SqLiteHelper.ExecuteNonQuery(sql, _connectionManager.Connection);
                }

                foreach (var sett in wmWordManagerSettings)
                {
                    string toDelete = (sett.ToDelete) ? "1" : "0";
                    string opt = ((int)sett.ReplaceType).ToString();
                    sql = $"INSERT INTO [{NameHelper.GetWordManagerTable(importedDataInfo.TableName)}] (TABLE_NAME, COLUMN_NAME, COLUMN_VALUE, ToDelete, ToReplace, ReplaceOption) VALUES ('{importedDataInfo.TableName}','{sett.ColumnName}','{sett.SearchValue.Replace("'", "''")}',{toDelete},'{sett.ReplaceValue.Replace("'", "''")}',{opt})";
                    SqLiteHelper.ExecuteNonQuery(sql, _connectionManager.Connection);
                }
            }
            else
            {
                var sql = SqLiteHelper.GetDeleteQuery(NameHelper.GetWordManagerTable(importedDataInfo.TableName), $"[TABLE_NAME]='{importedDataInfo.TableName}' AND [COLUMN_NAME]='{columnName}'");
                SqLiteHelper.ExecuteNonQuery(sql, _connectionManager.Connection);
            }
        }

        public List<ReportResultData> GetWordManagerColumnValues(ImportedDataInfo importedDataInfo, string columnName)
        {
            _connectionManager.CheckConnectionState();
            var sql = SqLiteHelper.GetSelectQuery(NameHelper.GetColumnValuesTable(importedDataInfo.TableName), "[COLUMN_VALUE] as ColVal, [VALUE_COUNT] as ColValCnt", $"[COLUMN_NAME]='{columnName}'");

            var res = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection);
            if (res.Rows.Count == 0)
            {
                UpdateUniqueValuesForColumn(importedDataInfo.TableName, columnName);
                res = SqLiteHelper.ExecuteQuery(sql, _connectionManager.Connection);
            }
            return res.AsEnumerable().Select(x => new ReportResultData
            {
                Description = x.Field<string>("ColVal"),
                RecordValue = Convert.ToInt32(x.Field<long>("ColValCnt"))
            }).Where(x => !string.IsNullOrEmpty(x.Description)).ToList();

        }


        //TODO fix typo
        //TODO column name to constants.
        private void CreateStatisticResultTable(string tableName)
        {
            var conn = _connectionManager.Connection;

            SqLiteHelper.RemoveTableIfExists(conn, tableName);

            var sql = "create table [" + tableName + "] (FieldName TEXT PRIMARY KEY, " +
                              "FieldType TEXT DEFAULT ''," +
                              "Filled INTEGER DEFAULT 0," +
                              "Empty INTEGER DEFAULT 0," +
                              "[Distinct] INTEGER DEFAULT 0," +
                              "LeadingSpaces INTEGER DEFAULT 0," +
                              "TrailingSpaces INTEGER DEFAULT 0," +
                              "MultipleSpaces INTEGER DEFAULT 0," +
                              "WithSpaces INTEGER DEFAULT 0," +
                              "Numbers INTEGER DEFAULT 0," +
                              "Punctuation INTEGER DEFAULT 0," +
                              "UpperOnly INTEGER DEFAULT 0," +
                              "LowerOnly INTEGER DEFAULT 0," +
                              "ProperCase INTEGER DEFAULT 0," +
                              "MixedCase INTEGER DEFAULT 0," +
                              "TabChar INTEGER DEFAULT 0," +
                              "NewLineChar INTEGER DEFAULT 0," +
                              "MostCommon TEXT DEFAULT ''," +
                              "MostCommonCount TEXT DEFAULT ''," +
                              "MinNumber REAL DEFAULT 0," +
                              "MaxNumber REAL DEFAULT 0," +
                              "MaxWords INTEGER DEFAULT 0," +
                              "AvgWords TEXT DEFAULT ''," +
                              "MaxLength INTEGER DEFAULT 0," +
                              "AvgLength TEXT DEFAULT ''," +
                              "[Comma] INTEGER DEFAULT 0," +
                              "[Dots] INTEGER DEFAULT 0," +
                              "[Hypens] INTEGER DEFAULT 0," +
                              "[Apostrophes] INTEGER DEFAULT 0," +
                              "[Letters] INTEGER DEFAULT 0," +
                              "[NonPrint] INTEGER DEFAULT 0" +
                              ")";
            SqLiteHelper.ExecuteNonQuery(sql, conn);
        }

        private void CreateColumnValuesTable(string tableName)
        {
            var conn = _connectionManager.Connection;
            SqLiteHelper.RemoveTableIfExists(conn, tableName);

            var sql = "create table [" + tableName + "] (COLUMN_NAME TEXT, " +
                              "COLUMN_VALUE TEXT," +
                              "VALUE_COUNT INTEGER DEFAULT 0)";

            SqLiteHelper.ExecuteNonQuery(sql, conn);
        }

        private void CreateWordManagerTable(string tableName)
        {
            var conn = _connectionManager.Connection;
            SqLiteHelper.RemoveTableIfExists(conn, tableName);

            var sql = "create table [" + tableName + "] (TABLE_NAME TEXT, COLUMN_NAME TEXT, " +
                              "COLUMN_VALUE TEXT," +
                              "ToDelete INTEGER DEFAULT 0," +
                              "ToReplace TEXT," +
                              "ReplaceOption INTEGER DEFAULT 0)";

            SqLiteHelper.ExecuteNonQuery(sql, conn);
        }

        private void CreateCleanSettingsTable(string tableName)
        {
            var conn = _connectionManager.Connection;
            SqLiteHelper.RemoveTableIfExists(conn, tableName);
            string sql = "create table [" + tableName + "] (COLUMN_NAME TEXT PRIMARY KEY, " +
                              "RM_TrailingSpaces INTEGER DEFAULT 0," +
                              "RM_Commas INTEGER DEFAULT 0," +
                              "RM_Dots INTEGER DEFAULT 0," +
                              "RM_Hyphens INTEGER DEFAULT 0," +
                              "RM_Apostrophes INTEGER DEFAULT 0," +
                              "RM_LeadingSpaces INTEGER DEFAULT 0," +
                              "RM_Letters INTEGER DEFAULT 0," +
                              "RM_Nonprintable INTEGER DEFAULT 0," +
                              "RM_Spaces INTEGER DEFAULT 0," +
                              "RM_Numbers INTEGER DEFAULT 0," +
                              "RM_MultiSpaces INTEGER DEFAULT 0," +
                              "RM_NewLine INTEGER DEFAULT 0," +
                              "RM_Tab INTEGER DEFAULT 0," +
                              "RM_Others TEXT DEFAULT ''," +
                              "CV_0sToNothing INTEGER DEFAULT 0," +
                              "CV_LsTo1s INTEGER DEFAULT 0," +
                              "CV_NoughtsTo0s INTEGER DEFAULT 0," +
                              "CV_1sToLs INTEGER DEFAULT 0," +
                              "CV_EmptyToDefault TEXT DEFAULT ''," +
                              "ST_UpperCase INTEGER DEFAULT 0," +
                              "ST_LowerCase INTEGER DEFAULT 0," +
                              "ST_ProperCase INTEGER DEFAULT 0," +
                              "SP_EmailAddress INTEGER DEFAULT 0," +
                              "SP_Email INTEGER DEFAULT 0," +
                              "SP_Telephones INTEGER DEFAULT 0," +
                              "SP_DateTime INTEGER DEFAULT 0," +
                              "SP_Words INTEGER DEFAULT 0," +
                              "Merger INTEGER DEFAULT 0," +
                              "SH_LEFT INTEGER DEFAULT 0," +
                              "SH_RIGHT INTEGER DEFAULT 0," +
                              "AF_Address INTEGER DEFAULT 0," +
                              "AF_City INTEGER DEFAULT 0," +
                              "AF_State INTEGER DEFAULT 0," +
                              "AF_Zip INTEGER DEFAULT 0," +
                              "RE_Expression TEXT DEFAULT ''," +
                              "RE_Replace TEXT DEFAULT ''," +
                              "RE_Copy TEXT DEFAULT ''," +
                              "Merger_order INTEGER DEFAULT 0," +
                              "CHK_Email INTEGER DEFAULT 0)";
            SqLiteHelper.ExecuteNonQuery(sql, conn);
        }

        private void UpdateUniqueValuesForColumn(string tableName, string columnName)
        {
            var sql = SqLiteHelper.GetDeleteQuery(NameHelper.GetColumnValuesTable(tableName), $"[COLUMN_NAME]='[{columnName}]'");
            SqLiteHelper.ExecuteNonQuery(sql, _connectionManager.Connection);

            sql = $"insert into [{NameHelper.GetColumnValuesTable(tableName)}] select '{columnName}', [{columnName}], count(*) from [{tableName}] group by [{columnName}]";
            SqLiteHelper.ExecuteNonQuery(sql, _connectionManager.Connection);
        }
    }
}