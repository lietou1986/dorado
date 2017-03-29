using System;

namespace Dorado.Core.GlobalTimer
{
    /// <summary>
    /// 全局定时器的定时任务句柄
    /// </summary>
    public interface IGlobalTimerTaskHandle : IDisposable
    {
        /// <summary>
        /// 启用/禁用状态
        /// </summary>
        bool Enable { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        void Start();

        /// <summary>
        /// 禁用
        /// </summary>
        void Stop();
    }
}