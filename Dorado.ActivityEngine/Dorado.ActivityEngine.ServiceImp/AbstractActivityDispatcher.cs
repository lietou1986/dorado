using Dorado.ActivityEngine.ServiceInterface;
using Dorado.Configuration;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.Queue.Persistence;
using Dorado.Utils;
using System;

namespace Dorado.ActivityEngine.ServiceImp
{
    public abstract class AbstractActivityDispatcher : IActivityDispatcher
    {
        private PersistentQueueProcessor<Activity> activityQueue;
        private ActivityFilterList activityFilter;

        public string Name
        {
            get;
            private set;
        }

        public bool Enabled
        {
            get;
            private set;
        }

        public AbstractActivityDispatcher(ActivitySubscriberConfig subscriberConfig)
        {
            Guard.ArgumentNotNull<ActivitySubscriberConfig>(subscriberConfig);
            Guard.ArgumentNotEmpty(subscriberConfig.Name);
            this.Name = subscriberConfig.Name;
            this.Enabled = subscriberConfig.Enabled;
            this.activityFilter = new ActivityFilterList(subscriberConfig.ActivityFilters);
            this.activityQueue = new PersistentQueueProcessor<Activity>(subscriberConfig.Name, TryHandleActivity);
            if (this.Enabled)
            {
                this.activityQueue.Start();
            }
        }

        public void Dispatch(params Activity[] activities)
        {
            Guard.ArgumentNotNull<Activity[]>(activities);
            Guard.ArgumentPositive(activities.Length);
            Guard.ArgumentValuesNotNull<Activity>(activities);
            for (int i = 0; i < activities.Length; i++)
            {
                Activity activity = activities[i];
                if (this.activityFilter.Decide(activity) != ActivityFilterDecision.Deny)
                {
                    this.activityQueue.Enqueue(activity);
                }
            }
        }

        private bool TryHandleActivity(PersistentQueueProcessor<Activity> queue, Activity activity)
        {
            bool result;
            try
            {
                result = this.HandleActivity(queue, activity);
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("AbstractActivityDispatcher", ex);
                result = false;
            }
            return result;
        }

        protected abstract bool HandleActivity(PersistentQueueProcessor<Activity> queue, Activity activity);

        public virtual void ReloadConfig()
        {
            ActivitySubscriberConfig subscriberConfig = BaseConfig<ActivityEngineConfig>.Instance.GetActivitySubscriberConfig(this.Name);
            if (subscriberConfig == null)
            {
                this.Enabled = false;
                this.activityQueue.Stop();
                return;
            }
            this.Enabled = subscriberConfig.Enabled;
            this.activityFilter = new ActivityFilterList(subscriberConfig.ActivityFilters);
            if (this.Enabled)
            {
                this.activityQueue.Start();
                return;
            }
            this.activityQueue.Stop();
        }
    }
}