using System;

namespace Dorado.DataExpress
{
    public class OutOfRangeException : BaseDbException
    {
        public OutOfRangeException(string message)
            : base(message)
        {
        }

        public OutOfRangeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}