using System.Diagnostics.Contracts;

namespace Dorado.Core.TaskDispatcher
{
    /// <summary>
    /// 用于任务调度的信号量
    /// </summary>
    public class TaskSemaphore : ITaskSemaphore
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="max">信号个数</param>
        public TaskSemaphore(int max)
        {
            Contract.Requires(max > 0);

            _Semaphore = new System.Threading.Semaphore(max, max);
        }

        private readonly System.Threading.Semaphore _Semaphore;

        #region ITaskSemaphore Members

        /// <summary>
        /// 信号的句柄
        /// </summary>
        public System.Threading.WaitHandle Handler
        {
            get { return _Semaphore; }
        }

        /// <summary>
        /// 释放一个信号
        /// </summary>
        public void Release()
        {
            _Semaphore.Release();
        }

        #endregion ITaskSemaphore Members

        #region IDisposable Members

        public void Dispose()
        {
            _Semaphore.Dispose();
        }

        #endregion IDisposable Members
    }
}