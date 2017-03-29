using System.Configuration;

namespace Dorado.DataExpress.Configuration
{
    public class PoolConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PoolConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as PoolConfigurationElement).Name;
        }
    }
}