using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Dorado.Core.Collection
{
    /// <summary>
    /// 表示一个可以周而复始的环。
    /// </summary>
    /// <typeparam name="T">环中元素类型。</typeparam>
    public sealed class Loop<T> : IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
    {
        private readonly object m_LockObject=new object();

        private readonly bool m_IsSynchronized;

        private int m_CurrentIndex;

        private List<T> m_InnerList;

        /// <summary>
        /// 获取环中元素的数量。
        /// </summary>
        public int Count
        {
            get
            {
                return m_InnerList.Count;
            }
        }

        /// <summary>
        /// 获取一个值，该值指示是否同步对当前实例的访问（线程安全）。
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return m_IsSynchronized;
            }
        }

        /// <summary>
        /// 获取环中指定位置的元素，位置由环的初始集合决定。
        /// </summary>
        /// <param name="index">指定位置。</param>
        /// <returns>指定位置的元素。</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index小于0或者大于环的上限。</exception>
        public T this[int index]
        {
            get
            {
                return m_InnerList[index];
            }
        }

        /// <summary>
        /// 使用指定的集合初始化一个非线程安全的环。
        /// </summary>
        /// <param name="source">源集合。</param>
        public Loop(IEnumerable<T> source)
            : this(source, false)
        {
        }

        /// <summary>
        /// 使用指定的集合初始化一个环，并指定是否线程安全。
        /// </summary>
        /// <param name="source">源集合。</param>
        /// <param name="isSynchronized">true为线程安全，否则指定false。</param>
        public Loop(IEnumerable<T> source, bool isSynchronized)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            m_InnerList = new List<T>(source);
            if (m_InnerList.Count == 0)
            {
                throw new ArgumentException("环中元素数量不能为0");
            }
            m_IsSynchronized = isSynchronized;
        }

        /// <summary>
        /// 获取环中下一个元素。
        /// </summary>
        /// <returns>下一个元素。</returns>
        public T Next()
        {
            if (IsSynchronized)
            {
                Monitor.TryEnter(m_LockObject);
            }
            T item = m_InnerList[m_CurrentIndex];
            Loop<T> mCurrentIndex = this;
            mCurrentIndex.m_CurrentIndex = mCurrentIndex.m_CurrentIndex + 1;
            if (m_CurrentIndex >= m_InnerList.Count)
            {
                m_CurrentIndex = 0;
            }
            if (IsSynchronized)
            {
                Monitor.Exit(m_LockObject);
            }
            return item;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            yield return Next();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return Next();
        }
    }
}