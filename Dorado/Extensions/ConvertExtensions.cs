using Dorado.Core.ComponentModel;
using Dorado.Utils;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Dorado.Extensions
{
    /// <summary>
    /// 数据转换工具类
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public static class ConvertExtensions
    {
        /// <summary>
        /// 转换为Int16
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt16(this string value, short defaultValue = 0)
        {
            short result;
            if (short.TryParse(value, out result))
                return result;

            return defaultValue;
        }

        /// <summary>
        /// 转换为Int32
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt32(this string value, int defaultValue = 0)
        {
            int result;
            if (int.TryParse(value, out result))
                return result;

            return defaultValue;
        }

        /// <summary>
        /// 转换为Int64
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ToInt64(this string value, long defaultValue = 0)
        {
            long result;
            if (long.TryParse(value, out result))
                return result;

            return defaultValue;
        }

        /// <summary>
        /// 转换为System.Signle
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float ToSingle(this string value, float defaultValue = 0.0f)
        {
            float result;
            if (float.TryParse(value, out result))
                return result;

            return defaultValue;
        }

        /// <summary>
        /// 转换为System.Double
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ToDouble(this string value, double defaultValue = 0.0)
        {
            double result;
            if (double.TryParse(value, out result))
                return result;

            return defaultValue;
        }

        /// <summary>
        /// 转化为指定的类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T ToType<T>(this object value, T defaultValue = default(T))
        {
            try
            {
                return (T)System.Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        #region Object

        public static T Convert<T>(this object value)
        {
            return (T)(Convert(value, typeof(T)) ?? default(T));
        }

        public static T Convert<T>(this object value, T defaultValue)
        {
            return (T)(Convert(value, typeof(T)) ?? defaultValue);
        }

        public static T Convert<T>(this object value, CultureInfo culture)
        {
            return (T)(Convert(value, typeof(T), culture) ?? default(T));
        }

        public static T Convert<T>(this object value, T defaultValue, CultureInfo culture)
        {
            return (T)(Convert(value, typeof(T), culture) ?? defaultValue);
        }

        public static object Convert(this object value, Type to)
        {
            return value.Convert(to, CultureInfo.InvariantCulture);
        }

        public static object Convert(this object value, Type to, CultureInfo culture)
        {
            Guard.ArgumentNotNull(to, "to");

            if (value == null || value == DBNull.Value || to.IsInstanceOfType(value))
            {
                return value == DBNull.Value ? null : value;
            }

            Type from = value.GetType();

            if (culture == null)
            {
                culture = CultureInfo.InvariantCulture;
            }

            // get a converter for 'to' (value -> to)
            var converter = TypeConverterFactory.GetConverter(to);
            if (converter != null && converter.CanConvertFrom(from))
            {
                return converter.ConvertFrom(culture, value);
            }

            // try the other way round with a 'from' converter (to <- from)
            converter = TypeConverterFactory.GetConverter(from);
            if (converter != null && converter.CanConvertTo(to))
            {
                return converter.ConvertTo(culture, null, value, to);
            }

            // use Convert.ChangeType if both types are IConvertible
            if (value is IConvertible && typeof(IConvertible).IsAssignableFrom(to))
            {
                return System.Convert.ChangeType(value, to, culture);
            }

            throw Error.InvalidCast(from, to);
        }

        #endregion

        #region int

        public static char ToHex(this int value)
        {
            if (value <= 9)
            {
                return (char)(value + 48);
            }
            return (char)((value - 10) + 97);
        }

        /// <summary>
        /// Returns kilobytes
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToKb(this int value)
        {
            return value * 1024;
        }

        /// <summary>
        /// Returns megabytes
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToMb(this int value)
        {
            return value * 1024 * 1024;
        }

        /// <summary>Returns a <see cref="TimeSpan"/> that represents a specified number of minutes.</summary>
        /// <param name="minutes">number of minutes</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        /// <example>3.Minutes()</example>
        public static TimeSpan ToMinutes(this int minutes)
        {
            return TimeSpan.FromMinutes(minutes);
        }

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> that represents a specified number of seconds.
        /// </summary>
        /// <param name="seconds">number of seconds</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        /// <example>2.Seconds()</example>
        public static TimeSpan ToSeconds(this int seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> that represents a specified number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">milliseconds for this timespan</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        public static TimeSpan ToMilliseconds(this int milliseconds)
        {
            return TimeSpan.FromMilliseconds(milliseconds);
        }

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> that represents a specified number of days.
        /// </summary>
        /// <param name="days">Number of days.</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        public static TimeSpan ToDays(this int days)
        {
            return TimeSpan.FromDays(days);
        }

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> that represents a specified number of hours.
        /// </summary>
        /// <param name="hours">Number of hours.</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        public static TimeSpan ToHours(this int hours)
        {
            return TimeSpan.FromHours(hours);
        }

        #endregion int

        #region double

        /// <summary>Returns a <see cref="TimeSpan"/> that represents a specified number of minutes.</summary>
        /// <param name="minutes">number of minutes</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        /// <example>3D.Minutes()</example>
        public static TimeSpan ToMinutes(this double minutes)
        {
            return TimeSpan.FromMinutes(minutes);
        }

        /// <summary>Returns a <see cref="TimeSpan"/> that represents a specified number of hours.</summary>
        /// <param name="hours">number of hours</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        /// <example>3D.Hours()</example>
        public static TimeSpan ToHours(this double hours)
        {
            return TimeSpan.FromHours(hours);
        }

        /// <summary>Returns a <see cref="TimeSpan"/> that represents a specified number of seconds.</summary>
        /// <param name="seconds">number of seconds</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        /// <example>2D.Seconds()</example>
        public static TimeSpan ToSeconds(this double seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        /// <summary>Returns a <see cref="TimeSpan"/> that represents a specified number of milliseconds.</summary>
        /// <param name="milliseconds">milliseconds for this timespan</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        public static TimeSpan ToMilliseconds(this double milliseconds)
        {
            return TimeSpan.FromMilliseconds(milliseconds);
        }

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> that represents a specified number of days.
        /// </summary>
        /// <param name="days">Number of days, accurate to the milliseconds.</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        public static TimeSpan ToDays(this double days)
        {
            return TimeSpan.FromDays(days);
        }

        #endregion double

        #region String

        public static T ToEnum<T>(this string value, T defaultValue) where T : IComparable, IFormattable
        {
            Guard.ArgumentIsEnumType(typeof(T), "T");

            T result;
            if (CommonUtility.TryConvert(value, out result))
            {
                return result;
            }

            return defaultValue;
        }

        public static int ToInt(this string value, int defaultValue = 0)
        {
            int result;
            if (CommonUtility.TryConvert(value, out result))
            {
                return result;
            }

            return defaultValue;
        }

        public static char ToChar(this string value, bool unescape = false, char defaultValue = '\0')
        {
            char result;
            if (value.HasValue() && char.TryParse(unescape ? Regex.Unescape(value) : value, out result))
            {
                return result;
            }
            return defaultValue;
        }

        public static float ToFloat(this string value, float defaultValue = 0)
        {
            float result;
            if (CommonUtility.TryConvert(value, out result))
            {
                return result;
            }

            return defaultValue;
        }

        public static bool ToBool(this string value, bool defaultValue = false)
        {
            bool result;
            if (CommonUtility.TryConvert(value, out result))
            {
                return result;
            }

            return defaultValue;
        }

        public static DateTime? ToDateTime(this string value, DateTime? defaultValue)
        {
            return value.ToDateTime(null, defaultValue);
        }

        public static DateTime? ToDateTime(this string value, string[] formats, DateTime? defaultValue)
        {
            return value.ToDateTime(formats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces, defaultValue);
        }

        public static DateTime? ToDateTime(this string value, string[] formats, IFormatProvider provider, DateTimeStyles styles, DateTime? defaultValue)
        {
            DateTime result;

            if (formats.IsNullOrEmpty())
            {
                if (DateTime.TryParse(value, provider, styles, out result))
                {
                    return result;
                }
            }

            if (DateTime.TryParseExact(value, formats, provider, styles, out result))
            {
                return result;
            }

            return defaultValue;
        }

        public static Guid ToGuid(this string value)
        {
            if ((!string.IsNullOrEmpty(value)) && (value.Trim().Length == 22))
            {
                string encoded = string.Concat(value.Trim().Replace("-", "+").Replace("_", "/"), "==");

                byte[] base64 = System.Convert.FromBase64String(encoded);

                return new Guid(base64);
            }

            return Guid.Empty;
        }

        public static byte[] ToByteArray(this string value)
        {
            return Encoding.Default.GetBytes(value);
        }

        /// <summary>
		/// Parse ISO-8601 UTC timestamp including milliseconds.
		/// </summary>
		/// <remarks>
		/// Dublicate can be found in HmacAuthentication class.
		/// </remarks>
		public static DateTime? ToDateTimeIso8601(this string value)
        {
            if (value.HasValue())
            {
                DateTime dt;
                if (DateTime.TryParseExact(value, "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dt))
                    return dt;

                if (DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dt))
                    return dt;
            }
            return null;
        }


        [DebuggerStepThrough]
        public static Version ToVersion(this string value, Version defaultVersion = null)
        {
            try
            {
                return new Version(value);
            }
            catch
            {
                return defaultVersion ?? new Version("1.0");
            }
        }

        #endregion String

        #region DateTime

        // [...]

        #endregion DateTime

        #region Stream

        public static byte[] ToByteArray(this Stream stream)
        {
            Guard.ArgumentNotNull(stream, "stream");

            if (stream is MemoryStream)
            {
                return ((MemoryStream)stream).ToArray();
            }
            else
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        public static string AsString(this Stream stream)
        {
            return stream.AsString(Encoding.UTF8);
        }

        public static string AsString(this Stream stream, Encoding encoding)
        {
            Guard.ArgumentNotNull(() => encoding);

            // convert stream to string
            string result;

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            using (StreamReader sr = new StreamReader(stream, encoding))
            {
                result = sr.ReadToEnd();
            }

            return result;
        }


        #endregion Stream

        #region ByteArray

        /// <summary>
        /// Converts a byte array into an object.
        /// </summary>
        /// <param name="bytes">Object to deserialize. May be null.</param>
        /// <returns>Deserialized object, or null if input was null.</returns>
        public static object ToObject(this byte[] bytes)
        {
            if (bytes == null)
                return null;

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return new BinaryFormatter().Deserialize(stream);
            }
        }

        public static Image ToImage(this byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return Image.FromStream(stream);
            }
        }

        public static Stream ToStream(this byte[] bytes)
        {
            return new MemoryStream(bytes);
        }

        public static string AsString(this byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }

        /// <summary>
        /// Computes the MD5 hash of a byte array
        /// </summary>
        /// <param name="value">The byte array to compute the hash for</param>
        /// <returns>The hash value</returns>
        //[DebuggerStepThrough]
        public static string Hash(this byte[] value, bool toBase64 = false)
        {
            Guard.ArgumentNotNull(value, "value");

            using (MD5 md5 = MD5.Create())
            {
                if (toBase64)
                {
                    byte[] hash = md5.ComputeHash(value);
                    return System.Convert.ToBase64String(hash);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();

                    byte[] hashBytes = md5.ComputeHash(value);
                    foreach (byte b in hashBytes)
                    {
                        sb.Append(b.ToString("x2").ToLower());
                    }

                    return sb.ToString();
                }
            }
        }

        #endregion ByteArray

        #region Image/Bitmap

        public static byte[] ToByteArray(this Image image)
        {
            Guard.ArgumentNotNull(image);

            byte[] bytes;

            ImageConverter converter = new ImageConverter();
            bytes = (byte[])converter.ConvertTo(image, typeof(byte[]));
            return bytes;
        }

        internal static byte[] ToByteArray(this Image image, ImageFormat format)
        {
            Guard.ArgumentNotNull(image);
            Guard.ArgumentNotNull(format);

            using (var stream = new MemoryStream())
            {
                image.Save(stream, format);
                return stream.ToByteArray();
            }
        }

        internal static Image ConvertTo(this Image image, ImageFormat format)
        {
            Guard.ArgumentNotNull(image);
            Guard.ArgumentNotNull(format);

            using (var stream = new MemoryStream())
            {
                image.Save(stream, format);
                return Image.FromStream(stream);
            }
        }

        #endregion Image/Bitmap
    }
}