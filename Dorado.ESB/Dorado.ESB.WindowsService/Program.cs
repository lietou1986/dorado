using System.ServiceProcess;

namespace Dorado.ESB.WindowsService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
			{
				new PlatformServices()
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}