using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;

namespace Dorado.Package.WindowsService
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SERVICE_STATUS
    {
        public int serviceType;
        public int currentState;
        public int controlsAccepted;
        public int win32ExitCode;
        public int serviceSpecificExitCode;
        public int checkPoint;
        public int waitHint;
    }

    public enum State
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    partial class PackageService : ServiceBase
    {
        [DllImport("ADVAPI32.DLL", EntryPoint = "SetServiceStatus")]
        public static extern bool SetServiceStatus(
                        IntPtr hServiceStatus,
                        ref SERVICE_STATUS lpServiceStatus
                        );

        private SERVICE_STATUS serviceStatus;

        public PackageService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            try
            {
                IntPtr handle = this.ServiceHandle;
                serviceStatus.currentState = (int)State.SERVICE_START_PENDING;
                SetServiceStatus(handle, ref serviceStatus);

                ThreadPool.QueueUserWorkItem(new WaitCallback(StartPackageService));

                serviceStatus.currentState = (int)State.SERVICE_RUNNING;
                SetServiceStatus(handle, ref serviceStatus);
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("Dorado.Package.WindowsService", ex);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
                LoggerWrapper.Logger.Error("Dorado.Package.WindowsService.AppDomain.UnhandledException", ex);
        }

        private void StartPackageService(object state)
        {
            LoggerWrapper.Logger.Info("The Dorado.Package.WindowsService have been opened.");

            Dorado.Package.ServiceImp.PackageProvider.Instance.Running();
        }

        protected override void OnStop()
        {
            IntPtr handle = this.ServiceHandle;
            serviceStatus.currentState = (int)State.SERVICE_STOP_PENDING;
            SetServiceStatus(handle, ref serviceStatus);

            Dorado.Package.ServiceImp.PackageProvider.Instance.Stop();

            serviceStatus.currentState = (int)State.SERVICE_STOPPED;
            SetServiceStatus(handle, ref serviceStatus);
            LoggerWrapper.Logger.Info("The Dorado.Package.WindowsService have been stopped.");
        }
    }
}