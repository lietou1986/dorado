using Dorado.ActivityEngine.ServiceInterface;
using Dorado.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

namespace Dorado.ActivityEngine.ServiceImp
{
    internal class RestfulActivitySubscriber : ActivitySubscriber
    {
        private string targetUrl;
        private JsonSerializer serializer;

        public RestfulActivitySubscriber(ActivitySubscriberConfig subscriberConfig)
            : base(subscriberConfig)
        {
            Guard.ArgumentIsTrue(subscriberConfig.SubscriberType == ActivitySubscriberType.RESTful, "Wrong subscriber type: " + subscriberConfig.SubscriberType + ", must be RESTful");
            Guard.ArgumentNotEmpty(subscriberConfig.RestUrl);
            this.serializer = new JsonSerializer();
            if (subscriberConfig.JsonDateTimeFormat == JsonDateTimeFormat.IsoDateTime)
            {
                IsoDateTimeConverter converter = new IsoDateTimeConverter();
                converter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                this.serializer.Converters.Add(converter);
            }
            this.targetUrl = subscriberConfig.RestUrl;
        }

        protected override bool HandleActivityImpl(Activity[] activities)
        {
            using (StringWriter writer = new StringWriter())
            {
                this.serializer.Serialize(writer, activities);
                RestHelper.PostJson(writer.ToString(), this.targetUrl);
            }
            return true;
        }
    }
}