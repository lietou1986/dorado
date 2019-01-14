using System;

namespace Dorado.Ioc
{
    public interface IServiceFactory
    {
        object CreateInstance(Type type);
        T CreateInstance<T>();
    }
}
