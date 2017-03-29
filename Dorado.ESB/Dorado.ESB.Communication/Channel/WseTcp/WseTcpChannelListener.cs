using System;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Dorado.ESB.Communication
{
    public class WseTcpChannelListener : CustomChannelListener
    {
        #region fields

        private Socket listenSocket;

        public Socket ListenSocket
        {
            get { return listenSocket; }
        }

        #endregion fields

        #region ctor

        public WseTcpChannelListener(CustomTransportBindingElement bindingElement, BindingContext context)
            : base(bindingElement, context)
        {
        }

        #endregion ctor

        #region ChannelListenerBase Members

        protected override IDuplexSessionChannel OnAcceptChannel(TimeSpan timeout)
        {
            Socket dataSocket = listenSocket.Accept();
            return new ServerWseTcpChannel(this.EncoderFactory, this.BufferManager, new EndpointAddress(this.Uri), this, dataSocket);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            OpenListenSocket();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            CloseListenSocket(timeout);
        }

        protected override void OnAbort()
        {
            CloseListenSocket(TimeSpan.Zero);
        }

        protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new AcceptChannelAsyncResult(this, timeout, callback, state);
        }

        protected override IDuplexSessionChannel OnEndAcceptChannel(IAsyncResult result)
        {
            return AcceptChannelAsyncResult.End(result);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            OpenListenSocket();
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            CloseListenSocket(timeout);
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        #endregion ChannelListenerBase Members

        #region socket operation

        private void OpenListenSocket()
        {
            if (Uri == null)
                throw new InvalidOperationException("Uri is null");

            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Uri.Port);
            this.listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.listenSocket.Bind(localEndPoint);
            this.listenSocket.Listen(10);
        }

        private void CloseListenSocket(TimeSpan timeout)
        {
            this.listenSocket.Close((int)timeout.TotalMilliseconds);
        }

        #endregion socket operation
    }
}