using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Dorado.ESB.Communication
{
    public abstract class CustomDuplexSessionChannel : ChannelBase, IDuplexSessionChannel
    {
        #region fields

        private const int MaxBufferSize = 64 * 1024;
        private BufferManager bufferManager;

        private MessageEncoder encoder;

        public MessageEncoder Encoder
        {
            get { return encoder; }
        }

        private object readLock = new object();
        private object writeLock = new object();

        internal static readonly EndpointAddress AnonymousAddress =
            new EndpointAddress("http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous");

        #endregion fields

        #region ctor

        public CustomDuplexSessionChannel(MessageEncoderFactory encoderFactory, BufferManager bufferManager,
            EndpointAddress remoteAddress, EndpointAddress localAddress, Uri via, ChannelManagerBase channelManager)
            : base(channelManager)
        {
            this.remoteAddress = remoteAddress;
            this.localAddress = localAddress;
            this.via = via;
            this.session = new CustomDuplexSession(this);
            this.encoder = encoderFactory.CreateSessionEncoder();
            this.bufferManager = bufferManager;
        }

        #endregion ctor

        #region ChannelBase Members

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            OnOpen(timeout);
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            OnClose(timeout);
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        #endregion ChannelBase Members

        #region IInputChannel Members

        private EndpointAddress localAddress;

        public EndpointAddress LocalAddress
        {
            get { return localAddress; }
        }

        public Message Receive()
        {
            return Receive(this.DefaultReceiveTimeout);
        }

        public Message Receive(TimeSpan timeout)
        {
            base.ThrowIfDisposedOrNotOpen();
            lock (readLock)
            {
                ArraySegment<byte> encodedBytes = ReadData();
                return DecodeMessage(encodedBytes);
            }
        }

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            try
            {
                message = Receive(timeout);
                return true;
            }
            catch (TimeoutException)
            {
                message = null;
                return false;
            }
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return BeginReceive(this.DefaultReceiveTimeout, callback, state);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new ReceiveAsyncResult(this, timeout, callback, state);
        }

        public Message EndReceive(IAsyncResult result)
        {
            return ReceiveAsyncResult.End(result);
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            base.ThrowIfDisposedOrNotOpen();
            return new TryReceiveAsyncResult(this, timeout, callback, state);
        }

        public bool EndTryReceive(IAsyncResult result, out Message message)
        {
            try
            {
                return TryReceiveAsyncResult.End(result, out message);
            }
            catch (TimeoutException)
            {
                message = null;
                return false;
            }
        }

        #region WaitForMessage: not implemented

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        #endregion WaitForMessage: not implemented

        #endregion IInputChannel Members

        #region IOutputChannel Members

        private EndpointAddress remoteAddress;

        public EndpointAddress RemoteAddress
        {
            get { return remoteAddress; }
        }

        private Uri via;

        public Uri Via
        {
            get { return via; }
        }

        public void Send(Message message)
        {
            Send(message, this.DefaultSendTimeout);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            base.ThrowIfDisposedOrNotOpen();
            ArraySegment<byte> encodedBytes = EncodeMessage(message);
            lock (writeLock)
            {
                WriteData(encodedBytes);
            }
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            return BeginSend(message, this.DefaultSendTimeout, callback, state);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            base.ThrowIfDisposedOrNotOpen();
            return new SendAsyncResult(this, message, timeout, callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            SendAsyncResult.End(result);
        }

        public void CloseOutput()
        {
            CloseOutput(base.DefaultCloseTimeout);
        }

        #endregion IOutputChannel Members

        #region ISessionChannel<IDuplexSession> Members

        private IDuplexSession session;

        public IDuplexSession Session
        {
            get { return this.session; }
        }

        #endregion ISessionChannel<IDuplexSession> Members

        #region BufferManager Members

        public void ReturnBuffer(byte[] buffer)
        {
            bufferManager.ReturnBuffer(buffer);
        }

        public byte[] TakeBuffer(int size)
        {
            return bufferManager.TakeBuffer(size);
        }

        #endregion BufferManager Members

        #region Message Encoding

        // Address the message and serialize it into a byte array
        public ArraySegment<byte> EncodeMessage(Message message)
        {
            try
            {
                //this.RemoteAddress.ApplyTo(message);
                return encoder.WriteMessage(message, MaxBufferSize, bufferManager);
            }
            finally
            {
                // We've consumed the message by serializing it, so clean up
                message.Close();
            }
        }

        public Message DecodeMessage(ArraySegment<byte> data)
        {
            if (data.Array == null)
                return null;
            else
                return encoder.ReadMessage(data, bufferManager);
        }

        #endregion Message Encoding

        #region abstract members

        // ChannelBase Members
        // protected override void OnOpen(TimeSpan timeout)
        // protected override void OnClose(TimeSpan timeout)
        // protected override void OnAbort()

        // IInputChannel Members helper
        protected abstract ArraySegment<byte> ReadData();

        public abstract IAsyncResult BeginReadData(AsyncCallback callback, object state);

        public abstract ArraySegment<byte> EndReadData(IAsyncResult result);

        // IOutputChannel Members helper
        protected abstract void WriteData(ArraySegment<byte> data);

        public abstract IAsyncResult BeginWriteData(ArraySegment<byte> data, TimeSpan timeout, AsyncCallback callback, object state);

        public abstract void EndWriteData(IAsyncResult result);

        public abstract void CloseOutput(TimeSpan timeout);

        #endregion abstract members
    }
}