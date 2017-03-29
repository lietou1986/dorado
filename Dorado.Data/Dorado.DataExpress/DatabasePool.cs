using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.DataExpress.Configuration;
using Dorado.DataExpress.Dialect;
using Dorado.DataExpress.Driver;
using Dorado.DataExpress.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading;

namespace Dorado.DataExpress
{
    public sealed class DatabasePool : BasePool<Database>
    {
        public const string DefaultDialectNamespace = "Dorado.DataExpress.Dialect.{0}";
        public const string DefaultDeriverNamespace = "Dorado.DataExpress.Driver.{0}";
        private bool _logging = true;
        private int _gcCollectCounter;
        private Timer _gcCollectTmr;
        private string _connectionString = "";

        public int GcCounterThreshold
        {
            get;
            private set;
        }

        public long GcMemoryThreshold
        {
            get;
            private set;
        }

        public bool IsDefault
        {
            get;
            set;
        }

        public bool Logging
        {
            get
            {
                return this._logging;
            }
            set
            {
                this._logging = value;
            }
        }

        public BaseDriver Driver
        {
            get;
            set;
        }

        public BaseDialect Dialect
        {
            get;
            set;
        }

        public string ConnectionString
        {
            get
            {
                return this._connectionString;
            }
            protected set
            {
                this._connectionString = value;
            }
        }

        public string[] Alias
        {
            get;
            set;
        }

        public string Name
        {
            get;
            protected set;
        }

        public Guid InstanceId
        {
            get;
            set;
        }

        public Dictionary<string, Regex> Filters
        {
            get;
            set;
        }

        public bool EnableFilters
        {
            get;
            set;
        }

        public bool UseNativePool
        {
            get;
            set;
        }

        public DatabasePool(PoolConfigurationElement cfg)
            : this()
        {
            this.InstanceId = Guid.NewGuid();
            this.Name = cfg.Name;
            this._logging = cfg.Logging;
            this.Alias = cfg.Alias.AllKeys;
            this.UseNativePool = cfg.UseNativePool;
            this._maxActive = cfg.Initiator.MaxActive;
            this._initSize = cfg.Initiator.InitialSize;
            this._maxWait = cfg.Initiator.MaxWait;
            this._maxTimeout = cfg.Initiator.MaxTimeOut;
            base.TimeoutCheck = cfg.Initiator.TimeOutCheck;
            this.IsDefault = cfg.Default;
            this._maxIdle = cfg.Idle.MaxIdle;
            base.IdleCheck = cfg.Idle.Check;
            base.IdleCheckInterval = cfg.Idle.Interval;
            this.Driver = DatabasePool.GetDriver(cfg.Features.Driver);
            this.Dialect = DatabasePool.GetDialect(cfg.Features.Dialect);
            this.ConnectionString = cfg.Connection.ToString();
            if (cfg.Filters.Count > 0)
            {
                this.EnableFilters = true;
                foreach (NameValueConfigurationElement filter in cfg.Filters)
                {
                    this.Filters.Add(filter.Name, new Regex(filter.Value, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline));
                }
            }
            if (!this.UseNativePool)
            {
                this.Init();
            }
        }

        public DatabasePool()
        {
            this.UseNativePool = false;
            this.Alias = null;
            this.Driver = null;
            this.Dialect = null;
            this.Filters = new Dictionary<string, Regex>();
            this._gcCollectCounter = 0;
            this.GcCounterThreshold = 512;
            this.GcMemoryThreshold = 268435455L;
        }

        private static BaseDialect GetDialect(string dialect)
        {
            string dialectTypeName = dialect;
            if (!dialectTypeName.Contains("."))
            {
                dialectTypeName = string.Format("Dorado.DataExpress.Dialect.{0}", dialectTypeName);
            }
            Type type = Type.GetType(dialectTypeName, true, true);
            return (BaseDialect)Activator.CreateInstance(type);
        }

