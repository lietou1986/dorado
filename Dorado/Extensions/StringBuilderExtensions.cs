using System;
using System.Collections.Generic;
using System.Text;

namespace Dorado.Extensions
{
    internal static class StringBuilderExtensions
    {
        public static void AppendMetaNameContent<T>(this StringBuilder stringBuilder, string name, T content)
        {
            stringBuilder.Append("<meta name=\"");
            stringBuilder.Append(name);
            stringBuilder.Append("\" content=\"");
            stringBuilder.Append(content);
            stringBuilder.AppendLine("\">");
        }

        public static void AppendMetaNameContentIfNotNull<T>(this StringBuilder stringBuilder, string name, T content)
        {
            if (content != null)
            {
                stringBuilder.AppendMetaNameContent(name, content);
            }
        }

        public static void AppendMetaPropertyContent<T>(this StringBuilder stringBuilder, string property, T content)
        {
            stringBuilder.Append("<meta property=\"");
            stringBuilder.Append(property);
            stringBuilder.Append("\" content=\"");
            stringBuilder.Append(content);
            stringBuilder.AppendLine("\">");
        }

        public static void AppendMetaPropertyContent(this StringBuilder stringBuilder, string property, DateTime content)
        {
            stringBuilder.Append("<meta property=\"");
            stringBuilder.Append(property);
            stringBuilder.Append("\" content=\"");
            if (content.Hour == 0 && content.Minute == 0 && content.Second == 0)
            {
                stringBuilder.Append(content.ToString("yyyy-MM-dd"));
            }
            else
            {
                stringBuilder.Append(content.ToString("s") + "Z");
            }
            stringBuilder.AppendLine("\">");
        }

        public static void AppendMetaPropertyContent<T>(this StringBuilder stringBuilder, string property, IEnumerable<T> content)
        {
            foreach (T item in content)
            {
                stringBuilder.AppendMetaPropertyContent(property, item);
            }
        }

        public static void AppendMetaPropertyContentIfNotNull<T>(this StringBuilder stringBuilder, string property, T content)
        {
            if (content != null)
            {
                stringBuilder.AppendMetaPropertyContent(property, content);
            }
        }

        public static void AppendMetaPropertyContentIfNotNull(this StringBuilder stringBuilder, string property, DateTime? content)
        {
            if (content.HasValue)
            {
                stringBuilder.AppendMetaPropertyContent(property, content.Value);
            }
        }

        public static void AppendMetaPropertyContentIfNotNull<T>(this StringBuilder stringBuilder, string property, IEnumerable<T> content)
        {
            if (content != null)
            {
                stringBuilder.AppendMetaPropertyContent(property, content);
            }
        }
    }
}