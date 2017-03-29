using System;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class DispatcherTypeInvalidException : ApplicationException
    {
        public DispatcherTypeInvalidException(string typeName)
            : base("Invalid dispatcher type '" + typeName + "', ctor with one argument of type ActivitySubscriberConfig is required")
        {
        }
    }
}