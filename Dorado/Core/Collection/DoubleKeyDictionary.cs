using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Dorado.Core.Collection
{
    /// <summary>
    /// 一个支持两种类型键和值的集合。
    /// </summary>
    /// <typeparam name="Key1">字典中第一种键类型。</typeparam>
    /// <typeparam name="Key2">字典中第二种键类型。</typeparam>
    /// <typeparam name="TValue">字典中值的类型。</typeparam>
    public sealed class DoubleKeyDictionary<Key1, Key2, TValue> : ICollection<TValue>, IEnumerable<TValue>, IEnumerable
    {
        private IDictionary<Key1, TValue> m_Key1Dictionary;

        private IDictionary<Key2, TValue> m_Key2Dictionary;

        private IList<TValue> m_InnerList;

        private Func<TValue, Key1> m_Thunk1;

        private Func<TValue, Key2> m_Thunk2;

        /// <summary>
        /// 获取集合中包含的元素数。
        /// </summary>
        public int Count
        {
            get
            {
                return m_InnerList.Count;
            }
        }

        /// <summary>
        /// 获取内部列表的只读版本。
        /// </summary>
        public IReadOnlyList<TValue> InnerList
        {
            get
            {
                return new ReadOnlyCollection<TValue>(m_InnerList);
            }
        }

        /// <summary>
        /// 获取一个值，指示集合是否为只读。
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return m_InnerList.IsReadOnly;
            }
        }

        /// <summary>
        /// 通过键获取值。
        /// </summary>
        /// <param name="key">需要查找的键。</param>
        /// <returns>对应的值。</returns>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">找不到指定的键</exception>
        public TValue this[Key1 key]
        {
            get
            {
                return m_Key1Dictionary[key];
            }
        }

        /// <summary>
        /// 通过键获取值。
        /// </summary>
        /// <param name="key">需要查找的键。</param>
        /// <returns>对应的键。</returns>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">找不到指定的键</exception>
        public TValue this[Key2 key]
        {
            get
            {
                return m_Key2Dictionary[key];
            }
        }

        /// <summary>
        /// 创建一个集合的实例对象。
        /// </summary>
        /// <param name="thunk1">获取第一个键值。</param>
        /// <param name="thunk2">获取第二个键值。</param>
        public DoubleKeyDictionary(Func<TValue, Key1> thunk1, Func<TValue, Key2> thunk2)
            : this(thunk1, thunk2, EqualityComparer<Key1>.Default, EqualityComparer<Key2>.Default, null)
        {
        }

        /// <summary>
        /// 创建一个集合的实例对象。
        /// </summary>
        /// <param name="thunk1">获取第一个键值。</param>
        /// <param name="thunk2">获取第二个键值。</param>
        /// <param name="comparer1">第一个键比较器。</param>
        /// <param name="comparer2">第二个键比较器。</param>
        public DoubleKeyDictionary(Func<TValue, Key1> thunk1, Func<TValue, Key2> thunk2, IEqualityComparer<Key1> comparer1, IEqualityComparer<Key2> comparer2)
            : this(thunk1, thunk2, comparer1, comparer2, null)
        {
        }

        /// <summary>
        /// 创建一个集合的实例对象。
        /// </summary>
        /// <param name="thunk1">获取第一个键值。</param>
        /// <param name="thunk2">获取第二个键值。</param>
        /// <param name="sourceList">原始的集合。</param>
        public DoubleKeyDictionary(Func<TValue, Key1> thunk1, Func<TValue, Key2> thunk2, IEnumerable<TValue> sourceList)
            : this(thunk1, thunk2, EqualityComparer<Key1>.Default, EqualityComparer<Key2>.Default, sourceList)
        {
        }

        /// <summary>
        /// 创建一个集合的实例对象。
        /// </summary>
        /// <param name="thunk1">获取第一个键值。</param>
        /// <param name="thunk2">获取第二个键值。</param>
        /// <param name="comparer1">第一个键比较器。</param>
        /// <param name="comparer2">第二个键比较器。</param>
        /// <param name="sourceList">原始的集合。</param>
        public DoubleKeyDictionary(Func<TValue, Key1> thunk1, Func<TValue, Key2> thunk2, IEqualityComparer<Key1> comparer1, IEqualityComparer<Key2> comparer2, IEnumerable<TValue> sourceList)
        {
            if (comparer1 == null)
            {
                throw new ArgumentNullException("comparer1");
            }
            if (comparer2 == null)
            {
                throw new ArgumentNullException("comparer2");
            }
            m_Thunk1 = thunk1 ?? throw new ArgumentNullException("thunk1");
            m_Thunk2 = thunk2 ?? throw new ArgumentNullException("thunk2");
            m_Key1Dictionary = new Dictionary<Key1, TValue>(comparer1);
            m_Key2Dictionary = new Dictionary<Key2, TValue>(comparer2);
            m_InnerList = new List<TValue>();
            if (sourceList != null)
            {
                AddRange(sourceList);
            }
        }

        /// <summary>
        /// 将某项添加到集合中。
        /// </summary>
        /// <param name="item">新的项。</param>
        public void Add(TValue item)
        {
            m_InnerList.Add(item);
            m_Key1Dictionary.Add(m_Thunk1(item), item);
            m_Key2Dictionary.Add(m_Thunk2(item), item);
        }

        /// <summary>
        /// 向字典添加指定的集合元素。
        /// </summary>
        /// <param name="collection">指定的集合。</param>
        public void AddRange(IEnumerable<TValue> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection", "collection不能为null。");
            }
            foreach (TValue tValue in collection)
            {
                Key1 mThunk1 = m_Thunk1(tValue);
                Key2 mThunk2 = m_Thunk2(tValue);
                if (m_Key1Dictionary.ContainsKey(mThunk1) || m_Key2Dictionary.ContainsKey(mThunk2))
                {
                    object[] objArray = new object[] { mThunk1, mThunk2, tValue };
                }
                m_Key1Dictionary.Add(mThunk1, tValue);
                m_Key2Dictionary.Add(mThunk2, tValue);
                m_InnerList.Add(tValue);
            }
        }

        /// <summary>
        /// 从集合中移除所有项。
        /// </summary>
        public void Clear()
        {
            m_InnerList.Clear();
            m_Key1Dictionary.Clear();
            m_Key2Dictionary.Clear();
        }

        /// <summary>
        /// 确定集合中是否包含特定值。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(TValue item)
        {
            return m_InnerList.Contains(item);
        }

        /// <summary>
        /// 判断是否包含指定的键。
        /// </summary>
        /// <param name="key">指定的键。</param>
        /// <returns>存在返回true，否则返回false。</returns>
        public bool ContainsKey1(Key1 key)
        {
            return m_Key1Dictionary.ContainsKey(key);
        }

        /// <summary>
        /// 判断是否包含指定的值。
        /// </summary>
        /// <param name="key">指定的值。</param>
        /// <returns>存在返回true，否则返回false。</returns>
        public bool ContainsKey2(Key2 key)
        {
            return m_Key2Dictionary.ContainsKey(key);
        }

        /// <summary>
        /// 从特定的 Array 索引开始，将集合的元素复制到一个 Array 中。
        /// </summary>
        /// <param name="array">作为从集合复制的元素的目标位置的一维 Array。Array 必须具有从零开始的索引。</param>
        /// <param name="arrayIndex">array 中从零开始的索引，从此处开始复制。</param>
        public void CopyTo(TValue[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }
            if (array.Rank > 1)
            {
                throw new ArgumentException("array 是多维的。");
            }
            if (arrayIndex >= array.Length)
            {
                throw new ArgumentException("arrayIndex 等于或大于 array 的长度。");
            }
            if (Count > array.Length - arrayIndex)
            {
                throw new ArgumentException("源 ICollection 中的元素数目大于从 arrayIndex 到目标 array 末尾之间的可用空间。");
            }
            int num = arrayIndex;
            int num1 = 0;
            while (num < array.Length)
            {
                array[num] = m_InnerList[num1];
                num++;
                num1++;
            }
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        public IEnumerator<TValue> GetEnumerator()
        {
            return m_InnerList.GetEnumerator();
        }

        /// <summary>
        /// 通过键1获取值。
        /// </summary>
        /// <param name="key">需要查找的键。</param>
        /// <returns>对应的值。</returns>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">找不到指定的键</exception>
        public TValue GetValue1(Key1 key)
        {
            return m_Key1Dictionary[key];
        }

        /// <summary>
        /// 通过键2获取值。
        /// </summary>
        /// <param name="key">需要查找的键。</param>
        /// <returns>对应的值。</returns>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">找不到指定的键</exception>
        public TValue GetValue2(Key2 key)
        {
            return m_Key2Dictionary[key];
        }

        /// <summary>
        /// 从集合中移除特定对象的第一个匹配项。
        /// </summary>
        /// <param name="item">要从集合中移除的对象。</param>
        /// <returns>如果已从集合中成功移除 item，则为 true；否则为 false。如果在原始的集合中没有找到 item，该方法也会返回 false。</returns>
        public bool Remove(TValue item)
        {
            bool flag = m_InnerList.Remove(item);
            if (flag)
            {
                m_Key1Dictionary.Remove(m_Thunk1(item));
                m_Key2Dictionary.Remove(m_Thunk2(item));
            }
            return flag;
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_InnerList.GetEnumerator();
        }
    }
}