using System.IO;
using Newtonsoft.Json;
using WinPure.Common.Enums;
using WinPure.Configuration.Enums;
using WinPure.Configuration.Models.Configuration;
using WinPure.DataService.Senzing.Helpers;

namespace WinPure.DataService.Senzing.Models.G2;

public class SenzingConfiguration
{
    internal const string DATABASE_NAME = "G2C.db";
    internal const string DATABASE_NAMEC1 = "G2C_RES.db";
    internal const string DATABASE_NAMEC2 = "G2C_LIBFEAT.db";

    public SenzingConfiguration()
    {
    }

    public SenzingConfiguration(EntityResolutionSetting erSettings)
    {
        var path = erSettings.DataFolder.Replace("\\", "/");
        Pipeline = new PathConfiguration
        {
            ConfigPath = $"{path}/api/etc",
            ResourcePath = $"{path}/api/resources",
            SupportPath = $"{path}/api/data",
            Enable_Diagnostic_Logging = erSettings.EnableDebugLogs ? "Y" : "N",
            Enable_Score_Logging = erSettings.EnableDebugLogs ? "Y" : "N"
        };

        switch (erSettings.Database)
        {
            case EntityResolutionDatabase.Internal:
                DbPath = Path.Combine(path, DATABASE_NAME);
                DbPathC1 = Path.Combine(path, DATABASE_NAMEC1);
                DbPathC2 = Path.Combine(path, DATABASE_NAMEC2);

                Sql = new SqlConnectionConfiguration
                {
                    Backend = "HYBRID",
                    Connection = $"sqlite3://na:na@/{path}/{DATABASE_NAME}"
                };
                C1 = new ClusterConfiguration
                {
                    Cluster_size = 1,
                    DB_1 = $"sqlite3://na:na@/{path}/{DATABASE_NAMEC1}"
                };
                C2 = new ClusterConfiguration
                {
                    Cluster_size = 1,
                    DB_1 = $"sqlite3://na:na@/{path}/{DATABASE_NAMEC2}"
                };
                Hybrid = new HybridConfiguration
                {
                    Res_Feat_Ekey = "C1",
                    Res_Feat_Lkey = "C1",
                    Res_Feat_Stat = "C1",
                    Lib_Feat = "C2",
                    Lib_Feat_Hkey = "C2"
                };
                break;
            case EntityResolutionDatabase.Postgres:
                var pgConnectionSettings = ConfigurationHelper.GetConnectionString(ExternalSourceTypes.Postgres, erSettings.ConnectionString);

                Sql = new SqlConnectionConfiguration
                {
                    Connection = @$"postgresql://{pgConnectionSettings.UserName}:{pgConnectionSettings.Password}@{pgConnectionSettings.ServerAddress}:{pgConnectionSettings.Port}:{pgConnectionSettings.DatabaseName}/?schema=public"
                };
                break;
            case EntityResolutionDatabase.SqlServer:
                var sqlConnectionSettings = ConfigurationHelper.GetConnectionString(ExternalSourceTypes.SqlServer, erSettings.ConnectionString);

                Sql = new SqlConnectionConfiguration
                {
                    Connection = @$"mssql://{sqlConnectionSettings.UserName}:{sqlConnectionSettings.Password}@{sqlConnectionSettings.ServerAddress}:{sqlConnectionSettings.Port}:{sqlConnectionSettings.DatabaseName}/?driver=ODBC Driver 17 for SQL Server"
                };
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [JsonIgnoreAttribute]
    public string DbPath { get; private set; }
    [JsonIgnoreAttribute]
    public string DbPathC1 { get; private set; }
    [JsonIgnoreAttribute]
    public string DbPathC2 { get; private set; }
    public PathConfiguration Pipeline { get; set; }
    public SqlConnectionConfiguration Sql { get; set; }
    public ClusterConfiguration C1 { get; set; }
    public ClusterConfiguration C2 { get; set; }
    public HybridConfiguration Hybrid { get; set; }

    public static SenzingConfiguration Default => new SenzingConfiguration
    {
        Sql = new SqlConnectionConfiguration
        {
            Connection = "sqlite3://na:na@/d:/senzing/G2C.db"
        },
        Pipeline = new PathConfiguration
        {
            ConfigPath = "D:/senzing/api/etc",
            ResourcePath = "d:/senzing/api/resources",
            SupportPath = "d:/senzing/api/data",
            Enable_Diagnostic_Logging = "Y",
            Enable_Score_Logging = "Y"
        }
    };
}

public class SqlConnectionConfiguration
{
    public string Backend { get; set; }
    public string Connection { get; set; }
}

public class PathConfiguration
{
    public string ConfigPath { get; set; }
    public string ResourcePath { get; set; }
    public string SupportPath { get; set; }
    public string Enable_Diagnostic_Logging { get; set; }
    public string Enable_Score_Logging { get; set; }
    public string LICENSESTRINGBASE64 { get; set; } = "AQAAADgCAAAAAAAAV2luUHVyZSAtIFNlcmdleSBaYWhhcm92IDxzZXJnZXkuemFoYXJvdkB3aW5wdXJlLmNvbT4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAASVNWIFBhcnRuZXIgYWdyZWVtZW50IC0gc3VwcG9ydEBzZW56aW5nLmNvbQAAAAAAAAAAAAAAAAAAAAAAAAAAADIwMjUtMTAtMjIAAAAAAAAAAAAARU5EIFVTRVIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEJST05aRQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAmDX8AAICWmAAAAAAAMjAyNi0wOS0zMAAAAAAAAAAAAABNT05USExZAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADlrvNC2Q/wLzmABlJ4JE6+nAYHJEHiiaxQM+sJ52A6aRY1LrkODTXiqhYsD1t7f8j8xPEOmPRUQ7b+mQuytB7bKNPyfkJKUqixoFn2UBrc08dcp6V+eSAPj0XEMQM30JOrfbaGcfGe2AtnsG0H0OXpp4a45/SsgvidJB3MSTxldzcMATLQ0fMkpgcCewgCuRcO3k3jLgIxSD9TiD7y2L0oRlv1on4r7c3AGDXpKMcJorwh1SX+nr/J7vn5hIfXVCOiXjWUG6qbpUbI56BeA5j76iylsQy6aSCZN7xwie32yd3DQzUgGt++h9IgOmJy+BAhk8aAhCp3nWVwaS5wBlaxl561N+3xui+cu3y/4HyO3e+CqH2aAuiB16QNj6rBNpdGi9Ny4A9fkxZDQ1uh20nAaDbsfWGCadXoMpuS08MWwLMpyX5+wirKMNrVeHvLMp8N/SgAnKMBSGXmFVDYr/mX26+ehTVWQV2YEfXtQGSg+d2eRw98VtkmlMlcsCIQ+vGRNvDo7ySeAXZK2vDjtFuWTBsztmdXgAPrYAnNuErtg9amTfDvqUe7aKlv1xgoTwsbjt3BSz13n0nPe1dZ12Kniy0A92R6U0NMpyFqlwMqFe//8LthHLNm1jVYN7DoWxOQVFEXnRyMMi/xMWO0Z5tma1dytjktrO9Y775C7I53E";
}

public class ClusterConfiguration
{
    public int Cluster_size { get; set; }
    public string DB_1 { get; set; }
}

public class HybridConfiguration
{
    public string Res_Feat_Ekey { get; set; }
    public string Res_Feat_Lkey { get; set; }
    public string Res_Feat_Stat { get; set; }
    public string Lib_Feat { get; set; }
    public string Lib_Feat_Hkey { get; set; }
}