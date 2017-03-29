namespace Dorado.ActivityEngine.ServiceInterface
{
    public interface IActivityDispatcher
    {
        string Name
        {
            get;
        }

        bool Enabled
        {
            get;
        }

        void Dispatch(params Activity[] activities);

        void ReloadConfig();
    }
}