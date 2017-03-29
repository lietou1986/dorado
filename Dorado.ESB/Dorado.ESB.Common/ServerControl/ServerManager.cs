using System.ServiceProcess;

namespace Dorado.ESB.Common.ServerControl
{
    /// <summary>
    /// 服务操作类
    /// </summary>
    public class ServerManager
    {
        /// <summary>
        /// 获取所有服务
        /// </summary>
        public static ServiceController[] GetAllServices
        {
            get
            {
                return System.ServiceProcess.ServiceController.GetServices();
            }
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="strServiceName">服务名</param>
        /// <returns></returns>
        public static ServiceController GetServiceByName(string ServiceName)
        {
            try
            {
                foreach (ServiceController sc in ServiceController.GetServices())
                {
                    if (sc.ServiceName.ToLower().Trim() == ServiceName.ToLower().Trim())
                    {
                        return sc;
                    }
                } //end foreach
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取指定类型的服务
        /// </summary>
        /// <param name="ServiceType">服务类型</param>
        /// <returns></returns>
        public static object GetService(ServiceType serviceType)
        {
            ServiceController[] allServices = GetAllServices;
            foreach (ServiceController sc in allServices)
            {
                if (sc.ServiceType == serviceType)
                {
                    return sc;
                }
            }
            return null;
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="Service">服务对象</param>
        /// <returns></returns>
        public static bool StopService(string ServiceName)
        {
            try
            {
                ServiceController Service = new ServiceController(ServiceName);
                if (Service.CanStop && Service.Status != ServiceControllerStatus.Stopped && Service.Status != ServiceControllerStatus.StopPending)
                {
                    Service.Stop();
                    Service.WaitForStatus(ServiceControllerStatus.Stopped);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="Service">服务对象</param>
        /// <returns></returns>
        public static bool StartService(string ServiceName)
        {
            try
            {
                ServiceController Service = new ServiceController(ServiceName);

                if (Service.Status != ServiceControllerStatus.Running && Service.Status != ServiceControllerStatus.StartPending)
                {
                    Service.Start();
                    Service.WaitForStatus(ServiceControllerStatus.Running);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 重启服务
        /// </summary>
        /// <param name="Service"></param>
        /// <returns></returns>
        public static bool ResetService(string ServiceName)
        {
            try
            {
                if (StopService(ServiceName))
                    return StartService(ServiceName);
                return false;
            }
            catch
            { return false; }
        }

        /// <summary>
        /// 暂停服务
        /// </summary>
        /// <param name="Service"></param>
        /// <returns></returns>
        public static bool PauseService(string ServiceName)
        {
            try
            {
                ServiceController Service = new ServiceController(ServiceName);
                if (Service.Status != ServiceControllerStatus.Paused && Service.Status != ServiceControllerStatus.PausePending)
                {
                    Service.Pause();
                    Service.WaitForStatus(ServiceControllerStatus.Paused);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 继续服务
        /// </summary>
        /// <param name="Service"></param>
        /// <returns></returns>
        public static bool ResumeService(string ServiceName)
        {
            try
            {
                ServiceController Service = new ServiceController(ServiceName);
                if (Service.Status == ServiceControllerStatus.Paused)
                {
                    Service.Continue();
                    Service.WaitForStatus(ServiceControllerStatus.Running);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}