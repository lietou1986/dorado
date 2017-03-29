using Dorado.ActivityEngine.ServiceInterface;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.Queue.Persistence;
using Dorado.Utils;
using System;
using System.Collections.Generic;

namespace Dorado.ActivityEngine.ServiceImp
{
    public abstract class MultiSubscriberActivityDispatcher : IActivityDispatcher
    {
        protected PersistentQueueProcessor<ActivityWithTarget> activityQueue;
        protected Dictionary<string, ActivitySubscriber> activitySubscribers;

        public abstract string Name
        {
            get;
        }

        public abstract bool Enabled
        {
            get;
        }

        public MultiSubscriberActivityDispatcher()
        {
            this.BuildActivitySubscribers();
            this.activityQueue = new PersistentQueueProcessor<ActivityWithTarget>(this.Name, new PersistentQueueProcessor<ActivityWithTarget>.QueueItemHandler(this.HandleActivity));
            this.activityQueue.Start();
        }

        protected abstract void BuildActivitySubscribers();

        public void Dispatch(params Activity[] activities)
        {
            Guard.ArgumentNotNull<Activity[]>(activities);
            Guard.ArgumentPositive(activities.Length);
            Guard.ArgumentValuesNotNull<Activity>(activities);
            for (int i = 0; i < activities.Length; i++)
            {
                Activity activity = activities[i];
                foreach (KeyValuePair<string, ActivitySubscriber> subscriber in this.activitySubscribers)
                {
                    if (subscriber.Value.Enabled)
                    {
                        this.activityQueue.Enqueue(new ActivityWithTarget(subscriber.Key, activity));
                    }
                }
            }
        }

        private bool HandleActivity(PersistentQueueProcessor<ActivityWithTarget> queue, ActivityWithTarget activity)
        {
            bool result;
            try
            {
                ActivitySubscriber subscriber;
                if (this.activitySubscribers.TryGetValue(activity.Target, out subscriber) && subscriber.Enabled)
                {
                    result = subscriber.HandleActivity(new Activity[]
					{
						activity.Activity
					});
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("MultiSubscriberActivityDispatcher", ex);
                result = false;
            }
            return result;
        }

        public virtual void ReloadConfig()
        {
            this.BuildActivitySubscribers();
        }
    }
}