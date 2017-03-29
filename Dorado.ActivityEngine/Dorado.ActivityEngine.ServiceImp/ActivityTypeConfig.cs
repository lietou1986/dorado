using System.Xml.Serialization;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class ActivityTypeConfig
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

        [XmlAttribute("Group")]
        public int Group
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