using System;

namespace Dorado.DataExpress.Utility
{
    [Serializable]
    public class ObtainTimeoutException : ApplicationException
    {
        public ObtainTimeoutException(string message)
            : base(message)
        {
        }
    }
}