using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Dorado.Configuration.ConfigurationManager;

namespace Dorado.ESB.Json
{
    [XmlRoot("JsonFormatting")]
    public class JsonFormattingConfig
    {
        private List<string> ignoredInterfaces = new List<string>();

        [XmlElement("IgnoredInterface")]
        public List<String> IgnoredInterfaces
        {
            get { return ignoredInterfaces; }
            set { ignoredInterfaces = value; }
        }

        private static JsonFormattingConfig instance = ConfigManager.GetSection<JsonFormattingConfig>();

        public static JsonFormattingConfig Instance
        {
            get { return instance; }
        }

        public bool MethodInIgnoreList(string method)
        {
            if (ignoredInterfaces == null || ignoredInterfaces.Count == 0)
            {
                return false;
            }

            return ignoredInterfaces.Contains(method, StringComparer.OrdinalIgnoreCase);
        }
    }
}