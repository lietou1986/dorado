using System;
using System.ServiceModel.Channels;

namespace Dorado.ESB.Communication
{
    public class CustomEncoderFactory : MessageEncoderFactory
    {
        private MessageEncoder encoder;

        public override MessageEncoder Encoder
        {
            get { return encoder; }
        }

        public override MessageVersion MessageVersion
        {
            get { return encoder.MessageVersion; }
        }

        public CustomEncoderFactory(string encoderType)
        {
            Type type = Type.GetType(encoderType);
            encoder = (MessageEncoder)Activator.CreateInstance(type);
        }
    }
}