using System;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Dorado.ESB.Communication
{
    public class AcceptChannelAsyncResult : AsyncResult
    {
        private IDuplexSessionChannel channel;
        private WseTcpChannelListener listener;
        private TimeSpan timeout;

        private static AsyncCallback onAcceptComplete = new AsyncCallback(OnAcceptComplete);

        public AcceptChannelAsyncResult(WseTcpChannelListener listener, TimeSpan timeout, AsyncCallback callback, object state)
            : base(callback, state)
        {
            this.listener = listener;
            this.timeout = timeout;

            IAsyncResult acceptResult = listener.ListenSocket.BeginAccept(onAcceptComplete, this);
            if (!acceptResult.CompletedSynchronously) return;

            if (CompleteAccept(acceptResult))
                base.Complete(true);
        }

        private bool CompleteAccept(IAsyncResult result)
        {
            Socket dataSocket = listener.ListenSocket.EndAccept(result);
            this.channel = new ServerWseTcpChannel(listener.EncoderFactory, listener.BufferManager,
                new EndpointAddress(listener.Uri), listener, dataSocket);
            return true;
        }

        private static void OnAcceptComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously) return;

            AcceptChannelAsyncResult acceptResult = (AcceptChannelAsyncResult)result.AsyncState;

            bool complete = false;
            Exception completionException = null;
            try
            {
                complete = acceptResult.CompleteAccept(result);
            }
            catch (Exception ex)
            {
                complete = true;
                completionException = ex;
            }

            if (complete) acceptResult.Complete(false, completionException);
        }

        public static IDuplexSessionChannel End(IAsyncResult result)
        {
            AcceptChannelAsyncResult acceptResult = AsyncResult.End<AcceptChannelAsyncResult>(result);
            return acceptResult.channel;
        }
    }
}