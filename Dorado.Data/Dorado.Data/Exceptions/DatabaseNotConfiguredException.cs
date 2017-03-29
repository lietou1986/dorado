using System;
using System.Runtime.Serialization;

namespace Dorado.Data.Exceptions
{
    [Serializable]
    public class DatabaseNotConfiguredException : DataException
    {
        internal DatabaseNotConfiguredException(string instanceName)
            : base("Unable to retrieve database " + instanceName + " from connection string configuration file.")
        {
        }

        protected DatabaseNotConfiguredException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}