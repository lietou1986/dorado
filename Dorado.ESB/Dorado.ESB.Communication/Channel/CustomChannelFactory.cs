using System.ServiceModel.Channels;

namespace Dorado.ESB.Communication
{
    public abstract class CustomChannelFactory : ChannelFactoryBase<IDuplexSessionChannel>
    {
        #region fields

        private MessageEncoderFactory encoderFactory;

        public MessageEncoderFactory EncoderFactory
        {
            get { return encoderFactory; }
        }

        private BufferManager bufferManager;

        public BufferManager BufferManager
        {
            get { return bufferManager; }
        }

        #endregion fields

        #region ctor

        public CustomChannelFactory(CustomTransportBindingElement bindingElement, BindingContext context)
            : base(context.Binding)
        {
            int maxBufferSize = (int)bindingElement.MaxReceivedMessageSize;
            this.bufferManager = BufferManager.CreateBufferManager(bindingElement.MaxBufferPoolSize, maxBufferSize);

            var encodingBindingElements = context.BindingParameters.FindAll<MessageEncodingBindingElement>();
            if (encodingBindingElements.Count > 1)
                throw new InvalidChannelBindingException("More than one MessageEncodingBindingElement was found");
            if (encodingBindingElements.Count <= 0)
                throw new InvalidChannelBindingException("No MessageEncodingBindingElement was found");

            this.encoderFactory = encodingBindingElements[0].CreateMessageEncoderFactory();
        }

        #endregion ctor
    }
}