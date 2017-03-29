using Dorado.Extensions;
using System;

namespace Dorado.Core.Cache.CacheExpireDependencies
{
    /// <summary>
    /// 固定时间的缓存过期方式
    /// </summary>
    public class TermlyCacheExpireDependency : ICacheExpireDependency
    {
        public TermlyCacheExpireDependency(DateTime expireTime)
        {
            _expireTime = expireTime;
        }

        public TermlyCacheExpireDependency(TimeSpan interval)
            : this(DateTime.Now + interval)
        {
        }

        private readonly DateTime _expireTime;

        #region ICacheExpireDependency Members

        public void Reset()
        {
        }

        public void AccessNotify()
        {
        }

        public bool HasExpired()
        {
            return CommonExtension.QuickGetTime() > _expireTime;
        }

        #endregion ICacheExpireDependency Members
    }
}