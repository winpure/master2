using System.Data;
using System.IO;
using System.Text;

namespace WinPure.Common.Helpers;

internal static class FileHelper
{
    public static Encoding GetEncoding(int codePage)
    {
        switch (codePage)
        {
            case 65000:
                return Encoding.UTF7;
            case 65001:
                return Encoding.UTF8;
            case 65005:
                return Encoding.UTF32;
            case 65006:
                return Encoding.BigEndianUnicode;

            case 20127:
                return Encoding.ASCII;

            default:
                return Encoding.GetEncoding(codePage);
        }
    }

    public static void CreateOrOverrideFile(string destinationFile, string fileContent)
    {
        File.WriteAllText(destinationFile, fileContent);
    }

    public static string ReadFile(string destinationFile)
    {
        if (File.Exists(destinationFile))
        {
            return File.ReadAllText(destinationFile);
        }
        throw new FileNotFoundException($"File {destinationFile} not found");
    }

    public static DataTable GetTextEncoding()
    {
        var dt = new DataTable();
        dt.Columns.Add("Id", typeof(int));
        dt.Columns.Add("Text", typeof(string));
        var newRow = dt.NewRow();
        newRow[0] = 863;
        newRow[1] = "French-Canadian (DOS)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 20106;
        newRow[1] = "German (IA5)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 20277;
        newRow[1] = "IBM EBCDIC (Denmark-Norway)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 1142;
        newRow[1] = "IBM EBCDIC (Denmark-Norway-Euro)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 20278;
        newRow[1] = "IBM EBCDIC (Finland-Sweden)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 1143;
        newRow[1] = "IBM EBCDIC (Finland-Sweden-Euro)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 20297;
        newRow[1] = "IBM EBCDIC (France)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 1147;
        newRow[1] = "IBM EBCDIC (France-Euro)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 20273;
        newRow[1] = "IBM EBCDIC (Germany)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 1141;
        newRow[1] = "IBM EBCDIC (Germany-Euro)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 20871;
        newRow[1] = "IBM EBCDIC (Icelandic)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 1149;
        newRow[1] = "IBM EBCDIC (Icelandic-Euro)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 500;
        newRow[1] = "IBM EBCDIC (International)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 1148;
        newRow[1] = "IBM EBCDIC (International-Euro)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 20280;
        newRow[1] = "IBM EBCDIC (Italy)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 1144;
        newRow[1] = "IBM EBCDIC (Italy-Euro)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 20284;
        newRow[1] = "IBM EBCDIC (Spain)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 1145;
        newRow[1] = "IBM EBCDIC (Spain-Euro)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 1026;
        newRow[1] = "IBM EBCDIC (Turkish Latin-5)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 20905;
        newRow[1] = "IBM EBCDIC (Turkish)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 20285;
        newRow[1] = "IBM EBCDIC (UK)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 1146;
        newRow[1] = "IBM EBCDIC (UK-Euro)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 37;
        newRow[1] = "IBM EBCDIC (US-Canada)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 1140;
        newRow[1] = "IBM EBCDIC (US-Canada-Euro)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 20924;
        newRow[1] = "IBM Latin-1 (IBM00924)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 1047;
        newRow[1] = "IBM Latin-1 (IBM01047)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 861;
        newRow[1] = "Icelandic (DOS)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 10079;
        newRow[1] = "Icelandic (Mac)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 20269;
        newRow[1] = "ISO-6937";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 28605;
        newRow[1] = "Latin 9 (ISO)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 865;
        newRow[1] = "Nordic (DOS)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 20108;
        newRow[1] = "Norwegian (IA5)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 855;
        newRow[1] = "OEM Cyrillic";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 858;
        newRow[1] = "OEM Multilingual Latin I";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 437;
        newRow[1] = "OEM United States";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 860;
        newRow[1] = "Portuguese (DOS)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 20107;
        newRow[1] = "Swedish (IA5)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 20261;
        newRow[1] = "T.61";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 65000;
        newRow[1] = "Unicode (UTF-7)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 65001;
        newRow[1] = "Unicode (UTF-8)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 1200;
        newRow[1] = "Unicode";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 1201;
        newRow[1] = "Unicode (Big-Endian)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 20127;
        newRow[1] = "US-ASCII";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 850;
        newRow[1] = "Western European (DOS)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 20105;
        newRow[1] = "Western European (IA5)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 28591;
        newRow[1] = "Western European (ISO)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 10000;
        newRow[1] = "Western European (Mac)";
        dt.Rows.Add(newRow);
        newRow = dt.NewRow();
        newRow[0] = 1252;
        newRow[1] = "Western European (Windows)";
        dt.Rows.Add(newRow);

        return dt;
    }

    public static bool SafeDeleteFileWithLogging(IWpLogger logger, string filePath, string errorMessage)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return true;
        }
        catch (Exception e)
        {
            logger.Information(errorMessage, e);
            return false;
        }
    }

    public static bool SafeDeleteFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return true;
        }
        catch
        {
            //ignore
        }
        return false;
    }

    /// <summary>
    /// Returns the size of a SQLite database file in a human-readable format.
    /// Rules:
    /// - Less than 0.1 Mb → round up to 0.1Mb
    /// - Less than 1 Mb   → show with one decimal (e.g. 0.2Mb)
    /// - From 1 Mb to 1 Gb → show as integer Mb (e.g. 123Mb)
    /// - Greater than 1 Gb → show in Gb with one decimal (e.g. 1.5Gb)
    /// </summary>
    public static string GetDatabaseSize(string dbPath)
    {
        if (string.IsNullOrWhiteSpace(dbPath) || !File.Exists(dbPath))
            return "File not exists";

        long sizeBytes = new FileInfo(dbPath).Length;

        // Convert bytes to megabytes
        double sizeMb = sizeBytes / (1024.0 * 1024.0);

        // Round up very small sizes
        if (sizeMb < 0.1)
            sizeMb = 0.1;

        if (sizeMb < 1.0)
        {
            // Below 1 Mb → show with one decimal
            return $"{sizeMb:0.0}Mb";
        }

        if (sizeMb < 1024.0)
        {
            // Between 1 Mb and 1 Gb → show as integer Mb
            return $"{(int)sizeMb}Mb";
        }

        // Above 1 Gb → show in Gb with one decimal
        double sizeGb = sizeMb / 1024.0;
        return $"{sizeGb:0.0}Gb";
    }
}