using System.ComponentModel;
using System.Configuration.Install;

namespace Dorado.Package.WindowsService
{
    [RunInstaller(true)]
    public partial class PackageServiceInstaller : Installer
    {
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstaller;

        public PackageServiceInstaller()
        {
            InitializeComponent();
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();

            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;

            this.serviceInstaller.ServiceName = "Dorado.Package.WindowsService";
            this.serviceInstaller.DisplayName = "Dorado.Package.WindowsService";
            this.serviceInstaller.Description = "Shuffles data around real good. Don't taunt the Dorado.Package.WindowsService";
            this.serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            //
            // ProjectInstaller
            //
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller,
            this.serviceInstaller});
        }
    }
}