namespace Dorado.Core.Cache.CacheExpireDependencies
{
    /// <summary>
    /// 永不过期的缓存过期方式
    /// </summary>
    public class NoExpireCacheExpireDependency : ICacheExpireDependency
    {
        #region ICacheExpireDependency Members

        public void Reset()
        {
        }

        public void AccessNotify()
        {
        }

        public bool HasExpired()
        {
            return false;
        }

        #endregion ICacheExpireDependency Members

        public static readonly NoExpireCacheExpireDependency Instance = new NoExpireCacheExpireDependency();
    }
}