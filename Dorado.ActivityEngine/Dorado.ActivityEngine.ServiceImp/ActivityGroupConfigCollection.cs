using System.Collections.ObjectModel;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class ActivityGroupConfigCollection : KeyedCollection<int, ActivityGroupConfig>
    {
        protected override int GetKeyForItem(ActivityGroupConfig item)
        {
            return item.Id;
        }
    }
}