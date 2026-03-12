using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace WinPure.ConsoleTestCore
{
    class Program
    {

        private static string _localDbPath = @"C:\WinPureCore\Solution\WinPure.ConsoleSampleCore\WinPure.ConsoleTestCore\WinPureSampleDb.db";
        static void Main(string[] args)
        {
            //TestAV();
            Console.ReadLine();
        }

        private static void TestAV()
        {
            //var newApi = new WinPureApi();

            //Console.WriteLine(newApi.CheckLicenseState().ToString());
            //var cancellationToken = CancellationToken.None;
            //try
            //{
            //    //var data = GetTestData();

            //    //var settings = new UsAddressVerificationSettings
            //    //{
            //    //    IsOnlineVerification = true,
            //    //    Verification = true,
            //    //    AddressColumns = new List<string> { "Address 1", "Address 2", "Address 3" },
            //    //    GeoTag = true,
            //    //    CassField = true,
            //    //    Country = "USA",
            //    //    LicenseKey = "KJ49-EC59-DG79-TJ99",
            //    //    ReverseGeocode = false,
            //    //    PostalCodeColumns = new List<string> { "ZIP" },
            //    //    StateColumns = new List<string> { "State" },
            //    //};

            //    var data = GetGeoData();
            //    var settings = new UsAddressVerificationSettings
            //    {
            //        IsOnlineVerification = true,
            //        Verification = false,
            //        LatitudeColumn = "Latitude",
            //        LongitudeColumn = "Longitude",
            //        GeoTag = false,
            //        ReverseGeocode = true,
            //        Country = "USA"
            //    };

            //    var avResult = newApi.VerifyUsAddresses(data, settings, cancellationToken);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine($"{e.Message} {e.InnerException?.Message}");
            //}
            //Console.WriteLine("AV complete...");
        }

        private static DataTable GetGeoData()
        {
            var dt = new DataTable();

            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Latitude", typeof(decimal)));
            dt.Columns.Add(new DataColumn("Longitude", typeof(decimal)));

            var r = dt.NewRow();
            r["Name"] = "Base name";
            r["Latitude"] = 33.551770m;
            r["Longitude"] = -112.258720m;
            dt.Rows.Add(r);
            return dt;
        }

        private static DataTable GetTestData()
        {
            var testData = new DataTable("TestData");
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = $"SELECT top 100 * FROM [dbo].[AspNetUsers]";

                using (var reader = selectCmd.ExecuteReader())
                {
                    testData.Load(reader);
                }
                return testData;
            }
        }

        private static string GetConnectionString()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.DataSource = @"winpure.database.windows.net";
            connectionStringBuilder.IntegratedSecurity = false;
            connectionStringBuilder.ConnectTimeout = 120;
            connectionStringBuilder.InitialCatalog = "WinPureIntegration"; //"winpure_db1";
            connectionStringBuilder.UserID = "winpure2020";
            connectionStringBuilder.Password = "Quality2020!";
            return connectionStringBuilder.ConnectionString;
        }
    }
}
