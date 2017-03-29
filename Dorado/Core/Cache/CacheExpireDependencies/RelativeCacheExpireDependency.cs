using Dorado.Extensions;
using System;

namespace Dorado.Core.Cache.CacheExpireDependencies
{
    /// <summary>
    /// 相对时间过期方式
    /// </summary>
    public class RelativeCacheExpireDependency : ICacheExpireDependency
    {
        public RelativeCacheExpireDependency(TimeSpan interval)
        {
            _interval = interval;
            _expireTime = DateTime.Now + _interval;
        }

        private readonly TimeSpan _interval;
        private DateTime _expireTime;

        #region ICacheExpireDependency Members

        public void Reset()
        {
            _expireTime = DateTime.Now + _interval;
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