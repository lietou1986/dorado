using Dorado.Extensions;
using System;

namespace Dorado.Core.Cache.CacheExpireDependencies
{
    /// <summary>
    /// 动态过期时间的缓存，依赖于请求的频率，自动延长过期时间
    /// </summary>
    public class DynamicCacheExpireDependency : ICacheExpireDependency
    {
        public DynamicCacheExpireDependency(TimeSpan init, TimeSpan step, TimeSpan max)
            : this(() => { TimeSpan t = init; init += step; if (init > max) init = max; return t; })
        {
        }

        public DynamicCacheExpireDependency(Func<TimeSpan> nextFunc)
        {
            _nextFunc = nextFunc;
            _expireTime = CommonExtension.QuickGetTime() + nextFunc();
        }

        private readonly Func<TimeSpan> _nextFunc;

        private DateTime _expireTime;

        #region ICacheExpireDependency Members

        public void Reset()
        {
        }

        public void AccessNotify()
        {
            _expireTime = CommonExtension.QuickGetTime() + _nextFunc();
        }

        public bool HasExpired()
        {
            return CommonExtension.QuickGetTime() >= _expireTime;
        }

        #endregion ICacheExpireDependency Members
    }
}