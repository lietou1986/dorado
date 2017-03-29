using System;

namespace Dorado.Core.Cache
{
    /// <summary>
    /// 缓存的存储策略
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public interface IStorageStrategy<TKey, TValue> : IDisposable
        where TValue : class
    {
        /// <summary>
        /// 添加缓存项
        /// </summary>
        /// <param name="cacheItem"></param>
        void Add(CacheItem<TKey, TValue> cacheItem);

        /// <summary>
        /// 获取一个缓存项
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>缓存项</returns>
        CacheItem<TKey, TValue> Get(TKey key);

        /// <summary>
        /// 删除缓存项
        /// </summary>
        /// <param name="key">键</param>
        void Remove(TKey key);
    }
}