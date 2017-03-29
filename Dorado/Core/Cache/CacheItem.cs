namespace Dorado.Core.Cache
{
    /// <summary>
    /// 缓存存储项
    /// </summary>
    /// <typeparam name="TKey">缓存键类型</typeparam>
    /// <typeparam name="TValue">缓存值类型</typeparam>
    public class CacheItem<TKey, TValue>
        where TValue : class
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expireDependency">缓存过期方式</param>
        /// <param name="valueLoader">缓存值加载策略</param>
        internal CacheItem(TKey key, TValue value, ICacheExpireDependency expireDependency, ICacheValueLoader<TKey, TValue> valueLoader)
        {
            Key = key;
            Value = value;
            ExpireDependency = expireDependency;
            ValueLoader = valueLoader;
        }

        /// <summary>
        /// 缓存键
        /// </summary>
        public TKey Key { get; private set; }

        /// <summary>
        /// 缓存值
        /// </summary>
        public TValue Value { get; internal set; }

        /// <summary>
        /// 缓存过期方式
        /// </summary>
        public ICacheExpireDependency ExpireDependency { get; private set; }

        /// <summary>
        /// 缓存值的加载策略
        /// </summary>
        public ICacheValueLoader<TKey, TValue> ValueLoader { get; private set; }
    }
}