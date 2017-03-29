using Dorado.DataExpress.Resources;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Dorado.DataExpress.Utility
{
    public class BasePool<T> : IPool<T>, IDisposable where T : class
    {
        public readonly object SyncRoot = new object();
        private Timer _idleTmr;
        private Timer _recycelTmr;
        protected int _activeCount;
        private bool _idleCheck;
        private int _idleCheckInterval = 1000;
        protected int _initSize;
        protected int _maxActive = 10;
        protected int _maxIdle = 1000;
        protected int _maxTimeout = 10000;
        protected int _maxWait;
        private bool _timeoutCheck;
        private bool _isDisposing;

        public int CurrentSize
        {
            get
            {
                return this.IdleCount + this.ActiveCount;
            }
        }

        public virtual int MaxTimeout
        {
            get
            {
                return this._maxTimeout;
            }
            set
            {
                this._maxTimeout = value;
            }
        }

        public bool TimeoutCheck
        {
            get
            {
                return this._timeoutCheck;
            }
            set
            {
                this._timeoutCheck = value;
            }
        }

        public bool IdleCheck
        {
            get
            {
                return this._idleCheck;
            }
            set
            {
                this._idleCheck = value;
            }
        }

        public int IdleCheckInterval
        {
            get
            {
                return this._idleCheckInterval;
            }
            set
            {
                this._idleCheckInterval = value;
            }
        }

        public int InitSize
        {
            get
            {
                return this._initSize;
            }
        }

        public virtual int IdleCount
        {
            get
            {
                object syncRoot;
                Monitor.Enter(syncRoot = this.SyncRoot);
                int count;
                try
                {
                    count = this.InnerObjects.Count;
                }
                finally
                {
                    Monitor.Exit(syncRoot);
                }
                return count;
            }
        }

        public virtual int ActiveCount
        {
            get
            {
                object syncRoot;
                Monitor.Enter(syncRoot = this.SyncRoot);
                int activeCount;
                try
                {
                    activeCount = this._activeCount;
                }
                finally
                {
                    Monitor.Exit(syncRoot);
                }
                return activeCount;
            }
        }

        public virtual int MaxActive
        {
            get
            {
                return this._maxActive;
            }
        }

        public virtual int MaxWait
        {
            get
            {
                return this._maxWait;
            }
        }

        public int MaxIdle
        {
            get
            {
                return this._maxIdle;
            }
        }

        public LinkedList<T> InnerObjects
        {
            get;
            protected set;
        }

        public LinkedList<T> OutObjects
        {
            get;
            protected set;
        }

        public virtual T Obtain()
        {
            T obj = default(T);
            object syncRoot;
            Monitor.Enter(syncRoot = this.SyncRoot);
            try
            {
                LinkedListNode<T> node = this.InnerObjects.Last;
                if (node == null)
                {
                    if (this._activeCount < this._maxActive)
                    {
                        obj = this.CreateInstance();
                    }
                }
                else
                {
                    obj = node.Value;
                    this.InnerObjects.RemoveLast();
                }
                if (obj != null)
                {
                    this._activeCount++;
                    this.OutObjects.AddFirst(obj);
                    T result = obj;
                    return result;
                }
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
            for (long waitCount = 0L; waitCount < (long)this.MaxWait; waitCount += 10L)
            {
                Thread.Sleep(10);
                object syncRoot2;
                Monitor.Enter(syncRoot2 = this.SyncRoot);
                try
                {
                    LinkedListNode<T> node2 = this.InnerObjects.Last;
                    if (node2 == null)
                    {
                        if (this._activeCount < this._maxActive)
                        {
                            obj = this.CreateInstance();
                        }
                    }
                    else
                    {
                        obj = node2.Value;
                        this.InnerObjects.RemoveLast();
                    }
                    if (obj != null)
                    {
                        this._activeCount++;
                        this.OutObjects.AddLast(obj);
                        T result = obj;
                        return result;
                    }
                }
                finally
                {
                    Monitor.Exit(syncRoot2);
                }
            }
            throw new ObtainTimeoutException(string.Format(DataExpressResources.ObtainTimeoutDescription, this.MaxActive, this.MaxWait));
        }

        public virtual void Return(T obj)
        {
            if (obj == null)
            {
                return;
            }
            object syncRoot;
            Monitor.Enter(syncRoot = this.SyncRoot);
            try
            {
                if (this.OutObjects.Contains(obj))
                {
                    this.OutObjects.Remove(obj);
                }
                this.InnerObjects.AddFirst(obj);
                this._activeCount--;
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
        }

        public virtual void Invalidate(T obj)
        {
            this.Return(obj);
        }

        public virtual void Clear()
        {
            object syncRoot;
            Monitor.Enter(syncRoot = this.SyncRoot);
            try
            {
                for (LinkedListNode<T> node = this.InnerObjects.First; node != null; node = node.Next)
                {
                    T obj = node.Value;
                    if (obj != null)
                    {
                        this.DestroyInstance(obj);
                    }
                }
                if (this._timeoutCheck)
                {
                    for (LinkedListNode<T> node = this.OutObjects.First; node != null; node = node.Next)
                    {
                        T obj2 = node.Value;
                        if (obj2 != null)
                        {
                            this.DestroyInstance(obj2);
                        }
                    }
                }
                this.InnerObjects.Clear();
                this.OutObjects.Clear();
                this._activeCount = 0;
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
        }

        public virtual void Close()
        {
            if (this._idleTmr != null)
            {
                try
                {
                    this._idleTmr.Dispose();
                }
                finally
                {
                    this._idleTmr = null;
                }
            }
            if (this._recycelTmr != null)
            {
                try
                {
                    this._recycelTmr.Dispose();
                }
                finally
                {
                    this._recycelTmr = null;
                }
            }
            this.Clear();
        }

        public virtual T CreateInstance()
        {
            return Activator.CreateInstance<T>();
        }

        public virtual void DestroyInstance(T obj)
        {
            if (obj != null && obj is IDisposable)
            {
                ((IDisposable)obj).Dispose();
            }
        }

        public virtual void Evict(object obj)
        {
            int arg_06_0 = this.IdleCount;
            object syncRoot;
            Monitor.Enter(syncRoot = this.SyncRoot);
            try
            {
                LinkedListNode<T> node = this.InnerObjects.First;
                if (node != null)
                {
                    while (node != null)
                    {
                        if (this.NeedEvict(node.Value))
                        {
                            this.InnerObjects.Remove(node);
                            this.DestroyInstance(node.Value);
                        }
                        node = node.Next;
                    }
                }
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
        }

        public virtual void Recycle(object obj)
        {
            object syncRoot;
            Monitor.Enter(syncRoot = this.SyncRoot);
            try
            {
                if (this.OutObjects != null)
                {
                    if (this.OutObjects.Count != 0)
                    {
                        LinkedListNode<T> node = this.OutObjects.First;
                        if (node != null)
                        {
                            while (node != null)
                            {
                                if (this.NeedRecycle(node.Value))
                                {
                                    this.OutObjects.Remove(node);
                                    this.Return(node.Value);
                                }
                                node = node.Next;
                            }
                        }
                    }
                }
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
        }

        public virtual bool NeedEvict(T obj)
        {
            return false;
        }

        public virtual bool NeedRecycle(T obj)
        {
            return false;
        }

        public void Dispose()
        {
            object syncRoot;
            Monitor.Enter(syncRoot = this.SyncRoot);
            try
            {
                if (!this._isDisposing)
                {
                    this._isDisposing = true;
                    this.Close();
                    if (this._idleTmr != null)
                    {
                        this._idleTmr.Dispose();
                    }
                    if (this._recycelTmr != null)
                    {
                        this._recycelTmr.Dispose();
                    }
                }
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
        }

        public virtual void Init()
        {
            object syncRoot;
            Monitor.Enter(syncRoot = this.SyncRoot);
            try
            {
                this.InnerObjects = new LinkedList<T>();
                this.OutObjects = new LinkedList<T>();
                for (int i = 0; i < this._initSize; i++)
                {
                    T obj = this.CreateInstance();
                    this.Return(obj);
                }
                this._activeCount = 0;
                if (this._idleCheck)
                {
                    this._idleTmr = new Timer(new TimerCallback(this.Evict), this, this._maxIdle, this._maxIdle);
                }
                if (this._timeoutCheck)
                {
                    this._recycelTmr = new Timer(new TimerCallback(this.Recycle), this, this._maxTimeout, this._maxTimeout);
                }
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
        }

        ~BasePool()
        {
            this.Dispose();
        }
    }
}