using System;

namespace Dorado.Core.SuperCache
{
    public interface ICache<TKey, TResult>
    {
        TResult Get(TKey key, Func<AcquireContext<TKey>, TResult> acquire);
    }
}