using System.Collections.ObjectModel;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class ActivityTypeConfigCollection : KeyedCollection<int, ActivityTypeConfig>
    {
        protected override int GetKeyForItem(ActivityTypeConfig item)
        {
            return item.Id;
        }
    }
}