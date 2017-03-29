using Dorado.ActivityEngine.ServiceInterface;
using Dorado.Configuration;
using System.Collections.Generic;
using System.Xml;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class ActivityGroupFilter : IActivityFilter
    {
        private List<int> allowedActivityGroups;
        private List<int> deniedActivityGroups;

        public IActivityFilter Next
        {
            get;
            set;
        }

        public ActivityGroupFilter(XmlElement filterConfig)
        {
            if (filterConfig != null)
            {
                XmlNode node = filterConfig.SelectSingleNode("Allow");
                if (node != null)
                {
                    this.allowedActivityGroups = Util.ParseInts(node.InnerText);
                }
                node = filterConfig.SelectSingleNode("Deny");
                if (node != null)
                {
                    this.deniedActivityGroups = Util.ParseInts(node.InnerText);
                }
            }
        }

        public ActivityFilterDecision Decide(Activity activity)
        {
            int group = BaseConfig<ActivityEngineConfig>.Instance.GetActivityGroupId(activity.ActivityType);
            if (this.allowedActivityGroups != null && this.allowedActivityGroups.Count > 0)
            {
                if (this.allowedActivityGroups.Contains(group))
                {
                    return ActivityFilterDecision.Neutral;
                }
                return ActivityFilterDecision.Deny;
            }
            else
            {
                if (this.deniedActivityGroups == null || this.deniedActivityGroups.Count <= 0)
                {
                    return ActivityFilterDecision.Neutral;
                }
                if (this.deniedActivityGroups.Contains(group))
                {
                    return ActivityFilterDecision.Deny;
                }
                return ActivityFilterDecision.Neutral;
            }
        }
    }
}