using Dorado.ActivityEngine.ServiceInterface;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class AllowAllFilter : IActivityFilter
    {
        public IActivityFilter Next
        {
            get;
            set;
        }

        public ActivityFilterDecision Decide(Activity activity)
        {
            return ActivityFilterDecision.Accept;
        }
    }
}