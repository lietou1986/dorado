using System;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class UnknownActivityFilterException : ApplicationException
    {
        public UnknownActivityFilterException(string filter)
            : base("Unknown Activity Filter '" + filter + "'")
        {
        }
    }
}