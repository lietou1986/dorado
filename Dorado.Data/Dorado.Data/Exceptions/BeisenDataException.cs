using System;

namespace Dorado.Data.Exceptions
{
    public class DoradoDataException : Exception
    {
        internal DoradoDataException(string instanceName, string connectionString, string errorMessage, string operation, Exception innerException)
            : base("Error against: " + instanceName + ":" + errorMessage, innerException)
        {
        }
    }
}