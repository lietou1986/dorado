using System;
using System.Runtime.Serialization;

namespace Dorado.Data.Exceptions
{
    [Serializable]
    public class DatabaseDownException : DataException
    {
        internal DatabaseDownException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        internal DatabaseDownException(string message)
            : base(message)
        {
        }

        protected DatabaseDownException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}