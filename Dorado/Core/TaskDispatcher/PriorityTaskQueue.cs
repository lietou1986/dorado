using Dorado.Core.Collection;
using System;
using System.Diagnostics.Contracts;

namespace Dorado.Core.TaskDispatcher
{
    /// <summary>
    /// 具有优先级的任务队列
    /// </summary>
    /// <typeparam name="TTask">任务类型</typeparam>
    /// <typeparam name="TPriority">优先级数值类型</typeparam>
    public class PriorityTaskQueue<TTask, TPriority> : ITaskQueue<TTask>
        where TTask : class, ITask, IPriority<TPriority>
        where TPriority : IComparable<TPriority>
    {
        private PriorityQueue<TTask, TPriority> _Queue = new PriorityQueue<TTask, TPriority>();

        #region ITaskQueue<TTask> Members

        public void Enqueue(TTask task)
        {
            Contract.Requires(task != null);

            lock (_Queue)
            {
                _Queue.Enqueue(task);
            }
        }

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