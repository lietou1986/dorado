using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Dorado.ESB.Common.Config
{
    public class ConfigHelper
    {
        private ConfigHelper()
        {
        }

        /// <summary>
        /// Deserialize xml to the instance for a specific configuration section type
        /// </summary>
        /// <typeparam name="T">type of the section</typeparam>
        /// <param name="config">xml representat of the configuration section</param>
        /// <returns>Type of the configuration section</returns>
        public static T DeserializeSection<T>(string config) where T : class
        {
            T cfgSection = Activator.CreateInstance<T>();
            byte[] buffer = new ASCIIEncoding().GetBytes(config.TrimStart(new char[] { '\r', '\n', ' ' }));
            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;

            using (MemoryStream ms = new MemoryStream(buffer))
            {
                using (XmlReader reader = XmlReader.Create(ms, xmlReaderSettings))
                {
                    try
                    {
                        Type cfgType = typeof(ConfigurationSection);
                        MethodInfo mi = cfgType.GetMethod("DeserializeSection", BindingFlags.Instance | BindingFlags.NonPublic);
                        mi.Invoke(cfgSection, new object[] { reader });
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("DeserializeSection<{0}> failed, {1}", cfgSection.ToString(), ex.InnerException == null ? ex.Message : ex.InnerException.Message));
                    }
                }
            }
            return cfgSection;
        }

        public static KeyValueConfigurationCollection AppSettings(string config)
        {
            KeyValueConfigurationCollection col = new KeyValueConfigurationCollection();
            if (string.IsNullOrEmpty(config) == false)
            {
                AppSettingsSection appSettings = null;
                if (config.TrimStart().StartsWith("<"))
                {
                    appSettings = ConfigHelper.DeserializeSection<AppSettingsSectionHelper>(config).AppSettings;
                }
                else
                {
                    ExeConfigurationFileMap filemap = new ExeConfigurationFileMap();
                    filemap.ExeConfigFilename = config;
                    System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(filemap, ConfigurationUserLevel.None);
                    if (configuration != null)
                    {
                        appSettings = configuration.AppSettings;
                    }
                }
                if (appSettings != null && appSettings.Settings.Count > 0)
                {
                    return appSettings.Settings;
                }
            }
            return col;
        }

        public static LocalRepositorySection GetServerConfig(PlatformServicesServerConfig serverConfig)
        {
            LocalRepositorySection config = null;
            if (serverConfig == null)
            {
                serverConfig = PlatformServicesServerConfig.Instance;

                if (serverConfig == null)
                {
                    config = (LocalRepositorySection)ConfigurationManager.GetSection("LocalRepository");
                    return config;
                }
            }

            config = new LocalRepositorySection();
            config.Enable = true;
            config.EndpointName = "boot";

            HostMetadataElement hostMeta = new HostMetadataElement();

            //config.HostMetadata.HostName = hostMeta.HostName;
            //config.HostMetadata.AssemblyNames = hostMeta.AssemblyNames;
            hostMeta.HostName = serverConfig.HostMetadata.HostName;
            hostMeta.AssemblyNames = serverConfig.HostMetadata.AssemblyNames;

            foreach (ServiceConfig service in serverConfig.HostMetadata.Services)
            {
                ServiceMetadataElement sme = new ServiceMetadataElement();
                sme.Name = service.Name;
                sme.ServiceType = service.ServiceType;
                sme.AppDomainHostName = service.AppDomainHostName;
                if (service.ServiceNamespace != null)
                {
                    sme.ServiceNameSpace = service.ServiceNamespace;
                }
                if (service.GenerateWcfServiceType != null)
                {
                    sme.GenerateWcfServiceType = service.GenerateWcfServiceType;
                }
                sme.Config = service.ConfigFile;
                sme.AssemblyFolderName = service.AssemblyFolderName;
                sme.AssemblyNames = service.AssemblyNames;
                sme.WcfType = service.WcfType;
                sme.Wrapper = service.Wrapper;
                sme.ServiceInterfaceType = service.ServiceInterfaceType;
                sme.BaseAddresses = service.BaseAddresses;
                sme.IsAuthorization = service.IsAuthorization;
                hostMeta.Services.Add(sme);
            }

            config.HostMetadata = hostMeta;

            return config;
        }
    }

    #region AppSettingsSectionHelper

    public sealed class AppSettingsSectionHelper : ConfigurationSection
    {
        private AppSettingsSection _appSettings = new AppSettingsSection();

        public AppSettingsSection AppSettings { get { return _appSettings; } }

        protected override void DeserializeSection(XmlReader reader)
        {
            Type cfgType = typeof(ConfigurationSection);
            MethodInfo mi = cfgType.GetMethod("DeserializeElement", BindingFlags.Instance | BindingFlags.NonPublic);

            reader.ReadStartElement("configuration");
            reader.ReadToFollowing("appSettings");
            reader.MoveToContent();
            mi.Invoke(_appSettings, new object[] { reader, false });
            reader.ReadEndElement();
            reader.ReadEndElement();
        }
    }

    #endregion AppSettingsSectionHelper
}