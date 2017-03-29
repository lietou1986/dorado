using Dorado.ActivityEngine.ServiceInterface;
using Dorado.Utils;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class ActivityWithTarget
    {
        public string Target
        {
            get;
            set;
        }

        public Activity Activity
        {
            get;
            set;
        }

        public ActivityWithTarget()
        {
        }

        public ActivityWithTarget(string target, Activity activity)
        {
            Guard.ArgumentNotEmpty(target);
            Guard.ArgumentNotNull<Activity>(activity);
            this.Target = target;
            this.Activity = activity;
        }
    }
}