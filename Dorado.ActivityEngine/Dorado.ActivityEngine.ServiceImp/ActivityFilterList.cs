using Dorado.ActivityEngine.ServiceInterface;
using Dorado.Utils;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class ActivityFilterList : IActivityFilter
    {
        private IActivityFilter head;
        private IActivityFilter tail;

        public IActivityFilter Next
        {
            get;
            set;
        }

        public ActivityFilterList(ActivityFilterConfig[] activityFilterConfigs)
        {
            if (activityFilterConfigs != null && activityFilterConfigs.Length > 0)
            {
                for (int i = 0; i < activityFilterConfigs.Length; i++)
                {
                    ActivityFilterConfig filter = activityFilterConfigs[i];
                    this.AddFilter(ActivityFilterFactory.CreateFilter(filter.FilterType, filter.FilterConfig));
                }
            }
        }

        public void AddFilter(IActivityFilter filter)
        {
            Guard.ArgumentNotNull<IActivityFilter>(filter);
            if (this.head == null)
            {
                this.tail = filter;
                this.head = filter;
                return;
            }
            this.tail.Next = filter;
            this.tail = filter;
        }

        public ActivityFilterDecision Decide(Activity activity)
        {
            Guard.ArgumentNotNull<Activity>(activity);
            for (IActivityFilter filter = this.head; filter != null; filter = filter.Next)
            {
                ActivityFilterDecision decision = filter.Decide(activity);
                if (decision != ActivityFilterDecision.Neutral)
                {
                    return decision;
                }
            }
            return ActivityFilterDecision.Neutral;
        }
    }
}