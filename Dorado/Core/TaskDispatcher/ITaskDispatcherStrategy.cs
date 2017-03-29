using System;
using System.Collections.Generic;

namespace Dorado.Core.TaskDispatcher
{
    /// <summary>
    /// 调度器的调度策略
    /// </summary>
    /// <typeparam name="TTask"></typeparam>
    public interface ITaskDispatcherStrategy<TTask> : IDisposable
        where TTask : class, ITask
    {
        /// <summary>
        /// 批量添加任务
        /// </summary>
        /// <param name="tasks">任务</param>
        void AddTasks(IEnumerable<TTask> tasks);

        /// <summary>
        /// 执行调度
        /// </summary>
        /// <param name="callback">任务运行时的回调函数</param>
        void RunDispatch(TaskRunCallback<TTask> callback);

        /// <summary>
        /// 任务完成的通知
        /// </summary>
        /// <param name="task"></param>
        void TaskCompletedNotify(TTask task);
    }

    /// <summary>
    /// 任务执行时的回调函数
    /// </summary>
    /// <typeparam name="TTask">任务类型</typeparam>
    /// <param name="task">任务</param>
    public delegate void TaskRunCallback<in TTask>(TTask task)
        where TTask : class, ITask;
}