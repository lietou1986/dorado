using Newtonsoft.Json;
using System;
using System.Text;

namespace Dorado.Platform.Redis
{
    public class JsonNetSerializer : ISerializer
    {
        protected readonly JsonSerializerSettings Settings;

        public JsonNetSerializer(JsonSerializerSettings settings = null)
        {
            Settings = settings ?? new JsonSerializerSettings();
        }

        public object Deserialize(byte[] value, Type objectType)
        {
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(value), objectType, Settings);
        }

        public byte[] Serialize(object value)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value, Settings));
        }
    }
}