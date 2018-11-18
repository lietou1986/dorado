using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Dorado.Core
{
    public class XmlSerializerWrapper<T> where T : new()
    {
        /// <summary>
        /// 配置对象转换为字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isOnlyInnerContent"></param>
        /// <returns></returns>
        public static string Export(T obj, bool isOnlyInnerContent = false)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer ser = new XmlSerializer(typeof(T), new Type[]{});
                ser.Serialize(stream, obj);
                string xml = Encoding.UTF8.GetString(stream.GetBuffer());
                xml = xml.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (isOnlyInnerContent)
                    return xml.Replace("<?xml version=\"1.0\"?>", string.Empty).Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", string.Empty).Replace(" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", string.Empty);
                return xml;
            }
        }

        /// <summary>
        /// 字符串转为配置对象
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T Import(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                throw new ArgumentNullException("无效的xml字符串");

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                XmlSerializer ser = new XmlSerializer(typeof(T), new Type[]
                    {
                    });
                return (T)ser.Deserialize(stream);
            }
        }
    }
}