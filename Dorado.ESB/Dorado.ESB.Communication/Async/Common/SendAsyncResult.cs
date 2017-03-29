using System;
using System.ServiceModel.Channels;

namespace Dorado.ESB.Communication
{
    public class SendAsyncResult : AsyncResult
    {
        #region fields

        private CustomDuplexSessionChannel channel;
        private AsyncCallback onWriteComplete = new AsyncCallback(OnWriteComplete);

        #endregion fields

        #region ctor

        public SendAsyncResult(CustomDuplexSessionChannel channel, Message message, TimeSpan timeout, AsyncCallback callback, object state)
            : base(callback, state)
        {
            this.channel = channel;

            ArraySegment<byte> encodedBytes = this.channel.EncodeMessage(message);
            IAsyncResult writeResult = this.channel.BeginWriteData(encodedBytes, timeout, onWriteComplete, this);
            if (writeResult.CompletedSynchronously)
            {
                CompleteWrite(writeResult);
                base.Complete(true);
            }
        }

        #endregion ctor

        private void CompleteWrite(IAsyncResult result)
        {
            this.channel.EndWriteData(result);
        }

        private static void OnWriteComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously) return;

            Exception completionException = null;
            SendAsyncResult sendResult = (SendAsyncResult)result.AsyncState;
            try
            {
                sendResult.CompleteWrite(result);
            }
            catch (Exception ex)
            {
                completionException = ex;
            }

            sendResult.Complete(false, completionException);
        }

        public static void End(IAsyncResult result)
        {
            AsyncResult.End<SendAsyncResult>(result);
        }
    }
}