using System.ServiceModel.Channels;

namespace Dorado.Wcf.MiniEncoder
{
    /// <summary>
    /// 消息编码工厂
    /// </summary>
    public class MiniEncoderFactory : MessageEncoderFactory
    {
        private MessageEncodingBindingElement _innerMessageEncodingBindingElement;
        private MiniEncoder _messageEncoder;
        private CompressAlgorithm _algorithm;

        public MiniEncoderFactory(MessageEncodingBindingElement innerMessageEncodingBindingElement, CompressAlgorithm algorithm)
        {
            _innerMessageEncodingBindingElement = innerMessageEncodingBindingElement;
            _algorithm = algorithm;
            _messageEncoder = new MiniEncoder(this, algorithm);
        }

        public override MessageEncoder CreateSessionEncoder()
        {
            return base.CreateSessionEncoder();
        }

        public override MessageEncoder Encoder
        {
            get { return _messageEncoder; }
        }

        public override MessageVersion MessageVersion
        {
            get { return _innerMessageEncodingBindingElement.MessageVersion; }
        }

        public MessageEncodingBindingElement InnerMessageEncodingBindingElement
        {
            get
            {
                return _innerMessageEncodingBindingElement;
            }
        }
    }
}