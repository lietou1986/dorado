using Dorado.Core.GlobalTimer;
using Dorado.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

namespace Dorado.Core.Cache.StorageStrategys
{
    /// <summary>
    /// 以AspNetCache作存储的缓存项存储策略
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public class AspNetStorageStrategy<TKey, TValue> : IStorageStrategyEx<TKey, TValue>
        where TValue : class
    {
        private const string RegionName = "$$Dorado$$";

        // AspNetCache object does not have a ContainsKey() method:
        // Therefore we put a special string into cache if value is null,
        // otherwise our 'Contains()' would always return false,
        // which is bad if we intentionally wanted to save NULL values.
        private const string FakeNull = "__[NULL]__";

        public IEnumerable<KeyValuePair<TKey, CacheItem<TKey, TValue>>> Entries
        {
            get
            {
                if (HttpRuntime.Cache == null)
                    return Enumerable.Empty<KeyValuePair<TKey, CacheItem<TKey, TValue>>>();

                return from entry in HttpRuntime.Cache.Cast<DictionaryEntry>()
                       let key = entry.Key.ToString()
                       where key.StartsWith(RegionName)
                       select new KeyValuePair<TKey, CacheItem<TKey, TValue>>(
                          (TKey)(object)key.Substring(RegionName.Length),
                           entry.Value as CacheItem<TKey, TValue>);
            }
        }

        public static string BuildKey(string key)
        {
            return key.HasValue() ? RegionName + key : null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AspNetStorageStrategy()
        {
            _taskHandle = Global.GlobalTimer.Add(TimeSpan.FromSeconds(30), new TaskFuncAdapter(_CheckExpired), false, true);
        }

        #region IStorageStrategy<TKey,TValue> Members

        private readonly RwLocker _locker = new RwLocker();

        /// <summary>
        /// 添加一个缓存项
        /// </summary>
        /// <param name="cacheItem">缓存项</param>
        public void Add(CacheItem<TKey, TValue> cacheItem)
        {
            if (HttpRuntime.Cache == null)
                return;

            Contract.Requires(cacheItem != null);

            using (_locker.Write())
            {
                HttpRuntime.Cache.Insert(BuildKey(cacheItem.Key.ToString()), cacheItem);
            }
        }

        /// <summary>
        /// 批量添加缓存项
        /// </summary>
        /// <param name="cacheItems">缓存项</param>
        public void AddRange(IEnumerable<CacheItem<TKey, TValue>> cacheItems)
        {
            if (HttpRuntime.Cache == null)
                return;

            Contract.Requires(cacheItems != null);

            using (_locker.Write())
            {
                foreach (var item in cacheItems)
                {
                    HttpRuntime.Cache.Insert(BuildKey(item.Key.ToString()), item);
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
            if (HttpRuntime.Cache == null)
                return null;

            Contract.Requires(key != null);

            using (_locker.Read())
            {
                var value = HttpRuntime.Cache.Get(BuildKey(key.ToString()));

                if (value.Equals(FakeNull))
                    return null;

                return value as CacheItem<TKey, TValue>;
            }
        }

        public CacheItem<TKey, TValue>[] GetRange(IEnumerable<TKey> keys)
        {
            if (HttpRuntime.Cache == null)
                return null;

            Contract.Requires(keys != null);

            List<CacheItem<TKey, TValue>> result = new List<CacheItem<TKey, TValue>>();

            using (_locker.Read())
            {
                foreach (TKey key in keys)
                {
                    CacheItem<TKey, TValue> cacheItem;

                    var value = HttpRuntime.Cache.Get(BuildKey(key.ToString()));

                    if (value.Equals(FakeNull))
                        cacheItem = null;
                    else
                        cacheItem = value as CacheItem<TKey, TValue>;
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
                return Entries.Select(item => item.Key).ToArray();
            }
        }

        /// <summary>
        /// 清除所有缓存记录
        /// </summary>
        public void Clear()
        {
            using (_locker.Read())
            {
                Entries.ForEach(n => HttpRuntime.Cache.Remove(BuildKey(n.Key.ToString())));
            }
        }

        /// <summary>
        /// 删除一个缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        public void Remove(TKey key)
        {
            if (HttpRuntime.Cache == null)
                return;

            Contract.Requires(key != null);

            using (_locker.Write())
            {
                HttpRuntime.Cache.Remove(BuildKey(key.ToString()));
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
                    HttpRuntime.Cache.Remove(BuildKey(key.ToString()));
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

        ~AspNetStorageStrategy()
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
                expiredKeys.AddRange(from item in Entries where item.Value.ExpireDependency.HasExpired() select item.Key);
            }

            if (expiredKeys.Count > 0)
                RemoveRange(expiredKeys);
        }

        #endregion ICacheExpireCheckSupported Members
    }
}