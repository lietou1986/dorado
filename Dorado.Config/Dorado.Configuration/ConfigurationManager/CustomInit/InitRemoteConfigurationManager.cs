using System;
using System.Threading;

namespace Dorado.Configuration.ConfigurationManager.CustomInit
{
    public class InitRemoteConfigurationManager
    {
        private static readonly ManualResetEvent InitSingle;

        internal static object SyncRoot
        {
            get;
            private set;
        }

        static InitRemoteConfigurationManager()
        {
            SyncRoot = new object();
            InitRemoteConfiguration.ConfigChanged += new EventHandler(OnConfigChanged);
            InitSingle = new ManualResetEvent(false);
            InitConfig();
        }

        private static void OnConfigChanged(object sender, EventArgs args)
        {
            InitConfig();
        }

        private static InitRemoteConfiguration _config;

        public static InitRemoteConfiguration Config
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
        private static void InitConfig()
        {
            lock (SyncRoot)
            {
                InitSingle.Reset();
                try
                {
                    Config = InitRemoteConfiguration.Instance;
                }
                finally
                {
                    InitSingle.Set();
                }
            }
        }
    }
}