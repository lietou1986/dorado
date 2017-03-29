using System;
using System.ComponentModel;
using System.Reflection;

namespace Dorado.Extensions
{
    public static class EmunExtensions
    {
        /// <summary>
        /// Gets the <see cref="EnumFriendlyNameAttribute" /> of an <see cref="Enum" />
        /// type value.
        /// </summary>
        /// <param name="value">The <see cref="Enum" /> type value.</param>
        /// <returns>A string containing the text of the
        /// <see cref="EnumFriendlyNameAttribute"/>.</returns>
        public static string GetFriendlyName(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            string friendlyName = value.ToString();
            FieldInfo fieldInfo = value.GetType().GetField(friendlyName);

            var attributes = (EnumFriendlyNameAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumFriendlyNameAttribute), false);

            if (attributes.Length == 1)
            {
                friendlyName = attributes[0].FriendlyName;
            }
            return friendlyName;
        }

        public static string GetLable(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            string description = value.ToString();
            FieldInfo fi = value.GetType().GetField(description);
            if (fi != null)
            {
                EnumLableAttribute[] attributes = (EnumLableAttribute[])fi.GetCustomAttributes(typeof(EnumLableAttribute), false);
                description = attributes.Length > 0 ? attributes[0].Lable : string.Empty;
            }
            else
            {
                description = string.Empty;
            }
            return description;
        }

        public static string GetDescription(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            string description = value.ToString();
            FieldInfo fi = value.GetType().GetField(description);
            if (fi != null)
            {
                EnumDescriptionAttribute[] attributes = (EnumDescriptionAttribute[])fi.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
                description = attributes.Length > 0 ? attributes[0].Description : string.Empty;
            }
            else
            {
                description = string.Empty;
            }
            return description;
        }

