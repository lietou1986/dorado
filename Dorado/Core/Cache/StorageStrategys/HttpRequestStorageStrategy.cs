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
    /// 以HttpRequest作存储的缓存项存储策略
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public class HttpRequestStorageStrategy<TKey, TValue> : IStorageStrategyEx<TKey, TValue>
        where TValue : class
    {
        private const string RegionName = "$$Dorado$$";

        protected IDictionary GetItems()
        {
            if (_context != null)
                return _context.Items;

            return null;
        }

        public IEnumerable<KeyValuePair<TKey, CacheItem<TKey, TValue>>> Entries
        {
            get
            {
                var items = GetItems();
                if (items == null)
                    yield break;

                var enumerator = items.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    string key = enumerator.Key as string;
                    if (key == null)
                        continue;
                    if (key.StartsWith(RegionName))
                    {
                        yield return new KeyValuePair<TKey, CacheItem<TKey, TValue>>((TKey)(object)key.Substring(RegionName.Length), enumerator.Value as CacheItem<TKey, TValue>);
                    }
                }
            }
        }

        public static string BuildKey(string key)
        {
            return key.HasValue() ? RegionName + key : null;
        }

        private readonly HttpContextBase _context;

        /// <summary>
        /// Constructor
        /// </summary>
        public HttpRequestStorageStrategy(HttpContextBase context)
        {
            this._context = context;
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
            Contract.Requires(cacheItem != null);

            using (_locker.Write())
            {
                var items = GetItems();
                if (items == null)
                    return;

                string key = BuildKey(cacheItem.Key.ToString());

                if (items.Contains(key))
                    items[key] = cacheItem;
                else
                    items.Add(key, cacheItem);
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
                var items = GetItems();
                if (items == null)
                    return;
                foreach (var cacheItem in cacheItems)
                {
                    string key = BuildKey(cacheItem.Key.ToString());

                    if (items.Contains(key))
                        items[key] = cacheItem;
                    else
                        items.Add(key, cacheItem);
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
                var items = GetItems();
                if (items == null)
                    return null;

                return items[BuildKey(key.ToString())] as CacheItem<TKey, TValue>;
            }
        }

        public CacheItem<TKey, TValue>[] GetRange(IEnumerable<TKey> keys)
        {
            Contract.Requires(keys != null);

            List<CacheItem<TKey, TValue>> result = new List<CacheItem<TKey, TValue>>();

            using (_locker.Read())
            {
                var items = GetItems();
                if (items == null)
                    return null;

                result.AddRange(keys.Select(key => items[BuildKey(key.ToString())]).Select(value => value as CacheItem<TKey, TValue>));
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
            using (_locker.Write())
            {
                var items = GetItems();
                if (items == null)
                    return;
                Entries.ForEach(n => items.Remove(BuildKey(n.Key.ToString())));
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
                var items = GetItems();
                if (items == null)
                    return;
                items.Remove(BuildKey(key.ToString()));
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
                var items = GetItems();
                if (items == null)
                    return;
                foreach (TKey key in keys)
                {
                    items.Remove(BuildKey(key.ToString()));
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

        ~HttpRequestStorageStrategy()
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