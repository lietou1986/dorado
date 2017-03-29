using System.Xml.Serialization;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class ActivityGroupConfig
    {
        [XmlAttribute("Id")]
        public int Id
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

        [XmlAttribute("Enabled")]
        public bool Enabled
        {
            get;
            set;
        }
    }
}