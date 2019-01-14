using System;

namespace Dorado.Ioc
{
    /// <summary>
    /// 服务不能找到
    /// </summary>
    public class ServiceNotFountException : CoreException
    {
        public ServiceNotFountException(string message)
            : base(message)
        {
        }
        public ServiceNotFountException(Type type)
            : base(string.Format("接口服务调用发生异常[{0}]", type.FullName))
        {
        }

        public ServiceNotFountException(Type type, Exception error)
           : base(string.Format("接口服务调用发生异常[{0}]", type.FullName), error)
        {
        }
    }
}
