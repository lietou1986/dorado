using Dorado.Core;
using Dorado.Core.TaskDispatcher;
using System;

namespace Dorado
{
    /// <summary>
    /// 基本组件的创建工厂
    /// </summary>
    public static class CoreFactory
    {
        /// <summary>
        /// 创建任务调度器
        /// </summary>
        /// <typeparam name="TTask"></typeparam>
        /// <returns></returns>
        public static TaskDispatcher<TTask> CreateTaskDispatcher<TTask>()
            where TTask : class, ITask
        {
            return new TaskDispatcher<TTask>();
        }

        /// <summary>
        /// 创建具有优先级的任务调度器
        /// </summary>
        /// <typeparam name="TTask">任务类型</typeparam>
        /// <typeparam name="TPriority">优先级类型</typeparam>
        /// <param name="dispatcherExecutor">任务调度策略</param>
        /// <param name="maxRunningCount">最大任务运行数</param>
        /// <returns></returns>
        public static TaskDispatcher<TTask> CreatePriorityTaskDispatcher<TTask, TPriority>(
            int maxRunningCount = TaskDispatcherStrategy<TTask>.DEFAULT_MAX_RUNNING_COUNT, ITaskExecutor<TTask> dispatcherExecutor = null)
            where TTask : class, ITask, IPriority<TPriority>
            where TPriority : IComparable<TPriority>
        {
            return new TaskDispatcher<TTask>(new TaskDispatcherStrategy<TTask>(maxRunningCount, new PriorityTaskQueue<TTask, TPriority>()), dispatcherExecutor);
        }
    }
}