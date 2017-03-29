using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Dorado.Core.TaskDispatcher
{
    /// <summary>
    /// 任务调度策略的基类
    /// </summary>
    /// <typeparam name="TTask">任务类型</typeparam>
    public class TaskDispatcherStrategy<TTask> : ITaskDispatcherStrategy<TTask>
        where TTask : class, ITask
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maxRunningCount">最大的运行数量</param>
        /// <param name="taskQueue">任务队列</param>
        public TaskDispatcherStrategy(ITaskSemaphore taskSemaphore = null, ITaskQueue<TTask> taskQueue = null)
        {
            _TaskSemaphore = taskSemaphore ?? new TaskSemaphore(DEFAULT_MAX_RUNNING_COUNT);
            _TaskQueue = taskQueue ?? new TaskQueue<TTask>();
            _DispatchThread = new Thread(_DispatchFunc);
        }

        internal const int DEFAULT_MAX_RUNNING_COUNT = 10;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maxRunningCount">任务运行的最大数量</param>
        /// <param name="taskQueue">任务队列</param>
        public TaskDispatcherStrategy(int maxRunningCount, ITaskQueue<TTask> taskQueue = null)
            : this(new TaskSemaphore(maxRunningCount), taskQueue)
        {
        }

        private readonly ITaskQueue<TTask> _TaskQueue;
        private readonly ITaskSemaphore _TaskSemaphore;
        private readonly Thread _DispatchThread;
        private readonly AutoResetEvent _WaitForTaskEvent = new AutoResetEvent(true);
        private readonly ManualResetEvent _DisposeNotifyEvent = new ManualResetEvent(false);
        private volatile bool _Running;

        private bool _Wait(WaitHandle[] waitHandles)
        {
            WaitHandle.WaitAny(waitHandles);
            return _Running;
        }

        // 调度线程
        private void _DispatchFunc(object state)
        {
            TaskRunCallback<TTask> callback = (TaskRunCallback<TTask>)state;
            WaitHandle[] waitTaskHandlers = new WaitHandle[] { _WaitForTaskEvent, _DisposeNotifyEvent };

            while (_Wait(waitTaskHandlers))
            {
                TTask task;
                while ((task = _TaskQueue.Dequeue()) != null)
                {
                    if (!_Wait(new[] { _TaskSemaphore.Handler, _DisposeNotifyEvent }))
                        break;

                    callback(task);
                }
            }
        }

        #region ITaskDispatcherStrategy<TTask> Members

        /// <summary>
        /// 批量添加任务
        /// </summary>
        /// <param name="tasks">任务</param>
        public void AddTasks(IEnumerable<TTask> tasks)
        {
            Contract.Requires(tasks != null);

            foreach (TTask task in tasks)
            {
                _TaskQueue.Enqueue(task);
            }

            _WaitForTaskEvent.Set();
        }

        /// <summary>
        /// 执行调度
        /// </summary>
        /// <param name="callback"></param>
        public void RunDispatch(TaskRunCallback<TTask> callback)
        {
            Contract.Requires(callback != null);

            _Running = true;
            _DispatchThread.Start(callback);
        }

        /// <summary>
        /// 任务完成通知
        /// </summary>
        /// <param name="task">已完成的任务</param>
        public void TaskCompletedNotify(TTask task)
        {
            Contract.Requires(task != null);

            _TaskSemaphore.Release();
        }

        #endregion ITaskDispatcherStrategy<TTask> Members

        #region IDisposable Members

        public void Dispose()
        {
            _Running = false;
            _DisposeNotifyEvent.Set();
        }

        #endregion IDisposable Members
    }
}