using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Dorado.Core.Data.DataType;
using Dorado.Extensions;
using Dorado.Utils;

namespace Dorado.Core.Data
{
    [Serializable]
    public class DataArrayColumn : IConvertible
    {
        internal string _name;      //名称
        internal int _hash;         //哈希码
        internal int _cursor;       //下标
        internal object _type;      //类型
        internal object _data;      //数据
        internal DataArrayColumns _columns; //相关联的DataArray
        internal DataArrayColumn Left;
        internal DataArrayColumn Right;

        public DataArrayColumn(DataArrayColumns cols, string name, Type type)
        {
            _columns = cols;
            _name = name.ToLower();
            _hash = Name.GetHashCode();
            _type = type;
            _cursor = cols.DataArray.Cursor;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    _data = new bool[_columns.DataArray.RowSize];
                    break;

                case TypeCode.Char:
                    _data = new char[_columns.DataArray.RowSize];
                    break;

                case TypeCode.Byte:
                    _data = new byte[_columns.DataArray.RowSize];
                    break;

                case TypeCode.SByte:
                    _data = new sbyte[_columns.DataArray.RowSize];
                    break;

                case TypeCode.Int16:
                    _data = new short[_columns.DataArray.RowSize];
                    break;

                case TypeCode.UInt16:
                    _data = new ushort[_columns.DataArray.RowSize];
                    break;

                case TypeCode.Int32:
                    _data = new int[_columns.DataArray.RowSize];
                    break;

                case TypeCode.UInt32:
                    _data = new uint[_columns.DataArray.RowSize];
                    break;

                case TypeCode.Int64:
                    _data = new long[_columns.DataArray.RowSize];
                    break;

                case TypeCode.UInt64:
                    _data = new ulong[_columns.DataArray.RowSize];
                    break;

                case TypeCode.Decimal:
                    _data = new decimal[_columns.DataArray.RowSize];
                    break;

                case TypeCode.Double:
                    _data = new double[_columns.DataArray.RowSize];
                    break;

                case TypeCode.Single:
                    _data = new float[_columns.DataArray.RowSize];
                    break;

                case TypeCode.DateTime:
                    _data = new DateTime[_columns.DataArray.RowSize];
                    break;

                case TypeCode.String:
                    _data = new string[_columns.DataArray.RowSize];
                    break;

                case TypeCode.Object:
                    _data = new object[_columns.DataArray.RowSize];
                    break;

                default:
                    throw new ApplicationException("对不起，DataArrayColumn是" + type.Name + "类型！");
            }
            Left = null;
            Right = null;
        }

        internal object Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        public Type Type
        {
            get
            {
                return (Type)_type;
            }
        }

        public TypeCode TypeCode
        {
            get
            {
                return Type.GetTypeCode((Type)_type);
            }
        }

        public DataArray DataArray
        {
            get { return _columns.DataArray; }
        }

        public string Name
        {
            get { return _name; }
        }

        public int Hash
        {
            get { return _hash; }
        }

        public DataArrayColumn this[int index]
        {
            get
            {
                _cursor = index;
                return this;
            }
        }

        public int Cursor
        {
            get { return _cursor; }
            set { _cursor = value; }
        }

        public bool ToBool()
        {
            return ToBoolean(null);
        }

        public ushort ToUShort()
        {
            return ToUInt16(null);
        }

        public short ToShort()
        {
            return ToInt16(null);
        }

        public uint ToUInt()
        {
            return ToUInt32(null);
        }

        public int ToInt()
        {
            return ToInt32(null);
        }

        public ulong ToULong()
        {
            return ToUInt64(null);
        }

        public long ToLong()
        {
            return ToInt64(null);
        }

        public override string ToString()
        {
            return ToString(null);
        }

        public string ToString(int len)
        {
            if (len <= 0) return ToString(null);
            return DataTypeExtensions.Trim(ToString(null), len);
        }

        public string ToMD5(int len)
        {
            return ToString(null).ToMD5(len);
        }

        public string ToMD5()
        {
            return ToString(null).ToMD5();
        }

        public string ToSafeString()
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToString(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return ((char[])_data)[_cursor].ToString();

                case TypeCode.Byte:
                    return ((byte[])_data)[_cursor].ToString();

                case TypeCode.SByte:
                    return ((sbyte[])_data)[_cursor].ToString();

                case TypeCode.Int16:
                    return ((short[])_data)[_cursor].ToString();

                case TypeCode.UInt16:
                    return ((ushort[])_data)[_cursor].ToString();

                case TypeCode.Int32:
                    return ((int[])_data)[_cursor].ToString();

                case TypeCode.UInt32:
                    return ((uint[])_data)[_cursor].ToString();

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return DataTypeExtensions.ToSafeString(((long[])_data)[_cursor].ToString());

                case TypeCode.Decimal:
                    return ((decimal[])_data)[_cursor].ToString();

                case TypeCode.Double:
                    return ((double[])_data)[_cursor].ToString();

                case TypeCode.Single:
                    return ((float[])_data)[_cursor].ToString();

                case TypeCode.DateTime:
                    return "\"" + DataTypeExtensions.ToString(((DateTime[])_data)[_cursor]) + "\"";

                case TypeCode.String:
                    return DataTypeExtensions.ToSafeString(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToSafeString(((Array)_data).GetValue(_cursor));

                default:
                    return string.Empty;
            }
        }

        public float ToFloat()
        {
            return ToSingle(null);
        }

        public double ToDouble()
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToFloat(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return DataTypeExtensions.ToFloat(((char[])_data)[_cursor]);

                case TypeCode.Byte:
                    return DataTypeExtensions.ToFloat(((byte[])_data)[_cursor]);

                case TypeCode.SByte:
                    return DataTypeExtensions.ToFloat(((sbyte[])_data)[_cursor]);

                case TypeCode.Int16:
                    return DataTypeExtensions.ToFloat(((short[])_data)[_cursor]);

                case TypeCode.UInt16:
                    return DataTypeExtensions.ToFloat(((ushort[])_data)[_cursor]);

                case TypeCode.Int32:
                    return DataTypeExtensions.ToFloat(((int[])_data)[_cursor]);

                case TypeCode.UInt32:
                    return DataTypeExtensions.ToFloat(((uint[])_data)[_cursor]);

                case TypeCode.Int64:
                    return DataTypeExtensions.ToFloat(((long[])_data)[_cursor]);

                case TypeCode.UInt64:
                    return DataTypeExtensions.ToFloat(((ulong[])_data)[_cursor]);

                case TypeCode.Decimal:
                    return DataTypeExtensions.ToFloat(((decimal[])_data)[_cursor]);

                case TypeCode.Double:
                    return ((double[])_data)[_cursor];

                case TypeCode.Single:
                    return ((float[])_data)[_cursor];

                case TypeCode.DateTime:
                    return DataTypeExtensions.ToFloat(((DateTime[])_data)[_cursor]);

                case TypeCode.String:
                    return DataTypeExtensions.ToFloat(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToFloat(((Array)_data).GetValue(_cursor));

                default:
                    return 0;
            }
        }

        public decimal ToDecimal()
        {
            return ToDecimal(null);
        }

        public DateTime ToDateTime()
        {
            return ToDateTime(null);
        }

        public string ToDateTimeString()
        {
            return ToDateTimeString("yyyy-MM-dd");
        }

        public string ToDateTimeString(string format)
        {
            return ToDateTime(null).ToString(format);
        }

        public byte ToByte()
        {
            return ToByte(null);
        }

        public string ToBinary()
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToBinary(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return DataTypeExtensions.ToBinary(((char[])_data)[_cursor]);

                case TypeCode.Byte:
                    return DataTypeExtensions.ToBinary(((byte[])_data)[_cursor]);

                case TypeCode.SByte:
                    return DataTypeExtensions.ToBinary(((sbyte[])_data)[_cursor]);

                case TypeCode.Int16:
                    return DataTypeExtensions.ToBinary(((short[])_data)[_cursor]);

                case TypeCode.UInt16:
                    return DataTypeExtensions.ToBinary(((ushort[])_data)[_cursor]);

                case TypeCode.Int32:
                    return DataTypeExtensions.ToBinary(((int[])_data)[_cursor]);

                case TypeCode.UInt32:
                    return DataTypeExtensions.ToBinary(((uint[])_data)[_cursor]);

                case TypeCode.Int64:
                    return DataTypeExtensions.ToBinary(((long[])_data)[_cursor]);

                case TypeCode.UInt64:
                    return DataTypeExtensions.ToBinary(((ulong[])_data)[_cursor]);

                case TypeCode.Decimal:
                    return DataTypeExtensions.ToBinary(((decimal[])_data)[_cursor]);

                case TypeCode.Double:
                    return DataTypeExtensions.ToBinary(((double[])_data)[_cursor]);

                case TypeCode.Single:
                    return DataTypeExtensions.ToBinary(((float[])_data)[_cursor]);

                case TypeCode.DateTime:
                    return DataTypeExtensions.ToBinary(((DateTime[])_data)[_cursor]);

                case TypeCode.String:
                    return DataTypeExtensions.ToBinary(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToBinary(((Array)_data).GetValue(_cursor));

                default:
                    return "'" + string.Empty + "'";
            }
        }

        public object ToObject()
        {
            return ((Array)_data).GetValue(_cursor);
        }

        public string ToList()
        {
            StringBuilder ret = new StringBuilder();

            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        if (ret.Length > 0) ret.Append(',');
                        ret.Append(DataTypeExtensions.ToString(((bool[])_data)[i]));
                    }
                    break;

