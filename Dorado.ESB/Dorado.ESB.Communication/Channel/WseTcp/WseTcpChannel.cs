using System;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Dorado.ESB.Communication
{
    public abstract class WseTcpChannel : CustomDuplexSessionChannel
    {
        #region fields

        private Socket socket;

        public Socket Socket
        {
            get { return socket; }
        }

        #endregion fields

        #region wse constants

        public byte[] WseType
        {
            get
            {
                if (this.Encoder.MessageVersion.Envelope == EnvelopeVersion.Soap11)
                    return WseType1;
                return WseType2;
            }
        }

        public static readonly byte[] WseId = { 0x00, 0x00, 0x00, 0x00 };

        public static readonly byte[] WseType1 = Encoding.UTF8.GetBytes("http://schemas.xmlsoap.org/soap/envelope/");
        public static readonly byte[] WseType2 = Encoding.UTF8.GetBytes("http://www.w3.org/2003/05/soap-envelope");

        public static readonly int WseAmbleLength = 12;

        public static readonly byte[] WsePreamble = { // WSE 3.0 uses the SOAP namespace
                0x0E, // version 0x01+MB+ME
                0x20, 0, 0 }; // TYPE_T=URI, no options

        public static readonly byte[] WsePostamble = {
            0x0A, 0x40, 0, 0, // version 0x01+ME, no type, no options
            0, 0, 0, 0, 0, 0, 0, 0 }; // no lengths

        #endregion wse constants

        #region wse helper

        public void WseGetIdAndTypeLength(byte[] preambleBytes, out int idLen, out int typeLen)
        {
            // drain the ID + TYPE
            idLen = (preambleBytes[4] << 8) + preambleBytes[5];
            typeLen = (preambleBytes[6] << 8) + preambleBytes[7];

            // need to also drain padding
            if (idLen % 4 > 0) idLen += 4 - idLen % 4;
            if (typeLen % 4 > 0) typeLen += 4 - typeLen % 4;
        }

        public void WseGetDataLength(byte[] preambleBytes, out int dataLen, out int bytesToRead)
        {
            // now read the data
            dataLen = (preambleBytes[8] << 24)
                + (preambleBytes[9] << 16)
                + (preambleBytes[10] << 8)
                + preambleBytes[11];

            // total to read should include padding
            bytesToRead = dataLen;
            if (bytesToRead % 4 > 0)
                bytesToRead += 4 - bytesToRead % 4;
        }

        public ArraySegment<byte> WseGetPreDataBytes(ArraySegment<byte> data)
        {
            byte[] typeBytes = this.WseType;

            byte[] lengthBytes = new byte[] {
                (byte)((WseId.Length >> 8) & 0xff),
                (byte)(WseId.Length & 0xff),
                (byte)((typeBytes.Length >> 8) & 0xff),
                (byte)(typeBytes.Length & 0xff),
                (byte)((data.Count >> 24) & 0xff),
                (byte)((data.Count >> 16) & 0xff),
                (byte)((data.Count >> 8) & 0xff),
                (byte)(data.Count & 0xff)
            };

            // need to pad to multiple of 4 bytes
            int padLen = 4 - typeBytes.Length % 4;

            int total = WsePreamble.Length + lengthBytes.Length + WseId.Length + typeBytes.Length + padLen;

            int offset = 0;
            byte[] buffer = this.TakeBuffer(total);

            Buffer.BlockCopy(WsePreamble, 0, buffer, offset, WsePreamble.Length);
            offset += WsePreamble.Length;

            Buffer.BlockCopy(lengthBytes, 0, buffer, offset, lengthBytes.Length);
            offset += lengthBytes.Length;

            Buffer.BlockCopy(WseId, 0, buffer, offset, WseId.Length);
            offset += WseId.Length;

            Buffer.BlockCopy(typeBytes, 0, buffer, offset, typeBytes.Length);

            return new ArraySegment<byte>(buffer, 0, total);
        }

        public void WseVerifyPostamble(byte[] endRecord)
        {
            for (int i = 0; i < WseAmbleLength; ++i)
            {
                if (endRecord[i] != WsePostamble[i])
                {
                    this.ReturnBuffer(endRecord);
                    throw new CommunicationException("Invalid second framing record");
                }
            }
        }

        #endregion wse helper

        #region ctor

        public WseTcpChannel(MessageEncoderFactory encoderFactory, BufferManager bufferManager,
            EndpointAddress remoteAddress, EndpointAddress localAddress, Uri via, ChannelManagerBase channelManager)
            : base(encoderFactory, bufferManager, remoteAddress, localAddress, via, channelManager)
        {
        }

        #endregion ctor

        #region CustomDuplexSessionChannel Members

        protected override void OnOpen(TimeSpan timeout)
        {
            // has nothing to do
        }

        protected override void OnClose(TimeSpan timeout)
        {
            socket.Close((int)timeout.TotalMilliseconds);
        }

        protected override void OnAbort()
        {
            if (this.socket != null)
                socket.Close(0);
        }

        protected override ArraySegment<byte> ReadData()
        {
            byte[] preambleBytes = SocketReceiveBytes(WseAmbleLength, false);
            if (preambleBytes == null)
                return new ArraySegment<byte>();

            int idLen, typeLen;
            WseGetIdAndTypeLength(preambleBytes, out idLen, out typeLen);
            byte[] dummy = SocketReceiveBytes(idLen + typeLen);
            this.ReturnBuffer(dummy);

            int dataLen, bytesToRead;
            WseGetDataLength(preambleBytes, out dataLen, out bytesToRead);
            byte[] data = SocketReceiveBytes(bytesToRead);

            if ((preambleBytes[0] & 0x02) == 0)
            {
                byte[] endRecord = SocketReceiveBytes(WseAmbleLength);
                WseVerifyPostamble(endRecord);
                this.ReturnBuffer(endRecord);
            }

            this.ReturnBuffer(preambleBytes);

            return new ArraySegment<byte>(data, 0, dataLen);
        }

        protected override void WriteData(ArraySegment<byte> data)
        {
            ArraySegment<byte> buffer = WseGetPreDataBytes(data);
            try
            {
                SocketSend(buffer);
                SocketSend(data);
                if ((data.Count % 4) > 0) // need to pad data to multiple of 4 bytes
                {
                    byte[] padBytes = new byte[4 - data.Count % 4];
                    SocketSend(padBytes);
                }
            }
            finally
            {
                this.ReturnBuffer(buffer.Array);
            }
        }

        public override IAsyncResult BeginReadData(AsyncCallback callback, object state)
        {
            return new ReadDataAsyncResult(this, callback, state);
        }

        public override ArraySegment<byte> EndReadData(IAsyncResult result)
        {
            return ReadDataAsyncResult.End(result);
        }

        public override IAsyncResult BeginWriteData(ArraySegment<byte> data, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new WriteDataAsyncResult(this, data, timeout, callback, state);
        }

        public override void EndWriteData(IAsyncResult result)
        {
            WriteDataAsyncResult.End(result);
        }

        public override void CloseOutput(TimeSpan timeout)
        {
            this.socket.Shutdown(SocketShutdown.Send);
        }

        #endregion CustomDuplexSessionChannel Members

        #region socket operation

        public void InitializeSocket(Socket socket)
        {
            if (this.socket != null)
                throw new InvalidOperationException("Socket is already set");
            this.socket = socket;
        }

        private void SocketSend(byte[] buffer)
        {
            SocketSend(new ArraySegment<byte>(buffer));
        }

        private void SocketSend(ArraySegment<byte> buffer)
        {
            try
            {
                socket.Send(buffer.Array, buffer.Offset, buffer.Count, SocketFlags.None);
            }
            catch (SocketException ex)
            {
                throw ConvertSocketException(ex, "Send");
            }
        }

        private int SocketReceive(byte[] buffer, int offset, int size)
        {
            try
            {
                return socket.Receive(buffer, offset, size, SocketFlags.None);
            }
            catch (SocketException ex)
            {
                throw ConvertSocketException(ex, "Receive");
            }
        }

        private byte[] SocketReceiveBytes(int size)
        {
            return SocketReceiveBytes(size, true);
        }

        private byte[] SocketReceiveBytes(int size, bool throwOnEmpty)
        {
            int bytesReadTotal = 0;
            byte[] data = this.TakeBuffer(size);
            while (bytesReadTotal < size)
            {
                int bytesRead = SocketReceive(data, bytesReadTotal, size - bytesReadTotal);
                bytesReadTotal += bytesRead;
                if (bytesRead == 0)
                {
                    if (bytesReadTotal == 0 && !throwOnEmpty)
                    {
                        this.ReturnBuffer(data);
                        return null;
                    }
                    throw new CommunicationException("Premature EOF reached");
                }
            }
            return data;
        }

        public IAsyncResult BeginSocketSend(byte[] buffer, AsyncCallback callback, object state)
        {
            return BeginSocketSend(new ArraySegment<byte>(buffer), callback, state);
        }

        public IAsyncResult BeginSocketSend(ArraySegment<byte> buffer, AsyncCallback callback, object state)
        {
            return new SocketSendAsyncResult(this, buffer, callback, state);
        }

        public void EndSocketSend(IAsyncResult result)
        {
            SocketSendAsyncResult.End(result);
        }

        public IAsyncResult BeginSocketReceive(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            try
            {
                return socket.BeginReceive(buffer, offset, size, SocketFlags.None, callback, state);
            }
            catch (SocketException ex)
            {
                throw ConvertSocketException(ex, "BeginReceive");
            }
        }

        public int EndSocketReceive(IAsyncResult result)
        {
            try
            {
                return socket.EndReceive(result);
            }
            catch (SocketException ex)
            {
                throw ConvertSocketException(ex, "EndReceive");
            }
        }

        public IAsyncResult BeginSocketReceiveBytes(int size, AsyncCallback callback, object state)
        {
            return BeginSocketReceiveBytes(size, true, callback, state);
        }

        public IAsyncResult BeginSocketReceiveBytes(int size, bool throwOnEmpty, AsyncCallback callback, object state)
        {
            return new SocketReceiveAsyncResult(this, size, throwOnEmpty, callback, state);
        }

        public byte[] EndSocketReceiveBytes(IAsyncResult result)
        {
            return SocketReceiveAsyncResult.End(result);
        }

        public static Exception ConvertSocketException(SocketException socketException, string operation)
        {
            if (socketException.ErrorCode == 10049 // WSAEADDRNOTAVAIL
                || socketException.ErrorCode == 10061 // WSAECONNREFUSED
                || socketException.ErrorCode == 10050 // WSAENETDOWN
                || socketException.ErrorCode == 10051 // WSAENETUNREACH
                || socketException.ErrorCode == 10064 // WSAEHOSTDOWN
                || socketException.ErrorCode == 10065 // WSAEHOSTUNREACH
            )
            {
                return new EndpointNotFoundException(string.Format(operation + " error: {0} ({1})",
                    socketException.Message, socketException.ErrorCode), socketException);
            }

            if (socketException.ErrorCode == 10060) // WSAETIMEDOUT
                return new TimeoutException(operation + " timed out", socketException);

            return new CommunicationObjectFaultedException(string.Format(operation + " error: {0} ({1})",
                socketException.Message, socketException.ErrorCode), socketException);
        }

        #endregion socket operation
    }
}