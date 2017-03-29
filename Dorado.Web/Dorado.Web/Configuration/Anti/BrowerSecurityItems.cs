using System;
using System.Xml.Serialization;

namespace Dorado.Web.Configuration.Anti
{
    [Serializable]
    public class BrowerSecurityItems
    {
        [XmlAttribute("Value")]
        public string Value
        {
            get;
            set;
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get;
            set;
        }
    }
}