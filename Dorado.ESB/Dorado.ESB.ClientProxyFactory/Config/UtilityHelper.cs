using System;
using System.Text;

namespace Dorado.ESB.ClientProxyFactory.Config
{
    internal class UtilityHelper
    {
        public static TimeSpan GetTimeSpanAttribute(string elm)
        {
            if (string.IsNullOrEmpty(elm))
                return new TimeSpan();
            else
                return TimeSpan.Parse(elm);
        }

        public static T GetEnumAttribute<T>(string elm)
        {
            if (string.IsNullOrEmpty(elm))
                return default(T);
            else
                return (T)Enum.Parse(typeof(T), elm);
        }

        public static Encoding GetEncodingAttribute(string elm)
        {
            if (string.IsNullOrEmpty(elm))
                return Encoding.UTF8;
            else
                return Encoding.GetEncoding(elm);
        }

        public static Uri GetUriAttribute(string elm)
        {
            if (string.IsNullOrEmpty(elm))
                return null;
            else
                return new Uri(elm);
        }
    }
}