using System.Threading;

namespace Dorado.Core.TaskDispatcher
{
    /// <summary>
    /// 任务调度所用的信号量，用于控制有多少任务在运行
    /// </summary>
    public interface ITaskSemaphore
    {
        /// <summary>
        /// 信号的句柄
        /// </summary>
        WaitHandle Handler { get; }

        /// <summary>
        /// 释放一个信号
        /// </summary>
        void Release();
    }
}