using System;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Dorado.ESB.Communication
{
    public class ClientWseTcpChannel : WseTcpChannel
    {
        public ClientWseTcpChannel(MessageEncoderFactory encoderFactory, BufferManager bufferManager,
            EndpointAddress remoteAddress, Uri via, ChannelManagerBase channelManager)
            : base(encoderFactory, bufferManager, remoteAddress, CustomDuplexSessionChannel.AnonymousAddress, via, channelManager)
        {
        }

        private void Connect()
        {
            int port = base.Via.Port;
            if (port == -1) port = 8081; // the default port used by wse 3.0

            IPHostEntry hostEntry;
            try
            {
                hostEntry = Dns.GetHostEntry(base.Via.Host);
            }
            catch (SocketException ex)
            {
                throw new EndpointNotFoundException("Unable to resolve host " + base.Via.Host, ex);
            }

            Socket socket = null;
            for (int i = 0; i < hostEntry.AddressList.Length; ++i)
            {
                try
                {
                    IPAddress address = hostEntry.AddressList[i];
                    socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(new IPEndPoint(address, port));
                    break;
                }
                catch (SocketException ex)
                {
                    if (i >= (hostEntry.AddressList.Length - 1))
                        throw ConvertSocketException(ex, "Connect");
                }
            }

            base.InitializeSocket(socket);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            Connect();
            base.OnOpen(timeout);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new ConnectAsyncResult(this, timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            ConnectAsyncResult.End(result);
        }
    }
}