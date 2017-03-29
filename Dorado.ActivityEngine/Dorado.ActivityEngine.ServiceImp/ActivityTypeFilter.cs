using Dorado.ActivityEngine.ServiceInterface;
using System.Collections.Generic;
using System.Xml;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class ActivityTypeFilter : IActivityFilter
    {
        private List<int> allowedActivityTypes;
        private List<int> deniedActivityTypes;

        public IActivityFilter Next
        {
            get;
            set;
        }

        public ActivityTypeFilter(XmlElement filterConfig)
        {
            if (filterConfig != null)
            {
                XmlNode node = filterConfig.SelectSingleNode("Allow");
                if (node != null)
                {
                    this.allowedActivityTypes = Util.ParseInts(node.InnerText);
                }
                node = filterConfig.SelectSingleNode("Deny");
                if (node != null)
                {
                    this.deniedActivityTypes = Util.ParseInts(node.InnerText);
                }
            }
        }

        public ActivityFilterDecision Decide(Activity activity)
        {
            if (this.allowedActivityTypes != null && this.allowedActivityTypes.Count > 0)
            {
                if (this.allowedActivityTypes.Contains(activity.ActivityType))
                {
                    return ActivityFilterDecision.Accept;
                }
                return ActivityFilterDecision.Deny;
            }
            else
            {
                if (this.deniedActivityTypes == null || this.deniedActivityTypes.Count <= 0)
                {
                    return ActivityFilterDecision.Neutral;
                }
                if (this.deniedActivityTypes.Contains(activity.ActivityType))
                {
                    return ActivityFilterDecision.Deny;
                }
                return ActivityFilterDecision.Accept;
            }
        }
    }
}