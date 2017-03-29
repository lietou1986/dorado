using System;
using System.Threading;

namespace Dorado.Configuration
{
    public class RemoteConfigurationWrapper<T, M>
        where T : new()
        where M : new()
    {
        private static ManualResetEvent _initSingle;
        private static object _syncRoot;
        private static M _configManager = new M();
        private static T _config;

        public static event EventHandler ConfigChangedEvent;

        protected RemoteConfigurationWrapper()
        {
            _syncRoot = new object();
            BaseConfig<T>.ConfigChanged += new EventHandler(OnConfigChanged);
            _initSingle = new ManualResetEvent(false);
            InitConfig();
        }

        private void OnConfigChanged(object sender, EventArgs args)
        {
            InitConfig();
            if (ConfigChangedEvent != null)
                ConfigChangedEvent(Config, EventArgs.Empty);
        }

        public static T Config
        {
            get
            {
                return _config;
            }
            set
            {
                _config = value;
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
                    Config = BaseConfig<T>.Instance;
                    Validation();
                }
                finally
                {
                    _initSingle.Set();
                }
            }
        }

        protected virtual void Validation()
        {
        }
    }
}