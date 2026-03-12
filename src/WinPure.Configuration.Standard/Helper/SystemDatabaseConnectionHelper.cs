using Microsoft.Data.Sqlite;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace WinPure.Configuration.Helper;

[Obfuscation(Exclude = true, ApplyToMembers = true)]
internal static class SystemDatabaseConnectionHelper
{
    public static string GetConnectionString(string databasePath)
    {
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadWriteCreate
        };
        return builder.ConnectionString;
    }

    public static string GetReaderConnectionString(string databasePath)
    {
        return $"Data Source={databasePath};Read Only=True;Pooling=True;Journal Mode=WAL;Synchronous=NORMAL;BusyTimeout=5000;";
    }

    public static string GetWriterConnectionString(string databasePath)
    {
        return $"Data Source={databasePath};Read Only=false;Pooling=True;Journal Mode=WAL;Synchronous=NORMAL;BusyTimeout=5000;";
    }

    public static string GetDatabasePath(bool useRelativePath, string path, string databaseFileName)
    {
        var databasePath = path;

        if (useRelativePath)
        {
            var currentLocation = Path.GetDirectoryName(AssemblyHelper.GetCurrentLocation());
            databasePath = Path.Combine(currentLocation, databasePath);
        }

        if (!Directory.Exists(databasePath))
        {
            Directory.CreateDirectory(databasePath);
        }

        return Path.Combine(databasePath, databaseFileName);
    }

    /// <summary>
    /// Applies recommended PRAGMAs for bulk operations. Call right after opening a connection.
    /// For writers you may consider SyncMode=Off if durability is not critical.
    /// </summary>
    public static void ApplyRuntimePragmas(SQLiteConnection connection, bool isWriter)
    {
        using var cmd = connection.CreateCommand();
        // WAL is already set via connection string, but setting again is idempotent.
        cmd.CommandText =
            "PRAGMA journal_mode=WAL;" +
            "PRAGMA synchronous=NORMAL;" +
            "PRAGMA temp_store=MEMORY;" +
            // Tune mmap if your environment benefits; harmless if unsupported.
            "PRAGMA mmap_size=268435456;";
        cmd.ExecuteNonQuery();

        if (isWriter)
        {
            // Optional extra boosts for heavy bulk inserts:

            // Increase cache size (negative means size in KB, so -200000 = ~200 MB page cache in memory).
            cmd.CommandText = "PRAGMA cache_size=-200000;";
            cmd.ExecuteNonQuery();

            // Use memory for temporary objects (faster than disk temp files).
            cmd.CommandText = "PRAGMA temp_store=MEMORY;";
            cmd.ExecuteNonQuery();

            // Disable full fsyncs – fastest but risk of corruption if power loss.
            // NORMAL is safer; OFF gives max throughput.
            cmd.CommandText = "PRAGMA synchronous=OFF;";
            cmd.ExecuteNonQuery();

            // Adjust locking behavior to minimize contention.
            cmd.CommandText = "PRAGMA locking_mode=NORMAL;";
            cmd.ExecuteNonQuery();

            // Enlarge the write-ahead log (WAL) autocheckpoint threshold – fewer fsyncs.
            cmd.CommandText = "PRAGMA wal_autocheckpoint=1000;";
            cmd.ExecuteNonQuery();

            // Increase mmap size for faster I/O if supported (256 MB here).
            cmd.CommandText = "PRAGMA mmap_size=268435456;";
            cmd.ExecuteNonQuery();
        }
    }
}