using System.Xml.Serialization;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class ActivitySubscriberConfig
    {
        [XmlAttribute("Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("Type")]
        public ActivitySubscriberType SubscriberType
        {
            get;
            set;
        }

        [XmlAttribute("DispatcherImpl")]
        public string DispatcherImpl
        {
            get;
            set;
        }

        [XmlAttribute("EsbInterfaceType")]
        public string EsbInterfaceType
        {
            get;
            set;
        }

        [XmlAttribute("EsbMethod")]
        public string EsbMethod
        {
            get;
            set;
        }

        [XmlAttribute("RestUrl")]
        public string RestUrl
        {
            get;
            set;
        }

        [XmlAttribute("JsonDateTimeFormat")]
        public JsonDateTimeFormat JsonDateTimeFormat
        {
            get;
            set;
        }

        [XmlAttribute("Enabled")]
        public bool Enabled
        {
            get;
            set;
        }

        [XmlArray("Filters"), XmlArrayItem("Filter")]
        public ActivityFilterConfig[] ActivityFilters
        {
            get;
            set;
        }

        public ActivitySubscriberConfig()
        {
            this.Enabled = true;
            this.JsonDateTimeFormat = JsonDateTimeFormat.IsoDateTime;
        }
    }
}