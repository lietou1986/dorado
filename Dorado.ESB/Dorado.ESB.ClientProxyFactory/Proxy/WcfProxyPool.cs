using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.ESB.ClientProxyFactory.Config;
using System;
using System.ServiceModel;

namespace Dorado.ESB.ClientProxyFactory.Proxy
{
    public class WcfProxyPool<T> where T : class
    {
        private IWcfClientFactory<T> factory;
        private ClientBase<T> singleProxy;
        private Pool<System.ServiceModel.ClientBase<T>> pool;
        private ChannelPool channelPool;

        private int PoolSize
        {
            get
            {
                return channelPool.PoolSize;
            }
        }

        private int WaitingTimeout
        {
            get
            {
                return channelPool.WaitingTimeout;
            }
        }

        public WcfProxyPool()
        {
        }

        ~WcfProxyPool()
        {
            ClosePool();
        }

        public void ClosePool()
        {
            if (singleProxy != null)
            {
                if (singleProxy.State == CommunicationState.Opened)
                {
                    singleProxy.Abort();
                }
            }

            int poolLen = pool.Count;
            for (int i = 0; i < poolLen; i++)
            {
                ClientBase<T> clientBase = pool.Take();
                if (clientBase.State == CommunicationState.Opened)
                {
                    clientBase.Abort();
                }
            }
            pool.Clear();
        }

        public WcfProxyPool(IWcfClientFactory<T> factory, ChannelPool channelPool)
        {
            this.factory = factory;
            this.channelPool = channelPool;
            InitializeWcfClient();
        }

        private void InitializeWcfClient()
        {
            lock (this)
            {
                try
                {
                    LoggerWrapper.Logger.Info("Creating WcfProxyPool...");

                    if (channelPool.PoolSize <= 1)
                    {
                        singleProxy = CreateProxy();
                    }
                    else
                    {
                        InitializePool();
                    }
                }
                catch (Exception exception)
                {
                    LoggerWrapper.Logger.Error("Create Pool", exception);
                }
            }
        }

        private void InitializePool()
        {
            LoggerWrapper.Logger.Info("Creating pool of size: " + PoolSize + ", waitingTimeout:" + WaitingTimeout + "\n");
            pool = new Pool<System.ServiceModel.ClientBase<T>>(PoolSize);
        }

        public System.ServiceModel.ClientBase<T> GetProxy()
        {
            System.ServiceModel.ClientBase<T> proxy = null;

            if (PoolSize <= 1)
            {
                if (singleProxy != null && singleProxy.State != CommunicationState.Opened)
                {
                    lock (singleProxy)
                    {
                        singleProxy.Abort();
                        singleProxy = CreateProxy();
                        proxy = singleProxy;
                    }
                }
                else
                {
                    if (singleProxy == null)
                    {
                        LoggerWrapper.Logger.Info("Creating new proxy.");
                        singleProxy = CreateProxy();
                    }

                    proxy = singleProxy;
                }
            }
            else
            {
                lock (pool)
                {
                    proxy = pool.Take();
                }

                if (proxy != null && proxy.State != CommunicationState.Opened)
                {
                    //to prevent the abort exceptions
                    proxy.Abort();
                    proxy = null;
                }

                // Create the proxy if not found from the pool.
                if (proxy == null)
                {
                    LoggerWrapper.Logger.Info("Creating new proxy.");
                    proxy = CreateProxy();
                }
            }
            return proxy;
        }

        private System.ServiceModel.ClientBase<T> CreateProxy()
        {
            System.ServiceModel.ClientBase<T> apiWcf = factory.CreateProxy();
            try
            {
                apiWcf.Open();
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("Create Proxy", ex);
            }

            return apiWcf;
        }

        //------------------Add By jiangsong 2009-11-27 multiAddressHeartbeat
        public void ClearPool(string remoteAddress)
        {
            try
            {
                pool.Clear();
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("Clear Pool Fail：" + remoteAddress, ex);
                throw;
            }
        }

        public int PoolCount()
        {
            return pool.Count;
        }

        public void CheckInPool(System.ServiceModel.ClientBase<T> proxy)
        {
            if (PoolSize <= 1)
            {
            }
            else
            {
                // Trying to put the proxy back to the pool
                bool shouldClose = false;
                lock (pool)
                {
                    shouldClose = !pool.Return(proxy);
                }

                //Close the proxy if it's not put back into pool
                if (shouldClose)
                {
                    try
                    {
                        proxy.Close();
                    }
                    catch (Exception ex)
                    {
                        proxy.Abort();
                        LoggerWrapper.Logger.Error("Pool Error", ex);
                        throw;
                    }
                }
            }
        }
    }

    internal class Pool<T> where T : class
    {
        private T[] items;
        private int count;

        public Pool(int maxCount)
        {
            items = new T[maxCount];
        }

        public int Count
        {
            get { return count; }
        }

        public bool Add(T item)
        {
            if (count < items.Length)
            {
                items[count++] = item;
                return true;
            }
            else
            {
                return false;
            }
        }

        public T Take()
        {
            if (count > 0)
            {
                T item = items[--count];
                items[count] = null;
                return item;
            }
            else
            {
                return null;
            }
        }

        public bool Return(T item)
        {
            if (count < items.Length)
            {
                items[count++] = item;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < count; i++)
                items[i] = null;
            count = 0;
        }
    }
}