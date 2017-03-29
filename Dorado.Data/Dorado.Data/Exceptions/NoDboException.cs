using System;
using System.Runtime.Serialization;

namespace Dorado.Data.Exceptions
{
    [Serializable]
    public class NoDboException : DataException
    {
        internal NoDboException(string instanceName, string procedureName)
            : base("Did not specify \"dbo\" before the procedure call " + procedureName + " against database " + instanceName)
        {
        }

        protected NoDboException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}