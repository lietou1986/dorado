using System;

namespace Dorado.Core
{
    /// <summary>
    /// 任务调度器的任务执行器
    /// </summary>
    /// <typeparam name="TTask">任务类型</typeparam>
    public interface ITaskExecutor<TTask>
        where TTask : class, ITask
    {
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="completedCallback">任务完成的回调</param>
        /// <param name="state">其它的参数，在completedCallback中被传回</param>
        void Execute(TTask task, TaskCompletedCallback<TTask> completedCallback, object state);
    }

    /// <summary>
    /// 任务完成的回调函数
    /// </summary>
    /// <typeparam name="TTask">任务类型</typeparam>
    /// <param name="task">任务</param>
    /// <param name="error">错误信息</param>
    /// <param name="state">其它的参数</param>
    public delegate void TaskCompletedCallback<in TTask>(TTask task, Exception error, object state)
        where TTask : class, ITask;
}