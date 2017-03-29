using Dorado.Core.TaskDispatcher;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Dorado.Core
{
    /// <summary>
    /// 任务调度器
    /// </summary>
    /// <typeparam name="TTask">任务类型</typeparam>
    public class TaskDispatcher<TTask> : IDisposable
        where TTask : class, ITask
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dispatcherStrategy">任务调度策略</param>
        /// <param name="dispatcherExecutor">任务执行策略</param>
        public TaskDispatcher(
            ITaskDispatcherStrategy<TTask> dispatcherStrategy = null,
            ITaskExecutor<TTask> dispatcherExecutor = null
         )
        {
            _DispatcherExecutor = dispatcherExecutor ?? TaskExecutor<TTask>.Instance;
            _DispatcherStrategy = dispatcherStrategy ?? new TaskDispatcherStrategy<TTask>();

            _DispatcherStrategy.RunDispatch(_TaskRunCallback);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maxRunningCount">最大运行数量</param>
        /// <param name="dispatcherExecutor">任务执行策略</param>
        public TaskDispatcher(int maxRunningCount, ITaskExecutor<TTask> dispatcherExecutor = null)
            : this(new TaskDispatcherStrategy<TTask>(new TaskSemaphore(maxRunningCount)), dispatcherExecutor)
        {
        }

        private readonly ITaskDispatcherStrategy<TTask> _DispatcherStrategy;
        private readonly ITaskExecutor<TTask> _DispatcherExecutor;

        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="task">任务</param>
        public void Add(TTask task)
        {
            Contract.Requires(task != null);

            Add(new[] { task });
        }

        /// <summary>
        /// 批量添加任务
        /// </summary>
        /// <param name="tasks"></param>
        public void Add(IEnumerable<TTask> tasks)
        {
            Contract.Requires(tasks != null);

            _WaitForAllCompletedEvent.Reset();
            _DispatcherStrategy.AddTasks(tasks);
        }

        // 当任务需要执行时的回调
        private void _TaskRunCallback(TTask task)
        {
            _RunningCount++;
            _DispatcherExecutor.Execute(task, _TaskCompletedCallback, null);
        }

        private volatile int _RunningCount = 0;

        /// <summary>
        /// 当前正在执行的任务数
        /// </summary>
        public int RunningCount
        {
            get { return _RunningCount; }
        }

        // 任务完成时的回调
        private void _TaskCompletedCallback(TTask task, Exception error, object state)
        {
            _DispatcherStrategy.TaskCompletedNotify(task);
            _RaiseTaskCompletedEvent(task, error);

            if (--_RunningCount <= 0)
            {
                _WaitForAllCompletedEvent.Set();
                _RaiseAllTaskCompletedEvent();

                Contract.Requires(_RunningCount == 0);
            }
        }

        // 触发任务完成事件
        private void _RaiseTaskCompletedEvent(TTask task, Exception error)
        {
            var eh = TaskCompleted;
            if (eh != null)
                eh(this, new TaskCompletedEventArgs<TTask>(task, error));
        }

        // 触发所有任务完成事件
        private void _RaiseAllTaskCompletedEvent()
        {
            var eh = AllTaskCompleted;
            if (eh != null)
                eh(this, EventArgs.Empty);
        }

        /// <summary>
        /// 任务完成事件
        /// </summary>
        public event TaskCompletedEventHandler<TTask> TaskCompleted;

        /// <summary>
        /// 所有任务完成事件
        /// </summary>
        public event EventHandler AllTaskCompleted;

        private readonly AutoResetEvent _WaitForAllCompletedEvent = new AutoResetEvent(false);

        /// <summary>
        /// 等待所有的任务执行完毕
        /// </summary>
        public void WaitForAllCompleted()
        {
            _WaitForAllCompletedEvent.WaitOne();
        }

        #region IDisposable Members

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _DispatcherStrategy.Dispose();
        }

        #endregion IDisposable Members
    }

    /// <summary>
    /// 任务完成事件的参数
    /// </summary>
    /// <typeparam name="TTask"></typeparam>
    public class TaskCompletedEventArgs<TTask> : EventArgs
        where TTask : class, ITask
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="error">错误信息</param>
        /// <param name="task">任务</param>
        internal TaskCompletedEventArgs(TTask task, Exception error)
        {
            Task = task;
            Error = error;
        }

        /// <summary>
        /// 任务
        /// </summary>
        public TTask Task { get; private set; }

        /// <summary>
        /// 错误
        /// </summary>
        /// <remarks>如果执行正确，则为空引用</remarks>
        public Exception Error { get; private set; }

        /// <summary>
        /// 是否已经执行成功
        /// </summary>
        public bool Success
        {
            get { return Error == null; }
        }
    }

    /// <summary>
    /// 任务完成事件
    /// </summary>
    /// <typeparam name="TTask">任务类型</typeparam>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void TaskCompletedEventHandler<TTask>(object sender, TaskCompletedEventArgs<TTask> e)
        where TTask : class, ITask;
}