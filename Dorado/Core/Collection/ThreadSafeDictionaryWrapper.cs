using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Dorado.Core.Collection
{
    /// <summary>
    /// 线程安全的Dictionary
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public class ThreadSafeDictionaryWrapper<TKey, TValue> : IDictionary<TKey, TValue>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="innerDict">内含的Dict</param>
        public ThreadSafeDictionaryWrapper(IDictionary<TKey, TValue> innerDict = null)
        {
            _Dict = innerDict ?? new Dictionary<TKey, TValue>();
        }

        private readonly IDictionary<TKey, TValue> _Dict;
        private readonly RwLocker _RwLocker = new RwLocker();

        #region IDictionary<TKey,TValue> Members

        /// <summary>
        /// 添加一组键值对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            using (_RwLocker.Write())
            {
                _Dict.Add(key, value);
            }
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            Contract.Requires(items != null);

            using (_RwLocker.Write())
            {
                foreach (var item in items)
                {
                    _Dict.Add(item.Key, item.Value);
                }
            }
        }

        /// <summary>
        /// 是否包含指定的键值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return _Dict.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return _Dict.Keys; }
        }

        /// <summary>
        /// 删除指定键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            using (_RwLocker.Write())
            {
                return _Dict.Remove(key);
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="keys"></param>
        public void RemoveRange(IEnumerable<TKey> keys)
        {
            Contract.Requires(keys != null);

            using (_RwLocker.Write())
            {
                foreach (TKey key in keys)
                {
                    _Dict.Remove(key);
                }
            }
        }

        /// <summary>
        /// 获取一项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _Dict.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return _Dict.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return _Dict[key];
            }
            set
            {
                using (_RwLocker.Write())
                {
                    _Dict[key] = value;
                }
            }
        }

        #endregion IDictionary<TKey,TValue> Members

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            using (_RwLocker.Write())
            {
                ((IDictionary<TKey, TValue>)_Dict).Add(item);
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            using (_RwLocker.Write())
            {
                _Dict.Clear();
            }
        }

        /// <summary>
        /// 是否包含指定的键值对
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            using (_RwLocker.Read())
            {
                return ((ICollection<KeyValuePair<TKey, TValue>>)_Dict).Contains(item);
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            using (_RwLocker.Read())
            {
                ((ICollection<KeyValuePair<TKey, TValue>>)_Dict).CopyTo(array, arrayIndex);
            }
        }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count
        {
            get { return _Dict.Count; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<TKey, TValue>>)_Dict).IsReadOnly; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_Dict).Remove(item);
        }

        #endregion ICollection<KeyValuePair<TKey,TValue>> Members

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            using (_RwLocker.Read())
            {
                foreach (var item in _Dict)
                {
                    yield return item;
                }
            }
        }

        #endregion IEnumerable<KeyValuePair<TKey,TValue>> Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}