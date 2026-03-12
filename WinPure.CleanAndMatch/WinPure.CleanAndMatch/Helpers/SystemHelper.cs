using System.Security.Principal;

namespace WinPure.CleanAndMatch.Helpers;

public static class SystemHelper
{
    public static bool IsAdministrator()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public static (bool, string) IsAutomationModuleInstalled()
    {
        var solutionFolder = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        var automationPath = Path.Combine(solutionFolder, "Automation", "Winpure.AutomationService.exe");
        var isAutomationModuleInstalled = File.Exists(automationPath);
        return (isAutomationModuleInstalled, automationPath);
    }

    public static void SetAutomationWindowsService(bool isAutomationEnabled)
    {
        var (isInstalled, automationPath) = IsAutomationModuleInstalled();

        if (!isInstalled) return;

        System.Diagnostics.ProcessStartInfo myProcessInfo = new System.Diagnostics.ProcessStartInfo(); //Initializes a new ProcessStartInfo of name myProcessInfo
        myProcessInfo.FileName = automationPath; //Sets the FileName property of myProcessInfo to %SystemRoot%\System32\cmd.exe where %SystemRoot% is a system variable which is expanded using Environment.ExpandEnvironmentVariables

        //Sets the arguments to cd..
        myProcessInfo.Arguments = isAutomationEnabled
            ? "/START"
            : "/STOP";

        var proc = System.Diagnostics.Process.Start(myProcessInfo);
        proc.WaitForExit();
    }
}