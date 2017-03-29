using System;
using System.Diagnostics.Contracts;

namespace Dorado.Core.Cache
{
    /// <summary>
    /// 从代理向缓存加载接口的适配器
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class CacheValueLoaderFuncAdapter<TKey, TValue> : ICacheValueLoader<TKey, TValue>
        where TValue : class
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="valueLoaderFunc">加载方法</param>
        public CacheValueLoaderFuncAdapter(Func<TKey, TValue> valueLoaderFunc)
        {
            Contract.Requires(valueLoaderFunc != null);

            _ValueLoaderFunc = valueLoaderFunc;
        }

        private readonly Func<TKey, TValue> _ValueLoaderFunc;

        #region ICacheValueLoader<TKey,TValue> Members

        public TValue Load(TKey key)
        {
            return _ValueLoaderFunc(key);
        }

        #endregion ICacheValueLoader<TKey,TValue> Members
    }
}