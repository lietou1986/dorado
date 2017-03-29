using Dorado.ActivityEngine.ServiceInterface;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class DenyAllFilter : IActivityFilter
    {
        public IActivityFilter Next
        {
            get;
            set;
        }

        public ActivityFilterDecision Decide(Activity activity)
        {
            return ActivityFilterDecision.Deny;
        }
    }
}