using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dorado.ESB.ServiceGenerateFactory
{
    /// <summary>
    /// Provides Runtime loading of Classes.
    /// MarshalByRefObject允许在支持远程处理的应用程序中跨应用程序域边界访问对象。
    /// </summary>
    public class Factory : MarshalByRefObject//
    {
        /// <summary>
        /// Loads the named class from the given named assembly.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly.</param>
        /// <param name="className">The namd of the class.</param>
        /// <param name="fileName">Out.  The filename that was loaded.</param>
        /// <returns>An instance of the named class.</returns>
        public object LoadClass(string assemblyName, string className, out string fileName)
        {
            object loadedClass = null;
            fileName = String.Empty;
            LoggerWrapper.Logger.Info("Platform Service Factory:Loading " + className + " from " + assemblyName + ".");
            try
            {
                //Assembly assembly = AppDomain.CurrentDomain.Load(assemblyName, AppDomain.CurrentDomain.Evidence);//获取与此应用程序域相关联的 Evidence，它用作安全策略的输入。
                Assembly assembly = AppDomain.CurrentDomain.Load(assemblyName);
                LoggerWrapper.Logger.Info("Platform Service Factory:Got " + assembly.FullName + " from file " + assembly.Location + ".");
                fileName = assembly.Location.Substring(assembly.Location.LastIndexOf(@"\") + 1).ToLower();
                loadedClass = assembly.CreateInstance(className);//激活器创建它的实例。
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Info("Platform Service Factory:Error loading assembly: " + ex.ToString());
                loadedClass = null;
            }
            return loadedClass;
        }

        /// <summary>
        /// 动态建立host，取得类型
        /// </summary>
        /// <param name="serviceModelType"></param>
        /// <param name="serviceNamespaces"></param>
        /// <param name="type"></param>
        /// <param name="baseAddresses"></param>
        /// <returns></returns>
        public Type GetPlatformServiceHost(ServiceModelType serviceModelType, string serviceNamespaces, List<Type[]> listTypes)
        {
            try
            {
                //Assembly assembly = AppDomain.CurrentDomain.Load("Dorado.ESB.GenerateServices", AppDomain.CurrentDomain.Evidence);
                Assembly assembly = AppDomain.CurrentDomain.Load("Dorado.ESB.GenerateServices");
                IGenerateService genService = (IGenerateService)assembly.CreateInstance("Dorado.ESB.GenerateServices.GenerateService");
                Type genServiceType = genService.GetGenerateServiceType(serviceModelType, serviceNamespaces, listTypes);
                return genServiceType;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Info("Dorado Platform Service:Error loading: " + ex.ToString());
                return null;
            }
        }

        public Assembly GetGenerateServiceAssemblyForwarding(Forwarding forwarding, ServiceModelType serviceModelType, string serviceNamespaces, List<Type[]> listTypes, bool isAuthorization, string name)
        {
            try
            {
                //Assembly assembly = AppDomain.CurrentDomain.Load("Dorado.ESB.GenerateServices", AppDomain.CurrentDomain.Evidence);
                Assembly assembly = AppDomain.CurrentDomain.Load("Dorado.ESB.GenerateServices");
                IGenerateService genService = (IGenerateService)assembly.CreateInstance("Dorado.ESB.GenerateServices.GenerateService");
                Assembly genServiceAssembly = genService.GetGenerateServiceAssemblyForwarding(forwarding, serviceModelType, serviceNamespaces, listTypes, isAuthorization, name);
                return genServiceAssembly;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Info("Dorado Platform Service:Error loading: " + ex.ToString());
                return null;
            }
        }
    }
}