using System;

namespace Dorado.Ioc
{
    public class ServiceUnvailableException : CoreException
    {
        public ServiceUnvailableException(Type type) : this(string.Format("中未能获取服务[{0}]实例。", type.FullName))
        {
        }

        public ServiceUnvailableException(string message)
            : base(message)
        {
        }
    }
}