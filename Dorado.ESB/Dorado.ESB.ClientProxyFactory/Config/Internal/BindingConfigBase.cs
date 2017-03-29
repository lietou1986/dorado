using System;
using System.ServiceModel;

namespace Dorado.ESB.ClientProxyFactory.Config
{
    public class BindingConfigBase
    {
        public string Name { get; set; }

        public TimeSpan CloseTimeout { get; set; }

        public TimeSpan OpenTimeout { get; set; }

        public TimeSpan ReceiveTimeout { get; set; }

        public TimeSpan SendTimeout { get; set; }

        public TransferMode TransferMode { get; set; }

        public HostNameComparisonMode HostNameComparisonMode { get; set; }

        public int MaxBufferPoolSize { get; set; }

        public int MaxBufferSize { get; set; }

        public int MaxReceivedMessageSize { get; set; }
    }
}