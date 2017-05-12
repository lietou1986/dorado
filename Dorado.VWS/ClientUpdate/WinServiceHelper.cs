using System.ServiceProcess;

namespace ClientUpdate
{
    public class WinServiceHelper
    {
        /// <summary>
        ///     开启服务
        /// </summary>
        /// <param name = "name">服务简称</param>
        /// <returns>结果</returns>
        internal static bool StartService(string name)
        {
            var sc = new ServiceController(name);
            if (!ServiceControllerStatus.Running.Equals(sc.Status) &&
                !ServiceControllerStatus.StartPending.Equals(sc.Status))
            {
                sc.Start();
            }

            return true;
        }

        /// <summary>
        ///     关闭服务
        /// </summary>
        /// <param name = "name">服务简称</param>
        /// <returns>结果</returns>
        internal static bool StopService(string name)
        {
            var sc = new ServiceController(name);
            if (!ServiceControllerStatus.Stopped.Equals(sc.Status) &&
                !ServiceControllerStatus.StopPending.Equals(sc.Status))
            {
                sc.Stop();
            }

            return true;
        }

        /// <summary>
        ///     重启服务
        /// </summary>
        /// <param name = "name">服务简称</param>
        /// <returns>结果</returns>
        internal static bool ReStartService(string name)
        {
            var sc = new ServiceController(name);
            if (sc == null)
            {
                return false;
            }

            return StopService(name) && StartService(name);
        }
    }
}