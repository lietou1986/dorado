using System;
using System.ServiceModel;

namespace Dorado.ESB.Communication
{
    public class SocketReceiveAsyncResult : AsyncResult
    {
        #region fields

        private WseTcpChannel channel;
        private int size;
        private byte[] buffer;
        private bool throwOnEmpty;
        private int bytesReadTotal;

        private static AsyncCallback onReadBytesComplete = new AsyncCallback(OnReadBytesComplete);

        #endregion fields

        public SocketReceiveAsyncResult(WseTcpChannel channel, int size, bool throwOnEmpty, AsyncCallback callback, object state)
            : base(callback, state)
        {
            this.channel = channel;
            this.size = size;
            this.buffer = channel.TakeBuffer(size);
            this.throwOnEmpty = throwOnEmpty;
            this.bytesReadTotal = 0;

            bool success = false;
            try
            {
                IAsyncResult socketReceiveResult = channel.BeginSocketReceive(buffer, bytesReadTotal, size, onReadBytesComplete, this);
                if (socketReceiveResult.CompletedSynchronously)
                {
                    if (CompleteReadBytes(socketReceiveResult))
                        base.Complete(true);
                }
                success = true;
            }
            finally
            {
                if (!success) Cleanup();
            }
        }

        private bool CompleteReadBytes(IAsyncResult result)
        {
            int bytesRead = channel.EndSocketReceive(result);
            bytesReadTotal += bytesRead;
            if (bytesRead == 0)
            {
                if (size == 0 && !throwOnEmpty)
                {
                    channel.ReturnBuffer(this.buffer);
                    this.buffer = null;
                    return true;
                }
                else
                    throw new CommunicationObjectFaultedException("Premature EOF reached");
            }

            while (bytesReadTotal < size)
            {
                IAsyncResult socketReceiveResult = channel.BeginSocketReceive(buffer, bytesReadTotal, size - bytesReadTotal, onReadBytesComplete, this);
                if (!socketReceiveResult.CompletedSynchronously)
                    return false;
            }

            return true;
        }

        private void Cleanup()
        {
            if (this.buffer != null)
            {
                channel.ReturnBuffer(this.buffer);
                this.buffer = null;
            }
        }

        private static void OnReadBytesComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously) return;

            bool complete = false;
            Exception completionException = null;
            SocketReceiveAsyncResult socketReceiveResult = (SocketReceiveAsyncResult)result.AsyncState;
            try
            {
                complete = socketReceiveResult.CompleteReadBytes(result);
            }
            catch (Exception ex)
            {
                complete = true;
                completionException = ex;
                socketReceiveResult.Cleanup();
            }

            if (complete) socketReceiveResult.Complete(false, completionException);
        }

        public static byte[] End(IAsyncResult result)
        {
            SocketReceiveAsyncResult socketReceiveResult = AsyncResult.End<SocketReceiveAsyncResult>(result);
            return socketReceiveResult.buffer;
        }
    }
}