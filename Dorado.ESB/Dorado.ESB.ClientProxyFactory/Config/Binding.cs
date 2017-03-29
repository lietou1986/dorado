using System.Xml.Serialization;

namespace Dorado.ESB.ClientProxyFactory.Config
{
    public class Binding
    {
        [XmlAttribute("Name")]
        public string Name;

        [XmlAttribute("CloseTimeout")]
        public string CloseTimeout;

        [XmlAttribute("OpenTimeout")]
        public string OpenTimeout;

        [XmlAttribute("ReceiveTimeout")]
        public string ReceiveTimeout;

        [XmlAttribute("SendTimeout")]
        public string SendTimeout;

        [XmlAttribute("TransferMode")]
        public string TransferMode;

        [XmlAttribute("HostNameComparisonMode")]
        public string HostNameComparisonMode;

        [XmlAttribute("MaxBufferPoolSize")]
        public int MaxBufferPoolSize;

        [XmlAttribute("MaxBufferSize")]
        public int MaxBufferSize;

        [XmlAttribute("MaxReceivedMessageSize")]
        public int MaxReceivedMessageSize;

        [XmlAttribute("AllowCookies")]
        public bool AllowCookies;

        [XmlAttribute("BypassProxyOnLocal")]
        public bool BypassProxyOnLocal;

        [XmlAttribute("ProxyAddress")]
        public string ProxyAddress;

        [XmlAttribute("UseDefaultWebProxy")]
        public bool UseDefaultWebProxy;

        [XmlAttribute("MessageEncoding")]
        public string MessageEncoding;

        [XmlAttribute("TextEncoding")]
        public string TextEncoding;

        [XmlAttribute("ListenBacklog")]
        public int ListenBacklog;

        [XmlAttribute("MaxConnections")]
        public int MaxConnections;

        [XmlElement("ReaderQuotas")]
        public ReaderQuotas ReaderQuotas;
    }
}