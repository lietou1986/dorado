namespace Dorado.ActivityEngine.ServiceInterface
{
    public interface IActivityFilter
    {
        IActivityFilter Next
        {
            get;
            set;
        }

        ActivityFilterDecision Decide(Activity activity);
    }
}