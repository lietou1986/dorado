using System.ServiceModel.Channels;
using System.Xml;

namespace Dorado.ESB.Communication
{
    public class MessageBase : Message
    {
        #region Message Members

        private MessageVersion version;

        public override MessageVersion Version
        {
            get { return version; }
        }

        private MessageHeaders headers;

        public override MessageHeaders Headers
        {
            get { return headers; }
        }

        private MessageProperties properties;

        public override MessageProperties Properties
        {
            get { return properties; }
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
        }

        #endregion Message Members

        #region ctor

        public MessageBase(MessageVersion version)
        {
            this.version = MessageVersion.Default;
            this.headers = new MessageHeaders(MessageVersion.Default);
            this.properties = new MessageProperties();
        }

        public MessageBase()
            : this(MessageVersion.None)
        {
        }

        #endregion ctor
    }
}