using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.ESB.Common.Config;
using Dorado.ESB.Core.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Wintellect.PowerCollections;
using Wintellect.Threading.AsyncProgModel;
using Wintellect.Threading.ResourceLocks;

namespace Dorado.ESB.Core
{
    #region HostServices

    /// <summary>
    /// 当Assembly文件变化时的代理事件
    /// </summary>
    /// <param name="appDomainName"></param>
    internal delegate void LoadedAssemblyChangeDelegate(string appDomainName);

    public sealed class HostServices : IDisposable
    {
        #region Private Members

        /// <summary>
        /// 定义支持单个写线程和多个读线程_rwl锁。
        /// </summary>
        private ReaderWriterLock _rwl = new ReaderWriterLock();

        /// <summary>
        /// host名称
        /// </summary>
        private string _hostName = string.Empty;

        /// <summary>
        /// 应用程序域列表
        /// </summary>
        private List<AppDomain> _appDomains = new List<AppDomain>();

        //*
        private static string _key = typeof(ServiceHostActivator).FullName;

        private bool _selfHosted = false;
        private HostMetadata _hostMetadata;

        //List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

        /// <summary>
        /// 定义一个词典，侦听文件系统更改通知，并在目录或目录中的文件发生更改时引发事件。同时侦听多个文件目录等
        /// </summary>
        private Dictionary<string, FileSystemWatcher> watchers = new Dictionary<string, FileSystemWatcher>();

        /// <summary>
        /// Wintellect资源锁，一个多个资源锁
        /// </summary>
        private ResourceLock resourceLock = new OneManyResourceLock();

        /// <summary>
        /// Assembly重新调入的分钟集合
        /// </summary>
        private Set<string> pendingAssemblyReloadMinute = new Set<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Assembly files集合
        /// </summary>
        private Set<string> pendingAssemblyFileNames = new Set<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Assembly文件改变代理事件
        /// </summary>
        private LoadedAssemblyChangeDelegate nodeChangedDelegate = null;

        //public ConfigChangeDelegate configChangeDelegate = null;

        #endregion Private Members

        #region Constructors

        public HostServices()
            : this(false)
        {
            nodeChangedDelegate = new LoadedAssemblyChangeDelegate(ReloadAssembly);
        }

        public HostServices(bool selfHosted)
        {
            _selfHosted = selfHosted;
            nodeChangedDelegate = new LoadedAssemblyChangeDelegate(ReloadAssembly);
        }

