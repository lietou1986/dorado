using System;

namespace Dorado.DataExpress
{
    public class BaseDbException : Exception
    {
        public BaseDbException(string message)
            : base(message)
        {
        }

        public BaseDbException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}