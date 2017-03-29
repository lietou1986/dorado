using System.Configuration;

namespace Dorado.DataExpress.Configuration
{
    public class FeaturesConfigurationElement : ConfigurationElement
    {
        private const string S_DRIVER = "driver";
        private const string S_DIALECT = "dialect";

        [ConfigurationProperty("driver", DefaultValue = "Dorado.DataExpress.Driver.MsSql2000")]
        public string Driver
        {
            get
            {
                return base["driver"] as string;
            }
        }

        [ConfigurationProperty("dialect", DefaultValue = "Dorado.DataExpress.Dialect.MsSql")]
        public string Dialect
        {
            get
            {
                return base["dialect"] as string;
            }
        }
    }
}