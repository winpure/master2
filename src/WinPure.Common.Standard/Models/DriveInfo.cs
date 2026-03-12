namespace WinPure.Common.Models;

internal class DriveInfo
{
    public string Letter { get; set; }
    public DriveConnectionType ConnectionType { get; set; } = DriveConnectionType.Unknown;
    public DriveType DriveType { get; set; } = DriveType.Unknown;
}