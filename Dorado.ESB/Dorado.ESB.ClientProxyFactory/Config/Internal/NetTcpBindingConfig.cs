using System;
using System.ServiceModel;

namespace Dorado.ESB.ClientProxyFactory.Config
{
    public class NetTcpBindingConfig : BindingConfigBase
    {
        public NetTcpBindingConfig()
        {
        }

        public NetTcpBindingConfig(Binding binding, bool reliable)
        {
            CloseTimeout = UtilityHelper.GetTimeSpanAttribute(binding.CloseTimeout);
            OpenTimeout = UtilityHelper.GetTimeSpanAttribute(binding.OpenTimeout);
            ReceiveTimeout = UtilityHelper.GetTimeSpanAttribute(binding.ReceiveTimeout);
            SendTimeout = UtilityHelper.GetTimeSpanAttribute(binding.SendTimeout);

            TransferMode = UtilityHelper.GetEnumAttribute<TransferMode>(binding.TransferMode);
            HostNameComparisonMode = UtilityHelper.GetEnumAttribute<HostNameComparisonMode>(binding.HostNameComparisonMode);
            ListenBacklog = binding.ListenBacklog;

            MaxBufferPoolSize = binding.MaxBufferPoolSize;
            MaxBufferSize = binding.MaxBufferSize;
            MaxConnections = binding.MaxConnections;
            MaxReceivedMessageSize = binding.MaxReceivedMessageSize;

            Reliable = reliable;

            ReaderQuotas = new ReaderQuotas()
            {
                MaxArrayLength = binding.ReaderQuotas.MaxArrayLength,
                MaxBytesPerRead = binding.ReaderQuotas.MaxBytesPerRead,
                MaxDepth = binding.ReaderQuotas.MaxDepth,
                MaxNameTableCharCount = binding.ReaderQuotas.MaxNameTableCharCount,
                MaxStringContentLength = binding.ReaderQuotas.MaxStringContentLength
            };
        }

        public int ListenBacklog { get; set; }

        public int MaxConnections { get; set; }

        public bool Reliable { get; set; }

        public ReaderQuotas ReaderQuotas = new ReaderQuotas();

        public System.ServiceModel.Channels.Binding GetBinding()
        {
            System.ServiceModel.NetTcpBinding binding = new System.ServiceModel.NetTcpBinding(SecurityMode.None);

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

            binding.ReliableSession.Enabled = this.Reliable;
            binding.ReliableSession.Ordered = true;
            binding.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;

            return binding;
        }
    }
}