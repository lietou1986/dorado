using System.Xml.Serialization;

namespace Dorado.ESB.ClientProxyFactory.Config
{
    public class ReaderQuotas
    {
        [XmlAttribute("MaxArrayLength")]
        public int MaxArrayLength;

        [XmlAttribute("MaxBytesPerRead")]
        public int MaxBytesPerRead;

        [XmlAttribute("MaxDepth")]
        public int MaxDepth;

        [XmlAttribute("MaxNameTableCharCount")]
        public int MaxNameTableCharCount;

        [XmlAttribute("MaxStringContentLength")]
        public int MaxStringContentLength;
    }
}