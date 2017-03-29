using System;

namespace Dorado.DataExpress
{
    public class NoTableNameException : BaseDbException
    {
        public NoTableNameException(string message)
            : base(message)
        {
        }

        public NoTableNameException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}