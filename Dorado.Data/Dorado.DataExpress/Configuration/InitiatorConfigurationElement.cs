using System.Configuration;

namespace Dorado.DataExpress.Configuration
{
    public class InitiatorConfigurationElement : ConfigurationElement
    {
        private const string S_MAXACTIVE = "maxActive";
        private const string S_MAXWAIT = "maxWait";
        private const string S_MAX_TIMEOUT = "maxTimeout";
        private const string S_INITIALSIZE = "initialSize";
        private const string S_TIMEOUT_CHECK = "timeoutCheck";

        [ConfigurationProperty("maxActive", DefaultValue = 20)]
        public int MaxActive
        {
            get
            {
                return (int)base["maxActive"];
            }
        }

        [ConfigurationProperty("maxWait", DefaultValue = 20000)]
        public int MaxWait
        {
            get
            {
                return (int)base["maxWait"];
            }
        }

        [ConfigurationProperty("initialSize", DefaultValue = 20)]
        public int InitialSize
        {
            get
            {
                return (int)base["initialSize"];
            }
        }

        [ConfigurationProperty("maxTimeout", DefaultValue = 100000)]
        public int MaxTimeOut
        {
            get
            {
                return (int)base["maxTimeout"];
            }
        }

        [ConfigurationProperty("timeoutCheck", DefaultValue = true)]
        public bool TimeOutCheck
        {
            get
            {
                return (bool)base["timeoutCheck"];
            }
        }
    }
}