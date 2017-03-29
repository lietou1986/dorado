using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.Extensions.Behaviors
{
    #region EpInfoMessageInspector

    public class EpInfoCheckerInspector : IDispatchMessageInspector
    {
        public const string Header_Client = "CLIENT-MachineName";
        public const string BSSaaS_Cookie_Client = "BSSaaS_Cookie";

        #region IDispatchMessageInspector Members

        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            //  OperationContext.Current.IncomingMessageHeaders.GetHeader<string>(BSSaaS_Cookie_Client, "http://Dorado");
            string BSSaaS_Cookie = request.Headers.GetHeader<string>(Header_Client, "http://Dorado");
            string clientMachineName = request.Headers.GetHeader<string>(BSSaaS_Cookie_Client, "http://Dorado");
            return null;
        }

        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
        }

        #endregion IDispatchMessageInspector Members
    }

    #endregion EpInfoMessageInspector

    #region EpInfoCheckerAttribute

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class EpInfoCheckerAttribute : Attribute, IEndpointBehavior, IServiceBehavior
    {
        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new EpInfoCheckerInspector());
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion IEndpointBehavior Members

        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher cDispatcher in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher endpointDispatcher in cDispatcher.Endpoints)
                {
                    endpointDispatcher.DispatchRuntime.MessageInspectors.Add(
                       new EpInfoCheckerInspector());
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        #endregion IServiceBehavior Members
    }

    #endregion EpInfoCheckerAttribute

    #region EpInfoCheckerBehaviorSection

    public class EpInfoCheckerBehaviorSection : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(EpInfoCheckerAttribute); }
        }

        protected override object CreateBehavior()
        {
            return new EpInfoCheckerAttribute();
        }
    }

    #endregion EpInfoCheckerBehaviorSection
}