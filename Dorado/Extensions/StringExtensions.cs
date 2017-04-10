using Dorado.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Dorado.Extensions
{
    /// <summary>
    /// 字符串的工具类
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public static class StringExtensions
    {
        public const string CarriageReturnLineFeed = "\r\n";
        public const string Empty = "";
        public const char CarriageReturn = '\r';
        public const char LineFeed = '\n';
        public const char Tab = '\t';

        private delegate void ActionLine(TextWriter textWriter, string line);

        [DebuggerStepThrough]
        public static int ToInt(this char value)
        {
            if ((value >= '0') && (value <= '9'))
            {
                return (value - '0');
            }
            if ((value >= 'a') && (value <= 'f'))
            {
                return ((value - 'a') + 10);
            }
            if ((value >= 'A') && (value <= 'F'))
            {
                return ((value - 'A') + 10);
            }
            return -1;
        }

        [DebuggerStepThrough]
        public static string ToUnicode(this char c)
        {
            using (StringWriter w = new StringWriter(CultureInfo.InvariantCulture))
            {
                WriteCharAsUnicode(c, w);
                return w.ToString();
            }
        }

        internal static void WriteCharAsUnicode(char c, TextWriter writer)
        {
            Guard.ArgumentNotNull(writer, "writer");

            char h1 = ((c >> 12) & '\x000f').ToHex();
            char h2 = ((c >> 8) & '\x000f').ToHex();
            char h3 = ((c >> 4) & '\x000f').ToHex();
            char h4 = (c & '\x000f').ToHex();

            writer.Write('\\');
            writer.Write('u');
            writer.Write(h1);
            writer.Write(h2);
            writer.Write(h3);
            writer.Write(h4);
        }

        [DebuggerStepThrough]
        public static T ToEnum<T>(this string value, T defaultValue)
        {
            if (!value.HasValue())
            {
                return defaultValue;
            }
            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch (ArgumentException)
            {
                return defaultValue;
            }
        }

        [DebuggerStepThrough]
        public static string ToSafe(this string value, string defaultValue = null)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }
            return (defaultValue ?? String.Empty);
        }

        [DebuggerStepThrough]
        public static string EmptyNull(this string value)
        {
            return (value ?? string.Empty).Trim();
        }

        [DebuggerStepThrough]
        public static string NullEmpty(this string value)
        {
            return (string.IsNullOrEmpty(value)) ? null : value;
        }

        /// <summary>
        /// Formats a string to an invariant culture
        /// </summary>
        /// <param name="formatString">The format string.</param>
        /// <param name="objects">The objects.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string FormatInvariant(this string format, params object[] objects)
        {
            return string.Format(CultureInfo.InvariantCulture, format, objects);
        }

        /// <summary>
        /// Formats a string to the current culture.
        /// </summary>
        /// <param name="formatString">The format string.</param>
        /// <param name="objects">The objects.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string FormatCurrent(this string format, params object[] objects)
        {
            return string.Format(CultureInfo.CurrentCulture, format, objects);
        }

        /// <summary>
        /// Formats a string to the current UI culture.
        /// </summary>
        /// <param name="formatString">The format string.</param>
        /// <param name="objects">The objects.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string FormatCurrentUI(this string format, params object[] objects)
        {
            return string.Format(CultureInfo.CurrentUICulture, format, objects);
        }

        [DebuggerStepThrough]
        public static string FormatWith(this string format, params object[] args)
        {
            return FormatWith(format, CultureInfo.CurrentCulture, args);
        }

        [DebuggerStepThrough]
        public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            return string.Format(provider, format, args);
        }

        /// <summary>
        /// Determines whether this instance and another specified System.String object have the same value.
        /// </summary>
        /// <param name="instance">The string to check equality.</param>
        /// <param name="comparing">The comparing with string.</param>
        /// <returns>
        /// <c>true</c> if the value of the comparing parameter is the same as this string; otherwise, <c>false</c>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsCaseSensitiveEqual(this string value, string comparing)
        {
            return string.CompareOrdinal(value, comparing) == 0;
        }

        /// <summary>
        /// Determines whether this instance and another specified System.String object have the same value.
        /// </summary>
        /// <param name="instance">The string to check equality.</param>
        /// <param name="comparing">The comparing with string.</param>
        /// <returns>
        /// <c>true</c> if the value of the comparing parameter is the same as this string; otherwise, <c>false</c>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsCaseInsensitiveEqual(this string value, string comparing)
        {
            return string.Compare(value, comparing, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Determines whether the string is null, empty or all whitespace.
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsEmpty(this string value)
        {
            if (value == null || value.Length == 0)
                return true;

            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the string is all white space. Empty string will return false.
        /// </summary>
        /// <param name="s">The string to test whether it is all white space.</param>
        /// <returns>
        /// 	<c>true</c> if the string is all white space; otherwise, <c>false</c>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsWhiteSpace(this string value)
        {
            Guard.ArgumentNotNull(value, "value");

            if (value.Length == 0)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                    return false;
            }

            return true;
        }

        [DebuggerStepThrough]
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <remarks>codehint: sm-edit</remarks>
        /// <remarks>to get equivalent result to PHPs md5 function call Hash("my value", false, false).</remarks>
        [DebuggerStepThrough]
        public static string Hash(this string value, bool toBase64 = false, bool unicode = false)
        {
            Guard.ArgumentNotNull(value, "value");

            using (MD5 md5 = MD5.Create())
            {
                byte[] data = null;
                if (unicode)
                    data = Encoding.Unicode.GetBytes(value);
                else
                    data = Encoding.ASCII.GetBytes(value);

                if (toBase64)
                {
                    byte[] hash = md5.ComputeHash(data);
                    return Convert.ToBase64String(hash);
                }
                else
                {
                    return md5.ComputeHash(data).ToHexString().ToLower();
                }
            }
        }

        [DebuggerStepThrough]
        public static bool IsWebUrl(this string value)
        {
            return !string.IsNullOrEmpty(value) && RegularExpressions.IsWebUrl.IsMatch(value.Trim());
        }

        [DebuggerStepThrough]
        public static bool IsNumeric(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            return !RegularExpressions.IsNotNumber.IsMatch(value) &&
            !RegularExpressions.HasTwoDot.IsMatch(value) &&
            !RegularExpressions.HasTwoMinus.IsMatch(value) &&
            RegularExpressions.IsNumeric.IsMatch(value);
        }

        /// <summary>
        /// Ensures that a string only contains numeric values
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Input string with only numeric values, empty string if input is null or empty</returns>
        [DebuggerStepThrough]
        public static string EnsureNumericOnly(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            return new String(str.Where(c => Char.IsDigit(c)).ToArray());
        }

        [DebuggerStepThrough]
        public static bool IsAlpha(this string value)
        {
            return RegularExpressions.IsAlpha.IsMatch(value);
        }

        [DebuggerStepThrough]
        public static bool IsAlphaNumeric(this string value)
        {
            return RegularExpressions.IsAlphaNumeric.IsMatch(value);
        }

        [DebuggerStepThrough]
        public static string Truncate(this string value, int maxLength, string suffix = "")
        {
            Guard.ArgumentNotNull(suffix, "suffix");
            Guard.ArgumentPositive(maxLength, "maxLength");

            int subStringLength = maxLength - suffix.Length;

            if (subStringLength <= 0)
                throw new ArgumentException("Length of suffix string is greater or equal to maximumLength", "maxLength");

            if (value != null && value.Length > maxLength)
            {
                string truncatedString = value.Substring(0, subStringLength);
                // in case the last character is a space
                truncatedString = truncatedString.Trim();
                truncatedString += suffix;

                return truncatedString;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Determines whether the string contains white space.
        /// </summary>
        /// <param name="s">The string to test for white space.</param>
        /// <returns>
        /// 	<c>true</c> if the string contains white space; otherwise, <c>false</c>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool ContainsWhiteSpace(this string value)
        {
            Guard.ArgumentNotNull(value, "value");

            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsWhiteSpace(value[i]))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Ensure that a string starts with a string.
        /// </summary>
        /// <param name="value">The target string</param>
        /// <param name="startsWith">The string the target string should start with</param>
        /// <returns>The resulting string</returns>
        [DebuggerStepThrough]
        public static string EnsureStartsWith(this string value, string startsWith)
        {
            Guard.ArgumentNotNull(value, "value");
            Guard.ArgumentNotNull(startsWith, "startsWith");

            return value.StartsWith(startsWith) ? value : (startsWith + value);
        }

        /// <summary>
        /// Ensures the target string ends with the specified string.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        /// <returns>The target string with the value string at the end.</returns>
        [DebuggerStepThrough]
        public static string EnsureEndsWith(this string value, string endWith)
        {
            Guard.ArgumentNotNull(value, "value");
            Guard.ArgumentNotNull(endWith, "endWith");

            if (value.Length >= endWith.Length)
            {
                if (string.Compare(value, value.Length - endWith.Length, endWith, 0, endWith.Length, StringComparison.OrdinalIgnoreCase) == 0)
                    return value;

                string trimmedString = value.TrimEnd(null);

                if (string.Compare(trimmedString, trimmedString.Length - endWith.Length, endWith, 0, endWith.Length, StringComparison.OrdinalIgnoreCase) == 0)
                    return value;
            }

            return value + endWith;
        }

        [DebuggerStepThrough]
        public static int? GetLength(this string value)
        {
            if (value == null)
                return null;
            else
                return value.Length;
        }

        public static string UrlEncode(this string value)
        {
            var urlEncode = HttpUtility.UrlEncode(value, System.Text.Encoding.UTF8);
            if (urlEncode != null)
                return urlEncode
                    .Replace("!", "%21")
                    .Replace("(", "%28")
                    .Replace(")", "%29")
                    .Replace("+", "%20");
            return value;
        }

        [DebuggerStepThrough]
        public static string UrlDecode(this string value)
        {
            return HttpUtility.UrlDecode(value);
        }

        [DebuggerStepThrough]
        public static string AttributeEncode(this string value)
        {
            return HttpUtility.HtmlAttributeEncode(value);
        }

        [DebuggerStepThrough]
        public static string HtmlEncode(this string value)
        {
            return HttpUtility.HtmlEncode(value);
        }

        [DebuggerStepThrough]
        public static string HtmlDecode(this string value)
        {
            return HttpUtility.HtmlDecode(value);
        }

        [DebuggerStepThrough]
        public static string RemoveHtml(this string value)
        {
            return RemoveHtmlInternal(value, null);
        }

        public static string RemoveHtml(this string value, ICollection<string> removeTags)
        {
            return RemoveHtmlInternal(value, removeTags);
        }

        private static string RemoveHtmlInternal(string s, ICollection<string> removeTags)
        {
            List<string> removeTagsUpper = null;
            if (removeTags != null)
            {
                removeTagsUpper = new List<string>(removeTags.Count);

                foreach (string tag in removeTags)
                {
                    removeTagsUpper.Add(tag.ToUpperInvariant());
                }
            }

            return RegularExpressions.RemoveHtml.Replace(s, delegate (Match match)
            {
                string tag = match.Groups["tag"].Value.ToUpperInvariant();

                if (removeTagsUpper == null)
                    return string.Empty;
                else if (removeTagsUpper.Contains(tag))
                    return string.Empty;
                else
                    return match.Value;
            });
        }

        /// <summary>
        /// Replaces pascal casing with spaces. For example "CustomerId" would become "Customer Id".
        /// Strings that already contain spaces are ignored.
        /// </summary>
        /// <param name="input">String to split</param>
        /// <returns>The string after being split</returns>
        [DebuggerStepThrough]
        public static string SplitPascalCase(this string value)
        {
            //return Regex.Replace(input, "([A-Z][a-z])", " $1", RegexOptions.Compiled).Trim();
            StringBuilder sb = new StringBuilder();
            char[] ca = value.ToCharArray();
            sb.Append(ca[0]);
            for (int i = 1; i < ca.Length - 1; i++)
            {
                char c = ca[i];
                if (char.IsUpper(c) && (char.IsLower(ca[i + 1]) || char.IsLower(ca[i - 1])))
                {
                    sb.Append(" ");
                }
                sb.Append(c);
            }
            if (ca.Length > 1)
            {
                sb.Append(ca[ca.Length - 1]);
            }

            return sb.ToString();
        }

        /// <remarks>codehint: sm-add</remarks>
        [DebuggerStepThrough]
        public static string[] SplitSafe(this string value, string separator)
        {
            if (string.IsNullOrEmpty(value))
                return new string[0];
            return value.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>Splits a string into two strings</summary>
        /// <remarks>codehint: sm-add</remarks>
        /// <returns>true: success, false: failure</returns>
        [DebuggerStepThrough]
        public static bool SplitToPair(this string value, out string strLeft, out string strRight, string delimiter)
        {
            int idx = -1;
            if (value.IsNullOrEmpty() || delimiter.IsNullOrEmpty() || (idx = value.IndexOf(delimiter)) == -1)
            {
                strLeft = value;
                strRight = "";
                return false;
            }
            strLeft = value.Substring(0, idx);
            strRight = value.Substring(idx + delimiter.Length);
            return true;
        }

        [DebuggerStepThrough]
        public static string ToCamelCase(this string instance)
        {
            char ch = instance[0];
            return (ch.ToString().ToLowerInvariant() + instance.Substring(1));
        }

        [DebuggerStepThrough]
        public static string ReplaceNewLines(this string value, string replacement)
        {
            StringReader sr = new StringReader(value);
            StringBuilder sb = new StringBuilder();

            bool first = true;

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (first)
                    first = false;
                else
                    sb.Append(replacement);

                sb.Append(line);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Indents the specified string.
        /// </summary>
        /// <param name="s">The string to indent.</param>
        /// <param name="indentation">The number of characters to indent by.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string Indent(this string value, int indentation)
        {
            return Indent(value, indentation, ' ');
        }

        /// <summary>
        /// Indents the specified string.
        /// </summary>
        /// <param name="s">The string to indent.</param>
        /// <param name="indentation">The number of characters to indent by.</param>
        /// <param name="indentChar">The indent character.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string Indent(this string value, int indentation, char indentChar)
        {
            Guard.ArgumentNotNull(value, "value");
            Guard.ArgumentPositive(indentation, "indentation");

            StringReader sr = new StringReader(value);
            StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);

            ActionTextReaderLine(sr, sw, delegate (TextWriter tw, string line)
            {
                tw.Write(new string(indentChar, indentation));
                tw.Write(line);
            });

            return sw.ToString();
        }

        /// <summary>
        /// Numbers the lines.
        /// </summary>
        /// <param name="s">The string to number.</param>
        /// <returns></returns>
        public static string NumberLines(this string value)
        {
            Guard.ArgumentNotNull(value, "value");

            StringReader sr = new StringReader(value);
            StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);

            int lineNumber = 1;

            ActionTextReaderLine(sr, sw, delegate (TextWriter tw, string line)
            {
                tw.Write(lineNumber.ToString(CultureInfo.InvariantCulture).PadLeft(4));
                tw.Write(". ");
                tw.Write(line);

                lineNumber++;
            });

            return sw.ToString();
        }

        [DebuggerStepThrough]
        public static string EncodeJsString(this string value)
        {
            return EncodeJsString(value, '"', true);
        }

        [DebuggerStepThrough]
        public static string EncodeJsString(this string value, char delimiter, bool appendDelimiters)
        {
            StringBuilder sb = new StringBuilder(value.GetLength() ?? 16);
            using (StringWriter w = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                EncodeJsString(w, value, delimiter, appendDelimiters);
                return w.ToString();
            }
        }

        [DebuggerStepThrough]
        public static bool IsEnclosedIn(this string value, string enclosedIn)
        {
            return value.IsEnclosedIn(enclosedIn, StringComparison.CurrentCulture);
        }

        [DebuggerStepThrough]
        public static bool IsEnclosedIn(this string value, string enclosedIn, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(enclosedIn))
                return false;

            if (enclosedIn.Length == 1)
                return value.IsEnclosedIn(enclosedIn, enclosedIn, comparisonType);

            if (enclosedIn.Length % 2 == 0)
            {
                int len = enclosedIn.Length / 2;
                return value.IsEnclosedIn(
                    enclosedIn.Substring(0, len),
                    enclosedIn.Substring(len, len),
                    comparisonType);
            }

            return false;
        }

        [DebuggerStepThrough]
        public static bool IsEnclosedIn(this string value, string start, string end)
        {
            return value.IsEnclosedIn(start, end, StringComparison.CurrentCulture);
        }

        [DebuggerStepThrough]
        public static bool IsEnclosedIn(this string value, string start, string end, StringComparison comparisonType)
        {
            return value.StartsWith(start, comparisonType) && value.EndsWith(end, comparisonType);
        }

        public static string RemoveEncloser(this string value, string encloser)
        {
            return value.RemoveEncloser(encloser, StringComparison.CurrentCulture);
        }

        public static string RemoveEncloser(this string value, string encloser, StringComparison comparisonType)
        {
            if (value.IsEnclosedIn(encloser, comparisonType))
            {
                int len = encloser.Length / 2;
                return value.Substring(
                    len,
                    value.Length - (len * 2));
            }

            return value;
        }

        public static string RemoveEncloser(this string value, string start, string end)
        {
            return value.RemoveEncloser(start, end, StringComparison.CurrentCulture);
        }

        public static string RemoveEncloser(this string value, string start, string end, StringComparison comparisonType)
        {
            if (value.IsEnclosedIn(start, end, comparisonType))
                return value.Substring(
                    start.Length,
                    value.Length - (start.Length + end.Length));

            return value;
        }

        // codehint: sm-add (begin)

        /// <summary>Debug.WriteLine</summary>
        /// <remarks>codehint: sm-add</remarks>
        [DebuggerStepThrough]
        public static void Dump(this string value)
        {
            Debug.WriteLine(value);
        }

        /// <summary>Smart way to create a HTML attribute with a leading space.</summary>
        /// <remarks>codehint: sm-add</remarks>
        /// <param name="name">Name of the attribute.</param>
        public static string ToAttribute(this string value, string name, bool htmlEncode = true)
        {
            if (value == null || name.IsNullOrEmpty())
                return "";

            if (value == "" && name != "value" && !name.StartsWith("data"))
                return "";

            if (name == "maxlength" && (value == "" || value == "0"))
                return "";

            if (name == "checked" || name == "disabled" || name == "multiple")
            {
                if (value == "" || string.Compare(value, "false", true) == 0)
                    return "";
                value = (string.Compare(value, "true", true) == 0 ? name : value);
            }

            if (name.StartsWith("data"))
                name = name.Insert(4, "-");

            return string.Format(" {0}=\"{1}\"", name, htmlEncode ? HttpUtility.HtmlEncode(value) : value);
        }

        /// <summary>Appends grow and uses delimiter if the string is not empty.</summary>
        [DebuggerStepThrough]
        public static string Grow(this string value, string grow, string delimiter)
        {
            if (string.IsNullOrEmpty(value))
                return (string.IsNullOrEmpty(grow) ? "" : grow);

            if (string.IsNullOrEmpty(grow))
                return (string.IsNullOrEmpty(value) ? "" : value);

            return string.Format("{0}{1}{2}", value, delimiter, grow);
        }

        /// <summary>Returns n/a if string is empty else self.</summary>
        [DebuggerStepThrough]
        public static string NaIfEmpty(this string value)
        {
            return (value.HasValue() ? value : "n/a");
        }

        /// <summary>Replaces substring with position x1 to x2 by replaceBy.</summary>
        [DebuggerStepThrough]
        public static string Replace(this string value, int x1, int x2, string replaceBy = null)
        {
            if (value.HasValue() && x1 > 0 && x2 > x1 && x2 < value.Length)
            {
                return value.Substring(0, x1) + (replaceBy == null ? "" : replaceBy) + value.Substring(x2 + 1);
            }
            return value;
        }

        [DebuggerStepThrough]
        public static string TrimSafe(this string value)
        {
            return (value.HasValue() ? value.Trim() : value);
        }

        [DebuggerStepThrough]
        public static string Prettify(this string value, bool allowSpace = false, char[] allowChars = null)
        {
            string res = "";
            try
            {
                if (value.HasValue())
                {
                    StringBuilder sb = new StringBuilder();
                    bool space = false;
                    char ch;

                    for (int i = 0; i < value.Length; ++i)
                    {
                        ch = value[i];

                        if (ch == ' ' || ch == '-')
                        {
                            if (allowSpace && ch == ' ')
                                sb.Append(' ');
                            else if (!space)
                                sb.Append('-');
                            space = true;
                            continue;
                        }

                        space = false;

                        if ((ch >= 48 && ch <= 57) || (ch >= 65 && ch <= 90) || (ch >= 97 && ch <= 122))
                        {
                            sb.Append(ch);
                            continue;
                        }

                        if (allowChars != null && allowChars.Contains(ch))
                        {
                            sb.Append(ch);
                            continue;
                        }

                        switch (ch)
                        {
                            case '_':
                                sb.Append(ch);
                                break;

                            case 'ä':
                                sb.Append("ae");
                                break;

                            case 'ö':
                                sb.Append("oe");
                                break;

                            case 'ü':
                                sb.Append("ue");
                                break;

                            case 'ß':
                                sb.Append("ss");
                                break;

                            case 'Ä':
                                sb.Append("AE");
                                break;

                            case 'Ö':
                                sb.Append("OE");
                                break;

                            case 'Ü':
                                sb.Append("UE");
                                break;

                            case 'é':
                            case 'è':
                            case 'ê':
                                sb.Append('e');
                                break;

                            case 'á':
                            case 'à':
                            case 'â':
                                sb.Append('a');
                                break;

                            case 'ú':
                            case 'ù':
                            case 'û':
                                sb.Append('u');
                                break;

                            case 'ó':
                            case 'ò':
                            case 'ô':
                                sb.Append('o');
                                break;
                        }	// switch
                    }	// for

                    if (sb.Length > 0)
                    {
                        res = sb.ToString().Trim(new char[] { ' ', '-' });

                        Regex pat = new Regex(@"(-{2,})");		// remove double SpaceChar
                        res = pat.Replace(res, "-");
                        res = res.Replace("__", "_");
                    }
                }
            }
            catch (Exception exp)
            {
                exp.Dump();
            }
            return (res.Length > 0 ? res : "null");
        }

        public static string SanitizeHtmlId(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            StringBuilder builder = new StringBuilder(value.Length);
            int index = value.IndexOf("#");
            int num2 = value.LastIndexOf("#");
            if (num2 > index)
            {
                ReplaceInvalidHtmlIdCharacters(value.Substring(0, index), builder);
                builder.Append(value.Substring(index, (num2 - index) + 1));
                ReplaceInvalidHtmlIdCharacters(value.Substring(num2 + 1), builder);
            }
            else
            {
                ReplaceInvalidHtmlIdCharacters(value, builder);
            }
            return builder.ToString();
        }

        private static bool IsValidHtmlIdCharacter(char c)
        {
            bool invalid = (c == '?' || c == '!' || c == '#' || c == '.' || c == ' ' || c == ';' || c == ':');
            return !invalid;
        }

        private static void ReplaceInvalidHtmlIdCharacters(string part, StringBuilder builder)
        {
            for (int i = 0; i < part.Length; i++)
            {
                char c = part[i];
                if (IsValidHtmlIdCharacter(c))
                {
                    builder.Append(c);
                }
                else
                {
                    builder.Append('_');
                }
            }
        }

        public static string Sha(this string value)
        {
            if (value.HasValue())
            {
                using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                {
                    byte[] data = Encoding.ASCII.GetBytes(value);

                    return sha1.ComputeHash(data).ToHexString();
                }
            }
            return "";
        }

        [DebuggerStepThrough]
        public static bool IsMatch(this string input, string pattern, RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline)
        {
            return Regex.IsMatch(input, pattern, options);
        }

        [DebuggerStepThrough]
        public static bool IsMatch(this string input, string pattern, out Match match, RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline)
        {
            match = Regex.Match(input, pattern, options);
            return match.Success;
        }

        public static string RegexRemove(this string input, string pattern, RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline)
        {
            return Regex.Replace(input, pattern, string.Empty, options);
        }

        public static string RegexReplace(this string input, string pattern, string replacement, RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline)
        {
            return Regex.Replace(input, pattern, replacement, options);
        }

        [DebuggerStepThrough]
        public static string ToValidFileName(this string input, string replacement = "-")
        {
            return input.ToValidPathInternal(false, replacement);
        }

        [DebuggerStepThrough]
        public static string ToValidPath(this string input, string replacement = "-")
        {
            return input.ToValidPathInternal(true, replacement);
        }

        private static string ToValidPathInternal(this string input, bool isPath, string replacement)
        {
            var result = input.ToSafe();

            char[] invalidChars = isPath ? Path.GetInvalidPathChars() : Path.GetInvalidFileNameChars();

            foreach (var c in invalidChars)
            {
                result = result.Replace(c.ToString(), replacement ?? "-");
            }

            return result;
        }

        [DebuggerStepThrough]
        public static int[] ToIntArray(this string s)
        {
            return Array.ConvertAll(s.SplitSafe(","), v => int.Parse(v));
        }

        [DebuggerStepThrough]
        public static bool ToIntArrayContains(this string s, int value, bool defaultValue)
        {
            var arr = s.ToIntArray();
            if (arr == null || arr.Count() <= 0)
                return defaultValue;
            return arr.Contains(value);
        }

        [DebuggerStepThrough]
        public static string RemoveInvalidXmlChars(this string s)
        {
            if (s.IsNullOrEmpty())
                return s;

            return Regex.Replace(s, @"[^\u0009\u000A\u000D\u0020-\uD7FF\uE000-\uFFFD]", "", RegexOptions.Compiled);
        }

        private static void EncodeJsChar(TextWriter writer, char c, char delimiter)
        {
            switch (c)
            {
                case '\t':
                    writer.Write(@"\t");
                    break;

                case '\n':
                    writer.Write(@"\n");
                    break;

                case '\r':
                    writer.Write(@"\r");
                    break;

                case '\f':
                    writer.Write(@"\f");
                    break;

                case '\b':
                    writer.Write(@"\b");
                    break;

                case '\\':
                    writer.Write(@"\\");
                    break;
                //case '<':
                //case '>':
                //case '\'':
                //  StringUtils.WriteCharAsUnicode(writer, c);
                //  break;
                case '\'':
                    // only escape if this charater is being used as the delimiter
                    writer.Write((delimiter == '\'') ? @"\'" : @"'");
                    break;

                case '"':
                    // only escape if this charater is being used as the delimiter
                    writer.Write((delimiter == '"') ? "\\\"" : @"""");
                    break;

                default:
                    if (c > '\u001f')
                        writer.Write(c);
                    else
                        WriteCharAsUnicode(c, writer);
                    break;
            }
        }

        private static void EncodeJsString(TextWriter writer, string value, char delimiter, bool appendDelimiters)
        {
            // leading delimiter
            if (appendDelimiters)
                writer.Write(delimiter);

            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    EncodeJsChar(writer, value[i], delimiter);
                }
            }

            // trailing delimiter
            if (appendDelimiters)
                writer.Write(delimiter);
        }

        private static void ActionTextReaderLine(TextReader textReader, TextWriter textWriter, ActionLine lineAction)
        {
            string line;
            bool firstLine = true;
            while ((line = textReader.ReadLine()) != null)
            {
                if (!firstLine)
                    textWriter.WriteLine();
                else
                    firstLine = false;

                lineAction(textWriter, line);
            }
        }

        /// <summary>
        /// 截取从开始到指定字符的子串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string TruncateTo(this string s, char c)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            int index = s.IndexOf(c);
            if (index < 0)
                return s;

            return s.Substring(0, index);
        }

        /// <summary>
        /// 截取指定字符之后的字符串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string TruncateFrom(this string s, char c)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            int index = s.IndexOf(c);
            if (index < 0)
                return s;

            return s.Substring(index + 1);
        }

        /// <summary>
        /// 截取指定字符之后的字符串，该字符是从后面开始寻找
        /// </summary>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string TruncateFromLast(this string s, char c)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            int index = s.LastIndexOf(c);
            if (index < 0)
                return s;

            return s.Substring(index + 1);
        }

        /// <summary>
        /// 将Byte数组读取为字符串，自动辨别其编码
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string ReadBuffer(byte[] buffer, int start, int count)
        {
            Contract.Requires(buffer != null && start >= 0 && count >= 0);

            using (StreamReader sr = new StreamReader(new MemoryStream(buffer, start, count)))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// 将字符串转换为字节流，并在开头附加其编码标识
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ToBytes(string s, Encoding encoding)
        {
            using (MemoryStream mStream = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(mStream, encoding))
            {
                sw.Write(s);
                sw.Flush();
                return mStream.ToArray();
            }
        }

        /// <summary>
        /// 将Byte数组读取为字符串，自动辨别其编码
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string ReadBuffer(byte[] buffer)
        {
            Contract.Requires(buffer != null);

            return ReadBuffer(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 判断字符串是否以指定的字符开始
        /// </summary>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool StartsWith(this string s, char c)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            return s[0] == c;
        }

        /// <summary>
        /// 判断字符串是否以指定的字符结尾
        /// </summary>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool EndsWith(this string s, char c)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            return s[s.Length - 1] == c;
        }

        public static string Join(string separator, bool ignoreNull, params string[] values)
        {
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < values.Length; k++)
            {
                string item = values[k];
                if (item != null)
                {
                    if (sb.Length > 0)
                        sb.Append(separator);

                    sb.Append(item);
                }
            }

            return sb.ToString();
        }

        public static string Join(string seprator, string str1, string str2, bool ignoreNull = false)
        {
            if (ignoreNull && (str1 == null || str2 == null))
                return str1 ?? str2;

            return str1 + seprator + str2;
        }

        public static string FilterSpecial(string str)
        {
            if (string.IsNullOrEmpty(str))
            { //如果字符串为空，直接返回。
                return str;
            }
            else
            {
                str = str.Replace("'", "‘");

                //str = str.Replace("<", "");
                //str = str.Replace(">", "");
                str = str.Replace("%", "％");

                //str = str.Replace("'delete", "");
                str = str.Replace("''", "‘");
                str = str.Replace("\"\"", "");
                str = str.Replace(",", "，");

                //str = str.Replace(".", "。");
                str = str.Replace(">=", "");
                str = str.Replace("=<", "");
                str = str.Replace(";", "：");
                str = str.Replace("||", "");
                str = str.Replace("[", "");
                str = str.Replace("]", "");

                //str = str.Replace("&", "");
                str = str.Replace("/", "");
                str = str.Replace("|", "");
                str = str.Replace("?", "？");

                //str = str.Replace(" ", "");
                return str;
            }
        }

        /// <summary>
        /// 在字符串数组中查找指定值是否存在
        /// </summary>
        /// <param name="arStr">数组</param>
        /// <param name="strFind">值</param>
        /// <returns></returns>
        public static bool SearchValueInArrayIsExist(string[] arStr, string strFind)
        {
            bool IsExist = false;
            for (int i = 0; i < arStr.Length; i++)
            {
                if (arStr[i] == strFind)
                {
                    IsExist = true;
                    break;
                }
            }
            return IsExist;
        }

        public static string ClearHtmlLite(this string htmlString)
        {
            if (string.IsNullOrEmpty(htmlString))
                return htmlString;

            //删除脚本
            htmlString = Regex.Replace(htmlString, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);

            //删除HTML
            htmlString = Regex.Replace(htmlString, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"<!--.*?-->", "", RegexOptions.IgnoreCase);

            htmlString = Regex.Replace(htmlString, @"<(?!br|/?p)[^>]*>", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"-->", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"<!--.*", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"(\s*<br.*?>\s*)+", "<br/>", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"<p[^>]*>\s*", "<p>", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"<p[^>]*>\s*</p>", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"^(\s*<br.*?>\s*)*", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"^(\s*)*", "", RegexOptions.IgnoreCase);
            htmlString.Replace("<", "");
            htmlString.Replace(">", "");
            htmlString.Replace("\r\n", "");

            return htmlString;
        }

        public static string ClearHtml(this string htmlString)
        {
            return ClearHtml(htmlString, string.Empty);
        }

        public static string ClearHtml(this string htmlString, string alternate)
        {
            if (string.IsNullOrEmpty(htmlString))
                return htmlString;

            //删除脚本
            htmlString = Regex.Replace(htmlString, @"<script[^>]*?>.*?</script>", alternate, RegexOptions.IgnoreCase);
            //删除HTML
            htmlString = Regex.Replace(htmlString, @"<(.[^>]*)>", alternate, RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"([\r\n])[\s]+", alternate, RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"-->", alternate, RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"<!--.*", alternate, RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&#(\d+);", alternate, RegexOptions.IgnoreCase);

            htmlString.Replace("<", alternate);
            htmlString.Replace(">", alternate);
            htmlString.Replace("\r\n", alternate);
            htmlString = HttpContext.Current != null ? HttpContext.Current.Server.HtmlEncode(htmlString).Trim() : htmlString;
            return htmlString;
        }

        public static string Clear(this string input)
        {
            return input.Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);
        }

        /// <summary>
        /// 检测一个字符串是否可以转化为日期。
        /// </summary>
        /// <param name="date">日期字符串。</param>
        /// <returns>是否可以转换。</returns>
        public static bool IsStringDate(string date)
        {
            DateTime dt;
            try
            {
                dt = DateTime.Parse(date);
            }
            catch (FormatException e)
            {
                //日期格式不正确时
                e.ToString();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取拆分符左边的字符串
        /// </summary>
        /// <param name="String">需要做处理的字符串</param>
        /// <param name="splitChar">拆分字符</param>
        /// <returns>按照拆分字符拆分好的左侧字符串</returns>
        public static string GetLeftSplitString(string String, char splitChar)
        {
            string result = null;
            string[] tempString = String.Split(splitChar);
            if (tempString.Length > 0)
            {
                result = tempString[0].ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取拆分符右边的字符串
        /// </summary>
        /// <param name="String">需要做处理的字符串</param>
        /// <param name="splitChar">拆分字符</param>
        /// <returns>按照拆分字符拆分号的右侧字符串</returns>
        public static string GetRightSplitString(string String, char splitChar)
        {
            string result = null;
            string[] tempString = String.Split(splitChar);
            if (tempString.Length > 0)
            {
                result = tempString[tempString.Length - 1].ToString();
            }
            return result;
        }

        /// <summary>
        ///  判断是否有非法字符
        /// </summary>
        /// <param name="strString"></param>
        /// <returns>返回TRUE表示有非法字符，返回FALSE表示没有非法字符。</returns>
        public static bool CheckBadStr(string strString)
        {
            bool outValue = false;
            if (strString != null && strString.Length > 0)
            {
                string[] bidStrlist = new string[21];
                bidStrlist[0] = "'";
                bidStrlist[1] = ";";
                bidStrlist[2] = ":";
                bidStrlist[3] = "%";
                bidStrlist[4] = "@";
                bidStrlist[5] = "&";
                bidStrlist[6] = "#";
                bidStrlist[7] = "\"";
                bidStrlist[8] = "net user";
                bidStrlist[9] = "exec";
                bidStrlist[10] = "net localgroup";
                bidStrlist[11] = "select";
                bidStrlist[12] = "asc";
                bidStrlist[13] = "char";
                bidStrlist[14] = "mid";
                bidStrlist[15] = "insert";
                bidStrlist[16] = "delete";
                bidStrlist[17] = "drop";
                bidStrlist[18] = "truncate";
                bidStrlist[19] = "xp_cmdshell";
                bidStrlist[19] = "order";

                string tempStr = strString.ToLower();
                for (int i = 0; i < bidStrlist.Length; i++)
                {
                    //if (tempStr.IndexOf(bidStrlist[i]) != -1)
                    if (tempStr == bidStrlist[i])
                    {
                        outValue = true;
                        break;
                    }
                }
            }
            return outValue;
        }

        /// <summary>
        /// 转换特殊字符为全角,防止SQL注入攻击
        /// </summary>
        /// <param name="str">要过滤的字符</param>
        /// <returns>返回全角转换后的字符</returns>
        public static string ChangeFullAngle(string str)
        {
            string tempStr = str;
            if (string.IsNullOrEmpty(tempStr))
            { //如果字符串为空，直接返回。
                return tempStr;
            }
            else
            {
                tempStr = str.ToLower();
                tempStr = str.Replace("'", "‘");
                tempStr = str.Replace("--", "－－");
                tempStr = str.Replace(";", "；");
                tempStr = str.Replace("exec", "ＥＸＥＣ");
                tempStr = str.Replace("execute", "ＥＸＥＣＵＴＥ");
                tempStr = str.Replace("declare", "ＤＥＣＬＡＲＥ");
                tempStr = str.Replace("update", "ＵＰＤＡＴＥ");
                tempStr = str.Replace("delete", "ＤＥＬＥＴＥ");
                tempStr = str.Replace("insert", "ＩＮＳＥＲＴ");
                tempStr = str.Replace("select", "ＳＥＬＥＣＴ");
                tempStr = str.Replace("<", "＜");
                tempStr = str.Replace(">", "＞");
                tempStr = str.Replace("%", "％");
                tempStr = str.Replace(@"\", "＼");
                tempStr = str.Replace(",", "，");
                tempStr = str.Replace(".", "．");
                tempStr = str.Replace("=", "＝＝");
                tempStr = str.Replace("||", "｜｜");
                tempStr = str.Replace("[", "【");
                tempStr = str.Replace("]", "】");
                tempStr = str.Replace("&", "＆");
                tempStr = str.Replace("/", "／");
                tempStr = str.Replace("|", "｜");
                tempStr = str.Replace("?", "？");
                tempStr = str.Replace("_", "＿");

                return str;
            }
        }

        /// <summary>
        /// 检测某字符是否为英文字母,数字,下划线
        /// </summary>
        /// <param name="str">要检查的字符串</param>
        /// <returns>True表示是英文字母,False表示不是英文字母</returns>
        public static bool CheckIsCharaterAndNumber(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            else
            {
                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^[0-9a-zA-Z_]+$");//正则表达式 验证英文、数字、下划线和点Regex(@"^[0-9a-zA-Z_]+$");--^[a-zA-Z0-9_\u4e00-\u9fa5]+$

                return reg.IsMatch(str);
            }
        }

        /// <summary>
        /// 检测某字符在某字符串中出现的次数
        /// </summary>
        /// <param name="checkStr">要检测的字符,比如"A"</param>
        /// <param name="str">要检测的字符串,比如"AAABBAACCC"</param>
        /// <returns>返回此字符出现的次数</returns>
        public static int CountCharacter(char checkStr, string str)
        {
            int count = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == checkStr)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 按字节数获取字符串的长度
        /// </summary>
        /// <param name="String">要计算的字符串</param>
        /// <returns>字符串的字节数</returns>
        public static int GetByteCount(string String)
        {
            int intCharCount = 0;
            for (int i = 0; i < String.Length; i++)
            {
                if (System.Text.UTF8Encoding.UTF8.GetByteCount(String.Substring(i, 1)) == 3)
                {
                    intCharCount = intCharCount + 2;
                }
                else
                {
                    intCharCount = intCharCount + 1;
                }
            }
            return intCharCount;
        }

        /// <summary>
        /// 截取指定字节数的字符串
        /// </summary>
        /// <param name="Str">原字符串</param>
        /// <param name="Num">要截取的字节数</param>
        /// <returns>截取后的字符串</returns>
        public static string CutStr(string Str, int Num)
        {
            if (Encoding.Default.GetBytes(Str).Length <= Num)
            {
                return Str;
            }
            else
            {
                int CutBytes = 0;
                for (int i = 0; i < Str.Length; i++)
                {
                    if (Convert.ToInt32(Str.ToCharArray().GetValue(i)) > 255)
                    {
                        CutBytes = CutBytes + 2;
                    }
                    else
                    {
                        CutBytes = CutBytes + 1;
                    }
                    if (CutBytes == Num)
                    {
                        return Str.Substring(0, i + 1);
                    }
                    if (CutBytes == Num + 1)
                    {
                        return Str.Substring(0, i);
                    }
                }
                return Str;
            }
        }

        /// <summary>
        /// 防止sql注入
        /// </summary>
        /// <param name="inputName"></param>
        /// <returns></returns>
        public static string SqlReplace(string inputName)
        {
            if (string.IsNullOrEmpty(inputName))
            {
                return string.Empty;
            }

            string[] strCheck = {
                "'",
                "%",
                "--",
                ";",
                "EXE",
                "EXECUTE",
                "DECLARE",
                "UPDATE",
                "DELETE",
                "INSERT",
                "SELECT",
                "_"
            };
            string[] strReplace = {
                "＇",
                "％",
                "－－",
                "；",
                "ＥＸＥＣ",
                "ＥＸＥＣＵＴＥ",
                "ＤＥＣＬＡＲＥ",
                "ＵＰＤＡＴＥ",
                "ＤＥＬＥＴＥ",
                "ＩＮＳＥＲＴ",
                "ＳＥＬＥＣＴ",
                "＿"
            };
            for (int i = 0; i < strCheck.Length; i++)
            {
                inputName = Regex.Replace(inputName, strCheck[i], strReplace[i], RegexOptions.IgnoreCase);
            }
            return inputName;
        }

        /// <summary>
        /// 对入库字符进行编码和转换
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncodeStr(string str)
        {
            str = "" + str;//防止str为NULL时出错
            str = str.Replace("&nbsp", "&amp;nbsp");
            str = str.Replace(" ", "&nbsp;");
            str = str.Replace("'", "’");
            str = str.Replace("\"", "&quot;");
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt;");
            str = str.Replace("\n", "<br/>");
            return str;
        }

        /// <summary>
        /// 对出库字符进入显示时的转换
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DecodeStr(string str)
        {
            str = "" + str;//防止str为NULL时出错
            str = str.Replace("&amp;nbsp", "&nbsp");
            str = str.Replace("&nbsp;", " ");//用于文本框中输入的空格转化成html标记
            str = str.Replace("’", "'");
            str = str.Replace("&quot;", "\"");
            str = str.Replace("&lt;", "<");
            str = str.Replace("&gt;", ">");
            str = str.Replace("<br/>", "\n");

            return str;
        }

        /// <summary>
        /// 是否数字字符串
        /// </summary>
        /// <param name="value">输入字符串</param>
        /// <returns></returns>
        public static bool IsNumber(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            Regex regex = new Regex("^[0-9]+$");//正整数
            Match m = regex.Match(value);
            return m.Success;
        }

        /// <summary>
        /// 是否数字字符串 可带正负号
        /// </summary>
        /// <param name="value">输入字符串</param>
        /// <returns></returns>
        public static bool IsNumberSign(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            Regex regex = new Regex("^[+-]?[0-9]+$");//正负整数
            Match m = regex.Match(value);
            return m.Success;
        }

        /// <summary>
        /// 是否是浮点数
        /// </summary>
        /// <param name="value">输入字符串</param>
        /// <returns></returns>
        public static bool IsDecimal(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            Regex regex = new Regex("^[0-9]+[.]?[0-9]+$");//小数
            Match m = regex.Match(value);
            return m.Success;
        }

        /// <summary>
        /// 是否是浮点数 可带正负号
        /// </summary>
        /// <param name="value">输入字符串</param>
        /// <returns></returns>
        public static bool IsDecimalSign(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            Regex regex = new Regex("^[+-]?[0-9]+[.]?[0-9]+$"); //等价于^[+-]?\d+[.]?\d+$
            Match m = regex.Match(value);
            return m.Success;
        }

        /// <summary>
        /// 检测是否有中文字符
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsChinese(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            return RegularExpressions.IsChinese.IsMatch(value);
        }

        /// <summary>
        /// 检查是否有非法字符
        /// </summary>
        /// 编码作者：LYT
        /// <param name="value">输入字符串</param>
        /// <returns></returns>
        public static bool ValidatorStr(string value)
        {
            bool isPass = false;
            if (value.Length > 0)
            {
                Regex regex = new Regex(@"^[^<>'=&*,]+$");
                Match m = regex.Match(value);
                isPass = m.Success;
            }
            else
            {
                isPass = true;
            }

            return isPass;
        }

        /// <summary>
        /// 是否是邮件地址
        /// </summary>
        /// <param name="value">输入字符串</param>
        /// <returns></returns>
        public static bool IsEmail(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            return RegularExpressions.IsEmail.IsMatch(value);
        }

        /// <summary>
        /// 是否是国内电话号码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsTel(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            return RegularExpressions.IsTel.IsMatch(value);
        }

        /// <summary>
        /// 是否是QQ
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsQQ(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            return RegularExpressions.IsQQ.IsMatch(value);
        }

        /// <summary>
        /// 是否是国内身份证号
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsIDCard(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            return RegularExpressions.IsIDCard.IsMatch(value);
        }

        /// <summary>
        /// 账号是否合法，字母开头，数字、26个英文字母或者下划线组成6-16位的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsUserName(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            Regex regex = new Regex(@"^[a-zA-Z]\w{5,15}$");//匹配由字母开头，数字、26个英文字母或者下划线组成6-16位的字符串
            Match m = regex.Match(value);
            return m.Success;
        }

        /// <summary>
        /// 是否是由26个英文字母组成的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEnglish(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            return RegularExpressions.IsAlpha.IsMatch(value);
        }

        /// <summary>
        /// 是否有空行
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsTrimRow(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            Regex regex = new Regex(@"\n[\s| ]*\r");//空行
            Match m = regex.Match(value);
            return m.Success;
        }

        /// <summary>
        /// 是否是国内手机
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsMobile(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            return RegularExpressions.IsMobile.IsMatch(value);
        }

        /// <summary>
        /// 检查是否是日期
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDate(string value)
        {
            return DateTime.TryParse(value, out DateTime _TempDate);
        }

        /// <summary>
        /// 检查字符串最大长度，返回指定长度的串
        /// </summary>
        /// <param name="sqlInput">输入字符串</param>
        /// <param name="maxLength">最大长度</param>
        /// <returns></returns>
        public static string SqlText(string sqlInput, int maxLength)
        {
            if (!string.IsNullOrEmpty(sqlInput))
            {
                sqlInput = sqlInput.Trim();
                if (sqlInput.Length > maxLength)//按最大长度截取字符串
                    sqlInput = sqlInput.Substring(0, maxLength);
            }
            return sqlInput;
        }

        public static string FilterHtm(string htmlStr)
        {
            int flag;
            if (htmlStr.IndexOf(">") < htmlStr.IndexOf("<") || htmlStr.IndexOf("<") == 0)
                flag = 0;
            else
                flag = 1;
            string filterStr = "";
            foreach (char str in htmlStr)
            {
                if (str.ToString() == "<")
                    flag = 0;
                if (flag == 1)
                    filterStr += str.ToString();
                if (str.ToString() == ">")
                    flag = 1;
            }
            return filterStr;
        }

        /// <summary>
        /// 去掉最后一个逗号
        /// </summary>
        /// <param name="String">要做处理的字符串</param>
        /// <returns>去掉最后一个逗号的字符串</returns>
        public static string DelLastComma(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            if (str.IndexOf(",") == -1)
            {
                return str;
            }
            return str.Substring(0, str.LastIndexOf(","));
        }

        /// <summary>
        /// 根据0,1,2等数字转换为相应的字母ABC
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string GetChangedCharacter(int num)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();

            byte[] byteArray = new byte[] { (byte)(num + 65) };
            string strCharacter = asciiEncoding.GetString(byteArray);

            return strCharacter;
        }

        public static string GetPopJScript(string message)
        {
            return string.Format("<script type='text/javascript'>alert('{0}');</script>", message);
        }

        public static string GetRunJscript(string message)
        {
            return string.Format("<script type='text/javascript'>{0}</script>", message);
        }

        public static string DownLoadUrlFile(string urlInfo)
        {
            return string.Format("<script type='text/javascript'>$('#DownLoad').></a></script>", urlInfo);
        }

        public static string Request(string url, string paras)
        {
            string[] paraString = url.Substring(url.IndexOf("?") + 1).Split('&');
            for (int i = 0; i < paraString.Length; i++)
            {
                if (paraString[i].Substring(0, paraString[i].IndexOf("=")) == paras)
                {
                    return paraString[i].Substring(paraString[i].IndexOf("=") + 1);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 删除不可见字符
        /// </summary>
        /// <param name="String">需要做处理的字符串</param>
        /// <returns>处理后的字符串</returns>
        public static string DeleteUnVisibleChar(string String)
        {
            System.Text.StringBuilder sBuilder = new System.Text.StringBuilder(131);
            for (int i = 0; i < String.Length; i++)
            {
                int Unicode = String[i];
                if (Unicode >= 16)
                {
                    sBuilder.Append(String[i].ToString());
                }
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 获取字符串数组合并后的字符串
        /// </summary>
        /// <param name="stringArray">字符串数组</param>
        /// <returns>合并后的字符串</returns>
        public static string GetArrayString(string[] stringArray)
        {
            string totalString = null;
            for (int i = 0; i < stringArray.Length; i++)
            {
                totalString = totalString + stringArray[i];
            }
            return totalString;
        }

        /// <summary>
        /// 获取某一字符串在一个字符串中出现的次数
        /// </summary>
        /// <param name="sourceString">原字符串</param>
        /// <param name="findString">要比较的字符串</param>
        /// <returns>在原字符串中出现要比较字符串的次数</returns>
        public static int GetStringCount(string sourceString, string findString)
        {
            int count = 0;
            int findStringLength = findString.Length;
            string subString = sourceString;

            while (subString.IndexOf(findString) >= 0)
            {
                subString = subString.Substring(subString.IndexOf(findString) + findStringLength);
                count++;
            }
            return count;
        }

        /// <summary>
        /// 获取某一字符串在字符串数组中出现的次数
        /// </summary>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="findString">要比较的字符串</param>
        /// <returns>该字符串在字符串数组中出现的次数</returns>
        public static int GetStringCount(string[] stringArray, string findString)
        {
            string totalString = GetArrayString(stringArray);
            return (GetStringCount(totalString, findString));
        }

        /// <summary>
        /// 按字节数获取字符串的位置
        /// </summary>
        /// <param name="intNum">字符串的位置</param>
        /// <param name="String">要计算的字符串</param>
        /// <returns>字节的位置</returns>
        public static int GetByteIndex(int intNum, string String)
        {
            int intReIns = 0;
            if (String.Trim() == "")
            {
                return intNum;
            }
            for (int i = 0; i < String.Length; i++)
            {
                if (System.Text.UTF8Encoding.UTF8.GetByteCount(String.Substring(i, 1)) == 3)
                {
                    intReIns = intReIns + 2;
                }
                else
                {
                    intReIns = intReIns + 1;
                }
                if (intReIns >= intNum)
                {
                    intReIns = i + 1;
                    break;
                }
            }
            return intReIns;
        }

        /// <summary>
        /// 从字符串中的尾部删除指定的字符串
        /// </summary>
        /// <param name="sourceString">原字符串</param>
        /// <param name="removedString">想要删除的字符串</param>
        /// <returns>删除指定字符串后的字符串</returns>
        public static string RemoveString(string sourceString, string removedString)
        {
            try
            {
                if (sourceString.IndexOf(removedString) < 0)
                    throw new Exception("原字符串中不包含移除字符串！");
                string result = sourceString;
                int lengthOfSourceString = sourceString.Length;
                int lengthOfRemovedString = removedString.Length;
                int startIndex = lengthOfSourceString - lengthOfRemovedString;
                string tempSubString = sourceString.Substring(startIndex);
                if (tempSubString.ToUpper() == removedString.ToUpper())
                {
                    result = sourceString.Remove(startIndex, lengthOfRemovedString);
                }
                return result;
            }
            catch (Exception ex)
            {
                string strErrorMessage = ex.Message;
                return sourceString;
            }
        }

        /// <summary>
        /// 截取从指定字符串开始到原字符串结尾的所有字符
        /// </summary>
        /// <param name="sourceString">原字符串</param>
        /// <param name="startString">指定开始查找的字符串</param>
        /// <returns>处理后的字符串</returns>
        public static string GetSubString(string sourceString, string startString)
        {
            try
            {
                int index = sourceString.ToUpper().IndexOf(startString);
                if (index > 0)
                {
                    return sourceString.Substring(index);
                }
                return sourceString;
            }
            catch (Exception ex)
            {
                string strErrorMessage = ex.Message;
                return "";
            }
        }

        public static string CamelFriendly(this string camel)
        {
            if (String.IsNullOrWhiteSpace(camel))
                return "";

            var sb = new StringBuilder(camel);

            for (int i = camel.Length - 1; i > 0; i--)
            {
                var current = sb[i];
                if ('A' <= current && current <= 'Z')
                {
                    sb.Insert(i, ' ');
                }
            }

            return sb.ToString();
        }

        public static string Ellipsize(this string text, int characterCount)
        {
            return text.Ellipsize(characterCount, "&#160;&#8230;");
        }

        public static string Ellipsize(this string text, int characterCount, string ellipsis, bool wordBoundary = false)
        {
            if (String.IsNullOrWhiteSpace(text))
                return "";

            if (characterCount < 0 || text.Length <= characterCount)
                return text;

            // search beginning of word
            var backup = characterCount;
            while (characterCount > 0 && text[characterCount - 1].IsLetter())
            {
                characterCount--;
            }

            // search previous word
            while (characterCount > 0 && text[characterCount - 1].IsSpace())
            {
                characterCount--;
            }

            // if it was the last word, recover it, unless boundary is requested
            if (characterCount == 0 && !wordBoundary)
            {
                characterCount = backup;
            }

            var trimmed = text.Substring(0, characterCount);
            return trimmed + ellipsis;
        }

        public static string HtmlClassify(this string text)
        {
            if (String.IsNullOrWhiteSpace(text))
                return "";

            var friendlier = text.CamelFriendly();

            var result = new char[friendlier.Length];

            var cursor = 0;
            var previousIsNotLetter = false;
            for (var i = 0; i < friendlier.Length; i++)
            {
                char current = friendlier[i];
                if (IsLetter(current) || (Char.IsDigit(current) && cursor > 0))
                {
                    if (previousIsNotLetter && i != 0 && cursor > 0)
                    {
                        result[cursor++] = '-';
                    }

                    result[cursor++] = Char.ToLowerInvariant(current);
                    previousIsNotLetter = false;
                }
                else
                {
                    previousIsNotLetter = true;
                }
            }

            return new string(result, 0, cursor);
        }

        public static string RemoveTags(this string html, bool htmlDecode = false)
        {
            if (string.IsNullOrEmpty(html))
            {
                return String.Empty;
            }

            var result = new char[html.Length];

            var cursor = 0;
            var inside = false;
            for (var i = 0; i < html.Length; i++)
            {
                char current = html[i];

                switch (current)
                {
                    case '<':
                        inside = true;
                        continue;
                    case '>':
                        inside = false;
                        continue;
                }

                if (!inside)
                {
                    result[cursor++] = current;
                }
            }

            var stringResult = new string(result, 0, cursor);

            if (htmlDecode)
            {
                stringResult = HttpUtility.HtmlDecode(stringResult);
            }

            return stringResult;
        }

        // not accounting for only \r (e.g. Apple OS 9 carriage return only new lines)
        public static string ReplaceNewLinesWith(this string text, string replacement)
        {
            return String.IsNullOrWhiteSpace(text)
                       ? String.Empty
                       : text
                             .Replace("\r\n", "\r\r")
                             .Replace("\n", String.Format(replacement, "\r\n"))
                             .Replace("\r\r", String.Format(replacement, "\r\n"));
        }

        public static string ToHexString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        public static byte[] ToHexByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length).
                Where(x => 0 == x % 2).
                Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).
                ToArray();
        }

        private static readonly char[] ValidSegmentChars = "/?#[]@\"^{}|`<>\t\r\n\f ".ToCharArray();

        public static bool IsValidUrlSegment(this string segment)
        {
            // valid isegment from rfc3987 - http://tools.ietf.org/html/rfc3987#page-8
            // the relevant bits:
            // isegment    = *ipchar
            // ipchar      = iunreserved / pct-encoded / sub-delims / ":" / "@"
            // iunreserved = ALPHA / DIGIT / "-" / "." / "_" / "~" / ucschar
            // pct-encoded = "%" HEXDIG HEXDIG
            // sub-delims  = "!" / "$" / "&" / "'" / "(" / ")" / "*" / "+" / "," / ";" / "="
            // ucschar     = %xA0-D7FF / %xF900-FDCF / %xFDF0-FFEF / %x10000-1FFFD / %x20000-2FFFD / %x30000-3FFFD / %x40000-4FFFD / %x50000-5FFFD / %x60000-6FFFD / %x70000-7FFFD / %x80000-8FFFD / %x90000-9FFFD / %xA0000-AFFFD / %xB0000-BFFFD / %xC0000-CFFFD / %xD0000-DFFFD / %xE1000-EFFFD
            //
            // rough blacklist regex == m/^[^/?#[]@"^{}|\s`<>]+$/ (leaving off % to keep the regex simple)

            return !segment.Any(ValidSegmentChars);
        }

        /// <summary>
        /// Generates a valid technical name.
        /// </summary>
        /// <remarks>
        /// Uses a white list set of chars.
        /// </remarks>
        public static string ToSafeName(this string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return String.Empty;

            name = RemoveDiacritics(name);
            name = name.Strip(c =>
                c != '_'
            && c != '-'
            && !c.IsLetter()
            && !Char.IsDigit(c)
            );

            name = name.Trim();

            // don't allow non A-Z chars as first letter, as they are not allowed in prefixes
            while (name.Length > 0 && !IsLetter(name[0]))
            {
                name = name.Substring(1);
            }

            if (name.Length > 128)
                name = name.Substring(0, 128);

            return name;
        }

        /// <summary>
        /// Whether the char is a letter between A and Z or not
        /// </summary>
        public static bool IsLetter(this char c)
        {
            return ('A' <= c && c <= 'Z') || ('a' <= c && c <= 'z');
        }

        public static bool IsSpace(this char c)
        {
            return (c == '\r' || c == '\n' || c == '\t' || c == '\f' || c == ' ');
        }

        public static string RemoveDiacritics(this string name)
        {
            string stFormD = name.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char t in stFormD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(t);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(t);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        public static string Strip(this string subject, params char[] stripped)
        {
            if (stripped == null || stripped.Length == 0 || string.IsNullOrEmpty(subject))
            {
                return subject;
            }

            var result = new char[subject.Length];

            var cursor = 0;
            for (var i = 0; i < subject.Length; i++)
            {
                char current = subject[i];
                if (Array.IndexOf(stripped, current) < 0)
                {
                    result[cursor++] = current;
                }
            }

            return new string(result, 0, cursor);
        }

        public static string Strip(this string subject, Func<char, bool> predicate)
        {
            var result = new char[subject.Length];

            var cursor = 0;
            for (var i = 0; i < subject.Length; i++)
            {
                char current = subject[i];
                if (!predicate(current))
                {
                    result[cursor++] = current;
                }
            }

            return new string(result, 0, cursor);
        }

        public static bool Any(this string subject, params char[] chars)
        {
            if (string.IsNullOrEmpty(subject) || chars == null || chars.Length == 0)
            {
                return false;
            }

            Array.Sort(chars);

            for (var i = 0; i < subject.Length; i++)
            {
                char current = subject[i];
                if (Array.BinarySearch(chars, current) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool All(this string subject, params char[] chars)
        {
            if (string.IsNullOrEmpty(subject))
            {
                return true;
            }

            if (chars == null || chars.Length == 0)
            {
                return false;
            }

            Array.Sort(chars);

            for (var i = 0; i < subject.Length; i++)
            {
                char current = subject[i];
                if (Array.BinarySearch(chars, current) < 0)
                {
                    return false;
                }
            }

            return true;
        }

        public static string Translate(this string subject, char[] from, char[] to)
        {
            if (string.IsNullOrEmpty(subject))
            {
                return subject;
            }

            if (from == null || to == null)
            {
                throw new ArgumentNullException();
            }

            if (from.Length != to.Length)
            {
                throw new ArgumentNullException("from", "Parameters must have the same length");
            }

            var map = new Dictionary<char, char>(from.Length);
            for (var i = 0; i < from.Length; i++)
            {
                map[from[i]] = to[i];
            }

            var result = new char[subject.Length];

            for (var i = 0; i < subject.Length; i++)
            {
                var current = subject[i];
                if (map.ContainsKey(current))
                {
                    result[i] = map[current];
                }
                else
                {
                    result[i] = current;
                }
            }

            return new string(result);
        }

        public static string More(this string input, int displayLenth, string omit = "...")
        {
            if (input.Length <= displayLenth)
                return input;
            return input.Substring(0, displayLenth) + omit;
        }

        public static string InnerText(this string input)
        {
            return Regex.Replace(input, "<[^>]+>", string.Empty);
        }

        public static string Html(this string input)
        {
            return input.Replace(" ", "&nbsp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace('\n'.ToString(), "<br>");
        }

        public static string FileName(this string input)
        {
            if (Path.HasExtension(input))
                return Path.GetFileName(input);
            return string.Empty;
        }

        public static bool IsPrivateNetwork(this string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return false;

            if (String.Equals(ip, "::1") || String.Equals(ip, "127.0.0.1"))
                return true;

            // 10.0.0.0 – 10.255.255.255 (Class A)
            if (ip.StartsWith("10."))
                return true;

            // 172.16.0.0 – 172.31.255.255 (Class B)
            if (ip.StartsWith("172."))
            {
                for (var range = 16; range < 32; range++)
                {
                    if (ip.StartsWith("172." + range + "."))
                        return true;
                }
            }

            // 192.168.0.0 – 192.168.255.255 (Class C)
            return ip.StartsWith("192.168.");
        }

        public static string GetNewToken()
        {
            return GetRandomString(40);
        }

        public static string GetRandomString(int length, string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length", "length cannot be less than zero.");

            if (string.IsNullOrEmpty(allowedChars))
                throw new ArgumentException("allowedChars may not be empty.");

            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(allowedChars).ToArray();
            if (byteSize < allowedCharSet.Length)
                throw new ArgumentException(String.Format("allowedChars may contain no more than {0} characters.", byteSize));

            using (var rng = new RNGCryptoServiceProvider())
            {
                var result = new StringBuilder();
                var buf = new byte[128];

                while (result.Length < length)
                {
                    rng.GetBytes(buf);
                    for (var i = 0; i < buf.Length && result.Length < length; ++i)
                    {
                        var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
                        if (outOfRangeStart <= buf[i])
                            continue;
                        result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                    }
                }

                return result.ToString();
            }
        }

        public static string UpFirst(this string str)
        {
            if (str.Length > 0)
            {
                str = str.Substring(0, 1).ToUpper() + str.Substring(1);
            }
            return str;
        }

        public static string Abbr(this string str)
        {
            int pos = str.IndexOf("_");
            if (pos > -1)
            {
                str = str.Substring(pos + 1);
            }
            return UpFirst(str);
        }

        /// <summary>
        /// 压缩指定的字符串。
        /// </summary>
        /// <param name="str">要压缩的字符串。</param>
        /// <returns>压缩后的byte数组。</returns>
        public static byte[] Compress(this string str)
        {
            return str.Compress(Encoding.Unicode);
        }

        /// <summary>
        /// 压缩指定的字符串。
        /// </summary>
        /// <param name="str">要压缩的字符串。</param>
        /// <param name="encoding">字符串的编码。</param>
        /// <returns>压缩后的byte数组。</returns>
        public static byte[] Compress(this string str, Encoding encoding)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException("str");
            }
            return (encoding ?? Encoding.Unicode).GetBytes(str).Compress();
        }
    }
}