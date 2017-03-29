using System;
using System.Collections.Generic;
using System.Linq;

namespace Dorado.Core.Collection
{
    /// <summary>
    /// 优先级队列
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <typeparam name="TPriority">优先级数值类型</typeparam>
    public class PriorityQueue<T, TPriority>
        where T : IPriority<TPriority>
        where TPriority : IComparable<TPriority>
    {
        private readonly SortedDictionary<TPriority, ValueContainer> _Dict = new SortedDictionary<TPriority, ValueContainer>();

        private class ValueContainer
        {
            public Queue<T> Queue;

            public T SingleItem;
        }

        /// <summary>
        /// 将一个元素添加到队列中
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            ValueContainer valueContainer;
            TPriority priority = item.GetPriority();
            if (_Dict.TryGetValue(priority, out valueContainer))
            {
                if (valueContainer.Queue != null)
                {
                    valueContainer.Queue.Enqueue(item);
                }
                else
                {
                    var q = new Queue<T>();
                    valueContainer.Queue = q;

                    q.Enqueue(valueContainer.SingleItem);
                    q.Enqueue(item);

                    valueContainer.SingleItem = default(T);
                }
            }
            else
            {
                _Dict.Add(priority, new ValueContainer { SingleItem = item });
            }

            _Count++;
        }

        /// <summary>
        /// 从队列中取出一个元素
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            if (_Dict.Count == 0)
                throw new InvalidOperationException("Queue is empty!");

            T result;

            var pair = _Dict.First();
            ValueContainer valueContainer = pair.Value;
            if (valueContainer.Queue != null)
            {
                result = valueContainer.Queue.Dequeue();
                if (valueContainer.Queue.Count == 0)
                    _Dict.Remove(pair.Key);
            }
            else
            {
                result = valueContainer.SingleItem;
                _Dict.Remove(pair.Key);
            }

            _Count--;
            return result;
        }

        /// <summary>
        /// 获得队首的元素，但并不从队列中取出
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            if (_Dict.Count == 0)
                throw new InvalidOperationException("Queue is empty!");

            var pair = _Dict.First();
            ValueContainer valueContainer = pair.Value;
            return valueContainer.Queue != null ? valueContainer.Queue.Peek() : valueContainer.SingleItem;
        }

        /// <summary>
        /// 拷贝到指定的
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="arrayIndex">起始位置</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if ((uint)(arrayIndex + _Count) >= array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");

            foreach (var vc in _Dict.Values)
            {
                if (vc.Queue != null)
                {
                    vc.Queue.CopyTo(array, arrayIndex);
                    arrayIndex += vc.Queue.Count;
                }
                else
                {
                    array[arrayIndex++] = vc.SingleItem;
                }
            }
        }

        /// <summary>
        /// 转换为数组
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            T[] array = new T[_Count];
            CopyTo(array, 0);
            return array;
        }

        private int _Count = 0;

        /// <summary>
        /// 数量
        /// </summary>
        public int Count
        {
            get { return _Count; }
        }

        /// <summary>
        /// 清空所有元素
        /// </summary>
        public void Clear()
        {
            _Dict.Clear();
        }

        /// <summary>
        /// 是否包含指定的元素
        /// </summary>
        /// <param name="item">元素</param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            if (item == null)
                return false;

            ValueContainer valueContainer;
            if (!_Dict.TryGetValue(item.GetPriority(), out valueContainer))
                return false;

            return _ContainsInValueContainer(item, valueContainer);
        }

        private static bool _ContainsInValueContainer(T item, ValueContainer valueContainer)
        {
            if (valueContainer.Queue == null)
                return object.Equals(valueContainer.SingleItem, item);
            else
                return valueContainer.Queue.Contains(item);
        }
    }
}