using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Xml;

namespace Dorado.ESB.Json
{
    public class JsonpBindingConfigurationElement : StandardBindingElement
    {
        #region Properties

        [ConfigurationProperty("allowCookies", DefaultValue = false)]
        public bool AllowCookies
        {
            get
            {
                return (bool)base["allowCookies"];
            }
            set
            {
                base["allowCookies"] = value;
            }
        }

        [ConfigurationProperty("bypassProxyOnLocal", DefaultValue = false)]
        public bool BypassProxyOnLocal
        {
            get
            {
                return (bool)base["bypassProxyOnLocal"];
            }
            set
            {
                base["bypassProxyOnLocal"] = value;
            }
        }

        [ConfigurationProperty("hostNameComparisonMode", DefaultValue = 0)]
        public HostNameComparisonMode HostNameComparisonMode
        {
            get
            {
                return (HostNameComparisonMode)base["hostNameComparisonMode"];
            }
            set
            {
                base["hostNameComparisonMode"] = value;
            }
        }

        [LongValidator(MinValue = 0L), ConfigurationProperty("maxBufferPoolSize", DefaultValue = 0x80000L)]
        public long MaxBufferPoolSize
        {
            get
            {
                return (long)base["maxBufferPoolSize"];
            }
            set
            {
                base["maxBufferPoolSize"] = value;
            }
        }

        [ConfigurationProperty("maxBufferSize", DefaultValue = 0x10000), IntegerValidator(MinValue = 1)]
        public int MaxBufferSize
        {
            get
            {
                return (int)base["maxBufferSize"];
            }
            set
            {
                base["maxBufferSize"] = value;
            }
        }

        [LongValidator(MinValue = 1L), ConfigurationProperty("maxReceivedMessageSize", DefaultValue = 0x10000L)]
        public long MaxReceivedMessageSize
        {
            get
            {
                return (long)base["maxReceivedMessageSize"];
            }
            set
            {
                base["maxReceivedMessageSize"] = value;
            }
        }

        private ConfigurationPropertyCollection properties;

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection properties = base.Properties;
                    properties.Add(new ConfigurationProperty("allowCookies", typeof(bool), false, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("bypassProxyOnLocal", typeof(bool), false, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("hostNameComparisonMode", typeof(HostNameComparisonMode), HostNameComparisonMode.StrongWildcard, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxBufferSize", typeof(int), 0x10000, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxBufferPoolSize", typeof(long), 0x80000L, null, new LongValidator(0L, 0x7fffffffffffffffL, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxReceivedMessageSize", typeof(long), 0x10000L, null, new LongValidator(1L, 0x7fffffffffffffffL, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("proxyAddress", typeof(Uri), null, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("readerQuotas", typeof(XmlDictionaryReaderQuotasElement), null, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("security", typeof(WebHttpSecurityElement), null, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("transferMode", typeof(TransferMode), TransferMode.Buffered, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("useDefaultWebProxy", typeof(bool), true, null, null, ConfigurationPropertyOptions.None));
                    this.properties = properties;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("proxyAddress", DefaultValue = null)]
        public Uri ProxyAddress
        {
            get
            {
                return (Uri)base["proxyAddress"];
            }
            set
            {
                base["proxyAddress"] = value;
            }
        }

        [ConfigurationProperty("readerQuotas")]
        public XmlDictionaryReaderQuotasElement ReaderQuotas
        {
            get
            {
                return (XmlDictionaryReaderQuotasElement)base["readerQuotas"];
            }
        }

        [ConfigurationProperty("security")]
        public WebHttpSecurityElement Security
        {
            get
            {
                return (WebHttpSecurityElement)base["security"];
            }
        }

        [ConfigurationProperty("transferMode", DefaultValue = 0)]
        public TransferMode TransferMode
        {
            get
            {
                return (TransferMode)base["transferMode"];
            }
            set
            {
                base["transferMode"] = value;
            }
        }

        [ConfigurationProperty("useDefaultWebProxy", DefaultValue = true)]
        public bool UseDefaultWebProxy
        {
            get
            {
                return (bool)base["useDefaultWebProxy"];
            }
            set
            {
                base["useDefaultWebProxy"] = value;
            }
        }

        #endregion Properties

        #region ctor

        public JsonpBindingConfigurationElement(string configName)
            : base(configName)
        {
        }

        public JsonpBindingConfigurationElement()
            : this(null)
        {
        }

        #endregion ctor

        #region StandardBindingElement Members

        protected override Type BindingElementType
        {
            get { return typeof(JsonpBinding); }
        }

        protected override void OnApplyConfiguration(Binding binding)
        {
            if (binding == null) throw new ArgumentNullException("binding");

            Type bindingType = binding.GetType();
            if (bindingType != this.BindingElementType)
            {
                string message = string.Format("Unexpected binding type: {0}, expected: {1}",
                    bindingType.FullName, this.BindingElementType.FullName);
                throw new ArgumentException(message);
            }

            JsonpBinding jsonpBinding = (JsonpBinding)binding;
            jsonpBinding.BypassProxyOnLocal = this.BypassProxyOnLocal;
            jsonpBinding.HostNameComparisonMode = this.HostNameComparisonMode;
            jsonpBinding.MaxBufferPoolSize = this.MaxBufferPoolSize;
            jsonpBinding.MaxReceivedMessageSize = this.MaxReceivedMessageSize;
            jsonpBinding.TransferMode = this.TransferMode;
            jsonpBinding.UseDefaultWebProxy = this.UseDefaultWebProxy;
            jsonpBinding.AllowCookies = this.AllowCookies;
            if (this.ProxyAddress != null)
            {
                jsonpBinding.ProxyAddress = this.ProxyAddress;
            }
            if (base.ElementInformation.Properties["maxBufferSize"].ValueOrigin != PropertyValueOrigin.Default)
            {
                jsonpBinding.MaxBufferSize = this.MaxBufferSize;
            }
            ApplyReaderQuotasConfiguration(jsonpBinding.ReaderQuotas);
        }

        private void ApplyReaderQuotasConfiguration(XmlDictionaryReaderQuotas readerQuotas)
        {
            if (readerQuotas == null)
            {
                throw new ArgumentNullException("readerQuotas");
            }
            if (this.ReaderQuotas.MaxDepth != 0)
            {
                readerQuotas.MaxDepth = this.ReaderQuotas.MaxDepth;
            }
            if (this.ReaderQuotas.MaxStringContentLength != 0)
            {
                readerQuotas.MaxStringContentLength = this.ReaderQuotas.MaxStringContentLength;
            }
            if (this.ReaderQuotas.MaxArrayLength != 0)
            {
                readerQuotas.MaxArrayLength = this.ReaderQuotas.MaxArrayLength;
            }
            if (this.ReaderQuotas.MaxBytesPerRead != 0)
            {
                readerQuotas.MaxBytesPerRead = this.ReaderQuotas.MaxBytesPerRead;
            }
            if (this.ReaderQuotas.MaxNameTableCharCount != 0)
            {
                readerQuotas.MaxNameTableCharCount = this.ReaderQuotas.MaxNameTableCharCount;
            }
        }

        #endregion StandardBindingElement Members
    }
}