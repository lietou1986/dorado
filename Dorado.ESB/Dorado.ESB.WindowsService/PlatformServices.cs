using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.ESB.Core;
using Dorado.ESB.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;

namespace Dorado.ESB.WindowsService
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

    partial class PlatformServices : ServiceBase
    {
        [DllImport("ADVAPI32.DLL", EntryPoint = "SetServiceStatus")]
        public static extern bool SetServiceStatus(
                        IntPtr hServiceStatus,
                        ref SERVICE_STATUS lpServiceStatus
                        );

        private SERVICE_STATUS serviceStatus;

        public PlatformServices()
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

                string baseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                Directory.SetCurrentDirectory(baseDir);

                ThreadPool.QueueUserWorkItem(new WaitCallback(StartPlatformServer));

                serviceStatus.currentState = (int)State.SERVICE_RUNNING;
                SetServiceStatus(handle, ref serviceStatus);
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("Exception Policy", ex);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
                LoggerWrapper.Logger.Error("Dorado PlatformServices.WindowsService", ex);
        }

        private void StartPlatformServer(object state)
        {
            #region Extension

            //try
            //{
            //    if (ConfigurationManager.AppSettings["PowerShellServerHost"].ToLower() == "enabled")
            //    {
            //        //----------------------------------------
            //        //Modify by jiangsong 2009-11-13
            //        //Dorado.ESB.ServiceHost.PowershellCmdServiceHost.Instance.Start();
            //        //----------------------------------------

            //        LoggerWrapper.Logger.Info("Powershell CMD ServerHost Started  " + DateTime.Now.ToString(), "PlatformWcf Server");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LoggerWrapper.Logger.Info("Error Start PowerShell CMD ServiceHost : " + ex.ToString(), "Dorado PlatformServices.WindowsService");
            //}

            //try
            //{
            //    if (ConfigurationManager.AppSettings["DlrServiceHost"].ToLower() == "enabled")
            //    {
            //        //----------------------------------------
            //        //Modify by jiangsong 2009-11-13
            //        //Dorado.ESB.DlrServiceHost.Instance.Start();
            //        //----------------------------------------

            //        LoggerWrapper.Logger.Info("Dlr Service Started " + DateTime.Now.ToString());
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LoggerWrapper.Logger.Info("Error Start DlrService ServiceHost : " + ex.ToString(), "Dorado PlatformServices.WindowsService");
            //}

            #endregion Extension

            // boot process
            HostServices services = HostServices.Current.Boot();

            // log event
            List<ServiceMetadataBase> listOfHostedServices = services.GetHostedServices;
            LoggerWrapper.Logger.Info("The list of loaded PlatformServices:");
            for (int ii = 0; ii < listOfHostedServices.Count; ii++)
            {
                LoggerWrapper.Logger.Info(String.Format("[{0}]: {1}", listOfHostedServices[ii].AppDomainHostName, listOfHostedServices[ii].Name));
            }

            // open services
            HostServices.Current.Open();

            // done
            LoggerWrapper.Logger.Info("The PlatformServices have been opened");
        }

        protected override void OnStop()
        {
            try
            {
                if (HostServices.Current != null)
                {
                    IntPtr handle = this.ServiceHandle;
                    serviceStatus.currentState = (int)State.SERVICE_STOP_PENDING;
                    SetServiceStatus(handle, ref serviceStatus);

                    HostServices.Current.Close();
                    LoggerWrapper.Logger.Info("Service stopped   " + DateTime.Now.ToString());

                    try
                    {
                        if (ConfigurationManager.AppSettings["PowerShellServerHost"].ToLower() == "enabled")
                        {
                            //----------------------------------------
                            //Modify by jiangsong 2009-11-13
                            //Dorado.ESB.ServiceHost.PowershellCmdServiceHost.Instance.Stop();
                            //----------------------------------------

                            LoggerWrapper.Logger.Info("Powershell CMD ServiceHost stopped   " + DateTime.Now.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerWrapper.Logger.Info("Error stopping PowerShell CMD ServiceHost : " + ex.ToString());
                    }
                    serviceStatus.currentState = (int)State.SERVICE_STOPPED;
                    SetServiceStatus(handle, ref serviceStatus);
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Info("Error stopping service: " + ex.ToString());
            }
        }
    }
}