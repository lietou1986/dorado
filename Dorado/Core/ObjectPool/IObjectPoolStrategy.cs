namespace Dorado.Core.ObjectPool
{
    /// <summary>
    /// 对象缓存池对象创建器
    /// </summary>
    public interface IObjectPoolStrategy
    {
        /// <summary>
        /// 用于等待对象资源
        /// </summary>
        /// <param name="count">对象数量</param>
        /// <param name="timeoutMillseconds">最大等待时间</param>
        bool Wait(int count, int timeoutMillseconds);

        /// <summary>
        /// 获取对象通知
        /// </summary>
        /// <param name="count">对象数量</param>
        /// <param name="newCount">新创建的对象数量</param>
        /// <returns></returns>
        void AccquireNotify(int count, int newCount);

        /// <summary>
        /// 释放对象通知
        /// </summary>
        /// <param name="count">对象数量</param>
        void ReleaseNotify(int count);

        /// <summary>
        /// 对象池清理通知
        /// </summary>
        /// <returns>需要保留的最大数量</returns>
        int TrimNotify();
    }
}