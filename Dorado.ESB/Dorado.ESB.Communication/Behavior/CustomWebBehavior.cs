using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.Communication
{
    public class CustomWebBehavior : IEndpointBehavior
    {
        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            // has nothing to do
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            foreach (OperationDescription description in endpoint.Contract.Operations)
            {
                if (clientRuntime.Operations.Contains(description.Name))
                {
                    ClientOperation operation = clientRuntime.Operations[description.Name];
                    operation.SerializeRequest = true;
                    operation.DeserializeReply = (description.Messages.Count > 1) && !UriTemplateHelper.IsUntypedMessage(description.Messages[1]);
                }
            }
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.AddressFilter = new PrefixEndpointAddressMessageFilter(endpoint.Address);
            endpointDispatcher.ContractFilter = new MatchAllMessageFilter();
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            // nothing
        }

        #endregion IEndpointBehavior Members
    }

    public class CustomWebSection : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(CustomWebBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new CustomWebBehavior();
        }
    }
}