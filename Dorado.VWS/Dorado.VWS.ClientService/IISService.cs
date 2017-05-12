/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/29 10:14:37
 * 作者：len
 * 联系方式：len@dorado.com
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Threading;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.VWS.Utils;
using Microsoft.Web.Administration;
using Microsoft.Win32;

#endregion using

namespace Dorado.VWS.ClientHost
{
    public class IISService
    {
        private const string ServerName = "localhost";

        /// <summary>
        ///     获取IIS站点状态
        /// </summary>
        /// <returns> 1：运行中； 2：停止状态 ；3：未知状态；4：站点不存在</returns>
        public int IISSiteStatus(string siteName)
        {
            if (!string.IsNullOrEmpty(siteName))
            {
                WebServerTypes webServerType = GetIISServerType();
                if (webServerType.Equals(WebServerTypes.IIS6))
                {
                    return SiteStatusIIS6(siteName);
                }
                if (webServerType.Equals(WebServerTypes.IIS7))
                {
                    return SiteStatusIIS7(siteName);
                }
            }
            return 3;
        }

        /// <summary>
        ///     获取IIS应用程序池状态
        /// </summary>
        /// <returns> 1：运行中； 2：停止状态 ；3：未知状态；4：站点不存在</returns>
        public int IISAppPoolStatus(string appPoolName)
        {
            if (!string.IsNullOrEmpty(appPoolName))
            {
                WebServerTypes webServerType = GetIISServerType();
                if (webServerType.Equals(WebServerTypes.IIS6))
                {
                    return AppPoolStatusIIS6(appPoolName);
                }
                if (webServerType.Equals(WebServerTypes.IIS7))
                {
                    return AppPoolStatusIIS7(appPoolName);
                }
            }
            return 3;
        }

        /// <summary>
        ///     IIS站点控制
        /// </summary>
        /// <param name = "iisOperate">IIS操作枚举</param>
        /// <param name = "siteName">站点名称</param>
        /// <param name = "message">消息</param>
        /// <returns></returns>
        public bool SiteInvoke(EnumIISOperate iisOperate, string siteName, out string message)
        {
            if (!string.IsNullOrEmpty(siteName))
            {
                try
                {
                    WebServerTypes webServerType = GetIISServerType();
                    if (webServerType.Equals(WebServerTypes.IIS6))
                    {
                        return SiteInvokeIIS6(iisOperate, siteName, out message);
                    }
                    if (webServerType.Equals(WebServerTypes.IIS7))
                    {
                        return SiteInvokeIIS7(iisOperate, siteName, out message);
                    }

                    message = "Invalid Namespace Exception" + Environment.NewLine + Environment.NewLine +
                              "This program only works with IIS 6 and later";

                    return false;
                }
                catch
                {
                    message = "Invalid Namespace Exception" + Environment.NewLine + Environment.NewLine +
                              "This program only works with IIS 6 and later";

                    return false;
                }
            }
            message = "Input siteName is Empty or NULL,Please check";

            return false;
        }

        /// <summary>
        ///     IIS应用程序池控制
        /// </summary>
        /// <param name = "iisOperate">IIS操作枚举</param>
        /// <param name = "appPoolName">应用程序池名称</param>
        /// <param name = "message">消息</param>
        /// <returns></returns>
        public bool AppPoolInvoke(EnumIISOperate iisOperate, string appPoolName, out string message)
        {
            if (!string.IsNullOrEmpty(appPoolName))
            {
                try
                {
                    WebServerTypes webServerType = GetIISServerType();
                    if (webServerType.Equals(WebServerTypes.IIS6))
                    {
                        return AppPoolInvokeIIS6(iisOperate, appPoolName, out message);
                    }
                    if (webServerType.Equals(WebServerTypes.IIS7))
                    {
                        return AppPoolInvokeIIS7(iisOperate, appPoolName, out message);
                    }

                    message = "Invalid Namespace Exception" + Environment.NewLine + Environment.NewLine +
                              "This program only works with IIS 6 and later";

                    return false;
                }
                catch
                {
                    message = "Invalid Namespace Exception" + Environment.NewLine + Environment.NewLine +
                              "This program only works with IIS 6 and later";

                    return false;
                }
            }
            message = "Input appPoolName is Empty or NULL,Please check";

            return false;
        }

        #region 适应环境 IIS 6.0 兼容组件

