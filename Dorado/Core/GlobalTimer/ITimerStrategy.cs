namespace Dorado.Core.GlobalTimer
{
    /// <summary>
    /// 定时策略
    /// </summary>
    public interface ITimerStrategy
    {
        /// <summary>
        /// 是否时间到
        /// </summary>
        bool IsTimeUp();

        /// <summary>
        /// 任务开始运行通知
        /// </summary>
        void RunNotify();

        /// <summary>
        /// 任务运行完成通知
        /// </summary>
        void CompletedNotify();
    }
}