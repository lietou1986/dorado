using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.ESB.ClientProxyFactory;
using Dorado.ESB.ClientProxyFactory.Config;
using Dorado.ESB.ClientProxyFactory.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dorado.ESB.Client
{
    public class PlatformServiceFactory<T> where T : class
    {
        private static Dictionary<string, object> dictionaryCache = new Dictionary<string, object>();
        private static readonly Object thisLock = new Object();

        private static string ApplicationName
        {
            get
            {
                string applicationName = System.Configuration.ConfigurationManager.AppSettings["applicationName"];
                if (applicationName != null)
                {
                    return applicationName;
                }
                else
                {
                    Exception ex = new Exception("ApplicationName in the configuration file can not be empty");
                    LoggerWrapper.Logger.Error("Configuration Error", ex);
                    throw ex;
                }
            }
        }

        private static Service GetService(string serviceModule)
        {
            try
            {
                Service service = ESBClientSettings.Instance.Services.SingleOrDefault(c => c.Name == String.Format("PlatformServices_{0}", serviceModule));
                if (service != null)
                {
                    return service;
                }
                else
                {
                    Exception ex = new Exception(String.Format("{0} Service in the configuration file can not be empty", serviceModule));
                    LoggerWrapper.Logger.Error("Configuration Error", ex);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("Configuration Error", ex);
                throw ex;
            }
        }

        private static object CreateImpObject(string serviceAssemblyName, string typeName)
        {
            Type serviceType = (Type)AssembliesProvider.Instance.CurrentAssemblies.FirstOrDefault(s => s.GetName().Name == serviceAssemblyName).GetType(typeName);
            if (serviceType != null)
            {
                BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty;
                object objType = serviceType.InvokeMember("Instance", bindingFlags, null, null, null);
                lock (thisLock)
                {
                    if (!dictionaryCache.ContainsKey(typeName))
                    {
                        dictionaryCache.Add(typeName, objType);
                        LoggerWrapper.Logger.Info(String.Format("Add {0} to the Dictionary Cache", objType.ToString()));
                    }
                }

                return objType;
            }
            else
            {
                Exception ex = new Exception(String.Format("The current run program of the directory does not contain implementation dll,Namespace:{0},Type:{1}", serviceAssemblyName, typeName));
                LoggerWrapper.Logger.Error("Configuration Error", ex);
                throw ex;
            }
        }

        public static T CreateInstance()
        {
            string serviceNamespace = GetImplementName(typeof(T).Namespace);
            string serviceAssemblyName = GetImplementName(typeof(T).Assembly.GetName().Name);
            string serviceTypeName = typeof(T).Name.Remove(0, 1);
            string applicationName = ApplicationName;
            Service service = GetService(serviceAssemblyName);
            T obj = default(T);
            object objType = null;
            if (service.UseLocalService && String.Compare(service.ControlLevel, "Whole", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                // Whole,Local
                try
                {
                    string typeName = String.Format("{0}.{1}", serviceNamespace, serviceTypeName);
                    dictionaryCache.TryGetValue(typeName, out objType);
                    if (objType == null)
                    {
                        try
                        {
                            objType = CreateImpObject(serviceAssemblyName, typeName);
                        }
                        catch (Exception ex)
                        {
                            LoggerWrapper.Logger.Error("Configuration Error", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Logger.Error("Configuration Error", ex);
                }
                obj = objType as T;
                return obj;
            }
            else
            {
                // Whole,Remote/ Single,Local or Remote
                try
                {
                    obj = (T)WcfClientManager.Instance.GetProtocolObject<T>(String.Format("PlatformServices_{0}", serviceAssemblyName));
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Logger.Error("Call Error", ex);
                }
                return obj;
            }
        }

        private static string GetImplementName(string ns)
        {
            return ns.Replace("ServiceInterface", "ServiceImp");
        }
    }
}