using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Newtonsoft.Json;
using WinPure.AddressVerification.Models;
using WinPure.API;
using WinPure.Cleansing.Models;
using WinPure.Common.Helpers;
using WinPure.Matching.Enums;
using WinPure.Matching.Models;
using WinPure.Matching.Services;

namespace WinPure.ConsoleTest
{
    class Program
    {
        private static string PathToTestDb = "WinPureSampleDb.db";
        private static string TableName = "Table1";
        private static WinPureApi _api;

        static void Main(string[] args)
        {
            _api = new WinPureApi();
            var res = _api.Register("X3NT0-C2ADN-3Z219-19FAJ");
            var cancellationToken = new CancellationToken();
            try
            {
                TestCleansingTable1(cancellationToken);
                TestCleansingTable2(cancellationToken);
                TestCleansingAddressAndGenderSplitting(cancellationToken);
                //TestOnlineAddressVerification(cancellationToken);
                //TestOfflineAddressVerification(cancellationToken);
                TestMatchingCompanies(cancellationToken);
                TestMatching100KUnique(cancellationToken);
                TestCompaniesWith100KUnique(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message} {e.InnerException?.Message}");
            }

            Console.ReadLine();
        }


        private static void TestOnlineAddressVerification(CancellationToken cancellationToken)
        {
            TableName = "Table4";
            var testData = GetTestData();
            var settings = new UsAddressVerificationSettings()
            {
                Verification = true,
                GeoTag = true,
                LicenseKey = "XM11-HU24-BF94-DH35",
                IsOnlineVerification = true,
                AddressColumns = new List<string> { "address" },
                PostalCodeColumns = new List<string> { "zip" },
                LocalityColumns = new List<string> { "city" },
                StateColumns = new List<string> { "state" },
                Country = "USA",
                CassField = true,
                AvailableCredits = 5000
            };

            var addressReport = _api.VerifyUsAddresses(testData, settings, cancellationToken);
            Console.WriteLine($"Total original rows {testData.Rows.Count}, Processed time {addressReport.VerifyTime.ToString("g")}, Verifiyed rows {addressReport.CommonData.AddressSuccess}, used credits {addressReport.UsedCredits}");
        }

        private static void TestOfflineAddressVerification(CancellationToken cancellationToken)
        {
            TableName = "Table4";
            var testData = GetTestData();
            var settings = new UsAddressVerificationSettings()
            {
                GeoTag = true,
                LicenseKey = "XM11-HU24-BF94-DH35",
                IsOnlineVerification = false,
                AddressColumns = new List<string> { "address" },
                PostalCodeColumns = new List<string> { "zip" },
                LocalityColumns = new List<string> { "city" },
                StateColumns = new List<string> { "state" },
                Country = "USA",
                CassField = true,
                AvailableCredits = 5000
            };

            var addressReport = _api.VerifyUsAddresses(testData, settings, cancellationToken);
            Console.WriteLine($"Total original rows {testData.Rows.Count}, Processed time {addressReport.VerifyTime.ToString("g")}, Verifiyed rows {addressReport.CommonData.AddressSuccess}, used credits {addressReport.UsedCredits}");
        }

        private static void RaiseOnProgressUpdate(string description, int progress, int currentStep = 0, int totalSteps = 10)
        {
            Console.WriteLine($"Caption {description} progress {progress}");
        }

