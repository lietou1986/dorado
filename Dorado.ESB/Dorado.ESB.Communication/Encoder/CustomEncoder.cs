using System;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;

namespace Dorado.ESB.Communication
{
    public abstract class CustomEncoder : MessageEncoder
    {
        protected IObjectSerializer serializer;

        public CustomEncoder(IObjectSerializer serializer)
        {
            if (serializer == null) throw new ArgumentNullException("serializer");
            this.serializer = serializer;
        }

        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {
            byte[] data = new byte[buffer.Count];
            Array.Copy(buffer.Array, buffer.Offset, data, 0, data.Length);
            bufferManager.ReturnBuffer(buffer.Array);

            InMessage message = (InMessage)serializer.Deserialize(data, typeof(InMessage));
            message.Properties.Encoder = this;
            return message;
        }

        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                byte[] data = Encoding.UTF8.GetBytes(reader.ReadToEnd());
                InMessage message = (InMessage)serializer.Deserialize(data, typeof(InMessage));
                message.Properties.Encoder = this;
                return message;
            }
        }

        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            byte[] messageBytes = serializer.Serialize(message);
            int messageLength = messageBytes.Length;
            int totalLength = messageOffset + messageLength;
            byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
            Array.Copy(messageBytes, 0, totalBytes, messageOffset, messageLength);

            return new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
        }

        public override void WriteMessage(Message message, Stream stream)
        {
            byte[] messageBytes = serializer.Serialize(message);
            stream.Write(messageBytes, 0, messageBytes.Length);
        }
    }
}