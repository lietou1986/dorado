using System;

namespace Dorado.ESB.Communication
{
    // A strongly typed AsyncResult
    public abstract class TypedAsyncResult<T> : AsyncResult
    {
        private T data;

        public T Data
        {
            get { return data; }
        }

        protected TypedAsyncResult(AsyncCallback callback, object state)
            : base(callback, state)
        {
        }

        protected void Complete(T data, bool completedSynchronously)
        {
            this.data = data;
            Complete(completedSynchronously);
        }

        public static T End(IAsyncResult result)
        {
            TypedAsyncResult<T> typedResult = AsyncResult.End<TypedAsyncResult<T>>(result);
            return typedResult.Data;
        }
    }
}