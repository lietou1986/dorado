using System;
using System.Collections.Generic;

namespace Dorado.Core.Cache.StorageStrategys
{
    /// <summary>
    /// 不存储
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public class NullStorageStrategy<TKey, TValue> : IStorageStrategyEx<TKey, TValue>
        where TValue : class
    {
        #region IStorageStrategy<TKey,TValue> Members

        /// <summary>
        /// 添加一个缓存项
        /// </summary>
        /// <param name="cacheItem">缓存项</param>
        public void Add(CacheItem<TKey, TValue> cacheItem)
        {
        }

        /// <summary>
        /// 批量添加缓存项
        /// </summary>
        /// <param name="cacheItems">缓存项</param>
        public void AddRange(IEnumerable<CacheItem<TKey, TValue>> cacheItems)
        {
        }

        /// <summary>
        /// 获取一个缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public CacheItem<TKey, TValue> Get(TKey key)
        {
            return null;
        }

        public CacheItem<TKey, TValue>[] GetRange(IEnumerable<TKey> keys)
        {
            return null;
        }

        /// <summary>
        /// 获取全部缓存项
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TKey> GetAllKeys()
        {
            return null;
        }

        public void Clear()
        {
        }

        /// <summary>
        /// 删除一个缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        public void Remove(TKey key)
        {
        }

        /// <summary>
        /// 批量删除缓存项
        /// </summary>
        /// <param name="keys">缓存键</param>
        public void RemoveRange(IEnumerable<TKey> keys)
        {
        }

        #endregion IStorageStrategy<TKey,TValue> Members

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        ~NullStorageStrategy()
        {
            Dispose();
        }

        #endregion IDisposable Members

        #region ICacheExpireCheckSupported Members

        /// <summary>
        /// 检查缓存过期
        /// </summary>
        private void _CheckExpired()
        {
        }

        #endregion ICacheExpireCheckSupported Members
    }
}