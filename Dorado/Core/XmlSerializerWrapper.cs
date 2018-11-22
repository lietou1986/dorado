using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Dorado.Core
{
    public class XmlSerializerWrapper<T> where T : new()
    {
        /// <summary>
        /// 对象转换为XML字符串，并移除BOM头
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="indentRequired"></param>
        /// <param name="headRequired"></param>
        /// <returns></returns>
        public static string Export(T obj, bool indentRequired = false, bool headRequired = false)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = indentRequired;
            if (indentRequired)
            {
                settings.IndentChars = "    ";
                settings.NewLineChars = "\r\n";
            }
            settings.OmitXmlDeclaration = headRequired;
            using (MemoryStream stream = new MemoryStream())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings))
                {
                    // 强制指定命名空间，覆盖默认的命名空间
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);
                    serializer.Serialize(xmlWriter, obj, namespaces);
                };
                return GetUTF8NoBOMString(stream.GetBuffer());
            }
        }

        /// <summary>
        /// 对象转换为XML字节流，并移除BOM头
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="indentRequired"></param>
        /// <param name="headRequired"></param>
        /// <returns></returns>
        public static byte[] ExportBuffer(T obj, bool indentRequired = false, bool headRequired = false)
        {
            return Encoding.UTF8.GetBytes(Export(obj, indentRequired, headRequired));
        }

        /// <summary>
        /// XML字符串转为对象
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

        /// <summary>
        /// 移除BOM头
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string GetUTF8NoBOMString(byte[] buffer)
        {
            if (buffer == null)
                return null;

            if (buffer.Length <= 3)
            {
                return Encoding.UTF8.GetString(buffer);
            }

            byte[] bomBuffer = new byte[] { 0xef, 0xbb, 0xbf };

            if (buffer[0] == bomBuffer[0] && buffer[1] == bomBuffer[1] && buffer[2] == bomBuffer[2])
            {
                return new UTF8Encoding(false).GetString(buffer, 3, buffer.Length - 3);
            }

            return Encoding.UTF8.GetString(buffer);
        }
    }
}