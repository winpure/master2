using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using Winpure.AutomationService.DependencyInjection;

namespace Winpure.AutomationService
{
    public partial class WinPureAutomation : ServiceBase
    {
        Thread t;
        public WinPureAutomation()
        {
            InitializeComponent();
        }

        private CheckSchedule _checker;

        private void Init()
        {

        }

        private void InitChecking()
        {
            try
            {
                Init();
                _checker = new CheckSchedule();
                _checker.StartWatching();
            }
            catch (Exception ex)
            {
                Program.WriteExtendLog("InitChecking error.", ex);
            }

        }

        protected override void OnStart(string[] args)
        {
            try
            {
                //System.Diagnostics.Debugger.Launch();
                Program.WriteExtendLog("OnStart of automation initiate service provider");
                var sp = WinPureAutomationDependencyResolver.Instance.ServiceProvider;//actually initialize instance
                t = new Thread(InitChecking);
                t.Start();
            }
            catch (Exception ex)
            {
                Program.WriteExtendLog("OnStart Error.", ex);
            }

        }

        protected override void OnStop()
        {
            try
            {
                if (_checker != null)
                {
                    _checker.StopWatching();
                    _checker = null;
                }
                if (t != null)
                {
                    t.Abort();
                    t = null;
                }
            }
            catch (Exception ex)
            {
                Program.WriteExtendLog("OnStop Error.", ex);
            }
        }
    }
}