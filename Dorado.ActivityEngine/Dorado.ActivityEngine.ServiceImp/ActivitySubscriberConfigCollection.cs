using System.Collections.ObjectModel;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class ActivitySubscriberConfigCollection : KeyedCollection<string, ActivitySubscriberConfig>
    {
        protected override string GetKeyForItem(ActivitySubscriberConfig item)
        {
            return item.Name;
        }
    }
}