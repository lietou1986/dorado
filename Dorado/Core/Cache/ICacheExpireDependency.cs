namespace Dorado.Core.Cache
{
    /// <summary>
    /// 缓存过期策略
    /// </summary>
    public interface ICacheExpireDependency
    {
        /// <summary>
        /// 重置
        /// </summary>
        void Reset();

        /// <summary>
        /// 访问通知
        /// </summary>
        void AccessNotify();

        /// <summary>
        /// 是否已经过期
        /// </summary>
        /// <returns></returns>
        bool HasExpired();
    }
}