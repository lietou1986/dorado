using System;
using System.Xml.Serialization;

namespace Dorado.Web.Fileset
{
    [XmlInclude(typeof(MergedStaticFile)), XmlRoot("fileset")]
    [Serializable]
    public class StaticFileset
    {
        [XmlIgnore]
        public FilesetConfiguration ConfigurationRoot
        {
            get;
            set;
        }

        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("enable")]
        public bool Enable
        {
            get;
            set;
        }

        [XmlArray("implements"), XmlArrayItem("from")]
        public string[] Implements
        {
            get;
            set;
        }

        [XmlArray("files"), XmlArrayItem("file")]
        public StaticFile[] Files
        {
            get;
            set;
        }
    }
}