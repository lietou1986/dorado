using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Dorado.Data
{
    internal class DataRecord : IRecordSet, IDisposable, IRecord
    {
        private IDataReader wr;

        public int FieldCount
        {
            get
            {
                return this.wr.FieldCount;
            }
        }

        public object this[string name]
        {
            get
            {
                return this.wr[name];
            }
        }

        public object this[int i]
        {
            get
            {
                return this.wr[i];
            }
        }

        public int Depth
        {
            get
            {
                return this.wr.Depth;
            }
        }

        public bool IsClosed
        {
            get
            {
                return this.wr.IsClosed;
            }
        }

        public int RecordsAffected
        {
            get
            {
                return this.wr.RecordsAffected;
            }
        }

        internal DataRecord(IDataReader wrappedReader)
        {
            this.wr = wrappedReader;
        }

        public bool GetBoolean(int i)
        {
            return this.wr.GetBoolean(i);
        }

        public bool GetBooleanFromByte(int i)
        {
            return this.wr.GetByte(i) == 1;
        }

        public byte GetByte(int i)
        {
            return this.wr.GetByte(i);
        }

        public byte GetByteOrDefault(int i, byte defaultValue)
        {
            if (!this.IsDBNull(i))
            {
                return this.wr.GetByte(i);
            }
            return defaultValue;
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return this.wr.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return this.wr.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return this.wr.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public IDataReader GetData(int i)
        {
            return this.wr.GetData(i);
        }

        public string GetDataTypeName(int i)
        {
            return this.wr.GetDataTypeName(i);
        }

        public DateTime GetDateTime(int i)
        {
            return this.wr.GetDateTime(i);
        }

        public decimal GetDecimal(int i)
        {
            return this.wr.GetDecimal(i);
        }

        public double GetDouble(int i)
        {
            return this.wr.GetDouble(i);
        }

        public Type GetFieldType(int i)
        {
            return this.wr.GetFieldType(i);
        }

        public float GetFloat(int i)
        {
            return this.wr.GetFloat(i);
        }

        public Guid GetGuid(int i)
        {
            return this.wr.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return this.wr.GetInt16(i);
        }

        public int GetInt32FromByte(int i)
        {
            return Convert.ToInt32(this.wr.GetByte(i));
        }

        public int GetInt32(int i)
        {
            return this.wr.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return this.wr.GetInt64(i);
        }

        public string GetName(int i)
        {
            return this.wr.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return this.wr.GetOrdinal(name);
        }

        public string GetString(int i)
        {
            return this.wr.GetString(i);
        }

        public object GetValue(int i)
        {
            return this.wr.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return this.wr.GetValues(values);
        }

        public bool IsDBNull(int i)
        {
            return this.wr.IsDBNull(i);
        }

        public void Close()
        {
            this.wr.Close();
        }

        public DataTable GetSchemaTable()
        {
            return this.wr.GetSchemaTable();
        }

        public bool NextResult()
        {
            return this.wr.NextResult();
        }

        public bool Read()
        {
            return this.wr.Read();
        }

        public void Dispose()
        {
            this.wr.Dispose();
        }

        public string GetStringOrDefault(int i, string defaultValue)
        {
            if (!this.IsDBNull(i))
            {
                return this.GetString(i);
            }
            return defaultValue;
        }

        public string GetStringOrEmpty(int i)
        {
            return this.GetStringOrDefault(i, string.Empty);
        }

        public string GetStringOrNull(int i)
        {
            if (!this.IsDBNull(i))
            {
                return this.GetString(i);
            }
            return null;
        }

        public string GetStringOrNullFromCompressedByteArray(int fieldIndex)
        {
            return null;
        }

        public short? GetInt16Nullable(int fieldIndex)
        {
            if (this.IsDBNull(fieldIndex))
            {
                return null;
            }
            return new short?(this.GetInt16(fieldIndex));
        }

        public int? GetInt32Nullable(int fieldIndex)
        {
            if (this.IsDBNull(fieldIndex))
            {
                return null;
            }
            return new int?(this.GetInt32(fieldIndex));
        }

        public int GetInt32OrDefault(int fieldIndex, int defaultValue)
        {
            if (!this.IsDBNull(fieldIndex))
            {
                return this.GetInt32(fieldIndex);
            }
            return defaultValue;
        }

        public int GetInt32OrEmpty(int fieldIndex)
        {
            return this.GetInt32OrDefault(fieldIndex, -2147483648);
        }

        public int GetInt32OrDefaultFromByte(int fieldIndex, int defaultValue)
        {
            if (!this.IsDBNull(fieldIndex))
            {
                return this.GetInt32FromByte(fieldIndex);
            }
            return defaultValue;
        }

        public int GetInt32OrEmptyFromByte(int fieldIndex)
        {
            return this.GetInt32OrDefaultFromByte(fieldIndex, -2147483648);
        }

        public short GetInt16OrDefault(int fieldIndex, short defaultValue)
        {
            if (!this.IsDBNull(fieldIndex))
            {
                return this.GetInt16(fieldIndex);
            }
            return defaultValue;
        }

        public byte[] GetByteArrayOrNull(int fieldIndex)
        {
            if (this.IsDBNull(fieldIndex))
            {
                return null;
            }
            byte[] bytes = new byte[(int)((object)((IntPtr)this.GetBytes(fieldIndex, 0L, null, 0, 2147483647)))];
            this.GetBytes(fieldIndex, 0L, bytes, 0, bytes.Length);
            return bytes;
        }

        public DateTime GetDateTimeOrEmpty(int fieldIndex)
        {
            if (!this.IsDBNull(fieldIndex))
            {
                return this.GetDateTime(fieldIndex);
            }
            return DateTime.MinValue;
        }

        public SqlXml GetSqlXml(int fieldIndex)
        {
            if (this.IsDBNull(fieldIndex))
            {
                return null;
            }
            SqlDataReader sdr = this.wr as SqlDataReader;
            if (sdr == null)
            {
                return null;
            }
            return sdr.GetSqlXml(fieldIndex);
        }

        public T Get<T>(int i)
        {
            if (typeof(string) != typeof(T))
            {
                return (T)this[i];
            }
            object defaultvalue = "";
            if (!this.IsDBNull(i))
            {
                return (T)this[i];
            }
            return (T)defaultvalue;
        }

        public T Get<T>(string fieldName)
        {
            if (typeof(string) != typeof(T))
            {
                return (T)this[fieldName];
            }
            object defaultvalue = "";
            if (!this.IsDBNull(this.GetOrdinal(fieldName)))
            {
                return (T)this[fieldName];
            }
            return (T)defaultvalue;
        }

        public T GetOrDefault<T>(int i, T defaultValue)
        {
            if (!this.IsDBNull(i))
            {
                return this.Get<T>(i);
            }
            return defaultValue;
        }

        public T GetOrDefault<T>(string fieldName, T defaultValue)
        {
            if (!this.IsDBNull(this.GetOrdinal(fieldName)))
            {
                return this.Get<T>(fieldName);
            }
            return defaultValue;
        }

        public T GetOrNull<T>(int fieldIndex) where T : class
        {
            if (!this.IsDBNull(fieldIndex))
            {
                return this.Get<T>(fieldIndex);
            }
            return default(T);
        }

        public T GetOrNull<T>(string fieldName) where T : class
        {
            if (!this.IsDBNull(this.GetOrdinal(fieldName)))
            {
                return this.Get<T>(fieldName);
            }
            return default(T);
        }

        public bool GetBooleanOrFalse(int fieldIndex)
        {
            return !this.IsDBNull(fieldIndex) && this.GetBoolean(fieldIndex);
        }
    }
}