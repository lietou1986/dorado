using Dorado.DataExpress.Ldo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;

namespace Dorado.DataExpress.Utility
{
    public static class TypeExtensions
    {
        private static readonly Dictionary<Type, Func<object>> TypeCache = new Dictionary<Type, Func<object>>();
        private static readonly object SycnRoot = new object();

        public static TableAttribute GetTable(this Type type)
        {
            object[] attrs = type.GetCustomAttributes(true);
            IEnumerable<TableAttribute> items =
                from att in attrs
                where att is TableAttribute
                select (TableAttribute)att;
            if (items.Count<TableAttribute>() > 0)
            {
                return items.First<TableAttribute>();
            }
            IEnumerable<DataContractAttribute> items2 =
                from att in attrs
                where att is DataContractAttribute
                select (DataContractAttribute)att;
            if (items2.Count<DataContractAttribute>() > 0)
            {
                DataContractAttribute item = items2.First<DataContractAttribute>();
                return new TableAttribute(item.Name ?? type.Name);
            }
            return new TableAttribute
            {
                TableName = type.Name
            };
        }

        public static Dictionary<string, DataProperty> GetDataFields(this Type type)
        {
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            Dictionary<string, DataProperty> propertiesDic = new Dictionary<string, DataProperty>(properties.Length, StringComparer.OrdinalIgnoreCase);
            PropertyInfo[] array = properties;
            for (int i = 0; i < array.Length; i++)
            {
                PropertyInfo propItem = array[i];
                if (propItem.CanRead && propItem.CanWrite)
                {
                    FieldAttribute field = propItem.GetField();
                    if (field != null)
                    {
                        propertiesDic.Add(field.FieldName, new DataProperty
                        {
                            Field = field,
                            Property = propItem
                        });
                    }
                }
            }
            return propertiesDic;
        }

        internal static FieldAttribute GetField(this PropertyInfo prop)
        {
            object[] attrs = prop.GetCustomAttributes(true);
            if (attrs.Length == 0)
            {
                return new FieldAttribute
                {
                    FieldName = prop.Name
                };
            }
            int attrCount = (
                from p in attrs
                where p is IgnorAttribute
                select p).Count<object>();
            if (attrCount > 0)
            {
                return null;
            }
            bool isPrimaryKeyMark = (
                from pr in attrs
                where pr is PrimaryKeyAttribute
                select pr).Count<object>() != 0;
            IEnumerable<FieldAttribute> items =
                from a in attrs
                where a is FieldAttribute
                select (FieldAttribute)a;
            if (items.Count<FieldAttribute>() > 0)
            {
                FieldAttribute attr2 = items.First<FieldAttribute>();
                if (isPrimaryKeyMark)
                {
                    attr2.IsPrimaryKey = true;
                }
                if (prop.PropertyType.IsGenericType)
                {
                    attr2.AllowNull = true;
                }
                return attr2;
            }
            IEnumerable<DataMemberAttribute> items2 =
                from attr in attrs
                where attr is DataMemberAttribute
                select (DataMemberAttribute)attr;
            if (items2.Count<DataMemberAttribute>() > 0)
            {
                DataMemberAttribute item = items2.First<DataMemberAttribute>();
                return new FieldAttribute
                {
                    FieldIndex = item.Order,
                    FieldName = item.Name ?? prop.Name,
                    AllowNull = item.IsRequired,
                    IsPrimaryKey = isPrimaryKeyMark
                };
            }
            return new FieldAttribute
            {
                FieldName = prop.Name
            };
        }

        public static void SetValue(this PropertyInfo prop, object obj, object value)
        {
            prop.SetValue(obj, value, null);
        }

        public static void SetNullValue(this PropertyInfo prop, object obj)
        {
            prop.SetValue(obj, null);
        }

        private static object CreateInstance(Type type)
        {
            object sycnRoot;
            Monitor.Enter(sycnRoot = TypeExtensions.SycnRoot);
            object result;
            try
            {
                Func<object> func = null;
                if (TypeExtensions.TypeCache.TryGetValue(type, out func))
                {
                    result = func();
                }
                else
                {
                    NewExpression exp = Expression.New(type);
                    Expression<Func<object>> lambdaExp = Expression.Lambda<Func<object>>(exp, null);
                    func = lambdaExp.Compile();
                    TypeExtensions.TypeCache[type] = func;
                    result = func();
                }
            }
            finally
            {
                Monitor.Exit(sycnRoot);
            }
            return result;
        }

        public static object New(this Type type)
        {
            return TypeExtensions.CreateInstance(type);
        }
    }
}