        #endregion Constructors

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                //获得写锁
                _rwl.AcquireWriterLock(TimeSpan.FromSeconds(60));
                _appDomains.Clear();
            }
            finally
            {
                //释放写锁
                _rwl.ReleaseWriterLock();
            }
        }

        #endregion IDisposable Members

        #region HostName

        public string HostName
        {
            get { return _hostName; }
        }

        #endregion HostName

        #region CreateCatalog

        /// <summary>
        /// 建立目录
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="inpCatalog"></param>
        /// <returns></returns>
        public HostMetadata CreateCatalog(string serviceName, HostMetadata inpCatalog)
        {
            //LocalRepositorySection localRepository = (LocalRepositorySection)ConfigurationManager.GetSection("LocalRepository");

            //从资源文件中读取信息，存入LocalRepositorySection对象
            LocalRepositorySection localRepository = ConfigHelper.GetServerConfig(null);

            if (localRepository == null)
                throw new InvalidOperationException("Missing LocalRepository section in the default appDomain config file");

            // create catalog of the services
            //将localRepository中的信息传给HostMetadata对象
            HostMetadata catalog = new HostMetadata
            {
                HostName = localRepository.HostMetadata.HostName,
                ApplicationName = localRepository.HostMetadata.ApplicationName,
                MachineName = localRepository.HostMetadata.MachineName,
                AssemblyNames = localRepository.HostMetadata.AssemblyNames,
                List = new ServiceMetadataCollection()
            };

            if (localRepository.Enable)
            {
                #region the highest priority - LocalRepository

                //if (string.IsNullOrEmpty(localRepository.EndpointName) == false)
                //{
                //    // add to the catalog
                //    ChannelFactory<IBootstrap> factory = null;
                //    try
                //    {
                //        factory = new ChannelFactory<IBootstrap>(localRepository.EndpointName);
                //        IBootstrap channel = factory.CreateChannel();
                //        HostMetadata metadata = channel.GetMetadata(localRepository.HostMetadata.HostName, localRepository.HostMetadata.ApplicationName, localRepository.HostMetadata.MachineName, serviceName);
                //        if (metadata.List != null)
                //        {
                //            catalog.List.AddRange(metadata.List);
                //        }
                //        if (!string.IsNullOrEmpty(metadata.AssemblyNames))
                //        {
                //            catalog.AssemblyNames = string.IsNullOrEmpty(catalog.AssemblyNames) ? metadata.AssemblyNames : string.Concat(catalog.AssemblyNames, "; ", metadata.AssemblyNames);
                //        }
                //        factory.Close();
                //    }
                //    catch (Exception ex)
                //    {
                //        if (factory != null)
                //            factory.Abort();

                //        LoggerWrapper.Logger.Info(string.Format("Bootstrap channel failed, {0}", ex.Message));
                //        //tbd: logging
                //    }
                //}

                #endregion the highest priority - LocalRepository

                #region the middle priority - config file

                if (localRepository.HostMetadata.Services.Count > 0)
                {
                    // add to the catalog
                    //将localRepository中Services集合信息转换ServiceMetadata，放放到HostMetadata对象的List = new ServiceMetadataCollection()
                    foreach (ServiceMetadataElement item in localRepository.HostMetadata.Services)
                    {
                        if (catalog.List.Find(delegate(ServiceMetadata sm) { return sm.Name == item.Name; }) == null)
                        {
                            ServiceMetadata sm = new ServiceMetadata { AppDomainHostName = item.AppDomainHostName, Topic = item.Topic, Name = item.Name, ServiceType = item.ServiceType, ServiceNamespaces = item.ServiceNameSpace, GenerateWcfServiceType = item.GenerateWcfServiceType, Config = item.Config, AssemblyFolderName = item.AssemblyFolderName, AssemblyNames = item.AssemblyNames, BaseAddresses = item.BaseAddresses, WcfType = item.WcfType, Wrapper = item.Wrapper, ServiceInterfaceType = item.ServiceInterfaceType, IsAuthorization = item.IsAuthorization };
                            catalog.List.Add(sm);
                        }
                    }
                }

                #endregion the middle priority - config file
            }

            #region the lowest priority - by code

            if (inpCatalog != null)
            {
                // add to the catalog
                if (inpCatalog.List != null && inpCatalog.List.Count > 0)
                {
                    foreach (ServiceMetadata item in inpCatalog.List)
                    {
                        //从inpCatalog.List中的ServiceMetadata有无
                        if (catalog.List.Find(delegate(ServiceMetadata sm) { return sm.Name == item.Name; }) == null)
                        {
                            catalog.List.Add(item);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(inpCatalog.AssemblyNames))
                {
                    catalog.AssemblyNames = string.IsNullOrEmpty(catalog.AssemblyNames) ? inpCatalog.AssemblyNames : string.Concat(catalog.AssemblyNames, "; ", inpCatalog.AssemblyNames);
                }
            }

            #endregion the lowest priority - by code

            #region remove multiple assembly names

            if (!string.IsNullOrEmpty(catalog.AssemblyNames))
            {
                string[] asmNames = catalog.AssemblyNames.Split(new char[] { ';' });
                if (asmNames.Length > 1)
                {
                    StringBuilder sb = new StringBuilder();
                    string name = string.Empty;
                    Array.Sort<string>(asmNames);
                    for (int ii = 0; ii < asmNames.Length; ii++)
                    {
                        if (name == asmNames[ii]) continue;
                        name = asmNames[ii];
                        sb.AppendFormat("; {0}", name);
                    }
                    catalog.AssemblyNames = sb.ToString().Trim(new char[] { ';' });
                }
            }

            #endregion remove multiple assembly names

            // option: select only specified serviceName
            if (!string.IsNullOrEmpty(serviceName) && serviceName != "*")
            {
                catalog.List.RemoveAll(delegate(ServiceMetadata sm) { return sm.Name != serviceName; });
            }

            return catalog;
        }

        #endregion CreateCatalog

        #region CreateServices

        /// <summary>
        /// 建立服务
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns></returns>
        public HostServices CreateServices(HostMetadata catalog)
        {
            try
            {
                //change writerlook time By jiangsong 2009-11-14
                //_rwl.AcquireWriterLock(TimeSpan.FromSeconds(60));

                #region pre-validation

                if (catalog == null || catalog.List == null || catalog.List.Count == 0)
                {
                    throw new ArgumentNullException("There is no metadata in the catalog to load services");
                }
                this.CanBeAdded(catalog, true);

                #endregion pre-validation

                #region pre-load assemblies for services

                if (!string.IsNullOrEmpty(catalog.AssemblyNames))
                {
                    string[] assemblies = catalog.AssemblyNames.Split(new char[] { ';' });
                    foreach (string name in assemblies)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(name)) continue;

                            //AppDomain.CurrentDomain.Load(name, AppDomain.CurrentDomain.Evidence);

                            AppDomain.CurrentDomain.Load(name);
                        }
                        catch
                        {
                            // warning
                        }
                    }
                }

                #endregion pre-load assemblies for services

                #region create all services based on the metadata

                ///建立服务
                foreach (ServiceMetadata sm in catalog.List)
                {
                    try
                    {
                        // mandatory properties
                        if (string.IsNullOrEmpty(sm.Name))
                        {
                            throw new ArgumentException("ServiceMetadata.Name");
                        }

                        // runtime config data
                        ServiceConfigData serviceConfigData = new ServiceConfigData { Name = sm.Name, ServiceTypes = sm.ServiceType, ServiceNameSpaces = sm.ServiceNamespaces, GenerateWcfServiceType = sm.GenerateWcfServiceType, Topic = sm.Topic, Config = sm.Config, AssemblyFolderName = sm.AssemblyFolderName, AssemblyNames = sm.AssemblyNames, WcfType = sm.WcfType, Wrapper = sm.Wrapper, ServiceInterfaceType = sm.ServiceInterfaceType, IsAuthorization = sm.IsAuthorization };

                        Add(sm.AppDomainHostName, serviceConfigData, sm.BaseAddresses);
                    }
                    catch (Exception ex)
                    {
                        LoggerWrapper.Logger.Error(string.Format("Create {0} services failed, {1}", sm.AppDomainHostName, ex.Message), ex);

                        //throw ex;
                    }
                    finally
                    {
                        // tbd
                    }
                }

                #endregion create all services based on the metadata

                return this;
            }
            finally
            {
                //change writerlook time By jiangsong 2009-11-14
                //_rwl.ReleaseWriterLock();
            }
        }

        #endregion CreateServices

        #region Boot

        /// <summary>
        /// Boot services based on the source priority (config/repository)
        /// </summary>
        /// <returns></returns>
        public HostServices Boot()
        {
            return this.Boot(null, null);
        }

        /// <summary>
        /// Boot specific service based on the source priority (code, config, repository)
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public HostServices Boot(string serviceName)
        {
            return this.Boot(serviceName, null);
        }

        /// <summary>
        /// Boot services based on the source priority (code, config, repository)
        /// </summary>
        /// <param name="metadata">Metadata generated within the code (the lowest priority)</param>
        /// <returns>Services loaded into their appDomains</returns>
        public HostServices Boot(HostMetadata inpCatalog)
        {
            return this.Boot(null, inpCatalog);
        }

        /// <summary>
        ///  Boot specific service based on the source priority (code, config, repository)
        /// </summary>
        /// <param name="serviceName">name of the service to boot it (option)</param>
        /// <param name="inpCatalog"></param>
        /// <returns></returns>
        public HostServices Boot(string serviceName, HostMetadata inpCatalog)
        {
            // Load metadata to the catalog
            HostMetadata catalog = this.CreateCatalog(serviceName, inpCatalog);
            _hostMetadata = catalog;

            // host all services based on the metadata
            HostServices services = this.CreateServices(catalog);

            // done
            _hostName = catalog.HostName;

            return services;
        }

        #endregion Boot

        #region Add

        /// <summary>
        /// 是否可以添加服务
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="bThrow"></param>
        /// <returns></returns>
        private bool CanBeAdded(HostMetadata catalog, bool bThrow)
        {
            List<ServiceMetadataBase> hostedServices = this.GetHostedServices;

            foreach (ServiceMetadataBase hostedService in hostedServices)
            {
                //当前是否已经存在于当前domain
                ServiceMetadata reqService = catalog.List.Find(delegate(ServiceMetadata sm) { return sm.Name == hostedService.Name && sm.AppDomainHostName == hostedService.AppDomainHostName; });
                if (reqService == null) continue;

                if (bThrow)
                    throw new Exception(string.Format("The service {0} already exists in the domain {1}", hostedService.Name, hostedService.AppDomainHostName));
                else
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 动态建立host，service.
        /// </summary>
        /// <param name="appDomainName"></param>
        /// <param name="config"></param>
        /// <param name="serviceType"></param>
        /// <param name="baseAddresses"></param>
        private void Add(string appDomainName, ServiceConfigData config, List<Type> serviceTypes, string baseAddresses)
        {
            try
            {
                //change writerlook time By jiangsong 2009-11-14
                //_rwl.AcquireWriterLock(TimeSpan.FromSeconds(60));
                appDomainName = ValidateAppDomainName(appDomainName);
                AppDomain appDomain = this.CreateDomainHost(appDomainName, null);
                ServiceHostActivator.Create(appDomain, config, serviceTypes, baseAddresses);
            }
            finally
            {
                //change writerlook time By jiangsong 2009-11-14
                //_rwl.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// 动态建立host，service.
        /// </summary>
        private void Add(string appDomainName, ServiceConfigData config, string baseAddresses)
        {
            try
            {
                //获得写锁
                //change writerlook time By jiangsong 2009-11-14
                //_rwl.AcquireWriterLock(TimeSpan.FromSeconds(60));

                #region AppDomain Loading for Service

                appDomainName = ValidateAppDomainName(appDomainName);

                //得到当前GetExecutingAssembly的执行路径
                string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                //合并两个路径字符串。
                string shadowCacheFolder = Path.Combine(appPath, "ShadowCopy_" + config.AssemblyFolderName);

                //如果不存在，建立目录
                if (!Directory.Exists(shadowCacheFolder))
                {
                    Directory.CreateDirectory(shadowCacheFolder);
                }

                //合并两个路径字符串。
                string assemblyPath = Path.Combine(appPath, config.AssemblyFolderName);
                if (!Directory.Exists(assemblyPath))
                {
                    Directory.CreateDirectory(assemblyPath);
                }

                //如果appDomainName不包含在文件侦探器词典内，则监控此文件，当文件发生改变、建立、删除。
                if (!watchers.ContainsKey(appDomainName))
                {
                    FileSystemWatcher watcher = new FileSystemWatcher(assemblyPath);
                    watcher.Changed += new FileSystemEventHandler(AssemblyDirChanged);
                    watcher.Created += new FileSystemEventHandler(AssemblyDirChanged);
                    watcher.Deleted += new FileSystemEventHandler(AssemblyDirChanged);

                    //启用此组件。
                    watcher.EnableRaisingEvents = true;

                    //添加到文件监控词典中
                    watchers.Add(appDomainName, watcher);
                }

                //要追加到专用路径的目录名称。//*
                //----------------------------------------
                //Modify by jiangsong 2009-11-13
                //AppDomain.CurrentDomain.AppendPrivatePath(config.AssemblyFolderName);
                //----------------------------------------

                //应用程序域的每个实例同时包含属性和 AppDomainSetup 信息。您可以使用 System..::.AppDomain 类从应用程序域中检索安装信息。此类提供了几个检索有关应用程序域的配置信息的成员。
                AppDomainSetup ads = new AppDomainSetup();

                //设置包含该应用程序的目录的名称。
                ads.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

                //设置特定于应用程序的、影像复制文件所使用的区域的名称。
                ads.CachePath = shadowCacheFolder;

                //设置指示影像复制是打开还是关闭的字符串。
                ads.ShadowCopyFiles = "true";

                //设置应用程序基目录下的目录列表，这些目录被探测以寻找其中的私有程序集。
                ads.PrivateBinPath = config.AssemblyFolderName;

                //设置应用程序域的配置文件的名称。
                ads.ConfigurationFile = config.Config;
                AppDomain appDomain = this.CreateDomainHost(appDomainName, ads);

                #endregion AppDomain Loading for Service

                //建立激活ServiceHost
                ServiceHostActivator.Create(appDomain, config, null, baseAddresses);
            }
            finally
            {
                //释放写锁
                //change writerlook time By jiangsong 2009-11-14
                //_rwl.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// 建立DomainHos
        /// </summary>
        /// <param name="appDomainName"></param>
        /// <param name="ads"></param>
        /// <returns></returns>
        private AppDomain CreateDomainHost(string appDomainName, AppDomainSetup ads)
        {
            AppDomain appDomain = _appDomains.Find(delegate(AppDomain ad) { return ad.FriendlyName == appDomainName; });

            if (appDomain == null)
            {
                if (ads != null)
                {
                    appDomain = AppDomain.CurrentDomain.FriendlyName == appDomainName ? AppDomain.CurrentDomain : AppDomain.CreateDomain(appDomainName, null, ads);
                }
                else
                {
                    appDomain = AppDomain.CurrentDomain.FriendlyName == appDomainName ? AppDomain.CurrentDomain : AppDomain.CreateDomain(appDomainName);
                }
                _appDomains.Add(appDomain);

                appDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e)
                {
                    LoggerWrapper.Logger.Info(string.Format("[{0}] UnhandledException = {1}", (sender as AppDomain).FriendlyName, e.ExceptionObject));
                };
                appDomain.DomainUnload += delegate(object sender, EventArgs e)
                {
                    LoggerWrapper.Logger.Info(string.Format("[{0}] DomainUnload", (sender as AppDomain).FriendlyName));
                };
                appDomain.ProcessExit += delegate(object sender, EventArgs e)
                {
                    LoggerWrapper.Logger.Info(string.Format("[{0}] ProcessExit", (sender as AppDomain).FriendlyName));

                    // this.Close(appDomainName);
                };
            }
            return appDomain;
        }

        /// <summary>
        /// 验证AppDomainName，获取此应用程序域的友好名称。
        /// </summary>
        /// <param name="appDomainName"></param>
        /// <returns></returns>
        private string ValidateAppDomainName(string appDomainName)
        {
            if (string.IsNullOrEmpty(appDomainName) || appDomainName == "*" || appDomainName.ToLower() == "default")
            {
                appDomainName = AppDomain.CurrentDomain.FriendlyName;
            }
            return appDomainName;
        }

        #endregion Add

        #region GetAppDomainHost

        public AppDomain this[string appDomainName]
        {
            get
            {
                try
                {
                    //change writerlook time By jiangsong 2009-11-14
                    // _rwl.AcquireWriterLock(TimeSpan.FromSeconds(60));
                    appDomainName = ValidateAppDomainName(appDomainName);
                    AppDomain appDomain = _appDomains.Find(delegate(AppDomain ad) { return ad.FriendlyName == appDomainName; });
                    if (appDomain != null)
                    {
                        return appDomain;
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("Requested appDomain '{0}' doesn't exist in the catalog", appDomainName));
                    }
                }
                finally
                {
                    //change writerlook time By jiangsong 2009-11-14
                    // _rwl.ReleaseWriterLock();
                }
            }
        }

        #endregion GetAppDomainHost

        #region GetHostedServices

        public List<ServiceMetadataBase> GetHostedServices
        {
            get
            {
                List<ServiceMetadataBase> lists = new List<ServiceMetadataBase>();
                try
                {
                    //change writerlook time By jiangsong 2009-11-14
                    //_rwl.AcquireReaderLock(TimeSpan.FromSeconds(60));
                    foreach (AppDomain appDomain in _appDomains)
                    {
                        //为指定名称_key获取存储在当前应用程序域中的值。
                        List<ServiceHostActivator> activators = appDomain.GetData(_key) as List<ServiceHostActivator>;
                        if (activators != null)
                        {
                            foreach (ServiceHostActivator activator in activators)
                            {
                                lists.Add(new ServiceMetadataBase { AppDomainHostName = activator.AppDomainHost.FriendlyName, Name = activator.Name });
                            }
                        }
                    }
                }
                finally
                {
                    //change writerlook time By jiangsong 2009-11-14
                    //_rwl.ReleaseReaderLock();
                }
                return lists;
            }
        }

        #endregion GetHostedServices

        #region GetHostedService

        public ServiceHostBase GetHostedService(string name, bool flagRemove)
        {
            ServiceHostBase service = null;
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Not valid service name");

            try
            {
                //change writerlook time By jiangsong 2009-11-14
                //_rwl.AcquireReaderLock(TimeSpan.FromSeconds(60));
                foreach (AppDomain appDomain in _appDomains)
                {
                    //为指定名称_key获取存储在当前应用程序域中的值。
                    List<ServiceHostActivator> activators = appDomain.GetData(_key) as List<ServiceHostActivator>;
                    if (activators != null)
                    {
                        ServiceHostActivator activator = activators.Find(delegate(ServiceHostActivator a) { return a.Name == name; });
                        if (activator != null)
                        {
                            service = activator.Host;
                            if (flagRemove)
                            {
                                activators.Remove(activator);
                            }
                        }
                    }
                }
            }
            finally
            {
                //change writerlook time By jiangsong 2009-11-14
                //_rwl.ReleaseReaderLock();
            }
            return service;
        }

        #endregion GetHostedService

        #region Reload

        /// <summary>
        /// 重新载入HostMetadata
        /// </summary>
        /// <param name="appDomainName"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public int Reload(string appDomainName, HostMetadata metadata)
        {
            //已经打开或已经载入的数量
            int numberOfReloadedAndOpened = 0;

            //当HostMetadata中的服务为空时，异常
            if (metadata == null || metadata.List == null || metadata.List.Count == 0)
                throw new ArgumentException("Missing metadata");

            try
            {
                //写日志，并记录当前线程id,appdomainName
                LoggerWrapper.Logger.Info(string.Format("[{0}] Reload {1} waiting ...", Thread.CurrentThread.ManagedThreadId, appDomainName));

                //打开写锁
                //change writerlook time By jiangsong 2009-11-14
                //_rwl.AcquireWriterLock(TimeSpan.FromSeconds(160));

                LoggerWrapper.Logger.Info(string.Format("[{0}] Reload {1} starting ...", Thread.CurrentThread.ManagedThreadId, appDomainName));

                //将服务列表清除
                metadata.List.RemoveAll(delegate(ServiceMetadata sm) { return sm.AppDomainHostName != appDomainName; });
                if (metadata.List.Count > 0)
                {
                    //开始终止此线程的过程
                    this.Abort(appDomainName, true);

                    if (this.CreateServices(metadata) == null)
                        throw new InvalidOperationException("The appDomain has been aborted, but the loading failed");

                    // 打开appdomain
                    this.Open(appDomainName);

                    //从HostMetadata得到服务数量
                    numberOfReloadedAndOpened = metadata.List.Count;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Mismatch input parameters. The metadata is not valid for {0} appDomainName.", appDomainName));
                }
            }
            finally
            {
                //记录日志
                LoggerWrapper.Logger.Info(string.Format("[{0}] Reload {1} done.", Thread.CurrentThread.ManagedThreadId, appDomainName));

                //释放锁
                ////change writerlook time By jiangsong 2009-11-14
                //_rwl.ReleaseLock();
            }
            return numberOfReloadedAndOpened;
        }

        #endregion Reload

        #region Load

        /// <summary>
        /// Load services to the non-default appDomain
        /// 与Reload注释相似
        /// </summary>
        /// <param name="appDomainName"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public int Load(string appDomainName, HostMetadata metadata)
        {
            int numberOfReloadedAndOpened = 0;
            if (metadata == null || metadata.List == null || metadata.List.Count == 0)
                throw new ArgumentException("Missing metadata");

            try
            {
                LoggerWrapper.Logger.Info(string.Format("[{0}] Load {1} waiting ...", Thread.CurrentThread.ManagedThreadId, appDomainName));

                //change writerlook time By jiangsong 2009-11-14
                //_rwl.AcquireWriterLock(TimeSpan.FromSeconds(160));

                LoggerWrapper.Logger.Info(string.Format("[{0}] Load {1} starting ...", Thread.CurrentThread.ManagedThreadId, appDomainName));

                metadata.List.RemoveAll(delegate(ServiceMetadata sm) { return string.IsNullOrEmpty(sm.AppDomainHostName) || sm.AppDomainHostName != appDomainName; });
                if (metadata.List.Count > 0)
                {
                    // check domain for services
                    CanBeAdded(metadata, true);

                    // create services based on the metadata
                    if (this.CreateServices(metadata) == null)
                        throw new InvalidOperationException(string.Format("Loading service(s) into domain {0} failed", appDomainName));

                    // open just registered services
                    this.Open(appDomainName);

                    numberOfReloadedAndOpened = metadata.List.Count;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Mismatch input parameters. The metadata is not valid for {0} appDomainName.", appDomainName));
                }
            }
            finally
            {
                LoggerWrapper.Logger.Info(string.Format("[{0}] Load {1} done.", Thread.CurrentThread.ManagedThreadId, appDomainName));

                //change writerlook time By jiangsong 2009-11-14
                //_rwl.ReleaseLock();
            }
            return numberOfReloadedAndOpened;
        }

        #endregion Load

        #region Open

        /// <summary>
        /// 打开服务列表
        /// </summary>
        /// <returns></returns>
        public int Open()
        {
            int count = 0;
            try
            {
                //change writerlook time By jiangsong 2009-11-14
                // _rwl.AcquireWriterLock(TimeSpan.FromSeconds(60));
                foreach (AppDomain ad in _appDomains)
                {
                    List<ServiceHostActivator> activators = ad.GetData(_key) as List<ServiceHostActivator>;
                    if (activators != null)
                    {
                        //遍历打开激活
                        activators.ForEach(delegate(ServiceHostActivator activator)
                        {
                            activator.Open();
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("Dorado.ESB.Framework", ex);

                //throw ex;
            }
            finally
            {
                count = _appDomains.Count;

                //change writerlook time By jiangsong 2009-11-14
                //_rwl.ReleaseWriterLock();
            }
            return count;
        }

        /// <summary>
        /// 相上注释相似，重载
        /// </summary>
        /// <param name="appDomainName"></param>
        public void Open(string appDomainName)
        {
            try
            {
                //change writerlook time By jiangsong 2009-11-14
                //_rwl.AcquireWriterLock(TimeSpan.FromSeconds(60));
                appDomainName = ValidateAppDomainName(appDomainName);
                AppDomain appDomain = _appDomains.Find(delegate(AppDomain ad) { return ad.FriendlyName == appDomainName; });
                if (appDomain != null)
                {
                    if (appDomain.IsDefaultAppDomain() == false)
                    {
                        List<ServiceHostActivator> activators = appDomain.GetData(_key) as List<ServiceHostActivator>;
                        if (activators != null)
                        {
                            activators.ForEach(delegate(ServiceHostActivator activator)
                            {
                                activator.Open();
                            });
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Open '{0}' appDomain host failed - doesn't exist", appDomainName));
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("Dorado.ESB.Framework", ex);
            }
            finally
            {
                //change writerlook time By jiangsong 2009-11-14
                //_rwl.ReleaseWriterLock();
            }
        }

        #endregion Open

        #region Close

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            try
            {
                _rwl.AcquireWriterLock(TimeSpan.FromSeconds(60));
                _appDomains.Reverse();
                foreach (AppDomain ad in _appDomains)
                {
                    List<ServiceHostActivator> activators = ad.GetData(_key) as List<ServiceHostActivator>;
                    if (activators != null)
                    {
                        activators.Reverse();
                        activators.ForEach(delegate(ServiceHostActivator activator)
                        {
                            activator.Close();
                        });
                        activators.Clear();
                    }
                }

                int count = _appDomains.RemoveAll(delegate(AppDomain ad)
                {
                    if (!ad.IsDefaultAppDomain())
                    {
                        AppDomain.Unload(ad);
                    }
                    return true;
                });
                _appDomains.Clear();
            }
            finally
            {
                _rwl.ReleaseWriterLock();
            }
        }

        public void Close(string appDomainName)
        {
            this.Close(appDomainName, true);
        }

        /// <summary>
        /// 关闭（重载）
        /// </summary>
        /// <param name="appDomainName"></param>
        /// <param name="bThrow"></param>
        /// <returns></returns>
        public bool Close(string appDomainName, bool bThrow)
        {
            bool bHasBeenClosed = false;
            try
            {
                LoggerWrapper.Logger.Info(string.Format("[{0}] Close {1} waiting ...", Thread.CurrentThread.ManagedThreadId, appDomainName));
                _rwl.AcquireWriterLock(TimeSpan.FromSeconds(60));

                LoggerWrapper.Logger.Info(string.Format("[{0}] Close {1} starting ...", Thread.CurrentThread.ManagedThreadId, appDomainName));
                appDomainName = ValidateAppDomainName(appDomainName);
                AppDomain appDomain = _appDomains.Find(delegate(AppDomain ad) { return ad.FriendlyName == appDomainName; });
                if (appDomain != null)
                {
                    if (appDomain.IsDefaultAppDomain() == false)
                    {
                        List<ServiceHostActivator> activators = appDomain.GetData(_key) as List<ServiceHostActivator>;
                        activators.Reverse();
                        if (activators != null)
                        {
                            activators.ForEach(delegate(ServiceHostActivator activator)
                            {
                                activator.Close();
                            });
                        }

                        // clean-up
                        activators.Clear();
                        _appDomains.Remove(appDomain);
                        AppDomain.Unload(appDomain);
                        bHasBeenClosed = true;
                    }
                    else if (bThrow)
                    {
                        throw new InvalidOperationException("The Close operation can't be processed on the default appDomain");
                    }
                }
            }
            finally
            {
                LoggerWrapper.Logger.Info(string.Format("[{0}] Close {1} done ...", Thread.CurrentThread.ManagedThreadId, appDomainName));
                _rwl.ReleaseWriterLock();
            }
            return bHasBeenClosed;
        }

        #endregion Close

        #region Abort

        /// <summary>
        /// 终止appDomain
        /// </summary>
        /// <param name="appDomainName"></param>
        /// <param name="bThrow"></param>
        /// <returns></returns>
        public bool Abort(string appDomainName, bool bThrow)
        {
            bool bHasBeenAborted = false;
            try
            {
                LoggerWrapper.Logger.Info(string.Format("[{0}] Abort {1} waiting ...", Thread.CurrentThread.ManagedThreadId, appDomainName));

                //_rwl.AcquireWriterLock(TimeSpan.FromSeconds(60));

                LoggerWrapper.Logger.Info(string.Format("[{0}] Abort {1} starting ...", Thread.CurrentThread.ManagedThreadId, appDomainName));
                appDomainName = ValidateAppDomainName(appDomainName);
                AppDomain appDomain = _appDomains.Find(delegate(AppDomain ad) { return ad.FriendlyName == appDomainName; });

                if (appDomain != null)
                {
                    if (appDomain.IsDefaultAppDomain() == false)
                    {
                        List<ServiceHostActivator> activators = appDomain.GetData(_key) as List<ServiceHostActivator>;
                        if (activators != null)
                        {
                            activators.Reverse();
                            if (activators != null)
                            {
                                activators.ForEach(delegate(ServiceHostActivator activator)
                                {
                                    activator.Abort();
                                });
                            }

                            // clean-up
                            activators.Clear();
                        }
                        _appDomains.Remove(appDomain);
                        AppDomain.Unload(appDomain);
                        bHasBeenAborted = true;
                    }
                    else if (bThrow)
                    {
                        throw new InvalidOperationException("The Abort operation can't be processed on the default appDomain");
                    }
                }
            }
            finally
            {
                LoggerWrapper.Logger.Info(string.Format("[{0}] Abort {1} done", Thread.CurrentThread.ManagedThreadId, appDomainName));

                //_rwl.ReleaseWriterLock();
            }
            return bHasBeenAborted;
        }

        #endregion Abort

        #region Current

        /// <summary>
        /// 得到当前CurrentDomain
        /// </summary>
        public static HostServices Current
        {
            get
            {
                string key = typeof(HostServices).FullName;
                HostServices hostservices = AppDomain.CurrentDomain.GetData(key) as HostServices;
                if (hostservices == null)
                {
                    lock (AppDomain.CurrentDomain.FriendlyName)
                    {
                        hostservices = AppDomain.CurrentDomain.GetData(key) as HostServices;
                        if (hostservices == null)
                        {
                            hostservices = new HostServices(true);
                            AppDomain.CurrentDomain.SetData(key, hostservices);
                        }
                    }
                }
                return hostservices;
            }
        }

        #endregion Current

        #region Service AssemblyChanged

        /// <summary>
        /// Stops the server, reloads the assembly and restarts the server.
        /// </summary>
        public void ReloadAssembly(string serviceName)
        {
            try
            {
                HostMetadata hostMD = CreateCatalog(serviceName, null);
                if (hostMD != null && hostMD.List.Count > 0)
                {
                    ServiceMetadata sMD = hostMD.List[0];
                    HostServices.Current.Reload(sMD.AppDomainHostName, hostMD);
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Info("Dorado Platform Server:Exception recycling Dorado Platform Server Domain: " + ex.ToString() + Environment.NewLine + "Trying again with no runstate.");

                //HostServices.Current.Boot(serviceName);
                //HostServices.Current.Open();
            }
        }

        /// <summary>
        /// Assembly目录改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssemblyDirChanged(object sender, FileSystemEventArgs e)
        {
            string thisMinute = System.DateTime.Now.Hour.ToString() + ":" + System.DateTime.Now.Minute.ToString();
            resourceLock.Enter(true);
            {
                //checks to see if a reload was already scheduled keyed by hh:mm, if so
                //checks to see if it contained the specific file
                if (pendingAssemblyReloadMinute.Contains(thisMinute))
                {
                    if (!pendingAssemblyFileNames.Contains(e.Name))
                    {
                        pendingAssemblyFileNames.Add(e.Name);
                        LoggerWrapper.Logger.Info("Dorado Platform Server:Got change for " + e.Name + ". Processing with other changes made during " + thisMinute);
                    }
                    return;
                }

                //store this reload to ensure we only reload once for multiple files being changed.
                LoggerWrapper.Logger.Info("Dorado Platform Server:Got change for " + e.Name + " during " + thisMinute + ". Processing in five seconds.");
                pendingAssemblyReloadMinute.Add(thisMinute);
                pendingAssemblyFileNames.Add(e.Name);
            }
           resourceLock.Leave();
            string serviceName = "default";
            if (_hostMetadata != null)
            {
                try
                {
                    ServiceMetadata srvM = _hostMetadata.List.Find(delegate(ServiceMetadata sm) { return e.FullPath.Contains("\\" + sm.AssemblyFolderName + "\\"); });
                    serviceName = srvM.Name;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            //setup a timer to defer the reload to ensure all files in the directory have changed
            //this would allow time for files being copied to complete
            CountdownTimer timer = new CountdownTimer();
            timer.BeginCountdown(5000, ProcessAssemblyChange, thisMinute + "|" + serviceName);
        }

        /// <summary>
        /// 处理Assembly改变
        /// </summary>
        /// <param name="ar"></param>
        private void ProcessAssemblyChange(IAsyncResult ar)
        {
            //string thisMinute = (string)ar.AsyncState;
            string[] value = ((string)ar.AsyncState).Split('|');
            string thisMinute = value[0];
            string serviceName = value[1];

            resourceLock.Enter(true);
            {
                pendingAssemblyReloadMinute.Remove(thisMinute);
                pendingAssemblyFileNames.Clear();
                if (nodeChangedDelegate != null)
                {
                    try
                    {
                        nodeChangedDelegate(serviceName);
                    }
                    catch (Exception ex)
                    {
                        LoggerWrapper.Logger.Info("Error Changing Node Assembly: " + ex.ToString());
                    }
                }
            }
            resourceLock.Leave();
        }

        #endregion Service AssemblyChanged
    }

    #endregion HostServices
}