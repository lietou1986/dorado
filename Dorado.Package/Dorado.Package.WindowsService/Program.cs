using System.ServiceProcess;

namespace Dorado.Package.WindowsService
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
				new PackageService()
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}