using System;
using System.ServiceModel.Channels;

namespace Dorado.ESB.Communication
{
    public class CustomEncodingBindingElement : MessageEncodingBindingElement
    {
        private MessageVersion version;

        public override MessageVersion MessageVersion
        {
            get
            {
                if (version == null) return MessageVersion.None;
                return version;
            }
            set { version = value; }
        }

        private string encoderType;

        public string EncoderType
        {
            get { return encoderType; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                encoderType = value;
            }
        }

        #region ctor

        public CustomEncodingBindingElement(string encoderType)
        {
            this.EncoderType = encoderType;
        }

        public CustomEncodingBindingElement(CustomEncodingBindingElement binding)
            : this(binding.EncoderType)
        {
        }

        public CustomEncodingBindingElement()
            : this(string.Empty)
        {
        }

        #endregion ctor

        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new CustomEncoderFactory(encoderType);
        }

        public override BindingElement Clone()
        {
            return new CustomEncodingBindingElement(this);
        }

        #region nothing

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            context.BindingParameters.Add(this);
            return context.BuildInnerChannelListener<TChannel>();
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelListener<TChannel>();
        }

        #endregion nothing
    }
}