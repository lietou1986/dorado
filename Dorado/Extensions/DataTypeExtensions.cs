using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Dorado.Extensions
{
    public static class DataTypeExtensions
    {
        public static int Int(this string input)
        {
            int myInt;
            Int32.TryParse(input, out myInt);
            return myInt;
        }

        public static T[] RemoveDuplicate<T>(this T[] input)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < input.Length; i++)
            {
                if (list.IndexOf(input[i]) < 0)
                    list.Add(input[i]);
            }
            return list.ToArray();
        }

        public static decimal Decimal(this string input)
        {
            decimal myDecimal;
            decimal.TryParse(input, out myDecimal);
            return myDecimal;
        }

        public static long Long(this string input)
        {
            long myLong;
            long.TryParse(input, out myLong);
            return myLong;
        }

        public static double Double(this string input)
        {
            double myDouble;
            double.TryParse(input, out myDouble);
            return myDouble;
        }

        public static DateTime Date(this string input)
        {
            if (input == "")
            {
                return new DateTime(1900, 1, 1);
            }
            DateTime myDateTime;

            DateTime.TryParse(input, out myDateTime);
            if (myDateTime.Equals(default(DateTime)))
            {
                return new DateTime(1900, 1, 1);
            }
            if (myDateTime.Year == 0001)
            {
                return new DateTime(1900, 1, 1);
            }
            return myDateTime;
        }

        public static string DateFormat(this string input, string separator = "-")
        {
            if (input == null)
                return string.Empty;
            return input.Contains("1900") ? string.Empty : input.Date().ToString(string.Format("yyyy{0}MM{0}dd", separator));
        }

        public static bool Bool(this string input)
        {
            if (input == null)
                return false;

            string temp = input.ToLower();
            return (temp != "0" && temp != "false");
        }

        public static bool Bool(this int input)
        {
            return input != 0;
        }

        public static DateTime Date(this object input)
        {
            return input.IsNotNull() ? input.ToString().Date() : new DateTime(1900, 1, 1);
        }

        public static bool Bool(this object input)
        {
            return input.IsNotNull() && input.ToString().Bool();
        }

        public static Decimal Decimal(this object input)
        {
            return input.IsNotNull() ? input.ToString().Decimal() : 0;
        }

        public static long Long(this object input)
        {
            return input.IsNotNull() ? input.ToString().Long() : 0;
        }

        public static Double Double(this object input)
        {
            return input.IsNotNull() ? input.ToString().Double() : 0.0;
        }

        public static int Int(this object input)
        {
            return input.IsNotNull() ? input.ToString().Int() : 0;
        }

        public static int Int(this Enum input)
        {
            return System.Convert.ToInt32(input);
        }

        public static Guid Guid(this object input)
        {
            return input.IsNotNull() ? (Guid)input : new Guid().Guid();
        }

        public static string Trim(this object input)
        {
            return input.IsNotNull() ? input.ToString().Trim() : string.Empty;
        }

        public static bool IsNotNull(this object input)
        {
            bool temp = input.IsNotDBNull();
            if (temp)
            {
                temp = (input != null);
            }
            return temp;
        }

        public static bool IsNotDBNull(this object input)
        {
            return !System.Convert.IsDBNull(input);
        }

        public static bool IsNullOrDefault<T>(this T? value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }

        public static unsafe int Compare(string a, string b)
        {
            if (a.Length != b.Length) return a.Length < b.Length ? -32 : 32;

            fixed (char* pa = a, pb = b)
            {
                for (int i = 0; i < a.Length; i++)
                {
                    if (pa[i] == pb[i]) continue;
                    switch (pa[i])
                    {
                        case 'a':
                            if (pb[i] == 'A') continue;
                            break;

                        case 'b':
                            if (pb[i] == 'B') continue;
                            break;

                        case 'c':
                            if (pb[i] == 'C') continue;
                            break;

                        case 'd':
                            if (pb[i] == 'D') continue;
                            break;

                        case 'e':
                            if (pb[i] == 'E') continue;
                            break;

                        case 'f':
                            if (pb[i] == 'F') continue;
                            break;

                        case 'g':
                            if (pb[i] == 'G') continue;
                            break;

                        case 'h':
                            if (pb[i] == 'H') continue;
                            break;

                        case 'i':
                            if (pb[i] == 'I') continue;
                            break;

                        case 'j':
                            if (pb[i] == 'J') continue;
                            break;

                        case 'k':
                            if (pb[i] == 'K') continue;
                            break;

                        case 'l':
                            if (pb[i] == 'L') continue;
                            break;

                        case 'm':
                            if (pb[i] == 'M') continue;
                            break;

                        case 'n':
                            if (pb[i] == 'N') continue;
                            break;

                        case 'o':
                            if (pb[i] == 'O') continue;
                            break;

                        case 'p':
                            if (pb[i] == 'P') continue;
                            break;

                        case 'q':
                            if (pb[i] == 'Q') continue;
                            break;

                        case 'r':
                            if (pb[i] == 'R') continue;
                            break;

                        case 's':
                            if (pb[i] == 'S') continue;
                            break;

                        case 't':
                            if (pb[i] == 'T') continue;
                            break;

                        case 'u':
                            if (pb[i] == 'U') continue;
                            break;

                        case 'v':
                            if (pb[i] == 'V') continue;
                            break;

                        case 'w':
                            if (pb[i] == 'W') continue;
                            break;

                        case 'x':
                            if (pb[i] == 'X') continue;
                            break;

                        case 'y':
                            if (pb[i] == 'Y') continue;
                            break;

                        case 'z':
                            if (pb[i] == 'Z') continue;
                            break;

                        case 'A':
                            if (pb[i] == 'a') continue;
                            break;

                        case 'B':
                            if (pb[i] == 'b') continue;
                            break;

                        case 'C':
                            if (pb[i] == 'c') continue;
                            break;

                        case 'D':
                            if (pb[i] == 'd') continue;
                            break;

                        case 'E':
                            if (pb[i] == 'e') continue;
                            break;

                        case 'F':
                            if (pb[i] == 'f') continue;
                            break;

                        case 'G':
                            if (pb[i] == 'g') continue;
                            break;

                        case 'H':
                            if (pb[i] == 'h') continue;
                            break;

                        case 'I':
                            if (pb[i] == 'i') continue;
                            break;

                        case 'J':
                            if (pb[i] == 'j') continue;
                            break;

                        case 'K':
                            if (pb[i] == 'k') continue;
                            break;

                        case 'L':
                            if (pb[i] == 'l') continue;
                            break;

                        case 'M':
                            if (pb[i] == 'm') continue;
                            break;

                        case 'N':
                            if (pb[i] == 'n') continue;
                            break;

                        case 'O':
                            if (pb[i] == 'o') continue;
                            break;

                        case 'P':
                            if (pb[i] == 'p') continue;
                            break;

                        case 'Q':
                            if (pb[i] == 'q') continue;
                            break;

                        case 'R':
                            if (pb[i] == 'r') continue;
                            break;

                        case 'S':
                            if (pb[i] == 's') continue;
                            break;

                        case 'T':
                            if (pb[i] == 't') continue;
                            break;

                        case 'U':
                            if (pb[i] == 'u') continue;
                            break;

                        case 'V':
                            if (pb[i] == 'v') continue;
                            break;

                        case 'W':
                            if (pb[i] == 'w') continue;
                            break;

                        case 'X':
                            if (pb[i] == 'x') continue;
                            break;

                        case 'Y':
                            if (pb[i] == 'y') continue;
                            break;

                        case 'Z':
                            if (pb[i] == 'z') continue;
                            break;
                    }
                    return pa[i] - pb[i];
                }
            }
            return 0;
        }

        #region ToString 转换成string

        /// <summary>
        /// 将<c>object</c>转换成<c>string</c>字符串
        /// </summary>
        /// <param name="value">输入值</param>
        /// <returns>返回转换后的<c>string</c>字符串</returns>
        public static string ToString(bool value)
        {
            return value ? "1" : "0";
        }

        public static string ToString(DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToString(string value)
        {
            if (value == null) return string.Empty;
            return value;
        }

        /// <summary>
        /// 将<c>byte</c>数组转换成<c>string</c>字符串
        /// </summary>
        /// <param name="value">输入<c>byte</c>数组</param>
        /// <returns>返回转换后的<c>string</c>字符串</returns>
        public static string ToString(byte[] value)
        {
            StringBuilder tmp = new StringBuilder("0x");
            foreach (byte t in value)
            {
                tmp.Append(t.ToString("x"));
            }
            return tmp.ToString();
        }

        public static string ToString(object value)
        {
            if (value == null) return string.Empty;
            if (value is byte[]) return ToString((byte[])value);
            return value.ToString();
        }

        #endregion ToString 转换成string

        public static string ToSafeString(string value)
        {
            if (value == null) return string.Empty;
            return "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r") + "\"";
        }

        public static string ToSafeString(int value)
        {
            return value.ToString();
        }

        public static string ToSafeString(long value)
        {
            return value.ToString();
        }

        public static string ToSafeString(float value)
        {
            return value.ToString();
        }

        public static string ToSafeString(double value)
        {
            return value.ToString();
        }

        public static string ToSafeString(decimal value)
        {
            return value.ToString();
        }

        public static string ToSafeString(DateTime value)
        {
            return "\"" + value.ToShortDateString() + "\"";
        }

        public static string ToSafeString(object value)
        {
            if (value == null) return string.Empty;
            if (value is DateTime) return "\"" + ((DateTime)value).ToShortDateString() + "\"";
            return ToSafeString(value.ToString());
        }

        #region ToSQL 转换成sql字符串

        /// <summary>
        /// 将<c>Array</c>数组转换成<c>string</c>字符串
        /// </summary>
        /// <param name="value">输入数组</param>
        /// <returns>返回转换后的<c>string</c>字符串</returns>
        public static string ToSql(IList value)
        {
            StringBuilder ret = new StringBuilder();
            for (int i = 0; i < value.Count; i++)
            {
                object o = value[i];
                if (o is string)
                {
                    if (string.Empty.Equals(o) || o == null) continue;
                    if (ret.Length > 0) ret.Append(',');
                    ret.Append("'" + o.ToString().Replace("'", "''") + "'");
                }
                else
                {
                    if (o.Equals(0)) continue;
                    if (ret.Length > 0) ret.Append(',');
                    ret.Append(o.ToString());
                }
            }
            return ret.ToString();
        }

        public static string ToSql(ICollection value)
        {
            StringBuilder ret = new StringBuilder();
            foreach (object o in value)
            {
                if (o is string)
                {
                    if (string.Empty.Equals(o) || o == null) continue;
                    if (ret.Length > 0) ret.Append(',');
                    ret.Append("'" + o.ToString().Replace("'", "''") + "'");
                }
                else
                {
                    if (o.Equals(0)) continue;
                    if (ret.Length > 0) ret.Append(',');
                    ret.Append(o.ToString());
                }
            }
            return ret.ToString();
        }

        public static string ToSql(bool value)
        {
            return value ? "1" : "0";
        }

        public static string ToSql(char value)
        {
            return value.ToString();
        }

        public static string ToSql(byte value)
        {
            return value.ToString();
        }

        public static string ToSql(sbyte value)
        {
            return value.ToString();
        }

        public static string ToSql(short value)
        {
            return value.ToString();
        }

        public static string ToSql(ushort value)
        {
            return value.ToString();
        }

        public static string ToSql(int value)
        {
            return value.ToString();
        }

        public static string ToSql(uint value)
        {
            return value.ToString();
        }

        public static string ToSql(long value)
        {
            return value.ToString();
        }

        public static string ToSql(ulong value)
        {
            return value.ToString();
        }

        public static string ToSql(decimal value)
        {
            return value.ToString();
        }

        public static string ToSql(double value)
        {
            return value.ToString();
        }

        public static string ToSql(float value)
        {
            return value.ToString();
        }

        public static string ToSql(DateTime value)
        {
            return "'" + value.ToString() + "'";
        }

        public static string ToSql(Guid value)
        {
            return "'" + value.ToString() + "'";
        }

        public static string ToSql(string value)
        {
            if (value == null) return "'" + string.Empty + "'";
            return "'" + value.Replace("'", "''") + "'";
        }

        public static string ToSql(object value)
        {
            if (value == null) return "'" + string.Empty + "'";
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    return ToString((bool)value);

                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return value.ToString();

                case TypeCode.String:
                    return "'" + value.ToString().Replace("'", "''") + "'";

                case TypeCode.Char:
                case TypeCode.DateTime:
                    return "'" + value.ToString() + "'";

                case TypeCode.Object:
                    if (value is byte[]) return ToString((byte[])value);
                    return "'" + value.ToString() + "'";

                default:
                    return "'" + string.Empty + "'";
            }
        }

        #endregion ToSQL 转换成sql字符串

        #region ToBinary 转换成16进制字符串

        public static string ToBinary(bool value)
        {
            return value ? "0x01" : "0x00";
        }

        public static string ToBinary(char value)
        {
            return "0x" + ToInt(value).ToString("x");
        }

        public static string ToBinary(byte value)
        {
            return "0x" + value.ToString("x");
        }

        public static string ToBinary(sbyte value)
        {
            return "0x" + value.ToString("x");
        }

        public static string ToBinary(short value)
        {
            return "0x" + value.ToString("x");
        }

        public static string ToBinary(ushort value)
        {
            return "0x" + value.ToString("x");
        }

        public static string ToBinary(int value)
        {
            return "0x" + value.ToString("x");
        }

        public static string ToBinary(uint value)
        {
            return "0x" + value.ToString("x");
        }

        public static string ToBinary(long value)
        {
            return "0x" + value.ToString("x");
        }

        public static string ToBinary(ulong value)
        {
            return "0x" + value.ToString("x");
        }

        public static string ToBinary(string value)
        {
            return ToString(ToByteArray(value));
        }

        public static string ToBinary(object value)
        {
            if (value == null) return "'" + string.Empty + "'";
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    return ToBinary((bool)value);

                case TypeCode.Byte:
                    return "0x" + ((byte)value).ToString("x");

                case TypeCode.SByte:
                    return "0x" + ((sbyte)value).ToString("x");

                case TypeCode.Int16:
                    return "0x" + ((short)value).ToString("x");

                case TypeCode.UInt16:
                    return "0x" + ((ushort)value).ToString("x");

                case TypeCode.Int32:
                    return "0x" + ((int)value).ToString("x");

                case TypeCode.UInt32:
                    return "0x" + ((uint)value).ToString("x");

                case TypeCode.Int64:
                    return "0x" + ((long)value).ToString("x");

                case TypeCode.UInt64:
                    return "0x" + ((ulong)value).ToString("x");

                case TypeCode.String:
                    return "'" + value.ToString().Replace("'", "''") + "'";

                case TypeCode.Char:
                    return ToBinary((char)value);

                case TypeCode.Object:
                    if (value is byte[]) return ToString((byte[])value);
                    if (value is Guid) return ToString(((Guid)value).ToByteArray());
                    throw new InvalidCastException("对不起，" + value.GetType().Name + "不能转换成Binary！");
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                default:
                    throw new InvalidCastException("对不起，" + value.GetType().Name + "不能转换成Binary！");
            }
        }

        #endregion ToBinary 转换成16进制字符串

        public static Guid ToGuid(string value)
        {
            return new Guid(value);
        }

        public static Guid ToGuid(object value)
        {
            return ToGuid(value.ToString());
        }

        /// <summary>
        /// 从string字符串转换成Byte数组
        /// </summary>
        /// <param name="value">string字符串</param>
        /// <returns>返回Byte数组</returns>
        public static unsafe byte[] ToByteArray(string value)
        {
            if (value == null) return null;
            value = value.Trim().Replace("0x", "");
            if (value.Length % 2 != 0) value = "0" + value;

            byte[] tmp = new byte[value.Length / 2];
            fixed (char* pa = value)
            {
                for (int i = 0; i < value.Length / 2; i++)
                {
                    tmp[i] = (byte)(ToByte(pa[2 * i]) * 16 + ToByte(pa[2 * i + 1]));
                }
            }
            return tmp;
        }

        #region ToBool 转换成bool

        /// <summary>
        /// 转换成Byte
        /// </summary>
        public static bool ToBool(char value)
        {
            return value > 0 ? true : false;
        }

        public static bool ToBool(string value)
        {
            if (value == null) return false;
            return (value.Trim() == "1" || value.Trim().ToLower() == "true") ? true : false;
        }

        public static bool ToBool(bool value)
        {
            return value;
        }

        public static bool ToBool(byte value)
        {
            return value > 0 ? true : false;
        }

        public static bool ToBool(sbyte value)
        {
            return value > 0 ? true : false;
        }

        public static bool ToBool(short value)
        {
            return value > 0 ? true : false;
        }

        public static bool ToBool(ushort value)
        {
            return value > 0 ? true : false;
        }

        public static bool ToBool(int value)
        {
            return value > 0 ? true : false;
        }

        public static bool ToBool(uint value)
        {
            return value > 0 ? true : false;
        }

        public static bool ToBool(long value)
        {
            return value > 0 ? true : false;
        }

        public static bool ToBool(ulong value)
        {
            return value > 0 ? true : false;
        }

        public static bool ToBool(decimal value)
        {
            return value > 0 ? true : false;
        }

        public static bool ToBool(double value)
        {
            return value > 0 ? true : false;
        }

        public static bool ToBool(float value)
        {
            return value > 0 ? true : false;
        }

        public static bool ToBool(object value)
        {
            IConvertible tmp = value as IConvertible;
            if (tmp == null) return false;
            try
            {
                return System.Convert.ToBoolean(tmp);
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ToBool 转换成bool

        #region ToChar 转换成char

        /// <summary>
        /// 转换成Byte
        /// </summary>
        public static char ToChar(char value)
        {
            return value;
        }

        public static char ToChar(string value)
        {
            if (value == null) return char.MinValue;
            return value[0];
        }

        public static char ToChar(byte value)
        {
            return (char)value;
        }

        public static char ToChar(sbyte value)
        {
            if (value >= char.MinValue)
                return (char)value;
            return char.MinValue;
        }

        public static char ToChar(short value)
        {
            if (value >= char.MinValue)
                return (char)value;
            return char.MinValue;
        }

        public static char ToChar(ushort value)
        {
            return (char)value;
        }

        public static char ToChar(int value)
        {
            if (value >= char.MinValue && value <= char.MaxValue)
                return (char)value;
            return char.MinValue;
        }

        public static char ToChar(uint value)
        {
            if (value <= char.MaxValue)
                return (char)value;
            return char.MinValue;
        }

        public static char ToChar(long value)
        {
            if (value >= char.MinValue && value <= char.MaxValue)
                return (char)value;
            return char.MinValue;
        }

        public static char ToChar(ulong value)
        {
            if (value <= char.MaxValue)
                return (char)value;
            return char.MinValue;
        }

        public static char ToChar(double value)
        {
            if (value >= char.MinValue && value <= char.MaxValue)
                return (char)value;
            return char.MinValue;
        }

        public static char ToChar(float value)
        {
            if (value >= char.MinValue && value <= char.MaxValue)
                return (char)value;
            return char.MinValue;
        }

        public static char ToChar(object value)
        {
            IConvertible tmp = value as IConvertible;
            if (tmp == null) return char.MinValue;
            try
            {
                return System.Convert.ToChar(tmp);
            }
            catch (Exception)
            {
                return char.MinValue;
            }
        }

        #endregion ToChar 转换成char

        #region ToByte 转换成Byte

        /// <summary>
        /// 转换成Byte
        /// </summary>
        public static byte ToByte(char c)
        {
            if (c >= 48 && c <= 57) return (byte)(c - 48);
            if (c >= 97 && c <= 102) return (byte)(c - 87);
            if (c >= 65 && c <= 70) return (byte)(c - 55);
            return 0;
        }

        public static byte ToByte(string value)
        {
            return ToByte(value, 10);
        }

        public static byte ToByte(string value, int fromBase)
        {
            if (value == null) return 0;
            switch (fromBase)
            {
                case 2:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[01]+$"))
                        return 0;
                    break;

                case 8:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-7]+$"))
                        return 0;
                    break;

                case 10:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value.Replace(",", ""), @"^\d+$"))
                        return 0;
                    break;

                case 16:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[\da-fA-F]+$"))
                        return 0;
                    break;

                default:
                    return 0;
            }
            try
            {
                return System.Convert.ToByte(value, fromBase);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static byte ToByte(bool value)
        {
            if (value) return 1;
            return 0;
        }

        public static byte ToByte(byte value)
        {
            return value;
        }

        public static byte ToByte(sbyte value)
        {
            if (value >= byte.MinValue)
                return (byte)value;
            return 0;
        }

        public static byte ToByte(short value)
        {
            if (value >= byte.MinValue && value <= byte.MaxValue)
                return (byte)value;
            return 0;
        }

        public static byte ToByte(ushort value)
        {
            if (value <= byte.MaxValue)
                return (byte)value;
            return 0;
        }

        public static byte ToByte(int value)
        {
            if (value >= byte.MinValue && value <= byte.MaxValue)
                return (byte)value;
            return 0;
        }

        public static byte ToByte(uint value)
        {
            if (value <= byte.MaxValue)
                return (byte)value;
            return 0;
        }

        public static byte ToByte(long value)
        {
            if (value >= byte.MinValue && value <= byte.MaxValue)
                return (byte)value;
            return 0;
        }

        public static byte ToByte(ulong value)
        {
            if (value <= byte.MaxValue)
                return (byte)value;
            return 0;
        }

        public static byte ToByte(decimal value)
        {
            try
            {
                return decimal.ToByte(decimal.Round(value, 0));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static byte ToByte(double value)
        {
            if (value >= byte.MinValue && value <= byte.MaxValue)
                return (byte)value;
            return 0;
        }

        public static byte ToByte(float value)
        {
            if (value >= byte.MinValue && value <= byte.MaxValue)
                return (byte)value;
            return 0;
        }

        public static byte ToByte(object value)
        {
            IConvertible tmp = value as IConvertible;
            if (tmp == null) return 0;
            try
            {
                return System.Convert.ToByte(tmp);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion ToByte 转换成Byte

        #region ToSByte 转换成SByte

        /// <summary>
        /// 转换成SByte
        /// </summary>
        public static sbyte ToSByte(char value)
        {
            if (value <= sbyte.MaxValue) return (sbyte)value;
            return 0;
        }

        public static sbyte ToSByte(string value)
        {
            return ToSByte(value, 10);
        }

        public static sbyte ToSByte(string value, int fromBase)
        {
            if (value == null) return 0;
            switch (fromBase)
            {
                case 2:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[01]+$"))
                        return 0;
                    break;

                case 8:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-7]+$"))
                        return 0;
                    break;

                case 10:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value.Replace(",", ""), @"^-?\d+$"))
                        return 0;
                    break;

                case 16:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[\da-fA-F]+$"))
                        return 0;
                    break;

                default:
                    return 0;
            }
            try
            {
                return System.Convert.ToSByte(value, fromBase);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static sbyte ToSByte(bool value)
        {
            if (value) return 1;
            return 0;
        }

        public static sbyte ToSByte(byte value)
        {
            if (value <= sbyte.MaxValue) return (sbyte)value;
            return 0;
        }

        public static sbyte ToSByte(sbyte value)
        {
            return value;
        }

        public static sbyte ToSByte(short value)
        {
            if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
                return (sbyte)value;
            return 0;
        }

        public static sbyte ToSByte(ushort value)
        {
            if (value <= sbyte.MaxValue)
                return (sbyte)value;
            return 0;
        }

        public static sbyte ToSByte(int value)
        {
            if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
                return (sbyte)value;
            return 0;
        }

        public static sbyte ToSByte(uint value)
        {
            if (value <= sbyte.MaxValue)
                return (sbyte)value;
            return 0;
        }

        public static sbyte ToSByte(long value)
        {
            if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
                return (sbyte)value;
            return 0;
        }

        public static sbyte ToSByte(ulong value)
        {
            if (value <= (ulong)sbyte.MaxValue)
                return (sbyte)value;
            return 0;
        }

        public static sbyte ToSByte(decimal value)
        {
            try
            {
                return decimal.ToSByte(decimal.Round(value, 0));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static sbyte ToSByte(double value)
        {
            if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
                return (sbyte)value;
            return 0;
        }

        public static sbyte ToSByte(float value)
        {
            if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
                return (sbyte)value;
            return 0;
        }

        public static sbyte ToSByte(object value)
        {
            IConvertible tmp = value as IConvertible;
            if (tmp == null) return 0;
            try
            {
                return System.Convert.ToSByte(tmp);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion ToSByte 转换成SByte

        #region ToShort 转换成short

        /// <summary>
        /// 转换成Byte
        /// </summary>
        public static short ToShort(char c)
        {
            return (short)c;
        }

        public static short ToShort(string value)
        {
            return ToShort(value, 10);
        }

        public static short ToShort(string value, int fromBase)
        {
            if (value == null) return 0;
            switch (fromBase)
            {
                case 2:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[01]+$"))
                        return 0;
                    break;

                case 8:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-7]+$"))
                        return 0;
                    break;

                case 10:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value.Replace(",", ""), @"^-?\d+$"))
                        return 0;
                    break;

                case 16:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[\da-fA-F]+$"))
                        return 0;
                    break;

                default:
                    return 0;
            }
            try
            {
                return System.Convert.ToInt16(value, fromBase);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static short ToShort(bool value)
        {
            if (value) return 1;
            return 0;
        }

        public static short ToShort(byte value)
        {
            return (short)value;
        }

        public static short ToShort(sbyte value)
        {
            return (short)value;
        }

        public static short ToShort(short value)
        {
            return value;
        }

        public static short ToShort(ushort value)
        {
            if (value <= short.MaxValue) return (short)value;
            return 0;
        }

        public static short ToShort(int value)
        {
            if (value >= short.MinValue && value <= short.MaxValue) return (short)value;
            return 0;
        }

        public static short ToShort(uint value)
        {
            if (value <= short.MaxValue)
                return (short)value;
            return 0;
        }

        public static short ToShort(long value)
        {
            if (value >= short.MinValue && value <= short.MaxValue)
                return (short)value;
            return 0;
        }

        public static short ToShort(ulong value)
        {
            if (value <= (ulong)short.MaxValue)
                return (short)value;
            return 0;
        }

        public static short ToShort(decimal value)
        {
            try
            {
                return decimal.ToInt16(decimal.Round(value, 0));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static short ToShort(double value)
        {
            if (value >= short.MinValue && value <= short.MaxValue)
                return (short)value;
            return 0;
        }

        public static short ToShort(float value)
        {
            if (value >= short.MinValue && value <= short.MaxValue)
                return (short)value;
            return 0;
        }

        public static short ToShort(object value)
        {
            IConvertible tmp = value as IConvertible;
            if (tmp == null) return 0;
            try
            {
                return System.Convert.ToInt16(tmp);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion ToShort 转换成short

        #region ToUShort 转换成ushort

        /// <summary>
        /// 转换成Byte
        /// </summary>
        public static ushort ToUShort(char c)
        {
            return (ushort)c;
        }

        public static ushort ToUShort(string value)
        {
            return ToUShort(value, 10);
        }

        public static ushort ToUShort(string value, int fromBase)
        {
            if (value == null) return 0;
            switch (fromBase)
            {
                case 2:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[01]+$"))
                        return 0;
                    break;

                case 8:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-7]+$"))
                        return 0;
                    break;

                case 10:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value.Replace(",", ""), @"^\d+$"))
                        return 0;
                    break;

                case 16:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[\da-fA-F]+$"))
                        return 0;
                    break;

                default:
                    return 0;
            }
            try
            {
                return System.Convert.ToUInt16(value, fromBase);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static ushort ToUShort(bool value)
        {
            if (value) return 1;
            return 0;
        }

        public static ushort ToUShort(byte value)
        {
            return (ushort)value;
        }

        public static ushort ToUShort(sbyte value)
        {
            return (ushort)value;
        }

        public static ushort ToUShort(ushort value)
        {
            return value;
        }

        public static ushort ToUShort(short value)
        {
            if (value >= ushort.MinValue) return (ushort)value;
            return 0;
        }

        public static ushort ToUShort(int value)
        {
            if (value >= ushort.MinValue && value <= ushort.MaxValue) return (ushort)value;
            return 0;
        }

        public static ushort ToUShort(uint value)
        {
            if (value <= ushort.MaxValue)
                return (ushort)value;
            return 0;
        }

        public static ushort ToUShort(long value)
        {
            if (value >= ushort.MinValue && value <= ushort.MaxValue)
                return (ushort)value;
            return 0;
        }

        public static ushort ToUShort(ulong value)
        {
            if (value <= (ulong)ushort.MaxValue)
                return (ushort)value;
            return 0;
        }

        public static ushort ToUShort(decimal value)
        {
            try
            {
                return decimal.ToUInt16(decimal.Round(value, 0));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static ushort ToUShort(double value)
        {
            if (value >= ushort.MinValue && value <= ushort.MaxValue)
                return (ushort)value;
            return 0;
        }

        public static ushort ToUShort(float value)
        {
            if (value >= ushort.MinValue && value <= ushort.MaxValue)
                return (ushort)value;
            return 0;
        }

        public static ushort ToUShort(object value)
        {
            IConvertible tmp = value as IConvertible;
            if (tmp == null) return 0;
            try
            {
                return System.Convert.ToUInt16(tmp);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion ToUShort 转换成ushort

        #region ToInt 转换成int

        /// <summary>
        /// 转换成Byte
        /// </summary>
        public static int ToInt(char c)
        {
            return (int)c;
        }

        public static int ToInt(string value)
        {
            return ToInt(value, 10);
        }

        public static int ToInt(string value, int fromBase)
        {
            if (value == null) return 0;
            switch (fromBase)
            {
                case 2:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[01]+$"))
                        return 0;
                    break;

                case 8:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-7]+$"))
                        return 0;
                    break;

                case 10:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value.Replace(",", ""), @"^-?\d+$"))
                        return 0;
                    break;

                case 16:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[\da-fA-F]+$"))
                        return 0;
                    break;

                default:
                    return 0;
            }
            try
            {
                return System.Convert.ToInt32(value, fromBase);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static int ToInt(bool value)
        {
            if (value) return 1;
            return 0;
        }

        public static int ToInt(byte value)
        {
            return value;
        }

        public static int ToInt(sbyte value)
        {
            return (int)value;
        }

        public static int ToInt(short value)
        {
            return (int)value;
        }

        public static int ToInt(ushort value)
        {
            return (int)value;
        }

        public static int ToInt(int value)
        {
            return value;
        }

        public static int ToInt(uint value)
        {
            if (value <= int.MaxValue)
                return (int)value;
            return 0;
        }

        public static int ToInt(long value)
        {
            if (value >= int.MinValue && value <= int.MaxValue)
                return (int)value;
            return 0;
        }

        public static int ToInt(ulong value)
        {
            if (value <= int.MaxValue)
                return (int)value;
            return 0;
        }

        public static int ToInt(decimal value)
        {
            try
            {
                return decimal.ToInt32(decimal.Round(value, 0));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static int ToInt(double value)
        {
            if (value >= int.MinValue && value <= int.MaxValue)
                return (int)value;
            return 0;
        }

        public static int ToInt(float value)
        {
            if (value >= int.MinValue && value <= int.MaxValue)
                return (int)value;
            return 0;
        }

        public static int ToInt(object value)
        {
            IConvertible tmp = value as IConvertible;
            if (tmp == null) return 0;
            try
            {
                return System.Convert.ToInt32(tmp);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion ToInt 转换成int

        #region ToUInt 转换成uint

        /// <summary>
        /// 转换成Byte
        /// </summary>
        public static uint ToUInt(char c)
        {
            return (uint)c;
        }

        public static uint ToUInt(string value)
        {
            return ToUInt(value, 10);
        }

        public static uint ToUInt(string value, int fromBase)
        {
            if (value == null) return 0;
            switch (fromBase)
            {
                case 2:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[01]+$"))
                        return 0;
                    break;

                case 8:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-7]+$"))
                        return 0;
                    break;

                case 10:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value.Replace(",", ""), @"^\d+$"))
                        return 0;
                    break;

                case 16:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[\da-fA-F]+$"))
                        return 0;
                    break;

                default:
                    return 0;
            }
            try
            {
                return System.Convert.ToUInt32(value, fromBase);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static uint ToUInt(bool value)
        {
            if (value) return 1;
            return 0;
        }

        public static uint ToUInt(byte value)
        {
            return value;
        }

        public static uint ToUInt(sbyte value)
        {
            if (value >= uint.MinValue) return (uint)value;
            return 0;
        }

        public static uint ToUInt(short value)
        {
            if (value >= uint.MinValue) return (uint)value;
            return 0;
        }

        public static uint ToUInt(ushort value)
        {
            return (uint)value;
        }

        public static uint ToUInt(uint value)
        {
            return value;
        }

        public static uint ToUInt(int value)
        {
            if (value >= uint.MinValue)
                return (uint)value;
            return 0;
        }

        public static uint ToUInt(long value)
        {
            if (value >= uint.MinValue && value <= uint.MaxValue)
                return (uint)value;
            return 0;
        }

        public static uint ToUInt(ulong value)
        {
            if (value <= uint.MaxValue)
                return (uint)value;
            return 0;
        }

        public static uint ToUInt(decimal value)
        {
            try
            {
                return decimal.ToUInt32(decimal.Round(value, 0));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static uint ToUInt(double value)
        {
            if (value >= uint.MinValue && value <= uint.MaxValue)
                return (uint)value;
            return 0;
        }

        public static uint ToUInt(float value)
        {
            if (value >= uint.MinValue && value <= uint.MaxValue)
                return (uint)value;
            return 0;
        }

        public static uint ToUInt(object value)
        {
            IConvertible tmp = value as IConvertible;
            if (tmp == null) return 0;
            try
            {
                return System.Convert.ToUInt32(tmp);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion ToUInt 转换成uint

        #region ToLong 转换成long

        /// <summary>
        /// 转换成Byte
        /// </summary>
        public static long ToLong(char c)
        {
            return (long)c;
        }

        public static long ToLong(string value)
        {
            return ToLong(value, 10);
        }

        public static long ToLong(string value, int fromBase)
        {
            if (value == null) return 0;
            switch (fromBase)
            {
                case 2:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[01]+$"))
                        return 0;
                    break;

                case 8:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-7]+$"))
                        return 0;
                    break;

                case 10:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value.Replace(",", ""), @"^-?\d+$"))
                        return 0;
                    break;

                case 16:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[\da-fA-F]+$"))
                        return 0;
                    break;

                default:
                    return 0;
            }
            try
            {
                return System.Convert.ToInt64(value, fromBase);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static long ToLong(bool value)
        {
            if (value) return 1;
            return 0;
        }

        public static long ToLong(byte value)
        {
            return value;
        }

        public static long ToLong(sbyte value)
        {
            return (long)value;
        }

        public static long ToLong(short value)
        {
            return (long)value;
        }

        public static long ToLong(ushort value)
        {
            return (long)value;
        }

        public static long ToLong(int value)
        {
            return (long)value;
        }

        public static long ToLong(uint value)
        {
            return (long)value;
        }

        public static long ToLong(long value)
        {
            return value;
        }

        public static long ToLong(ulong value)
        {
            if (value <= long.MaxValue)
                return (long)value;
            return 0;
        }

        public static long ToLong(decimal value)
        {
            try
            {
                return decimal.ToInt64(decimal.Round(value, 0));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static long ToLong(double value)
        {
            if (value >= long.MinValue && value <= long.MaxValue)
                return (long)value;
            return 0;
        }

        public static long ToLong(float value)
        {
            if (value >= long.MinValue && value <= long.MaxValue)
                return (long)value;
            return 0;
        }

        public static long ToLong(object value)
        {
            IConvertible tmp = value as IConvertible;
            if (tmp == null) return 0;
            try
            {
                return System.Convert.ToInt64(tmp);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion ToLong 转换成long

        #region ToULong 转换成ulong

        /// <summary>
        /// 转换成Byte
        /// </summary>
        public static ulong ToULong(char c)
        {
            return (ulong)c;
        }

        public static ulong ToULong(string value)
        {
            return ToULong(value, 10);
        }

        public static ulong ToULong(string value, int fromBase)
        {
            if (value == null) return 0;
            switch (fromBase)
            {
                case 2:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[01]+$"))
                        return 0;
                    break;

                case 8:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-7]+$"))
                        return 0;
                    break;

                case 10:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value.Replace(",", ""), @"^\d+$"))
                        return 0;
                    break;

                case 16:
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[\da-fA-F]+$"))
                        return 0;
                    break;

                default:
                    return 0;
            }
            try
            {
                return System.Convert.ToUInt64(value, fromBase);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static ulong ToULong(bool value)
        {
            if (value) return 1;
            return 0;
        }

        public static ulong ToULong(byte value)
        {
            return value;
        }

        public static ulong ToULong(sbyte value)
        {
            return (ulong)value;
        }

        public static ulong ToULong(short value)
        {
            return (ulong)value;
        }

        public static ulong ToULong(ushort value)
        {
            return (ulong)value;
        }

        public static ulong ToULong(int value)
        {
            return (ulong)value;
        }

        public static ulong ToULong(uint value)
        {
            return (ulong)value;
        }

        public static ulong ToULong(ulong value)
        {
            return value;
        }

        public static ulong ToULong(long value)
        {
            if (value >= (long)ulong.MinValue)
                return (ulong)value;
            return 0;
        }

        public static ulong ToULong(decimal value)
        {
            try
            {
                return decimal.ToUInt64(decimal.Round(value, 0));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static ulong ToULong(double value)
        {
            if (value >= ulong.MinValue && value <= ulong.MaxValue)
                return (ulong)value;
            return 0;
        }

        public static ulong ToULong(float value)
        {
            if (value >= ulong.MinValue && value <= ulong.MaxValue)
                return (ulong)value;
            return 0;
        }

        public static ulong ToULong(object value)
        {
            IConvertible tmp = value as IConvertible;
            if (tmp == null) return 0;
            try
            {
                return System.Convert.ToUInt64(tmp);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion ToULong 转换成ulong

        #region ToDecimal 转换成decimal

        /// <summary>
        /// 转换成Byte
        /// </summary>
        public static decimal ToDecimal(char value)
        {
            return new decimal(value);
        }

        public static decimal ToDecimal(string value)
        {
            if (value == null || !Regex.IsMatch(value.Replace(",", ""), @"^-?\d+?(\.\d+)?$"))
                return 0;
            try
            {
                return System.Convert.ToDecimal(value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static decimal ToDecimal(bool value)
        {
            if (value) return 1;
            return 0;
        }

        public static decimal ToDecimal(byte value)
        {
            return new decimal(value);
        }

        public static decimal ToDecimal(sbyte value)
        {
            return new decimal(value);
        }

        public static decimal ToDecimal(short value)
        {
            return new decimal(value);
        }

        public static decimal ToDecimal(ushort value)
        {
            return new decimal(value);
        }

        public static decimal ToDecimal(int value)
        {
            return new decimal(value);
        }

        public static decimal ToDecimal(uint value)
        {
            return new decimal(value);
        }

        public static decimal ToDecimal(long value)
        {
            return new decimal(value);
        }

        public static decimal ToDecimal(ulong value)
        {
            return new decimal(value);
        }

        public static decimal ToDecimal(decimal value)
        {
            return value;
        }

        public static decimal ToDecimal(double value)
        {
            return new decimal(value);
        }

        public static decimal ToDecimal(float value)
        {
            return new decimal(value);
        }

        public static decimal ToDecimal(object value)
        {
            IConvertible tmp = value as IConvertible;
            if (tmp == null) return decimal.Zero;
            try
            {
                return System.Convert.ToDecimal(tmp);
            }
            catch (Exception)
            {
                return decimal.Zero;
            }
        }

        #endregion ToDecimal 转换成decimal

        #region ToDouble 转换成double

        /// <summary>
        /// 转换成Byte
        /// </summary>
        public static double ToDouble(char value)
        {
            return value;
        }

        public static double ToDouble(string value)
        {
            if (value == null || !System.Text.RegularExpressions.Regex.IsMatch(value.Replace(",", ""), @"^-?\d+?(\.\d+)?$"))
                return 0;
            try
            {
                return System.Convert.ToDouble(value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static double ToDouble(bool value)
        {
            if (value) return 1;
            return 0;
        }

        public static double ToDouble(byte value)
        {
            return value;
        }

        public static double ToDouble(sbyte value)
        {
            return value;
        }

        public static double ToDouble(short value)
        {
            return value;
        }

        public static double ToDouble(ushort value)
        {
            return value;
        }

        public static double ToDouble(int value)
        {
            return value;
        }

        public static double ToDouble(uint value)
        {
            return value;
        }

        public static double ToDouble(long value)
        {
            return value;
        }

        public static double ToDouble(ulong value)
        {
            return value;
        }

        public static double ToDouble(decimal value)
        {
            try
            {
                return decimal.ToDouble(value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static double ToDouble(double value)
        {
            return value;
        }

        public static double ToDouble(float value)
        {
            return value;
        }

        public static double ToDouble(object value)
        {
            IConvertible tmp = value as IConvertible;
            if (tmp == null) return 0;
            try
            {
                return System.Convert.ToDouble(tmp);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion ToDouble 转换成double

        #region ToFloat 转换成float

        /// <summary>
        /// 转换成Byte
        /// </summary>
        public static float ToFloat(char value)
        {
            return value;
        }

        public static float ToFloat(string value)
        {
            if (value == null || !System.Text.RegularExpressions.Regex.IsMatch(value.Replace(",", ""), @"^-?\d+?(\.\d+)?$"))
                return 0;
            try
            {
                return System.Convert.ToSingle(value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static float ToFloat(bool value)
        {
            if (value) return 1;
            return 0;
        }

        public static float ToFloat(byte value)
        {
            return value;
        }

        public static float ToFloat(sbyte value)
        {
            return value;
        }

        public static float ToFloat(short value)
        {
            return value;
        }

        public static float ToFloat(ushort value)
        {
            return value;
        }

        public static float ToFloat(int value)
        {
            return value;
        }

        public static float ToFloat(uint value)
        {
            return value;
        }

        public static float ToFloat(long value)
        {
            return value;
        }

        public static float ToFloat(ulong value)
        {
            return value;
        }

        public static float ToFloat(decimal value)
        {
            try
            {
                return decimal.ToSingle(value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static float ToFloat(double value)
        {
            if (value >= float.MinValue && value <= float.MaxValue) return (float)value;
            return 0;
        }

        public static float ToFloat(float value)
        {
            return value;
        }

        public static float ToFloat(object value)
        {
            IConvertible tmp = value as IConvertible;
            if (tmp == null) return 0;
            try
            {
                return System.Convert.ToSingle(tmp);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion ToFloat 转换成float

        #region ToDateTime 转换成DateTime

        /// <summary>
        /// 转换成Byte
        /// </summary>
        public static DateTime ToDateTime(string value)
        {
            try
            {
                return System.Convert.ToDateTime(value);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        public static DateTime ToDateTime(int value)
        {
            return new DateTime(value);
        }

        public static DateTime ToDateTime(uint value)
        {
            return new DateTime(value);
        }

        public static DateTime ToDateTime(long value)
        {
            return new DateTime(value);
        }

        public static DateTime ToDateTime(ulong value)
        {
            return new DateTime((long)value);
        }

        public static DateTime ToDateTime(DateTime value)
        {
            return value;
        }

        public static DateTime ToDateTime(double value)
        {
            return new DateTime((long)value);
        }

        public static DateTime ToDateTime(float value)
        {
            return new DateTime((long)value);
        }

        public static DateTime ToDateTime(object value)
        {
            IConvertible tmp = value as IConvertible;
            if (tmp == null) return DateTime.MinValue;
            try
            {
                return System.Convert.ToDateTime(tmp);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        #endregion ToDateTime 转换成DateTime

        private static object Gen(Type type)
        {
            object tmp = Gen(type);
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].Name.Equals("String")) fields[i].SetValue(tmp, string.Empty);
            }
            return tmp;
        }

        private static object Gen(Type type, FieldInfo[] fields)
        {
            object tmp = Gen(type);
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].Name.Equals("String")) fields[i].SetValue(tmp, string.Empty);
            }
            return tmp;
        }

        /// <summary>
        /// 创建指定基本类型的实例
        /// </summary>
        /// <param name="type">基本类型</param>
        /// <returns>返回指定类型的实例（如果是<see cref="String"/>，返回<see cref="string.Empty"/>）</returns>
        public static object Create(TypeCode code)
        {
            switch (code)
            {
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return 0;

                case TypeCode.Char:
                    return char.MinValue;

                case TypeCode.Boolean:
                    return false;

                case TypeCode.DateTime:
                    return DateTime.MinValue;

                case TypeCode.String:
                    return string.Empty;

                default:
                    return null;
            }
        }

        public static object Create(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return 0;

                case TypeCode.Char:
                    return char.MinValue;

                case TypeCode.Boolean:
                    return false;

                case TypeCode.DateTime:
                    return DateTime.MinValue;

                case TypeCode.String:
                    return string.Empty;

                default:
                    return Gen(type);
            }
        }

        public static object Convert(object value, object dest)
        {
            if (value == null) return dest;
            Type destType = dest.GetType();
            switch (Type.GetTypeCode(destType))
            {
                case TypeCode.Boolean:
                    return ToBool(value);

                case TypeCode.String:
                    return ToString(value);

                case TypeCode.Char:
                    return ToChar(value);

                case TypeCode.Byte:
                    return ToByte(value);

                case TypeCode.SByte:
                    return ToSByte(value);

                case TypeCode.Int16:
                    return ToShort(value);

                case TypeCode.UInt16:
                    return ToUShort(value);

                case TypeCode.Int32:
                    return ToInt(value);

                case TypeCode.UInt32:
                    return ToUInt(value);

                case TypeCode.Int64:
                    return ToLong(value);

                case TypeCode.UInt64:
                    return ToULong(value);

                case TypeCode.Decimal:
                    return ToDecimal(value);

                case TypeCode.Double:
                    return ToDouble(value);

                case TypeCode.Single:
                    return ToFloat(value);

                case TypeCode.DateTime:
                    return ToDateTime(value);

                case TypeCode.Empty:
                case TypeCode.DBNull:
                    throw new InvalidCastException("不能转换为空值！");
                default:
                    Type srcType = value.GetType();
                    if (srcType.Equals(destType)) return value;
                    if (srcType.IsArray) return Convert(value, destType);
                    FieldInfo[] srcInfo = srcType.GetFields(BindingFlags.Instance | BindingFlags.Public);

                    //如果目标是数组
                    if (destType.IsArray)
                    {
                        Type subType = dest.GetType().GetElementType();
                        Array arr = dest as Array;
                        if (arr == null) arr = Array.CreateInstance(subType, srcInfo.Length);
                        int len = arr.Length < srcInfo.Length ? arr.Length : srcInfo.Length;
                        for (int i = 0; i < len; i++)
                        {
                            arr.SetValue(Convert(srcInfo[i].GetValue(value), subType), i);
                        }
                        return arr;
                    }

                    FieldInfo[] destInfo = destType.GetFields(BindingFlags.Instance | BindingFlags.Public);
                    foreach (FieldInfo info in srcInfo)
                    {
                        for (int i = 0; i < destInfo.Length; i++)
                        {
                            if (Compare(destInfo[i].Name, info.Name) == 0)
                            {
                                destInfo[i].SetValue(dest, Convert(info.GetValue(value), destInfo[i].FieldType));
                                break;
                            }
                        }
                    }
                    return dest;
            }
        }

        public static object Convert(object value, Type destType)
        {
            if (value == null)
            {
                if (destType.Name.Equals("String")) return string.Empty;
                return Gen(destType);
            }
            switch (Type.GetTypeCode(destType))
            {
                case TypeCode.Boolean:
                    return ToBool(value);

                case TypeCode.String:
                    return ToString(value);

                case TypeCode.Char:
                    return ToChar(value);

                case TypeCode.Byte:
                    return ToByte(value);

                case TypeCode.SByte:
                    return ToSByte(value);

                case TypeCode.Int16:
                    return ToShort(value);

                case TypeCode.UInt16:
                    return ToUShort(value);

                case TypeCode.Int32:
                    return ToInt(value);

                case TypeCode.UInt32:
                    return ToUInt(value);

                case TypeCode.Int64:
                    return ToLong(value);

                case TypeCode.UInt64:
                    return ToULong(value);

                case TypeCode.Decimal:
                    return ToDecimal(value);

                case TypeCode.Double:
                    return ToDouble(value);

                case TypeCode.Single:
                    return ToFloat(value);

                case TypeCode.DateTime:
                    return ToDateTime(value);

                case TypeCode.Empty:
                case TypeCode.DBNull:
                    throw new InvalidCastException("不能转换为空值！");
                default:
                    Type srcType = value.GetType();

                    //如果两个类型一样，则直接返回
                    if (srcType == destType) return value;

                    if (srcType.IsArray) return Convert(value, destType);
                    FieldInfo[] srcInfo = srcType.GetFields(BindingFlags.Instance | BindingFlags.Public);

                    //如果目标是数组
                    if (destType.IsArray)
                    {
                        Type subType = destType.GetElementType();
                        Array arr = Array.CreateInstance(subType, srcInfo.Length);
                        for (int i = 0; i < arr.Length; i++)
                        {
                            arr.SetValue(Convert(srcInfo[i].GetValue(value), subType), i);
                        }
                        return arr;
                    }
                    FieldInfo[] destInfo = destType.GetFields(BindingFlags.Instance | BindingFlags.Public);
                    object dest = Gen(destType);
                    foreach (FieldInfo info in srcInfo)
                    {
                        for (int i = 0; i < destInfo.Length; i++)
                        {
                            if (Compare(destInfo[i].Name, info.Name) == 0)
                            {
                                destInfo[i].SetValue(dest, Convert(info.GetValue(value), destInfo[i].FieldType));
                                break;
                            }
                        }
                    }
                    return dest;
            }
        }

        /// <summary>
        /// 根据名称在两个结构对象之间进行转换
        /// </summary>
        /// <param name="value">源对象</param>
        /// <param name="dest">目标对象</param>
        /// <returns>返回转换后的目标对象</returns>
        public static object ConvertByName(object value, object dest)
        {
            return Convert(value, dest);
        }

        public static object ConvertByName(object value, Type destType)
        {
            return Convert(value, destType);
        }

        /// <summary>
        /// 根据顺序在两个结构对象之间进行转换
        /// </summary>
        /// <param name="value">源对象</param>
        /// <param name="dest">目标对象</param>
        /// <returns>返回转换后的目标对象</returns>
        public static object ConvertByOrder(object value, object dest)
        {
            if (value == null) return dest;
            Type destType = dest.GetType();
            switch (Type.GetTypeCode(destType))
            {
                case TypeCode.Boolean:
                    return ToBool(value);

                case TypeCode.String:
                    return ToString(value);

                case TypeCode.Char:
                    return ToChar(value);

                case TypeCode.Byte:
                    return ToByte(value);

                case TypeCode.SByte:
                    return ToSByte(value);

                case TypeCode.Int16:
                    return ToShort(value);

                case TypeCode.UInt16:
                    return ToUShort(value);

                case TypeCode.Int32:
                    return ToInt(value);

                case TypeCode.UInt32:
                    return ToUInt(value);

                case TypeCode.Int64:
                    return ToLong(value);

                case TypeCode.UInt64:
                    return ToULong(value);

                case TypeCode.Decimal:
                    return ToDecimal(value);

                case TypeCode.Double:
                    return ToDouble(value);

                case TypeCode.Single:
                    return ToFloat(value);

                case TypeCode.DateTime:
                    return ToDateTime(value);

                case TypeCode.Empty:
                case TypeCode.DBNull:
                    throw new InvalidCastException("不能转换为空值！");
                default:
                    Type srcType = value.GetType();
                    if (srcType.IsArray) return Convert(value, destType);

                    FieldInfo[] srcInfo = srcType.GetFields(BindingFlags.Instance | BindingFlags.Public);

                    int len;

                    //如果目标是数组
                    if (destType.IsArray)
                    {
                        Type subType = dest.GetType().GetElementType();
                        Array arr = dest as Array;
                        if (arr == null) arr = Array.CreateInstance(subType, srcInfo.Length);
                        len = arr.Length < srcInfo.Length ? arr.Length : srcInfo.Length;
                        for (int i = 0; i < len; i++)
                        {
                            arr.SetValue(Convert(srcInfo[i].GetValue(value), subType), i);
                        }
                        return arr;
                    }
                    FieldInfo[] destInfo = destType.GetFields(BindingFlags.Instance | BindingFlags.Public);

                    len = srcInfo.Length < destInfo.Length ? srcInfo.Length : destInfo.Length;
                    for (int i = 0; i < len; i++)
                    {
                        destInfo[i].SetValue(dest, Convert(srcInfo[i].GetValue(value), destInfo[i].FieldType));
                    }
                    return dest;
            }
        }

        public static object ConvertByOrder(object value, Type destType)
        {
            if (value == null)
            {
                if (destType.Name.Equals("String")) return string.Empty;
                return Gen(destType);
            }
            switch (Type.GetTypeCode(destType))
            {
                case TypeCode.Boolean:
                    return ToBool(value);

                case TypeCode.String:
                    return ToString(value);

                case TypeCode.Char:
                    return ToChar(value);

                case TypeCode.Byte:
                    return ToByte(value);

                case TypeCode.SByte:
                    return ToSByte(value);

                case TypeCode.Int16:
                    return ToShort(value);

                case TypeCode.UInt16:
                    return ToUShort(value);

                case TypeCode.Int32:
                    return ToInt(value);

                case TypeCode.UInt32:
                    return ToUInt(value);

                case TypeCode.Int64:
                    return ToLong(value);

                case TypeCode.UInt64:
                    return ToULong(value);

                case TypeCode.Decimal:
                    return ToDecimal(value);

                case TypeCode.Double:
                    return ToDouble(value);

                case TypeCode.Single:
                    return ToFloat(value);

                case TypeCode.DateTime:
                    return ToDateTime(value);

                case TypeCode.Empty:
                case TypeCode.DBNull:
                    throw new InvalidCastException("不能转换为空值！");
                default:
                    Type srcType = value.GetType();
                    if (srcType.IsArray) return Convert(value, destType);

                    FieldInfo[] srcInfo = srcType.GetFields(BindingFlags.Instance | BindingFlags.Public);

                    //如果目标是数组
                    if (destType.IsArray)
                    {
                        Type subType = destType.GetElementType();
                        Array arr = Array.CreateInstance(subType, srcInfo.Length);
                        for (int i = 0; i < arr.Length; i++)
                        {
                            arr.SetValue(Convert(srcInfo[i].GetValue(value), subType), i);
                        }
                        return arr;
                    }
                    FieldInfo[] destInfo = destType.GetFields(BindingFlags.Instance | BindingFlags.Public);

                    int len = srcInfo.Length < destInfo.Length ? srcInfo.Length : destInfo.Length;
                    object dest = Gen(destType);
                    for (int i = 0; i < len; i++)
                    {
                        destInfo[i].SetValue(dest, Convert(srcInfo[i].GetValue(value), destInfo[i].FieldType));
                    }
                    return dest;
            }
        }

        public static object Convert(Type ParaType, NameValueCollection List)
        {
            FieldInfo[] info = ParaType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            object dest = Gen(ParaType);
            foreach (FieldInfo fld in info)
            {
                fld.SetValue(dest, Convert(List[fld.Name], fld.FieldType));
            }
            return dest;
        }

        public static object Convert(object dest, NameValueCollection List)
        {
            FieldInfo[] info = dest.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo fld in info)
            {
                fld.SetValue(dest, Convert(List[fld.Name], fld.FieldType));
            }
            return dest;
        }

        /// <summary>
        /// 是否是字母
        /// </summary>
        public static bool IsLetter(char c)
        {
            return (c >= 65 && c <= 90 || c >= 97 && c <= 122);
        }

        /// <summary>
        /// 是否是字母或汉字
        /// </summary>
        public static bool IsCharacter(char c)
        {
            return (c >= 65 && c <= 90 || c >= 97 && c <= 122 || c > 255);
        }

        /// <summary>
        /// 是否是数字
        /// </summary>
        public static bool IsDigit(char c)
        {
            return (c >= 48 && c <= 57);
        }

        /// <summary>
        /// 是否是字母或数字
        /// </summary>
        public static bool IsWord(char c)
        {
            return (c >= 65 && c <= 90 || c >= 97 && c <= 122 || c >= 48 && c <= 57);
        }

        public unsafe static string Trim(string str, int maxlen)
        {
            if (maxlen <= 0) return str;
            StringBuilder sb = new StringBuilder();
            str = str.Replace(" ", string.Empty);
            maxlen *= 2;
            int count = 0;
            fixed (char* p = str)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (p[i] > 255 || p[i] < 0)
                        count += 2;
                    else
                        count++;
                    if (count > maxlen) break;
                    sb.Append(p[i]);
                    if (count == maxlen) break;
                }
            }
            return sb.ToString();
        }

        public static string FileEncode(string str)
        {
            return Regex.Replace(str, @"%|\/|\\|:|\*|\?|""|<|>|\|", new MatchEvaluator(replace), RegexOptions.Compiled);
        }

        private static string replace(Match m)
        {
            switch (m.Groups[0].Value)
            {
                case "%":
                    return "%25";

                case "\\":
                    return "%5C";

                case "/":
                    return "%2F";

                case ":":
                    return "%3A";

                case "*":
                    return "%2A";

                case "?":
                    return "%3F";

                case "\"":
                    return "%22";

                case "<":
                    return "%3C";

                case ">":
                    return "%3E";

                case "|":
                    return "%7C";

                default:
                    return m.Groups[0].Value;
            }
        }
    }
}