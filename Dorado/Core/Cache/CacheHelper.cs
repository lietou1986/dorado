using Dorado.Core.Cache.CacheExpireDependencies;
using System;
using System.Diagnostics.Contracts;

namespace Dorado.Core.Cache
{
    /// <summary>
    /// 缓存的工具函数
    /// </summary>
    public static class CacheHelper
    {
        /// <summary>
        /// 添加一项固定时间过期方式的缓存
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="cache">缓存</param>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expireTime">缓存过期时间</param>
        /// <param name="valueLoader">缓存值加载策略</param>
        public static void AddOfTermly<TKey, TValue>(this Cache<TKey, TValue> cache, TKey key, TValue value, DateTime expireTime, ICacheValueLoader<TKey, TValue> valueLoader = null)
            where TValue : class
        {
            Contract.Requires(cache != null);

            cache.Add(key, value, new TermlyCacheExpireDependency(expireTime), valueLoader);
        }

        /// <summary>
        /// 获取或添加一项固定时间过期方式的缓存
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="cache">缓存</param>
        /// <param name="key">键</param>
        /// <param name="expireTime">过期时间</param>
        /// <param name="valueLoader">缓存值加载器</param>
        public static TValue GetOrAddOfTermly<TKey, TValue>(this Cache<TKey, TValue> cache, TKey key, DateTime expireTime, ICacheValueLoader<TKey, TValue> valueLoader)
            where TValue : class
        {
            return GetOrAdd(cache, key, new TermlyCacheExpireDependency(expireTime), valueLoader);
        }

        /// <summary>
        /// 添加一项固定时间过期方式的缓存
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="cache">缓存</param>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expireTime">缓存过期时间</param>
        /// <param name="valueLoader">缓存值加载策略</param>
        public static void AddOfTermly<TKey, TValue>(this Cache<TKey, TValue> cache, TKey key, TValue value, DateTime expireTime, Func<TKey, TValue> valueLoader)
            where TValue : class
        {
            Contract.Requires(cache != null);

            cache.Add(key, value, new TermlyCacheExpireDependency(expireTime), _ConvertToValueLoader(valueLoader));
        }

        /// <summary>
        /// 获取或添加一项固定时间过期方式的缓存
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="cache">缓存</param>
        /// <param name="key">缓存键</param>
        /// <param name="expireTime">缓存过期时间</param>
        /// <param name="valueLoaderFunc">缓存值加载策略</param>
        public static TValue GetOrAddOfTermly<TKey, TValue>(this Cache<TKey, TValue> cache, TKey key, DateTime expireTime, Func<TKey, TValue> valueLoaderFunc)
            where TValue : class
        {
            return GetOrAdd(cache, key, new TermlyCacheExpireDependency(expireTime), valueLoaderFunc);
        }

        /// <summary>
        /// 添加一项相对时间过期方式的缓存
        /// </summary>
        /// <typeparam name="TKey">缓存键</typeparam>
        /// <typeparam name="TValue">缓存值</typeparam>
        /// <param name="cache">缓存</param>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="interval">缓存过期时间间隔</param>
        /// <param name="valueLoader">缓存值加载策略</param>
        public static void AddOfRelative<TKey, TValue>(this Cache<TKey, TValue> cache, TKey key, TValue value, TimeSpan interval, ICacheValueLoader<TKey, TValue> valueLoader = null)
            where TValue : class
        {
            Contract.Requires(cache != null);

            cache.Add(key, value, new RelativeCacheExpireDependency(interval), valueLoader);
        }

        /// <summary>
        /// 获取或添加一项相对时间过期方式的缓存
        /// </summary>
        /// <typeparam name="TKey">缓存键</typeparam>
        /// <typeparam name="TValue">缓存值</typeparam>
        /// <param name="cache">缓存</param>
        /// <param name="key">缓存键</param>
        /// <param name="interval">缓存过期时间间隔</param>
        /// <param name="valueLoader">缓存值加载策略</param>
        public static TValue GetOrAddOfRelative<TKey, TValue>(this Cache<TKey, TValue> cache, TKey key, TimeSpan interval, ICacheValueLoader<TKey, TValue> valueLoader)
            where TValue : class
        {
            return GetOrAdd(cache, key, new RelativeCacheExpireDependency(interval), valueLoader);
        }

