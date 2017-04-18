using Dorado.Configuration;
using Dorado.Utils;
using System;
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
            string queueRootPath = AppDomain.CurrentDomain.BaseDirectory + "QueueData\\";
            IOUtility.CreateDirectory(queueRootPath);
            this.PersistenceRootPath = queueRootPath;
        }
    }
}