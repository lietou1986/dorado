using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Dorado.Core.Cache
{
    /// <summary>
    /// 缓存存储策略的包装器
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class CacheStorageStrategyExWrapper<TKey, TValue> : IStorageStrategyEx<TKey, TValue>
        where TValue : class
    {
        /// <summary>
        /// Constructors
        /// </summary>
        /// <param name="strategy"></param>
        public CacheStorageStrategyExWrapper(IStorageStrategy<TKey, TValue> strategy)
        {
            _strategy = strategy;
        }

        private readonly IStorageStrategy<TKey, TValue> _strategy;

        #region IStorageStrategyEx<TKey,TValue> Members

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="cacheItems"></param>
        public void AddRange(IEnumerable<CacheItem<TKey, TValue>> cacheItems)
        {
            Contract.Requires(cacheItems != null);

            foreach (CacheItem<TKey, TValue> item in cacheItems)
            {
                _strategy.Add(item);
            }
        }

        /// <summary>
        /// 批量获取
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public CacheItem<TKey, TValue>[] GetRange(IEnumerable<TKey> keys)
        {
            Contract.Requires(keys != null);

            List<CacheItem<TKey, TValue>> result = new List<CacheItem<TKey, TValue>>();
            foreach (TKey key in keys)
            {
                var item = Get(key);
                if (item != null)
                    result.Add(item);
            }

            return result.ToArray();
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="keys"></param>
        public void RemoveRange(IEnumerable<TKey> keys)
        {
            foreach (TKey key in keys)
            {
                _strategy.Remove(key);
            }
        }

        /// <summary>
        /// 获取所有缓存键
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TKey> GetAllKeys()
        {
            throw new NotSupportedException("不支持获取所有的缓存键");
        }

        public void Clear()
        {
            throw new NotSupportedException("不支持清除所有的缓存键");
        }

        #endregion IStorageStrategyEx<TKey,TValue> Members

        #region IStorageStrategy<TKey,TValue> Members

        public void Add(CacheItem<TKey, TValue> cacheItem)
        {
            _strategy.Add(cacheItem);
        }

        public CacheItem<TKey, TValue> Get(TKey key)
        {
            return _strategy.Get(key);
        }

        public void Remove(TKey key)
        {
            _strategy.Remove(key);
        }

        #endregion IStorageStrategy<TKey,TValue> Members

        #region IDisposable Members

        public void Dispose()
        {
            _strategy.Dispose();
        }

        #endregion IDisposable Members
    }
}