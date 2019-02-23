using Dorado.Core;
using Dorado.Core.ComponentModel;
using Dorado.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;

namespace Dorado.Utils
{
    public static partial class CommonUtility
    {
        public static bool TryConvert<T>(object value, out T convertedValue)
        {
            return TryConvert<T>(value, CultureInfo.InvariantCulture, out convertedValue);
        }

        public static bool TryConvert<T>(object value, CultureInfo culture, out T convertedValue)
        {
            return TryAction<T>(() => value.Convert<T>(culture), out convertedValue);
        }

        public static bool TryConvert(object value, Type to, out object convertedValue)
        {
            return TryConvert(value, to, CultureInfo.InvariantCulture, out convertedValue);
        }

        public static bool TryConvert(object value, Type to, CultureInfo culture, out object convertedValue)
        {
            return TryAction<object>(() => value.Convert(to, culture), out convertedValue);
        }

        public static IDictionary<string, object> ObjectToDictionary(object obj)
        {
            Guard.ArgumentNotNull(() => obj);

            return FastProperty.ObjectToDictionary(
                obj,
                key => key.Replace("_", "-"));
        }

        /// <summary>
        /// Gets a setting from the application's <c>web.config</c> <c>appSettings</c> node
        /// </summary>
        /// <typeparam name="T">The type to convert the setting value to</typeparam>
        /// <param name="key">The key of the setting</param>
        /// <param name="defValue">The default value to return if the setting does not exist</param>
        /// <returns>The casted setting value</returns>
        public static T GetAppSetting<T>(string key, T defValue = default(T))
        {
            Guard.ArgumentNotEmpty(() => key);

            var setting = ConfigurationManager.AppSettings[key];

            if (setting == null)
            {
                return defValue;
            }

            return setting.Convert<T>();
        }

        private static bool TryAction<T>(Func<T> func, out T output)
        {
            Guard.ArgumentNotNull(() => func);

            try
            {
                output = func();
                return true;
            }
            catch
            {
                output = default(T);
                return false;
            }
        }

        /// <summary>
        /// 生成结合时间戳的GUID
        /// </summary>
        /// <returns></returns>
        public static Guid GenerateGuid()
        {
            byte[] guidArray = Guid.NewGuid().ToByteArray();

            DateTime baseDate = new DateTime(1900, 1, 1);

            DateTime now = DateTime.Now;

            // Get the days and milliseconds which will be used to build

            //the byte string

            TimeSpan days = new TimeSpan(now.Ticks - baseDate.Ticks);

            TimeSpan msecs = now.TimeOfDay;

            // Convert to a byte array

            // Note that SQL Server is accurate to 1/300th of a

            // millisecond so we divide by 3.333333

            byte[] daysArray = BitConverter.GetBytes(days.Days);

            byte[] msecsArray = BitConverter.GetBytes((long)

              (msecs.TotalMilliseconds / 3.333333));

            // Reverse the bytes to match SQL Servers ordering

            Array.Reverse(daysArray);

            Array.Reverse(msecsArray);

            // Copy the bytes into the guid

            Array.Copy(daysArray, daysArray.Length - 2, guidArray,

              guidArray.Length - 6, 2);

            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray,

              guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }

        /// <summary>
        /// 生成Id
        /// </summary>
        /// <returns></returns>
        public static long GenerateGID()
        {
            return GenerateGuid().ToLong();
        }

        /// <summary>
        /// 生成雪花Id
        /// </summary>
        /// <returns></returns>
        public static long GenerateId()
        {
            return Snowflake.Instance().GetId();
        }
    }
}