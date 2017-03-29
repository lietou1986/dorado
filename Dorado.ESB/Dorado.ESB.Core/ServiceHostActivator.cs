using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.ESB.Common.Utility;
using Dorado.ESB.Core.Contracts;
using Dorado.ESB.ServiceGenerateFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;

namespace Dorado.ESB.Core
{
    #region Service activator

    // this is a service activator acrross the appDomains
    // 激活应用程序域
    public sealed class ServiceHostActivator : MarshalByRefObject, IDisposable
    {
        private ServiceHostBase _host = null;

        #region Dispose

        void IDisposable.Dispose()
        {
            //Dorado.Parallel.ParallelProvider.Instance.Dispose();
            Close();
        }

        public override object InitializeLifetimeService()
        {
            // infinite lifetime
            return null;
        }

        #endregion Dispose

        #region Create

        private const string IWCFAPI = "Dorado.PlatformServices.ServiceInterface.IWcfApi";
        private const string WCFAPI = "Dorado.PlatformServices.ServiceImp.WcfApi";

        /// <summary>
        ///
        /// </summary>
        /// <param name="appDomain"></param>
        /// <param name="config"></param>
        /// <param name="serviceTypes"></param>
        /// <param name="baseAddresses"></param>
        /// <returns></returns>
        public static ServiceHostActivator Create(AppDomain appDomain, ServiceConfigData config, List<Type> serviceTypes, string baseAddresses)
        {
            //从Assembly中得到类型为ServiceHostActivator的名称
            string _assemblyName = Assembly.GetAssembly(typeof(ServiceHostActivator)).FullName;

            //创建指定类型的新实例。
            ServiceHostActivator activator = appDomain.CreateInstanceAndUnwrap(_assemblyName, typeof(ServiceHostActivator).ToString()) as ServiceHostActivator;
            activator.SetHost(config, serviceTypes, baseAddresses);
            return activator;
        }

        /// <summary>
        /// 在AppDomain中host assembly
        /// </summary>
        /// <param name="config"></param>
        /// <param name="serviceTypes"></param>
        /// <param name="baseAddresses"></param>
        private void SetHost(ServiceConfigData config, List<Type> serviceTypes, string baseAddresses)
        {
            try
            {
                if (_host == null)
                {
                    // workaround for passing file/string to the override ApplyConfiguration
                    //提供与执行代码路径一起传送的属性集,存储给定对象并将其与指定名称关联
                    //_config要与新项关联的调用上下文中的名称
                    //要存储在调用上下文中的对象
                    CallContext.SetData("_config", config.Config);

                    if (serviceTypes == null)
                    {
                        if (!string.IsNullOrEmpty(config.AssemblyNames))
                        {
                            string[] assemblies = config.AssemblyNames.Split(new char[] { ';' });
                            foreach (string name in assemblies)
                            {
                                try
                                {
                                    if (string.IsNullOrEmpty(name)) continue;

                                    //AppDomain.CurrentDomain.Load(name, AppDomain.CurrentDomain.Evidence);
                                    AppDomain.CurrentDomain.Load(name);
                                }
                                catch (Exception ex)
                                {
                                    LoggerWrapper.Logger.Info(string.Format("load  Assembly failed: {0}", ex.Message));
                                    throw ex;
                                }
                            }
                        }

                        serviceTypes = new List<Type>();
                        List<string> configServiceTypes = config.ServiceTypes.Split(';').ToList();
                        configServiceTypes.ForEach(delegate(string tmpServiceType)
                        {
                            Type type = Type.GetType(tmpServiceType, false);
                            if (type != null) serviceTypes.Add(type);
                        });

                        if (serviceTypes == null || serviceTypes.Count == 0)
                        {
                            configServiceTypes.ForEach
                                (
                                    delegate(string tmpServiceType)
                                    {
                                        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                                        {
                                            Type type = assembly.GetType(tmpServiceType, false);
                                            if (type != null)
                                            {
                                                serviceTypes.Add(type);
                                                break;
                                            }
                                        }
                                    }
                                );
                        }

                        if (serviceTypes == null)
                        {
                            throw new ArgumentException(string.Format("Can't create the service type from metadata '{0}' where serviceType=null.", config.ServiceTypes));
                        }
                    }

                    Type interfaceType, impType;

                    if (config.WcfType == "ServiceHost")
                    {
                        GenerateWrapperForwarding(config, serviceTypes, out interfaceType, out impType, ServiceModelType.ServiceHost);
                        _host = (ServiceHostBase)new ServiceHostPlatformServices(config, impType, interfaceType, this.BaseAddresses(baseAddresses));
                    }
                    else if (config.WcfType == "WebServiceHost")
                    {
                        GenerateWrapperForwarding(config, serviceTypes, out interfaceType, out impType, ServiceModelType.WebServiceHost);
                        _host = (ServiceHostBase)new WebServiceHostPlatformServices(config, impType, interfaceType, this.BaseAddresses(baseAddresses));
                    }

                    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                    _host.Faulted += new EventHandler(_host_Faulted);
                    this.AddToStorage(this);
                }
                else
                {
                    throw new InvalidOperationException("The ServiceHost has been already setup");
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Info(string.Format("ServiceHostActivator failed: {0}", ex.Message));
                throw ex;
            }
            finally
            {
                CallContext.FreeNamedDataSlot("_config");
            }
        }

        private static Type GetInterfaceType(string interfaceType)
        {
            Type type = null;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(interfaceType, false);
                if (type != null)
                {
                    break;
                }
            }
            return type;
        }

