using System;

namespace Dorado.ESB.Communication
{
    public class ReadDataAsyncResult : AsyncResult
    {
        private static AsyncCallback onReadPreambleComplete = new AsyncCallback(OnReadPreambleComplete);
        private static AsyncCallback onReadDummyComplete = new AsyncCallback(OnReadDummyComplete);
        private static AsyncCallback onReadDataComplete = new AsyncCallback(OnReadDataComplete);
        private static AsyncCallback onReadPostambleComplete = new AsyncCallback(OnReadPostambleComplete);

        private WseTcpChannel channel;
        private ArraySegment<byte> buffer;
        private int dataLength;
        private byte[] preambleBytes;
        private byte[] data;

        public ReadDataAsyncResult(WseTcpChannel channel, AsyncCallback callback, object state)
            : base(callback, state)
        {
            this.channel = channel;

            bool success = false;
            try
            {
                IAsyncResult readPreambleResult = channel.BeginSocketReceiveBytes(WseTcpChannel.WseAmbleLength, false, onReadPreambleComplete, this);
                if (readPreambleResult.CompletedSynchronously)
                {
                    if (CompleteReadPreamble(readPreambleResult))
                        base.Complete(true);
                }
                success = true;
            }
            finally
            {
                if (!success) Cleanup();
            }
        }

        private bool CompleteReadPreamble(IAsyncResult result)
        {
            preambleBytes = this.channel.EndSocketReceiveBytes(result);
            if (preambleBytes == null)
            {
                buffer = new ArraySegment<byte>();
                return true;
            }

            int idLen = 0, typeLen = 0;
            this.channel.WseGetIdAndTypeLength(preambleBytes, out idLen, out typeLen);
            IAsyncResult readDummyResult = channel.BeginSocketReceiveBytes(idLen + typeLen, onReadDummyComplete, this);
            if (!readDummyResult.CompletedSynchronously)
                return false;

            return CompleteReadDummy(readDummyResult);
        }

        private bool CompleteReadDummy(IAsyncResult result)
        {
            byte[] bytes = channel.EndSocketReceiveBytes(result);
            this.channel.ReturnBuffer(bytes);

            int bytesToRead;
            this.channel.WseGetDataLength(preambleBytes, out dataLength, out bytesToRead);
            IAsyncResult readDataResult = channel.BeginSocketReceiveBytes(bytesToRead, onReadDataComplete, this);
            if (!readDataResult.CompletedSynchronously)
                return false;

            return CompleteReadData(readDataResult);
        }

        private bool CompleteReadData(IAsyncResult result)
        {
            data = channel.EndSocketReceiveBytes(result);
            if ((preambleBytes[0] & 0x02) == 0)
            {
                IAsyncResult readPostambleResult = channel.BeginSocketReceiveBytes(WseTcpChannel.WseAmbleLength, onReadPostambleComplete, this);
                if (!readPostambleResult.CompletedSynchronously)
                    return false;
                return CompleteReadPostamble(readPostambleResult);
            }
            else
            {
                this.buffer = new ArraySegment<byte>(data, 0, dataLength);
                CleanupPreamble();
                return true;
            }
        }

        private bool CompleteReadPostamble(IAsyncResult result)
        {
            byte[] endRecord = channel.EndSocketReceiveBytes(result);
            this.channel.WseVerifyPostamble(endRecord);
            this.channel.ReturnBuffer(endRecord);
            this.buffer = new ArraySegment<byte>(data, 0, dataLength);
            CleanupPreamble();
            return true;
        }

        private void Cleanup()
        {
            if (this.data != null)
            {
                this.channel.ReturnBuffer(data);
                this.data = null;
            }
            CleanupPreamble();
        }

        private void CleanupPreamble()
        {
            if (preambleBytes != null)
            {
                this.channel.ReturnBuffer(preambleBytes);
                this.preambleBytes = null;
            }
        }

        private static void OnReadPreambleComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously) return;

            bool complete = false;
            Exception completionException = null;
            var readDataResult = (ReadDataAsyncResult)result.AsyncState;
            try
            {
                complete = readDataResult.CompleteReadPreamble(result);
            }
            catch (Exception ex)
            {
                complete = true;
                completionException = ex;
                readDataResult.Cleanup();
            }

            if (complete) readDataResult.Complete(false, completionException);
        }

        private static void OnReadDummyComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously) return;

            bool complete = false;
            Exception completionException = null;
            ReadDataAsyncResult readDataResult = (ReadDataAsyncResult)result.AsyncState;
            try
            {
                complete = readDataResult.CompleteReadDummy(result);
            }
            catch (Exception ex)
            {
                complete = true;
                completionException = ex;
                readDataResult.Cleanup();
            }

            if (complete) readDataResult.Complete(false, completionException);
        }

        private static void OnReadDataComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously) return;

            bool complete = false;
            Exception completionException = null;
            ReadDataAsyncResult readDataResult = (ReadDataAsyncResult)result.AsyncState;
            try
            {
                complete = readDataResult.CompleteReadData(result);
            }
            catch (Exception ex)
            {
                complete = true;
                completionException = ex;
                readDataResult.Cleanup();
            }

            if (complete) readDataResult.Complete(false, completionException);
        }

        private static void OnReadPostambleComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously) return;

            bool complete = false;
            Exception completionException = null;
            ReadDataAsyncResult readDataResult = (ReadDataAsyncResult)result.AsyncState;
            try
            {
                complete = readDataResult.CompleteReadPostamble(result);
            }
            catch (Exception ex)
            {
                complete = true;
                completionException = ex;
                readDataResult.Cleanup();
            }

            if (complete) readDataResult.Complete(false, completionException);
        }

        public static ArraySegment<byte> End(IAsyncResult result)
        {
            ReadDataAsyncResult readDataResult = AsyncResult.End<ReadDataAsyncResult>(result);
            return readDataResult.buffer;
        }
    }
}