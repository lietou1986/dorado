using Dorado.Configuration;
using Dorado.Utils;
using System.Xml.Serialization;

namespace Dorado.ActivityEngine.ServiceImp
{
    [XmlRoot("ActivityEngine")]
    public class ActivityEngineConfig : BaseConfig<ActivityEngineConfig>
    {
        [XmlArrayItem("ActivityGroup"), XmlArray("ActivityGroups")]
        public ActivityGroupConfigCollection ActivityGroups
        {
            get;
            set;
        }

        [XmlArray("ActivityTypes"), XmlArrayItem("ActivityType")]
        public ActivityTypeConfigCollection ActivityTypes
        {
            get;
            set;
        }

        [XmlArrayItem("ActivitySubscriber"), XmlArray("ActivitySubscribers")]
        public ActivitySubscriberConfigCollection ActivitySubscribers
        {
            get;
            set;
        }

        [XmlElement("ActivityDataDirectory")]
        public string ActivityDataDirectory
        {
            get;
            set;
        }

        public ActivityEngineConfig()
        {
            this.ActivityGroups = new ActivityGroupConfigCollection();
            this.ActivityTypes = new ActivityTypeConfigCollection();
            this.ActivitySubscribers = new ActivitySubscriberConfigCollection();
            this.ActivityDataDirectory = "ActivityData";
        }

        public ActivitySubscriberConfig GetActivitySubscriberConfig(string subscriberName)
        {
            Guard.ArgumentNotEmpty(subscriberName);
            if (this.ActivitySubscribers.Contains(subscriberName))
            {
                return this.ActivitySubscribers[subscriberName];
            }
            return null;
        }

        public ActivityTypeConfig GetActivityTypeConfig(int activityType)
        {
            if (this.ActivityTypes.Contains(activityType))
            {
                return this.ActivityTypes[activityType];
            }
            return null;
        }

        public ActivityGroupConfig GetActivityGroupConfig(int activityGroup)
        {
            if (this.ActivityGroups.Contains(activityGroup))
            {
                return this.ActivityGroups[activityGroup];
            }
            return null;
        }

        public int GetActivityGroupId(int activityType)
        {
            if (this.ActivityTypes.Contains(activityType))
            {
                return this.ActivityTypes[activityType].Group;
            }
            throw new ActivityTypeNotFoundException(activityType);
        }

        public bool IsActivityEnabled(int activityType)
        {
            return this.IsActivityGroupEnabled(activityType) && this.IsActivityTypeEnabled(activityType);
        }

        public bool IsActivityTypeEnabled(int activityType)
        {
            ActivityTypeConfig activityTypeConfig = this.GetActivityTypeConfig(activityType);
            if (activityTypeConfig == null)
            {
                throw new ActivityTypeNotFoundException(activityType);
            }
            return activityTypeConfig.Enabled;
        }

        public bool IsActivityGroupEnabled(int activityType)
        {
            int activityGroup = this.GetActivityGroupId(activityType);
            if (activityGroup <= 0)
            {
                throw new ActivityGroupNotSpecifiedException(activityType);
            }
            ActivityGroupConfig activityGroupConfig = this.GetActivityGroupConfig(activityGroup);
            if (activityGroupConfig == null)
            {
                throw new ActivityGroupNotFoundException(activityGroup);
            }
            return activityGroupConfig.Enabled;
        }
    }
}