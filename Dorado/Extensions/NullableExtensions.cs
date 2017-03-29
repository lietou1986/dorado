using System.Text;

namespace Dorado.Extensions
{
    public static class NullableExtensions
    {
        public static int Val(this int? thisInt, int defualtValue)
        {
            if (!thisInt.HasValue)
                return defualtValue;
            return thisInt.Value;
        }

        public static int Val(this int? thisInt)
        {
            return thisInt.Val(0);
        }

        public static long Val(this long? thisLong, long defaultValue)
        {
            long? num = thisLong;
            if (!num.HasValue)
                return defaultValue;
            return num.GetValueOrDefault();
        }

        public static long Val(this long? thisLong)
        {
            return thisLong.Val(0L);
        }

        public static uint Val(this uint? thisUint, uint defaultValue)
        {
            uint? num = thisUint;
            if (!num.HasValue)
                return defaultValue;
            return num.GetValueOrDefault();
        }

        public static uint Val(this uint? thisUint)
        {
            return thisUint.Val(0u);
        }

        public static string Val(this string thisString, string defaultValue)
        {
            return thisString ?? defaultValue;
        }

        public static string Val(this string thisString)
        {
            return thisString.Val(string.Empty);
        }

        public static string ToSqlParameter(this string thisString)
        {
            return thisString.Replace("'", "''");
        }

        public static bool IsNaN(this string thisString)
        {
            return string.IsNullOrEmpty(thisString);
        }

        internal static int CharCount(this string thisStr)
        {
            int count = 0;
            for (int i = 0; i < thisStr.Length; i++)
            {
                char ch = thisStr[i];
                if (ch > 'ÿ')
                    count += 2;
                else
                    count++;
            }
            return count;
        }

        public static string Cutout(this string thisString, int maxLength)
        {
            if (thisString.IsNaN())
                return thisString.Val();
            int len = thisString.CharCount();
            if (len <= maxLength)
                return thisString;
            StringBuilder builder = new StringBuilder(len);
            int i = 0;
            len = 0;
            int max = maxLength - 3;
            while (i < max)
            {
                char ch = thisString[len];
                len++;
                builder.Append(ch);
                if (ch > 'ÿ')
                    i += 2;
                else
                    i++;
            }
            builder.Append("...");
            return builder.ToString();
        }

        public static string EscapeSql(this string str)
        {
            if (str != null)
                return str.Replace("[", "[[]").Replace("]", "[]]").Replace("_", "[_]").Replace("%", "[%]");
            return str;
        }
    }
}