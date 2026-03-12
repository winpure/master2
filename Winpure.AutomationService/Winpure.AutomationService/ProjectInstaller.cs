using System.ComponentModel;
using System.ServiceProcess;

namespace Winpure.AutomationService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            serviceInstaller1.Description = Program.ServiceDisplayName;
            serviceInstaller1.DisplayName = Program.ServiceDisplayName;
            serviceInstaller1.ServiceName = Program.ServiceName;
            serviceInstaller1.StartType = ServiceStartMode.Automatic;
            serviceProcessInstaller1.Account = ServiceAccount.LocalSystem;
        }
    }
}
