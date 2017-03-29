using System;
using System.Net;
using System.Net.Sockets;

namespace Dorado.ESB.Communication
{
    public class ConnectAsyncResult : AsyncResult
    {
        private ClientWseTcpChannel channel;
        private TimeSpan timeout;
        private IPHostEntry hostEntry;
        private Socket socket;
        private bool connected;
        private int currentEntry;
        private int port;

        private static AsyncCallback onDnsGetHostComplete = new AsyncCallback(OnDnsGetHostComplete);
        private static AsyncCallback onSocketConnectComplete = new AsyncCallback(OnSocketConnectComplete);

        public ConnectAsyncResult(ClientWseTcpChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
            : base(callback, state)
        {
            this.channel = channel;
            this.timeout = timeout;

            IAsyncResult dnsGetHostResult = Dns.BeginGetHostEntry(channel.Via.Host, onDnsGetHostComplete, this);
            if (!dnsGetHostResult.CompletedSynchronously) return;

            if (CompleteDnsGetHost(dnsGetHostResult))
                base.Complete(true);
        }

        private IAsyncResult BeginSocketConnect()
        {
            IPAddress address = hostEntry.AddressList[currentEntry];
            socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            while (true)
            {
                try
                {
                    return socket.BeginConnect(new IPEndPoint(address, port), onSocketConnectComplete, this);
                }
                catch (SocketException ex)
                {
                    if (currentEntry >= (hostEntry.AddressList.Length - 1))
                        throw WseTcpChannel.ConvertSocketException(ex, "Connect");
                    ++currentEntry;
                }
            }
        }

        private bool CompleteDnsGetHost(IAsyncResult result)
        {
            try
            {
                hostEntry = Dns.EndGetHostEntry(result);
            }
            catch (SocketException ex)
            {
                throw new EntryPointNotFoundException("Unable to resolve host " + channel.Via.Host, ex);
            }

            port = this.channel.Via.Port;
            if (port == -1) port = 8081;

            IAsyncResult connectResult = BeginSocketConnect();
            if (!connectResult.CompletedSynchronously)
                return false;

            return CompleteSocketConnect(connectResult);
        }

        private bool CompleteSocketConnect(IAsyncResult result)
        {
            while (!connected && currentEntry < hostEntry.AddressList.Length)
            {
                try
                {
                    socket.EndConnect(result);
                    connected = true;
                    break;
                }
                catch (SocketException ex)
                {
                    if (currentEntry >= (hostEntry.AddressList.Length - 1))
                        throw WseTcpChannel.ConvertSocketException(ex, "Connect");
                    ++currentEntry;
                }

                result = BeginSocketConnect();
                if (!result.CompletedSynchronously)
                    return false;
            }

            this.channel.InitializeSocket(socket);
            return true;
        }

        private static void OnDnsGetHostComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously) return;

            ConnectAsyncResult connectResult = (ConnectAsyncResult)result.AsyncState;

            bool complete = false;
            Exception completionException = null;
            try
            {
                complete = connectResult.CompleteDnsGetHost(result);
            }
            catch (Exception ex)
            {
                complete = true;
                completionException = ex;
            }

            if (complete) connectResult.Complete(false, completionException);
        }

        private static void OnSocketConnectComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously) return;

            ConnectAsyncResult connectResult = (ConnectAsyncResult)result.AsyncState;

            bool complete = false;
            Exception completionException = null;
            try
            {
                complete = connectResult.CompleteSocketConnect(result);
            }
            catch (Exception ex)
            {
                complete = true;
                completionException = ex;
            }

            if (complete) connectResult.Complete(false, completionException);
        }

        public static void End(IAsyncResult result)
        {
            AsyncResult.End<ConnectAsyncResult>(result);
        }
    }
}