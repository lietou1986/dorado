using Dorado.DataExpress.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Dorado.DataExpress.Ldo
{
    public static class DataReaderHelper
    {
        internal static Dictionary<Type, Func<DbDataReader, int, object>> ReadFuncCache;
        internal static Func<DbDataReader, int, object> DefaultReadFunc;

        private static T ReadData<T>(DbDataReader reader, int ordinal, Func<object, T> converAction)
        {
            object value = reader.GetValue(ordinal);
            if (value is T)
            {
                return (T)value;
            }
            return converAction(value);
        }

        internal static List<T> ReadEntities<T>(this DbDataReader reader) where T : new()
        {
            List<T> rows = new List<T>(20);
            IEntityBinder<T> binder = BinderManager<T>.Binder;
            List<DataReaderField> fields = reader.GetFieldsInfo();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    T entity = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
                    binder.ReadEntity(reader, fields, entity);
                    rows.Add(entity);
                }
            }
            return rows;
        }

        static DataReaderHelper()
        {
            Dictionary<Type, Func<DbDataReader, int, object>> dictionary = new Dictionary<Type, Func<DbDataReader, int, object>>();
            dictionary.Add(typeof(short), (DbDataReader reader, int ordinal) => DataReaderHelper.ReadData<short>(reader, ordinal, new Func<object, short>(Convert.ToInt16)));
            dictionary.Add(typeof(int), (DbDataReader reader, int ordianl) => DataReaderHelper.ReadData<int>(reader, ordianl, new Func<object, int>(Convert.ToInt32)));
            dictionary.Add(typeof(long), (DbDataReader reader, int ordianl) => DataReaderHelper.ReadData<long>(reader, ordianl, new Func<object, long>(Convert.ToInt64)));
            dictionary.Add(typeof(bool), (DbDataReader reader, int ordianl) => DataReaderHelper.ReadData<bool>(reader, ordianl, new Func<object, bool>(Convert.ToBoolean)));
            dictionary.Add(typeof(string), (DbDataReader reader, int ordianl) => DataReaderHelper.ReadData<string>(reader, ordianl, new Func<object, string>(Convert.ToString)));
            dictionary.Add(typeof(byte), (DbDataReader reader, int ordianl) => DataReaderHelper.ReadData<byte>(reader, ordianl, new Func<object, byte>(Convert.ToByte)));
            dictionary.Add(typeof(double), (DbDataReader reader, int ordianl) => DataReaderHelper.ReadData<double>(reader, ordianl, new Func<object, double>(Convert.ToDouble)));
            dictionary.Add(typeof(decimal), (DbDataReader reader, int ordianl) => DataReaderHelper.ReadData<decimal>(reader, ordianl, new Func<object, decimal>(Convert.ToDecimal)));
            dictionary.Add(typeof(float), (DbDataReader reader, int ordianl) => DataReaderHelper.ReadData<float>(reader, ordianl, new Func<object, float>(Convert.ToSingle)));
            dictionary.Add(typeof(DateTime), (DbDataReader reader, int ordianl) => DataReaderHelper.ReadData<DateTime>(reader, ordianl, new Func<object, DateTime>(Convert.ToDateTime)));
            dictionary.Add(typeof(Guid), (DbDataReader reader, int ordianl) => DataReaderHelper.ReadData<Guid>(reader, ordianl, (object val) => new Guid(val.ToString())));
            dictionary.Add(typeof(byte[]), new Func<DbDataReader, int, object>(DataReaderHelper.ReadBytes));
            dictionary.Add(typeof(char[]), new Func<DbDataReader, int, object>(DataReaderHelper.ReadChars));
            DataReaderHelper.ReadFuncCache = dictionary;
        }

        private static object ReadChars(DbDataReader arg1, int arg2)
        {
            throw new NotImplementedException();
        }

        internal static byte[] ReadBytes(DbDataReader dataReader, int ordianl)
        {
            throw new NotImplementedException();
        }

        public static List<DataReaderField> GetFieldsInfo(this DbDataReader dataReader)
        {
            List<DataReaderField> fields = new List<DataReaderField>(dataReader.FieldCount);
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                fields.Add(new DataReaderField
                {
                    FieldName = dataReader.GetName(i),
                    DataType = dataReader.GetFieldType(i)
                });
            }
            return fields;
        }

        public static object GetTypedValue(this DbDataReader reader, int ordianl, Type targetType)
        {
            Func<DbDataReader, int, object> readFunc;
            if (DataReaderHelper.ReadFuncCache.TryGetValue(targetType, out readFunc))
            {
                return readFunc(reader, ordianl);
            }
            object value = reader.GetValue(ordianl);
            return Converter.Convert(targetType, value);
        }
    }
}