using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading;
using WinPure.DeduplicationModule.Model.CleanSettings;
using WinPureApiLib;

namespace WinPure.ConsoleTestOldSolution
{
    class Program
    {
        private static string PathToTestDb = @"WinPureSampleDb.db";
        private static string TableName = "Table3";

        static void Main(string[] args)
        {
            var testData = GetTestData();
            var cancellationToken = new CancellationToken();
            var wpApi = new WinPureApi();
            wpApi.LicenseKey = "Y5PRZ-N4AHE-280ZB-3ADKM";//work
            //wpApi.LicenseKey = "Y3PRZ-N0ADP-1801A-1ADKG"; // home
            TestCleansing(testData, wpApi, cancellationToken);
            Console.ReadLine();
        }

        private static void TestCleansing(DataTable testData, WinPureApi api, CancellationToken cancellationToken)
        {
            var cleanSettings = GetCleanSettingsForAddress();

            var timer = new Stopwatch();
            timer.Start();

            var statistic = api.CalculateStatistic(testData);
            timer.Stop();
            Console.WriteLine($"OLD Cleansing: Statistic time: {timer.Elapsed.ToString("g")}");
            timer.Reset();
            timer.Start();
            var result = api.CleanTable(cleanSettings, testData);
            timer.Stop();
            Console.WriteLine($"OLD Cleansing: Total time: {timer.Elapsed.ToString("g")}");
        }

        private static WinPureCleanSettings GetCleanSettingsForAddress()
        {
            var cleanSettings = new WinPureCleanSettings();
            cleanSettings.ColumnSplitSettings.AddRange(new List<ColumnSplitSetting>
            {
                new ColumnSplitSetting
                {
                    ColumnName = "Address1",
                    AddressSplitting = new SplitAddress() { IsAddress = true}
                },
                new ColumnSplitSetting
                {
                    ColumnName = "Address2",
                    AddressSplitting = new SplitAddress() { IsAddress = true }
                },
                new ColumnSplitSetting
                {
                    ColumnName = "Address3",
                    AddressSplitting = new SplitAddress() { IsAddress = true }
                },
                new ColumnSplitSetting
                {
                    ColumnName = "ZIP",
                    AddressSplitting = new SplitAddress() { IsZipOrState = true }
                },
                new ColumnSplitSetting
                {
                    ColumnName = "State",
                    AddressSplitting = new SplitAddress() { IsZipOrState = true }
                }
            });

            cleanSettings.ColumnSplitSettings.Add(new ColumnSplitSetting
            {
                ColumnName = "ContactName",
                GenderSplitting = new SplitGender() { IsSplitGender = true }
            }); 
            return cleanSettings;
        }

        private static WinPureCleanSettings GetCleanSettingsForTable1()
        {
            var cleanSettings = new WinPureCleanSettings();
            cleanSettings.TextCleanerSettings.Add(new TextCleanerSetting
            {
                ColumnName = "ZIP",
                RemoveHyphens = true
            });
            cleanSettings.CaseConverterSettings.Add(new CaseConverterSetting
            {
                ColumnName = "CompanyName",
                ToProperCase = true
            });
            cleanSettings.TextCleanerSettings.Add(new TextCleanerSetting
            {
                ColumnName = "CompanyName",
                RemoveNonPrintableCharacters = true
            });
            return cleanSettings;
        }

        private static WinPureCleanSettings GetCleanSettingsForTable2()
        {
            var cleanSettings = new WinPureCleanSettings();
            cleanSettings.TextCleanerSettings.Add(new TextCleanerSetting
            {
                ColumnName = "telephone",
                RemoveAllDigits = true,
                //RemoveAllSpaces = true,
                //RemoveHyphens = true
            });
            //cleanSettings.TextCleanerSettings.Add(new TextCleanerSetting
            //{
            //    ColumnName = "email",
            //    RemoveAllLetters = true,
            //});
            //cleanSettings.TextCleanerSettings.Add(new TextCleanerSetting
            //{
            //    ColumnName = "web",
            //    RemovePunctuation = true,

            //});
            //cleanSettings.TextCleanerSettings.Add(new TextCleanerSetting
            //{
            //    ColumnName = "full_name",
            //    RemoveNewLine = true,

            //});
            cleanSettings.CaseConverterSettings.Add(new CaseConverterSetting
            {
                ColumnName = "full_name",
                ToProperCase = true
            });
            cleanSettings.ColumnSplitSettings.Add(new ColumnSplitSetting()
            {
                ColumnName = "email",
                SplitEmailAddressIntoAccountDomainAndZone = new SplitEmailAddressIntoAccountDomainAndZone() { IsSplitAccount = true, IsSplitDomain = true, IsSplitEmailCountry = true, IsSplitSubDomain = true }
            });
            cleanSettings.ColumnSplitSettings.Add(new ColumnSplitSetting
            {
                ColumnName = "web",
                RegexCopy = @"://(?<host>([a-z\d][-a-z\d]*[a-z\d]\.)*[a-z][-a-z\d]+[a-z])"
            });
            return cleanSettings;
        }

        private static WinPureCleanSettings GetCleanSettingsForDefault()
        {
            var cleanSettings = new WinPureCleanSettings();
            cleanSettings.ColumnSplitSettings.Add(new ColumnSplitSetting
            {
                ColumnName = "Email",
                SplitEmailAddressIntoAccountDomainAndZone = new SplitEmailAddressIntoAccountDomainAndZone() { IsSplitAccount = true, IsSplitDomain = true, IsSplitSubDomain = true, IsSplitEmailCountry = true }
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
            return cleanSettings;
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
