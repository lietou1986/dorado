namespace Dorado.Core.TaskDispatcher
{
    /// <summary>
    /// 任务队列
    /// </summary>
    public interface ITaskQueue<TTask>
        where TTask : class, ITask
    {
        /// <summary>
        /// 将一个任务入队
        /// </summary>
        /// <param name="task">任务</param>
        void Enqueue(TTask task);

        /// <summary>
        /// 从队列中获取一个任务
        /// </summary>
        /// <returns>如果队列为空则返回null</returns>
        TTask Dequeue();
    }
}