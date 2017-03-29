using System.Data;

namespace Dorado.Data
{
    public interface IParameterSet
    {
        void AddWithValue(string key, object value);

        void AddWithValue(string key, object value, bool check);

        void AddWithValue(string key, object value, ParameterDirectionWrap direction);

        void AddWithValue(string key, object value, ParameterDirectionWrap direction, int? size);

        void AddTypedDbNull(string key, ParameterDirectionWrap direction, DbType dbType);

        object GetValue(string key);
    }
}