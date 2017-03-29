using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Dorado.ESB.Extensions.Behaviors
{
    internal class PoolingInstanceProvider : IInstanceProvider
    {
        private const int idleTimeout = 5 * 60 * 1000;     // 5 minutes

        #region Private Fields

        private int minPoolSize;

        private Type instanceType;

        private Stack<object> pool;

        private object poolLock = new object();

        private int activeObjectsCount;

        private Timer idleTimer;

        #endregion Private Fields

        public PoolingInstanceProvider(Type instanceType, int minPoolSize)
        {
            this.minPoolSize = minPoolSize;
            this.instanceType = instanceType;

            pool = new Stack<object>();
            activeObjectsCount = 0;

            idleTimer = new Timer(idleTimeout);
            idleTimer.Elapsed += new System.Timers.ElapsedEventHandler(idleTimer_Elapsed);

            Initialize();
        }

        #region IInstanceProvider Members

        object IInstanceProvider.GetInstance(InstanceContext instanceContext)
        {
            return ((IInstanceProvider)this).GetInstance(instanceContext, null);
        }

        object IInstanceProvider.GetInstance(InstanceContext instanceContext, Message message)
        {
            object obj = null;

            lock (poolLock)
            {
                if (pool.Count > 0)
                {
                    obj = pool.Pop();
                }
                else
                {
                    obj = CreateNewPoolObject();
                }
                activeObjectsCount++;
            }

            idleTimer.Stop();

            return obj;
        }

        void IInstanceProvider.ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            lock (poolLock)
            {
                pool.Push(instance);
                activeObjectsCount--;

                if (activeObjectsCount == 0)
                    idleTimer.Start();
            }
        }

        #endregion IInstanceProvider Members

        private void Initialize()
        {
            for (int i = 0; i < minPoolSize; i++)
            {
                pool.Push(CreateNewPoolObject());
            }
        }

        private object CreateNewPoolObject()
        {
            return Activator.CreateInstance(instanceType);
        }

        private void idleTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            idleTimer.Stop();

            lock (poolLock)
            {
                if (activeObjectsCount == 0)
                {
                    while (pool.Count > minPoolSize)
                    {
                        object removedItem = pool.Pop();

                        if (removedItem is IDisposable)
                        {
                            ((IDisposable)removedItem).Dispose();
                        }
                    }
                }
            }
        }
    }
}