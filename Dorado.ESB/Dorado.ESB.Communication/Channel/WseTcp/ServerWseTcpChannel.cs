using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Dorado.ESB.Communication
{
    public class ServerWseTcpChannel : WseTcpChannel
    {
        public ServerWseTcpChannel(MessageEncoderFactory encoderFactory, BufferManager bufferManager,
            EndpointAddress localAddress, ChannelManagerBase channelManager, Socket socket)
            : base(encoderFactory, bufferManager, CustomDuplexSessionChannel.AnonymousAddress, localAddress,
                   CustomDuplexSessionChannel.AnonymousAddress.Uri, channelManager)
        {
            base.InitializeSocket(socket);
        }
    }
}