                case TypeCode.Byte:
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        if (ret.Length > 0) ret.Append(',');
                        ret.Append(((byte[])_data)[i].ToString());
                    }
                    break;

                case TypeCode.SByte:
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        if (ret.Length > 0) ret.Append(',');
                        ret.Append(((sbyte[])_data)[i].ToString());
                    }
                    break;

                case TypeCode.Int16:
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        if (ret.Length > 0) ret.Append(',');
                        ret.Append(((short[])_data)[i].ToString());
                    }
                    break;

                case TypeCode.UInt16:
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        if (ret.Length > 0) ret.Append(',');
                        ret.Append(((ushort[])_data)[i].ToString());
                    }
                    break;

                case TypeCode.Int32:
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        if (ret.Length > 0) ret.Append(',');
                        ret.Append(((int[])_data)[i].ToString());
                    }
                    break;

                case TypeCode.UInt32:
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        if (ret.Length > 0) ret.Append(',');
                        ret.Append(((uint[])_data)[i].ToString());
                    }
                    break;

                case TypeCode.Int64:
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        if (ret.Length > 0) ret.Append(',');
                        ret.Append(((long[])_data)[i].ToString());
                    }
                    break;

                case TypeCode.UInt64:
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        if (ret.Length > 0) ret.Append(',');
                        ret.Append(((ulong[])_data)[i].ToString());
                    }
                    break;

                case TypeCode.Decimal:
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        if (ret.Length > 0) ret.Append(',');
                        ret.Append(((decimal[])_data)[i].ToString());
                    }
                    break;

                case TypeCode.Double:
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        if (ret.Length > 0) ret.Append(',');
                        ret.Append(((double[])_data)[i].ToString());
                    }
                    break;

                case TypeCode.Single:
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        if (ret.Length > 0) ret.Append(',');
                        ret.Append(((float[])_data)[i].ToString());
                    }
                    break;

                case TypeCode.DateTime:
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        if (ret.Length > 0) ret.Append(',');
                        ret.Append(DataTypeExtensions.ToString(((DateTime[])_data)[i]));
                    }
                    break;

                case TypeCode.String:
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        string tmp = ((string[])_data)[i];
                        if (tmp != null && tmp != string.Empty)
                        {
                            if (ret.Length > 0) ret.Append(',');
                            ret.Append(tmp);
                        }
                    }
                    break;

                case TypeCode.Object:
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        object tmp = ((object[])_data)[i];
                        if (tmp != null)
                        {
                            if (ret.Length > 0) ret.Append(',');
                            ret.Append(tmp.ToString());
                        }
                    }
                    break;
            }
            return ret.ToString();
        }

        public string ToSQL()
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToSql(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return ((char[])_data)[_cursor].ToString();

                case TypeCode.Byte:
                    return ((byte[])_data)[_cursor].ToString();

                case TypeCode.SByte:
                    return ((sbyte[])_data)[_cursor].ToString();

                case TypeCode.Int16:
                    return ((short[])_data)[_cursor].ToString();

                case TypeCode.UInt16:
                    return ((ushort[])_data)[_cursor].ToString();

                case TypeCode.Int32:
                    return ((int[])_data)[_cursor].ToString();

                case TypeCode.UInt32:
                    return ((uint[])_data)[_cursor].ToString();

                case TypeCode.Int64:
                    return ((long[])_data)[_cursor].ToString();

                case TypeCode.UInt64:
                    return ((ulong[])_data)[_cursor].ToString();

                case TypeCode.Decimal:
                    return ((decimal[])_data)[_cursor].ToString();

                case TypeCode.Double:
                    return ((double[])_data)[_cursor].ToString();

                case TypeCode.Single:
                    return ((float[])_data)[_cursor].ToString();

                case TypeCode.DateTime:
                    return DataTypeExtensions.ToSql(((DateTime[])_data)[_cursor]);

                case TypeCode.String:
                    return DataTypeExtensions.ToSql(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToSql(((Array)_data).GetValue(_cursor));

                default:
                    return "'" + string.Empty + "'";
            }
        }

        public bool ChangeType(Type type)
        {
            if (type.Equals(_type)) return true;

            Array tmp = Array.CreateInstance((Type)_type, _columns.DataArray.RowSize);
            for (int i = 0; i < _columns.DataArray.Count; i++)
                tmp.SetValue(DataTypeExtensions.Convert(((Array)_data).GetValue(i), type), i);
            _data = tmp;
            return true;
        }

        public DataArrayColumn GenerateId()
        {
            Set(CommonUtility.GenerateId(), _cursor);
            return this;
        }

        public DataArrayColumn Set(bool value)
        {
            Set(value, _cursor);
            return this;
        }

        public DataArrayColumn Set(char value)
        {
            Set(value, _cursor);
            return this;
        }

        public DataArrayColumn Set(byte value)
        {
            Set(value, _cursor);
            return this;
        }

        public DataArrayColumn Set(sbyte value)
        {
            Set(value, _cursor);
            return this;
        }

        public DataArrayColumn Set(short value)
        {
            Set(value, _cursor);
            return this;
        }

        public DataArrayColumn Set(ushort value)
        {
            Set(value, _cursor);
            return this;
        }

        public DataArrayColumn Set(int value)
        {
            Set(value, _cursor);
            return this;
        }

        public DataArrayColumn Set(uint value)
        {
            Set(value, _cursor);
            return this;
        }

        public DataArrayColumn Set(long value)
        {
            Set(value, _cursor);
            return this;
        }

        public DataArrayColumn Set(ulong value)
        {
            Set(value, _cursor);
            return this;
        }

        public DataArrayColumn Set(decimal value)
        {
            Set(value, _cursor);
            return this;
        }

        public DataArrayColumn Set(double value)
        {
            Set(value, _cursor);
            return this;
        }

        public DataArrayColumn Set(float value)
        {
            Set(value, _cursor);
            return this;
        }

        public DataArrayColumn Set(DateTime value)
        {
            Set(value, _cursor);
            return this;
        }

        public DataArrayColumn Set(string value)
        {
            Set(value, _cursor);
            return this;
        }

        public DataArrayColumn Set(object value)
        {
            Set(value, _cursor);
            return this;
        }

        public DataArrayColumn Set(Enum value)
        {
            Set(value, _cursor);
            return this;
        }

        public DataArrayColumn Set(bool value, int index)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    ((bool[])_data)[index] = value;
                    break;

                case TypeCode.Char:
                    ((char[])_data)[index] = DataTypeExtensions.ToChar(value);
                    break;

                case TypeCode.Byte:
                    ((byte[])_data)[index] = DataTypeExtensions.ToByte(value);
                    break;

                case TypeCode.SByte:
                    ((sbyte[])_data)[index] = DataTypeExtensions.ToSByte(value);
                    break;

                case TypeCode.Int16:
                    ((short[])_data)[index] = DataTypeExtensions.ToShort(value);
                    break;

                case TypeCode.UInt16:
                    ((ushort[])_data)[index] = DataTypeExtensions.ToUShort(value);
                    break;

                case TypeCode.Int32:
                    ((int[])_data)[index] = DataTypeExtensions.ToInt(value);
                    break;

                case TypeCode.UInt32:
                    ((uint[])_data)[index] = DataTypeExtensions.ToUInt(value);
                    break;

                case TypeCode.Int64:
                    ((long[])_data)[index] = DataTypeExtensions.ToLong(value);
                    break;

                case TypeCode.UInt64:
                    ((ulong[])_data)[index] = DataTypeExtensions.ToULong(value);
                    break;

                case TypeCode.Decimal:
                    ((decimal[])_data)[index] = DataTypeExtensions.ToDecimal(value);
                    break;

                case TypeCode.Double:
                    ((double[])_data)[index] = DataTypeExtensions.ToDouble(value);
                    break;

                case TypeCode.Single:
                    ((float[])_data)[index] = DataTypeExtensions.ToFloat(value);
                    break;

                case TypeCode.DateTime:
                    ((DateTime[])_data)[index] = DataTypeExtensions.ToDateTime(value);
                    break;

                case TypeCode.String:
                    ((string[])_data)[index] = DataTypeExtensions.ToString(value);
                    break;

                case TypeCode.Object:
                    ((Array)_data).SetValue(value, index);
                    break;
            }
            return this;
        }

        public DataArrayColumn Set(char value, int index)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    ((bool[])_data)[index] = DataTypeExtensions.ToBool(value);
                    break;

                case TypeCode.Char:
                    ((char[])_data)[index] = value;
                    break;

                case TypeCode.Byte:
                    ((byte[])_data)[index] = DataTypeExtensions.ToByte(value);
                    break;

                case TypeCode.SByte:
                    ((sbyte[])_data)[index] = DataTypeExtensions.ToSByte(value);
                    break;

                case TypeCode.Int16:
                    ((short[])_data)[index] = DataTypeExtensions.ToShort(value);
                    break;

                case TypeCode.UInt16:
                    ((ushort[])_data)[index] = DataTypeExtensions.ToUShort(value);
                    break;

                case TypeCode.Int32:
                    ((int[])_data)[index] = DataTypeExtensions.ToInt(value);
                    break;

                case TypeCode.UInt32:
                    ((uint[])_data)[index] = DataTypeExtensions.ToUInt(value);
                    break;

                case TypeCode.Int64:
                    ((long[])_data)[index] = DataTypeExtensions.ToLong(value);
                    break;

                case TypeCode.UInt64:
                    ((ulong[])_data)[index] = DataTypeExtensions.ToULong(value);
                    break;

                case TypeCode.Decimal:
                    ((decimal[])_data)[index] = DataTypeExtensions.ToDecimal(value);
                    break;

                case TypeCode.Double:
                    ((double[])_data)[index] = DataTypeExtensions.ToDouble(value);
                    break;

                case TypeCode.Single:
                    ((float[])_data)[index] = DataTypeExtensions.ToFloat(value);
                    break;

                case TypeCode.DateTime:
                    ((DateTime[])_data)[index] = DataTypeExtensions.ToDateTime(value);
                    break;

                case TypeCode.String:
                    ((string[])_data)[index] = DataTypeExtensions.ToString(value);
                    break;

                case TypeCode.Object:
                    ((Array)_data).SetValue(value, index);
                    break;
            }
            return this;
        }

        public DataArrayColumn Set(byte value, int index)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    ((bool[])_data)[index] = DataTypeExtensions.ToBool(value);
                    break;

                case TypeCode.Char:
                    ((char[])_data)[index] = DataTypeExtensions.ToChar(value);
                    break;

                case TypeCode.Byte:
                    ((byte[])_data)[index] = value;
                    break;

                case TypeCode.SByte:
                    ((sbyte[])_data)[index] = DataTypeExtensions.ToSByte(value);
                    break;

                case TypeCode.Int16:
                    ((short[])_data)[index] = DataTypeExtensions.ToShort(value);
                    break;

                case TypeCode.UInt16:
                    ((ushort[])_data)[index] = DataTypeExtensions.ToUShort(value);
                    break;

                case TypeCode.Int32:
                    ((int[])_data)[index] = DataTypeExtensions.ToInt(value);
                    break;

                case TypeCode.UInt32:
                    ((uint[])_data)[index] = DataTypeExtensions.ToUInt(value);
                    break;

                case TypeCode.Int64:
                    ((long[])_data)[index] = DataTypeExtensions.ToLong(value);
                    break;

                case TypeCode.UInt64:
                    ((ulong[])_data)[index] = DataTypeExtensions.ToULong(value);
                    break;

                case TypeCode.Decimal:
                    ((decimal[])_data)[index] = DataTypeExtensions.ToDecimal(value);
                    break;

                case TypeCode.Double:
                    ((double[])_data)[index] = DataTypeExtensions.ToDouble(value);
                    break;

                case TypeCode.Single:
                    ((float[])_data)[index] = DataTypeExtensions.ToFloat(value);
                    break;

                case TypeCode.DateTime:
                    ((DateTime[])_data)[index] = DataTypeExtensions.ToDateTime(value);
                    break;

                case TypeCode.String:
                    ((string[])_data)[index] = DataTypeExtensions.ToString(value);
                    break;

                case TypeCode.Object:
                    ((Array)_data).SetValue(value, index);
                    break;
            }
            return this;
        }

        public DataArrayColumn Set(sbyte value, int index)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    ((bool[])_data)[index] = DataTypeExtensions.ToBool(value);
                    break;

                case TypeCode.Char:
                    ((char[])_data)[index] = DataTypeExtensions.ToChar(value);
                    break;

                case TypeCode.Byte:
                    ((byte[])_data)[index] = DataTypeExtensions.ToByte(value);
                    break;

                case TypeCode.SByte:
                    ((sbyte[])_data)[index] = value;
                    break;

                case TypeCode.Int16:
                    ((short[])_data)[index] = DataTypeExtensions.ToShort(value);
                    break;

                case TypeCode.UInt16:
                    ((ushort[])_data)[index] = DataTypeExtensions.ToUShort(value);
                    break;

                case TypeCode.Int32:
                    ((int[])_data)[index] = DataTypeExtensions.ToInt(value);
                    break;

                case TypeCode.UInt32:
                    ((uint[])_data)[index] = DataTypeExtensions.ToUInt(value);
                    break;

                case TypeCode.Int64:
                    ((long[])_data)[index] = DataTypeExtensions.ToLong(value);
                    break;

                case TypeCode.UInt64:
                    ((ulong[])_data)[index] = DataTypeExtensions.ToULong(value);
                    break;

                case TypeCode.Decimal:
                    ((decimal[])_data)[index] = DataTypeExtensions.ToDecimal(value);
                    break;

                case TypeCode.Double:
                    ((double[])_data)[index] = DataTypeExtensions.ToDouble(value);
                    break;

                case TypeCode.Single:
                    ((float[])_data)[index] = DataTypeExtensions.ToFloat(value);
                    break;

                case TypeCode.DateTime:
                    ((DateTime[])_data)[index] = DataTypeExtensions.ToDateTime(value);
                    break;

                case TypeCode.String:
                    ((string[])_data)[index] = DataTypeExtensions.ToString(value);
                    break;

                case TypeCode.Object:
                    ((Array)_data).SetValue(value, index);
                    break;
            }
            return this;
        }

        public DataArrayColumn Set(short value, int index)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    ((bool[])_data)[index] = DataTypeExtensions.ToBool(value);
                    break;

                case TypeCode.Char:
                    ((char[])_data)[index] = DataTypeExtensions.ToChar(value);
                    break;

                case TypeCode.Byte:
                    ((byte[])_data)[index] = DataTypeExtensions.ToByte(value);
                    break;

                case TypeCode.SByte:
                    ((sbyte[])_data)[index] = DataTypeExtensions.ToSByte(value);
                    break;

                case TypeCode.Int16:
                    ((short[])_data)[index] = value;
                    break;

                case TypeCode.UInt16:
                    ((ushort[])_data)[index] = DataTypeExtensions.ToUShort(value);
                    break;

                case TypeCode.Int32:
                    ((int[])_data)[index] = DataTypeExtensions.ToInt(value);
                    break;

                case TypeCode.UInt32:
                    ((uint[])_data)[index] = DataTypeExtensions.ToUInt(value);
                    break;

                case TypeCode.Int64:
                    ((long[])_data)[index] = DataTypeExtensions.ToLong(value);
                    break;

                case TypeCode.UInt64:
                    ((ulong[])_data)[index] = DataTypeExtensions.ToULong(value);
                    break;

                case TypeCode.Decimal:
                    ((decimal[])_data)[index] = DataTypeExtensions.ToDecimal(value);
                    break;

                case TypeCode.Double:
                    ((double[])_data)[index] = DataTypeExtensions.ToDouble(value);
                    break;

                case TypeCode.Single:
                    ((float[])_data)[index] = DataTypeExtensions.ToFloat(value);
                    break;

                case TypeCode.DateTime:
                    ((DateTime[])_data)[index] = DataTypeExtensions.ToDateTime(value);
                    break;

                case TypeCode.String:
                    ((string[])_data)[index] = DataTypeExtensions.ToString(value);
                    break;

                case TypeCode.Object:
                    ((Array)_data).SetValue(value, index);
                    break;
            }
            return this;
        }

        public DataArrayColumn Set(ushort value, int index)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    ((bool[])_data)[index] = DataTypeExtensions.ToBool(value);
                    break;

                case TypeCode.Char:
                    ((char[])_data)[index] = DataTypeExtensions.ToChar(value);
                    break;

                case TypeCode.Byte:
                    ((byte[])_data)[index] = DataTypeExtensions.ToByte(value);
                    break;

                case TypeCode.SByte:
                    ((sbyte[])_data)[index] = DataTypeExtensions.ToSByte(value);
                    break;

                case TypeCode.Int16:
                    ((short[])_data)[index] = DataTypeExtensions.ToShort(value);
                    break;

                case TypeCode.UInt16:
                    ((ushort[])_data)[index] = value;
                    break;

                case TypeCode.Int32:
                    ((int[])_data)[index] = DataTypeExtensions.ToInt(value);
                    break;

                case TypeCode.UInt32:
                    ((uint[])_data)[index] = DataTypeExtensions.ToUInt(value);
                    break;

                case TypeCode.Int64:
                    ((long[])_data)[index] = DataTypeExtensions.ToLong(value);
                    break;

                case TypeCode.UInt64:
                    ((ulong[])_data)[index] = DataTypeExtensions.ToULong(value);
                    break;

                case TypeCode.Decimal:
                    ((decimal[])_data)[index] = DataTypeExtensions.ToDecimal(value);
                    break;

                case TypeCode.Double:
                    ((double[])_data)[index] = DataTypeExtensions.ToDouble(value);
                    break;

                case TypeCode.Single:
                    ((float[])_data)[index] = DataTypeExtensions.ToFloat(value);
                    break;

                case TypeCode.DateTime:
                    ((DateTime[])_data)[index] = DataTypeExtensions.ToDateTime(value);
                    break;

                case TypeCode.String:
                    ((string[])_data)[index] = DataTypeExtensions.ToString(value);
                    break;

                case TypeCode.Object:
                    ((Array)_data).SetValue(value, index);
                    break;
            }
            return this;
        }

        public DataArrayColumn Set(int value, int index)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    ((bool[])_data)[index] = DataTypeExtensions.ToBool(value);
                    break;

                case TypeCode.Char:
                    ((char[])_data)[index] = DataTypeExtensions.ToChar(value);
                    break;

                case TypeCode.Byte:
                    ((byte[])_data)[index] = DataTypeExtensions.ToByte(value);
                    break;

                case TypeCode.SByte:
                    ((sbyte[])_data)[index] = DataTypeExtensions.ToSByte(value);
                    break;

                case TypeCode.Int16:
                    ((short[])_data)[index] = DataTypeExtensions.ToShort(value);
                    break;

                case TypeCode.UInt16:
                    ((ushort[])_data)[index] = DataTypeExtensions.ToUShort(value);
                    break;

                case TypeCode.Int32:
                    ((int[])_data)[index] = value;
                    break;

                case TypeCode.UInt32:
                    ((uint[])_data)[index] = DataTypeExtensions.ToUInt(value);
                    break;

                case TypeCode.Int64:
                    ((long[])_data)[index] = DataTypeExtensions.ToLong(value);
                    break;

                case TypeCode.UInt64:
                    ((ulong[])_data)[index] = DataTypeExtensions.ToULong(value);
                    break;

                case TypeCode.Decimal:
                    ((decimal[])_data)[index] = DataTypeExtensions.ToDecimal(value);
                    break;

                case TypeCode.Double:
                    ((double[])_data)[index] = DataTypeExtensions.ToDouble(value);
                    break;

                case TypeCode.Single:
                    ((float[])_data)[index] = DataTypeExtensions.ToFloat(value);
                    break;

                case TypeCode.DateTime:
                    ((DateTime[])_data)[index] = DataTypeExtensions.ToDateTime(value);
                    break;

                case TypeCode.String:
                    ((string[])_data)[index] = DataTypeExtensions.ToString(value);
                    break;

                case TypeCode.Object:
                    ((Array)_data).SetValue(value, index);
                    break;
            }
            return this;
        }

        public DataArrayColumn Set(uint value, int index)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    ((bool[])_data)[index] = DataTypeExtensions.ToBool(value);
                    break;

                case TypeCode.Char:
                    ((char[])_data)[index] = DataTypeExtensions.ToChar(value);
                    break;

                case TypeCode.Byte:
                    ((byte[])_data)[index] = DataTypeExtensions.ToByte(value);
                    break;

                case TypeCode.SByte:
                    ((sbyte[])_data)[index] = DataTypeExtensions.ToSByte(value);
                    break;

                case TypeCode.Int16:
                    ((short[])_data)[index] = DataTypeExtensions.ToShort(value);
                    break;

                case TypeCode.UInt16:
                    ((ushort[])_data)[index] = DataTypeExtensions.ToUShort(value);
                    break;

                case TypeCode.Int32:
                    ((int[])_data)[index] = DataTypeExtensions.ToInt(value);
                    break;

                case TypeCode.UInt32:
                    ((uint[])_data)[index] = value;
                    break;

                case TypeCode.Int64:
                    ((long[])_data)[index] = DataTypeExtensions.ToLong(value);
                    break;

                case TypeCode.UInt64:
                    ((ulong[])_data)[index] = DataTypeExtensions.ToULong(value);
                    break;

                case TypeCode.Decimal:
                    ((decimal[])_data)[index] = DataTypeExtensions.ToDecimal(value);
                    break;

                case TypeCode.Double:
                    ((double[])_data)[index] = DataTypeExtensions.ToDouble(value);
                    break;

                case TypeCode.Single:
                    ((float[])_data)[index] = DataTypeExtensions.ToFloat(value);
                    break;

                case TypeCode.DateTime:
                    ((DateTime[])_data)[index] = DataTypeExtensions.ToDateTime(value);
                    break;

                case TypeCode.String:
                    ((string[])_data)[index] = DataTypeExtensions.ToString(value);
                    break;

                case TypeCode.Object:
                    ((Array)_data).SetValue(value, index);
                    break;
            }
            return this;
        }

        public DataArrayColumn Set(long value, int index)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    ((bool[])_data)[index] = DataTypeExtensions.ToBool(value);
                    break;

                case TypeCode.Char:
                    ((char[])_data)[index] = DataTypeExtensions.ToChar(value);
                    break;

                case TypeCode.Byte:
                    ((byte[])_data)[index] = DataTypeExtensions.ToByte(value);
                    break;

                case TypeCode.SByte:
                    ((sbyte[])_data)[index] = DataTypeExtensions.ToSByte(value);
                    break;

                case TypeCode.Int16:
                    ((short[])_data)[index] = DataTypeExtensions.ToShort(value);
                    break;

                case TypeCode.UInt16:
                    ((ushort[])_data)[index] = DataTypeExtensions.ToUShort(value);
                    break;

                case TypeCode.Int32:
                    ((int[])_data)[index] = DataTypeExtensions.ToInt(value);
                    break;

                case TypeCode.UInt32:
                    ((uint[])_data)[index] = DataTypeExtensions.ToUInt(value);
                    break;

                case TypeCode.Int64:
                    ((long[])_data)[index] = value;
                    break;

                case TypeCode.UInt64:
                    ((ulong[])_data)[index] = DataTypeExtensions.ToULong(value);
                    break;

                case TypeCode.Decimal:
                    ((decimal[])_data)[index] = DataTypeExtensions.ToDecimal(value);
                    break;

                case TypeCode.Double:
                    ((double[])_data)[index] = DataTypeExtensions.ToDouble(value);
                    break;

                case TypeCode.Single:
                    ((float[])_data)[index] = DataTypeExtensions.ToFloat(value);
                    break;

                case TypeCode.DateTime:
                    ((DateTime[])_data)[index] = DataTypeExtensions.ToDateTime(value);
                    break;

                case TypeCode.String:
                    ((string[])_data)[index] = DataTypeExtensions.ToString(value);
                    break;

                case TypeCode.Object:
                    ((Array)_data).SetValue(value, index);
                    break;
            }
            return this;
        }

        public DataArrayColumn Set(ulong value, int index)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    ((bool[])_data)[index] = DataTypeExtensions.ToBool(value);
                    break;

                case TypeCode.Char:
                    ((char[])_data)[index] = DataTypeExtensions.ToChar(value);
                    break;

                case TypeCode.Byte:
                    ((byte[])_data)[index] = DataTypeExtensions.ToByte(value);
                    break;

                case TypeCode.SByte:
                    ((sbyte[])_data)[index] = DataTypeExtensions.ToSByte(value);
                    break;

                case TypeCode.Int16:
                    ((short[])_data)[index] = DataTypeExtensions.ToShort(value);
                    break;

                case TypeCode.UInt16:
                    ((ushort[])_data)[index] = DataTypeExtensions.ToUShort(value);
                    break;

                case TypeCode.Int32:
                    ((int[])_data)[index] = DataTypeExtensions.ToInt(value);
                    break;

                case TypeCode.UInt32:
                    ((uint[])_data)[index] = DataTypeExtensions.ToUInt(value);
                    break;

                case TypeCode.Int64:
                    ((long[])_data)[index] = DataTypeExtensions.ToLong(value);
                    break;

                case TypeCode.UInt64:
                    ((ulong[])_data)[index] = value;
                    break;

                case TypeCode.Decimal:
                    ((decimal[])_data)[index] = DataTypeExtensions.ToDecimal(value);
                    break;

                case TypeCode.Double:
                    ((double[])_data)[index] = DataTypeExtensions.ToDouble(value);
                    break;

                case TypeCode.Single:
                    ((float[])_data)[index] = DataTypeExtensions.ToFloat(value);
                    break;

                case TypeCode.DateTime:
                    ((DateTime[])_data)[index] = DataTypeExtensions.ToDateTime(value);
                    break;

                case TypeCode.String:
                    ((string[])_data)[index] = DataTypeExtensions.ToString(value);
                    break;

                case TypeCode.Object:
                    ((Array)_data).SetValue(value, index);
                    break;
            }
            return this;
        }

        public DataArrayColumn Set(decimal value, int index)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    ((bool[])_data)[index] = DataTypeExtensions.ToBool(value);
                    break;

                case TypeCode.Char:
                    ((char[])_data)[index] = DataTypeExtensions.ToChar(value);
                    break;

                case TypeCode.Byte:
                    ((byte[])_data)[index] = DataTypeExtensions.ToByte(value);
                    break;

                case TypeCode.SByte:
                    ((sbyte[])_data)[index] = DataTypeExtensions.ToSByte(value);
                    break;

                case TypeCode.Int16:
                    ((short[])_data)[index] = DataTypeExtensions.ToShort(value);
                    break;

                case TypeCode.UInt16:
                    ((ushort[])_data)[index] = DataTypeExtensions.ToUShort(value);
                    break;

                case TypeCode.Int32:
                    ((int[])_data)[index] = DataTypeExtensions.ToInt(value);
                    break;

                case TypeCode.UInt32:
                    ((uint[])_data)[index] = DataTypeExtensions.ToUInt(value);
                    break;

                case TypeCode.Int64:
                    ((long[])_data)[index] = DataTypeExtensions.ToLong(value);
                    break;

                case TypeCode.UInt64:
                    ((ulong[])_data)[index] = DataTypeExtensions.ToULong(value);
                    break;

                case TypeCode.Decimal:
                    ((decimal[])_data)[index] = value;
                    break;

                case TypeCode.Double:
                    ((double[])_data)[index] = DataTypeExtensions.ToDouble(value);
                    break;

                case TypeCode.Single:
                    ((float[])_data)[index] = DataTypeExtensions.ToFloat(value);
                    break;

                case TypeCode.DateTime:
                    ((DateTime[])_data)[index] = DataTypeExtensions.ToDateTime(value);
                    break;

                case TypeCode.String:
                    ((string[])_data)[index] = DataTypeExtensions.ToString(value);
                    break;

                case TypeCode.Object:
                    ((Array)_data).SetValue(value, index);
                    break;
            }
            return this;
        }

        public DataArrayColumn Set(double value, int index)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    ((bool[])_data)[index] = DataTypeExtensions.ToBool(value);
                    break;

                case TypeCode.Char:
                    ((char[])_data)[index] = DataTypeExtensions.ToChar(value);
                    break;

                case TypeCode.Byte:
                    ((byte[])_data)[index] = DataTypeExtensions.ToByte(value);
                    break;

                case TypeCode.SByte:
                    ((sbyte[])_data)[index] = DataTypeExtensions.ToSByte(value);
                    break;

                case TypeCode.Int16:
                    ((short[])_data)[index] = DataTypeExtensions.ToShort(value);
                    break;

                case TypeCode.UInt16:
                    ((ushort[])_data)[index] = DataTypeExtensions.ToUShort(value);
                    break;

                case TypeCode.Int32:
                    ((int[])_data)[index] = DataTypeExtensions.ToInt(value);
                    break;

                case TypeCode.UInt32:
                    ((uint[])_data)[index] = DataTypeExtensions.ToUInt(value);
                    break;

                case TypeCode.Int64:
                    ((long[])_data)[index] = DataTypeExtensions.ToLong(value);
                    break;

                case TypeCode.UInt64:
                    ((ulong[])_data)[index] = DataTypeExtensions.ToULong(value);
                    break;

                case TypeCode.Decimal:
                    ((decimal[])_data)[index] = DataTypeExtensions.ToDecimal(value);
                    break;

                case TypeCode.Double:
                    ((double[])_data)[index] = value;
                    break;

                case TypeCode.Single:
                    ((float[])_data)[index] = DataTypeExtensions.ToFloat(value);
                    break;

                case TypeCode.DateTime:
                    ((DateTime[])_data)[index] = DataTypeExtensions.ToDateTime(value);
                    break;

                case TypeCode.String:
                    ((string[])_data)[index] = DataTypeExtensions.ToString(value);
                    break;

                case TypeCode.Object:
                    ((Array)_data).SetValue(value, index);
                    break;
            }
            return this;
        }

        public DataArrayColumn Set(float value, int index)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    ((bool[])_data)[index] = DataTypeExtensions.ToBool(value);
                    break;

                case TypeCode.Char:
                    ((char[])_data)[index] = DataTypeExtensions.ToChar(value);
                    break;

                case TypeCode.Byte:
                    ((byte[])_data)[index] = DataTypeExtensions.ToByte(value);
                    break;

                case TypeCode.SByte:
                    ((sbyte[])_data)[index] = DataTypeExtensions.ToSByte(value);
                    break;

                case TypeCode.Int16:
                    ((short[])_data)[index] = DataTypeExtensions.ToShort(value);
                    break;

                case TypeCode.UInt16:
                    ((ushort[])_data)[index] = DataTypeExtensions.ToUShort(value);
                    break;

                case TypeCode.Int32:
                    ((int[])_data)[index] = DataTypeExtensions.ToInt(value);
                    break;

                case TypeCode.UInt32:
                    ((uint[])_data)[index] = DataTypeExtensions.ToUInt(value);
                    break;

                case TypeCode.Int64:
                    ((long[])_data)[index] = DataTypeExtensions.ToLong(value);
                    break;

                case TypeCode.UInt64:
                    ((ulong[])_data)[index] = DataTypeExtensions.ToULong(value);
                    break;

                case TypeCode.Decimal:
                    ((decimal[])_data)[index] = DataTypeExtensions.ToDecimal(value);
                    break;

                case TypeCode.Double:
                    ((double[])_data)[index] = DataTypeExtensions.ToDouble(value);
                    break;

                case TypeCode.Single:
                    ((float[])_data)[index] = value;
                    break;

                case TypeCode.DateTime:
                    ((DateTime[])_data)[index] = DataTypeExtensions.ToDateTime(value);
                    break;

                case TypeCode.String:
                    ((string[])_data)[index] = DataTypeExtensions.ToString(value);
                    break;

                case TypeCode.Object:
                    ((Array)_data).SetValue(value, index);
                    break;
            }
            return this;
        }

        public DataArrayColumn Set(DateTime value, int index)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    ((bool[])_data)[index] = DataTypeExtensions.ToBool(value);
                    break;

                case TypeCode.Char:
                    ((char[])_data)[index] = DataTypeExtensions.ToChar(value);
                    break;

                case TypeCode.Byte:
                    ((byte[])_data)[index] = DataTypeExtensions.ToByte(value);
                    break;

                case TypeCode.SByte:
                    ((sbyte[])_data)[index] = DataTypeExtensions.ToSByte(value);
                    break;

                case TypeCode.Int16:
                    ((short[])_data)[index] = DataTypeExtensions.ToShort(value);
                    break;

                case TypeCode.UInt16:
                    ((ushort[])_data)[index] = DataTypeExtensions.ToUShort(value);
                    break;

                case TypeCode.Int32:
                    ((int[])_data)[index] = DataTypeExtensions.ToInt(value);
                    break;

                case TypeCode.UInt32:
                    ((uint[])_data)[index] = DataTypeExtensions.ToUInt(value);
                    break;

                case TypeCode.Int64:
                    ((long[])_data)[index] = DataTypeExtensions.ToLong(value);
                    break;

                case TypeCode.UInt64:
                    ((ulong[])_data)[index] = DataTypeExtensions.ToULong(value);
                    break;

                case TypeCode.Decimal:
                    ((decimal[])_data)[index] = DataTypeExtensions.ToDecimal(value);
                    break;

                case TypeCode.Double:
                    ((double[])_data)[index] = DataTypeExtensions.ToDouble(value);
                    break;

                case TypeCode.Single:
                    ((float[])_data)[index] = DataTypeExtensions.ToFloat(value);
                    break;

                case TypeCode.DateTime:
                    ((DateTime[])_data)[index] = value;
                    break;

                case TypeCode.String:
                    ((string[])_data)[index] = DataTypeExtensions.ToString(value);
                    break;

                case TypeCode.Object:
                    ((Array)_data).SetValue(value, index);
                    break;
            }
            return this;
        }

        public DataArrayColumn Set(string value, int index)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    ((bool[])_data)[index] = DataTypeExtensions.ToBool(value);
                    break;

                case TypeCode.Char:
                    ((char[])_data)[index] = DataTypeExtensions.ToChar(value);
                    break;

                case TypeCode.Byte:
                    ((byte[])_data)[index] = DataTypeExtensions.ToByte(value);
                    break;

                case TypeCode.SByte:
                    ((sbyte[])_data)[index] = DataTypeExtensions.ToSByte(value);
                    break;

                case TypeCode.Int16:
                    ((short[])_data)[index] = DataTypeExtensions.ToShort(value);
                    break;

                case TypeCode.UInt16:
                    ((ushort[])_data)[index] = DataTypeExtensions.ToUShort(value);
                    break;

                case TypeCode.Int32:
                    ((int[])_data)[index] = DataTypeExtensions.ToInt(value);
                    break;

                case TypeCode.UInt32:
                    ((uint[])_data)[index] = DataTypeExtensions.ToUInt(value);
                    break;

                case TypeCode.Int64:
                    ((long[])_data)[index] = DataTypeExtensions.ToLong(value);
                    break;

                case TypeCode.UInt64:
                    ((ulong[])_data)[index] = DataTypeExtensions.ToULong(value);
                    break;

                case TypeCode.Decimal:
                    ((decimal[])_data)[index] = DataTypeExtensions.ToDecimal(value);
                    break;

                case TypeCode.Double:
                    ((double[])_data)[index] = DataTypeExtensions.ToDouble(value);
                    break;

                case TypeCode.Single:
                    ((float[])_data)[index] = DataTypeExtensions.ToFloat(value);
                    break;

                case TypeCode.DateTime:
                    ((DateTime[])_data)[index] = DataTypeExtensions.ToDateTime(value);
                    break;

                case TypeCode.String:
                    ((string[])_data)[index] = DataTypeExtensions.ToString(value);
                    break;

                case TypeCode.Object:
                    ((Array)_data).SetValue(value, index);
                    break;
            }
            return this;
        }

        public DataArrayColumn Set(object value, int index)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    ((bool[])_data)[index] = DataTypeExtensions.ToBool(value);
                    break;

                case TypeCode.Char:
                    ((char[])_data)[index] = DataTypeExtensions.ToChar(value);
                    break;

                case TypeCode.Byte:
                    ((byte[])_data)[index] = DataTypeExtensions.ToByte(value);
                    break;

                case TypeCode.SByte:
                    ((sbyte[])_data)[index] = DataTypeExtensions.ToSByte(value);
                    break;

                case TypeCode.Int16:
                    ((short[])_data)[index] = DataTypeExtensions.ToShort(value);
                    break;

                case TypeCode.UInt16:
                    ((ushort[])_data)[index] = DataTypeExtensions.ToUShort(value);
                    break;

                case TypeCode.Int32:
                    ((int[])_data)[index] = DataTypeExtensions.ToInt(value);
                    break;

                case TypeCode.UInt32:
                    ((uint[])_data)[index] = DataTypeExtensions.ToUInt(value);
                    break;

                case TypeCode.Int64:
                    ((long[])_data)[index] = DataTypeExtensions.ToLong(value);
                    break;

                case TypeCode.UInt64:
                    ((ulong[])_data)[index] = DataTypeExtensions.ToULong(value);
                    break;

                case TypeCode.Decimal:
                    ((decimal[])_data)[index] = DataTypeExtensions.ToDecimal(value);
                    break;

                case TypeCode.Double:
                    ((double[])_data)[index] = DataTypeExtensions.ToDouble(value);
                    break;

                case TypeCode.Single:
                    ((float[])_data)[index] = DataTypeExtensions.ToFloat(value);
                    break;

                case TypeCode.DateTime:
                    ((DateTime[])_data)[index] = DataTypeExtensions.ToDateTime(value);
                    break;

                case TypeCode.String:
                    ((string[])_data)[index] = DataTypeExtensions.ToString(value);
                    break;

                case TypeCode.Object:
                    ((Array)_data).SetValue(value, index);
                    break;
            }
            return this;
        }

        public DataArrayColumn Set(Enum value, int index)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    ((bool[])_data)[index] = DataTypeExtensions.ToBool(value);
                    break;

                case TypeCode.Char:
                    ((char[])_data)[index] = DataTypeExtensions.ToChar(value);
                    break;

                case TypeCode.Byte:
                    ((byte[])_data)[index] = DataTypeExtensions.ToByte(value);
                    break;

                case TypeCode.SByte:
                    ((sbyte[])_data)[index] = DataTypeExtensions.ToSByte(value);
                    break;

                case TypeCode.Int16:
                    ((short[])_data)[index] = DataTypeExtensions.ToShort(value);
                    break;

                case TypeCode.UInt16:
                    ((ushort[])_data)[index] = DataTypeExtensions.ToUShort(value);
                    break;

                case TypeCode.Int32:
                    ((int[])_data)[index] = DataTypeExtensions.ToInt(value);
                    break;

                case TypeCode.UInt32:
                    ((uint[])_data)[index] = DataTypeExtensions.ToUInt(value);
                    break;

                case TypeCode.Int64:
                    ((long[])_data)[index] = DataTypeExtensions.ToLong(value);
                    break;

                case TypeCode.UInt64:
                    ((ulong[])_data)[index] = DataTypeExtensions.ToULong(value);
                    break;

                case TypeCode.Decimal:
                    ((decimal[])_data)[index] = DataTypeExtensions.ToDecimal(value);
                    break;

                case TypeCode.Double:
                    ((double[])_data)[index] = DataTypeExtensions.ToDouble(value);
                    break;

                case TypeCode.Single:
                    ((float[])_data)[index] = DataTypeExtensions.ToFloat(value);
                    break;

                case TypeCode.DateTime:
                    ((DateTime[])_data)[index] = DataTypeExtensions.ToDateTime(value);
                    break;

                case TypeCode.String:
                    ((string[])_data)[index] = DataTypeExtensions.ToString(value);
                    break;

                case TypeCode.Object:
                    ((Array)_data).SetValue(value, index);
                    break;
            }
            return this;
        }

        #region IConvertible 成员

        public ulong ToUInt64(IFormatProvider provider)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToULong(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return DataTypeExtensions.ToULong(((char[])_data)[_cursor]);

                case TypeCode.Byte:
                    return DataTypeExtensions.ToULong(((byte[])_data)[_cursor]);

                case TypeCode.SByte:
                    return DataTypeExtensions.ToULong(((sbyte[])_data)[_cursor]);

                case TypeCode.Int16:
                    return DataTypeExtensions.ToULong(((short[])_data)[_cursor]);

                case TypeCode.UInt16:
                    return DataTypeExtensions.ToULong(((ushort[])_data)[_cursor]);

                case TypeCode.Int32:
                    return DataTypeExtensions.ToULong(((int[])_data)[_cursor]);

                case TypeCode.UInt32:
                    return DataTypeExtensions.ToULong(((uint[])_data)[_cursor]);

                case TypeCode.Int64:
                    return DataTypeExtensions.ToULong(((long[])_data)[_cursor]);

                case TypeCode.UInt64:
                    return ((ulong[])_data)[_cursor];

                case TypeCode.Decimal:
                    return DataTypeExtensions.ToULong(((decimal[])_data)[_cursor]);

                case TypeCode.Double:
                    return DataTypeExtensions.ToULong(((double[])_data)[_cursor]);

                case TypeCode.Single:
                    return DataTypeExtensions.ToULong(((float[])_data)[_cursor]);

                case TypeCode.DateTime:
                    return DataTypeExtensions.ToULong(((DateTime[])_data)[_cursor]);

                case TypeCode.String:
                    return DataTypeExtensions.ToULong(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToULong(((Array)_data).GetValue(_cursor));

                default:
                    return 0;
            }
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToSByte(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return DataTypeExtensions.ToSByte(((char[])_data)[_cursor]);

                case TypeCode.Byte:
                    return DataTypeExtensions.ToSByte(((byte[])_data)[_cursor]);

                case TypeCode.SByte:
                    return ((sbyte[])_data)[_cursor];

                case TypeCode.Int16:
                    return DataTypeExtensions.ToSByte(((short[])_data)[_cursor]);

                case TypeCode.UInt16:
                    return DataTypeExtensions.ToSByte(((ushort[])_data)[_cursor]);

                case TypeCode.Int32:
                    return DataTypeExtensions.ToSByte(((int[])_data)[_cursor]);

                case TypeCode.UInt32:
                    return DataTypeExtensions.ToSByte(((uint[])_data)[_cursor]);

                case TypeCode.Int64:
                    return DataTypeExtensions.ToSByte(((long[])_data)[_cursor]);

                case TypeCode.UInt64:
                    return DataTypeExtensions.ToSByte(((ulong[])_data)[_cursor]);

                case TypeCode.Decimal:
                    return DataTypeExtensions.ToSByte(((decimal[])_data)[_cursor]);

                case TypeCode.Double:
                    return DataTypeExtensions.ToSByte(((double[])_data)[_cursor]);

                case TypeCode.Single:
                    return DataTypeExtensions.ToSByte(((float[])_data)[_cursor]);

                case TypeCode.DateTime:
                    return DataTypeExtensions.ToSByte(((DateTime[])_data)[_cursor]);

                case TypeCode.String:
                    return DataTypeExtensions.ToSByte(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToSByte(((Array)_data).GetValue(_cursor));

                default:
                    return 0;
            }
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToDouble(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return DataTypeExtensions.ToDouble(((char[])_data)[_cursor]);

                case TypeCode.Byte:
                    return DataTypeExtensions.ToDouble(((byte[])_data)[_cursor]);

                case TypeCode.SByte:
                    return ((sbyte[])_data)[_cursor];

                case TypeCode.Int16:
                    return DataTypeExtensions.ToDouble(((short[])_data)[_cursor]);

                case TypeCode.UInt16:
                    return DataTypeExtensions.ToDouble(((ushort[])_data)[_cursor]);

                case TypeCode.Int32:
                    return DataTypeExtensions.ToDouble(((int[])_data)[_cursor]);

                case TypeCode.UInt32:
                    return DataTypeExtensions.ToDouble(((uint[])_data)[_cursor]);

                case TypeCode.Int64:
                    return DataTypeExtensions.ToDouble(((long[])_data)[_cursor]);

                case TypeCode.UInt64:
                    return DataTypeExtensions.ToDouble(((ulong[])_data)[_cursor]);

                case TypeCode.Decimal:
                    return DataTypeExtensions.ToDouble(((decimal[])_data)[_cursor]);

                case TypeCode.Double:
                    return DataTypeExtensions.ToDouble(((double[])_data)[_cursor]);

                case TypeCode.Single:
                    return DataTypeExtensions.ToDouble(((float[])_data)[_cursor]);

                case TypeCode.DateTime:
                    return DataTypeExtensions.ToDouble(((DateTime[])_data)[_cursor]);

                case TypeCode.String:
                    return DataTypeExtensions.ToDouble(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToDouble(((Array)_data).GetValue(_cursor));

                default:
                    return 0;
            }
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToDateTime(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return DataTypeExtensions.ToDateTime(((char[])_data)[_cursor]);

                case TypeCode.Byte:
                    return DataTypeExtensions.ToDateTime(((byte[])_data)[_cursor]);

                case TypeCode.SByte:
                    return DataTypeExtensions.ToDateTime(((sbyte[])_data)[_cursor]);

                case TypeCode.Int16:
                    return DataTypeExtensions.ToDateTime(((short[])_data)[_cursor]);

                case TypeCode.UInt16:
                    return DataTypeExtensions.ToDateTime(((ushort[])_data)[_cursor]);

                case TypeCode.Int32:
                    return DataTypeExtensions.ToDateTime(((int[])_data)[_cursor]);

                case TypeCode.UInt32:
                    return DataTypeExtensions.ToDateTime(((uint[])_data)[_cursor]);

                case TypeCode.Int64:
                    return DataTypeExtensions.ToDateTime(((long[])_data)[_cursor]);

                case TypeCode.UInt64:
                    return DataTypeExtensions.ToDateTime(((ulong[])_data)[_cursor]);

                case TypeCode.Decimal:
                    return DataTypeExtensions.ToDateTime(((decimal[])_data)[_cursor]);

                case TypeCode.Double:
                    return DataTypeExtensions.ToDateTime(((double[])_data)[_cursor]);

                case TypeCode.Single:
                    return DataTypeExtensions.ToDateTime(((float[])_data)[_cursor]);

                case TypeCode.DateTime:
                    return DataTypeExtensions.ToDateTime(((DateTime[])_data)[_cursor]);

                case TypeCode.String:
                    return DataTypeExtensions.ToDateTime(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToDateTime(((Array)_data).GetValue(_cursor));

                default:
                    return DateTime.MinValue;
            }
        }

        public float ToSingle(IFormatProvider provider)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToFloat(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return DataTypeExtensions.ToFloat(((char[])_data)[_cursor]);

                case TypeCode.Byte:
                    return DataTypeExtensions.ToFloat(((byte[])_data)[_cursor]);

                case TypeCode.SByte:
                    return DataTypeExtensions.ToFloat(((sbyte[])_data)[_cursor]);

                case TypeCode.Int16:
                    return DataTypeExtensions.ToFloat(((short[])_data)[_cursor]);

                case TypeCode.UInt16:
                    return DataTypeExtensions.ToFloat(((ushort[])_data)[_cursor]);

                case TypeCode.Int32:
                    return DataTypeExtensions.ToFloat(((int[])_data)[_cursor]);

                case TypeCode.UInt32:
                    return DataTypeExtensions.ToFloat(((uint[])_data)[_cursor]);

                case TypeCode.Int64:
                    return DataTypeExtensions.ToFloat(((long[])_data)[_cursor]);

                case TypeCode.UInt64:
                    return DataTypeExtensions.ToFloat(((ulong[])_data)[_cursor]);

                case TypeCode.Decimal:
                    return DataTypeExtensions.ToFloat(((decimal[])_data)[_cursor]);

                case TypeCode.Double:
                    return DataTypeExtensions.ToFloat(((double[])_data)[_cursor]);

                case TypeCode.Single:
                    return ((float[])_data)[_cursor];

                case TypeCode.DateTime:
                    return DataTypeExtensions.ToFloat(((DateTime[])_data)[_cursor]);

                case TypeCode.String:
                    return DataTypeExtensions.ToFloat(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToFloat(((Array)_data).GetValue(_cursor));

                default:
                    return 0;
            }
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return ((bool[])_data)[_cursor];

                case TypeCode.Char:
                    return DataTypeExtensions.ToBool(((char[])_data)[_cursor]);

                case TypeCode.Byte:
                    return DataTypeExtensions.ToBool(((byte[])_data)[_cursor]);

                case TypeCode.SByte:
                    return DataTypeExtensions.ToBool(((sbyte[])_data)[_cursor]);

                case TypeCode.Int16:
                    return DataTypeExtensions.ToBool(((short[])_data)[_cursor]);

                case TypeCode.UInt16:
                    return DataTypeExtensions.ToBool(((ushort[])_data)[_cursor]);

                case TypeCode.Int32:
                    return DataTypeExtensions.ToBool(((int[])_data)[_cursor]);

                case TypeCode.UInt32:
                    return DataTypeExtensions.ToBool(((uint[])_data)[_cursor]);

                case TypeCode.Int64:
                    return DataTypeExtensions.ToBool(((long[])_data)[_cursor]);

                case TypeCode.UInt64:
                    return DataTypeExtensions.ToBool(((ulong[])_data)[_cursor]);

                case TypeCode.Decimal:
                    return DataTypeExtensions.ToBool(((decimal[])_data)[_cursor]);

                case TypeCode.Double:
                    return DataTypeExtensions.ToBool(((double[])_data)[_cursor]);

                case TypeCode.Single:
                    return DataTypeExtensions.ToBool(((float[])_data)[_cursor]);

                case TypeCode.DateTime:
                    return DataTypeExtensions.ToBool(((DateTime[])_data)[_cursor]);

                case TypeCode.String:
                    return DataTypeExtensions.ToBool(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToBool(((Array)_data).GetValue(_cursor));

                default:
                    return false;
            }
        }

        public int ToInt32(IFormatProvider provider)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToInt(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return DataTypeExtensions.ToInt(((char[])_data)[_cursor]);

                case TypeCode.Byte:
                    return DataTypeExtensions.ToInt(((byte[])_data)[_cursor]);

                case TypeCode.SByte:
                    return DataTypeExtensions.ToInt(((sbyte[])_data)[_cursor]);

                case TypeCode.Int16:
                    return DataTypeExtensions.ToInt(((short[])_data)[_cursor]);

                case TypeCode.UInt16:
                    return DataTypeExtensions.ToInt(((ushort[])_data)[_cursor]);

                case TypeCode.Int32:
                    return ((int[])_data)[_cursor];

                case TypeCode.UInt32:
                    return DataTypeExtensions.ToInt(((uint[])_data)[_cursor]);

                case TypeCode.Int64:
                    return DataTypeExtensions.ToInt(((long[])_data)[_cursor]);

                case TypeCode.UInt64:
                    return DataTypeExtensions.ToInt(((ulong[])_data)[_cursor]);

                case TypeCode.Decimal:
                    return DataTypeExtensions.ToInt(((decimal[])_data)[_cursor]);

                case TypeCode.Double:
                    return DataTypeExtensions.ToInt(((double[])_data)[_cursor]);

                case TypeCode.Single:
                    return DataTypeExtensions.ToInt(((float[])_data)[_cursor]);

                case TypeCode.DateTime:
                    return DataTypeExtensions.ToInt(((DateTime[])_data)[_cursor]);

                case TypeCode.String:
                    return DataTypeExtensions.ToInt(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToInt(((Array)_data).GetValue(_cursor));

                default:
                    return 0;
            }
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToDateTime(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return DataTypeExtensions.ToDateTime(((char[])_data)[_cursor]);

                case TypeCode.Byte:
                    return DataTypeExtensions.ToDateTime(((byte[])_data)[_cursor]);

                case TypeCode.SByte:
                    return DataTypeExtensions.ToDateTime(((sbyte[])_data)[_cursor]);

                case TypeCode.Int16:
                    return DataTypeExtensions.ToDateTime(((short[])_data)[_cursor]);

                case TypeCode.UInt16:
                    return DataTypeExtensions.ToDateTime(((ushort[])_data)[_cursor]);

                case TypeCode.DateTime:
                    return ((DateTime[])_data)[_cursor];

                case TypeCode.UInt32:
                    return DataTypeExtensions.ToDateTime(((uint[])_data)[_cursor]);

                case TypeCode.Int64:
                    return DataTypeExtensions.ToDateTime(((long[])_data)[_cursor]);

                case TypeCode.UInt64:
                    return DataTypeExtensions.ToDateTime(((ulong[])_data)[_cursor]);

                case TypeCode.Decimal:
                    return DataTypeExtensions.ToDateTime(((decimal[])_data)[_cursor]);

                case TypeCode.Double:
                    return DataTypeExtensions.ToDateTime(((double[])_data)[_cursor]);

                case TypeCode.Single:
                    return DataTypeExtensions.ToDateTime(((float[])_data)[_cursor]);

                case TypeCode.Int32:
                    return DataTypeExtensions.ToDateTime(((int[])_data)[_cursor]);

                case TypeCode.String:
                    return DataTypeExtensions.ToDateTime(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToDateTime(((Array)_data).GetValue(_cursor));

                default:
                    return DateTime.MinValue;
            }
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToUShort(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return DataTypeExtensions.ToUShort(((char[])_data)[_cursor]);

                case TypeCode.Byte:
                    return DataTypeExtensions.ToUShort(((byte[])_data)[_cursor]);

                case TypeCode.SByte:
                    return DataTypeExtensions.ToUShort(((sbyte[])_data)[_cursor]);

                case TypeCode.Int16:
                    return DataTypeExtensions.ToUShort(((short[])_data)[_cursor]);

                case TypeCode.UInt16:
                    return ((ushort[])_data)[_cursor];

                case TypeCode.Int32:
                    return DataTypeExtensions.ToUShort(((int[])_data)[_cursor]);

                case TypeCode.UInt32:
                    return DataTypeExtensions.ToUShort(((uint[])_data)[_cursor]);

                case TypeCode.Int64:
                    return DataTypeExtensions.ToUShort(((long[])_data)[_cursor]);

                case TypeCode.UInt64:
                    return DataTypeExtensions.ToUShort(((ulong[])_data)[_cursor]);

                case TypeCode.Decimal:
                    return DataTypeExtensions.ToUShort(((decimal[])_data)[_cursor]);

                case TypeCode.Double:
                    return DataTypeExtensions.ToUShort(((double[])_data)[_cursor]);

                case TypeCode.Single:
                    return DataTypeExtensions.ToUShort(((float[])_data)[_cursor]);

                case TypeCode.DateTime:
                    return DataTypeExtensions.ToUShort(((DateTime[])_data)[_cursor]);

                case TypeCode.String:
                    return DataTypeExtensions.ToUShort(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToUShort(((Array)_data).GetValue(_cursor));

                default:
                    return 0;
            }
        }

        public short ToInt16(IFormatProvider provider)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToShort(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return DataTypeExtensions.ToShort(((char[])_data)[_cursor]);

                case TypeCode.Byte:
                    return DataTypeExtensions.ToShort(((byte[])_data)[_cursor]);

                case TypeCode.SByte:
                    return DataTypeExtensions.ToShort(((sbyte[])_data)[_cursor]);

                case TypeCode.Int16:
                    return ((short[])_data)[_cursor];

                case TypeCode.UInt16:
                    return DataTypeExtensions.ToShort(((ushort[])_data)[_cursor]);

                case TypeCode.Int32:
                    return DataTypeExtensions.ToShort(((int[])_data)[_cursor]);

                case TypeCode.UInt32:
                    return DataTypeExtensions.ToShort(((uint[])_data)[_cursor]);

                case TypeCode.Int64:
                    return DataTypeExtensions.ToShort(((long[])_data)[_cursor]);

                case TypeCode.UInt64:
                    return DataTypeExtensions.ToShort(((ulong[])_data)[_cursor]);

                case TypeCode.Decimal:
                    return DataTypeExtensions.ToShort(((decimal[])_data)[_cursor]);

                case TypeCode.Double:
                    return DataTypeExtensions.ToShort(((double[])_data)[_cursor]);

                case TypeCode.Single:
                    return DataTypeExtensions.ToShort(((float[])_data)[_cursor]);

                case TypeCode.DateTime:
                    return DataTypeExtensions.ToShort(((DateTime[])_data)[_cursor]);

                case TypeCode.String:
                    return DataTypeExtensions.ToShort(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToShort(((Array)_data).GetValue(_cursor));

                default:
                    return 0;
            }
        }

        public string ToString(IFormatProvider provider)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToString(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return ((char[])_data)[_cursor].ToString();

                case TypeCode.Byte:
                    return ((byte[])_data)[_cursor].ToString();

                case TypeCode.SByte:
                    return ((sbyte[])_data)[_cursor].ToString();

                case TypeCode.Int16:
                    return ((short[])_data)[_cursor].ToString();

                case TypeCode.UInt16:
                    return ((ushort[])_data)[_cursor].ToString();

                case TypeCode.Int32:
                    return ((int[])_data)[_cursor].ToString();

                case TypeCode.UInt32:
                    return ((uint[])_data)[_cursor].ToString();

                case TypeCode.Int64:
                    return ((long[])_data)[_cursor].ToString();

                case TypeCode.UInt64:
                    return ((ulong[])_data)[_cursor].ToString();

                case TypeCode.Decimal:
                    return ((decimal[])_data)[_cursor].ToString();

                case TypeCode.Double:
                    return ((double[])_data)[_cursor].ToString();

                case TypeCode.Single:
                    return ((float[])_data)[_cursor].ToString();

                case TypeCode.DateTime:
                    DateTime data = ((DateTime[])_data)[_cursor];
                    string value = data == DateTime.MinValue ? string.Empty : data.ToString("yyyy-MM-dd HH:mm:ss");
                    return value.StartsWith("1900-01-01") ? string.Empty : value;

                case TypeCode.String:
                    return DataTypeExtensions.ToString(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToString(((Array)_data).GetValue(_cursor));

                default:
                    return string.Empty;
            }
        }

        public byte ToByte(IFormatProvider provider)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToByte(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return DataTypeExtensions.ToByte(((char[])_data)[_cursor]);

                case TypeCode.Byte:
                    return ((byte[])_data)[_cursor];

                case TypeCode.SByte:
                    return DataTypeExtensions.ToByte(((sbyte[])_data)[_cursor]);

                case TypeCode.Int16:
                    return DataTypeExtensions.ToByte(((short[])_data)[_cursor]);

                case TypeCode.UInt16:
                    return DataTypeExtensions.ToByte(((ushort[])_data)[_cursor]);

                case TypeCode.Int32:
                    return DataTypeExtensions.ToByte(((int[])_data)[_cursor]);

                case TypeCode.UInt32:
                    return DataTypeExtensions.ToByte(((uint[])_data)[_cursor]);

                case TypeCode.Int64:
                    return DataTypeExtensions.ToByte(((long[])_data)[_cursor]);

                case TypeCode.UInt64:
                    return DataTypeExtensions.ToByte(((ulong[])_data)[_cursor]);

                case TypeCode.Decimal:
                    return DataTypeExtensions.ToByte(((decimal[])_data)[_cursor]);

                case TypeCode.Double:
                    return DataTypeExtensions.ToByte(((double[])_data)[_cursor]);

                case TypeCode.Single:
                    return DataTypeExtensions.ToByte(((float[])_data)[_cursor]);

                case TypeCode.DateTime:
                    return DataTypeExtensions.ToByte(((DateTime[])_data)[_cursor]);

                case TypeCode.String:
                    return DataTypeExtensions.ToByte(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToByte(((Array)_data).GetValue(_cursor));

                default:
                    return 0;
            }
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToChar(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return ((char[])_data)[_cursor];

                case TypeCode.Byte:
                    return DataTypeExtensions.ToChar(((byte[])_data)[_cursor]);

                case TypeCode.SByte:
                    return DataTypeExtensions.ToChar(((sbyte[])_data)[_cursor]);

                case TypeCode.Int16:
                    return DataTypeExtensions.ToChar(((short[])_data)[_cursor]);

                case TypeCode.UInt16:
                    return DataTypeExtensions.ToChar(((ushort[])_data)[_cursor]);

                case TypeCode.Int32:
                    return DataTypeExtensions.ToChar(((int[])_data)[_cursor]);

                case TypeCode.UInt32:
                    return DataTypeExtensions.ToChar(((uint[])_data)[_cursor]);

                case TypeCode.Int64:
                    return DataTypeExtensions.ToChar(((long[])_data)[_cursor]);

                case TypeCode.UInt64:
                    return DataTypeExtensions.ToChar(((ulong[])_data)[_cursor]);

                case TypeCode.Decimal:
                    return DataTypeExtensions.ToChar(((decimal[])_data)[_cursor]);

                case TypeCode.Double:
                    return DataTypeExtensions.ToChar(((double[])_data)[_cursor]);

                case TypeCode.Single:
                    return DataTypeExtensions.ToChar(((float[])_data)[_cursor]);

                case TypeCode.DateTime:
                    return DataTypeExtensions.ToChar(((DateTime[])_data)[_cursor]);

                case TypeCode.String:
                    return DataTypeExtensions.ToChar(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToChar(((Array)_data).GetValue(_cursor));

                default:
                    return '\0';
            }
        }

        public long ToInt64(IFormatProvider provider)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToLong(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return DataTypeExtensions.ToLong(((char[])_data)[_cursor]);

                case TypeCode.Byte:
                    return DataTypeExtensions.ToLong(((byte[])_data)[_cursor]);

                case TypeCode.SByte:
                    return DataTypeExtensions.ToLong(((sbyte[])_data)[_cursor]);

                case TypeCode.Int16:
                    return DataTypeExtensions.ToLong(((short[])_data)[_cursor]);

                case TypeCode.UInt16:
                    return DataTypeExtensions.ToLong(((ushort[])_data)[_cursor]);

                case TypeCode.Int32:
                    return DataTypeExtensions.ToLong(((int[])_data)[_cursor]);

                case TypeCode.UInt32:
                    return DataTypeExtensions.ToLong(((uint[])_data)[_cursor]);

                case TypeCode.Int64:
                    return ((long[])_data)[_cursor];

                case TypeCode.UInt64:
                    return DataTypeExtensions.ToLong(((ulong[])_data)[_cursor]);

                case TypeCode.Decimal:
                    return DataTypeExtensions.ToLong(((decimal[])_data)[_cursor]);

                case TypeCode.Double:
                    return DataTypeExtensions.ToLong(((double[])_data)[_cursor]);

                case TypeCode.Single:
                    return DataTypeExtensions.ToLong(((float[])_data)[_cursor]);

                case TypeCode.DateTime:
                    return DataTypeExtensions.ToLong(((DateTime[])_data)[_cursor]);

                case TypeCode.String:
                    return DataTypeExtensions.ToLong(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToLong(((Array)_data).GetValue(_cursor));

                default:
                    return 0;
            }
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToDecimal(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return DataTypeExtensions.ToDecimal(((char[])_data)[_cursor]);

                case TypeCode.Byte:
                    return DataTypeExtensions.ToDecimal(((byte[])_data)[_cursor]);

                case TypeCode.SByte:
                    return DataTypeExtensions.ToDecimal(((sbyte[])_data)[_cursor]);

                case TypeCode.Int16:
                    return DataTypeExtensions.ToDecimal(((short[])_data)[_cursor]);

                case TypeCode.UInt16:
                    return DataTypeExtensions.ToDecimal(((ushort[])_data)[_cursor]);

                case TypeCode.Int32:
                    return DataTypeExtensions.ToDecimal(((int[])_data)[_cursor]);

                case TypeCode.UInt32:
                    return DataTypeExtensions.ToDecimal(((uint[])_data)[_cursor]);

                case TypeCode.Int64:
                    return DataTypeExtensions.ToDecimal(((long[])_data)[_cursor]);

                case TypeCode.UInt64:
                    return DataTypeExtensions.ToDecimal(((ulong[])_data)[_cursor]);

                case TypeCode.Decimal:
                    return ((decimal[])_data)[_cursor];

                case TypeCode.Double:
                    return DataTypeExtensions.ToDecimal(((double[])_data)[_cursor]);

                case TypeCode.Single:
                    return DataTypeExtensions.ToDecimal(((float[])_data)[_cursor]);

                case TypeCode.DateTime:
                    return DataTypeExtensions.ToDecimal(((DateTime[])_data)[_cursor]);

                case TypeCode.String:
                    return DataTypeExtensions.ToDecimal(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToDecimal(((Array)_data).GetValue(_cursor));

                default:
                    return 0;
            }
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.Boolean:
                    return DataTypeExtensions.ToUInt(((bool[])_data)[_cursor]);

                case TypeCode.Char:
                    return DataTypeExtensions.ToUInt(((char[])_data)[_cursor]);

                case TypeCode.Byte:
                    return DataTypeExtensions.ToUInt(((byte[])_data)[_cursor]);

                case TypeCode.SByte:
                    return DataTypeExtensions.ToUInt(((sbyte[])_data)[_cursor]);

                case TypeCode.Int16:
                    return DataTypeExtensions.ToUInt(((short[])_data)[_cursor]);

                case TypeCode.UInt16:
                    return DataTypeExtensions.ToUInt(((ushort[])_data)[_cursor]);

                case TypeCode.Int32:
                    return DataTypeExtensions.ToUInt(((int[])_data)[_cursor]);

                case TypeCode.UInt32:
                    return ((uint[])_data)[_cursor];

                case TypeCode.Int64:
                    return DataTypeExtensions.ToUInt(((long[])_data)[_cursor]);

                case TypeCode.UInt64:
                    return DataTypeExtensions.ToUInt(((ulong[])_data)[_cursor]);

                case TypeCode.Decimal:
                    return DataTypeExtensions.ToUInt(((decimal[])_data)[_cursor]);

                case TypeCode.Double:
                    return DataTypeExtensions.ToUInt(((double[])_data)[_cursor]);

                case TypeCode.Single:
                    return DataTypeExtensions.ToUInt(((float[])_data)[_cursor]);

                case TypeCode.DateTime:
                    return DataTypeExtensions.ToUInt(((DateTime[])_data)[_cursor]);

                case TypeCode.String:
                    return DataTypeExtensions.ToUInt(((string[])_data)[_cursor]);

                case TypeCode.Object:
                    return DataTypeExtensions.ToUInt(((Array)_data).GetValue(_cursor));

                default:
                    return 0;
            }
        }

        public object ToType(Type conversionType)
        {
            return ToType(conversionType, null);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return DataTypeExtensions.Convert(((Array)_data).GetValue(_cursor), conversionType);
        }

        public TypeCode GetTypeCode()
        {
            return Type.GetTypeCode((Type)_type);
        }

        #endregion IConvertible 成员

        public DataArrayColumn Replace(string old, string value)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.String:
                    string[] data = (string[])_data;
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        data[i] = data[i].Replace(old, value);
                    }
                    break;
            }
            return this;
        }

        public DataArrayColumn ReplaceReg(string regStr, string value)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.String:
                    string[] data = (string[])_data;
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        data[i] = Regex.Replace(data[i], regStr, value, RegexOptions.IgnoreCase);
                    }
                    break;
            }
            return this;
        }

        public DataArrayColumn ReplaceReg(string regStr, MatchEvaluator func)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.String:
                    string[] data = (string[])_data;
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        data[i] = Regex.Replace(data[i], regStr, func, RegexOptions.IgnoreCase);
                    }
                    break;
            }
            return this;
        }

        public DataArrayColumn HighLight(string value)
        {
            switch (Type.GetTypeCode((Type)_type))
            {
                case TypeCode.String:
                    string[] data = (string[])_data;
                    for (int i = 0; i < _columns.DataArray.Count; i++)
                    {
                        data[i] = Regex.Replace(data[i], "(" + value + ")", "<B><font color='red'>$1</font></B>", RegexOptions.IgnoreCase);
                    }
                    break;
            }
            return this;
        }

        public DataArrayColumn Replace(Func<object, object> tran)
        {
            for (int i = 0; i < _columns.DataArray.Count; i++)
            {
                ((Array)_data).SetValue(tran(((Array)_data).GetValue(i)), i);
            }
            return this;
        }

        public DataArrayColumn Replace(Func<DataArray, object> tran)
        {
            while (_columns.DataArray.Read())
            {
                ((Array)_data).SetValue(tran(_columns.DataArray), _columns.DataArray.Cursor);
            }
            _columns.DataArray.Cursor = 0;
            return this;
        }
    }

    [Serializable]
    public class DataArrayColumns : IEnumerable, IEnumerator, ICloneable
    {
        private TreeName _tree = new TreeName();

        public DataArrayColumns(DataArray data)
        {
            DataArray = data;
        }

        public DataArray DataArray { get; }

        public int Count
        {
            get { return _tree.Count; }
        }

        public DataArrayColumn Add(string name)
        {
            return Add(name, typeof(string));
        }

        public DataArrayColumns Add(params string[] names)
        {
            names.ForEach(name => { Add(name); });
            return this;
        }

        public DataArrayColumn Add(string name, Type type)
        {
            TreeNameNode node = _tree.Add(new TreeNameNode(name, new DataArrayColumn(this, name, type)));
            if (node == null) return DataArray[name];
            return (DataArrayColumn)node.Data;
        }

        public DataArrayColumn Add(DataArrayColumn col)
        {
            TreeNameNode node = _tree.Add(new TreeNameNode(col.Name, col));
            if (node == null) return DataArray[col.Name];
            return col;
        }

        public DataArrayColumns Add(params DataArrayColumn[] cols)
        {
            cols.ForEach(col => { Add(col); });
            return this;
        }

        public DataArrayColumns Add(Type type)
        {
            FieldInfo[] list = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo t in list)
            {
                Add(t.Name.ToLower(), t.FieldType);
            }
            return this;
        }

        public DataArrayColumns Add(params Type[] types)
        {
            types.ForEach(type => { Add(type); });
            return this;
        }

        public bool Contains(string name)
        {
            return this[name] != null ? true : false;
        }

        public DataArrayColumn this[string name]
        {
            get
            {
                TreeNameNode node = _tree[name];
                if (node == null) return null;
                DataArrayColumn col = (DataArrayColumn)node.Data;
                col._cursor = DataArray.Cursor;
                return col;
            }
        }

        public DataArrayColumn this[string name, int row]
        {
            get
            {
                TreeNameNode node = _tree[name];
                if (node == null) return null;
                DataArrayColumn col = (DataArrayColumn)node.Data;
                col._cursor = row > DataArray.Count - 1 ? DataArray.Count - 1 : row;
                return (DataArrayColumn)node.Data;
            }
        }

        public DataArrayColumn Delete(string name)
        {
            TreeNameNode node = _tree.Delete(name);
            if (node == null) return null;
            return (DataArrayColumn)node.Data;
        }

        public DataArrayColumn Rename(string old, string name)
        {
            TreeNameNode node = _tree.Rename(old, name);
            if (node == null) throw new ApplicationException("从" + old + "改名" + name + "没有成功！");
            return (DataArrayColumn)node.Data;
        }

        public string[] List
        {
            get
            {
                string[] list = new string[_tree.Count];

                _tree.Reset();

                int index = 0;

                while (_tree.MoveNext())
                {
                    list[index] = ((DataArrayColumn)((TreeNameNode)_tree.Current).Data).Name;
                    index++;
                }
                return list;
            }
        }

        #region ICloneable 成员

        public object Clone()
        {
            DataArrayColumns cols = new DataArrayColumns(DataArray);
            cols._tree = (TreeName)_tree.Clone();
            return cols;
        }

        #endregion ICloneable 成员

        #region IEnumerator 成员

        public void Reset()
        {
            _tree.Reset();
        }

        public object Current
        {
            get
            {
                TreeNameNode node = (TreeNameNode)_tree.Current;
                if (node == null) return null;
                DataArrayColumn col = (DataArrayColumn)node.Data;
                col._cursor = DataArray.Cursor;
                return col;
            }
        }

        public bool MoveNext()
        {
            return _tree.MoveNext();
        }

        #endregion IEnumerator 成员

        #region IEnumerable 成员

        public IEnumerator GetEnumerator()
        {
            Reset();
            return this;
        }

        #endregion IEnumerable 成员
    }
}