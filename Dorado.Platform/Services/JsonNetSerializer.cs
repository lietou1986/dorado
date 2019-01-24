using Dorado.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Dorado.Platform.Services
{
    /// <summary>
    /// An implementation of <see cref="IJsonSerializer"/> using the Newtonsoft.Json library
    /// </summary>
    public class JsonNetSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public JsonNetSerializer()
        {
            _settings = new JsonSerializerSettings
            {
                //ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public string Serialize(object o)
        {
            return Serialize(o, JsonFormat.None);
        }

        public string Serialize(object o, JsonFormat format)
        {
            return JsonConvert.SerializeObject(o, format == JsonFormat.Indented ? Formatting.Indented : Formatting.None, _settings);
        }

        public dynamic Deserialize(string json)
        {
            dynamic result = JObject.Parse(json);
            return result;
        }

        public object Deserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, _settings);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }
    }
}