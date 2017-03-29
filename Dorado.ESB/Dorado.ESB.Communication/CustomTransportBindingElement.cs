using System;
using System.ServiceModel.Channels;

namespace Dorado.ESB.Communication
{
    public class CustomTransportBindingElement : TransportBindingElement
    {
        private string scheme;

        public override string Scheme
        {
            get { return scheme; }
        }

        private string channelFactoryType;

        public string ChannelFactoryType
        {
            get { return channelFactoryType; }
            set { channelFactoryType = value; }
        }

        private string channelListenerType;

        public string ChannelListenerType
        {
            get { return channelListenerType; }
            set { channelListenerType = value; }
        }

        #region ctor

        public CustomTransportBindingElement(string scheme, string channelFactoryType, string channelListenerType)
        {
            this.scheme = scheme;
            this.channelFactoryType = channelFactoryType;
            this.channelListenerType = channelListenerType;
        }

        protected CustomTransportBindingElement(CustomTransportBindingElement other)
            : this(other.Scheme, other.ChannelFactoryType, other.ChannelListenerType)
        {
        }

        public CustomTransportBindingElement()
            : this(string.Empty, string.Empty, string.Empty)
        {
        }

        #endregion ctor

        public override BindingElement Clone()
        {
            return new CustomTransportBindingElement(this);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            Type type = Type.GetType(channelFactoryType);
            return (IChannelFactory<TChannel>)Activator.CreateInstance(type, this, context);
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            Type type = Type.GetType(channelListenerType);
            return (IChannelListener<TChannel>)Activator.CreateInstance(type, this, context);
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (typeof(TChannel) == typeof(IDuplexSessionChannel))
                return true;
            return false;
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            if (typeof(TChannel) == typeof(IDuplexSessionChannel))
                return true;
            return false;
        }

        public void SetScheme(string scheme)
        {
            this.scheme = scheme;
        }
    }
}