        /// <summary>
        /// 添加一项相对时间过期方式的缓存
        /// </summary>
        /// <typeparam name="TKey">缓存键</typeparam>
        /// <typeparam name="TValue">缓存值</typeparam>
        /// <param name="cache">缓存</param>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="interval">缓存过期时间间隔</param>
        /// <param name="valueLoaderFunc">缓存值加载策略</param>
        public static void AddOfRelative<TKey, TValue>(this Cache<TKey, TValue> cache, TKey key, TValue value, TimeSpan interval, Func<TKey, TValue> valueLoaderFunc)
            where TValue : class
        {
            Contract.Requires(cache != null);

            cache.Add(key, value, new RelativeCacheExpireDependency(interval), _ConvertToValueLoader(valueLoaderFunc));
        }

        /// <summary>
        /// 获取或添加一项相对时间过期方式的缓存
        /// </summary>
        /// <typeparam name="TKey">缓存键</typeparam>
        /// <typeparam name="TValue">缓存值</typeparam>
        /// <param name="cache">缓存</param>
        /// <param name="key">缓存键</param>
        /// <param name="interval">缓存过期时间间隔</param>
        /// <param name="valueLoaderFunc">缓存值加载策略</param>
        public static TValue GetOrAddOfRelative<TKey, TValue>(this Cache<TKey, TValue> cache, TKey key, TimeSpan interval, Func<TKey, TValue> valueLoaderFunc)
            where TValue : class
        {
            return GetOrAdd(cache, key, new RelativeCacheExpireDependency(interval), valueLoaderFunc);
        }

        /// <summary>
        /// 添加一项永不过期的缓存
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOfNoExpire<TKey, TValue>(this Cache<TKey, TValue> cache, TKey key, TValue value)
            where TValue : class
        {
            Contract.Requires(cache != null);

            cache.Add(key, value, NoExpireCacheExpireDependency.Instance);
        }

        /// <summary>
        /// 获取或添加一项永不过期的缓存
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="cache">缓存</param>
        /// <param name="key">键</param>
        /// <param name="valueLoader">值加载器</param>
        public static TValue GetOrAddOfNoExpire<TKey, TValue>(this Cache<TKey, TValue> cache, TKey key, ICacheValueLoader<TKey, TValue> valueLoader)
            where TValue : class
        {
            return GetOrAdd(cache, key, NoExpireCacheExpireDependency.Instance, valueLoader);
        }

        /// <summary>
        /// 获取或添加一项永不过期的缓存
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="cache">缓存</param>
        /// <param name="key">键</param>
        /// <param name="valueLoaderFunc">值加载器</param>
        public static TValue GetOrAddOfNoExpire<TKey, TValue>(this Cache<TKey, TValue> cache, TKey key, Func<TKey, TValue> valueLoaderFunc)
            where TValue : class
        {
            Contract.Requires(cache != null);

            return GetOrAdd(cache, key, NoExpireCacheExpireDependency.Instance, valueLoaderFunc);
        }

        /// <summary>
        /// 添加一项动态过期时间的缓存
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="cache">缓存</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="init">初始过期时间</param>
        /// <param name="step">递增步长</param>
        /// <param name="max">最大过期时间</param>
        /// <param name="valueLoader">缓存值加载方式</param>
        public static void AddOfDynamic<TKey, TValue>(this Cache<TKey, TValue> cache, TKey key, TValue value, TimeSpan init, TimeSpan step, TimeSpan max, ICacheValueLoader<TKey, TValue> valueLoader = null)
            where TValue : class
        {
            Contract.Requires(cache != null);

            cache.Add(key, value, new DynamicCacheExpireDependency(init, step, max), valueLoader);
        }