        public static string GetDefaultDescription(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            string description = value.ToString();
            FieldInfo fi = value.GetType().GetField(description);
            if (fi != null)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                description = attributes.Length > 0 ? attributes[0].Description : string.Empty;
            }
            else
            {
                description = string.Empty;
            }
            return description;
        }

        public static string GetExplain(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            string enumvaluestr = value.ToString();
            FieldInfo fi = value.GetType().GetField(enumvaluestr);
            if (fi != null)
            {
                EnumeExplainAttribute[] attributes = (EnumeExplainAttribute[])fi.GetCustomAttributes(typeof(EnumeExplainAttribute), false);
                if (attributes.Length > 0)
                {
                    return attributes[0].Explain;
                }
            }
            return string.Empty;
        }

        public static bool GetDisplay(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            string description = value.ToString();
            FieldInfo fi = value.GetType().GetField(description);
            if (fi != null)
            {
                EnumeDisplayAttribute[] attributes = (EnumeDisplayAttribute[])fi.GetCustomAttributes(typeof(EnumeDisplayAttribute), false);
                return attributes.Length > 0 && attributes[0].Display;
            }
            return false;
        }

        public static string GetIndexFieldName(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            string description = value.ToString();
            FieldInfo fi = value.GetType().GetField(description);
            if (fi != null)
            {
                EnumeIndexFieldNameAttribute[] attributes = (EnumeIndexFieldNameAttribute[])fi.GetCustomAttributes(typeof(EnumeIndexFieldNameAttribute), false);
                if (attributes.Length > 0)
                {
                    return attributes[0].IndexFieldName;
                }
            }
            throw new ArgumentException("参数无效。有可能没有定义该枚举类型值。", "value");
        }

        public static string GetDataFieldName(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            string description = value.ToString();
            FieldInfo fi = value.GetType().GetField(description);
            if (fi != null)
            {
                EnumeDataFieldNameAttribute[] attributes = (EnumeDataFieldNameAttribute[])fi.GetCustomAttributes(typeof(EnumeDataFieldNameAttribute), false);
                if (attributes.Length > 0)
                {
                    return attributes[0].DataFieldName;
                }
            }
            throw new ArgumentException("参数无效。有可能没有定义该枚举类型值。", "value");
        }

        public static int Key(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        public static byte Flag(this Enum value)
        {
            return Convert.ToByte(value);
        }

        /// <summary>
        /// Checks if the specified enum flag is set on a flagged enumeration type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool IsSet<T>(this T value, T flags) where T : struct
        {
            Type type = typeof(T);

            // only works with enums
            if (!type.IsEnum)
                throw Error.Argument("T", "The type parameter T must be an enum type.");

            // handle each underlying type
            Type numberType = Enum.GetUnderlyingType(type);

            if (numberType == typeof(int))
            {
                return BoxUnbox<int>(value, flags, (a, b) => (a & b) == b);
            }
            else if (numberType == typeof(sbyte))
            {
                return BoxUnbox<sbyte>(value, flags, (a, b) => (a & b) == b);
            }
            else if (numberType == typeof(byte))
            {
                return BoxUnbox<byte>(value, flags, (a, b) => (a & b) == b);
            }
            else if (numberType == typeof(short))
            {
                return BoxUnbox<short>(value, flags, (a, b) => (a & b) == b);
            }
            else if (numberType == typeof(ushort))
            {
                return BoxUnbox<ushort>(value, flags, (a, b) => (a & b) == b);
            }
            else if (numberType == typeof(uint))
            {
                return BoxUnbox<uint>(value, flags, (a, b) => (a & b) == b);
            }
            else if (numberType == typeof(long))
            {
                return BoxUnbox<long>(value, flags, (a, b) => (a & b) == b);
            }
            else if (numberType == typeof(ulong))
            {
                return BoxUnbox<ulong>(value, flags, (a, b) => (a & b) == b);
            }
            else if (numberType == typeof(char))
            {
                return BoxUnbox<char>(value, flags, (a, b) => (a & b) == b);
            }
            else
            {
                throw new ArgumentException("Unknown enum underlying type " +
                    numberType.Name + ".");
            }
        }

        /// <SUMMARY>
        /// Helper function for handling the value types. Boxes the params to
        /// object so that the cast can be called on them.
        /// </SUMMARY>
        private static bool BoxUnbox<T>(object value, object flags, Func<T, T, bool> op)
        {
            return op((T)value, (T)flags);
        }
    }

    /// <summary>
    /// Provides a friendly display name for an enumerated type value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class EnumFriendlyNameAttribute : Attribute
    {
        public EnumFriendlyNameAttribute(string friendlyName)
        {
            FriendlyName = friendlyName;
        }

        public string FriendlyName { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false)]
    public class EnumDescriptionAttribute : Attribute
    {
        private readonly string _description;

        public string Description
        {
            get
            {
                return this._description;
            }
        }

        public EnumDescriptionAttribute(string description)
        {
            this._description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class EnumeDataFieldNameAttribute : Attribute
    {
        private readonly string _explain;

        public string DataFieldName
        {
            get
            {
                return this._explain;
            }
        }

        public EnumeDataFieldNameAttribute(string explain)
        {
            this._explain = explain;
        }
    }

    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class EnumeDisplayAttribute : Attribute
    {
        private readonly bool _display;

        public bool Display
        {
            get
            {
                return this._display;
            }
        }

        public EnumeDisplayAttribute(bool display)
        {
            this._display = display;
        }
    }

    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class EnumeIndexFieldNameAttribute : Attribute
    {
        private readonly string _explain;

        public string IndexFieldName
        {
            get
            {
                return this._explain;
            }
        }

        public EnumeIndexFieldNameAttribute(string explain)
        {
            this._explain = explain;
        }
    }

    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false)]
    public class EnumLableAttribute : Attribute
    {
        private readonly string _lable;

        public string Lable
        {
            get
            {
                return this._lable;
            }
        }

        public EnumLableAttribute(string lable)
        {
            this._lable = lable;
        }
    }

    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class EnumeExplainAttribute : Attribute
    {
        private readonly string _explain;

        public string Explain
        {
            get
            {
                return this._explain;
            }
        }

        public EnumeExplainAttribute(string explain)
        {
            this._explain = explain;
        }
    }
}