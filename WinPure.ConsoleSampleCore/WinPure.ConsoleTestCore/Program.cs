using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using WinPure.AddressVerification.Models;
using WinPure.APICore;
using WinPure.Cleansing.Models;
using WinPure.Matching.Enums;
using WinPure.Matching.Models;
using MatchType = WinPure.Matching.Enums.MatchType;

namespace WinPure.TestApiConsole
{
    class Program
    {
        private static string PathToTestDb = "WinPureSampleDb.db";
        private static WinPureApi _api;

        static void Main(string[] args)
        {
            _api = new WinPureApi();
            var registrationCode = _api.GetRegistrationCode();
            Console.WriteLine($"You have to provide that code {registrationCode} to WinPure to get your license key");
            //uncomment for API registration
            //var res = _api.Register("DEMO");
            //if (res == Licensing.Enums.LicenseState.Valid)
            //{
            //    Console.WriteLine("You API is registered");
            //}
            var cancellationToken = new CancellationToken();

            try
            {
                TestCleansingTable1(cancellationToken);
                TestCleansingTable2(cancellationToken);
                //TestOfflineAddressVerification(cancellationToken);
                TestMatchingCompanies(cancellationToken);
                TestSearchCompanies(cancellationToken);
                TestMatchingCompaniesBetweenTwoTables(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message} {e.InnerException?.Message}");
            }

            Console.ReadLine();
        }

        private static void TestCleansingTable1(CancellationToken cancellationToken)
        {
            var testData = GetTestData("Table1");

            var cleanSettings = GetCleanSettingsForTable1();

            var statistic = _api.CalculateStatistic(testData, cancellationToken);
            Console.WriteLine("Statistic ready");
            _api.CleanTable(testData, cleanSettings, cancellationToken);
            Console.WriteLine("Cleansing complete");
        }

        private static WinPureCleanSettings GetCleanSettingsForTable1()
        {
            var cleanSettings = new WinPureCleanSettings();
            cleanSettings.TextCleanerSettings.Add(new TextCleanerSetting
            {
                ColumnName = "Company Name",
                ConvertOnesToLs = true,
                RemoveCommas = true,
                RemoveNonPrintableCharacters = true
            });
            cleanSettings.TextCleanerSettings.Add(new TextCleanerSetting
            {
                ColumnName = "State",
                RemoveAllDigits = true
            });
            cleanSettings.TextCleanerSettings.Add(new TextCleanerSetting
            {
                ColumnName = "Title",
                RemoveNonPrintableCharacters = true,
                RemoveLeadingSpace = true,
                RemoveTrailingSpace = true
            });
            cleanSettings.CaseConverterSettings.Add(new CaseConverterSetting
            {
                ColumnName = "Title",
                ToProperCase = true
            });
            cleanSettings.CaseConverterSettings.Add(new CaseConverterSetting
            {
                ColumnName = "State",
                ToUpperCase = true
            });
            cleanSettings.ColumnSplitSettings.Add(new ColumnSplitSetting
            {
                ColumnName = "ZIP",
                SplitIntoWords = new SplitIntoWords { SplitSeparator = "-" }
            });
            cleanSettings.WordManagerSettings.Add(new WordManagerSetting
            {
                ColumnName = "Company Name",
                SearchValue = "Corp",
                ReplaceValue = "Corporation",
                ReplaceType = Cleansing.Enums.WordManagerReplaceType.WholeWord
            });
            cleanSettings.WordManagerSettings.Add(new WordManagerSetting
            {
                ColumnName = "Company Name",
                SearchValue = "inc",
                ReplaceValue = "Incorporated",
                ReplaceType = Cleansing.Enums.WordManagerReplaceType.WholeWord
            });
            return cleanSettings;
        }

        private static void TestCleansingTable2(CancellationToken cancellationToken)
        {
            var testData = GetTestData("Table2");

            var cleanSettings = GetCleanSettingsForTable2();

            var statistic = _api.CalculateStatistic(testData, cancellationToken);
            Console.WriteLine("Statistic ready");
            _api.CleanTable(testData, cleanSettings, cancellationToken);
            Console.WriteLine("Cleansing complete");
        }

