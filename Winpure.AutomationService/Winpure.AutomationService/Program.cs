using System;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using Newtonsoft.Json;
using WinPure.Common.Enums;

namespace Winpure.AutomationService
{
    static class Program
    {
        internal static string ServiceName = "AutomationService";
        internal static string ServiceDisplayName = "WinPure Automation service";

        internal static ProgramType CurrentProgramVersion = ProgramType.CamEnt;
        internal static bool _writeExtendLogs = false;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            var servicesToRun = new ServiceBase[]
            {
                new WinPureAutomation()
            };

            var extendLogConfiguration = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "appsettings.json");

            if (File.Exists(extendLogConfiguration))
            {
                var data = File.ReadAllText(extendLogConfiguration);
                var configuration = JsonConvert.DeserializeObject<ExtendLogConfiguration>(data);
            }

            if (args.Length > 0)
            {
                var p = args[0];
                switch (p.ToUpper())
                {
                    case "/INSTALL": InstallService(ServiceName); break;
                    case "/UNINSTALL": UninstallService(ServiceName); break;
                    case "/REINSTALL":
                        UninstallService(ServiceName);
                        InstallService(ServiceName); break;
                    case "/START": StartService(); break;
                    case "/STOP": ServiceStop(); break;
                    case "/D": RunAutomationDebug(); break;
                }
            }
            else
            {
                ServiceBase.Run(servicesToRun);
            }

        }

        public static void WriteExtendLog(string message, Exception ex = null)
        {
            if (!_writeExtendLogs) return;

            var sb = new StringBuilder();
            sb.AppendLine($"\r\n{DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss")}: {message}");
            if (ex != null)
            {
                sb.AppendLine($"Exception message: {ex.Message}");
                sb.AppendLine($"nStackTrace: {ex.StackTrace}");
            }

            File.AppendAllText("AutomationErrorLog.txt", sb.ToString());
        }

        private static void ServiceStop()
        {
            try
            {
                var service = new ServiceController(ServiceName);
                var timeout = TimeSpan.FromMilliseconds(5000);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch (Exception ex)
            {
                WriteExtendLog("Could not Stop the service in timeout of 5000.", ex);
            }
        }

        private static void StartService()
        {
            WriteExtendLog("DEBUG MESSAGE: Try start the Winpure Automation Service");
            try
            {
                var service = new ServiceController(ServiceName);
                var timeout = TimeSpan.FromMilliseconds(5000);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                WriteExtendLog("DEBUG MESSAGE: Service started");
            }
            catch (Exception ex)
            {
                WriteExtendLog("Could not Start the service.", ex);
            }
        }

        private static void RunAutomationDebug()
        {
            var scheduleChecker = new CheckSchedule();
            //scheduleChecker.RunConfiguration(9,1, new CancellationToken());
            scheduleChecker.StartWatching();
        }

        private static void InstallService(string clientName)
        {
            ServiceName = clientName;
            WriteExtendLog("Try to install service");
            try
            {
                ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location, "/ServiceName=" + clientName });
            }
            catch (Exception ex)
            {
                WriteExtendLog("Service installation error", ex);
                Console.WriteLine("ERROR!");
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine(@"Press enter to finish application");
        }

        private static void UninstallService(string clientName)
        {
            WriteExtendLog("Try to uninstall service");
            ServiceName = clientName;
            try
            {
                ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location, "/ServiceName=" + clientName });
                Console.WriteLine(@"Press enter to finish application");
            }
            catch (Exception ex)
            {
                WriteExtendLog("Service uninstall error", ex);
                Console.WriteLine("ERROR!");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
