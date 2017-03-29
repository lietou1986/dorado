using System;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class ActivityEngineException : ApplicationException
    {
        public ActivityEngineException(string message)
            : base(message)
        {
        }

        public ActivityEngineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}