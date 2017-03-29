using Dorado.ActivityEngine.ServiceInterface;
using Dorado.Utils;

namespace Dorado.ActivityEngine.ServiceImp
{
    internal class EsbInterfaceActivitySubscriber : ActivitySubscriber
    {
        private readonly string esbInterfaceType;
        private readonly string esbMethod;

        public EsbInterfaceActivitySubscriber(ActivitySubscriberConfig subscriberConfig)
            : base(subscriberConfig)
        {
            Guard.ArgumentIsTrue(subscriberConfig.SubscriberType == ActivitySubscriberType.ESBInterface, "Wrong subscriber type: " + subscriberConfig.SubscriberType + ", must be ESBInterface");
            Guard.ArgumentNotEmpty(subscriberConfig.EsbInterfaceType);
            Guard.ArgumentNotEmpty(subscriberConfig.EsbMethod);
            this.esbInterfaceType = subscriberConfig.EsbInterfaceType;
            this.esbMethod = subscriberConfig.EsbMethod;
        }

        protected override bool HandleActivityImpl(Activity[] activities)
        {
            EsbInterfaceCaller esbCaller = EsbInterfaceHelper.GetEsbInterfaceCaller(this.esbInterfaceType, this.esbMethod);
            esbCaller(activities);
            return true;
        }
    }
}