using System.Configuration;

namespace Dorado.DataExpress.Configuration
{
    public class PoolConfigurationSection : ConfigurationSection
    {
        private const string PoolCollectionSetionName = "pool-collection";

        [ConfigurationCollection(typeof(PoolConfigurationCollection), AddItemName = "pool", ClearItemsName = "clear", RemoveItemName = "remove"), ConfigurationProperty("pool-collection")]
        public PoolConfigurationCollection PoolCollection
        {
            get
            {
                return base["pool-collection"] as PoolConfigurationCollection;
            }
        }
    }
}