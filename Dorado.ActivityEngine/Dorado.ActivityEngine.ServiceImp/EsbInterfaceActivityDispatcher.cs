using Dorado.Configuration;
using System.Collections.Generic;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class EsbInterfaceActivityDispatcher : MultiSubscriberActivityDispatcher
    {
        public override string Name
        {
            get
            {
                return "EsbInterfaceDispatcher";
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
                    if (subscriberConfig.SubscriberType == ActivitySubscriberType.ESBInterface)
                    {
                        Util.ExecuteWithCatch(delegate
                        {
                            subscribers.Add(subscriberConfig.Name, new EsbInterfaceActivitySubscriber(subscriberConfig));
                        }
                        );
                    }
                }
            }
            this.activitySubscribers = subscribers;
        }
    }
}