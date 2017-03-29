using System;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class DispatcherTypeNotSpecifiedException : ApplicationException
    {
        public DispatcherTypeNotSpecifiedException(string subscriberName)
            : base("Dispatcher implementation type not specified for subscriber '" + subscriberName + "'")
        {
        }
    }
}