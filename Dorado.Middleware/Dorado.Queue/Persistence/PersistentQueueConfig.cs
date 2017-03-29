using Dorado.Configuration;
using System.Xml.Serialization;

namespace Dorado.Queue.Persistence
{
    [XmlRoot("PersistentQueue")]
    public class PersistentQueueConfig : BaseConfig<PersistentQueueConfig>
    {
        [XmlElement("PersistenceRootPath")]
        public string PersistenceRootPath;

        public PersistentQueueConfig()
        {
            this.PersistenceRootPath = "D:\\PersistentQueueData\\";
        }
    }
}