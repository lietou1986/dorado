using Dorado.ActivityEngine.ServiceInterface;
using Dorado.Configuration;
using Dorado.Utils;
using System;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class ActivityEngineProvider : IActivityEngineProvider
    {
        private static ActivityEngineProvider instance = new ActivityEngineProvider();

        public static ActivityEngineProvider Instance
        {
            get
            {
                return ActivityEngineProvider.instance;
            }
        }

        private ActivityEngineProvider()
        {
        }

        public void RaiseActivity(Activity activity)
        {
            Guard.ArgumentNotNull<Activity>(activity);
            Guard.ArgumentPositive(activity.ActivityType);
            Guard.ArgumentPositive(activity.TenantId);
            Guard.ArgumentIsTrue(activity.Timestamp > DateTime.MinValue, "Timestamp must be set");
            if (BaseConfig<ActivityEngineConfig>.Instance.IsActivityEnabled(activity.ActivityType))
            {
                foreach (IActivityDispatcher dispatcher in ActivityDispatcherManager.Instance.ActivityDispatchers)
                {
                    if (dispatcher.Enabled)
                    {
                        Util.ExecuteWithCatch(delegate
                        {
                            dispatcher.Dispatch(new Activity[]
							{
								activity
							});
                        }
                        );
                    }
                }
            }
        }
    }
}