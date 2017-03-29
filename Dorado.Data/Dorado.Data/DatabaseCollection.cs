using System;
using System.Collections.ObjectModel;

namespace Dorado.Data
{
    internal class DatabaseCollection : KeyedCollection<string, Database>
    {
        public DatabaseCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        protected override string GetKeyForItem(Database item)
        {
            return item.InstanceName;
        }
    }
}