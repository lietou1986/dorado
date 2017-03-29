using System.ServiceModel.Channels;
using System.Text;

namespace Dorado.ESB.Json
{
    public class JsonMessage : Message
    {
        #region Payload: Json data

        private byte[] jsonData;

        public byte[] JsonData
        {
            get { return jsonData; }
        }

        #endregion Payload: Json data

        #region Message Members

        public override MessageVersion Version
        {
            get { return MessageVersion.None; }
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

        private bool isEmpty;

        public override bool IsEmpty
        {
            get { return isEmpty; }
        }

        protected override void OnWriteBodyContents(System.Xml.XmlDictionaryWriter writer)
        {
        }

        #endregion Message Members

        #region ctor

        public JsonMessage(byte[] data)
        {
            headers = new MessageHeaders(Version);
            properties = new MessageProperties();
            jsonData = data;
            isEmpty = (data == null);
        }

        #endregion ctor

        public static string GetJsonString(Message message)
        {
            JsonMessage jsonMessage = message as JsonMessage;
            if (jsonMessage == null)
                return null;
            else
                return Encoding.UTF8.GetString(jsonMessage.JsonData);
        }
    }
}