using System;
using System.Net.Sockets;

namespace Dorado.ESB.Communication
{
    public class SocketSendAsyncResult : AsyncResult
    {
        #region fields

        private WseTcpChannel channel;
        private static AsyncCallback onSendComplete = new AsyncCallback(OnSendComplete);

        #endregion fields

        public SocketSendAsyncResult(WseTcpChannel channel, ArraySegment<byte> buffer, AsyncCallback callback, object state)
            : base(callback, state)
        {
            this.channel = channel;

            IAsyncResult sendResult = channel.Socket.BeginSend(buffer.Array, buffer.Offset, buffer.Count, SocketFlags.None, onSendComplete, this);
            if (sendResult.CompletedSynchronously)
            {
                CompleteSend(sendResult);
                base.Complete(true);
            }
        }

        private void CompleteSend(IAsyncResult result)
        {
            try
            {
                channel.Socket.EndSend(result);
            }
            catch (SocketException ex)
            {
                throw WseTcpChannel.ConvertSocketException(ex, "Send");
            }
        }

        private static void OnSendComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously) return;

            Exception completionException = null;
            SocketSendAsyncResult socketSendResult = (SocketSendAsyncResult)result.AsyncState;
            try
            {
                socketSendResult.CompleteSend(result);
            }
            catch (Exception ex)
            {
                completionException = ex;
            }

            socketSendResult.Complete(false, completionException);
        }

        public static void End(IAsyncResult result)
        {
            AsyncResult.End<SocketSendAsyncResult>(result);
        }
    }
}