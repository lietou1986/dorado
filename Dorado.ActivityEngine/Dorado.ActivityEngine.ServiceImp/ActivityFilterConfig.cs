using System.Xml;
using System.Xml.Serialization;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class ActivityFilterConfig
    {
        [XmlAttribute("Type")]
        public string FilterType
        {
            get;
            set;
        }

        [XmlAnyElement("Config")]
        public XmlElement FilterConfig
        {
            get;
            set;
        }
    }
}