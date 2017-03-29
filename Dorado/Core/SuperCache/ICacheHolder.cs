using System;

namespace Dorado.Core.SuperCache
{
    public interface ICacheHolder
    {
        ICache<TKey, TResult> GetCache<TKey, TResult>(Type component);
    }
}