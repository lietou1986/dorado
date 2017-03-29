using System.ServiceModel.Channels;

namespace Dorado.Wcf.MiniEncoder
{
    public sealed class MiniEncodingBindingElement : MessageEncodingBindingElement
    {
        private MessageEncodingBindingElement _innerMessageEncodingBindingElement;
        private CompressAlgorithm _algorithm;

        public MessageEncodingBindingElement InnerMessageEncodingBindingElement
        {
            get
            {
                return _innerMessageEncodingBindingElement;
            }
            set
            {
                _innerMessageEncodingBindingElement = value;
            }
        }

        /// <summary>
        /// 压缩算法
        /// </summary>
        public CompressAlgorithm CompressAlgorithm
        {
            get
            {
                return _algorithm;
            }
            set
            {
                _algorithm = value;
            }
        }

        public MiniEncodingBindingElement()
        {
        }

        public MiniEncodingBindingElement(MessageEncodingBindingElement innerMessageEncodingBindingElement, CompressAlgorithm algorithm)
        {
            _algorithm = algorithm;
            _innerMessageEncodingBindingElement = innerMessageEncodingBindingElement;
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            context.BindingParameters.Add(this);
            return context.BuildInnerChannelListener<TChannel>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelListener<TChannel>();
        }

        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new MiniEncoderFactory(_innerMessageEncodingBindingElement, _algorithm);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            T result = _innerMessageEncodingBindingElement.GetProperty<T>(context) ?? context.GetInnerProperty<T>();

            return result;
        }

        public override MessageVersion MessageVersion
        {
            get
            {
                return _innerMessageEncodingBindingElement.MessageVersion;
            }
            set
            {
                _innerMessageEncodingBindingElement.MessageVersion = value;
            }
        }

        public override BindingElement Clone()
        {
            return new MiniEncodingBindingElement(_innerMessageEncodingBindingElement, _algorithm);
        }
    }
}