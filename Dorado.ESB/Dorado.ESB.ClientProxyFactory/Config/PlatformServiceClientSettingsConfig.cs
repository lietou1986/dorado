using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Beisen.Configuration;
using System.Xml;
using System.ServiceModel;
using Beisen.ESB.ClientProxyFactory.Proxy;

namespace Beisen.ESB.ClientProxyFactory.Config
{
    [XmlRoot("PlatformServiceClientSettingsConfig")]
    public class PlatformServiceClientSettingsConfig : IXmlSerializable
    {

        private static PlatformServiceClientSettingsConfig instance = Beisen.Configuration.ConfigManager.GetSection<PlatformServiceClientSettingsConfig>();
        public static PlatformServiceClientSettingsConfig Instance
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
            //return new List<Type>(configsByType.Keys).ToArray();
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
                 //-----Add By Guantao 2009-11-27 multiAddressHeartbeat
                ServiceStatus serviceStatus = new ServiceStatus();
                List<AddressStatus> addressStatusList = new List<AddressStatus>();
                List<EndpointAddress> endpointAddressList;
                string fullName = string.Empty;

                //总的词典，暂时不要
                //IDictionary<string, List<AddressStatus>> serviceStatusDict = new Dictionary<string, List<AddressStatus>>();
                //-----


                try
                {
                    string name = elm.GetAttribute("name");
                    string typeName = elm.GetAttribute("type");
                    string wrapper = elm.GetAttribute("wrapper");
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
                                Beisen.Configuration.Logging.LoggingWrapper.Write("Unknown type name '" + typeName + "' for Service", "PlatformServiceClientSettingsConfig");
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
                            Beisen.Configuration.Logging.LoggingWrapper.Write("Unknown type name '" + typeName + "' for Service", "PlatformServiceClientSettingsConfig");
                            //break;
                        }
                    }

                    if (type != null)
                    {

                        //-----Add By Guantao 2009-11-27 multiAddressHeartbeat
                        fullName = type.FullName.ToLower();
                        //-----


                        XmlElement bindingElm = (XmlElement)elm.SelectSingleNode("binding");
                        if (bindingElm == null)
                            throw new FormatException("need binding element for Service type:" + typeName);

                        WcfClientConfig clientConfig = new WcfClientConfig();


                        //-------------------------------------------------------------------------Add By Guantao 2009-11-27 multiAddressHeartbeat
                        //string url = elm.GetAttribute("address");
                        List<string> urlList= elm.GetAttribute("address").Split(';').ToList();

                        //-------------------------------------------------
 

                        if (urlList.Count == 0)
                            throw new FormatException("MUST define adddress for Service type:" + typeName);

                        XmlElement channelPoolElm = (XmlElement)elm.SelectSingleNode("channelPoolConfig");
                        if (channelPoolElm != null)
                            clientConfig.ChannelPoolConfig = new ChannelPoolConfig(channelPoolElm);

                        XmlElement fileGenElm = (XmlElement)elm.SelectSingleNode("fileGenerationConfig");
                        if (fileGenElm != null)
                            clientConfig.FileGenerationConfig = new FileGenerationConfig(fileGenElm);

                        //------------------------------------------By Guantao 2009-11-25
                        XmlElement heartbeatElm = (XmlElement)elm.SelectSingleNode("heartbeatConfig");
                        if (heartbeatElm != null)
                            clientConfig.HeartbeatConfig = new HeartbeatConfig(heartbeatElm);


                        //XmlElement referencedAssembliesElm = (XmlElement)elm.SelectSingleNode("referencedAssembliesConfig");
                        //if (referencedAssembliesElm != null)
                        //    clientConfig.ReferencedAssembliesConfig = new ReferencedAssembliesConfig(referencedAssembliesElm);
                        //----------------------------------By Guantao 2009-11-25


                        //-------------------------------------Add By Guantao 2009-11-27 multiAddressHeartbeat

                        endpointAddressList = new List<EndpointAddress>(urlList.Count);

                        foreach(string url in urlList)
                        {
                            endpointAddressList.Add(new EndpointAddress(url));

                            AddressStatus addressStatus=new AddressStatus();
                            addressStatus.Address = url;
                            addressStatus.AddressOK = true;
                            addressStatus.AddressRuntime=0.0;
                            addressStatus.ServiceInterface = fullName;
                            addressStatusList.Add(addressStatus);


                        }
                        serviceStatus.AddressStatusList = addressStatusList;
                        serviceStatus.ServiceName = fullName;


                        //-------------------------------------------------------------------------
                        
                        clientConfig.serviceStatus = serviceStatus;
                        clientConfig.Name = name;
                        clientConfig.Type = type;
                        if (String.IsNullOrEmpty(wrapper))
                            clientConfig.Wrapper = true;
                        else
                            bool.TryParse(wrapper, out clientConfig.Wrapper);
                        clientConfig.RemoteAddress = endpointAddressList;
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
            
                    }
                }
                catch (Exception ex)
                {
                    Beisen.Configuration.Logging.LoggingWrapper.HandleException(ex, "PlatformServiceClientSettingsConfig");
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
