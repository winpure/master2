using DriveType = WinPure.Common.Models.DriveType;

namespace WinPure.CleanAndMatch.Support;

public partial class frmSystemInfo : XtraForm
{
    private readonly ThemeDetectionService _themeDetectionService;
    public frmSystemInfo()
    {
        InitializeComponent();

        _themeDetectionService = WinPureUiDependencyResolver.Resolve<ThemeDetectionService>();
        _themeDetectionService.SetReferenceControl(this);

        SetInformation();
    }

    public void SetInformation()
    {
        var systemInformation = SystemInfoHelper.GetSystemInformation();

        var configuration = WinPureUiDependencyResolver.Resolve<IConfigurationService>().Configuration;
        var drive = Path.GetPathRoot(configuration.ErSettings.DataFolder);
        var driveInfo = systemInformation.Drives.FirstOrDefault(x => x.Letter == drive);


        if (systemInformation.CoresCount < 2)
        {
            imgCpu.SvgImage = svgImageCollection1[2];
        }
        else if (systemInformation.CoresCount < 4)
        {
            imgCpu.SvgImage = svgImageCollection1[1];
        }
        else
        {
            imgCpu.SvgImage = svgImageCollection1[0];
            lbCpuCaption.Text = "Sufficient CPU cores to run MatchAI.";
        }

        if (systemInformation.FreeMemorySize < 6)
        {
            imgMemory.SvgImage = svgImageCollection1[2];
        }
        else if (systemInformation.FreeMemorySize < 8)
        {
            imgMemory.SvgImage = svgImageCollection1[1];
        }
        else
        {
            imgMemory.SvgImage = svgImageCollection1[0];
            lbMemoryCaption.Text = "Memory is sufficient to run MatchAI.";
        }

        if (driveInfo.DriveType != DriveType.SSD ||
            (driveInfo.ConnectionType != DriveConnectionType.RAID &&
             driveInfo.ConnectionType != DriveConnectionType.SATA &&
             driveInfo.ConnectionType != DriveConnectionType.NVMe))
        {
            imgHdd.SvgImage = svgImageCollection1[1];
            var driveMessage = driveInfo.DriveType == DriveType.SSD ?
                "fast SSD"
                : driveInfo.DriveType == DriveType.USB
                    ? "slow USB"
                    : driveInfo.DriveType.ToString();
            var connectionType = (driveInfo.ConnectionType == DriveConnectionType.RAID
                                  || driveInfo.ConnectionType == DriveConnectionType.SATA
                                  || driveInfo.ConnectionType == DriveConnectionType.NVMe)
                ? $"fast {driveInfo.ConnectionType}"
                : $"slow {driveInfo.ConnectionType}";
            txtHardDrive.Text = $"• System uses {driveMessage} drive with {connectionType} bus type.";
        }
        else
        {
            imgHdd.SvgImage = svgImageCollection1[0];
            lbHardDriveCaption.Text = "Sufficient drive to run MatchAI.";
        }

        txtCpu.Text = $"• System has {systemInformation.CoresCount} physical cores and {systemInformation.ProcessorThreadCount} logical cores.\r\n• At least 4 physical CPU cores are recommended to run MatchAI.\r\n• The minimum requirement to run MatchAI is 2 physical CPU cores.";
        txtMemory.Text = $"• Total system memory is {systemInformation.MemorySize} GB with {systemInformation.FreeMemorySize} GB available.\r\n• At least 8.0 GB of available memory is recommended to run MatchAI.\r\n• The minimum requirement to run MatchAI is 6.0 GB available memory.";

        if(_themeDetectionService.IsDarkTheme())
        {
            //set readonly appearance for the text boxes
            txtCpu.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
            txtCpu.Properties.AppearanceReadOnly.Options.UseBackColor = true;

            txtMemory.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
            txtMemory.Properties.AppearanceReadOnly.Options.UseBackColor = true;

            txtHardDrive.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
            txtHardDrive.Properties.AppearanceReadOnly.Options.UseBackColor = true;
        }


    }
}