/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/8/15
 * 时间: 13:49
 *
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */

using System.Collections.Concurrent;

namespace Dorado.Core.Collection
{
    /// <summary>
    /// BlockingQueue.
    /// </summary>
    public class BlockingQueue<T> : BlockingCollection<T>
    {
        #region ctor(s)

        public BlockingQueue()
            : base(new ConcurrentQueue<T>())
        {
        }

        public BlockingQueue(int maxSize)
            : base(new ConcurrentQueue<T>(), maxSize)
        {
        }

        #endregion ctor(s)

        #region Methods

        /// <summary>
        /// Enqueue an Item
        /// </summary>
        /// <param name="item">Item to enqueue</param>
        /// <remarks>blocks if the blocking queue is full</remarks>
        public void Enqueue(T item)
        {
            Add(item);
        }

        /// <summary>
        /// Dequeue an item
        /// </summary>
        /// <param name="Item"></param>
        /// <returns>Item dequeued</returns>
        /// <remarks>blocks if the blocking queue is empty</remarks>
        public T Dequeue()
        {
            return Take();
        }

        #endregion Methods
    }
}