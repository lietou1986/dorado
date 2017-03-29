using Dorado.Core;
using Dorado.Core.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Text;

namespace Dorado.ESB.Communication
{
    public class JsonObjectSerializer : IObjectSerializer
    {
        #region fields

        private JsonSerializer serializer;

        #endregion fields

        #region singleton

        private static JsonObjectSerializer instance = new JsonObjectSerializer();

        public static JsonObjectSerializer Instance
        {
            get { return instance; }
        }

        #endregion singleton

        #region ctor

        public JsonObjectSerializer()
        {
            serializer = new JsonSerializer();
            serializer.Converters.Add(GetDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Include;
        }

        private JsonConverter GetDateTimeConverter()
        {
            IsoDateTimeConverter converter = new IsoDateTimeConverter();
            converter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            return converter;
        }

        #endregion ctor

        #region IObjectSerializer Members

        public byte[] Serialize(object instance)
        {
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, instance);
                return Encoding.UTF8.GetBytes(writer.ToString());
            }
        }

        public object Deserialize(byte[] data, Type objectType)
        {
            if (data == null) throw new ArgumentNullException("data");
            try
            {
                string json = Encoding.UTF8.GetString(data);
                using (StringReader reader = new StringReader(json))
                {
                    return serializer.Deserialize(reader, objectType);
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("JsonObjectSerializer", ex);
                return null;
            }
        }

        #endregion IObjectSerializer Members

        public object Deserialize(JsonReader reader, Type objectType)
        {
            return serializer.Deserialize(reader, objectType);
        }
    }
}