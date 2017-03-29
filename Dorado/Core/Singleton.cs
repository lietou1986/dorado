using System;
using System.Collections.Generic;

namespace Dorado.Core
{
    /// <summary>
    /// 单例对象惰性创建基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazySingleton<T> where T : new()
    {
        private static readonly Lazy<T> instance = new Lazy<T>(
            Activator.CreateInstance<T>,
                true);

        public static T Instance
        {
            get { return instance.Value; }
        }
    }

    public class Singleton<T> : Singleton
    {
        private static T instance;

        public static T Instance
        {
            get { return instance; }
            set
            {
                instance = value;
                AllSingletons[typeof(T)] = value;
            }
        }
    }

    public class SingletonList<T> : Singleton<IList<T>>
    {
        static SingletonList()
        {
            Singleton<IList<T>>.Instance = new List<T>();
        }

        public new static IList<T> Instance
        {
            get { return Singleton<IList<T>>.Instance; }
        }
    }

    public class SingletonDictionary<TKey, TValue> : Singleton<IDictionary<TKey, TValue>>
    {
        static SingletonDictionary()
        {
            Singleton<Dictionary<TKey, TValue>>.Instance = new Dictionary<TKey, TValue>();
        }

        public new static IDictionary<TKey, TValue> Instance
        {
            get { return Singleton<Dictionary<TKey, TValue>>.Instance; }
        }
    }

    public class Singleton
    {
        static Singleton()
        {
            allSingletons = new Dictionary<Type, object>();
        }

        private static readonly IDictionary<Type, object> allSingletons;

        /// <summary>Dictionary of type to singleton instances.</summary>
        public static IDictionary<Type, object> AllSingletons
        {
            get { return allSingletons; }
        }
    }
}