using System.Configuration;

namespace Dorado.DataExpress.Configuration
{
    public class PoolConfigurationElement : ConfigurationElement
    {
        private const string S_NAME = "name";
        private const string S_LOGGING = "logging";
        private const string S_USENATIVEPOOL = "useNativePool";
        private const string S_DEFAULT = "default";
        private const string S_ALIAS = "alias";
        private const string S_INITIATOR = "initiator";
        private const string S_FEATURES = "features";
        private const string S_FILTERS = "filters";
        private const string S_IDLE = "idle";
        private const string S_CONNECTION = "connection";

        [ConfigurationProperty("name", DefaultValue = "default")]
        public string Name
        {
            get
            {
                return base["name"] as string;
            }
        }

        [ConfigurationProperty("logging", DefaultValue = true)]
        public bool Logging
        {
            get
            {
                return (bool)base["logging"];
            }
        }

        [ConfigurationProperty("useNativePool", DefaultValue = false)]
        public bool UseNativePool
        {
            get
            {
                return (bool)base["useNativePool"];
            }
        }

        [ConfigurationProperty("default", DefaultValue = false)]
        public bool Default
        {
            get
            {
                return (bool)base["default"];
            }
        }

        [ConfigurationProperty("alias"), ConfigurationCollection(typeof(KeyValueConfigurationCollection))]
        public KeyValueConfigurationCollection Alias
        {
            get
            {
                return base["alias"] as KeyValueConfigurationCollection;
            }
        }

        [ConfigurationProperty("filters"), ConfigurationCollection(typeof(NameValueConfigurationCollection), AddItemName = "filter")]
        public NameValueConfigurationCollection Filters
        {
            get
            {
                return base["filters"] as NameValueConfigurationCollection;
            }
        }

        [ConfigurationProperty("features")]
        public FeaturesConfigurationElement Features
        {
            get
            {
                return base["features"] as FeaturesConfigurationElement;
            }
        }

        [ConfigurationProperty("initiator")]
        public InitiatorConfigurationElement Initiator
        {
            get
            {
                return base["initiator"] as InitiatorConfigurationElement;
            }
        }

        [ConfigurationProperty("idle")]
        public IdleConfigElement Idle
        {
            get
            {
                return base["idle"] as IdleConfigElement;
            }
        }

        [ConfigurationProperty("connection", IsRequired = true)]
        public ConnectionConfigurationElement Connection
        {
            get
            {
                return base["connection"] as ConnectionConfigurationElement;
            }
        }
    }
}