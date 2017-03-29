using Dorado.Configuration;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dorado.Web.Fileset
{
    [XmlRoot("fileset-configuration")]
    [Serializable]
    public class FilesetConfiguration : BaseConfig<FilesetConfiguration>
    {
        [XmlElement("parameter")]
        public FilesetParameter Parameter
        {
            get;
            set;
        }

        [XmlArrayItem("var"), XmlArray("vars")]
        public List<VariableItem> Variables
        {
            get;
            set;
        }

        [XmlArray("fileset-collection"), XmlArrayItem("fileset")]
        public List<StaticFileset> FilesetItems
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool Modified
        {
            get;
            set;
        }

        [XmlIgnore]
        public DateTime LastUpdate
        {
            get;
            set;
        }

        public FilesetConfiguration()
        {
            FilesetItems = new List<StaticFileset>();
            Parameter = new FilesetParameter
            {
                AutoMerge = false,
                EnableCache = true,
                VersionedUrl = true
            };
        }
    }
}