/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/9 17:05:00
 * 作者：len
 * 联系方式：len@dorado.com
 * 本类主要用途描述：Windows服务操作
 *  -------------------------------------------------------------------------*/

#region using

using System.ServiceProcess;

#endregion using

namespace Dorado.VWS.ClientHost
{
    /// <summary>
    ///     Windows服务操作
    /// </summary>
    internal class WinServiceHelper
    {
        /// <summary>
        ///     开启服务
        /// </summary>
        /// <param name = "name">服务简称</param>
        /// <returns>结果</returns>
        internal bool StartService(string name)
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
        internal bool StopService(string name)
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
        internal bool ReStartService(string name)
        {
            return StopService(name) && StartService(name);
        }
    }
}