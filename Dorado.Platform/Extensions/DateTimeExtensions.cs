using System;

namespace Dorado.Platform.Extensions
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime GetTime(this string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime); return dtStart.Add(toNow);
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"></param>
        /// <param name="toMilliseconds">是否精确到毫秒</param>
        /// <returns></returns>
        public static long GetUnixTime(this DateTime time, bool toMilliseconds = false)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            if (toMilliseconds)
                return (long)(time - startTime).TotalMilliseconds;
            return (long)(time - startTime).TotalSeconds;
        }

        public static DateTime GetMinDay(this DateTime time)
        {
            var day = new DateTime(time.Year, time.Month, time.Day);
            return day;
        }

        public static DateTime GetMaxDay(this DateTime time)
        {
            var day = new DateTime(time.Year, time.Month, time.Day, 23, 59, 59);
            return day;
        }
    }
}