using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.ESB.ClientProxyFactory.Config;
using Dorado.ESB.ClientProxyFactory.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dorado.ESB.ClientProxyFactory
{
    public class WcfClientManager
    {
        private WcfClientManager()
        {
            ESBClientSettings.ConfigChanged += new EventHandler(ESBClientSettings_ConfigChanged);
            ReloadConfig(ESBClientSettings.Instance);
        }

        private static WcfClientManager instance = new WcfClientManager();

        public static WcfClientManager Instance
        {
            get
            {
                return instance;
            }
        }

        private Dictionary<string, object> clientDictionaryCache = new Dictionary<string, object>();

        public void ReloadConfig(ESBClientSettings clientSettings)
        {
            object[] objs = WcfClientGeneratorManager.GetProtocolObjects(clientSettings);
            Dictionary<string, object> tmpDictionary = new Dictionary<string, object>(clientSettings.Services.Count);
            for (int i = 0; i < clientSettings.Services.Count; i++)
                tmpDictionary.Add(clientSettings.Services[i].Name, objs[i]);
            clientDictionaryCache = tmpDictionary;
        }

        private void ESBClientSettings_ConfigChanged(object sender, EventArgs e)
        {
            LoggerWrapper.Logger.Info("Configuration file changes, the service stops, restart");
            StopServices();
            ESBClientSettingsManager.Instance.ResetWcfClientConfigDictionary();
            ReloadConfig(ESBClientSettings.Instance);
        }

        private void StopServices()
        {
            clientDictionaryCache.ToList().ForEach(delegate(KeyValuePair<string, object> keyValue)
            {
                if (keyValue.Value != null)
                {
                    try
                    {
                        keyValue.Value.GetType().InvokeMember("StopServices", BindingFlags.InvokeMethod, null, null, null);
                    }
                    catch (Exception ex)
                    {
                        LoggerWrapper.Logger.Error("Dorado.ESB.ClientProxyFactory", ex);
                    }
                }
            });
            clientDictionaryCache.Clear();
        }

        public T GetProtocolObject<T>() where T : class
        {
            object obj;
            if (clientDictionaryCache.TryGetValue(typeof(T).FullName, out obj))
                return (T)obj;
            else
                return null;
        }

        public T GetProtocolObject<T>(string serviceName) where T : class
        {
            object obj;
            if (clientDictionaryCache.TryGetValue(serviceName, out obj))
                return (T)obj;
            else
                return null;
        }
    }
}