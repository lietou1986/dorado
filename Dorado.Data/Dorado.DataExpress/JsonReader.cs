using System.Runtime.Serialization.Json;
using System.Xml;

namespace Dorado.DataExpress
{
    internal class JsonReader
    {
        public static T Read<T>(string json)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            XmlReader reader = XmlReader.Create(json);
            return (T)ser.ReadObject(reader);
        }
    }
}