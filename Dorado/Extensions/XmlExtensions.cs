using Dorado.Core.Data;
using System;
using System.ComponentModel;
using System.Xml;

namespace Dorado.Extensions
{
    /// <remarks>codehint: sm-add</remarks>
    public static class XmlExtensions
    {
        /// <summary>Safe way to get inner text of an attribute.</summary>
        public static T GetAttributeText<T>(this XmlNode node, string attributeName, T defaultValue = default(T))
        {
            try
            {
                if (node != null && attributeName.HasValue())
                {
                    XmlAttribute attr = node.Attributes[attributeName];
                    if (attr != null)
                    {
                        return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(attr.InnerText);
                    }
                }
            }
            catch (Exception exc)
            {
                exc.Dump();
            }

            return defaultValue;
        }

        /// <summary>Safe way to get inner text of an attribute.</summary>
        public static string GetAttributeText(this XmlNode node, string attributeName)
        {
            return node.GetAttributeText<string>(attributeName, null);
        }

        /// <summary>Safe way to get inner text of a node.</summary>
        public static T GetText<T>(this XmlNode node, string xpath = null, T defaultValue = default(T))
        {
            try
            {
                if (node != null)
                {
                    if (xpath.IsNullOrEmpty())
                        return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(node.InnerText);

                    XmlNode n = node.SelectSingleNode(xpath);
                    if (n != null)
                        return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(n.InnerText);
                }
            }
            catch (Exception exc)
            {
                exc.Dump();
            }

            return defaultValue;
        }

        /// <summary>Safe way to get inner text of a node.</summary>
        public static string GetText(this XmlNode node, string xpath = null, string defaultValue = default(string))
        {
            return node.GetText<string>(xpath, defaultValue);
        }

        public static void WriteCData(this XmlWriter writer, string name, string value, string prefix = null,
            string ns = null)
        {
            if (name.HasValue() && value != null)
            {
                if (prefix == null && ns == null)
                    writer.WriteStartElement(name);
                else
                    writer.WriteStartElement(prefix, name, ns);
                writer.WriteCData(value.RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }
        }

        public static void WriteNode(this XmlWriter writer, string name, Action content)
        {
            if (name.HasValue() && content != null)
            {
                writer.WriteStartElement(name);
                content();
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// 生成Xml标签结点（如&lt;结点名&gt;内容&lt;/结点名&gt;）
        /// </summary>
        /// <param name="name">结点名</param>
        /// <param name="value">结点内容</param>
        /// <returns>返回生成的Xml结点</returns>
        public static string Quote(string name, string value)
        {
            if (value == null) return String.Empty;
            return "<" + name + ">" + Inbox(value) + "</" + name + ">";
        }

        /// <summary>
        /// 生成Xml标签结点（如&lt;结点名&gt;内容&lt;/结点名&gt;）
        /// </summary>
        /// <param name="name">结点名</param>
        /// <param name="value">结点内容</param>
        /// <returns>返回生成的Xml结点</returns>
        public static string Quote(string name, object value)
        {
            return Quote(name, DataTypeExtensions.ToString(value));
        }

        /// <summary>
        /// Html标签转义
        /// </summary>
        /// <param name="value">原有Html标签字符串</param>
        /// <returns>返回转义后的Html字符串</returns>
        public static string HtmlInbox(string value)
        {
            if (value == null) return String.Empty;
            return value.Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\n", "<br>");
        }

        /// <summary>
        /// 取消Html标签转义
        /// </summary>
        /// <param name="value">原有Html标签字符串</param>
        /// <returns>返回取消转义后的Html字符串</returns>
        public static string HtmlOutbox(string value)
        {
            if (value == null) return String.Empty;
            return value.Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">").Replace("<br>", "\n");
        }

        /// <summary>
        /// Xml标签转义
        /// </summary>
        /// <param name="value">原有Xml标签字符串</param>
        /// <returns>返回转义后的Xml字符串</returns>
        public static string Inbox(string value)
        {
            if (value == null) return String.Empty;
            return value.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        /// <summary>
        /// 取消Xml标签转义
        /// </summary>
        /// <param name="value">原有Xml标签字符串</param>
        /// <returns>返回取消转义后的Xml字符串</returns>
        public static string Outbox(string value)
        {
            if (value == null) return String.Empty;
            return
                value.Replace("&quot;", "\"")
                    .Replace("&lt;", "<")
                    .Replace("&gt;", ">")
                    .Replace("&amp;", "&")
                    .Replace("\\r", "\r")
                    .Replace("\\n", "\n");
        }
    }
}