        /// <summary>
        /// 获取或添加一项动态过期时间的缓存
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="cache">缓存</param>
        /// <param name="key">键</param>
        /// <param name="init">初始过期时间</param>
        /// <param name="step">递增步长</param>
        /// <param name="max">最大过期时间</param>
        /// <param name="valueLoader">缓存值加载方式</param>
        public static TValue GetOrAddOfDynamic<TKey, TValue>(this Cache<TKey, TValue> cache, TKey key, TimeSpan init, TimeSpan step, TimeSpan max, ICacheValueLoader<TKey, TValue> valueLoader = null)
            where TValue : class
        {
            return GetOrAdd(cache, key, new DynamicCacheExpireDependency(init, step, max), valueLoader);
        }

        /// <summary>
        /// 添加一项动态过期时间的缓存
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="cache">缓存</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="init">初始值</param>
        /// <param name="step">递增步长</param>
        /// <param name="max">最大过期时间</param>
        /// <param name="valueLoaderFunc">缓存值加载方式</param>
        public static void AddOfDynamic<TKey, TValue>(this Cache<TKey, TValue> cache, TKey key, TValue value, TimeSpan init, TimeSpan step, TimeSpan max, Func<TKey, TValue> valueLoaderFunc)
            where TValue : class
        {
            AddOfDynamic(cache, key, value, init, step, max, _ConvertToValueLoader(valueLoaderFunc));
        }

        /// <summary>
        /// 获取或添加一项动态过期时间的缓存
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="cache">缓存</param>
        /// <param name="key">键</param>
        /// <param name="init">初始值</param>
        /// <param name="step">递增步长</param>
        /// <param name="max">最大过期时间</param>
        /// <param name="valueLoaderFunc">缓存值加载方式</param>
        public static TValue GetOrAddOfDynamic<TKey, TValue>(this Cache<TKey, TValue> cache, TKey key, TimeSpan init, TimeSpan step, TimeSpan max, Func<TKey, TValue> valueLoaderFunc)
            where TValue : class
        {
            return GetOrAdd(cache, key, new DynamicCacheExpireDependency(init, step, max), _ConvertToValueLoader(valueLoaderFunc));
        }

        /// <summary>
        /// 获取或添加一项缓存
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="cache">缓存过期方式</param>
        /// <param name="key">键</param>
        /// <param name="valueLoader">值的加载器</param>
        /// <returns></returns>
        public static TValue GetOrAdd<TKey, TValue>(this Cache<TKey, TValue> cache, TKey key, ICacheExpireDependency expireDependency, ICacheValueLoader<TKey, TValue> valueLoader)
            where TValue : class
        {
            Contract.Requires(cache != null && expireDependency != null && valueLoader != null);

            TValue value = cache.Get(key);
            if (value == null)
            {
                lock (valueLoader)
                {
                    if ((value = cache.Get(key)) == null)
                    {
                        value = valueLoader.Load(key);
                        cache.Add(key, value, expireDependency, valueLoader);
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// 获取或添加一项缓存
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="cache">缓存</param>
        /// <param name="key">键</param>
        /// <param name="expireDependency">过期方式</param>
        /// <param name="valueLoaderFunc">值的加载器</param>
        /// <returns></returns>
        public static TValue GetOrAdd<TKey, TValue>(this Cache<TKey, TValue> cache, TKey key, ICacheExpireDependency expireDependency, Func<TKey, TValue> valueLoaderFunc)
            where TValue : class
        {
            return GetOrAdd(cache, key, expireDependency, _ConvertToValueLoader(valueLoaderFunc));
        }

        private static ICacheValueLoader<TKey, TValue> _ConvertToValueLoader<TKey, TValue>(Func<TKey, TValue> valueLoaderFunc)
            where TValue : class
        {
            if (valueLoaderFunc == null)
                return null;

            return new CacheValueLoaderFuncAdapter<TKey, TValue>(valueLoaderFunc);
        }
    }
}