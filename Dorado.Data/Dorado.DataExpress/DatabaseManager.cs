using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.DataExpress.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;

namespace Dorado.DataExpress
{
    internal class DatabaseManager
    {
        private static readonly object SyncRoot;
        private static readonly ConfigurationWatcher ConfigWatcher;
        private static string _watchConfigFile;

        public static Dictionary<string, DatabasePool> Pools
        {
            get;
            set;
        }

        public static DatabasePool DefaultPool
        {
            get;
            set;
        }

        public static Database Current
        {
            get
            {
                return DatabaseManager.GetDatabase("CURRENT");
            }
        }

        public static Database CurrentSession
        {
            get
            {
                return DbSession.Current;
            }
        }

        static DatabaseManager()
        {
            DatabaseManager.SyncRoot = new object();
            DatabaseManager._watchConfigFile = string.Empty;
            DatabaseManager.Pools = new Dictionary<string, DatabasePool>(StringComparer.OrdinalIgnoreCase);
            DatabaseManager.Init();
            DatabaseManager.ConfigWatcher = new ConfigurationWatcher(AppDomain.CurrentDomain.BaseDirectory, DatabaseManager._watchConfigFile);
            DatabaseManager.ConfigWatcher.OnFilesChanged += new ConfigurationWatcher.FilesChangedEventHandler(DatabaseManager.ConfigurationChanges);
            DatabaseManager.ConfigWatcher.Start();
            AppDomain.CurrentDomain.DomainUnload += new EventHandler(DatabaseManager.CurrentDomainDomainUnload);
        }

        private static void ConfigurationChanges(object sender, List<FileSystemEventArgs> events)
        {
            if (string.IsNullOrEmpty(DatabaseManager._watchConfigFile))
            {
                return;
            }
            bool canRestart = false;
            if (string.Compare(DatabaseManager._watchConfigFile, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile) == 0)
            {
                canRestart = true;
            }
            else
            {
                IEnumerable<FileSystemEventArgs> files =
                    from file in events
                    where file.Name.IndexOf(DatabaseManager._watchConfigFile, StringComparison.OrdinalIgnoreCase) > -1
                    select file;
                canRestart = (files.Count<FileSystemEventArgs>() > 0);
            }
            if (canRestart)
            {
                DatabaseManager.Shutdown();
                DatabaseManager.Init();
            }
        }

        private static void CurrentDomainDomainUnload(object sender, EventArgs e)
        {
            DatabaseManager.Shutdown();
            DatabaseManager.ConfigWatcher.Dispose();
        }

        internal static void Shutdown()
        {
            object syncRoot;
            Monitor.Enter(syncRoot = DatabaseManager.SyncRoot);
            try
            {
                foreach (KeyValuePair<string, DatabasePool> pool in DatabaseManager.Pools)
                {
                    try
                    {
                        pool.Value.Dispose();
                        LoggerWrapper.Logger.Debug(string.Format("关闭数据库[{0}]。", new object[]
						{
							pool.Key
						}));
                    }
                    catch (Exception error)
                    {
                        try
                        {
                            LoggerWrapper.Logger.Error(string.Format("关闭数据库连接池发生异常。", error));
                        }
                        catch
                        {
                        }
                    }
                }
                DatabaseManager.Pools.Clear();
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
        }

        private static void Init()
        {
            object syncRoot;
            Monitor.Enter(syncRoot = DatabaseManager.SyncRoot);
            try
            {
                PoolConfigurationSection section = ConfigurationManager.GetSection("dataExpress") as PoolConfigurationSection;
                if (section != null)
                {
                    DatabaseManager._watchConfigFile = section.SectionInformation.ConfigSource;
                    if (string.IsNullOrEmpty(DatabaseManager._watchConfigFile))
                    {
                        DatabaseManager._watchConfigFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                    }
                    foreach (PoolConfigurationElement poolCfg in section.PoolCollection)
                    {
                        try
                        {
                            DatabasePool pool = new DatabasePool(poolCfg);
                            DatabaseManager.RegisterPool(pool);
                        }
                        catch (Exception error)
                        {
                            LoggerWrapper.Logger.Error(string.Format("初始化数据库连接池失败[{0}]", poolCfg.Name), error);
                        }
                    }
                }
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
        }

        private static void RegisterPool(DatabasePool pool)
        {
            if (DatabaseManager.Pools.ContainsKey(pool.Name))
            {
                LoggerWrapper.Logger.Error(string.Format("已存在名为{0}的连接池，请检查配置文件。", new object[]
				{
					pool.Name
				}));
                return;
            }
            DatabaseManager.Pools.Add(pool.Name, pool);
            if (pool.IsDefault)
            {
                if (DatabaseManager.DefaultPool != null)
                {
                    LoggerWrapper.Logger.Info(string.Format("{0}连接池名重复，已使用最后一个名为{0}的连接池作为默认值。", new object[]
					{
						pool.Name
					}));
                }
                DatabaseManager.DefaultPool = pool;
            }
            if (pool.Alias != null)
            {
                string[] alias2 = pool.Alias;
                for (int i = 0; i < alias2.Length; i++)
                {
                    string alias = alias2[i];
                    if (DatabaseManager.Pools.ContainsKey(alias))
                    {
                        LoggerWrapper.Logger.Info(string.Format("已存在名称或别名为[{0}]的数据库。", new object[]
						{
							alias
						}));
                    }
                    else
                    {
                        DatabaseManager.Pools.Add(alias, pool);
                    }
                }
            }
        }

        public static Database GetDatabase(string hostName)
        {
            if (DatabaseManager.DefaultPool != null)
            {
                return DatabaseManager.DefaultPool.Obtain(hostName);
            }
            throw new Exception("未能找到默认数据连接池.");
        }

        public static Database GetDatabase(string poolName, string sourceName)
        {
            if (DatabaseManager.Pools.ContainsKey(poolName))
            {
                return DatabaseManager.Pools[poolName].Obtain();
            }
            throw new Exception(string.Format("未能找到名为[{0}]的数据库连接池.", poolName));
        }
    }
}