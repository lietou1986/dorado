using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Dorado.Utils
{
    public class SerializeUtility
    {
        #region Xml Serialize

        public static string SerializeIt(object ToBeSerialized)
        {
            XmlSerializer serializer = new XmlSerializer(ToBeSerialized.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, ToBeSerialized);
            byte[] storeit = stream.ToArray();
            string result = null;
            result = System.Text.Encoding.UTF8.GetString(storeit);
            stream.Close();
            stream.Dispose();
            return result;
        }

        public static T DeserializeIt<T>(string ToBeDeserialized, Type t)
        {
            XmlSerializer serializer = new XmlSerializer(t);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(ToBeDeserialized);
            MemoryStream stream = new MemoryStream(buffer);
            return (T)serializer.Deserialize(stream);
        }

        public static T DeserializeXml<T>(string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(xmlString);
            MemoryStream stream = new MemoryStream(buffer);

            return (T)serializer.Deserialize(stream);
        }

        #endregion Xml Serialize

        #region Json Serialize

        public static string SerializeJson(object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        public static T DeserializeJson<T>(string jsonString)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)serializer.ReadObject(memoryStream);
            memoryStream.Close();

            return obj;
        }

        #endregion Json Serialize
    }

    [XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public SerializableDictionary()
        {
        }

        public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }

        public SerializableDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        public SerializableDictionary(int capacity)
            : base(capacity)
        {
        }

        public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
        }

        protected SerializableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            if (reader.IsEmptyElement || !reader.Read())
            {
                return;
            }
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadEndElement();
                base.Add(key, value);
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            foreach (TKey key in base.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValue value = base[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
    }
}