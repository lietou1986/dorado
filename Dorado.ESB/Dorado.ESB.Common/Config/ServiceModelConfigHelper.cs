using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;

namespace Dorado.ESB.Common.Config
{
    public class ServiceModelConfigHelper
    {
        private ServiceModelConfigHelper()
        {
        }

        /// <summary>
        /// Get a specific ServiceElement from the config file
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configFilename"></param>
        /// <returns></returns>
        /// <remarks>The default (process) config file is used when argument configFilename is empty/null.</remarks>
        public static ServiceElement GetServiceElement(string configurationName, string configFilename)
        {
            ExeConfigurationFileMap filemap = new ExeConfigurationFileMap();
            filemap.ExeConfigFilename = string.IsNullOrEmpty(configFilename) ? AppDomain.CurrentDomain.SetupInformation.ConfigurationFile : Path.GetFullPath(configFilename);
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(filemap, ConfigurationUserLevel.None);
            ServiceModelSectionGroup serviceModel = ServiceModelSectionGroup.GetSectionGroup(config);

            foreach (ServiceElement se in serviceModel.Services.Services)
            {
                if (se.Name == configurationName)
                {
                    return se;
                }
            }
            throw new ArgumentException(string.Format("The service '{0}' doesn't exist in the config {1}", configurationName, filemap.ExeConfigFilename));
        }

