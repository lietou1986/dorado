using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.Xml.Serialization;

namespace Dorado.Configuration.ConfigurationManager
{
    /// <summary>
    /// 配置文件管理类
    /// </summary>
    public class ConfigManager
    {
        private static string GetConfigSectionName<T>()
        {
            Type t = typeof(T);
            object[] attrs = t.GetCustomAttributes(typeof(RemoteConfigKeyAttribute), false);
            if (attrs.Length > 0)
            {
                return ((RemoteConfigKeyAttribute)attrs[0]).ConfigKey;
            }
            else
            {
                attrs = t.GetCustomAttributes(typeof(XmlRootAttribute), false);
                if (attrs.Length > 0)
                {
                    return ((XmlRootAttribute)attrs[0]).ElementName;
                }
            }
            return t.Name;
        }

        public static T GetSection<T>()
        {
            string name = GetConfigSectionName<T>();
            return GetSection<T>(name);
        }

        public static T GetSection<T>(string name)
        {
            bool fromRemote;
            return GetSection<T>(name, out fromRemote);
        }

        /// <summary>
        /// 获取配置实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="fromRemote">if set to <c>true</c> [from remote].</param>
        /// <returns></returns>
        public static T GetSection<T>(string name, out bool fromRemote)
        {
            fromRemote = false;
            T obj = LocalConfigurationManager.Instance.GetSection<T>(name);//从本地取得配置对象
            if (obj != null)
                return obj;
            else//如果本地获取配置实例为null则从远程配置获取
            {
                LoggerWrapper.Logger.Info(string.Format("无法从本地配置文件取得配置实例 '{0}' ,正在从远程配置服务中加载...", name));
                fromRemote = true;
                return RemoteConfigurationManager.Instance.GetSection<T>(name);//从远程配置服务器获取配置对象
            }
        }

        #region 暂时没有用到

        public static T GetSection<T>(string name, string path)
        {
            bool fromRemote;
            return GetSection<T>(name, path, out fromRemote);
        }

        public static T GetSection<T>(string name, string path, out bool fromRemote)
        {
            if (System.IO.File.Exists(path))
            {
                fromRemote = false;
                return ConfigWatcher.CreateAndSetupWatcher<T>(LocalConfigurationManager.MapConfigPath(path));
            }
            else
            {
                fromRemote = true;
                return RemoteConfigurationManager.Instance.GetSection<T>(name);
            }
        }

        #endregion 暂时没有用到

        public static AppSettingCollection AppSettings
        {
            get
            {
                return AppSettingCollection.Instance;
            }
        }

        public static void RegisterAppSettingsConfigChangedNotification(EventHandler handler)
        {
            AppSettingCollection.RegisterConfigChangedNotification(handler);
        }

        public static string GetConnectionString(string name)
        {
            return ConnectionStringCollection.Instance[name];
        }
    }
}