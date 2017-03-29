using Dorado.Core;
using Dorado.Core.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Dorado.ESB.Json
{
    public class JsonObjectSerializer : XmlObjectSerializer
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

        private JsonObjectSerializer()
        {
            serializer = GetJsonSerializer();
        }

        private JsonSerializer GetJsonSerializer()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(GetDateTimeConverter());
            serializer.Converters.Add(new NameValueDictionaryConverter());
            serializer.NullValueHandling = NullValueHandling.Include;
            return serializer;
        }

        private JsonConverter GetDateTimeConverter()
        {
            IsoDateTimeConverter dateTimeConverter = new IsoDateTimeConverter();
            dateTimeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            return dateTimeConverter;
        }

        #endregion ctor

        #region XmlObjectSerializer Members

        public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
        {
        }

        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
        {
            writer.WriteRaw(Serialize(graph));
        }

        public override void WriteEndObject(XmlDictionaryWriter writer)
        {
        }

        public override void WriteObject(XmlDictionaryWriter writer, object graph)
        {
            try
            {
                WriteStartObject(writer, graph);
                WriteObjectContent(writer, graph);
                WriteEndObject(writer);
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("JsonObjectSerializer", ex);
            }
        }

        #endregion XmlObjectSerializer Members

        #region Json serialize / deserialize

        public string Serialize(object obj)
        {
            string result;

            // work around for "help"
            if (obj is MemoryStream)
            {
                MemoryStream ms = obj as MemoryStream;
                result = Encoding.UTF8.GetString(ms.ToArray());
                return result;
            }

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                result = writer.ToString();
                writer.Close();
            }

            return result;
        }

        public void Serialize(Newtonsoft.Json.JsonWriter writer, Object obj)
        {
            serializer.Serialize(writer, obj);
        }

        public object Deserialize(string jsonString, Type objectType)
        {
            if (jsonString == null) throw new ArgumentNullException(jsonString);

            object result = null;
            try
            {
                using (StringReader reader = new StringReader(jsonString))
                {
                    result = serializer.Deserialize(reader, objectType);
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("JsonObjectSerializer", ex);
            }

            return result;
        }

        public object Deserialize(JsonReader reader, Type objectType)
        {
            return serializer.Deserialize(reader, objectType);
        }

        #endregion Json serialize / deserialize

        #region not supported XmlObjectSerializer members

        public override bool IsStartObject(XmlDictionaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
        {
            throw new NotImplementedException();
        }

        #endregion not supported XmlObjectSerializer members
    }
}