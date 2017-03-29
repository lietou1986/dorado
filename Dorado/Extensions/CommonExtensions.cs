using Dorado.Core;
using Dorado.Core.GlobalTimer.TimerStrategies;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Dorado.Extensions
{
    /// <summary>
    /// 公用的工具类
    /// </summary>
    [DebuggerStepThrough]
    public static class CommonExtension
    {
        static CommonExtension()
        {
            Global.GlobalTimer.Add(new GlobalTimerIntervalTimerStrategy(), new TaskFuncAdapter(_UpdateCurrentTime));
            _UpdateCurrentTime();
        }

        private static void _UpdateCurrentTime()
        {
            _now = DateTime.Now;
        }

        private static DateTime _now;

        /// <summary>
        /// 获取对象的属性，如果对象为空则返回默认值
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="getter"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Get<TObj, T>(this TObj obj, Func<TObj, T> getter, T defaultValue)
        {
            Contract.Requires(getter != null);

            if (obj == null)
                return defaultValue;

            return getter(obj);
        }

        /// <summary>
        /// 获取对象的属性，如果对象为空则返回默认值
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static T Get<TObj, T>(this TObj obj, Func<TObj, T> getter)
        {
            return Get(obj, getter, default(T));
        }

        /// <summary>
        /// 在精度要求不高的情况下，快速获取当前时间
        /// </summary>
        /// <returns></returns>
        public static DateTime QuickGetTime()
        {
            return _now;
        }

        /// <summary>
        /// 从app.config中加载值并转换为指定的类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetFromAppConfig<T>(string key, T defaultValue = default(T))
        {
            string value = ConfigurationManager.AppSettings.Get(key);
            if (value == null)
                return defaultValue;

            return value.ToType<T>(defaultValue);
        }

        public static long ToLong(this Guid guid)
        {
            byte[] buffer = guid.ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }
    }
}