using System;
using System.ComponentModel;
using System.Configuration.Install;

namespace Dorado.ESB.WindowsService
{
    [RunInstaller(true)]
    public partial class PlatformServicesInstaller : Installer
    {
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstaller;

        public PlatformServicesInstaller()
        {
            InitializeComponent();
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();

            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;

            string instanceNumber = System.Configuration.ConfigurationManager.AppSettings["InstanceNumber"];

            Console.WriteLine("Instance number:" + instanceNumber);
            if (instanceNumber != null && instanceNumber != String.Empty)
            {
                this.serviceInstaller.ServiceName = "Dorado.ESB." + instanceNumber;
            }
            else
            {
                this.serviceInstaller.ServiceName = "Dorado.PlatformServices";
            }
            this.serviceInstaller.DisplayName = "Dorado PlatformServices";
            if (instanceNumber != null && instanceNumber != String.Empty)
            {
                this.serviceInstaller.DisplayName += " Instance " + instanceNumber;
            }
            this.serviceInstaller.Description = "Shuffles data around real good. Don't taunt the PlatformServices.";
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