        private static WinPureCleanSettings GetCleanSettingsForTable2()
        {
            var cleanSettings = new WinPureCleanSettings();
            cleanSettings.TextCleanerSettings.Add(new TextCleanerSetting
            {
                ColumnName = "Address1",
                RemoveHyphens = true
            });
            cleanSettings.TextCleanerSettings.Add(new TextCleanerSetting
            {
                ColumnName = "Surname",
                RemoveAllDigits = true
            });
            cleanSettings.CaseConverterSettings.Add(new CaseConverterSetting
            {
                ColumnName = "Company Name",
                ToProperCase = true,
            });
            cleanSettings.TextCleanerSettings.Add(new TextCleanerSetting
            {
                ColumnName = "Company Name",
                RemoveNonPrintableCharacters = true
            });
            cleanSettings.CaseConverterSettings.Add(new CaseConverterSetting
            {
                ColumnName = "Email",
                ToLowerCase = true
            });
            cleanSettings.ColumnSplitSettings.Add(new ColumnSplitSetting
            {
                ColumnName = "Email",
                SplitEmailAddressIntoAccountDomainAndZone = true
            });
            cleanSettings.ColumnSplitSettings.Add(new ColumnSplitSetting
            {
                ColumnName = "DateofBirth",
                SplitDatetime = true
            });
            cleanSettings.ColumnSplitSettings.Add(new ColumnSplitSetting
            {
                ColumnName = "Address1",
                SplitIntoWords = new SplitIntoWords { SplitSeparator = " " }
            });
            cleanSettings.ColumnSplitSettings.Add(new ColumnSplitSetting
            {
                ColumnName = "Email",
                SplitIntoWords = new SplitIntoWords { SplitSeparator = "@" }
            });
            cleanSettings.ColumnMergeSettings.Add(new ColumnMergeSetting
            {
                ColumnName = "FirstName",
                CharToInsertBetweenColumn = ",",
                Order = 1
            });
            cleanSettings.ColumnMergeSettings.Add(new ColumnMergeSetting
            {
                ColumnName = "Surname",
                CharToInsertBetweenColumn = ",",
                Order = 2
            });
            cleanSettings.CaseConverterSettings.Add(new CaseConverterSetting
            {
                ColumnName = "Confirmed",
                ToLowerCase = true
            });
            cleanSettings.TextCleanerSettings.Add(new TextCleanerSetting
            {
                ColumnName = "Confirmed",
                RegexExpression = "yes",
                RegexReplace = "true"
            });
            cleanSettings.TextCleanerSettings.Add(new TextCleanerSetting
            {
                ColumnName = "Confirmed",
                RegexExpression = "no",
                RegexReplace = "false"
            });
            return cleanSettings;
        }

        private static void TestMatchingCompanies(CancellationToken cancellationToken)
        {
            var TableName = "Table1";
            var testData = GetTestData(TableName);

            var (matchParameter, matchTables, fields) = GetCompaniesMatchParameter(testData, TableName);

            var result = _api.MatchData(new List<TableParameter> { matchTables }, matchParameter, fields, cancellationToken);
            Console.WriteLine("Match complete");
        }

        private static (MatchParameter, TableParameter, List<FieldMapping>) GetCompaniesMatchParameter(DataTable testDataTable, string tableName)
        {
            var table = new TableParameter
            {
                TableName = tableName,
                TableData = testDataTable
            };

            var matchCondition = new MatchCondition
            {
                IgnoreNullValues = true,
                MatchingType = MatchType.Fuzzy,
                Level = 0.9,
                Weight = 1
            };
            matchCondition.Fields.Add(new MatchField
            {
                ColumnDataType = typeof(string).ToString(),
                ColumnName = "Company Name",
                TableName = tableName
            });

            var matchGroup = new MatchGroup
            {
                GroupId = 1,
                GroupLevel = 0.9,

            };
            matchGroup.Conditions.Add(matchCondition);

            var matchParameter = new MatchParameter
            {
                FuzzyAlgorithm = MatchAlgorithm.JaroWinkler,
                CheckInternal = true
            };

            matchParameter.Groups.Add(matchGroup);

            var fields = new List<FieldMapping>();

            foreach (DataColumn column in testDataTable.Columns)
            {
                var matchField = new MatchField
                {
                    TableName = tableName,
                    ColumnName = column.ColumnName,
                    ColumnDataType = column.DataType.ToString()
                };
                var fieldMapping = new FieldMapping
                {
                    FieldName = column.ColumnName
                };
                fieldMapping.FieldMap.Add(matchField);
                fields.Add(fieldMapping);
            }

            return (matchParameter, table, fields);
        }

