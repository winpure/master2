using System.Collections.Generic;
using System.IO;
using System.Management;
using WinPure.Common.Models;
using System.Security.Principal;
using DriveType = WinPure.Common.Models.DriveType;

namespace WinPure.Common.Helpers;

internal static class SystemInfoHelper
{
    private const string ProcessorKey = "Win32_Processor";
    private const string ComputerKey = "Win32_ComputerSystem";
    private const string SystemKey = "Win32_OperatingSystem";
    private static List<Models.DriveInfo> _driveinfo;

    /// <summary>
    /// Returns the current Windows user in DOMAIN\User format.
    /// Works on .NET Framework 4.8 and .NET 8.
    /// </summary>
    public static string GetCurrentUserQualified()
    {
        // Environment-based way is fast and reliable on Windows
        // If the machine is not domain-joined, UserDomainName is the machine name.

        //var domain = Environment.UserDomainName;
        //var user = Environment.UserName;

        //if (!string.IsNullOrWhiteSpace(domain))
        //    return domain + "\\" + user;

        if (!string.IsNullOrWhiteSpace(Environment.UserName))
            return Environment.UserName;

        // Fallback to WindowsIdentity if domain is not available for some reason
        using (var wi = WindowsIdentity.GetCurrent())
        {
            return wi?.Name ?? "Unknown";
        }
    }

    public static HostSystemInformation GetSystemInformation()
    {
        var hostSystemInformation = new HostSystemInformation();
        try
        {
            var proc = new ManagementObjectSearcher($"select * from {ProcessorKey}").Get().Cast<ManagementBaseObject>();
            hostSystemInformation.Processor = proc.Max(x => x["Name"].ToString());
            hostSystemInformation.ProcessorDescription = proc.Max(x => x["Caption"].ToString());
            hostSystemInformation.CoresCount = proc.Sum(x => int.Parse(x["NumberOfCores"].ToString()));
        }
        catch
        {
        }

        try
        {
            var systemInfo = new ManagementObjectSearcher($"select * from {ComputerKey}").Get().Cast<ManagementBaseObject>();
            hostSystemInformation.ProcessorCount = systemInfo.Sum(x => int.Parse(x["NumberOfProcessors"].ToString()));
            hostSystemInformation.ProcessorThreadCount = systemInfo.Sum(x => int.Parse(x["NumberOfLogicalProcessors"].ToString()));
            var memory = systemInfo.Sum(x => double.Parse(x["TotalPhysicalMemory"].ToString()));
            hostSystemInformation.MemorySize = Math.Round(memory / 1073741824.0, 1, MidpointRounding.AwayFromZero);
        }
        catch
        {
        }

        try
        {
            var systemInfo = new ManagementObjectSearcher($"SELECT * FROM {SystemKey}").Get().Cast<ManagementBaseObject>();
            hostSystemInformation.Windows = systemInfo.Max(x => x["Caption"].ToString());
            hostSystemInformation.FreeMemorySize = Math.Round(systemInfo.Sum(x => double.Parse(x["FreePhysicalMemory"].ToString()) / 1048576.0), 1, MidpointRounding.AwayFromZero);
        }
        catch
        {
        }
        hostSystemInformation.AppBitness = Environment.Is64BitProcess ? "x64" : "x86";
        hostSystemInformation.SystemBitness = Environment.Is64BitOperatingSystem ? "x64" : "x86";

        try
        {
            hostSystemInformation.Drives = GetDrivesInfo();
        }
        catch
        {
        }

        return hostSystemInformation;
    }

    private static List<Models.DriveInfo> GetDrivesInfo()
    {
        if (_driveinfo != null)
            return _driveinfo;

        _driveinfo = new List<Models.DriveInfo>();

        var drives = System.IO.DriveInfo.GetDrives();

        foreach (var drive in drives)
        {
            var driveInfo = new Models.DriveInfo
            {
                Letter = drive.Name,
            };

            try
            {
                (driveInfo.DriveType, driveInfo.ConnectionType) = GetDriveType(drive.RootDirectory.Name);
            }
            catch
            {
                // ignored
            }

            _driveinfo.Add(driveInfo);
        }

        return _driveinfo;
    }

    private static (DriveType, DriveConnectionType) GetDriveType(string path)
    {
        try
        {
            var rootPath = Path.GetPathRoot(path);

            if (string.IsNullOrEmpty(rootPath))
            {
                return (DriveType.Unknown, DriveConnectionType.Unknown);
            }

            rootPath = rootPath[0].ToString();

            var scope = new ManagementScope(@"\\.\root\microsoft\windows\storage");
            scope.Connect();

            using var partitionSearcher = new ManagementObjectSearcher($"select * from MSFT_Partition where DriveLetter='{rootPath}'");
            partitionSearcher.Scope = scope;

            var partitions = partitionSearcher.Get();

            if (partitions.Count == 0)
            {
                return (DriveType.Unknown, DriveConnectionType.Unknown);
            }

            string diskNumber = null;

            foreach (var currentPartition in partitions)
            {
                diskNumber = currentPartition["DiskNumber"].ToString();

                if (!string.IsNullOrEmpty(diskNumber))
                {
                    break;
                }
            }

            if (string.IsNullOrEmpty(diskNumber))
            {
                return (DriveType.Unknown, DriveConnectionType.Unknown);
            }

            using var diskSearcher = new ManagementObjectSearcher($"SELECT * FROM MSFT_PhysicalDisk WHERE DeviceId='{diskNumber}'");
            diskSearcher.Scope = scope;

            var physicalDisks = diskSearcher.Get();

            if (physicalDisks.Count == 0)
            {
                return (DriveType.Unknown, DriveConnectionType.Unknown);
            }

            foreach (var currentDisk in physicalDisks)
            {
                var mediaType = Convert.ToInt16(currentDisk["MediaType"]);
                var busType = Convert.ToInt16(currentDisk["BusType"]);

                var connectionType = busType switch
                {
                    1 => DriveConnectionType.SCSI,
                    2 => DriveConnectionType.ATAPI,
                    3 => DriveConnectionType.ATA,
                    7 => DriveConnectionType.USB,
                    8 => DriveConnectionType.RAID,
                    10 => DriveConnectionType.SAS,
                    11 => DriveConnectionType.SATA,
                    17 => DriveConnectionType.NVMe,
                    _ => DriveConnectionType.Unknown
                };

                switch (mediaType)
                {
                    case 3:
                        return (DriveType.HDD, connectionType);
                    case 4:
                        return (DriveType.SSD, connectionType);
                    default:
                        return (DriveType.Unknown, connectionType);
                }
            }

            return (DriveType.Unknown, DriveConnectionType.Unknown);
        }
        catch
        {
            // TODO: Log error.

            return (DriveType.Unknown, DriveConnectionType.Unknown);
        }
    }
}