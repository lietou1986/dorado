using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.ESB.ClientProxyFactory.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;

namespace Dorado.ESB.ClientProxyFactory.Proxy
{
    public class WcfClientGeneratorManager
    {
        private const string NameSpace = "__wcfProxy";
        private static Dictionary<Type, ConstructorInfo> dictTypeConstructor = new Dictionary<Type, ConstructorInfo>();

        private static string GetClientFactoryClassName(string ns)
        {
            return ns.Replace('.', '_') + "_IWcfApi_ClientFactory";
        }

        public static string GetClientClassName(string ns)
        {
            return ns.Replace('.', '_') + "_IWcfApi_Client";
        }

        private static string GetImplementAssemblyName(WcfClientConfig config)
        {
            return config.Name + config.ABC.Type.FullName.Replace('.', '_') + "_assembly";
        }

        private static string GetImplementAssemblyName()
        {
            return "wcf_client_assembly";
        }

        public static object[] GetProtocolObjects(ESBClientSettings clientSettings)
        {
            dictTypeConstructor.Clear();
            List<string> listLocation = new List<string>();
            listLocation.Add(typeof(EndpointAddress).Assembly.Location);
            listLocation.Add(typeof(WcfClientGenerator).Assembly.Location);
            listLocation.Add(typeof(System.ServiceModel.WebHttpBinding).Assembly.Location);
            listLocation.Add(typeof(LoggerWrapper).Assembly.Location);
            listLocation.Add(typeof(AssembliesProvider).Assembly.Location);
            List<string> listCompiledServiceName = new List<string>();
            List<Type> listInterfaceType = new List<Type>();
            List<Type> listFactoryType = new List<Type>();
            List<string> listSourceCode = new List<string>();
            Dictionary<string, object> dictServiceObject = new Dictionary<string, object>(clientSettings.Services.Count);
            Type type = null;
            foreach (Service service in clientSettings.Services)
            {
                listSourceCode.Clear();
                WcfClientConfig config = null;
                if (ESBClientSettingsManager.Instance.GetWcfClientConfigDictionary().Count > 0)
                {
                    config = ESBClientSettingsManager.Instance.GetWcfClientConfigDictionary().SingleOrDefault(c => c.Key == service.Name).Value;
                    if (config == null)
                        continue;
                }
                else
                {
                    continue;
                }

                type = config.ABC.Type;

                ConstructorInfo ci;
                lock (dictTypeConstructor)
                {
                    dictTypeConstructor.TryGetValue(type, out ci);
                }
                if (ci != null)
                {
                    object obj = GetProtocolObject(ci, config);
                    dictServiceObject.Add(service.Name, obj);
                    continue;
                }

                try
                {
                    listLocation.Add(type.Assembly.Location);
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Logger.Error("LocationAssembly Error", ex);
                }

                string clientName = GetClientClassName(type.Namespace);
                string factoryName = GetClientFactoryClassName(type.Namespace);
                string interfaceName = type.Namespace + ".IWcfApi";

                Type[] serviceTypes = type.Assembly.GetTypes();
                string interfaceSource = string.Empty;
                List<MethodInfo[]> listMehtodInfo = new List<MethodInfo[]>();
                if (config.Wrapper)
                {
                    interfaceSource = WcfClientGenerator.GenerateInterfaceTypeSource(serviceTypes, type.Namespace);
                    listMehtodInfo = WcfClientGenerator.GetListMethod(serviceTypes, false);
                }
                else
                {
                    listMehtodInfo = WcfClientGenerator.GetListMethod(serviceTypes, true);
                }

                string clientSource = WcfClientGenerator.GenerateWcfClientSource(listMehtodInfo, clientName, interfaceName, config.Wrapper);
                string factorySource = WcfClientGenerator.GenerateWcfFactorySource(listMehtodInfo, clientName, factoryName, interfaceName, config.Wrapper, config);

                OutputFileGeneration(config, interfaceName, clientSource, factorySource);
                listSourceCode.Add(interfaceSource);
                listSourceCode.Add(clientSource);
                listSourceCode.Add(factorySource);

                Assembly ass = AssemblyGeneratorHelper.GenerateAssembly(GetImplementAssemblyName(), listSourceCode.ToArray(), listLocation.ToArray());
                Type tmpFactoryType = ass.GetType(NameSpace + "." + factoryName);
                Type tmpInterfaceType;
                if (config.Wrapper)
                    tmpInterfaceType = ass.GetType(interfaceName, true);
                else
                    tmpInterfaceType = type;
                listFactoryType.Add(tmpFactoryType);
                listInterfaceType.Add(tmpInterfaceType);
                listCompiledServiceName.Add(service.Name);
            }

            if (listCompiledServiceName.Count > 0)
            {
                for (int i = 0; i < listCompiledServiceName.Count; i++)
                {
                    WcfClientConfig config = ESBClientSettingsManager.Instance.GetWcfClientConfigDictionary().SingleOrDefault(c => c.Key == listCompiledServiceName[i]).Value;
                    if (config != null)
                    {
                        ConstructorInfo ci = listFactoryType[i].GetConstructor(new Type[] { typeof(WcfClientConfig) });
                        object obj = GetProtocolObject(ci, config);
                        lock (dictTypeConstructor)
                        {
                            dictTypeConstructor[listInterfaceType[i]] = ci;
                        }
                        dictServiceObject.Add(listCompiledServiceName[i], obj);
                    }
                }
            }
            object[] objs = new object[clientSettings.Services.Count];
            for (int i = 0; i < objs.Length; i++)
            {
                dictServiceObject.TryGetValue(clientSettings.Services[i].Name, out objs[i]);
            }
            return objs;
        }

        private static void OutputFileGeneration(WcfClientConfig config, string fullName, string clientSource, string factorySource)
        {
            if (config.FileGeneration.Enabled)
            {
                try
                {
                    string tmp = config.FileGeneration.SourceFileTemplate.Replace("{type}", "{0}");
                    System.IO.File.WriteAllText(string.Format(tmp, fullName), clientSource);

                    tmp = config.FileGeneration.SourceFactoryFileTemplate.Replace("{type}", "{0}");
                    System.IO.File.WriteAllText(string.Format(tmp, fullName), factorySource);
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Logger.Error("WcfClientGeneratorManager", ex);
                }
            }
        }

        private static object GetProtocolObject(ConstructorInfo ci, WcfClientConfig config)
        {
            object obj;
            try
            {
                obj = ci.Invoke(new object[] { config });
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Info(config.Name + "：" + ex.Message);
                throw new Exception("Invoke Errors '" + config.Name);
            }
            return obj;
        }
    }
}