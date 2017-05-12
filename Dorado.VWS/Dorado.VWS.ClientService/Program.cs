using Topshelf;
using Topshelf.HostConfigurators;

namespace Dorado.VWS.ClientHost
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
            x.SetDescription("zsync client");
            x.SetDisplayName("Dorado.VWS.ClientHost");
            x.SetServiceName("Dorado.VWS.ClientHost");
            x.StartAutomatically();
        }
    }
}