using Dorado.Core.GlobalTimer;
using Dorado.Core.ObjectPool;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Dorado.Core
{
    /// <summary>
    /// 对象缓存池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> : IDisposable
        where T : class
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strategy">对象池策略</param>
        /// <param name="creator">创建器</param>
        public ObjectPool(Func<T> creator, IObjectPoolStrategy strategy)
        {
            Contract.Requires(creator != null && strategy != null);

            _Creator = creator;
            _Stragety = strategy;

            _GlobalTimerTaskHandle = Global.GlobalTimer.Add(TimeSpan.FromSeconds(30), new TaskFuncAdapter(_Trim), false);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">创建器</param>
        /// <param name="maxCount">对象池策略</param>
        public ObjectPool(Func<T> creator, int maxCount)
            : this(creator, new ObjectPoolStrategy(maxCount))
        {
        }

        private readonly Queue<T> _Queue = new Queue<T>();
        private readonly IObjectPoolStrategy _Stragety;
        private readonly Func<T> _Creator;
        private readonly IGlobalTimerTaskHandle _GlobalTimerTaskHandle;

        /// <summary>
        /// 申请一个对象
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public T Accquire(TimeSpan timeout)
        {
            return Accquire((int)timeout.TotalMilliseconds);
        }

        /// <summary>
        /// 申请一个对象
        /// </summary>
        /// <param name="timeout">当对象列表为空时，最多等待的时间</param>
        /// <returns></returns>
        public T Accquire(int timeoutMillseconds = -1)
        {
            if (!_Stragety.Wait(1, timeoutMillseconds))
                return null;

            T result = null;
            try
            {
                lock (_Queue)
                {
                    if (_Queue.Count > 0)
                        result = _Queue.Dequeue();
                }

                if (result == null)
                {
                    result = _Creator();
                    Contract.Assert(result != null);
                    _Stragety.AccquireNotify(1, 1);
                }
                else
                    _Stragety.AccquireNotify(1, 0);

                return result;
            }
            catch
            {
                _Stragety.AccquireNotify(0, 0);
                throw;
            }
        }

        /// <summary>
        /// 申请一个对象，使用using语法可以释放资源
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="timeoutMillseconds"></param>
        /// <returns></returns>
        public IDisposable Accquire(out T obj, int timeoutMillseconds = -1)
        {
            obj = Accquire(timeoutMillseconds);
            return new ReleaseHelper(this, obj);
        }

        /// <summary>
        /// 申请一个对象，使用using语法可以释放资源
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public IDisposable Accquire(out T obj, TimeSpan timeout)
        {
            obj = Accquire(timeout);
            return new ReleaseHelper(this, obj);
        }

        /// <summary>
        /// 批量申请对象
        /// </summary>
        /// <param name="count"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public T[] AccquireRange(int count, TimeSpan timeout)
        {
            return AccquireRange(count, (int)timeout.TotalMilliseconds);
        }

        /// <summary>
        /// 批量申请对象
        /// </summary>
        /// <returns></returns>
        public T[] AccquireRange(int count, int timeoutMillseconds = -1)
        {
            Contract.Requires(count > 0);

            if (!_Stragety.Wait(count, timeoutMillseconds))
                return null;

            T[] objs = new T[count];
            int newCount = 0, index = 0;

            try
            {
                lock (_Queue)
                {
                    while (index < count)
                    {
                        if (_Queue.Count > 0)
                            objs[index++] = _Queue.Dequeue();
                        else
                        {
                            objs[index++] = _Creator();
                            newCount++;
                        }
                    }
                }

                _Stragety.AccquireNotify(count, newCount);
                return objs;
            }
            catch
            {
                _Stragety.AccquireNotify(0, 0);
                throw;
            }
        }

        /// <summary>
        /// 批量申请对象，使用using语法可以释放对象
        /// </summary>
        /// <param name="count"></param>
        /// <param name="objs"></param>
        /// <param name="timeoutMillseconds"></param>
        /// <returns></returns>
        public IDisposable AccquireRange(int count, out T[] objs, int timeoutMillseconds = -1)
        {
            objs = AccquireRange(count, timeoutMillseconds);
            return new ReleaseRangeHelper(this, objs);
        }

        /// <summary>
        /// 批量申请对象，使用using语法可以释放对象
        /// </summary>
        /// <param name="count"></param>
        /// <param name="objs"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public IDisposable AccquireRange(int count, out T[] objs, TimeSpan timeout)
        {
            objs = AccquireRange(count, timeout);
            return new ReleaseRangeHelper(this, objs);
        }

        /// <summary>
        /// 释放一个对象
        /// </summary>
        /// <param name="obj"></param>
        public void Release(T obj)
        {
            Contract.Requires(obj != null);

            lock (_Queue)
            {
                _Queue.Enqueue(obj);
            }

            _Stragety.ReleaseNotify(1);
        }

        /// <summary>
        /// 批量释放对象
        /// </summary>
        /// <param name="objs"></param>
        public void Release(IEnumerable<T> objs)
        {
            Contract.Requires(objs != null);

            int count = 0;
            lock (_Queue)
            {
                foreach (T obj in objs)
                {
                    _Queue.Enqueue(obj);
                    count++;
                }
            }

            if (count > 0)
                _Stragety.ReleaseNotify(count);
        }

        private void _Trim()
        {
            lock (_Queue)
            {
                int maxCount = Math.Max(0, _Stragety.TrimNotify());
                while (_Queue.Count > maxCount)
                {
                    _Queue.Dequeue();
                }
            }
        }

        #region IDisposable Members

        ~ObjectPool()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _GlobalTimerTaskHandle.Dispose();
        }

        #endregion IDisposable Members

        #region Class ReleaseRangeHelper ...

        private class ReleaseRangeHelper : IDisposable
        {
            public ReleaseRangeHelper(ObjectPool<T> owner, T[] objs)
            {
                _Owner = owner;
                _Objs = objs;
            }

            private ObjectPool<T> _Owner;
            private T[] _Objs;

            #region IDisposable Members

            public void Dispose()
            {
                _Owner.Release(_Objs);
            }

            #endregion IDisposable Members
        }

        #endregion Class ReleaseRangeHelper ...

        #region Class ReleaseHelper ...

        private class ReleaseHelper : IDisposable
        {
            public ReleaseHelper(ObjectPool<T> owner, T obj)
            {
                _Owner = owner;
                _Obj = obj;
            }

            private ObjectPool<T> _Owner;
            private T _Obj;

            #region IDisposable Members

            public void Dispose()
            {
                _Owner.Release(_Obj);
            }

            #endregion IDisposable Members
        }

        #endregion Class ReleaseHelper ...
    }
}