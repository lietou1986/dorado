using System.Configuration;

namespace Dorado.DataExpress.Configuration
{
    public class IdleConfigElement : ConfigurationElement
    {
        private const string S_CHECK = "check";
        private const string S_INTERVAL = "interval";
        private const string S_MAXIDLE = "maxIdle";

        [ConfigurationProperty("check", DefaultValue = true)]
        public bool Check
        {
            get
            {
                return (bool)base["check"];
            }
        }

        [ConfigurationProperty("interval", DefaultValue = 300000)]
        public int Interval
        {
            get
            {
                return (int)base["interval"];
            }
        }

        [ConfigurationProperty("maxIdle", DefaultValue = 300000)]
        public int MaxIdle
        {
            get
            {
                return (int)base["maxIdle"];
            }
        }
    }
}