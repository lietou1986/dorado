using System.Collections.Generic;

namespace Dorado.Core.Cache
{
    /// <summary>
    /// 缓存存储策略的扩展接口
    /// </summary>
    /// <typeparam name="TKey">缓存键类型</typeparam>
    /// <typeparam name="TValue">缓存值类型</typeparam>
    public interface IStorageStrategyEx<TKey, TValue> : IStorageStrategy<TKey, TValue>
        where TValue : class
    {
        /// <summary>
        /// 批量添加缓存项
        /// </summary>
        /// <param name="cacheItems">缓存项</param>
        void AddRange(IEnumerable<CacheItem<TKey, TValue>> cacheItems);

        /// <summary>
        /// 批量删除缓存项
        /// </summary>
        /// <param name="keys">键</param>
        void RemoveRange(IEnumerable<TKey> keys);

        /// <summary>
        /// 批量获取缓存项
        /// </summary>
        /// <param name="keys">缓存项</param>
        /// <returns></returns>
        CacheItem<TKey, TValue>[] GetRange(IEnumerable<TKey> keys);

        /// <summary>
        /// 获取全部的缓存键
        /// </summary>
        /// <returns></returns>
        IEnumerable<TKey> GetAllKeys();

        /// <summary>
        /// 删除所有缓存项
        /// </summary>
        void Clear();
    }
}