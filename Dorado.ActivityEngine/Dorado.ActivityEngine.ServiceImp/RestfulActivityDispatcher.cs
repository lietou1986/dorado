using Dorado.Configuration;
using System.Collections.Generic;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class RestfulActivityDispatcher : MultiSubscriberActivityDispatcher
    {
        public override string Name
        {
            get
            {
                return "RestfulDispatcher";
            }
        }

        public override bool Enabled
        {
            get
            {
                return true;
            }
        }

        protected override void BuildActivitySubscribers()
        {
            Dictionary<string, ActivitySubscriber> subscribers = new Dictionary<string, ActivitySubscriber>();
            ActivitySubscriberConfigCollection subscriberConfigs = BaseConfig<ActivityEngineConfig>.Instance.ActivitySubscribers;
            if (subscriberConfigs != null)
            {
                foreach (ActivitySubscriberConfig subscriberConfig in subscriberConfigs)
                {
                    if (subscriberConfig.SubscriberType == ActivitySubscriberType.RESTful)
                    {
                        Util.ExecuteWithCatch(delegate
                        {
                            subscribers.Add(subscriberConfig.Name, new RestfulActivitySubscriber(subscriberConfig));
                        }
                        );
                    }
                }
            }
            this.activitySubscribers = subscribers;
        }
    }
}