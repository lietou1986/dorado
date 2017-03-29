using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.Json
{
    public class JsonFormatterInserter : BehaviorExtensionElement, IEndpointBehavior
    {
        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            foreach (ClientOperation op in clientRuntime.Operations)
            {
                OperationDescription description = endpoint.Contract.Operations.Find(op.Name);
                op.Formatter = new JsonClientMessageFormatter(endpoint.Address.Uri, description, op.Formatter);
            }
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(new JsonErrorHandler());

            foreach (DispatchOperation op in endpointDispatcher.DispatchRuntime.Operations)
            {
                OperationDescription description = endpoint.Contract.Operations.Find(op.Name);

                //if (JsonFormatHelper.IsJsonMethod(description))
                //{
                op.Formatter = new JsonMessageFormatter(description, endpoint.Address.Uri, op.Formatter);

                //}
            }
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion IEndpointBehavior Members

        #region BehaviorExtensionElement Members

        public override Type BehaviorType
        {
            get { return typeof(JsonFormatterInserter); }
        }

        protected override object CreateBehavior()
        {
            return new JsonFormatterInserter();
        }

        #endregion BehaviorExtensionElement Members
    }
}