using System;

namespace Dorado.DataExpress
{
    public class NoDateSetNameException : BaseDbException
    {
        public NoDateSetNameException(string message)
            : base(message)
        {
        }

        public NoDateSetNameException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}