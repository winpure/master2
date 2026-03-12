namespace WinPure.Common.Models;

internal enum DriveConnectionType
{
    Unknown = 0,
    SCSI = 1,
    ATAPI = 2,
    ATA = 3,
    USB = 7,
    RAID = 8,
    SAS = 10,
    SATA = 11,
    NVMe = 17,
}