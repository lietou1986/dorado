using System;
using System.ServiceModel;

namespace Dorado.ESB.ClientProxyFactory.Config
{
    public class WebHttpBindingConfig : BindingConfigBase
    {
        public WebHttpBindingConfig()
        {
        }

        public WebHttpBindingConfig(Binding binding)
        {
            AllowCookies = binding.AllowCookies;
            BypassProxyOnLocal = binding.BypassProxyOnLocal;
            ProxyAddress = UtilityHelper.GetUriAttribute(binding.ProxyAddress);
            UseDefaultWebProxy = binding.UseDefaultWebProxy;

            CloseTimeout = UtilityHelper.GetTimeSpanAttribute(binding.CloseTimeout);
            OpenTimeout = UtilityHelper.GetTimeSpanAttribute(binding.OpenTimeout);
            ReceiveTimeout = UtilityHelper.GetTimeSpanAttribute(binding.ReceiveTimeout);
            SendTimeout = UtilityHelper.GetTimeSpanAttribute(binding.SendTimeout);

            TransferMode = UtilityHelper.GetEnumAttribute<TransferMode>(binding.TransferMode);
            HostNameComparisonMode = UtilityHelper.GetEnumAttribute<HostNameComparisonMode>(binding.HostNameComparisonMode);

            MaxBufferPoolSize = binding.MaxBufferPoolSize;
            MaxBufferSize = binding.MaxBufferSize;
            MaxReceivedMessageSize = binding.MaxReceivedMessageSize;

            ReaderQuotas = new ReaderQuotas()
            {
                MaxArrayLength = binding.ReaderQuotas.MaxArrayLength,
                MaxBytesPerRead = binding.ReaderQuotas.MaxBytesPerRead,
                MaxDepth = binding.ReaderQuotas.MaxDepth,
                MaxNameTableCharCount = binding.ReaderQuotas.MaxNameTableCharCount,
                MaxStringContentLength = binding.ReaderQuotas.MaxStringContentLength
            };
        }

        public ReaderQuotas ReaderQuotas { get; set; }

        public bool AllowCookies { get; set; }

        public bool BypassProxyOnLocal { get; set; }

        public Uri ProxyAddress { get; set; }

        public bool UseDefaultWebProxy { get; set; }

        public System.ServiceModel.Channels.Binding GetBinding()
        {
            WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.None);

            binding.AllowCookies = this.AllowCookies;
            binding.BypassProxyOnLocal = this.BypassProxyOnLocal;
            binding.ProxyAddress = this.ProxyAddress;
            binding.UseDefaultWebProxy = this.UseDefaultWebProxy;

            binding.CloseTimeout = this.CloseTimeout;
            binding.OpenTimeout = this.OpenTimeout;
            binding.ReceiveTimeout = this.ReceiveTimeout;
            binding.SendTimeout = this.SendTimeout;

            binding.HostNameComparisonMode = this.HostNameComparisonMode;
            binding.TransferMode = this.TransferMode;

            binding.MaxBufferPoolSize = this.MaxBufferPoolSize;
            binding.MaxBufferSize = this.MaxBufferSize;
            binding.MaxReceivedMessageSize = this.MaxReceivedMessageSize;

            binding.ReaderQuotas.MaxArrayLength = this.ReaderQuotas.MaxArrayLength;
            binding.ReaderQuotas.MaxBytesPerRead = this.ReaderQuotas.MaxBytesPerRead;
            binding.ReaderQuotas.MaxDepth = this.ReaderQuotas.MaxDepth;
            binding.ReaderQuotas.MaxNameTableCharCount = this.ReaderQuotas.MaxNameTableCharCount;
            binding.ReaderQuotas.MaxStringContentLength = this.ReaderQuotas.MaxStringContentLength;

            return binding;
        }
    }
}