        private static BaseDriver GetDriver(string driver)
        {
            string driverTypeName = driver;
            if (!driverTypeName.Contains("."))
            {
                driverTypeName = string.Format("Dorado.DataExpress.Driver.{0}", driverTypeName);
            }
            else
            {
                if (driverTypeName.Contains("{") && driverTypeName.Contains("}"))
                {
                    GeneralReflectionDriverConfig gconf = JsonReader.Read<GeneralReflectionDriverConfig>(driverTypeName);
                    return new GeneralReflectionDriver
                    {
                        CommandProviderType = gconf.CommandProviderType,
                        ConnectionProviderType = gconf.ConnectionProviderType,
                        DataAdapterProviderType = gconf.DataAdapterProviderType
                    };
                }
            }
            Type type = Type.GetType(driverTypeName, true, true);
            return (BaseDriver)Activator.CreateInstance(type);
        }

        public override Database CreateInstance()
        {
            Database db = base.CreateInstance();
            db.Driver = this.Driver;
            db.Dialect = this.Dialect;
            db.ConnectionString = this.ConnectionString;
            db.Pool = this;
            db.InitConnection();
            return db;
        }

        public override bool NeedEvict(Database obj)
        {
            if (this.UseNativePool)
            {
                return false;
            }
            if ((DateTime.Now - obj.LastActive).TotalMilliseconds > (double)base.MaxIdle)
            {
                LoggerWrapper.Logger.Debug(string.Format("数据库[ID:{0},来自{1}]闲置时间过长,将自动销毁.", new object[]
				{
					obj.GetHashCode(),
					this.Name
				}));
                this._gcCollectCounter += 2;
                return true;
            }
            return false;
        }

        public override bool NeedRecycle(Database obj)
        {
            if ((DateTime.Now - obj.LastActive).TotalMilliseconds > (double)this.MaxTimeout)
            {
                LoggerWrapper.Logger.Error(string.Format("将数据库[ID:{0}]使用超过时间限制,将自动归还到连接池[{1}],Host:[{2}]", new object[]
				{
					obj.GetHashCode(),
					this.Name,
					obj.HostSource
				}));
                obj.Clear();
                this._gcCollectCounter += 4;
                return true;
            }
            return false;
        }

        public override void DestroyInstance(Database obj)
        {
            try
            {
                obj.ReallyClose();
            }
            catch (Exception error)
            {
                LoggerWrapper.Logger.Error(string.Format("数据库[ID:{0}]销毁时发生错误(连接池[{1}])", obj.GetHashCode(), this.Name), error);
            }
        }

        public Database Obtain(string sourceName)
        {
            Database db = this.Obtain();
            db.HostSource = sourceName;
            if (this.Logging)
            {
                LoggerWrapper.Logger.Debug(string.Format("从连接池[{0}]取出数据库连接[ID:{1}],Host:[{2}]", new object[]
				{
					this.Name,
					db.GetHashCode(),
					sourceName
				}));
            }
            return db;
        }

        public override Database Obtain()
        {
            if (this.UseNativePool)
            {
                if (this.Logging)
                {
                    LoggerWrapper.Logger.Debug(string.Format("从连接池[{0}]取出数据库连接(使用原生连接池)", new object[]
					{
						this.Name
					}));
                }
                return this.CreateInstance();
            }
            Database db = base.Obtain();
            db.Closed = false;
            db.UpdateIdle();
            db.UpdateHostSource();
            return db;
        }

        public override void Init()
        {
            base.Init();
            this._gcCollectTmr = new Timer(new TimerCallback(this.GcMonitor), this, this._maxIdle, this._maxIdle);
        }

        private void GcMonitor(object state)
        {
            object syncRoot;
            Monitor.Enter(syncRoot = this.SyncRoot);
            try
            {
                if (this._gcCollectCounter > this.GcCounterThreshold || GC.GetTotalMemory(true) > this.GcMemoryThreshold)
                {
                    GC.Collect();
                    this._gcCollectCounter = 0;
                }
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
        }

        public override void Return(Database obj)
        {
            if (this.UseNativePool)
            {
                return;
            }
            if (this.Logging)
            {
                LoggerWrapper.Logger.Debug(string.Format("将数据库[ID:{0}]归还到连接池[{1}],Host:[{2}]", new object[]
				{
					obj.GetHashCode(),
					this.Name,
					obj.HostSource
				}));
            }
            obj.Closed = true;
            obj.UpdateIdle();
            base.Return(obj);
            this._gcCollectCounter++;
        }

        public override void Close()
        {
            if (this.Logging)
            {
                LoggerWrapper.Logger.Debug(string.Format("开始关闭连接池[{0}]",
                    this.Name));
            }
            base.Close();
        }
    }
}