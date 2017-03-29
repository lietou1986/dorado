using System;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Dorado.Core
{
    /// <summary>
    /// 默认的任务执行策略
    /// </summary>
    /// <typeparam name="TTask">任务类型</typeparam>
    internal class TaskExecutor<TTask> : ITaskExecutor<TTask>
        where TTask : class, ITask
    {
        #region ITaskExecutor<TTask> Members

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="completedCallback">任务完成的回调</param>
        /// <param name="state">其它参数，在completedCallback中被传回</param>
        public void Execute(TTask task, TaskCompletedCallback<TTask> completedCallback, object state)
        {
            Contract.Requires(task != null && completedCallback != null);

            ThreadPool.QueueUserWorkItem(_Execute, new object[] { task, completedCallback, state });
        }

        private void _Execute(object state)
        {
            object[] states = (object[])state;
            TTask task = (TTask)states[0];
            var completedCallback = (TaskCompletedCallback<TTask>)states[1];

            try
            {
                task.Execute();
                completedCallback(task, null, states[2]);
            }
            catch (Exception ex)
            {
                completedCallback(task, ex, states[2]);
            }
        }

        #endregion ITaskExecutor<TTask> Members

        #region IDisposable Members

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
        }

        #endregion IDisposable Members

        internal static readonly TaskExecutor<TTask> Instance = new TaskExecutor<TTask>();
    }
}