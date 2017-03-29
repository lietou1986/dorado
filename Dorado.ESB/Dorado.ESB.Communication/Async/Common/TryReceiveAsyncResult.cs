using System;
using System.ServiceModel.Channels;

namespace Dorado.ESB.Communication
{
    public class TryReceiveAsyncResult : AsyncResult
    {
        #region fields

        private static AsyncCallback onReceiveComplete = new AsyncCallback(OnReceiveComplete);

        private CustomDuplexSessionChannel channel;
        private bool receiveSuccess;
        private Message message;

        #endregion fields

        #region ctor

        public TryReceiveAsyncResult(CustomDuplexSessionChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
            : base(callback, state)
        {
            this.channel = channel;

            bool complete = true;

            try
            {
                IAsyncResult beginReceiveResult = this.channel.BeginReceive(timeout, onReceiveComplete, this);
                if (beginReceiveResult.CompletedSynchronously)
                    CompleteReceive(beginReceiveResult);
                else
                    complete = false;
            }
            catch (TimeoutException) { }

            if (complete) base.Complete(true);
        }

        #endregion ctor

        private void CompleteReceive(IAsyncResult result)
        {
            this.message = this.channel.EndReceive(result);
            this.receiveSuccess = true;
        }

        private static void OnReceiveComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously) return;

            Exception completionException = null;
            TryReceiveAsyncResult tryReceiveResult = (TryReceiveAsyncResult)result.AsyncState;
            try
            {
                tryReceiveResult.CompleteReceive(result);
            }
            catch (TimeoutException) { }
            catch (Exception ex)
            {
                completionException = ex;
            }

            tryReceiveResult.Complete(false, completionException);
        }

        public static bool End(IAsyncResult result, out Message message)
        {
            TryReceiveAsyncResult tryReceiveResult = AsyncResult.End<TryReceiveAsyncResult>(result);
            message = tryReceiveResult.message;
            return tryReceiveResult.receiveSuccess;
        }
    }
}