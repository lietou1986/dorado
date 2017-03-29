using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Dorado.Core.TaskDispatcher
{
    /// <summary>
    /// 默认的任务队列
    /// </summary>
    /// <typeparam name="TTask">任务类型</typeparam>
    internal class TaskQueue<TTask> : ITaskQueue<TTask>
        where TTask : class, ITask
    {
        private readonly Queue<TTask> _Queue = new Queue<TTask>();

        #region ITaskQueue<TTask> Members

        /// <summary>
        /// 将一个任务入队
        /// </summary>
        /// <param name="task"></param>
        public void Enqueue(TTask task)
        {
            Contract.Requires(task != null);

            lock (_Queue)
            {
                _Queue.Enqueue(task);
            }
        }

        /// <summary>
        /// 从队列中获取一个任务，如果队列为空则返回null
        /// </summary>
        /// <returns></returns>
        public TTask Dequeue()
        {
            lock (_Queue)
            {
                if (_Queue.Count == 0)
                    return null;

                return _Queue.Dequeue();
            }
        }

        #endregion ITaskQueue<TTask> Members
    }
}