using System;

namespace Dorado.ESB.Communication
{
    public class WriteDataAsyncResult : AsyncResult
    {
        #region fields

        private WseTcpChannel channel;
        private ArraySegment<byte> data;
        private ArraySegment<byte> preData;

        private AsyncCallback onSendPreDataComplete = new AsyncCallback(OnSendPreDataComplete);
        private AsyncCallback onSendDataComplete = new AsyncCallback(OnSendDataComplete);
        private AsyncCallback onSendPadDataComplete = new AsyncCallback(OnSendPadDataComplete);

        #endregion fields

        #region ctor

        public WriteDataAsyncResult(WseTcpChannel channel, ArraySegment<byte> data, TimeSpan timeout, AsyncCallback callback, object state)
            : base(callback, state)
        {
            this.channel = channel;
            this.data = data;

            bool success = false;
            try
            {
                preData = this.channel.WseGetPreDataBytes(data);
                IAsyncResult sendPreDataResult = this.channel.BeginSocketSend(preData, onSendPreDataComplete, this);
                if (!sendPreDataResult.CompletedSynchronously)
                    return;

                if (CompleteSendPreData(sendPreDataResult))
                {
                    Cleanup();
                    base.Complete(true);
                }

                success = true;
            }
            finally
            {
                if (!success) Cleanup();
            }
        }

        #endregion ctor

        public void Cleanup()
        {
            if (preData != null && preData.Array != null)
            {
                this.channel.ReturnBuffer(preData.Array);
                preData = new ArraySegment<byte>();
            }
        }

        private bool CompleteSendPreData(IAsyncResult result)
        {
            this.channel.EndSocketSend(result);
            IAsyncResult sendDataResult = this.channel.BeginSocketSend(data, onSendDataComplete, this);
            if (!sendDataResult.CompletedSynchronously)
                return false;
            return CompleteSendData(sendDataResult);
        }

        private bool CompleteSendData(IAsyncResult result)
        {
            this.channel.EndSocketSend(result);
            if (data.Count % 4 > 0)
            {
                byte[] padBytes = new byte[4 - data.Count % 4];
                IAsyncResult sendPadDataResult = this.channel.BeginSocketSend(padBytes, onSendPadDataComplete, this);
                if (!sendPadDataResult.CompletedSynchronously)
                    return false;
                CompleteSendPadData(sendPadDataResult);
            }
            return true;
        }

        private void CompleteSendPadData(IAsyncResult result)
        {
            this.channel.EndSocketSend(result);
        }

        public static void End(IAsyncResult result)
        {
            AsyncResult.End<WriteDataAsyncResult>(result);
        }

        private static void OnSendPreDataComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously) return;

            bool complete = false;
            Exception completionException = null;
            WriteDataAsyncResult writeResult = (WriteDataAsyncResult)result.AsyncState;
            try
            {
                complete = writeResult.CompleteSendPreData(result);
            }
            catch (Exception ex)
            {
                complete = true;
                completionException = ex;
            }
            finally
            {
                writeResult.Cleanup();
            }

            if (complete) writeResult.Complete(false, completionException);
        }

        private static void OnSendDataComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously) return;

            bool complete = false;
            Exception completionException = null;
            WriteDataAsyncResult writeResult = (WriteDataAsyncResult)result.AsyncState;
            try
            {
                complete = writeResult.CompleteSendData(result);
            }
            catch (Exception ex)
            {
                complete = true;
                completionException = ex;
            }

            if (complete) writeResult.Complete(false, completionException);
        }

        private static void OnSendPadDataComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously) return;

            Exception completionException = null;
            WriteDataAsyncResult writeResult = (WriteDataAsyncResult)result.AsyncState;
            try
            {
                writeResult.CompleteSendPadData(result);
            }
            catch (Exception ex)
            {
                completionException = ex;
            }

            writeResult.Complete(false, completionException);
        }
    }
}