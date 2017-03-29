using System;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.Communication
{
    public class MessageFormatterInserter : BehaviorExtensionElement, IEndpointBehavior
    {
        private const string FormatterTypeConfigKey = "formatterType";
        private const string SerializerTypeConfigKey = "serializerType";

        [ConfigurationProperty(FormatterTypeConfigKey)]
        public string FormatterType
        {
            get
            {
                return (string)base[FormatterTypeConfigKey];
            }
            set
            {
                base[FormatterTypeConfigKey] = value;
            }
        }

        [ConfigurationProperty(SerializerTypeConfigKey)]
        public string SerializerType
        {
            get
            {
                return (string)base[SerializerTypeConfigKey];
            }
            set
            {
                base[SerializerTypeConfigKey] = value;
            }
        }

        #region ctor

        public MessageFormatterInserter()
        {
        }

        public MessageFormatterInserter(MessageFormatterInserter inserter)
        {
            this.FormatterType = inserter.FormatterType;
            this.SerializerType = inserter.SerializerType;
        }

        #endregion ctor

        #region IEndpointBehavior Members

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            Type serializerType = Type.GetType(this.SerializerType);
            foreach (ClientOperation op in clientRuntime.Operations)
            {
                OperationDescription des = endpoint.Contract.Operations.Find(op.Name);
                op.Formatter = new ClientMessageFormatter(des, endpoint.Address.Uri,
                    (IObjectSerializer)Activator.CreateInstance(serializerType), op.Formatter);
            }
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            Type serializerType = Type.GetType(this.SerializerType);
            Type formatterType = Type.GetType(this.FormatterType);
            foreach (DispatchOperation op in endpointDispatcher.DispatchRuntime.Operations)
            {
                OperationDescription des = endpoint.Contract.Operations.Find(op.Name);
                op.Formatter = new DispatchMessageFormatter(des, endpoint.Address.Uri,
                    (IObjectSerializer)Activator.CreateInstance(serializerType),
                    (PostParametersFormatter)Activator.CreateInstance(formatterType, des));
            }

            endpointDispatcher.DispatchRuntime.OperationSelector = new CustomOperationSelector(endpoint);
            endpointDispatcher.AddressFilter = new CustomAddressFilter(endpointDispatcher.AddressFilter);
        }

        #region nothing

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion nothing

        #endregion IEndpointBehavior Members

        #region BehaviorExtensionElement Members

        public override Type BehaviorType
        {
            get { return typeof(MessageFormatterInserter); }
        }

        protected override object CreateBehavior()
        {
            return new MessageFormatterInserter(this);
        }

        #endregion BehaviorExtensionElement Members
    }
}