using System;
using System.Diagnostics;
using System.IO;
using System.ServiceModel.Channels;

namespace Dorado.Wcf.MiniEncoder
{
    /// <summary>
    /// 自定义编码器
    /// </summary>
    public class MiniEncoder : MessageEncoder
    {
        private MiniEncoderFactory _factory;
        private MessageEncoder _innserEncoder;
        private CompressAlgorithm _algorithm;

        public MiniEncoder(MiniEncoderFactory encoderFactory, CompressAlgorithm algorithm)
        {
            _factory = encoderFactory;
            _algorithm = algorithm;
            _innserEncoder = _factory.InnerMessageEncodingBindingElement.CreateMessageEncoderFactory().Encoder;
        }

        public override string ContentType
        {
            get { return _innserEncoder.ContentType; }
        }

        public override string MediaType
        {
            get { return _innserEncoder.MediaType; }
        }

        public override MessageVersion MessageVersion
        {
            get { return _innserEncoder.MessageVersion; }
        }

        public override bool IsContentTypeSupported(string contentType)
        {
            return _innserEncoder.IsContentTypeSupported(contentType);
        }

        public override T GetProperty<T>()
        {
            return _innserEncoder.GetProperty<T>();
        }

        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {
            ArraySegment<byte> bytes = new Compressor(_algorithm).DeCompress(buffer);
            int totalLength = bytes.Count;
            byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
            Array.Copy(bytes.Array, 0, totalBytes, 0, bytes.Count);
            ArraySegment<byte> byteArray = new ArraySegment<byte>(totalBytes, 0, bytes.Count);
            bufferManager.ReturnBuffer(byteArray.Array);
            Message msg = _innserEncoder.ReadMessage(byteArray, bufferManager, contentType);
            return msg;
        }

        public override Message ReadMessage(System.IO.Stream stream, int maxSizeOfHeaders, string contentType)
        {
            //读取消息的时候，二进制流为加密的，需要解压
            Stream ms = new Compressor(_algorithm).DeCompress(stream);
            Message msg = _innserEncoder.ReadMessage(ms, maxSizeOfHeaders, contentType);
            return msg;
        }

        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            ArraySegment<byte> bytes = _innserEncoder.WriteMessage(message, maxMessageSize, bufferManager);
            ArraySegment<byte> buffer = new Compressor(_algorithm).Compress(bytes);
            int totalLength = buffer.Count + messageOffset;
            byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
            Array.Copy(buffer.Array, 0, totalBytes, messageOffset, buffer.Count);
            ArraySegment<byte> byteArray = new ArraySegment<byte>(totalBytes, messageOffset, buffer.Count);
            Trace(bytes.Count, byteArray.Count);
            return byteArray;
        }

        public override void WriteMessage(Message message, System.IO.Stream stream)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            _innserEncoder.WriteMessage(message, ms);
            stream = new Compressor(_algorithm).Compress(ms);
        }

        [Conditional("DEBUG")]
        private void Trace(int rawCount, int newCount)
        {
            Console.WriteLine("算法:" + _algorithm + ",原来字节流大小:" + rawCount + ",压缩后字节流大小:" + newCount);
        }
    }
}