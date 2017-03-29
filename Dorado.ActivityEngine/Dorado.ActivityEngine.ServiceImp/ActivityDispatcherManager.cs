using Dorado.ActivityEngine.ServiceInterface;
using Dorado.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class ActivityDispatcherManager
    {
        private List<IActivityDispatcher> activityDispatchers;
        private static ActivityDispatcherManager instance = new ActivityDispatcherManager();

        public List<IActivityDispatcher> ActivityDispatchers
        {
            get
            {
                return this.activityDispatchers;
            }
        }

        public static ActivityDispatcherManager Instance
        {
            get
            {
                return ActivityDispatcherManager.instance;
            }
        }

        private ActivityDispatcherManager()
        {
            this.BuildActivityDispatchers();
            BaseConfig<ActivityEngineConfig>.ConfigChanged += new EventHandler(this.ReloadConfig);
        }

        private void BuildActivityDispatchers()
        {
            List<IActivityDispatcher> dispatchers = new List<IActivityDispatcher>();
            ActivityEngineConfig config = BaseConfig<ActivityEngineConfig>.Instance;
            if (config.ActivitySubscribers != null)
            {
                foreach (ActivitySubscriberConfig subscriber in config.ActivitySubscribers)
                {
                    if (subscriber.SubscriberType == ActivitySubscriberType.LocalDispatcher)
                    {
                        Util.ExecuteWithCatch(delegate
                        {
                            dispatchers.Add(this.BuildDispatcher(subscriber));
                        }
                        );
                    }
                }
            }
            dispatchers.Add(new EsbInterfaceActivityDispatcher());
            dispatchers.Add(new RestfulActivityDispatcher());
            this.activityDispatchers = dispatchers;
        }

        private void ReloadConfig(object sender, EventArgs e)
        {
            List<IActivityDispatcher> dispatchers = new List<IActivityDispatcher>(this.activityDispatchers);
            ActivityEngineConfig config = BaseConfig<ActivityEngineConfig>.Instance;
            List<IActivityDispatcher> removeList = new List<IActivityDispatcher>();
            foreach (IActivityDispatcher dispatcher4 in dispatchers)
            {
                if (!(dispatcher4 is EsbInterfaceActivityDispatcher) && !(dispatcher4 is RestfulActivityDispatcher))
                {
                    if (!config.ActivitySubscribers.Contains(dispatcher4.Name))
                    {
                        removeList.Add(dispatcher4);
                    }
                    else
                    {
                        ActivitySubscriberConfig subscriberConfig = config.GetActivitySubscriberConfig(dispatcher4.Name);
                        if (subscriberConfig.SubscriberType != ActivitySubscriberType.LocalDispatcher || !dispatcher4.GetType().AssemblyQualifiedName.StartsWith(subscriberConfig.DispatcherImpl ?? string.Empty))
                        {
                            removeList.Add(dispatcher4);
                        }
                    }
                }
            }
            foreach (IActivityDispatcher dispatcher2 in removeList)
            {
                dispatchers.Remove(dispatcher2);
            }
            foreach (IActivityDispatcher dispatcher in dispatchers)
            {
                Util.ExecuteWithCatch(delegate
                {
                    dispatcher.ReloadConfig();
                }
                );
            }
            Dictionary<string, string> map = new Dictionary<string, string>(dispatchers.Count);
            foreach (IActivityDispatcher dispatcher3 in dispatchers)
            {
                map.Add(dispatcher3.Name, string.Empty);
            }
            foreach (ActivitySubscriberConfig subscriber in config.ActivitySubscribers)
            {
                if (subscriber.SubscriberType == ActivitySubscriberType.LocalDispatcher && !map.ContainsKey(subscriber.Name))
                {
                    Util.ExecuteWithCatch(delegate
                    {
                        dispatchers.Add(this.BuildDispatcher(subscriber));
                    }
                    );
                }
            }
            this.activityDispatchers = dispatchers;
        }

        private IActivityDispatcher BuildDispatcher(ActivitySubscriberConfig subscriberConfig)
        {
            if (string.IsNullOrEmpty(subscriberConfig.DispatcherImpl))
            {
                throw new DispatcherTypeNotSpecifiedException(subscriberConfig.Name);
            }
            Type dispatcherType = Type.GetType(subscriberConfig.DispatcherImpl);
            if (dispatcherType == null)
            {
                throw new TypeNotFoundException(subscriberConfig.DispatcherImpl);
            }
            ConstructorInfo ctor = dispatcherType.GetConstructor(new Type[]
			{
				typeof(ActivitySubscriberConfig)
			});
            if (ctor == null)
            {
                throw new DispatcherTypeInvalidException(subscriberConfig.DispatcherImpl);
            }
            return (IActivityDispatcher)ctor.Invoke(new object[]
			{
				subscriberConfig
			});
        }
    }
}