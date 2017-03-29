using Dorado.Configuration.ConfigurationManager.CustomInit;
using Dorado.Configuration.Exceptions;
using Dorado.Core;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

namespace Dorado.Configuration
{
    public class LocalConfigurationWrapper<T, M>
        where T : new()
        where M : new()
    {
        private static string _configName;
        private static string _configFile;
        private static ManualResetEvent _initSingle;
        private static object _syncRoot;
        private static M _configManager = new M();
        private static T _config;

        public static event EventHandler ConfigChangedEvent;

        protected LocalConfigurationWrapper(string configName, string configFile = null)
        {
            _configName = configName;
            _configFile = configFile;

            if (string.IsNullOrEmpty(_configFile))
                _configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _configName + ".config");

            if (!File.Exists(_configFile))
                throw new NotFoundConfigurationException(_configName);

            _syncRoot = new object();
            FileWatcher.Instance.AddFile(_configFile, OnConfigChanged);
            _initSingle = new ManualResetEvent(false);
            InitConfig();
        }

        protected LocalConfigurationWrapper(string configName, InitConfiguration initConfig)
        {
            Config config = initConfig.ConfigSettings.Configs.FirstOrDefault(n => n.Name.Equals(configName, StringComparison.CurrentCultureIgnoreCase));
            if (config != null)
                _configFile = config.IsRelativePath ? Path.Combine(initConfig.ConfigSettings.BasePath, config.Path) : config.Path;

            if (string.IsNullOrEmpty(_configFile))
                _configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _configName + ".config");

            if (!File.Exists(_configFile))
                throw new NotFoundConfigurationException(_configName);

            _syncRoot = new object();
            FileWatcher.Instance.AddFile(_configFile, OnConfigChanged);
            _initSingle = new ManualResetEvent(false);
            InitConfig();
        }

        public static T Config
        {
            get
            {
                return _config;
            }
        }

        private void OnConfigChanged(object sender, EventArgs args)
        {
            InitConfig();
            if (ConfigChangedEvent != null)
                ConfigChangedEvent(Config, EventArgs.Empty);
        }

        /// <summary>
        /// 提交配置更改
        /// </summary>
        public static void SubmitChanges()
        {
            lock (_syncRoot)
            {
                _initSingle.Reset();
                try
                {
                    Save();
                }
                finally
                {
                    _initSingle.Set();
                }
            }
        }

        private static void Save()
        {
            using (FileStream fs = new FileStream(_configFile, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer ser = new XmlSerializer(typeof(T), new Type[]　{　
　　　　　　　
　　　　　　});
                ser.Serialize(fs, _config);
            }
        }

        /// <summary>
        /// 初始化配置文件
        /// </summary>
        private void InitConfig()
        {
            lock (_syncRoot)
            {
                _initSingle.Reset();
                try
                {
                    Load();
                    Validation();
                }
                finally
                {
                    _initSingle.Set();
                }
            }
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        private static void Load()
        {
            if (!File.Exists(_configFile))
            {
                _config = new T();
                return;
            }
            using (FileStream fs = new FileStream(_configFile, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer ser = new XmlSerializer(typeof(T), new Type[]　{　
　　　　　　　　
　　　　　　});
                _config = (T)ser.Deserialize(fs);
            }
        }

        protected virtual void Validation()
        {
        }

        /// <summary>
        /// 取得配置文件的路径
        /// </summary>
        public static string ConfigFile
        {
            get { return _configFile; }
        }
    }
}