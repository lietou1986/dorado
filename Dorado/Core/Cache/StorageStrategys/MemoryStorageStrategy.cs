using Dorado.Core.GlobalTimer;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Dorado.Core.Cache.StorageStrategys
{
    /// <summary>
    /// 以内存作存储的缓存项存储策略
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public class MemoryStorageStrategy<TKey, TValue> : IStorageStrategyEx<TKey, TValue>
        where TValue : class
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="capacity">初始容量</param>
        public MemoryStorageStrategy(int capacity = 0)
        {
            Contract.Requires(capacity >= 0);

            _dict = new Dictionary<TKey, CacheItem<TKey, TValue>>(capacity);
            _taskHandle = Global.GlobalTimer.Add(TimeSpan.FromSeconds(30), new TaskFuncAdapter(_CheckExpired), false, true);
        }

        #region IStorageStrategy<TKey,TValue> Members

        private readonly Dictionary<TKey, CacheItem<TKey, TValue>> _dict;
        private readonly RwLocker _locker = new RwLocker();

        /// <summary>
        /// 添加一个缓存项
        /// </summary>
        /// <param name="cacheItem">缓存项</param>
        public void Add(CacheItem<TKey, TValue> cacheItem)
        {
            Contract.Requires(cacheItem != null);

            using (_locker.Write())
            {
                _dict[cacheItem.Key] = cacheItem;
            }
        }

        /// <summary>
        /// 批量添加缓存项
        /// </summary>
        /// <param name="cacheItems">缓存项</param>
        public void AddRange(IEnumerable<CacheItem<TKey, TValue>> cacheItems)
        {
            Contract.Requires(cacheItems != null);

            using (_locker.Write())
            {
                foreach (var item in cacheItems)
                {
                    _dict[item.Key] = item;
                }
            }
        }

        /// <summary>
        /// 获取一个缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public CacheItem<TKey, TValue> Get(TKey key)
        {
            Contract.Requires(key != null);

            using (_locker.Read())
            {
                CacheItem<TKey, TValue> cacheItem;
                _dict.TryGetValue(key, out cacheItem);
                return cacheItem;
            }
        }

        public CacheItem<TKey, TValue>[] GetRange(IEnumerable<TKey> keys)
        {
            Contract.Requires(keys != null);

            List<CacheItem<TKey, TValue>> result = new List<CacheItem<TKey, TValue>>();

            using (_locker.Read())
            {
                foreach (TKey key in keys)
                {
                    CacheItem<TKey, TValue> cacheItem;
                    if (_dict.TryGetValue(key, out cacheItem))
                        result.Add(cacheItem);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// 获取全部缓存项
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TKey> GetAllKeys()
        {
            using (_locker.Read())
            {
                return _dict.Keys.ToArray();
            }
        }

        /// <summary>
        /// 清除所有缓存记录
        /// </summary>
        public void Clear()
        {
            using (_locker.Write())
            {
                _dict.Clear();
            }
        }

        /// <summary>
        /// 删除一个缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        public void Remove(TKey key)
        {
            Contract.Requires(key != null);

            using (_locker.Write())
            {
                _dict.Remove(key);
            }
        }

        /// <summary>
        /// 批量删除缓存项
        /// </summary>
        /// <param name="keys">缓存键</param>
        public void RemoveRange(IEnumerable<TKey> keys)
        {
            Contract.Requires(keys != null);

            using (_locker.Write())
            {
                foreach (TKey key in keys)
                {
                    _dict.Remove(key);
                }
            }
        }

        #endregion IStorageStrategy<TKey,TValue> Members

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _taskHandle.Dispose();
        }

        ~MemoryStorageStrategy()
        {
            Dispose();
        }

        #endregion IDisposable Members

        #region ICacheExpireCheckSupported Members

        private readonly IGlobalTimerTaskHandle _taskHandle;

        /// <summary>
        /// 检查缓存过期
        /// </summary>
        private void _CheckExpired()
        {
            List<TKey> expiredKeys = new List<TKey>();

            using (_locker.Read())
            {
                expiredKeys.AddRange(from item in _dict.Values where item.ExpireDependency.HasExpired() select item.Key);
            }

            if (expiredKeys.Count > 0)
                RemoveRange(expiredKeys);
        }

        #endregion ICacheExpireCheckSupported Members
    }
}