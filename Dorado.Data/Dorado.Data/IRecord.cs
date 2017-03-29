using System;
using System.Data;
using System.Data.SqlTypes;

namespace Dorado.Data
{
    public interface IRecord
    {
        int FieldCount
        {
            get;
        }

        object this[int i]
        {
            get;
        }

        object this[string name]
        {
            get;
        }

        T Get<T>(int i);

        T GetOrDefault<T>(int i, T defaultValue);

        T Get<T>(string fieldName);

        T GetOrDefault<T>(string fieldName, T defaultValue);

        T GetOrNull<T>(int fieldIndex) where T : class;

        T GetOrNull<T>(string fieldName) where T : class;

        string GetStringOrEmpty(int i);

        string GetStringOrDefault(int i, string defaultString);

        string GetStringOrNull(int fieldIndex);

        string GetStringOrNullFromCompressedByteArray(int fieldIndex);

        int GetInt32OrDefault(int fieldIndex, int defaultValue);

        int GetInt32OrEmpty(int fieldIndex);

        int GetInt32OrDefaultFromByte(int fieldIndex, int defaultValue);

        int GetInt32OrEmptyFromByte(int fieldIndex);

        byte[] GetByteArrayOrNull(int fieldIndex);

        short GetInt16OrDefault(int fieldIndex, short defaultValue);

        DateTime GetDateTimeOrEmpty(int fieldIndex);

        SqlXml GetSqlXml(int fieldIndex);

        bool GetBoolean(int i);

        bool GetBooleanFromByte(int i);

        bool GetBooleanOrFalse(int i);

        byte GetByte(int i);

        byte GetByteOrDefault(int i, byte defaultValue);

        long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length);

        char GetChar(int i);

        long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length);

        IDataReader GetData(int i);

        string GetDataTypeName(int i);

        DateTime GetDateTime(int i);

        decimal GetDecimal(int i);

        double GetDouble(int i);

        Type GetFieldType(int i);

        float GetFloat(int i);

        Guid GetGuid(int i);

        short GetInt16(int i);

        int GetInt32(int i);

        int? GetInt32Nullable(int i);

        short? GetInt16Nullable(int i);

        long GetInt64(int i);

        string GetName(int i);

        int GetOrdinal(string name);

        string GetString(int i);

        object GetValue(int i);

        int GetValues(object[] values);

        bool IsDBNull(int i);
    }
}