        public static ServiceElement GetServiceElement(string configurationName, Stream config)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// add service behaviors based on the config metadata
        /// </summary>
        /// <param name="host"></param>
        /// <param name="model"></param>
        public static void AddServiceBehaviors(ServiceHostBase host, ServiceModelSection model)
        {
            string serviceConfigurationName = host.Description.ConfigurationName;

            string behaviorConfigurationName = model.Services.Services[serviceConfigurationName].BehaviorConfiguration;
            if (!string.IsNullOrEmpty(behaviorConfigurationName))
            {
                MethodInfo mi = typeof(BehaviorExtensionElement).GetMethod("CreateBehavior", BindingFlags.Instance | BindingFlags.NonPublic);

                if (model.Behaviors.ServiceBehaviors.ContainsKey(behaviorConfigurationName) == false)
                    throw new Exception(string.Format("Missing service behavior '{0}' for service '{1}'", behaviorConfigurationName, serviceConfigurationName));

                foreach (ServiceBehaviorElement sbe in model.Behaviors.ServiceBehaviors)
                {
                    if (sbe.Name == behaviorConfigurationName)
                    {
                        foreach (BehaviorExtensionElement bee in sbe)
                        {
                            IServiceBehavior behavior = mi.Invoke(bee, null) as IServiceBehavior;
                            host.Description.Behaviors.Add(behavior);
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// add service's endpoint behaviors based on the config metadata
        /// </summary>
        /// <param name="host"></param>
        /// <param name="model"></param>
        public static void AddEndpointBehaviors(ServiceHostBase host, ServiceModelSection model)
        {
            string serviceConfigurationName = host.Description.ConfigurationName;

            MethodInfo mi = typeof(BehaviorExtensionElement).GetMethod("CreateBehavior", BindingFlags.Instance | BindingFlags.NonPublic);

            // walk through all endpoints for this service
            foreach (ServiceEndpointElement see in model.Services.Services[serviceConfigurationName].Endpoints)
            {
                if (string.IsNullOrEmpty(see.BehaviorConfiguration)) continue;

                if (model.Behaviors.EndpointBehaviors.ContainsKey(see.BehaviorConfiguration) == false)
                    throw new Exception(string.Format("Missing endpoint behavior '{0}' for service '{1}'", see.BehaviorConfiguration, serviceConfigurationName));

                foreach (EndpointBehaviorElement ebe in model.Behaviors.EndpointBehaviors)
                {
                    if (ebe.Name == see.BehaviorConfiguration)
                    {
                        foreach (string key in ebe.ElementInformation.Properties.Keys)
                        {
                            if (key == "name") continue;
                            IEndpointBehavior behavior = mi.Invoke(ebe.ElementInformation.Properties[key].Value, null) as IEndpointBehavior;
                            host.Description.Endpoints.Find(see.Address).Behaviors.Add(behavior);
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// add binding for all service endpoints based on the config metadata
        /// </summary>
        /// <param name="host"></param>
        /// <param name="model"></param>
        public static void AddServiceEndpointBinding(ServiceHostBase host, ServiceModelSection model)
        {
            string serviceConfigurationName = host.Description.ConfigurationName;

            ServiceEndpointElementCollection endpoints = model.Services.Services[serviceConfigurationName].Endpoints;
            for (int ii = 0; ii < endpoints.Count; ii++)
            {
                if (host.Description.Endpoints[ii].Binding == null)
                {
                    string bindingConfigurationName = endpoints[ii].BindingConfiguration;
                    Binding binding = CreateEndpointBinding(bindingConfigurationName, model);
                    if (binding == null)
                        throw new Exception(string.Format("Missing endpoint binding '{0}' in the service '{1}'", bindingConfigurationName, serviceConfigurationName));
                    host.Description.Endpoints[ii].Binding = binding;
                }
            }
        }

        /// <summary>
        /// Add client's endpoint behaviors
        /// </summary>
        /// <param name="behaviorConfiguration"></param>
        /// <param name="endpoint"></param>
        /// <param name="model"></param>
        public static void AddChannelEndpointBehaviors(string behaviorConfiguration, ServiceEndpoint endpoint, ServiceModelSection model)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");

            if (string.IsNullOrEmpty(behaviorConfiguration))
                return;

            if (model.Behaviors.EndpointBehaviors.ContainsKey(behaviorConfiguration) == false)
                throw new Exception(string.Format("Missing endpoint behavior '{0}' for endpoint name '{1}'", behaviorConfiguration, endpoint.Name));

            MethodInfo mi = typeof(BehaviorExtensionElement).GetMethod("CreateBehavior", BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (EndpointBehaviorElement ebe in model.Behaviors.EndpointBehaviors)
            {
                if (ebe.Name == behaviorConfiguration)
                {
                    foreach (string key in ebe.ElementInformation.Properties.Keys)
                    {
                        if (key == "name") continue;
                        IEndpointBehavior behavior = mi.Invoke(ebe.ElementInformation.Properties[key].Value, null) as IEndpointBehavior;
                        endpoint.Behaviors.Add(behavior);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// create endpoint binding
        /// </summary>
        /// <param name="bindingConfigurationName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Binding CreateEndpointBinding(string bindingConfigurationName, ServiceModelSection model)
        {
            Binding binding = null;

            BindingCollectionElement bce = model.Bindings.BindingCollections.Find(delegate(BindingCollectionElement element)
            {
                if (element.ConfiguredBindings.Count == 0) return false;
                for (int ii = 0; ii < element.ConfiguredBindings.Count; ii++)
                {
                    if (element.ConfiguredBindings[ii].Name == bindingConfigurationName) return true;
                }
                return false;
            });

            if (bce == null)
                return binding;

            foreach (IBindingConfigurationElement configuredBinding in bce.ConfiguredBindings)
            {
                if (bce.BindingType == typeof(CustomBinding))
                {
                    MethodInfo mi = typeof(BindingElementExtensionElement).GetMethod("CreateBindingElement", BindingFlags.Instance | BindingFlags.NonPublic);
                    foreach (CustomBindingElement cbe in model.Bindings.CustomBinding.Bindings)
                    {
                        if (bindingConfigurationName == cbe.Name)
                        {
                            CustomBinding cb = new CustomBinding();
                            foreach (BindingElementExtensionElement element in cbe)
                            {
                                cb.Elements.Add((BindingElement)mi.Invoke(element, null));
                            }
                            return cb;
                        }
                    }
                    break;
                }
                else if (bce.BindingType == typeof(NetNamedPipeBinding))
                    binding = new NetNamedPipeBinding();
                else if (bce.BindingType == typeof(NetTcpBinding))
                    binding = new NetTcpBinding();

                //else if (bce.BindingType == typeof(NetTcpContextBinding))
                //    binding = new NetTcpContextBinding();
                else if (bce.BindingType == typeof(NetPeerTcpBinding))
                    binding = new NetPeerTcpBinding();
                else if (bce.BindingType == typeof(NetMsmqBinding))
                    binding = new NetMsmqBinding();
                else if (bce.BindingType == typeof(WSHttpBinding))
                    binding = new WSHttpBinding();

                //else if (bce.BindingType == typeof(WSHttpContextBinding))
                //    binding = new WSHttpContextBinding();
                else if (bce.BindingType == typeof(BasicHttpBinding))
                    binding = new BasicHttpBinding();

                //else if (bce.BindingType == typeof(BasicHttpContextBinding))
                //    binding = new BasicHttpContextBinding();
                else if (bce.BindingType == typeof(WebHttpBinding))
                    binding = new WebHttpBinding();
                else if (bce.BindingType == typeof(WSDualHttpBinding))
                    binding = new WSDualHttpBinding();
                else if (bce.BindingType == typeof(WSFederationHttpBinding))
                    binding = new WSFederationHttpBinding();
                else if (bce.BindingType == typeof(WS2007FederationHttpBinding))
                    binding = new WS2007FederationHttpBinding();
                else if (bce.BindingType == typeof(WS2007HttpBinding))
                    binding = new WS2007HttpBinding();
                else
                    break;

                configuredBinding.ApplyConfiguration(binding);
                break;
            }
            return binding;
        }

        /// <summary>
        /// Create list of all client's endpoints
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static List<ServiceEndpoint> ListOfClientEndpoints(ServiceModelSection model)
        {
            List<ServiceEndpoint> endpoints = new List<ServiceEndpoint>();
            if (model.Client != null && model.Client.Endpoints != null && model.Client.Endpoints.Count > 0)
            {
                foreach (ChannelEndpointElement element in model.Client.Endpoints)
                {
                    // find a contract type
                    Type contractType = Type.GetType(element.Contract, false);
                    if (contractType == null)
                    {
                        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            contractType = assembly.GetType(element.Contract, false);
                            if (contractType != null)
                                break;
                        }
                    }
                    if (contractType == null)
                        throw new Exception(string.Format("Contract type '{0}' failed in the endpoint name '{1}'", element.Contract, element.Name));

                    ServiceEndpoint endpoint = new ServiceEndpoint(ContractDescription.GetContract(contractType));
                    endpoint.Address = new EndpointAddress(element.Address);
                    endpoint.Name = element.Name;
                    if (endpoint.Binding == null)
                    {
                        endpoint.Binding = CreateEndpointBinding(element.BindingConfiguration, model);
                        if (endpoint.Binding == null)
                            throw new Exception(string.Format("Missing endpoint binding '{0}' in the endpoint name '{1}'", element.BindingConfiguration, element.Name));
                    }
                    ServiceModelConfigHelper.AddChannelEndpointBehaviors(element.BehaviorConfiguration, endpoint, model);
                    endpoints.Add(endpoint);
                }
            }
            return endpoints;
        }
    }
}