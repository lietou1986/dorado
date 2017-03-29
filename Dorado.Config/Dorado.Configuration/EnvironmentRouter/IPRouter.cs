using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.Net;

namespace Dorado.Configuration.EnvironmentRouter
{
    /// <summary>
    /// 根据IP段获取远程配置环境（开发or测试...）
    /// </summary>
    public class IPRouter : IEnvironmentRouter
    {
        #region Singleton

        private static readonly IPRouter instance = new IPRouter();

        private IPRouter()
        {
        }

        static IPRouter()
        {
        }

        public static IPRouter Instance
        {
            get { return instance; }
        }

        #endregion Singleton

        #region EnvironmentRouter 成员

        public NetworkEnvironment GetCurrentEnvironment()
        {
            string strEnv = System.Web.Configuration.WebConfigurationManager.AppSettings["Evn"];//判断程序是否设置了环境标量
            if (!string.IsNullOrEmpty(strEnv))
            {
                try
                {
                    NetworkEnvironment env = (NetworkEnvironment)Enum.Parse(typeof(NetworkEnvironment), strEnv);
                    return env;
                }
                catch
                {
                }
            }

            try
            {
                IPHostEntry iph = Dns.GetHostEntry(Dns.GetHostName());
                if (iph.AddressList != null)
                {
                    foreach (IPAddress address in iph.AddressList)
                    {
                        byte[] bytes = address.GetAddressBytes();
                        if (bytes[0] == 10 && bytes[1] == 22)//如果配置文件没有设置环境标量则根据IP段确认开发环境
                            return NetworkEnvironment.Prod;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("ConfigUtility", ex);
            }

            return NetworkEnvironment.Dev;
        }

        #endregion EnvironmentRouter 成员

        public static IPAddress LocalIPAdddress
        {
            get
            {
                string hostName = Dns.GetHostName();
                string ip = Dns.GetHostEntry(hostName).AddressList[0].ToString();

                return IPAddress.Parse(ip);
            }
        }
    }
}