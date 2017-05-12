using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Dorado.VWS.Utils
{
    public class XmlSerializerHelper
    {
        /// <summary>
        /// XML序列化成字符串
        /// </summary>
        public static string XmlSerializer<T>(T t)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            string xmlstring = string.Empty;
            using (MemoryStream ms = new MemoryStream())
            {
                ser.Serialize(ms, t);
                xmlstring = Encoding.UTF8.GetString(ms.ToArray());
            }
            return xmlstring;
        }

        /// <summary>
        /// XML序列化到文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="filepath"></param>
        public static void XmlSerializer<T>(T t, string filepath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (Stream str = new FileStream(filepath, FileMode.Create))
            {
                ser.Serialize(str, t);
            }
        }

        /// <summary>
        /// xml反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlstring"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string xmlstring)
        {
            if (string.IsNullOrEmpty(xmlstring)) return default(T);
            XmlSerializer ser = new XmlSerializer(typeof(T));
            T obj;
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlstring)))
            {
                obj = (T)ser.Deserialize(ms);
            }
            return obj;
        }
    }
}