using System;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class ActivityTypeNotFoundException : ApplicationException
    {
        public ActivityTypeNotFoundException(int activityType)
            : base("Activity Type '" + activityType + "' not found")
        {
        }
    }
}