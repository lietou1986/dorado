using System;
using System.Runtime.Serialization;
using System.Xml;

namespace Dorado.ESB.Communication
{
    [DataContract]
    public class InMessage : MessageBase
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

        private byte[] data; // payload

        [DataMember]
        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }

        public override bool IsEmpty
        {
            get { return (data == null || data.Length <= 0); }
        }

        public InMessage()
        {
        }

        public InMessage(byte[] data)
        {
            this.data = data;
        }
    }
}