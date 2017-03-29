using System;

namespace Dorado.ESB.Communication
{
    public class DuplicateOperationException : ApplicationException
    {
        public DuplicateOperationException(string operation)
            : base("More than one operation named '" + operation + "'")
        {
        }

        public DuplicateOperationException(string operation, Exception innerException)
            : base("More than one operation named '" + operation + "'", innerException)
        {
        }
    }
}