        private static void TestSearchCompanies(CancellationToken cancellationToken)
        {
            var TableName = "Table1";
            var testData = GetTestData(TableName);

            var (searchParameter, searchTable) = GetCompaniesSearchParameter(testData, TableName, "007 Lacksmith 24", "Company Name");

            var result = _api.SearchData( searchTable , searchParameter, cancellationToken);
            Console.WriteLine("Search complete");
        }

        private static (SearchParameter, TableParameter) GetCompaniesSearchParameter(DataTable testDataTable, string tableName, string searchValue, string searchColumnName)
        {
            var table = new TableParameter
            {
                TableName = tableName,
                TableData = testDataTable
            };

            var searchCondition = new SearchCondition
            {
                IgnoreNullValues = true,
                MatchingType = MatchType.Fuzzy,
                Level = 0.9,
                Weight = 1,
                SearchValue = searchValue,
                SearchField = new MatchField
                {
                    ColumnDataType = typeof(string).ToString(),
                    ColumnName = searchColumnName,
                    TableName = tableName
                }
            };

            var searchGroup = new SearchGroup
            {
                GroupId = 1,
                GroupLevel = 0.9
            };
            searchGroup.Conditions.Add(searchCondition);

            var searchParameter = new SearchParameter
            {
                FuzzyAlgorithm = MatchAlgorithm.JaroWinkler,
            };

            searchParameter.Groups.Add(searchGroup);
            
            return (searchParameter, table);
        }

        private static void TestMatchingCompaniesBetweenTwoTables(CancellationToken cancellationToken)
        {
            var tableName1 = "Table1";
            var tableData1 = GetTestData(tableName1);
            var tableName2 = "Table2";
            var tableData2 = GetTestData(tableName2);

            var (matchParameter, matchTables, fields) = GetMatchingCompaniesBetweenTwoTablesParameter(tableData1, tableName1, tableData2, tableName2);


            var result = _api.MatchData(matchTables, matchParameter, fields, cancellationToken);
            Console.WriteLine("Match complete");
        }

        private static (MatchParameter, List<TableParameter>, List<FieldMapping>) GetMatchingCompaniesBetweenTwoTablesParameter(DataTable testDataTable1, string tableName1, DataTable testDataTable2, string tableName2)
        {
            var table1 = new TableParameter
            {
                TableName = tableName1,
                TableData = testDataTable1
            };

            var table2 = new TableParameter
            {
                TableName = tableName2,
                TableData = testDataTable2
            };

            var parametersJson = File.ReadAllText("Match_2tables_Parameter.json");
            var matchParameter = JsonConvert.DeserializeObject<MatchParameter>(parametersJson);

            var fieldsJson = File.ReadAllText("Match_2tables_Fields.json");
            var fields = JsonConvert.DeserializeObject<List<FieldMapping>>(fieldsJson);

            return (matchParameter, new List<TableParameter> { table1, table2 }, fields);
        }

        private static void TestOfflineAddressVerification(CancellationToken cancellationToken)
        {
            var testData = GetTestData("Table3");
            var settings = new UsAddressVerificationSettings()
            {
                Verification = true,
                GeoTag = true,
                IsOnlineVerification = false,
                AddressColumns = new List<string> { "Address1", "Address2" },
                PostalCodeColumns = new List<string> { "ZIP" },
                LocalityColumns = new List<string> { "Address3" },
                StateColumns = new List<string> { "State" },
                Country = "USA",
                PathToDb = "provide here path to offline address verification database"
            };

            var addressReport = _api.VerifyUsAddresses(testData, settings, cancellationToken);
            if (addressReport != null)
            {
                Console.WriteLine(
                    $"Total original rows {testData.Rows.Count}, Processed time {addressReport.VerifyTime.ToString("g")}, Verifiyed rows {addressReport.CommonData.AddressSuccess}, used credits {addressReport.UsedCredits}");
            }
        }

        private static DataTable GetTestData(string tableName)
        {
            var testData = new DataTable("TestData");
            using (var connection = new SqliteConnection($"DataSource={PathToTestDb}"))
            {
                connection.Open();
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = $"SELECT * FROM [{tableName}]";

                using (var reader = selectCmd.ExecuteReader())
                {
                    testData.Load(reader);
                }
            }

            return testData;
        }
    }
}
