using System;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class ActivityGroupNotFoundException : ApplicationException
    {
        public ActivityGroupNotFoundException(int activityGroup)
            : base("Activity Group '" + activityGroup + "' not found")
        {
        }
    }
}