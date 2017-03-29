using System;

namespace Dorado.ESB.Communication
{
    public class TypedCompletedAsyncResult<T> : TypedAsyncResult<T>
    {
        public TypedCompletedAsyncResult(T data, AsyncCallback callback, object state)
            : base(callback, state)
        {
            Complete(data, true);
        }

        public new static T End(IAsyncResult result)
        {
            TypedCompletedAsyncResult<T> completedResult = result as TypedCompletedAsyncResult<T>;
            if (completedResult == null)
                throw new ArgumentException("Invalid async result", "result");
            return TypedAsyncResult<T>.End(completedResult);
        }
    }
}