        /// <summary>
        ///     获取IIS6.0 站点状态
        /// </summary>
        /// <returns> 1：运行中； 2：停止状态 ；3：未知状态；4：站点不存在</returns>
        private int SiteStatusIIS6(string siteName)
        {
            int result = 3;
            if (!string.IsNullOrEmpty(siteName))
            {
                string siteId = GetSiteIdByName(siteName);

                if (!string.IsNullOrEmpty(siteId))
                {
                    var root = new DirectoryEntry("IIS://" + ServerName + "/W3SVC/" + siteId);
                    PropertyValueCollection pvc = root.Properties["ServerState"];
                    if (pvc.Value != null)
                    {
                        result = (pvc.Value.Equals((int)EnumIISOperate.Start)
                                      ? 1
                                      : pvc.Value.Equals((int)EnumIISOperate.Stop) ? 2 : 3);
                    }
                }
                else
                {
                    result = 4;
                }
            }
            return result;
        }

        /// <summary>
        ///     获取IIS6.0 应用程序池状态
        /// </summary>
        /// <returns> 1：运行中； 2：停止状态 ；3：未知状态</returns>
        private int AppPoolStatusIIS6(string siteName)
        {
            int result = 3;
            if (!string.IsNullOrEmpty(siteName))
            {
                var root = new DirectoryEntry("IIS://" + ServerName + "/W3SVC/AppPools/" + siteName);
                PropertyValueCollection pvc = root.Properties["AppPoolState"];
                if (pvc.Value != null)
                {
                    result = (pvc.Value.Equals((int)EnumIISOperate.Start)
                                  ? 1
                                  : pvc.Value.Equals((int)EnumIISOperate.Stop) ? 2 : 3);
                }
            }
            return result;
        }

        /// <summary>
        ///     IIS6.0 站点控制
        /// </summary>
        /// <param name = "iisOperate">IIS操作枚举</param>
        /// <param name = "siteName">站点名称</param>
        /// <param name = "message">消息</param>
        /// <returns></returns>
        private bool SiteInvokeIIS6(EnumIISOperate iisOperate, string siteName, out string message)
        {
            string site = GetSiteIdByName(siteName);
            if (site == null)
            {
                message = "IIS WebsiteName '" + siteName + "' not found";
                return false;
            }

            try
            {
                var root = new DirectoryEntry("IIS://" + ServerName + "/W3SVC/" + site);
                if (iisOperate == EnumIISOperate.ReStart)
                {
                    root.Invoke(EnumIISOperate.Stop.ToString());
                    root.Invoke(EnumIISOperate.Start.ToString());
                }
                else
                {
                    root.Invoke(iisOperate.ToString());
                }
                message = "Sucess";
                return true;
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Invalid namespace"))
                {
                    message = "Invalid Namespace Exception" + Environment.NewLine + Environment.NewLine +
                              "This program only works with IIS 6 and later";
                }
                else
                {
                    message = ex.ToString();
                }
                return false;
            }
        }

        /// <summary>
        ///     IIS6.0 应用程序池控制
        /// </summary>
        /// <param name="state"></param>
        /// <param name = "appPoolName">应用程序池名称</param>
        /// <param name = "message">消息</param>
        /// <returns></returns>
        private bool AppPoolInvokeIIS6(EnumIISOperate state, string appPoolName, out string message)
        {
            if (!ExsitAppPool(appPoolName))
            {
                message = "appPool '" + appPoolName + "' not found";
                return false;
            }
            try
            {
                var root = new DirectoryEntry("IIS://" + ServerName + "/W3SVC/AppPools/" + appPoolName);
                if (state == EnumIISOperate.ReStart)
                {
                    root.Invoke(EnumIISOperate.Stop.ToString());
                    string pid = W3wp.GetAllW3wp(appPoolName);
                    if (string.IsNullOrEmpty(pid))
                    {
                        message = "没有找到该进程";
                        root.Invoke(EnumIISOperate.Start.ToString());
                        return false;
                    }
                    Process process = Process.GetProcessById(int.Parse(pid));
                    if (process != null)
                    {
                        process.Kill();
                    }
                    root.Invoke(EnumIISOperate.Start.ToString());
                }
                else
                {
                    root.Invoke(state.ToString());
                }
                message = "Sucess";
                return true;
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return false;
            }
        }

        /// <summary>
        ///     Find the siteId for a specified website name. This assumes that the website's
        ///     ServerComment property contains the website name.
        /// </summary>
        /// <param name = "siteName"></param>
        /// <returns></returns>
        private string GetSiteIdByName(string siteName)
        {
            var root = new DirectoryEntry("IIS://" + ServerName + "/W3SVC");
            foreach (DirectoryEntry e in root.Children)
            {
                if (e.SchemaClassName != "IIsWebServer") continue;

                if (e.Properties["ServerComment"].Value.ToString().Equals(siteName,
                                                                          StringComparison.OrdinalIgnoreCase))
                {
                    return e.Name;
                }
            }
            return null;
        }

