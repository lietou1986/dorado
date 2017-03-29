using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Beisen.Configuration;
using System.Xml;
using System.ServiceModel;

namespace Beisen.BSP.ESB.ClientProxyFactory.Config
{
    [XmlRoot("PlatformServiceClientSettingsConfigV2")]
    public class PlatformServiceClientSettingsConfigV2 : IXmlSerializable
    {

        private static PlatformServiceClientSettingsConfigV2 instance = Beisen.Configuration.ConfigManager.GetSection<PlatformServiceClientSettingsConfigV2>();
        public static PlatformServiceClientSettingsConfigV2 Instance
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
                WcfClientManager.Instance.ReloadConfig(instance);
            }
        }

        public Type[] GetTypes()
        {
            List<Type> types = new List<Type>();
            string[] names = new List<string>(configs.Keys).ToArray();
            for (int i = 0; i < configs.Count; i++)
            {
                types.Add(configs[names[i]].Type);
            }
            return types.ToArray();
        }

        public string[] GetServiceNames()
        {
           
            return new List<String>(configs.Keys).ToArray();
        }

      

        public bool UsePool(string serviceName)
        {
            WcfClientConfig config;
            configs.TryGetValue(serviceName, out config);
            if (config == null || config.ChannelPoolConfig == null) return false;
            return config.ChannelPoolConfig.Enabled;
        }

       
        public WcfClientConfig GetWcfClientConfig(string serviceName)
        {
            WcfClientConfig config;
            configs.TryGetValue(serviceName, out config);
            return config;
        }

        Dictionary<string, WcfClientConfig> configs = new Dictionary<string, WcfClientConfig>();

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            XmlNodeList nodeList = doc.DocumentElement.SelectNodes("Service");
            foreach (XmlElement elm in nodeList)
            {
                try
                {
                    string name = elm.GetAttribute("name");
                    string typeName = elm.GetAttribute("type");
                    Type type = null;
                    if (name.Length == 0)
                    {
                        if (typeName.Length == 0)
                        {
                            throw new FormatException("MUST define type name for Service");
                        }
                        else
                        {
                            type = Type.GetType(typeName);
                            if (type == null)
                            {
                                //throw new FormatException("Unknown type name '" + typeName + "' for Service");
                                Beisen.Configuration.Logging.LoggingWrapper.Write("Unknown type name '" + typeName + "' for Service", "PlatformServiceClientSettingsConfigV2");
                                //break;                                
                            }
                            name = type.FullName;
                        }
                    }
                    else
                    {
                        type = Type.GetType(typeName);
                        if (type == null)
                        {
                            //throw new FormatException("Unknown type name '" + typeName + "' for Service");
                            Beisen.Configuration.Logging.LoggingWrapper.Write("Unknown type name '" + typeName + "' for Service", "PlatformServiceClientSettingsConfigV2");
                            //break;
                        }
                    }

                    if (type != null)
                    {
                        XmlElement bindingElm = (XmlElement)elm.SelectSingleNode("binding");
                        if (bindingElm == null)
                            throw new FormatException("need binding element for Service type:" + typeName);

                        WcfClientConfig clientConfig = new WcfClientConfig();
                        string url = elm.GetAttribute("address");
                        if (url.Length == 0)
                            throw new FormatException("MUST define adddress for Service type:" + typeName);

                        XmlElement channelPoolElm = (XmlElement)elm.SelectSingleNode("channelPoolConfig");
                        if (channelPoolElm != null)
                            clientConfig.ChannelPoolConfig = new ChannelPoolConfig(channelPoolElm);

                        XmlElement fileGenElm = (XmlElement)elm.SelectSingleNode("fileGenerationConfig");
                        if (fileGenElm != null)
                            clientConfig.FileGenerationConfig = new FileGenerationConfig(fileGenElm);

                        clientConfig.Name = name;
                        clientConfig.Type = type;
                        clientConfig.RemoteAddress = new EndpointAddress(url);
                        string bindingType = bindingElm.GetAttribute("type").ToLower();
                        switch (bindingType)
                        {
                            case "basichttpbinding":
                                clientConfig.Binding = new BasicHttpBindingConfig(bindingElm).GetBinding();
                                break;
                            case "nettcpbinding":
                                clientConfig.Binding = new NetTcpBindingConfig(bindingElm).GetBinding();
                                break;
                            case "webhttpbinding":
                                clientConfig.Binding = new WebHttpBindingConfig(bindingElm).GetBinding();
                                break;
                            default:
                                throw new FormatException("unknowing binding protocol " + bindingType + " for Service type:" + typeName);
                        }
                        configs.Add(name, clientConfig);
                        //configsByType.Add(type, name);
                    }
                }
                catch (Exception ex)
                {
                    Beisen.Configuration.Logging.LoggingWrapper.HandleException(ex, "PlatformServiceClientSettingsConfigV2");
                }
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
