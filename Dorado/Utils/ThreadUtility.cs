using Dorado.Extensions;
using System;
using System.Diagnostics.Contracts;
using System.Threading;
using Dictionary = System.Collections.Generic.Dictionary<string, object>;

namespace Dorado.Utils
{
    /// <summary>
    /// 线程工具类
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public static class ThreadUtility
    {
        private static readonly LocalDataStoreSlot _LocalDataStoreSlot = Thread.AllocateDataSlot();

        private static Dictionary _GetCurrentDictionary()
        {
            Dictionary dict = Thread.GetData(_LocalDataStoreSlot) as Dictionary;
            if (dict == null)
                Thread.SetData(_LocalDataStoreSlot, dict = new Dictionary());

            return dict;
        }

        /// <summary>
        /// 设置线程局部变量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetTlsData(string key, object value)
        {
            Contract.Requires(key != null);
            _GetCurrentDictionary()[key] = value;
        }

        /// <summary>
        /// 获取线程局部变量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetTlsData<T>(string key, T defaultValue = default(T))
            where T : class
        {
            Contract.Requires(key != null);
            return _GetCurrentDictionary().GetOrDefault(key, defaultValue) as T;
        }

        /// <summary>
        /// 获取或设置线程局部变量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="loader"></param>
        /// <returns></returns>
        public static T GetOrSetTlsData<T>(string key, Func<T> loader)
            where T : class
        {
            Contract.Requires(key != null);
            return _GetCurrentDictionary().GetOrSet(key, (item) => loader()) as T;
        }

        /// <summary>
        /// 获取线程局部变量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {
            Contract.Requires(key != null);
            return _GetCurrentDictionary().GetOrDefault(key);
        }

        /// <summary>
        /// 获取线程局部变量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="loader"></param>
        /// <returns></returns>
        public static object GetOrSetTlsData(string key, Func<object> loader)
        {
            Contract.Requires(key != null);
            return _GetCurrentDictionary().GetOrSet(key, (item) => loader());
        }
    }
}