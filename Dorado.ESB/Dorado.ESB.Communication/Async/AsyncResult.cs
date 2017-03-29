using System;
using System.Diagnostics;
using System.Threading;

namespace Dorado.ESB.Communication
{
    public abstract class AsyncResult : IAsyncResult
    {
        #region fields

        private AsyncCallback callback;
        private bool endCalled;
        private Exception exception;
        private ManualResetEvent manualResetEvent;
        private object padLock;

        #endregion fields

        #region ctor

        protected AsyncResult(AsyncCallback callback, object state)
        {
            this.callback = callback;
            this.state = state;
            this.padLock = new object();
        }

        #endregion ctor

        #region IAsyncResult Members

        private object state;

        public object AsyncState
        {
            get { return state; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (manualResetEvent == null)
                {
                    lock (padLock)
                    {
                        if (manualResetEvent == null)
                            manualResetEvent = new ManualResetEvent(isCompleted);
                    }
                }
                return manualResetEvent;
            }
        }

        private bool completedSynchronously;

        public bool CompletedSynchronously
        {
            get { return completedSynchronously; }
        }

        private bool isCompleted;

        public bool IsCompleted
        {
            get { return isCompleted; }
        }

        #endregion IAsyncResult Members

        // Call this version of Complete when your async operation is complete.
        // This will update the state of the operation and notify the callback.
        protected void Complete(bool completedSynchronously)
        {
            if (isCompleted)
                throw new InvalidOperationException("Cannot call Complete twice");

            this.completedSynchronously = completedSynchronously;

            if (completedSynchronously)
            {
                // If completed synchronously, then there is no chance that the manualResetEvent was created,
                // so we do NOT need to worry about a race condition
                Debug.Assert(manualResetEvent == null, "No manualResetEvent should be created for a synchronous AsyncResult");
                this.isCompleted = true;
            }
            else
            {
                lock (padLock)
                {
                    this.isCompleted = true;
                    if (manualResetEvent != null) manualResetEvent.Set();
                }
            }

            if (callback != null) callback(this);
        }

        // Call this version of Complete if you raise an exception during processing
        // In addition to notify the callback, it will capture the exception and store it to be thrown during AsyncResult.End
        protected void Complete(bool completedSynchronously, Exception exception)
        {
            this.exception = exception;
            Complete(completedSynchronously);
        }

        // End should be called when the End function for the async operation is complete.
        // It ensures the async operation is complete, and does some common validation
        protected static TAsyncResult End<TAsyncResult>(IAsyncResult result)
            where TAsyncResult : AsyncResult
        {
            if (result == null)
                throw new ArgumentNullException("result");

            TAsyncResult asyncResult = result as TAsyncResult;
            if (asyncResult == null)
                throw new ArgumentException("Invalid asyn result", "result");

            if (asyncResult.endCalled)
                throw new InvalidOperationException("Async object already ended.");

            asyncResult.endCalled = true;

            if (!asyncResult.isCompleted)
                asyncResult.AsyncWaitHandle.WaitOne();

            if (asyncResult.manualResetEvent != null)
                asyncResult.manualResetEvent.Close();

            if (asyncResult.exception != null)
                throw asyncResult.exception;

            return asyncResult;
        }
    }
}