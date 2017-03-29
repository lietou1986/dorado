using Dorado.Core.Cache;
using System;

namespace Dorado.Core
{
    public class SingleMemoryCache<TValue>
        where TValue : class
    {
        public SingleMemoryCache(TimeSpan timeout, Func<TValue> loader)
        {
            _timeout = timeout;
            _loader = loader;
        }

        public TValue Get()
        {
            return _innerCache.GetOrAddOfRelative(_key, _timeout, (key) => _loader());
        }

        private readonly Guid _key = Guid.NewGuid();
        private readonly TimeSpan _timeout;
        private readonly Func<TValue> _loader;

        private static readonly Cache<Guid, TValue> _innerCache = new Cache<Guid, TValue>();
    }
}