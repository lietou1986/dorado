using Microsoft.CSharp;
using System;
using System.Collections;
using System.ComponentModel;

namespace Dorado.DataExpress.Utility
{
    public class Converter
    {
        private static readonly CSharpCodeProvider Compiler = new CSharpCodeProvider();
        private static readonly Hashtable Converters = Hashtable.Synchronized(new Hashtable(64));

        public static object Convert(Type target, object source)
        {
            if (target.IsEnum)
            {
                if (source is int)
                {
                    return source;
                }
                if (source is string)
                {
                    return Enum.Parse(target, source.ToString());
                }
            }
            TypeConverter cv = null;
            if (!Converter.Converters.ContainsKey(target))
            {
                cv = Converter.Compiler.GetConverter(target);
                if (cv == null || !cv.CanConvertFrom(source.GetType()))
                {
                    return null;
                }
                Converter.Converters.Add(target, cv);
            }
            else
            {
                cv = (TypeConverter)Converter.Converters[target];
            }
            return cv.ConvertFrom(source);
        }

        public static object Convert(Type target, string source)
        {
            Type type = target;
            if (target.IsGenericType)
            {
                type = Nullable.GetUnderlyingType(target);
            }
            string typeName = type.ToString();
            string key;
            switch (key = typeName)
            {
                case "System.String":
                    {
                        return source;
                    }
                case "System.Double":
                    {
                        return double.Parse(source);
                    }
                case "System.Int":
                case "System.Int32":
                    {
                        return int.Parse(source);
                    }
                case "System.Int16":
                    {
                        return short.Parse(source);
                    }
                case "System.Int64":
                    {
                        return long.Parse(source);
                    }
                case "System.DateTime":
                    {
                        return DateTime.Parse(source);
                    }
                case "System.Decimal":
                    {
                        return decimal.Parse(source);
                    }
                case "System.UInt16":
                    {
                        return ushort.Parse(source);
                    }
                case "System.UInt":
                case "System.UInt32":
                    {
                        return uint.Parse(source);
                    }
                case "System.UInt64":
                    {
                        return ulong.Parse(source);
                    }
                case "System.Boolean":
                    {
                        return bool.Parse(source);
                    }
                case "System.Byte":
                    {
                        return byte.Parse(source);
                    }
                case "System.Char":
                    {
                        return char.Parse(source);
                    }
                case "System.Guid":
                    {
                        return new Guid(source);
                    }
            }
            TypeConverter cv = null;
            if (!Converter.Converters.ContainsKey(target))
            {
                cv = Converter.Compiler.GetConverter(target);
                if (cv == null || !cv.CanConvertFrom(typeof(string)))
                {
                    return null;
                }
                Converter.Converters.Add(target, cv);
            }
            else
            {
                cv = (TypeConverter)Converter.Converters[target];
            }
            return cv.ConvertFrom(source);
        }
    }
}