        private static void GenerateWrapperForwarding(ServiceConfigData config, List<Type> serviceTypes, out Type interfaceType, out Type impType, ServiceModelType serviceModelType)
        {
            Factory serviceFactory = new Factory();
            List<Type[]> listTypes = new List<Type[]>();
            serviceTypes.ForEach(delegate(Type type) { listTypes.Add(type.Assembly.GetTypes()); });
            Assembly genAssembly;
            if (config.Wrapper)
            {
                genAssembly = serviceFactory.GetGenerateServiceAssemblyForwarding(Forwarding.All, serviceModelType, config.ServiceNameSpaces, listTypes, config.IsAuthorization, config.Name);
                interfaceType = genAssembly.GetType(IWCFAPI, true);
                impType = genAssembly.GetType(WCFAPI, true);
            }
            else
            {
                interfaceType = GetInterfaceType(config.ServiceInterfaceType);
                genAssembly = serviceFactory.GetGenerateServiceAssemblyForwarding(Forwarding.Implement, serviceModelType, config.ServiceNameSpaces, listTypes, config.IsAuthorization, config.Name);
                impType = genAssembly.GetType(WCFAPI, true);
            }
        }

        /// <summary>
        /// 记录失败日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _host_Faulted(object sender, EventArgs e)
        {
            LoggerWrapper.Logger.Info(string.Format("ServiceHostActivator.Faulted: {0}", sender));
        }

        /// <summary>
        /// 将activator中配置对比，将没有的加入
        /// </summary>
        /// <param name="activator"></param>
        private void AddToStorage(ServiceHostActivator activator)
        {
            List<ServiceHostActivator> activators = this.GetStorage();
            if (activators.Exists(delegate(ServiceHostActivator host) { return host._host.Description.ConfigurationName == activator._host.Description.ConfigurationName; }))
            {
                throw new InvalidOperationException(string.Format("The service '{0}' is already hosted in the appDomain '{1}'", activator._host.Description.ConfigurationName, AppDomain.CurrentDomain.FriendlyName));
            }
            activators.Add(this);
        }

        /// <summary>
        /// 移除指定的ServiceHostActivator
        /// </summary>
        /// <param name="activator"></param>
        private void RemoveFromStorage(ServiceHostActivator activator)
        {
            List<ServiceHostActivator> activators = this.GetStorage();
            if (activators.Exists(delegate(ServiceHostActivator host) { return host._host.Description.ConfigurationName == activator._host.Description.ConfigurationName; }))
            {
                activators.Remove(activator);
            }
        }

        /// <summary>
        /// 从当前获得AppDomainServiceHostActivator列表
        /// </summary>
        /// <returns></returns>
        private List<ServiceHostActivator> GetStorage()
        {
            string key = typeof(ServiceHostActivator).FullName;
            List<ServiceHostActivator> activators = AppDomain.CurrentDomain.GetData(key) as List<ServiceHostActivator>;
            if (activators == null)
            {
                lock (AppDomain.CurrentDomain.FriendlyName)
                {
                    activators = AppDomain.CurrentDomain.GetData(key) as List<ServiceHostActivator>;
                    if (activators == null)
                    {
                        activators = new List<ServiceHostActivator>();
                        AppDomain.CurrentDomain.SetData(key, activators);
                    }
                }
            }
            return activators;
        }

        /// <summary>
        /// 分隔地址成列表
        /// </summary>
        /// <param name="baseAddresses"></param>
        /// <returns></returns>
        private Uri[] BaseAddresses(string baseAddresses)
        {
            List<Uri> listOfBaseAddr = new List<Uri>();
            if (string.IsNullOrEmpty(baseAddresses) == false)
            {
                string[] s = baseAddresses.Split(new char[] { ' ', ';' });
                for (int ii = 0; ii < s.Length; ii++)
                {
                    s[ii] = Helper.ReplaceLocalhostToIPAdress(s[ii]);
                    listOfBaseAddr.Add(new Uri(s[ii]));
                }
            }
            return listOfBaseAddr.ToArray();
        }

        /// <summary>
        /// 当前domain中未经过处理的异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
                LoggerWrapper.Logger.Error("PlatformService", ex);
        }

        #endregion Create

        #region Remoting

        public void Open()
        {
            if (_host != null)
            {
                try
                {
                    if (_host.State == CommunicationState.Created)
                    {
                        _host.Open();
                    }
                }
                catch (Exception ex)
                {
                    RemoveFromStorage(this);
                    LoggerWrapper.Logger.Info(string.Format("ServiceHostActivator.Open failed: {0}", ex.Message));
                    throw ex;
                }
            }
        }

        public void Close()
        {
            if (_host != null)
            {
                try
                {
                    if (_host.State == CommunicationState.Opened || _host.State == CommunicationState.Created)
                    {
                        //Dorado.Parallel.ParallelProvider.Instance.Dispose();
                        _host.Close();
                    }
                    else if (_host.State == CommunicationState.Faulted)
                    {
                        //Dorado.Parallel.ParallelProvider.Instance.Dispose();
                        _host.Abort();
                    }
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Logger.Info(string.Format("ServiceHostActivator.Close failed: {0}", ex.Message));
                    throw ex;
                }
            }
        }

        public void Abort()
        {
            if (_host != null)
            {
                try
                {
                    //Dorado.Parallel.ParallelProvider.Instance.Dispose();
                    _host.Abort();
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Logger.Info(string.Format("ServiceHostActivator.Abort failed: {0}", ex.Message));
                    throw ex;
                }
            }
        }

        public ServiceHostBase Host { get { return _host; } } // valid only for default domain!

        public string Name { get { return _host.Description.ConfigurationName; } }

        public AppDomain AppDomainHost { get { return AppDomain.CurrentDomain; } }

        #endregion Remoting
    }

    #endregion Service activator
}