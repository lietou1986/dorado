using Topshelf;

namespace Dorado.VWS.ClientHost
{
    public class WindowService : ServiceControl
    {
        private readonly TaskProcessor _taskProcessor = new TaskProcessor();

        public bool Start(HostControl hostControl)
        {
            _taskProcessor.Start();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _taskProcessor.Close();
            return true;
        }
    }
}