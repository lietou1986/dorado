using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dorado.ESB.Json
{
    public class NameValueDictionaryConverter : JsonConverter
    {
        #region fields

        private static Type defType = typeof(NameValueDictionary<string>).GetGenericTypeDefinition();

        #endregion fields

        #region JsonConverter Members

        public override bool CanConvert(Type objectType)
        {
            return (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == defType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JsonTextReader jsonReader = reader as JsonTextReader;

            if (jsonReader.TokenType == JsonToken.Null)
                return null;

            if (jsonReader.TokenType != JsonToken.StartObject)
                throw new FormatException("Should start with '{', line:" + jsonReader.LineNumber + ", position:" + jsonReader.LinePosition);

            var nvd = Activator.CreateInstance(objectType) as System.Collections.IDictionary;
            Type prmType = GetGenericArgumentType(objectType);
            while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndObject)
            {
                if (jsonReader.TokenType != JsonToken.PropertyName)
                    throw new FormatException("Should be PropertyName, line:" + jsonReader.LineNumber + ", position:" + jsonReader.LinePosition);

                string propName = (string)jsonReader.Value;
                object propval = JsonObjectSerializer.Instance.Deserialize(jsonReader, prmType);
                nvd.Add(propName, propval);
            }
            return nvd;
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteStartObject();
                var dict = value as System.Collections.IDictionary;
                foreach (System.Collections.DictionaryEntry de in dict)
                {
                    writer.WritePropertyName((string)de.Key);
                    JsonObjectSerializer.Instance.Serialize(writer, de.Value);
                }
                writer.WriteEndObject();
            }
        }

        #endregion JsonConverter Members

        #region helper

        private static Type GetGenericArgumentType(Type objectType)
        {
            return objectType.GetGenericArguments()[0];
        }

        #endregion helper
    }

    public class NameValueDictionary<T> : Dictionary<string, T>
    {
        public NameValueDictionary()
            : base()
        {
        }

        public NameValueDictionary(int count)
            : base(count)
        {
        }

        public NameValueDictionary(IDictionary<string, T> dictionary)
            : base(dictionary)
        {
        }
    }
}