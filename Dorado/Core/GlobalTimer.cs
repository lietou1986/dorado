using Dorado.Core.GlobalTimer;
using Dorado.Core.GlobalTimer.TimerStrategies;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Dorado.Core
{
    /// <summary>
    /// 全局定时器
    /// </summary>
    public class GlobalTimer<TTask> : IDisposable
        where TTask : class, ITask
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="interval">轮询周期</param>
        /// <param name="defaultTaskExecutor">任务执行策略</param>
        public GlobalTimer(TimeSpan interval, ITaskExecutor<TTask> defaultTaskExecutor = null)
            : this((int)interval.TotalMilliseconds, defaultTaskExecutor)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="interval">轮询周期（毫秒数）</param>
        /// <param name="defaultTaskExecutor">任务执行策略</param>
        public GlobalTimer(int interval, ITaskExecutor<TTask> defaultTaskExecutor = null)
        {
            Contract.Requires(interval > 0);

            _interval = interval;
            _defaultTaskExecutor = defaultTaskExecutor ?? TaskExecutor<TTask>.Instance;
            _timer = new Timer(_TimerCallback);
            _ChangeTimer();
        }

        private readonly Timer _timer;
        private readonly int _interval;
        private readonly HashSet<GlobalTimerTaskItem<TTask>> _tasks = new HashSet<GlobalTimerTaskItem<TTask>>();
        private readonly ITaskExecutor<TTask> _defaultTaskExecutor;

        private void _ChangeTimer()
        {
            try
            {
                _timer.Change(_interval, Timeout.Infinite);
            }
            catch (ObjectDisposedException) { }
        }

        private void _TimerCallback(object state)
        {
            try
            {
                lock (_tasks)
                {
                    _Dispatch();
                }
            }
            finally
            {
                _ChangeTimer();
            }
        }

        // 调度任务
        private void _Dispatch()
        {
            foreach (GlobalTimerTaskItem<TTask> taskItem in _tasks)
            {
                if (!taskItem.Enable)
                    continue;

                if (taskItem.EnableReenter)
                {
                    if (taskItem.TimerStrategy.IsTimeUp())
                    {
                        taskItem.RunningCount++;
                        _RunTask(taskItem);
                    }
                }
                else
                {
                    if (taskItem.RunningCount > 0 || !taskItem.TimerStrategy.IsTimeUp())
                        continue;

                    lock (taskItem)
                    {
                        if (taskItem.RunningCount <= 0)
                        {
                            taskItem.RunningCount++;
                            _RunTask(taskItem);
                        }
                    }
                }
            }
        }

        // 运行任务
        private void _RunTask(GlobalTimerTaskItem<TTask> taskItem)
        {
            taskItem.TimerStrategy.RunNotify();
            taskItem.TaskExecutor.Execute(taskItem.Task, _TaskCompletedCallback, taskItem);
        }

        // 任务运行完毕的回调
        private void _TaskCompletedCallback(TTask task, Exception error, object state)
        {
            var taskItem = (GlobalTimerTaskItem<TTask>)state;

            taskItem.TimerStrategy.CompletedNotify();
            taskItem.RunningCount--;
        }

        /// <summary>
        /// 添加一个定时任务
        /// </summary>
        /// <param name="timerStrategy">定时方式</param>
        /// <param name="task">任务</param>
        /// <param name="taskExecutor">任务执行策略</param>
        /// <param name="enableReenter">是否允许重入</param>
        /// <param name="enable">是否为启用状态</param>
        /// <returns>控制句柄</returns>
        public IGlobalTimerTaskHandle Add(ITimerStrategy timerStrategy, TTask task, ITaskExecutor<TTask> taskExecutor, bool enableReenter = true, bool enable = true)
        {
            Contract.Requires(timerStrategy != null && task != null);

            lock (_tasks)
            {
                var taskItem = new GlobalTimerTaskItem<TTask>(timerStrategy, task, taskExecutor ?? _defaultTaskExecutor, enableReenter, enable);
                _tasks.Add(taskItem);
                return new GlobalTimerTaskHandle<TTask>(this, taskItem);
            }
        }

        /// <summary>
        /// 添加一个定时任务
        /// </summary>
        /// <param name="timerStrategy">定时方式</param>
        /// <param name="task">任务</param>
        /// <param name="enableReenter">是否允许重入</param>
        /// <returns>控制句柄</returns>
        public IGlobalTimerTaskHandle Add(ITimerStrategy timerStrategy, TTask task, bool enableReenter = true)
        {
            return Add(timerStrategy, task, null, enableReenter);
        }

        /// <summary>
        /// 添加一个定时任务
        /// </summary>
        /// <param name="interval">周期</param>
        /// <param name="task">任务</param>
        /// <param name="enableReenter">是否允许重入</param>
        /// <param name="enable">是否为启用状态</param>
        /// <returns>控制句柄</returns>
        public IGlobalTimerTaskHandle Add(TimeSpan interval, TTask task, bool enableReenter = true, bool enable = true)
        {
            return Add(new StaticIntervalTimerStrategy(interval), task, null, enableReenter, enable);
        }

        /// <summary>
        /// 删除一项任务
        /// </summary>
        /// <param name="handle"></param>
        internal void Remove(GlobalTimerTaskHandle<TTask> handle)
        {
            lock (_tasks)
            {
                _tasks.Remove(handle.Task);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _timer.Dispose();
        }

        ~GlobalTimer()
        {
            Dispose();
        }

        #endregion IDisposable Members

        /// <summary>
        /// 默认实体
        /// </summary>
        public static readonly GlobalTimer<ITask> Default = new GlobalTimer<ITask>(TimeSpan.FromSeconds(1));
    }
}