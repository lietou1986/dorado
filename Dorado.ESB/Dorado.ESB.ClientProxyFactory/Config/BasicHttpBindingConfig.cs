using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Xml;
using System.ServiceModel.Channels;


namespace Beisen.ESB.ClientProxyFactory.Config
{
    public class BasicHttpBindingConfig
    {
        public BasicHttpBindingConfig(XmlElement elm)
        {

            AllowCookies= XmlCovertHelper.GetBoolAttribute(elm, "allowCookies");
            BypassProxyOnLocal = XmlCovertHelper.GetBoolAttribute(elm, "bypassProxyOnLocal");
            ProxyAddress = XmlCovertHelper.GetUriAttribute(elm, "proxyAddress");
            UseDefaultWebProxy = XmlCovertHelper.GetBoolAttribute(elm, "useDefaultWebProxy");;
            MessageEncoding = XmlCovertHelper.GetEnumAttribute<WSMessageEncoding >(elm,"messageEncoding");
            TextEncoding= XmlCovertHelper.GetEncodingAttribute(elm,"textEncoding");

            CloseTimeout = XmlCovertHelper.GetTimeSpanAttribute(elm, "closeTimeout");
            OpenTimeout = XmlCovertHelper.GetTimeSpanAttribute(elm, "openTimeout");
            ReceiveTimeout = XmlCovertHelper.GetTimeSpanAttribute(elm, "receiveTimeout");
            SendTimeout = XmlCovertHelper.GetTimeSpanAttribute(elm, "sendTimeout");

            TransferMode = XmlCovertHelper.GetEnumAttribute<TransferMode>(elm, "transferMode");
            HostNameComparisonMode = XmlCovertHelper.GetEnumAttribute<HostNameComparisonMode>(elm, "hostNameComparisonMode");

            MaxBufferPoolSize = XmlCovertHelper.GetIntAttribute(elm, "maxBufferPoolSize");
            MaxBufferSize = XmlCovertHelper.GetIntAttribute(elm, "maxBufferSize");
            MaxReceivedMessageSize = XmlCovertHelper.GetIntAttribute(elm, "maxReceivedMessageSize");

            XmlElement readerQuotasElm = (XmlElement)elm.SelectSingleNode("readerQuotas");
            if (readerQuotasElm != null)
            {
                ReaderQuotas = new ReaderQuotas(readerQuotasElm);
            }
        }

        public Binding GetBinding()
        {
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);            

            binding.AllowCookies = this.AllowCookies ;
            binding.BypassProxyOnLocal = this.BypassProxyOnLocal;
            binding.ProxyAddress = this.ProxyAddress;
            binding.UseDefaultWebProxy = this.UseDefaultWebProxy;
            binding.MessageEncoding = this.MessageEncoding;
            binding.TextEncoding = this.TextEncoding;

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

        public TimeSpan CloseTimeout;
        public TimeSpan OpenTimeout;
        public TimeSpan ReceiveTimeout;
        public TimeSpan SendTimeout;


        public bool AllowCookies;
        public bool BypassProxyOnLocal;
        public Uri ProxyAddress;
        public bool UseDefaultWebProxy;
        public WSMessageEncoding MessageEncoding;
        public Encoding TextEncoding;

        public int MaxBufferPoolSize;
        public int MaxBufferSize;
        public int MaxReceivedMessageSize;
        public HostNameComparisonMode HostNameComparisonMode;
        public TransferMode TransferMode;
        public ReaderQuotas ReaderQuotas = new ReaderQuotas();

    }
}
