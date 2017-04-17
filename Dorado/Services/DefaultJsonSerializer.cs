using System;
using System.Web.Script.Serialization;

namespace Dorado.Services
{
    /// <summary>
    /// An implementation of <see cref="IJsonSerializer"/> using the Newtonsoft.Json library
    /// </summary>
    public class DefaultJsonSerializer : IJsonSerializer
    {
        public dynamic Deserialize(string json)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.DeserializeObject(json);
        }

        public object Deserialize(string json, Type type)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.DeserializeObject(json);
        }

        public T Deserialize<T>(string json)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<T>(json);
        }

        public string Serialize(object o)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(o);
        }

        public string Serialize(object o, JsonFormat format)
        {
            return Serialize(o);
        }
    }
}