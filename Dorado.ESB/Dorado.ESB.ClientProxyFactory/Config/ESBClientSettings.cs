using Dorado.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Dorado.ESB.ClientProxyFactory.Config
{
    [XmlRoot("ESBClientSettings", Namespace = "http://Dorado.com/ESBClientSettings.xsd")]
    public class ESBClientSettings : BaseConfig<ESBClientSettings>
    {
        [XmlArray("Services")]
        [XmlArrayItem("Service")]
        public List<Service> Services = new List<Service>();

        [XmlArray("ClientNodes")]
        [XmlArrayItem("ClientNode")]
        public List<ClientNode> ClientNodes = new List<ClientNode>();

        public int Count
        {
            get
            {
                if (Services != null)
                {
                    return Services.Count;
                }
                return 0;
            }
        }

        public Service this[int i]
        {
            get
            {
                if (Services != null)
                {
                    return Services[i];
                }
                return null;
            }
        }

        public Service this[string name]
        {
            get
            {
                if (Services != null)
                {
                    return Services.SingleOrDefault(c => c.Name == String.Format("PlatformServices_{0}", name));
                }
                return null;
            }
        }
    }

    public class Service
    {
        [XmlAttribute("Name")]
        public string Name;

        [XmlAttribute("Enabled")]
        public bool Enabled;

        [XmlAttribute("ControlLevel")]
        public string ControlLevel;

        [XmlAttribute("Wrapper")]
        public bool Wrapper;

        [XmlAttribute("UseLocalService")]
        public bool UseLocalService;

        [XmlAttribute("Type")]
        public string Type;

        [XmlAttribute("Namespace")]
        public string Namespace;

        [XmlElement("Heartbeat")]
        public Heartbeat Heartbeat;

        [XmlElement("FileGeneration")]
        public FileGeneration FileGeneration;

        [XmlArray("Protocols")]
        [XmlArrayItem("Protocol")]
        public List<Protocol> Protocols = new List<Protocol>();

        [XmlElement("Providers")]
        public Providers Providers;

        [XmlArray("Bindings")]
        [XmlArrayItem("Binding")]
        public List<Binding> Bindings = new List<Binding>();
    }

    public class Protocol
    {
        [XmlAttribute("Name")]
        public string Name;

        [XmlElement("Nodes")]
        public Nodes Nodes;

        [XmlElement("ChannelPool")]
        public ChannelPool ChannelPool;
    }

    public class Nodes
    {
        [XmlAttribute("LoadBalanceStrategy")]
        public string LoadBalanceStrategy;

        [XmlAttribute("FailedStrategy")]
        public string FailedStrategy;

        [XmlAttribute("TryNumber")]
        public int TryNumber;

        [XmlElement("Node")]
        public List<Node> ListNode = new List<Node>();
    }

    public class Node
    {
        [XmlAttribute("Enabled")]
        public bool Enabled;

        [XmlAttribute("Host")]
        public string Host;

        [XmlAttribute("ListenPort")]
        public int ListenPort;

        [XmlAttribute("IsVIP")]
        public bool IsVIP;

        [XmlAttribute("Weight")]
        public double Weight;
    }

    public class ChannelPool
    {
        [XmlAttribute("Enabled")]
        public bool Enabled;

        [XmlAttribute("PoolSize")]
        public int PoolSize;

        [XmlAttribute("WaitingTimeout")]
        public int WaitingTimeout;
    }

    public class Heartbeat
    {
        [XmlAttribute("Enabled")]
        public bool Enabled;

        [XmlAttribute("Interval")]
        public int Interval;
    }

    public class FileGeneration
    {
        [XmlAttribute("SourceFileTemplate")]
        public string SourceFileTemplate;

        [XmlAttribute("SourceFactoryFileTemplate")]
        public string SourceFactoryFileTemplate;

        [XmlAttribute("Enabled")]
        public bool Enabled;
    }

    public class ClientNode
    {
        [XmlAttribute("Name")]
        public string Name;

        [XmlAttribute("CurrentProtocol")]
        public string CurrentProtocol;

        [XmlAttribute("VipChannel")]
        public bool VipChannel;
    }

    public class Providers
    {
        [XmlAttribute("NotConfiguredStrategy")]
        public string NotConfiguredStrategy;

        [XmlElement("Provider")]
        public List<Provider> ListProvider = new List<Provider>();
    }

    public class Provider
    {
        [XmlAttribute("Name")]
        public string Name;

        [XmlArray("Methods")]
        [XmlArrayItem("Method")]
        public List<Method> Methods = new List<Method>();
    }

    public class Method
    {
        [XmlAttribute("UseLocal")]
        public bool UseLocal;

        [XmlAttribute("Name")]
        public string Name;
    }
}