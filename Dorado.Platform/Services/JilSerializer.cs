using Dorado.Services;
using Jil;
using System;

namespace Dorado.Platform.Services
{
    /// <summary>
    /// An implementation of <see cref="IJsonSerializer"/> using the Jil library
    /// </summary>
    public class JilSerializer : IJsonSerializer
    {
        public string Serialize(object o)
        {
            return Serialize(o, JsonFormat.None);
        }

        public string Serialize(object o, JsonFormat format)
        {
            return JSON.Serialize(o, new Options(format == JsonFormat.Indented));
        }

        public dynamic Deserialize(string json)
        {
            dynamic result = JSON.DeserializeDynamic(json);
            return result;
        }

        public object Deserialize(string json, Type type)
        {
            return JSON.Deserialize(json, type);
        }

        public T Deserialize<T>(string json)
        {
            return JSON.Deserialize<T>(json);
        }
    }
}