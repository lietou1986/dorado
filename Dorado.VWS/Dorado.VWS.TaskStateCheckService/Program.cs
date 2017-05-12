using Topshelf;
using Topshelf.HostConfigurators;

namespace Dorado.VWS.TaskStateCheckService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 1024;
            HostFactory.New(SetHostConfigurator).Run();
        }

        private static void SetHostConfigurator(HostConfigurator x)
        {
            x.UseNLog();

            x.Service<WindowService>();
            x.RunAsLocalSystem();
            x.SetDescription("同步系统任务状态检查");
            x.SetDisplayName("Dorado.VWS.TaskStateCheckService");
            x.SetServiceName("Dorado.VWS.TaskStateCheckService");
            x.StartAutomatically();
        }
    }
}