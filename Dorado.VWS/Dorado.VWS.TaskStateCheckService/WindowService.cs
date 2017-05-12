using Topshelf;

namespace Dorado.VWS.TaskStateCheckService
{
    public class WindowService : ServiceControl
    {
        public bool Start(HostControl hostControl)
        {
            StateCheck.Start();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            StateCheck.Stop();
            return true;
        }
    }
}