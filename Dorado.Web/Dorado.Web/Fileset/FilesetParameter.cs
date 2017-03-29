using System;
using System.Xml.Serialization;

namespace Dorado.Web.Fileset
{
    [Serializable]
    public class FilesetParameter
    {
        [XmlAttribute("versionedUrl")]
        public bool VersionedUrl
        {
            get;
            set;
        }

        [XmlAttribute("enableDebug")]
        public bool EnableDebug
        {
            get;
            set;
        }

        [XmlAttribute("autoMerge")]
        public bool AutoMerge
        {
            get;
            set;
        }

        [XmlAttribute("enableCache")]
        public bool EnableCache
        {
            get;
            set;
        }
    }
}