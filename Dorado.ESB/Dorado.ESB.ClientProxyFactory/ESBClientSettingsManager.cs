using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.ESB.ClientProxyFactory.Config;
using Dorado.ESB.ClientProxyFactory.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Dorado.ESB.ClientProxyFactory
{
    public class ESBClientSettingsManager
    {
        private static Dictionary<string, WcfClientConfig> configs = new Dictionary<string, WcfClientConfig>();
        private static readonly ESBClientSettingsManager instance = new ESBClientSettingsManager();

        public static ESBClientSettingsManager Instance
        {
            get { return instance; }
        }

        private ESBClientSettingsManager()
        {
            if (VerifyServices() && VerifyApplicationName())
            {
                InitServices();
            }
            else
            {
                FormatException ex = new FormatException("Services or clientnodes in the configuration file undefined,or all services disable");
                LoggerWrapper.Logger.Error("Configuration Error", ex);
                throw ex;
            }
        }

        public void ResetWcfClientConfigDictionary()
        {
            configs.Clear();
            InitServices();
        }

        public Dictionary<string, WcfClientConfig> GetWcfClientConfigDictionary()
        {
            return configs;
        }

        private bool VerifyServices()
        {
            if (ESBClientSettings.Instance.Services.Count > 0 && ESBClientSettings.Instance.ClientNodes.Count > 0)
            {
                if (ESBClientSettings.Instance.Services.Where(c => c.Enabled == true).Count() > 0)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        private bool VerifyApplicationName()
        {
            string applicationName = System.Configuration.ConfigurationManager.AppSettings["applicationName"];
            if (applicationName != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string GetApplicationName()
        {
            string applicationName = System.Configuration.ConfigurationManager.AppSettings["applicationName"];
            if (applicationName != null)
            {
                return applicationName;
            }
            else
            {
                return string.Empty;
            }
        }

        private void InitServices()
        {
            WcfClientConfig clientConfig = null;
            try
            {
                foreach (Service service in ESBClientSettings.Instance.Services)
                {
                    clientConfig = new WcfClientConfig();
                    List<Node> listNode = new List<Node>();
                    string currentProtocol = String.Empty;
                    Type type = null;
                    if (string.IsNullOrEmpty(service.Name))
                    {
                        FormatException ex = new FormatException("Service Name is not defined");
                        LoggerWrapper.Logger.Error("Configuration Error", ex);
                        continue;
                    }
                    else if (service.ControlLevel.CompareTo("Whole") == 0 && service.UseLocalService)
                    {
                        continue;
                    }
                    else
                    {
                        clientConfig.Name = service.Name;
                        clientConfig.Namespace = service.Namespace;
                        clientConfig.Wrapper = service.Wrapper;
                        clientConfig.ControlLevel = service.ControlLevel;
                        clientConfig.UseLocalService = service.UseLocalService;
                        clientConfig.Enabled = service.Enabled;

                        type = Type.GetType(String.Format("{0}, {1}", service.Type, service.Namespace));
                        if (type == null)
                        {
                            LoggerWrapper.Logger.Info(String.Format("Service interface {0} is not reference to the project, please reference", service.Type));
                            continue;
                        }
                        else
                        {
                            string applicationName = GetApplicationName();
                            ClientNode clientNode = ESBClientSettings.Instance.ClientNodes.SingleOrDefault(c => c.Name == applicationName);
                            if (clientNode != null)
                            {
                                currentProtocol = clientNode.CurrentProtocol;

                                //reliable

                                listNode = service.Protocols.SingleOrDefault(c => c.Name == currentProtocol).Nodes.ListNode.Where(c => c.IsVIP == clientNode.VipChannel && c.Enabled == true).ToList();
                                if (listNode.Count == 0)
                                {
                                    LoggerWrapper.Logger.Info("Node in the configuration file undefined");
                                    continue;
                                }
                            }
                            else
                            {
                                LoggerWrapper.Logger.Info("Applicationname does not match");
                                continue;
                            }
                        }
                    }

                    if (service.Bindings.Count == 0 || service.Bindings.Count != 3)
                    {
                        LoggerWrapper.Logger.Info("Bindings in the configuration file undefined");
                        continue;
                    }

                    if (listNode != null && listNode.Count > 0 && service.Bindings.Count > 0 && type != null)
                    {
                        BindingABC(clientConfig, service, listNode, type, currentProtocol);
                        BindingNodeStrategy(clientConfig, service, currentProtocol);
                        BindingServiceStatus(clientConfig, service, listNode, currentProtocol);
                    }
                    else
                    {
                        continue;
                    }

                    BindingProviderStrategy(clientConfig, service);

                    clientConfig.ChannelPool = service.Protocols.SingleOrDefault(c => c.Name == currentProtocol).ChannelPool;
                    clientConfig.FileGeneration = service.FileGeneration;
                    clientConfig.Heartbeat = service.Heartbeat;
                    LoggerWrapper.Logger.Info(String.Format("Add {0} to WcfClientConfig Dictionary", service.Name));
                    configs.Add(service.Name, clientConfig);
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("Dorado.ESB.ClientProxyFactory", ex);
            }
        }

        private void BindingServiceStatus(WcfClientConfig clientConfig, Service service, List<Node> listNode, string currentProtocol)
        {
            List<HeartbeatStatus> listHeartbeatStatus = new List<HeartbeatStatus>();

            listNode.ForEach(
                    delegate(Node node)
                    {
                        HeartbeatStatus heartbeatStatus = new HeartbeatStatus();
                        heartbeatStatus.Address = BuildAddress(currentProtocol, node.Host, node.ListenPort);
                        heartbeatStatus.Delay = 0.0;
                        heartbeatStatus.Pulsate = true;
                        heartbeatStatus.Weight = (int)Math.Round(node.Weight * 100, 0);
                        heartbeatStatus.CurrentWeight = (int)Math.Round(node.Weight * 10, 0);
                        heartbeatStatus.ConnectionsNumber = 0;
                        listHeartbeatStatus.Add(heartbeatStatus);
                    }
                );

            ServiceStatus serviceStatus = new ServiceStatus()
            {
                ServiceName = service.Name,
                ListHeartbeatStatus = listHeartbeatStatus
            };

            clientConfig.ServiceStatus = serviceStatus;
        }

        private void BindingProviderStrategy(WcfClientConfig clientConfig, Service service)
        {
            clientConfig.ProviderStrategy.NotConfiguredStrategy = service.Providers.NotConfiguredStrategy;
            Dictionary<string, bool> dictionaryMethod = new Dictionary<string, bool>();
            foreach (Provider provider in service.Providers.ListProvider)
            {
                foreach (Method method in provider.Methods)
                {
                    dictionaryMethod.Add(method.Name.ToLower(), method.UseLocal);
                }
            }
            clientConfig.ProviderStrategy.DictionaryMethod = dictionaryMethod;
        }

        private void BindingNodeStrategy(WcfClientConfig clientConfig, Service service, string CurrentProtocol)
        {
            Nodes nodes = service.Protocols.SingleOrDefault(c => c.Name == CurrentProtocol).Nodes;
            clientConfig.NodeStrategy = new NodeStrategy()
            {
                FailedStrategy = nodes.FailedStrategy,
                LoadBalanceStrategy = nodes.LoadBalanceStrategy,
                TryNumber = nodes.TryNumber
            };

            if (clientConfig.NodeStrategy.TryNumber > 3 || clientConfig.NodeStrategy.TryNumber < 0)
                clientConfig.NodeStrategy.TryNumber = 1;
        }

        private string BuildAddress(string currentProtocol, string host, int port)
        {
            string address;
            switch (currentProtocol)
            {
                case "NetTcp":
                    address = String.Format("net.tcp://{0}:{1}/", host, port);
                    break;

                case "BasicHttp":
                    address = String.Format("http://{0}:{1}/", host, port);
                    break;

                case "WebHttp":
                    address = String.Format("http://{0}:{1}/", host, port);
                    break;

                default:
                    address = String.Format("net.tcp://{0}:{1}/", host, port);
                    break;
            }
            return address.ToLower();
        }

        private void BindingABC(WcfClientConfig clientConfig, Service service, List<Node> listNode, Type type, string currentProtocol)
        {
            bool reliable = IsReliableSession(service, currentProtocol);
            clientConfig.ABC.Type = type;
            listNode.ForEach(delegate(Node node) { clientConfig.ABC.RemoteAddress.Add(new EndpointAddress(BuildAddress(currentProtocol, node.Host, node.ListenPort))); });
            Binding binding = service.Bindings.SingleOrDefault(c => c.Name == currentProtocol);
            switch (currentProtocol)
            {
                case "NetTcp":
                    clientConfig.ABC.Binding = new NetTcpBindingConfig(binding, reliable).GetBinding();
                    break;

                case "BasicHttp":
                    clientConfig.ABC.Binding = new BasicHttpBindingConfig(binding).GetBinding();
                    break;

                case "WebHttp":
                    clientConfig.ABC.Binding = new WebHttpBindingConfig(binding).GetBinding();
                    break;

                default:
                    throw new FormatException("unknowing binding protocol " + currentProtocol + " for Service type:" + service.Type);
            }
        }

        private static bool IsReliableSession(Service service, string currentProtocol)
        {
            string failedStrategy = service.Protocols.SingleOrDefault(c => c.Name == currentProtocol).Nodes.FailedStrategy;
            if (String.Compare(failedStrategy, "Reliable", StringComparison.CurrentCultureIgnoreCase) == 0)
                return true;
            else
                return false;
        }
    }
}