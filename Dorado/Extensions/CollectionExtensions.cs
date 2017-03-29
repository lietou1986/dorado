using Dorado.Core.Collection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Dorado.Extensions
{
    /// <summary>
    /// 数组工具集
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public static class CollectionExtensions
    {
        public static IList<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
        {
            return new ReadOnlyCollection<T>(enumerable.ToList());
        }

        public static IEnumerable<TTarget> SafeCast<TTarget>(this IEnumerable source)
        {
            return source == null ? null : source.Cast<TTarget>();
        }

        /// <summary>
        /// 检查集合为空引用或空集
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="list">集合</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }

        /// <summary>
        /// 从Dictionary中读取指定键的值，如果不存在则返回默认值
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            Contract.Requires(dict != null);

            if (key == null)
                return defaultValue;

            TValue value;
            if (!dict.TryGetValue(key, out value))
                return defaultValue;

            return value;
        }

        /// <summary>
        /// 从Dictionary中读取指定键的值，如果不存在则调用creator来创建
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public static TValue GetOrSet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> creator)
        {
            Contract.Requires(dict != null && key != null && creator != null);

            TValue value;
            if (dict.TryGetValue(key, out value))
                return value;

            lock (dict)
            {
                if (!dict.TryGetValue(key, out value))
                {
                    dict.Add(key, value = creator(key));
                }
            }

            return value;
        }

        /// <summary>
        /// 从Dictionary中读取指定键的值，如果不存在则创建
        /// </summary>
        /// <typeparam name="TKey">键</typeparam>
        /// <typeparam name="TValue">值</typeparam>
        /// <param name="dict">Dictionary</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static TValue GetOrSet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
            where TValue : new()
        {
            return GetOrSet(dict, key, (item) => new TValue());
        }

        /// <summary>
        /// 从指定的Dictionary中读取指定的值，如果不存在则调用creator来创建
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <typeparam name="TArg">其它参数类型</typeparam>
        /// <param name="dict">哈希表</param>
        /// <param name="key">键</param>
        /// <param name="arg">其它参数</param>
        /// <param name="creator">创建器</param>
        /// <returns></returns>
        public static TValue GetOrSet<TKey, TValue, TArg>(this IDictionary<TKey, TValue> dict, TKey key, TArg arg, Func<TKey, TArg, TValue> creator)
        {
            Contract.Requires(dict != null && key != null && creator != null);

            TValue value;
            if (dict.TryGetValue(key, out value))
                return value;

            lock (dict)
            {
                if (!dict.TryGetValue(key, out value))
                {
                    dict.Add(key, value = creator(key, arg));
                }
            }

            return value;
        }

        /// <summary>
        /// 从Dictionary删除指定键的元素
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="dict">Dictionary</param>
        /// <param name="keys">要删除的键</param>
        /// <returns>已删除的个数</returns>
        public static int RemoveRange<TKey, TValue>(this IDictionary<TKey, TValue> dict, IEnumerable<TKey> keys)
        {
            Contract.Requires(dict != null && keys != null);

            return keys.Count(key => dict.Remove(key));
        }

        /// <summary>
        /// 将Dictionary转换为线程安全版本
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static ThreadSafeDictionaryWrapper<TKey, TValue> ToThreadSafe<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            Contract.Requires(dict != null);

            return dict as ThreadSafeDictionaryWrapper<TKey, TValue> ?? new ThreadSafeDictionaryWrapper<TKey, TValue>(dict);
        }

        /// <summary>
        /// 对集合中的元素执行指定的操作
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="source">集合</param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            Contract.Requires(source != null && action != null);

            foreach (T item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// 将集合转换为HashSet类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            Contract.Requires(source != null);

            HashSet<T> hashSet = source as HashSet<T>;
            if (hashSet != null)
                return hashSet;

            return new HashSet<T>(source);
        }

        /// <summary>
        /// 将集合转换为Queue类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Queue<T> ToQueue<T>(this IEnumerable<T> source)
        {
            Contract.Requires(source != null);

            Queue<T> queue = source as Queue<T>;
            if (queue != null)
                return queue;

            return new Queue<T>(source);
        }

        /// <summary>
        /// 将集合转换为Stack类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Stack<T> ToStack<T>(this IEnumerable<T> source)
        {
            Contract.Requires(source != null);

            Stack<T> stack = source as Stack<T>;
            if (stack != null)
                return stack;

            return new Stack<T>(source);
        }

        /// <summary>
        /// 截取指定范围之内元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> Range<T>(this IEnumerable<T> source, int start, int count = int.MaxValue)
        {
            Contract.Requires(count >= 0 && start >= 0);

            IList<T> list = source as IList<T>;
            if (list != null)
            {
                for (int k = 0, maxCount = Math.Min(list.Count, count); k < maxCount; k++)
                {
                    yield return list[start + k];
                }
            }
            else
            {
                int index = 0, end = index + count;
                foreach (T item in source)
                {
                    if (index >= start)
                    {
                        if (index++ < end)
                            yield return item;
                        else
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 将集合中的元素拷贝到指定的列表中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="destList"></param>
        /// <param name="arrayIndex"></param>
        public static void CopyTo<T>(this IEnumerable<T> source, IList<T> destList, int arrayIndex = 0)
        {
            Contract.Requires(source != null && destList != null && arrayIndex >= 0);

            foreach (T item in source)
            {
                destList[arrayIndex++] = item;
            }
        }

        /// <summary>
        /// 选择集合中不为空的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source)
        {
            Contract.Requires(source != null);

            return source.Where(v => v != null);
        }

        /// <summary>
        /// 向指定的Dictionary中批量添加元素
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="dict">Dictionary</param>
        /// <param name="items">需要添加的元素</param>
        /// <param name="keySelector">键选择器</param>
        /// <param name="valueSelector">值选择器</param>
        /// <returns>添加的个数</returns>
        public static int AddRange<TKey, TValue, T>(this IDictionary<TKey, TValue> dict,
            IEnumerable<T> items, Func<T, TKey> keySelector, Func<T, TValue> valueSelector, bool ignoreDupKey = false)
        {
            Contract.Requires(dict != null && items != null && keySelector != null && valueSelector != null);
            int count = 0;
            if (ignoreDupKey)
            {
                foreach (T item in items)
                {
                    dict[keySelector(item)] = valueSelector(item);
                    count++;
                }
            }
            else
            {
                foreach (T item in items)
                {
                    dict.Add(keySelector(item), valueSelector(item));
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// 向指定的Dictionary中批量添加元素
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="dict">Dictionary</param>
        /// <param name="items">需要添加的元素</param>
        /// <param name="keySelector">键选择器</param>
        /// <param name="clearFirst">是否在添加之前先清空Dictionary</param>
        /// <returns>添加的个数</returns>
        public static int AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dict,
            IEnumerable<TValue> items, Func<TValue, TKey> keySelector, bool ignoreDupKey = false, bool clearFirst = false)
        {
            Contract.Requires(dict != null && items != null && keySelector != null);

            if (clearFirst)
                dict.Clear();

            int count = 0;
            if (ignoreDupKey)
            {
                foreach (TValue item in items)
                {
                    dict[keySelector(item)] = item;
                    count++;
                }
            }
            else
            {
                foreach (TValue item in items)
                {
                    dict.Add(keySelector(item), item);
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// 将一个Dictionary中的所有值添加到另一个Dictionary中
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="dict"></param>
        /// <param name="dict2"></param>
        /// <param name="ignoreDupKey">是否忽略掉重复的键</param>
        /// <returns>已添加的元素个数</returns>
        public static int AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dict,
            IDictionary<TKey, TValue> dict2, bool ignoreDupKey = false, bool clearFirst = false)
        {
            Contract.Requires(dict != null && dict2 != null);

            if (clearFirst)
                dict.Clear();

            int count = 0;
            if (ignoreDupKey)
            {
                foreach (var item in dict2)
                {
                    dict[item.Key] = item.Value;
                    count++;
                }
            }
            else
            {
                foreach (var item in dict2)
                {
                    dict.Add(item.Key, item.Value);
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// 将集合转化为Dictionary
        /// </summary>
        /// <typeparam name="TItem">集合元素类型</typeparam>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="source">集合</param>
        /// <param name="keySelector">键选择器</param>
        /// <param name="valueSelector">值选择器</param>
        /// <param name="ignoreDupKeys">是否忽略掉重复的键</param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ToDictionary<TItem, TKey, TValue>(this IEnumerable<TItem> source,
            Func<TItem, TKey> keySelector, Func<TItem, TValue> valueSelector, bool ignoreDupKeys)
        {
            Contract.Requires(source != null && keySelector != null && valueSelector != null);

            if (!ignoreDupKeys)
                return source.ToDictionary(keySelector, valueSelector);

            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
            foreach (var item in source)
            {
                TKey key = keySelector(item);
                if (key != null)
                    dict[key] = valueSelector(item);
            }

            return dict;
        }

        /// <summary>
        /// 将集合转化为Dictionary
        /// </summary>
        /// <typeparam name="TItem">元素类型</typeparam>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <param name="source">集合</param>
        /// <param name="keySelector">键选择器</param>
        /// <param name="ignoreDupKeys">是否忽略掉重复的键</param>
        /// <returns></returns>
        public static Dictionary<TKey, TItem> ToDictionary<TItem, TKey>(this IEnumerable<TItem> source,
            Func<TItem, TKey> keySelector, bool ignoreDupKeys)
        {
            Contract.Requires(source != null && keySelector != null);

            if (!ignoreDupKeys)
                return source.ToDictionary(keySelector);

            Dictionary<TKey, TItem> dict = new Dictionary<TKey, TItem>();
            foreach (var item in source)
            {
                TKey key = keySelector(item);
                if (key != null)
                    dict[key] = item;
            }

            return dict;
        }

        /// <summary>
        /// 向队列中批量添加元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static int AddRange<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            Contract.Requires(queue != null && items != null);

            int count = 0;
            foreach (T item in items)
            {
                queue.Enqueue(item);
                count++;
            }

            return count;
        }

        /// <summary>
        /// 向堆栈中批量添加元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stack"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static int AddRange<T>(this Stack<T> stack, IEnumerable<T> items)
        {
            Contract.Requires(stack != null && items != null);

            int count = 0;
            foreach (T item in items)
            {
                stack.Push(item);
                count++;
            }

            return count;
        }

        /// <summary>
        /// 向HashSet中批量添加元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashSet"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static int AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> items)
        {
            Contract.Requires(hashSet != null && items != null);

            int count = 0;
            foreach (T item in items)
            {
                hashSet.Add(item);
                count++;
            }

            return count;
        }

        /// <summary>
        /// 返回除去指定元素的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IEnumerable<T> Except<T>(this IEnumerable<T> source, params T[] items)
        {
            Contract.Requires(source != null);

            return Enumerable.Except(source, items);
        }

        /// <summary>
        /// 返回除去指定元素的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="comparer"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IEnumerable<T> Except<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer, params T[] items)
        {
            Contract.Requires(source != null && comparer != null);

            return Enumerable.Except(source, items, comparer);
        }

        /// <summary>
        /// 将指定的集合与指定的元素组合到一起
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, params T[] items)
        {
            Contract.Requires(source != null);

            return Enumerable.Concat(source, items);
        }

        /// <summary>
        /// 删除符合条件的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="condition"></param>
        public static int RemoveWhere<T>(this IList<T> list, Func<T, bool> condition)
        {
            Contract.Requires(list != null && condition != null);

            int count = 0;
            for (int k = 0; k < list.Count; )
            {
                if (condition(list[k]))
                {
                    count++;
                    list.RemoveAt(k);
                }
                else
                {
                    k++;
                }
            }

            return count;
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return (source == null || source.Count == 0);
        }

        public static bool EqualsAll<T>(this IList<T> a, IList<T> b)
        {
            if (a == null || b == null)
                return (a == null && b == null);

            if (a.Count != b.Count)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            return !a.Where((t, i) => !comparer.Equals(t, b[i])).Any();
        }

        public static void AddRange<T>(this ICollection<T> initial, IEnumerable<T> other)
        {
            if (other == null)
                return;

            var list = initial as List<T>;

            if (list != null)
            {
                list.AddRange(other);
                return;
            }

            other.Each(x => initial.Add(x));
        }
    }
}