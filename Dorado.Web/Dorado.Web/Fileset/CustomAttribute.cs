using System.Xml.Serialization;

namespace Dorado.Web.Fileset
{
    public class CustomAttribute
    {
        [XmlAttribute("name")]
        public string AttributeName
        {
            get;
            set;
        }

        [XmlText]
        public string Value
        {
            get;
            set;
        }
    }
}