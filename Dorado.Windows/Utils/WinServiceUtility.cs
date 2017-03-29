using System.Diagnostics.Contracts;
using System.Linq;
using System.ServiceProcess;

namespace Dorado.Windows.Utils
{
    /// <summary>
    /// window服务操作帮助类
    /// </summary>
    public class WinServiceUtility
    {
        /// <summary>
        /// 启动指定名称的window服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        public static bool StartService(string serviceName)
        {
            Contract.Requires(!string.IsNullOrEmpty(serviceName), "服务名称不能为空！");
            Contract.Requires(IsServiceExists(serviceName), "指定的服务不存在！");

            ServiceController sc = new ServiceController(serviceName);
            if (sc.Status != ServiceControllerStatus.StartPending && sc.Status != ServiceControllerStatus.Running)
            {
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running);
            }
            return sc.Status == ServiceControllerStatus.Running;
        }

        /// <summary>
        /// 停止指定名称的window服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        public static bool StopService(string serviceName)
        {
            Contract.Requires(!string.IsNullOrEmpty(serviceName), "服务名称不能为空！");
            Contract.Requires(IsServiceExists(serviceName), "指定的服务不存在！");

            ServiceController sc = new ServiceController(serviceName);
            if (sc.Status != ServiceControllerStatus.StopPending && sc.Status != ServiceControllerStatus.Stopped)
            {
                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped);
            }
            return sc.Status == ServiceControllerStatus.Stopped;
        }

        /// <summary>
        /// 暂停指定名称的window服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        public static bool PauseService(string serviceName)
        {
            Contract.Requires(!string.IsNullOrEmpty(serviceName), "服务名称不能为空！");
            Contract.Requires(IsServiceExists(serviceName), "指定的服务不存在！");

            ServiceController sc = new ServiceController(serviceName);
            if (sc.Status != ServiceControllerStatus.PausePending && sc.Status != ServiceControllerStatus.Paused)
            {
                sc.Pause();
                sc.WaitForStatus(ServiceControllerStatus.Paused);
            }
            return sc.Status == ServiceControllerStatus.Paused;
        }

        /// <summary>
        /// 获取指定服务的当前状态
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        public static ServiceControllerStatus GetServiceStatus(string serviceName)
        {
            Contract.Requires(!string.IsNullOrEmpty(serviceName), "服务名称不能为空！");
            Contract.Requires(IsServiceExists(serviceName), "指定的服务不存在！");

            ServiceController sc = new ServiceController(serviceName);
            return sc.Status;
        }

        /// <summary>
        /// 判断指定的服务是否正在运行或即将运行
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        public static bool IsServiceRunning(string serviceName)
        {
            ServiceControllerStatus ss = GetServiceStatus(serviceName);
            return (ss == ServiceControllerStatus.StartPending || ss == ServiceControllerStatus.Running);
        }

        /// <summary>
        /// 判断指定的服务是否已经停止或正在停止
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static bool IsServiceStopped(string serviceName)
        {
            ServiceControllerStatus ss = GetServiceStatus(serviceName);
            return (ss == ServiceControllerStatus.StopPending || ss == ServiceControllerStatus.Stopped);
        }

        /// <summary>
        /// 判断指定的服务是否已经挂起或即将挂起
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        public static bool IsServicePaused(string serviceName)
        {
            ServiceControllerStatus ss = GetServiceStatus(serviceName);
            return (ss == ServiceControllerStatus.PausePending || ss == ServiceControllerStatus.Paused);
        }

        /// <summary>
        /// 判断指定名称的window服务是否存在
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        public static bool IsServiceExists(string serviceName)
        {
            Contract.Requires(!string.IsNullOrEmpty(serviceName), "服务名称不能为空！");

            ServiceController[] services = ServiceController.GetServices();
            return services.FirstOrDefault(n => n.ServiceName == serviceName) != null;
        }
    }
}