using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Dorado.Configuration.ConfigurationManager;

namespace Dorado.ESB.Common
{
    public delegate void ConfigChangeDelegate(PlatformServicesServerConfig platformServicesServerConfig);

    [Serializable]
    public class PlatformServicesServerConfig
    {
        public const string SectionName = "PlatformServicesServerConfig";

        // public delegate void EventHandler(Object sender, EventArgs e);
        //public static event EventHandler ConfigChangedEventHandler;
        public static event ConfigChangeDelegate configChangeDelegate = null;

        static PlatformServicesServerConfig()
        {
            configChangeDelegate += new ConfigChangeDelegate(ReloadService);
        }

        private static void ReloadService(PlatformServicesServerConfig platformServicesServerConfig)
        {
            // Dorado.ESB.Common.Config.ConfigHelper.GetServerConfig(platformServicesServerConfig);
            // ServerControl.ServerManager.ResetService("Dorado.PlatformServicesControler");
        }

        private static PlatformServicesServerConfig instance = ConfigManager.GetSection<PlatformServicesServerConfig>();

        public static PlatformServicesServerConfig Instance
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;

                if (configChangeDelegate != null)
                {
                    configChangeDelegate(value);
                }
            }
        }

        //[XmlAttribute("enable")]
        //public string Enable = string.Empty;

        //[XmlAttribute("boot")]
        //public string EndpointName = string.Empty;

        [XmlElement("HostMetadata")]
        public HostMetadataConfig HostMetadata;
    }

    public class HostMetadataConfig
    {
        [XmlAttribute("hostName")]
        public string HostName = string.Empty;

        [XmlAttribute("assemblyNames")]
        public string AssemblyNames = string.Empty;

        [XmlElement("Service")]
        public List<ServiceConfig> Services = new List<ServiceConfig>();
    }

    public class ServiceConfig
    {
        [XmlAttribute("name")]
        public string Name = string.Empty;

        [XmlAttribute("serviceType")]
        public string ServiceType = string.Empty;

        [XmlAttribute("appDomainHostName")]
        public string AppDomainHostName = string.Empty;

        [XmlAttribute("serviceNamespace")]
        public string ServiceNamespace = string.Empty;

        [XmlAttribute("generateWcfServiceType")]
        public string GenerateWcfServiceType = string.Empty;

        [XmlAttribute("config")]
        public string ConfigFile = string.Empty;

        [XmlAttribute("assemblyFolderName")]
        public string AssemblyFolderName = string.Empty;

        [XmlAttribute("assemblyNames")]
        public string AssemblyNames = string.Empty;

        [XmlAttribute("wcfType")]
        public string WcfType = string.Empty;

        [XmlAttribute("wrapper")]
        public bool Wrapper = true;

        [XmlAttribute("serviceInterfaceType")]
        public string ServiceInterfaceType = string.Empty;

        [XmlAttribute("baseAddresses")]
        public string BaseAddresses = string.Empty;

        [XmlAttribute("isAuthorization")]
        public bool IsAuthorization = false;
    }
}