        /// <summary>
        ///     if the app pool specified exsit
        /// </summary>
        /// <param name = "appPoolName">name of app pool</param>
        /// <returns>true if exsit, otherwise false</returns>
        private bool ExsitAppPool(string appPoolName)
        {
            var service = new DirectoryEntry("IIS://" + ServerName + "/W3SVC/AppPools");
            return service.Children.Cast<DirectoryEntry>().Any(entry => entry.Name.Trim().ToLower() == appPoolName.Trim().ToLower());
        }

        #endregion 适应环境 IIS 6.0 兼容组件

        #region 适应环境 IIS 7.0

        /// <summary>
        ///     IIS7.0 站点控制
        /// </summary>
        /// <param name = "iisOperate">IIS操作枚举</param>
        /// <param name = "siteName">站点名称</param>
        /// <param name = "message">消息</param>
        /// <returns></returns>
        private bool SiteInvokeIIS7(EnumIISOperate iisOperate, string siteName, out string message)
        {
            var webManager = new ServerManager();
            Site startSite = webManager.Sites[siteName];
            if (startSite == null)
            {
                message = "IIS WebsiteName '" + siteName + "' not found";
                return false;
            }
            switch (iisOperate)
            {
                case EnumIISOperate.Start:
                    {
                        message = startSite.Start().ToString();
                        return true;
                    }
                case EnumIISOperate.Stop:
                    {
                        message = startSite.Stop().ToString();
                        return true;
                    }
                case EnumIISOperate.ReStart:
                    {
                        //RestartWEbSite(siteName,out message);
                        //FixWebsite(siteName);
                        message = startSite.Stop().ToString();
                        message += "" + startSite.Start().ToString();
                        return true;
                    }
                default:
                    {
                        message = "Invalid Operate,Please Check";
                        return false;
                    }
            }
        }

        /// <summary>
        ///     IIS7.0 应用程序池控制
        /// </summary>
        /// <param name = "iisOperate">IIS操作枚举</param>
        /// <param name = "appPoolName">应用程序池名称</param>
        /// <param name = "message">消息</param>
        /// <returns></returns>
        private bool AppPoolInvokeIIS7(EnumIISOperate iisOperate, string appPoolName, out string message)
        {
            var webManager = new ServerManager();
            ApplicationPool appPool = webManager.ApplicationPools[appPoolName];
            if (appPool == null)
            {
                message = "IIS AppPoolName '" + appPoolName + "' not found";
                return false;
            }
            switch (iisOperate)
            {
                case EnumIISOperate.Start:
                    {
                        message = appPool.Start().ToString();
                        return true;
                    }
                case EnumIISOperate.Stop:
                    {
                        message = appPool.Stop().ToString();
                        return true;
                    }
                case EnumIISOperate.ReStart:
                    {
                        message = appPool.Stop().ToString();
                        string pid = W3wp.GetAllW3wp(appPoolName);
                        if (string.IsNullOrEmpty(pid))
                        {
                            message = "没有找到该进程";
                            appPool.Start();
                            return false;
                        }
                        Process process = Process.GetProcessById(int.Parse(pid));
                        if (process != null)
                        {
                            process.Kill();
                        }
                        message += " " + appPool.Start().ToString();
                        //message = "";
                        //RestartIISPool(appPoolName);
                        return true;
                    }
                default:
                    {
                        message = "Invalid Operate,Please Check";
                        return false;
                    }
            }
        }

