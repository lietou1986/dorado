using System;

namespace Dorado.Configuration
{
    /// <summary>
    /// 配置文件泛型基类（所有需要配置为远程的配置文件都需要继承此类）
    /// </summary>
    /// <typeparam name="T">配置文件类型</typeparam>
    public class BaseConfig<T> where T : new()
    {
        //取得配置实例对象
        private static T instance = ConfigurationManager.ConfigManager.GetSection<T>();

        public static T Instance
        {
            get { return instance; }
            set
            {
                instance = value;
                OnConfigChanged();
            }
        }

        public static T GetConfig(EventHandler reloadEventHandler)
        {
            ConfigChanged += reloadEventHandler;
            return Instance;
        }

        //配置文件被更新时执行（用来重新初始化配置实例）
        public static event EventHandler ConfigChanged;

        public static void OnConfigChanged()
        {
            if (ConfigChanged != null)
                ConfigChanged(Instance, EventArgs.Empty);
        }
    }
}