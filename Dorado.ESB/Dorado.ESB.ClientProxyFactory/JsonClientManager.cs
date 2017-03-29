using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.ESB.ClientProxyFactory.Proxy;
using System;
using System.Collections.Generic;

namespace Dorado.ESB.ClientProxyFactory
{
    public class JsonClientManager
    {
        private JsonClientManager()
        {
        }

        private static JsonClientManager instance = new JsonClientManager();

        public static JsonClientManager Instance
        {
            get
            {
                return instance;
            }
        }

        private object tableLock = new object();
        private Dictionary<Type, Entry> clientTable = new Dictionary<Type, Entry>();

        private class Entry
        {
            private object entryLock = new object();
            private Type type;
            private object factory;
            private bool isSet = false;

            public Entry(Type t, object factory)
            {
                this.type = t;
                this.factory = factory;
            }

            public Entry(Type t)
            {
                this.type = t;
            }

            public object Factory
            {
                get
                {
                    //first level check, prevent from locking
                    if (isSet) return factory;
                    lock (entryLock)
                    {
                        //second level, prevent from reentry
                        if (isSet) return factory;
                        try
                        {
                            object[] objs = JsonClientGenerator.GenerateProxyFactories(type.FullName + ".Proxy", new Type[] { type });
                            if (objs != null)
                                factory = objs[0];
                        }
                        catch (Exception ex)
                        {
                            LoggerWrapper.Logger.Error("JsonClientManager", ex);
                        }
                        isSet = true;
                        return factory;
                    }
                }
            }
        }

        public IJsonClientFactory<T> GetClientFactory<T>()
        {
            Entry entry;
            Type t = typeof(T);
            lock (tableLock)
            {
                if (!clientTable.TryGetValue(t, out entry))
                {
                    entry = new Entry(t);
                    clientTable.Add(t, entry);
                }
            }
            return entry.Factory as IJsonClientFactory<T>;
        }

        public T GetJsonProtocolObject<T>(string baseUrl)
        {
            IJsonClientFactory<T> jsonFactory = GetClientFactory<T>();
            return jsonFactory.GetJsonProtocolObject(baseUrl);
        }
    }
}