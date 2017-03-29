using System;
using System.ServiceModel.Channels;

namespace Dorado.ESB.Communication
{
    public class ReceiveAsyncResult : AsyncResult
    {
        #region fields

        private static AsyncCallback onReadDataComplete = new AsyncCallback(OnReadDataComplete);

        private CustomDuplexSessionChannel channel;
        private Message message;

        #endregion fields

        #region ctor

        public ReceiveAsyncResult(CustomDuplexSessionChannel channel,
            TimeSpan timeout, AsyncCallback callback, object state)
            : base(callback, state)
        {
            this.channel = channel;

            IAsyncResult readDataResult = channel.BeginReadData(onReadDataComplete, this);
            if (readDataResult.CompletedSynchronously)
            {
                CompleteReadData(readDataResult);
                base.Complete(true);
            }
        }

        #endregion ctor

        private void CompleteReadData(IAsyncResult result)
        {
            ArraySegment<byte> data = channel.EndReadData(result);
            this.message = channel.DecodeMessage(data);
        }

        private static void OnReadDataComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously) return;

            Exception completionException = null;
            ReceiveAsyncResult receiveResult = (ReceiveAsyncResult)result.AsyncState;
            try
            {
                receiveResult.CompleteReadData(result);
            }
            catch (Exception ex)
            {
                completionException = ex;
            }

            receiveResult.Complete(false, completionException);
        }

        public static Message End(IAsyncResult result)
        {
            ReceiveAsyncResult receiveResult = AsyncResult.End<ReceiveAsyncResult>(result);
            return receiveResult.message;
        }
    }
}