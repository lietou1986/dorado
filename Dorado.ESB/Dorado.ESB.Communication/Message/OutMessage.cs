using System;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.Xml;

namespace Dorado.ESB.Communication
{
    [DataContract]
    public class OutMessage : MessageBase
    {
        [DataMember]
        public string MessageId
        {
            get
            {
                if (Headers.MessageId == null) return string.Empty;
                return Headers.MessageId.ToString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    Headers.MessageId = new UniqueId(value);
            }
        }

        [DataMember]
        public string To
        {
            get
            {
                if (Headers.To == null) return string.Empty;
                return Headers.To.ToString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    Headers.To = new Uri(value);
            }
        }

        [DataMember]
        public string RelatesTo
        {
            get
            {
                if (Headers.RelatesTo == null) return string.Empty;
                return Headers.RelatesTo.ToString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    Headers.RelatesTo = new UniqueId(value);
            }
        }

        private byte[] data;

        [DataMember]
        public byte[] Data
        {
            get
            {
                if (data == null && body != null) data = serializer.Serialize(body);
                return data;
            }
            set
            {
                this.data = value;
            }
        }

        private object body;
        private IObjectSerializer serializer;

        public OutMessage()
        {
        }

        public OutMessage(MessageVersion version, object body, IObjectSerializer serializer)
            : base(version)
        {
            if (body != null && serializer == null)
                throw new ArgumentNullException("serializer");
            this.body = body;
            this.serializer = serializer;
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            if (body != null)
            {
                byte[] data = serializer.Serialize(body);
                writer.WriteBase64(data, 0, data.Length);
            }
        }
    }
}