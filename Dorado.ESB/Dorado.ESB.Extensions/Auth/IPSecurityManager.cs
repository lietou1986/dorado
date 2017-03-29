using System;
using System.Collections.Generic;

namespace Dorado.ESB.Extensions.Auth
{
    public class IPSecurityManager
    {
        private Dictionary<string, List<string>> methodAllowColl;

        private static IPSecurityManager instance = new IPSecurityManager();

        public static IPSecurityManager Instance
        {
            get
            {
                return instance;
            }
        }

        //读取配置文件
        private IPSecurityManager()
        {
            OnConfigReload(IPSecurityConfig.Instance, EventArgs.Empty);
        }

        /// <summary>
        /// 根据ip与方法名的对应，是否允许存取
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public bool AllowAccess(string IP, string methodName)
        {
            if (IP == null || methodName == null)
                return false;

            List<string> allowMethod = null;
            if (methodAllowColl.TryGetValue(IP, out allowMethod))
            {
                if (allowMethod.Contains(methodName.ToLower()))
                    return true;
                else
                    return false;
            }
            else
            {
                return true;
            }
        }

        private EventHandler configChangedEvent;

        internal void RegisterConfigChanged(EventHandler handler)
        {
            configChangedEvent = handler;
        }

        /// <summary>
        /// load配置事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        internal void OnConfigReload(object sender, EventArgs args)
        {
            IPSecurityConfig config = (IPSecurityConfig)sender;

            Dictionary<string, List<string>> oldMethodAllowColl = methodAllowColl;
            Dictionary<string, List<string>> newMethodAllowColl = new Dictionary<string, List<string>>();

            foreach (var ipSetConfig in config.IPSets)
            {
                string[] methodList = ipSetConfig.AllowMethodList.Split(';');
                if (methodList != null && methodList.Length > 0)
                {
                    List<string> allowMethods = new List<string>(methodList.Length);
                    foreach (string method in methodList)
                    {
                        if (!allowMethods.Contains(method.ToLower().Trim()))
                            allowMethods.Add(method.ToLower().Trim());
                    }

                    string[] mathIPs = ipSetConfig.Math.Split(';');
                    if (mathIPs != null && mathIPs.Length > 0)
                    {
                        foreach (string ip in mathIPs)
                        {
                            if (!newMethodAllowColl.ContainsKey(ip))
                                newMethodAllowColl.Add(ip, allowMethods);
                        }
                    }
                }
            }

            methodAllowColl = newMethodAllowColl;

            if (configChangedEvent != null)
                configChangedEvent(sender, args);
        }
    }
}