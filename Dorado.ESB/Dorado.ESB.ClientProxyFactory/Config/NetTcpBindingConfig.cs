using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Xml;
using System.ServiceModel.Channels;

namespace Beisen.ESB.ClientProxyFactory.Config
{
    /*
     * <binding name="CustomTcpBinding_ConfigBeisenPlatformServices"
				  closeTimeout="00:00:30" openTimeout="00:00:20" receiveTimeout="00:10:00"
				  sendTimeout="00:01:00" transactionFlow="false" transferMode="Buffered"
				  transactionProtocol="OleTransactions" hostNameComparisonMode="StrongWildcard"
				  listenBacklog="10" maxBufferPoolSize="524288" maxBufferSize="524288"
				  maxConnections="5" maxReceivedMessageSize="524288">
          <readerQuotas maxDepth="256" maxStringContentLength="131072"
					  maxArrayLength="131072" maxBytesPerRead="16384" maxNameTableCharCount="131072" />
     * 
     * 
          <reliableSession ordered="true" inactivityTimeout="00:00:30"
					  enabled="false" />
          <security mode="None">
            <transport clientCredentialType="Windows" protectionLevel="EncryptAndSign" />
            <message clientCredentialType="Windows" />
          </security>
        </binding>
     * */
    public class NetTcpBindingConfig
    {
        public NetTcpBindingConfig(XmlElement elm)
        {
            CloseTimeout = XmlCovertHelper.GetTimeSpanAttribute(elm, "closeTimeout");
            OpenTimeout = XmlCovertHelper.GetTimeSpanAttribute(elm, "openTimeout");
            ReceiveTimeout = XmlCovertHelper.GetTimeSpanAttribute(elm, "receiveTimeout");
            SendTimeout = XmlCovertHelper.GetTimeSpanAttribute(elm, "sendTimeout");

            TransferMode = XmlCovertHelper.GetEnumAttribute<TransferMode>(elm, "transferMode");
            HostNameComparisonMode = XmlCovertHelper.GetEnumAttribute<HostNameComparisonMode>(elm, "hostNameComparisonMode");
            ListenBacklog = XmlCovertHelper.GetIntAttribute(elm, "listenBacklog");

            MaxBufferPoolSize = XmlCovertHelper.GetIntAttribute(elm, "maxBufferPoolSize");
            MaxBufferSize = XmlCovertHelper.GetIntAttribute(elm, "maxBufferSize");
            MaxConnections = XmlCovertHelper.GetIntAttribute(elm, "maxConnections");
            MaxReceivedMessageSize = XmlCovertHelper.GetIntAttribute(elm, "maxReceivedMessageSize");

            XmlElement readerQuotasElm = (XmlElement )elm.SelectSingleNode("readerQuotas");
            if (readerQuotasElm != null)
            {
                ReaderQuotas = new ReaderQuotas(readerQuotasElm);
            }

        }

        public Binding GetBinding()
        {           
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
          
            binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            binding.CloseTimeout = this.CloseTimeout;
            binding.OpenTimeout = this.OpenTimeout;
            binding.ReceiveTimeout = this.ReceiveTimeout;
            //binding.ReceiveTimeout = TimeSpan.MaxValue;
            binding.SendTimeout = this.SendTimeout;        

            binding.TransactionFlow = false; //ignore
            binding.TransferMode = this.TransferMode;
            binding.HostNameComparisonMode = this.HostNameComparisonMode;
            binding.TransactionProtocol = TransactionProtocol.OleTransactions;
            binding.ListenBacklog = this.ListenBacklog;

            binding.MaxBufferPoolSize = this.MaxBufferPoolSize;
            binding.MaxBufferSize = this.MaxBufferSize;
            binding.MaxConnections = this.MaxConnections;
            binding.MaxReceivedMessageSize = this.MaxReceivedMessageSize;

            binding.ReaderQuotas.MaxArrayLength = this.ReaderQuotas.MaxArrayLength;
            binding.ReaderQuotas.MaxBytesPerRead = this.ReaderQuotas.MaxBytesPerRead;
            binding.ReaderQuotas.MaxDepth = this.ReaderQuotas.MaxDepth;
            binding.ReaderQuotas.MaxNameTableCharCount = this.ReaderQuotas.MaxNameTableCharCount;
            binding.ReaderQuotas.MaxStringContentLength = this.ReaderQuotas.MaxStringContentLength;

            binding.ReliableSession.Enabled = false;//ignore
            binding.ReliableSession.Ordered = true;
            binding.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;

            //PlatformNetTcpBinding platformBinding = new PlatformNetTcpBinding(binding);

            return binding;
        }

        
        public TimeSpan CloseTimeout;
        public TimeSpan OpenTimeout;
        public TimeSpan ReceiveTimeout;
        public TimeSpan SendTimeout;

        public int ListenBacklog;
        public int MaxBufferPoolSize;
        public int MaxBufferSize;
        public int MaxConnections;
        public int MaxReceivedMessageSize;
        public TransferMode TransferMode;
        public HostNameComparisonMode HostNameComparisonMode;
        public ReaderQuotas ReaderQuotas = new ReaderQuotas();
    }

    public class ReaderQuotas
    {
        public int MaxArrayLength;
        public int MaxBytesPerRead;
        public int MaxDepth;
        public int MaxNameTableCharCount;
        public int MaxStringContentLength;

        public ReaderQuotas()
        {
        }

        public ReaderQuotas(XmlElement readerQuotasElm)
        {
            MaxArrayLength = XmlCovertHelper.GetIntAttribute(readerQuotasElm, "maxArrayLength");
            MaxBytesPerRead = XmlCovertHelper.GetIntAttribute(readerQuotasElm, "maxBytesPerRead");
            MaxDepth = XmlCovertHelper.GetIntAttribute(readerQuotasElm, "maxDepth");
            MaxNameTableCharCount = XmlCovertHelper.GetIntAttribute(readerQuotasElm, "maxNameTableCharCount");
            MaxStringContentLength = XmlCovertHelper.GetIntAttribute(readerQuotasElm, "maxStringContentLength");
        }
    }

    public class PlatformNetTcpBinding : CustomBinding
    {
        public PlatformNetTcpBinding(Binding bind)
            : base(bind)
        {
        }

        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection col = base.CreateBindingElements();
            foreach (BindingElement elm in col)
            {

                ReliableSessionBindingElement reliableSession = elm as ReliableSessionBindingElement;
                if (reliableSession != null)
                {
                    reliableSession.Ordered = false;
                    reliableSession.InactivityTimeout = TimeSpan.MaxValue;
                    reliableSession.MaxTransferWindowSize = 4096;
                    reliableSession.FlowControlEnabled = true;
                    reliableSession.MaxPendingChannels = 128 * 128;
                    reliableSession.MaxRetryCount = 3;

                    break;
                }
            }
            return col;
        }
    }
}
