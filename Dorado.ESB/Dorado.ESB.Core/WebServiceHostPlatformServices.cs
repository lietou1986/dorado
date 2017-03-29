using Dorado.ESB.Common.Config;
using Dorado.ESB.Core.BehaviorExtensions;
using Dorado.ESB.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace Dorado.ESB.Core
{
    internal class WebServiceHostPlatformServices : WebServiceHost
    {
        private ServiceModelSection model = null;

        #region Constructors

        public WebServiceHostPlatformServices(ServiceConfigData config, Type serviceType, Type interfaceType, params Uri[] baseAddress)
            : base(serviceType, baseAddress)
        {
            // programatically changes in the service before its open such as extensions, behaiviors, etc.
            this.Extensions.Add(new ServiceConfigDataService(config));
            base.Description.Behaviors.Add(new ErrorBehaviorAttribute());

            ContractDescription contractDescription = ContractDescription.GetContract(interfaceType);
            List<ServiceEndpoint> newServiceEndpointList = new List<ServiceEndpoint>();
            List<ServiceEndpoint> oldServiceEndpointList = new List<ServiceEndpoint>();
            ServiceEndpointCollection oldServiceEndpointCollection = base.Description.Endpoints;
            foreach (ServiceEndpoint endpoint in oldServiceEndpointCollection)
            {
                if (endpoint.Name != "MetadataExchangeHttpBinding_IMetadataExchange")
                {
                    ServiceEndpoint serviceEndpoint = new ServiceEndpoint(contractDescription);
                    serviceEndpoint.Address = endpoint.Address;
                    serviceEndpoint.Binding = endpoint.Binding;
                    serviceEndpoint.ListenUri = endpoint.ListenUri;
                    serviceEndpoint.ListenUriMode = endpoint.ListenUriMode;
                    serviceEndpoint.Name = endpoint.Name;
                    oldServiceEndpointList.Add(endpoint);
                    newServiceEndpointList.Add(serviceEndpoint);
                }
            }

            oldServiceEndpointList.ForEach(delegate(ServiceEndpoint endpoint) { base.Description.Endpoints.Remove(endpoint); });
            newServiceEndpointList.ForEach(delegate(ServiceEndpoint endpoint) { base.Description.Endpoints.Add(endpoint); });

            ServiceBehaviorAttribute behaviorAttr = this.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            if (behaviorAttr == null)
            {
                behaviorAttr = new ServiceBehaviorAttribute();
                behaviorAttr.ConcurrencyMode = ConcurrencyMode.Multiple;
                behaviorAttr.InstanceContextMode = InstanceContextMode.PerCall;
                this.Description.Behaviors.Add(behaviorAttr);
            }
            else
            {
                behaviorAttr.ConcurrencyMode = ConcurrencyMode.Multiple;
                behaviorAttr.InstanceContextMode = InstanceContextMode.PerCall;
            }
        }

        #endregion Constructors

        #region ApplyConfiguration

        protected override void ApplyConfiguration()
        {
            string config = (string)CallContext.GetData("_config");
            string configurationName = this.Description.ConfigurationName;

            if (config != null && config.TrimStart().StartsWith("<"))
            {
                model = ConfigHelper.DeserializeSection<ServiceModelSection>(config);
                if (model == null || model.Services == null || model.Services.Services == null || !model.Services.Services.ContainsKey(configurationName))
                    throw new ArgumentException(string.Format("The service '{0}' doesn't exist in the config metadata", configurationName));

                // add service behaviors based on the config metadata
                ServiceModelConfigHelper.AddServiceBehaviors(this, model);

                ServiceElement se = model.Services.Services[configurationName];
                base.LoadConfigurationSection(se);

                // add custom binding based on the config metadata
                ServiceModelConfigHelper.AddServiceEndpointBinding(this, model);

                // add endpoint behaviors
                ServiceModelConfigHelper.AddEndpointBehaviors(this, model);

                return;
            }
            else
            {
                ServiceElement se = ServiceModelConfigHelper.GetServiceElement(configurationName, config);
                base.LoadConfigurationSection(se);
            }
        }

        #endregion ApplyConfiguration
    }
}