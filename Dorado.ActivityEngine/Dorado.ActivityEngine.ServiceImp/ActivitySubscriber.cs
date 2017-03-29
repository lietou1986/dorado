using Dorado.ActivityEngine.ServiceInterface;
using Dorado.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Dorado.ActivityEngine.ServiceImp
{
    public abstract class ActivitySubscriber
    {
        private ActivitySubscriberConfig subscriberConfig;
        private ActivityFilterList activityFilter;

        public string Name
        {
            get
            {
                return this.subscriberConfig.Name;
            }
        }

        public bool Enabled
        {
            get
            {
                return this.subscriberConfig.Enabled;
            }
        }

        public ActivitySubscriber(ActivitySubscriberConfig subscriberConfig)
        {
            Guard.ArgumentNotNull<ActivitySubscriberConfig>(subscriberConfig);
            Guard.ArgumentNotEmpty(subscriberConfig.Name);
            this.subscriberConfig = subscriberConfig;
            this.activityFilter = new ActivityFilterList(subscriberConfig.ActivityFilters);
        }

        public bool HandleActivity(params Activity[] activities)
        {
            List<Activity> filteredActivities = new List<Activity>(activities.Length);
            filteredActivities.AddRange(activities.Where(activity => this.activityFilter.Decide(activity) != ActivityFilterDecision.Deny));
            return filteredActivities.Count <= 0 || this.HandleActivityImpl(filteredActivities.ToArray());
        }

        protected abstract bool HandleActivityImpl(Activity[] activities);
    }
}