        private static void TestCleansingTable2(CancellationToken cancellationToken)
        {
            TableName = "Table2";
            var testData = GetTestData();

            var cleanSettings = GetCleanSettingsForTable2();

            var timer = new Stopwatch();
            timer.Start();

            var statistic = _api.CalculateStatistic(testData, cancellationToken);
            timer.Stop();
            Console.WriteLine($"NEW Cleansing: Statistic time: {timer.Elapsed.ToString("g")}");
            timer.Reset();
            timer.Start();
            _api.CleanTable(testData, cleanSettings, cancellationToken);
            timer.Stop();
            Console.WriteLine($"NEW Cleansing: Total time: {timer.Elapsed.ToString("g")}");
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
                ToProperCase = true
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
                CharToInsertBetweenColumn = ","
            });
            cleanSettings.ColumnMergeSettings.Add(new ColumnMergeSetting
            {
                ColumnName = "Surname",
                CharToInsertBetweenColumn = ","
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

        private static void TestCleansingAddressAndGenderSplitting(CancellationToken cancellationToken)
        {
            TableName = "Table3";
            var testData = GetTestData();

            var cleanSettings = GetCleanSettingsForAddress();

            var timer = new Stopwatch();
            timer.Start();

            var statistic = _api.CalculateStatistic(testData, cancellationToken);
            timer.Stop();
            Console.WriteLine($"NEW Cleansing: Statistic time: {timer.Elapsed.ToString("g")}");
            timer.Reset();
            timer.Start();
            _api.CleanTable(testData, cleanSettings, cancellationToken);
            timer.Stop();
            Console.WriteLine($"NEW Cleansing: Total time: {timer.Elapsed.ToString("g")}");
        }

        private static WinPureCleanSettings GetCleanSettingsForAddress()
        {
            var cleanSettings = new WinPureCleanSettings();
            cleanSettings.AddressAndGenderSplitSettings.AddressColumns = new List<string>
            {
                "Address1",
                "Address2",
                "Address3",
                "ZIP",
                "State",
            };
            cleanSettings.AddressAndGenderSplitSettings.GenderColumns = new List<string>
            {
                "ContactName"
            };
            return cleanSettings;
        }

        private static void TestCleansingTable1(CancellationToken cancellationToken)
        {
            TableName = "Table1";
            var testData = GetTestData();

            var cleanSettings = GetCleanSettingsForTable1();

            var timer = new Stopwatch();
            timer.Start();

            var statistic = _api.CalculateStatistic(testData, cancellationToken);
            timer.Stop();
            Console.WriteLine($"NEW Cleansing: Statistic time: {timer.Elapsed.ToString("g")}");
            timer.Reset();
            timer.Start();
            _api.CleanTable(testData, cleanSettings, cancellationToken);
            timer.Stop();
            Console.WriteLine($"NEW Cleansing: Total time: {timer.Elapsed.ToString("g")}");
        }

        private static WinPureCleanSettings GetCleanSettingsForTable1()
        {
            var cleanSettings = new WinPureCleanSettings();
            cleanSettings.TextCleanerSettings.Add(new TextCleanerSetting
            {
                ColumnName = "telephone",
                RemoveAllSpaces = true,
                RemoveHyphens = true,
                RemoveAllLetters = true,
                RemovePunctuation = true
            });
            cleanSettings.TextCleanerSettings.Add(new TextCleanerSetting
            {
                ColumnName = "email",
                RemoveAllLetters = true,
            });
            cleanSettings.TextCleanerSettings.Add(new TextCleanerSetting
            {
                ColumnName = "web",
                RemoveNonPrintableCharacters = true,
            });
            cleanSettings.TextCleanerSettings.Add(new TextCleanerSetting
            {
                ColumnName = "full_name",
                RemoveNewLine = true,
            });
            cleanSettings.CaseConverterSettings.Add(new CaseConverterSetting
            {
                ColumnName = "full_name",
                ToProperCase = true
            });
            cleanSettings.ColumnSplitSettings.Add(new ColumnSplitSetting()
            {
                ColumnName = "email",
                SplitEmailAddressIntoAccountDomainAndZone = true
            });
            cleanSettings.ColumnSplitSettings.Add(new ColumnSplitSetting
            {
                ColumnName = "web",
                RegexCopy = @"://(?<host>([a-z\d][-a-z\d]*[a-z\d]\.)*[a-z][-a-z\d]+[a-z])"
            });

            cleanSettings.WordManagerSettings.Add(new WordManagerSetting
            {
                ColumnName = "address",
                SearchValue = "Bahnhof",
                ReplaceValue = "Bhf.",
                ReplaceType = Cleansing.Enums.WordManagerReplaceType.AnyPart
            });
            cleanSettings.WordManagerSettings.Add(new WordManagerSetting
            {
                ColumnName = "address",
                SearchValue = "St.",
                ReplaceValue = "Saint",
                ReplaceType = Cleansing.Enums.WordManagerReplaceType.WholeWord
            });
            cleanSettings.WordManagerSettings.Add(new WordManagerSetting
            {
                ColumnName = "tatigkeitsbeschreibung",
                SearchValue = "Gasthaus",
                ReplaceValue = "Hotel",
                ReplaceType = Cleansing.Enums.WordManagerReplaceType.AnyPart
            });
            return cleanSettings;
        }

        private static void TestMatchingCompanies(CancellationToken cancellationToken)
        {
            TableName = "Companies";
            var testData = GetTestData();

            var (matchParameter, matchTables, fields) = GetCompaniesMatchParameter(testData);

            var timer = new Stopwatch();
            timer.Start();

            var result = _api.MatchData(new List<TableParameter>{matchTables}, matchParameter, fields, cancellationToken);
            timer.Stop();
            Console.WriteLine($"NEW Matching: Total time: {timer.Elapsed.ToString("g")}");

            Console.WriteLine("Assert result");
            result.AsEnumerable().Max(x => x.Field<int>("Group ID")).Should().Be(969);

            //TODO!!!
            //result.Rows.Count.Should().Be(testData.Rows.Count);

            var groups = result.AsEnumerable().GroupBy(x => x.Field<int>("Group ID"))
                .Select(x => new {GroupId = x.Key, Cnt = x.Count()});
            groups.Count(x => x.Cnt > 1).Should().Be(549);

            Console.WriteLine("All assertions are OK");
        }

        private static (MatchParameter, TableParameter, List<FieldMapping>) GetCompaniesMatchParameter(DataTable testDataTable)
        {
            string tableName = "Company";

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
                ColumnName = "CompanyName",
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

        private static void TestMatching100KUnique(CancellationToken cancellationToken)
        {
            TableName = "100KUnique";
            var testData = GetTestData();

            var (matchParameter, matchTables, fields) = Get100KMatchParameter(testData);
            Console.WriteLine("Start matching");
            var timer = new Stopwatch();
            timer.Start();

            var result = _api.MatchData(new List<TableParameter> { matchTables }, matchParameter, fields, cancellationToken);
            timer.Stop();
            Console.WriteLine($"NEW Matching: Total time: {timer.Elapsed.ToString("g")}, Old time approximately 2 min");

            //Console.WriteLine("Assert result");
            //result.AsEnumerable().Max(x => x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID)).Should().Be(969);

            ////TODO!!!
            ////result.Rows.Count.Should().Be(testData.Rows.Count);

            //var groups = result.AsEnumerable().GroupBy(x => x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID))
            //    .Select(x => new { GroupId = x.Key, Cnt = x.Count() });
            //groups.Count(x => x.Cnt > 1).Should().Be(549);

            //Console.WriteLine("All assertions are OK");
        }

        private static (MatchParameter, TableParameter, List<FieldMapping>) Get100KMatchParameter(DataTable testDataTable)
        {
            string tableName = "100KUnique";

            var table = new TableParameter
            {
                TableName = tableName,
                TableData = testDataTable
            };

            var parameters = File.ReadAllText("100k_parameters.json");
            var matchParameter = JsonConvert.DeserializeObject<MatchParameter>(parameters);

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

        private static void TestCompaniesWith100KUnique(CancellationToken cancellationToken)
        {
            TableName = "Companies";
            
            var testData = GetTestData();

            TableName = "100KUnique";
            var testData2 = GetTestData();
            var tableParameterC = new TableParameter
            {
                TableName = "100KUnique",
                TableData = testData2
            };
            var (matchParameter, matchTables, fields) = GetCompaniesWith100KMatchParameter(testData);

            //_api.OnProgress += _api_OnProgress;

            Console.WriteLine("Start matching");
            var timer = new Stopwatch();
            timer.Start();

            var result = _api.MatchData(new List<TableParameter> { matchTables, tableParameterC }, matchParameter, fields, cancellationToken);
            timer.Stop();
            Console.WriteLine($"NEW Matching: Total time: {timer.Elapsed.ToString("g")}, Old time approximately 2 min");
            //_api.OnProgress -= _api_OnProgress;
            Console.WriteLine("Assert result");
            result.Rows.Count.Should().Be(testData.Rows.Count + testData2.Rows.Count);

            var groups = result.AsEnumerable().GroupBy(x => x.Field<int>("Group ID"))
                .Select(x => new { GroupId = x.Key, Cnt = x.Count() });
            groups.Count(x => x.Cnt > 1).Should().Be(9100); // why 1 less then in UI?


            result.AsEnumerable().Max(x => x.Field<int>("Group ID")).Should().Be(87436);// why 1 less then in UI?


            //var representationService = new RepresentationService();
            //var acrossTables = representationService.GetMatchResult(result, MatchResultViewType.AcrossTable);
            //acrossTables.Rows.Count.Should().Be(711);
            //acrossTables.AsEnumerable().Select(x => x.Field<int>(WinPureColumnNamesHelper.WPCOLUMN_GROUPID))
            //    .Distinct()
            //    .Count()
            //    .Should().Be(292);
            Console.WriteLine("All assertions are OK");
        }

        private static void _api_OnProgress(string arg1, int arg2, int arg3, int arg4)
        {
            //if (InvokeRequired)
            //{
            //    Invoke(new MethodInvoker(UpdateMatchParameters));
            //    return;
            //}
            Console.Clear();
            Console.WriteLine($"Matching in progress: {arg2}%   Step {arg3} of {arg4}");
        }

        private static (MatchParameter, TableParameter, List<FieldMapping>) GetCompaniesWith100KMatchParameter(DataTable testDataTable)
        {
            string tableName = "Companies";

            var table = new TableParameter
            {
                TableName = tableName,
                TableData = testDataTable
            };

            var parameters = File.ReadAllText("Companies_with_100K.json");
            var matchParameter = JsonConvert.DeserializeObject<MatchParameter>(parameters);

            var fieldsString = File.ReadAllText("Companies_with_100K_fields.json");
            var fields = JsonConvert.DeserializeObject<List<FieldMapping>>(fieldsString);

            return (matchParameter, table, fields);
        }

        private static DataTable GetTestData()
        {
            var testData = new DataTable("TestData");
            using (var connection = new SQLiteConnection($"DataSource={PathToTestDb}"))
            {
                connection.Open();
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = $"SELECT * FROM [{TableName}]";

                using (var reader = selectCmd.ExecuteReader())
                {
                    testData.Load(reader);
                }
            }

            return testData;
        }
    }
}
