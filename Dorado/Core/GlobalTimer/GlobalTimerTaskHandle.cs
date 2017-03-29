namespace Dorado.Core.GlobalTimer
{
    /// <summary>
    /// 全局定时器的定时任务句柄
    /// </summary>
    internal class GlobalTimerTaskHandle<TTask> : IGlobalTimerTaskHandle
        where TTask : class, ITask
    {
        internal GlobalTimerTaskHandle(GlobalTimer<TTask> owner, GlobalTimerTaskItem<TTask> task)
        {
            _owner = owner;
            Task = task;
        }

        private readonly GlobalTimer<TTask> _owner;

        internal GlobalTimerTaskItem<TTask> Task { get; private set; }

        /// <summary>
        /// 是否为启用状态
        /// </summary>
        public bool Enable
        {
            get { return Task.Enable; }
            set { Task.Enable = value; }
        }

        /// <summary>
        /// 启用
        /// </summary>
        public void Start()
        {
            Enable = true;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public void Stop()
        {
            Enable = false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Stop();
            _owner.Remove(this);
        }

        #endregion IDisposable Members
    }
}