        /// <summary>
        ///     获取IIS7.0 站点状态
        /// </summary>
        /// <returns> 1：运行中； 2：停止状态 ；3：未知状态；4：站点不存在</returns>
        private int SiteStatusIIS7(string siteName)
        {
            int result = 3;
            if (!string.IsNullOrEmpty(siteName))
            {
                var webManager = new ServerManager();
                Site startSite = webManager.Sites[siteName];

                if (startSite != null)
                {
                    if (startSite.State.ToString().Equals("Started"))
                    {
                        result = 1;
                        return result;
                    }
                    if (startSite.State.ToString().Equals("Stopped"))
                    {
                        result = 2;
                        return result;
                    }
                }
                else
                {
                    result = 4;
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        ///     获取IIS7.0 应用程序池状态
        /// </summary>
        /// <returns> 1：运行中； 2：停止状态 ；3：未知状态</returns>
        private int AppPoolStatusIIS7(string appPoolName)
        {
            int result = 3;
            if (!string.IsNullOrEmpty(appPoolName))
            {
                var webManager = new ServerManager();
                ApplicationPool startSite = webManager.ApplicationPools[appPoolName];

                if (startSite != null)
                {
                    if (startSite.State.ToString().Equals("Started"))
                    {
                        result = 1;
                        return result;
                    }
                    if (startSite.State.ToString().Equals("Stopped"))
                    {
                        result = 2;
                        return result;
                    }
                }
            }
            return result;
        }

        #endregion 适应环境 IIS 7.0

        #region 服务器IIS版本

        /// <summary>
        ///     服务器IIS版本
        /// </summary>
        public enum WebServerTypes
        {
            /// <summary>
            ///     未知版本
            /// </summary>
            Unknown,

            /// <summary>
            ///     IIS 6.0
            /// </summary>
            IIS6,

            /// <summary>
            ///     IIS 7.0
            /// </summary>
            IIS7
        }

        /// <summary>
        ///     获取服务器IIS版本
        /// </summary>
        /// <returns></returns>
        public WebServerTypes GetIISServerType()
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey("software\\microsoft\\inetstp");
                int num = 0;
                if (key != null)
                {
                    num = Convert.ToInt32(key.GetValue("majorversion", -1));
                }
                switch (num)
                {
                    case 6:
                        return WebServerTypes.IIS6;
                    case 7:
                        return WebServerTypes.IIS7;
                    default:
                        return WebServerTypes.Unknown;
                }
            }
            catch
            {
                return WebServerTypes.Unknown;
            }
        }

        #endregion 服务器IIS版本

        #region 新的重启站点解决办法

        /// <summary>
        /// 根据名字重启站点.(没重启线程池)
        /// </summary>
        /// <param name="sitename"></param>

        private static void RestartWEbSite(string sitename, out string msg)
        {
            msg = "重启站点";
            try
            {
                var server = new ServerManager();
                var site = server.Sites.FirstOrDefault(s => s.Name == sitename);
                if (site != null)
                {
                    site.Stop();
                    if (site.State == ObjectState.Stopped)
                    {
                    }
                    else
                    {
                        msg = "Could not stop website!";
                        LoggerWrapper.Logger.Info("VWS.ClientHost", "站点停止失败");
                        throw new InvalidOperationException("Could not stop website!");
                    }
                    site.Start();
                }
                else
                {
                    msg = "Could not find website!";
                    LoggerWrapper.Logger.Info("VWS.ClientHost", "不存在该站点");
                    throw new InvalidOperationException("Could not find website!");
                }
            }
            catch (Exception e)
            {
                LoggerWrapper.Logger.Info("VWS.ClientHost 站点停止失败", e);
                msg += e.Message;
            }
        }

        /// <summary>
        /// 重启完之后.要再检测下.是否开启了
        /// </summary>
        /// <param name="sitename"></param>
        private static void FixWebsite(string sitename)
        {
            try
            {
                var server = new ServerManager();
                var site = server.Sites.FirstOrDefault(s => s.Name == sitename);
                if (site != null)
                {
                    if (site.State != ObjectState.Started)
                    {
                        Thread.Sleep(500);
                        //防止状态为正在开启
                        if (site.State != ObjectState.Started)
                        {
                            site.Start();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //
        /// <summary>
        /// 重启iis线程池:
        /// </summary>
        /// <param name="name"></param>
        private static void RestartIISPool(string name)
        {
            string[] cmds = { "c:", @"cd %windir%\system32\inetsrv", string.Format("appcmd stop apppool /apppool.name:{0}", name), string.Format("appcmd start apppool /apppool.name:{0}", name) };
            Cmd(cmds);
            CloseProcess("cmd.exe");
        }

        /// <summary>
        /// 运行CMD命令
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <returns></returns>
        public static string Cmd(string[] cmd)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.AutoFlush = true;
            for (int i = 0; i < cmd.Length; i++)
            {
                p.StandardInput.WriteLine(cmd[i]);
            }
            p.StandardInput.WriteLine("exit");
            string strRst = p.StandardOutput.ReadToEnd();

            //Debug.Print(strRst);
            p.WaitForExit();
            p.Close();
            return strRst;
        }

        /// <summary>
        /// 关闭进程
        /// </summary>
        /// <param name="ProcName">进程名称</param>
        /// <returns></returns>
        public static bool CloseProcess(string ProcName)
        {
            bool result = false;
            var procList = new ArrayList();
            foreach (Process thisProc in Process.GetProcesses())
            {
                var tempName = thisProc.ToString();

                int begpos = tempName.IndexOf("(") + 1;
                int endpos = tempName.IndexOf(")");
                tempName = tempName.Substring(begpos, endpos - begpos);
                procList.Add(tempName);
                if (tempName == ProcName)
                {
                    if (!thisProc.CloseMainWindow())
                        thisProc.Kill(); // 当发送关闭窗口命令无效时强行结束进程
                    result = true;
                }
            }
            return result;
        }

        #endregion 新的重启站点解决办法
    }
}