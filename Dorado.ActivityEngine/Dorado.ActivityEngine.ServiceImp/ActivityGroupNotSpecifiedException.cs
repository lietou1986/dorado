using System;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class ActivityGroupNotSpecifiedException : ApplicationException
    {
        public ActivityGroupNotSpecifiedException(int activityType)
            : base("Activity Group for Activity Type '" + activityType + "' not specified")
        {
        }
    }
}