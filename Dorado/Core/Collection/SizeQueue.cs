/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2015/8/15
 * 时间: 13:50
 *
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */

using System.Collections.Generic;
using System.Threading;

namespace Dorado.Core.Collection
{
    public class SizeQueue<T>
    {
        private readonly Queue<T> queue = new Queue<T>();
        private readonly int maxSize;

        public SizeQueue(int maxSize)
        {
            this.maxSize = maxSize;
        }

        public void Enqueue(T item)
        {
            lock (queue)
            {
                while (queue.Count >= maxSize)
                {
                    Monitor.Wait(queue);
                }
                queue.Enqueue(item);
                if (queue.Count == 1)
                {
                    // wake up any blocked dequeue
                    Monitor.PulseAll(queue);
                }
            }
        }

        public T Dequeue()
        {
            lock (queue)
            {
                while (queue.Count == 0)
                {
                    Monitor.Wait(queue);
                }
                T item = queue.Dequeue();
                if (queue.Count == maxSize - 1)
                {
                    // wake up any blocked enqueue
                    Monitor.PulseAll(queue);
                }
                return item;
            }
        }

        private bool closing;

        public void Close()
        {
            lock (queue)
            {
                closing = true;
                Monitor.PulseAll(queue);
            }
        }

        public bool TryDequeue(out T value)
        {
            lock (queue)
            {
                while (queue.Count == 0)
                {
                    if (closing)
                    {
                        value = default(T);
                        return false;
                    }
                    Monitor.Wait(queue);
                }
                value = queue.Dequeue();
                if (queue.Count == maxSize - 1)
                {
                    // wake up any blocked enqueue
                    Monitor.PulseAll(queue);
                }
                return true;
            }
        }
    }
}