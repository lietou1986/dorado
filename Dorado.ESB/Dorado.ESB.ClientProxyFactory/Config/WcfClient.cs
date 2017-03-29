using Dorado.ESB.ClientProxyFactory.Proxy;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Dorado.ESB.ClientProxyFactory.Config
{
    public class WcfClientConfig
    {
        public string Name { get; set; }

        public bool Enabled { get; set; }

        public string ControlLevel { get; set; }

        public bool Wrapper { get; set; }

        public bool UseLocalService { get; set; }

        public string Namespace { get; set; }

        public Heartbeat Heartbeat = new Heartbeat();

        public FileGeneration FileGeneration = new FileGeneration();

        public ServiceStatus ServiceStatus { get; set; }

        public ChannelPool ChannelPool = new ChannelPool();

        public ABC ABC = new ABC();

        public ProviderStrategy ProviderStrategy = new ProviderStrategy();

        public NodeStrategy NodeStrategy = new NodeStrategy();
    }

    public class ABC
    {
        public Type Type { get; set; }

        public System.ServiceModel.Channels.Binding Binding { get; set; }

        public List<EndpointAddress> RemoteAddress = new List<EndpointAddress>();
    }

    public class NodeStrategy
    {
        public string LoadBalanceStrategy { get; set; }

        public string FailedStrategy { get; set; }

        public int TryNumber { get; set; }
    }

    public class ProviderStrategy
    {
        public Dictionary<string, bool> DictionaryMethod { get; set; }

        public string NotConfiguredStrategy { get; set; }
    }
}