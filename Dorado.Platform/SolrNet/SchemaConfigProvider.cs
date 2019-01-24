using Dorado.Configuration.ConfigurationManager;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dorado.Platform.SolrNet
{
    public class SchemaConfigProvider
    {
        private const string SectionName = "SchemaConfig";

        static SchemaConfigProvider()
        {
            Instance = RemoteConfigurationManager.Instance.GetSection<SchemaConfig>(SectionName);
        }

        public static SchemaConfig Instance { get; set; }
    }

    [XmlRoot("SchemaConfig")]
    public class SchemaConfig
    {
        [XmlElement("field")]
        public List<SchemaField> Fields { get; set; }
    }

    public class SchemaField
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("CDATA")]
        public bool IsCdata { get; set; }

        [XmlAttribute("Boost")]
        public int Boost { get; set; }

        [XmlAttribute("multiValued")]
        public bool MultiValued { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }
    }
}